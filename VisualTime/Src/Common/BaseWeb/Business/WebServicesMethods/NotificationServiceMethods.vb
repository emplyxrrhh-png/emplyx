Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotifications.Notifications

Namespace API

    Public NotInheritable Class NotificationServiceMethods

#Region "Notifications"

        Public Shared Function GetNotifications(ByVal oPage As System.Web.UI.Page, Optional ByVal _SQLFilter As String = "", Optional ByVal bIncludeSystem As Boolean = False, Optional ByVal bAudit As Boolean = True) As Generic.List(Of roNotification)

            Dim oRet As Generic.List(Of roNotification) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roNotification)) = VTLiveApi.NotificationMethods.GetNotifications(_SQLFilter, bIncludeSystem, oState, bAudit)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-421")
            End Try

            Return oRet

        End Function

        Public Shared Function GetNotificationList(ByVal oPage As System.Web.UI.Page, Optional ByVal SQLFilter As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetNotificationList(SQLFilter, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-422")
            End Try

            Return oRet

        End Function

        Public Shared Function GetNotificationsTypes(ByVal oPage As System.Web.UI.Page, Optional ByVal bolIncludeSystem As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetNotificationsTypes(oState, bolIncludeSystem)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-423")
            End Try

            Return oRet

        End Function

        Public Shared Function GetNotificationByID(ByVal oPage As System.Web.UI.Page, ByVal intIDNotification As Integer, ByVal bAudit As Boolean) As roNotification

            Dim oRet As roNotification = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roNotification) = VTLiveApi.NotificationMethods.GetNotificationByID(intIDNotification, oState, bAudit)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-424")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la notification
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oNotification"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveNotification(ByVal oPage As System.Web.UI.Page, ByRef oNotification As roNotification, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roNotification) = VTLiveApi.NotificationMethods.SaveNotification(oNotification, oState, bAudit)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oNotification = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-425")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina la notificacion
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID de la notificacion a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteNotification(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.NotificationMethods.DeleteNotification(ID, oState, bAudit)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-426")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprueba si los datos son válidos
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oNotification">notificación a validar</param>
        ''' <returns>Devuelve TRUE si los datos són correctos</returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateNotification(ByVal oPage As System.Web.UI.Page, ByVal oNotification As roNotification) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.NotificationMethods.ValidateNotification(oNotification, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-427")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetPermissionOverNotifications(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetPermissionOverNotifications(IDPassport, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-428")
            End Try

            Return oRet
        End Function

        Public Shared Function GetNotificationsSupervisor(ByVal oPage As System.Web.UI.Page, ByVal IDPassport As Integer, ByVal IDEmployee As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetNotificationsSupervisors(IDPassport, IDEmployee, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-429")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDesktopAlerts(dLastStatusChange As DateTime, ByVal oPage As System.Web.UI.Page) As DataSet

            Dim oRet As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetDesktopAlerts(dLastStatusChange, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If wsRet IsNot Nothing AndAlso wsRet.Value IsNot Nothing Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-431")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDesktopAlerts(iType As Integer, ByVal oPage As System.Web.UI.Page) As DataSet

            Dim oRet As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.NotificationMethods.GetDesktopAlertsDetails(iType, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result = NotificationResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-432")
            End Try

            Return oRet
        End Function

#End Region

#Region "Custom languages"

        Public Shared Function LoadCustomizableNotifications(ByVal oPage As System.Web.UI.Page) As roNotificationType()

            Dim oRet As roNotificationType() = {}

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roNotificationType()) = VTLiveApi.NotificationMethods.LoadCustomizableNotifications(oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-432")
            End Try

            Return oRet
        End Function

        Public Shared Function LoadNotificationLanguage(ByVal oPage As System.Web.UI.Page, ByVal notificationType As eNotificationType, ByVal strLanguageKey As String) As roNotificationLanguage

            Dim oRet As roNotificationLanguage = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roNotificationLanguage) = VTLiveApi.NotificationMethods.LoadNotificationLanguage(notificationType, strLanguageKey, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-432")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveNotificationLanguage(ByVal oPage As System.Web.UI.Page, ByVal oNotificationLanguage As roNotificationLanguage) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.NotificationState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.NotificationMethods.SaveNotificationLanguage(oNotificationLanguage, oState)

                oSession.States.NotificationState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.NotificationState.Result <> NotificationResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.NotificationState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-432")
            End Try

            Return bolRet
        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.NotificationState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace