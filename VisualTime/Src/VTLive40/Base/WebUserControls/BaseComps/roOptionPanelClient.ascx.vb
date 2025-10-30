Partial Class WebUserControls_roOptionPanelClient
    Inherits System.Web.UI.UserControl

    ''' <summary>
    ''' Tipus de OptionPanel
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TypusOption

        '''' <summary>
        '''' Estil amb imatge
        '''' </summary>
        '''' <remarks></remarks>
        'ImageOption
        ''' <summary>
        ''' Estil amb RadioButton
        ''' </summary>
        ''' <remarks></remarks>
        RadioOption

        ''' <summary>
        ''' Estil amb Checkbox
        ''' </summary>
        ''' <remarks></remarks>
        CheckboxOption

    End Enum

    Private m_TypusOption As TypusOption = TypusOption.CheckboxOption             'Tipus de OptionPanel (Image,Radio, Checkbox)
    Private m_Checked As Boolean = False    'Es troba marcat?
    Private m_Value As String

    Private m_ImageChecked As String = "images/iconsi.png"        'URL de la imatge marcada
    Private m_ImageUnChecked As String = "images/iconno.png"     'URL de la imatge desmarcada

    Private cssText As String = "OptionPanelTextStyle"
    Private cssDesc As String = "OptionPanelDescStyle"
    Private cssImg As String = "OptionPanelImageStyle"
    Private cssRadio As String = "OptionPanelRadioStyle"
    Private cssCheckBox As String = "OptionPanelCheckBoxStyle"

    Private onClickScript As String = ""
    Private onClientClick As String = ""

    Private BorderStyle As Boolean = True

    Private strBorderClass As String = "optionPanelRoboticsV2"
    Public tblCSSPrefix As String = "op"

#Region "Propietats"

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Title() As PlaceHolder
        Get
            Return externalTitle
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Description() As PlaceHolder
        Get
            Return externalDescription
        End Get
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Content() As PlaceHolder
        Get
            Return externalContent
        End Get
    End Property

    ''' <summary>
    ''' CSS per asignar a la imatge
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property cssImgName() As String
        Get
            Return cssImg
        End Get
        Set(ByVal value As String)
            cssImg = value
        End Set
    End Property

    Public ReadOnly Property ContentVisible() As String
        Get
            If Me.externalContent.Controls.Count > 0 Then
                Return ""
            Else
                Return "none"
            End If
        End Get
    End Property

    ''' <summary>
    ''' CSS per asignar al RadioButton
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property cssRadioName() As String
        Get
            Return cssRadio
        End Get
        Set(ByVal value As String)
            cssRadio = value
        End Set
    End Property

    ''' <summary>
    ''' CSS per asignar al texte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property cssTextName() As String
        Get
            Return cssText
        End Get
        Set(ByVal value As String)
            cssText = value
        End Set
    End Property

    ''' <summary>
    ''' CSS per asignar a la descripcio
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property cssDescName() As String
        Get
            Return cssDesc
        End Get
        Set(ByVal value As String)
            cssDesc = value
        End Set
    End Property

    ''' <summary>
    ''' Tipus de Panell
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TypeOPanel() As TypusOption
        Get
            Return m_TypusOption
        End Get
        Set(ByVal value As TypusOption)
            m_TypusOption = value
        End Set
    End Property

    Public Property [Value]() As String
        Get
            If ViewState("Value") Is Nothing Then
                Return ViewState("Value")
            Else
                Return m_Value
            End If

        End Get
        Set(ByVal value As String)
            ViewState("Value") = value
            m_Value = value
        End Set
    End Property

    Public Property Border() As Boolean
        Get
            Return BorderStyle
        End Get
        Set(ByVal value As Boolean)
            If value = False Then
                BorderClass = "None"
                tblCSSPrefix = "none"
            End If

            BorderStyle = value
        End Set
    End Property

    Public Property BorderClass() As String
        Get
            If ViewState("BorderClass") IsNot Nothing Then
                Return ViewState("BorderClass")
            Else
                Return strBorderClass
            End If
        End Get
        Set(ByVal value As String)
            strBorderClass = value
            ViewState("BorderClass") = value
        End Set
    End Property

    Public Property ClientScript() As String
        Get
            If ViewState("ClientScript") Is Nothing Then
                Return ViewState("ClientScript")
            Else
                Return Me.onClickScript
            End If

        End Get
        Set(ByVal value As String)
            If Not value.EndsWith(";") Then value &= ";"
            ViewState("ClientScript") = value
            Me.onClickScript = value
        End Set
    End Property

    Public Property CConClick() As String
        Get
            If ViewState("CConClick") Is Nothing Then
                Return ViewState("CConClick")
            Else
                Return Me.onClientClick
            End If
        End Get
        Set(ByVal value As String)
            onClientClick = value
            ViewState("CConClick") = value
        End Set
    End Property

    ''' <summary>
    ''' Es troba marcat?
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Checked() As Boolean
        Get
            Select Case Me.m_TypusOption
                Case TypusOption.CheckboxOption
                    If chkButton IsNot Nothing Then
                        Return chkButton.Checked
                    Else
                        Return m_Checked
                    End If
                Case TypusOption.RadioOption
                    If rButton IsNot Nothing Then
                        Return rButton.Checked
                    Else
                        Return m_Checked
                    End If
            End Select

            Return False
        End Get
        Set(ByVal value As Boolean)
            Select Case Me.m_TypusOption
                Case TypusOption.CheckboxOption
                    If chkButton IsNot Nothing Then
                        chkButton.Checked = value
                    End If
                Case TypusOption.RadioOption
                    If rButton IsNot Nothing Then
                        rButton.Checked = value
                    End If
            End Select
            m_Checked = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            If ViewState("Enabled") Is Nothing Then
                Return True
            Else
                Return CBool(ViewState("Enabled"))
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("Enabled") = value
        End Set
    End Property

