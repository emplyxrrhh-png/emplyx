Imports System.Data.Common
Imports System.Drawing
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink


    Public Class roCustomTDC
        Inherits roDataLinkExport

        Public Shared Function EXCELExportAttendance(ByVal mEmployeesFilter As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, ByRef oState As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim cn As DbConnection = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0
                Dim NameFile As String = "DataLinkExportAttendance#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)
                Dim eRow As Integer = 2
                Dim GroupAnt As Integer = 0

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("ExportAttendance.LogEvent.Start", "") & vbNewLine

                Dim oCalManager As New VTCalendar.roCalendarManager(New VTCalendar.roCalendarState(oState.IDPassport))
                Dim oEmpFilter As String = String.Empty

                oEmpFilter = mEmployeesFilter

                Dim oCalendar As DTOs.roCalendar = oCalManager.Load(dtBeginDate, dtEndDate, oEmpFilter, DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Detail_30, True)

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

                ' Crea la cabecera del excel
                excel.SetCellValue(1, 1, "Grupo", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 2, "Seccion", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 3, "Empleado", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 4, "Fecha", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 5, "Ent.1", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 6, "Sal.1", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 7, "Ent.2", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(1, 8, "Sal.2", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                Dim eCol As Integer = 9
                For j As Integer = 7 To 23
                    excel.SetCellValue(1, eCol, j, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                    excel.SetPattern(1, eCol, Color.Green)
                    eCol += 2
                Next

                For j As Integer = 0 To 3
                    excel.SetCellValue(1, eCol, j, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                    excel.SetPattern(1, eCol, Color.LemonChiffon)
                    eCol += 2
                Next

                Dim ecolTotalDia As Integer = eCol - 1
                excel.SetCellValue(1, ecolTotalDia, "H / D", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                For Each oCal As DTOs.roCalendar In oCalendarList

                    GroupAnt = 0
                    Dim printingDate As DateTime = dtBeginDate
                    Dim actualIndex As Integer = 0

                    Dim currentGroupName As String = String.Empty
                    Dim dtEmployeePunches As DataTable = Nothing

                    While printingDate <= dtEndDate

                        For Each row As DTOs.roCalendarRow In oCal.CalendarData

                            If row.PeriodData.DayData(actualIndex).PlanDate = printingDate Then

                                ' Solo horarios con franjas rígidas
                                If row.PeriodData.DayData(actualIndex).EmployeeStatusOnDay = DTOs.EmployeeStatusOnDayEnum.Ok AndAlso row.PeriodData.DayData(actualIndex).HourData IsNot Nothing AndAlso row.PeriodData.DayData(actualIndex).HourData.Length > 0 Then

                                    ' Imprime datos del empleado
                                    Dim strFullGroupName As String = ""
                                    Dim strGroup As String = ""
                                    Dim strSection As String = ""

                                    ' Obtenemos los fichajes de ese dia y verificamos que como minimo haya una entrada y una salida
                                    ' para mostralo en la exportacion
                                    Dim bolExistPunches As Boolean = False
                                    dtEmployeePunches = CreateDataTable("@SELECT# * FROM Punches WHERE IDEmployee=" & row.EmployeeData.IDEmployee & " AND ShiftDate='" & printingDate.ToString("yyyyMMdd") & "' AND ActualType IN(1,2)")
                                    Dim oPunchesEm() As DataRow = dtEmployeePunches.Select("ActualType = 1 ", "DateTime ASC")
                                    If oPunchesEm Is Nothing OrElse oPunchesEm.Length = 0 Then
                                    Else
                                        oPunchesEm = dtEmployeePunches.Select("ActualType = 2 ", "DateTime ASC")
                                        If oPunchesEm Is Nothing OrElse oPunchesEm.Length = 0 Then
                                        Else
                                            bolExistPunches = True
                                        End If
                                    End If

                                    If bolExistPunches Then
                                        strFullGroupName = Any2String(ExecuteScalar("@SELECT# TOP 1 FullGroupName FROM sysroEmployeeGroups WHERE IDGroup=" & row.EmployeeData.IDGroup))

                                        If strFullGroupName.Contains("\") AndAlso strFullGroupName.Split("\").Count >= 3 Then
                                            strGroup = strFullGroupName.Split("\")(1)
                                            strSection = strFullGroupName.Split("\")(2)
                                        Else
                                            strGroup = strFullGroupName
                                        End If

                                        excel.SetCellValue(eRow, 1, strGroup, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, True)
                                        excel.SetCellValue(eRow, 2, strSection, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, True)
                                        excel.SetCellValue(eRow, 3, row.EmployeeData.EmployeeName, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, True)
                                        excel.SetCellValue(eRow, 4, row.PeriodData.DayData(actualIndex).PlanDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        Dim strPunch1 As String = ""
                                        Dim strPunch2 As String = ""
                                        Dim strPunch3 As String = ""
                                        Dim strPunch4 As String = ""

                                        ' Entradas del dia
                                        Dim oRowsPunch() As DataRow = dtEmployeePunches.Select("ActualType = 1 ", "DateTime ASC")
                                        If oRowsPunch Is Nothing OrElse oRowsPunch.Length = 0 Then
                                        Else

                                            strPunch1 = Format(oRowsPunch(0)("DateTime"), "HH:mm")
                                            If oRowsPunch.Length > 1 Then
                                                strPunch3 = Format(oRowsPunch(1)("DateTime"), "HH:mm")
                                            End If
                                        End If

                                        ' Salidas del dia
                                        oRowsPunch = dtEmployeePunches.Select("ActualType = 2 ", "DateTime ASC")
                                        If oRowsPunch Is Nothing OrElse oRowsPunch.Length = 0 Then
                                        Else
                                            strPunch2 = Format(oRowsPunch(0)("DateTime"), "HH:mm")
                                            If oRowsPunch.Length > 1 Then
                                                strPunch4 = Format(oRowsPunch(1)("DateTime"), "HH:mm")
                                            End If
                                        End If

                                        excel.SetCellValue(eRow, 5, strPunch1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                        excel.SetCellValue(eRow, 6, strPunch2, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                        excel.SetCellValue(eRow, 7, strPunch3, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                        excel.SetCellValue(eRow, 8, strPunch4, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                                        Dim curPosition As Integer = 9
                                        Dim tmpPlannedHours As Double = 0

                                        Dim beginindexHourdata As Integer = 62
                                        Dim EndindexHourdata As Integer = 102
                                        Dim indexHourData As Integer = 0
                                        Dim intTotalDay As Integer = 0

                                        For Each oHourData As DTOs.roCalendarRowHourData In row.PeriodData.DayData(actualIndex).HourData

                                            If indexHourData >= beginindexHourdata And indexHourData <= EndindexHourdata Then

                                                Dim bIsPresent As Boolean = False
                                                Dim bIsFuture As Boolean = True

                                                If printingDate <= DateTime.Now.Date Then
                                                    bIsFuture = False
                                                    Dim hourMin As String() = oCalendar.CalendarHeader.PeriodHeaderData(indexHourData).Row2Text.Split(":")
                                                    Dim checkDate As New DateTime(printingDate.Year, printingDate.Month, printingDate.Day, roTypes.Any2Integer(hourMin(0)), roTypes.Any2Integer(hourMin(1)) + 29, 59)

                                                    If (indexHourData) >= 96 Then
                                                        checkDate = checkDate.AddDays(1)
                                                    End If

                                                    Dim oRows() As DataRow = dtEmployeePunches.Select("DateTime <= '" & checkDate.ToString("yyyy-MM-dd HH:mm:ss") & "'", "DateTime DESC")

                                                    Dim _BeginPeriod As Date = checkDate.AddMinutes(-29).AddSeconds(-59)
                                                    Dim _EndPeriod As Date = checkDate

                                                    Dim _OUT As Date = Any2Time("0001/01/01").Value
                                                    Dim _IN As Date = Any2Time("0001/01/01").Value

                                                    If oRows Is Nothing OrElse oRows.Length = 0 Then
                                                        bIsPresent = False
                                                    Else
                                                        If roTypes.Any2String(oRows(0)("ActualType")).Trim = "2" Then
                                                            ' ultima sortida
                                                            _OUT = oRows(0)("DateTime")

                                                            ' En el caso que lo ultimo sea una salida debemos mirar si hay una entrada  anterior para que sea valida la salida
                                                            oRows = dtEmployeePunches.Select("DateTime < '" & checkDate.ToString("yyyy-MM-dd HH:mm:ss") & "' AND ActualType =1 ", "DateTime DESC")
                                                            If oRows Is Nothing OrElse oRows.Length = 0 Then
                                                            Else
                                                                _IN = oRows(0)("DateTime")
                                                            End If
                                                        Else
                                                            ' Ultima entrada
                                                            _IN = oRows(0)("DateTime")

                                                            ' En el caso que lo ultimo sea una entrada debemos mirar si hay una salida posterior para que sea valida la entrada
                                                            oRows = dtEmployeePunches.Select("DateTime > '" & checkDate.ToString("yyyy-MM-dd HH:mm:ss") & "' AND ActualType =2 ", "DateTime DESC")
                                                            If oRows Is Nothing OrElse oRows.Length = 0 Then
                                                            Else
                                                                _OUT = oRows(0)("DateTime")

                                                            End If
                                                        End If
                                                        If _IN <> Any2Time("0001/01/01").Value And _OUT <> Any2Time("0001/01/01").Value Then
                                                            If (Any2Time(_IN).VBNumericValue <= Any2Time(_BeginPeriod).VBNumericValue Or (Any2Time(_IN).VBNumericValue > Any2Time(_BeginPeriod).VBNumericValue And Any2Time(_IN).VBNumericValue < Any2Time(_EndPeriod).VBNumericValue)) And Any2Time(_OUT).VBNumericValue >= Any2Time(_BeginPeriod).VBNumericValue Then
                                                                bIsPresent = True
                                                            End If
                                                        End If

                                                    End If
                                                End If

                                                If bIsPresent Then
                                                    excel.SetCellValue(eRow, curPosition, 1, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                    intTotalDay += 1
                                                Else
                                                    excel.SetCellValue(eRow, curPosition, 0, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                                End If

                                                excel.SetPattern(eRow, curPosition, Color.White, Color.Black)

                                                curPosition = curPosition + 1

                                            End If

                                            indexHourData = indexHourData + 1
                                        Next

                                        ' Total dia
                                        excel.SetCellValue(eRow, ecolTotalDia, intTotalDay / 2,  , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                        eRow += 1
                                    End If
                                End If
                            End If
                        Next

                        actualIndex = actualIndex + 1
                        printingDate = printingDate.AddDays(1)
                    End While
                Next

                ' Autoajusta columnas
                For i = 1 To eCol
                    Select Case i
                        Case 1 To 8
                            excel.AutoFitColumn(i, 800)
                        Case 9 To eCol - 2
                            excel.ColumnSize(i, 3.15, 0)
                        Case ecolTotalDia
                            excel.ColumnSize(i, 6, 0)

                    End Select
                Next i

                ' Graba el archivo
                excel.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Exportación finalizada
                strlogevent += Now.ToString & " - -> " & oState.Language.Translate("ExportAttendance.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportAttendance")
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportAttendance.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile

        End Function

    End Class

End Namespace