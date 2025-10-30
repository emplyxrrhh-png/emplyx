Imports System.Runtime.Serialization

<DataContract()>
Public Class roTreeState

    Private strID As String = ""
    Private strActiveTree As String = "1"

    Private strSelected1 As String = ""
    Private strSelected2 As String = ""
    Private strSelected3 As String = ""
    Private strSelectedPath1 As String = ""
    Private strSelectedPath2 As String = ""
    Private strSelectedPath3 As String = ""
    Private strExpanded1 As String = ""
    Private strExpanded2 As String = ""
    Private strFilter As String = "11110"
    Private strUserFieldFilter As String = ""
    Private strUserField As String = ""
    Private strFieldFindColumn As String = ""
    Private strFieldFindValue As String = ""

    Public Sub New()

    End Sub

    Public Sub New(ByVal _ID As String)
        strID = _ID
    End Sub

    <DataMember(Name:="ID")>
    Public Property ID() As String
        Get
            Return strID
        End Get
        Set(ByVal value As String)
            strID = value
        End Set
    End Property

    <DataMember(Name:="ActiveTree")>
    Public Property ActiveTree() As String
        Get
            Return strActiveTree
        End Get
        Set(ByVal value As String)
            strActiveTree = value
        End Set
    End Property

    <DataMember(Name:="Selected1")>
    Public Property Selected1() As String
        Get
            Return strSelected1
        End Get
        Set(ByVal value As String)
            strSelected1 = value
        End Set
    End Property

    <DataMember(Name:="Selected2")>
    Public Property Selected2() As String
        Get
            Return strSelected2
        End Get
        Set(ByVal value As String)
            strSelected2 = value
        End Set
    End Property

    <DataMember(Name:="Selected3")>
    Public Property Selected3() As String
        Get
            Return strSelected3
        End Get
        Set(ByVal value As String)
            strSelected3 = value
        End Set
    End Property

    <DataMember(Name:="SelectedPath1")>
    Public Property SelectedPath1() As String
        Get
            Return strSelectedPath1
        End Get
        Set(ByVal value As String)
            strSelectedPath1 = value
        End Set
    End Property

    <DataMember(Name:="SelectedPath2")>
    Public Property SelectedPath2() As String
        Get
            Return strSelectedPath2
        End Get
        Set(ByVal value As String)
            strSelectedPath2 = value
        End Set
    End Property

    <DataMember(Name:="SelectedPath3")>
    Public Property SelectedPath3() As String
        Get
            Return strSelectedPath3
        End Get
        Set(ByVal value As String)
            strSelectedPath3 = value
        End Set
    End Property

    <DataMember(Name:="Expanded1")>
    Public Property Expanded1() As String
        Get
            Return strExpanded1
        End Get
        Set(ByVal value As String)
            strExpanded1 = value
        End Set
    End Property

    <DataMember(Name:="Expanded2")>
    Public Property Expanded2() As String
        Get
            Return strExpanded2
        End Get
        Set(ByVal value As String)
            strExpanded2 = value
        End Set
    End Property

    <DataMember(Name:="Filter")>
    Public Property Filter() As String
        Get
            Return strFilter
        End Get
        Set(ByVal value As String)
            strFilter = value
        End Set
    End Property

    <DataMember(Name:="UserFieldFilter")>
    Public Property UserFieldFilter() As String
        Get
            Return strUserFieldFilter
        End Get
        Set(ByVal value As String)
            strUserFieldFilter = value
        End Set
    End Property

    <DataMember(Name:="UserField")>
    Public Property UserField() As String
        Get
            Return strUserField
        End Get
        Set(ByVal value As String)
            strUserField = value
        End Set
    End Property

    <DataMember(Name:="FieldFindColumn")>
    Public Property FieldFindColumn() As String
        Get
            Return strFieldFindColumn
        End Get
        Set(ByVal value As String)
            strFieldFindColumn = value
        End Set
    End Property

    <DataMember(Name:="FieldFindValue")>
    Public Property FieldFindValue() As String
        Get
            Return strFieldFindValue
        End Get
        Set(ByVal value As String)
            strFieldFindValue = value
        End Set
    End Property

End Class