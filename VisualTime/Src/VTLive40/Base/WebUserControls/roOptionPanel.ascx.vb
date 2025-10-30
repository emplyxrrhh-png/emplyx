Imports Robotics.Web.Base

Partial Class roOptionPanel
    Inherits UserControlBase

    Public Event CheckedChanged(ByVal sender As Object)

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

    Private m_TypusOption As TypusOption = TypusOption.ImageOption              'Tipus de OptionPanel (Image,Radio, Checkbox)
    Private m_Panel As Panel                'Panel enllaçat amb el OptionPanel
    Private m_Checked As Boolean = False    'Es troba marcat?

    Private m_radioGroup As String = "OptionRadioGroup" ' Grup de butons Radio

    Private m_ImageChecked As String = "images/iconsi.png"        'URL de la imatge marcada
    Private m_ImageUnChecked As String = "images/iconno.png"     'URL de la imatge desmarcada

    Private m_Text As String = Me.UniqueID 'Descripcio del OptionPanel
    Private m_Description As String = Me.UniqueID 'Descripcio del OptionPanel

    'Controls per tipus Image -----------------------
    Private ImageBoto As ImageButton

    Private lblText As LinkButton
    Private lblDescripcio As LinkButton

    'Controls per tipus Radio -----------------------
    Private radioDescripcio As RadioButton

    'Controls per tipus Checkbox ---------------------
    Private chkDescripcio As CheckBox

    Private cssText As String = "OptionPanelTextStyle"
    Private cssDesc As String = "OptionPanelDescStyle"
    Private cssImg As String = "OptionPanelImageStyle"
    Private cssRadio As String = "OptionPanelRadioStyle"
    Private cssCheckBox As String = "OptionPanelCheckBoxStyle"

