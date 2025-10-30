Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roSecurityNode

        Protected _iID As Integer
        Protected _strName As String
        Protected _iIDParent As Integer
        Protected _strPath As String

        Protected _lstGroups As roSecurityNodeGroup()
        Protected _lstPassports As roSecurityNodePassport()

        Protected _lstChildren As roSecurityNode()

        Protected _IsProductiveCenter As Boolean
        Protected _IsGeographicLocation As Boolean
        Protected _iIDScheduleTemplate As Integer

        Public Sub New()
            _iID = -1
            _strName = String.Empty
            _strPath = String.Empty
            _iIDParent = -1
            _lstGroups = Nothing
            _lstPassports = Nothing
            _lstChildren = Nothing

            _IsProductiveCenter = False
            _IsGeographicLocation = False
            _iIDScheduleTemplate = 0
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
        Public Property Name As String
            Get
                Return _strName
            End Get
            Set(value As String)
                _strName = value
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
        Public Property Path As String
            Get
                Return _strPath
            End Get
            Set(value As String)
                _strPath = value
            End Set
        End Property

        <DataMember()>
        Public Property Groups As roSecurityNodeGroup()
            Get
                Return _lstGroups
            End Get
            Set(value As roSecurityNodeGroup())
                _lstGroups = value
            End Set
        End Property

        <DataMember()>
        Public Property Passports As roSecurityNodePassport()
            Get
                Return _lstPassports
            End Get
            Set(value As roSecurityNodePassport())
                _lstPassports = value
            End Set
        End Property

        <DataMember()>
        Public Property Children As roSecurityNode()
            Get
                Return _lstChildren
            End Get
            Set(value As roSecurityNode())
                _lstChildren = value
            End Set
        End Property

        <DataMember()>
        Public Property IsProductiveCenter As Boolean
            Get
                Return _IsProductiveCenter
            End Get
            Set(value As Boolean)
                _IsProductiveCenter = value
            End Set
        End Property

        <DataMember()>
        Public Property IsGeographicLocation As Boolean
            Get
                Return _IsGeographicLocation
            End Get
            Set(value As Boolean)
                _IsGeographicLocation = value
            End Set
        End Property
        <DataMember()>
        Public Property IDScheduleTemplate As Integer
            Get
                Return _iIDScheduleTemplate
            End Get
            Set(value As Integer)
                _iIDScheduleTemplate = value
            End Set
        End Property

    End Class

End Namespace