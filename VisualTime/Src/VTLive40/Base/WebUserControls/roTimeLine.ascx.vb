Imports Robotics.Web.Base

Partial Class Base_WebUserControls_roTimeLine
    Inherits UserControlBase

    'Private m_TitleMandatory As String
    'Private m_TitleBreak As String
    'Private m_TitleWorking As String

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Title() As PlaceHolder
        Get
            Return Me.tlTitle
        End Get
    End Property

    'Public Property TitleMandatory() As String
    '    Get
    '        Return m_TitleMandatory
    '    End Get
    '    Set(ByVal value As String)
    '        m_TitleMandatory = value
    '    End Set
    'End Property

    'Public Property TitleBreak() As String
    '    Get
    '        Return m_TitleBreak
    '    End Get
    '    Set(ByVal value As String)
    '        m_TitleBreak = value
    '    End Set
    'End Property

    'Public Property TitleWorking() As String
    '    Get
    '        Return m_TitleWorking
    '    End Get
    '    Set(ByVal value As String)
    '        m_TitleWorking = value
    '    End Set
    'End Property

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Me.IsPostBack Then
            'm_TitleMandatory = Me.Language.Translate("TimeLine.TitleMandatory", DefaultScope)
            'm_TitleBreak = Me.Language.Translate("TimeLine.TitleBreak", DefaultScope)
            'm_TitleWorking = Me.Language.Translate("TimeLine.TitleWorking", DefaultScope)
        End If
    End Sub

End Class