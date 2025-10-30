Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTNotifications.Notifications

Public Class NotificationProxy
    Implements INotificationSvc

    Public Function KeepAlive() As Boolean Implements INotificationSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene la solicitud (Notifications) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de solicitud</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve la solicitud (roNotification)</returns>
    ''' <remarks></remarks>
    Public Function GetNotificationByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roNotification) Implements INotificationSvc.GetNotificationByID
        Return NotificationMethods.GetNotificationByID(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve todas las notificaciones
    ''' </summary>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'NType = 1 And Reque...')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNotifications(ByVal SQLFilter As String, ByVal bIncludeSystem As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roNotification)) Implements INotificationSvc.GetNotifications
        Return NotificationMethods.GetNotifications(SQLFilter, bIncludeSystem, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con el id y nombre de todas las notificaciones
    ''' </summary>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'NType = 1 And Reque...')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetNotificationList(ByVal SQLFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetNotificationList

        Return NotificationMethods.GetNotificationList(SQLFilter, oState)
    End Function

    ''' <summary>
    ''' Devuelve todos los tipos de notificaciones
    ''' </summary>
    Public Function GetNotificationsTypes(ByVal oState As roWsState, ByVal bolIncludeSystem As Boolean) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetNotificationsTypes
        Return NotificationMethods.GetNotificationsTypes(oState, bolIncludeSystem)
    End Function

    ''' <summary>
    ''' Valida los datos de la notificación<br/>
    ''' Comprueba que:<br/>
    ''' </summary>
    ''' <param name="oNotification">La notificación a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function ValidateNotification(ByVal oNotification As roNotification, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements INotificationSvc.ValidateNotification
        Return NotificationMethods.ValidateNotification(oNotification, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos de la notificación. Si és nuevo, se actualiza el ID de la solicitud pasado.<br/>
    ''' Se comprueba ValidateNotification()<br/>
    ''' </summary>
    ''' <param name="oNotification">Notificación a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function SaveNotification(ByVal oNotification As roNotification, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roNotification) Implements INotificationSvc.SaveNotification
        Return NotificationMethods.SaveNotification(oNotification, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina la notificación con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' </summary>
    ''' <param name="ID">ID de notificación a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteNotification(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements INotificationSvc.DeleteNotification
        Return NotificationMethods.DeleteNotification(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los diferentes tipos de notificaciones y si el Passport tiene permiso para verlas en base a sus funcionalidades
    ''' </summary>
    ''' <param name="intIDPassport">ID del Passport.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDNotification, Available, Name </returns>
    ''' <remarks></remarks>
    Public Function GetPermissionOverNotifications(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetPermissionOverNotifications
        Return NotificationMethods.GetPermissionOverNotifications(intIDPassport, oState)
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los diferentes tipos de notificaciones y si el Passport tiene permiso para verlas en base a sus funcionalidades
    ''' </summary>
    ''' <param name="intIDPassport">ID del Passport.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDNotification, Available, Name </returns>
    ''' <remarks></remarks>
    Public Function GetNotificationsSupervisors(ByVal intIDPassport As Integer, ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetNotificationsSupervisors
        Return NotificationMethods.GetNotificationsSupervisors(intIDPassport, intIDEmployee, oState)
    End Function


    Public Function GetSupervisedEmployeesByPassport(ByVal intIDPassport As Integer, ByVal featureAlias As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Integer)) Implements INotificationSvc.GetSupervisedEmployeesByPassport
        Return NotificationMethods.GetSupervisedEmployeesByPassport(intIDPassport, featureAlias, oState)
    End Function

    Public Function GetDesktopAlerts(ByVal dLastStatusChange As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetDesktopAlerts
        Return NotificationMethods.GetDesktopAlerts(dLastStatusChange, oState)
    End Function

    Public Function GetDesktopAlertsDetails(ByVal iType As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements INotificationSvc.GetDesktopAlertsDetails
        Return NotificationMethods.GetDesktopAlertsDetails(iType, oState)
    End Function




#Region "Custom languages"
    Public Function LoadNotificationLanguage(ByVal notificationType As eNotificationType, ByVal strLanguageKey As String, ByVal oState As roWsState) As roGenericVtResponse(Of roNotificationLanguage) Implements INotificationSvc.LoadNotificationLanguage
        Return NotificationMethods.LoadNotificationLanguage(notificationType, strLanguageKey, oState)
    End Function

    Public Function SaveNotificationLanguage(ByVal oNotificationMessage As roNotificationLanguage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements INotificationSvc.SaveNotificationLanguage
        Return NotificationMethods.SaveNotificationLanguage(oNotificationMessage, oState)
    End Function

    Public Function LoadCustomizableNotifications(ByVal oState As roWsState) As roGenericVtResponse(Of roNotificationType()) Implements INotificationSvc.LoadCustomizableNotifications
        Return NotificationMethods.LoadCustomizableNotifications(oState)
    End Function
#End Region

End Class
