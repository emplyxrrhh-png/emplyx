Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class SDKServiceMethods

        Public Shared Function GetPunches(ByVal oPage As Page, ByVal ids As String, ByVal bAudit As Boolean) As roSDKPunchList
            Dim oRet As roSDKPunchList = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SDKState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.SDKMethods.GetPunchesWhere(" ID IN (" & ids & ")", bAudit, oState)
                oSession.States.SDKState = oRet.oState
                roWsUserManagement.SessionObject = oSession
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-569")
            End Try
            Return oRet
        End Function

        Public Shared Function ProcessPunches(ByVal oPage As Page, ByVal punches As List(Of roSDKPunch), ByVal DeletePunchesIDs As String, ByVal bAudit As Boolean) As roSDKGenericResponse
            Dim oRet As roSDKGenericResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SDKState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.SDKMethods.ProcessPunches(punches, DeletePunchesIDs, bAudit, oState)
                oSession.States.SDKState = oRet.oState
                roWsUserManagement.SessionObject = oSession
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-570")
            End Try
            Return oRet
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SDKState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As SDKResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SDKState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace