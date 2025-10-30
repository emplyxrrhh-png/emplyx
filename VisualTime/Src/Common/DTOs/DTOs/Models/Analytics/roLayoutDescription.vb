Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roLayoutDescription

        Public Sub New()
        End Sub

        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Caption As String
        <DataMember>
        Public Property Type As String
        <DataMember>
        Public Property ColumnName As String
    End Class

End Namespace