#Region "Propietats"

    ''' <summary>
    ''' Grup de botons radiobutton
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property RadioGroup() As String
        Get
            Return m_radioGroup
        End Get
        Set(ByVal value As String)
            m_radioGroup = value
        End Set
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

    ''' <summary>
    ''' Texte principal del OptionPanel
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Text() As String
        Get
            If ViewState(Me.UniqueID & "#Text") Is Nothing Then
                ViewState(Me.UniqueID & "#Text") = m_Text
                Return m_Text
            Else
                Return ViewState(Me.UniqueID & "#Text")
            End If
        End Get
        Set(ByVal value As String)
            ViewState(Me.UniqueID & "#Text") = value
            m_Text = value
            UpdateText()
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
            If ViewState(Me.UniqueID & "#Description") Is Nothing Then
                ViewState(Me.UniqueID & "#Description") = m_Description
                Return m_Description
            Else
                Return ViewState(Me.UniqueID & "#Description")
            End If
        End Get
        Set(ByVal value As String)
            ViewState(Me.UniqueID & "#Description") = value
            m_Description = value
            UpdateText()
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
            Return m_Panel
        End Get
        Set(ByVal value As Panel)
            m_Panel = value
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
            If hdEstat.Value = "True" Then
                Return True
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                hdEstat.Value = "True"
            Else
                hdEstat.Value = "False"
            End If
            Select Case Me.m_TypusOption
                Case TypusOption.CheckboxOption
                    If chkDescripcio Is Nothing Then Exit Property
                    Me.chkDescripcio.Checked = value
                Case TypusOption.ImageOption
                    If ImageBoto Is Nothing Then Exit Property
                    If value Then
                        ImageBoto.ImageUrl = m_ImageChecked
                    Else
                        ImageBoto.ImageUrl = m_ImageUnChecked
                    End If
                Case TypusOption.RadioOption
                    If radioDescripcio Is Nothing Then Exit Property
                    Me.radioDescripcio.Checked = value
            End Select
            m_Panel.Enabled = value
        End Set
    End Property

    Public Property Enabled() As Boolean
        Get
            If ViewState(Me.UniqueID & "#Enabled") Is Nothing Then
                Return True
            Else
                Return ViewState(Me.UniqueID & "#Enabled")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState(Me.UniqueID & "#Enabled") = value
            Me.LoadEnabled()
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
            'Carrega l'estat del control
            LoadState()

            'Mostra l'estil del control
            RenderStyle()

            ' Activa/Desactiva control
            LoadEnabled()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Comproba l'estat del control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadState()
        Try
            If hdEstat.Value = "True" Then
                m_Checked = True
            Else
                m_Checked = False
            End If
            PanelExtender.Enabled = (Me.Enabled And m_Checked)
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Mostra segons la selecció del control
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RenderStyle()

        Dim hTable As HtmlTable         'Table
        Dim hRow As HtmlTableRow        'TR
        Dim hCell As HtmlTableCell      'TD

        Try

            hTable = New HtmlTable
            hTable.CellPadding = 0
            hTable.CellSpacing = 0
            hTable.Width = "100%"
            'hTable.Height = "100px"
            hTable.Border = 0

            hRow = New HtmlTableRow
            hCell = New HtmlTableCell

            Select Case m_TypusOption
                Case TypusOption.RadioOption
                    'Control radioButton
                    radioDescripcio = New RadioButton
                    radioDescripcio.GroupName = m_radioGroup
                    radioDescripcio.ID = "radioDescripcio"
                    radioDescripcio.Text = Me.Text
                    radioDescripcio.CssClass = Me.cssRadio
                    radioDescripcio.Visible = True
                    radioDescripcio.AutoPostBack = True
                    If m_Checked Then radioDescripcio.Checked = True

                    'Controls.Add(radioDescripcio)
                    hCell.Controls.Add(radioDescripcio)
                    hCell.VAlign = "top"
                    hRow.Cells.Add(hCell)
                    hRow.VAlign = "top"
                    hTable.Rows.Add(hRow)

                    AddHandler radioDescripcio.CheckedChanged, AddressOf radioDescripcio_CheckedChanged

                Case TypusOption.ImageOption
                    ImageBoto = New ImageButton
                    ImageBoto.ID = "ImageButton"
                    If Me.m_Checked = True Then
                        ImageBoto.ImageUrl = m_ImageChecked
                    Else
                        ImageBoto.ImageUrl = m_ImageUnChecked
                    End If

                    ImageBoto.CssClass = Me.cssImg
                    ImageBoto.Visible = True

                    lblText = New LinkButton
                    lblText.ID = "lblText"
                    lblText.Text = Me.Text
                    lblText.CssClass = Me.cssTextName
                    lblText.Visible = True
                    lblText.Style("padding-left") = "8px"

                    hCell.Controls.Add(ImageBoto)
                    hCell.Width = "1px"
                    hCell.VAlign = "top"
                    hRow.Cells.Add(hCell)

                    hCell = New HtmlTableCell
                    Dim hBr As HtmlGenericControl = New HtmlGenericControl("Br")
                    hCell.Controls.Add(lblText)
                    hCell.Controls.Add(hBr)

                    'Afegeix la descripcio
                    lblDescripcio = New LinkButton
                    lblDescripcio.ID = "lblDescripcio"
                    lblDescripcio.Text = Me.Description
                    lblDescripcio.CssClass = Me.cssDescName
                    lblDescripcio.Visible = True
                    'lblDescripcio.Style("padding-left") = "8px"
                    hCell.Controls.Add(lblDescripcio)
                    AddHandler lblDescripcio.Click, AddressOf lblDescripcio_Click

                    hCell.VAlign = "top"
                    hRow.Cells.Add(hCell)
                    hTable.Rows.Add(hRow)

                    AddHandler lblText.Click, AddressOf lblDescripcio_Click
                    AddHandler ImageBoto.Click, AddressOf ImageBoto_Click
                Case TypusOption.CheckboxOption
                    chkDescripcio = New CheckBox
                    chkDescripcio.ID = "chkDescripcio"
                    chkDescripcio.Text = Me.Text
                    chkDescripcio.CssClass = Me.cssCheckBox
                    chkDescripcio.Visible = True
                    chkDescripcio.AutoPostBack = True
                    If m_Checked Then chkDescripcio.Checked = True

                    hCell.VAlign = "top"
                    hCell.Controls.Add(chkDescripcio)
                    hRow.Cells.Add(hCell)
                    hRow.VAlign = "top"
                    hTable.Rows.Add(hRow)

                    AddHandler chkDescripcio.CheckedChanged, AddressOf chkDescripcio_CheckedChanged
            End Select

            'Si es imageoption, no fer una altra linia
            If Me.m_TypusOption <> TypusOption.ImageOption Then
                'Afegeix la descripcio
                hRow = New HtmlTableRow
                hCell = New HtmlTableCell

                lblDescripcio = New LinkButton
                lblDescripcio.ID = "lblDescripcio"
                lblDescripcio.Text = Me.Description
                lblDescripcio.CssClass = Me.cssDescName
                lblDescripcio.Visible = True
                'lblDescripcio.Style("padding-left") = "8px"

                hCell.Controls.Add(lblDescripcio)
                AddHandler lblDescripcio.Click, AddressOf lblDescripcio_Click

                hRow.Cells.Add(hCell)
                hTable.Rows.Add(hRow)
            End If

            'Afegeix la Taula amb els controls
            Controls.Add(hTable)
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try

    End Sub

    Private Sub LoadEnabled()

        Select Case m_TypusOption
            Case TypusOption.RadioOption
                If Me.radioDescripcio IsNot Nothing Then Me.radioDescripcio.Enabled = Me.Enabled
            Case TypusOption.CheckboxOption
                If Me.chkDescripcio IsNot Nothing Then Me.chkDescripcio.Enabled = Me.Enabled
            Case TypusOption.ImageOption
                If Me.ImageBoto IsNot Nothing Then Me.ImageBoto.Enabled = Me.Enabled
        End Select

        If Me.lblDescripcio IsNot Nothing Then Me.lblDescripcio.Enabled = Me.Enabled
        If m_Panel IsNot Nothing Then m_Panel.Enabled = (Me.Enabled And Me.Checked)

    End Sub

    ''' <summary>
    ''' Actualitza el texte
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateText()
        Try
            Select Case m_TypusOption
                Case TypusOption.RadioOption
                    If radioDescripcio IsNot Nothing Then radioDescripcio.Text = Me.Text
                Case TypusOption.ImageOption
                    If lblText IsNot Nothing Then lblText.Text = Me.Text
                Case TypusOption.CheckboxOption
                    If chkDescripcio IsNot Nothing Then chkDescripcio.Text = Me.Text
            End Select
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub chkDescripcio_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            If m_Panel Is Nothing Then Exit Sub
            If sender.Checked = True Then
                m_Checked = True
            Else
                m_Checked = False
            End If

            hdEstat.Value = m_Checked.ToString

            'Dispara l'event
            m_Panel.Enabled = m_Checked

            'Dispara l'event
            RaiseEvent CheckedChanged(chkDescripcio)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub radioDescripcio_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            If m_Panel Is Nothing Then Exit Sub
            If sender.Checked = True Then
                m_Checked = True
            Else
                m_Checked = False
            End If

            hdEstat.Value = m_Checked.ToString

            'Dispara l'event
            m_Panel.Enabled = m_Checked

            'Si el radiobutton es desactiva, no fa res...
            If m_Checked = False Then Exit Sub

            'Dispara l'event
            RaiseEvent CheckedChanged(radioDescripcio)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub lblDescripcio_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If TypeOf sender Is LinkButton Then
            Dim lnk As LinkButton = CType(sender, LinkButton)
            If lnk.ID = "lblDescripcio" Then
                Select Case Me.m_TypusOption
                    Case TypusOption.CheckboxOption
                        Me.chkDescripcio.Checked = Not chkDescripcio.Checked
                        'Dispara l'event
                        Me.chkDescripcio_CheckedChanged(chkDescripcio, e)
                    Case TypusOption.RadioOption
                        If Me.radioDescripcio.Checked = False Then
                            radioDescripcio.Checked = True
                            Me.radioDescripcio_CheckedChanged(radioDescripcio, e)
                        End If
                    Case TypusOption.ImageOption
                        imgChange()
                End Select
            Else
                imgChange()
            End If
        End If
    End Sub

    Protected Sub ImageBoto_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs)
        imgChange()
    End Sub

    Private Sub imgChange()
        Try
            If m_Panel Is Nothing Then Exit Sub
            m_Checked = Not m_Checked
            hdEstat.Value = m_Checked.ToString

            'Dispara l'event
            m_Panel.Enabled = m_Checked

            If m_Checked Then
                ImageBoto.ImageUrl = m_ImageChecked
            Else
                ImageBoto.ImageUrl = m_ImageUnChecked
            End If

            'Dispara l'event
            RaiseEvent CheckedChanged(lblText)
        Catch ex As Exception
        End Try
    End Sub

End Class