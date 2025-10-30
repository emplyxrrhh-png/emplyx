Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTShiftEngines

    Public Class roCausesManager
        Public Const mPriority = 60
        Public Const mPreviousProcessPriority = 50

        Private mTask As roLiveTask
        Private mIDEmployee As Integer = 0
        Public mCurrentDate As Date
        Private mCostCenterInstallled As Boolean = False
        Private bolApplyUnexpectedOutRule As Boolean = False
        Private dblUnexpectedOutRule_IDCause As Double
        Private bolApplyPriorityRule As Boolean = False
        Private dblPriorityRuleIncidence As Double = 0
        Private strPriorityRuleCause As String = String.Empty
        Private dblPriorityRuleCauseID As Integer = 0
        Private bolProductiveAbsences As Boolean = False
        Private dblProductiveAbsenceCause As Integer = 0
        Private mLastPunchCenterEnabled As Boolean = False

        Private mActivatedMinBreakTime As Boolean = False

        ' Lista de justificaciones de trabajo externo
        Private zExternalWorkCauses As New roCollection

        ' Lista de justificaciones con valores maximos
        Private zMaxTimeToForecast As New roCollection
        ' Indica si el dia esta planificado como vacaciones
        Private bolIsHolidays As Boolean
        ' Indica si el dia esta planificado con vacaciones y existe PA que es prioritaria
        Private bolProgrammedAbsenceOnHolidays As Boolean = False

        ' IDs de justificaciones predeterminadas
        Public Const CAUSE_INCIDENCE_DEFAULT = 0
        Public Const CAUSE_WORKING_DEFAULT = 1
        Public Const CAUSE_OVERWORKING = 2
        Public Const CAUSE_BREAK = 3

        Private oState As roEngineState = Nothing

        Private mInitialRegisterStatus As Integer = 0
        Private mSQLExecute As New roCollection

        Private oLicense As New roServerLicense

        Private da As DbDataAdapter
        Private mProgrammedAbsence As roProgrammedAbsenceManager

        Private mGUIDChanged As Boolean = False

        Public ReadOnly Property State As roEngineState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New(IDTask As Integer)
            oState = New roEngineState()
            mTask = New roLiveTask(IDTask, New roLiveTaskState(oState.IDPassport))
        End Sub

        Public Sub New(IDTask As Integer, ByVal _State As roEngineState)
            oState = _State
            mTask = New roLiveTask(IDTask, New roLiveTaskState(oState.IDPassport))
        End Sub

#End Region

