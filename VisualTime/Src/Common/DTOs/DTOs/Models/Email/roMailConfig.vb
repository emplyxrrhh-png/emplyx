Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roMailConfig
        <DataMember>
        Public Property MailServer As String
        <DataMember>
        Public Property ServerPort As Integer
        <DataMember>
        Public Property MailAccount As String
        <DataMember>
        Public Property MailUser As String
        <DataMember>
        Public Property MailPWD As String
        <DataMember>
        Public Property MailAuthentication As String
        <DataMember>
        Public Property MailDomain As String
        <DataMember>
        Public Property UseSSL As Boolean
    End Class

End Namespace