Imports System.Runtime.Serialization
Imports System.Xml.Serialization

<DataContract>
<Serializable>
Public Class wscMenuElement

#Region "Declarations - Constructor"

    Private strName As String
    Private strPath As String
    Private strURL As String
    Private bImage As Byte()
    Private strImageUrl As String
    Private strMainForm As String
    Private iPriority As Integer

    Private lstChildElements As wscMenuElementList

    Public Sub New()
        strName = ""
        strPath = ""
        strURL = ""
        strMainForm = ""
        bImage = Nothing
        iPriority = -1
        lstChildElements = New wscMenuElementList
    End Sub

#End Region

#Region "Properties"

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
    Public Property Priority() As Integer
        Get
            Return Me.iPriority
        End Get
        Set(ByVal value As Integer)
            Me.iPriority = value
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
    Public Property URL() As String
        Get
            Return Me.strURL
        End Get
        Set(ByVal value As String)
            Me.strURL = value
        End Set
    End Property

    <DataMember>
    Public Property ImageUrl() As String
        Get
            Return Me.strImageUrl
        End Get
        Set(ByVal value As String)
            Me.strImageUrl = value
        End Set
    End Property

    <DataMember>
    Public Property ElementImage() As Byte()
        Get
            Return bImage
        End Get
        Set(ByVal value As Byte())
            Me.bImage = value
        End Set
    End Property

    <DataMember>
    Public Property MainForm() As String
        Get
            Return Me.strMainForm
        End Get
        Set(ByVal value As String)
            Me.strMainForm = value
        End Set
    End Property

    <DataMember>
    Public Property Childs() As wscMenuElementList
        Get
            Return Me.lstChildElements
        End Get
        Set(ByVal value As wscMenuElementList)
            Me.lstChildElements = value
        End Set
    End Property

#End Region

End Class

<XmlRoot("Result")>
<DataContract, KnownType(GetType(wscMenuElement))>
<Serializable>
Public Class wscMenuElementList

    Private MenuElements As ArrayList

    Public Sub New()
        MenuElements = New ArrayList
    End Sub

    <XmlArray("List"), XmlArrayItem("wscMenuElement", GetType(wscMenuElement))>
    <DataMember>
    Public Property List() As ArrayList
        Get
            Return Me.MenuElements
        End Get
        Set(ByVal value As ArrayList)
            Me.MenuElements = value
        End Set
    End Property

End Class