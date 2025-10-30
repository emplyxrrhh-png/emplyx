Imports Robotics.DataLayer
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class roFixedExportRequests
        Inherits roDataLinkExport


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportRequests(ByVal tmpEmployeeFilterTable As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0

                Dim NameFile As String = "ExportRequests#" & Me.State.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)
                Dim eRow As Integer = 2

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportRequests.LogEvent.Start", "") & vbNewLine

                ' Selecciona los conceptos a exportar
                Dim sSQL As String = String.Empty
                sSQL = $"@SELECT# Employees.Name,
                               Requests.Id IdRequest,
                        	   [dbo].[GetValueFromEmployeeUserFieldValues](Requests.IDEmployee, 'NIF', Requests.RequestDate) NIF,
                        	   Groups.Name GroupName,
                        	   EmployeeContracts.IDContract IDContract,
                        	   EmployeeContracts.BeginDate BeginContract,
                               CASE WHEN DATEPART(year,EmployeeContracts.Enddate) = 2079 THEN NULL ELSE EmployeeContracts.Enddate END EndContract,
                               CONVERT(DATE, Requests.RequestDate) RequestDate,
                               sysroRequestType.Type   RequestType,
                               CASE WHEN Requests.Status=0 THEN 'pending' WHEN Requests.Status=1 THEN 'ongoing' WHEN Requests.Status=2 THEN 'accepted' WHEN Requests.Status=3 THEN 'denied' WHEN Requests.Status=4 THEN 'canceled' END RequestStatus   ,
                               Isnull(causes.NAME, '')             CauseName,
                               Isnull(shifts.NAME, '')             ShiftName,
                               CASE WHEN Requests.RequestType = 6 THEN (@SELECT# COUNT(*) FROM  sysroRequestDays WHERE sysroRequestDays.IDRequest = Requests.ID) WHEN Requests.RequestType = 8 THEN NULL ELSE DATEDIFF(day,Date1,Date2) + 1 END AS NumDays,
                               Date1                               DateBegin,
                               CASE WHEN Requests.RequestType <> 8 THEN Date2 END                              DateEnd,
                               CASE WHEN FromTime IS NOT NULL THEN FORMAT(FromTime,'HH:mm')			   
                               ELSE (@SELECT# TOP(1) FORMAT(BeginTime,'HH:mm') FROM sysroRequestDays WHERE IDRequest = Requests.ID)
                               END AS FromTime,
                               CASE WHEN ToTime IS NOT NULL THEN FORMAT(ToTime,'HH:mm')			   
                               ELSE (@SELECT# TOP(1) FORMAT(EndTime,'HH:mm') FROM sysroRequestDays WHERE IDRequest = Requests.ID)
                               END AS ToTime,
                               CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, Hours * 3600, 0),24) AS TIME(0)))), 0),8)   TotalHours,
                               sysropassports.NAME                 LastSupervisorName,
                        	   ''                        NextSupervisors,
                        	   Requests.comments                   EmployeeComments,
                               Approvals.comments                  SupervisorComments,
                               CASE WHEN Requests.Status=2 THEN Approvals.DateTime END ApprovedAt   ,
							   CASE WHEN Requests.RequestType = 8 THEN Isnull(ExchangedShift.Name, '') END	ExchangedShiftName,
							   CASE WHEN Requests.RequestType = 8 THEN ISNULL(CompensationShift.Name, '') END  CompensationShift,
							   CASE WHEN Requests.RequestType = 8 THEN Date2 END                             CompensationDate,
							   CASE WHEN Requests.RequestType = 8 THEN ISNULL(EmployeeExchange.Name,'') END ExchangeWithEmployee,
                               Requests.Hours as RequestedTime
                        FROM   Requests
                               INNER JOIN {tmpEmployeeFilterTable} ON {tmpEmployeeFilterTable}.Id = Requests.IdEmployee
                               INNER JOIN sysroRequestType ON sysroRequestType.idtype = Requests.requesttype
                        	   INNER JOIN EmployeeContracts ON EmployeeContracts.IDEmployee = Requests.IDEmployee AND Requests.RequestDate BETWEEN EmployeeContracts.BeginDate AND EmployeeContracts.EndDate
                        	   INNER JOIN Employees ON Employees.Id = Requests.IDEmployee
                               LEFT JOIN Employees EmployeeExchange ON EmployeeExchange.Id = Requests.IDEmployeeExchange
                        	   LEFT JOIN EmployeeGroups ON EmployeeGroups.IDEmployee = requests.IDEmployee and  requests.RequestDate BETWEEN EmployeeGroups.BeginDate AND EmployeeGroups.EndDate
                        	   LEFT JOIN Groups ON Groups.ID = EmployeeGroups.IDGroup
                        	   LEFT JOIN causes ON causes.id = requests.idcause 
                               LEFT JOIN shifts ON shifts.id = requests.idshift
                               LEFT JOIN shifts ExchangedShift ON ExchangedShift.id = requests.Field4
                               LEFT JOIN (@SELECT# Row_number() OVER (partition BY idrequest ORDER BY idrequest ASC, datetime DESC) AS 'RowNumber1',* FROM RequestsApprovals) Approvals ON Approvals.IDRequest = Requests.ID AND ( RowNumber1 = 1 OR RowNumber1 IS NULL )
                               LEFT JOIN sysropassports ON Approvals.idpassport = sysropassports.id
							   LEFT JOIN DailySchedule ON DailySchedule.IDEmployee = Requests.IDEmployee AND DailySchedule.Date = Requests.Date2
							   LEFT JOIN Shifts CompensationShift ON CompensationShift.Id = DailySchedule.IDShift1
                        WHERE  CONVERT(DATE, requests.requestdate) BETWEEN {roTypes.Any2Time(dtBeginDate).SQLDateTime} AND {roTypes.Any2Time(dtEndDate).SQLDateTime}
                        ORDER BY Employees.Name ASC, Requests.RequestDate ASC"

                ' Cargamos datos
                Dim dt As DataTable = CreateDataTableWithoutTimeouts(sSQL, , "Requests")

                ' Crea la cabecera
                Dim arrColumnKeys As String() = {"Name", "NIF", "Group", "Contract", "BeginContract", "EndContract", "RequestDate", "RequestType", "Status", "Cause", "Shift", "TotalDays", "BeginDate", "EndDate", "BeginHour", "EndHour", "LastSupervisor", "NextSupervisor", "EmployeeComment", "SupervisorComments", "ApprovedAt", "ExchangedShiftName", "ExchangeWithEmployee", "CompensationShift", "CompensationDate", "RequestedTime"}
                Dim arrColumnDefaultNames As String() = {"Nombre", "NIF", "Grupo", "Contrato", "Fecha inicio contrato", "Fecha final contrato", "Fecha", "Tipo", "Estado", "Motivo", "Horario solicitado", "Días solicitados", "Fecha inicial solicitada", "Fecha final solicitada", "Hora inicial solicitada", "Hora final solicitada", "Gestionada por", "Pendiente de", "Comentarios usuario", "Comentarios supervisor", "Fecha aprobación definitiva", "Horario previo solicitud", "Empleado para intercambio", "Horario compensación", "Fecha compensación", "Total horas"}
                Dim dHeader As New Dictionary(Of String, String)
                Dim includeCompensation As Boolean = False
                Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered", New AdvancedParameter.roAdvancedParameterState())
                includeCompensation = roTypes.Any2Boolean(oParam.Value)

                For i = 0 To arrColumnKeys.Count - 1
                    If (includeCompensation AndAlso (arrColumnKeys(i) = "CompensationShift" OrElse arrColumnKeys(i) = "CompensationDate")) OrElse (arrColumnKeys(i) <> "CompensationShift" AndAlso arrColumnKeys(i) <> "CompensationDate") Then
                        dHeader.Add(arrColumnKeys(i), Me.State.Language.TranslateWithDefault("roDataLinkExport.Datalink_ExportRequests.Header." & arrColumnKeys(i), "", arrColumnDefaultNames(i)))
                    End If
                Next
                DataLink_Export_CreateHeaders(excel, dHeader, Me.State)

                Dim sSQLAux As String = String.Empty
                Dim sNextSupervisors As String = String.Empty
                Dim oLanguage As roLanguage = New roLanguage
                oLanguage.SetLanguageReference("LiveRequests", Me.State.Language.LanguageKey)
                ' Informamos resto de filas
                For Each row As System.Data.DataRow In dt.Rows
                    Try
                        excel.SetCellValue(eRow, 1, row("Name"))
                        excel.SetCellValue(eRow, 2, row("NIF"))
                        excel.SetCellValue(eRow, 3, row("GroupName"))
                        excel.SetCellValue(eRow, 4, row("IDContract"))
                        excel.SetCellValue(eRow, 5, row("BeginContract"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 6, row("EndContract"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 7, row("RequestDate"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 8, oLanguage.TranslateWithDefault($"Requests.RequestType.{row("RequestType")}", "", row("RequestType")))
                        excel.SetCellValue(eRow, 9, oLanguage.TranslateWithDefault($"Requests.RequestStatus.{row("RequestStatus")}", "", row("RequestStatus")))
                        excel.SetCellValue(eRow, 10, row("CauseName"))
                        excel.SetCellValue(eRow, 11, row("ShiftName"))
                        excel.SetCellValue(eRow, 12, row("NumDays"), "##0")
                        excel.SetCellValue(eRow, 13, row("DateBegin"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 14, row("DateEnd"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 15, row("FromTime"), "HH:mm")
                        excel.SetCellValue(eRow, 16, row("ToTime"), "HH:mm")
                        excel.SetCellValue(eRow, 17, row("LastSupervisorName"))
                        sNextSupervisors = String.Empty
                        If row("RequestStatus") = "pending" OrElse row("RequestStatus") = "ongoing" Then
                            sSQLAux = $"@SELECT# STRING_AGG(sysroPassports.Name, ', ') WITHIN GROUP (ORDER BY sysroPassports.Name) AS SupervisorName
                                    FROM sysrovwSecurity_PendingRequestsDependencies
                                    INNER JOIN sysroPassports ON sysroPassports.Id = sysrovwSecurity_PendingRequestsDependencies.IdPassport AND sysrovwSecurity_PendingRequestsDependencies.IsRoboticsUser = 0
                                    WHERE IdRequest = @idrequest AND DirectDependence = 1 AND AutomaticValidation = 0"

                            sSQLAux &= SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.DataLink_ExportRequests)

                            Dim parameters As New List(Of CommandParameter)
                            parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, row("IdRequest")))
                            sNextSupervisors = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(sSQLAux, parameters))
                        End If
                        excel.SetCellValue(eRow, 18, sNextSupervisors)
                        excel.SetCellValue(eRow, 19, row("EmployeeComments"))
                        excel.SetCellValue(eRow, 20, row("SupervisorComments"))
                        excel.SetCellValue(eRow, 21, row("ApprovedAt"), "yyyy/MM/dd")
                        excel.SetCellValue(eRow, 22, row("ExchangedShiftName"))
                        excel.SetCellValue(eRow, 23, row("ExchangeWithEmployee"))
                        If includeCompensation Then
                            excel.SetCellValue(eRow, 24, row("CompensationShift"))
                            excel.SetCellValue(eRow, 25, row("CompensationDate"), "yyyy/MM/dd")
                            If Not IsDBNull(row("RequestedTime")) Then
                                excel.SetCellValue(eRow, 26, roTypes.Any2DateTime(row("RequestedTime")).ToShortTimeString(), "HH:mm")
                            End If
                        Else
                            If Not IsDBNull(row("RequestedTime")) Then
                                excel.SetCellValue(eRow, 24, roTypes.Any2DateTime(row("RequestedTime")).ToShortTimeString(), "HH:mm")
                            End If
                        End If

                    Catch ex As Exception
                        'Do nothing
                    End Try
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
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportRequests.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::DataLink_ExportRequests")
                strlogevent += Now.ToString & " --> " & Me.State.Language.Translate("DataLink_ExportRequests.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine, idSchedule)
            End Try

            Return arrFile

        End Function

    End Class

End Namespace