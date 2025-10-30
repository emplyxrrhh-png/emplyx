Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Notifications

    Public Class roNotificationCreateManager

        Public Sub New()
        End Sub

        Private Function GetSchedulers() As roSendNotifications
            '
            ' Obtenemos tipos de notificaciones y sus tiempos de verificacion
            '

            Dim SQL As String

            Dim oNotificationSendConfiguration As New roSendNotifications

            Dim now As String = roTypes.Any2Time(DateTime.Now).SQLSmallDateTime

            Try

                ' Obtenemos todas las notificaciones a verificar
                SQL = "@SELECT# * FROM sysroNotificationTypes WHERE Scheduler > 0 AND ((IsRunning = 0 AND NextExecution <= " & now & ") OR (IsRunning = 1 AND  DATEAdd(minute,15,NextExecution) < " & now & "))  ORDER BY ID"
                Dim dt As DataTable = AccessHelper.CreateDataTable(SQL)

                ' Marcamos las notificaciones que se van a procesar como IsRunning
                SQL = "@UPDATE# sysroNotificationTypes SET IsRunning = 1, NextExecution = " & now & " WHERE Scheduler > 0 AND ((IsRunning = 0 AND NextExecution <= " & now & ") OR (IsRunning = 1 AND  DATEAdd(minute,15,NextExecution) < " & now & "))"
                AccessHelper.ExecuteSql(SQL)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    ' Para cada tipo de notificacion nos guardamos la hora en la que hay que ejecutar la tarea
                    For Each dr As DataRow In dt.Rows
                        oNotificationSendConfiguration.Collection.Add(dr("ID"), New roSendNotificationItem With {
                            .NextExecution = Any2Time(dr("NextExecution")).ValueDateTime,
                            .Type = CType(dr("ID"), eNotificationType)
                        })
                    Next
                End If
            Catch ex As Exception
                oNotificationSendConfiguration = Nothing
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roNotificationCreateManager::GetSchedulers: Unexpected error: ", ex)
            End Try

            Return oNotificationSendConfiguration
        End Function

        Public Sub ExecuteCreateNotifications()
            '
            ' Ejecutamos todas las tareas de notificacion
            '

            Dim strSQL As String
            Dim IDNotifications As String
            Dim bNdrficationProcessed As Boolean
            Dim cNotificationType As eNotificationType = 0

            Try

                Dim oSchueduleNotifications As roSendNotifications = GetSchedulers()

                '' Obtenemos las notificaciones a verificar
                IDNotifications = "-1"
                For Each oKey As eNotificationType In oSchueduleNotifications.Collection.Keys
                    IDNotifications = IDNotifications & "," & CInt(oKey)
                Next

                If IDNotifications = "-1" Then
                    Exit Sub
                End If

                'Marcamos todas las tareas de generación de notificaciones que lleven más de una hora en ejecución para que se vuelvan a ejecutar
                AccessHelper.ExecuteSql("@UPDATE# Notifications SET InProgress=0, LastCheck = NULL, GUID='' WHERE LastCheck <= DATEADD(minute,-60,getdate()) ")

                ' Obtenemos todas las notificaciones a verificar
                strSQL = "@SELECT# * FROM Notifications WHERE Activated=1 AND IDType IN(" & IDNotifications & ") AND IDType <> 11 AND (InProgress = 0 or InProgress is null)  ORDER BY IDType"

                Dim dt As DataTable = AccessHelper.CreateDataTable(strSQL)

                Dim gInstanceGUID As Guid = Guid.NewGuid()

                Dim oNotificationFactory As New roNotificationTaskFactory
                ' Para cada tipo de notificacion , verificamos todas las tareas de ese tipo
                For Each dr As DataRow In dt.Rows
                    Try
                        Dim idTaskExecuted As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# Id from Notifications WHERE ID=" & dr("ID") & " And InProgress = 0 "))

                        If idTaskExecuted = 0 Then 'Si el id obtenido es 0, la notificación ya la ha ejecutado otro thread.
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, dr("ID") & "::Already in progress")
                            Continue For
                        End If

                        AccessHelper.ExecuteSql("@UPDATE# Notifications SET InProgress= 1 , GUID='" & gInstanceGUID.ToString & "', LastCheck = GetDate() WHERE ID=" & dr("ID"))

                        bNdrficationProcessed = True
                        cNotificationType = Any2Double(dr("IDType"))

                        Dim oManager As roNotificationTaskManager = oNotificationFactory.GetNotificationTaskManager(gInstanceGUID.ToString(), cNotificationType)
                        If oManager IsNot Nothing Then bNdrficationProcessed = oManager.GenerateNotificationTasks(dr)

                        If bNdrficationProcessed Then
                            roTrace.GetInstance().AddTraceEvent(cNotificationType.ToString & "::Generated succesfully")
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, cNotificationType.ToString & "::Error generating notifications")
                        End If

                        AccessHelper.ExecuteSql("@UPDATE# sysroNotificationTypes SET IsRunning = 0, NextExecution = DATEADD(minute,Scheduler,GETDATE()) WHERE ID =" & dr("IDType"))
                        AccessHelper.ExecuteSql("@UPDATE# Notifications SET InProgress=0, LastCheck = NULL, GUID='' WHERE ID=" & dr("ID"))
                    Catch ex As Exception
                        AccessHelper.ExecuteSql("@UPDATE# Notifications SET InProgress=0, LastCheck = NULL, GUID='' WHERE ID=" & dr("ID"))
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "Error generating notification::" & roTypes.Any2Integer(dr("ID")), ex)
                    End Try
                Next

                Try
                    'Marcamos el executed a 0 de todas las notificaciones que estuvieramos revisando para que la siguiente iteración coja las que toquen en caso de haber fallado.
                    strSQL = "@UPDATE# sysroNotificationTypes SET IsRunning = 0 WHERE ID IN(" & IDNotifications & ")"
                    AccessHelper.ExecuteSql(strSQL)
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Error setting NotificationType as executed", ex)
                End Try

                Dim oTerminalManager As New roNotificationTask_Terminal_Disconnected_Manager(gInstanceGUID.ToString)
                oTerminalManager.GenerateNotificationTasks(Nothing)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "Unexpected error: ", ex)
            End Try

        End Sub

    End Class

End Namespace