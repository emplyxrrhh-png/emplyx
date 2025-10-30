Partial Class roOptionPanelContainer
    Inherits System.Web.UI.UserControl

    Public Event CheckedChanged(ByVal sender As Object)

    Public m_Width As String = "300px"
    Public m_Height As String = "100px"

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Content() As PlaceHolder
        Get
            Return externalContent
        End Get
    End Property

    Public Property Width() As String
        Get
            Return m_Width
        End Get
        Set(ByVal value As String)
            m_Width = value
            Me.tblContainer.Style("Width") = "calc(" & m_Width & " - 30px)"
        End Set
    End Property

    Public Property Height() As String
        Get
            Return m_Height
        End Get
        Set(ByVal value As String)
            m_Height = value
            Me.tblContainer.Style("Height") = m_Height
        End Set
    End Property

    Public Property tblContenidor() As HtmlGenericControl
        Get
            Return tblContainer
        End Get
        Set(ByVal value As HtmlGenericControl)
            tblContainer = value
        End Set
    End Property

    ''' <summary>
    ''' Tipus de OptionPanel
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum TypusOption

        ''' <summary>
        ''' Estil amb imatge
        ''' </summary>
        ''' <remarks></remarks>
        ImageOption

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

    ''' <summary>
    ''' Grup per agrupar els radioButtons
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property radioGroup() As String
        Get
            Return OptionPanel1.RadioGroup
        End Get
        Set(ByVal value As String)
            OptionPanel1.RadioGroup = value
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
            Return Me.OptionPanel1.TypeOPanel
        End Get
        Set(ByVal value As TypusOption)
            Me.OptionPanel1.TypeOPanel = value
        End Set
    End Property

    ''Public Property Text() As String
    ''    Get
    ''        If ViewState(Me.UniqueID & "#Text") Is Nothing Then
    ''            ViewState(Me.UniqueID & "#Text") = Me.OptionPanel1.Text
    ''        End If
    ''        Return ViewState(Me.UniqueID & "#Text")
    ''    End Get
    ''    Set(ByVal value As String)
    ''        ViewState(Me.UniqueID & "#Text") = value
    ''        Me.OptionPanel1.Text = value
    ''    End Set
    ''End Property

    Public Property Text() As String
        Get
            Return Me.OptionPanel1.Text
        End Get
        Set(ByVal value As String)
            Me.OptionPanel1.Text = value
        End Set
    End Property

    ''' <summary>
    ''' Descripcio llarga del OptionPanel
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Description() As String
        Get
            Return Me.OptionPanel1.Description
        End Get
        Set(ByVal value As String)
            Me.OptionPanel1.Description = value
        End Set
    End Property

    ''' <summary>
    ''' Panel que controlara el OptionPanel
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PanelExtender() As Panel
        Get
            Return Me.OptionPanel1.PanelExtender
        End Get
        Set(ByVal value As Panel)
            Me.OptionPanel1.PanelExtender = value
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
            Return OptionPanel1.Checked
        End Get
        Set(ByVal value As Boolean)
            OptionPanel1.Checked = value
            If value = True Then
                Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2-hover innerMargin"
                Me.tblContainer.Attributes("onmouseover") = ""
                Me.tblContainer.Attributes("onmouseout") = ""
            Else
                Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2 innerMargin"
                Me.tblContainer.Attributes("onmouseover") = "this.className='optionPanelRoboticsV2-hover innerMargin';"
                Me.tblContainer.Attributes("onmouseout") = "this.className='optionPanelRoboticsV2 innerMargin';"
            End If
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            Return Me.OptionPanel1.Enabled
        End Get
        Set(ByVal value As Boolean)
            Me.OptionPanel1.Enabled = value
        End Set
    End Property

#Region "Propietats de les imatges"

    Public WriteOnly Property ImageChecked() As String
        Set(ByVal value As String)
            Me.OptionPanel1.ImageChecked = value
        End Set
    End Property

    ''' <summary>
    ''' URL de la imatge quan es troba desmarcada
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property ImageUnchecked() As String
        Set(ByVal value As String)
            Me.OptionPanel1.ImageUnchecked = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Acces directe a OptionPanel
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property OptionPanel() As roOptionPanel
        Get
            Return OptionPanel1
        End Get
        Set(ByVal value As roOptionPanel)
            OptionPanel1 = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        OptionPanel1.PanelExtender = Panell
        If OptionPanel1.Checked Then
            Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2-hover innerMargin"
            Me.tblContainer.Attributes("onmouseover") = ""
            Me.tblContainer.Attributes("onmouseout") = ""
        Else
            Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2 innerMargin"
            Me.tblContainer.Attributes("onmouseover") = "this.className='optionPanelRoboticsV2-hover innerMargin';"
            Me.tblContainer.Attributes("onmouseout") = "this.className='optionPanelRoboticsV2 innerMargin';"
        End If
        AddHandler OptionPanel1.CheckedChanged, AddressOf OPanel_radioChanged
    End Sub

    Protected Sub OPanel_radioChanged(ByVal sender As Object)
        RaiseEvent CheckedChanged(Me)
        'Cambia el estil del control
        If OptionPanel1.Checked Then
            Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2-hover innerMargin"
            Me.tblContainer.Attributes("onmouseover") = ""
            Me.tblContainer.Attributes("onmouseout") = ""
        Else
            Me.tblContainer.Attributes("class") = "optionPanelRoboticsV2 innerMargin"
            Me.tblContainer.Attributes("onmouseover") = "this.className='optionPanelRoboticsV2-hover innerMargin';"
            Me.tblContainer.Attributes("onmouseout") = "this.className='optionPanelRoboticsV2 innerMargin';"
        End If
    End Sub

End Class