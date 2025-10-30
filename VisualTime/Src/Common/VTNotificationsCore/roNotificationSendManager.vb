Imports Robotics.Azure
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Notifications

    Public Class roNotificationSendManager

        Public Sub New()
        End Sub

        Public Sub ExecuteSendNotifications()

            Try

                Dim strSQL As String = String.Empty

                strSQL = "@SELECT# sysroNotificationTasks.*, Notifications.Destination, Notifications.Condition, Notifications.IDType, Notifications.CreatorID, " &
                         "isnull(AllowMail, 0) AS AllowMail, Notifications.Name AS NotificationName " &
                         "FROM sysroNotificationTasks with (nolock), Notifications with (nolock) " &
                         "WHERE sysroNotificationTasks.IDNotification = Notifications.ID " &
                         "AND Executed=0 AND (sysroNotificationTasks.InProgress = 0 or sysroNotificationTasks.InProgress is null)  " &
                         "ORDER BY sysroNotificationTasks.ID ASC"

                Dim gInstanceGUID As Guid = Guid.NewGuid()

                Dim tb As DataTable = CreateDataTable(strSQL)

                Dim oNotificationFactory As New roNotificationTaskFactory

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Try
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationSendManager::ExecuteSendNotifications:Checking tasks on database " & RoAzureSupport.GetCompanyName())
                        Dim bolAnotherThreadinProgress As Boolean = False
                        For Each oRow As DataRow In tb.Rows
                            bolAnotherThreadinProgress = False

                            'Validamos que otro proceso no haya cogido el registro ya para enviar el email
                            Dim idNotExecuted As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Id from sysroNotificationTasks WHERE ID=" & oRow("ID") & " AND Executed = 0 And (InProgress = 0 or InProgress is null)"))

                            If idNotExecuted = 0 Then 'Si el id obtenido es 0, la notificación ya la ha ejecutado otro thread.
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roNotificationSendManager::ExecuteSendNotifications::" & oRow("ID") & "::Already in progress")
                                Continue For
                            End If

                            ' Marcamos inicio de proceso y el GUID del proceso
                            ExecuteSql("@UPDATE# sysroNotificationTasks SET InProgress= 1 , GUID='" & gInstanceGUID.ToString & "' WHERE ID=" & oRow("ID"))

                            Dim oManager As roNotificationTaskManager = oNotificationFactory.GetNotificationTaskManager(gInstanceGUID.ToString(), Any2Integer(oRow("IDType")))
                            If oManager IsNot Nothing Then oManager.Send(oRow)

                            roNotificationHelper.CheckTerminalConnectedNotifications(oRow) 'Solo se encarga de mantener las notificaciones de pantalla
                        Next
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::Work :", ex)
                    End Try
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CNotificationServer::Work :", Ex)
            Finally

            End Try
        End Sub

    End Class

End Namespace