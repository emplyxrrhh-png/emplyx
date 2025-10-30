Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roGroupTree

#Region "Declarations - Constructor"

        Private intID As Integer
        Private strName As String
        Private strPath As String

        Private lstChildrenGroups As Generic.List(Of roGroupTree)
        Private lstEmployees As Generic.List(Of roEmployeeTree)

        Public Sub New()
            Me.intID = -1
            Me.strName = ""
            Me.strPath = ""
            Me.lstChildrenGroups = New Generic.List(Of roGroupTree)
            Me.lstEmployees = New Generic.List(Of roEmployeeTree)
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Path As String)
            Me.intID = _ID
            Me.strName = _Name
            Me.strPath = _Path
            Me.lstChildrenGroups = New Generic.List(Of roGroupTree)
            Me.lstEmployees = New Generic.List(Of roEmployeeTree)
        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember>
        Public Property Path() As String
            Get
                Return Me.strPath
            End Get
            Set(ByVal value As String)
                Me.strPath = value
            End Set
        End Property

        <DataMember>
        Public Property ChildrenGroups() As Generic.List(Of roGroupTree)
            Get
                Return Me.lstChildrenGroups
            End Get
            Set(ByVal value As Generic.List(Of roGroupTree))
                Me.lstChildrenGroups = value
            End Set
        End Property

        <DataMember>
        Public Property Employees() As Generic.List(Of roEmployeeTree)
            Get
                Return Me.lstEmployees
            End Get
            Set(ByVal value As Generic.List(Of roEmployeeTree))
                Me.lstEmployees = value
            End Set
        End Property

#End Region


    End Class

End Namespace