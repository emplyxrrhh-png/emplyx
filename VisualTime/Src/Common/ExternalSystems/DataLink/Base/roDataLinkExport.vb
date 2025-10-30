Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Security.Base

Namespace DataLink

    Public Class roDataLinkExport
        Inherits roExternalSystemBase
        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

#Region "Header and Footer excel"

        Protected Shared Sub DataLink_ExportShifts_CreateHead(ByVal excel As ExcelExport, ByVal eRow As Integer, ByVal Grupo As String)
            Dim eCol As Integer = 3

            excel.SetCellValue(eRow, 1, Grupo, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

            For i As Integer = 1 To 3
                For j As Integer = 0 To 47
                    If (j Mod 2) = 0 Then
                        excel.SetCellValue(1, eCol, j / 2, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                        Select Case i
                            Case 1 : excel.SetPattern(1, eCol, Color.Orange)
                            Case 2 : excel.SetPattern(1, eCol, Color.Green)
                            Case 3 : excel.SetPattern(1, eCol, Color.LemonChiffon)

                        End Select
                    Else
                        'excel.SetCellValue(1, eCol, 0, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                        'excel.SetPattern(1, eCol, Color.Yellow, Color.Red)
                    End If

                    eCol += 1
                Next
            Next
        End Sub

        Protected Shared Sub DataLink_ExportShifts_CreateFootDay(ByVal excel As ExcelExport, ByVal eRowIni As Integer, ByVal eRowEnd As Integer, ByVal eCol As Integer, Optional ByVal hasAttendance As Boolean = False)
            Dim i As Integer
            excel.SetCellValue(eRowEnd, 2, "Total día:", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, True)
            excel.SetPattern(eRowEnd, 2, Color.Green, Color.Yellow)
            For i = 3 To eCol
                If Not hasAttendance Then
                    excel.SetCellValue(eRowEnd, i, "=SUM(" & excel.GetColumnLetter(i) & eRowIni & ":" & excel.GetColumnLetter(i) & (eRowEnd - 1) & ")", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                Else
                    excel.SetCellValue(eRowEnd, i, "=SUM(" & excel.GetColumnLetter(i) & eRowIni & ":" & excel.GetColumnLetter(i) & (eRowEnd - 1) & ")", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                End If

            Next
        End Sub

        Protected Shared Sub DataLink_ExportHolidays_CreateHeadGroup(ByVal excel As ExcelExport, ByVal eRow As Integer, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef DW() As String, ByVal MonthNames() As String)
            ' Crea la cabecera del calendario
            Dim mesAnt As Integer = 0
            Dim Fecha As Date = dtBeginDate
            Dim Col As Integer = 3
            Dim i As Integer = 0

            For i = 0 To DateDiff(DateInterval.Day, dtBeginDate, dtEndDate)
                ' Comprueba si cambia el mes
                If mesAnt <> Fecha.Month Then
                    excel.SetCellValue(eRow, Col, MonthNames(Fecha.Month - 1), , , True)
                    excel.SetPattern(eRow, Col, IIf(Fecha.Month Mod 2 = 0, Color.Orange, Color.LightYellow))
                    excel.SetBorder(eRow, Col, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)
                    mesAnt = Fecha.Month
                End If

                ' Color de fondo del mes
                excel.SetCellValue(eRow + 1, Col, DW(Fecha.DayOfWeek), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetPattern(eRow + 1, Col, IIf(Fecha.Month Mod 2 = 0, Color.Orange, Color.LightYellow))
                excel.SetBorder(eRow + 1, Col, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)

                ' Dia del mes
                excel.SetCellValue(eRow + 2, Col, Fecha, "dd", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetBorder(eRow + 2, Col, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)
                Fecha = Fecha.AddDays(1)
                Col += 1
            Next

        End Sub

        Protected Shared Sub DataLink_ExportHolidays_CreateFoot(ByVal excel As ExcelExport, ByVal eRow As Integer, ByVal RowGroup As Integer, ByVal NumDays As Integer, Presents() As Integer, Absents() As Integer, ByVal Ostate As roDataLinkState)
            ' Presentes
            excel.SetCellValue(eRow, 1, Ostate.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Presents", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, "", , , True)
            excel.MergeCells(eRow, 1, eRow, 2)

            ' Ausentes
            excel.SetCellValue(eRow + 1, 1, Ostate.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Absents", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow + 1, 2, "", , , True)
            excel.MergeCells(eRow + 1, 1, eRow + 1, 2)

            ' columnas de presentes y ausentes
            For i As Integer = 0 To NumDays
                excel.SetCellValue(eRow, i + 3, Presents(i), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                excel.SetCellValue(eRow + 1, i + 3, Absents(i), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            Next i

            excel.CreateBox(eRow, 1, eRow, 3 + NumDays, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)
            excel.CreateBox(eRow + 1, 1, eRow + 1, 3 + NumDays, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)
        End Sub

        Protected Shared Sub DataLink_ExportHolidays_CreateHead(ByVal excel As ExcelExport, ByRef eRow As Integer, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal bolWithShifts As Boolean, ByVal OState As roDataLinkState)
            ' Cabecera. Titulo
            If bolWithShifts = False Then
                excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportSchedule.Title", ""), , , , , 24, , True)
            Else
                excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportSchedule.Title2", ""), , , , , 24, , True)
            End If
            eRow += 2

            ' Cabecera. Periodos
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.BeginDate", "") & ":", , , True)
            excel.SetCellValue(eRow, 2, dtBeginDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, True)

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.EndDate", "") & ":", , , True)
            excel.SetCellValue(eRow, 2, dtEndDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, True)
            excel.CreateBox(eRow - 1, 1, eRow, 2, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)
            eRow += 2

            ' Cabecera. Leyendas
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.SC", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.DaysWithOutContract", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Dias sin planificar o sin contrato

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.AP", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.PlannedAbsences", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Ausencias previstas

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.XXNoColor", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.HolidaysPerHours", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Vacaciones/Permisos por horas

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.VHP", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.ApprovalPendingHolidaysPerHours", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Vacaciones/Permisos por horas PENDIENTES de aprobar

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.VP", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.ApprovalPendingHolidays", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Vacaciones pendientes de aprobar

            eRow += 1
            excel.SetCellValue(eRow, 1, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.XXColor", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
            excel.SetCellValue(eRow, 2, OState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.HolidayHoursApproved", ""), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True) ' Horas de vacaciones aprobadas

            excel.CreateBox(eRow - 5, 1, eRow, 2, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)

        End Sub

        Protected Shared Sub DataLink_Export_CreateHeaders(ByVal excel As ExcelExport, dHeader As Dictionary(Of String, String), ByRef oState As roDataLinkState)
            Dim eRow As Integer = 1
            Dim eColumn As Integer = 1
            Try
                For Each pair As KeyValuePair(Of String, String) In dHeader
                    excel.SetCellValue(eRow, eColumn, pair.Value)
                    eColumn += 1
                Next
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::DataLink_Export_CreateHeaders")
            End Try

        End Sub

#End Region

#Region "SHARED FUNCTIONS"
        Protected Shared Function SafeForeColor(obj As Integer) As Color
            Dim ForeColor = Color.Black
            If (ColorTranslator.FromWin32(obj) = Color.Black) Then ForeColor = Color.White
            Return ForeColor
        End Function

        Protected Shared Function SaveExportLog(ByVal intIDExport As Integer, ByVal strLogMessage As String, Optional idSchedule As Integer = 0) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try
                If idSchedule > 0 Then
                    strSQL = "@UPDATE# ExportGuidesSchedules Set LastLog='" & strLogMessage.Replace(",", ".").Replace("'", "''") & "' WHERE ID=" & idSchedule
                Else
                    strSQL = "@UPDATE# ExportGuides Set LastLog='" & strLogMessage.Replace(",", ".").Replace("'", "''") & "' WHERE ID=" & intIDExport
                End If

                bolRet = ExecuteSql(strSQL)
            Catch ex As Exception
                'oState.UpdateStateInfo(ex, "roDataLinkExport::SaveImportLog")
            End Try

            Return bolRet

        End Function

        Protected Shared Function GenerateStreamFromString(ByVal lst As List(Of String)) As Stream
            Dim stream = New MemoryStream()
            Dim writer = New StreamWriter(stream)
            For Each s In lst
                writer.WriteLine(s)
            Next
            writer.Flush()
            stream.Position = 0
            Return stream
        End Function

        Public Shared Function CreateTempFile(ByRef ProcessName As String, ByVal strFileExtension As String, ByRef _State As roDataLinkState) As String
            Try

                ' Obtener nombre del fichero temporal con los datos a importar
                Dim oSettings As New roSettings
                Dim strPath As String = oSettings.GetVTSetting(eKeys.Reports)
                Dim strPrefix As String = ProcessName
                If _State.IDPassport >= 0 Then strPrefix &= "#" & _State.IDPassport.ToString
                Dim tmpFileName As String = TemporalFileName(strPath, strPrefix, strFileExtension)

                Return tmpFileName
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDataLinkExport::CreateTempFile")
                Return String.Empty
            End Try
        End Function

        Private Shared Function TemporalFileName(ByVal strPath As String, ByVal strPrefix As String, ByVal strExtension As String) As String
            Dim strRet As String = ""

            Dim Files() As String = System.IO.Directory.GetFiles(strPath, strPrefix & "_*." & strExtension)
            Dim intIndex As Integer = -1
            Dim i As Integer
            For Each strFile As String In Files
                i = CInt(strFile.Split("_")(1).Split(".")(0))
                If i > intIndex Then
                    intIndex = i
                End If
            Next
            intIndex += 1

            ' Obtener nombre del fichero temporal
            strRet = System.IO.Path.Combine(strPath, strPrefix & "_" & intIndex.ToString & "." & strExtension)

            Return strRet
        End Function

        Public Shared Function GetExportPeriodFromPattern(sPattern As String, dExecutionDate As DateTime, ByRef dBegindate As DateTime, ByRef dEndDate As DateTime) As Boolean
            Try
                ' El patrón tiene el siguiente formato: <tipo periodo>,<número de periodos (si aplica)>
                ' Tipos de periodos:
                Dim iType As Integer = 0
                Dim iPeriods As Integer = 0
                Dim dFixedBeginDate As Date = Date.MinValue
                Dim dFixedEndDate As Date = Date.MinValue

                ' Valido formato
                If sPattern.Split(",").Count = 1 Then Return False

                iType = CInt(sPattern.Split(",")(0))
                If sPattern.Split(",").Count > 1 Then iPeriods = CInt(sPattern.Split(",")(1))
                If sPattern.Split(",").Count > 2 Then dFixedBeginDate = CDate(sPattern.Split(",")(2))
                If sPattern.Split(",").Count > 3 Then dFixedEndDate = CDate(sPattern.Split(",")(3))

                dExecutionDate = dExecutionDate.Date

                Select Case iType
                    Case 0 'Hoy
                        dBegindate = dExecutionDate
                        dEndDate = dExecutionDate
                    Case 1 'Ayer
                        dBegindate = dExecutionDate.AddDays(-1)
                        dEndDate = dExecutionDate.AddDays(-1)
                    Case 2 'N semanas empezando en la actual, incluida
                        dBegindate = dExecutionDate.AddDays(1 - Weekday(dExecutionDate, vbMonday))
                        dEndDate = dBegindate.AddDays(7 * iPeriods).AddDays(-1)
                    Case 3 'N semanas anteriores a la actual
                        dBegindate = dExecutionDate.AddDays(-6 - Weekday(dExecutionDate, vbMonday)).AddDays(-1 * 7 * (iPeriods - 1))
                        dEndDate = dExecutionDate.AddDays(-Weekday(dExecutionDate, vbMonday))
                    Case 4 'N meses empezando en el actual
                        dBegindate = New Date(dExecutionDate.Year, dExecutionDate.Month, 1)
                        dEndDate = dBegindate.AddMonths(iPeriods).AddDays(-1)
                    Case 5 'N meses anteriores al actual
                        dBegindate = New Date(dExecutionDate.AddMonths(-iPeriods).Year, dExecutionDate.AddMonths(-iPeriods).Month, 1)
                        dEndDate = New Date(dExecutionDate.AddMonths(-1).Year, dExecutionDate.AddMonths(-1).Month, (New Date(dExecutionDate.AddMonths(-1).Year, dExecutionDate.AddMonths(-1).Month, 1)).AddMonths(1).AddDays(-1).Day)
                    Case 6 'Año actual hasta el día actual
                        dBegindate = New Date(dExecutionDate.Year, 1, 1)
                        dEndDate = dExecutionDate
                    Case 7 'Libre
                        dBegindate = dFixedBeginDate
                        dEndDate = dFixedEndDate
                    Case 8 'Mañana
                        dBegindate = dExecutionDate.AddDays(1)
                        dEndDate = dExecutionDate.AddDays(1)
                    Case 9 ' N semanas a partir de la siguiente, incluida
                        dBegindate = dExecutionDate.AddDays(1 - Weekday(dExecutionDate, vbMonday)).AddDays(7)
                        dEndDate = dBegindate.AddDays(7 * iPeriods).AddDays(-1)
                End Select

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

#End Region

    End Class

End Namespace