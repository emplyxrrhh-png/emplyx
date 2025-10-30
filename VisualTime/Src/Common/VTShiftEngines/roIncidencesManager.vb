Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTShiftEngines

    Public Class roIncidencesManager
        Private oState As roEngineState = Nothing
        Public Const mPriority = 50
        Public Const mPreviousProcessPriority = 40
        Private mInitialRegisterStatus As Integer = 0
        ' Indica si el dia esta planificado como vacaciones
        Private bolIsHolidays As Boolean
        ' Indica si el dia esta planificado con vacaciones y existe PA que es prioritaria
        Private bolProgrammedAbsenceOnHolidays As Boolean
        ' Comprobar si existe ausencia por horas en franja rigida con inicio semiflexible
        ' para calcular el inicio de la misma
        Private mVerifyProgrammedCauseOnMandatory As String
        ' Comportamiento de ausencias productivas
        Private bolProductiveAbsences As Boolean
        Private dblProductiveAbsenceCause As Double
        Private oLicense As New roServerLicense
        Private mCostCenterInstallled As Boolean = False
        Private mLastPunchCenterEnabled As String = ""
        Private mTask As roLiveTask
        Private mIDEmployee As Integer = 0
        Private mGUIDChanged As Boolean = False
        Public mCurrentDate As Date = Date.MinValue

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

        Public Function ExecuteBatch(ByRef bolGUIDChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim FirstDate As New roTime
            Dim CurrentDate As New roTime
            Dim iLastProcessStatus As Integer = VTShiftEngines.roDetectorManager.mPriority

            Try

                ' Recuperamos ID de empleado de la tarea
                mIDEmployee = If(Not mTask.Parameters Is Nothing AndAlso mTask.Parameters.Exists("IDEmployee"), roTypes.Any2Integer(mTask.Parameters("IDEmployee")), 0)
                If mIDEmployee = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roIncidencesManager::ExecuteBatch: Unable to set IDEmployee from task.")
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
                    Dim totalActions As Integer = dTbl.Rows.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Double = 100 / totalActions

                    For Each oRowEmp As DataRow In dTbl.Rows
                        mInitialRegisterStatus = Any2Integer(oRowEmp("Status"))
                        mCurrentDate = Any2DateTime(oRowEmp("Date")).Date
                        bolIsHolidays = Any2Boolean(oRowEmp("Holidays"))
                        bolProgrammedAbsenceOnHolidays = False
                        bolRet = ExecuteSingleDay(Any2Long(oRowEmp("IDEmployee")), oRowEmp("Date"))
                        If Not bolRet Then
                            Return False
                            Exit Function
                        End If

                        If mGUIDChanged Then
                            ' En el caso que se haya modificado el GUID de algun registro a procesar , salimos
                            bolGUIDChanged = True
                            Return bolRet
                            Exit Function
                        End If

                        ' Actualizo progeso (con lo que se fuerza actualización de IsAliveAt
                        mTask.Progress = mTask.Progress + stepProgress
                        mTask.Save()

                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidencesManager:: ExecuteBatch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidencesManager::ExecuteBatch")
            Finally

            End Try

            Return bolRet

        End Function

        Private Sub LoadConfig()
            Try
                'TODOCACHE
                mVerifyProgrammedCauseOnMandatory = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "VerifyProgrammedCauseOnMandatory")
                mCostCenterInstallled = oLicense.FeatureIsInstalled("Feature\CostControl")
                ' Miramos si tiene que tener el comportamiento
                ' de buscar el último fichaje de centro de coste aunque no sea del mismo dia
                'mLastPunchCenterEnabled = Any2String(ExecuteScalar("@SELECT# value from sysroLiveAdvancedParameters where parametername = 'LastPunchCenterEnabled'"))
                mLastPunchCenterEnabled = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "LastPunchCenterEnabled")

                ' Verifica si tiene comportamiento de ausencias productivas
                bolProductiveAbsences = False
                If DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ApplyProductiveAbsences").ToUpper = "TRUE" Then
                    'If UCase(Any2String(ExecuteScalar("@SELECT# value from sysroLiveAdvancedParameters with (nolock) where parametername = 'Engine.ApplyProductiveAbsences'"))) = "TRUE" Then
                    Dim strCause As String = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.ProductiveAbsencesCause")
                    'Dim strCause As String = Any2String(ExecuteScalar("@SELECT# value from sysroLiveAdvancedParameters with (nolock) where parametername = 'Engine.ProductiveAbsencesCause'"))
                    If strCause.Length > 0 Then
                        dblProductiveAbsenceCause = Any2Double(ExecuteScalar("@SELECT# ID from Causes with (nolock) where Shortname like '" & strCause & "'"))
                        If dblProductiveAbsenceCause > 0 Then bolProductiveAbsences = True
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roIncidencesManager::Init: Error recovering config data.", ex)
            End Try
        End Sub

        Private Function ExecuteSingleDay(ByVal EmployeeID As Long, ByVal TaskDate As Date) As Boolean
            '
            ' Procesa un dia concreto
            '
            Dim bolret As Boolean = False

            Dim zDate As New roTime
            Dim zMoves As New roMoveList
            Dim zTimes As New roTimeBlockList
            Dim zShift As New roShiftEngine
            Dim zZones As Generic.List(Of roShiftEngineTimeZone)
            Dim zForeCast As New roTimeBlockList
            Dim zFloatingTime As Double

            Try

                ' 000. Si la fecha a procesar es futura, no hace nada
                If DateDiff("d", TaskDate, Now) < 0 Then
                    Return True
                    Exit Function
                End If

                Debug.Print(Now & "     ----> INCIDENCES: Processing employee '" & EmployeeID & "', date " & TaskDate)

                ' Obtiene fecha de la tarea
                zDate = Any2Time(TaskDate)

                ' Obtiene horario utilizados
                zShift = roBaseEngineManager.Execute_GetShift(EmployeeID, zDate, bolIsHolidays, bolProgrammedAbsenceOnHolidays, oState)

                ' Obtiene movimientos
                zMoves = roBaseEngineManager.Execute_GetMovesLive(EmployeeID, zDate, oState)

                ' Completa movimientos (si son incompletos, intenta rellenar para poder
                '  calcular algo de todas formas)
                bolret = roBaseEngineManager.Execute_ProcessMoves(zMoves, zDate, oState)
                If Not bolret Then
                    Return bolret
                    Exit Function
                End If

                If Not zShift Is Nothing Then
                    ' Comprobamos si el horario es de tipo flotante y obtenemos el tiempo a desplazar
                    roBaseEngineManager.GetFloatingTime(EmployeeID, Any2Double(zShift.ID), zDate.Value, zFloatingTime, oState)
                Else
                    zFloatingTime = 0
                End If

                ' Obtiene previsiones en caso necesario
                If mVerifyProgrammedCauseOnMandatory = "1" Then
                    zForeCast = roBaseEngineManager.Execute_GetForecast(EmployeeID, zDate, zShift, zMoves, oState)
                End If

                ' Inicializamos bloques de tiempo
                zTimes = roBaseEngineManager.InitializeBlocks(zDate, zMoves, oState)

                ' Procesa comportamiento de ausencia por trabajo externo OK
                If bolProductiveAbsences Then
                    ' Se utiliza unicamente la justificacion configurada en el registro
                Else
                    ' En caso contrario, se utilizan las configuradas mediante la pantalla de justificaciones v2
                    dblProductiveAbsenceCause = -1
                End If
                roBaseEngineManager.ProcessProductiveAbsences(zDate, zShift, zTimes, dblProductiveAbsenceCause, oState)

                ' Procesa capas horarias para una fecha y horario y devuelve lista de tiempos.
                bolret = roBaseEngineManager.ProcessLayers(zDate, zShift, zTimes, zFloatingTime, EmployeeID, oState, zForeCast)
                If Not bolret Then
                    Return bolret
                    Exit Function
                End If

                ' Elimina descansos (de momento VisualTime no graba los tiempos de descanso, pero los
                '  usa internamente y en un futuro se podrían grabar)
                bolret = roBaseEngineManager.Execute_RemoveBreakTimes(zTimes, oState)
                If Not bolret Then
                    Return bolret
                    Exit Function
                End If

                ' Procesa zonas horarias
                If Not zShift Is Nothing Then
                    zZones = zShift.TimeZones
                    If Not zZones Is Nothing Then
                        bolret = roBaseEngineManager.Execute_ProcessZones(zDate, zTimes, zZones, zShift, zFloatingTime, oState)
                        If Not bolret Then
                            Return bolret
                            Exit Function
                        End If
                    End If
                End If

                ' Asignamos el centro de coste a cada incidencia generada
                If mCostCenterInstallled Then
                    bolret = roBaseEngineManager.Execute_ProcessCenters(EmployeeID, zDate, zTimes, zShift, oState, zMoves, bolProgrammedAbsenceOnHolidays, mLastPunchCenterEnabled)
                    If Not bolret Then
                        Return bolret
                        Exit Function
                    End If
                End If

                ' Compacta incidencias
                bolret = roBaseEngineManager.CompactLayers(zTimes, oState)
                If Not bolret Then
                    Return bolret
                    Exit Function
                End If

                ' Verificamos que no se ha marcado para procesar por otro proceso
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(EmployeeID, zDate.ValueDateTime, mPreviousProcessPriority, oState)
                If mGUIDChanged Then
                    Return True
                    Exit Function
                End If

                ' Guarda datos en la base de datos
                bolret = Execute_SaveData(EmployeeID, zDate, zShift, zTimes)
                If Not bolret Then
                    Return bolret
                    Exit Function
                End If


            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidencesManager::ExecuteSingleDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidencesManager::ExecuteSingleDay")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_SaveData(ByVal zEmployee As Long, ByVal zDate As roTime, ByRef Shift As roShiftEngine, ByRef TimeBlockList As roTimeBlockList) As Boolean
            '
            ' Guarda datos en la base de datos
            '
            Dim sSQL As String
            Dim UsedIDCollection As New roCollection
            Dim NextValidID As Long
            Dim CurrentRegisterStatus As Integer = 0
            Dim bolret As Boolean = False

            Try

                ' Sincroniza incidencias con las de la base de datos
                '  Si una incidencia ya existe en la base de datos y es igual, la quita de memoria.
                '  Si una incidencia esta en la BD pero es distinta, la quita de la BD.
                '  Si una incidencia esta en la BD pero no en memoria, la quita.
                Dim sSQLDaily As String = "@SELECT# * FROM DailyIncidences WHERE (IDEmployee=" & zEmployee &
                            " AND Date=" & zDate.SQLSmallDateTime & "AND IDType BETWEEN 1000 AND 2000)"
                'If myDS.OpenRecordset(sSQL, mConnection, adOpenForwardOnly, adLockOptimistic) Then
                Dim dTbl As System.Data.DataTable = CreateDataTable(sSQLDaily)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    ' Comprueba cada incidencia que hay en la base de datos
                    For Each oRowEmp As DataRow In dTbl.Rows
                        UsedIDCollection.Add(oRowEmp("ID"))  ' Guarda ID en la lista de IDs usados
                        bolret = Execute_SaveDate_SyncIncidence(oRowEmp, TimeBlockList)
                        If Not bolret Then
                            Return bolret
                            Exit Function
                        End If
                    Next
                End If

                ' Verificamos que no se ha marcado para procesar por otro proceso
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(zEmployee, zDate.ValueDateTime, mPreviousProcessPriority, oState)
                If mGUIDChanged Then
                    Return True
                    Exit Function
                End If

                ' Finalmente, grabamos las incidencias que aun tenemos en memoria

                NextValidID = 0
                For index As Integer = 1 To TimeBlockList.Count
                    ' Obtiene siguiente ID libre para grabar
                    Do
                        NextValidID = NextValidID + 1
                    Loop While UsedIDCollection.Exists(NextValidID)
                    If TimeBlockList.Item(index).TimeValue.NumericValue(True) >= 0 Then
                        ' Graba incidencia
                        sSQL = "@INSERT# INTO DailyIncidences(ID,IDEmployee,Date,IDType,IDZone,Value,BeginTime,EndTime, IDCenter, DefaultCenter) " &
                                "VALUES(" & NextValidID & "," & zEmployee & "," & zDate.SQLSmallDateTime & "," &
                                    TimeBlockList.Item(index).BlockType & "," & Any2Long(TimeBlockList.Item(index).TimeZone) & "," & TimeBlockList.Item(index).TimeValue.NumericValue(True).ToString.Replace(",", ".") &
                                        "," & TimeBlockList.Item(index).Period.Begin.SQLSmallDateTime & "," & TimeBlockList.Item(index).Period.Finish.SQLSmallDateTime & "," & Any2Double(TimeBlockList.Item(index).IDCenter) & "," & IIf(Any2Double(TimeBlockList.Item(index).IDCenter) = 0, 1, IIf(TimeBlockList.Item(index).DefaultCenter, 1, 0)) & ")"
                        If Not ExecuteSql(sSQL) Then
                            Err.Raise(16313, "SQLExecute", "SQL Insert DailyIncidences failed.")
                        End If
                    End If
                Next

                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(zEmployee, zDate.ValueDateTime, mPreviousProcessPriority, oState)
                If mGUIDChanged Then
                    Return True
                    Exit Function
                End If

                sSQL = "@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET TimestampEngine = GETDATE(), Status=" & mPriority.ToString &
                         " WHERE IDEmployee=" & zEmployee & " AND Date=" & zDate.SQLSmallDateTime
                If Not ExecuteSql(sSQL) Then
                    Err.Raise(16313, "SQLExecute", "SQL Update Status failed.")
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidencesManager:: Execute_SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidencesManager::Execute_SaveData")
            Finally

            End Try

            Return bolret

        End Function

        Private Function Execute_SaveDate_SyncIncidence(ByRef Incidence As DataRow, ByRef TimeBlockList As roTimeBlockList) As Boolean
            '
            ' Busca si la incidencia que nos pasan esta en la lista de tiempos en memoria.
            '  Si esta, la borra de la lista de tiempos en memoria.
            '  Si no está, la borra de la base de datos, con las causas relacionadas, y devuelve True
            '   para que sepamos que algo ha cambiado.
            Dim IncidenceBegin As Double
            Dim IncidenceFinish As Double
            Dim IncidenceType As Object
            Dim IncidenceValue As Double
            Dim IncidenceZone As Double
            Dim IncidenceCenter As Double
            Dim Index As Long
            Dim bRemove As Boolean
            Dim bolret As Boolean = False

            Try

                ' Cache de incidencia
                IncidenceBegin = Any2Double(Any2DateTime(Incidence("BeginTime")))
                IncidenceFinish = Any2Double(Any2DateTime(Incidence("EndTime")))
                IncidenceType = Any2Integer(Incidence("IDType"))
                IncidenceValue = Any2Double(Any2DateTime(Incidence("Value")))
                IncidenceZone = Any2Integer(Incidence("IDZone"))
                IncidenceCenter = Any2Double(Incidence("IDCenter"))

                ' Mira si la incidencia ya existe y la podemos quitar de memoria
                bRemove = False
                For Index = 1 To TimeBlockList.Count
                    With TimeBlockList.Item(Index)
                        If .Period.Begin.VBNumericValue = IncidenceBegin And
                            .Period.Finish.VBNumericValue = IncidenceFinish And
                            .TimeValue.VBNumericValue = IncidenceValue And
                            .BlockType = IncidenceType And
                            .TimeZone = IncidenceZone And
                            .IDCenter = IncidenceCenter Then
                            ' La incidencia es la misma, la quitamos de memoria
                            bRemove = True
                        End If
                    End With
                    If bRemove Then
                        TimeBlockList.Remove(Index)
                        bolret = True
                        Return bolret
                        Exit Function
                    End If
                Next

                ' Si llegamos aqui es que la incidencia no existe en memoria y por lo tanto debe
                '  eliminarse de la base de datos, junto con las causas relacionadas.
                bolret = True

                If Not ExecuteSql("@DELETE# FROM DailyCauses WHERE " &
                    "(IDEmployee=" & Incidence("IDEmployee") & " AND " &
                    "Date=" & Any2Time(Incidence("Date")).SQLSmallDateTime & " AND " &
                    "IDRelatedIncidence=" & Incidence("ID") & ")") Then
                    Err.Raise(16313, "SQLExecute", "SQL Delete Causes failed.")
                End If

                If Not ExecuteSql("@DELETE# FROM DailyIncidences WHERE " &
                    "(IDEmployee=" & Incidence("IDEmployee") & " AND " &
                    "Date=" & Any2Time(Incidence("Date")).SQLSmallDateTime & " AND " &
                    "ID=" & Incidence("ID") & ")") Then
                    Err.Raise(16314, "SQLExecute Delete Incidence", "SQL Statement failed.")
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roIncidencesManager:: Execute_SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roIncidencesManager::Execute_SaveData")
            Finally

            End Try

            Return bolret

        End Function

#End Region

    End Class

End Namespace