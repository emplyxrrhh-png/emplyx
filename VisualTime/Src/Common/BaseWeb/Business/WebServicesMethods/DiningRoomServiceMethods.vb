Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.DiningRoom

Namespace API

    Public NotInheritable Class DiningRoomServiceMethods

        Public Shared Function GetDiningRoomTurns(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DinningRoomState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.DiningRoomMethods.GetDiningRoomTurnsByDiningRoom(1, oState)

                oSession.States.DinningRoomState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DinningRoomState.Result = DiningRoomResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DinningRoomState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-177")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDiningRoomTurnByID(ByVal oPage As System.Web.UI.Page, ByVal intIDTurn As Integer, ByVal bAudit As Boolean) As roDiningRoomTurn

            Dim oRet As roDiningRoomTurn = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DinningRoomState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roDiningRoomTurn) = VTLiveApi.DiningRoomMethods.GetDiningRoomTurn(intIDTurn, oState, bAudit)

                oRet = wsRet.Value
                oSession.States.DinningRoomState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DinningRoomState.Result <> DiningRoomResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DinningRoomState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-178")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveDiningRoomTurn(ByVal oPage As System.Web.UI.Page, ByRef oDiningRoomTurn As roDiningRoomTurn, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DinningRoomState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roDiningRoomTurn) = VTLiveApi.DiningRoomMethods.SaveDiningRoomTurn(oDiningRoomTurn, oState, bAudit)

                oSession.States.DinningRoomState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DinningRoomState.Result <> DiningRoomResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DinningRoomState)

                End If

                If wsRet.Value IsNot Nothing Then
                    oRet = True
                    oDiningRoomTurn = wsRet.Value
                Else
                    oRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-179")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteDiningRoomTurn(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DinningRoomState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DiningRoomMethods.DeleteDiningRoomTurn(ID, oState, bAudit)

                bolRet = wsRet.Value
                oSession.States.DinningRoomState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DinningRoomState.Result <> DiningRoomResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DinningRoomState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-180")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExitsDiningRoomTurn(ByVal oPage As System.Web.UI.Page, ByVal IDDiningRoomTurn As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DinningRoomState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DiningRoomMethods.ExitsDiningRoomTurn(IDDiningRoomTurn, oState)

                oSession.States.DinningRoomState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DinningRoomState.Result <> DiningRoomResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DinningRoomState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-181")
            End Try

            Return bolRet

        End Function

#Region "Funciones State"

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DinningRoomState.ErrorText
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace