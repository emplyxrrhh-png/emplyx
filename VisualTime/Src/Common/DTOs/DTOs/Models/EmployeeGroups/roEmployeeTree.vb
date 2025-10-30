Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roEmployeeTree

        Private intID As Integer
        Private strName As String
        Private intType As Integer

        Public Sub New()
            Me.intID = -1
            Me.strName = ""
            Me.intType = 1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Type As Integer)
            Me.intID = _ID
            Me.strName = _Name
            Me.intType = _Type
        End Sub

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
        Public Property Type() As Integer
            Get
                Return Me.intType
            End Get
            Set(ByVal value As Integer)
                Me.intType = value
            End Set
        End Property

    End Class

End Namespace