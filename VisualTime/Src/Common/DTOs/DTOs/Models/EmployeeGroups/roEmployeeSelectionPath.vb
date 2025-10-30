Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roEmployeeSelectionPath


        Public Sub New()
            Me.IDEmployee = -1
            Me.IDEmployee = -1
            Me.GroupSelectionPath = ""
            Me.EmployeePath = ""
        End Sub

        <DataMember>
        Public Property IDEmployee As Integer

        <DataMember>
        Public Property IDGroup As Integer

        <DataMember>
        Public Property GroupSelectionPath As String

        <DataMember>
        Public Property EmployeePath As String

    End Class

End Namespace