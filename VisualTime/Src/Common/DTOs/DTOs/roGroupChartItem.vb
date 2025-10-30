Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roGroupChartItem

        Protected _iID As Integer
        Protected _iIDParent As Integer
        Protected _strName As String
        Protected _strPath As String
        Protected _strFullGroupName As String

        Protected _iIDSecurityNode As Integer
        Protected _strSecurityNodeName As String

        Protected _iAssignedEmployees As Integer

        Public Sub New()
            _iID = -1
            _iIDParent = -1
            _iIDSecurityNode = -1
            _strName = String.Empty
            _strPath = String.Empty
            _strSecurityNodeName = String.Empty
            _strFullGroupName = String.Empty
            _iAssignedEmployees = 0
        End Sub

        <DataMember()>
        Public Property ID As Integer
            Get
                Return _iID
            End Get
            Set(value As Integer)
                _iID = value
            End Set
        End Property

        <DataMember()>
        Public Property IDParent As Integer
            Get
                Return _iIDParent
            End Get
            Set(value As Integer)
                _iIDParent = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _strName
            End Get
            Set(value As String)
                _strName = value
            End Set
        End Property

        <DataMember()>
        Public Property Path As String
            Get
                Return _strPath
            End Get
            Set(value As String)
                _strPath = value
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

        <DataMember()>
        Public Property SecurityNodeName As String
            Get
                Return _strSecurityNodeName
            End Get
            Set(value As String)
                _strSecurityNodeName = value
            End Set
        End Property

        <DataMember()>
        Public Property FullGroupName As String
            Get
                Return _strFullGroupName
            End Get
            Set(value As String)
                _strFullGroupName = value
            End Set
        End Property

        <DataMember()>
        Public Property AssignedEmployees As Integer
            Get
                Return _iAssignedEmployees
            End Get
            Set(value As Integer)
                _iAssignedEmployees = value
            End Set
        End Property

    End Class

End Namespace