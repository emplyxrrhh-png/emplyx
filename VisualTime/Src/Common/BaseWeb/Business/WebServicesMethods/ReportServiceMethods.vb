Imports Robotics.Base
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class ReportServiceMethods

#Region "Devextreme reports"

        Public Shared Function GetReportLayoutByName(ByVal oPage As System.Web.UI.Page, ByVal action As ReportPermissionTypes, ByVal reportName As String, ByVal bolAudit As Boolean) As Report

            Dim oRet As Report = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                ' oSession.AccessApi.WebServices.AccessGroupService.Timeout = System.Threading.Timeout.Infinite
                Dim wsRet As roGenericVtResponse(Of Report) = VTLiveApi.ReportMethods.GetReportByName(reportName, action, WLHelperWeb.CurrentPassport.ID, oState, bolAudit)

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function GetReportsSimplified(ByVal oPage As PageBase) As List(Of Report)
            Dim oRet As roGenericVtResponse(Of List(Of Report)) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                oRet = VTLiveApi.ReportMethods.GetReportsSimplified(WLHelperWeb.CurrentPassport.ID, oState)

                oSession.States.ReportState = oRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-493") 'CAMBIAR CÓDIGO
            End Try

            Return oRet.Value

        End Function

        Public Shared Function GetReportById(ByVal reportId As Integer, ByVal action As ReportPermissionTypes, ByVal oPage As PageBase) As Report
            Dim oRet As roGenericVtResponse(Of Report) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                oRet = VTLiveApi.ReportMethods.GetReportById(reportId, action, WLHelperWeb.CurrentPassport.ID, oState)

                oSession.States.ReportState = oRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-493") 'CAMBIAR CÓDIGO
            End Try

            Return oRet.Value

        End Function

        Public Shared Function GetExecutionStatus(ByVal executionId As Guid, ByVal oPage As PageBase) As ReportExecution
            Dim oRet As roGenericVtResponse(Of ReportExecution) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                oRet = VTLiveApi.ReportMethods.GetExecutionStatus(executionId, oState)

                oSession.States.ReportState = oRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-493") 'CAMBIAR CÓDIGO
            End Try

            Return oRet.Value

        End Function

        Public Shared Function GetReportsPageConfiguration(ByVal oPage As PageBase) As String
            Dim oRet As roGenericVtResponse(Of String) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                oRet = VTLiveApi.ReportMethods.GetReportsPageConfiguration(WLHelperWeb.CurrentPassport.ID, oState)

                oSession.States.ReportState = oRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-493") 'CAMBIAR CÓDIGO
            End Try

            Return oRet.Value

        End Function

        Public Shared Function GetExecutionAssocietedExportFile(ByVal reportId As Guid, ByVal oPage As PageBase) As (Byte(), String)
            Dim oRet As roGenericVtResponse(Of (Byte(), String)) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                oRet = VTLiveApi.ReportMethods.GetExecutionAssociatedExportDataAndExtension(reportId, WLHelperWeb.CurrentPassport.ID, oState)

                oSession.States.ReportState = oRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-493") 'CAMBIAR CÓDIGO
            End Try

            Return oRet.Value

        End Function

        Public Shared Function SaveReportLastParameters(lastParameters As String, reportId As Integer, passportId As Integer, ByVal oPage As System.Web.UI.Page, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.SaveReportLastParameters(lastParameters, reportId, passportId, oState, bolAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function InsertReport(ByVal oPage As System.Web.UI.Page, ByVal reportLayout As Report, ByVal action As ReportPermissionTypes, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                ' oSession.AccessApi.WebServices.AccessGroupService.Timeout = System.Threading.Timeout.Infinite

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.InsertReport(reportLayout, action, oState, bolAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateReport(ByVal oPage As System.Web.UI.Page, ByVal reportLayout As Report, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.UpdateReport(reportLayout, oState, bolAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateReportExecutions(reportExecutionsList As List(Of ReportExecution), reportId As Integer, ByVal oPage As System.Web.UI.Page, ByVal report As Report, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.UpdateReportExecutions(reportExecutionsList, reportId, WLHelperWeb.CurrentPassport.ID, oState, bolAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateReportPlannedExecutions(reportPlannedExecutionsList As List(Of ReportPlannedExecution), parametersJson As String, reportId As Integer, ByVal oPage As System.Web.UI.Page, ByVal report As Report, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.UpdateReportPlannedExecutions(reportPlannedExecutionsList, parametersJson, reportId, WLHelperWeb.CurrentPassport.ID, oState, bolAudit)

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function DeleteReport(ByVal oPage As System.Web.UI.Page, ByVal reportId As Integer, ByVal bolAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try

                ' oSession.AccessApi.WebServices.AccessGroupService.Timeout = System.Threading.Timeout.Infinite

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.DeleteReport(reportId, WLHelperWeb.CurrentPassport.ID, oState, bolAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function CopyReport(ByVal reportId As Integer, ByVal newReportName As String, ByVal passportId As Integer, ByVal oPage As System.Web.UI.Page, bAudit As Boolean) As Boolean

            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.CopyReport(reportId, newReportName, passportId, oState, bAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateReportCategories(ByVal reportId As Integer, ByVal reportCategories As List(Of (Integer, Integer)), ByVal passportId As Integer, ByVal oPage As System.Web.UI.Page, bAudit As Boolean) As Boolean
            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.UpdateReportCategories(reportId, reportCategories, passportId, oState, bAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet
        End Function

        Public Shared Function GetEmergencyReport(ByVal oPage As System.Web.UI.Page, bAudit As Boolean) As Report
            Dim oRet As Report
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Report) = VTLiveApi.ReportMethods.GetEmergencyReport(WLHelperWeb.CurrentPassportID, oState, bAudit)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet
        End Function

        Public Shared Function ExecuteEmergencyReport(ByVal oPage As System.Web.UI.Page, bAudit As Boolean, Optional strReports As String = Nothing) As Boolean
            Dim oRet As Boolean
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ReportState

            WebServiceHelper.SetState(oState)

            Dim passport = WLHelperWeb.CurrentPassportID

            If passport = 0 Then
                Dim state As New VTBusiness.Punch.roPunchState
                passport = state.IDPassport
            End If

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ReportMethods.ExecuteEmergencyReport(passport, oState, bAudit, strReports)
                oRet = wsRet.Value

                oSession.States.ReportState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ReportState.Result <> ReportResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ReportState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                oRet = ExceptionHandler(oPage)
            End Try

            Return oRet
        End Function

        Private Shared Function ExceptionHandler(oPage As System.Web.UI.Page) As Object
            Dim oTmpState As New Robotics.Base.DTOs.roWsState
            Dim oLanguage As New roLanguageWeb
            oTmpState.Result = 1
            oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
            HelperWeb.ShowError(oPage, oTmpState, "9-BW01-004")

            Return Nothing
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ReportState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastDevextremeErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ReportState.ErrorText
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace