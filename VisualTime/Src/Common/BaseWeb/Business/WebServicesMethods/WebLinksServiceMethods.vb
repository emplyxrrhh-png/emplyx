Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class WebLinkServiceMethods

        Public Shared Function GetAllWebLinks(ByVal oPage As System.Web.UI.Page) As List(Of roWebLink)
            Dim oRet As List(Of roWebLink) = New List(Of roWebLink)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WebLinksState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of List(Of roWebLink)) = VTLiveApi.WebLinkMethods.GetAllWebLinks(oState)

                oSession.States.WebLinksState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WebLinksState.Result = WebLinksResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.WebLinksState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-687")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateWebLink(ByVal webLink As roWebLink, ByVal oPage As System.Web.UI.Page) As Integer
            Dim oRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WebLinksState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.WebLinkMethods.CreateOrUpdateWebLink(webLink, oState)

                oSession.States.WebLinksState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WebLinksState.Result = WebLinksResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.WebLinksState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-687")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteWebLink(ByVal webLink As roWebLink, ByVal oPage As System.Web.UI.Page) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.WebLinksState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.WebLinkMethods.DeleteWebLink(webLink, oState)

                oSession.States.WebLinksState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.WebLinksState.Result = WebLinksResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.WebLinksState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-687")
            End Try

            Return oRet
        End Function
    End Class

End Namespace