Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roCookie
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public Value As String
    End Class

End Namespace