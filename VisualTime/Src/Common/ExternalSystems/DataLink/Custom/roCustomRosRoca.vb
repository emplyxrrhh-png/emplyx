Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Newtonsoft.Json
Imports Robotics.DataLayer
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports SwiftExcel
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Security.Base
Imports Robotics.Base.VTBusiness.DiningRoom
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Azure
Imports System.Activities.Expressions
Imports Robotics.Base.VTSelectorManager
Imports System.Text

Namespace DataLink


    Public Class roCustomRosRoca
        Inherits roDataLinkExport

        '"ROS ROCA - Exportacion a Dynamics (ID 10099)"
        Public Shared Function EXCELExportDynamics(ByVal nIDExport As Integer, ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, Optional idSchedule As Integer = -1) As Byte()

            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try

                Dim NameFile As String

                ' Si es automática, exporto los tres últimos meses completos desde hoy
                NameFile = "ExcelExportDynamics#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim eRow As Integer = 2

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("ExportTasks.LogEvent.Start", "") & vbNewLine

                Dim strSQL As String = "@SELECT# DailyTaskAccruals.IDEmployee, DailyTaskAccruals.IDTask, ISNULL(SUM(DailyTaskAccruals.Value), 0) as Total, ISNULL(SUM(DailyTaskAccruals.Field4), 0) as Cantidad , DailyTaskAccruals.Date  " &
                         "FROM DailyTaskAccruals INNER JOIN Tasks " &
                                    "ON DailyTaskAccruals.IDTask = Tasks.ID " &
                               " WHERE DailyTaskAccruals.Date >= " & Any2Time(dtBeginDate).SQLSmallDateTime & " AND DailyTaskAccruals.Date <= " & Any2Time(dtEndDate).SQLSmallDateTime
                strSQL &= " AND DailyTaskAccruals.IDTask > 0   "
                strSQL &= " GROUP By DailyTaskAccruals.IDEmployee, DailyTaskAccruals.Date, DailyTaskAccruals.IDTask   "
                strSQL &= " ORDER By DailyTaskAccruals.IDEmployee , DailyTaskAccruals.Date, DailyTaskAccruals.IDTask   "

                Dim dtDailyTaskAccruals As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                '' Crea la cabecera
                Dim arrColumnKeys() As String = {"Dia", "OF", "Tipo", "Tarea", "Operacion", "Recurso", "Empleado", "Tiempo", "Cantidad", "Completado"}
                Dim arrColumnDefaultNames() As String = {"Día", "Orden de Fabricación", "Tipo", "Tarea", "Operación", "Recurso", "Empleado", "Tiempo (h)", "Cantidad", "Completado"}
                Dim dHeader As New Dictionary(Of String, String)
                For i = 0 To arrColumnKeys.Count - 1
                    dHeader.Add(arrColumnKeys(i), arrColumnDefaultNames(i))
                Next

                DataLink_Export_CreateHeaders(excel, dHeader, oState)



                If dtDailyTaskAccruals IsNot Nothing Then
                    Dim OldDate As String = ""
                    Dim NewEmployee As String = ""
                    Dim OldEmployee As String = ""
                    Dim Operario As String
                    Dim IDJob As Double = 0
                    Dim Fecha As String = ""
                    Dim OrdenTrabajo As String = ""
                    Dim OrdenOperacion As String = ""
                    Dim FechaAux As String = ""
                    Dim Situacion As String = ""
                    Dim Tiempo As String = ""
                    Dim FechaTraspaso As String = ""
                    Dim Tarea As String = ""
                    Dim Horario As String = ""
                    Dim TipoHoras As String = ""
                    Dim IDTask As String = ""
                    Dim Recurso As String = ""
                    Dim Cantidad As Double = 0
                    For Each cRow As DataRow In dtDailyTaskAccruals.Rows
                        ' Para cada empleado, dia y tarea generamos una linea
                        NewEmployee = Any2String(cRow("IDEmployee"))
                        Operario = Any2String(ExecuteScalar("@SELECT# TOP(1) Value FROM EmployeeUserFieldValues WHERE FieldName = 'CODI_DYNAMICS' AND (IDEmployee = " & cRow("IDEmployee") & ") AND (Date <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") ORDER BY Date DESC"))

                        IDJob = Any2Double(cRow("IDTask"))

                        Fecha = Right$("        " & Replace(Format$(Any2Time(cRow("Date")).DateOnly, "dd/MM/yyyy"), "/", ""), 8) & "|"
                        OrdenTrabajo = Any2String(ExecuteScalar("@SELECT# Project from Tasks WHERE ID=" & IDJob.ToString))
                        OrdenOperacion = Any2String(ExecuteScalar("@SELECT# isnull(Tag,'') from Tasks WHERE ID=" & IDJob.ToString))
                        IDTask = Any2String(ExecuteScalar("@SELECT# isnull(Field2,'') from Tasks WHERE ID=" & IDJob.ToString))
                        Recurso = Any2String(ExecuteScalar("@SELECT# isnull(Field1,'') from Tasks WHERE ID=" & IDJob.ToString))

                        FechaAux = Format$(Any2Time(cRow("Date")).DateOnly, "dd/MM/yyyy")

                        Situacion = Any2String(ExecuteScalar("@SELECT# EndDate FROM Tasks WHERE ID = " & IDJob))

                        Cantidad = 0
                        If Len(Situacion) > 0 Then
                            Situacion = Format$(Any2Time(Situacion).DateOnly, "dd/MM/yyyy")
                            If CDate(FechaAux) >= CDate(Situacion) Then
                                Situacion = "YES"
                                Cantidad = 1
                            Else
                                Situacion = "NO"
                            End If
                        Else
                            Situacion = "NO"
                        End If

                        excel.SetCellValue(eRow, 1, Any2Time(cRow("Date")).DateOnly, "dd/MM/yyyy")
                        excel.SetCellValue(eRow, 2, OrdenTrabajo)
                        excel.SetCellValue(eRow, 3, "Process")
                        excel.SetCellValue(eRow, 4, IDTask)
                        excel.SetCellValue(eRow, 5, OrdenOperacion)
                        excel.SetCellValue(eRow, 6, Recurso)
                        excel.SetCellValue(eRow, 7, Operario)
                        excel.SetCellValue(eRow, 8, cRow("Total"), "#0.00")
                        excel.SetCellValue(eRow, 9, Cantidad, "#0.00")
                        excel.SetCellValue(eRow, 10, Situacion)
                        eRow += 1
                    Next
                End If

                excel.AutoFitColumn(1, excel.GetWorksheetStatistics.EndColumnIndex)

                ' Graba el archivo
                excel.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                ' Libera memoria
                dtDailyTaskAccruals.Dispose()

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportTasks.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine


            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportDynamics")
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportTasks.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIDExport, strlogevent & msgLog & vbNewLine, idSchedule)
            End Try

            Return arrFile

        End Function



    End Class
End Namespace