#Region "Métodos"

        Private Sub LoadConfig()
            Try
                mCostCenterInstallled = oLicense.FeatureIsInstalled("Feature\CostControl")

                bolApplyUnexpectedOutRule = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.UnexpectedOutRule"))
                If bolApplyUnexpectedOutRule Then
                    ' Obtenemos la justificacion a utilizar para justificar las incidencias de interrupcion
                    dblUnexpectedOutRule_IDCause = Any2Double(ExecuteScalar("@SELECT# ID FROM Causes WHERE Description like '%#DESRET#%'"))
                    bolApplyUnexpectedOutRule = (dblUnexpectedOutRule_IDCause > 0)
                End If

                bolApplyPriorityRule = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ApplyPriorityRule"))
                If bolApplyPriorityRule Then
                    dblPriorityRuleIncidence = roTypes.Any2Double(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.PriorityRuleIncidence"))
                    strPriorityRuleCause = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.PriorityRuleCause"))
                    If dblPriorityRuleIncidence > 0 AndAlso strPriorityRuleCause.Length > 0 Then
                        dblPriorityRuleCauseID = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '" & strPriorityRuleCause & "'"))
                        bolApplyPriorityRule = (dblPriorityRuleCauseID > 0)
                    End If
                End If

                bolProductiveAbsences = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ApplyProductiveAbsences"))
                If bolProductiveAbsences Then
                    Dim strCause As String = String.Empty
                    strCause = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ProductiveAbsencesCause"))
                    If strCause.Length > 0 Then
                        dblProductiveAbsenceCause = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '" & strCause & "'"))
                        bolProductiveAbsences = (dblProductiveAbsenceCause > 0)
                    End If
                End If

                mLastPunchCenterEnabled = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "LastPunchCenterEnabled"))

                ' Carga de colecciones varias
                Execute_GetMaxTimeToForecast()
                Execute_GetExternalWorkCauses()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roCausesManager::Init: Error recovering config data.", ex)
            End Try
        End Sub

        Public Function ExecuteBatch(ByRef bolGUIDChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim FirstDate As New roTime
            Dim CurrentDate As New roTime
            Dim iLastProcessStatus As Integer = VTShiftEngines.roIncidencesManager.mPriority

            Try

                ' Recuperamos ID de empleado de la tarea
                mIDEmployee = If(Not mTask.Parameters Is Nothing AndAlso mTask.Parameters.Exists("IDEmployee"), roTypes.Any2Integer(mTask.Parameters("IDEmployee")), 0)
                If mIDEmployee = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCausesManager::ExecuteBatch: Unable to set IDEmployee from task.")
                    Me.oState.Result = EngineResultEnum.EmployeeRequired
                    Return False
                End If

                Me.oState.Result = EngineResultEnum.NoError

                ' Únicamente considero los registros con status comprendido entre mi prioridad y la prioridad del proceso precedente
                Dim strSQL As String = "@SELECT# IDEmployee,Date, Status, isnull(IsHolidays,0) as Holidays FROM DailySchedule with (nolock) " &
                                            " WHERE Status >= " & iLastProcessStatus.ToString & " And Status < " & mPriority.ToString &
                                                " AND Date<=" & Any2Time(Now.Date).SQLSmallDateTime & " AND IDEmployee = " & mIDEmployee.ToString &
                                                " AND GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "'" &
                                                    " ORDER BY IDEmployee,Date"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then

                    ' Inicializamos
                    LoadConfig()

                    ' Inicializamos el gestor de ausencias prolongadas
                    mProgrammedAbsence = New roProgrammedAbsenceManager
                    mProgrammedAbsence.Init(mPriority)

                    Dim totalActions As Integer = dTbl.Rows.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Double = 100 / totalActions

                    For Each oRowEmp As DataRow In dTbl.Rows
                        mInitialRegisterStatus = Any2Integer(oRowEmp("Status"))
                        bolIsHolidays = Any2Boolean(oRowEmp("Holidays"))
                        mCurrentDate = Any2DateTime(oRowEmp("Date")).Date
                        bolProgrammedAbsenceOnHolidays = False

                        ' Realiza el proceso
                        bolRet = ExecuteSingleDay(Any2Long(oRowEmp("IDEmployee")), oRowEmp("Date"))

                        ' Actualizo progeso
                        mTask.Progress = mTask.Progress + stepProgress
                        mTask.Save()

                        If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError OrElse mGUIDChanged Then
                            ' Si hubo errro, dejo de procesar ...
                            bolGUIDChanged = mGUIDChanged
                            bolRet = False
                            Exit For
                        End If
                    Next
                End If

                ' Cerramos el gestor de ausencias prolongadas
                If Not mProgrammedAbsence Is Nothing Then mProgrammedAbsence.Close()
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: ExecuteBatch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::ExecuteBatch")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay(ByVal EmployeeID As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Procesa un dia concreto
            '

            Dim zShift As roShiftEngine
            Dim zIncidences As New roTimeBlockList
            Dim zForeCast As roTimeBlockList
            Dim zProgrammedHolidays As roTimeBlockList
            Dim zProgrammedOvertimes As roTimeBlockList
            Dim Incidence As New roTimeBlockItem
            Dim sSQLUpdate As String = ""
            Dim sSQL As String = ""
            Dim CurrentRegisterStatus As Integer = 0
            Dim zDate As New roTime

            Try

                oState.Result = EngineResultEnum.NoError

                ' 000. Si la fecha a procesar es futura, no hace nada
                If DateDiff("d", TaskDate, Now) < 0 Then
                    Return True
                End If

                Debug.Print(Now & "     ----> CAUSES: Processing employee '" & EmployeeID & "', date " & TaskDate)

                ' Obtiene empleado y fecha de la tarea
                zDate = Any2Time(TaskDate)

                ' Obtiene horario utilizados
                zShift = roBaseEngineManager.Execute_GetShift(EmployeeID, zDate, bolIsHolidays, bolProgrammedAbsenceOnHolidays, oState)
                '                If zShift Is Nothing Then Return True

                ' Comprueba las ausencias prolongadas
                If Not mProgrammedAbsence.Analyze(EmployeeID, zDate) Then
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.CouldNotAnalyzeProgrammedAbsence
                        Return False
                    End If
                End If

                ' Obtiene previsiones de ausencia por horas
                zForeCast = Execute_GetForecast(EmployeeID, zDate, zShift)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorGettingProgrammedCauses
                    Return False
                End If

                ' Obtiene previsiones de horas de exceso
                zProgrammedOvertimes = Execute_GetProgrammedOvertimes(EmployeeID, zDate, zShift)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorGettingProgrammedOvertimes
                    Return False
                End If

                ' Obtiene incidencias
                zIncidences = Execute_GetIncidences(EmployeeID, zDate)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorGettingIncidences
                    Return False
                End If

                ' Obtiene previsiones de vacaciones por horas
                zProgrammedHolidays = Execute_GetProgrammedHolidays(EmployeeID, zDate, zShift, zIncidences)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorGettingProgrammedHolidays
                    Return False
                End If

                ' Elimina justificaciones anteriores <NO JUSTIFICADAS> o automáticas
                If DailyScheduleGUIDChanged() Then Return True
                Execute_RemovePreviousUnjustifiedCauses(EmployeeID, zDate)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorRemovingPreviousUnjustifiedCauses
                    Return False
                End If

                ' Obtiene justificaciones actuales
                Dim strSQL As String = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & EmployeeID.ToString &
                                            " AND Date=" & zDate.SQLSmallDateTime

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                ' Elimina de la lista las incidencias que ya están codificadas
                ' (el proceso Incidences borra las justificaciones de una incidencia cuando esta cambia, asi que si
                '  encontramos justificaciones de incidencias, significa que esa incidencia ya ha sido tratada)
                If DailyScheduleGUIDChanged() Then Return True
                Execute_RemoveUnchangedIncidences(zIncidences, zCauses)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorRemovingUnchangedIncidences
                    Return False
                End If

                ' Si tiene ausencia prolongada y ha venido, cierra la ausencia
                If mProgrammedAbsence.HasActiveProgrammedAbsence Then
                    If DailyScheduleGUIDChanged() Then Return True
                    If Execute_IsEmployeePresent(zIncidences) Then mProgrammedAbsence.CloseProgrammedAbsenceIfNeeded()
                End If

                ' Mira si hay que aplicar las reglas o la ausencia prolongada
                If ApplyRules(mProgrammedAbsence, EmployeeID, zDate, zShift) Then
                    ' Aplicamos las justificaciones de trabajo externo sobre las incidencias producidas
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyProductiveAbsencesCauses(zCauses, zIncidences, EmployeeID, zDate)

                    ' Si hay justificaciones por terminal, justifica según se indique en las incidencias
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyPunchedCauses(zCauses, zIncidences, EmployeeID, zDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyPunchedCauses
                        Return False
                    End If

                    ' En el caso que tengamos comportamiento de regla prioritaria
                    ' debemos revisar si existe alguna que haya que marcar como prioritaria
                    ' y procesarla antes que cualquier otra cosa
                    If bolApplyPriorityRule Then
                        If DailyScheduleGUIDChanged() Then Return True
                        Execute_ApplyPriorityCause(zCauses, zShift, zIncidences, EmployeeID, zDate)
                    End If

                    ' Miramos si debemos aplicar la lógica de Live, o la heredada para Win32 (actualizaciones de Win32 a Live)
                    ' Si no hay previsiones no hace nada
                    If zForeCast.Count >= 1 Then
                        If DailyScheduleGUIDChanged() Then Return True
                        If Any2Boolean(zForeCast.Item(1).TimeZone) Then
                            ' Lógica heredada
                            Execute_ApplyForecastCauses(zCauses, zIncidences, EmployeeID, zDate, zForeCast)
                        Else
                            ' TESTOK: Lógica Live
                            Execute_ApplyForecastCausesLive(zCauses, zIncidences, EmployeeID, zDate, zForeCast)
                        End If
                    End If

                    ' Aplicamos previsiones de vacaciones por horas
                    If zProgrammedHolidays.Count > 0 Then
                        If DailyScheduleGUIDChanged() Then Return True
                        Execute_ApplyProgrammedHolidays(zCauses, zIncidences, EmployeeID, zDate, zProgrammedHolidays)
                    End If
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingProgrammedHolidays
                        Return False
                    End If

                    ' Aplicamos previsiones de horas de exceso
                    If zProgrammedOvertimes.Count > 0 Then
                        If DailyScheduleGUIDChanged() Then Return True
                        Execute_ApplyProgrammedOvertimes(zCauses, zIncidences, EmployeeID, zDate, zProgrammedOvertimes)
                    End If

                    ' Aplica reglas sobre incidencias de interrupcion avanzadas, en caso necesario
                    If bolApplyUnexpectedOutRule Then
                        If DailyScheduleGUIDChanged() Then Return True
                        Execute_ApplyUnexpectedOutRule(zCauses, zIncidences, EmployeeID, zDate)
                        If oState.Result <> EngineResultEnum.NoError Then
                            oState.Result = EngineResultEnum.ErrorApplyingProgrammedHolidays
                            Return False
                        End If
                    End If

                    ' Aplica reglas si hay incidencias
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyAllShiftRules(zCauses, zShift, zIncidences, EmployeeID, zDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingAllShiftRules
                        Return False
                    End If
                Else
                    ' Si no ha venido, justifica todo el tiempo ausente como inidique la ausencia prog.
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyProgrammedAbsence(zCauses, zIncidences, EmployeeID, zDate.Value, mProgrammedAbsence.ActiveCause)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingProgrammedAbsences
                        Return False
                    End If
                End If

                ' Aplica justificación de horario de vacaciones en caso necesario
                If Not bolProgrammedAbsenceOnHolidays Then
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyHolidaysRules(zCauses, zShift, zIncidences, EmployeeID, zDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingHolidaysRules
                        Return False
                    End If
                End If

                ' Ahora codifica como tiempo sin codificar o tiempo trabajado
                ' el resto de incidencias que aun nos quedan
                If DailyScheduleGUIDChanged() Then Return True
                Execute_ApplyDefaultCauses(zIncidences, zCauses, EmployeeID, zDate.Value)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorApplyingDefaultCauses
                    Return False
                End If

                ' Asignamos los centros de coste por defecto
                If mCostCenterInstallled Then
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_ApplyDefaultCenters(EmployeeID, zDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingDefaultCenters
                        Return False
                    End If
                End If

                ' Ejecutamos sentencias de actualización de justificaciones diarias ya existentes
                If DailyScheduleGUIDChanged() Then Return True
                ExecutePendingSQL()
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorExecutingPendingSQL
                    Return False
                End If

                ' Indica que hemos procesado esa fecha
                If DailyScheduleGUIDChanged() Then Return True
                sSQLUpdate = "@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=" & mPriority.ToString &
                     " WHERE IDEmployee=" & EmployeeID & " AND Date=" & zDate.SQLSmallDateTime
                If Not ExecuteSql(sSQLUpdate) Then
                    oState.Result = EngineResultEnum.ErrorUpdatingStatus
                End If

                ' Gestión de alertas
                ManageAlertsWhenPA(EmployeeID, zDate)

                mSQLExecute = New roCollection
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::ExecuteSingleDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::ExecuteSingleDay")
            Finally

            End Try

            Return (oState.Result = EngineResultEnum.NoError)

        End Function

        Private Function ApplyRules(ByVal mProgrammedAbsence As roProgrammedAbsenceManager, ByVal zEmployee As Long, ByVal TaskDate As roTime, ByVal zShift As roShiftEngine) As Boolean
            '
            ' Verificamos si se tienen que aplicar las reglas del horario o la ausencia prolongada
            '
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si no tiene ausencia prolongada aplica reglas del horario
                If Not mProgrammedAbsence.HasActiveProgrammedAbsence Then
                    bolret = True
                Else

                    If zShift Is Nothing Then
                        ' Si no tiene horario asignado siempre se aplica la ausencia prolongada
                        Return False
                    End If

                    ' Si tiene ausencia prolongada comprobamos si el dia es festivo y si la justificacion de la ausencia esta
                    ' marcada para que se aplica o no en días festivos
                    If zShift.ExpectedWorkingHours = 0 Then
                        If Any2Boolean(ExecuteScalar("@SELECT# ApplyAbsenceOnHolidays FROM Causes where id=" & mProgrammedAbsence.ActiveCause.ToString)) Then
                            ' Se aplica la justificacion de la ausencia prolongada en dia festivo
                            bolret = False
                        Else
                            ' En caso que no se aplique la ausencia prolongada se aplican las reglas normales
                            ' del horario
                            bolret = True
                        End If
                    Else
                        ' Si no es festivo siempre se aplica la ausencia prolongada
                        bolret = False
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::ApplyRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::ApplyRules")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_GetIncidences(ByVal zEmployee As Long, ByVal zDate As roTime) As roTimeBlockList
            '
            ' Obtiene incidencias del dia indicado
            '

            Dim myList As New roTimeBlockList
            Dim myItem As roTimeBlockItem

            Try

                oState.Result = EngineResultEnum.NoError

                Dim strSQL As String = "@SELECT#  IDType,IDZone,BeginTime,EndTime,Value,ID FROM DailyIncidences WHERE IDEmployee=" & zEmployee &
                " AND Date=" & zDate.SQLSmallDateTime & " ORDER BY EndTime"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    ' Añade cada incidencia a la lista
                    For Each oRow As DataRow In dTbl.Rows
                        myItem = New roTimeBlockItem
                        myItem.BlockType = oRow("IDType")
                        myItem.TimeZone = oRow("IDZone")
                        myItem.Period.Begin = Any2Time(oRow("BeginTime"))
                        myItem.Period.Finish = Any2Time(oRow("EndTime"))
                        If Any2Double(oRow("Value")) >= 0 Then
                            myItem.TimeValue = Any2Time(oRow("Value"))
                            myItem.Tag = oRow("ID")
                            myList.Add(myItem)
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetIncidences")
            Finally

            End Try

            Return myList

        End Function

        Private Function Execute_ApplyProgrammedAbsence(ByRef Causes As DataTable, ByRef Incidences As roTimeBlockList, ByVal IDEmployee As Long, ByVal DateTime As Date, ByVal CauseID As Integer) As Boolean
            '
            ' Justifica todo el tiempo ausente como se indique en la ausencia prolongada
            '
            Dim bolRet As Boolean = False

            Dim dblExpectedWorkingHours As Double
            Dim TimeToJustify As Double
            Dim bApplyIncidence As Boolean = False
            Dim xTimeValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                Dim bolEnabledJustifyUntilExpectedHours As Boolean = True
                Dim sAux As String = String.Empty
                sAux = Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "VTLive.ProgrammedAbsences.JustifyUntilExpectedHours"))
                bolEnabledJustifyUntilExpectedHours = (sAux.Trim = "1" OrElse sAux.Trim = "")

                If bolEnabledJustifyUntilExpectedHours Then
                    ' Comportamiento Nuevo: Justificamos solo hasta el maximo de horas del horario

                    ' Justificamos solo hasta el maximo de horas del horario
                    If Not bolProgrammedAbsenceOnHolidays Then
                        dblExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours,ISNULL(Shifts.ExpectedWorkingHours, 0)) FROM DailySchedule, Shifts WHERE IDEmployee = " & IDEmployee & " AND Shifts.ID = DailySchedule.IDShift1 " & " AND Date = " & Any2Time(DateTime).SQLSmallDateTime))
                    Else
                        dblExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours,ISNULL(Shifts.ExpectedWorkingHours, 0)) FROM DailySchedule, Shifts WHERE IDEmployee = " & IDEmployee & " AND Shifts.ID = DailySchedule.IDShiftBase " & " AND Date = " & Any2Time(DateTime).SQLSmallDateTime))
                    End If

                    TimeToJustify = dblExpectedWorkingHours

                    For index = 1 To Incidences.Count

                        bApplyIncidence = True

                        If bApplyIncidence Then
                            If TimeToJustify <= 0 Then
                                xTimeValue = 0
                            Else
                                ' Justificamos el tiempo hasta el maximo
                                If TimeToJustify >= Incidences.Item(index).TimeValue.NumericValue Then
                                    ' Si el tiempo maximo es mayor o igual a la incidnecia
                                    ' justificamos todo el tiempo de la incidencia
                                    xTimeValue = Incidences.Item(index).TimeValue.NumericValue

                                    ' Restamos del tiempo maximo el valor de la incidencia
                                    TimeToJustify = TimeToJustify - xTimeValue
                                Else
                                    ' Justificamos parte de la incidencia con el valor del tiempo maximo
                                    xTimeValue = TimeToJustify

                                    ' Dejamos el tiempo maximo a 0
                                    TimeToJustify = 0
                                End If

                                ' Justifica la incidencia
                                Execute_AddCause(Causes, IDEmployee, DateTime, CauseID, xTimeValue, Incidences.Item(index).Tag)
                            End If

                            ' Restamos el tiempo a la incidencia
                            Incidences.Item(index).TimeValue = Any2Time(0)
                        End If

                    Next
                Else
                    For index = 1 To Incidences.Count
                        If Not Incidences.Item(index).IsWorkingTime Then
                            ' Esta incidencia es de tiempo ausente, justifica como indique la ausencia prolongada
                            Execute_AddCause(Causes, IDEmployee, DateTime, CauseID, Incidences.Item(index).TimeValue.NumericValue(True), Incidences.Item(index).Tag)
                            Incidences.Item(index).TimeValue = Any2Time("00:00")
                        End If

                        If Incidences.Item(index).IsWorkingTime Then
                            ' Esta incidencia es de tiempo presente, justifica como indique la ausencia prolongada ya que ha
                            ' trabajado en un periodo de ausencia prolongada por lo que no se puede contabilizar como tiempo de trabajo
                            Execute_AddCause(Causes, IDEmployee, DateTime, CauseID, Incidences.Item(index).TimeValue.NumericValue(True), Incidences.Item(index).Tag)
                            Incidences.Item(index).TimeValue = Any2Time("00:00")
                        End If
                    Next
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedAbsence")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub Execute_ApplyPriorityCause(ByRef zCauses As DataTable, ByRef zShift As roShiftEngine, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef TaskDate As roTime)
            '
            ' Aplica la regla prioritaria en caso necesario
            '
            Dim Incidence As roTimeBlockItem

            Try

                oState.Result = EngineResultEnum.NoError

                If Not zShift Is Nothing Then
                    For Each rule As roShiftEngineRule In zShift.SimpleRules
                        If Any2Double(rule.IDIncidence) = dblPriorityRuleIncidence AndAlso Any2Double(rule.IDCause) = dblPriorityRuleCauseID Then
                            ' Evaluamos la regla prioritaria y la marcamos para no volver a procesarla después
                            rule.Aux = "-1"
                            If rule.Type = ShiftRuleType.Simple Then
                                ' Para cada incidencias
                                For Index As Integer = 1 To zIncidences.Count
                                    Incidence = zIncidences.Item(Index)
                                    ' Evalua condición para cada incidencia
                                    If Execute_EvaluateShiftRuleCondition(rule, zIncidences.Item(Index), zEmployee, TaskDate.Value) Then
                                        ' Si la condición es cierta, aplica la regla
                                        Execute_ApplyShiftRule(zCauses, rule, Incidence, zEmployee, TaskDate.Value)
                                    End If

                                    If oState.Result <> EngineResultEnum.NoError Then Exit For

                                Next
                            End If
                        End If
                    Next
                End If

                Exit Sub
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyPriorityCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyPriorityCause")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyForecastCausesLive(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal zForeCast As roTimeBlockList)
            '
            '  Si hay una previsión de incidencias, justifica todas las incidencias entre el periodo indicada hasta el tiempo maximo indicado
            '
            Dim myItem As roTimeBlockItem
            Dim TimeToForecast As Double
            Dim MinTimeToForecast As Double
            Dim i As Integer
            Dim w As Integer
            Dim xTimeValue As Double
            Dim z As Integer
            Dim ValidIncidence As Boolean
            Dim TimeForecast As Double
            Dim strDescription As String = ""
            Dim dblMinDefaultTime As Double
            Dim intMinTimeIDCause As String
            Dim Layer As New roTimeBlockItem

            Try

                ' Si no hay previsiones no hace nada
                If zForeCast.Count = 0 Then Exit Sub

                For w = 1 To zForeCast.Count

                    ' Tiempo maximo a justificar
                    TimeToForecast = zForeCast.Item(w).TimeValue.NumericValue

                    ' IDN mCustomization = TACI eliminada
                    MinTimeToForecast = zForeCast.Item(w).InCause

                    TimeForecast = 0

                    ' Verificamos si la justificación
                    ' tiene un mínimo obligado
                    Try
                        strDescription = Any2String(ExecuteScalar("@SELECT# Description FROM Causes WHERE ID=" & Any2Double(zForeCast.Item(w).Tag)))
                        If InStr(strDescription, "#MINTIME_PC=") > 0 Then
                            dblMinDefaultTime = 0
                            dblMinDefaultTime = Any2Time(Mid(strDescription, InStr(strDescription, "#MINTIME_PC=") + 12, 5)).NumericValue
                            If dblMinDefaultTime > MinTimeToForecast Then
                                MinTimeToForecast = dblMinDefaultTime
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    For z = 1 To 2
                        For i = 1 To zIncidences.Count

                            ValidIncidence = False

                            If TimeToForecast <= 0 Then Exit For

                            With zIncidences.Item(i)
                                ' 01. Pirmero justificamos todas las incidencias de ausencia menos (Ausencia en horas flexibles y diferencia negativa)
                                ' 02. Finalmente restamos el tiempo maximo que queda de las incidencias Ausencia en horas flexibles y diferencia negativa
                                If z = 1 Then
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence) AndAlso .BlockType <> roTimeBlockItem.roBlockType.roBTFlexibleUnderworking AndAlso .BlockType <> roTimeBlockItem.roBlockType.roBTDailyUnderworking AndAlso .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                Else
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence) AndAlso (.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleUnderworking Or .BlockType = roTimeBlockItem.roBlockType.roBTDailyUnderworking) And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                End If

                                If ValidIncidence Then
                                    ' Si alguna parte de la incidencia se solapa con el periodo indicado
                                    myItem = New roTimeBlockItem
                                    myItem.Period.Begin = roConversions.Max(zIncidences.Item(i).Period.Begin, zForeCast.Item(w).Period.Begin)
                                    myItem.Period.Finish = roConversions.Min(zIncidences.Item(i).Period.Finish, zForeCast.Item(w).Period.Finish)
                                    myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue, True)
                                    If myItem.TimeValue.NumericValue > 0 Then
                                        myItem.TimeValue = roConversions.Min(zIncidences.Item(i).TimeValue, myItem.TimeValue)
                                        ' Justificamos el tiempo hasta el maximo
                                        If TimeToForecast >= myItem.TimeValue.NumericValue Then
                                            ' Si el tiempo maximo es mayor o igual a la incidnecia
                                            ' justificamos todo el tiempo de la incidencia
                                            xTimeValue = myItem.TimeValue.NumericValue

                                            ' Restamos del tiempo maximo el valor de la incidencia
                                            TimeToForecast = TimeToForecast - xTimeValue

                                            TimeForecast = TimeForecast + xTimeValue
                                        Else
                                            ' Justificamos parte de la incidencia con el valor del tiempo maximo
                                            xTimeValue = TimeToForecast

                                            TimeForecast = TimeForecast + TimeToForecast

                                            ' Dejamos el tiempo maximo a 0
                                            TimeToForecast = 0
                                        End If

                                        ' Justifica la incidencia
                                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, zForeCast.Item(w).Tag, xTimeValue, .Tag)

                                        ' Restamos el tiempo a la incidencia
                                        .TimeValue = Any2Time(.TimeValue.NumericValue - xTimeValue, True)
                                    End If
                                End If
                            End With
                        Next
                    Next

                    ' Si no hemos llegado al minimo
                    If TimeForecast < MinTimeToForecast Then
                        ' Genera justificación hasta llegar al mínimo
                        ' On Error Resume Next
                        intMinTimeIDCause = zForeCast.Item(w).Tag
                        Try
                            ' En el caso que este configurado una motivo diferente para justificar hasta llegar al mínimo, lo utilizamos
                            If InStr(strDescription, "#MINTIME_CAUSE=") > 0 Then
                                intMinTimeIDCause = Any2String(ExecuteScalar("@SELECT# Isnull(ID,0) FROM Causes WHERE ShortName like '" & Mid(strDescription, InStr(strDescription, "#MINTIME_CAUSE=") + 15, 3) & "'"))
                                If intMinTimeIDCause = "0" OrElse intMinTimeIDCause = "" Then intMinTimeIDCause = zForeCast.Item(w).Tag
                            End If
                        Catch ex As Exception
                        End Try

                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, intMinTimeIDCause, MinTimeToForecast - TimeForecast, 0)

                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyForecastCausesLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyForecastCausesLive")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyForecastCauses(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal zForeCast As roTimeBlockList)
            '
            '  Comportamiento específico para previsiones de horas heredadas de Win32 (que pueden estar vigentes en Live ...)
            '  Si hay una previsión de incidencias, justifica la incidencia mayor como se indique
            '  hasta el maximo previsto.
            '
            Dim i As Integer
            Dim posMaxIncidence As Integer  'Posición de la incidencia de ausencia de mayor duración dentro de la lista de incidencias
            Dim timeMaxIncidence As Double  'Duración mayor de las incidencias de ausencia
            Dim justifiedTime As Double  'Tiempo justificado. Será el menor entre el tirmpo previsto y el de la incidencia real

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si no hay previsiones no hace nada
                If zForeCast.Count = 0 Then Exit Sub

                posMaxIncidence = 0
                timeMaxIncidence = 0

                ' Repasamos incidencias buscando la que mejor se ajusta a la ausencia prevista
                For i = 1 To zIncidences.Count
                    With zIncidences.Item(i)
                        If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence) And .TimeValue.VBNumericValue > 0 Then

                            ' Busco la incidencia de ausencia de mayor duración
                            If .TimeValue.NumericValue > timeMaxIncidence Then
                                posMaxIncidence = i
                                timeMaxIncidence = .TimeValue.NumericValue
                            End If

                        End If
                    End With
                Next

                ' Finalmente ... si encontré un máximo
                If posMaxIncidence <> 0 Then

                    'Tiempo justificado
                    justifiedTime = IIf(zForeCast.Item(1).TimeValue.NumericValue < zIncidences.Item(posMaxIncidence).TimeValue.NumericValue, zForeCast.Item(1).TimeValue.NumericValue, zIncidences.Item(posMaxIncidence).TimeValue.NumericValue)

                    ' Justifica incidencia
                    Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, zForeCast.Item(1).Tag, justifiedTime, zIncidences.Item(posMaxIncidence).Tag)

                    ' Restamos el como máximo el tiempo justificado del pendiente de justificar para esa incidencia
                    zIncidences.Item(posMaxIncidence).TimeValue = Any2Time(zIncidences.Item(posMaxIncidence).TimeValue.NumericValue - justifiedTime)

                    'TODO:>> ¿Qué hago si no encuentro ninguna?
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyForecastCausesLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyForecastCausesLive")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyProgrammedHolidays(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal zProgrammedHolidays As roTimeBlockList)
            '
            '  Si hay una previsión de vacaciones por horas, justifica todas las incidencias entre el periodo indicada como vacaciones o permisos
            '
            Dim myItem As roTimeBlockItem
            Dim TimeToForecast As Double
            Dim i As Integer
            Dim w As Integer
            Dim xTimeValue As Double
            Dim z As Integer
            Dim ValidIncidence As Boolean
            Dim TimeForecast As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si no hay previsiones no hace nada
                If zProgrammedHolidays.Count = 0 Then Exit Sub

                For w = 1 To zProgrammedHolidays.Count

                    ' Tiempo maximo a justificar
                    TimeToForecast = zProgrammedHolidays.Item(w).TimeValue.NumericValue
                    TimeForecast = 0

                    For z = 1 To 2
                        For i = 1 To zIncidences.Count

                            ValidIncidence = False

                            If TimeToForecast <= 0 Then Exit For

                            With zIncidences.Item(i)
                                ' 01. Pirmero justificamos todas las incidencias de ausencia menos (Ausencia en horas flexibles y diferencia negativa)
                                ' 02. Finalmente restamos el tiempo maximo que queda de las incidencias Ausencia en horas flexibles y diferencia negativa
                                If z = 1 Then
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence) And .BlockType <> roTimeBlockItem.roBlockType.roBTFlexibleUnderworking And .BlockType <> roTimeBlockItem.roBlockType.roBTDailyUnderworking And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                Else
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence) And (.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleUnderworking Or .BlockType = roTimeBlockItem.roBlockType.roBTDailyUnderworking) And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                End If

                                If ValidIncidence Then
                                    ' Si alguna parte de la incidencia se solapa con el periodo indicado
                                    myItem = New roTimeBlockItem
                                    myItem.Period.Begin = roConversions.Max(zIncidences.Item(i).Period.Begin, zProgrammedHolidays.Item(w).Period.Begin)
                                    myItem.Period.Finish = roConversions.Min(zIncidences.Item(i).Period.Finish, zProgrammedHolidays.Item(w).Period.Finish)
                                    myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue, True)
                                    If myItem.TimeValue.NumericValue > 0 Then
                                        myItem.TimeValue = roConversions.Min(zIncidences.Item(i).TimeValue, myItem.TimeValue)
                                        ' Justificamos el tiempo hasta el maximo
                                        If TimeToForecast >= myItem.TimeValue.NumericValue Then
                                            ' Si el tiempo maximo es mayor o igual a la incidnecia
                                            ' justificamos todo el tiempo de la incidencia
                                            xTimeValue = myItem.TimeValue.NumericValue

                                            ' Restamos del tiempo maximo el valor de la incidencia
                                            TimeToForecast = TimeToForecast - xTimeValue

                                            TimeForecast = TimeForecast + xTimeValue
                                        Else
                                            ' Justificamos parte de la incidencia con el valor del tiempo maximo
                                            xTimeValue = TimeToForecast

                                            TimeForecast = TimeForecast + TimeToForecast

                                            ' Dejamos el tiempo maximo a 0
                                            TimeToForecast = 0
                                        End If

                                        ' Justifica la incidencia
                                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, zProgrammedHolidays.Item(w).Tag, xTimeValue, .Tag)

                                        ' Restamos el tiempo a la incidencia
                                        .TimeValue = Any2Time(.TimeValue.NumericValue - xTimeValue)
                                    End If
                                End If
                            End With
                        Next
                    Next

                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedHolidays")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedHolidays")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyProgrammedOvertimes(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal zProgrammedOvertimes As roTimeBlockList)
            '
            '  Si hay una previsión de horas de exceso, justifica todas las incidencias de exceso entre el periodo indicado
            '
            Dim myItem As roTimeBlockItem
            Dim TimeToOvertime As Double
            Dim MinTimeToOvertime As Double
            Dim i As Integer
            Dim w As Integer
            Dim xTimeValue As Double
            Dim z As Integer
            Dim ValidIncidence As Boolean
            Dim TimeOvertime As Double
            Dim intMinTimeIDCause As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si no hay previsiones no hace nada
                If zProgrammedOvertimes.Count = 0 Then Exit Sub

                For w = 1 To zProgrammedOvertimes.Count

                    ' Tiempo maximo a justificar
                    TimeToOvertime = zProgrammedOvertimes.Item(w).TimeValue.NumericValue
                    TimeOvertime = 0

                    ' Tiempo minimo
                    MinTimeToOvertime = zProgrammedOvertimes.Item(w).InCause

                    For z = 1 To 2
                        For i = 1 To zIncidences.Count

                            ValidIncidence = False

                            If TimeToOvertime <= 0 Then Exit For

                            With zIncidences.Item(i)
                                ' 01. Pirmero justificamos todas las incidencias de exceso menos (Exceso en horas flexibles y diferencia positiva)
                                ' 02. Finalmente restamos el tiempo maximo que queda de las incidencias de exceso en horas flexibles y diferencia positiva
                                If z = 1 Then
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyOvertime) And .BlockType <> roTimeBlockItem.roBlockType.roBTFlexibleOverworking And .BlockType <> roTimeBlockItem.roBlockType.roBTDailyOverworking And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                Else
                                    If .ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyOvertime) And (.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleOverworking Or .BlockType = roTimeBlockItem.roBlockType.roBTDailyOverworking) And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True
                                End If

                                If ValidIncidence Then
                                    ' Si alguna parte de la incidencia se solapa con el periodo indicado
                                    myItem = New roTimeBlockItem
                                    myItem.Period.Begin = roConversions.Max(zIncidences.Item(i).Period.Begin, zProgrammedOvertimes.Item(w).Period.Begin)
                                    myItem.Period.Finish = roConversions.Min(zIncidences.Item(i).Period.Finish, zProgrammedOvertimes.Item(w).Period.Finish)
                                    myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue, True)
                                    If myItem.TimeValue.NumericValue > 0 Then
                                        myItem.TimeValue = roConversions.Min(zIncidences.Item(i).TimeValue, myItem.TimeValue)
                                        ' Justificamos el tiempo hasta el maximo
                                        If TimeToOvertime >= myItem.TimeValue.NumericValue Then
                                            ' Si el tiempo maximo es mayor o igual a la incidnecia
                                            ' justificamos todo el tiempo de la incidencia
                                            xTimeValue = myItem.TimeValue.NumericValue

                                            ' Restamos del tiempo maximo el valor de la incidencia
                                            TimeToOvertime = TimeToOvertime - xTimeValue

                                            TimeOvertime = TimeOvertime + xTimeValue
                                        Else
                                            ' Justificamos parte de la incidencia con el valor del tiempo maximo
                                            xTimeValue = TimeToOvertime

                                            TimeOvertime = TimeOvertime + TimeToOvertime

                                            ' Dejamos el tiempo maximo a 0
                                            TimeToOvertime = 0
                                        End If

                                        ' Justifica la incidencia
                                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, zProgrammedOvertimes.Item(w).Tag, xTimeValue, .Tag)

                                        ' Restamos el tiempo a la incidencia
                                        .TimeValue = Any2Time(.TimeValue.NumericValue - xTimeValue)
                                    End If
                                End If
                            End With
                        Next
                    Next

                    ' Si no hemos llegado al minimo
                    If TimeOvertime < MinTimeToOvertime Then
                        ' Genera justificación hasta llegar al mínimo
                        intMinTimeIDCause = zProgrammedOvertimes.Item(w).Tag
                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, intMinTimeIDCause, MinTimeToOvertime - TimeOvertime, 0)
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedOvertimes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProgrammedOvertimes")
            Finally

            End Try
        End Sub

        Private Sub Execute_ApplyUnexpectedOutRule(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime)
            '
            '  Si hay una regla avanzada de interrupcion, justifica todas las incidencias de interrupción hasta el tiempo maximo indicado
            '
            Dim sSQL As String
            Dim AdvancedParameters As String
            Dim myItem As roTimeBlockItem
            Dim xTimeValue As Double
            Dim ValidIncidence As Boolean
            Dim MaxTime As Double
            Dim strMaxTime As String
            Dim TimeToForecast As Double
            Dim TimeForecast As Double
            Dim IDCause As Double
            Dim IDShift As Double
            Dim Pos As Integer

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si el horario planificado no tiene regla de interrupcionno hace nada

                ' Obtenemos el horario utilizado ese dia
                sSQL = "@SELECT# IDShiftUsed  FROM DailySchedule with (nolock)  WHERE IDEmployee=" & zEmployee & " AND Date=" & zTaskDate.SQLSmallDateTime
                IDShift = Any2Double(ExecuteScalar(sSQL))
                If IDShift <= 0 Then Exit Sub

                ' Obtenemos el campo de opciones avanzadas del horario utilizado.
                sSQL = "@SELECT# AdvancedParameters  FROM Shifts WHERE ID=" & IDShift
                AdvancedParameters = Any2String(ExecuteScalar(sSQL)).Trim
                If Len(AdvancedParameters) = 0 Then Exit Sub

                ' Miramos si el horario esta marcado tiempo maximo a justificar de interrupcion
                Pos = InStr(1, AdvancedParameters, "#DESRET=")
                If Pos = 0 Then Exit Sub

                ' Obtenemos el tiempo maximo a justificar
                strMaxTime = Mid$(AdvancedParameters, Pos + 8, 5)
                If Not IsDate(strMaxTime) Then Exit Sub

                MaxTime = Any2Time(strMaxTime).NumericValue

                ' Obtenemos la justificacion a utilizar para justificar las incidencias de interrupcion
                IDCause = dblUnexpectedOutRule_IDCause
                If IDCause <= 0 Then Exit Sub

                ' Tiempo maximo a justificar
                TimeToForecast = MaxTime
                TimeForecast = 0

                For i = 1 To zIncidences.Count

                    ValidIncidence = False

                    If TimeToForecast <= 0 Then Exit For

                    With zIncidences.Item(i)
                        ' Justificamos todas las incidencias de interrupciuon hasta el tiempo max
                        If .BlockType = roTimeBlockItem.roBlockType.roBTUnexpectedBreak And .TimeValue.VBNumericValue > 0 Then ValidIncidence = True

                        If ValidIncidence Then
                            ' Si alguna parte de la incidencia se solapa con el periodo indicado
                            myItem = New roTimeBlockItem
                            myItem.Period.Begin = zIncidences.Item(i).Period.Begin
                            myItem.Period.Finish = zIncidences.Item(i).Period.Finish
                            myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue)
                            If myItem.TimeValue.NumericValue > 0 Then
                                myItem.TimeValue = roConversions.Min(zIncidences.Item(i).TimeValue, myItem.TimeValue)
                                ' Justificamos el tiempo hasta el maximo
                                If TimeToForecast >= myItem.TimeValue.NumericValue Then
                                    ' Si el tiempo maximo es mayor o igual a la incidnecia
                                    ' justificamos todo el tiempo de la incidencia
                                    xTimeValue = myItem.TimeValue.NumericValue

                                    ' Restamos del tiempo maximo el valor de la incidencia
                                    TimeToForecast = TimeToForecast - xTimeValue

                                    TimeForecast = TimeForecast + xTimeValue
                                Else
                                    ' Justificamos parte de la incidencia con el valor del tiempo maximo
                                    xTimeValue = TimeToForecast

                                    TimeForecast = TimeForecast + TimeToForecast

                                    ' Dejamos el tiempo maximo a 0
                                    TimeToForecast = 0
                                End If

                                ' Justifica la incidencia
                                Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, IDCause, xTimeValue, .Tag)

                                ' Restamos el tiempo a la incidencia
                                .TimeValue = Any2Time(.TimeValue.NumericValue - xTimeValue)
                            End If
                        End If
                    End With
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyUnexpectedOutRule")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyUnexpectedOutRule")
            Finally

            End Try

        End Sub

        Private Function Execute_RemovePreviousUnjustifiedCauses(ByVal zEmployee As Long, ByRef zTime As roTime) As Boolean
            '
            '  Elimina justificaciones que existian anteriormente y no estaban justificadas o las
            '  que se generaron automáticamente para que se procesen de nuevo.
            '

            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                Dim sSQL As String = "@DELETE# FROM DailyCauses WHERE IDEmployee=" &
                    zEmployee & " AND Date=" & zTime.SQLSmallDateTime & " AND (IDCause=0 OR isnull(Manual,0)=0) AND isnull(AccrualsRules,0)=0"
                bolret = ExecuteSql(sSQL)

                If bolret Then
                    sSQL = "@DELETE# FROM DailyCauses WHERE IDEmployee=" &
                    zEmployee & " AND Date=" & zTime.SQLSmallDateTime & " AND Value = 0 AND IDCenter > 0 AND Manual=1"
                    bolret = ExecuteSql(sSQL)
                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_RemovePreviousUnjustifiedCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_RemovePreviousUnjustifiedCauses")
            Finally

            End Try

            Return bolret
        End Function

        Private Function Execute_RemoveUnchangedIncidences(ByRef Incidences As roTimeBlockList, ByRef Causes As DataTable) As Boolean
            '
            ' Elimina de la lista las incidencias que ya están codificadas
            ' (el Calculador borra las justificaciones de una incidencia cuando esta cambia, asi que si
            '  encontramos justificaciones de incidencias, significa que esa incidencia ya ha sido tratada)
            '
            Dim Index As Long
            Dim bolret = False

            Try

                oState.Result = EngineResultEnum.NoError

                If Causes Is Nothing OrElse Causes.Rows.Count = 0 Then
                    Return True
                End If

                Index = 1
                While Index <= Incidences.Count
                    ' Busca justificaciones asociadas
                    Dim rows() As DataRow = Causes.Select("IDRelatedIncidence=" & Incidences.Item(Index).Tag.ToString)
                    If rows.Length > 0 Then
                        ' Hay justificaciones asociadas, elimina esta incidencia
                        Incidences.Remove(Index)
                    Else
                        ' No hay justificaciones asociadas
                        Index = Index + 1
                    End If
                End While

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_RemoveUnchangedIncidences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_RemoveUnchangedIncidences")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_ApplyAllShiftRules(ByRef zCauses As DataTable, ByRef zShift As roShiftEngine, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef TaskDate As roTime) As Boolean
            '
            ' Aplica todas las reglas de justificaciones indicadas en el horario
            '
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                If Not zShift Is Nothing Then
                    ' Para cada regla del horario... (Ordenadas por RuleType e ID)
                    For Each rule As roShiftEngineRule In zShift.SimpleRules
                        If rule.Aux <> "-1" Then
                            If rule.Type = ShiftRuleType.Simple Then
                                ' Para cada incidencias
                                For Index As Integer = 1 To zIncidences.Count
                                    ' Evalua condición para cada incidencia
                                    If Execute_EvaluateShiftRuleCondition(rule, zIncidences.Item(Index), zEmployee, TaskDate.Value) Then
                                        ' Si la condición es cierta, aplica la regla
                                        Execute_ApplyShiftRule(zCauses, rule, zIncidences.Item(Index), zEmployee, TaskDate.Value)
                                    End If

                                    If oState.Result <> EngineResultEnum.NoError Then Exit For

                                Next
                            End If
                        End If
                    Next
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_ApplyAllShiftRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyAllShiftRules")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_EvaluateShiftRuleCondition(ByRef rule As roShiftEngineRule, ByRef Incidence As roTimeBlockItem, ByVal IDEmployee As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Devuelve True si la incidencia cumple la condición de la regla
            '
            Dim iTmp As Long

            Dim sFromTime As String = ""
            Dim sToTime As String = ""
            Dim sAux As String = ""
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Comprueba tipo de incidencia
                If Incidence.BlockType <> Any2Long(rule.IDIncidence) Then
                    Return bolret
                    Exit Function
                End If

                ' Comprueba zona de la incidencia
                iTmp = Any2Long(rule.IDZone)
                If iTmp <> -1 Then
                    If Incidence.TimeZone <> iTmp Then
                        Return bolret
                        Exit Function
                    End If
                End If

                ' Comprueba tiempo de incidencia
                If rule.ConditionValueType = eShiftRuleValueType.DirectValue Then
                    ' Valores directos
                    If Incidence.TimeValue.VBNumericValue > Any2Time(rule.ToTime, True).VBNumericValue Or
                        Incidence.TimeValue.VBNumericValue < Any2Time(rule.FromTime, True).VBNumericValue Then
                        Return bolret
                        Exit Function
                    End If
                Else
                    ' Campos de la ficha
                    If Any2String(rule.FromValueUserFieldName) <> "" Then
                        sFromTime = "00:00:00"
                        Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.FromValueUserFieldName, TaskDate, New UserFields.roUserFieldState, False)
                        If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then
                            sFromTime = Any2String(oUserField.FieldRawValue)
                            If InStr(sFromTime, "-") > 0 Then
                                sFromTime = "00:00:00"
                            End If
                        End If
                        sToTime = "23:59:00"
                    ElseIf Any2String(rule.ToValueUserFieldName) <> "" Then
                        sFromTime = "00:00:00"
                        'sToTime = "23:59:00"
                        Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.ToValueUserFieldName, TaskDate, New UserFields.roUserFieldState, False)
                        If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then
                            sToTime = Any2String(oUserField.FieldRawValue)
                            If InStr(sToTime, "-") > 0 Then
                                sToTime = "00:00:00"
                            End If
                        End If

                    ElseIf Any2String(rule.BetweenValueUserFieldName) <> "" Then
                        Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.BetweenValueUserFieldName, TaskDate, New UserFields.roUserFieldState, False)
                        If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then sAux = oUserField.FieldRawValue
                        sFromTime = "" : sToTime = ""
                        If StringItemsCount(sAux, "*") = 2 Then
                            sFromTime = String2Item(sAux, 0, "*")
                            sToTime = String2Item(sAux, 1, "*")
                            If InStr(sToTime, "-") > 0 Then
                                sToTime = "00:00:00"
                            End If
                            If InStr(sFromTime, "-") > 0 Then
                                sFromTime = "00:00:00"
                            End If
                        Else
                            Return bolret
                            Exit Function
                        End If
                    Else
                        Return bolret
                        Exit Function
                    End If

                    If Incidence.TimeValue.VBNumericValue > Any2Time(sToTime, True).VBNumericValue Or
                        Incidence.TimeValue.VBNumericValue < Any2Time(sFromTime, True).VBNumericValue Then
                        Return bolret
                        Exit Function
                    End If
                End If

                ' Si hemos llegado aqui, la condicion es cierta
                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_ApplyAllShiftRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyAllShiftRules")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_ApplyShiftRule(ByRef Causes As DataTable, ByRef rule As roShiftEngineRule, ByRef Incidence As roTimeBlockItem, ByVal IDEmployee As Long, ByVal DateTime As Date) As Boolean
            '
            ' Aplica una regla (previamente se ha evaluado la condición)
            '
            Dim TotalMinutes As Double
            Dim sMaxTime As Double
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtiene tiempo a justificar
                If rule.ActionValueType = eShiftRuleValueType.DirectValue Then
                    ' Valor directo

                    TotalMinutes = roConversions.Min(Incidence.TimeValue.VBNumericValue, Any2Time(rule.MaxTime, True).VBNumericValue)
                Else
                    ' Campo de la ficha
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.MaxValueUserFieldName, DateTime, New UserFields.roUserFieldState, False)
                    If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then sMaxTime = Any2Time(oUserField.FieldRawValue, True).VBNumericValue

                    TotalMinutes = roConversions.Min(Incidence.TimeValue.VBNumericValue, sMaxTime)
                End If

                If TotalMinutes < 0 Then TotalMinutes = 0
                ' Crea justificación con justificación
                Execute_AddCause(Causes, IDEmployee, DateTime, rule.IDCause, Any2Time(Date.FromOADate(TotalMinutes)).NumericValue(True), Incidence.Tag)

                ' Elimina tiempo justificado de la incidencia
                Incidence.TimeValue = Incidence.TimeValue.Substract(TotalMinutes)

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyShiftRule")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyShiftRule")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_AddCause(ByRef Causes As DataTable, ByVal IDEmployee As Long, ByVal DateTime As Date, ByVal IDCause As Integer, ByVal Value As Double, ByVal IDRelatedIncidence As Long, Optional ByVal AnyValue As Boolean = False) As Boolean
            '
            ' Crea un registro nuevo de justificación
            '
            Dim bolret As Boolean = True

            Try

                ' Si el valor es 0, no graba nada
                If Not AnyValue Then
                    If Value <= 0 Then
                        Return bolret
                    End If
                End If

                Dim oRow As DataRow
                oRow = Causes.NewRow
                oRow("IDEmployee") = IDEmployee
                oRow("Date") = DateTime
                oRow("IDCause") = IDCause
                oRow("Value") = Value
                oRow("IDRelatedIncidence") = IDRelatedIncidence
                oRow("AccrualsRules") = 0
                oRow("DailyRule") = 0
                oRow("AccruedRule") = 0
                oRow("Manual") = 0

                Try
                    ' Guarda datos en la base de datos
                    Causes.Rows.Add(oRow)
                    da.Update(Causes)
                Catch ex As Exception
                    If ex.Message.ToUpper.Contains("PRIMARY") Then
                        Causes.Rows.Remove(oRow)
                        mSQLExecute.Add(mSQLExecute.Count + 1, "@UPDATE# DailyCauses Set Value=Value + " & Value.ToString.Replace(",", ".") & " WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & Any2Time(DateTime).SQLSmallDateTime & " AND IDCause=" & IDCause.ToString & " AND IDRelatedIncidence=" & IDRelatedIncidence.ToString)
                    End If
                End Try

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_AddCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_AddCause")
            End Try

            Return bolret

        End Function

        Private Sub AddCauseWithDifferenceTime(ByVal IDEmployee As Integer, ByVal ActualDate As roTime, ByVal IDCause As Double, ByVal DifTime As Double)
            '
            ' Insertamos una justificación independiente con la diferencia de tiempo
            '

            Dim sSQL As String

            Try

                sSQL = "@INSERT# INTO DailyCauses (IDEmployee,Date,IDRelatedIncidence,IDCause,Value,Manual) Values ("
                sSQL = sSQL & IDEmployee & "," & ActualDate.SQLSmallDateTime & ",0,"
                sSQL = sSQL & IDCause & "," & Any2String(DifTime).Replace(",", ".") & ",0)"

                ExecuteSql(sSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: AddCauseWithDifferenceTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::AddCauseWithDifferenceTime")
            Finally

            End Try

        End Sub

        Private Function Execute_GetForecast(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine) As roTimeBlockList
            '
            ' Obtiene previsiones de ausencia por horas del dia indicado.
            '
            Dim Index As Long
            Dim strSQL As String

            Dim myList As New roTimeBlockList
            Dim myItem As roTimeBlockItem
            Dim AddDay As Integer
            Dim auxDate As Date
            Dim auxFinishDate As Date

            Dim bolApplyForecast As Boolean

            Dim zShiftBefore As roShiftEngine = Nothing
            Dim zShiftAfter As roShiftEngine = Nothing
            Dim zShiftActual As roShiftEngine = Nothing
            Dim zShifttmp As roShiftEngine = Nothing

            Try

                ' Select de incidencias previstas para ese dia y empleado

                strSQL = "@SELECT# * FROM ProgrammedCauses WHERE IDEmployee=" & zEmployee &
                        " AND ( (Date <=" & zDate.SQLSmallDateTime & " And IsNull(FinishDate, Date) >= " & zDate.SQLSmallDateTime & ") " &
                        "       OR  (Date =" & zDate.Add(1, "d").SQLSmallDateTime & ") OR  (Date =" & zDate.Add(-1, "d").SQLSmallDateTime & ")) "

                Me.oState.Result = EngineResultEnum.NoError

                Dim dTbl As DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRow As DataRow In dTbl.Rows

                        If Index = 0 Then
                            If Not zShift Is Nothing Then zShiftActual = roBaseEngineManager.Execute_SetShiftlimits(zShift, zDate, zEmployee, oState)

                            Dim bIsHoliday As Boolean
                            ' Obtenemos el horario de ayer
                            bIsHoliday = roTypes.Any2Boolean(ExecuteScalar("@SELECT# IsHolidays FROM DailySchedule with (nolock)  WHERE IdEmployee = " & zEmployee.ToString & " AND Date = " & zDate.Add(-1, "d").SQLSmallDateTime))
                            zShifttmp = roBaseEngineManager.Execute_GetShift(zEmployee, zDate.Add(-1, "d"), bIsHoliday, False, oState)
                            If Not zShifttmp Is Nothing Then zShiftBefore = roBaseEngineManager.Execute_SetShiftlimits(zShifttmp, zDate.Add(-1, "d"), zEmployee, oState)

                            ' Obtenemos el horario de mañana
                            bIsHoliday = roTypes.Any2Boolean(ExecuteScalar("@SELECT# IsHolidays FROM DailySchedule with (nolock)  WHERE IdEmployee = " & zEmployee.ToString & " AND Date = " & zDate.Add(1, "d").SQLSmallDateTime))
                            zShifttmp = roBaseEngineManager.Execute_GetShift(zEmployee, zDate.Add(1, "d"), bIsHoliday, False, oState)
                            If Not zShifttmp Is Nothing Then zShiftAfter = roBaseEngineManager.Execute_SetShiftlimits(zShifttmp, zDate.Add(1, "d"), zEmployee, oState)
                        End If

                        ' Obtiene datos de la prevision
                        myItem = New roTimeBlockItem

                        'myitem.TimeZone
                        myItem.BlockType = 1010     'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.TimeZone = 0         'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        '2012/05/04: Usaremos esta campo para distinguir en Live Ausencias calculadas con la lógica de Win32 (aplica a actualizaciones Win32 a Live)
                        myItem.Tag = oRow("IDCause")

                        ' Miro si se calculó con la lógica de Win32
                        If Any2Boolean(oRow("Win32")) Then
                            myItem.TimeZone = 1
                        End If

                        AddDay = 0

                        ' Fecha inicio
                        auxDate = oRow("Date")

                        ' Fecha fin
                        auxFinishDate = IIf(IsDBNull(oRow("FinishDate")), oRow("Date"), oRow("FinishDate"))

                        If zDate.NumericValue >= Any2Time(auxDate).NumericValue And zDate.NumericValue <= Any2Time(auxFinishDate).NumericValue Then
                            ' Si la fecha a calcular esta dentro del periodo de la prevision, es del mismo dia
                            AddDay = 0
                        Else
                            ' En caso contrario, la prevision aplica en un dia que a priori no es el de la fecha a procesar
                            If Any2Time(auxDate).NumericValue > zDate.NumericValue Then
                                AddDay = 1
                            ElseIf Any2Time(auxFinishDate).NumericValue < zDate.NumericValue Then
                                AddDay = -1
                            End If
                        End If

                        If Not IsDate(oRow("BeginTime")) Then
                            oRow("BeginTime") = Any2Time("1899/12/30 00:00").Value
                        End If
                        If Not IsDate(oRow("EndTime")) Then
                            oRow("EndTime") = Any2Time("1899/12/30 23:59").Value
                        End If

                        myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("BeginTime")).Value)).Add(AddDay, "d")  ' Inicio del Periodo
                        myItem.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("EndTime")).Value)).Add(AddDay, "d")  ' Fin del Periodo

                        myItem.TimeValue = Any2Time(oRow("Duration")) 'Duracion

                        ' Duracion minima
                        myItem.InCause = 0
                        If IsNumeric(oRow("MinDuration")) Then
                            myItem.InCause = Any2Time(oRow("MinDuration")).NumericValue
                        End If

                        ' Si la ausencia por horas es del dia posterior
                        ' miramos si el dia a procesar tiene horario nocturno
                        ' para aplicarla o no
                        bolApplyForecast = True
                        If AddDay > 0 Then
                            If Not zShiftActual Is Nothing Then
                                If zShiftActual.EndLimit.Date = zShiftActual.StartLimit.Date Then
                                    ' Si no es nocturno no aplica
                                    bolApplyForecast = False
                                ElseIf Any2Time(DateTimeAdd(zDate.Value, zShiftActual.EndLimit)).NumericValue < myItem.Period.Begin.NumericValue Then
                                    ' Si es nocturno, pero la prevision empieza despues del fin del horaro, no aplica
                                    bolApplyForecast = False
                                End If
                            End If
                        ElseIf AddDay = 0 Then
                            If Not zShiftBefore Is Nothing Then
                                If zShiftBefore.ID > 0 Then
                                    ' Si el horario de ayer es nocturno
                                    If zShiftBefore.EndLimit.Date > zShiftBefore.StartLimit.Date Then
                                        ' y la prevision de horas finaliza antes del fin del horario de ayer
                                        ' no aplica hoy
                                        If Any2Time(DateTimeAdd(zDate.Add(-1, "d").Value, zShiftBefore.EndLimit)).NumericValue >= myItem.Period.Finish.NumericValue Then
                                            bolApplyForecast = False
                                        End If
                                    End If
                                End If
                            End If

                            If Not zShiftAfter Is Nothing Then
                                If zShiftAfter.ID > 0 Then
                                    ' Si el horario de mañana es nocturno, pero emnpieza hoy
                                    If zShiftAfter.EndLimit.Date <> zShiftAfter.StartLimit.Date AndAlso zShiftAfter.StartLimit.Date <= Any2Time("1899/12/29 23:59").Value Then
                                        ' y la prevision de horas finaliza despues del inicio del horario de mañana
                                        ' no aplica hoy
                                        If Any2Time(DateTimeAdd(zDate.Add(1, "d").Value, zShiftAfter.StartLimit)).NumericValue < myItem.Period.Finish.NumericValue Then
                                            bolApplyForecast = False
                                        End If
                                    End If
                                End If
                            End If

                        ElseIf AddDay = -1 Then
                            If Not zShiftActual Is Nothing Then
                                If zShiftActual.StartLimit.Date = zShiftActual.EndLimit.Date Then
                                    ' Si no es nocturno  no aplica
                                    bolApplyForecast = False
                                ElseIf zShiftActual.StartLimit.Date <= Any2Time("1899/12/29 23:59").Value AndAlso Any2Time(DateTimeAdd(zDate.Value, zShiftActual.StartLimit)).NumericValue >= myItem.Period.Finish.NumericValue Then
                                    ' Si es nocturno del dia anterior, pero la prevision acaba antes del inicio del horaro, no aplica
                                    bolApplyForecast = False
                                ElseIf zShiftActual.StartLimit.Date >= Any2Time("1899/12/30 00:00").Value AndAlso zShiftActual.StartLimit.Date <= Any2Time("1899/12/30 23:59").Value AndAlso zShiftActual.EndLimit.Date > zShiftActual.StartLimit.Date Then
                                    ' Si el horario de hoy es nocturno pero acaba mañana, no aplica
                                    bolApplyForecast = False
                                End If
                            End If
                        End If

                        If bolApplyForecast Then myList.Add(myItem)
                        Index = Index + 1
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetForecast")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetForecast")
            Finally

            End Try

            Return myList

        End Function

        Private Function Execute_GetProgrammedHolidays(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine, ByVal zIncidences As roTimeBlockList) As roTimeBlockList
            '
            ' Obtiene previsiones de vacaciones por horas del dia indicado.
            '

            Dim myList As New roTimeBlockList
            Dim myItem As roTimeBlockItem
            Dim strSQL As String

            Dim AddDay As Integer
            Dim auxDate As Date

            Dim bolApplyForecast As Boolean

            Try

                If zShift Is Nothing Then
                    ' Si no tiene horario asignado salimos
                    Return myList
                End If

                Me.oState.Result = EngineResultEnum.NoError

                ' Select de vacaciones previstas por hora para ese dia y empleado,
                ' y el dia posterior que no sean de dias completos
                strSQL = "@SELECT# * FROM ProgrammedHolidays WHERE IDEmployee=" & zEmployee &
                         " AND ( (Date =" & zDate.SQLSmallDateTime & ") " &
                         "       OR  (Date =" & zDate.Add(1, "d").SQLSmallDateTime & " AND AllDay =0)) "

                Dim dTbl As DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRow As DataRow In dTbl.Rows
                        ' Añade cada incidencia a la lista
                        myItem = New roTimeBlockItem

                        'myitem.TimeZone
                        myItem.BlockType = 1010     'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.TimeZone = 0         'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.Tag = oRow("IDCause")

                        AddDay = 0
                        If Any2Boolean(oRow("AllDay")) Then
                            ' Si es el dia completo
                            ' marcamos todo el periodo del horario
                            If zIncidences IsNot Nothing AndAlso zIncidences.Count > 0 Then
                                For z = 1 To zIncidences.Count
                                    If z = 1 Then
                                        myItem.Period.Begin = zIncidences.Item(z).Period.Begin
                                        myItem.Period.Finish = zIncidences.Item(z).Period.Finish
                                    Else
                                        myItem.Period.Begin = roConversions.Min(zIncidences.Item(z).Period.Begin, myItem.Period.Begin)
                                        myItem.Period.Finish = roConversions.Max(zIncidences.Item(z).Period.Finish, myItem.Period.Finish)
                                    End If
                                Next z
                            Else
                                myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, zShift.StartLimit))
                                myItem.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, zShift.EndLimit))
                            End If
                        Else
                            ' Si la prevision es del dia posterior
                            ' el periodo debe ser del dia siguiente
                            AddDay = 0
                            auxDate = oRow("Date")
                            If Any2Time(auxDate).NumericValue > zDate.NumericValue Then
                                AddDay = 1
                            End If

                            myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("BeginTime")).Value)).Add(AddDay, "d")  ' Inicio del Periodo
                            myItem.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("EndTime")).Value)).Add(AddDay, "d")  ' Fin del Periodo
                        End If

                        myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue) 'Duracion

                        ' Si la prevision de vacaciones por horas es del dia posterior
                        ' miramos si el dia a procesar tiene horario nocturno
                        ' para aplicarla o no
                        bolApplyForecast = True
                        If AddDay > 0 Then
                            If Not zShift Is Nothing Then
                                If zShift.EndLimit.Date = zShift.StartLimit.Date Then
                                    bolApplyForecast = False
                                End If
                            End If
                        End If

                        If bolApplyForecast Then myList.Add(myItem)
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetProgrammedHolidays")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetProgrammedHolidays")
            Finally

            End Try

            Return myList

        End Function

        Private Function Execute_GetProgrammedOvertimes(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine) As roTimeBlockList
            '
            ' Obtiene previsiones de horas de exceso del dia indicado.
            '

            Dim myList As New roTimeBlockList
            Dim strSQL As String
            Dim Index As Long

            Dim myItem As roTimeBlockItem
            Dim AddDay As Integer
            Dim auxDate As Date
            Dim auxFinishDate As Date

            Dim bolApplyOvertime As Boolean

            Dim zShiftBefore As roShiftEngine = Nothing
            Dim zShiftAfter As roShiftEngine = Nothing

            Try

                ' Select de incidencias previstas para ese dia y empleado

                strSQL = "@SELECT# * FROM ProgrammedOvertimes WHERE IDEmployee=" & zEmployee &
                         " AND ( (BeginDate <=" & zDate.SQLSmallDateTime & " And EndDate >= " & zDate.SQLSmallDateTime & ") " &
                         "       OR  (BeginDate =" & zDate.Add(1, "d").SQLSmallDateTime & ") OR  (BeginDate =" & zDate.Add(-1, "d").SQLSmallDateTime & ")) "

                Me.oState.Result = EngineResultEnum.NoError

                Dim dTbl As DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRow As DataRow In dTbl.Rows
                        If Index = 0 Then
                            Dim bIsHoliday As Boolean
                            ' Obtenemos el horario de ayer
                            bIsHoliday = roTypes.Any2Boolean(ExecuteScalar("@SELECT# IsHolidays FROM DailySchedule with (nolock)  WHERE IdEmployee = " & zEmployee.ToString & " AND Date = " & zDate.Add(-1, "d").SQLSmallDateTime))
                            zShiftBefore = roBaseEngineManager.Execute_GetShift(zEmployee, zDate.Add(-1, "d"), bIsHoliday, False, oState)
                            ' Obtenemos el horario de mañana
                            bIsHoliday = roTypes.Any2Boolean(ExecuteScalar("@SELECT# IsHolidays FROM DailySchedule with (nolock)  WHERE IdEmployee = " & zEmployee.ToString & " AND Date = " & zDate.Add(1, "d").SQLSmallDateTime))
                            zShiftBefore = roBaseEngineManager.Execute_GetShift(zEmployee, zDate.Add(-1, "d"), bIsHoliday, False, oState)
                        End If

                        ' Añade cada incidencia a la lista
                        myItem = New roTimeBlockItem

                        myItem.BlockType = 1010     'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.TimeZone = 0         'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.Tag = oRow("IDCause")

                        AddDay = 0

                        ' Fecha inicio
                        auxDate = oRow("BeginDate")

                        ' Fecha fin
                        auxFinishDate = oRow("EndDate")

                        If zDate.NumericValue >= Any2Time(auxDate).NumericValue And zDate.NumericValue <= Any2Time(auxFinishDate).NumericValue Then
                            ' Si la fecha a calcular esta dentro del periodo de la prevision, es del mismo dia
                            AddDay = 0
                        Else
                            ' En caso contrario, la prevision aplica en un dia que a priori no es el de la fecha a procesar
                            If Any2Time(auxDate).NumericValue > zDate.NumericValue Then
                                AddDay = 1
                            ElseIf Any2Time(auxFinishDate).NumericValue < zDate.NumericValue Then
                                AddDay = -1
                            End If
                        End If

                        If Not IsDate(oRow("BeginTime")) Then
                            oRow("BeginTime") = Any2Time("1899/12/30 00:00").Value
                        End If
                        If Not IsDate(oRow("EndTime")) Then
                            oRow("EndTime") = Any2Time("1899/12/30 23:59").Value
                        End If

                        myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("BeginTime")).Value)).Add(AddDay, "d")  ' Inicio del Periodo
                        myItem.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, Any2Time(oRow("EndTime")).Value)).Add(AddDay, "d")  ' Fin del Periodo

                        myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue) 'Duracion

                        ' Duracion minima
                        myItem.InCause = 0
                        If IsNumeric(oRow("MinDuration")) Then
                            myItem.InCause = Any2Time(oRow("MinDuration")).NumericValue
                        End If

                        ' Si la prevision de horas de exceso es del dia posterior
                        ' miramos si el dia a procesar tiene horario nocturno
                        ' para aplicarla o no
                        bolApplyOvertime = True
                        If AddDay > 0 Then
                            If Not zShift Is Nothing Then
                                If zShift.EndLimit.Date = zShift.StartLimit.Date Then
                                    ' Si no es nocturno no aplica
                                    bolApplyOvertime = False
                                ElseIf Any2Time(DateTimeAdd(zDate.Value, zShift.EndLimit)).NumericValue < myItem.Period.Begin.NumericValue Then
                                    ' Si es nocturno, pero la prevision empieza despues del fin dle horaro, no aplica
                                    bolApplyOvertime = False
                                End If
                            End If
                        ElseIf AddDay = 0 Then
                            If Not zShiftBefore Is Nothing Then
                                If zShiftBefore.ID > 0 Then
                                    ' Si el horario de ayer es nocturno
                                    If zShiftBefore.EndLimit.Date > zShiftBefore.StartLimit.Date Then
                                        ' y la prevision de horas finaliza antes del fin del horario de ayer
                                        ' no aplica hoy
                                        If Any2Time(DateTimeAdd(zDate.Add(-1, "d").Value, zShiftBefore.EndLimit)).NumericValue >= myItem.Period.Finish.NumericValue Then
                                            bolApplyOvertime = False
                                        End If
                                    End If
                                End If
                            End If

                            If Not zShiftAfter Is Nothing Then
                                If zShiftAfter.ID > 0 Then
                                    ' Si el horario de mañana es nocturno, pero emnpieza hoy
                                    If zShiftAfter.EndLimit.Date <> zShiftAfter.StartLimit.Date AndAlso zShiftAfter.StartLimit.Date = Any2Time("1899/12/29 00:00").Value Then
                                        ' y la prevision de horas finaliza despues del inicio del horario de mañana
                                        ' no aplica hoy
                                        If Any2Time(DateTimeAdd(zDate.Add(1, "d").Value, zShiftAfter.StartLimit)).NumericValue < myItem.Period.Finish.NumericValue Then
                                            bolApplyOvertime = False
                                        End If
                                    End If
                                End If
                            End If

                        ElseIf AddDay = -1 Then
                            If Not zShift Is Nothing Then
                                If zShift.StartLimit.Date = zShift.EndLimit.Date Then
                                    ' Si no es nocturno  no aplica
                                    bolApplyOvertime = False
                                ElseIf zShift.StartLimit.Date = Any2Time("1899/12/29 00:00").Value AndAlso Any2Time(DateTimeAdd(zDate.Value, zShift.StartLimit)).NumericValue >= myItem.Period.Finish.NumericValue Then
                                    ' Si es nocturno del dia anterior, pero la prevision acaba antes del inicio del horaro, no aplica
                                    bolApplyOvertime = False
                                End If
                            End If
                        End If

                        If bolApplyOvertime Then myList.Add(myItem)
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetProgrammedOvertimes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetProgrammedOvertimes")
            Finally

            End Try

            Return myList

        End Function

        Private Sub Execute_ApplyPunchedCauses(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime)
            '
            ' Aplica todas las reglas de justificaciones indicadas por el terminal
            '
            Dim Index As Long

            Dim MoveInIDCause As Long
            Dim MoveOutIDCause As Long
            Dim MoveInTime As Double
            Dim MoveOutTime As Double
            Dim OtherTime As Double
            Dim zMoves As roMoveList
            Dim i As Integer
            Dim lMovesItems As New List(Of roMoveItem)

            Dim dMaxForecast As New Dictionary(Of Integer, Double)

            Try

                ' Obtenemos los valores máximos de las justificaciones que se cargaron en la inicialización
                For i = 1 To zMaxTimeToForecast.Count
                    dMaxForecast.Add(Any2Double(zMaxTimeToForecast.Key(i)), Any2Double(zMaxTimeToForecast.Item(i, roCollection.roSearchMode.roByIndex)))
                Next i

                ' Obtenemos los movimientos del dia
                zMoves = roBaseEngineManager.Execute_GetMovesLive(zEmployee, zTaskDate, oState)

                ' Eliminamos los que no estan completos
                Index = 0
                While Index <= zMoves.Moves.Count - 1
                    If Not zMoves.Moves(Index).Period.Begin.IsValid OrElse Not zMoves.Moves(Index).Period.Finish.IsValid Then
                        ' Eliminamos el movimiento
                        zMoves.Moves.RemoveAt(Index)
                    Else
                        ' Pasamos al siguiene movimiento
                        Index = Index + 1
                    End If
                End While

                Dim oNewMoveItem As roMoveItem
                For iMove As Integer = 0 To zMoves.Moves.Count - 1
                    oNewMoveItem = New roMoveItem
                    oNewMoveItem.InCause = zMoves.Moves(iMove).InCause
                    oNewMoveItem.OutCause = zMoves.Moves(iMove).OutCause
                    oNewMoveItem.Period.Begin.Value = zMoves.Moves(iMove).Period.Begin.Value
                    oNewMoveItem.Period.Finish.Value = zMoves.Moves(iMove).Period.Finish.Value
                    lMovesItems.Add(oNewMoveItem)
                Next

                ' Miramos los movimientos uno por uno
                Dim oMoveItem As roMoveItem
                For Index = 0 To lMovesItems.Count - 1
                    oMoveItem = New roMoveItem
                    oMoveItem = lMovesItems.Item(Index)
                    ' Obtiene datos del movimiento
                    MoveInIDCause = Any2Long(oMoveItem.InCause)
                    MoveOutIDCause = Any2Long(oMoveItem.OutCause)
                    MoveInTime = Any2Double(oMoveItem.Period.Begin.Value)
                    MoveOutTime = Any2Double(oMoveItem.Period.Finish.Value)

                    ' Si han fichado para entrar la misma justificacion que la ultima salida la ignora o el resto de proceso no funciona
                    If (Index > 0) Then
                        If MoveInIDCause = lMovesItems.Item(Index - 1).OutCause Then MoveInIDCause = 0
                    End If

                    ' Mira si la entrada tiene justificación y no es de trabajo externo
                    If zExternalWorkCauses.Exists(MoveInIDCause) Then
                        MoveInIDCause = 0
                    End If

                    If MoveInIDCause <> 0 Then 'And mCausesCache.Exists(MoveInIDCause) Then
                        Dim oCauseIN As roCauseEngine = roBaseEngineManager.GetCauseFromCache(MoveInIDCause, oState)
                        If oCauseIN IsNot Nothing Then
                            ' Mira tipo de justificación
                            If oCauseIN.WorkingType Then
                                ' Viene a realizar extras

                                ' Justifica todas las extras de este movimiento
                                Execute_ApplyPunchedCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveInIDCause, MoveInTime, MoveOutTime, roTimeBlockItem.roManualEntryCauseType.roInWorking, dMaxForecast)
                            Else
                                ' Viene tarde con un motivo

                                ' Obtiene final del anterior movimiento
                                If Index = 0 Then OtherTime = 0 Else OtherTime = Any2Double(lMovesItems.Item(Index - 1).Period.Finish.Value)

                                ' Justifica todas las ausencias entre la ultima salida (si hay) y esta entrada
                                Execute_ApplyPunchedCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveInIDCause, OtherTime, MoveInTime, roTimeBlockItem.roManualEntryCauseType.roInAbsence, dMaxForecast)
                            End If
                        End If
                    End If

                    ' Mira si la salida tiene justificación y no es de trabajo externo
                    If zExternalWorkCauses.Exists(MoveOutIDCause) Then
                        MoveOutIDCause = 0
                    End If

                    If MoveOutIDCause <> 0 Then 'And mCausesCache.Exists(MoveOutIDCause) Then
                        Dim oCauseOUT As roCauseEngine = roBaseEngineManager.GetCauseFromCache(MoveOutIDCause, oState)

                        If oCauseOUT IsNot Nothing Then
                            ' Mira tipo de justificación
                            If oCauseOUT.WorkingType Then
                                ' Se quedo a hacer extras

                                ' Justifica todas las extras entre el incio y final del movimiento
                                Execute_ApplyPunchedCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveOutIDCause, MoveInTime, MoveOutTime, roTimeBlockItem.roManualEntryCauseType.roOutWorking, dMaxForecast)
                            Else
                                ' Se va antes por algun motivo

                                ' Obtiene inicio del siguiente movimiento (si hay)
                                If Index = (lMovesItems.Count - 1) Then OtherTime = 99999 Else OtherTime = Any2Double(lMovesItems.Item(Index + 1).Period.Begin.Value)

                                ' Justifica todas las ausencias entre esta salida y la proxima entrada (si hay)
                                Execute_ApplyPunchedCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveOutIDCause, MoveOutTime, OtherTime, roTimeBlockItem.roManualEntryCauseType.roInAbsence, dMaxForecast)

                                ' Si es la ultima salida diaria y esta justificación abre una ausencia prolongada,
                                '  la abrimos ahora
                                If Index = (lMovesItems.Count - 1) AndAlso oCauseOUT.StartsProgrammedAbsence Then
                                    mProgrammedAbsence.OpenProgrammedAbsenceForNextDay(MoveOutIDCause, oCauseOUT.MaxProgrammedAbsence)
                                End If
                            End If
                        End If
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_ApplyPunchedCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyPunchedCauses")
            Finally

            End Try

        End Sub

        Private Sub Execute_GetMaxTimeToForecast()
            Dim strSQL As String

            Try

                ' Obtenemos todas las justificaciones configuradas con valores maximos
                zMaxTimeToForecast = New roCollection

                strSQL = "@SELECT# ID,MaxTimeToForecast FROM Causes WHERE MaxTimeToForecast > 0 "
                Dim dtAux As DataTable
                dtAux = CreateDataTable(strSQL)
                If Not dtAux Is Nothing AndAlso dtAux.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtAux.Rows
                        zMaxTimeToForecast.Add(Any2Double(oRow("ID")), Any2Double(oRow("MaxTimeToForecast")))
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetMaxTimeToForecast")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetMaxTimeToForecast")
            Finally

            End Try
        End Sub

        Private Sub Execute_GetExternalWorkCauses()
            Dim strSQL As String

            Try

                ' Obtenemos todas las justificaciones configuradas como trabajo externo
                zExternalWorkCauses = New roCollection

                If bolProductiveAbsences Then
                    ' Justificacion inidcada en el registro del proceso
                    zExternalWorkCauses.Add(dblProductiveAbsenceCause)
                Else
                    ' v2 Justificacion inidcada desde la pantalla de justificaciones
                    strSQL = "@SELECT# ID FROM CAUSES WHERE ExternalWork= 1 AND WorkingType=0 "
                    Dim dtAux As DataTable
                    dtAux = CreateDataTable(strSQL)
                    If Not dtAux Is Nothing AndAlso dtAux.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtAux.Rows
                            zExternalWorkCauses.Add(Any2Double(oRow("ID")))
                        Next
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_GetExternalWorkCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_GetExternalWorkCauses")
            Finally

            End Try
        End Sub

        Private Sub Execute_ApplyPunchedCausesLogic(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal IDCause As Long, ByVal FromTime As Double, ByVal ToTime As Double, ByVal CauseType As roTimeBlockItem.roManualEntryCauseType, ByRef dMaxTimeToForecast As Dictionary(Of Integer, Double))
            '
            ' Justifica una incidencia mediante la justificación introducida en el movimiento
            '  de la forma apropiada.
            '
            Dim Incidence As roTimeBlockItem

            Dim Inci As roTimeBlockItem

            Dim ValidIncidence As Boolean
            Dim MinTime As Double
            Dim ApplyJustifyPeriod As Boolean
            Dim JustifyPeriodStart As Double
            Dim JustifyPeriodEnd As Double
            Dim JustifyAllPeriod As Boolean
            Dim TotalIncidence As Double
            Dim JustifyIncidences As Boolean
            Dim TotalTime As Double

            Try

                ' Comprobamos si la justificación tiene marcada la opción de solo Justificar entre horas
                ' obtenemos los parametros necesarios
                ApplyJustifyPeriod = Any2Boolean(ExecuteScalar("@SELECT# ApplyJustifyPeriod FROM Causes WHERE ID=" & IDCause))

                If ApplyJustifyPeriod Then
                    ' Si es Live y la justificación tiene marcada la opción de "Justificar entre horas"
                    ' Calculamos el total de tiempo a justificar en función del intervalo marcado
                    JustifyPeriodStart = Any2Double(ExecuteScalar("@SELECT# JustifyPeriodStart FROM Causes WHERE ID=" & IDCause))
                    JustifyPeriodEnd = Any2Double(ExecuteScalar("@SELECT# JustifyPeriodEnd FROM Causes WHERE ID=" & IDCause))
                    JustifyAllPeriod = Any2Boolean(ExecuteScalar("@SELECT# JustifyPeriodType FROM Causes WHERE ID=" & IDCause))

                    ' Obtenemos el total de tiempo de incidencias a justificar
                    For i As Integer = 1 To zIncidences.Count
                        Incidence = zIncidences.Item(i)
                        ' Comprueba el tipo de incidencia
                        If CauseType = roTimeBlockItem.roManualEntryCauseType.roInAbsence OrElse CauseType = roTimeBlockItem.roManualEntryCauseType.roOutAbsence Then
                            ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence)
                        Else
                            ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyOvertime)
                        End If

                        ' Solo acumulamos la ausencia si no es diferencia negativa
                        If Incidence.BlockType <> roTimeBlockItem.roBlockType.roBTDailyUnderworking Then
                            If ValidIncidence Then
                                ' Si el tipo es valido, comprueba que se encuentre en los limites tratados
                                If Incidence.Period.Begin.VBNumericValue < ToTime And Incidence.Period.Finish.VBNumericValue > FromTime Then
                                    ' Si se encuentra en los limites, acumulamos su valor
                                    TotalIncidence = TotalIncidence + Any2Time(Date.FromOADate(roConversions.Min(Incidence.TimeValue.VBNumericValue, ToTime - FromTime))).NumericValue
                                End If
                            End If
                        End If
                    Next

                    ' Calculamos el tiempo a justificar en función del intervalo definido
                    If TotalIncidence >= Any2Time("00:00").Add(JustifyPeriodStart, "n").NumericValue And TotalIncidence <= Any2Time("00:00").Add(JustifyPeriodEnd, "n").NumericValue Then
                        ' Si esta dentro del periodo se justifica todo
                        JustifyIncidences = True
                    ElseIf TotalIncidence > Any2Time("00:00").Add(JustifyPeriodEnd, "n").NumericValue Then
                        ' Si es mayor que el final del periodo
                        If JustifyAllPeriod Then
                            ' si se justifica el periodo, se justifica hasta el valor final del periodo
                            TotalIncidence = Any2Time("00:00").Add(JustifyPeriodEnd, "n").NumericValue
                            JustifyIncidences = True
                        Else
                            ' si no se justifica el periodo, no justificamos nada
                            JustifyIncidences = False
                        End If

                    ElseIf TotalIncidence < Any2Time("00:00").Add(JustifyPeriodStart, "n").NumericValue Then
                        ' Si es menor que el inicio del periodo no se justifica nada
                        JustifyIncidences = False
                    End If
                Else
                    ' Se justifica todo
                    JustifyIncidences = True
                    'TotalIncidence = 9999
                    TotalIncidence = Any2Time(Date.FromOADate(ToTime - FromTime)).NumericValue

                End If

                ' Si el total de la incidencia ya es mas grande que el valor máximo la ajustamos al valor maximo actual
                Dim dCauseMaxValue As Double = 0
                If dMaxTimeToForecast.TryGetValue(IDCause, dCauseMaxValue) Then
                    If dCauseMaxValue < TotalIncidence Then
                        TotalIncidence = dCauseMaxValue
                    End If
                End If

                If JustifyIncidences Then
                    ' Se justifican las incidencias hasta el valor indicado en TotalIncidence
                    For i As Integer = 1 To zIncidences.Count
                        Incidence = zIncidences.Item(i)
                        ' Comprueba el tipo de incidencia
                        If CauseType = roTimeBlockItem.roManualEntryCauseType.roInAbsence OrElse CauseType = roTimeBlockItem.roManualEntryCauseType.roOutAbsence Then
                            ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence)
                        Else
                            ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyOvertime)
                        End If

                        ' Solo justificamos la ausencia si no es diferencia negativa
                        If Incidence.BlockType <> roTimeBlockItem.roBlockType.roBTDailyUnderworking Then
                            'If mVTLiveInstalled Then
                            ' En el caso que sea Live las ausencias o extras en flexibles se justifican las ultimas
                            If Incidence.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleUnderworking Or Incidence.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleOverworking Then ValidIncidence = False
                            'End If

                            If ValidIncidence Then
                                ' Si el tipo es valido, comprueba que se encuentre en los limites tratados
                                If Incidence.Period.Begin.VBNumericValue < ToTime And Incidence.Period.Finish.VBNumericValue > FromTime Then
                                    ' Si se encuentra en los limites tratados y es del tipo tratado,
                                    '   hay que generar una justificación para dicha incidencia con el tiempo máximo entre el incio y final
                                    '   del motivo.

                                    Inci = New roTimeBlockItem
                                    Inci.Period.Begin = roConversions.Max(Incidence.Period.Begin, Any2Time(Date.FromOADate(FromTime)))
                                    Inci.Period.Finish = roConversions.Min(Incidence.Period.Finish, Any2Time(Date.FromOADate(ToTime)))

                                    MinTime = Any2Time(Date.FromOADate(roConversions.Min(Inci.TimeValue.VBNumericValue, ToTime - FromTime))).VBNumericValue

                                    MinTime = Any2Time(Date.FromOADate(roConversions.Min(Incidence.TimeValue.VBNumericValue, MinTime))).NumericValue(True)
                                    If TotalIncidence >= MinTime Then
                                        TotalTime = MinTime
                                        TotalIncidence = TotalIncidence - MinTime
                                    Else
                                        TotalTime = roConversions.Min(TotalIncidence, MinTime)
                                        TotalIncidence = 0
                                    End If

                                    If TotalTime > 0 Then
                                        'Justificamos la incidencia con la justificacion
                                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, IDCause, TotalTime, Incidence.Tag)

                                        ' Restamos el tiempo justificado del pendiente de justificar para esa incidencia
                                        Incidence.TimeValue = Any2Time(Incidence.TimeValue.NumericValue - TotalTime)

                                        ' Restamos el valor justificado del maximo restante
                                        If dMaxTimeToForecast.ContainsKey(IDCause) Then
                                            dMaxTimeToForecast.Item(IDCause) = dMaxTimeToForecast.Item(IDCause) - TotalTime
                                        End If
                                    End If

                                    If TotalIncidence = 0 Then Exit Sub

                                End If
                            End If
                        End If
                    Next

                    ' Si es Live o win32 justificamos al final las ausencias o extras en flexible
                    'If mVTLiveInstalled And TotalIncidence > 0 Then
                    If TotalIncidence > 0 Then
                        For i As Integer = 1 To zIncidences.Count
                            Incidence = zIncidences.Item(i)
                            ' Comprueba el tipo de incidencia
                            If CauseType = roTimeBlockItem.roManualEntryCauseType.roInAbsence Or CauseType = roTimeBlockItem.roManualEntryCauseType.roOutAbsence Then
                                ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAbsence)
                            Else
                                ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyOvertime)
                            End If

                            ' Solo justificamos la ausencia si es ausencia o extras en flexible
                            If Incidence.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleUnderworking Or Incidence.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleOverworking Then
                                If ValidIncidence Then
                                    ' Si el tipo es valido, comprueba que se encuentre en los limites tratados
                                    If Incidence.Period.Begin.VBNumericValue < ToTime And Incidence.Period.Finish.VBNumericValue > FromTime Then
                                        ' Si se encuentra en los limites tratados y es del tipo tratado,
                                        '   hay que generar una justificación para dicha incidencia con el tiempo máximo entre el incio y final
                                        '   del motivo.
                                        MinTime = Any2Time(Date.FromOADate(roConversions.Min(Incidence.TimeValue.VBNumericValue, ToTime - FromTime))).NumericValue

                                        If TotalIncidence >= MinTime Then
                                            TotalTime = MinTime
                                            TotalIncidence = TotalIncidence - MinTime
                                        Else
                                            TotalTime = roConversions.Min(TotalIncidence, MinTime)
                                            TotalIncidence = 0
                                        End If

                                        If TotalTime > 0 Then
                                            'Justificamos la incidencia con la justificacion
                                            Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, IDCause, TotalTime, Incidence.Tag)

                                            ' Restamos el tiempo justificado del pendiente de justificar para esa incidencia
                                            Incidence.TimeValue = Any2Time(Incidence.TimeValue.NumericValue - TotalTime)

                                            ' Restamos el valor justificado del maximo restante
                                            If dMaxTimeToForecast.ContainsKey(IDCause) Then
                                                dMaxTimeToForecast.Item(IDCause) = dMaxTimeToForecast.Item(IDCause) - TotalTime
                                            End If
                                        End If
                                        If TotalIncidence = 0 Then Exit Sub
                                    End If
                                End If
                            End If
                        Next
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager:: Execute_ApplyPunchedCausesLogic")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyPunchedCausesLogic")
            Finally

            End Try
        End Sub

        Private Function ExecutePendingSQL() As Boolean

            Dim z As Integer
            Dim bolret As Boolean = False

            oState.Result = EngineResultEnum.NoError

            Try

                ' Ejecutamos todas las instrucciones pendientes de actualziación de justificaciones diarias
                For z = 1 To mSQLExecute.Count
                    If Not ExecuteSql(mSQLExecute(z)) Then
                        Err.Raise(16313, "ExecutePendingSQL error :SQL Statement failed.")
                        Return False
                        Exit Function

                    End If
                Next z

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::ExecutePendingSQL")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::ExecutePendingSQL")
            Finally

            End Try

            Return bolret

        End Function

        Private Sub ManageAlertsWhenPA(ByVal IDEmployee As Long, ByVal roDate As roTime)
            '
            ' Eliminamos posibles alertas de empleados que deberían estar y no están
            '
            Dim sSQL As String

            Try

                '1.- Si el empleado deberían estar (alerta 1010) y se ha creado una ausencia de días para hoy, borro la alerta
                '    Borro las alertas que no apliquen (para el caso en que me han colocado una ausencia prolongada

                'Sólo si estamos en día en curso
                If Any2Time(Now).DateOnly = Any2Time(roDate).DateOnly Then
                    sSQL = "@DELETE# sysroNotificationTasks WHERE Id IN (" &
                            "@SELECT# sysroNotificationTasks.Id FROM sysroNotificationTasks,  ProgrammedAbsences " &
                            "WHERE sysroNotificationTasks.IDNotification = 1010 " &
                            "AND ProgrammedAbsences.IdEmployee = " & Any2String(IDEmployee) & " " &
                            "AND sysroNotificationTasks.Key1Numeric = " & Any2String(IDEmployee) & " " &
                            "AND ProgrammedAbsences.BeginDate <= " & Any2Time(roDate.DateOnly).SQLSmallDateTime & " " &
                            "AND (ISNULL(ProgrammedAbsences.FinishDate,DATEADD(day, ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate)) >= " & Any2Time(roDate.DateOnly).SQLSmallDateTime & "))"

                    If ExecuteSql(sSQL) Then
                        'roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCausesManager::ManageAlertsWhenPA: Alerts, about employees late arrival, deleted for employee " & Any2String(IDEmployee))
                        ' Actualizo la fecha de último borrado de alertas para el correcto refresco de pantallas
                        sSQL = "@UPDATE# Notifications SET LastTaskDeleted = " & Any2Time(Now).SQLDateTime & " WHERE ID = 1010"
                        If Not ExecuteSql(sSQL) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCausesManager::ManageAlertsWhenPA: Error updating LastTaskDeleted for IDNotification = 1010")
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::ExecutePendingSQL")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::ExecutePendingSQL")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyProductiveAbsencesCauses(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime)
            '
            ' Justificamos con el motivo de la Ausencia de trabajo externo, todas las incidencias de trabajo que sean necesarias
            '

            Dim Index As Long

            Dim MoveInIDCause As Long
            Dim MoveOutIDCause As Long
            Dim MoveInTime As Double
            Dim MoveOutTime As Double
            Dim OtherTime As Double
            Dim zMoves As roMoveList
            Dim i As Integer
            Dim lngProductiveAbsenceCause As Long

            Dim lMovesItems As New List(Of roMoveItem)

            Try

                ' Obtenemos los movimientos del dia
                zMoves = roBaseEngineManager.Execute_GetMovesLive(zEmployee, zTaskDate, oState)

                ' Eliminamos los que no estan completos
                Index = 0
                While Index <= zMoves.Moves.Count - 1
                    If Not zMoves.Moves(Index).Period.Begin.IsValid Or Not zMoves.Moves(Index).Period.Finish.IsValid Then
                        ' Eliminamos el movimiento
                        zMoves.Moves.RemoveAt(Index)
                    Else
                        ' Pasamos al siguiene movimiento
                        Index = Index + 1
                    End If
                End While

                Dim oNewMoveItem As roMoveItem
                For iMove As Integer = 0 To zMoves.Moves.Count - 1
                    oNewMoveItem = New roMoveItem
                    oNewMoveItem.InCause = zMoves.Moves(iMove).InCause
                    oNewMoveItem.OutCause = zMoves.Moves(iMove).OutCause
                    oNewMoveItem.Period.Begin.Value = zMoves.Moves(iMove).Period.Begin.Value
                    oNewMoveItem.Period.Finish.Value = zMoves.Moves(iMove).Period.Finish.Value
                    lMovesItems.Add(oNewMoveItem)
                Next

                For i = 1 To zExternalWorkCauses.Count
                    ' Para cada justificacion de trabajo externo
                    lngProductiveAbsenceCause = Any2Long(zExternalWorkCauses.Key(i))

                    ' Miramos los movimientos uno por uno
                    'For Each oMoveItem As roMoveItem In lMovesItems
                    Dim oMoveItem As roMoveItem
                    For Index = 0 To lMovesItems.Count - 1
                        oMoveItem = New roMoveItem
                        oMoveItem = lMovesItems.Item(Index)
                        ' Obtiene datos del movimiento
                        MoveInIDCause = Any2Long(oMoveItem.InCause)
                        MoveOutIDCause = Any2Long(oMoveItem.OutCause)
                        MoveInTime = Any2Double(oMoveItem.Period.Begin.Value)
                        MoveOutTime = Any2Double(oMoveItem.Period.Finish.Value)

                        ' Mira si la entrada tiene justificación productiva
                        If MoveInIDCause = lngProductiveAbsenceCause Then
                            '  V2 Si hay una entrada con justificacon de trabajo externo, justificamos todo el movimiento con dicha justificacion
                            Execute_ApplyProductiveAbsencesCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveInIDCause, MoveInTime, MoveOutTime)
                        End If

                        '  V2 Mira si la salida tiene justificación productiva
                        If MoveOutIDCause = lngProductiveAbsenceCause Then
                            ' Obtiene inicio del siguiente movimiento (si hay)
                            If Index = (lMovesItems.Count - 1) Then OtherTime = 99999 Else OtherTime = Any2Double(lMovesItems.Item(Index + 1).Period.Begin.Value)

                            If OtherTime <> 99999 Then
                                ' Solo justiifcamos tramos intermedios
                                ' Justifica todas las horas de trabajo entre esta salida y la proxima entrada
                                Execute_ApplyProductiveAbsencesCausesLogic(zCauses, zIncidences, zEmployee, zTaskDate, MoveOutIDCause, MoveOutTime, OtherTime)
                            End If
                        End If
                    Next
                Next i
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProductiveAbsencesCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProductiveAbsencesCauses")
            Finally

            End Try

        End Sub

        Private Sub Execute_ApplyProductiveAbsencesCausesLogic(ByRef zCauses As DataTable, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef zTaskDate As roTime, ByVal IDCause As Long, ByVal FromTime As Double, ByVal ToTime As Double)
            '
            ' Justifica incidencias de trabajo entre el periodo indicado con la justificacion de Ausencia productiva
            '
            Dim Incidence As roTimeBlockItem

            Dim Inci As roTimeBlockItem

            Dim ValidIncidence As Boolean
            Dim MinTime As Double
            Dim TotalIncidence As Double

            Dim TotalTime As Double

            Try

                ' Se justifica todo el tramo
                TotalIncidence = Any2Time(Date.FromOADate(ToTime - FromTime)).NumericValue

                ' Se justifican las incidencias hasta el valor indicado en TotalIncidence
                For index As Integer = 1 To zIncidences.Count
                    Incidence = zIncidences.Item(index)
                    ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAttendance)

                    If Incidence.BlockType <> roTimeBlockItem.roBlockType.roBTFlexibleOverworking Then
                        If ValidIncidence Then
                            ' Si el tipo es valido, comprueba que se encuentre en los limites tratados
                            If Incidence.Period.Begin.VBNumericValue < ToTime And Incidence.Period.Finish.VBNumericValue > FromTime Then
                                ' Si se encuentra en los limites tratados y es del tipo tratado,
                                '   hay que generar una justificación para dicha incidencia con el tiempo máximo entre el incio y final
                                '   del motivo.

                                Inci = New roTimeBlockItem
                                Inci.Period.Begin = roConversions.Max(Incidence.Period.Begin, Any2Time(Date.FromOADate(FromTime)))
                                Inci.Period.Finish = roConversions.Min(Incidence.Period.Finish, Any2Time(Date.FromOADate(ToTime)))

                                MinTime = Any2Time(Date.FromOADate(roConversions.Min(Inci.TimeValue.VBNumericValue, ToTime - FromTime))).VBNumericValue

                                MinTime = Any2Time(Date.FromOADate(roConversions.Min(Incidence.TimeValue.VBNumericValue, MinTime))).NumericValue

                                If TotalIncidence >= MinTime Then
                                    TotalTime = MinTime
                                    TotalIncidence = TotalIncidence - MinTime
                                Else
                                    TotalTime = roConversions.Min(TotalIncidence, MinTime)
                                    TotalIncidence = 0
                                End If

                                If TotalTime > 0 Then
                                    'Justificamos la incidencia con la justificacion
                                    Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, IDCause, TotalTime, Incidence.Tag)

                                    ' Restamos el tiempo justificado del pendiente de justificar para esa incidencia
                                    Incidence.TimeValue = Any2Time(Incidence.TimeValue.NumericValue - TotalTime)
                                End If

                                If TotalIncidence = 0 Then Exit Sub

                            End If
                        End If
                    End If
                Next

                If TotalIncidence > 0 Then
                    For index As Integer = 1 To zIncidences.Count
                        Incidence = zIncidences.Item(index)
                        ' Comprueba el tipo de incidencia
                        ValidIncidence = Incidence.ValidatesFilter(roTimeBlockItem.roBlockType.roBTAnyAttendance)

                        ' Solo justificamos extras en flexible
                        If Incidence.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleOverworking Then
                            If ValidIncidence Then
                                ' Si el tipo es valido, comprueba que se encuentre en los limites tratados
                                If Incidence.Period.Begin.VBNumericValue < ToTime And Incidence.Period.Finish.VBNumericValue > FromTime Then
                                    ' Si se encuentra en los limites tratados y es del tipo tratado,
                                    '   hay que generar una justificación para dicha incidencia con el tiempo máximo entre el incio y final
                                    '   del motivo.
                                    MinTime = Any2Time(Date.FromOADate(roConversions.Min(Incidence.TimeValue.VBNumericValue, ToTime - FromTime))).NumericValue

                                    If TotalIncidence >= MinTime Then
                                        TotalTime = MinTime
                                        TotalIncidence = TotalIncidence - MinTime
                                    Else
                                        TotalTime = roConversions.Min(TotalIncidence, MinTime)
                                        TotalIncidence = 0
                                    End If

                                    If TotalTime > 0 Then
                                        'Justificamos la incidencia con la justificacion
                                        Execute_AddCause(zCauses, zEmployee, zTaskDate.DateOnly, IDCause, TotalTime, Incidence.Tag)

                                        ' Restamos el tiempo justificado del pendiente de justificar para esa incidencia
                                        Incidence.TimeValue = Any2Time(Incidence.TimeValue.NumericValue - TotalTime)
                                    End If

                                    If TotalIncidence = 0 Then Exit Sub
                                End If
                            End If
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProductiveAbsencesCausesLogic")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyProductiveAbsencesCausesLogic")
            Finally

            End Try

        End Sub

        Private Function Execute_ApplyHolidaysRules(ByRef zCauses As DataTable, ByRef zShift As roShiftEngine, ByRef zIncidences As roTimeBlockList, ByVal zEmployee As Long, ByRef TaskDate As roTime) As Boolean
            '
            ' Aplica justificación del horario de vacaciones
            '

            Dim ExpectedworkingHours As roTime
            Dim IDShiftBase As Double
            Dim IDCauseHolidays As Long
            Dim ExpectedWorkingHoursDailySchedule As Double
            Dim DirectValue As Double
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                If Not zShift Is Nothing Then

                    ' Si el dia esta marcado como Vacaciones
                    If Any2Boolean(ExecuteScalar("@SELECT# isnull(IsHolidays, 0) FROM DailySchedule with (nolock)  WHERE IDEmployee=" &
                        zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime)) Then

                        IDShiftBase = Any2Double(ExecuteScalar("@SELECT# isnull(IDShiftBase, 0) FROM DailySchedule with (nolock)  WHERE IDEmployee=" &
                        zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime))

                        ' Obtenemos las horas teoricas del horario base
                        ExpectedWorkingHoursDailySchedule = Any2Double(ExecuteScalar("@SELECT# ISNULL(ExpectedWorkingHours,-3) FROM DailySchedule with (nolock)  WHERE IDEmployee=" &
                        zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime))

                        If ExpectedWorkingHoursDailySchedule = -3 Then
                            ExpectedworkingHours = Any2Time(ExecuteScalar("@SELECT# isnull(ExpectedWorkingHours, 0) FROM Shifts with (nolock)  WHERE ID=" &
                                    IDShiftBase))
                        Else
                            ExpectedworkingHours = Any2Time(ExpectedWorkingHoursDailySchedule)
                        End If

                        ExpectedworkingHours.Value = ExpectedworkingHours.TimeOnly

                        ' Obtenemos la justificacion a aplicar
                        IDCauseHolidays = Any2Long(ExecuteScalar("@SELECT# isnull(IDCauseHolidays, 0) FROM Shifts WHERE ID=" &
                                                zShift.ID))

                        ' En funcion del tipo de valor a aplicar, asignamos las horas teoricas o un valor directo
                        If Any2Double(ExecuteScalar("@SELECT# isnull(TypeHolidayValue, 0) FROM Shifts WHERE ID=" & zShift.ID)) = 0 Then
                            ' Añadimos las horas teoricas del horario
                            Execute_AddCause(zCauses, zEmployee, TaskDate.Value, IDCauseHolidays, ExpectedworkingHours.NumericValue, 0, True)
                        Else
                            ' Añadimos un valor directo
                            DirectValue = Any2Double(ExecuteScalar("@SELECT# isnull(HolidayValue, 0) FROM Shifts WHERE ID=" & zShift.ID))
                            Execute_AddCause(zCauses, zEmployee, TaskDate.Value, IDCauseHolidays, DirectValue, 0, True)
                        End If

                    End If

                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyHolidaysRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyHolidaysRules")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_IsEmployeePresent(ByRef Incidences As roTimeBlockList) As Boolean
            '
            ' Devuelve True si el empleado tiene fichajes de presencia (es decir, no tiene unicamente
            '  una ausencia)
            '
            Dim bolRet As Boolean = False
            Dim bCloseProgrammedAbsence As Boolean

            Try

                For Index = 1 To Incidences.Count
                    If Incidences.Item(Index).IsWorkingTime Then
                        bolRet = True

                        ' Comprobamos si la justificacion permite el cierre automatico de la ausencia prolongada
                        bCloseProgrammedAbsence = Any2Boolean(ExecuteScalar("@SELECT# PunchCloseProgrammedAbsence FROM Causes where id=" & mProgrammedAbsence.ActiveCause.ToString))
                        If Not bCloseProgrammedAbsence Then bolRet = False
                        Return bolRet
                        Exit Function
                    End If
                Next
                bolRet = False
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_IsEmployeePresent")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_IsEmployeePresent")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function Execute_ApplyDefaultCauses(ByRef Incidences As roTimeBlockList, ByRef Causes As DataTable, ByVal IDEmployee As Long, ByVal DateTime As Date) As Boolean
            '
            ' Crea justificaciones por defecto para los tiempos que no se han procesado con reglas
            '  (asigna la justificación "No especificado" a las incidencias, y "Horas trabajadas" a las
            '  horas trabajadas).
            '
            Dim bolRet As Boolean = False

            oState.Result = EngineResultEnum.NoError

            Try

                For index As Integer = 1 To Incidences.Count
                    If Incidences.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTWorking Then
                        ' Las horas normales se codifican como trabajadas
                        Execute_AddCause(Causes, IDEmployee, DateTime, CAUSE_WORKING_DEFAULT, DateTime2Double(Incidences.Item(index).TimeValue.Value, True), Incidences.Item(index).Tag)
                    ElseIf Incidences.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTBreak Then
                        ' Los descansos se codifican como descansos
                        Execute_AddCause(Causes, IDEmployee, DateTime, CAUSE_BREAK, DateTime2Double(Incidences.Item(index).TimeValue.Value, True), Incidences.Item(index).Tag)
                    Else
                        ' Incidencia sin justificar
                        Execute_AddCause(Causes, IDEmployee, DateTime, CAUSE_INCIDENCE_DEFAULT, DateTime2Double(Incidences.Item(index).TimeValue.Value, True), Incidences.Item(index).Tag)
                    End If
                Next

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyDefaultCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyDefaultCauses")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub Execute_ApplyDefaultCenters(ByVal zEmployee As Long, ByRef TaskDate As roTime)
            '
            ' Aplica centro de coste por defecto a las justificaciones diarias
            '

            Dim IDCenter As Double
            Dim IDRelatedIncidence As Long
            Dim DefaultCenter As Double
            Dim IDShiftCenter As Double
            Dim bolApplyonAbsences As Boolean
            Dim IsDefaultCenter As Double
            Dim sSQL As String

            Try

                ' Obtenemos las incidencias diarias
                Dim tb As DataTable = Nothing
                sSQL = "@SELECT# ISNULL(IDCenter, 0) AS IDCenter,ID, ISNULL(DefaultCenter,1) AS DefaultCenter FROM DailyIncidences WHERE IDEmployee=" & zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime & " ORDER BY EndTime"
                tb = CreateDataTable(sSQL)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        ' Obtenemos el centro y el ID
                        IDCenter = Any2Double(oRow("IDCenter"))
                        IDRelatedIncidence = Any2Long(oRow("ID"))
                        DefaultCenter = If(Any2Boolean(oRow("DefaultCenter")), 1, 0)

                        ' Actualizamos el centro de coste de las justificaciones diarias relaciondas que no se hayan modificado manualmente
                        ExecuteSql("@UPDATE# DailyCauses Set IDCenter=" & IDCenter & ", DefaultCenter=" & DefaultCenter & " WHERE IDEmployee=" & zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime & " AND IDRelatedIncidence=" & IDRelatedIncidence & " AND ISNULL(ManualCenter,0) =0")
                    Next
                End If

                ' Actualizamos las justificaciones de pluses o de vacaciones, no relacionadas con ninguna incidencia con el centro del horario o el que tenga asignado por defecto
                'DefaultCenter = BusinessCenter.roBusinessCenter.GetEmployeeDefaultBusinessCenter(New BusinessCenter.roBusinessCenterState(oState.IDPassport), zEmployee, Any2DateTime(TaskDate).Date, oCn.Transaction)
                DefaultCenter = roBaseEngineManager.GetDefaultCenter(zEmployee, TaskDate, oState)
                roBaseEngineManager.GetShiftCenter(IDShiftCenter, zEmployee, TaskDate, bolApplyonAbsences, oState, bolProgrammedAbsenceOnHolidays)

                IsDefaultCenter = 1
                If IDShiftCenter <> 0 Then
                    If DefaultCenter <> IDShiftCenter Then
                        IsDefaultCenter = 0
                    End If
                End If

                If IDShiftCenter <> 0 Then
                    DefaultCenter = IDShiftCenter
                End If

                Try
                    sSQL = "@UPDATE# DailyCauses Set IDCenter=" & DefaultCenter & ", DefaultCenter=" & IsDefaultCenter & "  WHERE IDEmployee=" & zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime & " AND IDRelatedIncidence=0 AND ISNULL(ManualCenter,0)=0 AND AccrualsRules=0 AND Value <> 0"
                    ExecuteSql(sSQL)
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roCausesManager::Execute_ApplyDefaultCenters: Unable to set defaultcenter.Employee: " & zEmployee & " " & TaskDate.Value)
                End Try

                If mLastPunchCenterEnabled = "1" Then
                    ' En el caso que se deba mirar el último fichaje de centro de coste
                    ' debemos actualizar las justificaciones generadas por vacaciones con dicho centro de coste
                    sSQL = "@SELECT# TOP 1 * FROM Punches WHERE " _
                        & " IDEmployee=" & zEmployee & " AND " _
                        & " DateTime <" & TaskDate.SQLDateTime & " AND " _
                        & " ActualType=13 ORDER BY ShiftDate desc, DateTime desc, ID desc"

                    tb = Nothing
                    tb = CreateDataTable(sSQL)
                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        IsDefaultCenter = 1
                        If DefaultCenter <> Any2Double(tb.Rows(0)("TypeData")) Then
                            IsDefaultCenter = 0
                        End If
                        ExecuteSql("@UPDATE# DailyCauses Set IDCenter=" & Any2Double(tb.Rows(0)("TypeData")) & ", DefaultCenter=" & IsDefaultCenter & "  WHERE IDEmployee=" & zEmployee & " AND Date=" & TaskDate.SQLSmallDateTime & " AND IDRelatedIncidence=0 AND ISNULL(ManualCenter,0)=0 AND AccrualsRules=0 AND DailyRule=0 AND Manual=0 AND AccruedRule=0")
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyDefaultCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::Execute_ApplyDefaultCenters")
            Finally

            End Try

        End Sub

        Public Function DailyScheduleGUIDChanged() As Boolean
            '
            ' Verificamos si el GUID del dia ha sido modificado posteriormente al iniciar el proceso de cálculo
            '

            Try
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(mIDEmployee, mCurrentDate, mPreviousProcessPriority, oState)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCausesManager::DailyScheduleGUIDChanged")
            End Try

            Return mGUIDChanged
        End Function

