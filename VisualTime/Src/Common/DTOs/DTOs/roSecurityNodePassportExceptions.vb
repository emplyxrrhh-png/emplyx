Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roSecurityNodePassportExceptions

        Protected _iIDPassport As Integer
        Protected _iIDSecurityNode As Integer
        Protected _iIDEmployee As Integer
        Protected _iIDApplication As Integer
        Protected _iPermission As Integer

        Public Sub New()
            _iIDPassport = -1
            _iIDSecurityNode = -1
            _iIDEmployee = -1
            _iIDApplication = -1
            _iPermission = 0
        End Sub

        <DataMember()>
        Public Property IDPassport As Integer
            Get
                Return _iIDPassport
            End Get
            Set(value As Integer)
                _iIDPassport = value
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
        Public Property IDEmployee As Integer
            Get
                Return _iIDEmployee
            End Get
            Set(value As Integer)
                _iIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property IDApplication As Integer
            Get
                Return _iIDApplication
            End Get
            Set(value As Integer)
                _iIDApplication = value
            End Set
        End Property

        <DataMember()>
        Public Property Permission As Integer
            Get
                Return _iPermission
            End Get
            Set(value As Integer)
                _iPermission = value
            End Set
        End Property

    End Class

End Namespace