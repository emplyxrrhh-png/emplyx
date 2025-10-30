Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class AccessMoveServiceMethods

        '*** VISTAS DE ANALYTICS DE TAREAS
        Public Shared Function GetAccessPlatesViews(ByVal oPage As System.Web.UI.Page, ByVal IdPassport As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessMoveState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessMoveMethods.GetAccessPlatesViewsDataSet(IdPassport, oState)

                oSession.States.AccessMoveState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AccessMoveState.Result = AccessMoveResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AccessMoveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-023")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessPlatesViewbyID(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal IdPassport As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessMoveState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AccessMoveMethods.GetAccessPlatesViewbyID(ID, IdPassport, oState)

                oSession.States.AccessMoveState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oState.Result = AccessMoveResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AccessMoveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-024")
            End Try

            Return oRet

        End Function

        Public Shared Function NewAccessPlatesView(ByVal oPage As System.Web.UI.Page, ByVal IdView As Integer, ByVal IdPassport As Integer, ByVal NameView As String,
                                                    ByVal Description As String, ByVal DateView As DateTime, ByVal Employees As String,
                                                    ByVal DateInf As DateTime, ByVal DateSup As DateTime, ByVal CubeLayout As String, ByVal FilterData As String) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessMoveState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessMoveMethods.NewAccessPlatesView(IdView, IdPassport, NameView, Description, DateView, Employees, DateInf, DateSup, CubeLayout, FilterData, oState)

                oSession.States.AccessMoveState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.AccessMoveState.Result <> AccessMoveResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessMoveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-025")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteAccessPlatesView(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AccessMoveState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AccessMoveMethods.DeleteAccessPlatesView(ID, oState)

                oSession.States.AccessMoveState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.AccessMoveState.Result <> AccessMoveResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AccessMoveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-026")
            End Try

            Return oRet

        End Function

        '*** FIN VISTAS DE ANALYTICS DE TAREAS

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AccessMoveState.ErrorText
            End If

            Return strRet
        End Function

    End Class

End Namespace