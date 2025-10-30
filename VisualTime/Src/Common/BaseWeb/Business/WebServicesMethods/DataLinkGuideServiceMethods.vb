Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class DataLinkGuideServiceMethods

        Public Shared Function GetDatalinkGuides(oPage As Page, bAudit As Boolean) As List(Of roDatalinkGuide)
            Dim oRet As roDatalinkGuideListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.GetDatalinkGuides(oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roDatalinkGuideListResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Guides.ToList
        End Function

        Public Shared Function GetDatalinkGuide(oPage As Page, ByVal eConcept As roDatalinkConcept, bAudit As Boolean) As roDatalinkGuide
            Dim oRet As roDatalinkGuideResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.GetDatalinkGuide(eConcept, oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roDatalinkGuideResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Guide
        End Function

        Public Shared Function SaveDatalinkGuide(oPage As Page, ByVal oGuide As roDatalinkGuide, bAudit As Boolean) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.SaveDatalinkGuide(oGuide, oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roStandarResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function GetExportTemplateBytes(oPage As Page, ByVal idExportGuide As Integer, ByVal idTemplate As Integer) As Byte()
            Dim oRet As roDatalinkTemplateBytesResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.GetExportTemplateBytes(idExportGuide, idTemplate, oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roDatalinkTemplateBytesResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Content
        End Function

        Public Shared Function DuplicateExportTemplateBytes(oPage As Page, ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer, ByVal newTemplateName As String) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.DuplicateExportTemplateBytes(bTemplateContent, idExportGuide, idTemplate, newTemplateName, oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roStandarResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function SaveExportTemplateBytes(oPage As Page, ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DatalinkGuideState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DatalinkMethods.SaveExportTemplateBytes(bTemplateContent, idExportGuide, idTemplate, oState)
                oSession.States.DatalinkGuideState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DatalinkGuideState.Result <> DataLinkGuideResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DatalinkGuideState)
                End If
            Catch ex As Exception
                oRet = New roStandarResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DatalinkGuideState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace