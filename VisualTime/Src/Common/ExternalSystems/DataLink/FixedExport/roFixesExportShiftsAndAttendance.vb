Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink
    Public Class roFixesExportShiftsAndAttendance
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportShiftsWithAttendance(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim cn As DbConnection = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0
                Dim NameFile As String = "ExportSchedule#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)
                Dim eRow As Integer = 2
                Dim eRowIni As Integer = 0
                Dim eCol As Integer = 2 + (48 * 3)
                Dim DateAnt As Date = Nothing
                Dim GroupAnt As Integer = 0
                Dim bIncludeHolidaysAndLeaves As Boolean = False

                ' Vemos si hay que añadir Vacaciones y ausencias
                Dim oAdvParameter As New AdvancedParameter.roAdvancedParameter("VTLive.Datalink.ExportShiftsWithAttendance.IncludeHolidaysAndLeaves", New AdvancedParameter.roAdvancedParameterState(), Nothing)
                bIncludeHolidaysAndLeaves = (oAdvParameter.Value = "1")

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportShiftsWithAttendance.LogEvent.Start", "") & vbNewLine

                Dim oCalManager As New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(Me.State.IDPassport))

                Dim oCalendar As DTOs.roCalendar = oCalManager.Load(dtBeginDate, dtEndDate, mEmployees, DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Detail_30, True)

                Dim eColMin As Integer = 999
                Dim eColMax As Integer = -1

                Dim DayCount As Integer() = {0, 0, 0, 0, 0, 0, 0}
                ' Cuenta los días de la semana
                Dim xdate As Date = dtBeginDate
                For i = 0 To DateDiff("d", dtBeginDate, dtEndDate)
                    Select Case (xdate.DayOfWeek)
                        Case DayOfWeek.Monday
                            DayCount(0) += 1
                        Case DayOfWeek.Tuesday
                            DayCount(1) += 1
                        Case DayOfWeek.Wednesday
                            DayCount(2) += 1
                        Case DayOfWeek.Thursday
                            DayCount(3) += 1
                        Case DayOfWeek.Friday
                            DayCount(4) += 1
                        Case DayOfWeek.Saturday
                            DayCount(5) += 1
                        Case DayOfWeek.Sunday
                            DayCount(6) += 1
                    End Select
                    xdate = xdate.AddDays(1)
                Next

                Dim oCalendarList As New Generic.List(Of DTOs.roCalendar)

                If oCalendar IsNot Nothing Then
                    Dim iActualGroup As Integer = -1
                    Dim oActualCalendarRows As New Generic.List(Of DTOs.roCalendarRow)

                    For Each oCalRow As DTOs.roCalendarRow In oCalendar.CalendarData
                        If iActualGroup <> -1 AndAlso iActualGroup <> oCalRow.EmployeeData.IDGroup Then
                            Dim oGroupCalendar As New DTOs.roCalendar
                            oGroupCalendar.CalendarData = oActualCalendarRows.ToArray
                            oCalendarList.Add(oGroupCalendar)
                            oActualCalendarRows = New Generic.List(Of DTOs.roCalendarRow)

                        End If

                        iActualGroup = oCalRow.EmployeeData.IDGroup
                        oActualCalendarRows.Add(oCalRow)

                    Next

                    If oActualCalendarRows.Count > 0 Then
                        Dim oGroupCalendar As New DTOs.roCalendar
                        oGroupCalendar.CalendarData = oActualCalendarRows.ToArray
                        oCalendarList.Add(oGroupCalendar)
                    End If
                End If

                For Each oCal As DTOs.roCalendar In oCalendarList

                    GroupAnt = 0
                    Dim printingDate As DateTime = dtBeginDate
                    Dim actualIndex As Integer = 0

                    Dim DayTotal As Double() = {0, 0, 0, 0, 0, 0, 0}
                    Dim DayEmps As Integer() = {0, 0, 0, 0, 0, 0, 0}

                    Dim currentGroupName As String = String.Empty

                    Dim oEmpHours As New Hashtable

                    While printingDate <= dtEndDate

                        For Each row As DTOs.roCalendarRow In oCal.CalendarData
                            Dim bIsHolidayOrLeave As Boolean = False
                            If row.PeriodData.DayData(actualIndex).PlanDate = printingDate Then

                                If GroupAnt = 0 Then GroupAnt = row.EmployeeData.IDGroup

                                ' Si cambia la fecha imprime la cabecera de la fecha y totaliza
                                If DateAnt <> row.PeriodData.DayData(actualIndex).PlanDate Or GroupAnt <> row.EmployeeData.IDGroup Then
                                    If DateAnt <> #12:00:00 AM# Then
                                        ' Totaliza el dia
                                        DataLink_ExportShifts_CreateFootDay(excel, eRowIni, eRow, eCol, True)
                                        eRow += 2
                                    End If

                                    ' Si cambia el grupo totaliza horas teoricas por empleado
                                    If GroupAnt <> row.EmployeeData.IDGroup Then
                                        GroupAnt = row.EmployeeData.IDGroup
                                    End If

                                    ' Crea la cabecera del excel
                                    DataLink_ExportShifts_CreateHead(excel, eRow, row.EmployeeData.GroupName)
                                    eRow += 1

                                    ' Imprime cabecera de la fecha y horas totales
                                    excel.SetCellValue(eRow, 2, row.PeriodData.DayData(actualIndex).PlanDate, "ddd, dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                    excel.SetPattern(eRow, 2, Color.Green, Color.Yellow)
                                    excel.SetCellValue(eRow, eCol + 3, "Horas Totales", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                    excel.SetPattern(eRow, eCol + 3, Color.Green, Color.Yellow)
                                    eRow += 1
                                    eRowIni = eRow

                                    DateAnt = row.PeriodData.DayData(actualIndex).PlanDate
                                End If

                                ' Solo horarios con franjas rígidas
                                If row.PeriodData.DayData(actualIndex).EmployeeStatusOnDay = DTOs.EmployeeStatusOnDayEnum.Ok _
                                    AndAlso (Not row.PeriodData.DayData(actualIndex).IsHoliday OrElse bIncludeHolidaysAndLeaves) _
                                    AndAlso row.PeriodData.DayData(actualIndex).HourData IsNot Nothing _
                                    AndAlso row.PeriodData.DayData(actualIndex).HourData.Length > 0 _
                                    AndAlso row.PeriodData.DayData(actualIndex).MainShift IsNot Nothing Then

                                    If row.PeriodData.DayData(actualIndex).IsHoliday Then bIsHolidayOrLeave = True

                                    Dim strEmployeeSQL As String = "@SELECT# TOP 1 HighlightColor, isnull((@SELECT# top 1 IDContract from EmployeeContracts where '" & printingDate.ToString("yyyyMMdd") & "' between BeginDate and EndDate and EmployeeContracts.IDEmployee = Employees.ID order by EndDate DESC),'') as IDContract from Employees WHERE Employees.ID = " & row.EmployeeData.IDEmployee

                                    Dim dtEmpInfo As DataTable = CreateDataTable(strEmployeeSQL)

                                    Dim bInsertEmployee As Boolean = False
                                    For Each oHourData As DTOs.roCalendarRowHourData In row.PeriodData.DayData(actualIndex).HourData
                                        If oHourData.DailyHourType <> DTOs.DailyHourTypeEnum.Untyped AndAlso (Not oHourData.IsHoursAbsence OrElse bIncludeHolidaysAndLeaves) Then
                                            If oHourData.IsHoursAbsence Then bIsHolidayOrLeave = True
                                            bInsertEmployee = True
                                        End If
                                    Next

                                    If bInsertEmployee AndAlso dtEmpInfo IsNot Nothing AndAlso dtEmpInfo.Rows.Count = 1 Then

                                        Dim dtEmployeePunches As DataTable = CreateDataTable("@SELECT# * FROM Punches WHERE IDEmployee=" & row.EmployeeData.IDEmployee & " AND ShiftDate='" & printingDate.ToString("yyyyMMdd") & "'")

                                        If Not oEmpHours.ContainsKey(row.EmployeeData.IDEmployee & "@" & row.EmployeeData.EmployeeName) Then oEmpHours.Add(row.EmployeeData.IDEmployee & "@" & row.EmployeeData.EmployeeName, 0)

                                        Dim iWin32Color As Long = roTypes.Any2Long(dtEmpInfo.Rows(0)("HighlightColor"))
                                        ' Imprime datos del empleado
                                        excel.SetCellValue(eRow, 1, row.EmployeeData.EmployeeName, , , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        'TODO Falta rellenar con el contrato
                                        excel.SetCellValue(eRow, 2, dtEmpInfo.Rows(0)("IDContract"), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        Dim curPosition As Integer = 3
                                        Dim tmpPlannedHours As Double = 0
                                        Dim layerInfoPosition As Integer = eCol + 4
                                        Dim lastTLayerType As DTOs.DailyHourTypeEnum = DTOs.DailyHourTypeEnum.Untyped

                                        If Not bIsHolidayOrLeave Then
                                            For Each oHourData As DTOs.roCalendarRowHourData In row.PeriodData.DayData(actualIndex).HourData

                                                If oHourData.DailyHourType <> DTOs.DailyHourTypeEnum.Untyped Then

                                                    If eColMin > curPosition Then eColMin = curPosition
                                                    If eColMax < curPosition Then eColMax = curPosition

                                                    Dim oColor As Color = IIf(iWin32Color <> 0, ColorTranslator.FromWin32(iWin32Color), ColorTranslator.FromHtml(row.PeriodData.DayData(actualIndex).MainShift.Color))

                                                    If oHourData.IsHoursAbsence Then
                                                        excel.SetCellValue(eRow, curPosition, 0, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True,,,,, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Dotted)
                                                        oColor = Color.Gray
                                                    Else
                                                        Dim bIsPresent As Boolean = True
                                                        Dim bIsFuture As Boolean = True

                                                        If printingDate <= DateTime.Now.Date Then
                                                            bIsFuture = False
                                                            Dim hourMin As String() = oCalendar.CalendarHeader.PeriodHeaderData(curPosition - 3).Row2Text.Split(":")
                                                            Dim checkDate As New DateTime(printingDate.Year, printingDate.Month, printingDate.Day, roTypes.Any2Integer(hourMin(0)), roTypes.Any2Integer(hourMin(1)) + 29, 0)

                                                            If (curPosition - 3) < 48 Then : checkDate = checkDate.AddDays(-1)
                                                            ElseIf (curPosition - 3) >= 96 Then : checkDate = checkDate.AddDays(1)
                                                            End If

                                                            Dim oRows() As DataRow = dtEmployeePunches.Select("DateTime <= '" & checkDate.ToString("yyyy-MM-dd HH:mm:ss") & "'", "DateTime DESC")

                                                            If oRows Is Nothing OrElse oRows.Length = 0 Then
                                                                bIsPresent = False
                                                            Else
                                                                If roTypes.Any2String(oRows(0)("ActualType")).Trim = "2" Then
                                                                    bIsPresent = False
                                                                End If
                                                            End If
                                                        End If

                                                        If bIsPresent Then
                                                            If oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Mandatory Then
                                                                excel.SetCellValue(eRow, curPosition, 1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                                tmpPlannedHours += 30
                                                            ElseIf oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Flexible Then
                                                                If bIsFuture Then
                                                                    excel.SetCellValue(eRow, curPosition, 0, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True,,,,, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Dotted)
                                                                Else
                                                                    excel.SetCellValue(eRow, curPosition, 1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True,,,,, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Dotted)
                                                                End If

                                                            ElseIf oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Complementary Then
                                                                excel.SetCellValue(eRow, curPosition, 1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                                tmpPlannedHours += 30
                                                            Else
                                                                excel.SetCellValue(eRow, curPosition, 1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                            End If
                                                        Else
                                                            oColor = Color.Red
                                                            If oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Mandatory OrElse oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Complementary Then
                                                                tmpPlannedHours += 30
                                                            End If
                                                            excel.SetCellValue(eRow, curPosition, 0, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True,,,,, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Dotted)
                                                        End If

                                                    End If

                                                    excel.SetPattern(eRow, curPosition, oColor, oColor)
                                                End If

                                                If (lastTLayerType = DTOs.DailyHourTypeEnum.Untyped AndAlso oHourData.DailyHourType <> DTOs.DailyHourTypeEnum.Untyped) OrElse
                                                (lastTLayerType <> DTOs.DailyHourTypeEnum.Untyped AndAlso oHourData.DailyHourType = DTOs.DailyHourTypeEnum.Untyped) Then

                                                    excel.SetCellValue(eRow, layerInfoPosition, oCalendar.CalendarHeader.PeriodHeaderData(curPosition - 3).Row2Text, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                    layerInfoPosition += 1
                                                    lastTLayerType = oHourData.DailyHourType
                                                End If

                                                curPosition = curPosition + 1
                                            Next

                                        End If

                                        tmpPlannedHours = tmpPlannedHours / 60

                                        currentGroupName = row.EmployeeData.GroupName
                                        excel.SetCellValue(eRow, eCol + 1, row.PeriodData.DayData(actualIndex).PlanDate, "ddd, dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                        excel.SetCellValue(eRow, eCol + 2, row.EmployeeData.GroupName, , , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        ' Imprime el horario
                                        excel.SetCellValue(eRow, eCol + 3, roConversions.ConvertHoursToTime(tmpPlannedHours.ToString()), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        oEmpHours(row.EmployeeData.IDEmployee & "@" & row.EmployeeData.EmployeeName) += tmpPlannedHours

                                        If Not bIsHolidayOrLeave Then
                                            Select Case (printingDate.DayOfWeek)
                                                Case DayOfWeek.Monday
                                                    DayEmps(0) += 1
                                                    DayTotal(0) += tmpPlannedHours
                                                Case DayOfWeek.Tuesday
                                                    DayEmps(1) += 1
                                                    DayTotal(1) += tmpPlannedHours
                                                Case DayOfWeek.Wednesday
                                                    DayEmps(2) += 1
                                                    DayTotal(2) += tmpPlannedHours
                                                Case DayOfWeek.Thursday
                                                    DayEmps(3) += 1
                                                    DayTotal(3) += tmpPlannedHours
                                                Case DayOfWeek.Friday
                                                    DayEmps(4) += 1
                                                    DayTotal(4) += tmpPlannedHours
                                                Case DayOfWeek.Saturday
                                                    DayEmps(5) += 1
                                                    DayTotal(5) += tmpPlannedHours
                                                Case DayOfWeek.Sunday
                                                    DayEmps(6) += 1
                                                    DayTotal(6) += tmpPlannedHours
                                            End Select

                                        End If

                                        eRow += 1
                                    End If
                                End If
                            End If
                        Next

                        actualIndex = actualIndex + 1
                        printingDate = printingDate.AddDays(1)
                    End While

                    If Not IsNothing(DateAnt) Then
                        ' Totaliza el dia
                        DataLink_ExportShifts_CreateFootDay(excel, eRowIni, eRow, eCol, True)
                        eRow += 1

                        ' Totaliza el grupo
                        DataLink_ExportShiftsAttendnace_CreateFootGroup(excel, eRow, eCol, DayCount, DayTotal, DayEmps, oEmpHours)
                    End If

                Next

                ' Autoajusta columnas
                eRow += 2
                For i = 1 To eCol + 8
                    Select Case i
                        Case 1 To 2
                            excel.AutoFitColumn(i, 500)
                        Case 3 To eColMin - 2
                            excel.ColumnSize(i, 0.1, 0)
                        Case eColMin + 1 To eColMax + 1
                            excel.ColumnSize(i, 3.15, 0)
                        Case eColMax + 2 To eCol
                            excel.ColumnSize(i, 0.1, 0)
                        Case eCol + 1 To eCol + 8
                            excel.AutoFitColumn(i, 500)
                    End Select

                Next i

                ' Graba el archivo

                excel.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("ExportShiftsWithAttendance.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportShiftsWithAttendance")
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("ExportShiftsWithAttendance.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Cierra la base de datos
                If Not IsNothing(cn) Then
                    If cn.State = ConnectionState.Open Then cn.Close()
                    cn.Dispose()
                End If

                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile

        End Function


        Private Sub DataLink_ExportShiftsAttendnace_CreateFootGroup(ByVal excel As ExcelExport, ByRef eRow As Integer, ByVal eCol As Integer, ByVal WeekDaysCount() As Integer, ByVal WeekDayTotal() As Double, ByVal WeekDaysEmps() As Integer, ByVal oEmpHours As Hashtable)
            Dim bRow As Integer = eRow
            Dim total As Double = 0

            excel.SetCellValue(eRow, eCol + 3, "TOTAL HORAS TEORICAS POR EMPLEADO", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetPattern(eRow, eCol + 3, Color.Green, Color.Yellow)
            eRow += 1

            ' Imprime el total de cada empleado
            For Each key As String In oEmpHours.Keys

                excel.SetCellValue(eRow, eCol + 3, key.Split("@")(1), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(eRow, eCol + 4, roConversions.ConvertHoursToTime(oEmpHours(key)), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                total += oEmpHours(key)
                eRow += 1
            Next

            ' Imprime el total de todos los empleados
            excel.SetCellValue(eRow, eCol + 3, "TOTAL", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, True)
            excel.SetPattern(eRow, eCol + 3, Color.Green, Color.Yellow)
            excel.SetCellValue(eRow, eCol + 4, roConversions.ConvertHoursToTime(total.ToString()), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            eRow += 1

            ' Totales por día de la semana
            bRow += 1
            excel.SetCellValue(bRow, eCol + 7, "HORAS", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetPattern(bRow, eCol + 7, Color.Green, Color.Yellow)
            excel.SetCellValue(bRow, eCol + 8, "EMPLEADOS", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetPattern(bRow, eCol + 8, Color.Green, Color.Yellow)
            bRow += 1

            Dim DS() As String = {"Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo"}
            Dim n As Integer = 0

            For Each rowName As String In DS
                Dim totalCount As Integer = 0
                Dim totalEmployees As Integer = 0
                Dim totalHours As Integer = 0

                excel.SetCellValue(bRow, eCol + 6, rowName & "(" & WeekDaysCount(n) & ")", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(bRow, eCol + 7, roConversions.ConvertHoursToTime(WeekDayTotal(n)), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(bRow, eCol + 8, WeekDaysEmps(n), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                bRow += 1
                n += 1
            Next

            If eRow < bRow Then eRow = bRow
            eRow += 1

        End Sub



    End Class

End Namespace