Imports System.Data.Common
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTDailyRecord
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTLabAgrees.LabAgree
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTShiftEngines

    Public Class roAccrualsManager
        Public Const mPriority = 70
        Public Const mPreviousProcessPriority = 60
        Public Const CONCEPT_SHIFTEXPECTEDHOURS = 4
        Public Const roMaxTimeNumericValue = 999999999999.0#
        Public Const roNullDate = "1/1/2079"

        Private mTask As roLiveTask
        Private mIDEmployee As Integer = 0
        Public mCurrentDate As Date
        Private oState As roEngineState = Nothing
        Private mInitialRegisterStatus As Integer = 0
        Private oConceptsCache As New Hashtable
        Private dConceptsValues As New Dictionary(Of Integer, ConceptValues)
        Private mTaskID As String = ""
        Private oLicense As New roServerLicense
        Private mEmployeeRules As New roCollection  'Reglas definidas para el usuario en cuestión
        Private bolApplyCarryOverOnSameDate As Boolean
        Private dblMaximumValues As Integer
        Private dblMinimumValues As Integer
        Private dblCauseLimits As Integer
        Private bolAutomaticAccruals As Boolean
        Private mCostCenterInstallled As Boolean = False
        Private bolIsHolidays As Boolean
        Private bolProgrammedAbsenceOnHolidays As Boolean
        Private bolAutomaticEquivalence As Boolean
        Private AutomaticEquivalenceCausesDB As DataTable
        Private mEmployeeCauseLimits As New roCollection
        Private intMonthIniDay As Integer
        Private intYearIniMonth As Integer
        Private intWeekIniday As Integer
        Private bolRigidBreakRule As Boolean = False
        Private RigidBreakRule_IDCauseDES As Double
        Private RigidBreakRule_IDConcept As Double
        Private mCreateConceptsRules As New roCollection

        ' Flag para saber si se tiene que volver a lanzar el proceso para fechas de caducidad de valores
        Private mMarkNextExpiredDate As Boolean

        ' Obtener el valor de una justificacion generada por otra regla
        Private bolGetValueAccrualRuleNextDay As Boolean

        Private mVerifyProgrammedCauseOnMandatory As String

        ' Comportamiento de ausencias productivas
        Private bolProductiveAbsences As Boolean

        Private dblProductiveAbsenceCause As Double
        Private bolApplyGratificationRule As Boolean = False
        Private mGUIDChanged As Boolean = False
        Private intIDConcept_DailyRecordwithAutomaticApproval As Integer = -1
        Private intIDPassportSystem As Integer = -1
        Private bolAnnualWorkConceptsDefined As Boolean = False
        Private bolSetAnnualWorkPeriod As Boolean = False
        Private strCustomization As String = ""
        Private bolLatam_OvertimeManagement As Boolean

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

        Private Function LoadConfig() As Boolean

            Try
                oState.Result = EngineResultEnum.NoError
                mCostCenterInstallled = oLicense.FeatureIsInstalled("Feature\CostControl")

                ' Aseguramos que el caché de saldos está cargado
                Dim dtConcepts As DataTable = Nothing
                dtConcepts = roConcept.GetConceptListLite(New roConceptState(oState.IDPassport))
                If dtConcepts IsNot Nothing AndAlso dtConcepts.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtConcepts.Rows
                        roBaseEngineManager.GetConceptFromCache(oRow("ID"), oState)
                        ' Inicializamos valores de saldos para esta ejecución del hilo
                        dConceptsValues.Add(oRow("ID"), New ConceptValues(0, 0, 0))
                    Next
                End If

                oConceptsCache = roBaseEngineManager.ConceptsCache(oState)


                dblMaximumValues = Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM StartUpValues WHERE LEN(MaximumValue) > 0 AND MaximumValue is Not Null"))

                ' Revisamos si tenemos que comprobar valores minimos de saldos
                dblMinimumValues = Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM StartUpValues WHERE LEN(MinimumValue) > 0 AND MinimumValue is Not Null"))

                ' Revisamos si tenemos que comprobar los máximos por justificación
                dblCauseLimits = Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM CauseLimitValues "))

                ' Revisamos si tenemos que comprobar los devengos automaticos por justificación
                bolAutomaticAccruals = False
                For Each oConcept As roConceptEngine In oConceptsCache.Values
                    If oConcept.AutomaticAccrualType > eAutomaticAccrualType.DeactivatedType Then
                        bolAutomaticAccruals = True
                        Exit For
                    End If
                Next

                ' Revisamos si tenemos que comprobar las aprobaciones automaticas de solicitudes de declaracion de la jornada
                intIDConcept_DailyRecordwithAutomaticApproval = -1
                For Each oConcept As roConceptEngine In oConceptsCache.Values
                    If oConcept.AutoApproveRequestsDR Then
                        intIDConcept_DailyRecordwithAutomaticApproval = oConcept.ID
                        Exit For
                    End If
                Next

                ' Obtenemos passport de sistema
                intIDPassportSystem = roTypes.Any2Integer(roConstants.GetSystemUserId())

                bolAutomaticEquivalence = False
                If Any2Double(ExecuteScalar("@SELECT# COUNT(*) FROM Causes WHERE AutomaticEquivalenceIDCause > 0 ")) > 0 Then
                    bolAutomaticEquivalence = True
                    AutomaticEquivalenceCausesDB = GetAutomaticEquivalenceCauses()
                End If

                ' Mes de inicio de año, día de inicio de mes, día de inicio de la semana
                Dim oParams As New roParameters("OPTIONS", True)
                intMonthIniDay = oParams.Parameter(Parameters.MonthPeriod)
                If intMonthIniDay = 0 Then intMonthIniDay = 1
                intYearIniMonth = oParams.Parameter(Parameters.YearPeriod)
                If intYearIniMonth = 0 Then intYearIniMonth = 1
                intWeekIniday = oParams.Parameter(Parameters.WeekPeriod)

                bolGetValueAccrualRuleNextDay = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "GetValueAccrualRuleNextDay"))

                ' Comprueba si tenemos que aplicar regla de descanso rigido
                bolRigidBreakRule = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.RigidBreakRule"))
                If bolRigidBreakRule Then
                    ' Obtenemos la justificacion de descanso
                    Dim strSQL As String = "@SELECT# ID FROM Causes WHERE Description like '%#DESCC#%'"
                    RigidBreakRule_IDCauseDES = Any2Double(ExecuteScalar(strSQL))
                    If RigidBreakRule_IDCauseDES <= 0 Then RigidBreakRule_IDCauseDES = -1


                    ' Obtenemos las horas trabajadas reales del dia
                    strSQL = "@SELECT# ID FROM Concepts WHERE Description LIKE '%#HTRR#%'"
                    RigidBreakRule_IDConcept = Any2Double(ExecuteScalar(strSQL))

                End If

                ' Comprobar si existe ausencia por horas en franja rigida con inicio semiflexible
                ' para calcular el inicio de la misma
                mVerifyProgrammedCauseOnMandatory = Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "VerifyProgrammedCauseOnMandatory"))

                ' Verifica si tiene comportamiento de ausencias productivas
                bolProductiveAbsences = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ApplyProductiveAbsences"))
                If bolProductiveAbsences Then
                    Dim strCause As String = String.Empty
                    strCause = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ProductiveAbsencesCause"))
                    If strCause.Length > 0 Then
                        dblProductiveAbsenceCause = roTypes.Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '" & strCause & "'"))
                        bolProductiveAbsences = (dblProductiveAbsenceCause > 0)
                    End If
                End If

                ' Comprueba si tenemos que aplicar regla de gratificacion
                bolApplyGratificationRule = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.GratificationRule"))

                ' Verifica si hay alguna cusotmizacion del cliente
                strCustomization = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Customization"))

                ' Gestion de horas extras en latam
                bolLatam_OvertimeManagement = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Latam.OvertimeManagement"))

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: LoadConfig")
            End Try

            Return (oState.Result = EngineResultEnum.NoError)
        End Function

        Public Function ExecuteBatch(ByRef bolGUIDChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Recuperamos ID de empleado de la tarea
                mIDEmployee = If(mTask.Parameters IsNot Nothing AndAlso mTask.Parameters.Exists("IDEmployee"), roTypes.Any2Integer(mTask.Parameters("IDEmployee")), 0)
                If mIDEmployee = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteBatch: Unable to set IDEmployee from task.")
                    Me.oState.Result = EngineResultEnum.EmployeeRequired
                    Return False
                End If

                Me.oState.Result = EngineResultEnum.NoError

                PreProcessAccrualsRules()
                If oState.Result <> EngineResultEnum.NoError OrElse mGUIDChanged Then
                    bolGUIDChanged = mGUIDChanged
                    Return False
                End If

                ' Únicamente considero los registros con status comprendido entre mi prioridad y la prioridad del proceso precedente
                Dim strSQL As String = "@SELECT# IDEmployee,Date, Status, ISNULL(IsHolidays,0) as Holidays FROM DailySchedule with (nolock) " &
                                            " WHERE Status >= " & mPreviousProcessPriority.ToString & " And Status < " & mPriority.ToString &
                                                " AND Date<=" & Any2Time(Now.Date).SQLSmallDateTime & " AND IDEmployee = " & mIDEmployee.ToString &
                                                  " AND GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "'" &
                                                    " ORDER BY IDEmployee,Date"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then

                    ' Inicializamos
                    If Not LoadConfig() Then Return False

                    For Each oRowEmp As DataRow In dTbl.Rows
                        ' Estado actual
                        mInitialRegisterStatus = Any2Integer(oRowEmp("Status"))
                        bolIsHolidays = Any2Boolean(oRowEmp("Holidays"))
                        bolProgrammedAbsenceOnHolidays = False
                        mInitialRegisterStatus = Any2Integer(oRowEmp("Status"))
                        mCurrentDate = Any2DateTime(oRowEmp("Date")).Date
                        bolSetAnnualWorkPeriod = False

                        ' Realiza el proceso
                        bolRet = ExecuteSingleDay(Any2Long(oRowEmp("IDEmployee")), oRowEmp("Date"))
                        If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError OrElse mGUIDChanged Then
                            ' Salimos si hubo error, o bien cambió el GUID de alguno de los registros a procesar.
                            bolGUIDChanged = mGUIDChanged
                            Return False
                        End If
                        'Para los horarios flexibles, debes de calcular el tiempo que falta por trabajar y si es necesario, actualizar el campo BeginMandatory
                        Try
                            Dim employeeId = Any2Long(oRowEmp("IDEmployee"))
                            'Solo calculamos los horarios flexibles para el día actual
                            If oRowEmp("Date") = Date.Now.Date Then
                                Dim IdShift = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# IDShiftUsed FROM DailySchedule WHERE IDEmployee= " & employeeId & " AND Date=" & roTypes.Any2Time(roTypes.Any2Time(oRowEmp("Date")).DateOnly).SQLSmallDateTime))
                                Dim isFlexible = roShift.IsShiftFlexible(IdShift, New roShiftState(-1))

                                If isFlexible Then
                                    roBaseEngineManager.SetBeginMandatoryIfFlexibleShift(IdShift, employeeId, oRowEmp("Date"), oState)
                                End If
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "roAccrualsManager::ExecuteBatch::FixBeginMandatoryIfFlexible error: ", ex)
                        End Try

                        ' Comprueba notificacion alerta de maximos Version Live
                        If dblMaximumValues > 0 Then
                            If DailyScheduleGUIDChanged() Then
                                bolGUIDChanged = mGUIDChanged
                                Return True
                            End If
                            ExecuteConceptMaximumValues(Any2Long(oRowEmp("IDEmployee")), Any2Time(oRowEmp("Date")))
                            If oState.Result <> EngineResultEnum.NoError Then
                                Return False
                            End If
                        End If

                        ' Comprueba notificacion alerta de mínimos Version Live
                        If dblMinimumValues > 0 Then
                            If DailyScheduleGUIDChanged() Then
                                bolGUIDChanged = mGUIDChanged
                                Return True
                            End If
                            ExecuteConceptMinimumValues(Any2Long(oRowEmp("IDEmployee")), Any2Time(oRowEmp("Date")))
                            If oState.Result <> EngineResultEnum.NoError Then
                                Return False
                            End If
                        End If

                    Next

                    If mMarkNextExpiredDate Then
                        ' Generamos la tarea para que se proceso el dia marcado por causa de caducidades
                        Dim oParams As New roCollection
                        oParams.Add("TaskType", TasksType.DAILYCAUSES.ToString)
                        oParams.Add("IDEmployee", mIDEmployee)
                        If (roLiveTask.CreateLiveTask(roLiveTaskTypes.RunEngineEmployee, oParams, New roLiveTaskState()) <= 0) Then
                            Return False
                        End If
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteBatch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteBatch")

            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay(ByVal EmployeeID As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Procesa un dia concreto
            '
            Dim bolRet As Boolean = False
            Dim zCurrentCauses As roCollection

            Dim zDate As New roTime
            Dim zShift As roShiftEngine

            Try

                oState.Result = EngineResultEnum.NoError

                Debug.Print(Now & "     ----> ACCRUALS: Processing employee '" & EmployeeID & "', date " & TaskDate)

                ' Fecha a procesar
                zDate = Any2Time(TaskDate)

                ' Obtiene horario de ese dia
                zShift = roBaseEngineManager.Execute_GetShift(EmployeeID, zDate, bolIsHolidays, bolProgrammedAbsenceOnHolidays, oState)

                ' Aplicamos regla de gratificacion en caso necesario
                If bolApplyGratificationRule Then
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_GratificationRule(EmployeeID, zDate, zShift)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingGratificationRule
                        Return False
                    End If
                End If

                ' Aplicamos regla de descanso obligado en caso necesario
                If bolRigidBreakRule Then
                    If DailyScheduleGUIDChanged() Then Return True
                    Execute_RigidBreakRule(EmployeeID, zDate, zShift)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingBreakRules
                        Return False
                    End If
                End If

                ' Ejecutamos el tratamiento de equivalencias automaticas
                If bolAutomaticEquivalence Then
                    If DailyScheduleGUIDChanged() Then Return True
                    ExecuteSingleDay_DoAutomaticEquivalence(EmployeeID, TaskDate, zShift)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorApplyingEquivalences
                        Return False
                    End If
                End If

                ' Ejecutamos las reglas diarias de horarios y de horas extras de latam, en caso necesario
                If DailyScheduleGUIDChanged() Then Return True
                bolRet = ExecuteSingleDay_DoShiftDailyRules(EmployeeID, TaskDate)
                If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorDoingShiftDailyRules
                    Return False
                End If

                ' Generamos los devengos automaticos de los saldos, en caso necesario
                If bolAutomaticAccruals Then
                    If DailyScheduleGUIDChanged() Then Return True
                    ExecuteSingleDay_DoAutomaticAccruals(EmployeeID, TaskDate, zShift)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorDoingAutomaticAccruals
                        Return False
                    End If
                End If

                ' Ejecuta los límites por justificación. Lo hacemos después de tener todas las justificaciones generadas en el día
                If dblCauseLimits > 0 Then
                    If DailyScheduleGUIDChanged() Then Return True
                    ExecuteSingleDay_DoCauseLimits(EmployeeID, TaskDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorDoingCausesLimits
                        Return False
                    End If
                End If

                ' Obtiene causas actuales
                zCurrentCauses = Execute_GetCausesSum(EmployeeID, zDate)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorGettingCausesSum
                    Return False
                End If

                ' Sumamos tiempos de las causas para cada concepto o solo uno si solo tocamos uno
                bolRet = ExecuteSingleDay_DoAccruals(EmployeeID, zCurrentCauses, TaskDate)
                If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorDoingAccruals
                    Return False
                End If

                ' Verificamos que no se ha marcado para procesar por otro proceso
                If DailyScheduleGUIDChanged() Then Return True
                ' Guarda datos en la base de datos
                bolRet = Execute_SaveData(EmployeeID, zDate)
                If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorSavingData
                    Return False
                End If

                ' Aprobaciones automaticas de solicitudes de declaracion de la jornada, en caso necesario
                If intIDConcept_DailyRecordwithAutomaticApproval > 0 Then
                    bolRet = ExecuteSingleDay_AutomaticApprovalDailyRecord(EmployeeID, TaskDate)
                    If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.AutomaticApprovalDailyRecord
                        Return False
                    End If
                    If DailyScheduleGUIDChanged() Then Return True
                End If

                'Ejecuta las reglas de acumulados. Lo hacemos después de salvar los acumulados en el paso anterior
                ExecuteSingleDay_DoRules(EmployeeID, zDate)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorDoingRules
                    Return False
                End If

                'Ejecutamos el cálculo de valores iniciales version Live
                If DailyScheduleGUIDChanged() Then Return True
                ExecuteSingleDay_DoStartupValuesLive(EmployeeID, zDate)
                If oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorDoingStartupValues
                    Return False
                End If
                If DailyScheduleGUIDChanged() Then Return True

                'Ejecutamos la asignación de tramos a los valores negativos de saldos anuales laborales
                If bolAnnualWorkConceptsDefined AndAlso bolSetAnnualWorkPeriod Then
                    If DailyScheduleGUIDChanged() Then Return True
                    ExecuteSingleDay_DoSetPeriodAnnualWork(EmployeeID, zDate)
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorSetPeriodAnnualWork
                        Return False
                    End If
                End If

                'Ejecutamos las reglas de caducidades para saldos anuales laborables
                If bolAnnualWorkConceptsDefined Then
                    If DailyScheduleGUIDChanged() Then Return True
                    mCreateConceptsRules = New roCollection
                    ExecuteSingleDay_DoExpiredDateRules(EmployeeID, zDate, True)
                    CreateConceptsRules()
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorDoingExpiredDateRules
                        Return False
                    End If
                    If DailyScheduleGUIDChanged() Then Return True
                End If

                ' Actualizamos el estado del empleado/dia
                bolRet = ExecuteSingleDay_UpdateStatus(EmployeeID, zDate)
                If Not bolRet OrElse oState.Result <> EngineResultEnum.NoError Then
                    oState.Result = EngineResultEnum.ErrorUpdatingStatus
                    Return False
                End If
                If mGUIDChanged Then
                    Return True
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteSingleDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteSingleDay")

            End Try

            Return bolRet

        End Function

        Private Sub PreProcessAccrualsRules()

            Dim dEmployeeFirstDate As Date
            Dim dEmployeeLastDate As Date
            Try

                oState.Result = EngineResultEnum.NoError

                ' Comprobamos si existen saldos anuales laborales
                bolAnnualWorkConceptsDefined = (Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM Concepts WHERE DefaultQuery = 'L'")) > 0)

                ' Revisamos si hay que aplicar los arrastres en el mismo dia
                bolApplyCarryOverOnSameDate = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "ApplyCarryOverOnSameDate"))


                Dim strSQL As String = "@SELECT# IDEmployee, MIN(Date) FirstDate, MAX(Date) LastDate FROM DailySchedule with (nolock) WHERE Status >= " & mPreviousProcessPriority.ToString &
                                       " And Status < " & mPriority.ToString & " AND Date<=" & Any2Time(Now.Date).SQLSmallDateTime & " AND IDEmployee = " & mIDEmployee.ToString & " GROUP BY IDEmployee"

                Dim tbl As DataTable
                tbl = CreateDataTableWithoutTimeouts(strSQL)
                If tbl IsNot Nothing AndAlso tbl.Rows.Count > 0 Then
                    dEmployeeFirstDate = Any2DateTime(tbl.Rows(0)("FirstDate")).Date
                    dEmployeeLastDate = Any2DateTime(tbl.Rows(0)("LastDate")).Date
                    ExecuteBatch_MarkAccrualsRules(Any2Time(dEmployeeFirstDate))
                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorPreprocessingDaysForAccrualsRules
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: PreProcessAccrualsRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::PreProcessAccrualsRules")

            End Try
        End Sub

        Private Sub ExecuteBatch_MarkAccrualsRules(dFirstDate As roTime)

            Dim dCurrentDate As Date
            Dim LastIDLabAgree As Double
            Dim LastIDContract As String
            Dim IDLabAgree As Double
            Dim IDContract As String
            Dim strSQL As String = String.Empty
            Dim BeginDate As roTime
            Dim EndDate As roTime
            Dim oLabAgreeAccrualRules As New Generic.List(Of roLabAgreeEngineAccrualRule)
            Try

                oState.Result = EngineResultEnum.NoError

                ' Cargo en memoria la reglas definidas para el usuario y elimina las reglas generadas anteriormente
                ExecuteBatch_BeginEmployee(mIDEmployee, dFirstDate)

                ' Bucle desde firstDate hasta hoy
                ' marcando todos los dias que se aplica alguna regla de acumulado
                LastIDLabAgree = 0
                LastIDContract = ""
                dCurrentDate = dFirstDate.Value

                Dim DBAnnualWorkPeriod As DataTable = Nothing

                While DateDiff("d", dCurrentDate.Date, Now) >= 0

                    ' Obtenemos convenio del dia
                    Dim oCurrentContract As New VTEmployees.Contract.roContract
                    oCurrentContract = Contract.roContract.GetContractInDateLite(mIDEmployee, dCurrentDate, New Contract.roContractState(oState.IDPassport))
                    If oCurrentContract IsNot Nothing AndAlso oCurrentContract.LabAgree IsNot Nothing Then
                        IDLabAgree = Any2Integer(oCurrentContract.LabAgree.ID)
                        IDContract = Any2String(oCurrentContract.IDContract)
                        BeginDate = Any2Time(oCurrentContract.BeginDate)
                        EndDate = Any2Time(oCurrentContract.EndDate)

                        If LastIDLabAgree <> IDLabAgree Then
                            ' Si cambia el convenio volvemos a cargar las reglas de acumulados
                            ' Cargamos colección de roLAbAgreeRule
                            Dim oLabAgreeEngine As roLabAgreeEngine
                            oLabAgreeEngine = roBaseEngineManager.GetLabAgreeeFromCache(IDLabAgree, oState)
                            oLabAgreeAccrualRules = oLabAgreeEngine.LabAgreeAccrualRules

                            LastIDLabAgree = IDLabAgree
                            LastIDContract = IDContract
                            ' Marcamos para recalculo los inicios de contrato para volver a revisar los valores iniciales con excepciones, en caso que sea necesario
                            If Len(strSQL) > 0 Then
                                strSQL = strSQL & ","
                            End If
                            strSQL += Any2Time(dCurrentDate).SQLSmallDateTime
                        End If

                        ' Para cada regla del convenio
                        For Each oLabAgreeAccrualRule In oLabAgreeAccrualRules
                            Dim oTmpCurrentRule As New roLabAgreeEngineAccrualRule
                            ' Clonamos e Indicamos el periodo de validez
                            oTmpCurrentRule = roLabAgreeManager.CloneLabAgreeAccrualRule(oLabAgreeAccrualRule)
                            oTmpCurrentRule.BeginDate = Any2Time(roConversions.Max(Any2Time(oTmpCurrentRule.BeginDate), BeginDate)).Value
                            oTmpCurrentRule.EndDate = Any2Time(roConversions.Min(Any2Time(oTmpCurrentRule.EndDate), EndDate)).Value

                            ' La regla se aplica a este empleado, miramos si se aplica hoy o no
                            Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(oState.IDPassport))
                            If oLabAgreeManager.AccrualRuleApplyOnDate(mIDEmployee, oTmpCurrentRule, dCurrentDate, DBAnnualWorkPeriod) AndAlso
                                oTmpCurrentRule.BeginDate <= dCurrentDate AndAlso
                                oTmpCurrentRule.EndDate >= dCurrentDate Then
                                ' La regla aplica hoy
                                ' Marcamos el status para el día en el que estamos y para el siguiente (en previsión de que la aplicación de la regla implique la creación de una justificación)
                                If Len(strSQL) > 0 Then
                                    strSQL = strSQL & ","
                                End If
                                strSQL += Any2Time(dCurrentDate).SQLSmallDateTime
                                strSQL += "," & Any2Time(dCurrentDate.AddDays(1)).SQLSmallDateTime
                                Exit For
                            End If
                        Next

                    End If

                    ' Sumo un día y sigo
                    dCurrentDate = dCurrentDate.AddDays(1)

                    If oState.Result <> EngineResultEnum.NoError Then
                        oState.Result = EngineResultEnum.ErrorMarkingAccrualsRulesNextDates
                        Exit While
                    End If
                End While

                If oState.Result = EngineResultEnum.NoError Then
                    ' Verifico que no me han cambiado el GUID
                    If Len(strSQL) > 0 Then
                        ' Ejecutamos el update para marcar todos los días necesarios del empleado
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ",GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' WHERE IDEmployee = " + mIDEmployee.ToString + " AND (Date IN(" & strSQL & ")) AND Date <= getdate() and Status > " & roCausesManager.mPriority.ToString
                        If Not ExecuteSql(strSQL) Then
                            oState.Result = EngineResultEnum.ErrorMarkingAccrualsRulesNextDates
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteBatch_MarkAccrualsRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteBatch_MarkAccrualsRules")

            End Try
        End Sub

        Private Sub ExecuteBatch_BeginEmployee(ByVal IDEmployee As Long, ByVal TaskDate As roTime)
            '
            '   Al empezar con un empleado nuevo:
            '     -Borra todos los calculos de las reglas de acumulados de un dia en adelante.
            '     -Carga en memoria la lista de reglas que se aplican a ese empleado
            '
            Dim strSQL As String

            Dim iStep As Integer

            Try

                oState.Result = EngineResultEnum.NoError

                ' 0. Marcamos para recalcular los dias con justificaciones provenientes de arrastres , ya que hay que volver
                '    a calcular los acumulados que afecten a esas justificaciones
                iStep = 1
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ",GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' WHERE IDEmployee = " & IDEmployee & " AND Date IN(@SELECT# DISTINCT Date FROM DailyCauses WITH (NOLOCK) WHERE AccrualsRules=1 AND IDEmployee=" & IDEmployee & " AND Date >" & TaskDate.SQLSmallDateTime & " )  AND Date <= getdate() AND Status > " & roCausesManager.mPriority.ToString
                ExecuteSql(strSQL)

                ' Marcamos para recalcular los dias con fechas futuras que sean de saldos con fecha de caducidad
                iStep = 2
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ",GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' WHERE IDEmployee = " & IDEmployee & " AND Date IN(@SELECT# DISTINCT ExpiredDate FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND ExpiredDate >" & TaskDate.SQLSmallDateTime & " AND ExpiredDate IS NOT NULL AND IDConcept IN(@SELECT# id FROM Concepts WHERE (DefaultQuery = 'C' or DefaultQuery = 'L') AND (ISNULL(ApplyExpiredHours,0) = 1) or DefaultQuery = 'L')  )   and Date <= getdate() and Status > " & roCausesManager.mPriority.ToString
                ExecuteSql(strSQL)

                ' 1. Borra datos obsoletos para fechas superiores a la fecha de la tarea
                iStep = 3
                strSQL = "@DELETE# FROM DailyCauses WHERE AccrualsRules=1 AND IDEmployee=" & IDEmployee & " AND Date >" & TaskDate.SQLSmallDateTime
                ExecuteSql(strSQL)

                ' Borro los acumulados provenientes de reglas de acumulado (carryover = 1) y preservando los que vengan de valores iniciales (StartupValue = 1)
                iStep = 4
                strSQL = "@DELETE# FROM DailyAccruals WHERE CarryOver=1 AND StartupValue = 0 AND IDEmployee=" & IDEmployee & " AND Date >" & TaskDate.SQLSmallDateTime
                If bolApplyCarryOverOnSameDate Then
                    ' Si se debe aplicar la regla el mismo dia se borran los valores del mismo dia tambien
                    strSQL = "@DELETE# FROM DailyAccruals WHERE CarryOver=1 AND StartupValue = 0 AND IDEmployee=" & IDEmployee & " AND Date >=" & TaskDate.SQLSmallDateTime
                End If
                ExecuteSql(strSQL)

                ' Borramos las acumulados de reglas de saldos de caducidad del mismo dia de calculo
                iStep = 5
                strSQL = "@DELETE# FROM DailyAccruals WHERE CarryOver=1 AND StartupValue = 0 AND IDEmployee=" & IDEmployee & " AND Date=" & TaskDate.SQLSmallDateTime & " AND IDConcept IN(@SELECT# id FROM Concepts WHERE (DefaultQuery = 'C' or DefaultQuery = 'L') AND (ISNULL(ApplyExpiredHours,0) = 1 or  DefaultQuery = 'L')) "
                ExecuteSql(strSQL)

                ' Marcamos para recalclar los valores iniciales futuros
                ' ya que puede que si su valor tiene
                ' que ser el del contrato anterior se tengan que volver a calcular
                iStep = 6
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ",GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' WHERE IDEmployee = " & IDEmployee & " AND Date IN(@SELECT# DISTINCT Date FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND CarryOver=1 AND StartupValue = 1 AND Date>" & TaskDate.SQLSmallDateTime & " AND Date <= getdate()) AND Status > " & roCausesManager.mPriority.ToString & "  AND Date>" & TaskDate.SQLSmallDateTime & "  and Date <= getdate() "
                ExecuteSql(strSQL)

                ' Borramos los valores iniciales futuros
                iStep = 7
                strSQL = "@DELETE# FROM DailyAccruals WHERE CarryOver=1 AND StartupValue = 1 AND IDEmployee=" & IDEmployee & " AND Date>=" & TaskDate.SQLSmallDateTime
                ExecuteSql(strSQL)

                ' En el caso que haya saldos anuales laborables, debemos marcar para recalcular las fechas futuras que contengan valores de esos saldos
                If bolAnnualWorkConceptsDefined Then
                    iStep = 8
                    If Not ExecuteSingleDay_MarkNextAnnualWorkDates(IDEmployee, TaskDate, -1) Then
                        oState.Result = EngineResultEnum.ErrorUpdatingPeriodAnnualWork
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteBatch_BeginEmployee: Step " & iStep)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteBatch_BeginEmployee: Step " & iStep)
            End Try
        End Sub

        Private Sub ExecuteSingleDay_DoAutomaticEquivalence(ByVal EmployeeID As Long, ByVal TaskDate As Date, ByVal zShift As roShiftEngine)
            '
            ' Ejecuta la tarea indicada
            '
            Dim zDate As New roTime
            Dim DBCauses As DataTable

            Dim DBManualCenters As DataTable

            Dim CauseValue As Double
            Dim IDCause As Double

            Dim m_Definition As roCollection
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtiene empleado y fecha de la tarea
                zDate = Any2Time(TaskDate)

                ' Antes de eliminar las justificaciones generadas por las reglas diarias
                ' nos guardamos las que tengan un centro de coste manual,
                ' y en el caso que se vuelva a generar la misma justificacion
                ' asignar de nuevo el mismo centro

                strSQL = "@SELECT# IDCause,IDCenter,Value,isnull(DefaultCenter,0) as DefaultCenter  FROM DailyCauses WHERE IDEmployee=" & EmployeeID &
                         " AND Date=" & zDate.SQLSmallDateTime & " AND DailyRule= 0 AND AccruedRule = 0 AND AccrualsRules=0 AND IDRelatedIncidence= 0  AND ManualCenter=1 AND IDCause IN(@SELECT# DISTINCT AutomaticEquivalenceIDCause FROM Causes WHERE AutomaticEquivalenceIDCause > 0 AND AutomaticEquivalenceType > 0)"
                DBManualCenters = CreateDataTable(strSQL)

                ' Eliminamos las justificaciones que se hayan generado previamente de equivalencia
                strSQL = "@DELETE# FROM DailyCauses WHERE IDEmployee= " & EmployeeID & " AND Date=" & zDate.SQLSmallDateTime &
                         " AND DailyRule= 0 AND AccruedRule = 0 AND AccrualsRules=0 AND IDRelatedIncidence= 0 AND " &
                         " IDCause IN(@SELECT# DISTINCT ID FROM Causes WHERE AutomaticEquivalenceIDCause > 0 AND AutomaticEquivalenceType > 0)"

                If Not ExecuteSql(strSQL) Then
                    oState.Result = EngineResultEnum.ErrorDeletingEquivalences
                    Exit Sub
                End If

                ' Obtenemos las justificaciones diarias actuales
                DBCauses = GetDailyCauses(EmployeeID, TaskDate)

                ' Para cada justificacion diaria del empleado
                ' miramos si debemos generar la justificacion de equivalencia
                For Each oDailyCauseRow As DataRow In DBCauses.Rows
                    IDCause = Any2Double(oDailyCauseRow("IDCause"))
                    For Each oEquivalence As DataRow In AutomaticEquivalenceCausesDB.Rows
                        If IDCause = Any2Double(oEquivalence("AutomaticEquivalenceIDCause")) Then
                            CauseValue = 0
                            m_Definition = New roCollection

                            ' Debemos generar la justificacion a partir de la formula de equivalencia indicada
                            Select Case Any2Double(oEquivalence("AutomaticEquivalenceType"))
                                Case 1 ' Horas teoricas del horario
                                    ' Si tenemos horario
                                    If Not zShift Is Nothing Then
                                        ' Valor de la justificacion/ horas teoricas
                                        If Any2Time(zShift.ExpectedWorkingHours).NumericValue > 0 Then
                                            CauseValue = Any2Time(Any2Double(oDailyCauseRow("Value")), True).NumericValue / Any2Time(zShift.ExpectedWorkingHours).NumericValue
                                        End If
                                    End If
                                Case 2 ' Horas teoricas en un campo de la ficha
                                    m_Definition.LoadXMLString(Any2String(oEquivalence("AutomaticEquivalenceCriteria")))
                                    If m_Definition.Exists("FactorField") Then
                                        Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, m_Definition("FactorField"), TaskDate, New UserFields.roUserFieldState, False)
                                        Dim FactorValue As Double = Any2Double(oUserField.FieldRawValue)
                                        CauseValue = Any2Time(FactorValue).NumericValue
                                        If CauseValue <> 0 Then
                                            CauseValue = Any2Time(oDailyCauseRow("Value"), True).NumericValue / CauseValue
                                        End If
                                    End If
                                Case 3 ' Coeficiente directo
                                    m_Definition.LoadXMLString(Any2String(oEquivalence("AutomaticEquivalenceCriteria")))
                                    If m_Definition.Exists("FactorValue") Then
                                        CauseValue = Any2Time(Any2Double(m_Definition("FactorValue"))).NumericValue
                                        If CauseValue <> 0 Then
                                            CauseValue = Any2Double(oDailyCauseRow("Value")) * CauseValue
                                        End If
                                    End If
                            End Select

                            If CauseValue <> 0 Then
                                ' Generamos el valor de la equivalencia
                                CreateDailyEquivalenceCause(EmployeeID, Any2Time(TaskDate), CauseValue, Any2Integer(oEquivalence("ID")), DBManualCenters)
                            End If

                        End If
                    Next
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAutomaticEquivalence")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteSingleDay_DoAutomaticEquivalence")
            End Try
        End Sub

        Private Function Execute_GetCausesSum(ByVal zEmployee As Long, ByVal zDate As roTime) As roCollection
            '
            ' Obtiene causas del dia indicado, sumando ya todas las que son iguales y redondeando
            '  segun se deba.
            '
            Dim rcResult As New roCollection
            Dim CurrentCause As New roCauseEngine
            Dim CurrentID As Long
            Dim tmpCurrentID As Long
            Dim CurrentSumValue As Double
            Dim CurrentValue As Double
            Dim LastID As Long
            Dim sSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtiene causas, ordenadas por ID
                sSQL = "@SELECT# IDCause,Value FROM DailyCauses WHERE IDEmployee=" & zEmployee.ToString & " AND Date=" & zDate.SQLSmallDateTime & " ORDER BY IDCause"

                ' FIAT. Añado a la colección las justificaciones producidas en franja nocturna
                If strCustomization = "taif" Then
                    sSQL = "@SELECT# IDCause,Value FROM DailyCauses WHERE IDEmployee=" & zEmployee.ToString & " AND Date=" & zDate.SQLSmallDateTime & " " &
                           " UNION ALL @SELECT# 90000 + DailyCauses.IDCause IDCause, DailyCauses.Value " &
                           " From DailyCauses " &
                           " LEFT OUTER JOIN DailyIncidences on DailyCauses.IDEmployee = DailyIncidences.IDEmployee and DailyCauses.Date = DailyIncidences.Date and DailyCauses.IDRelatedIncidence = DailyIncidences.ID " &
                           " LEFT OUTER JOIN TimeZones on DailyIncidences.IDZone = TimeZones.ID " &
                           " LEFT OUTER JOIN Causes on  Causes.ID = Dailycauses.IDCause " &
                           " Where DailyCauses.IDEmployee =" & zEmployee.ToString & " AND DailyCauses.Date=" & zDate.SQLSmallDateTime & " AND CONVERT(nvarchar,ISNULL(Timezones.Description,'')) = '#NOC' " &
                           " AND Causes.Description Like '%NOC%' " &
                           " ORDER BY IDCause"
                End If

                ' Ahora carga las causas, sumando todos los tiempos que pertenezcan a un mismo ID.
                '  Además redondea cada una o solo la suma según el tipo de redondeo que se quiera.
                LastID = -1
                CurrentSumValue = 0
                Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In dTbl.Rows
                        ' Obtiene el ID y valor de este registro
                        CurrentID = oRowEmp("IDCause")

                        'Especial per srive para el standar
                        ' si viene un valor negativo hay que tratarlo en positivo hasta el momento de sumarlo que se
                        ' canvia el valor
                        If oRowEmp("Value") >= 0 Then
                            CurrentValue = Any2Time(oRowEmp("Value"), True).VBNumericValue
                        Else
                            CurrentValue = Any2Time(oRowEmp("Value") * -1, True).VBNumericValue
                        End If
                        ' Mira si seguimos sumando o empezamos de nuevo (si se trata de un ID distinto al
                        '  anterior)
                        If CurrentID <> LastID Then
                            ' Empezamos a sumar un ID nuevo
                            If LastID <> -1 Then
                                ' Guardamos la suma anterior, redondeando si es necesario
                                'especial si tiene un valor negativo hacemos el tratamiento
                                'en positivo y al final le cambiamos el valor

                                If CurrentSumValue >= 0 Then
                                    rcResult.Add(LastID, Execute_GetCausesSum_CompleteSum(CurrentSumValue, CurrentCause))
                                Else
                                    rcResult.Add(LastID, Execute_GetCausesSum_CompleteSum(CurrentSumValue * -1, CurrentCause) * -1)
                                End If
                            End If
                            ' Inicializamos la nueva suma
                            CurrentSumValue = 0
                            If strCustomization = "taif" Then
                                ' Especial FIAT
                                tmpCurrentID = CurrentID
                                If CurrentID >= 90000 Then tmpCurrentID = CurrentID - 90000
                                CurrentCause = roBaseEngineManager.GetCauseFromCache(tmpCurrentID, oState)

                            Else
                                CurrentCause = roBaseEngineManager.GetCauseFromCache(CurrentID, oState)
                            End If

                        End If

                        ' Sumamos el tiempo actual a la causa actual
                        With CurrentCause
                            If Not .RoundingByDailyScope AndAlso .RoundingBy <> 1 Then
                                ' Si debemos redondear cada causa, lo hacemos ahora
                                ' si viene un valor negativo hay que tratarlo en positivo hasta el momento de sumarlo que se
                                ' canvia el valor
                                If oRowEmp("Value") >= 0 Then
                                    CurrentSumValue = CurrentSumValue + RoundTime(CurrentValue, .RoundingType, .RoundingBy)
                                Else
                                    CurrentSumValue = CurrentSumValue + (RoundTime(CurrentValue, .RoundingType, .RoundingBy) * -1)
                                End If
                            Else
                                ' Si solo redondeamos la suma de todas, ahora lo suma sin redondeo (de momento)
                                If oRowEmp("Value") >= 0 Then
                                    CurrentSumValue = CurrentSumValue + CurrentValue
                                Else
                                    ' Si era negativo, recupero el signo ahora
                                    CurrentSumValue = CurrentSumValue + CurrentValue * -1
                                End If

                            End If
                        End With
                        LastID = CurrentID
                    Next
                End If

                ' Guarda la ultima suma de causas (si hay)
                If LastID <> -1 Then rcResult.Add(LastID, Execute_GetCausesSum_CompleteSum(CurrentSumValue, CurrentCause))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_GetCausesSum")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::Execute_GetCausesSum")
            End Try

            Return rcResult

        End Function

        Private Function Execute_GetCausesSum_CompleteSum(ByVal CurrentSumValue As Double, ByRef CurrentCause As roCauseEngine) As Double
            '
            ' Completa la suma de tiempos diarios de una causa
            '   Si hay que redondear diariamente, lo hace ahora.
            '   Además, convierte el valor de VB a formato Robotics.
            '
            Dim bolret As Double = 0

            Try
                With CurrentCause
                    If .RoundingByDailyScope AndAlso .RoundingBy <> 1 Then
                        ' Si esta causa se redondea diariamente lo hace ahora
                        CurrentSumValue = RoundTime(CurrentSumValue, .RoundingType, .RoundingBy)
                    End If
                End With

                ' Ahora pasa a formato Robotics
                If CurrentSumValue >= 0 Then
                    bolret = Any2Time(Date.FromOADate(CurrentSumValue), True).NumericValue(True)
                Else
                    ' Los negativos hay que tratarlos de manera especial
                    bolret = Any2Time(Date.FromOADate(CurrentSumValue * -1), True).NumericValue(True) * -1
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_GetCausesSum_CompleteSum")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::Execute_GetCausesSum_CompleteSum")

            End Try

            Return bolret

        End Function

        Private Function RoundTime(ByVal VBTime As Double, ByVal RoundingType As eRoundingType, ByVal RoundingBy As Integer) As Double
            '
            ' Redondea un tiempo
            '
            Dim TotalMinutes As Long
            Dim RoundedMinutes As Double
            Dim Divergence As Double
            Dim Base As Double
            Dim bolRet As Double = 0

            Try

                ' Obtiene tiempo a justificar en minutos
                TotalMinutes = DateDiff("n", "1899/12/30", Date.FromOADate(VBTime))

                ' Calcula tiempo redondeado
                If RoundingBy <> 0 Then Divergence = TotalMinutes Mod RoundingBy
                Base = TotalMinutes - Divergence

                Select Case RoundingType
                    Case eRoundingType.Round_Near   ' Por aproximación
                        RoundedMinutes = Base
                        If Divergence > (RoundingBy / 2) Then RoundedMinutes = RoundedMinutes + RoundingBy

                    Case eRoundingType.Round_UP   ' Por exceso
                        RoundedMinutes = Base
                        If Divergence > 0 Then RoundedMinutes = RoundedMinutes + RoundingBy

                    Case eRoundingType.Round_Down   ' Por defecto
                        RoundedMinutes = Base
                End Select

                ' Devuelve el tiempo redondeado en formato VB
                bolRet = Any2Double(DateAdd("n", RoundedMinutes, "1899/12/30"))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: RoundTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::RoundTime")

            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_AutomaticApprovalDailyRecord(ByVal EmployeeID As Long, ByVal zDate As Date) As Boolean

            ' Obtenemos todas las solicitudes que esten pendientes de aprobar de declaracion de la jornada
            ' para el empleado y fecha indicados
            Dim oDailyRecordManager As roDailyRecordManager = Nothing
            Dim oDailyRecord As roDailyRecord = Nothing
            Dim oRecordState As roDailyRecordState = Nothing
            Dim bolret As Boolean = True
            Try

                Dim strSQL = "@SELECT# ID FROM Requests WITH (NOLOCK) WHERE IDEmployee=" & EmployeeID.ToString & " AND Date1 = " & Any2Time(zDate).SQLDateTime & "  AND RequestType= " & eRequestType.DailyRecord & " And Status in(" & eRequestStatus.Pending & "," & eRequestStatus.OnGoing & ")   ORDER BY RequestDate "
                Dim tb As DataTable = CreateDataTableWithoutTimeouts(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oRecordState = New roDailyRecordState(-1)
                    For Each oRequestRow As DataRow In tb.Rows
                        Dim oRequest As New roRequest(oRequestRow("ID"), New roRequestState(-1))
                        If oDailyRecordManager Is Nothing Then oDailyRecordManager = New roDailyRecordManager(oRecordState)
                        oDailyRecord = oDailyRecordManager.LoadDailyRecord(oRequestRow("ID"), oRequest)
                        If oDailyRecord IsNot Nothing AndAlso oDailyRecord.Adjusted AndAlso oRequest IsNot Nothing Then
                            ' Se intenta aprobar la solicitud
                            Dim bolretApproveRefuse = oRequest.ApproveRefuse(intIDPassportSystem, True, "SYSTEM",,, False, True)
                            If Not bolretApproveRefuse Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteSingleDay_AutomaticApprovalDailyRecord: Approved:: Request ID: " & oRequest.ID & " Employee ID: " & oRequest.IDEmployee & " : Error:: " & oRequest.State.ErrorText)
                            End If
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_AutomaticApprovalDailyRecord")
                bolret = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteSingleDay_AutomaticApprovalDailyRecord")
                bolret = False
            End Try

            Return bolret

        End Function

        Private Function ExecuteSingleDay_DoAccruals(ByVal EmployeeID As Long, ByRef CausesSum As roCollection, ByRef zDate As Date) As Boolean
            '
            ' Calcula acumulados y guarda el valor dentro de la colección de conceptos
            '
            Dim myConceptValue As Double
            Dim myConceptPositiveValue As Double
            Dim myConceptNegativeValue As Double
            Dim myConceptCause As roEngineConceptComposition
            Dim ExpectedWorkingHours As Double
            Dim ExpectedWorkingHoursShift As Double
            Dim CauseIndex As Long
            Dim CauseID As Long
            Dim CauseValue As Double

            ' Especial FIAT. Es posible que deba compactar la colección de justificaciones
            Dim originalCausesSum As New roCollection
            Dim compactCausesSum As New roCollection
            Dim compactCausesOnlyNOCSum As New roCollection
            Dim mCausesNOC As New roCollection


            Dim ShiftID As Double
            Dim isHolidays As Boolean

            Dim bolValid As Boolean

            Dim sSQL As String

            Dim rulesValue As Double
            Dim NotRulesValue As Double
            Dim FactorValue As Double

            Dim mEmployeeFieldsFactor As New roCollection  'Valores de la ficha

            Dim bolRet As Boolean = False
            ' Especial UPF
            Dim ExpectedWorkingHoursUPF As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ExpectedWorkingHours = -1
                ExpectedWorkingHoursShift = -1
                ShiftID = -1
                isHolidays = False
                ExpectedWorkingHoursUPF = -1

                '0. Obtenemos el valor de la ficha que se utilizan en la composicion de los saldos
                mEmployeeFieldsFactor.Clear()

                For Each myConcept As roConceptEngine In oConceptsCache.Values
                    For Each myConceptCause In myConcept.Composition
                        If Len(myConceptCause.CompositionUserField) > 0 Then
                            ' Si utilizamos un campo de la ficha,
                            ' debemos obtener el valor del campo y aplicarlo como factor
                            If Not mEmployeeFieldsFactor.Exists(myConceptCause.CompositionUserField) Then
                                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, myConceptCause.CompositionUserField, zDate, New UserFields.roUserFieldState, False)
                                If oUserField IsNot Nothing Then
                                    FactorValue = Any2Double(oUserField.FieldRawValue)
                                Else
                                    FactorValue = 0
                                End If
                                mEmployeeFieldsFactor.Add(myConceptCause.CompositionUserField, FactorValue)
                            End If
                        End If

                        ' Especial FIAT
                        If strCustomization = "taif" AndAlso myConceptCause.AdvParameter = "#NOC" Then
                            ' Cargo la lista de justificaciones marcadas como que sólo acumulan si se producen en nocturnas, por si debo usarla luego
                            If Not mCausesNOC.Exists(myConceptCause.IDCause) Then
                                mCausesNOC.Add(myConceptCause.IDCause, myConceptCause.IDCause)
                            End If
                        End If
                    Next
                Next

                If strCustomization = "taif" Then
                    ' Inicializamos colección compactada de justificaciones
                    originalCausesSum.Clear()
                    If CausesSum.Count > 0 Then
                        originalCausesSum.ImportKeyandDoubleValue(CausesSum)
                    End If
                End If

                For Each myConcept As roConceptEngine In oConceptsCache.Values
                    myConceptValue = 0
                    myConceptPositiveValue = 0
                    myConceptNegativeValue = 0


                    ' Solo procesamos si el concepto está activo en esta fecha
                    If ExecuteSingleDay_DoAccruals_IsConceptActive(zDate, myConcept) Then
                        ' Especial FIAT, tratamiento de justificaciones nocturnas
                        If strCustomization = "taif" Then
                            ExecuteSingleDay_DoAccruals_FIAT(myConcept, CausesSum, originalCausesSum, compactCausesSum, compactCausesOnlyNOCSum, mCausesNOC)
                        End If

                        ' Para cada composicion que acumula en este concepto..
                        For Each myConceptCause In myConcept.Composition
                            Dim myConceptCause_HoursFactor As Double = 0
                            Dim myConceptCause_OccurrencesFactor As Double = 0
                            myConceptCause_HoursFactor = IIf(myConcept.IDType = "H", myConceptCause.FactorValue, 0)
                            myConceptCause_OccurrencesFactor = IIf(myConcept.IDType = "O", myConceptCause.FactorValue, 0)

                            ' Revisamos el valor que tiene que tener el factor
                            If Len(myConceptCause.CompositionUserField) > 0 Then
                                ' Si utilizamos un campo de la ficha,
                                ' debemos obtener el valor del campo y aplicarlo como factor
                                FactorValue = mEmployeeFieldsFactor(myConceptCause.CompositionUserField, roCollection.roSearchMode.roByKey)
                                myConceptCause_HoursFactor = IIf(myConcept.IDType = "H", FactorValue, 0)
                                myConceptCause_OccurrencesFactor = IIf(myConcept.IDType = "O", FactorValue, 0)
                            End If

                            ' --- JUSTIFICACIONES
                            If myConceptCause.IDType = CompositionType.Cause Then
                                ' Para cada causa que acumula
                                If myConceptCause.IDCause <> CONCEPT_SHIFTEXPECTEDHOURS Then
                                    ' Sumamos un concepto estándar
                                    For CauseIndex = 1 To CausesSum.Count
                                        CauseID = CausesSum.Key(CauseIndex)
                                        CauseValue = CausesSum(CauseIndex, roCollection.roSearchMode.roByIndex)

                                        ' Si la causa que tenemos acumula en el concepto, lo hace ahora
                                        If CauseID = myConceptCause.IDCause Then
                                            ' Esta causa acumula en este concepto
                                            ' Comprobamos si tiene regla de composicion o no
                                            If myConceptCause.Conditions Is Nothing OrElse myConceptCause.Conditions.Count = 0 Then
                                                'Acumulamos de forma basica
                                                myConceptValue = myConceptValue + myConceptCause_HoursFactor * CauseValue
                                                If (myConceptCause_HoursFactor * CauseValue) >= 0 Then
                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_HoursFactor * CauseValue
                                                Else
                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_HoursFactor * CauseValue) * -1)
                                                End If

                                                ' Tratamiento para justificaciones que acumulan en saldo de veces
                                                If myConceptCause_OccurrencesFactor <> 0 Then
                                                    sSQL = "@SELECT# SUM(Value) FROM DailyCauses where IDEmployee = " & EmployeeID.ToString
                                                    sSQL = sSQL + " and Date = " & Any2Time(zDate).SQLSmallDateTime & " and (AccrualsRules = 1 or DailyRule = 1 or AccruedRule=1) "
                                                    sSQL = sSQL + " and IDCause = " & myConceptCause.IDCause.ToString
                                                    rulesValue = Any2Double(ExecuteScalar(sSQL))
                                                    If rulesValue <> 0 Then
                                                        ' Alguna justificación viene de reglas, por tanto es una arrastre, y debo sumar el valor de la justificación, y no 1 ocurrencia
                                                        ' PENDIENTE: No estoy teniendo en cuenta redondeo
                                                        myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * rulesValue

                                                        If (myConceptCause_OccurrencesFactor * rulesValue) >= 0 Then
                                                            myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * rulesValue
                                                        Else
                                                            myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * rulesValue) * -1)
                                                        End If

                                                        ' Miro si además hay alguna que no venga de reglas (la trataré diferente
                                                        sSQL = "@SELECT# sum(Value) from DailyCauses where IDEmployee = " & EmployeeID.ToString
                                                        sSQL = sSQL + " and Date = " & Any2Time(zDate).SQLSmallDateTime & " and (AccrualsRules = 0 and DailyRule = 0 and AccruedRule=0)  "
                                                        sSQL = sSQL + " and IDCause = " & myConceptCause.IDCause.ToString
                                                        NotRulesValue = Any2Double(ExecuteScalar(sSQL))
                                                        If NotRulesValue <> 0 Then
                                                            Dim oCauseCahe As roCauseEngine = roBaseEngineManager.GetCauseFromCache(CauseID, oState)
                                                            If oCauseCahe.DayType OrElse oCauseCahe.CustomType Then
                                                                ' Si es de tipo dia o personalizada sumamos el valor de la justificacion
                                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * NotRulesValue

                                                                If (myConceptCause_OccurrencesFactor * NotRulesValue) >= 0 Then
                                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * NotRulesValue
                                                                Else
                                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * NotRulesValue) * -1)
                                                                End If
                                                            Else
                                                                ' Si no, sumo una ocurrencia
                                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor

                                                                If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                                Else
                                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                                End If
                                                            End If

                                                        End If
                                                    Else
                                                        ' No viene de reglas.
                                                        Dim oCauseCahe As roCauseEngine = roBaseEngineManager.GetCauseFromCache(CauseID, oState)
                                                        If oCauseCahe.DayType Or oCauseCahe.CustomType Then
                                                            ' Si es de tipo dia o personalizada sumamos el valor de la justificacion
                                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * CauseValue

                                                            If (myConceptCause_OccurrencesFactor * CauseValue) >= 0 Then
                                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * CauseValue
                                                            Else
                                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * CauseValue) * -1)
                                                            End If
                                                        Else
                                                            If strCustomization = "UEPMOP" And myConcept.Description.Contains("#SLT") Then
                                                                ' UPF
                                                                ' Tratamiento especial para la ausencia por motivos de salud
                                                                ' el valor generado debe ser el valor de la justificacion / horas teoricas del horario
                                                                If ExpectedWorkingHoursUPF = -1 Then ExpectedWorkingHoursUPF = Any2Double(ExecuteScalar("@SELECT# (case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end) FROM DailySchedule, Shifts  WHERE IDEmployee=" & EmployeeID.ToString & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                                                                If ExpectedWorkingHoursUPF <> 0 Then
                                                                    myConceptValue = myConceptValue + (CauseValue / ExpectedWorkingHoursUPF)
                                                                End If
                                                            Else
                                                                ' Si no, sumo una ocurrencia
                                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor

                                                                If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                                Else
                                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                                End If

                                                            End If
                                                        End If

                                                    End If
                                                End If
                                            Else
                                                If Execute_EvaluateConceptCauseRuleCondition(myConceptCause.Conditions, CausesSum, EmployeeID, zDate) Then
                                                    ' Miramos que tipo de factor hay que utilizar

                                                    If myConceptCause.FactorType = ValueType.DirectValue Then
                                                        If myConcept.IDType = "H" Then
                                                            ' Si es de tiempo
                                                            myConceptValue = myConceptValue + myConceptCause_HoursFactor * CauseValue

                                                            If (myConceptCause_HoursFactor * CauseValue) >= 0 Then
                                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_HoursFactor * CauseValue
                                                            Else
                                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_HoursFactor * CauseValue) * -1)
                                                            End If
                                                        Else
                                                            ' Si es de nº de ocurrencias
                                                            sSQL = "@SELECT# SUM(Value) FROM DailyCauses WHERE IDEmployee = " & Any2String(EmployeeID)
                                                            sSQL = sSQL + " AND Date = " & Any2Time(zDate).SQLSmallDateTime & " AND (AccrualsRules = 1 OR DailyRule = 1 OR AccruedRule=1)  "
                                                            sSQL = sSQL + " AND IDCause = " & Any2String(myConceptCause.IDCause)
                                                            rulesValue = Any2Double(ExecuteScalar(sSQL))
                                                            If rulesValue <> 0 Then
                                                                ' Alguna justificación viene de reglas, por tanto es una arrastre, y debo sumar el valor de la justificación, y no 1 ocurrencia
                                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * rulesValue

                                                                If (myConceptCause_OccurrencesFactor * rulesValue) >= 0 Then
                                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * rulesValue
                                                                Else
                                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * rulesValue) * -1)
                                                                End If

                                                                ' Miro si además hay alguna que no venga de reglas (la trataré diferente)
                                                                sSQL = "@SELECT# SUM(Value) FROM DailyCauses WHERE IDEmployee = " & Any2String(EmployeeID)
                                                                sSQL = sSQL + " AND Date = " & Any2Time(zDate).SQLSmallDateTime & " AND (AccrualsRules = 0 AND DailyRule = 0 AND AccruedRule=0)  "
                                                                sSQL = sSQL + " AND IDCause = " & Any2String(myConceptCause.IDCause)
                                                                NotRulesValue = Any2Double(ExecuteScalar(sSQL))
                                                                If NotRulesValue <> 0 Then
                                                                    Dim oCauseCahe As roCauseEngine = roBaseEngineManager.GetCauseFromCache(CauseID, oState)
                                                                    If oCauseCahe.DayType Or oCauseCahe.CustomType Then
                                                                        ' Si es de tipo dia o personalizada sumamos el valor de la justificacion
                                                                        myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * NotRulesValue

                                                                        If (myConceptCause_OccurrencesFactor * NotRulesValue) >= 0 Then
                                                                            myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * NotRulesValue
                                                                        Else
                                                                            myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * NotRulesValue) * -1)
                                                                        End If
                                                                    Else
                                                                        ' Si no, sumo una ocurrencia
                                                                        myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor

                                                                        If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                            myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                                        Else
                                                                            myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                                        End If

                                                                    End If
                                                                End If
                                                            Else
                                                                ' No viene de reglas.
                                                                Dim oCauseCahe As roCauseEngine = roBaseEngineManager.GetCauseFromCache(CauseID, oState)
                                                                If oCauseCahe.DayType OrElse oCauseCahe.CustomType Then
                                                                    ' Si es de tipo dia o personalizada sumamos el valor de la justificacion
                                                                    myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * CauseValue

                                                                    If (myConceptCause_OccurrencesFactor * CauseValue) >= 0 Then
                                                                        myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * CauseValue
                                                                    Else
                                                                        myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * CauseValue) * -1)
                                                                    End If
                                                                Else
                                                                    ' Si no, sumo una ocurrencia
                                                                    myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                                                    If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                        myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                                    Else
                                                                        myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                                    End If
                                                                End If
                                                            End If
                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    Next
                                Else
                                    ' Sumamos Horas Teoricas

                                    ' Si no tenemos las teoricas, las obtiene ahora
                                    If ExpectedWorkingHours = -1 Then
                                        If Not bolProgrammedAbsenceOnHolidays Then
                                            ExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# (CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 1 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  END) FROM DailySchedule with (nolock), Shifts with (nolock)  WHERE IDEmployee=" & EmployeeID.ToString & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                                        Else
                                            ExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  FROM DailySchedule with (nolock), Shifts with (nolock)  WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase  "))
                                        End If

                                    End If
                                    ' Comprobamos si tiene regla de composicion o no
                                    If myConceptCause.Conditions Is Nothing OrElse myConceptCause.Conditions.Count = 0 Then
                                        ' Acumulamos de forma basica
                                        myConceptValue = myConceptValue + myConceptCause_HoursFactor * ExpectedWorkingHours
                                        If (myConceptCause_HoursFactor * ExpectedWorkingHours) >= 0 Then
                                            myConceptPositiveValue = myConceptPositiveValue + myConceptCause_HoursFactor * ExpectedWorkingHours
                                        Else
                                            myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_HoursFactor * ExpectedWorkingHours) * -1)
                                        End If

                                        If ExpectedWorkingHours <> 0 Then
                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                            If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                            Else
                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                            End If
                                        End If
                                    Else
                                        ' Acumulamos si se cumple la regla
                                        If Execute_EvaluateConceptCauseRuleCondition(myConceptCause.Conditions, CausesSum, EmployeeID, zDate) Then
                                            ' Miramos que tipo de factor hay que utilizar
                                            If myConceptCause.FactorType = ValueType.DirectValue Then
                                                If myConcept.IDType = "H" Then
                                                    ' Si es de tiempo
                                                    myConceptValue = myConceptValue + myConceptCause_HoursFactor * ExpectedWorkingHours
                                                    If (myConceptCause_HoursFactor * ExpectedWorkingHours) >= 0 Then
                                                        myConceptPositiveValue = myConceptPositiveValue + myConceptCause_HoursFactor * ExpectedWorkingHours
                                                    Else
                                                        myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_HoursFactor * ExpectedWorkingHours) * -1)
                                                    End If
                                                Else
                                                    ' Si es de nº de ocurrencias
                                                    sSQL = "@SELECT# SUM(Value) FROM DailyCauses WHERE IDEmployee = " & Any2String(EmployeeID)
                                                    sSQL = sSQL + " AND Date = " & Any2Time(zDate).SQLSmallDateTime & " AND (AccrualsRules = 1 OR DailyRule = 1 OR AccruedRule=1)  "
                                                    sSQL = sSQL + " AND IDCause = " & Any2String(myConceptCause.IDCause)
                                                    rulesValue = Any2Double(ExecuteScalar(sSQL))
                                                    If rulesValue <> 0 Then
                                                        ' Alguna justificación viene de reglas, por tanto es una arrastre, y debo sumar el valor de la justificación, y no 1 ocurrencia
                                                        ' PENDIENTE: No estoy teniendo en cuenta redondeo
                                                        myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor * rulesValue
                                                        If (myConceptCause_OccurrencesFactor * rulesValue) >= 0 Then
                                                            myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor * rulesValue
                                                        Else
                                                            myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor * rulesValue) * -1)
                                                        End If

                                                        ' Miro si además hay alguna que no venga de reglas (la trataré diferente
                                                        sSQL = "@SELECT# Value FROM DailyCauses WHERE IDEmployee = " & Any2String(EmployeeID)
                                                        sSQL = sSQL + " AND Date = " & Any2Time(zDate).SQLSmallDateTime & " AND (AccrualsRules = 0 AND DailyRule = 0 AND AccruedRule=0)  "
                                                        sSQL = sSQL + " AND IDCause = " & Any2String(myConceptCause.IDCause)

                                                        If Any2Double(ExecuteScalar(sSQL)) > 0 Then
                                                            ' Hay alguna que no viene de reglas. Sumo una ocurrencia
                                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                                            If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                            Else
                                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                            End If

                                                        End If
                                                    Else
                                                        ' No viene de reglas. Acumulo como hasta ahora
                                                        If ExpectedWorkingHours <> 0 Then
                                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                                            If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                            Else
                                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                            End If

                                                        End If
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            ' --- HORARIOS
                            If myConceptCause.IDType = CompositionType.Shift AndAlso myConcept.IDType = "O" Then
                                ' Para cada horario que acumula y en el caso que el sado sea de veces
                                If ShiftID = -1 Then
                                    ' Obtenemos los datos del horario
                                    GetShiftData(zDate, EmployeeID, ShiftID, isHolidays, ExpectedWorkingHoursShift)
                                End If

                                ' Si es el mismo horario que hay que acumular
                                If (myConceptCause.IDShift = ShiftID) Then

                                    ' Revisamos si el dia planificado es Natural/Laboral/No Laboral
                                    bolValid = False
                                    If myConceptCause.TypeDayPlanned = TypeDayPlanned.AllDays OrElse Not isHolidays Then
                                        ' Dia Natural
                                        bolValid = True
                                    ElseIf myConceptCause.TypeDayPlanned = TypeDayPlanned.Laboral AndAlso ExpectedWorkingHoursShift > 0 Then
                                        ' Dia Laboral
                                        bolValid = True
                                    ElseIf myConceptCause.TypeDayPlanned = TypeDayPlanned.Laboral AndAlso ExpectedWorkingHoursShift <= 0 Then
                                        ' Dia No Laboral
                                        bolValid = True
                                    End If

                                    If bolValid Then
                                        If myConceptCause.Conditions Is Nothing OrElse myConceptCause.Conditions.Count = 0 Then
                                            'Acumulamos de forma basica
                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor

                                            If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                            Else
                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                            End If
                                        Else
                                            ' Acumulamos si se cumple la regla
                                            If Execute_EvaluateConceptCauseRuleCondition(myConceptCause.Conditions, CausesSum, EmployeeID, zDate) Then
                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                                If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                Else
                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                End If

                                            End If
                                        End If
                                    End If
                                End If
                            End If

                            '-- AUSENCIAS PREVISTAS
                            If myConceptCause.IDType = CompositionType.Absence And myConcept.IDType = "O" Then
                                ' Para cada ausencia que acumula y en el caso que el sado sea de veces
                                If ShiftID = -1 Then
                                    ' Obtenemos los datos del horario
                                    GetShiftData(zDate, EmployeeID, ShiftID, isHolidays, ExpectedWorkingHoursShift)
                                End If

                                ' Comprueba si tiene ausencia prolongada activa con la justificación indicada en la composición
                                bolValid = InProgrammedAbsences(EmployeeID, zDate, zDate, myConceptCause.IDCause)
                                If bolValid Then
                                    bolValid = False
                                    If myConceptCause.TypeDayPlanned = TypeDayPlanned.AllDays Then
                                        ' Dia Natural
                                        bolValid = True
                                    ElseIf myConceptCause.TypeDayPlanned = TypeDayPlanned.Laboral AndAlso (ExpectedWorkingHoursShift > 0 AndAlso Not isHolidays) Then
                                        ' Dia Laboral
                                        bolValid = True
                                    ElseIf myConceptCause.TypeDayPlanned = TypeDayPlanned.NonLaboral AndAlso (ExpectedWorkingHoursShift <= 0 OrElse isHolidays) Then
                                        ' Dia No Laboral
                                        bolValid = True
                                    End If

                                    If bolValid Then
                                        ' Si se tiene que acumular
                                        If myConceptCause.Conditions Is Nothing OrElse myConceptCause.Conditions.Count = 0 Then
                                            'Acumulamos de forma basica
                                            myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor

                                            If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                            Else
                                                myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                            End If
                                        Else
                                            ' Acumulamos si se cumple la regla
                                            If Execute_EvaluateConceptCauseRuleCondition(myConceptCause.Conditions, CausesSum, EmployeeID, zDate) Then
                                                myConceptValue = myConceptValue + myConceptCause_OccurrencesFactor
                                                If (myConceptCause_OccurrencesFactor) >= 0 Then
                                                    myConceptPositiveValue = myConceptPositiveValue + myConceptCause_OccurrencesFactor
                                                Else
                                                    myConceptNegativeValue = myConceptNegativeValue + ((myConceptCause_OccurrencesFactor) * -1)
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        Next
                    End If

                    'myConcept.Value = myConceptValue
                    dConceptsValues(myConcept.ID).Value = myConceptValue

                    ' Si el saldo tiene caducidades guardamos los valores positivos y negativos
                    If myConcept.ApplyExpiredHours OrElse myConcept.DefaultQuery = "L" Then
                        dConceptsValues(myConcept.ID).PositiveValue = myConceptPositiveValue
                        dConceptsValues(myConcept.ID).NegativeValue = myConceptNegativeValue
                        If dConceptsValues(myConcept.ID).Value <> (dConceptsValues(myConcept.ID).PositiveValue - dConceptsValues(myConcept.ID).NegativeValue) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccualsManager.ExecuteSingleDay_DoAccruals:Differents between value and breakdown...")
                        End If
                    End If

                Next

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::ExecuteSingleDay_DoAccruals")

            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_DoAccruals_IsConceptActive(ByVal zDate As Date, ByRef oConcept As Object) As Boolean
            '
            ' Devuelve True si este concepto está activado para la fecha indicada
            '
            Dim bolRet As Boolean = False
            Dim Concept As roConceptEngine

            Try
                Concept = TryCast(oConcept, roConceptEngine)
                If Concept Is Nothing Then Return False

                If DateDiff("d", zDate, Concept.BeginDate) > 0 Then
                    bolRet = False
                ElseIf DateDiff("d", Concept.FinishDate, zDate) > 0 Then
                    bolRet = False
                Else
                    bolRet = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAccruals_IsConceptActive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAccruals_IsConceptActive")
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_DoAccruals_FIAT(ByVal myConcept As roConceptEngine, ByRef causesSum As roCollection, ByRef originalCausesSum As roCollection, ByRef compactCausesSum As roCollection, ByRef compactCausesOnlyNOCSum As roCollection, ByRef mCausesNOC As roCollection) As Boolean
            '
            ' Tratamiento especial FIAT
            '
            Dim bolRet As Boolean = False


            Try
                Dim i As Integer
                Dim Index As Integer
                Dim CauseID As Integer
                Dim CauseValue As Double

                If myConcept.AdvParameter <> "#NOC" Then
                    ' Si el saldo NO es nocturno, lo compacto (si no lo hice ya) por si se generaron justificaciones en zona nocturna (vendrán con un ID superior a 9000)
                    If compactCausesSum.Count = 0 Then
                        compactCausesSum.Clear()
                        compactCausesSum.ImportKeyandDoubleValue(originalCausesSum)

                        Dim idcauses() As Long
                        ReDim idcauses(compactCausesSum.Count)
                        i = 0
                        For CauseIndex = 1 To compactCausesSum.Count
                            idcauses(i) = compactCausesSum.Key(CauseIndex)
                            i = i + 1
                        Next

                        Dim ids2Delete() As Long
                        ReDim ids2Delete(compactCausesSum.Count)
                        i = 0
                        For CauseIndex = 1 To compactCausesSum.Count
                            CauseID = idcauses(CauseIndex - 1)
                            CauseValue = compactCausesSum(CauseID, roCollection.roSearchMode.roByKey)
                            If CauseID >= 90000 Then
                                ids2Delete(i) = CauseID
                                i = i + 1
                            End If
                        Next
                        ' Borro
                        If i > 0 Then
                            For Index = 0 To i - 1
                                compactCausesSum.Remove(ids2Delete(Index), roCollection.roSearchMode.roByKey)
                            Next
                        End If
                    End If

                    ' Cargo la colección compactada en la que usaré para el proceso de acumulado
                    causesSum.Clear()
                    causesSum.ImportKeyandDoubleValue(compactCausesSum)

                Else
                    ' Preparo la colección compactada, eliminando las justificaciones NOC que no se produjeron en nocturno
                    If compactCausesOnlyNOCSum.Count = 0 Then
                        compactCausesOnlyNOCSum.Clear()
                        compactCausesOnlyNOCSum.ImportKeyandDoubleValue(originalCausesSum)

                        Dim idCauses1() As Long
                        ReDim idCauses1(compactCausesOnlyNOCSum.Count)

                        i = 0
                        For CauseIndex = 1 To compactCausesOnlyNOCSum.Count
                            idCauses1(i) = compactCausesOnlyNOCSum.Key(CauseIndex)
                            i = i + 1
                        Next

                        Dim ids2Delete1() As Long
                        ReDim ids2Delete1(compactCausesOnlyNOCSum.Count)
                        i = 0
                        For CauseIndex = 1 To compactCausesOnlyNOCSum.Count
                            CauseID = idCauses1(CauseIndex - 1)
                            CauseValue = compactCausesOnlyNOCSum(CauseID, roCollection.roSearchMode.roByKey)
                            If CauseID >= 90000 Then
                                If compactCausesOnlyNOCSum.Exists(CauseID - 90000) Then
                                    ' Si existe el correspondiente en zona no nocturna, lo elimino
                                    compactCausesOnlyNOCSum(CauseID - 90000, roCollection.roSearchMode.roByKey) = CauseValue
                                Else
                                    ' No debería ocurrir ...
                                    compactCausesOnlyNOCSum.Add((CauseID - 90000), CauseValue)
                                End If
                                ids2Delete1(i) = CauseID
                                i = i + 1
                            Else
                                ' Si la justificación se debe acumular sólo si se produjo en nocturno, como no es el caso, la debo eliminar
                                If mCausesNOC.Exists(CauseID) AndAlso Not compactCausesOnlyNOCSum.Exists(CauseID + 90000) Then
                                    ids2Delete1(i) = CauseID
                                    i = i + 1
                                End If
                            End If
                        Next
                        ' Borro
                        If i > 0 Then
                            For Index = 0 To i - 1
                                compactCausesOnlyNOCSum.Remove(ids2Delete1(Index), roCollection.roSearchMode.roByKey)
                            Next
                        End If
                    End If

                    ' Cargo la colección compactada en la que usaré para el proceso de acumulado
                    causesSum.Clear()
                    causesSum.ImportKeyandDoubleValue(compactCausesOnlyNOCSum)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAccruals_FIAT")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAccruals_FIAT")

            End Try

            Return bolRet

        End Function

        Private Function GetShiftData(ByVal zDate As Date, ByVal EmployeeID As Long, ByRef ShiftID As Double, ByRef isHolidays As Boolean, ByRef ExpectedWorkingHoursShift As Double) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtenemos el ID del horario utilizado ese día y las horas teoricas
                ShiftID = Any2Double(ExecuteScalar("@SELECT# IDShiftUsed FROM DailySchedule with (nolock) WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime))

                ' Si el horario es de vacaciones
                isHolidays = Any2Boolean(ExecuteScalar("@SELECT# isnull(isHolidays, 0) FROM DailySchedule with (nolock)  WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime))
                If isHolidays Then
                    ' Obtenemos las horas teoricas del horario base
                    ExpectedWorkingHoursShift = Any2Double(ExecuteScalar("@SELECT# isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase  "))
                Else
                    ' Obtenemos las horas teoricas del horario
                    ExpectedWorkingHoursShift = Any2Double(ExecuteScalar("@SELECT# isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetShiftData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetShiftData")
            End Try

            Return bolRet

        End Function

        Private Function InProgrammedAbsences(ByVal IDEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal IDCause As Long) As Boolean

            Dim strSQL As String

            Dim bolRet As Boolean = False

            Try

                ' Indica si existe ausencia en el periodo indicado con la justificacion concreta
                strSQL = "@SELECT# Count(*) " &
                     "FROM ProgrammedAbsences " &
                                "LEFT JOIN Causes On Causes.ID = ProgrammedAbsences.IDCause " &
                     "WHERE idEmployee = " & IDEmployee.ToString & " AND IDCause=" & IDCause & " AND " &
                           "((BeginDate BETWEEN " & Any2Time(BeginDate).SQLSmallDateTime & " AND " & Any2Time(EndDate).SQLSmallDateTime & ") OR " &
                            "((CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) BETWEEN " & Any2Time(BeginDate).SQLSmallDateTime & " AND " & Any2Time(EndDate).SQLSmallDateTime & ") OR " &
                            "(BeginDate < " & Any2Time(BeginDate).SQLSmallDateTime & " AND " &
                             "(CASE WHEN FinishDate IS NULL THEN DATEADD(day, MaxLastingDays-1, BeginDate) ELSE FinishDate END) > " & Any2Time(EndDate).SQLSmallDateTime & ")) "

                If Any2Double(ExecuteScalar(strSQL)) > 0 Then bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: InProgrammedAbsences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: InProgrammedAbsences")
            End Try

            Return bolRet

        End Function

        Private Function Execute_SaveData(ByVal EmployeeID As Long, ByVal zDate As roTime) As Boolean
            '
            ' Guarda los datos de acumulados en la tabla
            '
            Dim IDConcept As Long = 0
            Dim ExpiredDate As New roTime
            Dim m_ExpiredHoursCriteria As New roCollection

            Dim bolRet As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Primero eliminamos datos anteriores
                If Not ExecuteSql("@DELETE# FROM DailyAccruals WHERE isnull(CarryOver,0)=0 AND IDEmployee=" & EmployeeID.ToString & " AND Date=" & zDate.SQLSmallDateTime) Then
                    oState.Result = EngineResultEnum.ErrorSavingData
                    Return False
                End If

                Dim DBAnnualWorkPeriod As DataTable = Nothing

                For Each myConcept As roConceptEngine In oConceptsCache.Values
                    If dConceptsValues(myConcept.ID).Value <> 0 Then
                        ' El concepto tiene un valor a grabar
                        If dConceptsValues(myConcept.ID).Value > roMaxTimeNumericValue Then
                            dConceptsValues(myConcept.ID).Value = roMaxTimeNumericValue
                        End If

                        'Redondea si es necesario
                        If myConcept.RoundConceptBy <> 1 And myConcept.RoundConceptBy <> 0 Then
                            If dConceptsValues(myConcept.ID).Value >= 0 Then
                                dConceptsValues(myConcept.ID).Value = Any2Time(Date.FromOADate(RoundTime(Any2Time(dConceptsValues(myConcept.ID).Value, True).VBNumericValue, myConcept.RoundConveptType, myConcept.RoundConceptBy)), True).NumericValue(True)
                            Else
                                dConceptsValues(myConcept.ID).Value = dConceptsValues(myConcept.ID).Value * -1
                                dConceptsValues(myConcept.ID).Value = (Any2Time(Date.FromOADate(RoundTime(Any2Time(dConceptsValues(myConcept.ID).Value, True).VBNumericValue, myConcept.RoundConveptType, myConcept.RoundConceptBy)), True).NumericValue(True)) * -1
                            End If
                        End If

                        'Graba si al redondear es superior a 0 o si tiene caducidades
                        If dConceptsValues(myConcept.ID).Value <> 0 OrElse ((myConcept.ApplyExpiredHours Or myConcept.DefaultQuery = "L") AndAlso (dConceptsValues(myConcept.ID).PositiveValue <> 0 OrElse dConceptsValues(myConcept.ID).NegativeValue <> 0)) Then
                            ' Graba
                            If Not ExecuteSql("@INSERT# INTO DailyAccruals (IDEmployee,Date,IDConcept,Value) VALUES (" & EmployeeID.ToString & "," & zDate.SQLSmallDateTime & "," & myConcept.ID.ToString &
                                        "," & dConceptsValues(myConcept.ID).Value.ToString.Replace(",", ".") & ")") Then
                                oState.Result = EngineResultEnum.ErrorSavingData
                                Return False
                            End If

                            ' marcamos para actualizar las fechas del tramo correspondiente en el caso que el saldo sea de tipo año laboral "L"
                            ' y el valor sea negativo
                            If myConcept.DefaultQuery = "L" AndAlso dConceptsValues(myConcept.ID).Value < 0 Then bolSetAnnualWorkPeriod = True

                        End If

                        ' Actualizamos los valores de las caducidades en caso necesario
                        If (myConcept.ApplyExpiredHours Or myConcept.DefaultQuery = "L") AndAlso (dConceptsValues(myConcept.ID).PositiveValue <> 0 OrElse dConceptsValues(myConcept.ID).NegativeValue <> 0) Then
                            ExpiredDate = New roTime
                            ExpiredDate = Any2Time(roNullDate)

                            ' Calculamos la fecha de caducidad en caso de guardar valores positivos
                            If myConcept.ExpiredHoursCriteria IsNot Nothing AndAlso dConceptsValues(myConcept.ID).PositiveValue > 0 AndAlso myConcept.DefaultQuery <> "L" Then

                                Select Case myConcept.ExpiredHoursCriteria.ExpiredHoursType
                                    Case eExpiredHoursType.DaysType
                                        ExpiredDate = zDate.Add(Any2Double(myConcept.ExpiredHoursCriteria.Value), "d")
                                    Case eExpiredHoursType.MonthType
                                        ExpiredDate = zDate.Add(Any2Double(myConcept.ExpiredHoursCriteria.Value), "m")
                                End Select
                            End If
                            If Not ExecuteSql("@UPDATE# DailyAccruals Set PositiveValue= " & dConceptsValues(myConcept.ID).PositiveValue.ToString.Replace(",", ".") & ", NegativeValue=" & dConceptsValues(myConcept.ID).NegativeValue.ToString.Replace(",", ".") & ",ExpiredDate=" & IIf(ExpiredDate.Value = Any2Time(roNullDate).Value, "NULL", ExpiredDate.SQLSmallDateTime) &
                                              " WHERE IDEmployee= " & EmployeeID & " AND Date=" & zDate.SQLSmallDateTime & " AND IDConcept=" & myConcept.ID) Then
                                oState.Result = EngineResultEnum.ErrorSavingData
                                Return False
                            End If

                            ' Marcamos para recaculo la fecha en la que caduca el valor en caso de ser un dia anterior a hoy
                            If Not ExecuteSingleDay_MarkNextExpiredDate(EmployeeID, ExpiredDate) Then
                                Return False
                            End If
                        End If
                    End If
                Next
                bolRet = True
            Catch ex As Data.Common.DbException
                If ex.Message.Contains("Cannot insert duplicate key") Then
                    oState.Result = EngineResultEnum.ErrorSavingData
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager:: Execute_SaveData: Cannot insert duplicate key in object 'dbo.DailyAccruals'. Aborted. It will be retried soon")
                Else
                    oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_SaveData")
                End If
            Catch ex As Exception
                If ex.Message.Contains("Collection was modified") Then
                    oState.Result = EngineResultEnum.AccrualsCacheModified
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager:: Execute_SaveData: Concepts cache modified during process. Aborted. It will be retried soon")
                Else
                    oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_SaveData")
                End If
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_MarkNextExpiredDate(ByVal IDEmployee As Long, ByVal ExpiredDate As roTime) As Boolean
            '
            ' Actualizamos el estado del empleado/dia
            '

            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                oState.Result = EngineResultEnum.NoError

                If ExpiredDate IsNot Nothing AndAlso ExpiredDate.Value <> Any2Time(roNullDate).Value Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ", GUID='' WHERE IDEmployee = " & IDEmployee.ToString & " AND Date = " & ExpiredDate.SQLSmallDateTime & "  and Date <= getdate() AND Status >" & roCausesManager.mPriority.ToString & " AND ISNULL(GUID, '') <> '" & VTBase.roConstants.GetManagedThreadGUID() & "'"
                    Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                    Dim nRet As Integer = oSqlCommand.ExecuteNonQuery()

                    If nRet > 0 Then
                        ' Marcamos para lanzar la tarea de recalculo solo si se ha actualizado algun registro
                        mMarkNextExpiredDate = True
                    End If
                End If

                bolRet = True

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_MarkNextExpiredDate:" & IDEmployee.ToString & " " & ExpiredDate.Value)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_MarkNextExpiredDate" & IDEmployee.ToString & " " & ExpiredDate.Value)
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_MarkNextAnnualWorkDates(ByVal IDEmployee As Long, ByVal TaskDate As roTime, ByVal IDConcept As Integer) As Boolean
            '
            ' Actualizamos el estado del empleado/dia que hayan generado valores en el saldo anual laboral hasta el dia de hoy
            ' a partir de la fecha de calculo

            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                oState.Result = EngineResultEnum.NoError

                Dim strWhere As String = " = " & IDConcept.ToString & " "
                If IDConcept = -1 Then
                    strWhere = " IN (@SELECT# ID FROM Concepts WHERE DefaultQuery = 'L') "
                End If

                If TaskDate IsNot Nothing AndAlso TaskDate.Value < Any2Time(Now.Date).Value Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & IIf(IDConcept > 0, ", GUID='' ", ", GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' ") &
                                " WHERE IDEmployee = " & IDEmployee.ToString & " AND Date > " & TaskDate.SQLSmallDateTime & "  and Date <= getdate() " &
                                " AND Status >" & roCausesManager.mPriority.ToString & " AND ISNULL(GUID, '') <> '" & VTBase.roConstants.GetManagedThreadGUID() & "' " &
                                " AND EXISTS (
                                            @SELECT#
					                            1 AS ExistReg
				                            FROM DailyAccruals WITH (NOLOCK)
				                            WHERE DailyAccruals.IDConcept " & strWhere &
                                          " AND DailyAccruals.Date = DailySchedule.Date
                                            AND DailyAccruals.IDEmployee = DailySchedule.IDEmployee)"
                    Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                    Dim nRet As Integer = oSqlCommand.ExecuteNonQuery()

                    If nRet > 0 And IDConcept > 0 Then
                        ' Marcamos para lanzar la tarea de recalculo solo si se ha actualizado algun registro
                        mMarkNextExpiredDate = True
                    End If
                End If

                bolRet = True

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_MarkNextAnnualWorkDates:" & IDEmployee.ToString & " " & TaskDate.Value)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_MarkNextAnnualWorkDates" & IDEmployee.ToString & " " & TaskDate.Value)
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_UpdateStatus(ByVal IDEmployee As Long, ByVal TaskDate As roTime) As Boolean
            '
            ' Actualizamos el estado del empleado/dia
            '

            Dim bolRet As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Indicamos que hemos acumulado esta fecha
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(IDEmployee, TaskDate.ValueDateTime, mPreviousProcessPriority, oState)
                If Not mGUIDChanged Then
                    bolRet = ExecuteSql("@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=" & mPriority.ToString & " WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & TaskDate.SQLSmallDateTime)
                Else
                    bolRet = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_UpdateStatus:" & IDEmployee.ToString & " " & TaskDate.Value)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_UpdateStatus" & IDEmployee.ToString & " " & TaskDate.Value)
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay_DoShiftDailyRules(ByVal EmployeeID As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Ejecuta la tarea indicada
            '
            Dim zDate As New roTime
            Dim zShift As roShiftEngine

            Dim bolRet As Boolean = False
            Dim DBManualCenters As DataTable
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtiene empleado y fecha de la tarea
                zDate = Any2Time(TaskDate)

                ' Antes de eliminar las justificaciones generadas por las reglas diarias
                ' nos guardamos las que tengan un centro de coste manual,
                ' y en el caso que se vuelva a generar la misma justificacion
                ' asignar de nuevo el mismo centro
                strSQL = "@SELECT# IDCause,IDCenter,Value,isnull(DefaultCenter,0) as DefaultCenter  FROM DailyCauses WHERE IDEmployee=" & EmployeeID &
                         " AND Date=" & zDate.SQLSmallDateTime & " AND DailyRule=1 AND ManualCenter=1 "
                DBManualCenters = CreateDataTable(strSQL)

                ' Eliminamos las justificaciones que se hayan generado previamente con las reglas diarias
                ExecuteSql("@DELETE# FROM DailyCauses WHERE IDEmployee=" & EmployeeID & " AND Date = " & zDate.SQLSmallDateTime & " AND DailyRule=1")

                ' Obtiene horario utilizado
                zShift = roBaseEngineManager.Execute_GetShift(EmployeeID, zDate, bolIsHolidays, bolProgrammedAbsenceOnHolidays, oState)

                ' Si tenemos horario  y reglas diarias
                If zShift IsNot Nothing AndAlso zShift.DailyRules IsNot Nothing AndAlso zShift.DailyRules.Count > 0 Then
                    ' En el caso que el horario tenga reglas diarias las procesamos
                    For Each Rule As roShiftDailyRule In zShift.DailyRules
                        ' Obtenemos las justificaciones diarias actuales
                        Dim DBCauses As DataTable = GetDailyCauses(EmployeeID, TaskDate)

                        ' Evalua regla diaria
                        If Execute_EvaluateShiftDailyRule(Rule, DBCauses, EmployeeID, TaskDate) Then
                            ' Si la regla se cumple, se aplican las acciones
                            Execute_ApplyShiftDailyRule(Rule, DBCauses, EmployeeID, TaskDate, DBManualCenters)
                        End If
                        If oState.Result <> EngineResultEnum.NoError Then
                            oState.Result = EngineResultEnum.ErrorApplyingDailyConceptsRules
                            Return False
                        End If
                    Next
                End If

                ' Revisamos el tratamiento de las horas extras en latam, en caso necesario
                ' Si el parametro avanzado esta activo y el convenio tiene habilitado un comportamiento concreto
                If bolLatam_OvertimeManagement Then
                    Dim oCurrentContract As New VTEmployees.Contract.roContract
                    oCurrentContract = Contract.roContract.GetContractInDateLite(mIDEmployee, TaskDate, New Contract.roContractState(oState.IDPassport))
                    If oCurrentContract IsNot Nothing AndAlso oCurrentContract.LabAgree IsNot Nothing Then
                        Dim IDLabAgree As Integer = Any2Integer(oCurrentContract.LabAgree.ID)
                        Dim oLabAgreeEngine As roLabAgreeEngine = roBaseEngineManager.GetLabAgreeeFromCache(IDLabAgree, oState)
                        If oLabAgreeEngine IsNot Nothing AndAlso oLabAgreeEngine.ExtraHoursConfiguration <> LabAgreeExtraHoursConfiguration.Disabled Then
                            If oLabAgreeEngine.ExtraHoursIDCauseDoubles <> 0 AndAlso oLabAgreeEngine.ExtraHoursIDCauseSimples.Length > 0 AndAlso oLabAgreeEngine.ExtraHoursIDCauseTriples <> 0 Then
                                bolRet = ExecuteSingleDay_DoLatamOverTime(EmployeeID, TaskDate, DBManualCenters, oLabAgreeEngine, oCurrentContract)
                                If oState.Result <> EngineResultEnum.NoError Then
                                    oState.Result = EngineResultEnum.ErrorApplyingLatamOverTime
                                    Return False
                                End If
                            End If
                        End If
                    End If
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoShiftDailyRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoShiftDailyRules")
            End Try

            Return bolRet

        End Function

        Private Sub ExecuteSingleDay_DoAutomaticAccruals(ByVal EmployeeID As Long, ByRef TaskDate As Date, ByVal zShift As roShiftEngine)
            '
            ' Calculamos los devengos de cada acumulados y generamos la justificación correspondiente
            '
            Dim DBCauses As DataTable
            Dim zDate As New roTime
            Dim AccrualValue As Double
            Dim DBManualCenters As DataTable
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                zDate = Any2Time(TaskDate)

                ' Obtenemos las justificaciones generadas
                DBCauses = GetDailyCauses(EmployeeID, TaskDate)

                ' Antes de eliminar las justificaciones generadas por las reglas diarias
                ' nos guardamos las que tengan un centro de coste manual,
                ' y en el caso que se vuelva a generar la misma justificacion
                ' asignar de nuevo el mismo centro

                strSQL = "@SELECT# IDCause,IDCenter,Value,isnull(DefaultCenter,0) as DefaultCenter FROM DailyCauses WHERE IDEmployee=" & EmployeeID &
                         " AND Date=" & zDate.SQLSmallDateTime & " AND AccruedRule=1 AND ManualCenter=1 "
                DBManualCenters = CreateDataTable(strSQL)

                ' Eliminamos las justificaciones que se hayan generado previamente con devengos automaticos
                strSQL = "@DELETE# FROM DailyCauses WHERE IDEmployee= " & EmployeeID.ToString & " AND Date=" & zDate.SQLSmallDateTime & " AND Manual=0 AND AccruedRule=1"
                If Not ExecuteSql(strSQL) Then
                    oState.Result = EngineResultEnum.ErrorDeletingManualCauses
                End If

                For Each myConcept As roConceptEngine In oConceptsCache.Values
                    ' Solo procesamos si el concepto está activo en esta fecha
                    If ExecuteSingleDay_DoAccruals_IsConceptActive(zDate.Value, myConcept) Then
                        ' Si el saldo tiene devengo automatico
                        If myConcept.AutomaticAccrualType > eAutomaticAccrualType.DeactivatedType Then
                            If Not myConcept.AutomaticAccrualCriteria Is Nothing Then
                                ' Generamos el valor del devengo automatico
                                AccrualValue = GenerateAccrualValue(myConcept.AutomaticAccrualCriteria, DBCauses, EmployeeID, zDate.Value, zShift, myConcept)
                                If AccrualValue <> 0 Then CreateAutomaticAccrualCause(EmployeeID, zDate, AccrualValue, myConcept.AutomaticAccrualIDCause, DBManualCenters)
                                If oState.Result <> EngineResultEnum.NoError Then
                                    oState.Result = EngineResultEnum.ErrorDoingAutomaticAccruals
                                    Return
                                End If
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "ExecuteSingleDay_DoAutomaticAccruals: Automatic Accrual Criteria is empty. Concept:" & myConcept.ID)
                            End If
                        End If
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAutomaticAccruals")
            Catch ex As Exception
                If ex.Message.Contains("Collection was modified") Then
                    oState.Result = EngineResultEnum.AccrualsCacheModified
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "ExecuteSingleDay_DoAutomaticAccruals: Concepts cache modified during process. Aborted. It will be retried soon")
                Else
                    oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAutomaticAccruals")
                End If
            End Try

        End Sub

        Private Sub CreateAutomaticAccrualCause(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal pCauses As Double, ByVal zManualCenters As DataTable)
            '
            '   Crea Justificación resultante del devengo automatico
            '

            Dim dDefaultCenter As Double
            Dim bExistsManual As Boolean

            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                strSQL = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & IDEmployee &
                         " AND DailyRule= 0 AND AccruedRule = 1 AND AccrualsRules=0 AND IDRelatedIncidence = 0 AND IDCause = " & pCauses.ToString & " AND Date=" & TaskDay.SQLSmallDateTime

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                Dim da As DbDataAdapter
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                Dim oRow As DataRow

                ' Seleccionamos las justificaciones del mismo tipo para ese empleado y ese día

                ' Registramos la nueva justificación
                If zCauses.Rows.Count = 0 Then
                    oRow = zCauses.NewRow
                    oRow("IDEmployee") = IDEmployee
                    oRow("Date") = TaskDay.Value
                    oRow("IDCause") = pCauses
                    oRow("Value") = pValue
                    oRow("AccrualsRules") = 0
                    oRow("IDRelatedIncidence") = 0
                    oRow("DailyRule") = 0
                    oRow("AccruedRule") = 1
                    oRow("Manual") = 0
                    zCauses.Rows.Add(oRow)
                Else
                    ' Ya existía una justificación de este tipo para este empleado y día, luego acumulo el valor al ya existente
                    oRow = zCauses.Rows(0)
                    oRow("Value") = oRow("Value") + pValue
                End If

                ' Si tiene centro de coste lo asignamods a su centro de coste por defecto
                If mCostCenterInstallled Then
                    bExistsManual = False

                    ' En el caso que previamente existiera
                    ' la misma justificacion con un centro manual,
                    ' lo asignamos a ese centro
                    For Each oRowDC As DataRow In zManualCenters.Rows
                        If pCauses = Any2Double(oRowDC("IDCause")) AndAlso Any2Time(pValue).VBNumericValue = Any2Time(Any2Double(oRowDC("Value"))).VBNumericValue Then
                            oRow("IDCenter") = oRowDC("IDCenter")
                            oRow("DefaultCenter") = IIf(oRowDC("DefaultCenter"), 1, 0)
                            oRow("ManualCenter") = 1
                            bExistsManual = True
                            Exit For
                        End If
                    Next

                    If Not bExistsManual Then
                        ' En cualquier otro caso asignamos el centro de coste por defecto
                        ' del empleado
                        dDefaultCenter = roBaseEngineManager.GetDefaultCenter(IDEmployee, TaskDay, oState)
                        oRow("IDCenter") = dDefaultCenter
                        oRow("DefaultCenter") = 1
                        oRow("ManualCenter") = 0
                    End If

                End If

                da.Update(zCauses)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyPlusCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyPlusCause")
            End Try

        End Sub

        Private Sub ExecuteSingleDay_DoRules(ByVal IDEmployee As Long, ByVal TaskDate As roTime)
            '
            ' Ejecuta el proceso de reglas de acumulados
            '
            Dim iExRule As Integer
            Dim CurrentRule As roLabAgreeEngineAccrualRule
            Dim oTmpCurrentRule As New roLabAgreeEngineAccrualRule
            Dim oLabAgreeAccrualRules As New Generic.List(Of roLabAgreeEngineAccrualRule)
            Dim oExecutedLabAgreeAccrualRules As New Generic.List(Of roLabAgreeEngineAccrualRule)
            Dim mEmployeeRulesExecuted As New roCollection  'Reglas ya ejecutadas

            Try

                oState.Result = EngineResultEnum.NoError

                mCreateConceptsRules = New roCollection

                ' Cargo las reglas de este empleado

                ' Reglas de caducidades
                ' *** A los saldos por contato o anuales laborables que tengan caducidades no se les pueden
                ' *** definir reglas de arrastre por ahora, en ese caso debemos revisar , si exsiten valores que caducan en la fecha
                ' *** de calculo
                ' La revisión de las caducidades en el caso de saldos anuales laborables se hace posteriormente al crear los valores iniciales  y la asignación tramo a los valores negativos
                ExecuteSingleDay_DoExpiredDateRules(IDEmployee, TaskDate)
                If Not oState.Result = EngineResultEnum.NoError Then
                    Exit Sub
                End If

                ' Reglas de arrastre
                Dim IDContract As String = String.Empty
                Dim IDLabAgree As Integer
                LoadEmployee_Rules_Live(IDEmployee, TaskDate, IDContract, IDLabAgree)

                mEmployeeRulesExecuted.Clear()
                oExecutedLabAgreeAccrualRules.Clear()

                Dim DBAnnualWorkPeriod As DataTable = Nothing

                For iExRule = 1 To mEmployeeRules.Count
                    Dim oLabAgreeEngine As roLabAgreeEngine
                    oLabAgreeEngine = roBaseEngineManager.GetLabAgreeeFromCache(IDLabAgree, oState)
                    oLabAgreeAccrualRules = oLabAgreeEngine.LabAgreeAccrualRules

                    CurrentRule = oLabAgreeAccrualRules.FirstOrDefault(Function(x) x.IDAccrualRule = mEmployeeRules.Key(iExRule))

                    ' Clonamos regla
                    oTmpCurrentRule = roLabAgreeManager.CloneLabAgreeAccrualRule(CurrentRule)

                    'Asignamos el periodo de validez limitado al periodo del contrato
                    oTmpCurrentRule.BeginDate = Any2Time(String2Item(mEmployeeRules.Item(iExRule, roCollection.roSearchMode.roByIndex), 0, "@")).Value
                    oTmpCurrentRule.EndDate = Any2Time(String2Item(mEmployeeRules.Item(iExRule, roCollection.roSearchMode.roByIndex), 1, "@")).Value

                    Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(oState.IDPassport))
                    If oLabAgreeManager.AccrualRuleApplyOnDate(IDEmployee, oTmpCurrentRule, TaskDate.Value, DBAnnualWorkPeriod) Then
                        ' Miramos si ya se ha aplicado una regla igual a esta,
                        ' en caso de ser asi no la ejecutamos
                        Dim oDuplicatedLabagree As roLabAgreeEngineAccrualRule
                        oDuplicatedLabagree = oExecutedLabAgreeAccrualRules.FirstOrDefault(Function(x) oTmpCurrentRule.LabAgreeRule.Definition.Action = x.LabAgreeRule.Definition.Action _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.Comparation = x.LabAgreeRule.Definition.Comparation _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.DestiAccrual = x.LabAgreeRule.Definition.DestiAccrual _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.Dif = x.LabAgreeRule.Definition.Dif _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.Until = x.LabAgreeRule.Definition.Until _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.MainAccrual = x.LabAgreeRule.Definition.MainAccrual _
                                                                                    AndAlso ((oTmpCurrentRule.LabAgreeRule.Definition.UntilUserField Is Nothing AndAlso x.LabAgreeRule.Definition.UntilUserField Is Nothing) OrElse (oTmpCurrentRule.LabAgreeRule.Definition.UntilUserField IsNot Nothing AndAlso x.LabAgreeRule.Definition.UntilUserField IsNot Nothing AndAlso oTmpCurrentRule.LabAgreeRule.Definition.UntilUserField.FieldName = x.LabAgreeRule.Definition.UntilUserField.FieldName)) _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.Value = x.LabAgreeRule.Definition.Value _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.ValueIDConcept = x.LabAgreeRule.Definition.ValueIDConcept _
                                                                                    AndAlso oTmpCurrentRule.LabAgreeRule.Definition.ValueType = x.LabAgreeRule.Definition.ValueType _
                                                                                    AndAlso ((oTmpCurrentRule.LabAgreeRule.Definition.ValueUserField Is Nothing AndAlso x.LabAgreeRule.Definition.ValueUserField Is Nothing) OrElse (oTmpCurrentRule.LabAgreeRule.Definition.ValueUserField IsNot Nothing AndAlso x.LabAgreeRule.Definition.ValueUserField IsNot Nothing AndAlso oTmpCurrentRule.LabAgreeRule.Definition.ValueUserField.FieldName = x.LabAgreeRule.Definition.ValueUserField.FieldName)))
                        If oDuplicatedLabagree Is Nothing Then
                            mEmployeeRulesExecuted.Add(oTmpCurrentRule.IDAccrualRule)
                            oExecutedLabAgreeAccrualRules.Add(oTmpCurrentRule)
                            ExecuteSingleDay_DoRulesingleDayLive(IDEmployee, oTmpCurrentRule, TaskDate)
                            If oState.Result <> EngineResultEnum.NoError Then
                                oState.Result = EngineResultEnum.ErrorDoingSingleDayRules
                                Exit Sub
                            End If
                        End If
                    End If
                Next

                ' En caso necesario guardamos los valores de los saldos a aplicar generados por las reglas de saldos
                CreateConceptsRules()

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoRules")
            End Try

        End Sub

        Private Sub ExecuteSingleDay_DoRulesingleDayLive(ByVal IDEmployee As Long, ByRef rule As roLabAgreeEngineAccrualRule, ByVal TaskDay As roTime)
            '
            '   Ejecuta el calculo de las reglas de acumulados version Live
            '
            Dim MainAccrualValue As Double
            Dim subAcrualValue As Double
            Dim strSQL As String
            Dim maxCauseValue As Double
            Dim StartDateDayM As Date
            Dim StartDateDayY As Date
            Dim StartDateDayW As Date
            Dim ConceptType As String
            Dim StartAccrualDate As String = String.Empty
            Dim StartContractDate As String
            Dim UntilValue As Double
            Dim DateFieldValue As Double
            Dim intApplyDay As Integer

            Dim strRuleDescription As String
            Dim AccrualRuleCauseShortNameNextDay As String
            Dim AccrualRuleValueCauseNextDay As Double
            Dim i As Integer
            Dim bAccrualRuleAplied As Boolean
            Dim EndContractDate As String

            Try

                ' NUEVO FUNCIONAMIENTO DE REGLAS: Acumulamos todo (resultados de reglas y valores iniciales) desde el inicio de período (y no desde la última aplicación de la regla)

                oState.Result = EngineResultEnum.NoError

                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados mensuales
                StartDateDayM = GetStartDateForMonthAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados anuales
                StartDateDayY = GetStartDateForYearAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día de inicio de semana
                StartDateDayW = GetStartDateForWeekAccruals(intWeekIniday, TaskDay.Value)

                ' Obtenemos fecha de inicio de contrato actual
                ' y calculamos a partir de esa fecha
                StartContractDate = Any2String(ExecuteScalar("@SELECT# TOP 1 BeginDate FROM EmployeeContracts WITH(NOLOCK) WHERE IDEmployee = " & IDEmployee & " AND BeginDate <= " & TaskDay.SQLSmallDateTime & " ORDER BY BeginDate DESC"))

                ' Recupero el tipo de acumulado (mensual o anual). El valor del acumulado se calculará en función de dicho tipo
                ConceptType = ExecuteScalar("@SELECT# DefaultQuery FROM Concepts WITH(NOLOCK) WHERE ID = " & rule.LabAgreeRule.Definition.MainAccrual.ToString)

                ' Calculamos el valor del acumulado principal desde el primer día del período actual
                ' y hata el día de la tarea que estamos tratando
                Select Case ConceptType
                    Case "Y"
                        ' Si el acumulado es anual
                        StartAccrualDate = Any2Time(StartDateDayY).SQLSmallDateTime
                    Case "M"
                        ' Si el acumulado es mensual
                        StartAccrualDate = Any2Time(StartDateDayM).SQLSmallDateTime
                    Case "W"
                        ' Si el acumulado es semanal
                        StartAccrualDate = Any2Time(StartDateDayW).SQLSmallDateTime
                    Case "C"
                        ' Si el acumulado es por contrato
                        If IsDate(StartContractDate) Then
                            StartAccrualDate = Any2Time(StartContractDate).SQLSmallDateTime
                        Else
                            StartAccrualDate = Any2Time(roNullDate).SQLSmallDateTime
                        End If
                    Case "L"
                        ' Si el acumulado es anual laboral
                        Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, TaskDay.Value, New roContractState(-1))
                        If lstDates.Count > 0 Then
                            StartAccrualDate = Any2Time(lstDates(0)).SQLSmallDateTime
                        Else
                            StartAccrualDate = Any2Time(roNullDate).SQLSmallDateTime
                        End If
                End Select

                strSQL = " @SELECT# SUM(Value) FROM DailyAccruals WITH(NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND IDConcept=" & rule.LabAgreeRule.Definition.MainAccrual & " AND Date >= " & StartAccrualDate & " AND Date <= " & TaskDay.SQLSmallDateTime

                If IsDate(StartContractDate) Then
                    strSQL = strSQL & " AND Date >= " & Any2Time(StartContractDate).SQLSmallDateTime
                End If

                MainAccrualValue = Any2Double(ExecuteScalar(strSQL))

                ' Si se tiene que obtener el valor de una justificacion generada por otra regla
                If bolGetValueAccrualRuleNextDay Then
                    strRuleDescription = ExecuteScalar("@SELECT# ISNULL(Description,'') FROM AccrualsRules WHERE IdAccrualsRule = " & Any2Double(rule.IDAccrualRule))
                    If InStr(strRuleDescription, "#NEXTDAYVALUE#") > 0 Then
                        ' en la descripcion de la regla donde se quiere aplicar, el formato tiene que ser este
                        ' #NEXTDAYVALUE#ASH+#
                        ' #NEXTDAYVALUE# --> fijo
                        ' ASH --> nombre corto de la justificacion
                        ' + --> signo mas o menos como factor
                        ' # --> fijo

                        AccrualRuleCauseShortNameNextDay = Mid$(strRuleDescription, 15, 3)
                        AccrualRuleValueCauseNextDay = Any2Double(ExecuteScalar("@SELECT# ISNULL(Value,0) FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND Date=" & TaskDay.Add(1).SQLSmallDateTime & " AND AccrualsRules=1 and IDCause IN(@SELECT# ID FROM Causes WHERE Shortname LIKE '" & AccrualRuleCauseShortNameNextDay & "')"))
                        If Mid$(strRuleDescription, 18, 1) = "-" Then
                            MainAccrualValue = MainAccrualValue + (AccrualRuleValueCauseNextDay * -1)
                        Else
                            MainAccrualValue = MainAccrualValue + AccrualRuleValueCauseNextDay
                        End If
                    End If
                Else
                    ' En el caso que reglas anteriores hayan generado un valor en el saldo, lo tenemos en cuenta para el valor actual del mismo
                    If mCreateConceptsRules.Count > 0 Then
                        For i = 1 To mCreateConceptsRules.Count
                            If Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 2, "@")) = rule.LabAgreeRule.Definition.MainAccrual Then
                                MainAccrualValue = MainAccrualValue + Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 3, "@"))
                            End If
                        Next
                    End If
                End If
                Err.Clear()

                If rule.LabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.ConceptValue Then
                    ' El acumulado principal se compara con otro acumulado

                    ' Recupero el tipo de acumulado (mensual o anual)
                    ConceptType = ExecuteScalar("@SELECT# DefaultQuery FROM Concepts WHERE ID = " & Any2Double(rule.LabAgreeRule.Definition.ValueIDConcept))

                    Select Case ConceptType
                        Case "Y"
                            ' Si el acumulado es anual
                            StartAccrualDate = Any2Time(StartDateDayY).SQLSmallDateTime
                        Case "M"
                            ' Si el acumulado es mensual
                            StartAccrualDate = Any2Time(StartDateDayM).SQLSmallDateTime
                        Case "W"
                            ' Si el acumulado es semanal
                            StartAccrualDate = Any2Time(StartDateDayW).SQLSmallDateTime
                        Case "C"
                            ' Si el acumulado es por contrato
                            If IsDate(StartContractDate) Then
                                StartAccrualDate = Any2Time(StartContractDate).SQLSmallDateTime
                            Else
                                StartAccrualDate = Any2Time(roNullDate).SQLSmallDateTime
                            End If
                    End Select

                    strSQL = " @SELECT# SUM(Value) FROM DailyAccruals WITH(NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND IDConcept=" & Any2Double(rule.LabAgreeRule.Definition.ValueIDConcept) &
                             " AND Date >= " & StartAccrualDate & " AND Date <= " & TaskDay.SQLSmallDateTime

                    If IsDate(StartContractDate) Then
                        strSQL = strSQL & " AND Date >= " & Any2Time(StartContractDate).SQLSmallDateTime
                    End If

                    subAcrualValue = Any2Double(ExecuteScalar(strSQL))

                    ' En el caso que reglas anteriores hayan generado un valor en el saldo, lo tenemos en cuenta para el valor actual del mismo
                    If Not bolGetValueAccrualRuleNextDay Then
                        If mCreateConceptsRules.Count > 0 Then
                            For i = 1 To mCreateConceptsRules.Count
                                If Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 2, "@")) = rule.LabAgreeRule.Definition.ValueIDConcept Then
                                    subAcrualValue = subAcrualValue + Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 3, "@"))
                                End If
                            Next
                        End If
                    End If

                ElseIf rule.LabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.DirectValue Then
                    ' El acumulado principal se compara con una cantidad fija de tiempo
                    subAcrualValue = rule.LabAgreeRule.Definition.Value

                ElseIf rule.LabAgreeRule.Definition.ValueType = LabAgreeRuleDefinitionValueType.UserFieldValue Then
                    ' El acumulado principal se compara con un campo de la ficha
                    If rule.LabAgreeRule.Definition.ValueUserField IsNot Nothing Then
                        Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.LabAgreeRule.Definition.ValueUserField.FieldName, TaskDay.Value, New UserFields.roUserFieldState, False)
                        subAcrualValue = Any2Double(oUserField.FieldRawValue)
                    Else
                        subAcrualValue = 0
                    End If
                End If

                intApplyDay = 1
                If bolApplyCarryOverOnSameDate Then
                    ' En el caso que se tenga que aplicar el mismo dia
                    intApplyDay = 0
                End If

                If ComparationAccrual(MainAccrualValue, subAcrualValue, rule.LabAgreeRule.Definition.Comparation) Then
                    ' Obtenemos fecha de fin de contrato actual, no podemos crear ningun valor posterior a esa fecha
                    EndContractDate = Any2String(ExecuteScalar("@SELECT# TOP 1 EndDate FROM EmployeeContracts WITH(NOLOCK) WHERE IDEmployee = " & IDEmployee & " AND BeginDate <= " & TaskDay.SQLSmallDateTime & " ORDER BY BeginDate DESC"))
                    If TaskDay.Add(1).Value > Any2Time(EndContractDate).Value Then
                        Exit Sub
                    End If
                    Select Case rule.LabAgreeRule.Definition.Dif
                        Case LabAgreeRuleDefinitionDif.UntilValue 'El resultado es el valor del acumulado origen hasta un valor fijo
                            'Si el resultado es cero no hace falta hacer nada
                            UntilValue = roConversions.Min(MainAccrualValue, rule.LabAgreeRule.Definition.Until)

                            If UntilValue = 0 Then

                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            'Creamos la justificación resultante
                            CreateCause(IDEmployee, TaskDay.Add(1), UntilValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con el valor restado
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), UntilValue * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If

                        Case LabAgreeRuleDefinitionDif.Diff 'El resultado es la diferencia entre el acumulado origen y el valor o segundo acumulado
                            'Si la diferencia es cero no hace falta hacer nada
                            If (MainAccrualValue - subAcrualValue) = 0 Then
                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            'Creamos la justificación resultante
                            CreateCause(IDEmployee, TaskDay.Add(1), MainAccrualValue - subAcrualValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con el valor restado
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), (MainAccrualValue - subAcrualValue) * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If
                        Case LabAgreeRuleDefinitionDif.All 'El resultado es todo el tiempo del acumulado origen
                            'Creamos la justificación resultante
                            If MainAccrualValue = 0 Then
                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            CreateCause(IDEmployee, TaskDay.Add(1), MainAccrualValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con todo el valor
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), MainAccrualValue * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If
                        Case LabAgreeRuleDefinitionDif.Value 'El resultado es el valor fijo indicado
                            'Creamos la justificación resultante

                            'Como máximo copio/muevo la cantidad indicada en la regla como límite
                            maxCauseValue = IIf(MainAccrualValue >= rule.LabAgreeRule.Definition.Until, rule.LabAgreeRule.Definition.Until, MainAccrualValue)
                            If maxCauseValue = 0 Then
                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            CreateCause(IDEmployee, TaskDay.Add(1), maxCauseValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con el valor indicado
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), maxCauseValue * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If
                        Case LabAgreeRuleDefinitionDif.UserFieldUntilValue 'El resultado es el valor del acumulado origen hasta el valor de un campo de la ficha
                            'Si el resultado es cero no hace falta hacer nada
                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.LabAgreeRule.Definition.UntilUserField.FieldName, TaskDay.Value, New UserFields.roUserFieldState, False)
                            DateFieldValue = Any2Double(oUserField.FieldRawValue)

                            UntilValue = roConversions.Min(MainAccrualValue, DateFieldValue)

                            If UntilValue = 0 Then
                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            'Creamos la justificación resultante
                            CreateCause(IDEmployee, TaskDay.Add(1), UntilValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con el valor restado
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), UntilValue * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If

                        Case LabAgreeRuleDefinitionDif.UserFieldValue 'El resultado es el valor del fijo de un campo de la ficha
                            'Si el resultado es cero no hace falta hacer nada
                            If rule.LabAgreeRule.Definition.UntilUserField IsNot Nothing Then
                                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.LabAgreeRule.Definition.UntilUserField.FieldName, TaskDay.Value, New UserFields.roUserFieldState, False)
                                DateFieldValue = Any2Double(oUserField.FieldRawValue)
                            Else
                                DateFieldValue = 0
                            End If
                            'Como máximo copio/muevo la cantidad indicada en la regla como límite
                            maxCauseValue = IIf(MainAccrualValue >= DateFieldValue, DateFieldValue, MainAccrualValue)
                            If maxCauseValue = 0 Then
                                bAccrualRuleAplied = False
                                Exit Sub
                            End If
                            CreateCause(IDEmployee, TaskDay.Add(1), maxCauseValue, rule.LabAgreeRule.Definition.DestiAccrual)
                            bAccrualRuleAplied = True
                            If rule.LabAgreeRule.Definition.Action = 0 Then
                                'Mover: añadimos una incidencia al dia siguiente con el valor indicado
                                CreateConcept(IDEmployee, TaskDay.Add(intApplyDay), maxCauseValue * -1, rule.LabAgreeRule.Definition.MainAccrual)
                            End If
                    End Select
                Else
                    bAccrualRuleAplied = False
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoRulesingleDayLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoRulesingleDayLive")
            End Try
        End Sub

        Private Function ComparationAccrual(ByVal mainAc As Double, ByVal subAc As Double, ByVal Comparation As Integer) As Boolean
            '
            '   Compara dos acumulados dependiendo del tipo de comparació
            '   4: >; 3: <; 2: >=; 1: <= ; 0: =
            '
            ComparationAccrual = False
            Select Case Comparation
                Case 0 : If mainAc = subAc Then ComparationAccrual = True
                Case 1 : If mainAc <= subAc Then ComparationAccrual = True
                Case 2 : If mainAc >= subAc Then ComparationAccrual = True
                Case 3 : If mainAc < subAc Then ComparationAccrual = True
                Case 4 : If mainAc > subAc Then ComparationAccrual = True
            End Select

        End Function

        Private Sub CreateConceptsRules()
            '
            '   Creamos los valores en dailyaccruals a partir de la lista de valores
            '   
            Try
                Dim strSQL As String = ""
                Dim i As Integer

                oState.Result = EngineResultEnum.NoError

                If mCreateConceptsRules.Count > 0 Then
                    For i = 1 To mCreateConceptsRules.Count
                        ' Seleccionamos los acumulados del mismo tipo para este empleado y este día
                        strSQL = "@SELECT# * FROM DailyAccruals WHERE CarryOver=1 and StartupValue = 0 and IDEmployee=" & Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 0, "@")) &
                             " AND IDConcept = " & Any2String(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 2, "@")) & " AND Date=" & Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 1, "@")).SQLSmallDateTime & " and idconcept=" & Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 2, "@"))

                        If StringItemsCount(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), "@") > 4 Then
                            ' filtramos por el periodo del saldo de tipo "L"
                            strSQL += " AND BeginPeriod=" & Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 4, "@")).SQLSmallDateTime
                            strSQL += " AND EndPeriod=" & Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 5, "@")).SQLSmallDateTime
                        End If

                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim zAccruals As New DataTable("DailyAccruals")
                        Dim da As DbDataAdapter
                        da = CreateDataAdapter(cmd, True)
                        da.Fill(zAccruals)

                        Dim oRow As DataRow

                        If zAccruals.Rows.Count = 0 Then
                            ' No existía ningún acumulado de este tipo para este empleado y día, luego lo creo
                            oRow = zAccruals.NewRow
                            oRow("IDEmployee") = Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 0, "@"))
                            oRow("Date") = Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 1, "@")).Value
                            oRow("Value") = Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 3, "@"))
                            oRow("CarryOver") = 1
                            oRow("IDConcept") = Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 2, "@"))
                            If StringItemsCount(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), "@") > 4 Then
                                oRow("BeginPeriod") = Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 4, "@")).Value
                                oRow("EndPeriod") = Any2Time(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 5, "@")).Value
                            End If
                            ' Guarda datos en la base de datos
                            zAccruals.Rows.Add(oRow)
                        Else
                            ' Ya existía un acumulado de este tipo para este empleado y día, luego acumulo su valor al ya existente
                            oRow = zAccruals.Rows(0)
                            oRow("Value") = oRow("Value") + Any2Double(String2Item(mCreateConceptsRules(i, roCollection.roSearchMode.roByIndex), 3, "@"))
                        End If

                        da.Update(zAccruals)
                    Next

                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateConceptsRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateConceptsRules")
            End Try


        End Sub


        Private Sub LoadEmployee_Rules_Live(ByVal IDEmployee As Long, ByVal TaskDate As roTime, ByRef IDContract As String, ByRef IDLabAgree As Integer)
            '
            '  Carga lista de IDs de reglas que se aplican para este empleado/Contrato/Convenio. Las ordenamos de mayor a menor frecuencia (primero diarias, semanales, ...)
            '

            Dim strSQL As String
            Dim BeginDate As roTime
            Dim EndDate As roTime

            Dim BeginDateRule As roTime
            Dim EndDateRule As roTime

            Try

                oState.Result = EngineResultEnum.NoError

                mEmployeeRules.Clear()

                'Obtenemos el convenio del empleado y las fechas de contrato
                Dim oCurrentContract As New Contract.roContract
                oCurrentContract = Contract.roContract.GetContractInDateLite(IDEmployee, TaskDate.Value, New Contract.roContractState(oState.IDPassport))
                If oCurrentContract Is Nothing OrElse oCurrentContract.LabAgree Is Nothing Then Exit Sub

                IDContract = oCurrentContract.IDContract
                IDLabAgree = oCurrentContract.LabAgree.ID
                BeginDate = Any2Time(oCurrentContract.BeginDate)
                EndDate = Any2Time(oCurrentContract.EndDate)

                'Obtenemos las reglas de acumulado del convenio del empleado
                strSQL = "@SELECT# AccrualsRules.IDAccrualsRule,'Frecuency' = CASE "
                strSQL = strSQL & " WHEN Schedule LIKE 'D%' THEN 3 "
                strSQL = strSQL & " WHEN Schedule LIKE 'S%' THEN 2 "
                strSQL = strSQL & " WHEN Schedule LIKE 'M%' THEN 1 "
                strSQL = strSQL & " WHEN Schedule LIKE 'A%' THEN 0 END , LabAgreeAccrualsRules.BeginDate, LabAgreeAccrualsRules.EndDate "
                strSQL = strSQL & " FROM AccrualsRules WITH(NOLOCK) ,LabAgreeAccrualsRules WITH(NOLOCK) "
                strSQL = strSQL & " WHERE LabAgreeAccrualsRules.IDAccrualsRules = AccrualsRules.IDAccrualsRule"
                strSQL = strSQL & " AND  LabAgreeAccrualsRules.IDLabAgree = " & IDLabAgree
                strSQL = strSQL & " ORDER BY Frecuency DESC "

                Dim tbAccrualsRules As DataTable
                tbAccrualsRules = CreateDataTable(strSQL)

                ' Guardamos las reglas y sus periodos de validez
                If tbAccrualsRules IsNot Nothing AndAlso tbAccrualsRules.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbAccrualsRules.Rows
                        BeginDateRule = roConversions.Max(Any2Time(oRow("BeginDate")), BeginDate)
                        EndDateRule = roConversions.Min(Any2Time(oRow("EndDate")), EndDate)

                        ' Si el periodo es correcto
                        If BeginDateRule.VBNumericValue <= EndDateRule.VBNumericValue Then
                            mEmployeeRules.Add(Any2Integer(oRow("IDAccrualsRule")), Any2String(BeginDateRule.Value) & "@" & Any2String(EndDateRule.Value))
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: LoadEmployee_Rules_Live")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: LoadEmployee_Rules_Live")
            End Try
        End Sub

        Private Sub ExecuteSingleDay_DoExpiredDateRules(ByVal IDEmployee As Long, ByVal TaskDay As roTime, Optional ByVal bolAnnualWorkType As Boolean = False)
            '
            ' Validamos los valores de los saldos que tengan caducidad a dia de hoy
            '
            Dim strSQL As String
            Dim dblPositiveValue As Double
            Dim dblNegativeValue As Double
            Dim StartContractDate As String
            Dim dblTotalPeriodValue As Double
            Dim dblRuleValue As Double
            Dim dblTotalValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                Dim tbExpired As DataTable

                ' Obtenemos fecha de inicio de contrato actual
                ' y calculamos a partir de esa fecha
                StartContractDate = Any2String(ExecuteScalar("@SELECT# TOP 1 BeginDate FROM EmployeeContracts WITH (NOLOCK) where IDEmployee = " & IDEmployee & " AND BeginDate <= " & TaskDay.SQLSmallDateTime & " Order by BeginDate desc"))
                If Not IsDate(StartContractDate) Then Exit Sub

                Dim strFilterType As String = IIf(bolAnnualWorkType, "DefaultQuery = 'L'", "DefaultQuery = 'C'")

                ' Obtenemos los valores de los saldos por contrato (sólo los saldos por contrato tienen caducidad) , que tengan caducidades y la fecha de caducidad sea la del dia a procesar
                strSQL = "@SELECT# Date, PositiveValue, IDConcept,BeginPeriod,EndPeriod, (@SELECT# ISNULL(ExpiredIDCause, 0) FROM Concepts WITH (NOLOCK) WHERE ID=DailyAccruals.IDConcept) AS IDCause FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND ExpiredDate=" & TaskDay.SQLSmallDateTime & " AND IDCOncept IN(@SELECT# Id FROM Concepts where (" & strFilterType & ") " & IIf(Not bolAnnualWorkType, "AND ISNULL(ApplyExpiredHours,0) = 1", "") & ") AND Date >= " & roTypes.Any2Time(StartContractDate).SQLSmallDateTime & " ORDER BY Date, IDConcept"
                tbExpired = CreateDataTable(strSQL)

                If tbExpired IsNot Nothing AndAlso tbExpired.Rows.Count > 0 Then
                    Dim strWhereAnnualPeriod As String = ""
                    Dim bolDefaultQueryL As Boolean = False
                    For Each oExpiredRow As DataRow In tbExpired.Rows
                        bolDefaultQueryL = False
                        strWhereAnnualPeriod = ""

                        ' Para cada Fecha/saldo/valor
                        Dim oConcept As roConceptEngine = oConceptsCache.Item(Any2Integer(oExpiredRow("IDConcept")))
                        If oConcept IsNot Nothing AndAlso oConcept.DefaultQuery = "L" Then
                            strWhereAnnualPeriod = " AND BeginPeriod<= " & Any2Time(oExpiredRow("BeginPeriod")).SQLSmallDateTime
                            bolDefaultQueryL = True
                        End If

                        ' Obtenemos el valor total del saldo el dia antes a la fecha del saldo que caduca (Date - 1)
                        dblTotalPeriodValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(Value,0)) AS total FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND IDConcept = " & Any2Double(oExpiredRow("IDConcept")) & " AND Date >= " & Any2Time(StartContractDate).SQLSmallDateTime & " AND Date < " & Any2Time(oExpiredRow("Date")).SQLSmallDateTime))

                        ' Valor que caduca
                        dblPositiveValue = Any2Double(oExpiredRow("PositiveValue"))

                        ' Obtenemos el total de valores negativos entre la fecha del valor hasta el dia que caduca (las dos incluidas)
                        dblNegativeValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(NegativeValue,0)) AS NegativeValue FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND IDConcept = " & Any2Double(oExpiredRow("IDConcept")) & " AND Date >= " & Any2Time(oExpiredRow("Date")).SQLSmallDateTime & " AND Date <= " & TaskDay.SQLSmallDateTime & strWhereAnnualPeriod))

                        ' Obtenemos el total de valores de reglas de caducidad entre la fecha del valor hasta el dia anterior al que caduca (date)
                        dblRuleValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(Value,0)) AS total FROM DailyAccruals WITH (NOLOCK) WHERE IDEmployee=" & IDEmployee & " AND IDConcept = " & Any2Double(oExpiredRow("IDConcept")) & " AND Date >= " & Any2Time(oExpiredRow("Date")).SQLSmallDateTime & " AND Date < " & TaskDay.SQLSmallDateTime & " AND CarryOver=1 AND StartupValue=0 " & strWhereAnnualPeriod))

                        ' Sumamos todos los valores
                        dblTotalValue = dblTotalPeriodValue + dblPositiveValue - dblNegativeValue + dblRuleValue

                        ' Si el total es positivo debemos generar un ajuste en negativo del valor en caso de ser negativo no hacemos nada
                        If dblTotalValue > 0 Then
                            ' Creamos el valor en negativo del total para el saldo y la fecha en la que caduca
                            CreateConcept(IDEmployee, TaskDay, dblTotalValue * -1, Any2Double(oExpiredRow("IDConcept")), IIf(bolDefaultQueryL, oExpiredRow("BeginPeriod"), DateTime.MinValue), IIf(bolDefaultQueryL, oExpiredRow("EndPeriod"), DateTime.MinValue))

                            If Not bolAnnualWorkType Then
                                ' Creamos la justificacion caducada con el valor en el dia siguiente
                                CreateCause(IDEmployee, TaskDay.Add(1), dblTotalValue, Any2Double(oExpiredRow("IDCause")))
                                If oState.Result <> EngineResultEnum.NoError Then
                                    oState.Result = EngineResultEnum.ErrorDoingExpiredDateRules
                                    Exit For
                                End If

                                ' Marcamos para recalcular el dia que se ha generado la justificacion
                                If Not ExecuteSingleDay_MarkNextExpiredDate(IDEmployee, TaskDay.Add(1)) Then
                                    oState.Result = EngineResultEnum.ErrorDoingExpiredDateRules
                                    Exit Sub
                                End If
                            End If

                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoExpiredDateRules")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoExpiredDateRules")

            End Try
        End Sub
        Private Sub CreateConcept(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal pConcept As Integer, Optional ByVal BeginPeriod As Date = Nothing, Optional ByVal EndPeriod As Date = Nothing)
            '
            ' Crea Acumulado para compensar el tiempo que se ha movido a la justificación resultante de la regla de acumulado
            '
            Try
                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                Dim strPeriod As String = ""
                If BeginPeriod <> DateTime.MinValue AndAlso EndPeriod <> DateTime.MinValue Then
                    strPeriod = "@" & Any2Time(BeginPeriod).Value & "@" & Any2Time(EndPeriod).Value
                End If

                mCreateConceptsRules.Add(mCreateConceptsRules.Count + 1, IDEmployee & "@" & TaskDay.Value & "@" & pConcept & "@" & pValue.ToString.Replace(".", oInfo.NumberDecimalSeparator) & strPeriod)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateConcept")
            End Try
        End Sub

        Private Sub CreateCause(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal pCauses As Integer)
            '
            '   Crea Justificación resultante de la aplicación de la regla de acumulado
            '

            Dim DefaultCenter As Double

            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                strSQL = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND AccrualsRules=1 AND IDCause = " & pCauses.ToString & " AND Date=" & TaskDay.SQLSmallDateTime
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                Dim da As DbDataAdapter
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                Dim oRow As DataRow

                If zCauses.Rows.Count = 0 Then
                    oRow = zCauses.NewRow
                    oRow("IDEmployee") = IDEmployee
                    oRow("Date") = TaskDay.Value
                    oRow("IDCause") = pCauses
                    oRow("Value") = pValue
                    oRow("AccrualsRules") = 1

                    ' Si tiene centro de coste lo asignamods a su centro de coste por defecto
                    If mCostCenterInstallled Then
                        DefaultCenter = roBaseEngineManager.GetDefaultCenter(IDEmployee, TaskDay, oState)
                        oRow("IDCenter") = DefaultCenter
                        oRow("DefaultCenter") = 1
                        oRow("ManualCenter") = 0
                    End If
                    ' Guarda datos en la base de datos
                    zCauses.Rows.Add(oRow)
                Else
                    ' Ya existía una justificación de este tipo para este empleado y día, luego acumulo el valor al ya existente
                    oRow = zCauses.Rows(0)
                    oRow("Value") = oRow("Value") + pValue
                End If

                If da.Update(zCauses) = 0 Then
                    oState.Result = EngineResultEnum.ErrorCreatingOrUpdatingDailyCause
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateCause")
            End Try

        End Sub

        Private Function GenerateAccrualValue(ByVal AutomaticAccrualCriteria As roEngineAutomaticAccrualCriteria, ByVal zCauses As DataTable, ByVal IDEmployee As Long, ByVal TaskDate As Date, ByVal zShift As roShiftEngine, ByVal myConcept As roConceptEngine) As Double
            '
            ' Generamos el valor del devengo diario en funcion de la configuración del saldo
            '

            Dim dRet As Double = 0
            Dim dblFactorValue As Double
            Dim dblAccrualValue As Double
            Dim dblCauseValue As Double
            Dim ExpectedWorkingHours As Double
            Dim bolApplyAccrual As Boolean

            Try

                oState.Result = EngineResultEnum.NoError

                ExpectedWorkingHours = -1

                ' Coeficiente
                If AutomaticAccrualCriteria.FactorType = eFactorType.UserField Then
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, AutomaticAccrualCriteria.UserFieldName, TaskDate, New UserFields.roUserFieldState, False)
                    dblFactorValue = Any2Double(oUserField.FieldRawValue)
                Else
                    dblFactorValue = Any2Double(AutomaticAccrualCriteria.FactorValue)
                End If

                If myConcept.AutomaticAccrualType = eAutomaticAccrualType.DaysType Then
                    ' ** Devengo por dias(1)
                    If AutomaticAccrualCriteria.TypeAccrualDay = eAccrualDayType.AllDays Then
                        ' Todos los dias devengan (excepto si contienen justificaciones o horarios concretos)
                        bolApplyAccrual = True

                        ' Justificaciones que no devengan
                        For Each IDCause As Integer In AutomaticAccrualCriteria.Causes
                            For Each oCauseRow As DataRow In zCauses.Rows
                                If Any2Integer(oCauseRow("IDCause")) = IDCause Then
                                    bolApplyAccrual = False
                                    Exit For
                                End If
                            Next
                        Next

                        If bolApplyAccrual Then
                            ' Horarios que no devengan
                            For Each IDShift As Integer In AutomaticAccrualCriteria.Shifts
                                If Not zShift Is Nothing Then
                                    If Any2Double(zShift.ID) = IDShift Then
                                        bolApplyAccrual = False
                                        Exit For
                                    End If
                                End If
                            Next
                        End If

                        If bolApplyAccrual Then
                            dblAccrualValue = dblFactorValue
                        End If
                    Else
                        ' Solo devengan los dias que contengan (justificaciones o horarios concretos)
                        bolApplyAccrual = False

                        ' Justificaciones que devengan
                        For Each IDCause As Integer In AutomaticAccrualCriteria.Causes
                            For Each oCauseRow As DataRow In zCauses.Rows
                                If Any2Integer(oCauseRow("IDCause")) = IDCause Then
                                    bolApplyAccrual = True
                                    Exit For
                                End If
                            Next
                        Next

                        If Not bolApplyAccrual Then
                            ' Horarios que devengan
                            For Each IDShift As Integer In AutomaticAccrualCriteria.Shifts
                                If Not zShift Is Nothing Then
                                    If Any2Double(zShift.ID) = IDShift Then
                                        bolApplyAccrual = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If

                        If bolApplyAccrual Then
                            dblAccrualValue = dblFactorValue
                        End If

                    End If
                Else
                    ' ** Devengo por horas(2)

                    ' Obtenemos la lista de justificaciones que devengan
                    ' en el caso que se hayan generado las debemos utilizar
                    ' para calcular el coeficiente

                    For Each IDCause As Integer In AutomaticAccrualCriteria.Causes
                        If IDCause <> CONCEPT_SHIFTEXPECTEDHOURS Then
                            For Each oCauseRow As DataRow In zCauses.Rows
                                If Any2Integer(oCauseRow("IDCause")) = IDCause Then
                                    dblCauseValue = dblCauseValue + Any2Double(oCauseRow("Value"))
                                End If
                            Next
                        Else
                            ' Si es la justificacion de horas teoricas
                            If ExpectedWorkingHours = -1 Then
                                If Not bolProgrammedAbsenceOnHolidays Then
                                    ExpectedWorkingHours = Any2Double(
                                    ExecuteScalar("@SELECT# (CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 1 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  END) FROM DailySchedule with (nolock) , Shifts  with (nolock)  WHERE IDEmployee=" & IDEmployee & " AND DATE=" & Any2Time(TaskDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                                Else
                                    ExpectedWorkingHours = Any2Double(
                                    ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  FROM DailySchedule with (nolock) , Shifts with (nolock) WHERE IDEmployee=" & IDEmployee & " AND DATE=" & Any2Time(TaskDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase  "))
                                End If

                            End If
                            dblCauseValue = dblCauseValue + ExpectedWorkingHours
                        End If
                    Next

                    dblAccrualValue = dblCauseValue * dblFactorValue
                End If

                dRet = dblAccrualValue
            Catch ex As Data.Common.DbException
                dRet = 0
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GenerateAccrualValue")
            Catch ex As Exception
                dRet = 0
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GenerateAccrualValue")
            End Try

            Return dRet
        End Function

        Private Function GetDailyCauses(ByVal intIDEmployee As Long, ByVal xDate As Date) As DataTable
            '
            ' Obtiene las justificaciones diarias y su zona horaria correspondiente (no tenemos en cuenta las justificaciones generadas por reglas de convenio)
            '
            Dim myDS As New DataTable
            Dim strSQL As String

            Try

                strSQL = "@SELECT# convert(int,TimeZones.ID) as IDTimeZone, " &
                            "DailyCauses.Value, " &
                            "DailyCauses.IDRelatedIncidence, DailyCauses.IDCause " &
                         "FROM DailyIncidences INNER JOIN TimeZones ON DailyIncidences.IDZone = TimeZones.ID " &
                                "RIGHT OUTER JOIN DailyCauses ON DailyIncidences.IDEmployee = DailyCauses.IDEmployee AND " &
                                                                "DailyIncidences.Date = DailyCauses.Date AND " &
                                                                "DailyIncidences.ID = DailyCauses.IDRelatedIncidence " &
                         "WHERE DailyCauses.IDEmployee = " & intIDEmployee.ToString & " AND " &
                               "DailyCauses.Date = " & Any2Time(xDate).SQLSmallDateTime & " AND AccrualsRules=0  " &
                         "ORDER BY DailyIncidences.BeginTime"

                myDS = CreateDataTable(strSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetDailyCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetDailyCauses")
            End Try

            Return myDS

        End Function

        Private Sub ExecuteSingleDay_DoCauseLimits(ByVal IDEmployee As Long, ByVal TaskDate As Date)
            '
            ' Ejecuta el proceso de límites por justificacion
            '

            Dim oCurrentContract As Contract.roContract = Nothing
            Dim iExCauseLimit As Integer
            Dim currentCauseLimit As New roLabAgreeEngineCauseLimitValues

            Dim oLabAgree As roLabAgreeEngine

            Try

                oState.Result = EngineResultEnum.NoError

                ' Cargo los límites por justificación del empleado
                LoadEmployee_CauseLimits(Any2Time(TaskDate), oCurrentContract)
                If oState.Result <> EngineResultEnum.NoError Then
                    Exit Sub
                End If

                If oCurrentContract Is Nothing OrElse oCurrentContract.LabAgree Is Nothing OrElse oCurrentContract.LabAgree.ID = 0 Then
                    Exit Sub
                End If

                For iExCauseLimit = 1 To mEmployeeCauseLimits.Count
                    oLabAgree = roBaseEngineManager.GetLabAgreeeFromCache(oCurrentContract.LabAgree.ID, oState)
                    currentCauseLimit = oLabAgree.LabAgreeCauseLimitValues.Find(Function(x) x.IDCauseLimitValue = mEmployeeCauseLimits.Key(iExCauseLimit))

                    'Asignamos el periodo de validez limitado al min(periodo de contrato, periodo del limite asignado al convenio)
                    currentCauseLimit.BeginDate = Any2Time(String2Item(mEmployeeCauseLimits.Item(iExCauseLimit, roCollection.roSearchMode.roByIndex), 0, "@")).Value
                    currentCauseLimit.EndDate = Any2Time(String2Item(mEmployeeCauseLimits.Item(iExCauseLimit, roCollection.roSearchMode.roByIndex), 1, "@")).Value

                    Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(oState.IDPassport))
                    If oLabAgreeManager.CauseLimitExecuteToday(currentCauseLimit, TaskDate) Then
                        ExecuteSingleDay_DoCauseLimitSingleDayLive(IDEmployee, currentCauseLimit, TaskDate, oCurrentContract.BeginDate, oCurrentContract.EndDate)
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoCauseLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoCauseLimits")
            End Try
        End Sub

        Private Sub LoadEmployee_CauseLimits(ByVal TaskDate As roTime, ByRef oCurrentContract As Contract.roContract)
            '
            '  Carga lista de IDs de los límites por justificacion que se aplican para este empleado/Contrato/Convenio.
            '

            Dim strSQL As String

            Dim BeginDateRule As roTime
            Dim EndDateRule As roTime

            Try

                oState.Result = EngineResultEnum.NoError

                mEmployeeCauseLimits.Clear()

                'Obtenemos el convenio del empleado y las fechas de contrato
                oCurrentContract = New Contract.roContract
                oCurrentContract = Contract.roContract.GetContractInDateLite(mIDEmployee, TaskDate.Value, New Contract.roContractState(oState.IDPassport))
                If oCurrentContract Is Nothing OrElse oCurrentContract.LabAgree Is Nothing Then
                    Exit Sub
                End If

                'Obtenemos las límites por justificacion del convenio del empleado
                strSQL = "@SELECT# CauseLimitValues.IDCauseLimitValue "
                strSQL = strSQL & " , LabAgreeCauseLimitValues.BeginDate, LabAgreeCauseLimitValues.EndDate "
                strSQL = strSQL & " FROM CauseLimitValues,LabAgreeCauseLimitValues  "
                strSQL = strSQL & " WHERE LabAgreeCauseLimitValues.IDCauseLimitValue = CauseLimitValues.IDCauseLimitValue"
                strSQL = strSQL & " AND  LabAgreeCauseLimitValues.IDLabAgree = " & oCurrentContract.LabAgree.ID.ToString

                Dim tbCauseLimits As DataTable
                tbCauseLimits = CreateDataTable(strSQL)

                If tbCauseLimits IsNot Nothing AndAlso tbCauseLimits.Rows.Count > 0 Then

                    For Each oCauseLimitRow As DataRow In tbCauseLimits.Rows
                        ' Recortamos el periodo de validez al inicio o final del contrato en caso necesario
                        BeginDateRule = Any2Time(roConversions.Max(Any2Time(oCauseLimitRow("BeginDate")).Value, oCurrentContract.BeginDate))
                        EndDateRule = Any2Time(roConversions.Min(Any2Time(oCauseLimitRow("EndDate")).Value, oCurrentContract.EndDate))

                        ' Si el periodo es correcto
                        If BeginDateRule.VBNumericValue <= EndDateRule.VBNumericValue Then
                            mEmployeeCauseLimits.Add(Any2Integer(oCauseLimitRow("IDCauseLimitValue")), Any2String(BeginDateRule.Value) & "@" & Any2String(EndDateRule.Value))
                        End If
                    Next

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: LoadEmployee_CauseLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: LoadEmployee_CauseLimits")
            End Try
        End Sub

        Private Sub ExecuteSingleDay_DoCauseLimitSingleDayLive(ByVal IDEmployee As Long, ByRef CauseLimit As roLabAgreeEngineCauseLimitValues, ByVal TaskDay As Date, ByVal BeginContract As Date, ByVal EndContract As Date)
            '
            '   Ejecuta el calculo de los límites por justificación
            '
            Dim StartDateDayM As Date
            Dim StartDateDayY As Date

            Try

                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados mensuales
                StartDateDayM = GetStartDateForMonthAccruals(intYearIniMonth, intMonthIniDay, TaskDay)

                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados anuales
                StartDateDayY = GetStartDateForYearAccruals(intYearIniMonth, intMonthIniDay, TaskDay)

                ' Tiene Máximo menusal
                If CauseLimit.CauseLimitValue.MaximumMonthlyType <> LabAgreeValueType.None Then
                    ExecuteSingleDay_DoMonthlyCauseLimit(Any2Time(StartDateDayM), IDEmployee, CauseLimit, Any2Time(TaskDay), Any2Time(BeginContract), Any2Time(EndContract))
                End If

                ' Tiene Maximo anual
                If CauseLimit.CauseLimitValue.MaximumAnnualValueType <> LabAgreeValueType.None Then
                    ExecuteSingleDay_DoAnnualCauseLimit(Any2Time(StartDateDayY), IDEmployee, CauseLimit, Any2Time(TaskDay), Any2Time(BeginContract), Any2Time(EndContract))
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoCauseLimitSingleDayLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoCauseLimitSingleDayLive")
            End Try

        End Sub

        Private Sub ExecuteSingleDay_DoAnnualCauseLimit(ByVal StartDateDayY As roTime, ByVal IDEmployee As Long, ByRef CauseLimit As roLabAgreeEngineCauseLimitValues, ByVal TaskDay As roTime, ByVal BeginContract As roTime, ByVal EndContract As roTime)
            '
            ' Verificamos el limite mensual de la justificación
            ' y en caso de sobrepasarlo reemplazamos la justificación por la de exceso configurada
            Dim oValue As String
            Dim mValue As Double
            Dim sSQL As String
            Dim ActualValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si el dia de cálculo no contiene la justificacion del limite no hacemos nada
                sSQL = "@SELECT# COUNT(*) FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND IDCause =" & CauseLimit.CauseLimitValue.IDCause & " AND Date =" & TaskDay.SQLSmallDateTime
                If Any2Double(ExecuteScalar(sSQL)) = 0 Then Exit Sub

                mValue = -99999

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                Select Case CauseLimit.CauseLimitValue.MaximumAnnualValueType
                    Case LabAgreeValueType.DirectValue
                        ' Obtenemos el valor fijo del límite
                        mValue = CauseLimit.CauseLimitValue.MaximumAnnualValue
                    Case LabAgreeValueType.UserField
                        'Obtenemos el campo de la ficha del valor del limite al final del contrato
                        Dim oEmployeeUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, CauseLimit.CauseLimitValue.MaximumAnnualField.FieldName, TaskDay.Value, New UserFields.roUserFieldState, False)
                        oValue = Any2String(oEmployeeUserField.FieldRawValue)

                        If Len(oValue) > 0 AndAlso CauseLimit.CauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.UserField Then
                            Select Case CauseLimit.CauseLimitValue.MaximumAnnualField.FieldType
                                Case UserFieldsTypes.FieldTypes.tDecimal, UserFieldsTypes.FieldTypes.tNumeric, UserFieldsTypes.FieldTypes.tTime
                                    mValue = Any2Double(oValue.Replace(".", oInfo.NumberDecimalSeparator))
                            End Select
                        End If
                End Select

                If mValue = -99999 Then
                    Exit Sub
                End If

                ' Obtenemos el valor mensual de la justificación de los dias ya calculados y el del dia de cálculo
                sSQL = "@SELECT# SUM(isnull(value,0)) as total  FROM DailyCauses with (nolock), DailySchedule with (nolock) WHERE DailySchedule.IDEmployee = DailyCauses.IDEMployee AND DailySchedule.Date = DailyCauses.Date AND DailyCauses.IDEmployee=" & IDEmployee & " AND DailyCauses.IDCause =" & CauseLimit.CauseLimitValue.IDCause & " "
                sSQL = sSQL & " and ( (DailyCauses.date between " & StartDateDayY.SQLSmallDateTime & " and  " & StartDateDayY.Add(1, "yyyy").Add(-1, "d").SQLSmallDateTime & " And Status >= " & mPriority & ") or (DailyCauses.date = " & TaskDay.SQLSmallDateTime & "))"
                sSQL = sSQL & " and DailyCauses.date <= " & Any2Time(Now.Date).SQLSmallDateTime
                sSQL = sSQL & " and DailyCauses.date >= " & BeginContract.SQLSmallDateTime
                sSQL = sSQL & " and DailyCauses.date <= " & EndContract.SQLSmallDateTime

                ActualValue = Any2Double(ExecuteScalar(sSQL))

                ' Si no pasamos del máximo no hacemos nada
                If ActualValue <= mValue Then Exit Sub

                ' Obtenemos la diferencia
                mValue = ActualValue - mValue

                ' Reemplazamos el valor de la justificacion hasta la diferencia
                ExecuteSingleDay_DoReplaceCauseValue(IDEmployee, CauseLimit, TaskDay, mValue)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAnnualCauseLimit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoAnnualCauseLimit")
            End Try

        End Sub

        Private Sub ExecuteSingleDay_DoMonthlyCauseLimit(ByVal StartDateDayM As roTime, ByVal IDEmployee As Long, ByRef CauseLimit As roLabAgreeEngineCauseLimitValues, ByVal TaskDay As roTime, ByVal BeginContract As roTime, ByVal EndContract As roTime)
            '
            ' Verificamos el limite mensual de la justificación
            ' y en caso de sobrepasarlo reemplazamos la justificación por la de exceso configurada
            Dim oValue As String
            Dim mValue As Double
            Dim sSQL As String
            Dim ActualValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' Si el dia de cálculo no contiene la justificacion del limite no hacemos nada
                sSQL = "@SELECT# COUNT(*) FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND IDCause =" & CauseLimit.CauseLimitValue.IDCause & " AND Date =" & TaskDay.SQLSmallDateTime
                If Any2Double(ExecuteScalar(sSQL)) = 0 Then Exit Sub

                mValue = -99999

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                Select Case CauseLimit.CauseLimitValue.MaximumMonthlyType
                    Case LabAgreeValueType.DirectValue
                        ' Obtenemos el valor fijo del límite
                        mValue = CauseLimit.CauseLimitValue.MaximumMonthlyValue
                    Case LabAgreeValueType.UserField
                        'Obtenemos el campo de la ficha del valor del limite al final del contrato
                        Dim oEmployeeUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, CauseLimit.CauseLimitValue.MaximumMonthlyField.FieldName, TaskDay.Value, New UserFields.roUserFieldState, False)
                        oValue = Any2String(oEmployeeUserField.FieldRawValue)

                        If Len(oValue) > 0 AndAlso CauseLimit.CauseLimitValue.MaximumMonthlyType = LabAgreeValueType.UserField Then
                            Select Case CauseLimit.CauseLimitValue.MaximumMonthlyField.FieldType
                                Case UserFieldsTypes.FieldTypes.tDecimal, UserFieldsTypes.FieldTypes.tNumeric, UserFieldsTypes.FieldTypes.tTime
                                    mValue = Any2Double(oValue.Replace(".", oInfo.NumberDecimalSeparator))
                            End Select
                        End If
                End Select

                If mValue = -99999 Then
                    Exit Sub
                End If

                ' Obtenemos el valor mensual de la justificación de los dias ya calculados y el del dia de cálculo
                sSQL = "@SELECT# SUM(isnull(value,0)) as total  FROM DailyCauses with (nolock) , DailySchedule with (nolock)  WHERE DailySchedule.IDEmployee = DailyCauses.IDEMployee AND DailySchedule.Date = DailyCauses.Date AND DailyCauses.IDEmployee=" & IDEmployee & " AND DailyCauses.IDCause =" & CauseLimit.CauseLimitValue.IDCause.ToString & " "
                sSQL = sSQL & " and ( (DailyCauses.date between " & StartDateDayM.SQLSmallDateTime & " and  " & StartDateDayM.Add(1, "m").Add(-1, "d").SQLSmallDateTime & " And Status >= " & mPriority & ") or (DailyCauses.date = " & TaskDay.SQLSmallDateTime & "))"
                sSQL = sSQL & " and DailyCauses.date <= " & Any2Time(Now.Date).SQLSmallDateTime
                sSQL = sSQL & " and DailyCauses.date >= " & BeginContract.SQLSmallDateTime
                sSQL = sSQL & " and DailyCauses.date <= " & EndContract.SQLSmallDateTime

                ActualValue = Any2Double(ExecuteScalar(sSQL))

                ' Si no pasamos del máximo no hacemos nada
                If ActualValue <= mValue Then Exit Sub

                ' Obtenemos la diferencia
                mValue = ActualValue - mValue

                ' Reemplazamos el valor de la justificacion hasta la diferencia
                ExecuteSingleDay_DoReplaceCauseValue(IDEmployee, CauseLimit, TaskDay, mValue)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoMonthlyCauseLimit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoMonthlyCauseLimit")
            End Try

        End Sub

        Private Sub ExecuteSingleDay_DoReplaceCauseValue(ByVal IDEmployee As Long, ByRef CauseLimit As roLabAgreeEngineCauseLimitValues, ByVal TaskDay As roTime, ByVal mValue As Double)
            Dim sSQL As String
            Dim tbDailyCauses As DataTable
            Dim ValueToReplace As Double
            Dim strWhere As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Intentamos reemplazar con la justificación de reemplazo hasta la diferencia
                ' Obtenemos todas las justificaciones diarias para ese dia y empleado ordenadas descendentemente
                sSQL = "@SELECT# IDEmployee, Date, IDRelatedIncidence, IDCause, Value, Manual, CauseUser, CauseUserType, AccrualsRules, IsNotReliable, IDCenter, DefaultCenter, ManualCenter, DailyRule, AccruedRule  FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND Date=" & TaskDay.SQLSmallDateTime & " AND IDCause = " & CauseLimit.CauseLimitValue.IDCause & " Order by IDRelatedIncidence desc"
                tbDailyCauses = CreateDataTable(sSQL)

                If tbDailyCauses IsNot Nothing AndAlso tbDailyCauses.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbDailyCauses.Rows
                        If mValue >= Any2Double(oRow("Value")) Then
                            mValue = mValue - Any2Double(oRow("Value"))
                            ValueToReplace = Any2Double(oRow("Value"))
                        Else
                            ValueToReplace = mValue
                            mValue = 0
                        End If

                        If mValue > 0 Then
                            ' Reemplazamos todo el tiempo de la justificacion

                            ' Comprobamos si existe ya un registro con la justificacion de reemplazo
                            sSQL = "@SELECT# COUNT(*) FROM DailyCauses WHERE "
                            strWhere = " IDEmployee = " & IDEmployee.ToString
                            strWhere = strWhere & " AND Date = " & TaskDay.SQLSmallDateTime
                            strWhere = strWhere & " AND IDRelatedIncidence=" & Any2Double(oRow("IDRelatedIncidence"))
                            strWhere = strWhere & " AND IDCause = " & CauseLimit.CauseLimitValue.IDExcessCause
                            strWhere = strWhere & " AND AccrualsRules=" & Any2Double(oRow("AccrualsRules"))
                            strWhere = strWhere & " AND IDCenter=" & Any2Double(oRow("IDCenter"))
                            strWhere = strWhere & " AND DailyRule=" & Any2Double(oRow("DailyRule"))
                            strWhere = strWhere & " AND AccruedRule=" & Any2Double(oRow("AccruedRule"))

                            sSQL = sSQL & strWhere

                            If Any2Double(ExecuteScalar(sSQL)) = 0 Then
                                ' Si no existe actualizamos el registro actual con el ID de la justificacion de reemplazo
                                sSQL = "@UPDATE# DailyCauses Set IDCause = " & CauseLimit.CauseLimitValue.IDExcessCause & ", Manual=0 WHERE "
                                sSQL = sSQL & " IDEmployee = " & IDEmployee
                                sSQL = sSQL & " AND IDCause = " & CauseLimit.CauseLimitValue.IDCause
                                sSQL = sSQL & " AND Date = " & TaskDay.SQLSmallDateTime
                                sSQL = sSQL & " AND IDRelatedIncidence=" & Any2Double(oRow("IDRelatedIncidence"))
                                sSQL = sSQL & " AND AccrualsRules=" & Any2Double(oRow("AccrualsRules"))
                                sSQL = sSQL & " AND IDCenter=" & Any2Double(oRow("IDCenter"))
                                sSQL = sSQL & " AND DailyRule=" & Any2Double(oRow("DailyRule"))
                                sSQL = sSQL & " AND AccruedRule=" & Any2Double(oRow("AccruedRule"))

                                If Not ExecuteSql(sSQL) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: UPDATE 1 SQL Statement failed ")
                                    oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                    Exit Sub
                                End If
                            Else
                                ' Si ya existe una con la justificacion de reemplazo, añadimos el valor a la ya existente
                                sSQL = "@UPDATE# DailyCauses Set Value = Value + " & Double2Sql(ValueToReplace) & ", Manual=0 WHERE "
                                sSQL = sSQL & strWhere
                                If Not ExecuteSql(sSQL) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: UPDATE 2 SQL Statement failed")
                                    oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                    Exit Sub
                                End If

                                ' Eliminamos el registro actual de la justificacion original
                                sSQL = "@DELETE# FROM DailyCauses  WHERE "
                                sSQL = sSQL & " IDEmployee = " & IDEmployee
                                sSQL = sSQL & " AND IDCause = " & CauseLimit.CauseLimitValue.IDCause
                                sSQL = sSQL & " AND Date = " & TaskDay.SQLSmallDateTime
                                sSQL = sSQL & " AND IDRelatedIncidence=" & Any2Double(oRow("IDRelatedIncidence"))
                                sSQL = sSQL & " AND AccrualsRules=" & Any2Double(oRow("AccrualsRules"))
                                sSQL = sSQL & " AND IDCenter=" & Any2Double(oRow("IDCenter"))
                                sSQL = sSQL & " AND DailyRule=" & Any2Double(oRow("DailyRule"))
                                sSQL = sSQL & " AND AccruedRule=" & Any2Double(oRow("AccruedRule"))
                                If Not ExecuteSql(sSQL) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: DELETE 1 SQL Statement failed")
                                    oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                    Exit Sub
                                End If

                            End If
                        Else
                            ' Actualizamos el valor actual
                            sSQL = "@UPDATE# DailyCauses Set Value = Value - " & Double2Sql(ValueToReplace) & ", Manual=0 WHERE "
                            sSQL = sSQL & " IDEmployee = " & IDEmployee
                            sSQL = sSQL & " AND IDCause = " & CauseLimit.CauseLimitValue.IDCause
                            sSQL = sSQL & " AND Date = " & TaskDay.SQLSmallDateTime
                            sSQL = sSQL & " AND IDRelatedIncidence=" & Any2Double(oRow("IDRelatedIncidence"))
                            sSQL = sSQL & " AND AccrualsRules=" & Any2Double(oRow("AccrualsRules"))
                            sSQL = sSQL & " AND IDCenter=" & Any2Double(oRow("IDCenter"))
                            sSQL = sSQL & " AND DailyRule=" & Any2Double(oRow("DailyRule"))
                            sSQL = sSQL & " AND AccruedRule=" & Any2Double(oRow("AccruedRule"))

                            If Not ExecuteSql(sSQL) Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: UPDATE 3 SQL Statement failed")
                                oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                Exit Sub
                            End If

                            ' Creamos una nueva justificacion diaria con la justificacion de reemplazo y el valor a reemplazar
                            sSQL = "@SELECT# count(*) from DailyCauses where "
                            strWhere = " IDEmployee = " & IDEmployee
                            strWhere = strWhere & " AND Date = " & TaskDay.SQLSmallDateTime
                            strWhere = strWhere & " AND IDRelatedIncidence=" & Any2Double(oRow("IDRelatedIncidence"))
                            strWhere = strWhere & " AND IDCause = " & CauseLimit.CauseLimitValue.IDExcessCause
                            strWhere = strWhere & " AND AccrualsRules=" & Any2Double(oRow("AccrualsRules"))
                            strWhere = strWhere & " AND IDCenter=" & Any2Double(oRow("IDCenter"))
                            strWhere = strWhere & " AND DailyRule=" & Any2Double(oRow("DailyRule"))
                            strWhere = strWhere & " AND AccruedRule=" & Any2Double(oRow("AccruedRule"))

                            sSQL = sSQL & strWhere

                            If Any2Double(ExecuteScalar(sSQL)) = 0 Then
                                ' Si no existe creamos el registro
                                sSQL = "@INSERT# INTO  DailyCauses (IDEmployee, Date, IDRelatedIncidence, IDCause, Value, Manual, CauseUser, CauseUserType, AccrualsRules, IsNotReliable, IDCenter, DefaultCenter, ManualCenter,DailyRule,AccruedRule ) VALUES("
                                sSQL = sSQL & IDEmployee
                                sSQL = sSQL & " , " & TaskDay.SQLSmallDateTime
                                sSQL = sSQL & " , " & Any2Double(oRow("IDRelatedIncidence"))
                                sSQL = sSQL & ", " & CauseLimit.CauseLimitValue.IDExcessCause
                                sSQL = sSQL & ", " & Double2Sql(ValueToReplace)
                                sSQL = sSQL & " , 0"
                                sSQL = sSQL & " , " & Any2Double(oRow("CauseUser"))
                                sSQL = sSQL & " , " & Any2Double(oRow("CauseUserType"))
                                sSQL = sSQL & " , " & Any2Double(oRow("AccrualsRules"))
                                sSQL = sSQL & " , " & IIf(Any2Boolean(oRow("IsNotReliable")), 1, 0)
                                sSQL = sSQL & " , " & Any2Double(oRow("IDCenter"))
                                sSQL = sSQL & " , " & IIf(Any2Boolean(oRow("DefaultCenter")), 1, 0)
                                sSQL = sSQL & " , " & IIf(Any2Boolean(oRow("ManualCenter")), 1, 0)
                                sSQL = sSQL & " , " & IIf(Any2Boolean(oRow("DailyRule")), 1, 0)
                                sSQL = sSQL & " , " & IIf(Any2Boolean(oRow("AccruedRule")), 1, 0)
                                sSQL = sSQL & " ) "
                                If Not ExecuteSql(sSQL) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: INSERT SQL Statement failed")
                                    oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                    Exit Sub
                                End If
                            Else
                                ' Si ya existe, añadimos el valor
                                sSQL = "@UPDATE# DailyCauses Set Value = Value + " & Double2Sql(ValueToReplace) & ", Manual=0 WHERE "
                                sSQL = sSQL & strWhere
                                If Not ExecuteSql(sSQL) Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager::roAccrualsManager::ExecuteSingleDay_DoReplaceCauseValue: UPDATE 4 SQL Statement failed")
                                    oState.Result = EngineResultEnum.ErrorDoReplaceCauseValue
                                    Exit Sub
                                End If
                            End If

                        End If

                        If mValue <= 0 Then Exit For
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoReplaceCauseValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoReplaceCauseValue")

            End Try

        End Sub

        Private Function Execute_EvaluateShiftDailyRule(ByRef Rule As roShiftDailyRule, ByVal zCauses As DataTable, ByVal IDEmployee As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Devuelve True si la regla se cumple
            '
            Dim dblConditionValue As Double
            Dim dblCompareValue As Double
            Dim bCoincidence As Boolean
            Dim FromValue As Double
            Dim ToValue As Double
            Dim DayTask As Double
            Dim bolRet As Boolean = False

            Try

                If Rule Is Nothing OrElse Rule.Conditions.Count = 0 Then
                    Return bolRet
                End If

                ' Evaluamos si se tiene que validar para el dia de cálculo
                If Not Rule.DayValidationRule = Nothing Then
                    'Validamos si es Lunes, Martes.....Festivo
                    DayTask = Weekday(TaskDate, vbMonday)

                    If Rule.DayValidationRule <> DayValidationRule.Anyday_DayValidationRule AndAlso Rule.DayValidationRule <> DayValidationRule.Feast_DayValidationRule AndAlso Rule.DayValidationRule <> DayValidationRule.TelecommutingEfective_DayValidationRule AndAlso Rule.DayValidationRule <> DayValidationRule.TelecommutingPlanned_DayValidationRule Then
                        ' Validamos si el dia a procesar es el día de la semana configurado
                        If DayTask <> Rule.DayValidationRule Then
                            Return bolRet
                        End If
                    ElseIf Rule.DayValidationRule = DayValidationRule.Feast_DayValidationRule Then
                        ' Validamos si el dia a procesar es festivo

                        ' Comprobamos si es dia esta marcado como festivo
                        If Not Any2Boolean(ExecuteScalar("@SELECT# isnull(FeastDay,0)  FROM DailySchedule with (nolock)  WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & Any2Time(TaskDate).SQLSmallDateTime)) Then
                            Return bolRet
                        End If
                    ElseIf Rule.DayValidationRule = DayValidationRule.TelecommutingEfective_DayValidationRule Then
                        ' Validamos si el dia a procesar es de teletrabajo efectivo
                        If Not Any2Boolean(ExecuteScalar("@SELECT# InTelecommute FROM dbo.EmployeeZonesBetweenDates(" & Any2Time(TaskDate).SQLSmallDateTime & ", " & Any2Time(TaskDate).SQLSmallDateTime & ",'" & IDEmployee & "')")) Then
                            Return bolRet
                        End If

                    ElseIf Rule.DayValidationRule = DayValidationRule.TelecommutingPlanned_DayValidationRule Then
                        ' Validamos si el dia a procesar es de teletrabajo planificado
                        If Not Any2Boolean(ExecuteScalar("@SELECT# ISNULL(TelecommutePlanned,TelecommutingExpected) FROM dbo.EmployeeZonesBetweenDates(" & Any2Time(TaskDate).SQLSmallDateTime & "," & Any2Time(TaskDate).SQLSmallDateTime & ",'" & IDEmployee & "')")) Then
                            Return bolRet
                        End If
                    Else
                        ' Cualquier dia
                    End If
                End If

                ' Evaluamos si se tiene que validar en el caso que el anterior horario planificado sea uno 
                If Rule.PreviousShiftValidationRule IsNot Nothing AndAlso Rule.PreviousShiftValidationRule.Count > 0 Then
                    ' Si la lista contiene algún horario valido
                    If Not Rule.PreviousShiftValidationRule.Any(Function(x) x <= 0) Then
                        ' validamos si el anterior horario planificado existe en la lista
                        Dim IDPreviousShift As Integer = Any2Integer(ExecuteScalar("@SELECT# ISNULL(IDPreviousShift,0) FROM DailySchedule with (nolock)  WHERE IDEmployee=" & IDEmployee.ToString & " AND Date=" & Any2Time(TaskDate).SQLSmallDateTime))
                        If Not Rule.PreviousShiftValidationRule.Contains(IDPreviousShift) Then
                            Return bolRet
                        End If
                    Else
                        ' Cauqluier horario
                    End If
                End If

                ' Evaluamos si es preciso cumplimiento o no de regla de planificación de tipo descanso
                If Rule.ApplyScheduleValidationRule <> ApplyScheduleValidationRule.Disabled_ScheduleValidationRule AndAlso Rule.ScheduleRulesValidationRule.Any Then
                    ' Sólo verifico reglas de convenio, luego si el empleado no tiene convenio hoy, sigo
                    Dim oCurrentContract As New VTEmployees.Contract.roContract
                    oCurrentContract = Contract.roContract.GetContractInDateLite(IDEmployee, TaskDate, New Contract.roContractState(oState.IDPassport))
                    Dim bolContractHasLabAgree As Boolean = (oCurrentContract IsNot Nothing AndAlso oCurrentContract.LabAgree IsNot Nothing AndAlso oCurrentContract.LabAgree.ID > 0)
                    If bolContractHasLabAgree Then
                        Dim labAgreeId As Integer = oCurrentContract.LabAgree.ID
                        ' Miramos si el empleado tiene las reglas sobreescritas. Si es así, no le aplican las del convenio (las que no son de sistema)
                        Dim commandSQL As String = $"@SELECT# COUNT(*) FROM ScheduleRules WHERE IDContract = '{oCurrentContract.IDContract}' AND Definition NOT LIKE '%""RuleType"":0%'"
                        Dim scheduleRulesOverwroteOnContract As Boolean = (ExecuteScalar(commandSQL) > 0)
                        If scheduleRulesOverwroteOnContract Then
                            ' No le aplica las reglas de su convenio, por tanto no hace falta evaluar y nunca se aplicará la regla diaria
                            Return False
                        Else
                            ' Miramos si las reglas a evaluar son del convenio del empleado este día
                            Dim labAgreement As roLabAgreeEngine
                            labAgreement = roCacheManager.GetInstance().GetLabAgreeCache(RoAzureSupport.GetCompanyName, labAgreeId)
                            Dim ruleIds As List(Of Integer) = Rule.ScheduleRulesValidationRule
                            If labAgreement.LabAgreeScheduleRules IsNot Nothing AndAlso labAgreement.LabAgreeScheduleRules.Any AndAlso labAgreement.LabAgreeScheduleRules.Any(Function(x) ruleIds.Contains(x.Id)) Then
                                Dim calendarState As New roCalendarState(oState.IDPassport)
                                Dim calendarManager As New roCalendarManager(calendarState)
                                Dim calendar As roCalendar = calendarManager.Load(TaskDate, TaskDate, $"B{IDEmployee}", CalendarView.Planification, CalendarDetailLevel.Daily, True)
                                Dim calendarScheduleRulesManagerState As New roCalendarScheduleRulesState(oState.IDPassport)
                                Dim calendarScheduleRulesManager As New roCalendarScheduleRulesManager(calendarScheduleRulesManagerState)
                                Dim indictments As New List(Of roCalendarScheduleIndictment)
                                Dim labAgreeActualIdRulesToCheck As New Dictionary(Of Integer, List(Of Integer))

                                labAgreeActualIdRulesToCheck.Add(labAgreeId, ruleIds)
                                indictments = calendarScheduleRulesManager.CheckScheduleRules(calendar, dLabAgreeActualIdRulesToCheck:=labAgreeActualIdRulesToCheck)
                                Dim indictmentsExists As Boolean = indictments.FindAll(Function(x) x.DateBegin = TaskDate OrElse x.DateEnd = TaskDate).Any()
                                If (Rule.ApplyScheduleValidationRule = ApplyScheduleValidationRule.Cumple_ScheduleValidationRule AndAlso indictmentsExists) OrElse
                                    (Rule.ApplyScheduleValidationRule = ApplyScheduleValidationRule.NoCumple_ScheduleValidationRule AndAlso Not indictmentsExists) Then
                                    Return False
                                End If
                            End If
                        End If
                    End If
                End If

                For Each oCondition As roShiftDailyRuleCondition In Rule.Conditions
                    ' Para cada condicion, evaluamos si es correcta
                    bCoincidence = False
                    dblConditionValue = 0
                    dblCompareValue = 0

                    ' 01. Sumamos todas las justificaciones de la condicion (justificacion en una zona horaria)
                    dblConditionValue = GetConditionValue(oCondition.ConditionCauses, oCondition.ConditionTimeZones, zCauses, IDEmployee, TaskDate)

                    ' 02. Obtenemos el valor a comparar
                    Select Case oCondition.Type
                        Case DailyConditionValueType.DirectValue
                            ' Valor fijo
                            If Not oCondition.FromValue.Contains("-") Then
                                dblCompareValue = Any2Time(oCondition.FromValue, True).VBNumericValue
                            Else
                                dblCompareValue = Any2Time(Mid$(oCondition.FromValue, 2), True).VBNumericValue * -1
                            End If

                        Case DailyConditionValueType.UserField
                            ' Campo de la ficha
                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oCondition.UserField, TaskDate, New UserFields.roUserFieldState, False)
                            Dim strCompareValue As String = Any2String(oUserField.FieldRawValue)
                            If strCompareValue.StartsWith("-") Then
                                dblCompareValue = Any2Time(strCompareValue.Replace("-", "+"), True).VBNumericValue * -1
                            Else
                                dblCompareValue = Any2Time(strCompareValue, True).VBNumericValue
                            End If
                        Case DailyConditionValueType.ID
                            ' Justificaciones
                            dblCompareValue = GetConditionValue(oCondition.CompareCauses, oCondition.CompareTimeZones, zCauses, IDEmployee, TaskDate)
                    End Select

                    ' 03. Validamos la comparacion
                    Select Case oCondition.Compare
                        Case DailyConditionCompareType.Equal ' Igual
                            If dblConditionValue = dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.Minor ' Menor
                            If dblConditionValue < dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.MinorEqual ' Menor o igual
                            If dblConditionValue <= dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.Major ' Mayor
                            If dblConditionValue > dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.MajorEqual ' Mayor o igual
                            If dblConditionValue >= dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.Distinct ' Diferente
                            If dblConditionValue <> dblCompareValue Then bCoincidence = True
                        Case DailyConditionCompareType.Between ' Entre
                            If Not oCondition.FromValue.Contains("-") Then
                                FromValue = Any2Time(oCondition.FromValue, True).VBNumericValue
                            Else
                                FromValue = Any2Time(Mid$(oCondition.FromValue, 2), True).VBNumericValue * -1
                            End If
                            If Not oCondition.ToValue.Contains("-") Then
                                ToValue = Any2Time(oCondition.ToValue, True).VBNumericValue
                            Else
                                ToValue = Any2Time(Mid$(oCondition.ToValue, 2), True).VBNumericValue * -1
                            End If

                            If dblConditionValue >= FromValue And dblConditionValue <= ToValue Then bCoincidence = True
                    End Select
                    If Not bCoincidence Then
                        Return bolRet
                    End If
                Next
                If bCoincidence Then bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateShiftDailyRule")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateShiftDailyRule")
            End Try

            Return bolRet

        End Function

        Private Function Execute_EvaluateConceptCauseRuleCondition(ByRef rule As List(Of roEngineConceptCondition), ByRef CausesSum As roCollection, ByVal EmployeeID As Long, ByVal zDate As Date) As Boolean
            '
            ' Devuelve True si la regla de composicion del acumulado se cumple
            '
            Dim CauseID As Long
            Dim TotalCondition As Double
            Dim iOperator As Integer
            Dim ValueCompare As Double
            Dim ExpectedWorkingHours As Double
            Dim bolRet As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ExpectedWorkingHours = -1

                ' Al no haber ninguna condicion aplicamos el valor de la justificacion como toque
                If rule.Count = 0 Then Return True

                For Each oConcepRuleContition As roEngineConceptCondition In rule
                    bolRet = False

                    TotalCondition = 0
                    ' Obtenemos la suma de los valores de las justificaciones
                    For Each oConceptConditionCause As roEngineConceptConditionCause In oConcepRuleContition.IDCauses
                        ' Para cada justificacion de la condicion
                        If oConceptConditionCause.Operation = "+" Then
                            CauseID = oConceptConditionCause.IDCause
                            iOperator = 1
                        ElseIf oConceptConditionCause.Operation = "-" Then
                            CauseID = oConceptConditionCause.IDCause
                            iOperator = -1
                        Else
                            Return False
                        End If

                        If CauseID <> CONCEPT_SHIFTEXPECTEDHOURS Then
                            If CausesSum.Exists(CauseID) Then TotalCondition = TotalCondition + (Any2Double(CausesSum(CauseID)) * iOperator)
                        Else
                            If ExpectedWorkingHours = -1 Then
                                If Not bolProgrammedAbsenceOnHolidays Then
                                    ExpectedWorkingHours = Any2Double(
                                    ExecuteScalar("@SELECT# (CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 1 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  END) FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                                Else
                                    ExpectedWorkingHours = Any2Double(
                                    ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase  "))
                                End If
                            End If
                            TotalCondition = TotalCondition + (ExpectedWorkingHours * iOperator)
                        End If
                    Next

                    ' Comparamos el total con un valor
                    ValueCompare = 0
                    Select Case oConcepRuleContition.Value_Type
                        Case DTOs.ValueType.DirectValue
                            ' Valor directo
                            If IsDate(Any2String(oConcepRuleContition.Value_Direct)) Then
                                ValueCompare = Any2Time(oConcepRuleContition.Value_Direct).NumericValue
                            Else
                                ValueCompare = Any2Double(oConcepRuleContition.Value_Direct)
                            End If
                        Case DTOs.ValueType.IDCause
                            ' El valor de una justificacion
                            CauseID = Any2Double(oConcepRuleContition.Value_IDCause)

                            If CauseID <> CONCEPT_SHIFTEXPECTEDHOURS Then
                                If CausesSum.Exists(CauseID) Then ValueCompare = Any2Double(CausesSum(CauseID))
                            Else
                                If ExpectedWorkingHours = -1 Then
                                    If Not bolProgrammedAbsenceOnHolidays Then
                                        ExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# (CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 1 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  END)  FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & EmployeeID & " AND DATE=" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))
                                    Else
                                        ExpectedWorkingHours = Any2Double(ExecuteScalar("@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee =" & EmployeeID & " AND DATE =" & Any2Time(zDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase  "))
                                    End If
                                End If
                                ValueCompare = ExpectedWorkingHours
                            End If
                        Case DTOs.ValueType.UserField
                            ' El valor de un campo de la ficha
                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, oConcepRuleContition.Value_UserField, zDate, New UserFields.roUserFieldState(-1), False)
                            ValueCompare = Any2Double(oUserField.FieldRawValue)
                        Case Else
                            Return False
                    End Select

                    Select Case oConcepRuleContition.Compare
                        Case ConditionCompareType.Equal
                            If TotalCondition = ValueCompare Then bolRet = True
                        Case ConditionCompareType.Minor
                            If TotalCondition < ValueCompare Then bolRet = True
                        Case ConditionCompareType.MinorEqual
                            If TotalCondition <= ValueCompare Then bolRet = True
                        Case ConditionCompareType.Major
                            If TotalCondition > ValueCompare Then bolRet = True
                        Case ConditionCompareType.MajorEqual
                            If TotalCondition >= ValueCompare Then bolRet = True
                        Case ConditionCompareType.Distinct
                            If TotalCondition <> ValueCompare Then bolRet = True
                        Case Else
                            bolRet = False
                    End Select

                    If Not bolRet Then
                        Return False
                    End If
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateConceptCauseRuleCondition")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateConceptCauseRuleCondition")
            End Try

            Return bolRet

        End Function

        Private Sub ExecuteSingleDay_DoStartupValuesLive(ByVal IDEmployee As Long, ByVal TaskDay As roTime)
            '
            ' Tratamiento de Valores Inciales version Live
            '
            Dim strSQL As String
            Dim myConceptsStartupValue As New roCollection
            Dim IDLabAgree As Double
            Dim Index As Integer
            Dim bolIsStartupYear As Boolean
            Dim bolIsStartupMonth As Boolean
            Dim bolIsStartupWeek As Boolean
            Dim bolIsStartupContract As Boolean
            Dim bolIsContractAfterFirst As Boolean
            Dim bolContractHasLabAgree As Boolean
            Dim EndContractBefore As String = String.Empty
            Dim bolIsStartupAnnualWork As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' Comprobamos si hoy hay que aplicar el valor inicial
                ' Los valores iniciales se aplican el primer dia del año
                ' o el primer dia del contrato del empleado
                ' o el primer dia del mes

                ' 0. Eliminamos todos los saldos iniciales de ese día
                strSQL = "@DELETE# FROM DailyAccruals WHERE IDEmployee = " & IDEmployee & " AND Date = " & TaskDay.SQLSmallDateTime & " AND StartupValue = 1 and CarryOver = 1"
                If Not ExecuteSql(strSQL) Then
                    oState.Result = EngineResultEnum.ErrorDeletingDailyAccruals
                    Exit Sub
                End If

                If IsStartupDay(IDEmployee, TaskDay, bolIsStartupYear, bolIsStartupMonth, bolIsStartupWeek, bolIsStartupContract, bolIsContractAfterFirst, bolContractHasLabAgree, EndContractBefore, IDLabAgree, bolIsStartupAnnualWork) Then

                    ' 1. Comprobamos si tiene asignado un convenio  para el dia a procesar
                    If bolContractHasLabAgree Then
                        ' 2. Obtenemos los acumulados con saldos iniciales con su correspondiente valor
                        GetStartupValuesOnDate(IDEmployee, IDLabAgree, myConceptsStartupValue, TaskDay, bolIsStartupYear, bolIsStartupMonth, bolIsStartupWeek, bolIsStartupContract, bolIsContractAfterFirst, EndContractBefore, bolIsStartupAnnualWork)

                        If DailyScheduleGUIDChanged() Then
                            Exit Sub
                        End If

                        ' 3. Generamos los saldos para el dia a procesar
                        Dim lstDates As Generic.List(Of DateTime) = Nothing
                        Dim oCurrentContract As VTEmployees.Contract.roContract = Nothing
                        Dim oContractState = New Contract.roContractState(oState.IDPassport)
                        Dim oLabAgreeCache As roLabAgreeEngine = Nothing

                        For Index = 1 To myConceptsStartupValue.Count
                            strSQL = "@INSERT# INTO DailyAccruals(IDEmployee,IDConcept,Date,Value,CarryOver,StartupValue) VALUES (" & Any2String(IDEmployee) & "," &
                                    Any2String(myConceptsStartupValue.Key(Index)) & "," & Any2Time(TaskDay.DateOnly).SQLSmallDateTime & "," &
                                    Any2String(myConceptsStartupValue(Index, roCollection.roSearchMode.roByIndex)).Replace(",", ".") & ",1,1)"
                            If Not ExecuteSql(strSQL) Then
                                oState.Result = EngineResultEnum.ErrorInsertingStartupValue
                                Exit Sub
                            End If

                            Dim oConcept As roConceptEngine = oConceptsCache.Item(Any2Integer(myConceptsStartupValue.Key(Index)))
                            If Not oConcept Is Nothing Then
                                If oConcept.DefaultQuery = "L" Then
                                    ' Asignamos el valor al tramo que corresponde 
                                    Dim strPeriod As String = ""
                                    If lstDates Is Nothing Then
                                        lstDates = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, TaskDay.Value, New roContractState(-1))
                                    End If
                                    If lstDates IsNot Nothing AndAlso lstDates.Count > 0 Then
                                        strPeriod = " Set BeginPeriod= " & Any2Time(lstDates(0)).SQLSmallDateTime & ", EndPeriod=" & Any2Time(lstDates(1)).SQLSmallDateTime
                                    Else
                                        roLog.GetInstance().logMessage(roLog.EventType.roWarning, "roAccrualsManager:: ExecuteSingleDay_DoStartupValuesLive: The corresponding period for the date has not been found. Employee:" & IDEmployee.ToString & " Date=" & TaskDay.Value)
                                        oState.Result = EngineResultEnum.ErrorInsertingStartupValue
                                        Exit Sub
                                    End If

                                    ' Asignamos el valor inicial al campo positivo o negativo 
                                    Dim strValue As String = ""
                                    If Any2Double(myConceptsStartupValue(Index, roCollection.roSearchMode.roByIndex)) > 0 Then
                                        strValue = " , PositiveValue= " & myConceptsStartupValue(Index, roCollection.roSearchMode.roByIndex).ToString.Replace(",", ".") & ", NegativeValue=0"
                                    Else
                                        strValue = " , PositiveValue= 0, NegativeValue=" & myConceptsStartupValue(Index, roCollection.roSearchMode.roByIndex).ToString.Replace(",", ".")
                                    End If

                                    ' Indicamos la caducidad y el inicio de validez del valor en caso necesario
                                    Dim ExpiredDate = New roTime
                                    ExpiredDate = Any2Time(roNullDate)
                                    Dim StartEnjoymentDate = New roTime
                                    StartEnjoymentDate = Any2Time(roNullDate)

                                    If Any2Double(myConceptsStartupValue(Index, roCollection.roSearchMode.roByIndex)) > 0 Then
                                        If oCurrentContract Is Nothing Then
                                            'Obenemos el convenio del usuario para ver si hay que aplicar alguna caducidad
                                            oCurrentContract = Contract.roContract.GetContractInDateLite(IDEmployee, TaskDay.Value, oContractState)
                                            If oCurrentContract IsNot Nothing Then
                                                ' Obtenemos el convenio del contrato
                                                oLabAgreeCache = roBaseEngineManager.GetLabAgreeeFromCache(IDLabAgree, oState)
                                            End If
                                        End If
                                        If oLabAgreeCache IsNot Nothing AndAlso oLabAgreeCache.StartupValues IsNot Nothing AndAlso oLabAgreeCache.StartupValues.Count > 0 Then
                                            For Each oStartupValue As roEngineStartupValue In oLabAgreeCache.StartupValues
                                                If oStartupValue.IDConcept = oConcept.ID Then
                                                    If oStartupValue.Expiration IsNot Nothing Then
                                                        ' Calculamos la fecha de caducidad del valor inicial 
                                                        Select Case oStartupValue.Expiration.Unit
                                                            Case LabAgreeStartupValueExpirationUnit.Day
                                                                ExpiredDate = TaskDay.Add(Any2Double(oStartupValue.Expiration.ExpireAfter), "d")
                                                            Case LabAgreeStartupValueExpirationUnit.Month
                                                                ExpiredDate = TaskDay.Add(Any2Double(oStartupValue.Expiration.ExpireAfter), "m")
                                                        End Select
                                                    End If
                                                    If oStartupValue.Enjoyment IsNot Nothing Then
                                                        ' Calculamos la fecha de inicio de disfrute del valor inicial 
                                                        Select Case oStartupValue.Enjoyment.Unit
                                                            Case LabAgreeStartupValueEnjoymentUnit.Day
                                                                StartEnjoymentDate = TaskDay.Add(Any2Double(oStartupValue.Enjoyment.StartAfter), "d")
                                                            Case LabAgreeStartupValueEnjoymentUnit.Month
                                                                StartEnjoymentDate = TaskDay.Add(Any2Double(oStartupValue.Enjoyment.StartAfter), "m")
                                                        End Select
                                                    End If
                                                    Exit For
                                                End If
                                            Next
                                        End If
                                    End If

                                    ' Guardamos los valores en el registro 
                                    If Not ExecuteSql("@UPDATE# DailyAccruals WITH (ROWLOCK) " & strPeriod & strValue & ",ExpiredDate=" & IIf(ExpiredDate.Value = Any2Time(roNullDate).Value, "NULL", ExpiredDate.SQLSmallDateTime) & ",StartEnjoymentDate=" & IIf(StartEnjoymentDate.Value = Any2Time(roNullDate).Value, "NULL", StartEnjoymentDate.SQLSmallDateTime) &
                                                " WHERE IDEmployee= " & IDEmployee.ToString & " AND Date=" & TaskDay.SQLSmallDateTime & " AND IDConcept=" & oConcept.ID.ToString & " AND CarryOver=1 AND StartupValue=1 ") Then
                                        oState.Result = EngineResultEnum.ErrorInsertingStartupValue
                                        Exit Sub
                                    End If

                                    ' Marcamos para recalcular la fecha de caducidad para su revisión en caso necesario
                                    If Not ExecuteSingleDay_MarkNextExpiredDate(IDEmployee, ExpiredDate) Then
                                        oState.Result = EngineResultEnum.ErrorInsertingStartupValue
                                        Exit Sub
                                    End If

                                    ' Marcamos para recalcular los dias posteriores que hayan generado valores de vacaciones para su revisión en caso necesario
                                    If Not ExecuteSingleDay_MarkNextAnnualWorkDates(IDEmployee, TaskDay, oConcept.ID) Then
                                        oState.Result = EngineResultEnum.ErrorInsertingStartupValue
                                        Exit Sub
                                    End If
                                End If
                            End If

                        Next Index
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoStartupValuesLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoStartupValuesLive")

            End Try
        End Sub

        Private Sub ExecuteSingleDay_DoSetPeriodAnnualWork(ByVal IDEmployee As Long, ByVal TaskDay As roTime)
            '
            ' Tratamiento de Valores negativos de los saldos de tipo anual laboral 
            ' (es necesario hacerlo despues de generar los valores iniciales, ya que el mismo dia que generas el valor es posible que tambien lo quieras utilizar)
            Dim strSQL As String = ""
            Dim DBAnnualWorkPeriod As DataTable = Nothing
            Try

                oState.Result = EngineResultEnum.NoError
                Dim oCurrentContract As VTEmployees.Contract.roContract = Nothing

                For Each myConcept As roConceptEngine In oConceptsCache.Values
                    If dConceptsValues(myConcept.ID).Value < 0 AndAlso myConcept.DefaultQuery = "L" Then
                        Dim strPeriod As String = ""
                        Dim strWherePeriod As String = ""
                        Dim dblTotalPeriodValue As Double = 0
                        ' En el caso que el valor sea negativo, debemos asignarlo al tramo más antiguo que tenga saldo positivo
                        ' y como mucho hasta el tramo de la fecha de cálculo
                        ' Obtenemos los tramos del usuario ordenados de forma ascendente
                        If DBAnnualWorkPeriod Is Nothing Then
                            If oCurrentContract Is Nothing Then
                                oCurrentContract = Contract.roContract.GetContractInDateLite(IDEmployee, TaskDay.Value, New Contract.roContractState(oState.IDPassport))
                            End If
                            If oCurrentContract IsNot Nothing Then
                                strSQL = "@SELECT# BeginPeriod , EndPeriod From dbo.sysfnEmployeesAnnualWorkPeriods(" & IDEmployee.ToString & ") WHERE Beginperiod >=" & Any2Time(oCurrentContract.BeginDate).SQLSmallDateTime & "   order by BeginPeriod asc"
                                DBAnnualWorkPeriod = CreateDataTable(strSQL)
                            End If
                        End If
                        ' para cada tramo,  miramos si hay saldo positivo hasta el dia de calculo
                        If DBAnnualWorkPeriod IsNot Nothing AndAlso DBAnnualWorkPeriod.Rows.Count > 0 Then
                            For Each orow As DataRow In DBAnnualWorkPeriod.Rows
                                strPeriod = " Set BeginPeriod= " & Any2Time(orow("BeginPeriod")).SQLSmallDateTime & ", EndPeriod=" & Any2Time(orow("EndPeriod")).SQLSmallDateTime
                                strWherePeriod = " AND BeginPeriod= " & Any2Time(orow("BeginPeriod")).SQLSmallDateTime & " AND EndPeriod=" & Any2Time(orow("EndPeriod")).SQLSmallDateTime
                                ' Si es el tramo actual a fecha de cálculo , lo asignamos directamente y no seguimos revisando mas
                                If TaskDay.Value >= Any2Time(orow("BeginPeriod")).Value AndAlso TaskDay.Value <= Any2Time(orow("EndPeriod")).Value Then
                                    Exit For
                                Else
                                    ' Obtenemos el valor del saldo en el tramo correspondiente hasta taskday, si el valor es positivo lo asignamos a dicho tramo
                                    ' se tiene el cuenta hasta la fecha de calculo porque el mismo dia puede haberse generado un valor inicial
                                    dblTotalPeriodValue = Any2Double(ExecuteScalar("@SELECT# SUM(isnull(Value,0)) AS total FROM DailyAccruals with (nolock) WHERE IDEmployee=" & IDEmployee.ToString & " AND IDConcept = " & myConcept.ID.ToString & " AND Date <= " & TaskDay.SQLSmallDateTime & strWherePeriod))
                                    If dblTotalPeriodValue > 0 Then Exit For
                                End If
                            Next
                        End If
                        If strPeriod.Length = 0 Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roAccrualsManager:: Execute_SaveData: The corresponding period for the date has not been found. Employee:" & IDEmployee.ToString & " Date=" & TaskDay.Value)
                            oState.Result = EngineResultEnum.ErrorSetPeriodAnnualWork
                            Exit Sub
                        End If

                        ' Asignamos el tramo correspondiente al valor negativo generado
                        If Not ExecuteSql("@UPDATE# DailyAccruals WITH (ROWLOCK) " & strPeriod &
                                                " WHERE IDEmployee= " & IDEmployee.ToString & " AND Date=" & TaskDay.SQLSmallDateTime & " AND IDConcept=" & myConcept.ID.ToString & " AND isnull(CarryOver,0) =0 AND isnull(StartupValue,0)=0 ") Then
                            oState.Result = EngineResultEnum.ErrorSetPeriodAnnualWork
                            Exit Sub
                        End If

                        ' marcamos para recalcular todos los dias posteriores al dia de cálculo que hayan generado algun valor en el mismo saldo hasta 
                        ' la fecha actual
                        If Not ExecuteSingleDay_MarkNextAnnualWorkDates(IDEmployee, TaskDay, myConcept.ID) Then
                            oState.Result = EngineResultEnum.ErrorSetPeriodAnnualWork
                            Exit Sub
                        End If
                    End If
                Next

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoSetPeriodAnnualWork")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoSetPeriodAnnualWork")
            End Try
        End Sub


        Private Function IsStartupDay(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByRef bolIsStartupYear As Boolean, ByRef bolIsStartupMonth As Boolean, ByRef bolIsStartupWeek As Boolean, ByRef bolIsStartupContract As Boolean, ByRef bolIsContractAfterFirst As Boolean, ByRef bolContractHasLabAgree As Boolean, ByRef EndContractBefore As String, ByRef IDLabAgree As Integer, ByRef bolIsStartupAnnualWork As Boolean)
            '
            ' Comprobamos si el dia a procesar coincide con el inicio de año o con algun inicio de contrato del empleado
            '
            Dim bRet As Boolean = False
            Dim StartDateDayM As Date
            Dim StartDateDayY As Date
            Dim StartDateDayW As Date

            Try

                bolIsStartupYear = False
                bolIsStartupMonth = False
                bolIsStartupWeek = False
                bolIsStartupContract = False
                bolIsContractAfterFirst = False
                bolContractHasLabAgree = False
                EndContractBefore = ""

                ' Calculamos el día de inicio de mes
                StartDateDayM = GetStartDateForMonthAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día de inicio de año
                StartDateDayY = GetStartDateForYearAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día de inicio de semana
                StartDateDayW = GetStartDateForWeekAccruals(intWeekIniday, TaskDay.Value)

                If Any2Time(StartDateDayY).NumericValue = TaskDay.NumericValue Then
                    bRet = True
                    bolIsStartupYear = True
                End If

                If Any2Time(StartDateDayM).NumericValue = TaskDay.NumericValue Then
                    bRet = True
                    bolIsStartupMonth = True
                End If

                If Any2Time(StartDateDayW).NumericValue = TaskDay.NumericValue Then
                    bRet = True
                    bolIsStartupWeek = True
                End If

                ' Comprobamos si corresponde con alguno de los inicios de contrato del empleado
                Dim oCurrentContract As New VTEmployees.Contract.roContract
                oCurrentContract = Contract.roContract.GetContractInDateLite(IDEmployee, TaskDay.Value, New Contract.roContractState(oState.IDPassport))
                bolContractHasLabAgree = (oCurrentContract IsNot Nothing AndAlso oCurrentContract.LabAgree IsNot Nothing AndAlso oCurrentContract.LabAgree.ID > 0)
                If bolContractHasLabAgree Then IDLabAgree = oCurrentContract.LabAgree.ID

                If oCurrentContract IsNot Nothing AndAlso oCurrentContract.BeginDate = TaskDay.Value Then
                    ' Es inicio de contrato. Miro que no sea el primero

                    EndContractBefore = Any2String(ExecuteScalar("@SELECT# TOP 1 EndDate FROM EmployeeContracts WHERE IDEmployee = " & IDEmployee & " AND BeginDate < " & TaskDay.SQLSmallDateTime & " Order by BeginDate desc"))

                    If IsDate(EndContractBefore) Then
                        bolIsContractAfterFirst = True
                    End If

                    bolIsStartupYear = True
                    bolIsStartupMonth = True
                    bolIsStartupWeek = True
                    bolIsStartupContract = True
                    bolIsStartupAnnualWork = True
                    bRet = True
                End If

                ' Si se usan saldos de tipo anual laboral, comprobamos si corresponde con alguno de los inicios de tramo de los periodos anuales laborales
                If bolAnnualWorkConceptsDefined Then
                    Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, TaskDay.Value, New roContractState(-1))
                    If lstDates.Count > 0 Then
                        If TaskDay.Value = Any2Time(lstDates(0)).Value Then
                            bolIsStartupAnnualWork = True
                            bRet = True
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: IsStartupDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: IsStartupDay")

            End Try

            Return bRet

        End Function

        Private Function GetStartupValuesOnDate(ByVal IDEmployee As Long, ByVal IDLabAgree As Double, ByRef myConceptsStartupValues As roCollection, ByVal TaskDay As roTime, ByVal bolIsStartupYear As Boolean, ByVal bolIsStartupMonth As Boolean, ByVal bolIsStartupWeek As Boolean, ByVal bolIsStartupContract As Boolean, ByVal bolIsContractAfterFirst As Boolean, ByVal EndContractBefore As String, ByVal bolIsStartupAnnualWork As Boolean) As Boolean
            '
            ' Obtenemos los valores iniciales de los saldos
            '
            Dim bRet As Boolean = False
            Dim strSQL As String
            Dim StartField As String
            Dim StartValue As String
            Dim dblScalingStartValue As Double
            Dim ScalingUserField As String
            Dim ScalingCoefficientUserField As String
            Dim ScalingDefinition As String
            Dim StartFieldType As Double
            Dim strWhere As String = String.Empty
            Dim dblStartValue As Double
            Dim bolApplyException As Boolean
            Dim StartAccrualDate As New roTime
            Dim StartContractDate As String = String.Empty
            Dim mApplyStartUpValueAndCarryOver As String
            Dim StartValue_UPF_AP As Double = 0
            Dim StartValue_UPF_VAC As Double = 0

            Try

                oState.Result = EngineResultEnum.NoError

                strSQL = "@SELECT# * FROM StartupValues WITH(NOLOCK) , LabAgreeStartupValues WITH(NOLOCK) WHERE "
                strSQL = strSQL & " StartupValues.IDStartupValue = LabAgreeStartupValues.IDStartupValue "
                strSQL = strSQL & " AND LabAgreeStartupValues.IDLabAgree = " & IDLabAgree
                strSQL = strSQL & " AND LabAgreeStartupValues.IDConcept in(@SELECT# Id FROM Concepts WITH(NOLOCK) WHERE BeginDate <= " & TaskDay.SQLSmallDateTime & " AND FinishDate >= " & TaskDay.SQLSmallDateTime & ")"

                ' Filtramos por el tipo de saldo que se debe aplicar ese día
                ' Anual y/o Mensual
                If bolIsStartupYear Then
                    strWhere = "StartupValues.IDConcept IN(@SELECT# id from concepts where DefaultQuery like 'Y') "
                End If

                If bolIsStartupMonth Then
                    If Len(strWhere) > 0 Then
                        strWhere = strWhere & " OR "
                    End If
                    strWhere = strWhere & " StartupValues.IDConcept IN(@SELECT# id from concepts where DefaultQuery like 'M')"
                End If

                If bolIsStartupWeek Then
                    If Len(strWhere) > 0 Then
                        strWhere = strWhere & " OR "
                    End If
                    strWhere = strWhere & " StartupValues.IDConcept IN(@SELECT# id from concepts where DefaultQuery like 'W')"
                End If

                If bolIsStartupContract Then
                    If Len(strWhere) > 0 Then
                        strWhere = strWhere & " OR "
                    End If
                    strWhere = strWhere & " StartupValues.IDConcept IN(@SELECT# id from concepts where DefaultQuery like 'C')"
                End If

                If bolIsStartupAnnualWork Then
                    If Len(strWhere) > 0 Then
                        strWhere = strWhere & " OR "
                    End If
                    strWhere = strWhere & " StartupValues.IDConcept IN(@SELECT# id from concepts where DefaultQuery like 'L')"
                End If

                If Len(strWhere) > 0 Then
                    strSQL = strSQL & " AND (" & strWhere & ")"
                End If

                Dim tbStartup As DataTable
                tbStartup = CreateDataTable(strSQL)

                If tbStartup IsNot Nothing AndAlso tbStartup.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbStartup.Rows
                        bolApplyException = False
                        ' En el caso de que la fecha sea un inicio de contrato posterior al primero
                        ' y existe definida una excepción
                        If bolIsContractAfterFirst AndAlso Any2Boolean(oRow("NewContractException")) Then
                            ' Solo aplicamos la excepcion si la fecha de inicio de contrato actual
                            ' es superior o igual a la fecha de inicio del periodo del saldo
                            StartAccrualDate = GetStartAccrualDate(IDEmployee, oRow("IDConcept"), TaskDay.Value, StartContractDate)
                            If StartAccrualDate IsNot Nothing AndAlso StartAccrualDate.Value <= TaskDay.Value Then
                                ' Si se cumple la condicion
                                If Execute_EvaluateStartUpCondition(Any2String(oRow("NewContractExceptionCondition")), IDEmployee, TaskDay.Value) Then
                                    ' obtenemos el valor del saldo a fecha final del anterior contrato
                                    dblStartValue = GetAccrualValueOnDate(IDEmployee, oRow("IDConcept"), EndContractBefore)

                                    ' Nos guardamos el acumulado con su saldo
                                    myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), dblStartValue)

                                    bolApplyException = True

                                    mApplyStartUpValueAndCarryOver = roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "ApplyStartUpValueAndCarryOver")

                                    If Trim(mApplyStartUpValueAndCarryOver) = "1" Then
                                        ' En caso que este activado el parametro avanzado hay que incluir como valor inicial tanto el saldo arrastrado como el valor inicial calculado
                                        bolApplyException = False
                                    End If
                                End If
                            End If
                        End If

                        Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                        If Not bolApplyException Then
                            ' En otro caso, si no hay definidas excepciones o no se cumple la condicion de la misma

                            ' Obtenemos el valor inicial del saldo a partir de un valor fijo o campo de la ficha del empleado

                            ' En funcion del tipo obtenemos el valor de un campo de la ficha o un valor fijo
                            StartFieldType = Any2Double(oRow("StartValueType"))

                            Select Case StartFieldType
                                Case LabAgreeValueType.DirectValue
                                    ' Valor fijo
                                    StartValue = Any2String(oRow("StartValue"))
                                    StartValue = StartValue.Replace(".", oInfo.NumberDecimalSeparator)

                                    If strCustomization = "UEPMOP" And (Any2String(oRow("Name")).Contains("[UPF_AP]") Or Any2String(oRow("Name")).Contains("[UPF_VAC]")) Then
                                        ' UPF - Tratamiento especial valores iniciales
                                        ' Asuntos propios
                                        If Any2String(oRow("Name")).Contains("[UPF_AP]") Then
                                            GetStartupValue_UPF_AP(IDEmployee, oRow("IDConcept"), Any2Double(StartValue), IDLabAgree, StartValue_UPF_AP, TaskDay)
                                            ' Nos guardamos el acumulado con su saldo
                                            If myConceptsStartupValues.Exists(oRow("IDConcept")) Then
                                                myConceptsStartupValues(Any2Long(oRow("IDConcept"))) = myConceptsStartupValues(Any2Long(oRow("IDConcept"))) + StartValue_UPF_AP
                                            Else
                                                myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), StartValue_UPF_AP)
                                            End If

                                            ' Vacaciones
                                        ElseIf Any2String(oRow("Name")).Contains("[UPF_VAC]") Then
                                            GetStartupValue_UPF_VAC(IDEmployee, oRow("IDConcept"), Any2Double(StartValue), IDLabAgree, StartValue_UPF_VAC, TaskDay)
                                            ' Nos guardamos el acumulado con su saldo
                                            If myConceptsStartupValues.Exists(oRow("IDConcept")) Then
                                                myConceptsStartupValues(Any2Long(oRow("IDConcept"))) = myConceptsStartupValues(Any2Long(oRow("IDConcept"))) + StartValue_UPF_VAC
                                            Else
                                                myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), StartValue_UPF_VAC)
                                            End If
                                        End If
                                    Else
                                        ' Nos guardamos el acumulado con su saldo
                                        If myConceptsStartupValues.Exists(oRow("IDConcept")) Then
                                            myConceptsStartupValues(Any2Long(oRow("IDConcept"))) = myConceptsStartupValues(Any2Long(oRow("IDConcept"))) + Any2Double(StartValue)
                                        Else
                                            myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), Any2Double(StartValue))
                                        End If
                                    End If

                                Case LabAgreeValueType.UserField
                                    'Obtenemos el campo de la ficha del saldo a fecha
                                    StartField = Any2String(oRow("StartValue"))
                                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, StartField, TaskDay.Value, New UserFields.roUserFieldState, False)
                                    StartValue = Any2String(oUserField.FieldRawValue)

                                    If Len(StartValue) > 0 Then
                                        StartFieldType = Any2Double(ExecuteScalar("@SELECT# FieldType FROM sysroUserFields WHERE sysroUserFields.Type = 0 AND FieldName = '" & StartField.Replace("'", "''") & "' "))
                                        Select Case StartFieldType
                                            Case FieldTypesEnum.tNumeric, FieldTypesEnum.tDecimal, FieldTypesEnum.tTime 'Numeric, Decimal, Hora
                                                StartValue = StartValue.Replace(".", oInfo.NumberDecimalSeparator)
                                            Case Else
                                                StartValue = "-99999"
                                        End Select
                                        If StartValue <> "-99999" Then
                                            ' Nos guardamos el acumulado con su saldo
                                            If myConceptsStartupValues.Exists(oRow("IDConcept")) Then
                                                myConceptsStartupValues(Any2Long(oRow("IDConcept"))) = myConceptsStartupValues(Any2Long(oRow("IDConcept"))) + Any2Double(StartValue)
                                            Else
                                                myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), Any2Double(StartValue))
                                            End If
                                        End If
                                    End If
                                Case LabAgreeValueType.CalculatedValue
                                    ' Valor calculado
                                    If oConceptsCache(Any2Integer(oRow("IDConcept"))) IsNot Nothing Then
                                        For Each myConcept As roConceptEngine In oConceptsCache.Values
                                            dblScalingStartValue = 0
                                            If myConcept.ID = oRow("IDConcept") Then
                                                StartValue = GetStartupCalculatedValue(IDEmployee, myConcept, oRow, TaskDay)
                                                ScalingUserField = LTrim(RTrim(Any2String(oRow("ScalingUserField"))))
                                                ScalingCoefficientUserField = LTrim(RTrim(Any2String(oRow("ScalingCoefficientUserField"))))
                                                If Len(ScalingUserField) > 0 Then
                                                    ScalingDefinition = LTrim(RTrim(Any2String(oRow("ScalingValues"))))
                                                    dblScalingStartValue = 0
                                                    If strCustomization = "UEPMOP" And (Any2String(oRow("Name")).Contains("[UPF_ANT_AP]") Or Any2String(oRow("Name")).Contains("[UPF_ANT_VAC]")) Then
                                                        ' UPF - Tratamiento especial valores iniciales con escalado
                                                        ' Antigüedad asuntos propios
                                                        If Any2String(oRow("Name")).Contains("[UPF_ANT_AP]") Then
                                                            dblScalingStartValue = GetScalingStartupValue_UPF_ANT_AP(IDEmployee, myConcept, ScalingUserField, ScalingDefinition, TaskDay, ScalingCoefficientUserField)
                                                            ' Antigüedad Vacaciones
                                                        ElseIf Any2String(oRow("Name")).Contains("[UPF_ANT_VAC]") Then
                                                            dblScalingStartValue = GetScalingStartupValue_UPF_ANT_VAC(IDEmployee, myConcept, ScalingUserField, ScalingDefinition, TaskDay, ScalingCoefficientUserField)
                                                        End If
                                                    Else
                                                        dblScalingStartValue = GetScalingStartupValue(IDEmployee, myConcept, ScalingUserField, ScalingDefinition, TaskDay, ScalingCoefficientUserField)
                                                    End If
                                                End If
                                                If Any2Double(StartValue) <> 0 OrElse dblScalingStartValue <> 0 Then
                                                    ' Nos guardamos el acumulado con su saldo
                                                    If myConceptsStartupValues.Exists(oRow("IDConcept")) Then
                                                        myConceptsStartupValues(Any2Long(oRow("IDConcept"))) = myConceptsStartupValues(Any2Long(oRow("IDConcept"))) + Any2Double(StartValue) + dblScalingStartValue
                                                    Else
                                                        myConceptsStartupValues.Add(Any2Long(oRow("IDConcept")), Any2Double(StartValue) + dblScalingStartValue)
                                                    End If
                                                End If
                                                Exit For
                                            End If
                                        Next
                                    End If
                            End Select
                        End If
                    Next
                End If

                If myConceptsStartupValues.Count > 0 Then bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartupValuesOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartupValuesOnDate")
            End Try

            Return bRet

        End Function

        Private Function GetScalingStartupValue(ByVal EmployeeID As Long, ByVal myConcept As roConceptEngine, ByVal ScalingUserField As String, ByVal ScalingDefinition As String, ByVal TaskDate As roTime, ByVal ScalingCoefficientUserField As String) As Double
            Dim strScalingUserFieldValue As String
            Dim dateScalingUserFieldValue As Date
            Dim intScalingValue As Integer
            Dim intScalingCoefficientValue As Integer
            Dim strScalingCoefficientValue As String
            Dim dRet As Double = 0

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                ' Recupero año de antigüedad
                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, ScalingUserField, TaskDate.Value, New UserFields.roUserFieldState, False)
                strScalingUserFieldValue = Any2String(oUserField.FieldValue)
                If IsDate(strScalingUserFieldValue) Then
                    dateScalingUserFieldValue = Any2DateTime(strScalingUserFieldValue)
                Else
                    GetScalingStartupValue = 0
                    Exit Function
                End If

                ' Calculo antigüedad
                ' Sólo tengo en cuenta el año
                intScalingValue = Year(TaskDate.DateOnly) - Year(dateScalingUserFieldValue)

                ' La lista de escalado puede venir desordenada. Me voy a quedar con la diferencia positiva menor
                Dim iDiff As Integer
                Dim i As Integer
                Dim sTemp As String
                Dim iReference As Integer
                iDiff = -1
                For i = 0 To StringItemsCount(ScalingDefinition, "@") - 1
                    sTemp = String2Item(ScalingDefinition, i, "@")
                    If StringItemsCount(sTemp, "#") = 2 Then
                        iReference = String2Item(sTemp, 0, "#")
                        If (intScalingValue - iReference) >= 0 And (iDiff = -1 Or (intScalingValue - iReference) <= iDiff) Then
                            iDiff = intScalingValue - iReference
                            dRet = Any2Double(String2Item(sTemp, 1, "#").Replace(".", oInfo.NumberDecimalSeparator))
                        End If
                    Else
                        ' Especificación de escalado incorrecta (la pantalla no debería permitirlo ...)
                        GetScalingStartupValue = 0
                        Exit Function
                    End If
                Next

                ' Finalmente, si hay campo de coeficiente de proporcionalidad, lo aplico ahora
                If Len(ScalingCoefficientUserField) > 0 Then
                    Dim oUserFieldEx As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, ScalingCoefficientUserField, TaskDate.Value, New UserFields.roUserFieldState, False)
                    strScalingCoefficientValue = Any2String(oUserFieldEx.FieldRawValue)

                    If Len(strScalingCoefficientValue) > 0 Then
                        intScalingCoefficientValue = Any2Long(strScalingCoefficientValue)
                        If intScalingCoefficientValue > 0 Then
                            ' Como el campo de la ficha es numérico, y puede que acabe teniendo un cero por defecto, lo evito (no tiene sentido)
                            dRet = dRet * (intScalingCoefficientValue / 100)
                            ' Si resultan decimales, redondeo por abajo ...
                            dRet = Math.Round(dRet, 0)
                        End If
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetScalingStartupValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetScalingStartupValue")
            End Try

            Return dRet
        End Function

        Private Function GetScalingStartupValue_UPF_ANT_AP(ByVal EmployeeID As Long, ByVal myConcept As roConceptEngine, ByVal ScalingUserField As String, ByVal ScalingDefinition As String, ByVal TaskDate As roTime, ByVal ScalingCoefficientUserField As String) As Double
            Dim intScalingValue As Integer
            Dim dRet As Double = 0
            Dim sSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtenemos todos los valores del campo de la ficha hasta final de año para revisar si coincide alguno con el escalado
                sSQL = "@SELECT# Date, [Value] " &
                        " From EmployeeUserFieldValues with (nolock) " &
                        " WHERE EmployeeUserFieldValues.IDEmployee = " & EmployeeID & " AND " &
                        " EmployeeUserFieldValues.FieldName = '" & ScalingUserField.Replace("'", "''") & "'" &
                        " AND EmployeeUserFieldValues.Date >= " & Any2Time(TaskDate.Value).SQLSmallDateTime &
                        " AND EmployeeUserFieldValues.Date <= " & Any2Time("31/12/" & Year(TaskDate.Value)).SQLSmallDateTime &
                        " ORDER BY EmployeeUserFieldValues.Date asc "
                Dim ads As New DataTable
                ads = CreateDataTable(sSQL)

                If ads IsNot Nothing AndAlso ads.Rows.Count > 0 Then
                    For Each oRow As DataRow In ads.Rows
                        'Para cada valor , se verifica si coincide con el valor del escalado
                        intScalingValue = Any2Double(Any2String(oRow("Value")).Replace(".", roConversions.GetDecimalDigitFormat()))
                        ' La lista de escalado puede venir desordenada. Me voy a quedar con la diferencia positiva menor
                        Dim i As Integer
                        Dim sTemp As String
                        Dim iReference As Integer
                        For i = 0 To StringItemsCount(ScalingDefinition, "@") - 1
                            sTemp = String2Item(ScalingDefinition, i, "@")
                            If StringItemsCount(sTemp, "#") = 2 Then
                                iReference = Any2Double(String2Item(sTemp, 0, "#"))
                                If (intScalingValue = iReference) Then
                                    dRet = Any2Double(String2Item(sTemp, 1, "#").Replace(".", roConversions.GetDecimalDigitFormat()))
                                    Exit For
                                End If
                            Else
                                ' Especificación de escalado incorrecta (la pantalla no debería permitirlo ...)
                                Return 0
                                Exit Function
                            End If
                        Next
                    Next

                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::GetScalingStartupValue_UPF_ANT_AP")
                dRet = 0
            End Try

            Return dRet
        End Function

        Private Function GetScalingStartupValue_UPF_ANT_VAC(ByVal EmployeeID As Long, ByVal myConcept As roConceptEngine, ByVal ScalingUserField As String, ByVal ScalingDefinition As String, ByVal TaskDate As roTime, ByVal ScalingCoefficientUserField As String) As Double
            Dim strScalingUserFieldValue As String
            Dim intScalingValue As Double = 0
            Dim dRet As Double

            Try
                oState.Result = EngineResultEnum.NoError

                dRet = 0

                ' Recupero año de antigüedad a 31/12
                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, ScalingUserField, Any2Time("31/12/" & Year(TaskDate.Value) - 1).Value, New UserFields.roUserFieldState(-1), False)
                If oUserField IsNot Nothing Then
                    strScalingUserFieldValue = Any2String(oUserField.FieldRawValue)
                    intScalingValue = Any2Double(strScalingUserFieldValue.Replace(".", roConversions.GetDecimalDigitFormat()))
                End If

                Dim i As Integer
                Dim sTemp As String
                Dim iReference As Integer
                For i = 0 To StringItemsCount(ScalingDefinition, "@") - 1
                    sTemp = String2Item(ScalingDefinition, i, "@")
                    If StringItemsCount(sTemp, "#") = 2 Then
                        iReference = Any2Double(String2Item(sTemp, 0, "#"))
                        ' Si coincide la parte entera
                        If (Int(intScalingValue) = Int(iReference)) Then
                            dRet = Any2Double(String2Item(sTemp, 1, "#").Replace(".", roConversions.GetDecimalDigitFormat()))
                            Exit For
                        End If
                    Else
                        ' Especificación de escalado incorrecta (la pantalla no debería permitirlo ...)
                        Return 0
                        Exit Function
                    End If
                Next
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::GetScalingStartupValue_UPF_ANT_VAC")
                dRet = 0
            End Try

            Return dRet
        End Function

        Private Function GetStartupCalculatedValue(ByVal EmployeeID As Long, ByVal myConcept As roConceptEngine, ByVal oRow As DataRow, ByVal TaskDate As roTime) As String
            Dim StartContractDate As String = String.Empty
            Dim EndContractDate As String = String.Empty
            Dim StartAccrualDate As roTime = Nothing
            Dim EndAccrualDate As roTime = Nothing
            Dim strStartValueBase As String
            Dim dblStartValueBase As Double
            Dim strTotalPeriodBase As String
            Dim dblTotalPeriodBase As Double
            Dim ConceptType As String
            Dim bolInvalidConcept As Boolean
            Dim ActiveDays As Double
            Dim dblStartupCalculatedValue As Double
            Dim strAccruedValue As String
            Dim dblAccruedValue As Double
            Dim TotalHours As Double
            Dim sRet As String = String.Empty
            Dim StrEndCustomPeriodUserField As String

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                ' Valor inicial Base
                If Any2Double(oRow("StartValueBaseType")) = 0 Then
                    ' Directo
                    strStartValueBase = Any2String(oRow("StartValueBase")).Replace(".", oInfo.NumberDecimalSeparator)
                Else
                    ' Campo de la ficha
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, Any2String(oRow("StartValueBase")), TaskDate.Value, New UserFields.roUserFieldState(-1), False)
                    strStartValueBase = Any2String(oUserField.FieldRawValue)

                    If Len(strStartValueBase) > 0 Then
                        strStartValueBase = strStartValueBase.Replace(".", oInfo.NumberDecimalSeparator)
                    End If
                End If
                dblStartValueBase = Any2Double(strStartValueBase)

                ' Total Base
                If Any2Double(oRow("TotalPeriodBaseType")) = 0 Then
                    ' Directo
                    strTotalPeriodBase = Any2String(oRow("TotalPeriodBase")).Replace(".", oInfo.NumberDecimalSeparator)
                Else
                    ' Campo de la ficha
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, Any2String(oRow("TotalPeriodBase")), TaskDate.Value, New UserFields.roUserFieldState(-1), False)
                    strTotalPeriodBase = Any2String(oUserField.FieldRawValue)

                    If Len(strTotalPeriodBase) > 0 Then
                        strTotalPeriodBase = strTotalPeriodBase.Replace(".", oInfo.NumberDecimalSeparator)
                    End If
                End If
                dblTotalPeriodBase = Any2Double(strTotalPeriodBase)

                ' Total dias activos del empleado en el periodo del saldo
                ' Solo para saldos anuales y mensuales
                ConceptType = ExecuteScalar("@SELECT# ISNULL(DefaultQuery,'') FROM Concepts where ID = " & oRow("IDConcept"))
                Select Case ConceptType
                    Case "Y"
                        ' Si el acumulado es anual
                        StartAccrualDate = GetStartAccrualDate(EmployeeID, oRow("IDConcept"), TaskDate.Value, StartContractDate)
                        EndAccrualDate = StartAccrualDate.Add(1, "yyyy").Add(-1, "d")
                    Case "M"
                        ' Si el acumulado es mensual
                        StartAccrualDate = GetStartAccrualDate(EmployeeID, oRow("IDConcept"), TaskDate.Value, StartContractDate)
                        EndAccrualDate = StartAccrualDate.Add(1, "m").Add(-1, "d")
                    Case "L"
                        ' Si el acumulado es anual laboral
                        StartAccrualDate = GetStartAccrualDate(EmployeeID, oRow("IDConcept"), TaskDate.Value, StartContractDate)
                        Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(EmployeeID, TaskDate.Value, New roContractState(-1))
                        If lstDates.Count > 0 Then
                            StartAccrualDate = Any2Time(lstDates(0))
                            EndAccrualDate = Any2Time(lstDates(1))
                        End If

                    Case Else
                        bolInvalidConcept = True
                End Select

                If Not bolInvalidConcept Then
                    If IsDate(StartContractDate) Then
                        EndContractDate = Any2String(ExecuteScalar("@SELECT# EndDate FROM EmployeeContracts WHERE IDEmployee=" & EmployeeID & " AND  BeginDate=" & Any2Time(StartContractDate).SQLSmallDateTime))
                    End If

                    If IsDate(StartContractDate) And IsDate(EndContractDate) Then
                        If Any2Boolean(oRow("ApplyEndCustomPeriod")) Then
                            ' En el caso que el fin del periodo pueda ser configurable
                            ' Obtenmos si el campo de la ficha tiene un valor valido
                            ' en caso afirmativo,asignamos como fecha final la mínima de las 2
                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, Any2String(oRow("EndCustomPeriodUserField")), TaskDate.Value, New UserFields.roUserFieldState(-1), False)
                            StrEndCustomPeriodUserField = Any2String(oUserField.FieldValue)

                            If IsDate(StrEndCustomPeriodUserField) Then
                                EndContractDate = Any2String(roConversions.Min(Any2Time(StrEndCustomPeriodUserField).Value, Any2Time(EndContractDate).Value))
                            End If

                        End If

                        ActiveDays = Any2Double(DateDiff("d", roConversions.Max(StartAccrualDate.Value, Any2Time(StartContractDate).Value), roConversions.Min(Any2Time(EndContractDate).Value, EndAccrualDate.Value))) + 1
                        If ActiveDays < 0 Then ActiveDays = 0
                    End If
                End If

                ' En función del tipo de saldo,
                ' realizamos el calculo del valor inicial
                Select Case myConcept.IDType
                    Case "O" 'Dias/veces/personalizado

                        If dblTotalPeriodBase <> 0 Then
                            dblStartupCalculatedValue = (dblStartValueBase * ActiveDays) / dblTotalPeriodBase
                        End If

                    Case "H" ' Horas

                        ' Coeficiente de parcialidad
                        If Any2Double(oRow("AccruedValueType")) = 0 Then
                            ' Directo
                            strAccruedValue = Any2String(oRow("AccruedValue")).Replace(".", oInfo.NumberDecimalSeparator)
                        Else
                            ' Campo de la ficha
                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(EmployeeID, Any2String(oRow("AccruedValue")), TaskDate.Value, New UserFields.roUserFieldState(-1), False)
                            strAccruedValue = Any2String(oUserField.FieldRawValue)

                            If Len(strAccruedValue) > 0 Then
                                strAccruedValue = strAccruedValue.Replace(".", oInfo.NumberDecimalSeparator)
                            End If
                        End If
                        dblAccruedValue = Any2Double(strAccruedValue)

                        dblTotalPeriodBase = dblTotalPeriodBase * dblAccruedValue
                        dblStartValueBase = dblStartValueBase * dblAccruedValue

                        TotalHours = (ActiveDays * dblTotalPeriodBase) / (Any2Double(DateDiff("d", StartAccrualDate.Value, EndAccrualDate.Value)) + 1)

                        If dblTotalPeriodBase <> 0 Then
                            dblStartupCalculatedValue = (dblStartValueBase * TotalHours) / dblTotalPeriodBase
                        End If

                End Select

                ' Redondeo del valor resultante en caso necesario
                Select Case Any2Double(oRow("RoundingType"))
                    Case 1 ' Redondeo por arriba
                        If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                            dblStartupCalculatedValue = Int(dblStartupCalculatedValue) + 1
                        End If

                    Case 2 ' Redondeo por abajo
                        If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                            dblStartupCalculatedValue = Int(dblStartupCalculatedValue)
                        End If

                    Case 3 ' Redondeo por aproximacion
                        If dblStartupCalculatedValue - Int(dblStartupCalculatedValue) > 0 Then
                            If (dblStartupCalculatedValue - Int(dblStartupCalculatedValue)) >= 0.5 Then
                                dblStartupCalculatedValue = Int(dblStartupCalculatedValue) + 1
                            Else
                                dblStartupCalculatedValue = Int(dblStartupCalculatedValue)
                            End If
                        End If
                End Select

                sRet = Any2String(dblStartupCalculatedValue).Replace(".", oInfo.NumberDecimalSeparator)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartupCalculatedValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartupCalculatedValue")

            End Try

            Return sRet
        End Function

        Private Function GetAccrualValueOnDate(ByVal IDEmployee As Long, ByVal IDConcept As Integer, ByVal strAtDate As String) As Double
            Dim StartAccrualDate As roTime
            Dim StartContractDate As String = String.Empty
            Dim TaskDay As New roTime
            Dim strSQL As String
            Dim dRet As Double = 0

            Try

                oState.Result = EngineResultEnum.NoError

                If Not IsDate(strAtDate) Then Return dRet

                TaskDay = Any2Time(strAtDate)

                StartAccrualDate = GetStartAccrualDate(IDEmployee, IDConcept, TaskDay.Value, StartContractDate)

                strSQL = " @SELECT# SUM(Value) FROM DailyAccruals " &
                         " WHERE IDEmployee=" & IDEmployee & " AND IDConcept=" & IDConcept &
                         " AND Date >= " & StartAccrualDate.SQLSmallDateTime & " AND Date <= " & TaskDay.SQLSmallDateTime

                If IsDate(StartContractDate) Then
                    strSQL = strSQL & " AND Date >= " & Any2Time(StartContractDate).SQLSmallDateTime
                End If

                dRet = Any2Double(ExecuteScalar(strSQL))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetAccrualValueOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetAccrualValueOnDate")
            End Try

            Return dRet
        End Function

        Private Function Execute_EvaluateStartUpCondition(ByRef StartUpCondition As String, ByVal EmployeeID As Long, ByVal zDate As Date) As Boolean
            '
            ' Devuelve True si la regla de expcecion del valor inicial del saldo se cumple
            '
            Dim bolRet As Boolean = False
            Dim m_Definition As New roCollection
            Dim strUserFieldName As String
            Dim dblUserFieldDataType As Double
            Dim dblCompare As Double
            Dim strValue As String
            Dim strSQL As String
            Dim w As Integer
            Dim strRet As String = String.Empty

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                bolRet = False

                m_Definition.LoadXMLString(StartUpCondition)

                If Any2Double(m_Definition.Item("TotalConditions")) = 0 Then
                    ' Al no haber ninguna condicion aplicamos el valor del saldo del contrato anterior
                    bolRet = True
                    Return bolRet
                End If

                For w = 1 To Any2Double(m_Definition.Item("TotalConditions"))
                    bolRet = False

                    strUserFieldName = Replace(Any2String(m_Definition.Item("Condition" & w)("UserFieldName")), "'", "''")
                    dblUserFieldDataType = Any2Double(m_Definition.Item("Condition" & w)("UserFieldDataType"))
                    dblCompare = Any2Double(m_Definition.Item("Condition" & w)("Compare"))
                    strValue = Replace(Any2String(m_Definition.Item("Condition" & w)("Value")), "'", "''")

                    Select Case dblUserFieldDataType
                        Case 0 ' FieldTypes.tText
                            Select Case dblCompare
                                Case 0 'CompareType.Equal
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) = '" & strValue & "'"
                                Case 1 'CompareType.Minor
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) < '" & strValue & "'"
                                Case 2 'CompareType.MinorEqual
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) <= '" & strValue & "'"
                                Case 3 'CompareType.Major
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) > '" & strValue & "'"
                                Case 4 'CompareType.MajorEqual
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) >= '" & strValue & "'"
                                Case 5 'CompareType.Distinct
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) <> '" & strValue & "'"
                                Case 6 'CompareType.Contains
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '%" & strValue & "%'"
                                Case 7 ' CompareType.NotContains
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) NOT LIKE '%" & strValue & "%'"
                                Case 8 'CompareType.StartWith
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '" & strValue & "%'"
                                Case 9 'CompareType.EndWidth
                                    strRet = "CONVERT(varchar, ISNULL([Value],'')) LIKE '%" & strValue & "'"
                            End Select

                        Case 1 ' FieldTypes.tNumeric
                            If IsNumeric(strValue) Then
                                strValue = strValue.Replace(".", oInfo.NumberDecimalSeparator)
                                Select Case dblCompare
                                    Case 0 'CompareType.Equal
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) = " & strValue
                                    Case 1 ' CompareType.Minor
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) < " & strValue
                                    Case 2 'CompareType.MinorEqual
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) <= " & strValue
                                    Case 3 'CompareType.Major
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) > " & strValue
                                    Case 4 'CompareType.MajorEqual
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) >= " & strValue
                                    Case 5 'CompareType.Distinct
                                        strRet = "CONVERT(int, CONVERT(varchar, [Value])) <> " & strValue
                                End Select
                            End If
                    End Select

                    strSQL = "@DECLARE# @Date smalldatetime SET @Date = " & Any2Time(zDate).SQLSmallDateTime & " @SELECT# Employees.ID FROM Employees, GetAllEmployeeUserFieldValue('" & strUserFieldName & "', @Date) V "
                    strSQL = strSQL + " WHERE Employees.ID = V.idEmployee AND " & strRet & " AND Employees.ID = " & EmployeeID
                    If Any2Double(ExecuteScalar(strSQL)) > 0 Then bolRet = True

                    If Not bolRet Then Return bolRet

                Next w
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateStartUpCondition")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_EvaluateStartUpCondition")
            End Try

            Return bolRet

        End Function

        Private Function GetStartAccrualDate(ByVal IDEmployee As Long, ByVal IDConcept As Integer, ByVal strAtDate As String, ByRef StartContractDate As String) As roTime
            '
            ' Obtenemos el inicio del periodo del saldo en funcion del tipo (Anual, mensual....)
            '
            Dim bRet As roTime = Nothing
            Dim StartDateDayM As Date
            Dim StartDateDayY As Date
            Dim StartDateDayW As Date
            Dim ConceptType As String
            Dim StartAccrualDate As roTime
            Dim TaskDay As New roTime

            Try

                StartAccrualDate = Any2Time(roNullDate)

                If Not IsDate(strAtDate) Then Return bRet

                TaskDay = Any2Time(strAtDate)

                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados mensuales
                StartDateDayM = GetStartDateForMonthAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día desde el que tengo que acumular el valor de los acumulados anuales
                StartDateDayY = GetStartDateForYearAccruals(intYearIniMonth, intMonthIniDay, TaskDay.Value)
                ' Calculamos el día de inicio de semana
                StartDateDayW = GetStartDateForWeekAccruals(intWeekIniday, TaskDay.Value)

                ' Obtenemos fecha de inicio de contrato actual
                ' y calculamos a partir de esa fecha
                StartContractDate = Any2String(ExecuteScalar("@SELECT# TOP 1 BeginDate FROM EmployeeContracts where IDEmployee = " & IDEmployee & " AND BeginDate <= " & TaskDay.SQLSmallDateTime & " Order by BeginDate desc"))

                ' Recupero el tipo de acumulado (mensual o anual). El valor del acumulado se calculará en función de dicho tipo
                ConceptType = ExecuteScalar("@SELECT# DefaultQuery FROM Concepts WHERE ID = " & IDConcept)

                ' Calculamos el valor del acumulado principal desde el primer día del período actual
                ' y hata el día de la tarea que estamos tratando
                Select Case ConceptType
                    Case "Y"
                        ' Si el acumulado es anual
                        StartAccrualDate = Any2Time(StartDateDayY)
                    Case "M"
                        ' Si el acumulado es mensual
                        StartAccrualDate = Any2Time(StartDateDayM)
                    Case "W"
                        ' Si el acumulado es semanal
                        StartAccrualDate = Any2Time(StartDateDayW)
                    Case "C"
                        ' Si el acumulado es por contrato
                        If IsDate(StartContractDate) Then
                            StartAccrualDate = Any2Time(StartContractDate)
                        Else
                            StartAccrualDate = Any2Time(roNullDate)
                        End If
                    Case "L"
                        ' Si el acumulado es anual laboral
                        Dim lstDates As Generic.List(Of DateTime) = roBusinessSupport.GetDatesOfAnnualWorkPeriodsInDate(IDEmployee, TaskDay.Value, New roContractState(-1))
                        If lstDates.Count > 0 Then
                            StartAccrualDate = Any2Time(lstDates(0))
                        Else
                            StartAccrualDate = Any2Time(roNullDate)
                        End If

                End Select

                bRet = StartAccrualDate
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartAccrualDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartAccrualDate")

            End Try

            Return bRet
        End Function

        Private Function GetConditionValue(ByVal oConditionCauses As Generic.List(Of roShiftDailyRuleConditionCause), ByVal oConditionTimeZones As Generic.List(Of roShiftDailyRuleConditionTimeZone), ByVal zCauses As DataTable, ByVal IDEmployee As Long, ByVal TaskDate As Date) As Double
            ' Obtenemos el total de las justificacion diarias del empleado
            ' que se encuentran dentro de la condicion
            Dim mSign As Double
            Dim IDCause As Double
            Dim IDZone As Double
            Dim ExpectedWorkingHours As Double
            Dim bolret As Double = 0

            Try

                If oConditionCauses.Count = 0 Or oConditionTimeZones.Count = 0 Then
                    Return bolret
                    Exit Function
                End If

                If zCauses IsNot Nothing AndAlso zCauses.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In zCauses.Rows
                        ' Para cada justificacion diaria del empleado
                        ' Obtenemos ID, Zona y Valor
                        IDCause = Any2Double(oRowEmp("IDCause"))
                        If IsDBNull(oRowEmp("IDCause")) Then oRowEmp("IDCause") = -1
                        IDZone = Any2Double(oRowEmp("IDTimeZone"))

                        If IDCause <> CONCEPT_SHIFTEXPECTEDHOURS Then
                            ' Si la justificacion no es horas teoricas

                            ' Recorremos la lista de justificaciones y zonas de la condicion
                            ' para ver si coincide alguna
                            For Each xConditionCause As roShiftDailyRuleConditionCause In oConditionCauses
                                If IDCause = xConditionCause.IDCause Then
                                    ' Si coincide la justificacion, verificamos si coincide la zona horaria
                                    For Each xConditionTimeZone As roShiftDailyRuleConditionTimeZone In oConditionTimeZones
                                        If IDZone = xConditionTimeZone.IDTimeZone Or xConditionTimeZone.IDTimeZone = -1 Then
                                            mSign = 1
                                            If xConditionCause.Operation = OperatorCondition.Negative Then mSign = -1
                                            If Any2Double(oRowEmp("Value")) >= 0 Then
                                                bolret = bolret + (Any2Time(oRowEmp("Value"), True).VBNumericValue * mSign)
                                            Else
                                                bolret = bolret + ((Any2Time(oRowEmp("Value") * -1, True).VBNumericValue * -1) * mSign)
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If

                ' Si en la condicion existe la justificacion de horas teoricas, la añadimos con su valor
                For Each xConditionCause As roShiftDailyRuleConditionCause In oConditionCauses
                    If CONCEPT_SHIFTEXPECTEDHOURS = xConditionCause.IDCause Then
                        ExpectedWorkingHours = Any2Double(
                                    ExecuteScalar("@SELECT# (case when isnull(DailySchedule.IsHolidays,0) = 1 then 0 else isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  end) FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & IDEmployee & " AND DATE=" & Any2Time(TaskDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "))

                        mSign = 1
                        If xConditionCause.Operation = OperatorCondition.Negative Then mSign = -1
                        bolret = bolret + (Any2Time(ExpectedWorkingHours, True).VBNumericValue * mSign)
                        Exit For
                    End If
                Next

                If bolret >= 0 Then
                    bolret = Any2Time(Date.FromOADate(bolret), True).VBNumericValue
                Else
                    bolret = Any2Time(Date.FromOADate(bolret * -1), True).VBNumericValue * -1
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConditionValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConditionValue")
            End Try

            Return bolret

        End Function

        Private Function Execute_ApplyShiftDailyRule(ByRef Rule As roShiftDailyRule, ByVal zCauses As DataTable, ByVal IDEmployee As Long, ByVal TaskDate As Date, zManualCenters As DataTable) As Boolean
            '
            ' Se aplican las acciones de la regla
            '
            Dim IDCause As Double
            Dim IDCauseTo As Double
            Dim mValue As Double
            Dim mMaxValue As Double
            Dim mResultValue As Double
            Dim bolret As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                If Rule.Actions.Count = 0 Then
                    Return True
                End If

                For Each oAction As roShiftDailyRuleAction In Rule.Actions
                    ' Ejecutamos cada acción
                    mValue = 0
                    mResultValue = 0

                    ' En funcion de la accion
                    Select Case oAction.Action
                        Case RuleAction.CarryOver ' Arrastre
                            ' Accion
                            Select Case oAction.CarryOverAction
                                Case DailyConditionValueType.DirectValue ' Valor directo
                                    If Not oAction.CarryOverDirectValue Is Nothing Then
                                        If InStr(oAction.CarryOverDirectValue, ":") > 0 Then
                                            ' Formato HH:MM
                                            If InStr(oAction.CarryOverDirectValue, "-") = 0 Then
                                                mValue = Any2Time(oAction.CarryOverDirectValue, True).NumericValue(True)
                                            Else
                                                mValue = Any2Time(Mid$(oAction.CarryOverDirectValue, 2), True).NumericValue(True) * -1
                                            End If
                                        Else
                                            ' Formato numérico
                                            mValue = Any2Double(oAction.CarryOverDirectValue.Replace(".", oInfo.NumberDecimalSeparator))
                                        End If
                                    End If
                                Case DailyConditionValueType.UserField ' Campo de la ficha
                                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oAction.CarryOverUserFieldValue, TaskDate, New UserFields.roUserFieldState(-1), False)
                                    If oUserField IsNot Nothing Then
                                        mValue = Any2Time(Any2Double(oUserField.FieldRawValue), True).NumericValue(True)
                                    End If

                                Case DailyConditionValueType.ID  ' Con la diferencia entre
                                    'Parte/Condicion
                                    GetValueCondition(oAction.CarryOverConditionPart, oAction.CarryOverConditionNumber, zCauses, IDEmployee, TaskDate, Rule, mValue)

                                    ' y
                                    Select Case oAction.CarryOverActionResult
                                        Case DailyConditionValueType.DirectValue ' Valor directo
                                            If InStr(oAction.CarryOverDirectValueResult, ":") > 0 Then
                                                ' Formato HH:MM
                                                If InStr(oAction.CarryOverDirectValueResult, "-") = 0 Then
                                                    mResultValue = Any2Time(oAction.CarryOverDirectValueResult, True).NumericValue(True)
                                                Else
                                                    mResultValue = Any2Time(Mid$(oAction.CarryOverDirectValueResult, 2), True).NumericValue(True) * -1
                                                End If
                                            Else
                                                ' Formato numerico
                                                mResultValue = Any2Double(oAction.CarryOverDirectValueResult.Replace(".", oInfo.NumberDecimalSeparator))
                                            End If
                                        Case DailyConditionValueType.UserField ' Campo de la ficha
                                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oAction.CarryOverUserFieldValueResult, TaskDate, New UserFields.roUserFieldState(-1), False)
                                            If oUserField IsNot Nothing Then
                                                mResultValue = Any2Time(Any2Double(oUserField.FieldRawValue), True).NumericValue(True)
                                            End If

                                        Case DailyConditionValueType.ID
                                            'Parte/Condicion
                                            GetValueCondition(oAction.CarryOverConditionPartResult, oAction.CarryOverConditionNumberResult, zCauses, IDEmployee, TaskDate, Rule, mResultValue)
                                    End Select

                                    ' Obtenemos la diferencia absoluta
                                    mValue = mResultValue - mValue
                                    mValue = Math.Abs(mValue)
                            End Select

                            'Justificaciones a tratar
                            IDCause = oAction.CarryOverIDCauseFrom
                            IDCauseTo = oAction.CarryOverIDCauseTo

                            ' Generamos el ajuste en negativo a la primera
                            CreateDailyPlusCause(IDEmployee, Any2Time(TaskDate), mValue * -1, IDCause, zManualCenters)

                            ' Generamos el ajuste en positivo a la segunda
                            CreateDailyPlusCause(IDEmployee, Any2Time(TaskDate), mValue, IDCauseTo, zManualCenters)
                        Case RuleAction.Plus  ' Plus

                            Dim bValueIsUserField As Boolean
                            bValueIsUserField = False

                            ' Justificacion a generar
                            IDCause = oAction.PlusIDCause

                            ' Accion
                            Select Case oAction.PlusAction
                                Case DailyConditionValueType.DirectValue ' Valor directo
                                    If InStr(oAction.PlusDirectValue, ":") > 0 Then
                                        ' Formato HH:MM
                                        If InStr(oAction.PlusDirectValue, "-") = 0 Then
                                            mValue = Any2Time(oAction.PlusDirectValue, True).NumericValue(True)
                                        Else
                                            mValue = Any2Time(Mid$(oAction.PlusDirectValue, 2), True).NumericValue(True) * -1
                                        End If
                                    Else
                                        ' Formato numerico
                                        mValue = Any2Double(oAction.PlusDirectValue.Replace(".", oInfo.NumberDecimalSeparator))
                                    End If
                                Case DailyConditionValueType.UserField ' Campo de la ficha
                                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oAction.PlusUserFieldValue, TaskDate, New UserFields.roUserFieldState(-1), False)
                                    mValue = Any2Time(Any2Double(oUserField.FieldRawValue), True).NumericValue(True)
                                    bValueIsUserField = True
                                Case DailyConditionValueType.ID  ' Con la diferencia entre
                                    'Parte/Condicion
                                    GetValueCondition(oAction.PlusConditionPart, oAction.PlusConditionNumber, zCauses, IDEmployee, TaskDate, Rule, mValue)
                                    ' y
                                    Select Case oAction.PlusActionResult
                                        Case DailyConditionValueType.DirectValue ' Valor directo
                                            If InStr(oAction.PlusDirectValueResult, ":") > 0 Then
                                                If InStr(oAction.PlusDirectValueResult, "-") = 0 Then
                                                    mResultValue = Any2Time(oAction.PlusDirectValueResult, True).NumericValue(True)
                                                Else
                                                    mResultValue = Any2Time(Mid$(oAction.PlusDirectValueResult, 2), True).NumericValue(True) * -1
                                                End If
                                            Else
                                                ' Formato numérico
                                                mResultValue = Any2Double(oAction.PlusDirectValueResult.Replace(".", oInfo.NumberDecimalSeparator))
                                            End If
                                        Case DailyConditionValueType.UserField ' Campo de la ficha
                                            Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, oAction.PlusUserFieldValueResult, TaskDate, New UserFields.roUserFieldState(-1), False)
                                            mResultValue = Any2Time(Any2Double(oUserField.FieldRawValue), True).NumericValue(True)
                                        Case DailyConditionValueType.ID ' Parte/Condicion
                                            GetValueCondition(oAction.PlusConditionPartResult, oAction.PlusConditionNumberResult, zCauses, IDEmployee, TaskDate, Rule, mResultValue)
                                    End Select

                                    ' Obtenemos la diferencia absoluta
                                    mValue = mResultValue - mValue
                                    mValue = Math.Abs(mValue)

                                    ' Signo
                                    If oAction.PlusActionSign = OperatorCondition.Negative Then mValue = mValue * -1
                            End Select

                            ' Añadimos el Plus con la justificacion con el valor correspondiente
                            ' Si viene de campo de la ficha, sólo añado si el valor es distinto de cero, para que no se generen líneas con cero ... (Torras y estandar)
                            If Not bValueIsUserField OrElse (bValueIsUserField And mValue <> 0) Then
                                CreateDailyPlusCause(IDEmployee, Any2Time(TaskDate), mValue, IDCause, zManualCenters)
                            End If
                        Case RuleAction.CarryOverSingle 'Arrastre Individual
                            ' Obtenemos el valor maximo a arrastrar a partir del valor de la justificacion
                            IDCause = oAction.CarryOverSingleCause
                            mMaxValue = GetDailyCauseValue(IDEmployee, TaskDate, IDCause)

                            ' Obtenemos la lista de justificaciones a arrastrar
                            If oAction.ActionCauses IsNot Nothing AndAlso oAction.ActionCauses.Count > 0 Then
                                ' Aplicamos los arrastres individuales hasta el valor maximo de la justificacion
                                ApplyCarryOverValue(oAction.ActionCauses, IDEmployee, TaskDate, mMaxValue, zManualCenters, zCauses)
                            Else
                                Return True
                            End If
                    End Select
                Next

                bolret = True
            Catch ex As Data.Common.DbException
                oState.LogLevel = roLog.EventType.roError
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_ApplyShiftDailyRule")
            Catch ex As Exception
                oState.LogLevel = roLog.EventType.roError
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_ApplyShiftDailyRule")
            End Try

            Return bolret
        End Function

        Private Sub ApplyCarryOverValue(ByVal CarryOverActionCauses As List(Of roShiftDailyRuleActionCause), ByVal IDEmployee As Long, ByVal TaskDate As Date, ByVal mMaxValue As Double, ByVal zManualCenters As DataTable, ByVal zCauses As DataTable)
            ' Aplicamos a cada justificación de la lista el arrastre hasta el maximo del valor indicado
            ' que se encuentran dentro de la condicion, en el caso que se haya generado ese dia y en orden de creacion

            Dim IDCause As Double
            Dim xTimeValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                If CarryOverActionCauses.Count = 0 OrElse mMaxValue <= 0 Then Exit Sub

                mMaxValue = Any2Time(mMaxValue).NumericValue

                For Each oCauseRow As DataRow In zCauses.Rows
                    ' Para cada justificacion diaria del empleado
                    ' Obtenemos ID y Valor
                    IDCause = oCauseRow("IDCause")
                    If Any2String(oCauseRow("IDTimeZone")) = "" Then oCauseRow("IDTimeZone") = -1

                    ' Recorremos la lista de justificaciones para ver si coincide alguna
                    For Each oRule As roShiftDailyRuleActionCause In CarryOverActionCauses
                        If IDCause = Any2Double(oRule.IDCause) AndAlso Any2Double(oCauseRow("Value")) > 0 Then
                            ' Si coincide la justificacion

                            If mMaxValue <= 0 Then
                                xTimeValue = 0
                            Else
                                ' Justificamos el tiempo hasta el maximo
                                If mMaxValue >= Any2Time(oCauseRow("Value")).NumericValue Then
                                    ' Si el tiempo maximo es mayor o igual a la justificacion
                                    ' arrastramos todo el tiempo de la justificacion
                                    xTimeValue = Any2Time(oCauseRow("Value")).NumericValue

                                    ' Restamos del tiempo maximo el valor de la justificacion
                                    mMaxValue = mMaxValue - xTimeValue
                                Else
                                    ' Arrastramos parte de la justificacion con el valor del tiempo maximo
                                    xTimeValue = mMaxValue

                                    ' Dejamos el tiempo maximo a 0
                                    mMaxValue = 0
                                End If

                                ' Generamos el ajuste en negativo a la primera
                                CreateDailyPlusCause(IDEmployee, Any2Time(TaskDate), xTimeValue * -1, IDCause, zManualCenters)

                                ' Generamos el ajuste en positivo a la segunda
                                CreateDailyPlusCause(IDEmployee, Any2Time(TaskDate), xTimeValue, Any2Double(oRule.IDCause2), zManualCenters)
                            End If
                        End If
                    Next
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ApplyCarryOverValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ApplyCarryOverValue")
            End Try

        End Sub

        Private Sub ExecuteConceptMaximumValues(ByVal IDEmployee As Long, ByVal TaskDate As roTime)
            '
            ' Cálculo de valores maximos de los acumulados del convenio actual del empleado (Live)
            '
            Dim strSQL As String
            Dim MaximumValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtenemos el convenio que tiene asignado el empleado en la fecha de cálculo y los acumulados con Valores Maximos
                strSQL = "@SELECT# EmployeeContracts.IDEmployee, EmployeeContracts.IDLabAgree, MaximumValue, MaximumValueType, StartUpValues.IDConcept,EmployeeContracts.BeginDate as BeginContract, EmployeeContracts.EndDate as EndContract, EmployeeContracts.IDContract  FROM EmployeeContracts, LabAgreeStartUpValues, StartUpValues WHERE "
                strSQL = strSQL & " EmployeeContracts.IDEmployee= " & IDEmployee
                strSQL = strSQL & " AND EmployeeContracts.BeginDate <= " & TaskDate.SQLSmallDateTime
                strSQL = strSQL & " AND EmployeeContracts.EndDate >= " & TaskDate.SQLSmallDateTime
                strSQL = strSQL & " AND EmployeeContracts.IDLabAgree is Not Null AND EmployeeContracts.IDLabAgree > 0 "
                strSQL = strSQL & " AND LabAgreeStartUpValues.IDLabAgree = EmployeeContracts.IDLabAgree"
                strSQL = strSQL & " AND LabAgreeStartUpValues.IDStartupValue = StartUpValues.IDStartupValue AND LEN(MaximumValue) > 0 AND MaximumValue is Not Null "

                Dim tbConcepts As DataTable
                tbConcepts = CreateDataTable(strSQL)

                If tbConcepts IsNot Nothing AndAlso tbConcepts.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbConcepts.Rows
                        ' Para cada acumulado
                        If GetConceptMaxLimit(oRow, MaximumValue) Then
                            ' Comprobamos el valor máximo
                            ExecuteEmployeeMaximiumLimits(Any2Long(oRow("IDEmployee")), Any2Double(oRow("IDConcept")), MaximumValue, oRow, TaskDate)
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteConceptMaximumValues")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteConceptMaximumValues")
            End Try
        End Sub

        Private Sub ExecuteConceptMinimumValues(ByVal IDEmployee As Long, ByVal TaskDate As roTime)
            '
            ' Cálculo de valores mínimos de los acumulados del convenio actual del empleado (Live)
            '
            Dim strSQL As String
            Dim MinimumValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtenemos el convenio que tiene asignado a la fecha de calculo  el empleado y los acumulados con Valores Maximos
                strSQL = "@SELECT# EmployeeContracts.IDEmployee, EmployeeContracts.IDLabAgree, MinimumValue, MinimumValueType, StartUpValues.IDConcept ,EmployeeContracts.BeginDate as BeginContract, EmployeeContracts.EndDate as EndContract, EmployeeContracts.IDContract FROM EmployeeContracts, LabAgreeStartUpValues, StartUpValues WHERE "
                strSQL = strSQL & " EmployeeContracts.IDEmployee= " & IDEmployee
                strSQL = strSQL & " AND EmployeeContracts.BeginDate <= " & TaskDate.SQLSmallDateTime
                strSQL = strSQL & " AND EmployeeContracts.EndDate >= " & TaskDate.SQLSmallDateTime
                strSQL = strSQL & " AND EmployeeContracts.IDLabAgree is Not Null AND EmployeeContracts.IDLabAgree > 0 "
                strSQL = strSQL & " AND LabAgreeStartUpValues.IDLabAgree = EmployeeContracts.IDLabAgree"
                strSQL = strSQL & " AND LabAgreeStartUpValues.IDStartupValue = StartUpValues.IDStartupValue AND LEN(MinimumValue) > 0 AND MinimumValue is Not Null "

                Dim tbConcepts As DataTable
                tbConcepts = CreateDataTable(strSQL)

                ' Realiza comprobaciones de cada empleado y acumulado
                If tbConcepts IsNot Nothing AndAlso tbConcepts.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbConcepts.Rows
                        ' Para cada acumulado
                        If GetConceptMinLimit(oRow, MinimumValue) Then
                            ' Comprobamos el valor máximo
                            ExecuteEmployeeMinimumLimits(Any2Long(oRow("IDEmployee")), Any2Double(oRow("IDConcept")), MinimumValue, oRow, TaskDate)
                        End If
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteConceptMinimumValues")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteConceptMinimumValues")

            End Try
        End Sub

        Private Function GetConceptMaxLimit(ByVal myDS As DataRow, ByRef mMaximumValue As Double)
            '
            ' Obtenemos los limites para cada acumulado
            '
            Dim MaximumField As String
            Dim MaximumValue As String
            Dim MaximumFieldType As Double

            Dim bRet As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                ' En funcion del tipo de valor obtenemos el campo de la ficha o un valor fijo
                MaximumFieldType = Any2Double(myDS("MaximumValueType"))

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                Select Case MaximumFieldType
                    Case 1
                        ' Obtenemos el valor fijo
                        MaximumValue = Any2String(myDS("MaximumValue"))
                        MaximumValue = MaximumValue.Replace(".", oInfo.NumberDecimalSeparator)
                        mMaximumValue = Any2Double(MaximumValue)
                        bRet = True

                    Case Else

                        'Obtenemos el campo de la ficha del valor máximo a ultimo dia del contrato
                        MaximumField = Any2String(myDS("MaximumValue"))
                        MaximumValue = Any2String(ExecuteScalar("@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE IDEmployee=" & myDS("IDEmployee") & " AND FieldName = '" & MaximumField.Replace("'", "''") & "' AND Date <= " & Any2Time(myDS("EndContract")).SQLSmallDateTime & " AND Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " ORDER BY Date DESC"))

                        If Len(MaximumValue) > 0 And MaximumFieldType = 2 Then
                            MaximumFieldType = Any2Double(ExecuteScalar("@SELECT# FieldType FROM sysroUserFields WHERE FieldName = '" & MaximumField.Replace("'", "''") & "' "))
                            Select Case MaximumFieldType
                                Case 1, 3, 4 'Numeric, Decimal, Hora
                                    MaximumValue = MaximumValue.Replace(".", oInfo.NumberDecimalSeparator)
                                Case Else
                                    MaximumValue = "-99999"
                            End Select
                            If MaximumValue <> "-99999" Then
                                mMaximumValue = Any2Double(MaximumValue)
                                bRet = True
                            End If
                        End If
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConceptMaxLimit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConceptMaxLimit")
            End Try

            Return bRet
        End Function

        Private Function GetConceptMinLimit(ByVal myDS As DataRow, ByRef mMinimumValue As Double) As Boolean
            '
            ' Obtenemos los limites mínimos para cada acumulado
            '
            Dim MinimumField As String
            Dim MinimumValue As String
            Dim MinimumFieldType As Double
            Dim bRet As Boolean = False

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat

                ' En funcion del tipo de valor obtenemos el campo de la ficha o un valor fijo
                MinimumFieldType = Any2Double(myDS("MinimumValueType"))

                Select Case MinimumFieldType
                    Case 1
                        ' Obtenemos el valor fijo
                        MinimumValue = Any2String(myDS("MinimumValue"))
                        MinimumValue = MinimumValue.Replace(".", oInfo.NumberDecimalSeparator)
                        mMinimumValue = Any2Double(MinimumValue)
                        bRet = True

                    Case Else

                        'Obtenemos el campo de la ficha del valor minimo a final de contrato
                        MinimumField = Any2String(myDS("MinimumValue"))
                        MinimumValue = Any2String(ExecuteScalar("@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE IDEmployee=" & myDS("IDEmployee") & " AND FieldName = '" & MinimumField.Replace("'", "''") & "' AND Date <= " & Any2Time(myDS("EndContract")).SQLSmallDateTime & " AND Date <= " & Any2Time(Now.Date).SQLSmallDateTime & " Order By Date desc"))

                        If Len(MinimumValue) > 0 And MinimumFieldType = 2 Then
                            MinimumFieldType = Any2Double(ExecuteScalar("@SELECT# FieldType FROM sysroUserFields WHERE FieldName = '" & MinimumField.Replace("'", "''") & "' "))
                            Select Case MinimumFieldType
                                Case 1, 3, 4 'Numeric, Decimal, Hora
                                    MinimumValue = MinimumValue.Replace(".", oInfo.NumberDecimalSeparator)
                                Case Else
                                    MinimumValue = "-99999"
                            End Select
                            If MinimumValue <> "-99999" Then
                                mMinimumValue = Any2Double(MinimumValue)
                                bRet = True
                            End If
                        End If
                End Select
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConceptMinLimit")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetConceptMinLimit")
            End Try

            Return bRet

        End Function

        Private Sub ExecuteEmployeeMaximiumLimits(ByVal zEmployee As Long, ByVal zIDConcept As Integer, ByVal zMaxValue As Double, ByVal myDS As DataRow, ByVal TaskDate As roTime)
            '
            ' Comprueba los limites Anuales o mensuales
            '

            Dim strSQL As String
            Dim sConceptType As String
            Dim sTotalConcept As Double
            Dim sMaxValue As Double
            Dim FirstDay As Date
            Dim Lastday As Date
            Dim BeginMonth As Long
            Dim BeginDay As Long
            Dim WeekBeginDay As Integer
            Dim BeginYear As Long
            Dim BeginDateRule As roTime
            Dim EndDateRule As roTime
            Dim BeginDate As Date
            Dim FinishDate As Date

            Dim IDNotification As Double

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oConcept As roConceptEngine = oConceptsCache.Item(zIDConcept)
                If oConcept Is Nothing Then
                    Exit Sub
                End If
                sConceptType = oConcept.DefaultQuery
                BeginDate = oConcept.BeginDate
                FinishDate = oConcept.FinishDate

                ' Valor Máximo
                sMaxValue = Math.Round(zMaxValue, 4)

                If sConceptType = "M" Then
                    ' Obtenemos fechas de los acumulados mensuales
                    CHECK_GetEmployeeAccruals_MonthQueryDates(FirstDay, Lastday, TaskDate)
                    Lastday = DateAdd("m", 1, FirstDay)
                    Lastday = DateAdd("d", -1, Lastday)
                ElseIf sConceptType = "W" Then
                    ' Obtenemos fechas para saldos semanales
                    WeekBeginDay = intWeekIniday
                    If WeekBeginDay = 0 Then
                        WeekBeginDay = 1
                        intWeekIniday = 1
                    End If
                    FirstDay = GetStartDateForWeekAccruals(WeekBeginDay, TaskDate.Value)
                    If DateDiff("d", FirstDay, TaskDate.DateOnly) > 0 Then Lastday = DateAdd("d", -1, TaskDate.DateOnly)
                    If DateDiff("d", FirstDay, TaskDate.DateOnly) = 0 Then Lastday = FirstDay
                ElseIf sConceptType = "C" Then
                    ' Obtenemos fechas para saldos por contrato
                    FirstDay = Any2Time(myDS("BeginContract")).DateOnly
                    Lastday = Any2Time(myDS("EndContract")).DateOnly
                Else
                    ' Obtiene fechas para acumulados anuales

                    '* Obtener fechas de inicio de dia y mes
                    BeginMonth = intYearIniMonth
                    'Si no existía el parámetro roParYearPeriod, lo creo a 1 (enero)
                    If BeginMonth = 0 Then
                        intYearIniMonth = 1
                        BeginMonth = 1
                    End If
                    BeginDay = intMonthIniDay
                    'El año de inicio puede ser el actual o el anterior, en función del dia/mes
                    If BeginMonth < Month(TaskDate.Value) Or (BeginMonth = Month(TaskDate.Value) And BeginDay <= Day(TaskDate.Value)) Then
                        BeginYear = Year(TaskDate.Value)
                    Else
                        BeginYear = Year(TaskDate.Value) - 1
                    End If

                    FirstDay = DateSerial(BeginYear, BeginMonth, BeginDay)

                    Lastday = DateAdd("yyyy", 1, FirstDay)
                    Lastday = DateAdd("d", -1, Lastday)
                End If

                ' Si el periodo no es correcto, no validamos nada
                BeginDateRule = roConversions.Max(Any2Time(FirstDay), Any2Time(BeginDate))
                EndDateRule = roConversions.Min(Any2Time(Lastday), Any2Time(FinishDate))
                If BeginDateRule.VBNumericValue > EndDateRule.VBNumericValue Then
                    Exit Sub
                End If

                Dim sFirstDay As String = Any2Time(FirstDay).SQLSmallDateTime
                Dim sLastday As String = Any2Time(Lastday).SQLSmallDateTime

                'Crea la consulta
                strSQL = "@SELECT# CONVERT(NUMERIC(18,4),SUM(Value)) AS Total FROM DailyAccruals, Concepts "
                strSQL = strSQL & " WHERE DailyAccruals.IDConcept = Concepts.ID AND IDEmployee=" & zEmployee
                strSQL = strSQL & " AND IDConcept=" & zIDConcept
                strSQL = strSQL & " AND Date >=" & sFirstDay & " AND date <=" & sLastday

                ' Límitamos al periodo de contrato y al del saldo
                strSQL = strSQL & " AND Date >= " & Any2Time(myDS("BeginContract")).SQLSmallDateTime
                strSQL = strSQL & " AND Date <= " & Any2Time(myDS("EndContract")).SQLSmallDateTime
                strSQL = strSQL & " AND Date < " & Any2Time(Now.Date).SQLSmallDateTime
                strSQL = strSQL & " AND Date >= BeginDate "
                strSQL = strSQL & " AND Date <= FinishDate "

                Dim tbDailyAccruals As DataTable
                tbDailyAccruals = CreateDataTable(strSQL)

                If Not tbDailyAccruals Is Nothing AndAlso tbDailyAccruals.Rows.Count > 0 Then
                    Dim oAccrualRow As DataRow = tbDailyAccruals.Rows(0)

                    Dim oNotification As New Robotics.Base.VTNotifications.Notifications.roNotification
                    Dim lNotifications As List(Of VTNotifications.Notifications.roNotification)
                    lNotifications = VTNotifications.Notifications.roNotification.GetNotifications("IDType = 46 AND Activated=1", New VTNotifications.Notifications.roNotificationState(oState.IDPassport), , True)

                    If lNotifications IsNot Nothing AndAlso lNotifications.Count > 0 Then
                        oNotification = lNotifications.First
                    End If

                    IDNotification = oNotification.ID

                    sTotalConcept = Any2Double(oAccrualRow("Total"))
                    If sTotalConcept > sMaxValue Then
                        ' Ha sobrepasado el máximo, creamos tarea en caso necesario
                        strSQL = "@SELECT# COUNT(*) FROM sysroUserTasks WHERE ID LIKE "
                        strSQL = strSQL & "'" & (roBaseEngineManager.roUserTaskObject & ":\\ExceededMaxValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                        If Any2Long(ExecuteScalar(strSQL)) = 0 Then
                            strSQL = "@INSERT# INTO sysroUserTasks (ID, DateCreated, TaskType, Message, ResolverURL,  ResolverValue1, ResolverValue2, ResolverValue3, SecurityFlags ) VALUES ("
                            strSQL = strSQL & "'" & (roBaseEngineManager.roUserTaskObject & ":\\ExceededMaxValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                            strSQL = strSQL & "," & Any2Time(Now).SQLDateTime
                            strSQL = strSQL & "," & UserTask.TaskType.UserTaskReview
                            strSQL = strSQL & ",'" & Any2String(oState.Language.Translate("EngineResultEnum.ExceededMaxValue", "") & " " & ExecuteScalar("@SELECT# Name FROM Employees WHERE ID= " & zEmployee) & " " & oState.Language.Translate("EngineResultEnum.ExceededMaxValueContract", "") & " " & Any2String(myDS("IDContract")) & " " & oState.Language.Translate("EngineResultEnum.ExceededMaxValueConcept", "") & " " & oConcept.Name).Replace("'", "''") & "'"
                            strSQL = strSQL & ",'" & roBaseEngineManager.roFunctionCallObject & ":\\ExceededMaxValue" & "'"
                            strSQL = strSQL & ",'" & zEmployee & "'"
                            strSQL = strSQL & ",'" & zIDConcept & "'"
                            strSQL = strSQL & ",'" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"
                            strSQL = strSQL & ",'1111111111111111111111111111111111111111')"
                            ExecuteSql(strSQL)
                        End If

                        If IDNotification > 0 Then
                            strSQL = "@SELECT# COUNT(*)  " &
                                        " FROM sysroNotificationTasks " &
                                        " WHERE sysroNotificationTasks.Key1Numeric =" & zEmployee &
                                        " AND sysroNotificationTasks.Key2Numeric =" & zIDConcept &
                                        " AND IDNotification =" & IDNotification &
                                        " AND Parameters  like '" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"

                            If Any2Double(ExecuteScalar(strSQL)) = 0 Then
                                strSQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric, Parameters) VALUES (" &
                                  IDNotification & "," & zEmployee & "," & Any2Time(Now.Date).SQLDateTime & "," & zIDConcept & ",'" & Any2String(myDS("IDContract")).Replace("'", "''") & "')"
                                If Not ExecuteSql(strSQL) Then
                                    oState.Result = EngineResultEnum.ErrorInsertingEmployeeMaximumLimits
                                End If
                            End If
                        End If
                    Else
                        ' borramos la tarea
                        strSQL = "@DELETE# FROM sysroUserTasks WHERE ID LIKE "
                        strSQL = strSQL & "'" & Any2String(roBaseEngineManager.roUserTaskObject & ":\\ExceededMaxValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                        ExecuteSql(strSQL)

                        ' Borramos la notificacion
                        If IDNotification > 0 Then
                            strSQL = "@DELETE# FROM sysroNotificationTasks WHERE sysroNotificationTasks.Key1Numeric =" & zEmployee &
                        " AND sysroNotificationTasks.Key2Numeric =" & zIDConcept &
                        " AND IDNotification =" & IDNotification &
                        " AND Parameters  LIKE '" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"
                            ExecuteSql(strSQL)
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteEmployeeMaximiumLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteEmployeeMaximiumLimits")
            End Try

        End Sub

        Private Sub ExecuteEmployeeMinimumLimits(ByVal zEmployee As Long, ByVal zIDConcept As Integer, ByVal zMinValue As Double, ByVal myDS As DataRow, ByVal TaskDate As roTime)
            '
            ' Comprueba los limites Anuales o mensuales
            '

            Dim strSQL As String
            Dim sConceptType As String
            Dim sTotalConcept As Double
            Dim sMinValue As Double
            Dim FirstDay As Date
            Dim Lastday As Date
            Dim BeginMonth As Long
            Dim BeginDay As Long
            Dim WeekBeginDay As Integer
            Dim BeginYear As Long
            Dim BeginDateRule As roTime
            Dim EndDateRule As roTime
            Dim BeginDate As Date
            Dim FinishDate As Date

            Dim IDNotification As Double

            Try

                oState.Result = EngineResultEnum.NoError

                Dim oConcept As roConceptEngine = oConceptsCache.Item(zIDConcept)
                sConceptType = oConcept.DefaultQuery
                BeginDate = oConcept.BeginDate
                FinishDate = oConcept.FinishDate

                ' Valor Máximo
                sMinValue = Math.Round(zMinValue, 4)

                If sConceptType = "M" Then
                    ' Obtenemos fechas de los acumulados mensuales
                    CHECK_GetEmployeeAccruals_MonthQueryDates(FirstDay, Lastday, TaskDate)
                    Lastday = DateAdd("m", 1, FirstDay)
                    Lastday = DateAdd("d", -1, Lastday)
                ElseIf sConceptType = "W" Then
                    ' Obtenemos fechas para saldos semanales
                    WeekBeginDay = intWeekIniday
                    If WeekBeginDay = 0 Then
                        WeekBeginDay = 1
                        intWeekIniday = 1
                    End If
                    FirstDay = GetStartDateForWeekAccruals(WeekBeginDay, TaskDate.Value)
                    If DateDiff("d", FirstDay, TaskDate.DateOnly) > 0 Then Lastday = DateAdd("d", -1, TaskDate.DateOnly)
                    If DateDiff("d", FirstDay, TaskDate.DateOnly) = 0 Then Lastday = FirstDay
                ElseIf sConceptType = "C" Then
                    ' Obtenemos fechas para saldos por contrato
                    FirstDay = Any2Time(myDS("BeginContract")).DateOnly
                    Lastday = Any2Time(myDS("EndContract")).DateOnly
                Else
                    ' Obtiene fechas para acumulados anuales

                    '* Obtener fechas de inicio de dia y mes
                    BeginMonth = intYearIniMonth
                    'Si no existía el parámetro roParYearPeriod, lo creo a 1 (enero)
                    If BeginMonth = 0 Then
                        intYearIniMonth = 1
                        BeginMonth = 1
                    End If
                    BeginDay = intMonthIniDay
                    'El año de inicio puede ser el actual o el anterior, en función del dia/mes
                    If BeginMonth < Month(TaskDate.Value) Or (BeginMonth = Month(TaskDate.Value) And BeginDay <= Day(TaskDate.Value)) Then
                        BeginYear = Year(TaskDate.Value)
                    Else
                        BeginYear = Year(TaskDate.Value) - 1
                    End If

                    FirstDay = DateSerial(BeginYear, BeginMonth, BeginDay)

                    Lastday = DateAdd("yyyy", 1, FirstDay)
                    Lastday = DateAdd("d", -1, Lastday)
                End If

                ' Si el periodo no es correcto, no validamos nada
                BeginDateRule = roConversions.Max(Any2Time(FirstDay), Any2Time(BeginDate))
                EndDateRule = roConversions.Min(Any2Time(Lastday), Any2Time(FinishDate))
                If BeginDateRule.VBNumericValue > EndDateRule.VBNumericValue Then
                    Exit Sub
                End If

                Dim sFirstDay As String = Any2Time(FirstDay).SQLSmallDateTime
                Dim sLastday As String = Any2Time(Lastday).SQLSmallDateTime

                'Crea la consulta
                strSQL = "@SELECT# CONVERT(NUMERIC(18,4),SUM(Value)) AS Total FROM DailyAccruals, Concepts "
                strSQL = strSQL & " WHERE DailyAccruals.IDConcept = Concepts.ID AND IDEmployee=" & zEmployee
                strSQL = strSQL & " AND IDConcept=" & zIDConcept
                strSQL = strSQL & " AND Date >=" & sFirstDay & " AND date <=" & sLastday

                ' Límitamos al periodo de contrato y al del saldo
                strSQL = strSQL & " AND Date >= " & Any2Time(myDS("BeginContract")).SQLSmallDateTime
                strSQL = strSQL & " AND Date <= " & Any2Time(myDS("EndContract")).SQLSmallDateTime
                strSQL = strSQL & " AND Date < " & Any2Time(Now.Date).SQLSmallDateTime
                strSQL = strSQL & " AND Date >= BeginDate "
                strSQL = strSQL & " AND Date <= FinishDate "

                Dim tbDailyAccruals As DataTable
                tbDailyAccruals = CreateDataTable(strSQL)

                If Not tbDailyAccruals Is Nothing AndAlso tbDailyAccruals.Rows.Count > 0 Then
                    Dim oAccrualRow As DataRow = tbDailyAccruals.Rows(0)

                    Dim oNotification As New Robotics.Base.VTNotifications.Notifications.roNotification
                    Dim lNotifications As List(Of VTNotifications.Notifications.roNotification)
                    lNotifications = VTNotifications.Notifications.roNotification.GetNotifications("IDType = 47 AND Activated=1", New VTNotifications.Notifications.roNotificationState(oState.IDPassport), , True)

                    If lNotifications IsNot Nothing AndAlso lNotifications.Count > 0 Then
                        oNotification = lNotifications.First
                    End If

                    IDNotification = oNotification.ID

                    sTotalConcept = Any2Double(oAccrualRow("Total"))
                    If sTotalConcept <= sMinValue Then
                        ' No ha superado el mínimo, creamos tarea en caso necesario
                        strSQL = "@SELECT# COUNT(*) FROM sysroUserTasks WHERE ID LIKE "
                        strSQL = strSQL & "'" & (roBaseEngineManager.roUserTaskObject & ":\\ExceededMinValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                        If Any2Long(ExecuteScalar(strSQL)) = 0 Then
                            strSQL = "@INSERT# INTO sysroUserTasks (ID, DateCreated, TaskType, Message, ResolverURL,  ResolverValue1, ResolverValue2, ResolverValue3, SecurityFlags ) VALUES ("
                            strSQL = strSQL & "'" & (roBaseEngineManager.roUserTaskObject & ":\\ExceededMinValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                            strSQL = strSQL & "," & Any2Time(Now).SQLDateTime
                            strSQL = strSQL & "," & UserTask.TaskType.UserTaskReview
                            strSQL = strSQL & ",'" & Any2String(oState.Language.Translate("EngineResultEnum.ExceededMinValue", "") & " " & ExecuteScalar("@SELECT# Name FROM Employees WHERE ID= " & zEmployee) & " " & oState.Language.Translate("EngineResultEnum.ExceededMaxValueContract", "") & " " & Any2String(myDS("IDContract")) & " " & oState.Language.Translate("EngineResultEnum.ExceededMaxValueConcept", "") & " " & oConcept.Name).Replace("'", "''") & "'"
                            strSQL = strSQL & ",'" & roBaseEngineManager.roFunctionCallObject & ":\\ExceededMinValue" & "'"
                            strSQL = strSQL & ",'" & zEmployee & "'"
                            strSQL = strSQL & ",'" & zIDConcept & "'"
                            strSQL = strSQL & ",'" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"
                            strSQL = strSQL & ",'1111111111111111111111111111111111111111')"
                            ExecuteSql(strSQL)
                        End If

                        If IDNotification > 0 Then
                            strSQL = "@SELECT# COUNT(*)  " &
                                        " FROM sysroNotificationTasks " &
                                        " WHERE sysroNotificationTasks.Key1Numeric =" & zEmployee &
                                        " AND sysroNotificationTasks.Key2Numeric =" & zIDConcept &
                                        " AND IDNotification =" & IDNotification &
                                        " AND Parameters  like '" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"

                            If Any2Double(ExecuteScalar(strSQL)) = 0 Then
                                strSQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Key2Numeric, Parameters) VALUES (" &
                                  IDNotification & "," & zEmployee & "," & Any2Time(Now.Date).SQLDateTime & "," & zIDConcept & ",'" & Any2String(myDS("IDContract")).Replace("'", "''") & "')"
                                If Not ExecuteSql(strSQL) Then
                                    oState.Result = EngineResultEnum.ErrorInsertingEmployeeMaximumLimits
                                End If
                            End If
                        End If
                    Else
                        ' borramos la tarea
                        strSQL = "@DELETE# FROM sysroUserTasks WHERE ID LIKE "
                        strSQL = strSQL & "'" & Any2String(roBaseEngineManager.roUserTaskObject & ":\\ExceededMinValue" & zEmployee & "@" & zIDConcept & "@" & Any2String(myDS("IDContract"))).Replace("'", "''") & "'"
                        ExecuteSql(strSQL)

                        ' Borramos la notificacion
                        If IDNotification > 0 Then
                            strSQL = "@DELETE# FROM sysroNotificationTasks WHERE sysroNotificationTasks.Key1Numeric =" & zEmployee &
                        " AND sysroNotificationTasks.Key2Numeric =" & zIDConcept &
                        " AND IDNotification =" & IDNotification &
                        " AND Parameters  LIKE '" & Any2String(myDS("IDContract")).Replace("'", "''") & "'"
                            ExecuteSql(strSQL)
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteEmployeeMinimumLimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteEmployeeMinimumLimits")
            End Try

        End Sub

        Private Sub CHECK_GetEmployeeAccruals_MonthQueryDates(ByRef StartDate As Date, ByRef EndDate As Date, Optional ByVal TaskDate As roTime = Nothing)
            '
            ' Obtiene fechas para crear los acumulados mensuales
            '
            Dim mFirstMonthDay As Double

            Dim mDate As Date

            Try
                mDate = Now.Date

                If Not TaskDate Is Nothing Then
                    mDate = TaskDate.Value
                End If

                'Obtenemos el prime dia del mes que hay que consultar los acumulados
                mFirstMonthDay = intMonthIniDay
                If mFirstMonthDay = 0 Then mFirstMonthDay = 1

                ' Obtiene mes a acumular (para acumulados mensuales)
                If Day(mDate) > mFirstMonthDay Then
                    'Si el dia es posterior al inicio del periodo (mismo mes)
                    StartDate = Any2Time(DateSerial(Year(mDate), Month(mDate), mFirstMonthDay)).DateOnly
                ElseIf Day(mDate) < mFirstMonthDay Then
                    'Si el dia es anterior al inicio del periodo (mes anterior)
                    StartDate = Any2Time(DateSerial(Year(DateAdd("m", -1, mDate)), Month(DateAdd("m", -1, mDate)), mFirstMonthDay)).DateOnly
                ElseIf Day(mDate) = mFirstMonthDay Then
                    'Si es el mismo dia
                    StartDate = mDate
                End If

                If DateDiff("d", StartDate, mDate) > 0 Then EndDate = DateAdd("d", -1, mDate)
                If DateDiff("d", StartDate, mDate) = 0 Then EndDate = StartDate
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CHECK_GetEmployeeAccruals_MonthQueryDates")
            End Try

        End Sub

        Private Sub Execute_GratificationRule(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine)
            '
            ' Reglas avanzadas: Gratificacion
            '
            Dim strSQL As String
            Dim IDShift As Double
            Dim AdvancedParameters As String
            Dim Pos As Integer
            Dim strGra As String
            Dim IDCauseGRA As Integer

            Try

                oState.Result = EngineResultEnum.NoError

                ' 0. Eliminamos los pluses anteriores
                strSQL = "@SELECT# ID FROM Causes WHERE Description LIKE '%#GRA#%'"
                IDCauseGRA = Any2Integer(ExecuteScalar(strSQL))
                If IDCauseGRA <= 0 Then IDCauseGRA = -1
                ExecuteSql("@DELETE# FROM DailyCauses WHERE IDCause = " & IDCauseGRA.ToString & " AND  AccrualsRules = 0 And IDRelatedIncidence = 0 And (Manual = 0 Or Manual Is Null) And IDEmployee = " & zEmployee & " And Date = " & zDate.SQLSmallDateTime)

                ' 1. Obtenemos el horario utilizado ese dia
                If zShift Is Nothing Then Exit Sub
                If zShift.ID <= 0 Then Exit Sub

                IDShift = zShift.ID
                If IDShift <= 0 Then Exit Sub

                ' 2. Obtenemos el campo de opciones avanzadas del horario utilizado.
                AdvancedParameters = zShift.AdvancedParameters.Trim
                If Len(AdvancedParameters) = 0 Then Exit Sub

                ' 3. Miramos si el horario esta marcado para gratificacion
                Pos = InStr(1, AdvancedParameters, "#GRA=")
                If Pos = 0 Then Exit Sub

                ' Obtenemos el tiempo de gratificación
                strGra = Mid$(AdvancedParameters, Pos + 5, 5)
                If IsDate(strGra) Then
                    ' Aplicamos el tiempo de gratificación en caso necesario
                    Execute_ApplyGratification(zEmployee, zDate, strGra, IDShift)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_GratificationRule")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_GratificationRule")
            End Try

        End Sub

        Private Sub Execute_ApplyGratification(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal strPlus As String, ByVal IDShift As Double)
            '
            ' Aplicamos plus de gratificacion en caso necesario
            '
            Dim strSQL As String
            Dim IDCause As Double
            Dim ExpectedWorkingHours As Double
            Dim IDConcept As Double
            Dim CauseValue As Double
            Dim GRAPlus As Double
            Dim TotalTime As Double

            Try

                ' 0. Obtenemos las horas teóricas del horario
                strSQL = "@SELECT# ExpectedWorkingHours FROM Shifts WHERE ID=" & IDShift
                ExpectedWorkingHours = Any2Double(ExecuteScalar(strSQL))
                If ExpectedWorkingHours <= 0 Then Exit Sub

                ' 1. Obtenemos el saldo de horas trabajadas totales
                strSQL = "@SELECT# ID FROM Concepts WHERE Description like '%#HTR#%'"
                IDConcept = Any2Double(ExecuteScalar(strSQL))
                If IDConcept <= 0 Then Exit Sub

                ' 2. Obtenemos la justificacion de plus de gratificacion
                strSQL = "@SELECT# ID FROM Causes WHERE Description like '%#GRA#%'"
                IDCause = Any2Double(ExecuteScalar(strSQL))
                If IDCause <= 0 Then Exit Sub

                ' 3. Obtenemos la suma total de justificaciones que contienen el saldo de horas trabajadas totales
                strSQL = "@SELECT# SUM(convert(numeric(19,6), value)) as Total FROM DailyCauses with (nolock) WHERE IDEmployee=" & zEmployee.ToString & " AND Date=" & zDate.SQLSmallDateTime & " AND IDRelatedIncidence > 0 AND AccrualsRules = 0 AND IDCause IN(@SELECT# IDCause FROM ConceptCauses WHERE IDConcept = " & IDConcept.ToString & ")"
                TotalTime = Any2Double(ExecuteScalar(strSQL))

                ' 4. Tiempo de gratificacion
                GRAPlus = Any2Time(strPlus).NumericValue
                If GRAPlus <= 0 Then Exit Sub

                ' 5. Aplicamos las reglas de gratificacion
                If TotalTime = 0 Then
                    ' Si no tiene horas trabajadas no añadimos gratificacion
                    CauseValue = 0
                ElseIf TotalTime <= ExpectedWorkingHours Then
                    ' Si no supera las horas teoricas, añadimos el tiempo de gratificacion indicado en el horario
                    CauseValue = GRAPlus
                ElseIf TotalTime > ExpectedWorkingHours And TotalTime < (ExpectedWorkingHours + GRAPlus) Then
                    ' Si es mayor que las horas teoricas pero menor que teoricas + tiempo gratificacion, añadimos tiempo hasta llegar a teoricas + tiempo gratificacion
                    CauseValue = (ExpectedWorkingHours + GRAPlus) - TotalTime
                ElseIf TotalTime >= (ExpectedWorkingHours + GRAPlus) And TotalTime <= (ExpectedWorkingHours + GRAPlus + GRAPlus) Then
                    ' Si esta entre horas teoricas + gratificacion y horas teoricas + gratificacion * 2, no gratificamos nada
                    CauseValue = 0
                ElseIf TotalTime > (ExpectedWorkingHours + GRAPlus + GRAPlus) Then
                    ' Si es mayor que horas teoricas + gratificacion * 2,añadimos el tiempo de gratificacion indicado en el horario
                    CauseValue = GRAPlus
                Else
                    CauseValue = 0
                End If

                If CauseValue > 0 Then
                    ' Generamos el plus de gratificacion
                    CreatePlusCause(zEmployee, zDate, CauseValue, IDCause)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_ApplyGratification")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_ApplyGratification")

            End Try

        End Sub

        Private Sub Execute_RigidBreakRule(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine)
            '
            ' Reglas avanzadas: Descanso rigido obligado si hace mas de X horas trabajadas
            '
            Dim strSQL As String
            Dim IDShift As Double
            Dim Pos As Integer
            Dim strHTR As String
            Dim strDES As String
            Dim TotalTime As Double
            Dim TotalHTR As Double
            Dim TotalDesc As Double
            Dim DifWorked As Double
            Dim CauseValue As Double

            Try

                oState.Result = EngineResultEnum.NoError

                ' 0. Eliminamos los valores anteriores
                ExecuteSql("@DELETE# FROM DailyCauses WHERE IDCause =" & RigidBreakRule_IDCauseDES & " AND  AccrualsRules = 0 AND IDRelatedIncidence = 0 AND (Manual = 0 OR Manual IS NULL) AND IDEmployee = " & zEmployee.ToString & " AND Date = " & zDate.SQLSmallDateTime)

                ' 1. Obtenemos el horario utilizado ese dia
                If zShift Is Nothing Then Exit Sub
                If zShift.ID <= 0 Then Exit Sub
                IDShift = zShift.ID
                If IDShift <= 0 Then Exit Sub

                ' 2. Obtenemos el campo de opciones avanzadas del horario utilizado.
                If zShift.AdvancedParameters.Trim.Length = 0 Then Exit Sub

                ' 3. Miramos si el horario tiene indicado el valor de horas trabajadas a realizar para descanso obligado
                Pos = InStr(1, zShift.AdvancedParameters, "#HTRDESC=")
                If Pos = 0 Then Exit Sub

                ' Obtenemos el tiempo de horas trabajadas a realizar para el descanso obligado
                strHTR = Mid$(zShift.AdvancedParameters, Pos + 9, 5)
                If Not IsDate(strHTR) Then
                    Exit Sub
                End If

                ' Obtenemos el tiempo de descanso obligatorio
                Pos = InStr(1, zShift.AdvancedParameters, "#DESC=")
                If Pos = 0 Then Exit Sub

                strDES = Mid$(zShift.AdvancedParameters, Pos + 6, 5)
                If Not IsDate(strDES) Then
                    Exit Sub
                End If

                ' Obtenemos las horas trabajadas reales del dia
                If RigidBreakRule_IDConcept <= 0 Then Exit Sub

                ' Obtenemos la justificacion de descanso corto
                If RigidBreakRule_IDCauseDES <= 0 Then Exit Sub

                ' Obtenemos la suma total de justificaciones que contienen el saldo de horas trabajadas reales
                strSQL = "@SELECT# SUM(CONVERT(numeric(19,6), value)) AS Total FROM DailyCauses WHERE IDEmployee=" & zEmployee & " AND Date=" & zDate.SQLSmallDateTime & " AND IDRelatedIncidence > 0 AND AccrualsRules = 0 AND IDCause IN(@SELECT# IDCause FROM ConceptCauses WHERE IDConcept = " & RigidBreakRule_IDConcept & ")"
                TotalTime = Any2Double(ExecuteScalar(strSQL))

                TotalHTR = Any2Time(strHTR).NumericValue
                If TotalHTR <= 0 Then Exit Sub

                ' Si no se llega a las horas obligadas no hacemos nada
                If TotalTime < TotalHTR Then Exit Sub

                ' Obtenemos la diferencia entre lo trabajado y lo obligado
                DifWorked = TotalTime - TotalHTR

                ' Si no hay exceso no hacemos nada
                If DifWorked <= 0 Then
                    Exit Sub
                End If

                ' Obtenemos el total de horas de descanso
                TotalDesc = GetTotalDescOutRigid(zEmployee, zDate, IDShift)

                ' Si no hemos llegado al descanso obligado, generamos la diferencia
                If Any2Time(TotalDesc).VBNumericValue < Any2Time(strDES).VBNumericValue Then
                    CauseValue = Any2Time(strDES).NumericValue - Any2Time(TotalDesc).NumericValue

                    ' Como maximo generamos hasta el total de horas de exceso
                    CauseValue = roConversions.Min(Any2Time(CauseValue).NumericValue, Any2Time(DifWorked).NumericValue)

                    CreatePlusCause(zEmployee, zDate, CauseValue, RigidBreakRule_IDCauseDES)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_RigidBreakRule")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: Execute_RigidBreakRule")
            End Try

        End Sub

        Private Sub CreatePlusCause(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal pCauses As Double)
            '
            '   Crea Justificación resultante de la aplicación de la regla
            '

            Dim strSQL As String = String.Empty

            Try

                ' Seleccionamos las justificaciones del mismo tipo para ese empleado y ese día
                strSQL = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND AccruedRule= 0 AND DailyRule= 0 AND AccrualsRules=0 AND IDRelatedIncidence = 0 AND IDCause = " &
                         pCauses.ToString & " AND Date=" & TaskDay.SQLSmallDateTime

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                Dim da As DbDataAdapter
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                Dim oRow As DataRow
                ' Registramos la nueva justificación
                If zCauses.Rows.Count = 0 Then
                    oRow = zCauses.NewRow
                    oRow("IDEmployee") = IDEmployee
                    oRow("Date") = TaskDay.Value
                    oRow("IDCause") = pCauses
                    oRow("Value") = pValue
                    oRow("AccrualsRules") = 0
                    oRow("DailyRule") = 0
                    oRow("AccruedRule") = 0
                    oRow("IDRelatedIncidence") = 0
                    oRow("Manual") = 0
                    zCauses.Rows.Add(oRow)
                Else
                    ' Ya existía una justificación de este tipo para este empleado y día, luego acumulo el valor al ya existente
                    oRow = zCauses.Rows(0)
                    oRow("Value") = oRow("Value") + pValue
                End If

                da.Update(zCauses)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreatePlusCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreatePlusCause")
            End Try

        End Sub

        Private Function GetTotalDescOutRigid(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zIDShift As Double) As Double
            '
            ' Obtenemos los descanos que ha realizado el empleado antes o despues de su horario rígido
            ' y  Salida anticipada justificadas por Formacion y conciliacion
            Dim Total As Double
            Dim zIncidences As roTimeBlockList
            Dim Incidence As roTimeBlockItem
            Dim zShift As roShiftEngine
            Dim Layer As New roTimeBlockItem
            Dim FirstLayer As New roTimeBlockItem
            Dim LastLayer As New roTimeBlockItem
            Dim bolMandatory As Boolean
            Dim sSQL As String
            Dim zForeCast As roTimeBlockList = Nothing
            Dim sqlFilterCause As String
            Dim dRet As Double = 0

            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                FirstLayer.Period.Begin = Any2Time("2079/01/01 00:00")
                LastLayer.Period.Begin = Any2Time("1999/01/01 00:00")

                Dim zTimes As New roTimeBlockList
                Dim zMoves As New roMoveList

                zShift = roBaseEngineManager.GetShiftFromCache(zIDShift, oState)

                If zShift Is Nothing Then Return 0

                ' Obtiene movimientos
                zMoves = roBaseEngineManager.Execute_GetMovesLive(zEmployee, zDate, oState)

                ' Completa movimientos (si son incompletos, intenta rellenar para poder
                '  calcular algo de todas formas)
                Dim bolret As Boolean = roBaseEngineManager.Execute_ProcessMoves(zMoves, zDate, oState)
                If Not bolret Then
                    Return 0
                    Exit Function
                End If

                ' Inicializamos bloques de tiempo
                zTimes = roBaseEngineManager.InitializeBlocks(zDate, zMoves, oState)

                ' Obtiene previsiones en caso necesario
                If mVerifyProgrammedCauseOnMandatory = "1" Then
                    zForeCast = roBaseEngineManager.Execute_GetForecast(zEmployee, zDate, zShift, zMoves, oState)
                End If

                bolMandatory = False


                For Each oLayer As roShiftEngineLayer In zShift.Layers
                    Select Case oLayer.LayerType
                        Case roLayerTypes.roLTMandatory
                            bolMandatory = True
                            ' Obtenemos el periodo de obligadas
                            Layer = New roTimeBlockItem

                            Layer.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, oLayer.Data("Begin")))
                            Layer.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, oLayer.Data("Finish")))

                            ' Mira si hay entrada flotante
                            If oLayer.Data.Exists("FloatingBeginUpto") Then
                                ' Hay entrada flotante, mira cuando ha entrado
                                Layer.Period.Begin = roBaseEngineManager.ProcessLayer_Mandatory_GetFloatingBegin(Layer.Period.Begin, Any2Time(DateTimeAdd(zDate.Value, oLayer.Data("FloatingBeginUpto"))), zTimes, zForeCast, zShift, oState)
                            End If

                            If Layer.Period.Begin IsNot Nothing AndAlso oLayer.Data.Exists("FloatingFinishMinutes") Then
                                ' Si tambien hay salida flotante, establece en funcion de la entrada
                                Layer.Period.Finish = Layer.Period.Begin.Add(oLayer.Data("FloatingFinishMinutes"), "n")
                            End If



                            If Layer.Period.Begin IsNot Nothing Then
                                ' Nos guardamos el primer periodo
                                If FirstLayer.Period.Begin.VBNumericValue > Layer.Period.Begin.VBNumericValue Then
                                    FirstLayer.Period.Begin = Layer.Period.Begin
                                    FirstLayer.Period.Finish = Layer.Period.Finish
                                End If

                                ' Nos guardamos el último periodo
                                If LastLayer.Period.Begin.VBNumericValue < Layer.Period.Begin.VBNumericValue Then
                                    LastLayer.Period.Begin = Layer.Period.Begin
                                    LastLayer.Period.Finish = Layer.Period.Finish
                                End If
                            End If
                    End Select
                Next

                If Not bolMandatory Then
                    Return dRet
                End If

                ' Se deben obtener todas las salidas de presencia anteriores a la primera franja rigida
                ' y buscar el siguiente fichaje de entrada
                ' el descanso sera el Periodo entre la salida y MIN(siguiente entrada,inicio de franja)
                Total = 0

                sqlFilterCause = ""
                If dblProductiveAbsenceCause > 0 And bolProductiveAbsences Then
                    sqlFilterCause = " AND (TypeData is null or  TypeData <> " & dblProductiveAbsenceCause & ") "
                End If

                ' Obtenemnos todas las salidas de presencia anteriores a la primera franja rigida
                Dim tbPunches As DataTable
                strSQL = "@SELECT# Datetime FROM Punches with (nolock) WHERE IDEmployee=" & zEmployee & " AND ShiftDate=" & zDate.SQLSmallDateTime & " AND ActualType = 2 AND DateTime < " & FirstLayer.Period.Begin.SQLDateTime & sqlFilterCause & " ORDER BY Datetime ASC "
                tbPunches = CreateDataTable(strSQL)

                If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                    zIncidences = New roTimeBlockList
                    For Each oPunchRow As DataRow In tbPunches.Rows
                        Incidence = New roTimeBlockItem
                        ' Guardamos la hora de salida del fichaje
                        Incidence.Period.Begin = Any2Time(Format$(oPunchRow("Datetime"), "yyyy/MM/dd HH:mm"))

                        ' Obtenemos la siguiene entrada
                        Dim strNextDateTime As String
                        strNextDateTime = Any2String(ExecuteScalar("@SELECT# TOP 1 Datetime FROM Punches WHERE IDEmployee=" & zEmployee & " AND ShiftDate=" & zDate.SQLSmallDateTime & " AND ActualType = 1 AND DateTime > " & Incidence.Period.Begin.SQLDateTime & "   ORDER BY Datetime ASC, ID ASC "))
                        If Not IsDate(strNextDateTime) Then
                            strNextDateTime = "2079/01/01 00:00"
                        End If

                        ' Asignamos como final del periodo el minimo entre siguiente entrada y inicio franja rígida
                        Incidence.Period.Finish = roConversions.Min(Any2Time(strNextDateTime), FirstLayer.Period.Begin)
                        Try
                            Incidence.TimeValue = Any2Time(Incidence.Period.Finish.NumericValue - Incidence.Period.Begin.NumericValue)
                            If Incidence.TimeValue.NumericValue > 0 Then

                                Total = Total + Any2Time(Incidence.Period.Finish.NumericValue - Incidence.Period.Begin.NumericValue).NumericValue
                            End If
                        Catch ex As Exception

                        End Try
                    Next
                End If

                ' Se deben obtener todas las entradas de presencia posteriores al final de la última franja rigida
                ' y buscar la anterior salida
                ' el descanso sera el Periodo entre la MAX(anterior salida,fin de franja) y Entrada
                Dim tbPunchesAux As DataTable
                strSQL = "@SELECT# Datetime FROM Punches with (nolock) WHERE IDEmployee=" & zEmployee & " AND ShiftDate=" & zDate.SQLSmallDateTime & " AND ActualType = 1 AND DateTime > " & LastLayer.Period.Finish.SQLDateTime & " ORDER BY Datetime ASC "
                tbPunchesAux = CreateDataTable(strSQL)
                If tbPunchesAux IsNot Nothing AndAlso tbPunchesAux.Rows.Count > 0 Then
                    zIncidences = New roTimeBlockList
                    For Each oPunchRow As DataRow In tbPunchesAux.Rows
                        Incidence = New roTimeBlockItem
                        ' Guardamos la hora de entrada del fichaje como final del periodo
                        Incidence.Period.Finish = Any2Time(Format$(oPunchRow("Datetime"), "yyyy/MM/dd HH:mm"))

                        Dim strPreviousDateTime As String = String.Empty
                        If dblProductiveAbsenceCause > 0 And bolProductiveAbsences Then
                            ' Si la anterior salida es de Salida de trabajo la descartamos
                            If dblProductiveAbsenceCause = Any2Double(ExecuteScalar("@SELECT# TOP 1 ISNULL(TypeData,0) FROM Punches with (nolock) WHERE IDEmployee=" & zEmployee & " AND ShiftDate=" & zDate.SQLSmallDateTime & " AND ActualType = 2 AND DateTime < " & Incidence.Period.Finish.SQLDateTime & " ORDER BY Datetime desc, ID desc ")) Then
                                strPreviousDateTime = "-1"
                            End If
                        End If

                        ' Obtenemos la anterior salida siempre y cuando no sea salida de trabajo
                        If strPreviousDateTime <> "-1" Then
                            strPreviousDateTime = Any2String(ExecuteScalar("@SELECT# TOP 1 Datetime FROM Punches with (nolock) WHERE IDEmployee=" & zEmployee & " AND ShiftDate=" & zDate.SQLSmallDateTime & " AND ActualType = 2 AND DateTime < " & Incidence.Period.Finish.SQLDateTime & " ORDER BY Datetime DESC, ID DESC "))
                            If Not IsDate(strPreviousDateTime) Then
                                strPreviousDateTime = "2000/01/01 00:00"
                            End If

                            ' Asignamos como inicio del periodo el maximo entre anterior salida y final de la franja rígida
                            Incidence.Period.Begin = roConversions.Max(Any2Time(strPreviousDateTime), LastLayer.Period.Finish)

                            Try
                                Incidence.TimeValue = Any2Time(Incidence.Period.Finish.NumericValue - Incidence.Period.Begin.NumericValue)
                                If Incidence.TimeValue.NumericValue > 0 Then
                                    Total = Total + Any2Time(Incidence.Period.Finish.NumericValue - Incidence.Period.Begin.NumericValue).NumericValue
                                End If

                            Catch ex As Exception

                            End Try

                        End If
                    Next
                End If

                ' Obtenemos las horas de salida anticipada justificadas por Formacion y conciliacion
                ' este tiempo tambien se debe contemplar como descanso
                sSQL = "@SELECT# IsNull(SUM(DailyCauses.Value), 0) AS Total  FROM DailyCauses with (nolock) WHERE IDEmployee = " & zEmployee & " AND Date = " & zDate.SQLSmallDateTime & "  AND IDCause IN(@SELECT# ID FROM Causes WHERE ShortName = 'FIC') AND IDRelatedIncidence IN("
                sSQL = sSQL & " @SELECT# ID FROM DailyIncidences with (nolock) WHERE IDEmployee = " & zEmployee & "  AND Date  = " & zDate.SQLSmallDateTime & " AND IDType = 1022)"
                Total = Total + Any2Double(ExecuteScalar(sSQL))

                ' Devuelvo el total de horas de descanso
                dRet = Total
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetTotalDescOutRigid")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetTotalDescOutRigid")
            End Try

            Return dRet
        End Function

        Private Function GetValueCondition(ByVal mRulePart As RulePart, ByVal mRuleCondition As RuleCondition, ByVal zCauses As DataTable, ByVal IDEmployee As Long, ByVal TaskDate As Date, ByVal rule As roShiftDailyRule, ByRef mValue As Double) As Boolean
            '
            ' Obtenemnos el valor de la condicion y parte indicadas
            '
            Dim bolret As Boolean = False

            Try

                mValue = 0

                Select Case mRulePart
                    Case RulePart.Part1
                        ' Si es la primera parte,
                        ' obtenemos las justificaciones/zonas
                        mValue = GetConditionValue(rule.Conditions.Item(mRuleCondition).ConditionCauses, rule.Conditions.Item(mRuleCondition).ConditionTimeZones, zCauses, IDEmployee, TaskDate)
                        If mValue >= 0 Then
                            mValue = Any2Time(Date.FromOADate(mValue), True).NumericValue(True)
                        Else
                            mValue = Any2Time(Date.FromOADate(mValue * -1), True).NumericValue(True) * -1
                        End If
                    Case RulePart.Part2
                        ' Si es la segunda parte,hay que obtener de que tipo es la condicion(valor directo, campo ficha, justificacion)
                        Select Case rule.Conditions(mRuleCondition).Type
                            Case DailyConditionValueType.DirectValue ' Valor directo
                                If InStr(rule.Conditions(mRuleCondition).FromValue, "-") = 0 Then
                                    mValue = Any2Time(rule.Conditions(mRuleCondition).FromValue, True).NumericValue(True)
                                Else
                                    mValue = Any2Time(Mid$(rule.Conditions(mRuleCondition).FromValue, 2), True).NumericValue(True) * -1
                                End If
                            Case DailyConditionValueType.UserField ' Campo de la ficha
                                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(IDEmployee, rule.Conditions(mRuleCondition).UserField, TaskDate, New UserFields.roUserFieldState, False)
                                mValue = Any2Time(Any2Double(oUserField.FieldRawValue), True).NumericValue(True)
                            Case DailyConditionValueType.ID  ' Con la justificacion/Zonas
                                mValue = GetConditionValue(rule.Conditions.Item(mRuleCondition).CompareCauses, rule.Conditions.Item(mRuleCondition).CompareTimeZones, zCauses, IDEmployee, TaskDate)
                                If mValue >= 0 Then
                                    mValue = Any2Time(Date.FromOADate(mValue), True).NumericValue(True)
                                Else
                                    mValue = Any2Time(Date.FromOADate(mValue * -1), True).NumericValue(True) * -1
                                End If
                        End Select
                End Select

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetValueCondition")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetValueCondition")

            End Try

            Return bolret

        End Function

        Private Function CreateDailyPlusCause(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal pCauses As Double, zManualCenters As DataTable) As Boolean
            '
            '   Crea plus de la regla diaria
            '

            Dim bolret As Boolean = False

            Dim bExistsManual As Boolean = False
            Dim dDefaultCenter As Integer

            Try

                Dim strSQL As String = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & IDEmployee &
                " AND ISNULL(DailyRule,0)= 1 AND ISNULL(AccruedRule,0) = 0 AND ISNULL(AccrualsRules,0)=0 AND ISNULL(IDRelatedIncidence,0) = 0 AND IDCause = " & pCauses.ToString & " AND Date=" & TaskDay.SQLSmallDateTime

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                Dim da As DbDataAdapter
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                Dim oRow As DataRow

                If zCauses.Rows.Count = 0 Then
                    oRow = zCauses.NewRow
                    oRow("IDEmployee") = IDEmployee
                    oRow("Date") = TaskDay.Value
                    oRow("IDCause") = pCauses
                    oRow("Value") = pValue
                    oRow("AccrualsRules") = 0
                    oRow("IDRelatedIncidence") = 0
                    oRow("DailyRule") = 1
                    oRow("AccruedRule") = 0
                    oRow("Manual") = 0
                    ' Guarda datos en la base de datos
                    zCauses.Rows.Add(oRow)
                Else
                    oRow = zCauses.Rows(0)
                    oRow("Value") = oRow("Value") + pValue
                End If

                ' Si tiene centro de coste lo asignamods a su centro de coste por defecto
                If mCostCenterInstallled Then
                    bExistsManual = False

                    ' En el caso que previamente existiera
                    ' la misma justificacion con un centro manual,
                    ' lo asignamos a ese centro
                    For Each oRowDC As DataRow In zManualCenters.Rows
                        If pCauses = Any2Double(oRowDC("IDCause")) AndAlso Any2Time(pValue).VBNumericValue = Any2Time(Any2Double(oRowDC("Value"))).VBNumericValue Then
                            oRow("IDCenter") = oRowDC("IDCenter")
                            oRow("DefaultCenter") = IIf(oRowDC("DefaultCenter"), 1, 0)
                            oRow("ManualCenter") = 1
                            bExistsManual = True
                            Exit For
                        End If
                    Next

                    If Not bExistsManual Then
                        ' En cualquier otro caso asignamos el centro de coste por defecto
                        ' del empleado
                        dDefaultCenter = roBaseEngineManager.GetDefaultCenter(IDEmployee, TaskDay, oState)
                        oRow("IDCenter") = dDefaultCenter
                        oRow("DefaultCenter") = 1
                        oRow("ManualCenter") = 0
                    End If

                End If

                da.Update(zCauses)
                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyPlusCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyPlusCause")
            End Try

            Return bolret

        End Function

        Private Function GetDailyCauseValue(ByVal intIDEmployee As Long, ByVal xDate As Date, ByVal IDCause As Double) As Double
            '
            ' Obtiene el valor total actual de la justificación indicada para empleado y dia concreto (no tenemos en cuenta las justificaciones generadas por reglas de convenio)
            '
            Dim dRet As Double
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                If CONCEPT_SHIFTEXPECTEDHOURS = IDCause Then
                    If Not bolProgrammedAbsenceOnHolidays Then
                        strSQL = "@SELECT# (CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 1 THEN 0 ELSE ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) END) " &
                                 " FROM DailySchedule with (nolock) , Shifts with (nolock)   WHERE IDEmployee=" & intIDEmployee & " AND DATE=" & Any2Time(xDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftUsed  "
                        dRet = Any2Double(ExecuteScalar(strSQL))
                    Else
                        strSQL = "@SELECT# ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) FROM DailySchedule with (nolock) , Shifts with (nolock)  WHERE IDEmployee=" & intIDEmployee & " AND DATE=" & Any2Time(xDate).SQLSmallDateTime & " AND Shifts.ID = DailySchedule.IDShiftBase "
                        dRet = Any2Double(ExecuteScalar(strSQL))
                    End If
                Else
                    strSQL = "@SELECT# ISNULL(SUM(DailyCauses.Value),0) FROM DailyCauses with (nolock)  WHERE DailyCauses.IDEmployee = " & intIDEmployee & " AND " &
                             " DailyCauses.Date = " & Any2Time(xDate).SQLSmallDateTime & " AND IDCause=" & IDCause & " AND AccrualsRules=0  "
                    dRet = Any2Double(ExecuteScalar(strSQL))
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetDailyCauseValue")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetDailyCauseValue")
            End Try

            Return dRet

        End Function

        Private Function GetAutomaticEquivalenceCauses() As DataTable
            '
            ' Obtiene las justificaciones que tienen que generarse a partir de otra
            '
            Dim dtCauses As DataTable = Nothing
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                strSQL = "@SELECT# ID, AutomaticEquivalenceType, AutomaticEquivalenceCriteria, AutomaticEquivalenceIDCause FROM Causes " &
                         "WHERE isnull(AutomaticEquivalenceType,0) > 0 AND AutomaticEquivalenceIDCause > 0 "

                dtCauses = CreateDataTable(strSQL)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetAutomaticEquivalenceCauses")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetAutomaticEquivalenceCauses")
            End Try

            Return dtCauses
        End Function

        Private Sub CreateDailyEquivalenceCause(ByVal IDEmployee As Long, ByVal TaskDay As roTime, ByVal pValue As Double, ByVal idCause As Integer, ByVal zManualCenters As DataTable)
            '
            '   Crea valor de equivalencia
            '

            Dim bExistsManual As Boolean
            Dim dDefaultCenter As Integer
            Dim strSQL As String

            Try

                oState.Result = EngineResultEnum.NoError

                ' Seleccionamos las justificaciones del mismo tipo para ese empleado y ese día
                strSQL = "@SELECT# * FROM DailyCauses WHERE IDEmployee=" & IDEmployee & " AND DailyRule= 0 AND AccruedRule = 0 AND AccrualsRules=0 AND IDRelatedIncidence = 0 AND IDCause = " & idCause.ToString & " AND Date=" & TaskDay.SQLSmallDateTime

                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim zCauses As New DataTable("DailyCauses")
                Dim da As DbDataAdapter
                da = CreateDataAdapter(cmd, True)
                da.Fill(zCauses)

                Dim oRow As DataRow

                ' Registramos la nueva justificación
                If zCauses.Rows.Count = 0 Then
                    oRow = zCauses.NewRow
                    oRow("IDEmployee") = IDEmployee
                    oRow("Date") = TaskDay.Value
                    oRow("IDCause") = idCause.ToString
                    oRow("Value") = pValue
                    oRow("AccrualsRules") = 0
                    oRow("IDRelatedIncidence") = 0
                    oRow("DailyRule") = 0
                    oRow("AccruedRule") = 0
                    oRow("Manual") = 0
                    ' Guarda datos en la base de datos
                    zCauses.Rows.Add(oRow)
                Else
                    oRow = zCauses.Rows(0)
                    oRow("Value") = oRow("Value") + pValue
                End If

                ' Si tiene centro de coste lo asignamods a su centro de coste por defecto
                If mCostCenterInstallled Then
                    bExistsManual = False

                    ' En el caso que previamente existiera
                    ' la misma justificacion con un centro manual,
                    ' lo asignamos a ese centro
                    For Each oRowDC As DataRow In zManualCenters.Rows
                        If idCause = Any2Double(oRowDC("IDCause")) AndAlso Any2Time(pValue).VBNumericValue = Any2Time(Any2Double(oRowDC("Value"))).VBNumericValue Then
                            oRow("IDCenter") = oRowDC("IDCenter")
                            oRow("DefaultCenter") = IIf(oRowDC("DefaultCenter"), 1, 0)
                            oRow("ManualCenter") = 1
                            bExistsManual = True
                            Exit For
                        End If
                    Next

                    If Not bExistsManual Then
                        ' En cualquier otro caso asignamos el centro de coste por defecto
                        ' del empleado
                        dDefaultCenter = roBaseEngineManager.GetDefaultCenter(IDEmployee, TaskDay, oState)
                        oRow("IDCenter") = dDefaultCenter
                        oRow("DefaultCenter") = 1
                        oRow("ManualCenter") = 0
                    End If

                End If

                da.Update(zCauses)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyEquivalenceCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: CreateDailyEquivalenceCause")
            End Try

        End Sub

        Private Function GetStartDateForMonthAccruals(ByVal StartDateMonth As Long, ByVal StartDateDay As Long, ByVal TaskDay As Date) As Date
            '
            ' Recupera la fecha a partir de la cual debo acumular para un acumulado de tipo mensual
            '
            Dim StartDateYear As Integer
            Dim dRet As Date

            Try
                ' Para acumulados mensuales
                If StartDateDay <= TaskDay.Day Then
                    ' La fecha de inicio es este mes
                    StartDateMonth = TaskDay.Month
                    StartDateYear = TaskDay.Year
                Else
                    ' La fecha de inicio será del mes pasado
                    If (TaskDay.Month - 1) > 0 Then
                        ' El año es el actual
                        StartDateMonth = (TaskDay.Month - 1)
                        StartDateYear = TaskDay.Year
                    Else
                        ' Es el año pasado
                        StartDateMonth = 12
                        StartDateYear = (TaskDay.Year - 1)
                    End If
                End If

                ' Devuelvo la fecha de inicio
                dRet = DateSerial(StartDateYear, StartDateMonth, StartDateDay)
            Catch ex As Exception
                dRet = DateSerial(TaskDay.Year, TaskDay.Month, 1)
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartDateForMonthAccruals")
            End Try

            Return dRet
        End Function

        Private Function GetStartDateForYearAccruals(ByVal StartDateMonth As Long, ByVal StartDateDay As Long, ByVal TaskDay As Date) As Date
            '
            ' Recupera la fecha a partir de la cual debo acumular para un acumulado de tipo anual
            '

            Dim StartDateYear As Integer
            Dim dRet As Date

            Try

                'El año de inicio puede ser el actual o el anterior, en función del dia/mes
                If StartDateMonth < TaskDay.Month OrElse (StartDateMonth = TaskDay.Month AndAlso StartDateDay <= TaskDay.Day) Then
                    StartDateYear = TaskDay.Year
                Else
                    StartDateYear = (TaskDay.Year - 1)
                End If

                dRet = DateSerial(StartDateYear, StartDateMonth, StartDateDay)
            Catch ex As Exception
                dRet = DateSerial(TaskDay.Year, 1, 1)
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartDateForYearAccruals")
            End Try

            Return dRet

        End Function

        Private Function GetStartDateForWeekAccruals(ByVal StartDayWeek As Integer, ByVal TaskDay As Date) As Date
            '
            ' Recupera la fecha a partir de la cual se debe acumular un saldo semanal
            '
            Dim dRet As Date
            Dim iDayOfWeek As Integer

            Try
                ' Para acumulados semanales, recupero el día de inicio de semana
                iDayOfWeek = Weekday(TaskDay, vbMonday)
                If iDayOfWeek = 0 Then iDayOfWeek = 7
                If StartDayWeek > iDayOfWeek Then StartDayWeek = StartDayWeek - 7

                dRet = DateAdd("d", StartDayWeek - iDayOfWeek, TaskDay)
            Catch ex As Exception
                dRet = DateAdd("d", 1 - iDayOfWeek, TaskDay)
                oState.UpdateStateInfo(ex, "roAccrualsManager:: GetStartDateForYearAccruals")
            End Try

            Return dRet

        End Function

        Public Function DailyScheduleGUIDChanged() As Boolean
            '
            ' Verificamos si el GUID del dia ha sido modificado posteriormente al iniciar el proceso de cálculo
            '

            Try
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(mIDEmployee, mCurrentDate, mPreviousProcessPriority, oState)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::DailyScheduleGUIDChanged")
            End Try

            Return mGUIDChanged
        End Function

        Private Sub GetStartupValue_UPF_AP(ByVal IDEmployee As Long, ByVal IDConcept As Integer, ByVal dblBase As Double, ByVal IDLabAgree As Double, ByRef StartValue_UPF_AP As Double, ByVal TaskDay As roTime)
            Dim strLabAgree As String
            Dim strEmployee As String
            Dim IDEmployeeBase As Long
            Dim SQL As String
            Dim StartAccrualDate As New roTime
            Dim EndAccrualDate As New roTime
            Dim TeoricCalendarHours As Double
            Dim EmployeeCalendarHours As Double
            Dim BeginDate As Date
            Dim EndDate As Date
            Try

                StartValue_UPF_AP = 0

                ' 01.Obtenemos el calendario base del empleado plantilla del horario en funcion del convenio del empleado
                strLabAgree = Trim$(Any2String(ExecuteScalar("@SELECT# Description from LabAgree with (nolock) where id=" & IDLabAgree.ToString)))
                If InStr(strLabAgree, "#") > 0 Then
                    strEmployee = "Plantilla " & Mid$(strLabAgree, InStr(strLabAgree, "#") + 1)
                Else
                    strEmployee = ""
                    Exit Sub
                End If

                IDEmployeeBase = Any2Long(ExecuteScalar("@SELECT# ID from Employees with (nolock) where Name like '" & strEmployee & "'"))
                If IDEmployeeBase = 0 Then Exit Sub

                StartAccrualDate = Any2Time(DateSerial(Year(TaskDay.Value), 1, 1))
                EndAccrualDate = StartAccrualDate.Add(1, "yyyy").Add(-1, "d")

                ' Obtenemos el total de horas teoricas del calendario base del periodo del saldo
                TeoricCalendarHours = GetTeoric_UPF(IDEmployeeBase, StartAccrualDate.Value, EndAccrualDate.Value)

                ' Obtenemos el periodo del contrato actual y el periodo de fechas a obtener el calendario del empleado a procesar
                SQL = "@SELECT# * FROM EmployeeContracts with (nolock) WHERE IDEmployee =" & IDEmployee.ToString
                SQL = SQL & " AND BeginDate <=" & TaskDay.SQLSmallDateTime
                SQL = SQL & " AND EndDate >=" & TaskDay.SQLSmallDateTime
                Dim ads As New DataTable
                ads = CreateDataTable(SQL)
                If ads IsNot Nothing AndAlso ads.Rows.Count > 0 Then
                    BeginDate = roConversions.Max(Any2Time(ads.Rows(0)("BeginDate")).Value, StartAccrualDate.Value)
                    EndDate = roConversions.Min(Any2Time(ads.Rows(0)("EndDate")).Value, EndAccrualDate.Value)
                    ' Obtenemos el total de horas teoricas del calendario del empleado en el periodo calculado
                    EmployeeCalendarHours = GetTeoric_UPF(IDEmployee, BeginDate, EndDate)

                    ' Se eliminan los dias de previsiones de ausencia que no se deben tener en cuenta
                    EmployeeCalendarHours = EmployeeCalendarHours - HoursNotMerit_UPF(IDEmployee, BeginDate, EndDate)

                    ' dblBase --> mBase
                    'TeoricCalendarHours --> HorasTeoricasCalendar
                    'EmployeeCalendarHours --> HorasTeoricas
                    If TeoricCalendarHours <> 0 Then
                        StartValue_UPF_AP = (EmployeeCalendarHours * dblBase) / TeoricCalendarHours
                        If StartValue_UPF_AP - Int(StartValue_UPF_AP) > 0 Then
                            If (StartValue_UPF_AP - Int(StartValue_UPF_AP)) >= 0.5 Then
                                StartValue_UPF_AP = Int(StartValue_UPF_AP) + 1
                            Else
                                StartValue_UPF_AP = Int(StartValue_UPF_AP)
                            End If
                        End If
                    Else
                        StartValue_UPF_AP = 0
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::GetStartupValue_UPF_AP")
                StartValue_UPF_AP = 0
            End Try
        End Sub

        Private Sub GetStartupValue_UPF_VAC(ByVal IDEmployee As Long, ByVal IDConcept As Integer, ByVal dblBase As Double, ByVal IDLabAgree As Double, ByRef StartValue_UPF_VAC As Double, ByVal TaskDay As roTime)
            Dim MeritEndDate As String
            Dim MeritBeginDate As String
            Dim IsActiveContract As Boolean
            Dim BeginDate As String = ""
            Dim EndDate As String = ""
            Dim tmpBegindate As String
            Dim tmpEnddate As String = ""
            Dim mPeriod As Double
            Dim sSQL As String
            Dim mDifDays As Double

            Try

                StartValue_UPF_VAC = 0
                IsActiveContract = False

                'Formato parametro avanzado del periodo de calculo -01/09_31/08
                MeritBeginDate = Trim$(Any2String(ExecuteScalar("@SELECT# value from sysroLiveAdvancedParameters with (nolock) where parametername = 'UPF.StartupValue.CustomPeriod'")))
                If Len(MeritBeginDate) = 0 Then Exit Sub
                If StringItemsCount(MeritBeginDate, "_") <> 2 Then Exit Sub
                MeritEndDate = MeritBeginDate

                MeritEndDate = String2Item(MeritBeginDate, 1, "_") & "/" & Year(TaskDay.Value)
                MeritBeginDate = String2Item(MeritBeginDate, 0, "_")
                If Mid$(MeritBeginDate, 1, 1) = "-" Then
                    MeritBeginDate = Mid$(MeritBeginDate, 2) & "/" & (Year(TaskDay.Value) - 1)
                Else
                    MeritBeginDate = Mid$(MeritBeginDate, 2) & "/" & Year(TaskDay.Value)
                End If

                ' Seleccionamos el peridodo a tener en cuenta en funcion de los contratos del empleado
                sSQL = "@SELECT# * FROM EmployeeContracts with (nolock) WHERE " _
                        & "IDEmployee=" & IDEmployee & " AND " _
                        & "BeginDate<" & Any2Time(MeritEndDate).SQLDateTime & " AND EndDate>" & Any2Time(MeritBeginDate).SQLDateTime _
                        & " ORDER BY BeginDate asc"
                Dim ads = New DataTable
                ads = CreateDataTable(sSQL)

                If Not ads Is Nothing AndAlso ads.Rows.Count > 0 Then
                    For Each oRow As DataRow In ads.Rows
                        IsActiveContract = True
                        If BeginDate = "" Then
                            BeginDate = roConversions.Max(Any2Time(oRow("BeginDate")).Value, Any2Time(MeritBeginDate).Value)
                            EndDate = roConversions.Min(Any2Time(oRow("EndDate")).Value, Any2Time(MeritEndDate).Value)

                            tmpBegindate = BeginDate
                            tmpEnddate = EndDate

                        End If
                        If Not DateTime2Double(oRow("BeginDate")) = DateTime2Double(DateAdd("d", 1, tmpEnddate)) Then
                            BeginDate = roConversions.Max(Any2Time(oRow("BeginDate")).Value, Any2Time(MeritBeginDate).Value)
                        End If

                        EndDate = roConversions.Min(Any2Time(oRow("EndDate")).Value, Any2Time(MeritEndDate).Value)

                        tmpBegindate = BeginDate
                        tmpEnddate = EndDate
                    Next
                End If

                If IsActiveContract Then
                    mPeriod = (DateDiff("d", MeritBeginDate, MeritEndDate) + 1)
                    mDifDays = DateDiff("d", BeginDate, EndDate) + 1 - DaysNotMerit_UPF(Any2Double(IDEmployee), BeginDate, EndDate)
                    StartValue_UPF_VAC = (mDifDays * dblBase) / mPeriod

                    If StartValue_UPF_VAC - Int(StartValue_UPF_VAC) > 0 Then
                        If (StartValue_UPF_VAC - Int(StartValue_UPF_VAC)) >= 0.5 Then
                            StartValue_UPF_VAC = Int(StartValue_UPF_VAC) + 1
                        Else
                            StartValue_UPF_VAC = Int(StartValue_UPF_VAC)
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::GetStartupValue_UPF_VAC")
                StartValue_UPF_VAC = 0
            End Try

        End Sub

        Private Function GetTeoric_UPF(ByVal IDEmployee As Long, ByVal BeginDate As String, ByVal EndDate As String) As Double
            '
            ' Obtenemos las horas teoricas del empleado en el periodo seleccionado
            '
            Dim SQL As String
            Dim TeoricCalendarHours As Double

            Try
                SQL = "@SELECT# CONVERT(NUMERIC(18,6),sum(isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours))) AS TOTAL FROM DAILYSCHEDULE with (nolock) "
                SQL = SQL & " INNER JOIN SHIFTS with (nolock) ON dbo.Shifts.ID = CASE WHEN isnull(IsHolidays,0) = 1 THEN IDShiftBase ELSE IDShift1 END WHERE DAILYSCHEDULE.IDEmployee = " & IDEmployee.ToString
                SQL = SQL & " and Dailyschedule.Date >=" & Any2Time(BeginDate).SQLSmallDateTime & " and Dailyschedule.Date <=" & Any2Time(EndDate).SQLSmallDateTime

                TeoricCalendarHours = Any2Double(ExecuteScalar(SQL))
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::GetTeoric_UPF")
                TeoricCalendarHours = 0
            End Try

            Return TeoricCalendarHours
        End Function

        Private Function DaysNotMerit_UPF(ByVal IDEmployee As Double, ByVal BeginDate As String, ByVal EndDate As String) As Double
            Dim sSQL As String
            Dim tmpBegindate As String
            Dim tmpEnddate As String
            Dim ads As New DataTable
            Dim dblDaysNotMerit_UPF As Double = 0
            Try

                ' Seleccionamos todas las ausencias prolongadas que no meriten
                sSQL = "@SELECT# * FROM ProgrammedAbsences with (nolock)  WHERE " _
                    & "IDEmployee=" & IDEmployee.ToString & " AND (" _
                    & "(BeginDate<" & Any2Time(EndDate).SQLDateTime & " AND FinishDate>" & Any2Time(BeginDate).SQLDateTime _
                    & " ) or (FinishDate is null ))" _
                    & " and IDCause in(@SELECT# id from causes with (nolock)  where Description like '%%UPFCauseException%')" _
                    & " ORDER BY BeginDate asc"
                ads = CreateDataTable(sSQL)

                If ads IsNot Nothing AndAlso ads.Rows.Count > 0 Then
                    For Each oRow As DataRow In ads.Rows
                        tmpBegindate = roConversions.Max(Any2Time(oRow("BeginDate")).Value, Any2Time(BeginDate).Value)
                        tmpEnddate = Any2String(oRow("FinishDate"))
                        If Not IsDate(tmpEnddate) Then
                            tmpEnddate = DateAdd("d", Any2Double(oRow("MaxLastingDays")) - 1, tmpBegindate)
                        End If
                        tmpEnddate = roConversions.Min(Any2Time(tmpEnddate).Value, Any2Time(EndDate).Value)

                        If Any2Time(tmpEnddate).Value >= Any2Time(tmpBegindate).Value Then
                            dblDaysNotMerit_UPF = dblDaysNotMerit_UPF + DateDiff("d", tmpBegindate, tmpEnddate) + 1
                        End If
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::DaysNotMerit_UPF")
                dblDaysNotMerit_UPF = 0
            End Try

            Return dblDaysNotMerit_UPF

        End Function

        Private Function HoursNotMerit_UPF(ByVal IDEmployee As Double, ByVal BeginDate As Date, ByVal EndDate As Date) As Double
            Dim sSQL As String
            Dim tmpBegindate As String
            Dim tmpEnddate As String
            Dim ads As New DataTable
            Dim dblHoursNotMerit_UPF As Double = 0

            dblHoursNotMerit_UPF = 0
            Try

                ' Seleccionamos todas las ausencias prolongadas que no meriten
                sSQL = "@SELECT# * FROM ProgrammedAbsences with (nolock) WHERE " _
            & "IDEmployee=" & IDEmployee.ToString & " AND (" _
            & "(BeginDate<" & Any2Time(EndDate).SQLDateTime & " AND FinishDate>" & Any2Time(BeginDate).SQLDateTime _
            & " ) or (FinishDate is null ))" _
            & " and IDCause in(@SELECT# id from causes with (nolock)  where Description like '%UPFCauseException%')" _
            & " ORDER BY BeginDate asc"
                ads = CreateDataTable(sSQL)
                If ads IsNot Nothing AndAlso ads.Rows.Count > 0 Then
                    For Each oRow As DataRow In ads.Rows
                        tmpBegindate = roConversions.Max(Any2Time(oRow("BeginDate")).Value, Any2Time(BeginDate).Value)
                        tmpEnddate = Any2String(oRow("FinishDate"))
                        If Not IsDate(tmpEnddate) Then
                            tmpEnddate = DateAdd("d", Any2Double(oRow("MaxLastingDays")) - 1, tmpBegindate)
                        End If
                        tmpEnddate = roConversions.Min(Any2Time(tmpEnddate).Value, Any2Time(EndDate).Value)

                        If Any2Time(tmpEnddate).Value >= Any2Time(tmpBegindate).Value Then
                            dblHoursNotMerit_UPF = dblHoursNotMerit_UPF + GetTeoric_UPF(Any2Long(oRow("IDEmployee")), tmpBegindate, tmpEnddate)
                        End If
                    Next
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager::HoursNotMerit_UPF")
                dblHoursNotMerit_UPF = 0
            End Try

            Return dblHoursNotMerit_UPF

        End Function

        Private Function ExecuteSingleDay_DoLatamOverTime(ByVal EmployeeID As Long, ByVal TaskDate As Date, ByVal DBManualCenters As DataTable, ByVal oLabAgreeEngine As roLabAgreeEngine, ByVal oContract As roContract) As Boolean
            '
            ' Ejecuta la tarea indicada
            '
            Dim zDate As New roTime

            Dim bolRet As Boolean = False
            Dim strSQL As String
            Dim StartAccrualDate As String = String.Empty


            Try

                oState.Result = EngineResultEnum.NoError

                ' Obtiene empleado y fecha de la tarea
                zDate = Any2Time(TaskDate)

                Dim StartDateDayW As Date
                StartDateDayW = GetStartDateForWeekAccruals(intWeekIniday, zDate.Value)


                Select Case oLabAgreeEngine.ExtraHoursConfiguration
                    Case LabAgreeExtraHoursConfiguration.NineAcc ' 9 horas acumuladas dobles a la semana , el resto triples

                        '1. Obtenemos el total de horas extras dobles desde el inicio de la semana hasta ayer
                        StartAccrualDate = Any2Time(StartDateDayW).SQLSmallDateTime
                        strSQL = "@SELECT# sum(isnull(DailyCauses.Value,0)) FROM DailyCauses with (nolock) " &
                                             " WHERE DailyCauses.IDEmployee = " & EmployeeID.ToString &
                                             " AND DailyCauses.Date >= " & StartAccrualDate & " AND DailyCauses.Date < " & zDate.SQLSmallDateTime & " AND DailyCauses.Date >=" & Any2Time(oContract.BeginDate).SQLSmallDateTime &
                                                " AND AccrualsRules=0  " &
                                                " AND IDCause in(" & oLabAgreeEngine.ExtraHoursIDCauseDoubles & ")"

                        Dim TotalWeeklyDoubleHours As Double = Any2Double(ExecuteScalar(strSQL))


                        '2. Obtenemos el total de horas extras simples en la fecha de calculo
                        strSQL = "@SELECT# sum(isnull(DailyCauses.Value,0)) FROM DailyCauses with (nolock) " &
                                             " WHERE DailyCauses.IDEmployee = " & EmployeeID.ToString &
                                             " AND DailyCauses.Date = " & zDate.SQLSmallDateTime &
                                                " AND AccrualsRules=0  " &
                                                " AND IDCause in(" & oLabAgreeEngine.ExtraHoursIDCauseSimples & ")"

                        Dim DailyTotalSimpleHours As Double = Any2Double(ExecuteScalar(strSQL))
                        If DailyTotalSimpleHours > 0 Then
                            '3.Acumulamos como dobles las horas simples como maximo hasta 9,
                            ' el resto como triples
                            Dim MaxTotalDoubleHours = 9 - TotalWeeklyDoubleHours
                            Dim TripleHours As Double = 0
                            Dim DoubleHours As Double = 0
                            If MaxTotalDoubleHours >= DailyTotalSimpleHours Then
                                DoubleHours = DailyTotalSimpleHours
                            Else
                                DoubleHours = MaxTotalDoubleHours
                                TripleHours = DailyTotalSimpleHours - MaxTotalDoubleHours
                            End If

                            ' Creamos las extras dobles y triples en caso necesario
                            If DoubleHours > 0 Then CreateDailyPlusCause(EmployeeID, Any2Time(TaskDate), DoubleHours, oLabAgreeEngine.ExtraHoursIDCauseDoubles, DBManualCenters)
                            If TripleHours > 0 Then CreateDailyPlusCause(EmployeeID, Any2Time(TaskDate), TripleHours, oLabAgreeEngine.ExtraHoursIDCauseTriples, DBManualCenters)
                        End If

                    Case LabAgreeExtraHoursConfiguration.ThreeByThree ' 3x3, maximo 3 dobles por dia , hasta un maximo de 3 dias , el resto triples

                        '1. Obtenemos el nº total de dias que se han producido extras durante la semana
                        StartAccrualDate = Any2Time(StartDateDayW).SQLSmallDateTime
                        strSQL = "@SELECT# count(distinct Date) FROM DailyCauses with (nolock) " &
                                             " WHERE DailyCauses.IDEmployee = " & EmployeeID.ToString &
                                             " AND DailyCauses.Date >= " & StartAccrualDate & " AND DailyCauses.Date < " & zDate.SQLSmallDateTime & " AND DailyCauses.Date >=" & Any2Time(oContract.BeginDate).SQLSmallDateTime &
                                                " AND AccrualsRules=0  " &
                                                " AND IDCause in(" & oLabAgreeEngine.ExtraHoursIDCauseSimples & ")"

                        Dim TotalWeeklyOvertimeDays As Double = Any2Double(ExecuteScalar(strSQL))

                        Dim MaxTotalDoubleHours = 3
                        If TotalWeeklyOvertimeDays >= 3 Then
                            ' Si existen 3 o mas dias de la semana que se han generado extras, todas las extras del dia ya son triples
                            MaxTotalDoubleHours = 0
                        End If

                        '2. Obtenemos el total de horas extras simples en la fecha de calculo
                        strSQL = "@SELECT# sum(isnull(DailyCauses.Value,0)) FROM DailyCauses with (nolock) " &
                                             " WHERE DailyCauses.IDEmployee = " & EmployeeID.ToString &
                                             " AND DailyCauses.Date = " & zDate.SQLSmallDateTime &
                                                " AND AccrualsRules=0  " &
                                                " AND IDCause in(" & oLabAgreeEngine.ExtraHoursIDCauseSimples & ")"

                        Dim DailyTotalSimpleHours As Double = Any2Double(ExecuteScalar(strSQL))
                        If DailyTotalSimpleHours > 0 Then
                            '3.Acumulamos como dobles las horas simples como maximo hasta 3,
                            ' el resto como triples
                            Dim TripleHours As Double = 0
                            Dim DoubleHours As Double = 0
                            If MaxTotalDoubleHours >= DailyTotalSimpleHours Then
                                DoubleHours = DailyTotalSimpleHours
                            Else
                                DoubleHours = MaxTotalDoubleHours
                                TripleHours = DailyTotalSimpleHours - MaxTotalDoubleHours
                            End If

                            ' Creamos las extras dobles y triples en caso necesario
                            If DoubleHours > 0 Then CreateDailyPlusCause(EmployeeID, Any2Time(TaskDate), DoubleHours, oLabAgreeEngine.ExtraHoursIDCauseDoubles, DBManualCenters)
                            If TripleHours > 0 Then CreateDailyPlusCause(EmployeeID, Any2Time(TaskDate), TripleHours, oLabAgreeEngine.ExtraHoursIDCauseTriples, DBManualCenters)
                        End If
                End Select

                ' En el caso que la fecha de calculo sea diferente al dia de hoy y al ultimo dia de la semana 
                ' debemos marcar para recalcular el dia siguiente hasta el final de la semana
                If Not (TaskDate.Ticks = Now.Date.Ticks) AndAlso Not (TaskDate.Ticks = StartDateDayW.AddDays(6).Ticks) Then
                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status = " & roCausesManager.mPriority.ToString & ", GUID=''  WHERE IDEmployee = " & EmployeeID &
                     " AND Date >= " & Any2Time(TaskDate.AddDays(1)).SQLSmallDateTime & "  and Date <= " & Any2Time(StartDateDayW.AddDays(6)).SQLSmallDateTime &
                     " AND Date <= getdate() " &
                     " AND Status > " & roCausesManager.mPriority.ToString & " AND ISNULL(GUID, '') <> '" & VTBase.roConstants.GetManagedThreadGUID() & "'"

                    Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                    Dim nRet As Integer = oSqlCommand.ExecuteNonQuery()

                    If nRet > 0 Then
                        ' Marcamos para lanzar la tarea de recalculo solo si se ha actualizado algun registro
                        mMarkNextExpiredDate = True
                    End If

                End If


                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoLatamOverTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAccrualsManager:: ExecuteSingleDay_DoLatamOverTime")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

    Public Class ConceptValues
        Public Value As Double
        Public PositiveValue As Double
        Public NegativeValue As Double

        Public Sub New(dValue As Double, dPositiveValue As Double, dNegativeValue As Double)
            Value = dValue
            PositiveValue = dPositiveValue
            NegativeValue = dNegativeValue
        End Sub

    End Class

End Namespace