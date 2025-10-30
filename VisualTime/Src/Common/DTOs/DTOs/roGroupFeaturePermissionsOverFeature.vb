Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roGroupFeaturePermissionsOverFeature

        Protected _iIDGroupFeature As Integer
        Protected _iIDFeature As Integer
        Protected _iPermision As Integer

        Public Sub New()
            _iIDGroupFeature = -1
            _iIDFeature = -1
            _iPermision = 0

        End Sub

        <DataMember()>
        Public Property IDGroupFeature As Integer
            Get
                Return _iIDGroupFeature
            End Get
            Set(value As Integer)
                _iIDGroupFeature = value
            End Set
        End Property

        <DataMember()>
        Public Property IDFeature As Integer
            Get
                Return _iIDFeature
            End Get
            Set(value As Integer)
                _iIDFeature = value
            End Set
        End Property
        <DataMember()>
        Public Property Permision As Integer
            Get
                Return _iPermision
            End Get
            Set(value As Integer)
                _iPermision = value
            End Set
        End Property
    End Class

End Namespace