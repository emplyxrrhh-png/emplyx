Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class ParametersData

        Public Sub New()
            Parameters = New Hashtable
            ValidUntil = DateTime.Now.AddDays(1)
        End Sub

        <DataMember>
        Public Property Parameters As Hashtable
        <DataMember>
        Public Property ValidUntil As DateTime

    End Class

    <DataContract>
    <Serializable>
    Public Class roParameter

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property Value() As String

    End Class

End Namespace