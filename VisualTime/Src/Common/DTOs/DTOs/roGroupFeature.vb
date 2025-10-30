Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roGroupFeature

        Protected _iID As Integer
        Protected _strName As String
        Protected _strDescription As String
        Protected _businessGroupList As String()
        Protected _strExternalId As String

        Protected _lstFeatures As roGroupFeaturePermissionsOverFeature()

        Public Sub New()
            _iID = -1
            _strName = String.Empty
            _lstFeatures = Nothing
            _businessGroupList = {}
            _strExternalId = String.Empty
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
        Public Property Description As String
            Get
                Return _strDescription
            End Get
            Set(value As String)
                _strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property BusinessGroupList As String()
            Get
                Return _businessGroupList
            End Get
            Set(value As String())
                _businessGroupList = value
            End Set
        End Property

        <DataMember()>
        Public Property Features As roGroupFeaturePermissionsOverFeature()
            Get
                Return _lstFeatures
            End Get
            Set(value As roGroupFeaturePermissionsOverFeature())
                _lstFeatures = value
            End Set
        End Property
        <DataMember()>
        Public Property ExternalId As String
            Get
                Return _strExternalId
            End Get
            Set(value As String)
                _strExternalId = value
            End Set
        End Property

    End Class

End Namespace