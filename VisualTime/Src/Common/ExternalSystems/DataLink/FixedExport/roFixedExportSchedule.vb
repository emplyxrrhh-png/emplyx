Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Namespace DataLink

    Public Class roFixedExportSchedule
        Inherits roDataLinkExport


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportSchedule(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim oLog As New roLog("DataLink_ExportSchedule")

            Try
                Dim i As Integer = 0

                Dim NameFile As String = "ExportSchedule#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                oLog.logMessage(roLog.EventType.roDebug, "Starting Schedule Export ")

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)
                Dim eRow As Integer = 2

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportSchedule.LogEvent.Start", "") & vbNewLine

                'Recojo todas la info de planificación mediante CalendarManager (God Bless)

                Dim oCalendarState As Base.VTCalendar.roCalendarState = New Base.VTCalendar.roCalendarState(Me.State.IDPassport)
                Dim oCalendarManager As New Robotics.Base.VTCalendar.roCalendarManager(oCalendarState)
                Dim oCalendar As New DTOs.roCalendar
                oLog.logMessage(roLog.EventType.roDebug, "Loading data ... ")
                oCalendar = oCalendarManager.Load(dtBeginDate, dtEndDate, mEmployees, DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True, True)

                ' Compacto para evitar que haya varias filas para empleados que hayan tenido movilidad en el periodo
                Dim oCalendarCompact As New List(Of roCalendarRow)
                Dim oCalRuleState As New VTCalendar.roCalendarScheduleRulesState(Me.State.IDPassport)
                Dim oCalRulesManager As New VTCalendar.roCalendarScheduleRulesManager(oCalRuleState)
                oCalendarCompact = oCalRulesManager.CompactCalendar(oCalendar)

                ' Crea la cabecera
                Dim arrColumnKeys As String() = {"Name", "NIF", "Group", "FullGroupName", "Contract", "BeginContract", "EndContract", "Date", "Shift", "ShortName", "Export", "Feast", "StartLayer1", "EndLayer1", "StartLayer2", "EndLayer2", "OrdinaryHours", "ComplementaryHours", "AbsenceCause", "AbsenceTime", "OvertimeCause", "OvertimeTime", "HolicadysCause", "HolidaysTime"}
                Dim arrColumnDefaultNames As String() = {"Nombre", "NIF", "Grupo", "Grupo completo", "Contrato", "Fecha inicio contrato", "Fecha final contrato", "Fecha", "Horario", "Nombre abreviado", "Equivalencia", "Es festivo", "Hora entrada franja 1", "Hora salida franja 1", "Hora entrada franja 2", "Hora salida franja 2", "Horas ordinarias", "Hora complementarias", "Motivo previsión ausencia", "Tiempo previsión ausencia", "Motivo previsión de exceso", "Tiempo previsión exceso", "Motivo vacaciones o permisos por horas", "Tiempo vacaciones o permisos por horas"}
                Dim dHeader As New Dictionary(Of String, String)
                For i = 0 To arrColumnKeys.Count - 1
                    dHeader.Add(arrColumnKeys(i), Me.State.Language.TranslateWithDefault("roDataLinkExport.Datalink_ExportSchedule.Header." & arrColumnKeys(i), "", arrColumnDefaultNames(i)))
                Next
                DataLink_Export_CreateHeaders(excel, dHeader, Me.State)

                Dim iStart As Integer = 2
                Dim dtEmployees As DataTable '= GetEmployeeGeneralData(String.Join(",", oCalendarCompact.Select(Function(x) x.EmployeeData.IDEmployee).ToArray), dtBeginDate, dtEndDate, oCn.Connection, Me.state)
                Dim oSimplified As VTCalendar.roDayShiftSimplified

                'Dim dv As DataView = dtEmployees.DefaultView
                Dim oEmpRow As DataRow

                oLog.logMessage(roLog.EventType.roDebug, "... data ready")

                Dim n As Integer = 1
                For Each oCalRow As roCalendarRow In oCalendarCompact
                    ' Empleado
                    oLog.logMessage(roLog.EventType.roDebug, "Exporting Schedule for employee number " & n.ToString & " out of " & oCalendarCompact.Count.ToString)
                    For Each oDayData As roCalendarRowDayData In oCalRow.PeriodData.DayData
                        Try
                            ' Recupero información específica del empleado para ese día
                            ' dv.RowFilter = "IdEmployee = " & oCalRow.EmployeeData.IDEmployee.ToString & " AND RefDate = '" & Format(oDayData.PlanDate, "yyyy/MM/dd") & "'"
                            ' dv.Sort = "Name ASC, RefDate ASC, HourAbsence DESC, Overtime DESC, VacationsTime DESC"
                            ' dtEmp = dv.ToTable

                            ' Mejor rendimiento si hagola consulta por empleado y día ...
                            dtEmployees = GetEmployeeGeneralData(oCalRow.EmployeeData.IDEmployee.ToString, oDayData.PlanDate, oDayData.PlanDate)

                            If dtEmployees.Rows.Count > 0 Then
                                oEmpRow = dtEmployees.Rows(0)
                            Else
                                Continue For
                            End If

                            oSimplified = New VTCalendar.roDayShiftSimplified(oDayData.PlanDate)
                            oSimplified = oCalendarManager.GetDayDataSimplified(oDayData)

                            excel.SetCellValue(eRow, 1, oCalRow.EmployeeData.EmployeeName)
                            excel.SetCellValue(eRow, 2, oEmpRow("NIF"))
                            excel.SetCellValue(eRow, 3, oEmpRow("GroupName"))
                            excel.SetCellValue(eRow, 4, oEmpRow("FullGroupName"))
                            excel.SetCellValue(eRow, 5, oEmpRow("IDContract"))
                            excel.SetCellValue(eRow, 6, oEmpRow("BeginDate"), "yyyy/MM/dd")
                            excel.SetCellValue(eRow, 7, oEmpRow("EndDate"), "yyyy/MM/dd")
                            excel.SetCellValue(eRow, 8, oDayData.PlanDate, "yyyy/MM/dd")
                            excel.SetCellValue(eRow, 9, If(oDayData.MainShift IsNot Nothing, oDayData.MainShift.Name, ""))
                            excel.SetCellValue(eRow, 10, If(oDayData.MainShift IsNot Nothing, oDayData.MainShift.ShortName, ""))
                            excel.SetCellValue(eRow, 11, If(oDayData.MainShift IsNot Nothing, oDayData.MainShift.Export, ""))
                            excel.SetCellValue(eRow, 12, If(oDayData.MainShift IsNot Nothing AndAlso oDayData.Feast, "x", ""))
                            If oSimplified IsNot Nothing Then
                                excel.SetCellValue(eRow, 13, oSimplified.Layer1StartText, "HH:mm")
                                excel.SetCellValue(eRow, 14, oSimplified.Layer1EndText, "HH:mm")
                                excel.SetCellValue(eRow, 15, oSimplified.Layer2StartText, "HH:mm")
                                excel.SetCellValue(eRow, 16, oSimplified.Layer2EndText, "HH:mm")
                                excel.SetCellValue(eRow, 17, oSimplified.OrdinaryTimeText, "HH:mm")
                                excel.SetCellValue(eRow, 18, oSimplified.ComplementaryTimeText, "HH:mm")
                            End If
                            If Not IsDBNull(oEmpRow("DayAbsenceCause")) Then
                                excel.SetCellValue(eRow, 19, oEmpRow("DayAbsenceCause"))
                                excel.SetCellValue(eRow, 20, oEmpRow("DaysAbsence"))
                            ElseIf Not IsDBNull(oEmpRow("HoursAbsenceCause")) Then
                                excel.SetCellValue(eRow, 19, oEmpRow("HoursAbsenceCause"))
                                excel.SetCellValue(eRow, 20, oEmpRow("HourAbsence"), "HH:mm")
                            End If
                            excel.SetCellValue(eRow, 21, oEmpRow("OvertimeCause"))
                            excel.SetCellValue(eRow, 22, oEmpRow("Overtime"), "HH:mm")
                            excel.SetCellValue(eRow, 23, oEmpRow("VacationCause"))
                            excel.SetCellValue(eRow, 24, oEmpRow("VacationsTime"), "HH:mm")
                        Catch ex As Exception

                        End Try

                        eRow += 1
                    Next
                    n += 1
                Next

                ' Autoajusta columnas
                oLog.logMessage(roLog.EventType.roDebug, "Applying format ... ")

                excel.AutoFitColumn(1, excel.GetWorksheetStatistics.EndColumnIndex)

                oLog.logMessage(roLog.EventType.roDebug, "Format applied... ")

                ' Graba el archivo
                excel.SaveFile()
                oLog.logMessage(roLog.EventType.roDebug, "Schedule Export done !")

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportSchedule.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::DataLink_ExportSchedule")
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportSchedule.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine, idSchedule)
            End Try

            Return arrFile

        End Function


        Private Function GetEmployeeGeneralData(strEmployees As String, dDateIni As Date, dDateEnd As Date) As DataTable
            Dim sSQL As String
            Dim dt As New DataTable

            Try
                sSQL = "WITH alldays AS (  " &
                        "@SELECT# " & roTypes.Any2Time(dDateIni.Date).SQLSmallDateTime & " AS dt  " &
                        "UNION ALL  " &
                        "@SELECT# DATEADD(dd, 1, dt)  " &
                        "FROM alldays s  " &
                        "WHERE DATEADD(dd, 1, dt) <=  " & roTypes.Any2Time(dDateEnd.Date).SQLSmallDateTime & "  " &
                        ") " &
                        "@SELECT#  Employees.Name,  " &
                                "Employees.Id IdEmployee, " &
                                "EmployeeContracts.IDContract,   " &
                                "EmployeeContracts.BeginDate,   " &
                                "CASE When DATEPART(year,EmployeeContracts.Enddate) = 2079 Then NULL Else EmployeeContracts.Enddate End EndDate,  " &
                                "Groups.Name GroupName,  " &
                                "Groups.FullGroupName FullGroupName,  " &
                                "alldays.dt RefDate, " &
                                "EmployeeUserFieldValues.Value NIF,  " &
                                "C1.Name DayAbsenceCause, " &
                                "DATEDIFF(DAY,ProgrammedAbsences.BeginDate, Case When ProgrammedAbsences.FinishDate Is NULL Then DATEADD(day, ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate) Else ProgrammedAbsences.FinishDate End) + 1 DaysAbsence, " &
                                "C2.Name HoursAbsenceCause, " &
                                "CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedCauses.Duration * 3600, 0),24) As TIME(0)))), 0),8) HourAbsence, " &
                                "C3.Name OvertimeCause, " &
                                "CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedOvertimes.Duration * 3600, 0),24) As TIME(0)))), 0),8) Overtime, " &
                                "C4.Name VacationCause, " &
                                "CASE When ProgrammedHolidays.AllDay = 1 Then '24:00' ELSE CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, DATEDIFF(minute, ProgrammedHolidays.BeginTime,ProgrammedHolidays.EndTime) , 0),24) AS TIME(0)))), 0),8) END VacationsTime " &
                        "FROM Employees " &
                        "CROSS JOIN alldays " &
                        "INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = Employees.Id AND  dt BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate  " &
                        "LEFT JOIN ProgrammedAbsences ON ProgrammedAbsences.IDEmployee = Employees.Id AND dt BETWEEN ProgrammedAbsences.BeginDate AND CASE WHEN ProgrammedAbsences.FinishDate IS NULL THEN DATEADD(day, ProgrammedAbsences.MaxLastingDays-1,ProgrammedAbsences.BeginDate) ELSE ProgrammedAbsences.FinishDate END " &
                        "LEFT JOIN Causes C1 ON C1.id = ProgrammedAbsences.IDCause  " &
                        "LEFT JOIN ProgrammedCauses ON ProgrammedCauses.IDEmployee = Employees.Id AND dt BETWEEN ProgrammedCauses.Date AND ProgrammedCauses.FinishDate " &
                        "LEFT JOIN Causes C2 ON C2.id = ProgrammedCauses.IDCause  " &
                        "LEFT JOIN ProgrammedOvertimes ON ProgrammedOvertimes.IDEmployee = Employees.Id AND dt BETWEEN ProgrammedOvertimes.BeginDate AND ProgrammedOvertimes.EndDate " &
                        "LEFT JOIN Causes C3 ON C3.id = ProgrammedOvertimes.IDCause  " &
                        "LEFT JOIN ProgrammedHolidays ON ProgrammedHolidays.IDEmployee = Employees.Id AND dt = ProgrammedHolidays.Date " &
                        "LEFT JOIN Causes C4 ON C4.id = ProgrammedHolidays.IDCause  " &
                        "LEFT JOIN EmployeeGroups ON EmployeeGroups.IDEmployee = Employees.Id and  dt BETWEEN EmployeeGroups.BeginDate AND EmployeeGroups.EndDate  " &
                        "LEFT JOIN Groups ON Groups.ID = EmployeeGroups.IDGroup " &
                        "LEFT JOIN EmployeeUserFieldValues ON EmployeeUserFieldValues.IDEmployee = Employees.Id AND EmployeeUserFieldValues.FieldName = 'NIF'  " &
                        "WHERE Employees.ID IN (" & strEmployees & ") " &
                        "ORDER BY Employees.Name ASC, RefDate ASC, HourAbsence DESC, Overtime DESC, VacationsTime DESC " &
                        "OPTION (maxrecursion 0) "
                dt = DataLayer.AccessHelper.CreateDataTable(sSQL, "EmployeesData")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetEmployeeGeneralData")
            End Try

            Return dt
        End Function



    End Class

End Namespace