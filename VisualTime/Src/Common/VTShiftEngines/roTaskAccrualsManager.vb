Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace VTShiftEngines

    Public Class roTaskAccrualsManager
        Private oState As roEngineState = Nothing
        Public Const mPriority = 80
        Public Const mPreviousProcessPriority = 0
        Private mInitialRegisterStatus As Integer = 0
        Private mTask As roLiveTask
        Private mIDEmployee As Integer = 0
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

        Public Function ExecuteBatch(ByRef bolGUIDChanged As Boolean, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Recuperamos ID de empleado de la tarea
                mIDEmployee = If(mTask.Parameters IsNot Nothing AndAlso mTask.Parameters.Exists("IDEmployee"), roTypes.Any2Integer(mTask.Parameters("IDEmployee")), 0)
                If mIDEmployee = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roTaskAccrualsManager::ExecuteBatch: Unable to set IDEmployee from task.")
                    Me.oState.Result = EngineResultEnum.EmployeeRequired
                    Return False
                End If

                Me.oState.Result = EngineResultEnum.NoError

                ' Únicamente considero los registros con status comprendido entre mi prioridad y la prioridad del proceso precedente
                Dim strSQL As String = "@SELECT# IDEmployee,Date, TaskStatus, isnull(IDShiftUsed,0) as IDShift FROM DailySchedule with (nolock) INNER JOIN Employees with (nolock) ON DailySchedule.IDEmployee=Employees.ID" &
                                            " WHERE TaskStatus < " & mPriority.ToString &
                                                " AND Date<=" & Any2Time(Now.Date).SQLSmallDateTime & " AND IDEmployee = " & mIDEmployee.ToString &
                                                " AND GUID = '" & VTBase.roConstants.GetManagedThreadGUID() & "' AND Employees.Type='J' " &
                                                    " ORDER BY IDEmployee,Date"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    Dim totalActions As Integer = dTbl.Rows.Count
                    If totalActions = 0 Then totalActions = 1
                    Dim stepProgress As Double = 100 / totalActions

                    For Each oRowEmp As DataRow In dTbl.Rows
                        mInitialRegisterStatus = Any2Integer(oRowEmp("TaskStatus"))
                        bolRet = ExecuteSingleDay(Any2Long(oRowEmp("IDEmployee")), oRowEmp("Date"), oRowEmp("IDShift"))
                        If Not bolRet Then
                            Return False
                        End If

                        If mGUIDChanged Then
                            ' En el caso que se haya modificado el GUID de algun registro a procesar , salimos
                            bolGUIDChanged = True
                            Return bolRet
                        End If

                        ' Actualizo progeso (con lo que se fuerza actualización de IsAliveAt
                        mTask.Progress = mTask.Progress + stepProgress
                        mTask.Save()
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager:: ExecuteBatch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager::ExecuteBatch")
            End Try

            Return bolRet

        End Function

        Private Function ExecuteSingleDay(ByVal EmployeeID As Long, ByVal TaskDate As Date, ByVal IDShift As Integer) As Boolean
            '
            ' Procesa un dia concreto
            '
            Dim bolret As Boolean = False

            Dim AttendanceMoves As roMoveList
            Dim EmployeeTaskMoves As Generic.List(Of roTaskMoveItem)
            Dim DayBeginTime As New roTime
            Dim DayFinishTime As New roTime
            Dim AtomicAccruals As New roCollection
            Dim z As Integer = 0
            Dim zDate As New roTime
            Dim FilterMoves As New roMoveList
            Dim zShift As New roShiftEngine
            Dim mBreakLayers As New roTimeBlockList

            Try

                Debug.Print(Now & "     ----> TASKACCRUALS: Processing employee '" & EmployeeID & "', date " & TaskDate)

                ' Obtiene fecha de la tarea
                zDate = Any2Time(TaskDate)

                ' Obtiene movimientos de presencia del empleado
                AttendanceMoves = roBaseEngineManager.Execute_GetAttendanceMoves(EmployeeID, zDate, oState)

                If AttendanceMoves IsNot Nothing AndAlso AttendanceMoves.Moves.Count > 0 Then
                    ' Obtiene horario utilizado
                    zShift = roBaseEngineManager.GetShiftFromCache(IDShift, oState)

                    ' Obtenemos los datos necesarios del horario (Filtros y descansos)
                    If zShift IsNot Nothing AndAlso zShift.ID > 0 Then
                        roBaseEngineManager.Execute_GetTaskShiftFilter(zDate, zShift, FilterMoves, mBreakLayers, oState)
                    End If

                    ' Calcula limites diarios de presencia del empleado
                    DayBeginTime = AttendanceMoves.Moves(0).Period.Begin
                    DayFinishTime = AttendanceMoves.Moves(AttendanceMoves.Moves.Count - 1).Period.Finish

                    For z = 0 To AttendanceMoves.Moves.Count - 1
                        AttendanceMoves.Moves(z).Period.Begin = Any2Time(Format$(CDate(AttendanceMoves.Moves(z).Period.Begin.Value), "yyyy/MM/dd HH:mm"))
                        AttendanceMoves.Moves(z).Period.Finish = Any2Time(Format$(CDate(AttendanceMoves.Moves(z).Period.Finish.Value), "yyyy/MM/dd HH:mm"))
                    Next z

                    'Recortamos los fichajes de presencia en función de los filtros
                    If FilterMoves.Moves.Count > 0 Then
                        roBaseEngineManager.Execute_GetEfectiveEmployeeMoves(AttendanceMoves, FilterMoves, oState)
                    End If

                    'Recortamos los fichajes de presencia en funcion de los descansos
                    If mBreakLayers.Count > 0 Then
                        roBaseEngineManager.Execute_GetEfectiveEmployeeMoveswithoutBreakLayers(AttendanceMoves, mBreakLayers, oState)
                    End If

                    ' Obtiene movimientos de tareas del empleado
                    EmployeeTaskMoves = roBaseEngineManager.Execute_GetEmployeeTaskMoves(EmployeeID, DayBeginTime, DayFinishTime, oState)

                    ' Agrupamos los fichajes segun los campos de la ficha
                    roBaseEngineManager.Execute_SetEmployeeTaskMovesGroupbyFields(EmployeeTaskMoves, oState)

                    ' Obtiene tiempos parciales de fichajes individuales
                    roBaseEngineManager.Execute_AddEmployeeAtomicAccruals(AtomicAccruals, AttendanceMoves, EmployeeTaskMoves, EmployeeID, DayBeginTime, DayFinishTime, oState)
                Else
                    ' Si no tiene movimientos, no hace falta calcular nada, no tiene tiempos de tareas
                End If
                ' Verificamos que no se ha marcado para procesar por otro proceso
                mGUIDChanged = roBaseEngineManager.DailyScheduleGUIDChangedOrStatusOverwritted(EmployeeID, zDate.ValueDateTime, mPreviousProcessPriority, oState)
                If mGUIDChanged Then
                    Return True
                End If

                ' Guarda datos en la base de datos
                bolret = Execute_SaveData(AtomicAccruals, EmployeeID, zDate)
                If Not bolret Then
                    Return bolret
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager::ExecuteSingleDay")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager::ExecuteSingleDay")
            End Try

            Return bolret

        End Function

        Private Function Execute_SaveData(ByVal AtomicAccruals As roCollection, ByVal EmployeeID As Long, ByVal zDate As roTime) As Boolean
            '
            ' Guarda datos en la base de datos
            '
            Dim SQL As String
            Dim bolret As Boolean = False

            Dim Index As Integer = 1

            Try

                ' Elimina datos anteriores de acumulados
                If Not ExecuteSql("@DELETE# FROM DailyTaskAccruals WHERE IDEmployee=" & EmployeeID.ToString &
                            " AND Date=" & zDate.SQLSmallDateTime) Then
                    Err.Raise(16322, "SQLExecute", "Could not remove records from DailyTaskAccruals: SQL failed.")
                End If

                Index = 1
                ' Guarda todos los acumulados (si hay)
                For z = 1 To AtomicAccruals.Count
                    SQL = "@INSERT# INTO DailyTaskAccruals(IDEmployee,IDTask,Date,Value, IDPart, Field1, Field2, Field3,Field4, Field5, Field6) " &
                            "VALUES(" & EmployeeID.ToString & "," & AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).IDJob & "," &
                            zDate.SQLSmallDateTime & "," &
                            AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).TimeValue.NumericValue(True).ToString.Replace(",", ".") & "," &
                            Index.ToString & ",'" & Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field1).Replace(",", ".") & "'," &
                            "'" & Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field2).Replace(",", ".") & "'," &
                            "'" & Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field3).Replace(",", ".") & "'," &
                            Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field4).Replace(",", ".") & "," &
                            Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field5).Replace(",", ".") & "," &
                            Any2String(AtomicAccruals.Item(z, roCollection.roSearchMode.roByIndex).Field6).Replace(",", ".") & ")"
                    If Not ExecuteSql(SQL) Then
                        Err.Raise(12512, "Execute::SaveData", "SQL Statement failed (inserting DailyTaskAccruals)")
                    End If
                    Index = Index + 1
                Next

                SQL = "@UPDATE# DAILYSCHEDULE WITH (ROWLOCK) SET TaskStatus=" & mPriority.ToString &
                         " WHERE IDEmployee=" & EmployeeID.ToString & " AND Date=" & zDate.SQLSmallDateTime
                If Not ExecuteSql(SQL) Then
                    Err.Raise(16313, "SQLExecute", "SQL Update Status failed.")
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager:: Execute_SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTaskAccrualsManager::Execute_SaveData")
            End Try

            Return bolret

        End Function

#End Region

    End Class

End Namespace