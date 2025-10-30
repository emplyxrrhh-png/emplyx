Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Camera

Namespace API

    Public NotInheritable Class CameraServiceMethods

#Region "Camera"

        Public Shared Function GetCameras(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CameraState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CameraMethods.GetCamerasDataSet(oState)

                oSession.States.CameraState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CameraState.Result = CameraResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CameraState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-079")
            End Try

            Return oRet

        End Function

        Public Shared Function GetCameraByID(ByVal oPage As System.Web.UI.Page, ByVal intIDCamera As Integer, ByVal bAudit As Boolean) As roCamera

            Dim oRet As roCamera = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CameraState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roCamera) = VTLiveApi.CameraMethods.GetCameraByID(intIDCamera, oState, bAudit)

                oSession.States.CameraState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CameraState.Result <> CameraResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CameraState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-080")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda el convenio
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oCamera"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveCamera(ByVal oPage As System.Web.UI.Page, ByRef oCamera As roCamera, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CameraState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roCamera) = VTLiveApi.CameraMethods.SaveCamera(oCamera, oState, bAudit)

                oSession.States.CameraState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CameraState.Result <> CameraResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CameraState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oCamera = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-081")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina el convenio
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID del convenio a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteCamera(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CameraState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CameraMethods.DeleteCameraByID(ID, oState, bAudit)

                oSession.States.CameraState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CameraState.Result <> CameraResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CameraState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-082")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExitsCamera(ByVal oPage As System.Web.UI.Page, ByVal IDCamera As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CameraState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.CameraMethods.ExitsCamera(IDCamera, oState)

                oSession.States.CameraState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CameraState.Result <> CameraResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.CameraState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-083")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CameraState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace