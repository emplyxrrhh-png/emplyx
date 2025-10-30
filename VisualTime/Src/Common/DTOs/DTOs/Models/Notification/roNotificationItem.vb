Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum NotificationItemType
        <EnumMember> sms
        <EnumMember> email
        <EnumMember> push
    End Enum

    <DataContract>
    <Serializable>
    Public Class roNotificationItem

        <DataMember>
        Public Property [Type] As NotificationItemType

        <DataMember>
        Public Property Destination As String

        <DataMember>
        Public Property Subject As String

        <DataMember>
        Public Property Body As String

        <DataMember>
        Public Property Content As String

        <DataMember>
        Public Property CompanyId As String

    End Class

End Namespace