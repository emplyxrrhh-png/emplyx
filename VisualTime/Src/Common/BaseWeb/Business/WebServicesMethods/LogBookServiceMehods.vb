Imports Robotics.Base.DTOs
Imports Robotics.Base.VTChannels

Namespace API

    Public NotInheritable Class LogBookServiceMehods

        Public Shared Function GetComplaintLog(ByVal complaintRef As String, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roMessage)
            Dim oRet As List(Of roMessage) = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roLogBookState = oSession.States.LogBookState

            WebServiceHelper.SetState(oState)

            Try
                Dim oLogBookManager As New roLogBookManager(oState)
                oRet = oLogBookManager.GetComplaintLog(complaintRef, bAudit)

                oSession.States.LogBookState = oLogBookManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LogBookState.Result = MessageResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.LogBookState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

    End Class

End Namespace