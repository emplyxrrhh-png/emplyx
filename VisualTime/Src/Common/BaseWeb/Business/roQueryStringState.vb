Imports System.Runtime.Serialization

<DataContract()>
Public Class roQueryStringState

    Private strName As String = String.Empty

    Private strID As String = ""
    Private strIDGroup As String = String.Empty

    Private strHasReg As String = "0" 'Boolean 1=true -- 0=false
    Private strActiveTab As String = ""
    Private strShow As String = String.Empty

    'Propiedades utilizadas sólo para Calendario
    Private strCalendarTypeIntervalDates As String = String.Empty
    Private strCalendarDateInf As String = String.Empty
    Private strCalendarDateSup As String = String.Empty

    Public Sub New()
    End Sub

    Public Sub New(ByVal _Name As String)
        strName = _Name
    End Sub

    <DataMember(Name:="Name")>
    Public Property Name() As String
        Get
            Return strName
        End Get
        Set(ByVal value As String)
            strName = value
        End Set
    End Property

    <DataMember(Name:="ID")>
    Public Property ID() As String
        Get
            Return strID
        End Get
        Set(ByVal value As String)
            strID = value
        End Set
    End Property

    <DataMember(Name:="IDGroup")>
    Public Property IDGroup() As String
        Get
            Return strIDGroup
        End Get
        Set(ByVal value As String)
            strIDGroup = value
        End Set
    End Property

    <DataMember(Name:="HasReg")>
    Public Property HasReg() As String
        Get
            Return strHasReg
        End Get
        Set(ByVal value As String)
            strHasReg = value
        End Set
    End Property

    <DataMember(Name:="ActiveTab")>
    Public Property ActiveTab() As String
        Get
            Return strActiveTab
        End Get
        Set(ByVal value As String)
            strActiveTab = value
        End Set
    End Property

    <DataMember(Name:="Show")>
    Public Property Show() As String
        Get
            Return strShow
        End Get
        Set(ByVal value As String)
            strShow = value
        End Set
    End Property

    <DataMember(Name:="CalendarTypeIntervalDates")>
    Public Property CalendarTypeIntervalDates() As String
        Get
            Return strCalendarTypeIntervalDates
        End Get
        Set(ByVal value As String)
            strCalendarTypeIntervalDates = value
        End Set
    End Property

    <DataMember(Name:="CalendarDateInf")>
    Public Property CalendarDateInf() As String
        Get
            Return strCalendarDateInf
        End Get
        Set(ByVal value As String)
            strCalendarDateInf = value
        End Set
    End Property

    <DataMember(Name:="CalendarDateSup")>
    Public Property CalendarDateSup() As String
        Get
            Return strCalendarDateSup
        End Get
        Set(ByVal value As String)
            strCalendarDateSup = value
        End Set
    End Property

End Class