#Region "Propietats de les imatges"

    ''' <summary>
    ''' URL de la imatge quan es troba marcada
    ''' </summary>
    ''' <value></value>
    Public WriteOnly Property ImageChecked() As String
        Set(ByVal value As String)
            m_ImageChecked = value
        End Set
    End Property

    ''' <summary>
    ''' URL de la imatge quan es troba desmarcada
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property ImageUnchecked() As String
        Set(ByVal value As String)
            m_ImageUnChecked = value
        End Set
    End Property

#End Region

#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            IsScriptManagerInParent()

            'Mostra l'estil del control
            RenderStyle()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Mostra segons la selecció del control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RenderStyle()
        Try
            'Amaguem tots els botons abans de fer res...
            rButton.Style("display") = "none"
            chkButton.Style("display") = "none"
            imgButton.Style("display") = "none"

            Select Case m_TypusOption
                Case TypusOption.RadioOption
                    'Control radioButton
                    rButton.Style("display") = ""
                    If m_Checked Then rButton.Checked = True
                    If Enabled = True Then
                        rButton.Attributes("cconChange") = CConClick
                        chkButton.Attributes("cconChange") = CConClick
                        rButton.Attributes("disabled") = "false"
                        aTitle.Attributes("disabled") = "false"
                        aDescription.Attributes("disabled") = "false"
                        rButton.Attributes("onclick") = "clickOPC(this,'" & Me.ClientID & "');"
                        'Prova
                        aTitle.Attributes("onclick") = "CheckLinkClick('" & rButton.ClientID & "');"
                        aDescription.Attributes("onclick") = "CheckLinkClick('" & rButton.ClientID & "');"
                        'Bo
                        'aTitle.Attributes("onclick") = "checkOPCControls('" & rButton.ClientID & "','" & Me.ClientID & "');"
                        'aDescription.Attributes("onclick") = "checkOPCControls('" & rButton.ClientID & "','" & Me.ClientID & "');"
                    Else
                        rButton.Attributes("cconChange") = CConClick
                        chkButton.Attributes("cconChange") = CConClick
                        rButton.Attributes("disabled") = "true"
                        aTitle.Attributes("disabled") = "true"
                        aDescription.Attributes("disabled") = "true"
                        rButton.Attributes("onclick") = "clickOPC(this,'" & Me.ClientID & "');"
                        'Prova
                        aTitle.Attributes("onclick") = "CheckLinkClick('" & rButton.ClientID & "');"
                        aDescription.Attributes("onclick") = "CheckLinkClick('" & rButton.ClientID & "');"
                        'Bo
                        'aTitle.Attributes("onclick") = "checkOPCControls('" & rButton.ClientID & "','" & Me.ClientID & "');"
                        'aDescription.Attributes("onclick") = "checkOPCControls('" & rButton.ClientID & "','" & Me.ClientID & "');"
                    End If
                Case TypusOption.CheckboxOption
                    chkButton.Style("display") = ""
                    If Enabled = True Then
                        rButton.Attributes("cconChange") = CConClick
                        chkButton.Attributes("cconChange") = CConClick
                        rButton.Attributes("disabled") = "false"
                        aTitle.Attributes("disabled") = "false"
                        aDescription.Attributes("disabled") = "false"
                        chkButton.Attributes("onclick") = "clickOPC(this,'" & Me.ClientID & "');"
                        'Prova
                        aTitle.Attributes("onclick") = "CheckLinkClick('" & chkButton.ClientID & "');"
                        aDescription.Attributes("onclick") = "CheckLinkClick('" & chkButton.ClientID & "');"
                        'Bo
                        'aTitle.Attributes("onclick") = "checkOPCControls('" & chkButton.ClientID & "','" & Me.ClientID & "');"
                        'aDescription.Attributes("onclick") = "checkOPCControls('" & chkButton.ClientID & "','" & Me.ClientID & "');"
                    Else
                        rButton.Attributes("cconChange") = CConClick
                        chkButton.Attributes("cconChange") = CConClick
                        rButton.Attributes("disabled") = "true"
                        aTitle.Attributes("disabled") = "true"
                        aDescription.Attributes("disabled") = "true"
                        chkButton.Attributes("onclick") = "clickOPC(this,'" & Me.ClientID & "');"
                        'Prova
                        aTitle.Attributes("onclick") = "CheckLinkClick('" & chkButton.ClientID & "');"
                        aDescription.Attributes("onclick") = "CheckLinkClick('" & chkButton.ClientID & "');"
                        'Bo
                        'aTitle.Attributes("onclick") = "checkOPCControls('" & chkButton.ClientID & "','" & Me.ClientID & "');"
                        'aDescription.Attributes("onclick") = "checkOPCControls('" & chkButton.ClientID & "','" & Me.ClientID & "');"
                    End If

                    If m_Checked Then chkButton.Checked = True
            End Select
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase
        cacheManager.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js", Me.Parent.Page)
        Return True
    End Function

End Class