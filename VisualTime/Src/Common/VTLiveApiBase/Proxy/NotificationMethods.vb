Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTNotifications.Notifications

Public Class NotificationMethods

    ''' <summary>
    ''' Obtiene la solicitud (Notifications) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de solicitud</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve la solicitud (roNotification)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetNotificationByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roNotification)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roNotification)
        oResult.Value = New roNotification(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve todas las notificaciones
    ''' </summary>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'NType = 1 And Reque...')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetNotifications(ByVal SQLFilter As String, ByVal bIncludeSystem As Boolean, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roNotification))

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roNotification))
        oResult.Value = roNotification.GetNotifications(SQLFilter, bState, bAudit, bIncludeSystem)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con el id y nombre de todas las notificaciones
    ''' </summary>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'NType = 1 And Reque...')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetNotificationList(ByVal SQLFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roNotification.GetNotificationList(SQLFilter, bState)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve todos los tipos de notificaciones
    ''' </summary>
    Public Shared Function GetNotificationsTypes(ByVal oState As roWsState, ByVal bolIncludeSystem As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roNotification.GetNotificationsTypes(bState, bolIncludeSystem)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Valida los datos de la notificación<br/>
    ''' Comprueba que:<br/>
    ''' </summary>
    ''' <param name="oNotification">La notificación a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function ValidateNotification(ByVal oNotification As roNotification, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oNotification.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos de la notificación. Si és nuevo, se actualiza el ID de la solicitud pasado.<br/>
    ''' Se comprueba ValidateNotification()<br/>
    ''' </summary>
    ''' <param name="oNotification">Notificación a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function SaveNotification(ByVal oNotification As roNotification, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roNotification)

        'cambio mi state genérico a un estado especifico
        oNotification.State = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, oNotification.State)
        oNotification.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roNotification)
        If oNotification.Save(bAudit) Then
            oResult.Value = oNotification
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oNotification.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina la notificación con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' </summary>
    ''' <param name="ID">ID de notificación a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function DeleteNotification(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oNotification As New roNotification(ID, bState, False)

        oResult.Value = oNotification.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los diferentes tipos de notificaciones y si el Passport tiene permiso para verlas en base a sus funcionalidades
    ''' </summary>
    ''' <param name="intIDPassport">ID del Passport.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDNotification, Available, Name </returns>
    ''' <remarks></remarks>
    Public Shared Function GetPermissionOverNotifications(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = Nothing
        tb = roNotification.GetPermissionOverNotifications(intIDPassport, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un dataset con los datos de los diferentes tipos de notificaciones y si el Passport tiene permiso para verlas en base a sus funcionalidades
    ''' </summary>
    ''' <param name="intIDPassport">ID del Passport.</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Tabla con las columnas: IDNotification, Available, Name </returns>
    ''' <remarks></remarks>
    Public Shared Function GetNotificationsSupervisors(ByVal intIDPassport As Integer, ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = Nothing
        tb = roNotification.GetNotificationsSupervisor(intIDPassport, intIDEmployee, bState)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDesktopAlerts(ByVal dLastStatusChange As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roNotification.GetDesktopAlerts(bState, dLastStatusChange)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDesktopAlertsDetails(ByVal iType As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = roNotification.GetDesktopAlertsDetails(iType, bState)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#Region "Custom languages"

    Public Shared Function LoadNotificationLanguage(ByVal notificationType As eNotificationType, ByVal strLanguageKey As String, ByVal oState As roWsState) As roGenericVtResponse(Of roNotificationLanguage)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        'cambio mi state genérico a un estado especifico
        Dim oManager As New roNotificationLanguageManager()

        Dim oResult As New roGenericVtResponse(Of roNotificationLanguage)
        oResult.Value = oManager.LoadNotificationLanguage(notificationType, strLanguageKey)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveNotificationLanguage(ByVal oNotificationMessage As roNotificationLanguage, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oManager As New roNotificationLanguageManager()
        oResult.Value = oManager.SaveNotificationLanguage(oNotificationMessage)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function LoadCustomizableNotifications(ByVal oState As roWsState) As roGenericVtResponse(Of roNotificationType())

        Dim bState = New roNotificationState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        'cambio mi state genérico a un estado especifico
        Dim oManager As New roNotificationLanguageManager()

        Dim oResult As New roGenericVtResponse(Of roNotificationType())
        oResult.Value = oManager.LoadCustomizableNotifications()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

#End Region

End Class