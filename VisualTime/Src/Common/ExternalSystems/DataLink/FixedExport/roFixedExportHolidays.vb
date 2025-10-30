Imports System.Drawing
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roFixedExportHolidays
        Inherits roDataLinkExport


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub


        Public Function ExportHolidays(ByVal tmpEmployeeFilterTable As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, ByVal bolWithShifts As Boolean, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim queryString As String
                Dim i%, j%

                Dim eRow As Integer = 1

                Dim NameFile As String

                NameFile = "ExportHolidays#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportHolidays.LogEvent.Start", "") & vbNewLine

                ' Cabecera
                Call DataLink_ExportHolidays_CreateHead(excel, eRow, dtBeginDate, dtEndDate, bolWithShifts, Me.State)

                ' Selecciona horarios de vacaciones
                queryString = "" &
                    "@SELECT# Id, Name, ShortName, Description, Color " &
                    "FROM Shifts " &
                    "WHERE Id in (" &
                        "@SELECT# DISTINCT ISNULL(IDShiftUsed,IDShift1) AS Shift " &
                        "FROM dbo.DailySchedule " &
                            " INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = dbo.DailySchedule.IdEmployee " &
                        "WHERE (dbo.DailySchedule.Date >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ")" &
                            " AND (dbo.DailySchedule.Date <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") "

                If Not bolWithShifts Then queryString += " AND isnull(IsHolidays,0) = 1 "

                queryString += ")"

                Dim dtShifts As DataTable = CreateDataTable(queryString)

                ' Cabecera.de horarios
                For Each row As DataRow In dtShifts.Rows
                    eRow += 1

                    ' Nombre corto del horario
                    excel.SetCellValue(eRow, 1, row("ShortName"), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)

                    ' Color de fondo del horario
                    excel.SetPattern(eRow, 1, ColorTranslator.FromWin32(row("Color")), SafeForeColor(row("Color")))

                    ' Nombre del horario
                    excel.SetCellValue(eRow, 2, row("Name"), , , True)
                Next

                eRow += 1

                ' Selecciona ausencias previstas
                queryString = "" &
                    "@SELECT# idCause, idEmployee, BeginDate, ISNULL(FinishDate, DATEADD(D,maxlastingdays,BeginDate)) AS FinishDate " &
                    "from ProgrammedAbsences " &
                    " INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = dbo.ProgrammedAbsences.IdEmployee " &
                    " WHERE (ISNULL(FinishDate, DATEADD(D,maxlastingdays,BeginDate)) >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ")" &
                    " AND (BeginDate <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") "
                Dim dtAusPrev As DataTable = CreateDataTable(queryString)

                ' Selecciona solicitudes de vacaciones/permisos por horas pendientes
                queryString = "" &
                    "@SELECT# IDEmployee, IDShift, RequestType, rd.Date, rd.AllDay, rd.Duration, s.Color as ShiftColor, s.ExpectedWorkingHours from Requests r " &
                    " INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = r.IdEmployee " &
                    " INNER JOIN sysroRequestDays rd on r.ID=rd.IDRequest " &
                    " LEFT JOIN Shifts s on s.ID=r.IDShift " &
                    " WHERE Date BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & " and " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime &
                    " AND r.Status in(0,1)"
                '" AND RequestType=13 " '13 el por horas. 6 el por horario
                Dim dtRequests As DataTable = CreateDataTable(queryString)

                ' Selecciona vacaciones previstas
                queryString = "" &
                    "@SELECT# idCause, idEmployee, Date, AllDay, Duration " &
                    "from ProgrammedHolidays " &
                    " INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = ProgrammedHolidays.IdEmployee " &
                    " WHERE (Date >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ") " &
                    " AND (Date <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") "
                Dim dtVacPrev As DataTable = CreateDataTable(queryString)

                ' Selecciona todos los horarios
                queryString = "@SELECT# * from shifts"
                Dim dtShiftsAll As DataTable = CreateDataTable(queryString)

                ' Selecciona todos los contratos
                queryString = "@SELECT# idEmployee, idContract, BeginDate, EndDate from EmployeeContracts " &
                    " INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = EmployeeContracts.IdEmployee " &
                    " WHERE (EndDate >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ")" &
                    " AND (BeginDate <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") "
                Dim dtContracts As DataTable = CreateDataTable(queryString)

                ' Selecciona planificación
                queryString = "" &
                    "@SELECT# CEG.GroupName, CEG.FullGroupName, CEG.EmployeeName, " &
                    "CEG.idemployee, sysroDailyScheduleByContract.Date, NumContrato, IsHolidays, IDShiftBase, " &
                    "ISNULL(IDShiftUsed,IDShift1) AS Shift " &
                    "FROM sysroDailyScheduleByContract " &
                    "   INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = sysroDailyScheduleByContract.IdEmployee " &
                    "   INNER JOIN [sysrovwCurrentEmployeeGroups] CEG ON sysroDailyScheduleByContract.IDEmployee = CEG.IDEmployee " &
                    "WHERE (sysroDailyScheduleByContract.Date >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ")" &
                    " AND (sysroDailyScheduleByContract.Date <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") " &
                    " AND (sysroDailyScheduleByContract.IDShift1 IS NOT NULL)" &
                    " UNION " &
                    "@SELECT# CEG.GroupName, CEG.FullGroupName, CEG.EmployeeName, " &
                    "CEG.idemployee, sysroDailyScheduleByContract.Date, NumContrato, IsHolidays, IDShiftBase, " &
                    "ISNULL(IDShiftUsed,IDShift1) AS Shift " &
                    "FROM sysroDailyScheduleByContract " &
                    "   INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = sysroDailyScheduleByContract.IdEmployee " &
                    "   INNER JOIN [sysrovwFutureEmployeeGroups] CEG ON sysroDailyScheduleByContract.IDEmployee = CEG.IDEmployee " &
                    "WHERE (sysroDailyScheduleByContract.Date >=" & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & ")" &
                    " AND (sysroDailyScheduleByContract.Date <=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") " &
                    " AND (sysroDailyScheduleByContract.IDShift1 IS NOT NULL)" &
                    "AND sysroDailyScheduleByContract.IDEmployee NOT IN (@SELECT# CEG.IDEmployee FROM [sysrovwCurrentEmployeeGroups] CEG)" &
                    "ORDER BY CEG.FullGroupName, CEG.EmployeeName, sysroDailyScheduleByContract.date"

                Dim dtSch As DataTable = CreateDataTable(queryString)

                Dim DptoAnterior As String = ""
                Dim EmpleadoAnterior As Integer = -1
                Dim ContractAnterior As String = ""

                Dim col As Integer
                Dim RowGroup As Integer = 0

                Dim NumDays As Integer = DateDiff(DateInterval.Day, dtBeginDate, dtEndDate)
                Dim Presents(NumDays) As Integer
                Dim Absents(NumDays) As Integer

                ' Define el idioma de los dias de la semana y meses
                Dim DW() As String = Split(Me.State.Language.Translate("roDataLinkExport.ExcelExportSchedule.WeekDays", ""), ",")
                If DW.Length <> 7 Then DW = Split("D,L,M,X,J,V,S", ",")

                Dim MonthNames(12) As String
                For i = 1 To 12
                    MonthNames(i - 1) = Me.State.Language.Translate("roDataLinkExport.ExcelExportSchedule.month." & i, "")
                Next i

                Dim Absent As Boolean
                Dim VacancesAllDay As Boolean
                Dim VacancesPeriod As Boolean

                Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)

                'una fila per cada dia de contracte
                For Each row As DataRow In dtSch.Rows
                    Absent = True
                    VacancesAllDay = False
                    VacancesPeriod = False

                    If Not IsDBNull(row("NumContrato")) Then
                        ' Comprueba si cambia el dpto
                        If DptoAnterior <> row("FullGroupName") Then
                            ' Crea el pie del grupo
                            If DptoAnterior <> "" Then
                                eRow += 1
                                Call DataLink_ExportHolidays_CreateFoot(excel, eRow, RowGroup, NumDays, Presents, Absents, Me.State)
                                eRow += 1
                            End If

                            ' Crea la cabecera del grupo
                            eRow += 2
                            Call DataLink_ExportHolidays_CreateHeadGroup(excel, eRow, dtBeginDate, dtEndDate, DW, MonthNames)
                            eRow += 2

                            excel.SetCellValue(eRow, 1, row("FullGroupName"), , , True)
                            excel.SetCellValue(eRow, 2, "", , , True)
                            excel.MergeCells(eRow, 1, eRow, 2)
                            excel.CreateBox(eRow, 1, eRow, 2, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Medium)

                            DptoAnterior = row("FullGroupName")
                            RowGroup = eRow + 1

                            For i = 0 To NumDays
                                Presents(i) = 0
                                Absents(i) = 0
                            Next
                        End If

                        ' Comprueba si cambia el empleado o contrato
                        If EmpleadoAnterior <> row("idEmployee") Or ContractAnterior <> row("NumContrato") Then
                            Dim Fecha As Date
                            eRow += 1

                            ' Rellena todo el periodo como "SC"
                            ' TODO: No debería rellenar aquellos días que el empleado no tenga planificación pero si contrato
                            '       Sólo debería poner SC si no hay niguna fila en la tabla para ese empleado y fecha (los días sin contrato no aparecen en dtSch)
                            For i = 0 To NumDays
                                ' 1.- calculo fecha en función de i
                                ' 2.- filtro tabla para ese idemployee y día. Si no hay ningún registro pongo SC
                                Dim dRefDate As Date = roTypes.Any2DateTime(dtBeginDate.AddDays(i))
                                Dim bHasContract As Boolean = True
                                Dim strSQL As String
                                strSQL = "@SELECT# COUNT(*) FROM EmployeeContracts WHERE IdEmployee = " & roTypes.Any2Integer(row("idEmployee")).ToString
                                strSQL = strSQL & " AND IdContract = '" & roTypes.Any2String(row("NumContrato")) & "'"
                                strSQL = strSQL & " AND " & roTypes.Any2Time(dRefDate).SQLSmallDateTime & " BETWEEN BeginDate AND EndDate"
                                bHasContract = (ExecuteScalar(strSQL) <> 0)
                                If bHasContract = 0 Then
                                    excel.SetCellValue(eRow, i + 3, "SC", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                Else
                                    excel.SetCellValue(eRow, i + 3, "??", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                End If
                            Next i

                            ' Datos del empleado
                            excel.SetCellValue(eRow, 1, row("EmployeeName"), , , True)
                            excel.SetCellValue(eRow, 2, row("NumContrato"), , , True)

                            col = DateDiff(DateInterval.Day, dtBeginDate, Convert.ToDateTime(row("Date"))) + 3

                            ' Rellena ausencias programadas "AP" del contrato
                            Dim contract = dtContracts.Select("idContract='" & row("NumContrato") & "'")
                            For Each AusPrev In dtAusPrev.Select("idEmployee=" & row("idEmployee") & " and BeginDate>='" & Format(contract(0)("BeginDate"), "yyyy/MM/dd") & "' and (FinishDate<='" & Format(contract(0)("EndDate"), "yyyy/MM/dd") & "' or FinishDate is null)")
                                Dim IDate = IIf(AusPrev("BeginDate") < dtBeginDate, dtBeginDate, AusPrev("BeginDate"))
                                Dim EDate = IIf(AusPrev("FinishDate") > dtEndDate, dtEndDate, AusPrev("FinishDate"))
                                Fecha = IDate

                                For j = 0 To DateDiff(DateInterval.Day, IDate, EDate)
                                    col = DateDiff(DateInterval.Day, dtBeginDate, Fecha) + 3
                                    excel.SetCellValue(eRow, col, "AP", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                    Fecha = Fecha.AddDays(1)
                                Next
                            Next

                            ContractAnterior = row("NumContrato")
                            EmpleadoAnterior = row("idEmployee")
                        End If

                        ' Determina columna en el excel según fecha
                        col = DateDiff(DateInterval.Day, dtBeginDate, Convert.ToDateTime(row("Date"))) + 3

                        'SOLICITUD DIA COMPLETO (vac/perm por horas)
                        For Each req In dtRequests.Select("idEmployee=" & row("idEmployee") & " and Date='" & Format(row("Date"), "yyyy/MM/dd") & "' AND RequestType=13 AND AllDay=True ")
                            Dim tbDetail = VTBusiness.Scheduler.roScheduler.GetPlan(row("idEmployee"), row("Date"), row("Date"), oEmployeeState, , True)
                            If (tbDetail.Rows.Count > 0) Then excel.SetCellValue(eRow, col, roConversions.ConvertHoursToTime(tbDetail(0)("ExpectedWorkingHours1")) & " (PEN)", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                        Next

                        'SOLICITUD POR HORAS (vac/perm por horas)
                        For Each req In dtRequests.Select("idEmployee=" & row("idEmployee") & " and Date='" & Format(row("Date"), "yyyy/MM/dd") & "' AND RequestType=13 AND AllDay=False")
                            excel.SetCellValue(eRow, col, roConversions.ConvertHoursToTime(req("Duration")) & " (PEN)", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                        Next

                        'SOLICITUD POR HORARIO (por horario) - "LLUis: Vacaciones por dias. ha de funcionar com abans, nomes mostrar dues lletres).
                        For Each req In dtRequests.Select("idEmployee=" & row("idEmployee") & " and Date='" & Format(row("Date"), "yyyy/MM/dd") & "' AND RequestType=6")
                            excel.SetCellValue(eRow, col, "V (PEN)", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                            If req("ShiftColor") IsNot Nothing Then excel.SetPattern(eRow, col, ColorTranslator.FromWin32(req("ShiftColor")), SafeForeColor(req("ShiftColor")))
                        Next

                        ''''''''
                        ''''''''    VACACIONES (ya aprobadas) POR HORAS (PUEDEN SER ALLDAY O PERIODO)
                        ''''''''
                        Dim totalPeriodes As New Double
                        For Each v As DataRow In dtVacPrev.Select("idEmployee=" & row("idEmployee") & " and Date='" & Format(row("Date"), "yyyy/MM/dd") & "' ")
                            If (roTypes.Any2Boolean(v("AllDay").ToString())) Then
                                ''''''ALLDAY
                                VacancesAllDay = True
                                If Not IsDBNull(row("Shift")) Then
                                    Dim r = dtShiftsAll.Select("id=" & row("Shift"))
                                    excel.SetCellValue(eRow, col, roConversions.ConvertHoursToTime(r(0)("ExpectedWorkingHours")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                                End If
                            Else
                                ''''''PERIODO
                                VacancesPeriod = True
                                totalPeriodes += Convert.ToDouble(v("Duration"))
                                excel.SetCellValue(eRow, col, roConversions.ConvertHoursToTime(totalPeriodes), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, True)
                            End If
                        Next

                        'si tiene horario...
                        If Not IsDBNull(row("Shift")) Then
                            ' si es un horario de vacaciones..
                            If Not IsDBNull(row("IsHolidays")) AndAlso row("IsHolidays") <> 0 Then
                                ''''''VACACIONES POR HORARIO
                                ' Busca el horario base
                                Dim sb = dtShiftsAll.Select("id=" & row("idShiftBase"))
                                If sb.Length > 0 Then
                                    excel.SetCellValue(eRow, col, roConversions.ConvertHoursToTime(sb(0)("ExpectedWorkingHours")), , , True)

                                    Dim rh() As DataRow = dtShifts.Select("id=" & row("Shift"))
                                    If rh.Length > 0 Then excel.SetPattern(eRow, col, ColorTranslator.FromWin32(rh(0)("Color")), SafeForeColor(rh(0)("Color"))) ' dibuja color del horario
                                End If
                            Else
                                If excel.GetCellValue(eRow, col) = "??" Or VacancesPeriod Or VacancesAllDay Then
                                    ''''''DIA NORMAL DE TRABAJO
                                    ' Comprueba si es un horario festivo
                                    Dim r = dtShiftsAll.Select("id=" & row("Shift"))

                                    If (Not VacancesAllDay AndAlso Not VacancesPeriod) Then  'just to avoid overwriting once Vacacions per hores were written
                                        If Not bolWithShifts Then
                                            If r(0)("ExpectedWorkingHours") = 0 Then
                                                excel.SetCellValue(eRow, col, 0, "#", , True)
                                            Else
                                                excel.SetCellValue(eRow, col, "", , , True)
                                            End If
                                        Else
                                            excel.SetCellValue(eRow, col, r(0)("ShortName"), , True)
                                            excel.SetPattern(eRow, col, ColorTranslator.FromWin32(r(0)("Color")), SafeForeColor(r(0)("Color")))
                                        End If

                                    End If

                                    If r(0)("ExpectedWorkingHours") <> 0 Then
                                        Presents(DateDiff(DateInterval.Day, dtBeginDate, row("Date"))) += 1
                                        Absent = False
                                    End If

                                End If
                            End If
                        End If

                        If excel.GetCellValue(eRow, col) <> "SC" And Absent Then
                            Absents(DateDiff(DateInterval.Day, dtBeginDate, row("Date"))) += 1
                        End If

                        If VacancesAllDay Then 'VacancesAllDay afectan a Presents/Absents. En cambio, "ignoramos" vacaciones pequeñas (Periodos)
                            Presents(DateDiff(DateInterval.Day, dtBeginDate, row("Date"))) -= 1
                            Absents(DateDiff(DateInterval.Day, dtBeginDate, row("Date"))) += 1
                        End If
                    Else
                        Console.Write("Error: " & row("idEmployee") & vbCrLf)
                    End If
                Next

                ' Crea el pie del grupo
                If DptoAnterior <> "" Then
                    eRow += 1
                    Call DataLink_ExportHolidays_CreateFoot(excel, eRow, RowGroup, NumDays, Presents, Absents, Me.State)
                End If

                ' Autoajusta columnas
                For i = 1 To DateDiff(DateInterval.Day, dtBeginDate, dtEndDate) + 3
                    excel.AutoFitColumn(i, 500)
                Next i

                ' Graba el archivo
                excel.SaveFile()

                ' Devuelve array de bytes

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Libera memoria
                dtShifts.Dispose()
                dtSch.Dispose()
                dtAusPrev.Dispose()
                dtRequests.Dispose()
                dtContracts.Dispose()

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("ExportHolidays.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExcelExportHolidays")
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("Export.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile
        End Function

    End Class
End Namespace
