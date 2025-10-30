Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roSecurityNodeGroup

        Protected _iIDGroup As Integer
        Protected _iIDSecurityNode As Integer
        Protected _iGroupName As String
        Protected _iAssignedEmployees As Integer

        Public Sub New()
            _iIDGroup = -1
            _iIDSecurityNode = -1
            _iGroupName = ""
            _iAssignedEmployees = 0
        End Sub

        <DataMember()>
        Public Property AssignedEmployees As Integer
            Get
                Return _iAssignedEmployees
            End Get
            Set(value As Integer)
                _iAssignedEmployees = value
            End Set
        End Property

        <DataMember()>
        Public Property IDGroup As Integer
            Get
                Return _iIDGroup
            End Get
            Set(value As Integer)
                _iIDGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property GroupName As String
            Get
                Return _iGroupName
            End Get
            Set(value As String)
                _iGroupName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDSecurityNode As Integer
            Get
                Return _iIDSecurityNode
            End Get
            Set(value As Integer)
                _iIDSecurityNode = value
            End Set
        End Property

    End Class

End Namespace