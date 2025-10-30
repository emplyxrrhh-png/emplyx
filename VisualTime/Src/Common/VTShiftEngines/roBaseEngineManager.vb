Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.Base.VTCauses.Causes
Imports Robotics.Base.VTConcepts.Concepts
Imports Robotics.Base.VTLabAgrees.LabAgree
Imports Robotics.Base.VTShifts.Shifts
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase.Extensions

Namespace VTShiftEngines

    Public Class roBaseEngineManager

        Public Const roFilterTreatAsWork = "TreatAsWork"
        Public Const roFilterIgnore = "Ignore"
        Public Const roFilterGenerateIncidence = "Incidence"
        Public Const roFilterGenerateOvertime = "Overtime"
        Public Const roOvertimeAsOvertime = "Overtime"
        Public Const roOvertimeAsIncidence = "Incidence"
        Public Const roBreakCreateIncidence = "CreateIncidence"
        Public Const roBreakSubstractAttendanceTime = "SubstractAttendanceTime"
        Public Const roUserTaskObject = "USERTASK"
        Public Const roFunctionCallObject = "FN"
        Public Const roProcDetector = "PROC:\\DETECTOR"
        Public Const roProcIncidences = "PROC:\\INCIDENCES"
        Public Const roProcCauses = "PROC:\\CAUSES"
        Public Const roProcAccruals = "PROC:\\ACCRUALS"

        Public Const roNullDate = "1/1/2079"

