Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roWebLink

        Protected _id As Integer
        Protected _title As String
        Protected _description As String
        Protected _url As String
        Protected _linkCaption As String
        Protected _position As Integer
        Protected _showOnLiveDashboard As Boolean
        Protected _showOnPortalDashboard As Boolean
        Protected _showOnPortal As Boolean


        Public Sub New()

        End Sub

        <DataMember()>
        Public Property ID As Integer
            Get
                Return _id
            End Get
            Set(value As Integer)
                _id = value
            End Set
        End Property

        <DataMember()>
        Public Property Title As String
            Get
                Return _title
            End Get
            Set(value As String)
                _title = value
            End Set
        End Property

        <DataMember()>
        Public Property Description As String
            Get
                Return _description
            End Get
            Set(value As String)
                _description = value
            End Set
        End Property

        <DataMember()>
        Public Property LinkCaption As String
            Get
                Return _linkCaption
            End Get
            Set(value As String)
                _linkCaption = value
            End Set
        End Property

        <DataMember()>
        Public Property URL As String
            Get
                Return _url
            End Get
            Set(value As String)
                _url = value
            End Set
        End Property

        <DataMember()>
        Public Property Position As Integer
            Get
                Return _position
            End Get
            Set(value As Integer)
                _position = value
            End Set
        End Property

        <DataMember()>
        Public Property ShowOnLiveDashboard As Boolean
            Get
                Return _showOnLiveDashboard
            End Get
            Set(value As Boolean)
                _showOnLiveDashboard = value
            End Set
        End Property

        <DataMember()>
        Public Property ShowOnPortalDashboard As Boolean
            Get
                Return _showOnPortalDashboard
            End Get
            Set(value As Boolean)
                _showOnPortalDashboard = value
            End Set
        End Property

        <DataMember()>
        Public Property ShowOnPortal As Boolean
            Get
                Return _showOnPortal
            End Get
            Set(value As Boolean)
                _showOnPortal = value
            End Set
        End Property

    End Class

End Namespace