#End Region

    End Class

    Public Class roProgrammedAbsenceManager
        Private oState As roEngineState = Nothing
        Private PA_BeginDate As roTime          ' Fecha de inicio de la A.P. (si hay)
        Private PA_FinishDate As roTime         ' Fecha final de la A.P. en memoria (si hay y tiene)
        Private PA_MaxLastingDays As Integer    ' Máximo de dias de A.P. (si hay)
        Private PA_IDCause As Integer           ' Justificación de la A.P. (si hay)
        Private PA_IDEmployee As Long           ' Empleado en memoria
        Private PA_CurrentDate As New roTime    ' Fecha en memoria
        Private mCausesEnginePriority As Integer
        Private daPA As DbDataAdapter

        ' Flags
        Private bInitialized As Boolean
        Private bAnalyzed As Boolean

        Public ReadOnly Property State As roEngineState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roEngineState()
        End Sub

        Public Sub New(ByVal _State As roEngineState)
            oState = _State
        End Sub

        Public Function ActiveCause() As Integer
            '
            ' Devuelve el IDCause de la ausencia prolongada actual
            '
            Return PA_IDCause

        End Function

        Private Function Analyze_AbsenceIsActive(ByRef RS As DataRow, ByRef ProcessedDate As roTime) As Boolean
            '
            ' Devuelve True si la ausencia analizada esta activa en la fecha procesada
            '
            Dim LastPossibleDay As Double
            Dim bolret As Boolean = False

            Try
                If IsDBNull(RS("FinishDate")) Then
                    ' Si la fecha final es null, comprueba que la ausencia afecte a la
                    '  fecha procesada.
                    LastPossibleDay = Any2Double(DateAdd("d", RS("MaxLastingDays") - 1, RS("BeginDate")))
                    If LastPossibleDay < ProcessedDate.VBNumericValue Then
                        bolret = False
                    Else
                        bolret = True
                    End If
                Else
                    bolret = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_AbsenceIsActive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_AbsenceIsActive")
            Finally

            End Try

            Return bolret

        End Function

        Private Sub Analyze_CloseOverlappedAbsence(ByRef RS As DataRow, ByRef CloseDate As roTime)
            '
            ' Cierra o elimina una ausencia prolongada que se solapa con otra en la fecha actual
            '  Devuelve la nueva fecha de inicio a usar para las comparaciones.
            '

            Dim NewFinishDate As roTime
            Dim OverlappedBeginDate As roTime
            Dim OverlappedFinishDate As roTime

            Try

                ' Obtiene fecha de incio de la ausencia que se solapa
                OverlappedBeginDate = Any2Time(RS("BeginDate"))

                ' Calcula fecha de cierre nueva
                NewFinishDate = CloseDate.Substract(1, "d")

                ' Calcula fecha de cierre que tenia antiguamente
                If IsDate(RS("FinishDate")) Then
                    OverlappedFinishDate = Any2Time(RS("FinishDate"))
                Else
                    OverlappedFinishDate = OverlappedBeginDate.Add(RS("MaxLastingDays") - 1, "d")
                End If

                ' Borramos las justificaciones justificadas como la ausencia prolongada
                ExecuteSql("@DELETE# FROM DailyCauses WHERE IDEmployee=" & PA_IDEmployee.ToString &
                    " AND IDCause=" & RS("IDCause") &
                    " AND Date BETWEEN " & CloseDate.SQLSmallDateTime &
                    " AND " & OverlappedFinishDate.SQLSmallDateTime)

                If NewFinishDate.VBNumericValue < OverlappedBeginDate.VBNumericValue Then
                    ' Si la fecha de cierre es anterior a la de inicio, elimina la ausencia
                    RS.Delete()
                Else
                    ' Cierra la ausencia en la fecha máxima de cierre
                    RS("FinishDate") = NewFinishDate.DateOnly
                    RS("MaxLastingDays") = DateDiff("d", OverlappedBeginDate.DateOnly, NewFinishDate.DateOnly) + 1
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_CloseOverlappedAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_CloseOverlappedAbsence")
            Finally

            End Try

        End Sub

        Private Sub Analyze_RetrieveRecord(ByRef RS As DataRow)
            '
            ' Obtiene datos de la ausencia activa en memoria
            '
            Try

                PA_BeginDate = Any2Time(RS("BeginDate"))
                If Not IsDBNull(RS("FinishDate")) Then
                    PA_FinishDate = Any2Time(RS("FinishDate"))
                Else
                    PA_FinishDate = Nothing
                End If
                PA_MaxLastingDays = RS("MaxLastingDays")
                PA_IDCause = RS("IDCause")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_RetrieveRecord")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze_RetrieveRecord")
            Finally

            End Try

        End Sub

        Private Sub ReprocessChangedDays(ByRef StartDate As roTime, ByRef FinishDate As roTime, ByVal MaxLastingDays As Integer, ByVal DeleteOldCauses As Boolean, ByVal OldCauseID As Integer)
            '
            ' Crea tarea para reprocesar las fechas que antes estaban en una ausencia prolongada y ahora
            '  ya no porque la hemos cerrado.
            '
            Dim CurrentDate As New roTime

            Try

                ' Obtiene fecha final a reprocesar
                If FinishDate Is Nothing Then FinishDate = StartDate.Add(MaxLastingDays - 1, "d")

                ' Actualiza Status en todas las fechas necesarias
                ExecuteSql("@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=" & Any2String(mCausesEnginePriority - 1) & " WHERE IDEmployee=" & PA_IDEmployee.ToString &
                            " AND Date BETWEEN " & StartDate.SQLSmallDateTime &
                                " AND " & FinishDate.SQLSmallDateTime & " and Date <= getdate()")

                ' Borramos las justificaciones justificadas como la ausencia prolongada si se ha activado la opción
                If DeleteOldCauses Then
                    ExecuteSql("@DELETE# FROM DailyCauses WHERE IDEmployee=" & PA_IDEmployee &
                            " AND IDCause=" & OldCauseID &
                                " AND Date BETWEEN " & StartDate.SQLSmallDateTime &
                                    " AND " & FinishDate.SQLSmallDateTime)
                End If

                '' TODO Creamos tarea mediante trigger
                'mConn.Context(roVarDataOp) = roTableObject & ":\\PROGRAMMEDABSENCES"
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::ReprocessChangedDays")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::ReprocessChangedDays")
            Finally

            End Try

        End Sub

        Public Function HasActiveProgrammedAbsence() As Boolean
            '
            ' Devuelve True si el empleado tiene una ausencia prolongada abierta en esta fecha
            '
            Try

                If Not bAnalyzed Then Err.Raise(7344, "roProgrammedAbsenceManager", "Must call Analyze first.")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::HasActiveProgrammedAbsence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::HasActiveProgrammedAbsence")
            Finally

            End Try

            Return (PA_IDCause <> 0)

        End Function

        Public Sub CloseProgrammedAbsenceIfNeeded()
            '
            ' Cierra una ausencia programada si es necesario.
            '  Si se llama a esta función, significa que hay fichajes en la fecha analizada
            '  y que debe cerrarse la aus.programada si esta no esta cerrada.
            '
            Dim SQLWhere As String
            Dim FinishDate As New roTime

            If Not bAnalyzed Then Err.Raise(7344, "roProgrammedAbsenceManager", "Must call Analyze first.")

            Try

                ' Si no hay ausencia prolongada no hace nada
                If PA_IDCause = 0 Then Exit Sub

                ' Calcula clausula WHERE
                SQLWhere = " WHERE IDEmployee=" & PA_IDEmployee & " And IDCause =" & PA_IDCause & " And BeginDate=" & PA_BeginDate.SQLSmallDateTime

                If PA_CurrentDate.VBNumericValue <= PA_BeginDate.VBNumericValue Then
                    ' La fecha de inicio es la que ya debe estar cerrada, eliminamos la ausencia prolongada
                    ExecuteSql("@DELETE# FROM ProgrammedAbsences" & SQLWhere)
                Else
                    ' Obtenemos la fecha de finalización actual
                    If Not PA_FinishDate Is Nothing Then
                        FinishDate = PA_FinishDate
                    Else
                        FinishDate = Any2Time(DateAdd("d", PA_MaxLastingDays - 1, PA_BeginDate.Value))
                    End If

                    ' Cerramos la ausencia prolongada
                    ExecuteSql("@UPDATE#  ProgrammedAbsences SET FinishDate=" & PA_CurrentDate.Substract(1, "d").SQLSmallDateTime & SQLWhere)

                    ' Marcamos la ausencia como cerrada de forma automatica por un fichaje (esto se utiliza para el notificador)
                    ExecuteSql("@UPDATE# ProgrammedAbsences SET AutomaticClosed=1 " & SQLWhere)

                End If

                ' Forzamos recalculo de fechas que ya no están dentro de la ausencia prolongada y antes si
                ReprocessChangedDays(PA_CurrentDate.Add(1, "d"), PA_FinishDate, PA_MaxLastingDays, True, PA_IDCause)

                ' Actualizamos datos en memoria
                PA_IDCause = 0
                PA_BeginDate = Nothing
                PA_FinishDate = Nothing
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::CloseProgrammedAbsenceIfNeeded")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::CloseProgrammedAbsenceIfNeeded")
            Finally

            End Try

        End Sub

        Public Function Analyze(ByVal IDEmployee As Long, ByRef ProcessedDate As roTime) As Boolean
            '
            '   Inicializa gestor de ausencias programadas para un empleado
            '
            '   Devuelve True si no ha habido ningún error, False si hay algun error.
            '
            Dim LastBeginDate As roTime
            Dim CandidateBeginDate As roTime

            Try

                oState.Result = EngineResultEnum.NoError

                If Not bInitialized Then Err.Raise(7342, "roProgrammedAbsenceManager", "Must call Init first.")

                ' Inicializa datos
                PA_BeginDate = Nothing
                PA_FinishDate = Nothing
                PA_IDCause = 0
                PA_IDEmployee = IDEmployee

                PA_MaxLastingDays = 0
                LastBeginDate = Nothing

                ' Prepara select
                Dim SQL As String = "@SELECT#  * FROM ProgrammedAbsences WHERE IDEmployee=" & IDEmployee.ToString &
                    " AND BeginDate<=" & ProcessedDate.SQLSmallDateTime &
                        " AND (ISNULL(FinishDate," & ProcessedDate.SQLSmallDateTime & "))>=" & ProcessedDate.SQLSmallDateTime &
                            " ORDER BY BeginDate DESC"

                Dim cmd As DbCommand = CreateCommand(SQL)
                Dim dTbl As New DataTable("ProgrammedAbsences")
                daPA = CreateDataAdapter(cmd, True)
                daPA.Fill(dTbl)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In dTbl.Rows
                        If Analyze_AbsenceIsActive(oRowEmp, ProcessedDate) Then
                            If LastBeginDate Is Nothing Then
                                ' Si es la última ausencia activa para esta fecha, guarda datos
                                Analyze_RetrieveRecord(oRowEmp)
                                LastBeginDate = PA_BeginDate
                            Else
                                ' Si es una anterior a la última y también esta activada, la cierra
                                '  un dia antes del inicio de la última o la elimina si queda con 0 dias
                                CandidateBeginDate = Any2Time(oRowEmp("BeginDate"))
                                Analyze_CloseOverlappedAbsence(oRowEmp, LastBeginDate)
                                ' Calcula de nuevo la fecha para comparar si hay solapamientos
                                LastBeginDate = roConversions.Min(CandidateBeginDate, LastBeginDate)
                            End If
                        End If
                    Next
                    daPA.Update(dTbl)
                End If

                ' Valida funcion
                bAnalyzed = True
                PA_CurrentDate = ProcessedDate
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Analyze")
            Finally

            End Try

            Return bAnalyzed

        End Function

        Public Sub Init(ByVal EnginePriority As Integer)
            '
            ' Pasa conector y connection a este objeto
            '
            Try
                bInitialized = True
                mCausesEnginePriority = EnginePriority
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Init")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Init")
            Finally

            End Try

        End Sub

        Public Sub Close()
            '
            ' Pasa conector y connection a este objeto
            '
            Try
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Close")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::Close")
            Finally

            End Try

        End Sub

        Public Sub OpenProgrammedAbsenceForNextDay(ByVal IDCause As Integer, ByVal MaxLastingDays As Integer)
            '
            ' Abre una ausencia prolongada el dia siguiente al procesado porque se ha fichado una
            '  justificacion que asi lo requiere.
            '
            Dim SQL As String

            If Not bAnalyzed Then Err.Raise(7344, "roProgrammedAbsenceManager", "Must call Analyze first.")
            Try
                ' Si ya hay una ausencia prolongada que empieza ese dia no hace nada, ignora
                SQL = "@SELECT# ISNULL(IDCause, -3) FROM ProgrammedAbsences WHERE IDEmployee=" & PA_IDEmployee.ToString &
                        " AND BeginDate=" & PA_CurrentDate.Add(1, "d").SQLSmallDateTime
                If Any2Double(ExecuteScalar(SQL)) <> -3 Then Exit Sub

                ' En caso contrario, creamos una ausencia prolongada que empiece mañana
                SQL = "@INSERT# INTO ProgrammedAbsences (IDEmployee,BeginDate,IDCause,MaxLastingDays) VALUES " &
                        "(" & PA_IDEmployee.ToString & "," & PA_CurrentDate.Add(1, "d").SQLSmallDateTime & "," &
                            IDCause.ToString & "," & MaxLastingDays.ToString & ")"
                If Not ExecuteSql(SQL) Then Err.Raise(16313, "SQLExecute", "SQL Statement failed.")

                ' Ahora creamos una tarea para que se procesen las fechas que entran dentro de la nueva
                '  ausencia prolongada.
                ReprocessChangedDays(PA_CurrentDate.Add(1, "d"), Nothing, MaxLastingDays, False, 0)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::OpenProgrammedAbsenceForNextDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedAbsenceManager::OpenProgrammedAbsenceForNextDay")
            Finally

            End Try
        End Sub

#End Region

    End Class

End Namespace