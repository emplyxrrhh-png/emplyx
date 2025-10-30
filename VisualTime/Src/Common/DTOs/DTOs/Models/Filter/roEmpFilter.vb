Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class EmployeeFilter

        <DataMember>
        Public Property Filter As String

        <DataMember>
        Public Property Recursive As Boolean

        <DataMember>
        Public Property [When] As String

    End Class

End Namespace