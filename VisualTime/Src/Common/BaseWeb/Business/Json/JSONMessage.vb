Imports System.Runtime.Serialization

<DataContract()>
Public Class JSONMessage

    Public Enum JMessageType
        MessageBox
        RedirectPage
        LoadControls
        LoadScripts
        RemoteJavascript
        UpdateEmployeeStatus
        UpdateRoundToolbar
        MainMenu
    End Enum

    Private m_Type As JMessageType
    Private m_Result As Object

    Public Sub New()
        m_Type = JMessageType.RedirectPage
    End Sub

    Public Sub New(ByVal xType As JMessageType, ByVal xResult As Object)
        m_Type = xType
        m_Result = xResult
    End Sub

    <DataMember(Name:="msgtypeid")>
    Public Property Type() As JMessageType
        Get
            Return m_Type
        End Get
        Set(ByVal value As JMessageType)
            m_Type = value
        End Set
    End Property

    <DataMember(Name:="msgtype")>
    Public Property TypeStr() As String
        Get
            Select Case m_Type
                Case JMessageType.MessageBox
                    Return "JM_MESSAGEBOX"
                Case JMessageType.RedirectPage
                    Return "JM_REDIRECT"
                Case JMessageType.LoadControls
                    Return "JM_LOADCONTROLS"
                Case JMessageType.LoadScripts
                    Return "JM_LOADSCRIPT"
                Case JMessageType.RemoteJavascript
                    Return "JM_REMOTEJS"
                Case JMessageType.UpdateEmployeeStatus
                    Return "JM_UPDATEEMPLOYEESTATUS"
                Case JMessageType.UpdateRoundToolbar
                    Return "JM_UPDATEROUNDTOOLBAR"
                Case JMessageType.MainMenu
                    Return "JM_MAINMENU"
                Case Else
                    Return ""
            End Select
        End Get
        Set(ByVal value As String)
        End Set
    End Property

    <DataMember(Name:="result")>
    Public Property Result() As Object
        Get
            Return m_Result
        End Get
        Set(ByVal value As Object)
            m_Result = value
        End Set
    End Property

    Public Sub AddMsgBox(ByVal jMsgBox As JSONMsgBox)
        m_Type = JMessageType.MessageBox
        m_Result = jMsgBox
    End Sub

    Public Sub AddRedirectPage(ByVal jRedirect As JSONRedirect)
        m_Type = JMessageType.RedirectPage
        m_Result = jRedirect
    End Sub

    Public Sub AddLoadScript(ByVal jLoadScript As JSONLoadScript)
        m_Type = JMessageType.LoadScripts
        m_Result = jLoadScript
    End Sub

    Public Sub AddRemoteScript(ByVal jRemoteScript As JSONRemoteScript)
        m_Type = JMessageType.RemoteJavascript
        m_Result = jRemoteScript
    End Sub

End Class

