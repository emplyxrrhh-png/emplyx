Imports System.Runtime.Serialization

<DataContract()>
Public Class roTreeV3Item

    Private strId As String
    Private strTipo As String
    Private strNombre As String
    Private strNombreFull As String
    Private strRuta As String

    Public Sub New()
        strId = String.Empty
        strTipo = String.Empty
        strNombre = String.Empty
        strNombreFull = String.Empty
        strRuta = String.Empty
    End Sub

    <DataMember(Name:="id")>
    Public Property Id() As String
        Get
            Return Me.strId
        End Get
        Set(ByVal value As String)
            Me.strId = value
        End Set
    End Property

    <DataMember(Name:="tipo")>
    Public Property Tipo() As String
        Get
            Return Me.strTipo
        End Get
        Set(ByVal value As String)
            Me.strTipo = value
        End Set
    End Property

    <DataMember(Name:="nombre")>
    Public Property Nombre() As String
        Get
            Return Me.strNombre
        End Get
        Set(ByVal value As String)
            Me.strNombre = value
        End Set
    End Property

    <DataMember(Name:="nombrefull")>
    Public Property NombreFull() As String
        Get
            Return Me.strNombreFull
        End Get
        Set(ByVal value As String)
            Me.strNombreFull = value
        End Set
    End Property

    <DataMember(Name:="ruta")>
    Public Property Ruta() As String
        Get
            Return Me.strRuta
        End Get
        Set(ByVal value As String)
            Me.strRuta = value
        End Set
    End Property

End Class