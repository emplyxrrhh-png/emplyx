Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roSecurityNodePassport

        Protected _iIDPassport As Integer
        Protected _iIDSecurityNode As Integer
        Protected _iIDFeatureGroup As Integer
        Protected _iLvlOfAuthority As Integer
        Protected _iPassportName As String

        Protected _lstExceptions As roSecurityNodePassportExceptions()

        Public Sub New()
            _iIDPassport = -1
            _iIDSecurityNode = -1
            _iIDFeatureGroup = -1
            _iLvlOfAuthority = 1
            _iPassportName = ""

            _lstExceptions = Nothing
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
        Public Property PassportName As String
            Get
                Return _iPassportName
            End Get
            Set(value As String)
                _iPassportName = value
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
        Public Property IDFeatureGroup As Integer
            Get
                Return _iIDFeatureGroup
            End Get
            Set(value As Integer)
                _iIDFeatureGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property LvlOfAuthority As Integer
            Get
                Return _iLvlOfAuthority
            End Get
            Set(value As Integer)
                _iLvlOfAuthority = value
            End Set
        End Property

        <DataMember()>
        Public Property Exceptions As roSecurityNodePassportExceptions()
            Get
                Return _lstExceptions
            End Get
            Set(value As roSecurityNodePassportExceptions())
                _lstExceptions = value
            End Set
        End Property
    End Class

End Namespace