<DataContract()>
Public Class JSONMsgBox
    Dim strPrefix As String = ""
    Dim strTitle As String = ""
    Dim strDescription As String = ""
    Dim strButtonText1 As String = ""
    Dim strButtonText2 As String = ""
    Dim strButtonFunc1 As String = ""
    Dim strButtonFunc2 As String = ""
    Dim bolButtonVisible1 As Boolean = True
    Dim bolButtonVisible2 As Boolean = False
    Dim strButtonDescription1 As String = ""
    Dim strButtonDescription2 As String = ""

    Public Sub New()
    End Sub

    Public Sub New(ByVal xPrefix As String, ByVal xTitle As String, ByVal xDescription As String,
                   Optional ByVal xButtonText1 As String = "", Optional ByVal xButtonText2 As String = "",
                   Optional ByVal xButtonFunc1 As String = "", Optional ByVal xButtonFunc2 As String = "",
                   Optional ByVal xBtnVisible1 As Boolean = True, Optional ByVal xBtnVisible2 As Boolean = False,
                   Optional ByVal _ButtonDescription1 As String = "", Optional ByVal _ButtonDescription2 As String = "")
        strPrefix = xPrefix
        strTitle = xTitle
        strDescription = xDescription
        strButtonText1 = xButtonText1
        strButtonText2 = xButtonText2
        strButtonFunc1 = xButtonFunc1
        strButtonFunc2 = xButtonFunc2
        bolButtonVisible1 = xBtnVisible1
        bolButtonVisible2 = xBtnVisible2
        strButtonDescription1 = _ButtonDescription1
        strButtonDescription2 = _ButtonDescription2
    End Sub

    <DataMember(Name:="objprefix")>
    Public Property Prefix() As String
        Get
            Return strPrefix
        End Get
        Set(ByVal value As String)
            strPrefix = value
        End Set
    End Property

    <DataMember(Name:="title")>
    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property

    <DataMember(Name:="description")>
    Public Property Description() As String
        Get
            Return strDescription
        End Get
        Set(ByVal value As String)
            strDescription = value
        End Set
    End Property

    <DataMember(Name:="buttontext1")>
    Public Property ButtonText1() As String
        Get
            Return strButtonText1
        End Get
        Set(ByVal value As String)
            strButtonText1 = value
        End Set
    End Property

    <DataMember(Name:="buttontext2")>
    Public Property ButtonText2() As String
        Get
            Return strButtonText2
        End Get
        Set(ByVal value As String)
            strButtonText2 = value
        End Set
    End Property

    <DataMember(Name:="buttonfunct1")>
    Public Property ButtonFunct1() As String
        Get
            Return strButtonFunc1
        End Get
        Set(ByVal value As String)
            strButtonFunc1 = value
        End Set
    End Property

    <DataMember(Name:="buttonfunct2")>
    Public Property ButtonFunct2() As String
        Get
            Return strButtonFunc2
        End Get
        Set(ByVal value As String)
            strButtonFunc2 = value
        End Set
    End Property

    <DataMember(Name:="buttonvisible1")>
    Public Property ButtonVisible1() As Boolean
        Get
            Return bolButtonVisible1
        End Get
        Set(ByVal value As Boolean)
            bolButtonVisible1 = value
        End Set
    End Property

    <DataMember(Name:="buttonvisible2")>
    Public Property ButtonVisible2() As Boolean
        Get
            Return bolButtonVisible2
        End Get
        Set(ByVal value As Boolean)
            bolButtonVisible2 = value
        End Set
    End Property

    <DataMember(Name:="buttondescription1")>
    Public Property ButtonDescription1() As String
        Get
            Return strButtonDescription1
        End Get
        Set(ByVal value As String)
            strButtonDescription1 = value
        End Set
    End Property

    <DataMember(Name:="buttondescription2")>
    Public Property ButtonDescription2() As String
        Get
            Return strButtonDescription2
        End Get
        Set(ByVal value As String)
            strButtonDescription2 = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONRedirect
    Private strURLRedirect = ""

    Public Sub New()
    End Sub

    Public Sub New(ByVal xURLRedirect As String)
        strURLRedirect = xURLRedirect
    End Sub

    <DataMember(Name:="url")>
    Public Property UrlRedirect() As String
        Get
            Return strURLRedirect
        End Get
        Set(ByVal value As String)
            strURLRedirect = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONLoadScript
    Private strLoadScript = ""

    Public Sub New()
    End Sub

    Public Sub New(ByVal xLoadScript As String)
        strLoadScript = xLoadScript
    End Sub

    <DataMember(Name:="script")>
    Public Property Script() As String
        Get
            Return strLoadScript
        End Get
        Set(ByVal value As String)
            strLoadScript = value
        End Set
    End Property

End Class

<DataContract()>
Public Class JSONRemoteScript
    Private strRemoteScript = ""

    Public Sub New()
    End Sub

    Public Sub New(ByVal xRemoteScript As String)
        strRemoteScript = xRemoteScript
    End Sub

    <DataMember(Name:="execute")>
    Public Property Script() As String
        Get
            Return strRemoteScript
        End Get
        Set(ByVal value As String)
            strRemoteScript = value
        End Set
    End Property

End Class