#Region "Helper Methods"

        Public Shared Function InitializeBlocks(ByVal zDate As roTime, ByVal zMoves As roMoveList, ByRef oState As roEngineState) As roTimeBlockList
            '
            ' Realiza el paso previo al tratamiento de capas, consistente en pasar todos
            '  los movimientos como bloques de extras.
            ' Devuelve la lista de bloques.
            '

            Dim myList As New roTimeBlockList
            Dim myBlock As roTimeBlockItem
            Dim Index As Integer
            Try
                For Index = 0 To zMoves.Moves.Count - 1
                    ' Copia cada movimiento como extras
                    myBlock = New roTimeBlockItem
                    myBlock.LoadFromMove(zMoves.Moves(Index))
                    myBlock.BlockType = roTimeBlockItem.roBlockType.roBTOverworking
                    myList.Add(myBlock)
                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::InitializeBlocks")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::InitializeBlocks")
            End Try

            Return myList

        End Function

        Public Shared Function Execute_GetShift(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal bolIsHolidays As Boolean, ByRef bolProgrammedAbsenceOnHolidays As Boolean, ByRef oState As roEngineState) As roShiftEngine
            '
            ' Obtiene horarios posibles entre las fechas indicadas
            '
            Dim myShiftID As Integer
            Dim myShift As New roShiftEngine

            Dim myShiftIDBase As Integer = 0

            Try

                oState.Result = EngineResultEnum.NoError

                myShift = New roShiftEngine

                ' Select de planes diarios
                myShiftID = Any2Integer(ExecuteScalar("@SELECT#  IDShiftUsed FROM DailySchedule with (nolock) WHERE Date=" &
                                zDate.SQLSmallDateTime & " AND IDEmployee=" & zEmployee))

                If myShiftID > 0 Then
                    ' Coge el horario indicado
                    myShift = roBaseEngineManager.GetShiftFromCache(myShiftID, oState)
                    If bolIsHolidays Then
                        ' En el caso que el dia esté planificado con vacaciones y exista previsión de ausencia por dias de tipo baja
                        ' debemos utilizar como horario principal el base
                        myShiftIDBase = Any2Double(ExecuteScalar("@SELECT# isnull(IDShiftBase,0) from DailySchedule with (nolock) where IDShift1 in(@SELECT# ID from shifts with (nolock) where ShiftType =2)  and IDEmployee= " & zEmployee & " and Date =" & zDate.SQLSmallDateTime &
                        " AND EXISTS (@SELECT# 1 AS ExistReg FROM ProgrammedAbsences with (nolock) Left Join Causes with (nolock) On Causes.ID = ProgrammedAbsences.IDCause " &
                        " LEFT JOIN (@SELECT# row_number() over (partition by IdCause order by IdDocument desc) As 'RowNumber', IDCause, IDDocument from CausesDocumentsTracking with (nolock)) CDT on CDT.IDCause = ProgrammedAbsences.IDCause " &
                        " Left Join DocumentTemplates with (nolock) On DocumentTemplates.Id = CDT.IDDocument " &
                        " WHERE DailySchedule.IDEmployee= ProgrammedAbsences.IDEmployee and BeginDate <= DailySchedule.date and isnull(FinishDate,DATEADD(day, MaxLastingDays-1, BeginDate)) >= DailySchedule.date and RowNumber = 1 and DocumentTemplates.Scope = 3) "))
                        If myShiftIDBase > 0 Then
                            myShiftID = myShiftIDBase
                            myShift = roBaseEngineManager.GetShiftFromCache(myShiftID, oState)
                            bolProgrammedAbsenceOnHolidays = True
                        End If
                    End If
                Else
                    ' No hay horario este dia, usa horario vacio
                    myShift = Nothing
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetShift")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetShift")
            End Try

            Return myShift

        End Function

        Public Shared Function Execute_GetMovesLive(ByRef z As roEngineData, ByRef ListIDs As roCollection, ByRef oState As roEngineState) As Boolean
            '
            ' Obtiene fichajes marcados como de ayer, hoy y mañana
            ' y los guarda en memoria como movimientos
            Dim sSQL As String
            Dim myItem As roMoveItem
            Dim i As Integer
            Dim bolInserted As Boolean
            Dim bolret As Boolean = False

            Try

                ' Inicializamos lista de ID's de salida
                ListIDs = New roCollection

                ' Select de fichajes de presencia
                sSQL = "@SELECT# ID, ActualType, TypeData, ShiftDate, IDTerminal, DateTime FROM Punches with (nolock) WHERE IDEmployee=" & z.Employee.ToString & "" &
                        " AND (ShiftDate BETWEEN " &
                            z.ProcDate.Substract(1, "d").SQLSmallDateTime & " AND " &
                                z.ProcDate.Add(1, "d").SQLSmallDateTime & ")  AND " &
                                    " DateTime is not null AND " &
                                        " ActualType IN(1,2) ORDER BY ShiftDate, DateTime, ID"

                Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In dTbl.Rows

                        If Any2Integer(oRowEmp("ActualType")) = 1 Then
                            ' Si es una Entrada
                            ' creamos un nuevo movimiento
                            myItem = New roMoveItem
                            myItem.ID = Any2Double(oRowEmp("ID"))
                            myItem.InCause = Any2Long(oRowEmp("TypeData"))
                            myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                            myItem.InReader = Any2Long(oRowEmp("IDTerminal"))
                            myItem.Period.Begin = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                            ' Añade a la lista de movimientos
                            z.Moves.Moves.Add(myItem)
                        Else
                            ' Si es una Salida
                            ' comprobamos si el movimiento anterior tiene una entrada sin salida
                            bolInserted = False
                            If z.Moves.Moves.Count > 0 Then
                                i = z.Moves.Moves.Count - 1
                                ' Si el movimiento no tiene salida y el periodo no excede de 24 horas, lo asignamos como su salida
                                If z.Moves.Moves(i).Period.Begin.IsValid AndAlso Not z.Moves.Moves(i).Period.Finish.IsValid AndAlso
                                    (Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm")).NumericValue - z.Moves.Moves(i).Period.Begin.NumericValue) <= Any2Time("23:59").NumericValue Then
                                    z.Moves.Moves(i).Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))
                                    z.Moves.Moves(i).OutCause = Any2Long(oRowEmp("TypeData"))
                                    z.Moves.Moves(i).OutReader = Any2Long(oRowEmp("IDTerminal"))

                                    ' Guardamos el ID de la salida para posteriores actualizaciones del shiftdate
                                    ListIDs.Add("ID" & Any2String(i), Any2Double(oRowEmp("ID")))
                                    bolInserted = True
                                End If
                            End If

                            ' Si no hemos encontrado ninguno movimiento que se pueda enlazar
                            ' creamos una nuevo
                            If Not bolInserted Then
                                myItem = New roMoveItem
                                myItem.ID = oRowEmp("ID")
                                myItem.OutCause = Any2Long(oRowEmp("TypeData"))
                                myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                                myItem.OutReader = Any2Long(oRowEmp("IDTerminal"))
                                myItem.Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                                ' Añade a la lista de movimientos
                                z.Moves.Moves.Add(myItem)
                            End If
                        End If
                    Next

                    ' Recorremos todos los movimientos y los validamos
                    For i = 0 To z.Moves.Moves.Count - 1
                        ' Completa si es incompleto
                        If Not z.Moves.Moves(i).Period.Begin.IsValid Then
                            z.Moves.Moves(i).Period.Begin = z.Moves.Moves(i).Period.Finish.Substract(1, "n")
                        ElseIf Not z.Moves.Moves(i).Period.Finish.IsValid Then
                            z.Moves.Moves(i).Period.Finish = z.Moves.Moves(i).Period.Begin.Add(1, "n")
                        End If
                    Next

                End If

                ' Prepara lista de tiempos a partir de los movimientos
                z.TimeBlockList = roBaseEngineManager.InitializeBlocks(z.ProcDate, z.Moves, oState)

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetMovesLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetMovesLive")
            End Try

            Return bolret

        End Function

        Public Shared Function Execute_GetMovesLive(ByVal EmployeeID As Long, ByVal zDate As roTime, ByRef oState As roEngineState) As roMoveList
            '
            ' Obtiene fichajes marcados como de ayer, hoy y mañana
            ' y los guarda en memoria como movimientos
            Dim sSQL As String
            Dim myMoves As New roMoveList
            Dim myItem As roMoveItem
            Dim i As Integer
            Dim bolInserted As Boolean

            Try

                ' Select de fichajes de presencia
                sSQL = "@SELECT# ID, ActualType, TypeData, ShiftDate, IDTerminal, DateTime FROM Punches with (nolock) WHERE IDEmployee=" & EmployeeID.ToString & "" &
                        " AND ShiftDate = " & zDate.SQLSmallDateTime & " AND " &
                                    " DateTime is not null AND " &
                                        " ActualType IN(1,2) ORDER BY ShiftDate, DateTime, ID"

                Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In dTbl.Rows

                        If Any2Integer(oRowEmp("ActualType")) = 1 Then
                            ' Si es una Entrada
                            ' creamos un nuevo movimiento
                            myItem = New roMoveItem
                            myItem.ID = Any2Double(oRowEmp("ID"))
                            myItem.InCause = Any2Long(oRowEmp("TypeData"))
                            myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                            myItem.InReader = Any2Long(oRowEmp("IDTerminal"))
                            myItem.Period.Begin = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                            ' Añade a la lista de movimientos
                            myMoves.Moves.Add(myItem)
                        Else
                            ' Si es una Salida
                            ' comprobamos si el movimiento anterior tiene una entrada sin salida
                            bolInserted = False
                            If myMoves.Moves.Count > 0 Then
                                i = myMoves.Moves.Count - 1
                                ' Si el movimiento no tiene salida y el periodo no excede de 24 horas, lo asignamos como su salida
                                If myMoves.Moves(i).Period.Begin.IsValid AndAlso Not myMoves.Moves(i).Period.Finish.IsValid AndAlso
                                    (Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm")).NumericValue - myMoves.Moves(i).Period.Begin.NumericValue) <= Any2Time("23:59").NumericValue Then
                                    myMoves.Moves(i).Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))
                                    myMoves.Moves(i).OutCause = Any2Long(oRowEmp("TypeData"))
                                    myMoves.Moves(i).OutReader = Any2Long(oRowEmp("IDTerminal"))

                                    bolInserted = True
                                End If
                            End If

                            ' Si no hemos encontrado ninguno movimiento que se pueda enlazar
                            ' creamos una nuevo
                            If Not bolInserted Then
                                myItem = New roMoveItem
                                myItem.ID = oRowEmp("ID")
                                myItem.OutCause = Any2Long(oRowEmp("TypeData"))
                                myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                                myItem.OutReader = Any2Long(oRowEmp("IDTerminal"))
                                myItem.Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                                ' Añade a la lista de movimientos
                                myMoves.Moves.Add(myItem)
                            End If
                        End If
                    Next

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetMovesLive")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetMovesLive")
            End Try

            Return myMoves

        End Function

        Public Shared Function Execute_GetCenterPunches(ByVal EmployeeID As Long, ByVal zDate As roTime, ByVal zMoves As roMoveList, ByRef oState As roEngineState, ByVal mLastPunchCenterEnabled As String) As roMoveList
            '
            ' Obtiene fichajes de cesión de centro de coste
            '

            Dim myDS As New DataTable
            Dim sSQL As String
            Dim myMoves As New roMoveList
            Dim myItem As roMoveItem
            Dim DayBeginTime As New roTime
            Dim DayFinishTime As New roTime

            Try

                If zMoves.Moves.Count > 0 Then

                    ' Calcula limites diarios de presencia del empleado
                    DayBeginTime = zMoves.Moves(0).Period.Begin
                    DayFinishTime = zMoves.Moves(zMoves.Moves.Count - 1).Period.Finish

                    If mLastPunchCenterEnabled = "1" Then
                        ' En el caso que se deba mirar el último fichaje de centro de coste
                        ' aunque no sea del dia de cálculo
                        ' lo intentamos obtener
                        sSQL = "@SELECT# TOP 1 * FROM Punches with (nolock) WHERE " _
                            & " IDEmployee=" & EmployeeID.ToString & " AND " _
                            & " DateTime <" & DayBeginTime.SQLDateTime & " AND " _
                            & " ActualType=13 ORDER BY ShiftDate desc, DateTime desc, ID desc"
                        myDS = CreateDataTable(sSQL)
                        If myDS IsNot Nothing AndAlso myDS.Rows.Count > 0 Then
                            ' creamos un nuevo movimiento que empieza al inicio de la jornada, con el centro correspondiente
                            myItem = New roMoveItem
                            myItem.ID = myDS.Rows(0)("ID")
                            myItem.ShiftDate = Any2Time(DayBeginTime.DateOnly)
                            myItem.Period.Begin = Any2Time(Format$(myItem.ShiftDate.Value, "yyyy/MM/dd HH:mm"))
                            myItem.InCause = Any2Long(myDS.Rows(0)("TypeData"))

                            ' Añade a la lista de movimientos
                            myMoves.Moves.Add(myItem)
                        End If
                    End If

                    ' Select de movimientos
                    sSQL = "@SELECT# * FROM Punches with (nolock) WHERE " _
                        & " IDEmployee=" & EmployeeID.ToString & " AND " _
                        & " DateTime<=" & DayFinishTime.SQLDateTime & " AND DateTime >=" & DayBeginTime.SQLDateTime & " AND " _
                        & " ActualType=13 ORDER BY ShiftDate, DateTime, ID"

                    myDS = CreateDataTable(sSQL)
                    If myDS IsNot Nothing AndAlso myDS.Rows.Count > 0 Then
                        ' Obtiene movimientos
                        For Each orow As DataRow In myDS.Rows
                            ' creamos un nuevo movimiento para cada fichaje de cambio de centro de coste
                            myItem = New roMoveItem
                            myItem.ID = orow("ID")
                            myItem.ShiftDate = Any2Time(orow("ShiftDate"))
                            myItem.Period.Begin = Any2Time(Format$(orow("DateTime"), "yyyy/MM/dd HH:mm"))
                            myItem.InCause = Any2Long(orow("TypeData"))

                            ' Añade a la lista de movimientos
                            myMoves.Moves.Add(myItem)
                        Next

                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetCenterPunches")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetCenterPunches")
            End Try

            Return myMoves

        End Function

        Public Shared Function Execute_GetAttendanceMoves(ByVal EmployeeID As Long, ByVal zDate As roTime, ByRef oState As roEngineState) As roMoveList
            '
            ' Obtiene fichajes marcados como de ayer, hoy y mañana
            ' y los guarda en memoria como movimientos
            Dim sSQL As String
            Dim myMoves As New roMoveList
            Dim myItem As roMoveItem = Nothing
            Dim i As Integer
            Dim bolInserted As Boolean
            Dim sNextChangeTask As String

            Try

                ' Select de fichajes de presencia
                sSQL = "@SELECT# ID, ActualType, TypeData, ShiftDate, IDTerminal, DateTime FROM Punches with (nolock) WHERE IDEmployee=" & EmployeeID.ToString & "" &
                        " AND ShiftDate = " & zDate.SQLSmallDateTime & " AND " &
                                    " DateTime is not null AND " &
                                        " ActualType IN(1,2) ORDER BY ShiftDate, DateTime, ID"

                Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    For Each oRowEmp As DataRow In dTbl.Rows

                        If Any2Integer(oRowEmp("ActualType")) = 1 Then
                            ' Si es una Entrada
                            ' creamos un nuevo movimiento
                            myItem = New roMoveItem
                            myItem.ID = Any2Double(oRowEmp("ID"))
                            myItem.InCause = Any2Long(oRowEmp("TypeData"))
                            myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                            myItem.InReader = Any2Long(oRowEmp("IDTerminal"))
                            myItem.Period.Begin = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm:ss"))

                            ' Añade a la lista de movimientos
                            myMoves.Moves.Add(myItem)
                        Else
                            ' Si es una Salida
                            ' comprobamos si el movimiento anterior tiene una entrada sin salida
                            bolInserted = False
                            If myMoves.Moves.Count > 0 Then
                                i = myMoves.Moves.Count - 1
                                ' Si el movimiento no tiene salida y el periodo no excede de 24 horas, lo asignamos como su salida
                                If myMoves.Moves(i).Period.Begin.IsValid AndAlso Not myMoves.Moves(i).Period.Finish.IsValid AndAlso
                                    (Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm")).NumericValue - myMoves.Moves(i).Period.Begin.NumericValue) <= Any2Time("23:59").NumericValue Then
                                    myMoves.Moves(i).Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm:ss"))
                                    myMoves.Moves(i).OutCause = Any2Long(oRowEmp("TypeData"))
                                    myMoves.Moves(i).OutReader = Any2Long(oRowEmp("IDTerminal"))

                                    bolInserted = True
                                End If
                            End If

                            ' Si no hemos encontrado ninguno movimiento que se pueda enlazar
                            ' creamos una nuevo
                            If Not bolInserted Then
                                myItem = New roMoveItem
                                myItem.ID = oRowEmp("ID")
                                myItem.OutCause = Any2Long(oRowEmp("TypeData"))
                                myItem.ShiftDate = Any2Time(oRowEmp("ShiftDate"))
                                myItem.OutReader = Any2Long(oRowEmp("IDTerminal"))
                                myItem.Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm:ss"))

                                ' Añade a la lista de movimientos
                                myMoves.Moves.Add(myItem)
                            End If
                        End If
                    Next

                    ' Recorremos todos los movimientos y los validamos

                    If myItem IsNot Nothing AndAlso myItem.Period.Begin.IsValid Then
                        i = 0
                        While i <= myMoves.Moves.Count - 1
                            ' Elimina los mov. que solo tienen salida
                            ' Completa los mov. que les falta la salida en caso que sean de hoy
                            If Not myMoves.Moves(i).Period.IsValid Then
                                If myMoves.Moves(i).Period.Begin.IsValid AndAlso Any2Time(myMoves.Moves(i).Period.Begin.Value).DateOnly = Any2Time(Now.Date).DateOnly Then
                                    'Buscamos el siguiente cambio de tarea posterior a la entrada
                                    sSQL = "@SELECT# TOP 1 DateTime FROM Punches with (nolock) WHERE IDEmployee=" & EmployeeID & " AND ActualType = 4 AND DateTime >  " & myItem.Period.Begin.SQLDateTime & " AND DateTime < " & Any2Time(zDate.DateOnly).Add(1, "d").SQLDateTime & " Order by DateTime desc "
                                    sNextChangeTask = Any2String(ExecuteScalar(sSQL))
                                    ' Asignamos la hora de inicio de la tarea como salida del mov.
                                    ' para poder calcular el tiempo de tarea
                                    If IsDate(sNextChangeTask) Then
                                        myMoves.Moves(i).Period.Finish = Any2Time(Format$(CDate(sNextChangeTask), "yyyy/MM/dd HH:mm:ss"))
                                        i = i + 1
                                    Else
                                        myMoves.Moves.RemoveAt(i)
                                    End If
                                Else
                                    myMoves.Moves.RemoveAt(i)
                                End If
                            Else
                                i = i + 1
                            End If
                        End While
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetAttendanceMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetAttendanceMoves")
            End Try

            Return myMoves

        End Function

        Public Shared Function Execute_GetEmployeeTaskMoves(ByVal EmployeeID As Long, ByVal DayBeginTime As roTime, ByVal DayFinishTime As roTime, ByRef oState As roEngineState) As Generic.List(Of roTaskMoveItem)
            '
            ' Obtiene movimientos de tareas del empleado que afecten a esa fecha
            '
            Dim bRet As New Generic.List(Of roTaskMoveItem)
            Dim sSQL As String
            Dim myItem As roTaskMoveItem
            Dim i As Integer
            Dim myItemAux As roTaskMoveItem

            Try

                If DayBeginTime.IsValid AndAlso DayFinishTime.IsValid Then
                    ' Select de fichajes de productiV
                    sSQL = "@SELECT# * FROM Punches with (nolock) WHERE " _
                                & "IDEmployee=" & EmployeeID.ToString & " AND " _
                                & "ActualType = 4  AND " _
                                & "DateTime<=" & DayFinishTime.SQLDateTime & " AND DateTime >=" & DayBeginTime.SQLDateTime & " AND " _
                                & "TypeData is NOT Null  " _
                                & "ORDER BY Datetime, ID"

                    Dim dTbl As System.Data.DataTable = CreateDataTable(sSQL)
                    If dTbl IsNot Nothing Then
                        For Each oRowEmp As DataRow In dTbl.Rows
                            ' Crea nuevo movimiento con el fichaje de inicio
                            myItem = New roTaskMoveItem

                            myItem.ID = oRowEmp("ID")
                            myItem.IDEmployee = oRowEmp("IDEmployee")
                            myItem.IDJob = oRowEmp("TypeData")
                            myItem.Period.Begin = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                            myItem.Field1 = Any2String(oRowEmp("Field1"))
                            myItem.Field2 = Any2String(oRowEmp("Field2"))
                            myItem.Field3 = Any2String(oRowEmp("Field3"))
                            myItem.Field4 = Any2Double(oRowEmp("Field4"))
                            myItem.Field5 = Any2Double(oRowEmp("Field5"))
                            myItem.Field6 = Any2Double(oRowEmp("Field6"))

                            ' Si es el primero y empieza despues de la primera entrada
                            ' hay que crear otro movimiento con la tarea anterior
                            ' desde el inicio de presencia hasta el primer cambio de tarea
                            If bRet.Count = 0 AndAlso myItem.Period.Begin.VBNumericValue > DayBeginTime.VBNumericValue Then
                                myItemAux = New roTaskMoveItem
                                myItemAux.IDEmployee = oRowEmp("IDEmployee")

                                sSQL = "@SELECT# TOP 1 TypeData FROM Punches with (nolock) WHERE " _
                                    & "IDEmployee=" & EmployeeID.ToString & " AND " _
                                    & "ActualType = 4  AND " _
                                    & "DateTime<" & Any2Time(oRowEmp("DateTime")).SQLDateTime & " AND " _
                                    & "TypeData is NOT Null  " _
                                    & "ORDER BY Datetime desc, ID desc"

                                myItemAux.IDJob = Any2Double(ExecuteScalar(sSQL))
                                myItemAux.Period.Begin = Any2Time(Format$(CDate(DayBeginTime.Value), "yyyy/MM/dd HH:mm"))
                                myItemAux.Period.Finish = Any2Time(Format$(oRowEmp("DateTime"), "yyyy/MM/dd HH:mm"))

                                If myItemAux.IDJob > 0 Then
                                    bRet.Add(myItemAux)
                                End If
                            End If

                            bRet.Add(myItem)
                        Next

                        ' 02.Asignamos como final del periodo el siguiente inicio
                        For i = 0 To bRet.Count - 1
                            If i = bRet.Count - 1 Then
                                ' Si es el ultimo asignamos el final de presencia
                                If bRet(i).Period.Begin.VBNumericValue <= DayFinishTime.VBNumericValue Then
                                    bRet(i).Period.Finish = Any2Time(Format$(CDate(DayFinishTime.Value), "yyyy/MM/dd HH:mm"))
                                End If
                            Else
                                bRet(i).Period.Finish = bRet(i + 1).Period.Begin
                            End If
                        Next i

                        ' 03.Recorremos todos los movimientos y eliminamos los inválidos
                        i = 0
                        While i <= bRet.Count - 1
                            If bRet(i).Period.IsValid AndAlso bRet(i).IDJob > 0 Then
                                i = i + 1
                            Else
                                bRet.RemoveAt(i)
                            End If
                        End While

                        ' 04.En el caso que no haya ningun fichaje de tareas
                        ' debemos revisar si existen fichajes de dias anteriores
                        ' y crear un mov. con la ultima tarea fichada
                        If bRet.Count = 0 Then
                            sSQL = "@SELECT# TOP 1 TypeData FROM Punches with (nolock) WHERE " _
                                & "IDEmployee=" & EmployeeID.ToString & " AND " _
                                & "ActualType = 4  AND " _
                                & "DateTime<" & DayBeginTime.SQLDateTime & " AND " _
                                & "TypeData is NOT Null  " _
                                & "ORDER BY Datetime desc, ID desc"

                            myItemAux = New roTaskMoveItem
                            myItemAux.IDJob = Any2Double(ExecuteScalar(sSQL))
                            myItemAux.IDEmployee = EmployeeID
                            myItemAux.Period.Begin = Any2Time(Format$(CDate(DayBeginTime.Value), "yyyy/MM/dd HH:mm"))
                            myItemAux.Period.Finish = Any2Time(Format$(CDate(DayFinishTime.Value), "yyyy/MM/dd HH:mm"))
                            If myItemAux.IDJob > 0 Then
                                bRet.Add(myItemAux)
                            End If
                        End If
                    End If
                Else
                    Debug.Print("roBaseEngineManager::Execute_GetEmployeeTaskMoves: Info: Invalid Period")
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEmployeeTaskMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEmployeeTaskMoves")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_GetEfectiveEmployeeMoveswithoutBreakLayers(ByRef EmployeeMoves As roMoveList, ByVal mBreakLayers As roTimeBlockList, ByRef oState As roEngineState) As Boolean
            '
            ' Recortamos los fichajes de presencia en funcion de los descansos
            '

            Dim bRet As Boolean = True
            Dim myMove As roMoveItem
            Dim i As Integer
            Dim TimeBlocks As New roTimeBlockList
            Dim FirstIndex As Integer
            Dim MovesPres As New roTimeBlockItem

            Try

                If mBreakLayers.Count = 0 Then
                    Return True
                End If

                ' Creamos una lista de periodos de trabajo, con los fichajes de presencia
                For i = 0 To EmployeeMoves.Moves.Count - 1
                    MovesPres = New roTimeBlockItem
                    MovesPres.Period.Begin = EmployeeMoves.Moves(i).Period.Begin
                    MovesPres.Period.Finish = EmployeeMoves.Moves(i).Period.Finish
                    MovesPres.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                    TimeBlocks.Add(MovesPres)
                Next i

                'Cambiamos periodos de trabajo por las franjas de descanso
                For Index = 1 To mBreakLayers.Count
                    mBreakLayers.Item(Index).BlockType = roTimeBlockItem.roBlockType.roBTBreak
                    TimeBlocks.Replace(mBreakLayers.Item(Index), roTimeBlockItem.roBlockType.roBTWorking)
                Next Index

                ' Eliminamos los bloques de descanso que se hayan generado
                FirstIndex = 1
                While FirstIndex <= TimeBlocks.Count
                    If TimeBlocks.Item(FirstIndex).BlockType = roTimeBlockItem.roBlockType.roBTBreak Then
                        TimeBlocks.Remove(FirstIndex)
                    Else
                        FirstIndex = FirstIndex + 1
                    End If
                End While

                ' Eliminamos todos los movimientos antiguos
                FirstIndex = 0
                While FirstIndex <= EmployeeMoves.Moves.Count - 1
                    EmployeeMoves.Moves.RemoveAt(FirstIndex)
                End While

                ' Volvemos a crear los nuevos movimientos. despues de eliminar los periodos de descanso
                For i = 1 To TimeBlocks.Count
                    myMove = New roMoveItem
                    myMove.Period.Begin = TimeBlocks.Item(i).Period.Begin
                    myMove.Period.Finish = TimeBlocks.Item(i).Period.Finish
                    EmployeeMoves.Moves.Add(myMove)
                Next i

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEfectiveEmployeeMoveswithoutBreakLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEfectiveEmployeeMoveswithoutBreakLayers")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_SetEmployeeTaskMovesGroupbyFields(ByRef TaskMoveList As Generic.List(Of roTaskMoveItem), ByRef oState As roEngineState) As Boolean
            '
            ' Agrupamos los fichajes segun los campos de la ficha introducidos
            '

            Dim bRet As Boolean = True
            Dim Index As Integer
            Dim Index2 As Integer
            Dim NextTaskMove As New roTaskMoveItem
            Dim bolExit As Boolean

            Try

                ' Agrupa los tiempos que sean iguales (misma tarea)
                Index = 0
                While Index <= TaskMoveList.Count - 1

                    ' Si el movimiento no tiene asignado ningun campo de la ficha
                    ' miramos si podemos enlazarlo con alguno posterior
                    If Any2String(TaskMoveList(Index).Field1) = "" AndAlso Any2String(TaskMoveList(Index).Field2) = "" AndAlso Any2String(TaskMoveList(Index).Field3) = "" AndAlso TaskMoveList(Index).Field4 = 0 AndAlso TaskMoveList(Index).Field5 = 0 AndAlso TaskMoveList(Index).Field6 = 0 Then
                        With TaskMoveList(Index)
                            Index2 = Index + 1
                            bolExit = False
                            While Index2 <= TaskMoveList.Count - 1 AndAlso Not bolExit
                                NextTaskMove = New roTaskMoveItem
                                NextTaskMove = TaskMoveList(Index2)

                                ' Si el siguiente movimiento tiene algun campo asignado lo asignamos tambien al actual
                                If Any2String(NextTaskMove.Field1) <> "" OrElse Any2String(NextTaskMove.Field2) <> "" OrElse Any2String(NextTaskMove.Field3) <> "" Then
                                    TaskMoveList(Index).Field1 = NextTaskMove.Field1
                                    TaskMoveList(Index).Field2 = NextTaskMove.Field2
                                    TaskMoveList(Index).Field3 = NextTaskMove.Field3
                                    bolExit = True
                                Else
                                    Index2 = Index2 + 1
                                End If
                            End While
                        End With
                    End If

                    Index = Index + 1
                End While

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_SetEmployeeTaskMovesGroupbyFields")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_SetEmployeeTaskMovesGroupbyFields")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_SetShiftlimits(ByVal oShift As roShiftEngine, ByVal zDate As roTime, ByVal zEmployee As Long, ByRef oState As roEngineState) As roShiftEngine
            '
            ' Asignamos los limites del horario en función del tipo
            '
            Dim bRet As roShiftEngine = Nothing

            Try
                If oShift IsNot Nothing Then
                    bRet = New roShiftEngine With {
                    .ID = oShift.ID,
                    .StartLimit = oShift.StartLimit,
                    .EndLimit = oShift.EndLimit
                }
                    ' En el caso que el horario sea de tipo flotante
                    If oShift.ShiftType = ShiftType.NormalFloating Then
                        ' Obtenemos la hora de inicio planificada
                        Dim FloatingTimeShift As Double = Any2Time(ExecuteScalar("@SELECT# StartShift1 FROM DailySchedule with (nolock)  WHERE IdEmployee = " & zEmployee.ToString & " AND Date = " & zDate.SQLSmallDateTime), True).NumericValue(True)

                        ' Hora inicio base
                        Dim zStartFloating As roTime = Any2Time(oShift.StartFloating, True)
                        Dim zBaseFloatingTime As Double = zStartFloating.NumericValue(True)

                        ' Obtenemos tiempo a desplazar
                        FloatingTimeShift = FloatingTimeShift - zBaseFloatingTime

                        bRet.StartLimit = Any2Time(bRet.StartLimit).Add(Any2Time(FloatingTimeShift, True).Value).ValueDateTime
                        bRet.EndLimit = Any2Time(bRet.EndLimit).Add(Any2Time(FloatingTimeShift, True).Value).ValueDateTime
                    End If
                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_SetShiftlimits")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_SetShiftlimits")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_AddEmployeeAtomicAccruals(ByRef AtomicAccruals As roCollection, ByRef AttendanceMoves As roMoveList, ByRef EmployeeTaskMoves As Generic.List(Of roTaskMoveItem), ByVal EmployeeID As Long, ByVal BeginTime As roTime, ByVal FinishTime As roTime, ByRef oState As roEngineState) As Boolean
            '
            ' Obtiene tiempos parciales de fichajes individuales
            '
            ' Para obtener los tiempos parciales de fichajes individuales, intersecciona los
            ' movimientos de presencia del empleado con los de tareas del empleado.

            Dim bRet As Boolean = True
            Dim myTaskMove As roTaskMoveItem
            Dim myNewAccrualItem As roTaskAccrualItem

            Try

                ' Para cada fichaje de tareas, intersecciona con todos los de presencia
                For Each myTaskMove In EmployeeTaskMoves
                    For i = 0 To AttendanceMoves.Moves.Count - 1
                        If myTaskMove.IDJob = 0 Then
                            'si no tiene asignado una tarea
                            'no hacemos nada, el mov. no es coherente
                            '
                        ElseIf myTaskMove.Period.Finish.VBNumericValue < AttendanceMoves.Moves(i).Period.Begin.VBNumericValue Then
                            ' Este movimiento de presencia (y los siguientes) son posteriores al de tareas,
                            '  salimos del bucle
                            Exit For
                        ElseIf myTaskMove.Period.Begin.VBNumericValue <= AttendanceMoves.Moves(i).Period.Finish.VBNumericValue Then
                            ' Estos movimientos interseccionan
                            myNewAccrualItem = New roTaskAccrualItem
                            myNewAccrualItem.IDJob = myTaskMove.IDJob
                            myNewAccrualItem.Field1 = myTaskMove.Field1
                            myNewAccrualItem.Field2 = myTaskMove.Field2
                            myNewAccrualItem.Field3 = myTaskMove.Field3
                            myNewAccrualItem.Field4 = myTaskMove.Field4
                            myNewAccrualItem.Field5 = myTaskMove.Field5
                            myNewAccrualItem.Field6 = myTaskMove.Field6

                            myNewAccrualItem.Period.Begin = roConversions.Max(myTaskMove.Period.Begin, AttendanceMoves.Moves(i).Period.Begin)
                            myNewAccrualItem.Period.Finish = roConversions.Min(myTaskMove.Period.Finish, AttendanceMoves.Moves(i).Period.Finish)

                            'Calcula el tiempo trabajando utilizando
                            myNewAccrualItem.TimeValue = Any2Time((myNewAccrualItem.Period.Finish.NumericValue(True) - myNewAccrualItem.Period.Begin.NumericValue(True)), True)

                            If myNewAccrualItem.TimeValue.NumericValue > 0 Or ((myNewAccrualItem.Field6 <> 0 Or myNewAccrualItem.Field5 <> 0 Or myNewAccrualItem.Field4 <> 0 Or Any2String(myNewAccrualItem.Field1) <> "" Or Any2String(myNewAccrualItem.Field2) <> "" Or Any2String(myNewAccrualItem.Field3) <> "") And myNewAccrualItem.TimeValue.NumericValue >= 0) Then
                                AtomicAccruals.Add(AtomicAccruals.Count, myNewAccrualItem)
                            End If
                        End If
                    Next
                Next

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_AddEmployeeAtomicAccruals")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_AddEmployeeAtomicAccruals")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_GetEfectiveEmployeeMoves(ByRef EmployeeMoves As roMoveList, ByVal FilterMoves As roMoveList, ByRef oState As roEngineState) As Boolean
            '
            ' Obtiene todos los filtros de tareas
            '
            Dim bRet As Boolean = True
            Dim myFilterMove As roMoveItem
            Dim i As Integer

            Try

                '
                ' Recortamos los fichajes de presencia en funcion de los filtros
                '
                For Each myFilterMove In FilterMoves.Moves
                    For i = 0 To EmployeeMoves.Moves.Count - 1
                        ' Para cada mov. de presencia
                        If myFilterMove.ID = 1 Then
                            If EmployeeMoves.Moves.Item(i).Period.Begin.VBNumericValue >= myFilterMove.Period.Begin.VBNumericValue And EmployeeMoves.Moves.Item(i).Period.Begin.VBNumericValue <= myFilterMove.Period.Finish.VBNumericValue Then
                                EmployeeMoves.Moves.Item(i).Period.Begin = myFilterMove.Period.Finish
                                If Not EmployeeMoves.Moves.Item(i).Period.IsValid Then
                                    EmployeeMoves.Moves.RemoveAt(i)
                                End If
                            End If
                        Else
                            If EmployeeMoves.Moves.Item(i).Period.Finish.VBNumericValue >= myFilterMove.Period.Begin.VBNumericValue And EmployeeMoves.Moves.Item(i).Period.Finish.VBNumericValue <= myFilterMove.Period.Finish.VBNumericValue Then
                                EmployeeMoves.Moves.Item(i).Period.Finish = myFilterMove.Period.Begin
                                If Not EmployeeMoves.Moves.Item(i).Period.IsValid Then
                                    EmployeeMoves.Moves.RemoveAt(i)
                                End If
                            End If
                        End If
                    Next i
                Next

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEfectiveEmployeeMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetEfectiveEmployeeMoves")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_GetTaskShiftFilter(ByVal zDate As roTime, ByVal zShift As roShiftEngine, ByRef FilterMoves As roMoveList, ByRef mBreakLayers As roTimeBlockList, ByRef oState As roEngineState) As Boolean
            '
            ' Obtiene todos los filtros de tareas
            '
            Dim bRet As Boolean = True
            Dim oShiftLayersList As New Generic.List(Of roShiftEngineLayer)

            Try

                If zShift.AdvancedParameters.Length > 0 Then
                    FilterMoves = roBaseEngineManager.Execute_GetFilterMoves(zDate, zShift.AdvancedParameters, oState)
                End If

                ' Ordenamos las franjas por tipo
                oShiftLayersList = roBaseEngineManager.ProcessLayer_OrderLayersByType(zShift)
                For Each xShiftLayer In oShiftLayersList
                    Select Case xShiftLayer.LayerType
                        Case roLayerTypes.roLTBreak
                            ' Obtenemos el periodo de descanso
                            Dim Layer As New roTimeBlockItem
                            Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, xShiftLayer.Data("Begin")), Any2Time(0).Value))
                            Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, xShiftLayer.Data("Finish")), Any2Time(0).Value))
                            Layer.BlockType = roTimeBlockItem.roBlockType.roBTBreak

                            'El minimo tiene que coincidir con el periodo de descanso
                            'para que sea valido en tareas
                            If Any2Double(xShiftLayer.Data("MinBreakTime")) = Layer.Period.PeriodTime.VBNumericValue Then
                                mBreakLayers.Add(Layer)
                            End If
                    End Select
                Next

                bRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetTaskShiftFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetTaskShiftFilter")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_GetFilterMoves(ByVal zDate As roTime, ByVal sParameters As String, ByRef oState As roEngineState) As roMoveList
            '
            ' Obtiene todos los filtros de tareas
            '
            Dim bRet As New roMoveList
            Dim myItem As roMoveItem
            Dim sBegin As String
            Dim sEnd As String
            Dim sParam As String
            Dim SFunc As String
            Dim mLen As Integer
            Dim i As Integer

            Try

                'FORMATO
                'FILTERJOBXX=IN;XX:XX;XX:XX
                'FILTERJOBXX=OUT;XX:XX;XX:XX

                i = 1
                While InStr(UCase$(sParameters), "FILTERJOB" & Format$(i, "00")) > 0
                    SFunc = Mid(sParameters, InStr(UCase$(sParameters), "FILTERJOB" & Format$(i, "00")) + 12, 3)
                    If SFunc = "OUT" Then
                        mLen = 15
                    Else
                        SFunc = "IN"
                        mLen = 14
                    End If
                    sParam = Mid(sParameters, InStr(UCase$(sParameters), "FILTERJOB" & Format$(i, "00")) + 12, mLen)
                    sBegin = String2Item(sParam, 1, ";")
                    sBegin = $"{Format$(zDate.DateOnly, "yyyy/MM/dd")} {sBegin}"
                    sEnd = String2Item(sParam, 2, ";")
                    sEnd = $"{Format$(zDate.DateOnly, "yyyy/MM/dd")} {sEnd}"

                    If IsDate(sBegin) AndAlso IsDate(sEnd) Then
                        If DateTime2Double(sBegin) > DateTime2Double(sEnd) Then
                            sEnd = Any2Time(sEnd).Add(1, "d").Value
                        End If

                        myItem = New roMoveItem
                        myItem.Period.Begin = Any2Time(sBegin)
                        myItem.Period.Finish = Any2Time(sEnd)

                        If myItem.Period.IsValid Then
                            If SFunc = "IN" Then
                                myItem.ID = 1
                            Else
                                myItem.ID = 2
                            End If
                            bRet.Moves.Add(myItem)
                        End If
                    End If
                    i = i + 1
                End While
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetFilterMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_GetFilterMoves")
            End Try

            Return bRet

        End Function

        Public Shared Function Execute_ProcessMoves(ByRef zMoves As roMoveList, ByVal zDate As roTime, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa movimientos
            '  Intenta completar los incompletos de alguna forma o simplemente quitarlos
            '  para que el proceso del calculador se pueda realizar, aunque sea con
            '  datos "supuestos", que siempre es mejor que no calcular nada de nada.
            Dim bolRet As Boolean = False
            Dim Index As Integer

            Try

                Index = 0
                While Index <= zMoves.Moves.Count - 1
                    If Not zMoves.Moves(Index).Period.IsValid Then
                        zMoves.Moves.RemoveAt(Index)
                    Else
                        Index = Index + 1
                    End If
                End While

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessMoves")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessMoves")
            End Try

            Return bolRet

        End Function

        Public Shared Function ProcessLayers(ByVal zDate As roTime, ByRef zShift As roShiftEngine, ByVal TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByVal IDEmployee As Long, ByRef oState As roEngineState, Optional ByVal zForeCast As roTimeBlockList = Nothing) As Boolean
            '
            ' Procesa capas horarias para una fecha y horario y devuelve lista de tiempos.
            '
            Dim bolRet As Boolean = False
            Dim LayerMandatory As New roTimeBlockItem
            Dim oMandatoryLayerList As New roCollection
            Dim oShiftLayersList As New Generic.List(Of roShiftEngineLayer)

            Dim oXMLLayerDefinition As String = ""

            Try

                ' Si no hay horario, no hay capas, devolvemos all como extras
                If zShift Is Nothing Then
                    Return True
                End If

                ' Ordenamos las franjas por tipo
                oShiftLayersList = ProcessLayer_OrderLayersByType(zShift)

                ' Ahora procesamos todas las capas por prioridad segun el tipo
                For Each xShiftLayer In oShiftLayersList
                    Select Case xShiftLayer.LayerType
                        Case roLayerTypes.roLTWorking
                            ' Procesa capa de permitidas OK
                            bolRet = ProcessLayer_Working(zDate, xShiftLayer, TimeBlockList, oState, FloatingTime, zShift, IDEmployee)
                        Case roLayerTypes.roLTMandatory
                            ' Procesa capa de obligadas OK
                            bolRet = ProcessLayer_Mandatory(zDate, xShiftLayer, TimeBlockList, FloatingTime, zForeCast, zShift, IDEmployee, oState, LayerMandatory, oXMLLayerDefinition)
                            ' Nos guardamos el id de la franja y el periodo resultante de rígida
                            ' en caso que el horario tenga complementarias
                            If zShift.AllowComplementary Then oMandatoryLayerList.Add(xShiftLayer.ID.ToString, LayerMandatory)
                        Case roLayerTypes.roLTBreak
                            ' Procesa capa de descansos
                            bolRet = ProcessLayer_Break(zDate, xShiftLayer, TimeBlockList, oState, FloatingTime, oMandatoryLayerList)
                        Case roLayerTypes.roLTPaidTime
                            ' Procesa tiempo abonado
                            bolRet = ProcessLayer_PaidTime(zDate, xShiftLayer, TimeBlockList, FloatingTime, oState, oMandatoryLayerList)
                        Case roLayerTypes.roLTUnitFilter
                            ' Procesa filtro unitario OK
                            bolRet = ProcessLayer_UnitFilter(zDate, xShiftLayer, TimeBlockList, FloatingTime, oState)
                        Case roLayerTypes.roLTGroupFilter
                            ' Procesa filtro por grupo o total
                            bolRet = ProcessLayer_GroupFilter(zDate, xShiftLayer, TimeBlockList, FloatingTime, oState)
                        Case roLayerTypes.roLTWorkingMaxMinFilter
                            ' Procesa capa de filtro de max/min en permitidas OK
                            bolRet = ProcessLayer_WorkingMaxMinFilter(zDate, xShiftLayer, TimeBlockList, FloatingTime, oState, zShift, IDEmployee)
                        Case roLayerTypes.roLTDailyTotalsFilter
                            ' Procesa capa de diferencias positivas/negativas
                            bolRet = ProcessLayer_DailyTotalsFilter(zDate, xShiftLayer, TimeBlockList, zShift, FloatingTime, oState)
                        Case Else
                            ' Otras capas simplemente no hacemos nada (no son para nosotros)
                            bolRet = True
                    End Select
                    If Not bolRet Then
                        Return bolRet
                    End If
                Next

                ' En el caso que el horario tenga complementarias
                ' Aplicamos la configuracion que corresponda para cada franja rígida
                If zShift.AllowComplementary AndAlso oMandatoryLayerList.Count > 0 Then
                    For Index = 1 To oMandatoryLayerList.Count
                        ' Generamos horas complementarias en caso necesario OK
                        bolRet = ProcessLayer_Mandatory_Complementary(zDate, Any2Double(oMandatoryLayerList.Key(Index)), oMandatoryLayerList.Item(Index, roCollection.roSearchMode.roByIndex), TimeBlockList, zShift, IDEmployee, oState, oXMLLayerDefinition)
                        If Not bolRet Then
                            Return bolRet
                        End If
                    Next Index
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayers")
            End Try

            Return bolRet

        End Function

        Public Shared Function ProcessLayer_Mandatory(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByVal zForeCast As roTimeBlockList, ByRef zShift As roShiftEngine, ByVal IDEmployee As Long, ByRef oState As roEngineState, ByRef LayerMandatory As roTimeBlockItem, ByRef oXMLLayerDefinition As String) As Boolean
            '
            ' Procesa una capa de horas obligadas o rigidas
            '
            Dim bolRet As Boolean = False
            Dim IncidenceList As roTimeBlockList
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim oXML As String = ""
            Dim oCollection As New roCollection
            Dim dblLayer As Double = 0

            Try

                ' Obtenemos el periodo de obligadas
                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))

                ' Mira si hay entrada flotante
                If ShiftLayer.Data.Exists("FloatingBeginUpto") Then
                    ' Hay entrada flotante, mira cuando ha entrado
                    Layer.Period.Begin = ProcessLayer_Mandatory_GetFloatingBegin(Layer.Period.Begin, Any2Time(DateTimeAdd(zDate.Value, ShiftLayer.Data("FloatingBeginUpto"))), TimeBlockList, zForeCast, zShift, oState)
                End If

                ' Si tiene hora de inicio variable
                If ShiftLayer.Data.Exists("AllowModifyIniHour") Then
                    oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                    If Len(oXML) > 0 Then
                        Try
                            oCollection.LoadXMLString(oXML)
                            If Err.Number = 0 Then
                                ' Obtenemos la hora de la franja correspondiente
                                For i = 1 To 2
                                    If oCollection.Exists("LayerID_" & i.ToString) AndAlso Any2Double(ShiftLayer.ID) = Any2Double(oCollection("LayerID_" & i)) Then
                                        dblLayer = i
                                        Exit For
                                    End If
                                Next i
                                If dblLayer > 0 AndAlso oCollection.Exists("LayerFloatingBeginTime_" & dblLayer) Then
                                    ' Asignamos la hora de inicio
                                    Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, oCollection("LayerFloatingBeginTime_" & dblLayer)), Any2Time(FloatingTime, True).Value))
                                End If
                            End If
                        Catch ex As Exception
                            ' No hacemos nada, la excepción se produce si el XML no es correcto
                        End Try
                    End If
                End If

                If ShiftLayer.Data.Exists("FloatingFinishMinutes") Then
                    ' Si tambien hay salida flotante, establece en funcion de la entrada
                    Layer.Period.Finish = Layer.Period.Begin.Add(ShiftLayer.Data("FloatingFinishMinutes"), "n")
                End If

                ' Si tiene duracion variable a partir de la entrada
                If ShiftLayer.Data.Exists("AllowModifyDuration") Then
                    If Len(oXML) = 0 Then oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                    If Len(oXML) > 0 Then
                        Try
                            oCollection.LoadXMLString(oXML)
                            ' Obtenemos la hora de la franja correspondiente
                            For i = 1 To 2
                                If oCollection.Exists("LayerID_" & i.ToString) AndAlso Any2Double(ShiftLayer.ID) = Any2Double(oCollection("LayerID_" & i.ToString)) Then
                                    dblLayer = i
                                    Exit For
                                End If
                            Next i
                            If dblLayer > 0 AndAlso oCollection.Exists("LayerFloatingDuration_" & dblLayer.ToString) Then
                                ' Asignamos la hora de finalizacion
                                Layer.Period.Finish = Layer.Period.Begin.Add(oCollection("LayerFloatingDuration_" & dblLayer.ToString), "n")
                            End If
                        Catch ex As Exception
                            ' No hacemos nada, la excepción se produce si el XML no es correcto
                        End Try
                    End If
                End If

                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue

                ' Nos guardamos el periodo real de la franja rígida
                LayerMandatory = New roTimeBlockItem
                LayerMandatory.Period.Begin = Layer.Period.Begin
                LayerMandatory.Period.Finish = Layer.Period.Finish

                ' Cachea datos
                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                If Layer.Period.Finish.NumericValue(True) - Layer.Period.Begin.NumericValue(True) > 0 Then
                    ' Cambiamos extras por trabajadas en el intervalo rigido
                    TimeBlockList.Replace(Layer, roTimeBlockItem.roBlockType.roBTOverworking)
                End If

                ' Ahora generamos incidencias de llegada tardía, interrupción y salida anticipada
                IncidenceList = TimeBlockList.GetComplementary(Layer.Period, roTimeBlockItem.roBlockType.roBTAnyAttendance, roTimeBlockItem.roBlockType.roBTAny)

                ' Ahora fijamos el tipo de cada incidencia entre las tres posibles
                For index As Integer = 1 To IncidenceList.Count
                    If IncidenceList.Item(index).Period.Begin.VBNumericValue = LayerBeginValue Then
                        If IncidenceList.Item(index).Period.Finish.VBNumericValue = LayerFinishValue Then
                            ' Añade incidencia de ausencia de all el periodo de obligadas
                            IncidenceList.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTAbsence
                            TimeBlockList.Add(IncidenceList.Item(index))
                        Else
                            ' Añade incidencia de Llegada tardia
                            IncidenceList.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTLateArrival
                            TimeBlockList.Add(IncidenceList.Item(index))
                        End If
                    Else
                        If IncidenceList.Item(index).Period.Finish.VBNumericValue = LayerFinishValue Then
                            ' Añade incidencia de Salida anticipada
                            IncidenceList.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTEarlyLeave
                            TimeBlockList.Add(IncidenceList.Item(index))
                        Else
                            ' Añade incidencia de Interrupción
                            IncidenceList.Item(index).BlockType = roTimeBlockItem.roBlockType.roBTUnexpectedBreak
                            TimeBlockList.Add(IncidenceList.Item(index))
                        End If
                    End If
                Next

                oXMLLayerDefinition = oXML

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory")
            End Try

            Return bolRet

        End Function

        Public Shared Function ProcessLayer_Working(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByRef oState As roEngineState, ByVal FloatingTime As Double, ByRef zShift As roShiftEngine, ByVal IDEmployee As Long) As Boolean
            '
            ' Procesa una capa de horas permitidas o flexibles
            '
            Dim Layer As New roTimeBlockItem
            Dim bolRet As Boolean = False
            Dim StartHour As Double
            Dim EndHour As Double
            Dim DateTime As New roTime

            Try

                ' Obtenemos el periodo de permitidas
                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value)) 'Any2Time(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")))
                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))
                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking

                ' Si es un horario Starter
                If zShift.AdvancedParameters.Contains("Starter=[1]") Then
                    ' Debemos ajustar los limites a los valores del DailySchedule
                    Dim StarterDs As System.Data.DataTable = CreateDataTable("@SELECT# StartFlexible1,EndFlexible1 FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime)
                    If StarterDs IsNot Nothing AndAlso StarterDs.Rows.Count > 0 AndAlso Not IsDBNull(StarterDs.Rows(0)("StartFlexible1")) AndAlso Not IsDBNull(StarterDs.Rows(0)("EndFlexible1")) Then
                        StartHour = Any2Time(StarterDs.Rows(0)("StartFlexible1"), True).NumericValue
                        EndHour = Any2Time(StarterDs.Rows(0)("EndFlexible1"), True).NumericValue
                        If StartHour <> 0 OrElse EndHour <> 0 Then
                            DateTime = zDate
                            Layer.Period.Begin = DateTime.Add(Any2Time(StartHour, True))
                            Layer.Period.Finish = DateTime.Add(Any2Time(EndHour, True))
                        End If

                    End If
                End If

                ' Ahora ya tenemos la lista de horas trabajadas, reemplazamos las horas extras
                '  originales por las horas trabajadas.
                TimeBlockList.Replace(Layer, roTimeBlockItem.roBlockType.roBTOverworking)

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Working")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Working")
            End Try

            Return bolRet

        End Function

        Public Shared Function ProcessLayer_PaidTime(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByRef oState As roEngineState, ByVal oMandatoryLayerList As roCollection) As Boolean
            '
            ' Procesa una capa de tiempo abonado
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim FilterTarget As Long
            Dim NewAbsence As roTimeBlockItem
            Dim bolRet As Boolean = False
            Dim FirstLayerMandatory As New roTimeBlockItem

            Try
                ' Obtenemos el periodo del filtro y sus detalles
                ' En el caso que el horario sea por horas y tenga definido el descano por duracion, aplicamos el periodo correspondiente
                ' Aplicamos la configuracion que corresponda para cada franja rígida
                If oMandatoryLayerList.Count > 0 AndAlso Any2Time(Any2Time(ShiftLayer.Data("Begin")).DateOnly).Value = Any2Time("1899/12/01").Value Then
                    ' Obtenemos la primera franja obligada
                    For index = 1 To oMandatoryLayerList.Count
                        If index = 1 Then
                            FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                        Else
                            If FirstLayerMandatory.Period.Begin.NumericValue > oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex).Period.Begin.NumericValue Then
                                FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                            End If
                        End If
                    Next index

                    ' Sobre la primera franja sumamos las duraciones para crear el periodo de descanso
                    Layer.Period.Begin = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Begin")).TimeOnly))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Finish")).TimeOnly))
                Else
                    Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))
                End If

                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue
                Layer.TimeValue = Any2Time(ShiftLayer.Data("Value"))
                FilterTarget = Any2Long(ShiftLayer.Data("Target"))

                ' Ahora debemos convertir el tipo de tiempo que nos indica el filtro al nuevo tipo,
                '  unicamente hasta un máximo tambien conocido.
                If FilterTarget = roTimeBlockItem.roBlockType.roBTBreak AndAlso Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAbsence)) > 0 Then
                    ' Si existe una ausencia completa convierte el tipo de la ausencia
                    '  descanso del total de la ausencia menos el tiempo pagado
                    NewAbsence = New roTimeBlockItem
                    NewAbsence.Copy(Layer)
                    NewAbsence.BlockType = roTimeBlockItem.roBlockType.roBTAbsence
                    NewAbsence.TimeValue = Layer.TimeValue
                    TimeBlockList.Add(NewAbsence)
                Else
                    ' Sino convierte el tipo indicado por el filtro al nuevo tipo
                    TimeBlockList.Replace(Layer, FilterTarget)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_PaidTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_PaidTime")
            End Try

            Return bolRet
        End Function

        Public Shared Function ProcessLayer_Break(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByRef oState As roEngineState, ByVal FloatingTime As Double, ByVal oMandatoryLayerList As roCollection) As Boolean
            '
            ' Procesa una capa de descansos
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim NewLayer As New roTimeBlockItem

            Dim EmployeeBreakTime As Double

            Dim MaxBreakTime As Object
            Dim MinBreakTime As Object
            Dim NoPunchBreakTime As Object
            Dim MinBreakAction As Object
            Dim bolret = False

            Dim FirstLayerMandatory As New roTimeBlockItem

            Try

                ' Obtenemos el periodo de descanso
                ' En el caso que el horario sea por horas y tenga definido el descano por duracion, aplicamos el periodo correspondiente
                ' Aplicamos la configuracion que corresponda para cada franja rígida
                If oMandatoryLayerList.Count > 0 And Any2Time(Any2Time(ShiftLayer.Data("Begin")).DateOnly).Value = Any2Time("1899/12/01").Value Then
                    ' Obtenemos la primera franja obligada
                    For index = 1 To oMandatoryLayerList.Count
                        If index = 1 Then
                            FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                        Else
                            If FirstLayerMandatory.Period.Begin.NumericValue > oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex).Period.Begin.NumericValue Then
                                FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                            End If
                        End If
                    Next index

                    ' Sobre la primera franja sumamos las duraciones para crear el periodo de descanso
                    Layer.Period.Begin = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Begin")).TimeOnly))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Finish")).TimeOnly))
                Else
                    Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))
                End If

                Layer.BlockType = roTimeBlockItem.roBlockType.roBTBreak
                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue

                ' Obtenemos datos de filtros del descanso
                MinBreakTime = Any2Double(ShiftLayer.Data("MinBreakTime"))
                MaxBreakTime = Any2Double(ShiftLayer.Data("MaxBreakTime"))
                NoPunchBreakTime = Any2Double(ShiftLayer.Data("NoPunchBreakTime"))
                If MaxBreakTime = 0 Then MaxBreakTime = 999
                MinBreakAction = ShiftLayer.Data("MinBreakAction")

                ' Comprueba si existe una ausencia completa durante el descanso
                If Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAbsence)) > 0 Then
                    ' Si existe una ausencia completa durante el descanso, descuenta el tiempo permitido de descanso del total de la ausencia menos el tiempo pagado
                    TimeBlockList.SubstractTimeInPeriod(Layer.Period, MaxBreakTime, roTimeBlockItem.roBlockType.roBTAbsence)
                Else
                    ' No hay ausencia completa. Calculamos tiempo descansado
                    EmployeeBreakTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAnyAbsence))

                    ' Convertimos las incidencias de ausencias en este tiempo en tiempo de descanso
                    TimeBlockList.Replace(Layer, roTimeBlockItem.roBlockType.roBTAnyAbsence)

                    ' Mira si no ha fichado descanso o no ha llegado al mínimo
                    If EmployeeBreakTime = 0 And NoPunchBreakTime > 0 Then
                        ' No ha fichado descanso y hay un tiempo a quitar si no se ficha, lo hacemos

                        ' Resta el tiempo a quitar por no fichar de las horas trabajadas
                        TimeBlockList.SubstractTimeInPeriod(Layer.Period, NoPunchBreakTime, roTimeBlockItem.roBlockType.roBTAnyAttendance)

                        'Crea una bloque de tiempo de descanso con el tiempo sustraido
                        NewLayer = New roTimeBlockItem
                        NewLayer.Period.Begin = Layer.Period.Begin
                        NewLayer.Period.Finish = Layer.Period.Finish
                        NewLayer.BlockType = roTimeBlockItem.roBlockType.roBTBreak
                        NewLayer.TimeValue = Any2Time(Date.FromOADate(NoPunchBreakTime))
                        TimeBlockList.Add(NewLayer)

                    ElseIf EmployeeBreakTime < MinBreakTime Then
                        ' No ha hecho el minimo, hacemos lo que se haya configurado
                        Select Case MinBreakAction
                            Case roBreakCreateIncidence
                                ' Genera incidencia de descanso minimo
                                TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, roTimeBlockItem.roBlockType.roBTUndertimeBreak, Date.FromOADate(MinBreakTime - EmployeeBreakTime))

                            Case roBreakSubstractAttendanceTime
                                ' Resta el tiempo minimo de las horas trabajadas
                                If (MinBreakTime - EmployeeBreakTime) = Layer.Period.PeriodTime.VBNumericValue Then
                                    ' Si hay que restar el total de horas reemplazamos el periodo por descanso
                                    TimeBlockList.Replace(Layer, roTimeBlockItem.roBlockType.roBTAnyAttendance)
                                Else
                                    ' Si no es el total solo restammos el tiempo
                                    'TimeBlockList.SubstractTimeInPeriod Layer.Period, MinBreakTime - EmployeeBreakTime, roBTAnyAttendance
                                    TimeBlockList.ReplaceEx(Layer, MinBreakTime - EmployeeBreakTime, roTimeBlockItem.roBlockType.roBTAnyAttendance)
                                End If
                        End Select
                    End If

                    ' Miramos si ha hecho el maximo de descanso
                    If EmployeeBreakTime > MaxBreakTime Then
                        ' Supera el maximo permitido de descanso, genera incidencia y quita el tiempo de la
                        '  incidencia generada del descanso
                        TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, roTimeBlockItem.roBlockType.roBTOvertimeBreak, Date.FromOADate(EmployeeBreakTime - MaxBreakTime))
                        TimeBlockList.SubstractTimeInPeriod(Layer.Period, EmployeeBreakTime - MaxBreakTime, roTimeBlockItem.roBlockType.roBTBreak)
                    End If
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Break")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Break")
            End Try

            Return bolret
        End Function

        Public Shared Function ProcessLayer_DailyTotalsFilter(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByRef zShift As roShiftEngine, ByVal FloatingTime As Double, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa las diferencias positivas y negativas
            '
            Dim Layer As New roTimeBlockItem
            Dim EmployeeWorkingTime As Double
            Dim EmployeeAttendanceTime As Double
            Dim EmployeeAbsenceTime As Double
            Dim MinFilterValue As Object = Nothing
            Dim MaxFilterValue As Object = Nothing
            Dim MaxFilterAction As String = ""
            Dim Index As Integer
            Dim bolret As Boolean = False

            Try
                ' Obtenemos el periodo del filtro, en este caso all el maximo posible
                Layer.Period.Begin = Any2Time(DateAdd("d", -2, zDate.Value))
                Layer.Period.Finish = Any2Time(DateAdd("d", 2, zDate.Value))

                ' Obtenemos tiempo  minimo
                If ShiftLayer.Data("MinTime") IsNot Nothing Then MinFilterValue = Any2Double(ShiftLayer.Data("MinTime"))

                ' Obtenemos tiempo máximo
                If ShiftLayer.Data("MaxTime") IsNot Nothing Then
                    MaxFilterValue = Any2Double(ShiftLayer.Data("MaxTime"))
                    MaxFilterAction = Any2String(ShiftLayer.Data("MaxTimeAction"))
                Else
                    MaxFilterValue = 999
                End If

                ' Obtiene tiempo de presencia realizado
                EmployeeWorkingTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTWorking))
                EmployeeAttendanceTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAnyAttendance))
                EmployeeAbsenceTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAnyAbsence)) - Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTBreak))

                ' Procesa filtro de tiempo máximo
                If EmployeeWorkingTime > MaxFilterValue Then
                    ' Pasa del máximo: convertimos all el tiempo que se pasa en una incidencia o extras
                    '  según sea el filtro.
                    TimeBlockList.SubstractTimeInPeriod(Layer.Period, EmployeeWorkingTime - MaxFilterValue, roTimeBlockItem.roBlockType.roBTWorking)
                    If MaxFilterAction = roOvertimeAsOvertime Then Layer.BlockType = roTimeBlockItem.roBlockType.roBTOverworking Else Layer.BlockType = roTimeBlockItem.roBlockType.roBTDailyOverworking
                    TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, Layer.BlockType, Date.FromOADate(EmployeeWorkingTime - MaxFilterValue))
                End If

                ' Procesa filtro de tiempo minimo
                ' Si no ha hecho el mínimo de tiempo necesario, genera incidencia con
                '  el tiempo minimo a realizar menos el tiempo de incidencias ya
                '  existentes.
                If MinFilterValue IsNot Nothing Then 'AndAlso Not MinFilterValue Is vbEmpty Then
                    If EmployeeAttendanceTime <> 0 Then
                        TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, roTimeBlockItem.roBlockType.roBTDailyUnderworking, Date.FromOADate(roConversions.Max(MinFilterValue - EmployeeAbsenceTime - EmployeeAttendanceTime, 0)))
                    Else
                        Index = 1
                        While Index <= TimeBlockList.Count
                            TimeBlockList.Remove(Index)
                        End While

                        ' En este caso limitamos el periodo a los limites del horario, por si existe alguna prevision de ausencia por horas que pueda justificar correctamente los tiempos
                        Try
                            Layer.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, zShift.StartLimit))
                            Layer.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, zShift.EndLimit))
                        Catch ex As Exception
                            'No hacemos nada debido a un xml incorrecto
                        End Try
                        TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, roTimeBlockItem.roBlockType.roBTAbsence, Date.FromOADate(MinFilterValue))
                    End If
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_DailyTotalsFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_DailyTotalsFilter")
            End Try

            Return bolret

        End Function

        Public Shared Function ProcessLayer_GroupFilter(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa una capa de filtro total o por grupos (es igual que el unitario, pero es una
            '  capa distinta porque se debe procesar al final de all)
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim FilterValue As Date
            Dim FilterAction As String
            Dim FilterTarget As Long
            Dim IncidenceValue As Date
            Dim bolret As Boolean = False

            Try

                ' Obtenemos el periodo del filtro y sus detalles
                Layer.Period.Begin = zDate.Substract(2, "d")
                Layer.Period.Finish = zDate.Add(2, "d")
                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue
                FilterValue = Any2DateTime(ShiftLayer.Data("Value"))
                FilterAction = Any2String(ShiftLayer.Data("Action"))
                FilterTarget = Any2Long(ShiftLayer.Data("Target"))
                IncidenceValue = TimeBlockList.GetDateTimeInPeriod(Layer.Period, FilterTarget)

                FilterValue = New DateTime(1899, 12, 30).AddHours(FilterValue.Hour).AddMinutes(FilterValue.Minute)

                ' Ahora miramos si el tiempo entre el intervalo indicado supera el filtro o no
                If (IncidenceValue.Minute > 0 OrElse IncidenceValue.Hour > 0) AndAlso Any2Time(IncidenceValue, True).VBNumericValue < Any2Time(FilterValue, True).VBNumericValue Then
                    ' No supera el filtro, miramos que hacemos
                    Select Case FilterAction
                        Case roFilterTreatAsWork
                            ' Debemos considerar esas incidencias como trabajadas, cambiamos su
                            '  tipo a trabajadas
                            TimeBlockList.Replace(Layer, FilterTarget)

                        Case roFilterIgnore
                            ' Debemos eliminar esas incidencias
                            TimeBlockList.RemoveInPeriod(Layer.Period, FilterTarget)

                        Case Else  ' Tipo de filtro desconocido, ignora
                    End Select
                Else
                    ' Supera el filtro, no hacemos nada
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_GroupFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_GroupFilter")
            End Try

            Return bolret
        End Function

        Public Shared Function ProcessLayer_UnitFilter(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa una capa de filtro unitario
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim FilterValue As Date
            Dim FilterAction As String
            Dim FilterTarget As Long
            Dim IncidenceValue As Date
            Dim bolret As Boolean = False

            Try
                ' Obtenemos el periodo del filtro y sus detalles
                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))
                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue
                FilterValue = Any2DateTime(ShiftLayer.Data("Value"))
                FilterAction = Any2String(ShiftLayer.Data("Action"))
                FilterTarget = Any2Long(ShiftLayer.Data("Target"))

                ' Ahora miramos si el tiempo entre el intervalo indicado supera el filtro o no
                IncidenceValue = TimeBlockList.GetDateTimeInPeriod(Layer.Period, FilterTarget)
                FilterValue = New DateTime(1899, 12, 30).AddHours(FilterValue.Hour).AddMinutes(FilterValue.Minute)

                If (IncidenceValue.Minute > 0 Or IncidenceValue.Hour > 0) And Any2Time(IncidenceValue, True).VBNumericValue < Any2Time(FilterValue, True).VBNumericValue Then
                    ' No supera el filtro, miramos que hacemos
                    Select Case FilterAction
                        Case roFilterTreatAsWork
                            ' Debemos considerar esas incidencias como trabajadas, cambiamos su
                            '  tipo a trabajadas
                            TimeBlockList.Replace(Layer, FilterTarget)
                        Case roFilterIgnore
                            ' Debemos eliminar esas incidencias
                            TimeBlockList.RemoveInPeriod(Layer.Period, FilterTarget)

                        Case Else  ' Tipo de filtro desconocido, ignora
                    End Select
                Else
                    ' Supera el filtro, no hacemos nada
                End If

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_UnitFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_UnitFilter")
            Finally
            End Try

            Return bolret

        End Function

        Public Shared Function ProcessLayer_WorkingMaxMinFilter(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByRef TimeBlockList As roTimeBlockList, ByVal FloatingTime As Double, ByRef oState As roEngineState, ByRef zShift As roShiftEngine, ByVal IDEmployee As Long) As Boolean
            '
            ' Procesa una capa de filtros de horas máximas/minimas
            '
            Dim Layer As New roTimeBlockItem
            Dim EmployeeTotalTime As Double
            Dim EmployeeAbsenceTime As Double
            Dim MinFilterValue As Object
            Dim MaxFilterValue As Object
            Dim MaxFilterAction As Object
            Dim AvailableTime As Double
            Dim i As Integer
            Dim bolret As Boolean = False

            Dim StartHour As Double
            Dim EndHour As Double
            Dim DateTime As New roTime

            Try

                ' Obtenemos el periodo del filtro
                If Not ShiftLayer.Data.Exists("Begin") Then
                    Return True
                    Exit Function
                End If
                If Not ShiftLayer.Data.Exists("Finish") Then
                    Return True
                    Exit Function
                End If

                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))

                ' Obtenemos tiempo máximo y minimo
                MinFilterValue = Any2Double(ShiftLayer.Data("MinTime"))
                MaxFilterValue = Any2Double(ShiftLayer.Data("MaxTime"))
                MaxFilterAction = Any2String(ShiftLayer.Data("MaxTimeAction"))

                ' Si es un horario Starter
                If zShift.AdvancedParameters.Contains("Starter=[1]") Then
                    ' Debemos ajustar los limites a los valores del DailySchedule
                    Dim StarterDs As System.Data.DataTable = CreateDataTable("@SELECT# StartFlexible1,EndFlexible1,isnull(ExpectedWorkingHours,0) as Hours FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime)
                    If StarterDs IsNot Nothing AndAlso StarterDs.Rows.Count > 0 Then
                        If Not IsDBNull(StarterDs.Rows(0)("StartFlexible1")) AndAlso Not IsDBNull(StarterDs.Rows(0)("EndFlexible1")) Then
                            DateTime = zDate
                            StartHour = Any2Time(StarterDs.Rows(0)("StartFlexible1"), True).NumericValue
                            EndHour = Any2Time(StarterDs.Rows(0)("EndFlexible1"), True).NumericValue

                            If StartHour <> 0 Or EndHour <> 0 Then
                                Layer.Period.Begin = DateTime.Add(Any2Time(StartHour))
                                Layer.Period.Finish = DateTime.Add(Any2Time(EndHour))
                                MinFilterValue = Any2Time(StarterDs.Rows(0)("Hours")).VBNumericValue
                                MaxFilterValue = Any2Time(StarterDs.Rows(0)("Hours")).VBNumericValue
                            End If
                        End If
                    End If

                End If

                ' Obtiene tiempo de presencia realizado
                EmployeeTotalTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTWorking))
                EmployeeAbsenceTime = Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTAnyAbsence)) - Any2Double(TimeBlockList.GetDateTimeInPeriod(Layer.Period, roTimeBlockItem.roBlockType.roBTBreak))

                ' Procesa filtro de tiempo máximo
                If EmployeeTotalTime > MaxFilterValue Then
                    ' Ha hecho tiempo de más, generamos incidencia o horas extras
                    If MaxFilterAction = roOvertimeAsOvertime Then
                        ' Generamos incidencia "horas extras"
                        Layer.BlockType = roTimeBlockItem.roBlockType.roBTOverworking
                        Layer.TimeValue = Any2Time(Date.FromOADate(MaxFilterValue - EmployeeTotalTime))

                        ' Reemplazamos las horas trabajadas que sean necesarias por las extras
                        TimeBlockList.Replace(Layer, roTimeBlockItem.roBlockType.roBTWorking)
                    Else
                        ' Generamos incidencia "horas extras en flexibles"
                        Layer.BlockType = roTimeBlockItem.roBlockType.roBTFlexibleOverworking

                        ' Obtiene tiempo a recortar de horas extras
                        Layer.TimeValue = Any2Time(Date.FromOADate(MaxFilterValue - EmployeeTotalTime))

                        AvailableTime = Layer.TimeValue.VBNumericValue

                        For i = TimeBlockList.Count To 1 Step -1
                            ' Obtenemos el periodo a recortar de horas extras del bloque actual
                            Layer.Period.Begin = roConversions.Max(Any2Time(Any2Time(TimeBlockList.Item(i).Period.Finish.NumericValue(True), True).NumericValue - DateTime2Double(Date.FromOADate(AvailableTime), True)), TimeBlockList.Item(i).Period.Begin)
                            Layer.Period.Finish = TimeBlockList.Item(i).Period.Finish

                            Layer.TimeValue = Any2Time(Layer.Period.Finish.NumericValue(True) - Layer.Period.Begin.NumericValue(True))

                            'Recortamos el periodo de horas extras de la franja de horas trabajadas actual
                            TimeBlockList.ReplaceEx(Layer, Layer.TimeValue.NumericValue(True), roTimeBlockItem.roBlockType.roBTWorking)

                            AvailableTime = AvailableTime - Layer.TimeValue.VBNumericValue

                            If AvailableTime <= 0 Then Exit For
                        Next i
                    End If
                End If

                If EmployeeTotalTime + EmployeeAbsenceTime < MinFilterValue Then TimeBlockList.AddIncidence(Layer.Period.Begin, Layer.Period.Finish, roTimeBlockItem.roBlockType.roBTFlexibleUnderworking, Date.FromOADate(MinFilterValue - EmployeeTotalTime - EmployeeAbsenceTime))

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_WorkingMaxMinFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_WorkingMaxMinFilter")
            Finally

            End Try

            Return bolret

        End Function

        Public Shared Function Execute_RemoveBreakTimes(ByRef zTimes As roTimeBlockList, ByRef oState As roEngineState) As Boolean
            '
            ' Elimina descansos (de momento VisualTime no graba los tiempos de descanso, pero los
            '  usa internamente y en un futuro se podrían grabar)
            '
            Dim Index As Integer
            Dim bolret As Boolean = False

            Try

                Index = 1
                While Index <= zTimes.Count
                    If zTimes.Item(Index).BlockType = roTimeBlockItem.roBlockType.roBTBreak Then
                        zTimes.Remove(Index)
                    Else
                        Index = Index + 1
                    End If
                End While

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_RemoveBreakTimes")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_RemoveBreakTimes")
            Finally

            End Try

            Return bolret

        End Function

        Public Shared Function Execute_ProcessZones(ByVal zDate As roTime, ByRef TimeBlockList As roTimeBlockList, ByRef Zones As Generic.List(Of roShiftEngineTimeZone), ByRef zShift As roShiftEngine, ByVal FloatingTime As Double, ByRef oState As roEngineState) As Boolean
            '
            ' Aplica la informacion de zonas horarias a la lista de tiempos
            '
            Dim i As Integer
            Dim Period As New roTimePeriod
            Dim mFloatingTime As Double = 0
            Dim bolret As Boolean = False

            Try

                If Not Zones Is Nothing Then
                    For i = 0 To Zones.Count - 1
                        mFloatingTime = FloatingTime

                        With Zones(i)
                            If mFloatingTime <> 0 Then
                                If Any2Boolean(ExecuteScalar("@SELECT# IsBlocked FROM sysroShiftTimeZones WHERE IDZone= " & Zones(i).IDZone & " AND BeginTime=" & Any2Time(Zones(i).BeginTime).SQLDateTime & " AND IDShift=" & zShift.ID)) Then
                                    ' Si esta bloqueada no la desplazamos
                                    mFloatingTime = 0
                                End If
                            End If

                            'Period.Begin = Any2Time(zDate.Add(.BeginTime).Value)
                            Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, .BeginTime), Any2Time(mFloatingTime, True).Value))
                            'Period.Finish = Any2Time(zDate.Add(.EndTime).Value)
                            Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, .EndTime), Any2Time(mFloatingTime, True).Value))
                            TimeBlockList.SetTimeZoneInPeriod(Period, roTimeBlockItem.roBlockType.roBTAny, .IDZone)
                        End With
                    Next
                End If
                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessZones")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessZones")
            Finally

            End Try

            Return bolret

        End Function

        Public Shared Function Execute_ProcessCenters(ByVal zEmployee As Long, ByVal zDate As roTime, ByRef TimeBlockList As roTimeBlockList, ByRef zShift As roShiftEngine, ByRef oState As roEngineState, ByVal zMoves As roMoveList, ByVal bolProgrammedAbsenceOnHolidays As Boolean, ByVal mLastPunchCenterEnabled As String) As Boolean
            '
            ' Asignamos el centro de coste a cada incidencia generada
            '
            Dim bolret As Boolean = False
            Dim IDDefaultCenter As Double
            Dim IDShiftCenter As Double
            Dim IDPUCenter As Double
            Dim zCenterMoves As roMoveList
            Dim Index As Long
            Dim bolApplyonAbsences As Boolean
            Dim Inci As roTimeBlockItem
            Dim TimeValue As Double
            Dim sSQL As String

            Try

                'Obtenemos los fichajes de presencia
                'zMoves = roBaseEngineManager.Execute_GetMovesLive(zEmployee, zDate, oCn, oState)
                'bolret = roBaseEngineManager.Execute_ProcessMoves(zMoves, zDate, oState)
                'If Not bolret Then
                ' Return bolret
                ' Exit Function
                ' End If

                ' 1. Obtenemos el centro de coste al que está asignado por defecto,
                '    y el centro de coste del horario asignado y si se aplica en incidencias de ausencia
                IDDefaultCenter = GetDefaultCenter(zEmployee, zDate, oState)
                GetShiftCenter(IDShiftCenter, zEmployee, zDate, bolApplyonAbsences, oState, bolProgrammedAbsenceOnHolidays)
                'GetProductiveUnitCenter(IDPUCenter, EmployeeID, zDate, oState)

                ' 2.Obtenemos los fichajes de cesiones de centro de coste durante el día
                zCenterMoves = roBaseEngineManager.Execute_GetCenterPunches(zEmployee, zDate, zMoves, oState, mLastPunchCenterEnabled)

                ' 3. Asignamos a las incidencias al centor de coste que corresponde teniendo en cuenta las siguientes casuisticas
                '   a) las incidencias de ausencia se asigna el centro de coste con esta prioridad
                '        - que venga del horario y tenga indicado que se debe asignar las incidencias de ausencia
                '        - en cualquier otro caso el centro de coste del departamento del empleado
                '   b) las incidencias de trabajo se justifican con esta prioridad
                '        - que venga de una cesion a otro centro
                '        - si el dia esta asignado a una unidad productiva de un presupuesto que tiene asignado centro de coste
                '        - Si el horario asignado tiene centro de coste
                '        - en cualquier otro caso el centro de coste del departamento del empleado

                For Index = 1 To TimeBlockList.Count
                    If Not TimeBlockList.Item(Index).IsWorkingTime Then
                        ' Incidencias de ausencia
                        If bolApplyonAbsences Then
                            ' Si hay que aplicar el centro de coste del horario
                            TimeBlockList.Item(Index).IDCenter = IDShiftCenter
                        Else
                            ' Sino el del departamento del empleado
                            TimeBlockList.Item(Index).IDCenter = IDDefaultCenter
                        End If
                    Else
                        'Incidencias de trabajo
                        'Por defecto las asignamos todas al centro de coste de la Unidad Productiva, sino al horario o en su defecto al del departamento
                        If IDPUCenter > 0 Then
                            TimeBlockList.Item(Index).IDCenter = IDPUCenter
                        ElseIf IDShiftCenter > 0 Then
                            TimeBlockList.Item(Index).IDCenter = IDShiftCenter
                        Else
                            TimeBlockList.Item(Index).IDCenter = IDDefaultCenter
                        End If
                    End If
                Next

                ' Si el empleado ha realizado fichajes de cambios de centro de coste a lo largo del día
                If zCenterMoves.Moves.Count > 0 Then
                    ' Si existen fichajes de cambio de centro de coste
                    For Index = 0 To zCenterMoves.Moves.Count - 1
                        Dim TotalTimeBlocks As Integer = TimeBlockList.Count
                        Dim IndexBlock As Integer = 1

                        While IndexBlock <= TotalTimeBlocks
                            ' Si la incidencia es de trabajo y no es diferencia positiva, ni horas extras en flexible
                            ' ** las horas extras en flexibles y la diferencia positiva se les asigna el centro de coste
                            ' ** del departamento o del horario
                            ' ** si se quiere que el fichaje de cambio afecte a diferencia positiva
                            ' ** hay que cambiar el filtro de la franja flexible para que si supera el maximo genere horas extras
                            ' ** si se quiere que el fichaje de cambio afecte a horas extras en flexibles
                            ' ** hay que cambiar el filtro general para que genere horas extras si supera el valor XXX
                            If TimeBlockList.Item(IndexBlock).IsWorkingTime And (TimeBlockList.Item(IndexBlock).BlockType <> roTimeBlockItem.roBlockType.roBTDailyOverworking And TimeBlockList.Item(IndexBlock).BlockType <> roTimeBlockItem.roBlockType.roBTFlexibleOverworking) Then
                                ' Si la incidencia es de horas extras, pero no aplica sobre un periodo concreto del dia, tambien la descartamos
                                If Not (TimeBlockList.Item(IndexBlock).Period.Begin.VBNumericValue = Any2Time(DateAdd("d", -2, zDate.Value)).VBNumericValue And TimeBlockList.Item(IndexBlock).Period.Finish.VBNumericValue = Any2Time(DateAdd("d", 2, zDate.Value)).VBNumericValue And TimeBlockList.Item(IndexBlock).BlockType = roTimeBlockItem.roBlockType.roBTOverworking) Then
                                    ' Si el cambio de centro aplica al bloque
                                    If zCenterMoves.Moves(Index).Period.Begin.VBNumericValue < TimeBlockList.Item(IndexBlock).Period.Finish.VBNumericValue Then
                                        If zCenterMoves.Moves(Index).Period.Begin.VBNumericValue <= TimeBlockList.Item(IndexBlock).Period.Begin.VBNumericValue Then
                                            ' Si el cambio de centro y la incidencia empiezan al mismo tiempo,
                                            ' asignamos toda la incidencia al mismo centro de coste que el fichaje
                                            TimeBlockList.Item(IndexBlock).IDCenter = zCenterMoves.Moves(Index).InCause
                                        Else
                                            ' Si el cambio de centro se produce después del inicio de la incidencia,
                                            ' hay que partir la incidencia en dos y la incidencia que empieza a partir
                                            ' del cambio de centro de coste debemos asignarle el valor del centro de coste del fichaje

                                            ' Creamos el nuevo bloque
                                            Inci = New roTimeBlockItem
                                            Inci.Period.Begin = zCenterMoves.Moves(Index).Period.Begin
                                            Inci.Period.Finish = TimeBlockList.Item(IndexBlock).Period.Finish
                                            Inci.BlockType = TimeBlockList.Item(IndexBlock).BlockType
                                            Inci.TimeZone = TimeBlockList.Item(IndexBlock).TimeZone

                                            'Nos guardamos el valor inicial del periodo
                                            TimeValue = TimeBlockList.Item(IndexBlock).TimeValue.VBNumericValue

                                            'Modificamos el fin del bloque inicial
                                            TimeBlockList.Item(IndexBlock).Period.Finish = zCenterMoves.Moves(Index).Period.Begin

                                            ' calculamos el nuevo valor del bloque inicial
                                            TimeBlockList.Item(IndexBlock).TimeValue = Any2Time(Date.FromOADate(roConversions.Min(TimeValue, TimeBlockList.Item(IndexBlock).Period.Finish.VBNumericValue - TimeBlockList.Item(IndexBlock).Period.Begin.VBNumericValue)))

                                            'calculamos el nuevo valor del nuevo bloque
                                            TimeValue = TimeValue - TimeBlockList.Item(IndexBlock).TimeValue.VBNumericValue
                                            Inci.TimeValue = Any2Time(Date.FromOADate(TimeValue))

                                            ' Asignamos el centro de coste al nuevo bloque
                                            Inci.IDCenter = zCenterMoves.Moves(Index).InCause

                                            ' Lo añadimos a la lista de bloques
                                            If Inci.TimeValue.VBNumericValue > 0 Then
                                                TimeBlockList.Add(Inci)
                                                TotalTimeBlocks = TotalTimeBlocks + 1
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                            IndexBlock = IndexBlock + 1
                        End While
                    Next
                End If

                If mLastPunchCenterEnabled = "1" Then
                    ' En el caso que se deba mirar el último fichaje de centro de coste
                    ' aunque no sea del dia de cálculo,
                    ' las incidencias de ausencia las asignamos al ultimo fichaje de centro de coste anterior a la incidencia,
                    ' únicamente con los siguientes tipos Ausencia/Retraso/Interrupcion/Salida anticipada
                    For IndexBlock = 1 To TimeBlockList.Count

                        If Not TimeBlockList.Item(IndexBlock).IsWorkingTime And (TimeBlockList.Item(IndexBlock).BlockType = roTimeBlockItem.roBlockType.roBTAbsence Or TimeBlockList.Item(IndexBlock).BlockType = roTimeBlockItem.roBlockType.roBTLateArrival Or TimeBlockList.Item(IndexBlock).BlockType = roTimeBlockItem.roBlockType.roBTUnexpectedBreak Or TimeBlockList.Item(IndexBlock).BlockType = roTimeBlockItem.roBlockType.roBTEarlyLeave) Then
                            ' Obtenemos el fichaje de centro de coste anterior al inicio de la incidencia
                            sSQL = "@SELECT# TOP 1 DateTime, TypeData FROM Punches with (nolock) WHERE " _
                                  & " IDEmployee=" & zEmployee.ToString & " AND " _
                                  & " DateTime <=" & TimeBlockList.Item(IndexBlock).Period.Begin.SQLDateTime & " AND " _
                                  & " ActualType=13 ORDER BY ShiftDate desc, DateTime desc, ID desc"
                            Dim aDS = CreateDataTable(sSQL)
                            If aDS IsNot Nothing AndAlso aDS.Rows.Count > 0 Then
                                If Any2Time(Format$(aDS.Rows(0)("Datetime"), "yyyy/MM/dd HH:mm")).VBNumericValue <= TimeBlockList.Item(IndexBlock).Period.Begin.VBNumericValue Then
                                    ' Asignamos el centro de coste
                                    TimeBlockList.Item(IndexBlock).IDCenter = Any2Double(aDS.Rows(0)("TypeData"))
                                End If
                            End If
                        End If
                    Next
                End If

                ' Marcamos cada incidencia indicando si el centro de coste es el de defecto o no
                For IndexBlock = 1 To TimeBlockList.Count
                    If TimeBlockList.Item(IndexBlock).IDCenter = IDDefaultCenter Then
                        TimeBlockList.Item(IndexBlock).DefaultCenter = True
                    Else
                        TimeBlockList.Item(IndexBlock).DefaultCenter = False
                    End If
                Next
                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessCenters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::Execute_ProcessCenters")
            Finally

            End Try

            Return bolret

        End Function

        Public Shared Function GetDefaultCenter(ByVal zEmployee As Long, ByVal zDate As roTime, ByRef oState As roEngineState) As Double
            Dim sSQL As String = ""
            Dim aDS As DataTable
            Dim IDCenter As Double = 0
            Dim IDGroup As Double = 0
            Dim i As Integer

            Try

                IDCenter = 0
                ' Obtenemos el grupo y el centro de coste al que pertenece en la fecha indicada el empleado
                sSQL = "@SELECT#  isnull(IDCenter, 0) as DefaultCenter , Path, ID FROM Groups with (nolock) WHERE ID IN(@SELECT#  IDGROUP FROM EmployeeGroups with (nolock) WHERE IDEmployee = " & zEmployee.ToString & " AND BeginDate <= " & zDate.SQLSmallDateTime & " AND EndDate >= " & zDate.SQLSmallDateTime & ")"
                aDS = CreateDataTable(sSQL)
                If aDS IsNot Nothing AndAlso aDS.Rows.Count > 0 Then
                    IDCenter = Any2Double(aDS.Rows(0)("DefaultCenter"))

                    i = StringItemsCount(Any2String(aDS.Rows(0)("Path")), "\") - 1
                    ' Si el grupo actual no tiene centro asignado recorremos todos los grupos padres
                    ' hasta encontrar el primero que tenga asignado uno
                    If IDCenter = 0 And i > 0 Then
                        While i >= 0
                            IDGroup = Any2Double(String2Item(Any2String(aDS.Rows(0)("path")), i - 1, "\"))
                            If IDGroup > 0 Then
                                IDCenter = Any2Double(ExecuteScalar("@SELECT# isnull(IDCenter, 0) as DefaultCenter FROM Groups with (nolock) WHERE ID=" & IDGroup))
                                If IDCenter > 0 Then
                                    i = -1
                                End If
                            End If
                            i = i - 1
                        End While
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetDefaultCenter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetDefaultCenter")
            Finally

            End Try

            Return IDCenter
        End Function

        Public Shared Sub GetShiftCenter(ByRef IDShiftCenter As Double, ByVal EmployeeID As Long, ByVal zDate As roTime, ByRef bolApplyonAbsences As Boolean, ByRef oState As roEngineState, ByVal bolProgrammedAbsenceOnHolidays As Boolean)
            '
            ' Obtenemos el centro de coste del horario asignado
            '

            Dim sSQL As String
            Dim aDS As DataTable

            Try

                IDShiftCenter = 0
                bolApplyonAbsences = False

                ' Obtenemos el centro de coste del horario asignado, y si se aplica sobre las incidencias de ausencia
                If Not bolProgrammedAbsenceOnHolidays Then
                    sSQL = "@SELECT# isnull(IDCenter, 0) as ShiftCenter , isnull(ApplyCenterOnAbsence, 0) as ApplyCenter FROM Shifts with (nolock) WHERE ID IN(@SELECT# IDShiftused  FROM DailySchedule with (nolock) WHERE IDEmployee = " & EmployeeID.ToString & " AND Date = " & zDate.SQLSmallDateTime & ")"
                Else
                    ' Obtenemos el centro de coste del horario base
                    sSQL = "@SELECT# isnull(IDCenter, 0) as ShiftCenter , isnull(ApplyCenterOnAbsence, 0) as ApplyCenter FROM Shifts with (nolock) WHERE ID IN(@SELECT# IDShiftBase  FROM DailySchedule with (nolock) WHERE IDEmployee = " & EmployeeID.ToString & " AND Date = " & zDate.SQLSmallDateTime & ")"
                End If

                aDS = CreateDataTable(sSQL)
                If aDS IsNot Nothing AndAlso aDS.Rows.Count > 0 Then
                    IDShiftCenter = Any2Double(aDS.Rows(0)("ShiftCenter"))
                    If IDShiftCenter <> 0 Then
                        bolApplyonAbsences = IIf(Any2Double(aDS.Rows(0)("ApplyCenter")) = 0, False, True)
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetShiftCenter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetShiftCenter")
            Finally

            End Try

        End Sub

        Public Shared Function CompactLayers(ByRef TimeBlockList As roTimeBlockList, ByRef oState As roEngineState) As Boolean
            '
            ' 1. Elimina bloques con tiempo 0
            ' 2. Compacta bloques consecutivos que sean del mismo tipo y zona horaria
            '
            Dim FirstIndex As Long
            Dim SecondIndex As Long
            Dim SecondItem As roTimeBlockItem
            Dim TmpValue As New roTime
            Dim bolret As Boolean = False

            Try

                ' 1. Eliminación bloques vacios
                FirstIndex = 1
                While FirstIndex < TimeBlockList.Count
                    If TimeBlockList.Item(FirstIndex).TimeValue.VBNumericValue = 0 Then
                        TimeBlockList.Remove(FirstIndex)
                    Else
                        FirstIndex = FirstIndex + 1
                    End If
                End While

                ' 2. Compactación de bloques
                FirstIndex = 1
                While FirstIndex < TimeBlockList.Count
                    With TimeBlockList.Item(FirstIndex)
                        ' Ahora miramos los bloques posteriores para intentar compactar con este
                        SecondIndex = FirstIndex + 1
                        Do While SecondIndex <= TimeBlockList.Count
                            ' Guardamos bloque en referencia para mayor comodidad y rapidez
                            SecondItem = TimeBlockList.Item(SecondIndex)

                            ' Si el inicio del bloque es posterior al final del anterior no solo no puede
                            '  compactarse sino que debemos dejar de buscar (porque estan ordenados).
                            If SecondItem.Period.Begin.VBNumericValue > .Period.Finish.VBNumericValue Then Exit Do

                            ' En caso contario miramos si los bloques son continuos, del mismo tipo y misma
                            '  zona horaria.
                            If .Period.Finish.VBNumericValue = SecondItem.Period.Begin.VBNumericValue And
                                .BlockType = SecondItem.BlockType And .TimeZone = SecondItem.TimeZone And .IDCenter = SecondItem.IDCenter Then
                                ' Compactamos los dos bloques en uno
                                ' Compactamos bloque
                                TmpValue = .TimeValue
                                .Period.Finish = SecondItem.Period.Finish
                                .TimeValue = TmpValue.Add(SecondItem.TimeValue)
                                SecondItem = Nothing
                                TimeBlockList.Remove(SecondIndex)
                            Else
                                ' No podemos compactar este bloque, miramos el siguiente
                                SecondIndex = SecondIndex + 1
                            End If
                        Loop
                    End With
                    ' Miramos el siguiente bloque
                    FirstIndex = FirstIndex + 1
                End While

                bolret = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::CompactLayers")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::CompactLayers")
            Finally

            End Try

            Return bolret

        End Function

        Public Shared Function ProcessLayer_OrderLayersByType(ByRef zShift As roShiftEngine) As Generic.List(Of roShiftEngineLayer)
            Dim oShiftLayersList As New Generic.List(Of roShiftEngineLayer)
            For Index = 0 To zShift.Layers.Count - 1
                oShiftLayersList.Add(zShift.Layers(Index))
                If Not zShift.Layers(Index).ChildLayers Is Nothing AndAlso zShift.Layers(Index).ChildLayers.Count > 0 Then
                    For z = 0 To zShift.Layers(Index).ChildLayers.Count - 1
                        oShiftLayersList.Add(zShift.Layers(Index).ChildLayers(z))
                    Next
                End If
            Next

            Try
                ' Aisgnamos el inicio de la franja en caso que se pueda, para luego poder ordenarlo por esta fecha
                For Each olayer As roShiftEngineLayer In oShiftLayersList
                    olayer.BeginLayer = Nothing
                    If olayer.Data.Exists("Begin") Then
                        olayer.BeginLayer = Any2Time(olayer.Data("Begin")).Value
                    End If
                Next
            Catch ex As Exception
            End Try

            ' Ordenamos las franjas por tipo y inicio de la franja
            Return oShiftLayersList.OrderBy(Function(x) x.LayerType).ThenBy(Function(x) x.BeginLayer).ToList()

        End Function

        Public Shared Function ProcessLayer_Mandatory_GetFloatingBegin(ByRef FirstPossibleTime As roTime, LastPossibleTime As roTime, ByRef TimeBlockList As roTimeBlockList, ByVal zForeCast As roTimeBlockList, ByRef zShift As roShiftEngine, ByRef oState As roEngineState) As roTime
            '
            ' Subfuncion de ProcessLayer_Mandatory que devuelve la hora a usar como inicio de un periodo
            '  de rigidas si son flotantes.
            '
            Dim FromIn As Double
            Dim ToIn As Double
            Dim ThisIn As Double
            Dim BestIn As Double
            Dim bolOnPeriod As Boolean
            Dim bolWorkingTime As Boolean

            Try

                ' Pasa tiempos a formato double para ir más rápido
                FromIn = FirstPossibleTime.NumericValue(True)
                ToIn = LastPossibleTime.NumericValue(True)

                ' Inicialmente la entrada es la mas tarde posible
                BestIn = ToIn

                bolOnPeriod = False
                ' Obtiene mejor entrada (si hay)
                For Index = 1 To TimeBlockList.Count
                    If TimeBlockList.Item(Index).IsWorkingTime Then
                        bolWorkingTime = True
                        ThisIn = roConversions.Max(TimeBlockList.Item(Index).Period.Begin.NumericValue(True), FromIn)
                        ' Si este movimiento empieza antes que el mejor hasta ahora y acaba dentro o despues
                        '  del periodo de entrada, nos lo quedamos (si empieza y acaba antes del inicio NO)
                        If TimeBlockList.Item(Index).Period.Finish.NumericValue(True) >= FromIn And ThisIn <= BestIn Then
                            BestIn = ThisIn
                            bolOnPeriod = True
                        End If
                    End If
                Next

                If Not bolOnPeriod Then
                    ' Si la entrada es posterior al final del periodo de inicio flotante ,
                    ' tenemos en cuenta el inicio del periodo de entrada en caso que este marcado en el registro
                    If DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Engine.FloatingBeginIn").ToUpper = "TRUE" Then
                        BestIn = FromIn
                    End If
                End If

                ' Si hay fichajes y en el caso de existir prevision de ausencia por horas
                ' miramos si debemos modificar la hora de inicio  de la franja rígida
                If bolWorkingTime And Not zForeCast Is Nothing Then
                    ProcessLayer_GetFloatingBeginOnProgrammedCause(BestIn, TimeBlockList, FirstPossibleTime, LastPossibleTime, zForeCast, zShift, oState)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory_GetFloatingBegin")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory_GetFloatingBegin")
            Finally

            End Try

            ' Devuelve entrada
            Return Any2Time(BestIn, True)

        End Function

        Public Shared Sub ProcessLayer_GetFloatingBeginOnProgrammedCause(ByRef BestIn As Double, ByRef TimeBlock As roTimeBlockList, ByVal FirstPossibleTime As roTime, ByVal LastPossibleTime As roTime, ByVal zForeCast As roTimeBlockList, ByRef zShift As roShiftEngine, ByRef oState As roEngineState)
            '
            ' Subfuncion de ProcessLayer_Mandatory que devuelve la hora a usar como inicio de un periodo
            '  de rigidas si son flotantes en el caso que exista una ausencia por horas.
            '
            Dim ForeCast As roTimeBlockItem
            Dim ThisIn As Double
            Dim Index As Integer

            Try

                ' 0.Inicialmente el inicio de la franja es el final de la entrada semirigida o la de un fichaje anterior a ella
                If zForeCast Is Nothing Then Exit Sub

                For Index = 1 To zForeCast.Count
                    ForeCast = zForeCast.Item(Index)
                    If ForeCast.Period.Begin.VBNumericValue < FirstPossibleTime.VBNumericValue And ForeCast.Period.Finish.VBNumericValue > FirstPossibleTime.VBNumericValue Then
                        ' 1.Si la prevision de ausencia empieza antes del inicio de la franja de rigida y acaba despues del inicio
                        ThisIn = FirstPossibleTime.NumericValue(True)
                        BestIn = roConversions.Min(BestIn, ThisIn)

                    ElseIf ForeCast.Period.Begin.VBNumericValue >= FirstPossibleTime.VBNumericValue And ForeCast.Period.Begin.VBNumericValue <= LastPossibleTime.VBNumericValue Then
                        ' 2.Si la prevision empieza dentro del la franja
                        ThisIn = roConversions.Min(ForeCast.Period.Begin.NumericValue(True), BestIn)
                        BestIn = roConversions.Min(BestIn, ThisIn)
                    End If

                Next
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_GetFloatingBeginOnProgrammedCause")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_GetFloatingBeginOnProgrammedCause")
            Finally
            End Try

        End Sub

        Public Shared Function ProcessLayer_Mandatory_Complementary(ByRef zDate As roTime, ByVal IDLayer As Double, MandatoryLayer As roTimeBlockItem, ByRef TimeBlockList As roTimeBlockList, ByRef zShift As roShiftEngine, ByVal IDEmployee As Long, ByRef oState As roEngineState, ByVal oXMLLayerDefinition As String) As Boolean
            '
            ' Procesa las capa obligada para generar horas complementarias en caso necesario
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerComplementary As New roTimeBlockItem
            Dim oXML As String
            Dim oCollection As New roCollection
            Dim i As Integer
            Dim dblLayer As Double
            Dim dblComplementaryHours As Double
            Dim dblOrdinaryHours As Double
            Dim bolRet As Boolean = False

            Try

                'Obtenemos el total de horas complementarias de la franja correspondiente
                If oXMLLayerDefinition.Length > 0 Then
                    oXML = oXMLLayerDefinition
                Else
                    oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                End If

                If Len(oXML) > 0 Then
                    Try
                        oCollection.LoadXMLString(oXML)
                        For i = 1 To 2
                            If oCollection.Exists("LayerID_" & i.ToString) Then
                                If IDLayer = Any2Double(oCollection("LayerID_" & i.ToString)) Then
                                    dblLayer = i
                                    Exit For
                                End If
                            End If
                        Next i
                        If dblLayer > 0 Then
                            If oCollection.Exists("LayerComplementaryHours_" & dblLayer.ToString) And oCollection.Exists("LayerOrdinaryHours_" & dblLayer.ToString) Then
                                ' Nos guardamos las horas complementarias y ordinarias
                                dblComplementaryHours = Any2Double(oCollection("LayerComplementaryHours_" & dblLayer.ToString))
                                dblOrdinaryHours = Any2Double(oCollection("LayerOrdinaryHours_" & dblLayer.ToString))

                                ' Creamos la franja de complementarias
                                LayerComplementary.BlockType = roTimeBlockItem.roBlockType.roBTComplementary
                                LayerComplementary.Period.Begin = Any2Time(DateTimeAdd(MandatoryLayer.Period.Begin.Value, Any2Time(dblOrdinaryHours).Value)) 'Any2Time(MandatoryLayer.Period.Begin.NumericValue(True) + Any2Time(dblOrdinaryHours).NumericValue(True))
                                LayerComplementary.Period.Finish = MandatoryLayer.Period.Finish
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                LayerComplementary.BlockType = roTimeBlockItem.roBlockType.roBTComplementary
                If LayerComplementary.Period.Finish.NumericValue(True) - LayerComplementary.Period.Begin.NumericValue(True) > 0 And dblComplementaryHours > 0 Then
                    ' Reemplazamos horas trabajadas por complementarias
                    TimeBlockList.ReplaceEx(LayerComplementary, dblComplementaryHours, roTimeBlockItem.roBlockType.roBTWorking)
                End If
                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory_Complementary")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_Mandatory_Complementary")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Sub GetBaseFloatingTime(ByVal IDShift As Double, ByRef BaseFloatingTime As Double, ByRef oState As roEngineState)
            '
            ' Obtenemos la hora de inicio del horario base
            '
            Dim zStartFloating As roTime

            Dim oShift As roShiftEngine

            Try

                BaseFloatingTime = 0

                oShift = roBaseEngineManager.GetShiftFromCache(IDShift, oState)
                ' Comprobamos si el horario es de tipo flotante
                If oShift IsNot Nothing AndAlso oShift.ShiftType = ShiftType.NormalFloating Then 'Any2Boolean(ExecuteScalar("@SELECT# IsFloating FROM Shifts with (nolock) WHERE ID=" & IDShift.ToString)) Then
                    ' Obtenemos la hora de inicio base
                    zStartFloating = Any2Time(oShift.StartFloating, True)        'Any2Time(ExecuteScalar("@SELECT# StartFloating FROM Shifts with (nolock) WHERE ID=" & IDShift.ToString), True)
                    BaseFloatingTime = zStartFloating.NumericValue(True)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetBaseFloatingTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetBaseFloatingTime")
            Finally

            End Try

        End Sub

        Public Shared Sub ProcessProductiveAbsences(ByVal zDate As roTime, ByRef zShift As roShiftEngine, ByRef TimeBlockList As roTimeBlockList, ByVal mIDCauseEW As Double, ByRef oState As roEngineState)
            '
            ' Procesamos las ausencias productivas, generando tramos de trabajo
            '
            Dim TimePABlockList As New roTimeBlockList
            Dim TmpPABlock As New roTimeBlockItem
            Dim Index As Integer
            Dim bolExistPA As Boolean = False
            Dim oExternalCauseList As New roCollection
            Dim aDS As DataTable = Nothing
            Dim SQL As String
            Dim i As Integer

            Try

                If mIDCauseEW = 0 Then Exit Sub

                If mIDCauseEW = -1 Then
                    ' Obtenemos todas las ausencias configuradas como trabajo externo desde pantalla v2
                    SQL = "@SELECT# ID FROM CAUSES with (nolock) WHERE ExternalWork= 1 AND WorkingType=0 "
                    aDS = CreateDataTable(SQL)
                    If aDS IsNot Nothing AndAlso aDS.Rows.Count > 0 Then
                        For Each DBArray As DataRow In aDS.Rows
                            ' Añade cada justificacion a la lista
                            oExternalCauseList.Add(Any2Double(DBArray("ID")))
                        Next

                    End If
                Else
                    ' En caso de pasar directamente una concreta, solo añadimos esa
                    oExternalCauseList.Add(mIDCauseEW)
                End If

                For i = 1 To oExternalCauseList.Count
                    ' Para cada justificacion de trabajo externo
                    mIDCauseEW = Any2Double(oExternalCauseList.Key(i))

                    ' Buscamos en los bloques actuales si existe la justificacion de trabajo externo
                    ' V2 SOLO TENEMOS EN CUENTA SALIDAS CON TRABAJO EXTERNO PARA GENERAR TRAMOS DE TRABAJO
                    For Index = 1 To TimeBlockList.Count
                        If IsNothing(TimeBlockList.Item(Index).OutCause) Then TimeBlockList.Item(Index).OutCause = 0
                        If IsNothing(TimeBlockList.Item(Index).InCause) Then TimeBlockList.Item(Index).InCause = 0

                        If Index = 1 And TimeBlockList.Count > 1 Then
                            ' 01. Si es el primero bloque de varios, solo puede haber salida + TE
                            If Any2Double(TimeBlockList.Item(Index).OutCause) = mIDCauseEW Then
                                TmpPABlock = New roTimeBlockItem
                                TmpPABlock.BlockType = roTimeBlockItem.roBlockType.roBTOverworking
                                TmpPABlock.Period.Begin = TimeBlockList.Item(Index).Period.Finish
                                TmpPABlock.Period.Finish = TimeBlockList.Item(Index + 1).Period.Begin
                                TmpPABlock.Period.PeriodTime.Value = Any2Time(TmpPABlock.Period.Finish.NumericValue - TmpPABlock.Period.Begin.NumericValue).Value

                                TimePABlockList.Add(TmpPABlock)

                            End If
                        ElseIf TimeBlockList.Count > 1 And TimeBlockList.Count <> Index Then
                            ' 02. Si es un bloque intermedio, solo aplicamos sobre Salida + TE
                            If Any2Double(TimeBlockList.Item(Index).OutCause) = mIDCauseEW Then
                                ' Salida + TE
                                TmpPABlock = New roTimeBlockItem
                                TmpPABlock.BlockType = roTimeBlockItem.roBlockType.roBTOverworking
                                TmpPABlock.Period.Begin = TimeBlockList.Item(Index).Period.Finish
                                TmpPABlock.Period.Finish = TimeBlockList.Item(Index + 1).Period.Begin
                                TmpPABlock.Period.PeriodTime.Value = Any2Time(TmpPABlock.Period.Finish.NumericValue - TmpPABlock.Period.Begin.NumericValue).Value

                                TimePABlockList.Add(TmpPABlock)
                            End If
                        End If
                    Next
                Next i

                ' Añadimos todos los bloques nuevos, a los bloques ya existentes de trabajo
                If Not TimePABlockList Is Nothing Then
                    For Index = 1 To TimePABlockList.Count
                        TimeBlockList.Add(TimePABlockList.Item(Index))
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessProductiveAbsences")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessProductiveAbsences")
            Finally

            End Try
        End Sub

        Public Shared Sub GetFloatingTime(ByVal IDEmployee As Long, ByVal IDShift As Double, ByVal ShiftDate As Date, ByRef FloatingTime As Double, ByRef oState As roEngineState)
            '
            ' Obtenemos la hora de inicio del horario base
            '
            Dim zStartFloating As roTime
            Dim zStartFloatingScheduler As roTime
            Dim oShift As roShiftEngine

            Try

                FloatingTime = 0

                ' Comprobamos si el horario es de tipo flotante
                oShift = roBaseEngineManager.GetShiftFromCache(IDShift, oState)
                If oShift IsNot Nothing AndAlso oShift.ShiftType = ShiftType.NormalFloating Then  ' Any2Boolean(ExecuteScalar("@SELECT# IsFloating FROM Shifts with (nolock) WHERE ID=" & IDShift.ToString)) Then
                    ' Obtenemos la hora de inicio base
                    'zStartFloating = Any2Time(ExecuteScalar("@SELECT# StartFloating FROM Shifts with (nolock) WHERE ID=" & IDShift.ToString), True)
                    zStartFloating = Any2Time(oShift.StartFloating, True)

                    ' Obtenemos la hora de inicio del dia a calcular
                    zStartFloatingScheduler = Any2Time(ExecuteScalar("@SELECT# StartShiftUsed FROM DailySchedule with (nolock) WHERE IDEmployee=" & IDEmployee.ToString & " AND Date= " & Any2Time(ShiftDate).SQLSmallDateTime))

                    ' Obtenemos la diferencia de horas
                    FloatingTime = zStartFloatingScheduler.NumericValue(True) - zStartFloating.NumericValue(True)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetFloatingTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetFloatingTime")
            Finally

            End Try

        End Sub

        Public Shared Function Execute_GetForecast(ByVal zEmployee As Long, ByVal zDate As roTime, ByVal zShift As roShiftEngine, ByVal zMoves As roMoveList, ByRef oState As roEngineState) As roTimeBlockList
            '
            ' Obtiene previsiones de incidencias del dia indicado. A día de hoy (mayo-2012), sólo se permite una incidencia prevista por día y
            ' empleado. De todos modos, se monta la estructura de manera que soporte más de una, por generalidad
            '
            Dim myDS As New DataTable
            Dim sSQL As String = ""
            Dim myList As New roTimeBlockList
            Dim myItem As roTimeBlockItem
            Dim AddDay As Integer = 0
            Dim auxDate As Date
            Dim bolApplyForecast As Boolean = False
            Dim ShortNameCause As String = ""
            Dim CauseShiftData As String = ""

            Try

                sSQL = "@SELECT# * FROM ProgrammedCauses with (nolock) WHERE IDEmployee=" & zEmployee &
                            " AND ( (Date <=" & zDate.SQLSmallDateTime & " And IsNull(FinishDate, Date) >= " & zDate.SQLSmallDateTime & ") " &
                                "       OR  (Date =" & zDate.Add(1, "d").SQLSmallDateTime & ")) "

                myDS = CreateDataTable(sSQL)
                If myDS IsNot Nothing AndAlso myDS.Rows.Count > 0 Then
                    For Each DBArray As DataRow In myDS.Rows
                        ' Añade cada incidencia a la lista
                        myItem = New roTimeBlockItem

                        'myitem.TimeZone
                        myItem.BlockType = 1010     'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        myItem.TimeZone = 0         'Este dato no me interesa, pero es opbligatorio en un TimeBlockItem
                        '2012/05/04: Usaremos esta campo para distinguir en Live Ausencias calculadas con la lógica de Win32 (aplica a actualizaciones Win32 a Live)
                        myItem.Tag = DBArray("IDCause") ' IDCause

                        If Any2Boolean(DBArray("Win32")) Then
                            myItem.TimeZone = 1
                        End If

                        AddDay = 0

                        auxDate = DBArray("Date")
                        If Any2Time(auxDate).NumericValue > zDate.NumericValue Then
                            AddDay = 1
                        End If

                        If Not IsDate(DBArray("BeginTime")) Then
                            DBArray("BeginTime") = Any2Time("1899/12/30 00:00").Value
                        End If
                        If Not IsDate(DBArray("EndTime")) Then
                            DBArray("EndTime") = Any2Time("1899/12/30 23:59").Value
                        End If

                        myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, Any2Time(DBArray("BeginTime")).Value)).Add(AddDay, "d")  ' Inicio del Periodo
                        myItem.Period.Finish = Any2Time(DateTimeAdd(zDate.Value, Any2Time(DBArray("EndTime")).Value)).Add(AddDay, "d")  ' Fin del Periodo

                        myItem.TimeValue = Any2Time(DBArray("Duration")) 'Duracion

                        ' Duracion minima
                        myItem.InCause = 0
                        If IsNumeric(DBArray("MinDuration")) Then
                            myItem.InCause = Any2Time(DBArray("MinDuration")).NumericValue
                        End If

                        ' Si la ausencia por horas es del dia posterior
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

                ' Verificamos si existen parametros avanzados del horario en los que se
                ' indique si hay alguna justificacion que tenga este comportamiento
                Try
                    If Not zShift Is Nothing Then
                        If Len(zShift.AdvancedParameters) > 0 Then
                            ' verificamos si el primer fichaje tiene alguna
                            ' justificacion relacionada, que contenga el parametro avanzado del horario
                            If zMoves.Moves.Count >= 1 Then
                                If Any2Double(zMoves.Moves(0).InCause) > 0 Then
                                    If InStr(zShift.AdvancedParameters, "[BEGINMANDATORYBYCAUSE]=") > 0 Then
                                        ShortNameCause = Any2String(ExecuteScalar("@SELECT# SHORTNAME FROM CAUSES with (nolock) WHERE ID=" & Any2Double(zMoves.Moves(0).InCause)))
                                        CauseShiftData = Mid$(zShift.AdvancedParameters, Len("[BEGINMANDATORYBYCAUSE]=") + 1)
                                        For Index = 0 To StringItemsCount(CauseShiftData, ";") - 1
                                            If String2Item(String2Item(CauseShiftData, Index, ";"), 0, "@") = ShortNameCause Then
                                                ' Si la encontramos debemos
                                                ' simular una prevision de ausencia por horas con esa justificacion y que inicie
                                                ' como se indique en el horario
                                                myItem = New roTimeBlockItem
                                                myItem.BlockType = 1010
                                                myItem.TimeZone = 0
                                                myItem.Tag = Any2Double(zMoves.Moves(0).InCause)
                                                myItem.Period.Begin = Any2Time(DateTimeAdd(zDate.Value, Any2Time(Any2Time("1899/12/30 " & String2Item(String2Item(CauseShiftData, Index, ";"), 1, "@")).Value).Value))
                                                myItem.Period.Finish = myItem.Period.Begin.Add(1, "n")
                                                myItem.TimeValue = Any2Time(myItem.Period.Finish.NumericValue - myItem.Period.Begin.NumericValue)
                                                myItem.InCause = 0
                                                myList.Add(myItem)
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception

                End Try
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetFloatingTime")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetFloatingTime")
            Finally

            End Try

            Return myList
        End Function

        Public Shared Sub SetEmployeeLastPunchData(ByVal IDEmployee As Long, ByVal roDate As roTime, ByRef oState As roEngineState)
            '
            ' Actualizamos los datos del ultimo fichaje del empleado
            '
            Dim LastPunch As String = ""
            Dim ShiftDate As String = ""
            Dim LastIDCause As Double
            Dim IsPresent As Boolean
            Dim Operation As String = "S"
            Dim tb As New DataTable("EmployeeStatus")

            Try

                ' Obtenemos los datos de su ultimo fichaje
                GetLastPunch(IDEmployee, oState, LastPunch, LastIDCause, IsPresent, Operation, ShiftDate)

                ' Actualizamos el nuevo estado del empleado
                Dim strSQL As String = "@SELECT# * FROM EmployeeStatus WHERE IDEmployee=" & IDEmployee
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDEmployee") = IDEmployee
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("LastPunch") = IIf(Len(LastPunch) > 0, LastPunch, DBNull.Value)
                oRow("IDCause") = LastIDCause
                oRow("Type") = Operation
                oRow("IsPresent") = IsPresent
                oRow("ShiftDate") = IIf(Len(ShiftDate) > 0, ShiftDate, DBNull.Value)
                oRow("PRLStatus") = "?" 'Marcamos para que el proceso EOG actualice el estado de PRL

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                ' En caso que el empleado este presente eliminamos todas las alertas anteriores de "Cubrir empleado ausente", si existe
                If IsPresent Then
                    ExecuteSql("@DELETE# FROM sysroNotificationTasks WHERE Key1Numeric=" & IDEmployee.ToString &
                                     " AND Key3DateTime<=" & roDate.SQLDateTime & " AND IDNotification IN(@select# id from Notifications WHERE IDType=13)")
                End If

                'Debug.Print("Employee: " & IDEmployee.ToString & "  Set LastPunch at : " & LastPunch & " , Present=" & Any2String(IsPresent))
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::SetEmployeeLastPunchData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::SetEmployeeLastPunchData")
            Finally

            End Try

        End Sub

        Public Shared Sub SetEmployeeBeginMandatory(ByRef z As roEngineData, ByVal mShiftToday As roShiftEngine, ByVal mShiftTomorrow As roShiftEngine, ByRef oState As roEngineState)
            '
            ' Obtenemos la primera hora obligada que tiene que estar presente el empleado
            '
            Dim LastPunch As String = ""
            Dim ShiftDate As String = ""
            Dim LastIDCause As Double
            Dim IsPresent As Boolean
            Dim Operation As String = "S"
            Dim RS As DataTable = Nothing
            Dim FirstBeginMandatory As String = roNullDate
            Dim StartLimit As String = roNullDate
            Dim zFloatingTime As Double = 0
            Dim reProcessed As Boolean = False
            Dim NotPunches As Boolean = False
            Dim Verified As Double = 0
            Dim zBaseFloatingTime As Double = 0
            Dim updateBeginMandatory As Date
            Dim updateStartLimit As Date

            Try

                ' Si la fecha de calculo no es la de sistema ni la del día anterior
                ' no actualizamos los datos
                If Not (z.ProcDate.VBNumericValue = Any2Time(Now.Date).VBNumericValue OrElse z.ProcDate.VBNumericValue = Any2Time(Now.Date).Add(-1, "d").VBNumericValue) Then Exit Sub

                ' Si no tiene horario asignado
                If z.SelectedShiftID(roEngineData.Today) = 0 Then
                    ' marcamos como hora obligada una fecha futura
                    If Not ExecuteSql("@UPDATE# EmployeeStatus SET BeginMandatory=" & Any2Time(roNullDate).SQLSmallDateTime & " WHERE IDEmployee=" & z.Employee.ToString) Then
                        'LogHandle.LogMessage roError, "roCoverage::SetEmployeeBeginMandatory: Unexpected error setting BeginMandatory on EmployeeStatus table for employee " & z.Employee
                    End If
                    Exit Sub
                End If

                ' Obtenemos el ultimo fichaje del empleado
                RS = New DataTable
                RS = CreateDataTable("@SELECT# * FROM EmployeeStatus with (nolock) WHERE IDEmployee=" & z.Employee.ToString)
                If RS IsNot Nothing AndAlso RS.Rows.Count > 0 Then
                    LastPunch = Any2String(RS.Rows(0)("LastPunch"))
                    LastIDCause = Any2String(RS.Rows(0)("IDCause"))
                    Operation = Any2String(RS.Rows(0)("Type"))
                    IsPresent = Any2Boolean(RS.Rows(0)("IsPresent"))
                    ShiftDate = Any2String(RS.Rows(0)("ShiftDate"))
                End If

                ' Si la fecha de calculo es de ayer
                If z.ProcDate.VBNumericValue = Any2Time(Now.Date).Add(-1, "d").VBNumericValue Then

                    ' Si no tiene fichaje no hacemos nada
                    If Len(LastPunch) = 0 Then Exit Sub

                    ' Si el horario de ayer es nocturno
                    ' y el ultimo fichaje tiene la fecha del sistema seguimos procesando
                    If Not (Any2Time(LastPunch).DateOnly = Any2Time(Now.Date).DateOnly And z.Limits(0).Finish.DateOnly = Any2Time(Now.Date).DateOnly) Then
                        Exit Sub
                    Else
                        reProcessed = True
                    End If
                End If

                ' Si no tiene fichajes
                NotPunches = False

                If LastPunch = "" Then
                    NotPunches = True
                    LastPunch = z.ProcDate.DateOnly
                    Operation = "S"
                    IsPresent = False
                    ShiftDate = z.ProcDate.DateOnly
                    LastIDCause = "0"
                End If

                ' Obtenemos la primera hora obligada en funcion del ultimo fichaje
                ' y el horario asignado

                ' Hora Inicio horario planificado
                zFloatingTime = Any2Double(z.SelectedFloatingTime(roEngineData.Today))

                ' Hora de inicio base
                GetBaseFloatingTime(mShiftToday.ID, zBaseFloatingTime, oState)

                ' Obtenemos tiempo a desplazar del horario a calcular
                zFloatingTime = zFloatingTime - zBaseFloatingTime

                GetStartLimitFromPunch(z.ProcDate, mShiftToday, zFloatingTime, LastPunch, Operation, IsPresent, ShiftDate, LastIDCause, FirstBeginMandatory, StartLimit, Any2Long(z.Employee), z, oState)

                ' Si hemos calculado el dia de ayer y esta correcto, debemos calcular tambien el dia de hoy
                ' en caso de horarios nocturnos
                If reProcessed And Any2Time(FirstBeginMandatory).VBNumericValue = Any2Time(roNullDate).VBNumericValue Then
                    zFloatingTime = Any2Double(z.SelectedFloatingTime(roEngineData.Tomorrow))
                    zFloatingTime = zFloatingTime - zBaseFloatingTime

                    GetStartLimitFromPunch(z.ProcDate.Add(1, "d"), mShiftTomorrow, zFloatingTime, LastPunch, Operation, IsPresent, ShiftDate, LastIDCause, FirstBeginMandatory, StartLimit, Any2Long(z.Employee), z, oState)
                End If

                ' Indicamos la primera hora de presencia obligada
                updateBeginMandatory = Any2Time(FirstBeginMandatory).ValueDateTime
                updateStartLimit = Any2Time(StartLimit).ValueDateTime
                Dim LocalBeginMandatory As Date = updateBeginMandatory
                Dim bMultitimezone As Boolean = roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "MultiTimeZoneEnabled"))

                If Any2Time(roNullDate).VBNumericValue <> Any2Time(FirstBeginMandatory).VBNumericValue Then
                    ' Si tenemos la opción de aceptar empleados en multiples zonas horarias calculamos su zona horaria
                    Dim tZoneInfo As TimeZoneInfo = Nothing
                    If Any2Time(roNullDate).Value <> Any2Time(FirstBeginMandatory).Value Then
                        updateBeginMandatory = roNotificationHelper.GetServerTime(roTypes.Any2Integer(z.Employee), updateBeginMandatory, tZoneInfo)
                    End If

                    If Any2Time(roNullDate).Value <> Any2Time(StartLimit).Value AndAlso updateStartLimit > Date.MinValue Then
                        updateStartLimit = roNotificationHelper.GetServerTime(roTypes.Any2Integer(z.Employee), updateStartLimit, tZoneInfo)
                    End If
                End If

                ' Indicamos si se debe volver a revisar al empleado cuando llegue la proxima hora obligada

                ' Si no tiene fichajes de ese dia
                ' o esta presente
                ' o la hora de inicio obligado es 01/01/2079
                ' o la hora de inicio oblidaga es antigua
                ' hay que marcarlo como verificado
                If NotPunches OrElse IsPresent OrElse (Any2Time(FirstBeginMandatory).VBNumericValue = Any2Time(roNullDate).VBNumericValue) OrElse (Any2Time(FirstBeginMandatory).VBNumericValue < Any2Time(Now).VBNumericValue) Then
                    ' Se marca como verificado porque el mismo proceso de calculo de planificacion
                    ' real que se lanzara a posteriori no se deberá volver a recaclular en caso de que no
                    ' se vuelva a fichar
                    Verified = 1
                Else
                    ' Si el ultimo fichaje es de la fecha de cálculo y esta ausente
                    ' hay que dejarlo como pendiente de verificacion
                    If Any2Time(ShiftDate).VBNumericValue = z.ProcDate.VBNumericValue AndAlso Not IsPresent Then
                        Verified = 0
                    Else
                        Verified = 1
                    End If
                End If

                Dim sqlUpdate As String = $"@UPDATE# EmployeeStatus SET Verified = {Verified}, " &
                $"BeginMandatory = {Any2Time(updateBeginMandatory).SQLSmallDateTime}, " &
                $"StartLimit = {Any2Time(updateStartLimit).SQLSmallDateTime}"

                ' Ponemos el LocalBeginMandatory si MultiTimeZoneEnabled esta activado
                If bMultitimezone Then
                    sqlUpdate &= $", LocalBeginMandatory = {Any2Time(LocalBeginMandatory).SQLSmallDateTime}"
                Else
                    sqlUpdate &= ", LocalBeginMandatory = NULL"
                End If

                sqlUpdate &= $" WHERE IDEmployee = {z.Employee.ToString}"

                ExecuteSql(sqlUpdate)
                'Debug.Print("Employee: " & z.Employee.ToString & "  Set Verified : " & Verified & " BeginMandatory : " & updateBeginMandatory)

                ' Eliminamos las alertas si el empleado esta presente o no tiene que estar presente
                If ExecuteSql("@DELETE# FROM sysroNotificationTasks WHERE Key1Numeric = " & z.Employee.ToString &
                        " AND Key3DateTime<=" & Any2Time(Now).SQLDateTime & " AND IDNotification IN(@SELECT# id from Notifications with (nolock) WHERE (IDType=13 OR IDType=15))" &
                        " AND  Key1Numeric IN(" &
                        " @SELECT# IDEMPLOYEE FROM EmployeeStatus with (nolock) " &
                        " WHERE IDEmployee= " & z.Employee & " AND ((IsPresent=0 AND BeginMandatory>=" & Any2Time(Now).SQLSmallDateTime & ") OR (IsPresent=1)))") Then
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::SetEmployeeBeginMandatory")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::SetEmployeeBeginMandatory")
            End Try
        End Sub

        Public Shared Sub SetBeginMandatoryIfFlexibleShift(IdShift As Integer, employeeId As Long, zDate As Date, ByRef oState As roEngineState)
            Try
                Dim SQL = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IdShift & " AND IDType = 1000 Order by ID"
                Dim SqlLayer = "@SELECT# * FROM sysroShiftsLayers WHERE IDShift = " & IdShift & "  AND IDType = 1600"
                Dim hoursWorkedFraction As Double = Any2Double(AccessHelper.ExecuteScalar("@SELECT# SUM(Value) FROM DailyIncidences WHERE IDEmployee= " & employeeId & " And Date =" & roTypes.Any2Time(roTypes.Any2Time(zDate).DateOnly).SQLSmallDateTime & " AND IDType = 1001 GROUP BY IDType"))

                Dim workingHours = TimeSpan.FromHours(hoursWorkedFraction)

                Dim xNow As DateTime = roTypes.UnspecifiedNow()

                Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

                Dim mDefinition As roCollection
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    Dim dr = dt.Rows(0)
                    mDefinition = New roCollection
                    mDefinition.LoadXMLString(dr("Definition"))
                    Dim endTimeLayer = roTypes.DateTimeAdd(xNow.Date, mDefinition("Finish"))

                    Dim dtLayer As DataTable = AccessHelper.CreateDataTable(SqlLayer)

                    If dtLayer IsNot Nothing AndAlso dtLayer.Rows.Count > 0 Then
                        Dim drLayer = dtLayer.Rows(0)
                        mDefinition = New roCollection
                        mDefinition.LoadXMLString(drLayer("Definition"))

                        Dim minTimeLayer = TimeSpan.FromHours(roConversions.ConvertTimeToHours(mDefinition("MinTime")))
                        Dim difference = minTimeLayer - workingHours
                        'Si nos falta tiempo por trabajar
                        If difference.TotalHours > 0 Then
                            Dim tZoneInfo As TimeZoneInfo = Nothing
                            Dim calculatedBeginMandatory = endTimeLayer - difference

                            Dim BeginMandatory = roTypes.Any2Time(roNotificationHelper.GetServerTime(employeeId, calculatedBeginMandatory, tZoneInfo))
                            Dim strQuery As String = "@UPDATE# EmployeeStatus SET BeginMandatory = " & BeginMandatory.SQLSmallDateTime

                            Dim bMultitimezone As Boolean = roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "MultiTimeZoneEnabled"))

                            If bMultitimezone Then
                                strQuery &= ", LocalBeginMandatory = " & roTypes.Any2Time(calculatedBeginMandatory).SQLSmallDateTime
                            Else
                                strQuery &= ", LocalBeginMandatory = NULL"
                            End If

                            strQuery &= " WHERE IDEmployee = " & employeeId

                            AccessHelper.ExecuteSql(strQuery)
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::SetBeginMandatoryIfFlexibleShift")
            End Try

        End Sub

        Public Shared Sub GetStartLimitFromPunch(ByVal zDate As roTime, ByRef zShift As roShiftEngine, ByVal FloatingTime As Double, ByVal Punch As String, ByVal Operation As String, ByVal IsPresent As Boolean, ByVal ShiftDate As String, ByVal LastIDCause As String, ByRef FirstBeginMandatory As String, ByRef StartLimit As String, ByVal IDEmployee As Long, ByRef z As roEngineData, ByRef oState As roEngineState)

            '
            ' Obtenemos la hora de inicio obligada en funcion de un fichaje y un horario
            '
            Dim ExistOnMandatory As Boolean = False
            Dim ExistOnBreak As Boolean = False
            Dim ExistOnWorking As Boolean = False
            Dim LayerMandatory As New roTimeBlockItem
            Dim LayerBreak As New roTimeBlockItem
            Dim LayerWorking As New roTimeBlockItem
            Dim ExistLayer As Boolean = False
            Dim LayersCollection As New roCollection
            Dim xShiftLayer As New roShiftEngineLayer
            Dim PreviousFinish As New roTime
            Dim NextBegin As New roTime
            Dim Limits As New roTimeBlockItem
            Dim Index As Integer = 0
            Dim Layer As New roTimeBlockItem
            Dim oXML As String = ""
            Dim oCollection As New roCollection
            Dim dblLayer As Double = 0
            Dim i As Integer = 0

            Dim MinTime As Double = 0
            Dim StartHour As Double
            Dim EndHour As Double
            Dim DateTime As New roTime

            Dim oMandatoryLayerList As New roCollection

            Try

                FirstBeginMandatory = roNullDate

                ' Si esta presente es correcto
                If IsPresent Then Exit Sub

                ' Inicialziamos los periodos de las franjas
                LayerMandatory.Period.Begin = Any2Time(roNullDate)
                LayerMandatory.Period.Finish = Any2Time(roNullDate)
                LayerBreak.Period.Begin = Any2Time(roNullDate)
                LayerBreak.Period.Finish = Any2Time(roNullDate)
                LayerWorking.Period.Begin = Any2Time(roNullDate)
                LayerWorking.Period.Finish = Any2Time(roNullDate)

                ' Si no hay horario, no podemos calcular nada
                If zShift Is Nothing Then Exit Sub

                Dim oShiftLayersList As New Generic.List(Of roShiftEngineLayer)

                ' Ordenamos las franjas por tipo
                oShiftLayersList = ProcessLayer_OrderLayersByType(zShift)

                ' Verifica si el fichaje intersecciona con alguna franja
                For Each xShiftLayer In oShiftLayersList
                    Select Case xShiftLayer.LayerType
                        Case roLayerTypes.roLTMandatory
                            ' Procesa capa de obligadas
                            If Not ExistOnMandatory Then
                                ExistOnMandatory = ProcessLayer_ExistOnMandatory(zDate, xShiftLayer, Punch, FloatingTime, Layer, LayersCollection, IDEmployee, z, oState)
                                If ExistOnMandatory Then
                                    LayerMandatory.Period.Begin = Layer.Period.Begin
                                    LayerMandatory.Period.Finish = Layer.Period.Finish
                                End If
                            End If
                            If StartLimit = roNullDate Then
                                StartLimit = Layer.Period.Begin.Value
                            End If
                            ExistLayer = True
                            If zShift.AllowComplementary Then oMandatoryLayerList.Add(Any2String(xShiftLayer.ID), Layer)

                        Case roLayerTypes.roLTBreak
                            ' Procesa capa de descansos
                            If Not ExistOnBreak Then
                                ExistOnBreak = ProcessLayer_ExistOnBreak(zDate, xShiftLayer, Punch, FloatingTime, Layer, LayersCollection, oMandatoryLayerList, oState)
                                If ExistOnBreak Then
                                    LayerBreak.Period.Begin = Layer.Period.Begin
                                    LayerBreak.Period.Finish = Layer.Period.Finish
                                    ExistOnBreak = True
                                End If
                            End If

                            ExistLayer = True

                        Case roLayerTypes.roLTWorkingMaxMinFilter
                            ' Procesa capa de filtro de max/min en permitidas
                            If Not ExistOnWorking Then
                                ExistOnWorking = ProcessLayer_ExistOnWorkingMaxMinFilter(zDate, xShiftLayer, Punch, FloatingTime, Layer, LayersCollection, oState, zShift, IDEmployee)
                                If ExistOnWorking Then
                                    LayerWorking.Period.Begin = Layer.Period.Begin
                                    LayerWorking.Period.Finish = Layer.Period.Finish
                                End If
                            End If

                            ExistLayer = True
                        Case Else
                            ' Otras capas simplemente no hacemos nada
                    End Select
                Next

                ' Si no hay franjas,  no marcamos ningún inicio obligado
                If Not ExistLayer Then Exit Sub

                If Not (ExistOnMandatory Or ExistOnBreak Or ExistOnWorking) Then
                    ' Si el fichaje no intersecciona con ninguna franja
                    ' Buscamos el final de franja anterior y el inicio de franja posterior

                    ' Ademas de obtener el inicio mas pequeño y el final mas grande de todas las franjas

                    PreviousFinish = Any2Time(Punch)
                    NextBegin = Any2Time(roNullDate)

                    For Index = 1 To LayersCollection.Count

                        Dim ActualLayer = From xLayer As roShiftEngineLayer In oShiftLayersList
                                          Where xLayer.ID = Any2Integer(LayersCollection.Key(Index))
                                          Select xLayer

                        Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ActualLayer(0).Data("Begin")), Any2Time(FloatingTime, True).Value))

                        If ActualLayer(0).LayerType = roLayerTypes.roLTMandatory Then
                            ' Si la franja es rigida
                            If ActualLayer(0).Data.Exists("FloatingBeginUpto") Then
                                ' Si hay entrada flotante
                                Layer.Period.Begin = ProcessLayer_Mandatory_GetFloatingBegin(Layer.Period.Begin, Any2Time(DateTimeAdd(zDate.Value, ActualLayer(0).Data("FloatingBeginUpto"))), z.TimeBlockList, Nothing, Nothing, oState)
                            End If
                            ' Si tiene hora de inicio variable
                            If ActualLayer(0).Data.Exists("AllowModifyIniHour") Then
                                oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                                If Len(oXML) > 0 Then
                                    Try
                                        oCollection.LoadXMLString(oXML)

                                        ' Obtenemos la hora de la franja correspondiente
                                        For i = 1 To 2
                                            If oCollection.Exists("LayerID_" & i) Then
                                                If Any2Double(ActualLayer(0).ID) = Any2Double(oCollection("LayerID_" & i)) Then
                                                    dblLayer = i
                                                    Exit For
                                                End If
                                            End If
                                        Next i
                                        If dblLayer > 0 Then
                                            If oCollection.Exists("LayerFloatingBeginTime_" & dblLayer) Then
                                                ' Asignamos la hora de inicio
                                                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, oCollection("LayerFloatingBeginTime_" & dblLayer)), Any2Time(FloatingTime, True).Value))
                                            End If
                                        End If
                                    Catch ex As Exception
                                    End Try
                                End If
                            End If
                        End If

                        Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ActualLayer(0).Data("Finish")), Any2Time(FloatingTime, True).Value))

                        If ActualLayer(0).LayerType = roLayerTypes.roLTMandatory Then
                            If ActualLayer(0).Data.Exists("FloatingFinishMinutes") Then
                                ' Si tambien hay salida flotante, establece en funcion de la entrada
                                Layer.Period.Finish = Layer.Period.Begin.Add(ActualLayer(0).Data("FloatingFinishMinutes"), "n")
                            End If

                            ' Si tiene duracion variable a partir de la entrada
                            If ActualLayer(0).Data.Exists("AllowModifyDuration") Then
                                oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                                If Len(oXML) > 0 Then
                                    Try
                                        oCollection.LoadXMLString(oXML)
                                        ' Obtenemos la hora de la franja correspondiente
                                        dblLayer = 0
                                        For i = 1 To 2
                                            If oCollection.Exists("LayerID_" & i) Then
                                                If Any2Double(ActualLayer(0).ID) = Any2Double(oCollection("LayerID_" & i)) Then
                                                    dblLayer = i
                                                    Exit For
                                                End If
                                            End If
                                        Next i
                                        If dblLayer > 0 Then
                                            If oCollection.Exists("LayerFloatingDuration_" & dblLayer) Then
                                                ' Asignamos la hora de finalizacion indicada
                                                Layer.Period.Finish = Layer.Period.Begin.Add(oCollection("LayerFloatingDuration_" & dblLayer), "n")
                                            End If
                                        End If
                                    Catch ex As Exception
                                    End Try
                                End If
                            End If
                        End If

                        If ActualLayer(0).LayerType = roLayerTypes.roLTWorkingMaxMinFilter Then
                            ' Si la franja es flexible comprobamos los limites de la franja
                            ' Si es un horario Starter
                            If zShift.AdvancedParameters.Contains("Starter=[1]") Then
                                ' Debemos ajustar los limites a los valores del DailySchedule
                                Dim StarterDs As System.Data.DataTable = CreateDataTable("@SELECT# StartFlexible1,EndFlexible1,isnull(ExpectedWorkingHours,0) as Hours FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime)
                                If StarterDs IsNot Nothing AndAlso StarterDs.Rows.Count > 0 Then
                                    If Not IsDBNull(StarterDs.Rows(0)("StartFlexible1")) AndAlso Not IsDBNull(StarterDs.Rows(0)("EndFlexible1")) Then
                                        StartHour = Any2Time(StarterDs.Rows(0)("StartFlexible1"), True).NumericValue(True)
                                        EndHour = Any2Time(StarterDs.Rows(0)("EndFlexible1"), True).NumericValue(True)
                                        Layer.Period.Begin = zDate.Add(Any2Time(StartHour))
                                        Layer.Period.Finish = zDate.Add(Any2Time(EndHour))
                                        MinTime = Any2Time(StarterDs.Rows(0)("Hours")).NumericValue
                                    End If
                                End If
                            Else
                                MinTime = Any2Time(ActualLayer(0).Data("MinTime")).NumericValue
                            End If

                            If Any2Time(Punch).NumericValue >= (Layer.Period.Begin.NumericValue + MinTime) Then
                                Layer.Period.Finish = Any2Time(Layer.Period.Begin.NumericValue + MinTime)
                            Else
                                Layer.Period.Begin = Any2Time(Layer.Period.Finish.NumericValue - MinTime)
                            End If
                        End If

                        ' inicializamos los limites
                        If Index = 1 Then
                            Limits.Period.Begin = Layer.Period.Begin
                            Limits.Period.Finish = Layer.Period.Finish
                        End If

                        ' Obtenemos el final alnterior y el inicio posterior
                        If Any2Time(Punch).VBNumericValue > Layer.Period.Finish.VBNumericValue Then
                            PreviousFinish = roConversions.Max(Layer.Period.Finish, PreviousFinish)
                        End If
                        If Any2Time(Punch).VBNumericValue < Layer.Period.Begin.VBNumericValue Then
                            NextBegin = roConversions.Min(Layer.Period.Begin, NextBegin)
                        End If

                        ' recalculamos los limities de las franjas
                        Limits.Period.Begin = roConversions.Min(Limits.Period.Begin, Layer.Period.Begin)
                        Limits.Period.Finish = roConversions.Max(Limits.Period.Finish, Layer.Period.Finish)
                    Next

                    ' Si la salida es posterior al ultimo fin de franja, la salida es correcta
                    If Any2Time(Punch).VBNumericValue >= Limits.Period.Finish.VBNumericValue Then Exit Sub

                    ' Sino obtenemos el final anterior o el primer inicio posterior al fichaje
                    If PreviousFinish.VBNumericValue = Any2Time(Punch).VBNumericValue Then
                        PreviousFinish = NextBegin
                    End If
                    If NextBegin.VBNumericValue = Any2Time(Punch).VBNumericValue Then
                        NextBegin = PreviousFinish
                    End If

                    Layer.Period.Begin = roConversions.Min(NextBegin, PreviousFinish)
                Else
                    If ExistOnBreak Then
                        ' Si intersecciona con Descanso
                        Layer.Period.Begin = LayerBreak.Period.Finish
                        If ExistOnMandatory Then
                            ' Si el final del descanso es el final de la franja rigida , no tiene que estar presente
                            If Layer.Period.Begin.VBNumericValue = LayerMandatory.Period.Finish.VBNumericValue Then
                                Exit Sub
                            End If
                        Else
                            ' Si no intersecciona con una franja rigida
                            ' el horario tiene franja rigida con una duracion en funcion de la entrada
                            ' y es posible que la franja de descanso se salgo de la franja rigida
                            ' en ese caso no tiene que estar presente
                            Exit Sub
                        End If

                    ElseIf ExistOnMandatory Then
                        ' Si intersecciona con obligada
                        Layer.Period.Begin = LayerMandatory.Period.Begin
                    ElseIf ExistOnWorking Then
                        ' Si intersecciona con trabajada
                        Layer.Period.Begin = LayerWorking.Period.Begin
                    Else
                        Exit Sub
                    End If
                End If

                FirstBeginMandatory = Layer.Period.Begin.Value
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetStartLimitFromPunch")
                FirstBeginMandatory = roNullDate
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetStartLimitFromPunch")
                FirstBeginMandatory = roNullDate
            Finally

            End Try

        End Sub

        Public Shared Function ProcessLayer_ExistOnMandatory(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByVal Punch As String, ByVal FloatingTime As Double, ByRef mLayer As roTimeBlockItem, ByRef LayersCollection As roCollection, ByVal IDEmployee As Double, ByRef z As roEngineData, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa una capa de horas obligadas o rigidas
            '
            Dim Layer As New roTimeBlockItem
            Dim LayerBeginValue As Double
            Dim LayerFinishValue As Double
            Dim oXML As String
            Dim oCollection As New roCollection
            Dim dblLayer As Double
            Dim i As Integer
            Dim bolRet As Boolean = False

            Try

                ' Obtenemos el periodo de obligadas
                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))

                ' Mira si hay entrada flotante
                If ShiftLayer.Data.Exists("FloatingBeginUpto") Then
                    ' Hay entrada flotante, mira cuando ha entrado
                    Layer.Period.Begin = ProcessLayer_Mandatory_GetFloatingBegin(Layer.Period.Begin, Any2Time(DateTimeAdd(zDate.Value, ShiftLayer.Data("FloatingBeginUpto"))), z.TimeBlockList, Nothing, Nothing, oState)
                End If

                ' Si tiene hora de inicio variable
                If ShiftLayer.Data.Exists("AllowModifyIniHour") Then
                    oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                    If Len(oXML) > 0 Then
                        Try
                            oCollection.LoadXMLString(oXML)
                            ' Obtenemos la hora de la franja correspondiente
                            For i = 1 To 2
                                If oCollection.Exists("LayerID_" & i) Then
                                    If Any2Double(ShiftLayer.ID) = Any2Double(oCollection("LayerID_" & i)) Then
                                        dblLayer = i
                                        Exit For
                                    End If
                                End If
                            Next i
                            If dblLayer > 0 Then
                                If oCollection.Exists("LayerFloatingBeginTime_" & dblLayer) Then
                                    ' Asignamos la hora de inicio
                                    Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, oCollection("LayerFloatingBeginTime_" & dblLayer)), Any2Time(FloatingTime, True).Value))
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If

                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))

                If ShiftLayer.Data.Exists("FloatingFinishMinutes") Then
                    ' Si tambien hay salida flotante, establece en funcion de la entrada
                    Layer.Period.Finish = Layer.Period.Begin.Add(ShiftLayer.Data("FloatingFinishMinutes"), "n")

                End If

                ' Si tiene duracion variable a partir de la entrada
                If ShiftLayer.Data.Exists("AllowModifyDuration") Then
                    oXML = Any2String(ExecuteScalar("@SELECT# ISNULL(LayersDefinition, '') FROM DailySchedule with (nolock) WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime))
                    If Len(oXML) > 0 Then
                        Try
                            oCollection.LoadXMLString(oXML)
                            ' Obtenemos la hora de la franja correspondiente
                            dblLayer = 0
                            For i = 1 To 2
                                If oCollection.Exists("LayerID_" & i) Then
                                    If Any2Double(ShiftLayer.ID) = Any2Double(oCollection("LayerID_" & i)) Then
                                        dblLayer = i
                                        Exit For
                                    End If
                                End If
                            Next i
                            If dblLayer > 0 Then
                                If oCollection.Exists("LayerFloatingDuration_" & dblLayer) Then
                                    ' Asignamos la hora de finalizacion indicada
                                    Layer.Period.Finish = Layer.Period.Begin.Add(oCollection("LayerFloatingDuration_" & dblLayer), "n")
                                End If
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End If

                ' Cachea datos
                Layer.BlockType = roTimeBlockItem.roBlockType.roBTWorking
                LayerBeginValue = Layer.Period.Begin.VBNumericValue
                LayerFinishValue = Layer.Period.Finish.VBNumericValue

                ' Añadimos a la coleccion de franjas
                LayersCollection.Add(ShiftLayer.ID)

                ' Si el fichaje esta dentro de los limites de la franja
                If Layer.Period.Begin.VBNumericValue <= Any2Time(Punch).VBNumericValue And Layer.Period.Finish.VBNumericValue > Any2Time(Punch).VBNumericValue Then
                    ' Nos guardamos el periodo
                    bolRet = True
                End If

                mLayer.Period.Begin = Layer.Period.Begin
                mLayer.Period.Finish = Layer.Period.Finish
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnMandatory")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnMandatory")
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function ProcessLayer_ExistOnBreak(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByVal Punch As String, ByVal FloatingTime As Double, ByRef mLayer As roTimeBlockItem, ByRef LayersCollection As roCollection, ByVal oMandatoryLayerList As roCollection, ByRef oState As roEngineState) As Boolean
            '
            ' Procesa una capa de descansos
            '
            Dim Layer As New roTimeBlockItem
            Dim bolRet As Boolean = False

            Dim FirstLayerMandatory As New roTimeBlockItem
            Dim index As Integer = 0

            Try

                ' Obtenemos el periodo de descanso
                ' En el caso que el horario sea por horas y tenga definido el descano por duracion, aplicamos el periodo correspondiente
                ' Aplicamos la configuracion que corresponda para cada franja rígida
                If oMandatoryLayerList.Count > 0 And Any2Time(Any2Time(ShiftLayer.Data("Begin")).DateOnly).Value = Any2Time("1899/12/01").Value Then
                    ' Obtenemos la primera franja obligada
                    For index = 1 To oMandatoryLayerList.Count
                        If index = 1 Then
                            FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                        Else
                            If FirstLayerMandatory.Period.Begin.NumericValue > oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex).Period.Begin.NumericValue Then
                                FirstLayerMandatory = oMandatoryLayerList.Item(index, roCollection.roSearchMode.roByIndex)
                            End If
                        End If
                    Next index

                    ' Sobre la primera franja sumamos las duraciones para crear el periodo de descanso
                    Layer.Period.Begin = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Begin")).TimeOnly))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(FirstLayerMandatory.Period.Begin.Value, Any2Time(ShiftLayer.Data("Finish")).TimeOnly))
                Else
                    Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                    Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))
                End If

                Layer.BlockType = roTimeBlockItem.roBlockType.roBTBreak

                ' Si el fichaje esta dentro de los limites de la franja
                If Layer.Period.Begin.VBNumericValue <= Any2Time(Punch).VBNumericValue And Layer.Period.Finish.VBNumericValue > Any2Time(Punch).VBNumericValue Then
                    ' Nos guardamos el periodo
                    bolRet = True
                End If

                mLayer.Period.Begin = Layer.Period.Begin
                mLayer.Period.Finish = Layer.Period.Finish

                LayersCollection.Add(ShiftLayer.ID)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnBreak")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnBreak")
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Function ProcessLayer_ExistOnWorkingMaxMinFilter(ByRef zDate As roTime, ByRef ShiftLayer As roShiftEngineLayer, ByVal Punch As String, ByVal FloatingTime As Double, ByRef mLayer As roTimeBlockItem, ByRef LayersCollection As roCollection, ByRef oState As roEngineState, ByVal zShift As roShiftEngine, ByVal IDEmployee As Long) As Boolean
            '
            ' Procesa una capa de filtros de horas máximas/minimas
            '
            Dim Layer As New roTimeBlockItem
            Dim bolRet As Boolean = False
            Dim DateTime As New roTime
            Dim StartHour As Double = 0
            Dim EndHour As Double = 0
            Dim MinFilterValue As Double = 0

            Try
                bolRet = False

                ' Obtenemos el periodo del filtro
                If Not ShiftLayer.Data.Exists("Begin") Then
                    Return bolRet
                    Exit Function
                End If
                If Not ShiftLayer.Data.Exists("Finish") Then
                    Return bolRet
                    Exit Function
                End If

                Layer.Period.Begin = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Begin")), Any2Time(FloatingTime, True).Value))
                Layer.Period.Finish = Any2Time(DateTimeAdd(DateTimeAdd(zDate.Value, ShiftLayer.Data("Finish")), Any2Time(FloatingTime, True).Value))

                ' Si es un horario Starter
                If zShift.AdvancedParameters.Contains("Starter=[1]") Then
                    Dim StarterDs As System.Data.DataTable = CreateDataTable("@SELECT# StartFlexible1,EndFlexible1,isnull(ExpectedWorkingHours,0) as Hours FROM DailySchedule with (nolock)  WHERE IDEMPLOYEE=" & IDEmployee.ToString & " AND DATE=" & zDate.SQLSmallDateTime)
                    If StarterDs IsNot Nothing AndAlso StarterDs.Rows.Count > 0 Then
                        If Not IsDBNull(StarterDs.Rows(0)("StartFlexible1")) AndAlso Not IsDBNull(StarterDs.Rows(0)("EndFlexible1")) Then
                            DateTime = zDate
                            StartHour = Any2Time(StarterDs.Rows(0)("StartFlexible1"), True).NumericValue
                            EndHour = Any2Time(StarterDs.Rows(0)("EndFlexible1"), True).NumericValue

                            If StartHour <> 0 Or EndHour <> 0 Then
                                Layer.Period.Begin = DateTime.Add(Any2Time(StartHour))
                                Layer.Period.Finish = DateTime.Add(Any2Time(EndHour))
                                MinFilterValue = Any2Time(StarterDs.Rows(0)("Hours")).NumericValue
                            End If
                        End If
                    End If
                Else
                    MinFilterValue = Any2Time(ShiftLayer.Data("MinTime")).NumericValue
                End If

                ' Si el fichaje es posterior al inicio + Min es correcto no intersecciona
                If Any2Time(Punch).NumericValue >= (Layer.Period.Begin.NumericValue + MinFilterValue) Then
                    mLayer.Period.Begin = Layer.Period.Begin
                    mLayer.Period.Finish = Any2Time(Layer.Period.Begin.NumericValue + MinFilterValue)

                    ' Añadimos a la coleccion de franjas
                    LayersCollection.Add(ShiftLayer.ID)
                    Return bolRet
                    Exit Function
                End If

                Layer.Period.Begin = Any2Time(Layer.Period.Finish.NumericValue - MinFilterValue)

                ' Si el fichaje esta dentro de los limites de la franja
                If Layer.Period.Begin.VBNumericValue <= Any2Time(Punch).VBNumericValue And Layer.Period.Finish.VBNumericValue > Any2Time(Punch).VBNumericValue Then
                    ' Nos guardamos el periodo
                    bolRet = True
                End If

                mLayer.Period.Begin = Layer.Period.Begin
                mLayer.Period.Finish = Layer.Period.Finish

                ' Añadimos a la coleccion de franjas
                LayersCollection.Add(ShiftLayer.ID)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnWorkingMaxMinFilter")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::ProcessLayer_ExistOnWorkingMaxMinFilter")
            Finally

            End Try

            Return bolRet
        End Function

        Public Shared Sub GetLastPunch(ByVal IDEmployee As Long, ByRef oState As roEngineState, ByRef LastPunch As String, ByRef LastIDCause As Double, ByRef IsPresent As Boolean, ByRef Operation As String, ByRef ShiftDate As String)
            '
            ' Obtenemos el utlimo fichaje del empleado y su estado
            '
            Dim LastOutID As Long
            Dim LastOutIDCause As Double
            Dim LastInIDCause As Double
            Dim LastOut As New roTime
            Dim LastOutShiftDate As New roTime
            Dim LastInID As Long
            Dim LastIn As New roTime
            Dim LastInShiftDate As New roTime
            Dim bolIsPresent As Boolean
            Dim strLastPunch As String = Any2Time(Format$(Now, "yyyy/MM/dd HH:mm")).Value
            Dim strLastIDCause As String = "0"
            Dim strLastShiftDate As String = ""
            Dim strType As String = "S"
            Dim m_MaxPeriodHours As Double = 0

            Try

                ' Obtenemos ultima entrada
                Dim RS As DataTable = CreateDataTable("@SELECT# TOP 1 ID,DateTime, TypeData as IDCause, ShiftDate FROM Punches with (nolock) WHERE IDEmployee=" & IDEmployee.ToString & " AND ActualType = 1 ORDER BY DateTime DESC, ID DESC")
                If RS IsNot Nothing AndAlso RS.Rows.Count > 0 Then
                    LastIn = Any2Time(Format$(RS.Rows(0)("DateTime"), "yyyy/MM/dd HH:mm"))
                    LastInShiftDate = Any2Time(RS.Rows(0)("ShiftDate"))
                    LastInID = RS.Rows(0)("ID")
                    LastInIDCause = Any2Double(RS.Rows(0)("IDCause"))
                Else
                    LastIn = Nothing
                    LastInID = 0
                    LastInShiftDate = Nothing
                End If

                ' Obtenemos ultima salida
                RS = New DataTable
                RS = CreateDataTable("@SELECT# TOP 1 ID,DateTime, TypeData as IDCause, ShiftDate FROM Punches with (nolock) WHERE IDEmployee=" & IDEmployee & " AND ActualType = 2  ORDER BY DateTime DESC, ID DESC")
                If RS IsNot Nothing AndAlso RS.Rows.Count > 0 Then
                    LastOut = Any2Time(Format$(RS.Rows(0)("DateTime"), "yyyy/MM/dd HH:mm"))
                    LastOutShiftDate = Any2Time(RS.Rows(0)("ShiftDate"))
                    LastOutID = RS.Rows(0)("ID")
                    LastOutIDCause = Any2Double(RS.Rows(0)("IDCause"))
                Else
                    LastOut = Nothing
                    LastOutID = 0
                    LastOutShiftDate = Nothing
                End If

                ' Si no tiene fichajes,  no esta
                If LastInID = 0 And LastOutID = 0 Then
                    strType = "S"
                    strLastPunch = ""
                    strLastShiftDate = ""
                    ' Si solo tiene una entrada
                ElseIf LastInID <> 0 And LastOutID = 0 Then
                    strType = "E"
                    strLastPunch = LastIn.Value
                    strLastShiftDate = LastInShiftDate.Value
                    strLastIDCause = LastInIDCause
                    bolIsPresent = True
                    ' Si solo tiene una salida
                ElseIf LastInID = 0 And LastOutID <> 0 Then
                    strType = "S"
                    strLastPunch = LastOut.Value
                    strLastShiftDate = LastOutShiftDate.Value
                    strLastIDCause = LastOutIDCause
                    ' Si tiene entrada y salida
                ElseIf LastInID <> 0 And LastOutID <> 0 Then
                    strType = IIf(LastIn.NumericValue >= LastOut.NumericValue, "E", "S")
                    strLastPunch = IIf(LastIn.NumericValue >= LastOut.NumericValue, LastIn.Value, LastOut.Value)
                    strLastIDCause = IIf(LastIn.NumericValue >= LastOut.NumericValue, LastInIDCause, LastOutIDCause)
                    bolIsPresent = IIf(LastIn.NumericValue >= LastOut.NumericValue, True, False)
                    strLastShiftDate = IIf(LastIn.NumericValue >= LastOut.NumericValue, LastInShiftDate.Value, LastOutShiftDate.Value)
                End If

                ' Si el ultimo fichaje es una entrada, miramos si ha excedido el tiempo entre entrada y salida
                ' en ese caso el empleado ya no esta presente, creemos que se ha olvidado el fichaje de salida
                If strType = "E" Then
                    ' Obtenemos el parametro de nº de horas entre entrada y salida
                    Try
                        'Dim oParameters As New roParameters("OPTIONS", True)
                        'Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                        Dim oTime As roTime = roTypes.Any2Time(DataLayer.roCacheManager.GetInstance().GetParametersCache(Azure.RoAzureSupport.GetCompanyName(), Parameters.MovMaxHours))
                        If oTime.IsValid Then m_MaxPeriodHours = oTime.NumericValue
                        If m_MaxPeriodHours = 0 Then m_MaxPeriodHours = 12

                        If Any2Time(Now).NumericValue > Any2Time(strLastPunch).NumericValue AndAlso Any2Time(Now).Substract(Any2Time(strLastPunch)).NumericValue > m_MaxPeriodHours Then
                            bolIsPresent = False
                        End If
                    Catch ex As Exception
                    End Try

                End If

                LastPunch = strLastPunch
                LastIDCause = Any2Double(strLastIDCause)
                IsPresent = bolIsPresent
                Operation = strType
                ShiftDate = strLastShiftDate
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetLastPunch")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetLastPunch")
                LastPunch = ""
                LastIDCause = 0
                IsPresent = False
                Operation = "S"
                ShiftDate = ""
            Finally

            End Try

        End Sub

        Public Shared Function GetShiftFromCache(ByVal idShift As Integer, ByRef oState As roEngineState) As roShiftEngine
            Dim oRet As roShiftEngine = Nothing

            Try

                oRet = DataLayer.roCacheManager.GetInstance().GetShiftCache(Azure.RoAzureSupport.GetCompanyName(), idShift)

                If oRet Is Nothing Then
                    ' Si el horario que solicito no está an caché, lo añado
                    oRet = New roShiftEngine
                    Dim oShiftManager As New roShiftManager(New roShiftManagerState(oState.IDPassport))
                    oRet = oShiftManager.Load(idShift)
                    If oRet IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateShiftCache(Azure.RoAzureSupport.GetCompanyName(), oRet)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetShiftFromCache")
            End Try

            Return oRet
        End Function

        Public Shared Function GetCauseFromCache(ByVal idCause As Integer, ByRef oState As roEngineState) As roCauseEngine
            Dim oRet As roCauseEngine = Nothing

            Try

                oRet = DataLayer.roCacheManager.GetInstance().GetCauseCache(Azure.RoAzureSupport.GetCompanyName(), idCause)

                If oRet Is Nothing Then
                    ' Si la justificación que solicito no está an caché, lo añado
                    oRet = New roCauseEngine
                    Dim oCauseManager As roCauseManager = New roCauseManager(New roCauseManagerState(oState.IDPassport))
                    oRet = oCauseManager.Load(idCause)
                    If oRet IsNot Nothing Then DataLayer.roCacheManager.GetInstance().UpdateCauseCache(Azure.RoAzureSupport.GetCompanyName(), oRet)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetCauseFromCache")
            End Try

            Return oRet
        End Function

        Public Shared Function GetLabAgreeeFromCache(ByVal idLabAgree As Integer, ByRef oState As roEngineState) As roLabAgreeEngine
            Dim oRet As roLabAgreeEngine = Nothing

            Try

                oRet = DataLayer.roCacheManager.GetInstance().GetLabAgreeCache(Azure.RoAzureSupport.GetCompanyName(), idLabAgree)

                If oRet Is Nothing Then
                    ' Si el convenio que solicito no está an caché, lo añado
                    oRet = New roLabAgreeEngine
                    Dim oLabAgreeManager As roLabAgreeManager = New roLabAgreeManager(New roLabAgreeManagerState(oState.IDPassport))
                    oRet = oLabAgreeManager.Load(idLabAgree)
                    DataLayer.roCacheManager.GetInstance().UpdateLabAgreeCache(Azure.RoAzureSupport.GetCompanyName(), oRet)
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetLabAgreeeFromCache")
            End Try

            Return oRet
        End Function

        Public Shared Function GetConceptFromCache(ByVal idConcept As Integer, ByRef oState As roEngineState) As roConceptEngine
            Dim oRet As roConceptEngine = Nothing

            Try

                oRet = DataLayer.roCacheManager.GetInstance().GetConceptCache(Azure.RoAzureSupport.GetCompanyName, idConcept)

                If oRet Is Nothing Then
                    ' Si el saldo que solicito no está an caché, lo añado
                    oRet = New roConceptEngine
                    Dim oConceptManager As roConceptManager = New roConceptManager(New roConceptManagerState(oState.IDPassport))
                    oRet = oConceptManager.Load(idConcept)
                    If oRet IsNot Nothing Then
                        ' Los saldos personalizados se guardan como de tipo H, pero se tratan como de tipo O ...
                        If oRet.CustomType Then oRet.IDType = "O"
                        DataLayer.roCacheManager.GetInstance().UpdateConceptCache(Azure.RoAzureSupport.GetCompanyName, oRet)
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::GetConceptFromCache")
            End Try

            Return oRet
        End Function

        Public Shared Function ConceptsCache(ByRef oState As roEngineState) As Hashtable
            Dim oRet As Hashtable = Nothing
            Try
                oRet = DataLayer.roCacheManager.GetInstance().GetConceptsCache(Azure.RoAzureSupport.GetCompanyName)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::AccrualsCache")
            End Try

            Return oRet
        End Function

        Public Shared Function DailyScheduleGUIDChangedOrStatusOverwritted(ByVal idEmployee As Long, ByVal ScheduleDate As Date, ByVal previousProcessPriority As Integer, ByRef oState As roEngineState) As Boolean
            '
            ' Verificamos si el GUID del dia ha sido modificado posteriormente al iniciar el proceso de cálculo
            '

            Dim oRet As Boolean = False

            Dim CurrentGUID As String = String.Empty
            Dim currentStatus As Integer

            Try
                ' Miro el GUID actual del registro que me planteo actualizar
                Dim sSQL As String = "@SELECT# GUID, Status FROM DAILYSCHEDULE with (nolock) WHERE IDEmployee=" & idEmployee.ToString & " AND Date=" & Any2Time(ScheduleDate).SQLSmallDateTime
                Dim result As DataTable = CreateDataTable(sSQL)
                If result IsNot Nothing AndAlso result.Rows.Count > 0 Then
                    CurrentGUID = Any2String(result.Rows(0)("GUID"))
                    currentStatus = Any2Integer(result.Rows(0)("Status"))
                End If

                If CurrentGUID <> VTBase.roConstants.GetManagedThreadGUID() Then
                    oRet = True
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roBaseEngineManager::DailyScheduleGUIDChanged: GUID Changed:EmployeeID:" & idEmployee & " " & ScheduleDate & " --> " & CurrentGUID & "    -    " & VTBase.roConstants.GetManagedThreadGUID())
                ElseIf currentStatus < previousProcessPriority Then
                    oRet = True
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roBaseEngineManager::DailyScheduleGUIDChanged: Status Changed while processing:EmployeeID " & idEmployee & ", date " & ScheduleDate & ", current status " & currentStatus & ", status expected " & previousProcessPriority & ". Day will be calculated later!")
                End If

                Return oRet
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBaseEngineManager::DailyScheduleGUIDChanged")
            End Try

            Return oRet
        End Function

#End Region

    End Class

End Namespace