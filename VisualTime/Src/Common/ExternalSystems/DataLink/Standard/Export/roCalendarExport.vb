Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase

Namespace DataLink
    Public Class roCalendarExport
        Inherits roDataLinkExport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Function ExportProfilePlanningV2EXCEL(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelproFileName As String, ByVal OutputFileIsExcel2003 As Boolean, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Try
                Dim ExcelType As ProfileExportBody.FileTypeExport = IIf(OutputFileIsExcel2003 = True, ProfileExportBody.FileTypeExport.typ_2003, ProfileExportBody.FileTypeExport.typ_2007)
                arrFile = ExportPlanningV2(intIDExport, EmployeesFilter, BeginDate, EndDate, oExcelproFileName, ExcelType, oExcelProfileBytes, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportProfilePlanningV2EXCEL")
            End Try
            Return arrFile
        End Function

        Private Function ExportPlanningV2(ByVal intIDExport As Integer, ByVal EmployeesFilter As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oExcelFileName As String, ByVal OutputFileType As ProfileExportBody.FileTypeExport, Optional oExcelProfileBytes As Byte() = Nothing, Optional idSchedule As Integer = -1) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim bContinue As Boolean

            Try
                strlogevent = ""

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & Me.State.Language.Translate("ExportPlanningV2.LogEvent.Start", "") & vbNewLine

                ' Define documento plantilla
                Dim oSettings As New roSettings()

                Dim ExcelProfile As ExcelExport = Nothing
                If oExcelProfileBytes IsNot Nothing Then
                    ExcelProfile = New ExcelExport(oExcelProfileBytes)
                    bContinue = True
                End If

                If bContinue Then
                    If ExcelProfile.FileIsOK Then
                        ' Cargamos parámetros personalizados si aplica (sólo en caso de exportaciones desde Enlace, y desde plantillas distintas a la "OFICIAL" (se crean directamente por Robotics y se copian en la carpeta DATALINK)
                        Dim sExportShiftsFilter As String = String.Empty
                        If Not oExcelFileName.StartsWith("CalendarLink", StringComparison.OrdinalIgnoreCase) Then
                            If ExcelProfile.GetCellValue(1, 1, ExcelProfile.GetIDSheet("params")).ToString.ToUpper = "HORARIOS" Then
                                sExportShiftsFilter = ExcelProfile.GetCellValue(1, 2, ExcelProfile.GetIDSheet("params")).ToString.Replace(" ", "").ToUpper
                            End If
                        End If

                        ' Exporta tareas
                        Dim oCalendarState As Base.VTCalendar.roCalendarState = New Base.VTCalendar.roCalendarState(Me.State.IDPassport)
                        Dim oCalendarManager As New Robotics.Base.VTCalendar.roCalendarManager(oCalendarState)
                        Dim oCalendar As New DTOs.roCalendar
                        oCalendar = oCalendarManager.Load(BeginDate, EndDate, EmployeesFilter, DTOs.CalendarView.Planification, DTOs.CalendarDetailLevel.Daily, True, True)
                        If oExcelFileName = "CalendarLinkCellCompanyLayout.xlsx" Then
                            arrFile = oCalendarManager.ExportCompanyToExcel(oCalendar, oExcelFileName,, sExportShiftsFilter, oExcelProfileBytes)
                        Else
                            arrFile = oCalendarManager.ExportToExcel(oCalendar, oExcelFileName,, sExportShiftsFilter, oExcelProfileBytes)
                        End If
                    Else
                        strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelIsNotOK", "") & vbNewLine
                    End If
                Else
                    strlogevent += Me.State.Language.Translate("ResultEnum.ProfileExcelShouldBeInCloud", "") & vbNewLine
                End If

                ' Exportación finalizada
                strlogevent = strlogevent & Now.ToString & " --> " & Me.State.Language.Translate("ExportPlanningV2.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

                ' Graba el resultado
                SaveExportLog(intIDExport, strlogevent, idSchedule)
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::ExportPlanningV2")

            End Try

            Return arrFile
        End Function

    End Class

End Namespace