Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roFixedExportAbsences
        Inherits roDataLinkExport


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportAbsences(ByVal tmpEmployeeFilterTable As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0

                Dim NameFile As String = "ExportAbsences#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim eRow As Integer = 2

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportAbsences.LogEvent.Start", "") & vbNewLine

                ' Selecciona los conceptos a exportar
                Dim sSQL As String = String.Empty
                sSQL = "@SELECT#	Employees.Name, " &
                            "CONVERT(VARCHAR,EmployeeUserFieldValues.Value) NIF, " &
                            "sysrovwEmployeesInAllGroups.GroupName, " &
                            "EmployeeContracts.IDContract, " &
                            "EmployeeContracts.BeginDate ContractBeginDate, " &
                            "CASE WHEN DATEPART(YEAR,EmployeeContracts.EndDate) = 2079 THEN NULL ELSE EmployeeContracts.EndDate END ContractEndDate, " &
                            "dbo.Causes.Name CauseName, " &
                            "dbo.Causes.ShortName,  " &
                            "dbo.Causes.Export,  " &
                            "dbo.ProgrammedAbsences.BeginDate,  " &
                            "ISNULL(FinishDate,DATEADD(day, MaxLastingDays-1, ProgrammedAbsences.BeginDate)) FinishDate,  " &
                            "DATEDIFF(day, programmedabsences.begindate,Isnull(finishdate, Dateadd(day, maxlastingdays - 1,programmedabsences.begindate))) + 1 TotalDays, " &
                            "NULL HourBegin, " &
                            "NULL HourEnd, " &
                            "NULL TotalHours, " &
                            "NULL DateCreated, " &
                            "ProgrammedAbsences.Timestamp DateEdited, " &
                            "CONVERT(VARCHAR,ProgrammedAbsences.Description) Description " &
                        "FROM dbo.ProgrammedAbsences  " &
                        "INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = dbo.ProgrammedAbsences.IdEmployee " &
                        "INNER JOIN dbo.Causes ON dbo.ProgrammedAbsences.IDCause = dbo.Causes.ID  " &
                        "INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = ProgrammedAbsences.IDEmployee AND ProgrammedAbsences.BeginDate BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate " &
                        "INNER JOIN Employees ON Employees.ID = ProgrammedAbsences.IDEmployee " &
                        "INNER JOIN sysrovwEmployeesInAllGroups ON sysrovwEmployeesInAllGroups.IDEmployee = ProgrammedAbsences.IDEmployee  AND ProgrammedAbsences.BeginDate BETWEEN sysrovwEmployeesInAllGroups.BeginDate AND sysrovwEmployeesInAllGroups.EndDate " &
                        "LEFT OUTER JOIN EmployeeUserFieldValues ON EmployeeUserFieldValues.IDEmployee = ProgrammedAbsences.IDEmployee AND EmployeeUserFieldValues.FieldName = 'NIF' " &
                        "WHERE	ProgrammedAbsences.BeginDate <= " & roTypes.Any2Time(dtEndDate).SQLDateTime & "  " &
                        "		AND (ISNULL(FinishDate,DATEADD(day, MaxLastingDays-1, ProgrammedAbsences.BeginDate)) >= " & roTypes.Any2Time(dtBeginDate).SQLDateTime & ")  " &
                        "UNION  " &
                        "@SELECT#	Employees.Name, " &
                        "		CONVERT(VARCHAR,EmployeeUserFieldValues.Value) NIF, " &
                        "       sysrovwEmployeesInAllGroups.GroupName, " &
                        "		EmployeeContracts.IDContract, " &
                        "		EmployeeContracts.BeginDate ContractBeginDate, " &
                        "		CASE WHEN DATEPART(YEAR,EmployeeContracts.EndDate) = 2079 THEN NULL ELSE EmployeeContracts.EndDate END ContractEndDate, " &
                        "		dbo.Causes.Name CauseName, " &
                        "		dbo.Causes.ShortName,  " &
                        "       dbo.Causes.Export,  " &
                        "		dbo.ProgrammedCauses.Date BeginDate,  " &
                        "		dbo.ProgrammedCauses.FinishDate,  " &
                        "		NULL TotalDays, " &
                        "		FORMAT(ProgrammedCauses.Begintime,'HH:mm') HourBegin, " &
                        "		FORMAT(ProgrammedCauses.EndTime,'HH:mm') HourEnd, " &
                        "		CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedCauses.Duration * 3600, 0),24) As TIME(0)))), 0),108) TotalHours, " &
                        "		NULL DateCreated, " &
                        "		ProgrammedCauses.Timestamp DateEdited, " &
                        "		CONVERT(VARCHAR,ProgrammedCauses.Description) Description " &
                        "FROM dbo.ProgrammedCauses  " &
                        "INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = dbo.ProgrammedCauses.IdEmployee " &
                        "INNER JOIN dbo.Causes ON dbo.ProgrammedCauses.IDCause = dbo.Causes.ID  " &
                        "INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = ProgrammedCauses.IDEmployee AND ProgrammedCauses.Date BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate " &
                        "INNER JOIN Employees ON Employees.ID = ProgrammedCauses.IDEmployee " &
                        "INNER JOIN sysrovwEmployeesInAllGroups ON sysrovwEmployeesInAllGroups.IDEmployee = ProgrammedCauses.IDEmployee  AND ProgrammedCauses.Date BETWEEN sysrovwEmployeesInAllGroups.BeginDate AND sysrovwEmployeesInAllGroups.EndDate " &
                        "LEFT OUTER JOIN EmployeeUserFieldValues ON EmployeeUserFieldValues.IDEmployee = ProgrammedCauses.IDEmployee AND EmployeeUserFieldValues.FieldName = 'NIF' " &
                        "WHERE	ProgrammedCauses.Date <= " & roTypes.Any2Time(dtEndDate).SQLDateTime & "  " &
                        "		AND ProgrammedCauses.FinishDate >= " & roTypes.Any2Time(dtBeginDate).SQLDateTime & " " &
                        "UNION  " &
                        "@SELECT#	Employees.Name, " &
                        "		CONVERT(VARCHAR,EmployeeUserFieldValues.Value) NIF, " &
                        "       sysrovwEmployeesInAllGroups.GroupName, " &
                        "		EmployeeContracts.IDContract, " &
                        "		EmployeeContracts.BeginDate ContractBeginDate, " &
                        "		CASE WHEN DATEPART(YEAR,EmployeeContracts.EndDate) = 2079 THEN NULL ELSE EmployeeContracts.EndDate END ContractEndDate, " &
                        "		dbo.Causes.Name CauseName, " &
                        "		dbo.Causes.ShortName,  " &
                        "       dbo.Causes.Export,  " &
                        "		dbo.ProgrammedHolidays.Date BeginDate,  " &
                        "		dbo.ProgrammedHolidays.Date FinishDate,  " &
                        "		NULL TotalDays, " &
                        "		CASE WHEN ProgrammedHolidays.AllDay = 0 THEN FORMAT(ProgrammedHolidays.BeginTime,'hh:mm') ELSE NULL END HourBegin, " &
                        "		CASE WHEN ProgrammedHolidays.AllDay = 0 THEN FORMAT(ProgrammedHolidays.EndTime,'hh:mm') ELSE NULL END HourEnd, " &
                        "		CASE WHEN ProgrammedHolidays.AllDay = 0 THEN CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedHolidays.Duration * 3600, 0),24) As TIME(0)))), 0),108) ELSE NULL END TotalHours, " &
                        "		NULL DateCreated, " &
                        "		NULL DateEdited, " &
                        "		CONVERT(VARCHAR,ProgrammedHolidays.Description) Description " &
                        "FROM dbo.ProgrammedHolidays  " &
                        "INNER JOIN " & tmpEmployeeFilterTable & " ON " & tmpEmployeeFilterTable & ".Id = dbo.ProgrammedHolidays.IdEmployee " &
                        "INNER JOIN dbo.Causes ON dbo.ProgrammedHolidays.IDCause = dbo.Causes.ID  " &
                        "INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = ProgrammedHolidays.IDEmployee AND ProgrammedHolidays.Date BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate " &
                        "INNER JOIN Employees ON Employees.ID = ProgrammedHolidays.IDEmployee " &
                        "INNER JOIN sysrovwEmployeesInAllGroups ON sysrovwEmployeesInAllGroups.IDEmployee = ProgrammedHolidays.IDEmployee  AND ProgrammedHolidays.Date BETWEEN sysrovwEmployeesInAllGroups.BeginDate AND sysrovwEmployeesInAllGroups.EndDate " &
                        "LEFT OUTER JOIN EmployeeUserFieldValues ON EmployeeUserFieldValues.IDEmployee = ProgrammedHolidays.IDEmployee AND EmployeeUserFieldValues.FieldName = 'NIF' " &
                        "WHERE	ProgrammedHolidays.Date BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLDateTime & " AND " & roTypes.Any2Time(dtEndDate).SQLDateTime & " " &
                        "ORDER BY Name ASC, BeginDate ASC "

                ' Cargamos datos
                Dim dt As DataTable = CreateDataTable(sSQL, "Absences")

                ' Crea la cabecera
                Dim arrColumnKeys() As String = {"Name", "NIF", "Group", "Contract", "BeginContract", "EndContract", "Cause", "CauseShortName", "CauseExport", "AbsenceBeginDate", "AbsenceEndDate", "TotalDays", "AbsenceBeginTime", "AbsenceEndTime", "TotalHours", "LastModified", "Notes"}
                Dim arrColumnDefaultNames() As String = {"Nombre", "NIF", "Grupo", "Contrato", "Fecha inicio contrato", "Fecha final contrato", "Motivo", "Nombre corto motivo", "Código exportación motivo", "Fecha inicio ausencia", "Fecha final ausencia", "Días", "Hora inicio ausencia", "Hora final ausencia", "Horas", "Última modificación", "Observaciones"}
                Dim dHeader As New Dictionary(Of String, String)
                For i = 0 To arrColumnKeys.Count - 1
                    dHeader.Add(arrColumnKeys(i), Me.State.Language.TranslateWithDefault("roDataLinkExport.Datalink_ExportAbsences.Header." & arrColumnKeys(i), "", arrColumnDefaultNames(i)))
                Next
                DataLink_Export_CreateHeaders(excel, dHeader, Me.State)

                ' Informamos resto de filas
                For Each row As System.Data.DataRow In dt.Rows
                    excel.SetCellValue(eRow, 1, row("Name"))
                    excel.SetCellValue(eRow, 2, row("NIF"))
                    excel.SetCellValue(eRow, 3, row("GroupName"))
                    excel.SetCellValue(eRow, 4, row("IDContract"))
                    excel.SetCellValue(eRow, 5, row("ContractBeginDate"), "yyyy/MM/dd")
                    excel.SetCellValue(eRow, 6, row("ContractEndDate"), "yyyy/MM/dd")
                    excel.SetCellValue(eRow, 7, row("CauseName"))
                    excel.SetCellValue(eRow, 8, row("ShortName"))
                    excel.SetCellValue(eRow, 9, row("Export"))
                    excel.SetCellValue(eRow, 10, row("BeginDate"), "yyyy/MM/dd")
                    excel.SetCellValue(eRow, 11, row("FinishDate"), "yyyy/MM/dd")
                    excel.SetCellValue(eRow, 12, row("TotalDays"))
                    excel.SetCellValue(eRow, 13, row("HourBegin"), "HH:mm")
                    excel.SetCellValue(eRow, 14, row("HourEnd"), "HH:mm")
                    excel.SetCellValue(eRow, 15, row("TotalHours"), "HH:mm")
                    excel.SetCellValue(eRow, 16, row("DateEdited"), "yyyy/MM/dd HH:mm")
                    excel.SetCellValue(eRow, 17, row("Description"))
                    eRow += 1
                Next

                ' Autoajusta columnas

                excel.AutoFitColumn(1, excel.GetWorksheetStatistics.EndColumnIndex)

                ' Graba el archivo
                excel.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Libera memoria
                dt.Dispose()

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportAbsences.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::DataLink_ExportAbsences")
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportAbsences.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDataLinkExport::DataLink_ExportAbsences:Exception:Message ", ex)
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine, idSchedule)
            End Try

            Return arrFile

        End Function

    End Class
End Namespace