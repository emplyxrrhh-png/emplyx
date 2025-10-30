Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roSendNotifications

        <DataMember>
        Public Property Collection As Dictionary(Of eNotificationType, roSendNotificationItem)

        Public Sub New()
            Collection = New Dictionary(Of eNotificationType, roSendNotificationItem)
        End Sub

    End Class

    <DataContract>
    <Serializable>
    Public Class roSendNotificationItem

        <DataMember>
        Public Property [Type] As eNotificationType

        <DataMember>
        Public Property NextExecution As DateTime

    End Class

End Namespace