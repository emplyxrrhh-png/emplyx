Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Capture

Namespace API

    Public NotInheritable Class CaptureServiceMethods

#Region "Captures"

        Public Shared Function GetCaptureByID(ByVal oPage As System.Web.UI.Page, ByVal intIDCapture As Integer) As roCapture

            Dim oRet As roCapture = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CaptureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roCapture) = VTLiveApi.CaptureMethods.GetCaptureByID(intIDCapture, oState)

                oSession.States.CaptureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CaptureState.Result <> CaptureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CaptureState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-001")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveCapture(ByVal oPage As System.Web.UI.Page, ByRef oCapture As roCapture) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CaptureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CaptureMethods.SaveCapture(oCapture, oState)

                oSession.States.CaptureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CaptureState.Result <> CaptureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CaptureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-002")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteCapture(ByVal oPage As System.Web.UI.Page, ByVal intIDCapture As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CaptureState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CaptureMethods.DeleteCaptureByID(intIDCapture, oState)

                oSession.States.CaptureState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CaptureState.Result <> CaptureResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CaptureState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-003")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CaptureState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace