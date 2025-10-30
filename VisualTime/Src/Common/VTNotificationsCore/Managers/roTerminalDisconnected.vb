Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class roNotificationTask_Terminal_Disconnected_Manager
    Inherits roNotificationTaskManager

    Public Sub New(ByVal sGUID As String)
        MyBase.New(eNotificationType.Terminal_Disconnected, sGUID)
    End Sub

    Protected Overrides Function GetIdEmployee() As Integer
        Return -1
    End Function

    Protected Overrides Function GetIdPassport() As Integer
        Return -1
    End Function

    Protected Overrides Function MustSendPushNotification() As Boolean
        Return False
    End Function

    Protected Overrides Function GetNotificationAvailableDestinations() As roNotificationDestinationConfig()
        Return Me.GetDefaultDestinationConfig(True, False, False)
    End Function

    Protected Overrides Function GetTranslatedNotificationTexts(ByVal oConf As roNotificationDestinationConfig) As roNotificationItem
        Dim oItem As New roNotificationItem
        Try
            Dim Params As New ArrayList From {
                roTypes.Any2String(AccessHelper.ExecuteScalar("@SELECT# Description FROM Terminals WHERE ID = " & _oNotificationTask.Key1Numeric)),
                Format$(_oNotificationTask.key3Datetime, "HH:mm")
            }

            oItem.Type = NotificationItemType.email
            oItem.CompanyId = RoAzureSupport.GetCompanyName()

            If _oNotificationTask.Parameters = "NotConnected" Then
                oItem.Subject = roNotificationHelper.Message("Notification.TerminalDisconnected.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.TerminalDisconnected.Body", Params, , oConf.Language)
            Else
                oItem.Subject = roNotificationHelper.Message("Notification.TerminalConnected.Subject", Nothing, , oConf.Language)
                oItem.Body = roNotificationHelper.Message("Notification.TerminalConnected.Body", Params, , oConf.Language)
            End If

            oItem.Content = String.Empty
            oItem.Destination = oConf.Email
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTerminalDisconnected::Translate::", ex)
            oItem = Nothing
        End Try
        Return oItem
    End Function

    Protected Overrides Function PostSendAction() As Boolean

        Return True
    End Function

    Public Overrides Function GenerateNotificationTasks(ByVal ads As DataRow) As Boolean
        Dim bRet As Boolean = True
        Try
            ' Obtenemos las notificaciones de terminales
            Dim strSQL As String = "@SELECT# * FROM Notifications  WHERE Activated=1 AND IDType = 11 ORDER BY IDType"

            Dim terminalTypesIDs As String = ""
            Dim arr() As String

            Dim dt As DataTable = AccessHelper.CreateDataTable(strSQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Para cada tipo de notificacion , verificamos todas las tareas de ese tipo
                For Each dr As DataRow In dt.Rows
                    terminalTypesIDs = terminalTypesIDs & roTypes.Any2String(dr("ID")) & ","
                Next
                terminalTypesIDs = terminalTypesIDs.Substring(0, terminalTypesIDs.Length - 1)
            Else
                terminalTypesIDs = String.Empty
            End If

            If terminalTypesIDs <> "" Then
                arr = Split(terminalTypesIDs, ",")

                ' Terminales que gestionan las desconexiones con contadores de desconexion
                ExecuteNotificationsTerminalsDisconenctedLegacy(arr)
                ' Terminales que gestionan las desconexiones con fecha y hora de última acción
                ExecuteNotificationsTerminalsDisconenctedBasedOnLastAction(arr)

            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTerminalDisconnected::GenerateNotificationTasks:: Unexpected error: ", ex)
            bRet = False
        End Try

        Return bRet
    End Function

    Private Sub ExecuteNotificationsTerminalsDisconenctedLegacy(ByRef ads() As String)
        '
        ' Desconexion de Terminales
        '

        Dim SQL As String
        Dim iMaxAllowedDisconnectedTime As Integer
        Dim mExcludedTerminals As String
        Dim sStatus As String
        Dim iIDTerminal As Integer
        Dim iDisconnectionCounter As Integer
        Dim sSQLAux As String
        Dim bResetDiconnectionCounter As Boolean
        Dim sLastStatusNotified As String
        Dim element As Object

        Try

            mExcludedTerminals = "('Virtual', 'LivePortal', 'NFC', 'mx9', 'mx8', 'rx1','rxFP','rxFL','rxFPTD')"

            ' Obtenemos todos los terminales f�sicos
            SQL = "@SELECT# * FROM Terminals WHERE Type not in " & mExcludedTerminals & " AND LastUpdate IS NOT NULL  Order By ID "

            iMaxAllowedDisconnectedTime = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VisualTime.Procees.Notifier.TerminalDisconnectedTime"))

            If iMaxAllowedDisconnectedTime = 0 Or iMaxAllowedDisconnectedTime > 14400 Then
                ' Por defecto 10 minutos
                iMaxAllowedDisconnectedTime = 10
            End If

            Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Verificamos el estado de cada terminal
                For Each dr As DataRow In dt.Rows
                    sStatus = roTypes.Any2String(dr("LastStatus"))
                    iIDTerminal = roTypes.Any2Long(dr("ID"))
                    iDisconnectionCounter = roTypes.Any2Long(dr("DisconnectionCounter"))
                    sLastStatusNotified = roTypes.Any2String(dr("LastStatusNotified"))
                    bResetDiconnectionCounter = False

                    'Si el terminal no esta conectado augmentamos el contador de desconexiones
                    If sStatus <> "Ok" Then
                        ' Solo incrementamos el contador de desconexiones si no ha llegado al limite
                        ' Esto significa que la siguiente vez que cambie de estado mandara la notificaci�n
                        If iDisconnectionCounter < iMaxAllowedDisconnectedTime Then
                            ' Incremento el contador de n�mero de minutos desconectado
                            sSQLAux = "@UPDATE# Terminals SET DisconnectionCounter = " & (iDisconnectionCounter + 1) & " where id = " & roTypes.Any2String(iIDTerminal)
                            If Not AccessHelper.ExecuteSql(sSQLAux) Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Unexpected error incrementing DisconnectionCounter for terminal " & roTypes.Any2String(iIDTerminal))
                            End If
                        End If
                    Else
                        'Ponemos el contador al m�ximo para que notifique la primera conexi�n
                        iDisconnectionCounter = iMaxAllowedDisconnectedTime
                        'Primero borramos las alertas de terminal desconectado de live
                        sSQLAux = "@DELETE# FROM sysroUserTasks where ID = 'USERTASK:\\TERMINALDISCONNECTED" & roTypes.Any2String(iIDTerminal) & "'"
                        If Not AccessHelper.ExecuteSql(sSQLAux) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Unexpected error reseting alerts for terminal " & roTypes.Any2String(iIDTerminal))
                        End If
                        'Si el terminal esta conectado reseteamos el contador de desconexiones
                        sSQLAux = "@UPDATE# Terminals SET DisconnectionCounter = 0 where id = " & roTypes.Any2String(iIDTerminal)
                        If Not AccessHelper.ExecuteSql(sSQLAux) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Unexpected error reseting DisconnectionCounter for terminal " & roTypes.Any2String(iIDTerminal))
                        End If
                    End If

                    ' Terminal desconectado. Miro si lleva m�s de revisiones de las permitidas en estado desconectado
                    If iDisconnectionCounter >= iMaxAllowedDisconnectedTime Then
                        ' Notifico si no lo hice ya ...
                        If sStatus <> sLastStatusNotified Or sLastStatusNotified = "" Then

                            For Each element In ads
                                If element <> "" AndAlso roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# IDNotification FROM sysroNotificationtasks WHERE IDNotification=" & element & " AND Key1Numeric=" & dr("ID") & " AND  Key3DateTime=" & roTypes.Any2Time(dr("LastUpdate")).SQLDateTime)) = 0 Then
                                    SQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Parameters) VALUES (" &
                                          element & "," & dr("ID") & "," & roTypes.Any2Time(roTypes.Any2String(DateAdd("n", -1 * iMaxAllowedDisconnectedTime, Now))).SQLDateTime & ",'" & sStatus & "')"
                                    If Not AccessHelper.ExecuteSql(SQL) Then
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Insert Statement failed.")
                                    End If

                                    ' Reseteo contador
                                    bResetDiconnectionCounter = True
                                End If
                            Next

                            'Guardamos el ultimo estado que notificamos para no repetir la notificaci�n
                            sSQLAux = "@UPDATE# Terminals SET LastStatusNotified = '" & sStatus & "' where id = " & roTypes.Any2String(iIDTerminal)
                            If Not AccessHelper.ExecuteSql(sSQLAux) Then
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Unexpected error incrementing DisconnectionCounter for terminal " & roTypes.Any2String(iIDTerminal))
                            End If

                        End If

                    End If

                    ' Si es necesario reseteo el contador
                    If bResetDiconnectionCounter Then
                        sSQLAux = "@UPDATE# Terminals SET DisconnectionCounter = 0 where id = " & roTypes.Any2String(iIDTerminal)
                        If Not AccessHelper.ExecuteSql(sSQLAux) Then
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationCreateManager::ExecuteNotificationsTerminalsDisconenctedEx: Unexpected error reseting DisconnectionCounter for terminal " & roTypes.Any2String(iIDTerminal))
                        End If
                        bResetDiconnectionCounter = False
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTerminalDisconnected::ExecuteNotificationsTerminalsDisconectedLegacy:: Unexpected error: ", ex)
        Finally

        End Try

    End Sub

    Private Sub ExecuteNotificationsTerminalsDisconenctedBasedOnLastAction(ByRef ads() As String)
        '
        ' Desconexion de Terminales
        '

        Dim iMaxAllowedDisconnectedTime As Integer
        Dim mSupportedTerminals As String

        Dim iTerminalID As Integer
        Dim dTerminalLastAction As Date
        Dim sTerminalCurrentStatus As String
        Dim sTerminalLastStatusNotified As String

        Dim sSQL As String
        Dim oNotification As Object

        Try
            mSupportedTerminals = "('mx9','mx8','rx1','rxFP','rxFL','rxFPTD', 'rxTe', 'mxS', 'rxFe', 'Time Gate')"

            ' Obtenemos todos los terminales físicos compatibles con actualización constante de LastAction
            sSQL = "@SELECT# * FROM Terminals WHERE Type in " & mSupportedTerminals & " ORDER BY ID "

            ' Tiempo que deber estar desconectado un terminal para que se notifique su desconexión
            iMaxAllowedDisconnectedTime = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "VisualTime.Procees.Notifier.TerminalDisconnectedTime"))
            If iMaxAllowedDisconnectedTime = 0 OrElse iMaxAllowedDisconnectedTime > 1440 Then
                ' Por defecto 10 minutos
                iMaxAllowedDisconnectedTime = 10
            End If

            Dim dt As DataTable = AccessHelper.CreateDataTable(sSQL)
            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ' Verificamos el estado de cada terminal
                For Each oTerminalRow As DataRow In dt.Rows
                    iTerminalID = roTypes.Any2Long(oTerminalRow("ID"))
                    sTerminalLastStatusNotified = roTypes.Any2String(oTerminalRow("LastStatusNotified"))
                    dTerminalLastAction = If(IsDBNull(oTerminalRow("LastAction")), Date.MinValue, roTypes.Any2DateTime(oTerminalRow("LastAction")))

                    sSQL = String.Empty

                    ' Si aún no tiene informada la fecha  hora de última acción salgo
                    If dTerminalLastAction <> Date.MinValue Then
                        ' Actualizo posibles cambios de estado en función de LastAction
                        If Now.Subtract(dTerminalLastAction).TotalMinutes >= iMaxAllowedDisconnectedTime Then
                            ' Marco estado como desconectado, porque los terminales no lo pueden hacer de manera autónoma
                            sTerminalCurrentStatus = "NotConnected"
                            sSQL = "@UPDATE# Terminals SET LastStatus = 'NotConnected' WHERE ID = " & roTypes.Any2String(iTerminalID)
                            AccessHelper.ExecuteSql(sSQL)
                        Else
                            'Si el terminal está conectado, borramos las alertas de terminal desconectado en pantalla (si la hubiera).
                            'El estado Ok de conexión lo marcan los propios procesos de comunicaciones
                            sTerminalCurrentStatus = "Ok"
                            sSQL = "@DELETE# FROM sysroUserTasks WHERE ID = 'USERTASK:\\TERMINALDISCONNECTED" & roTypes.Any2String(iTerminalID) & "'"
                            AccessHelper.ExecuteSql(sSQL)
                        End If

                        ' Notifico, sólo si cambió el estado de conexión respecto al último que se notificó
                        If (sTerminalCurrentStatus <> sTerminalLastStatusNotified) OrElse sTerminalLastStatusNotified = "" Then
                            For Each oNotification In ads
                                ' Para cada notificación configurada
                                If oNotification <> "" AndAlso roTypes.Any2Double(AccessHelper.ExecuteScalar("@SELECT# IDNotification FROM sysroNotificationtasks WHERE IDNotification=" & oNotification & " AND Key1Numeric=" & oTerminalRow("ID") & " AND CONVERT(varchar,Parameters) = '" & sTerminalCurrentStatus & "' AND  Key3DateTime=" & roTypes.Any2Time(dTerminalLastAction).SQLDateTime)) = 0 Then
                                    sSQL = "@INSERT# INTO sysroNotificationtasks (IDNotification, Key1Numeric, Key3DateTime, Parameters) VALUES (" & oNotification & "," & oTerminalRow("ID") & "," & roTypes.Any2Time(dTerminalLastAction).SQLDateTime & ",'" & sTerminalCurrentStatus & "')"
                                    AccessHelper.ExecuteSql(sSQL)
                                End If
                            Next

                            'Guardamos el ultimo estado que notificamos para no repetir la notificaci�n
                            sSQL = "@UPDATE# Terminals SET LastStatusNotified = '" & sTerminalCurrentStatus & "' where id = " & roTypes.Any2String(iTerminalID)
                            AccessHelper.ExecuteSql(sSQL)
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCore::roTerminalDisconnected::ExecuteNotificationsTerminalsDisconectedBasedOnLastAction::", ex)
        Finally

        End Try

    End Sub

End Class