Imports System.Data.Common
Imports Robotics.DataLayer
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.Azure

Namespace DataLink

    Public Class roCustomLiven
        Inherits roDataLinkExport

        '"LIVEN - Exportación de justificaciones diarias (ID 9880)"
        Public Shared Function EXCELExportDailyCausesLIVEN(ByVal nIDExport As Integer, ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim cn As DbConnection = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0

                Dim NameFile As String

                ' Vemos si hay que filtrar las justificaciones a mostrar por su código de exportación
                Dim bOnlyCausesWithExportCode As Boolean = True
                bOnlyCausesWithExportCode = (roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "LIVEN.OnlyCausesWithExportCode")) = "1")

                ' Si es automática, exporto los tres últimos meses completos desde hoy
                NameFile = "LIVEN_DAilyCauses#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim eRow As Integer = 2
                Dim eCol As Integer = 0

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("ExportDailyCauses.LogEvent.Start", "") & vbNewLine

                ' Selecciona las justificaciones diarias
                Dim sSQL As String = String.Empty
                sSQL = "@SELECT#  " &
                        "CONVERT(DATE,dc.Date) as Fecha,  " &
                        "CONVERT(VARCHAR,eufv.Value) DNI, " &
                        "Causes.id              AS CodigoInc, " &
                        "Causes.Name              AS CauseName, " &
                        "CEILING(Sum(dc.value)*60) AS Minutos  " &
                        "FROM   dailycauses DC  " &
                        "INNER JOIN Employees ON Employees.ID = DC.IDEmployee  " &
                        "INNER JOIN dbo.causes ON dc.idcause = causes.id  "
                If bOnlyCausesWithExportCode Then
                    sSQL = sSQL & " AND causes.Export <> '0' "
                End If
                sSQL = sSQL & " INNER JOIN EmployeeUserfieldValues eufv ON eufv.IDEmployee = Employees.Id AND eufv.FieldName = 'NIF' " &
                        "LEFT JOIN DailyIncidences DI ON DI.IDEmployee = DC.IDEmployee AND DI.Date = DC.Date AND DC.IDRelatedIncidence = DI.ID   " &
                        "LEFT JOIN TimeZones ON TimeZones.ID = DI.IDZone  " &
                        "LEFT JOIN dailyschedule ds  " &
                        "        ON dc.idemployee = ds.idemployee  " &
                        "            AND dc.date = ds.date  " &
                        "WHERE  ds.status >= 60  " &
                        "AND dc.idemployee IN (" & mEmployees & ") " &
                        "AND dc.date BETWEEN  " & roTypes.Any2Time(dtBeginDate).SQLDateTime & " and " & roTypes.Any2Time(dtEndDate).SQLDateTime &
                        "GROUP  BY  CONVERT(DATE,dc.Date), " &
                        "           CONVERT(VARCHAR,eufv.Value), " &
                        "			causes.name,  " &
                        "			Causes.ID, " &
                        "           dc.date " &
                        "ORDER BY DC.Date ASC, DNI ASC "

                Dim dtDailyCauses As DataTable = CreateDataTableWithoutTimeouts(sSQL)


                ' Crea la cabecera
                Dim arrColumnKeys() As String = {"Fecha", "DNI", "CodigoInc", "Minutos"}
                Dim arrColumnDefaultNames() As String = {"Fecha", "DNI", "CodigoInc", "Minutos"}
                Dim dHeader As New Dictionary(Of String, String)
                For i = 0 To arrColumnKeys.Count - 1
                    dHeader.Add(arrColumnKeys(i), arrColumnDefaultNames(i))
                Next
                DataLink_Export_CreateHeaders(excel, dHeader, oState)

                Dim dCausesDictionary As New Dictionary(Of Integer, String)

                ' Informamos resto de filas
                For Each row As System.Data.DataRow In dtDailyCauses.Rows
                    excel.SetCellValue(eRow, 1, row("Fecha"), "dd/MM/yyyy")
                    excel.SetCellValue(eRow, 2, row("DNI"))
                    excel.SetCellValue(eRow, 3, row("CodigoInc"))
                    excel.SetCellValue(eRow, 4, row("Minutos"))
                    If Not dCausesDictionary.ContainsKey(row("CodigoInc")) Then
                        dCausesDictionary.Add(row("CodigoInc"), row("CauseName"))
                    End If
                    eRow += 1
                Next

                ' Autoajusta columnas

                excel.AutoFitColumn(1, excel.GetWorksheetStatistics.EndColumnIndex)


                ' Renombro Hoja 1
                excel.RenameSheet(0, "VTReg")

                ' Cambio a Hoja 2 y la llamo CodInc
                excel.AddWorkSheet("CodInc")
                excel.SetCellValue(1, 1, "Cod")
                excel.SetCellValue(1, 2, "Desc")

                eRow = 2
                eCol = 0

                ' Escribo todos los codigos de justificación que me hayan salido
                Dim sorted = From pair In dCausesDictionary Order By pair.Key
                Dim sortedDictionary = sorted.ToDictionary(Function(p) p.Key, Function(p) p.Value)

                For Each eDic As KeyValuePair(Of Integer, String) In sortedDictionary
                    excel.SetCellValue(eRow, 1, eDic.Key)
                    excel.SetCellValue(eRow, 2, eDic.Value)
                    eRow += 1
                Next

                ' Autoajusta columnas

                excel.AutoFitColumn(1, excel.GetWorksheetStatistics.EndColumnIndex)

                ' Vuelvo a hoja 1
                excel.SelectWorksheet("VTReg")

                ' Graba el archivo
                excel.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)


                ' Libera memoria
                dtDailyCauses.Dispose()

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportDailyCauses.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::LIVEN_DAilyCauses")
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportDailyCauses.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDataLinkExport::LIVEN_DAilyCauses:Exception:Message " & ex.Message)
            Finally
                ' Graba log
                SaveExportLog(nIDExport, strlogevent & msgLog & vbNewLine, idSchedule)
            End Try

            Return arrFile

        End Function
    End Class

End Namespace