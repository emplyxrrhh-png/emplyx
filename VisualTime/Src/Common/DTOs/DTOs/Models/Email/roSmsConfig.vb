Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roSmsConfig
        <DataMember>
        Public Property ConnectionString As String
        <DataMember>
        Public Property QueueName As String
    End Class

End Namespace