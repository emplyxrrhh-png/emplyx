Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class SecurityV2ServiceMethods

        Public Shared Function GetCurrentLoggedUsers(ByVal oPage As System.Web.UI.Page) As UserList
            Dim oRet As UserList = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV2State

            WebServiceHelper.SetState(oState)

            Try
                Dim oTmpObject As UserList = VTLiveApi.SecurityMethods.GetCurrentLoggedUsers(oState)
                oRet = oTmpObject

                oSession.States.SecurityV2State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV2State.Result <> SecurityResultEnum.NoError Then
                    oRet = Nothing
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV2State)
                End If
            Catch ex As Exception
                oRet = Nothing
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-840")
            End Try

            Return oRet
        End Function

        Public Shared Function GetConcurrencyInfo(ByVal oPage As System.Web.UI.Page, ByVal bAudit As Boolean) As ConcurrencyInfoList

            Dim oRet As ConcurrencyInfoList

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SecurityV2State

            WebServiceHelper.SetState(oState)

            Try
                Dim oTmpObject As ConcurrencyInfoList = VTLiveApi.SecurityMethods.GetConcurrencyInfo(oState)
                oRet = oTmpObject

                oSession.States.SecurityV2State = oTmpObject.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SecurityV2State.Result <> SecurityResultEnum.NoError Then
                    oRet = Nothing
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SecurityV2State)
                End If
            Catch ex As Exception
                oRet = Nothing
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-840")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SecurityV2State.ErrorText
            End If

            Return strRet
        End Function

    End Class

End Namespace