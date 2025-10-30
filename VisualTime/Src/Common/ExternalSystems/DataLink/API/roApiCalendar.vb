Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roApiCalendar
        Inherits roDataLinkApi


        Protected ReadOnly Property ImportEngine As roCalendarImport
            Get
                Return CType(Me.oDataImport, roCalendarImport)
            End Get
        End Property

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roCalendarImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If

        End Sub

        Public Function GetHolidays(ByRef oHolidays As Generic.List(Of RoboticsExternAccess.roHoliday), ByVal employeeID As String, ByVal StartDate As Date, ByVal EndDate As Date, ByRef strErrorMsg As String, ByRef iReturnCode As RoboticsExternAccess.Core.DTOs.ReturnCode, Optional ByVal GetHolidaysByTimestamp As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim iIdEmployee As Integer = 0

            Try
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError

                ' Obtenemos el campo identificador del empleado
                Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                oHolidays = New Generic.List(Of RoboticsExternAccess.roHoliday)

                If employeeID <> String.Empty Then
                    Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(strImportPrimaryKeyUserField, employeeID, Now, New UserFields.roUserFieldState(-1))
                    If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
                End If

                Dim strSQL As String = String.Empty
                If GetHolidaysByTimestamp Then
                    If iIdEmployee > 0 Then
                        strSQL = "@SELECT# Dailyschedule.Date, dbo.GetEmployeeUserFieldValueMin(Dailyschedule.IdEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, isnull(Shifts.Export,'') as ExportCode, Shifts.Name as ShiftName, Dailyschedule.TimestampHolidays as Timestamp  FROM  Dailyschedule inner join Shifts on Dailyschedule.idShift1 = Shifts.id WHERE  idemployee = " & iIdEmployee.ToString & "AND Dailyschedule.TimestampHolidays >=" & Any2Time(StartDate).SQLSmallDateTime & " AND Dailyschedule.TimestampHolidays <= " & Any2Time(EndDate).SQLSmallDateTime & " AND isnull(IsHolidays,0) = 1 "
                    Else
                        strSQL = "@SELECT# Dailyschedule.Date, dbo.GetEmployeeUserFieldValueMin(Dailyschedule.IdEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, Dailyschedule.IdEmployee, isnull(Shifts.Export,'') as ExportCode,  Shifts.Name as ShiftName, Dailyschedule. TimestampHolidays as Timestamp  FROM  Dailyschedule inner join Shifts on Dailyschedule.idShift1 = Shifts.id WHERE Dailyschedule.TimestampHolidays >=" & Any2Time(StartDate).SQLSmallDateTime & " AND Dailyschedule.TimestampHolidays <= " & Any2Time(EndDate).SQLSmallDateTime & " AND isnull(IsHolidays,0) = 1 "
                    End If
                Else
                    If iIdEmployee > 0 Then
                        strSQL = "@SELECT# Dailyschedule.Date, dbo.GetEmployeeUserFieldValueMin(Dailyschedule.IdEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, isnull(Shifts.Export,'') as ExportCode, Shifts.Name as ShiftName, TimestampHolidays as Timestamp  FROM  Dailyschedule inner join Shifts on Dailyschedule.idShift1 = Shifts.id WHERE  idemployee = " & iIdEmployee.ToString & " and date >=" & Any2Time(StartDate).SQLSmallDateTime & " And date <= " & Any2Time(EndDate).SQLSmallDateTime & " AND isnull(IsHolidays,0) = 1 "
                    Else
                        strSQL = "@SELECT# Dailyschedule.Date, dbo.GetEmployeeUserFieldValueMin(Dailyschedule.IdEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, Dailyschedule.IdEmployee, isnull(Shifts.Export,'') as ExportCode,  Shifts.Name as ShiftName, TimestampHolidays as Timestamp  FROM  Dailyschedule inner join Shifts on Dailyschedule.idShift1 = Shifts.id WHERE  Date >=" & Any2Time(StartDate).SQLSmallDateTime & " And date <= " & Any2Time(EndDate).SQLSmallDateTime & " AND isnull(IsHolidays,0) = 1 "
                    End If
                End If

                ' Cargamos lista de dias de vacaciones
                Dim oEmployeesDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)
                If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                    For Each orow As DataRow In oEmployeesDT.Rows
                        Dim oHoliday As New RoboticsExternAccess.roHoliday
                        oHoliday.Action = "CRU"
                        oHoliday.IDEmployee = roTypes.Any2String(orow("EmployeeKey"))
                        oHoliday.HolidayType = RoboticsExternAccess.HolidayType_Enum.Days
                        oHoliday.PlannedDate = New roWCFDate(CDate(orow("Date")))
                        oHoliday.IDReason = orow("ExportCode")
                        oHoliday.ReasonName = orow("ShiftName")

                        oHoliday.TimeStamp = New roWCFDate(roTypes.Any2DateTime(orow("TimeStamp")))

                        oHolidays.Add(oHoliday)
                    Next
                End If

                If GetHolidaysByTimestamp Then
                    If iIdEmployee > 0 Then
                        ' Cargamos lista de previsiones de horas de vacaciones
                        strSQL = "@SELECT# ProgrammedHolidays.ID, IDEmployee,IDCause, Date, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, " &
                                    "ProgrammedHolidays.AllDay ,  isnull(Causes.Export,'') as ExportCode, dbo.GetEmployeeUserFieldValueMin(ProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, Causes.Name as CauseName, ProgrammedHolidays.Timestamp " &
                             "FROM ProgrammedHolidays " &
                                        "LEFT JOIN Causes On Causes.ID = ProgrammedHolidays.IDCause " &
                             "WHERE idEmployee = " & iIdEmployee.ToString
                        strSQL = strSQL & " AND ProgrammedHolidays.Timestamp >=" & Any2Time(StartDate).SQLSmallDateTime & " AND ProgrammedHolidays.Timestamp <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY ProgrammedHolidays.Timestamp asc"
                    Else
                        ' Cargamos lista de previsiones de horas de vacaciones
                        strSQL = "@SELECT# ProgrammedHolidays.ID, IDEmployee,IDCause, Date, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, " &
                                    "ProgrammedHolidays.AllDay ,  isnull(Causes.Export,'') as ExportCode, dbo.GetEmployeeUserFieldValueMin(ProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey , Causes.Name as CauseName, ProgrammedHolidays.Timestamp  " &
                             "FROM ProgrammedHolidays " &
                                        "LEFT JOIN Causes On Causes.ID = ProgrammedHolidays.IDCause " &
                             "WHERE 1=1 "
                        strSQL = strSQL & " AND ProgrammedHolidays.Timestamp >=" & Any2Time(StartDate).SQLSmallDateTime & " AND ProgrammedHolidays.Timestamp <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY ProgrammedHolidays.Timestamp asc"
                    End If
                Else
                    If iIdEmployee > 0 Then
                        ' Cargamos lista de previsiones de horas de vacaciones
                        strSQL = "@SELECT# ProgrammedHolidays.ID, IDEmployee,IDCause, Date, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, " &
                                    "ProgrammedHolidays.AllDay ,  isnull(Causes.Export,'') as ExportCode, dbo.GetEmployeeUserFieldValueMin(ProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey, Causes.Name as CauseName, ProgrammedHolidays.Timestamp " &
                             "FROM ProgrammedHolidays " &
                                        "LEFT JOIN Causes On Causes.ID = ProgrammedHolidays.IDCause " &
                             "WHERE idEmployee = " & iIdEmployee.ToString
                        strSQL = strSQL & " AND date >=" & Any2Time(StartDate).SQLSmallDateTime & " AND date <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY Date asc"
                    Else
                        ' Cargamos lista de previsiones de horas de vacaciones
                        strSQL = "@SELECT# ProgrammedHolidays.ID, IDEmployee,IDCause, Date, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, " &
                                    "ProgrammedHolidays.AllDay ,  isnull(Causes.Export,'') as ExportCode, dbo.GetEmployeeUserFieldValueMin(ProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey , Causes.Name as CauseName, ProgrammedHolidays.Timestamp  " &
                             "FROM ProgrammedHolidays " &
                                        "LEFT JOIN Causes On Causes.ID = ProgrammedHolidays.IDCause " &
                             "WHERE 1=1 "
                        strSQL = strSQL & " AND date >=" & Any2Time(StartDate).SQLSmallDateTime & " AND date <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY Date asc"
                    End If
                End If

                oEmployeesDT = CreateDataTableWithoutTimeouts(strSQL)
                If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                    For Each orow As DataRow In oEmployeesDT.Rows
                        Dim oHoliday As New RoboticsExternAccess.roHoliday
                        oHoliday.Action = "CRU"
                        oHoliday.IDEmployee = roTypes.Any2String(orow("EmployeeKey"))
                        oHoliday.HolidayType = RoboticsExternAccess.HolidayType_Enum.Hours
                        oHoliday.PlannedDate = New roWCFDate(CDate(orow("Date")))
                        oHoliday.BeginTime = New roWCFDate(CDate(orow("BeginTime")))
                        oHoliday.EndTime = New roWCFDate(CDate(orow("EndTime")))
                        oHoliday.Duration = orow("duration")
                        oHoliday.IdHoursHoliday = orow("ID")
                        oHoliday.ApplyAllDay = Any2Boolean(orow("AllDay"))
                        oHoliday.IDReason = orow("ExportCode")
                        oHoliday.ReasonName = orow("CauseName")

                        oHoliday.TimeStamp = New roWCFDate(roTypes.Any2DateTime(orow("TimeStamp")))

                        oHolidays.Add(oHoliday)
                    Next
                End If

                'Cargamos lista de vacaciones canceladas
                If GetHolidaysByTimestamp Then
                    If iIdEmployee > 0 Then
                        strSQL = "@SELECT# *, dbo.GetEmployeeUserFieldValueMin(DeletedProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey  " &
                         "FROM DeletedProgrammedHolidays " &
                         "WHERE idEmployee = " & iIdEmployee.ToString
                        strSQL = strSQL & " AND timestamp >=" & Any2Time(StartDate).SQLSmallDateTime & " AND timestamp <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY timestamp asc"
                    Else
                        strSQL = "@SELECT# *, dbo.GetEmployeeUserFieldValueMin(DeletedProgrammedHolidays.IDEmployee,'" & strImportPrimaryKeyUserField & "', GETDATE()) as EmployeeKey   " &
                         "FROM DeletedProgrammedHolidays " &
                         "WHERE 1=1 "
                        strSQL = strSQL & " AND timestamp >=" & Any2Time(StartDate).SQLSmallDateTime & " AND timestamp <= " & Any2Time(EndDate).SQLSmallDateTime
                        strSQL = strSQL & " ORDER BY timestamp asc"
                    End If

                    oEmployeesDT = CreateDataTableWithoutTimeouts(strSQL)

                    If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                        For Each orow As DataRow In oEmployeesDT.Rows
                            Dim canceledHoliday As New RoboticsExternAccess.roHoliday
                            canceledHoliday.Action = "D"
                            canceledHoliday.IDEmployee = roTypes.Any2String(orow("EmployeeKey"))

                            If orow("IDHoursHoliday") Is DBNull.Value Then
                                canceledHoliday.PlannedDate = New roWCFDate(CDate(orow("PlannedDate")))
                                canceledHoliday.IdHoursHoliday = Nothing
                                canceledHoliday.TimeStamp = New roWCFDate(CDate(orow("TimeStamp")))
                                canceledHoliday.HolidayType = RoboticsExternAccess.HolidayType_Enum.Days
                            Else
                                canceledHoliday.PlannedDate = New roWCFDate(CDate(orow("PlannedDate")))
                                canceledHoliday.IdHoursHoliday = orow("IdHoursHoliday")
                                canceledHoliday.TimeStamp = New roWCFDate(CDate(orow("TimeStamp")))
                                canceledHoliday.HolidayType = RoboticsExternAccess.HolidayType_Enum.Hours
                            End If

                            oHolidays.Add(canceledHoliday)
                        Next
                    End If

                End If

                oHolidays = oHolidays.OrderBy(Function(x) x.PlannedDate.GetDate).ToList

                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetHolidays")
                bolRet = False
            End Try

            Return bolRet
        End Function


        Public Function CreateOrUpdateHolidays(ByVal oHolidaysData As RoboticsExternAccess.IDatalinkHolidays, ByRef strErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                bolRet = oHolidaysData.GetEmployeeColumnsDefinition(ColumnsVal, ColumnsPos)

                If bolRet Then
                    Dim tbShifts As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, isnull(Export,'') as Export FROM Shifts WHERE (ISNULL(TypeShift, '') = '') AND (ISNULL(ShiftType, -1) = 2)")

                    ' Graba vacaciones del horario
                    bolRet = ProcessHolidays(tbShifts, ColumnsVal, strErrorMsg, 1)
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid holidays object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateHolidays")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Private Function ProcessHolidays(ByVal tbShifts As DataTable, ByRef ColumnsVal() As String, ByRef msgLog As String, ByVal intRow As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                msgLog = ""
                Dim oEmployeeState As New Employee.roEmployeeState

                Dim action As String = ColumnsVal(RoboticsExternAccess.HolidaysAsciiColumns.Action)
                Dim ShiftKey As String = ColumnsVal(RoboticsExternAccess.HolidaysAsciiColumns.ShiftKey)

                Dim PlanDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.HolidaysAsciiColumns.PlanDate))

                ' Busca el empleado
                Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RoboticsExternAccess.HolidaysAsciiColumns.ImportPrimaryKey, RoboticsExternAccess.HolidaysAsciiColumns.NIF, New UserFields.roUserFieldState)
                If idEmployee > 0 Then
                    ' Busca el horario
                    Dim rw() As DataRow = tbShifts.Select("Export='" & ShiftKey & "'")
                    If rw.Length = 0 Then
                        Me.State.Result = DataLinkResultEnum.InvalidShift
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidShiftOnRegister", "") & " " & intRow & vbNewLine
                    Else
                        Dim idShift As Integer = rw(0)("id")

                        ' Crea el registro
                        ' TODO: Habria que utilizar alguna funcion del calendarManager para asignar planificacion a un empleado y dejar de utilizar el AssignShift
                        Select Case action
                            Case "I"
                                bolRet = Base.VTBusiness.Scheduler.roScheduler.AssignShift(idEmployee, PlanDate, idShift, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, -1, Base.DTOs.LockedDayAction.ReplaceAll, Base.DTOs.LockedDayAction.ReplaceAll, oEmployeeState, False, , True, Base.DTOs.ShiftPermissionAction.ContinueAll)
                                If bolRet = False Then msgLog = oEmployeeState.ErrorText

                            Case "U"
                                bolRet = Base.VTBusiness.Scheduler.roScheduler.AssignShift(idEmployee, PlanDate, idShift, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, -1, Base.DTOs.LockedDayAction.ReplaceAll, Base.DTOs.LockedDayAction.ReplaceAll, oEmployeeState, False, , True, Base.DTOs.ShiftPermissionAction.ContinueAll)
                                If bolRet = False Then msgLog = oEmployeeState.ErrorText

                            Case "D"
                                bolRet = Base.VTBusiness.Scheduler.roScheduler.AssignShift(idEmployee, PlanDate, -1, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, -1, Base.DTOs.LockedDayAction.ReplaceAll, Base.DTOs.LockedDayAction.ReplaceAll, oEmployeeState, False, , True, Base.DTOs.ShiftPermissionAction.ContinueAll)
                                If bolRet = False Then msgLog = oEmployeeState.ErrorText
                        End Select
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEmployee", "") & " " & intRow & vbNewLine
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ProcessHolidays")
            End Try

            Return bolRet

        End Function

        Public Function CreateOrUpdateCalendar(ByVal oCalendarData As RoboticsExternAccess.IDatalinkCalendar, ByRef strErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}

                bolRet = oCalendarData.GetEmployeeColumnsDefinition(ColumnsVal, ColumnsPos)

                If bolRet Then
                    ' TODO: En esta version solo dejamos planificar horarios normales o de Vacaciones
                    ' Por horas o flotantes excluidos, cuando se deban utilizar se deberán pedir mas parametros al WS
                    ' Excluido tambien informacion de puesto
                    Dim tbShifts As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, isnull(Export,'') as Export, ShiftType, AllowComplementary, AllowFloatingData, IsFloating FROM Shifts WHERE (ISNULL(TypeShift, '') = '') AND (ISNULL(ShiftType, -1) IN(0,1,2))")

                    ' Graba el horario
                    bolRet = ProcessCalendar(tbShifts, ColumnsVal, strErrorMsg, 1)
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid calendar object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateCalendar")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, True)
            End Try

            Return bolRet
        End Function

        Private Function ProcessCalendar(ByVal tbShifts As DataTable, ByRef ColumnsVal() As String, ByRef msgLog As String, ByVal intRow As Integer) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                msgLog = ""
                Dim oEmployeeState As New Employee.roEmployeeState

                Dim ShiftKey As String = ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.ShiftKey)

                Dim PlanDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.PlanDate))

                ' Busca el empleado
                Dim idEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal, RoboticsExternAccess.CalendarAsciiColumns.ImportPrimaryKey, RoboticsExternAccess.CalendarAsciiColumns.NIF, New UserFields.roUserFieldState)
                If idEmployee > 0 Then
                    ' Busca el horario
                    Dim rw() As DataRow = tbShifts.Select("Export='" & ShiftKey & "'")
                    If rw.Length = 0 Then
                        Me.State.Result = DataLinkResultEnum.InvalidShift
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidShiftOnRegister", "") & " " & intRow & vbNewLine
                    Else
                        Dim idShift As Integer = rw(0)("id")
                        ' Asignamos el horario al empleado

                        Dim intIDPassport As Integer = roConstants.GetSystemUserId()
                        If intIDPassport > 0 Then
                            Dim oCalendarState As New roCalendarState(intIDPassport)
                            Dim oCalendarManager As New roCalendarManager(oCalendarState)

                            ' Cargamos el calendario del empleado para ese dia
                            Dim oSourceCalendar As New Robotics.Base.DTOs.roCalendar
                            oSourceCalendar = oCalendarManager.Load(PlanDate, PlanDate, "B" & idEmployee.ToString, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                            ' Cargamos los datos del horario al dia planificado
                            Dim oNewCalendarRowDayData As New roCalendarRowDayData
                            oNewCalendarRowDayData.MainShift = New roCalendarRowShiftData
                            oNewCalendarRowDayData.MainShift = oCalendarManager.LoadShiftDayDataByIdShift(idShift)
                            oNewCalendarRowDayData.AltShift1 = Nothing
                            oNewCalendarRowDayData.AltShift2 = Nothing
                            oNewCalendarRowDayData.AltShift3 = Nothing
                            oNewCalendarRowDayData.IsHoliday = False
                            oNewCalendarRowDayData.ShiftBase = Nothing
                            oNewCalendarRowDayData.ShiftUsed = Nothing
                            oNewCalendarRowDayData.PlanDate = PlanDate
                            oNewCalendarRowDayData.AssigData = Nothing

                            If roTypes.Any2Integer(rw(0)("ShiftType")) = 1 Then
                                If roTypes.Any2Boolean(rw(0)("AllowComplementary")) OrElse roTypes.Any2Boolean(rw(0)("AllowFloatingData")) Then
                                    If ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer1StartTime).Length > 0 AndAlso oNewCalendarRowDayData.MainShift.ShiftLayers > 0 Then
                                        oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerStartTime = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer1StartTime))
                                        oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerOrdinaryHours = roTypes.Any2Integer(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer1OrdinaryHours))
                                        oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours = roTypes.Any2Integer(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer1ComplementaryHours))
                                        oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerDuration = oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerOrdinaryHours + oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerComplementaryHours
                                        oNewCalendarRowDayData.MainShift.PlannedHours = oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerDuration

                                        If ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer2StartTime).Length > 0 AndAlso oNewCalendarRowDayData.MainShift.ShiftLayers > 1 Then
                                            oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerStartTime = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer2StartTime))
                                            oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerOrdinaryHours = roTypes.Any2Integer(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer2OrdinaryHours))
                                            oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours = roTypes.Any2Integer(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer2ComplementaryHours))
                                            oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerDuration = oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerOrdinaryHours + oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerComplementaryHours

                                            oNewCalendarRowDayData.MainShift.PlannedHours = oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(0).LayerDuration + oNewCalendarRowDayData.MainShift.ShiftLayersDefinition(1).LayerDuration
                                        End If
                                    End If
                                ElseIf roTypes.Any2Boolean(rw(0)("IsFloating")) Then
                                    oNewCalendarRowDayData.MainShift.StartHour = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.Layer1StartTime))
                                End If
                            End If

                            Dim bolCopyTelecommute As Boolean = False
                            If ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.CanTelecommute).Length > 0 AndAlso ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.CanTelecommute) = "1" Then

                                If oSourceCalendar IsNot Nothing AndAlso oSourceCalendar.CalendarData IsNot Nothing Then
                                    For Each oSourceCalendarRowEmployeeData As roCalendarRow In oSourceCalendar.CalendarData
                                        If oSourceCalendarRowEmployeeData IsNot Nothing AndAlso oSourceCalendarRowEmployeeData.PeriodData IsNot Nothing AndAlso oSourceCalendarRowEmployeeData.PeriodData.DayData.Count = 1 Then
                                            oNewCalendarRowDayData.TelecommutingMandatoryDays = oSourceCalendarRowEmployeeData.PeriodData.DayData(0).TelecommutingMandatoryDays
                                            oNewCalendarRowDayData.TelecommutingOptionalDays = oSourceCalendarRowEmployeeData.PeriodData.DayData(0).TelecommutingOptionalDays
                                            oNewCalendarRowDayData.PresenceMandatoryDays = oSourceCalendarRowEmployeeData.PeriodData.DayData(0).PresenceMandatoryDays
                                            oNewCalendarRowDayData.CanTelecommute = oSourceCalendarRowEmployeeData.PeriodData.DayData(0).CanTelecommute
                                        End If
                                    Next
                                End If

                                If oNewCalendarRowDayData.CanTelecommute IsNot Nothing AndAlso oNewCalendarRowDayData.CanTelecommute Then
                                    Dim bolApplyDay As Boolean = False
                                    bolApplyDay = oNewCalendarRowDayData.TelecommutingMandatoryDays.Contains(PlanDate.DayOfWeek)
                                    If Not bolApplyDay Then bolApplyDay = oNewCalendarRowDayData.TelecommutingOptionalDays.Contains(PlanDate.DayOfWeek)
                                    If Not bolApplyDay Then bolApplyDay = oNewCalendarRowDayData.PresenceMandatoryDays.Contains(PlanDate.DayOfWeek)
                                    If bolApplyDay Then
                                        oNewCalendarRowDayData.CanTelecommute = True
                                        bolCopyTelecommute = True
                                        ' Si tiene valor debemos forzar un valor
                                        oNewCalendarRowDayData.TelecommuteForced = False
                                        oNewCalendarRowDayData.TelecommutingExpected = False
                                        oNewCalendarRowDayData.TelecommutingOptional = False

                                        If ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.TelecommuteForced) = "0" Then
                                            oNewCalendarRowDayData.TelecommutingExpected = oNewCalendarRowDayData.TelecommutingMandatoryDays.Contains(PlanDate.DayOfWeek)
                                            oNewCalendarRowDayData.TelecommutingOptional = oNewCalendarRowDayData.TelecommutingOptionalDays.Contains(PlanDate.DayOfWeek)
                                        Else
                                            oNewCalendarRowDayData.TelecommuteForced = True
                                            Select Case [Enum].GetName(GetType(TelecommutingType_Enum), CInt(ColumnsVal(RoboticsExternAccess.CalendarAsciiColumns.TelecommutingStatus)))
                                                Case TelecommutingType_Enum.Telecommuting.ToString ' Teletrabajo
                                                    oNewCalendarRowDayData.TelecommutingExpected = True
                                                Case TelecommutingType_Enum.Presence.ToString ' Presencial
                                                    oNewCalendarRowDayData.TelecommutingExpected = False
                                                Case TelecommutingType_Enum.TelecommutingOptional.ToString ' Opcional
                                                    oNewCalendarRowDayData.TelecommutingOptional = True
                                                Case Else
                                                    bolCopyTelecommute = False
                                            End Select
                                        End If
                                    Else
                                        oNewCalendarRowDayData.CanTelecommute = False
                                    End If
                                End If
                            End If

                            bolRet = oCalendarManager.AddCalendarRowDayData(oSourceCalendar, oNewCalendarRowDayData, idEmployee, True, True, False, False, True, False, False, False, False, bolCopyTelecommute)
                            If bolRet Then
                                ' Guardamos los cambios
                                Dim oCalendarResult As roCalendarResult = oCalendarManager.Save(oSourceCalendar, False, False)

                                If oCalendarResult.Status = CalendarStatusEnum.OK Then
                                    Me.State.Result = DataLinkResultEnum.NoError
                                    bolRet = True
                                Else
                                    bolRet = False
                                    Me.State.Result = DataLinkResultEnum.InvalidData
                                    If oCalendarResult.CalendarDataResult IsNot Nothing Then
                                        'Mostramos el primer error que tenga el resultado de guardar el calendario
                                        msgLog = oEmployeeState.ErrorText = oCalendarResult.CalendarDataResult(0).ErrorText
                                    End If
                                End If
                            Else
                                Me.State.Result = DataLinkResultEnum.InvalidData
                            End If
                        End If

                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEmployee", "") & " " & intRow & vbNewLine
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ProcessCalendar")
            End Try

            Return bolRet

        End Function




        ''' <summary>
        ''' Función para obtener la definicion de los horarios
        ''' </summary>
        ''' <param name="oEmployees"></param>
        ''' <param name="strErrorMsg"></param>
        ''' <param name="iReturnCode"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Function GetShifts(ByRef oShifts As List(Of RoboticsExternAccess.roShift), ByRef strErrorMsg As String, ByRef returnCode As RoboticsExternAccess.Core.DTOs.ReturnCode, Optional ByVal ShiftID As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try
                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError

                oShifts = New Generic.List(Of RoboticsExternAccess.roShift)

                Dim strSQL As String = "@SELECT# Export, (@SELECT# isnull(Name,'') From ShiftGroups WHERE ID = Shifts.IDGroup) as GroupName, Name, isnull(ExpectedWorkingHours,0) as ExpectedWorkingHours, StartLimit, EndLimit, ShiftType, isnull(IsFloating,0) as IsFloating, Shifts.Color FROM Shifts WHERE len(Export) > 0  "
                If ShiftID IsNot Nothing AndAlso ShiftID.Length > 0 Then strSQL += " AND Export = '" & ShiftID.Replace("'", "''") & "' "
                strSQL += " ORDER By id "
                ' Cargamos lista de horarios
                Dim oShiftsDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)
                If oShiftsDT IsNot Nothing AndAlso oShiftsDT.Rows.Count > 0 Then
                    For Each orow As DataRow In oShiftsDT.Rows
                        Dim oShift As New RoboticsExternAccess.roShift
                        oShift.IDGroup = Any2String(orow("GroupName"))
                        oShift.IDShift = Any2String(orow("Export"))
                        oShift.Name = Any2String(orow("Name"))
                        oShift.ExpectedWorkingHours = orow("ExpectedWorkingHours")
                        oShift.StartLimit = New roWCFDate(CDate(orow("StartLimit")))
                        oShift.EndLimit = New roWCFDate(CDate(orow("EndLimit")))
                        Select Case Any2Integer(orow("ShiftType"))
                            Case 0, 1
                                If Any2Boolean(orow("IsFloating")) Then
                                    oShift.Type = ShiftType.NormalFloating
                                Else
                                    oShift.Type = ShiftType.Normal
                                End If

                            Case 2
                                oShift.Type = ShiftType.Vacations
                        End Select
                        oShift.Color = roCalendarRowPeriodDataManager.HexConverter(System.Drawing.ColorTranslator.FromWin32(Any2Integer(orow("Color"))))

                        oShifts.Add(oShift)
                    Next
                End If

                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetShifts")
                bolRet = False
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Función para obtener las plantillas de festivos
        ''' </summary>
        ''' <param name="oEmployees"></param>
        ''' <param name="strErrorMsg"></param>
        ''' <param name="iReturnCode"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Function GetPublicHolidays(ByRef oPublicHolidays As List(Of RoboticsExternAccess.roPublicHoliday), ByRef strErrorMsg As String, ByRef returnCode As RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bolRet As Boolean = False

            Try
                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError

                oPublicHolidays = New Generic.List(Of RoboticsExternAccess.roPublicHoliday)

                Dim strSQL As String = "@SELECT# ID, Name FROM sysroScheduleTemplates  WHERE Mode=2 AND FeastTemplate = 1 ORDER BY Name"
                ' Cargamos lista de plantillas de festivos
                Dim oPublicHolidaysDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)
                If oPublicHolidaysDT IsNot Nothing AndAlso oPublicHolidaysDT.Rows.Count > 0 Then
                    For Each orow As DataRow In oPublicHolidaysDT.Rows
                        Dim oPublicHoliday As New RoboticsExternAccess.roPublicHoliday
                        oPublicHoliday.ID = Any2String(orow("Name"))
                        oPublicHoliday.PublicHolidaysDetails = Nothing
                        ' Obtenemos las fechas festivas de la plantilla
                        strSQL = "@SELECT# ScheduleDate, Description FROM sysroScheduleTemplates_Detail  WHERE IDTemplate=" & orow("ID").ToString & " Order by ScheduleDate"
                        Dim oPublicHolidaysDetailsDT As DataTable = CreateDataTable(strSQL)
                        Dim oPublicHolidayDates = New Generic.List(Of RoboticsExternAccess.roPublicHolidayDate)
                        If oPublicHolidaysDetailsDT IsNot Nothing AndAlso oPublicHolidaysDetailsDT.Rows.Count > 0 Then
                            For Each orowdetail As DataRow In oPublicHolidaysDetailsDT.Rows
                                Dim oPublicHolidayDate As New RoboticsExternAccess.roPublicHolidayDate
                                oPublicHolidayDate.Description = Any2String(orowdetail("Description"))
                                oPublicHolidayDate.PublicHolidayDate = New roWCFDate(CDate(orowdetail("ScheduleDate")))
                                oPublicHolidayDates.Add(oPublicHolidayDate)
                            Next
                        End If
                        oPublicHoliday.PublicHolidaysDetails = oPublicHolidayDates.ToArray
                        oPublicHolidays.Add(oPublicHoliday)
                    Next
                End If

                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetPublicHolidays")
                bolRet = False
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Función para obtener el calendario de empleados
        ''' </summary>
        ''' <param name="oEmployees"></param>
        ''' <param name="strErrorMsg"></param>
        ''' <param name="iReturnCode"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Function GetCalendar(ByVal oCalendarCriteria As RoboticsExternAccess.roCalendarCriteria, ByRef oCalendarList As Generic.List(Of RoboticsExternAccess.roCalendar), ByRef strErrorMsg As String, ByRef iReturnCode As Integer, Optional ByVal Timestamp As DateTime? = Nothing, Optional ByVal LoadScheduledLayers As Boolean = False) As Boolean
            Dim bolret As Boolean = False
            Try
                oCalendarList = New Generic.List(Of RoboticsExternAccess.roCalendar)
                Dim xStartDate As Date = oCalendarCriteria.StartDate.GetDate
                Dim xEndDate As Date = oCalendarCriteria.EndDate.GetDate
                Dim oParams As New roParameters("OPTIONS", True)
                Dim oCalState As New Robotics.Base.VTCalendar.roCalendarRowPeriodDataState(-1)
                Dim oEmployeeState As New Employee.roEmployeeState(-1)
                Dim oContractState As New Contract.roContractState(-1)
                Dim oPAState As New Absence.roProgrammedAbsenceState(-1)
                Dim oPOState As New VTHolidays.roProgrammedOvertimeState(-1)
                Dim oPHState As New VTHolidays.roProgrammedHolidayState(-1)
                Dim oPOManager As New VTHolidays.roProgrammedOvertimeManager(oPOState)
                Dim oPHManager As New VTHolidays.roProgrammedHolidayManager(oPHState)

                ' Obtenemos el campo identificador del empleado
                Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                ' Si hay mas de 366 dias recortamos el periodo
                If (DateDiff(DateInterval.Day, xStartDate, xEndDate) + 1) > 366 Then
                    xEndDate = xStartDate.AddDays(366)
                End If

                If oCalendarCriteria IsNot Nothing AndAlso oCalendarCriteria.IDEmployees IsNot Nothing AndAlso oCalendarCriteria.IDEmployees.Count > 0 Then

                    ' Si hay mas de 50 empleados recortamos la lista
                    If oCalendarCriteria.IDEmployees.Count > 50 Then
                        Dim IDEmployees As New Generic.List(Of String)
                        Dim x As Integer = 0
                        For Each sEmployee As String In oCalendarCriteria.IDEmployees
                            IDEmployees.Add(sEmployee)
                            x += 1
                            If x = 50 Then Exit For
                        Next
                        oCalendarCriteria.IDEmployees = IDEmployees.ToArray
                    End If

                    Dim tbShifts As DataTable = CreateDataTable("@SELECT# ID, Export FROM Shifts")

                    ' Para cada empleado de la lista, obtenemos su calendario
                    For Each sEmployee As String In oCalendarCriteria.IDEmployees
                        Dim tbEmployee As DataTable = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(strImportPrimaryKeyUserField, sEmployee, Now, New UserFields.roUserFieldState(-1))
                        Dim iIdEmployee As Integer = -1
                        If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
                        If iIdEmployee > 0 Then
                            Dim oCalendarPeriodData As roCalendarRowPeriodData = VTCalendar.roCalendarRowPeriodDataManager.LoadCellsByCalendar(xStartDate, xEndDate, iIdEmployee, -1, 3, oParams, CalendarView.Planification, CalendarDetailLevel.Daily, Nothing, Nothing, Nothing, oCalState, False,,,, False,,, oEmployeeState, oContractState, oPAState, oPOState, oPHState, oPOManager, oPHManager,, False, False)

                            If oCalendarPeriodData IsNot Nothing AndAlso oCalendarPeriodData.DayData IsNot Nothing AndAlso oCalendarPeriodData.DayData.Count > 0 Then
                                For Each oDay As roCalendarRowDayData In oCalendarPeriodData.DayData
                                    ' Comprobamos si se ha modificado posteriormente a la fecha indicada en timestamp
                                    If Timestamp Is Nothing OrElse oDay.Timestamp >= Timestamp Then

                                        Dim oCalendar As New RoboticsExternAccess.roCalendar
                                        oCalendar.IDEmployee = sEmployee
                                        oCalendar.PlannedDate = New roWCFDate(oDay.PlanDate)
                                        If oDay.MainShift IsNot Nothing Then
                                            Dim sDate As DateTime = New DateTime(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.MainShift.StartHour.Hour, oDay.MainShift.StartHour.Minute, 0)
                                            Select Case oDay.MainShift.StartHour.Day
                                                Case 29
                                                    sDate = sDate.AddDays(-1)
                                                Case 31
                                                    sDate = sDate.AddDays(1)
                                            End Select

                                            oCalendar.StartHour = New roWCFDate(sDate)

                                            Dim eDate As DateTime = New DateTime(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.MainShift.EndHour.Hour, oDay.MainShift.EndHour.Minute, 0)
                                            Select Case oDay.MainShift.EndHour.Day
                                                Case 29
                                                    eDate = eDate.AddDays(-1)
                                                Case 31
                                                    eDate = eDate.AddDays(1)
                                            End Select
                                            oCalendar.EndHour = New roWCFDate(eDate)
                                        End If
                                        If oDay.MainShift IsNot Nothing Then
                                            Dim oRowsInfo As DataRow() = tbShifts.Select("ID=" & oDay.MainShift.ID.ToString)
                                            If oRowsInfo.Length > 0 Then
                                                oCalendar.IDShift = oRowsInfo(0)("Export").ToString

                                                If oDay.MainShift IsNot Nothing Then
                                                    Dim oShift = New Shift.roShift(oDay.MainShift.ID, New roShiftState)
                                                    Dim oBreakLayers As New List(Of roBreakLayerDefinition)

                                                    If oShift.AllowComplementary Or oShift.AllowFloatingData Then
                                                        Dim oShiftDefinition = New roCalendarShiftManager().GetShiftDefinition(oDay.MainShift.ID)
                                                        If oShiftDefinition IsNot Nothing Then
                                                            For Each oLayer In oShiftDefinition.BreakLayers

                                                                If oLayer.Start.Day <> 1 Then
                                                                    Dim sBreakLayerStart As DateTime = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oLayer.Start.Hour, oLayer.Start.Minute, oLayer.Start.Second)
                                                                    Dim sBreakLayerEnd As DateTime = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oLayer.Finish.Hour, oLayer.Finish.Minute, oLayer.Finish.Second)

                                                                    Select Case oLayer.Start.Day
                                                                        Case 29
                                                                            sBreakLayerStart = sBreakLayerStart.AddDays(-1)
                                                                        Case 31
                                                                            sBreakLayerStart = sBreakLayerStart.AddDays(1)
                                                                    End Select

                                                                    Select Case oLayer.Finish.Day
                                                                        Case 29
                                                                            sBreakLayerEnd = sBreakLayerEnd.AddDays(-1)
                                                                        Case 31
                                                                            sBreakLayerEnd = sBreakLayerEnd.AddDays(1)
                                                                    End Select

                                                                    oBreakLayers.Add(New roBreakLayerDefinition() With {
                                                                                        .Start = sBreakLayerStart,
                                                                                        .Finish = sBreakLayerEnd
                                                                    })
                                                                Else
                                                                    Dim oPlannedDay As Date = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.MainShift.StartHour.Hour, oDay.MainShift.StartHour.Minute, oDay.MainShift.StartHour.Second)
                                                                    oBreakLayers.Add(New roBreakLayerDefinition() With {
                                                                    .Start = oPlannedDay.AddHours(oLayer.Start.Hour).AddMinutes(oLayer.Start.Minute),
                                                                    .Finish = oPlannedDay.AddHours(oLayer.Finish.Hour).AddMinutes(oLayer.Finish.Minute)
                                                                    })
                                                                End If

                                                            Next
                                                        End If
                                                    Else
                                                        For Each oLayer In oShift.Layers
                                                            If oLayer.LayerType = roLayerTypes.roLTBreak Then
                                                                Dim oPlannedDay As Date = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.MainShift.StartHour.Hour, oDay.MainShift.StartHour.Minute, oDay.MainShift.StartHour.Second)

                                                                Dim sBreakLayerStart As DateTime = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, Any2DateTime(oLayer.Data("Begin")).Hour, Any2DateTime(oLayer.Data("Begin")).Minute, Any2DateTime(oLayer.Data("Begin")).Second)
                                                                Dim sBreakLayerEnd As DateTime = New Date(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, Any2DateTime(oLayer.Data("Finish")).Hour, Any2DateTime(oLayer.Data("Finish")).Minute, Any2DateTime(oLayer.Data("Finish")).Second)

                                                                If oShift.ShiftType = ShiftType.NormalFloating Then
                                                                    If oShift.StartFloating <> oDay.MainShift.StartHour Then
                                                                        Dim iMinutes As Integer = (oDay.MainShift.StartHour - oShift.StartFloating).Value.TotalMinutes

                                                                        sBreakLayerStart = sBreakLayerStart.AddMinutes(iMinutes)
                                                                        sBreakLayerEnd = sBreakLayerEnd.AddMinutes(iMinutes)
                                                                    End If
                                                                End If

                                                                oBreakLayers.Add(New roBreakLayerDefinition() With {
                                                                    .Start = sBreakLayerStart,
                                                                    .Finish = sBreakLayerEnd
                                                                })
                                                            End If
                                                        Next
                                                    End If
                                                    oCalendar.BreakLayers = oBreakLayers.ConvertAll(AddressOf XBreakLayerToBreakLayerResponseConverter).ToArray
                                                    If oDay.MainShift.Type = ShiftTypeEnum.Holiday_Working OrElse oDay.MainShift.Type = ShiftTypeEnum.Holiday_NoWorking Then
                                                        Dim rowBaseInfo As DataRow() = tbShifts.Select("ID=" & oDay.ShiftBase.ID.ToString)
                                                        oCalendar.IDShiftBase = rowBaseInfo(0)("Export").ToString
                                                        Dim sDate As DateTime = New DateTime(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.ShiftBase.StartHour.Hour, oDay.ShiftBase.StartHour.Minute, 0)
                                                        Select Case oDay.ShiftBase.StartHour.Day
                                                            Case 29
                                                                sDate = sDate.AddDays(-1)
                                                            Case 31
                                                                sDate = sDate.AddDays(1)
                                                        End Select

                                                        oCalendar.StartBasePlanned = New roWCFDate(sDate)

                                                        Dim eDate As DateTime = New DateTime(oDay.PlanDate.Year, oDay.PlanDate.Month, oDay.PlanDate.Day, oDay.ShiftBase.EndHour.Hour, oDay.ShiftBase.EndHour.Minute, 0)
                                                        Select Case oDay.ShiftBase.EndHour.Day
                                                            Case 29
                                                                eDate = eDate.AddDays(-1)
                                                            Case 31
                                                                eDate = eDate.AddDays(1)
                                                        End Select
                                                        oCalendar.EndBasePlanned = New roWCFDate(eDate)
                                                    End If
                                                Else
                                                    oCalendar.BreakLayers = {}
                                                End If

                                            End If
                                        End If
                                        If oDay.MainShift IsNot Nothing AndAlso LoadScheduledLayers AndAlso oDay.MainShift.ShiftLayersDefinition IsNot Nothing AndAlso oDay.MainShift.ShiftLayersDefinition.Count > 0 Then
                                            Dim shiftLayerDefinitions As List(Of roShiftLayerDefinition) = New List(Of roShiftLayerDefinition)
                                            For Each oLayer In oDay.MainShift.ShiftLayersDefinition
                                                Dim shiftLayer As roShiftLayerDefinition = New roShiftLayerDefinition()
                                                shiftLayer.OrdinaryHours = oLayer.LayerOrdinaryHours
                                                shiftLayer.ComplemntaryHours = oLayer.LayerComplementaryHours
                                                If oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 30) Then
                                                    shiftLayer.StartDay = roDayInfo.CurrentDay
                                                ElseIf oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 31) Then
                                                    shiftLayer.StartDay = roDayInfo.DayAfter
                                                ElseIf oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 29) Then
                                                    shiftLayer.StartDay = roDayInfo.DayBefore
                                                End If
                                                shiftLayer.StartTime = oLayer.LayerStartTime.ToString("HH:mm", Globalization.CultureInfo.InvariantCulture)
                                                shiftLayerDefinitions.Add(shiftLayer)
                                            Next
                                            oCalendar.ShiftLayerDefinition = shiftLayerDefinitions.ToArray()
                                        End If
                                        If oDay.ShiftBase IsNot Nothing AndAlso LoadScheduledLayers AndAlso oDay.ShiftBase.ShiftLayersDefinition IsNot Nothing AndAlso oDay.ShiftBase.ShiftLayersDefinition.Count > 0 Then
                                            Dim shiftLayerDefinitions As List(Of roShiftLayerDefinition) = New List(Of roShiftLayerDefinition)
                                            For Each oLayer In oDay.ShiftBase.ShiftLayersDefinition
                                                Dim shiftLayer As roShiftLayerDefinition = New roShiftLayerDefinition()
                                                shiftLayer.OrdinaryHours = oLayer.LayerOrdinaryHours
                                                shiftLayer.ComplemntaryHours = oLayer.LayerComplementaryHours
                                                If oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 30) Then
                                                    shiftLayer.StartDay = roDayInfo.CurrentDay
                                                ElseIf oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 31) Then
                                                    shiftLayer.StartDay = roDayInfo.DayAfter
                                                ElseIf oLayer.LayerStartTime.ToShortDateString = New Date(1899, 12, 29) Then
                                                    shiftLayer.StartDay = roDayInfo.DayBefore
                                                End If
                                                shiftLayer.StartTime = oLayer.LayerStartTime.ToString("HH:mm", Globalization.CultureInfo.InvariantCulture)
                                                shiftLayerDefinitions.Add(shiftLayer)
                                            Next
                                            oCalendar.ShiftBaseLayerDefinition = shiftLayerDefinitions.ToArray()
                                        End If

                                        oCalendar.TelecommuteForced = Nothing
                                        oCalendar.CanTelecommute = oDay.CanTelecommute
                                        oCalendar.TelecommutingStatus = Nothing

                                        If oDay.CanTelecommute IsNot Nothing AndAlso oDay.CanTelecommute Then
                                            Dim bolApplyDay As Boolean = False
                                            bolApplyDay = oDay.TelecommutingMandatoryDays.Contains(oDay.PlanDate.DayOfWeek)
                                            If Not bolApplyDay Then bolApplyDay = oDay.TelecommutingOptionalDays.Contains(oDay.PlanDate.DayOfWeek)
                                            If Not bolApplyDay Then bolApplyDay = oDay.PresenceMandatoryDays.Contains(oDay.PlanDate.DayOfWeek)
                                            ' Si tiene acuerdo y el dia es de trabajo
                                            If bolApplyDay Then
                                                oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.Presence ' Por defecto es presencial
                                                If oDay.TelecommutingExpected Then oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.Telecommuting ' En teletrabajo
                                                If oDay.TelecommutingOptional Then oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.TelecommutingOptional ' Opcional
                                                ' Indicamos si es forzado (en caso contrario es por acuerdo)
                                                oCalendar.TelecommuteForced = oDay.TelecommuteForced
                                            Else
                                                oCalendar.CanTelecommute = False
                                            End If
                                        Else
                                            'Aunque no tenga acuerdo ese día, si se le forzó algún estado de teletrabajo en calendario, lo reflejamos
                                            If oDay.TelecommuteForced Then
                                                oCalendar.TelecommuteForced = oDay.TelecommuteForced
                                                oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.Presence ' Por defecto es presencial
                                                If oDay.TelecommutingExpected Then oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.Telecommuting ' En teletrabajo
                                                If oDay.TelecommutingOptional Then oCalendar.TelecommutingStatus = RoboticsExternAccess.TelecommutingType_Enum.TelecommutingOptional ' Opcional
                                            End If

                                        End If

                                        oCalendarList.Add(oCalendar)
                                    End If
                                Next
                            End If
                        End If
                    Next

                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Else
                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidCalendarData
                End If

                bolret = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetCalendar")
                bolret = False
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
            End Try

            Return bolret
        End Function

        Private Function XBreakLayerToBreakLayerResponseConverter(oBreakLayer As roBreakLayerDefinition) As roBreakLayerDefinitionResponse
            Dim oRet As roBreakLayerDefinitionResponse
            Try
                oRet = New roBreakLayerDefinitionResponse
                oRet.Start = New Robotics.VTBase.roWCFDate(oBreakLayer.Start)
                oRet.Finish = New Robotics.VTBase.roWCFDate(oBreakLayer.Finish)
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function


    End Class

End Namespace