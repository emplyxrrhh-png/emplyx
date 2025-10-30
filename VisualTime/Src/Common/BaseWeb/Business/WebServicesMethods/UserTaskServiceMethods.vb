Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.UserTask

Namespace API

    Public Class UserTaskServiceMethods

        Public Shared Function GetUserTasks(ByVal oPage As System.Web.UI.Page, ByVal _TaskType As TaskType, ByVal _TaskCompletedState As TaskCompletedState) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UsertaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.UserTaskMethods.GetUserTasks(_TaskType, _TaskCompletedState, oState)

                oSession.States.UsertaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UsertaskState.Result = UserTaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.UsertaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-808")
            End Try

            Return oRet

        End Function

        Public Shared Function GetUserTask(ByVal oPage As System.Web.UI.Page, ByVal _ID As String, ByVal bAudit As Boolean) As roUserTask

            Dim oRet As roUserTask = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UsertaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roUserTask) = VTLiveApi.UserTaskMethods.GetUserTask(_ID, oState, bAudit)

                oSession.States.UsertaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UsertaskState.Result <> UserTaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UsertaskState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-008")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveUserTask(ByVal oPage As System.Web.UI.Page, ByVal oUserTask As roUserTask, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As New roGenericVtResponse(Of Boolean)
            bolRet.Value = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UsertaskState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.UserTaskMethods.SaveUserTask(oUserTask, oState, bAudit)

                oSession.States.UsertaskState = bolRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UsertaskState.Result <> UserTaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UsertaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-810")
            End Try

            Return bolRet.Value

        End Function

        Public Shared Function DeleteUserTask(ByVal oPage As System.Web.UI.Page, ByVal oUserTask As roUserTask, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As New roGenericVtResponse(Of Boolean)
            bolRet.Value = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UsertaskState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.UserTaskMethods.DeleteUserTask(oUserTask, oState, bAudit)

                oSession.States.UsertaskState = bolRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UsertaskState.Result <> UserTaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UsertaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-811")
            End Try

            Return bolRet.Value

        End Function

        Public Shared Function DeleteUserTaskById(ByVal oPage As System.Web.UI.Page, ByVal _ID As String, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As New roGenericVtResponse(Of Boolean)
            bolRet.Value = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UsertaskState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.UserTaskMethods.DeleteUserTaskById(_ID, oState, bAudit)

                oSession.States.UsertaskState = bolRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UsertaskState.Result <> UserTaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UsertaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-812")
            End Try

            Return bolRet.Value

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.UsertaskState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace