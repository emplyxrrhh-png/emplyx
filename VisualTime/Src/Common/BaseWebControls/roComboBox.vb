Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls

<DefaultProperty("Text"), ToolboxData("<{0}:roComboBox runat=server></{0}:roComboBox>")>
Public Class roComboBox
    Inherits WebControl
    Implements INamingContainer

    Private strCssClassPrefix As String = "roComboBox"
    Private WithEvents oComboBox As New HtmlControls.HtmlAnchor    ''Button

    Private hTable As HtmlControls.HtmlTable 'Taula Principal
    Private hDiv As HtmlControls.HtmlGenericControl
    Public aLabel As New HtmlControls.HtmlAnchor

    Private WithEvents aLabelChilds As HtmlControls.HtmlAnchor

    Public ItemsText As New ArrayList
    Public ItemsClientClick As New ArrayList
    Public ItemsValue As New ArrayList
    Public ItemsEnabled As New ArrayList

    Private m_DS_Dtbl As DataTable
    Private m_Enabled As Boolean = True

    ''' <summary>
    ''' Texto a visualizar en el roComboBox
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property Text() As String
        Get
            If aLabel Is Nothing Then
                Dim s As String = CStr(ViewState("Text"))
                If s Is Nothing Then
                    Return String.Empty
                Else
                    Return s
                End If
            Else
                Return aLabel.InnerText
            End If
        End Get
        Set(ByVal Value As String)
            If Not aLabel Is Nothing Then aLabel.InnerText = Value
            ViewState("Text") = Value
        End Set
    End Property

    ''' <summary>
    ''' Valor Seleccionado en el roComboBox
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property Value() As String
        Get
            Try
                If aLabel.Attributes("value") Is Nothing Then
                    Dim s As String = CStr(ViewState("SelectedValue"))
                    If s Is Nothing Then
                        Return String.Empty
                    Else
                        Return s
                    End If
                Else
                    Return aLabel.Attributes("value")
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Get
        Set(ByVal Value As String)
            ViewState("SelectedValue") = Value
            If Not aLabel Is Nothing Then aLabel.Attributes("value") = Value
        End Set
    End Property

    ''' <summary>
    ''' Valor Seleccionado en el roComboBox
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property SelectedValue() As String
        Get
            Try
                If aLabel Is Nothing Then
                    Dim s As String = CStr(ViewState("SelectedValue"))
                    If s Is Nothing Then
                        Return String.Empty
                    Else
                        Return s
                    End If
                Else
                    If aLabel.Attributes("value") IsNot Nothing Then
                        Return aLabel.Attributes("value")
                    Else
                        Return ""
                    End If
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Get
        Set(ByVal Value As String)
            ViewState("SelectedValue") = Value
            If aLabel IsNot Nothing Then
                For n As Integer = 0 To ItemsText.Count - 1
                    'Si existeix aLabel posiciona el Texte
                    If Value = ItemsValue.Item(n) Then
                        aLabel.InnerText = ItemsText.Item(n)
                        Me.SelectedIndex = n
                        Exit For
                    End If
                Next
                aLabel.Attributes("value") = Value
            End If

            'Carrega del Text
            If Me.HiddenText <> "" Then
                If aLabel IsNot Nothing Then Me.SetHiddenText(aLabel.InnerText)
            End If

            'Carrega del Valor
            If Me.HiddenValue <> "" Then
                Me.SetHiddenValue(Value)
            End If
            'Posicionem a Value (encara que sembla que s'esborra...)
            Me.Value = Value
        End Set
    End Property

    ''' <summary>
    ''' Texto Seleccionado en el roComboBox
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property SelectedText() As String
        Get
            Try
                If aLabel Is Nothing Then
                    Dim s As String = CStr(ViewState("SelectedText"))
                    If s Is Nothing Then
                        Return Me.Text
                    Else
                        Return s
                    End If
                Else
                    Return aLabel.InnerText
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Get
        Set(ByVal Value As String)
            ViewState("SelectedText") = Value
            If Not aLabel Is Nothing Then
                For n As Integer = 0 To ItemsText.Count - 1
                    'Si existeix aLabel posiciona el Texte
                    If Value = ItemsText.Item(n) Then
                        aLabel.Attributes("value") = ItemsValue.Item(n)
                        Me.SelectedIndex = n
                        Exit For
                    End If
                Next
                aLabel.InnerText = Value
            End If

            'Carrega del Text
            If Me.HiddenText <> "" Then
                ''Dim hdnText As HiddenField = Me.Parent.FindControl(Me.HiddenText)
                ''If Not hdnText Is Nothing Then hdnText.Value = Value
                Me.SetHiddenText(Value)
            End If

            'Carrega del Valor
            If Me.HiddenValue <> "" Then
                ''Dim hdnValue As HiddenField = Me.Parent.FindControl(Me.HiddenValue)
                ''If Not hdnValue Is Nothing And Not aLabel Is Nothing Then hdnValue.Value = aLabel.Attributes("value")
                If Not aLabel Is Nothing Then Me.SetHiddenValue(aLabel.Attributes("value"))
            End If
            'Posicionem a Value (encara que sembla que s'esborra...)
            If Not aLabel Is Nothing Then Me.Value = aLabel.Attributes("value")
        End Set
    End Property

    ''' <summary>
    ''' Index del valor Seleccionado
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(-1), Localizable(True)> Property SelectedIndex() As Integer
        Get
            Try
                If ViewState("SelectedIndex") Is Nothing Then
                    Return -1
                Else
                    Dim s As Integer = CInt(ViewState("SelectedIndex"))
                    Return s
                End If
            Catch ex As Exception
                Return -1
            End Try
        End Get
        Set(ByVal Value As Integer)
            'Si el index existeix
            If ItemsText.Count < Value Then Exit Property

            If Not aLabel Is Nothing Then
                aLabel.Attributes("value") = ItemsValue.Item(Value)
                aLabel.InnerText = ItemsText.Item(Value)
            End If

            ViewState("SelectedIndex") = Value
            ViewState("SelectedText") = ItemsText.Item(Value)
            ViewState("SelectedValue") = ItemsValue.Item(Value)

            'Carrega del Text
            If Me.HiddenText <> "" Then
                ''Dim hdnText As HiddenField = Me.Parent.FindControl(Me.HiddenText)
                ''If Not hdnText Is Nothing And Not aLabel Is Nothing Then hdnText.Value = aLabel.InnerText
                If Not aLabel Is Nothing Then Me.SetHiddenText(aLabel.InnerText)
            End If

            'Carrega del Valor
            If Me.HiddenValue <> "" Then
                ''Dim hdnValue As HiddenField = Me.Parent.FindControl(Me.HiddenValue)
                ''If Not hdnValue Is Nothing Then hdnValue.Value = Value
                Me.SetHiddenValue(Value)
            End If

            Me.Value = ItemsValue(Value)
            Me.Text = ItemsText(Value)
        End Set
    End Property

    ''' <summary>
    ''' Alto del ComboBox (solo el padre)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue("16px"), Localizable(True)> Property ParentHeight() As String
        Get
            Dim s As String = CStr(ViewState("ParentHeight"))
            If s Is Nothing Then
                Return "16px"
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("ParentHeight") = Value
        End Set
    End Property

    ''' <summary>
    ''' Ancho del ComboBox (solo el padre)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue("100px"), Localizable(True)> Property ParentWidth() As String
        Get
            Dim s As String = CStr(ViewState("ParentWidth"))
            If s Is Nothing Then
                Return "100px"
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("ParentWidth") = Value
        End Set
    End Property

    ''' <summary>
    ''' Auto-Redimension de los hijos
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue(True), Localizable(True)> Property AutoResizeChildsWidth() As Boolean
        Get
            Dim s As Boolean
            Try
                s = CBool(ViewState("AutoResizeChildsWidth"))
            Catch ex As Exception
                s = True
            End Try
            Return s
        End Get
        Set(ByVal Value As Boolean)
            ViewState("AutoResizeChildsWidth") = Value
        End Set
    End Property

    ''' <summary>
    ''' Ancho de los hijos (solo si no esta en auto-resize), si no se define, coje el ancho del padre
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue("20px"), Localizable(True)> Property ChildsWidth() As String
        Get
            Dim s As String = CStr(ViewState("ChildsWidth"))
            If s Is Nothing Then
                Return "20px"
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("ChildsWidth") = Value
        End Set
    End Property

    ''' <summary>
    ''' Alto de los hijos
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue("16px"), Localizable(True)> Property ChildsHeight() As String
        Get
            Dim s As String = CStr(ViewState("ChildsHeight"))
            If s Is Nothing Then
                Return "16px"
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("ChildsHeight") = Value
        End Set
    End Property

    ''' <summary>
    ''' Cuantos hijos se visualizaran a la vez
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue("8"), Localizable(True)> Property ChildsVisible() As Integer
        Get
            Dim s As String = CStr(ViewState("ChildsVisible"))
            If s Is Nothing Then
                Return 8
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As Integer)
            ViewState("ChildsVisible") = Value
        End Set
    End Property

    Public Property CssClassPrefix() As String
        Get
            Return Me.strCssClassPrefix
        End Get
        Set(ByVal value As String)
            Me.strCssClassPrefix = value
        End Set
    End Property

    Public ReadOnly Property ComboBox() As HtmlControls.HtmlAnchor ''Button
        Get
            Return Me.oComboBox
        End Get
    End Property

    Public Property OnClientClick() As String
        Get
            Dim s As String = CStr(ViewState("OnClientClick"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal value As String)
            ViewState("OnClientClick") = value
        End Set
    End Property
    Public Property OnClientChange() As String
        Get
            Dim s As String = CStr(ViewState("OnClientChange"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal value As String)
            ViewState("OnClientChange") = value
        End Set
    End Property

    ''' <summary>
    ''' Desactiva els events de la part del servidor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Bindable(True), Category("Appearance"), DefaultValue(False), Localizable(True)> Public Property ItemsRunAtServer() As Boolean
        Get
            Try
                Dim s As Boolean = CBool(ViewState("ItemsRunAtServer"))
                Return s
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(ByVal value As Boolean)
            ViewState("ItemsRunAtServer") = value
        End Set
    End Property

    ''' <summary>
    ''' DataTable per carregar les dades
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataSource() As Object
        Get
            If ViewState("DataSource") Is Nothing Then
                Return Nothing
            End If
            Return ViewState("DataSource")
        End Get
        Set(ByVal value As Object)
            ViewState("DataSource") = value
        End Set
    End Property

    ''' <summary>
    ''' Camp del DataSource per carregar el Texte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataTextField() As String
        Get
            Dim s As String = CStr(ViewState("DataTextField"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal Value As String)
            ViewState("DataTextField") = Value
        End Set
    End Property

    ''' <summary>
    ''' Camp del DataSource per carregar el Valor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DataValueField() As String
        Get
            Dim s As String = CStr(ViewState("DataValueField"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal Value As String)
            ViewState("DataValueField") = Value
        End Set
    End Property

    ''' <summary>
    ''' Camp Declarat al Form per guardar el Texte
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HiddenText() As String
        Get
            Dim s As String = CStr(ViewState("HiddenText"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal Value As String)
            ViewState("HiddenText") = Value
        End Set
    End Property

    ''' <summary>
    ''' Camp Declarat al Form per guardar el Valor
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property HiddenValue() As String
        Get
            Dim s As String = CStr(ViewState("HiddenValue"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get
        Set(ByVal Value As String)
            ViewState("HiddenValue") = Value
        End Set
    End Property

    ''' <summary>
    ''' Control si la plana conte el DocType Indicat (arregla el Estil per CSS)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <PersistenceMode(PersistenceMode.InnerProperty)> <Bindable(True), Category("Appearance"), DefaultValue(False), Localizable(True)> Public Property DocTypeEnabled() As Boolean
        Get
            If ViewState("DocTypeEnabled") Is Nothing Then Return False
            Return CBool(ViewState("DocTypeEnabled"))
        End Get
        Set(ByVal Value As Boolean)
            ViewState("DocTypeEnabled") = Value
        End Set
    End Property

    ''' <summary>
    ''' Activa /desactiva el control
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Property [Enabled]() As Boolean
        Get
            Return m_Enabled
        End Get
        Set(ByVal value As Boolean)
            aLabel.Disabled = Not value
            m_Enabled = value
        End Set
    End Property

    Public Event Click As EventHandler

    Public Event ItemClick As EventHandler 'Event que dispara el ItemClick (si esta habilitat el ItemsRunAtServer)

    ''' <summary>
    ''' Afegeix un Item ComboBox
    ''' </summary>
    ''' <param name="Text">Texte a mostrar</param>
    ''' <param name="OnClientClick">Funcio Javascript</param>
    ''' <remarks></remarks>
    Public Sub AddItem(ByVal [Text] As String, ByVal OnClientClick As String)
        AddItem([Text], [Text], m_Enabled, OnClientClick)
    End Sub

    ''' <summary>
    ''' Afegeix un Item ComboBox (Value)
    ''' </summary>
    ''' <param name="Text">Texte a mostrar</param>
    ''' <param name="Value">Valor Intern de l'Item</param>
    ''' <param name="OnClientClick">Funcio Javascript</param>
    ''' <remarks></remarks>
    Public Sub AddItem(ByVal [Text] As String, ByVal [Value] As String, ByVal OnClientClick As String)
        AddItem([Text], [Value], m_Enabled, OnClientClick)
    End Sub

    ''' <summary>
    ''' Afegeix un Item ComboBox (Value, Enabled)
    ''' </summary>
    ''' <param name="Text">Texte a mostrar</param>
    ''' <param name="Value">Valor Intern de l'Item</param>
    ''' <param name="Enabled">Control habilitat o no</param>
    ''' <param name="OnClientClick">Funcio Javascript</param>
    ''' <remarks></remarks>
    Public Sub AddItem(ByVal [Text] As String, ByVal [Value] As String, ByVal [Enabled] As Boolean, ByVal OnClientClick As String)
        ItemsText.Add([Text])
        ItemsValue.Add([Value])
        ItemsClientClick.Add(OnClientClick)
        ItemsEnabled.Add([Enabled])
    End Sub

    ''' <summary>
    ''' Borra tots els Items ComboBox
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearItems()
        ItemsText.Clear()
        ItemsValue.Clear()
        ItemsClientClick.Clear()
        ItemsEnabled.Clear()
    End Sub

    Private Sub oComboBox_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles oComboBox.ServerClick
        RaiseEvent Click(sender, e)
    End Sub

    ''' <summary>
    ''' Recupera la Clase CSS asignada
    ''' </summary>
    ''' <param name="strCssClass"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCssClass(ByVal strCssClass As String) As String
        Dim strRet As String = strCssClass
        Return strRet
    End Function

    ''' <summary>
    ''' Pinta el control
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub CreateChildControls()
        'Try
        Dim hRow As HtmlControls.HtmlTableRow
        Dim hCell As HtmlControls.HtmlTableCell
        Dim aButtonDown As HtmlControls.HtmlAnchor
        'Dim aLabelChilds As HtmlControls.HtmlAnchor

        If Not Me.IsScriptManagerInParent Then Exit Sub
        If Me.DataSource IsNot Nothing Then DataBind()

        'Creem una taula per contenir els combo visible y el desplegable
        hTable = New HtmlControls.HtmlTable
        hTable.Width = Me.ParentWidth
        hTable.Height = Me.ParentHeight
        hTable.Attributes("class") = Me.strCssClassPrefix & "_Table"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0
        hTable.Attributes("name") = "roComboBox"
        hTable.Attributes("vid") = Me.ClientID

        'Celda ComboBox
        hRow = New HtmlControls.HtmlTableRow
        hCell = New HtmlControls.HtmlTableCell
        hCell.Attributes("style") = "width: " & Me.ParentWidth & "; height: " & Me.ParentHeight & ";"

        'aLabel = New HtmlControls.HtmlAnchor
        aLabel.ID = "ComboBoxLabel"
        aLabel.Attributes("class") = Me.strCssClassPrefix & "_ComboBoxLabel"

        'Alinear a la derecha el item seleccionado expresamente porque en FF y SF ya lo hace pero en IE no.
        If Me.DocTypeEnabled Then
            aLabel.Attributes("style") = "text-align: left; width: " & Me.ParentWidth & "; height: " & Me.ParentHeight & ";"
        Else
            aLabel.Attributes("style") = "text-align: left; width: " & Me.ParentWidth & "; height: " & Me.ParentHeight & "; *height: " & (CInt(Me.ParentHeight.Replace("px", "")) + 6) & "px;"
        End If

        aLabel.HRef = "javascript: void(0);"
        If m_Enabled Then aLabel.Attributes("onclick") = "return !roCB_DropDownClick('" & Me.ClientID & "_DropDown" & "',event);"

        If Me.OnClientChange <> "" Then aLabel.Attributes("cbonchange") = OnClientChange

        'Carrega del Text
        If Me.HiddenText <> "" Then
            ''Dim hdnText As HiddenField = Me.Parent.FindControl(Me.HiddenText)
            ''If Not hdnText Is Nothing Then aLabel.InnerText = hdnText.Value
            aLabel.InnerText = Me.GetHiddenText()
        Else
            aLabel.InnerText = Me.Text
        End If

        'Carrega del Valor
        If Me.HiddenValue <> "" Then
            ''Dim hdnValue As HiddenField = Me.Parent.FindControl(Me.HiddenValue)
            ''aLabel.Attributes("value") = hdnValue.Value
            ''Me.Value = hdnValue.Value
            aLabel.Attributes("value") = Me.GetHiddenValue()
            Me.Value = Me.GetHiddenValue()
            Me.SelectedValue = Me.Value
        Else
            aLabel.Attributes("value") = Me.Value
            Me.SelectedValue = Me.Value
        End If

        hCell.Controls.Add(aLabel) 'Afegim el control amb el texte (clicable per desplegar)
        hRow.Cells.Add(hCell)

        'Boto ComboBox
        hCell = New HtmlControls.HtmlTableCell
        hCell.Width = "20px"
        aButtonDown = New HtmlControls.HtmlAnchor
        aButtonDown.ID = "ButtonDown"
        aButtonDown.Attributes("class") = Me.strCssClassPrefix & "_ButtonDown"
        aButtonDown.Attributes("style") = "height: " & Me.ParentHeight & "; *height: " & (CInt(Me.ParentHeight.Replace("px", "")) + 6) & "px;"
        aButtonDown.InnerText = " "
        aButtonDown.HRef = "javascript: void(0);"

        If m_Enabled Then aButtonDown.Attributes("onclick") = "return !roCB_DropDownClick('" & Me.ClientID & "_DropDown" & "',event);"

        hCell.Controls.Add(aButtonDown)
        hRow.Cells.Add(hCell)

        hTable.Rows.Add(hRow)

        'Segona fila per el desplegable
        hRow = New HtmlControls.HtmlTableRow
        hCell = New HtmlControls.HtmlTableCell
        hCell.ColSpan = 2
        hDiv = New HtmlControls.HtmlGenericControl("DIV")
        hDiv.ID = "DropDown"
        hDiv.Attributes("class") = Me.strCssClassPrefix & "_DivContainer"
        Dim heightTotal As String = (CInt(Me.ChildsHeight.Replace("px", "")) * Me.ChildsVisible + 1) + (2 * Me.ChildsVisible + 1) & "px"
        Dim widthTotal As String
        Dim widthTotalIE As String

        If Me.ParentWidth.EndsWith("px") Then
            widthTotal = (CInt(Me.ParentWidth.Replace("px", "")) + 20) & "px"
            widthTotalIE = (CInt(Me.ParentWidth.Replace("px", "")) + 40) & "px"
        Else
            widthTotal = Me.ParentWidth
            widthTotalIE = Me.ParentWidth
        End If

        If Me.AutoResizeChildsWidth Then
            If Me.ItemsText.Count = 0 Then
                hDiv.Attributes("style") = "height: 0; width: auto; *width: 100%; display: none;"
            Else
                hDiv.Attributes("style") = "height: " & heightTotal & "; width: auto; *width: 100%; display: none;"
            End If
        Else
            If Me.ItemsText.Count = 0 Then
                hDiv.Attributes("style") = "height: 0; width: " & widthTotal & " *width: " & widthTotalIE & ";display: none;"
            Else
                hDiv.Attributes("style") = "height: " & heightTotal & "; width: " & widthTotal & " *width: " & widthTotalIE & ";display: none;"
            End If
        End If

        'Ainear a la derecha los items de la lista expresamente porque en FF y SF ya lo hace pero en IE no.
        hDiv.Attributes("align") = "left"

        Dim hTableChilds As New HtmlControls.HtmlTable
        hTableChilds.ID = "tblContainer"
        If Me.AutoResizeChildsWidth = True Then
            hTableChilds.Style("width") = "auto"
        Else
            hTableChilds.Style("width") = widthTotal
        End If
        Dim hTCRows As HtmlControls.HtmlTableRow
        Dim hTCCell As HtmlControls.HtmlTableCell

        'Afegim els Items a la Taula
        For n As Integer = 0 To Me.ItemsText.Count - 1
            hTCRows = New HtmlControls.HtmlTableRow
            hTCCell = New HtmlControls.HtmlTableCell
            If Me.AutoResizeChildsWidth Then
                hTCCell.Attributes("style") = "white-space: nowrap; width: auto;"
            Else
                hTCCell.Attributes("style") = "white-space: nowrap; width: " & widthTotal & ";"
            End If
            'hTCCell.Attributes("style") = "white-space: nowrap; width: " & widthTotal & ";"

            aLabelChilds = New HtmlControls.HtmlAnchor
            aLabelChilds.ID = "LabelChild_" & n
            aLabelChilds.Attributes("class") = Me.strCssClassPrefix & "_LabelChild"
            If Me.AutoResizeChildsWidth Then
                aLabelChilds.Attributes("style") = "height: " & Me.ChildsHeight & "; width: auto; overflow: hidden;"
            Else
                Dim childsHeightIE = CInt(Me.ChildsHeight.Replace("px", "") + 4) & "px"
                aLabelChilds.Attributes("style") = "height: " & Me.ChildsHeight & "; *height: " & childsHeightIE & "; width: " & widthTotal & "; overflow: hidden;"
            End If

            aLabelChilds.InnerText = Me.ItemsText.Item(n).ToString
            aLabelChilds.Attributes("value") = Me.ItemsValue.Item(n).ToString
            aLabelChilds.HRef = "javascript: void(0);"

            'Comproba el HiddenTEXT
            Dim ctlParentText As Control = Me.Parent.FindControl(Me.HiddenText)
            Dim ClientID_Text As String = ""
            If ctlParentText IsNot Nothing Then
                ClientID_Text = ctlParentText.ClientID
            Else
                ClientID_Text = ""
            End If

            'Comproba el HiddenVALUE
            Dim ctlParentvalue As Control = Me.Parent.FindControl(Me.HiddenValue)
            Dim ClientID_Value As String = ""
            If ctlParentvalue IsNot Nothing Then
                ClientID_Value = ctlParentvalue.ClientID
            Else
                ClientID_Value = ""
            End If

            'Si el item es troba actiu...
            If ItemsEnabled(n) = True Then
                aLabelChilds.Attributes("onclick") = "javascript: roCB_Clicked('" & Me.ClientID & "_LabelChild_" & n & "','" & Me.ClientID & "_ComboBoxLabel" & "','" & ClientID_Text & "','" & ClientID_Value & "'); " &
                                                        Me.ItemsClientClick.Item(n).ToString & "; roCB_HideDropDownClick('" & Me.ClientID & "_DropDown" & "',event);"

                If ItemsRunAtServer And ItemsEnabled(n) = True Then AddHandler aLabelChilds.ServerClick, AddressOf OnItemClick
            Else
                aLabelChilds.Disabled = Not ItemsEnabled(n)
                aLabelChilds.Style("color") = "silver"
            End If

            hTCCell.Controls.Add(aLabelChilds)
            hTCRows.Cells.Add(hTCCell)
            hTableChilds.Rows.Add(hTCRows)
        Next

        hDiv.Controls.Add(hTableChilds)
        hCell.InnerHtml &= "<script>roCB_addComboHandler('" & Me.ClientID & "_DropDown');</script>"
        hCell.Controls.Add(hDiv)
        hRow.Cells.Add(hCell)
        hTable.Rows.Add(hRow)

        Me.Controls.Add(hTable)
    End Sub

    ''' <summary>
    ''' Recarrega les dades del DataSource automaticament al ArrayList
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub DataBind()

        Dim dSet As DataSet
        Dim dTbl As New DataTable
        Dim dView As DataView

        'Comproba si els camps necessaris estan omplerts
        If Me.DataSource Is Nothing Then Exit Sub
        If Me.DataTextField = "" Then Exit Sub

        If TypeOf Me.DataSource Is DataSet Then
            dSet = CType(Me.DataSource, DataSet)
            If dSet.Tables.Count = 0 Then Exit Sub
            dTbl = dSet.Tables(0)
        ElseIf TypeOf Me.DataSource Is DataTable Then
            dTbl = CType(Me.DataSource, DataTable)
        ElseIf TypeOf Me.DataSource Is DataView Then
            dView = CType(Me.DataSource, DataView)
            dTbl = dView.ToTable
        Else
            Exit Sub ' El objecte no es valid, surt
        End If

        'Carrega els ArrayList amb les dades del DataSource (dTbl)
        ItemsText.Clear()
        ItemsClientClick.Clear()
        ItemsValue.Clear()
        ItemsEnabled.Clear()
        'DataTable carregat
        For Each dRow As DataRow In dTbl.Rows
            If Me.DataValueField <> "" Then
                AddItem(dRow(Me.DataTextField).ToString, dRow(Me.DataValueField).ToString, m_Enabled, "")
            Else
                AddItem(dRow(Me.DataTextField).ToString, dRow(Me.DataTextField).ToString, m_Enabled, "")
            End If
        Next

    End Sub

    Public Sub OnItemClick(ByVal sender As Object, ByVal e As System.EventArgs)
        RaiseEvent ItemClick(sender, e)
    End Sub

    Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
        MyBase.OnPreRender(e)
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cachebase As New Web.Base.NoCachePageBase
        cachebase.InsertExtraJavascript("roComboBox", "~/Base/Scripts/roComboBox.js", Me.Parent.Page)

        Return True
    End Function

    Private Function GetHiddenText() As String
        Dim strRet As String = ""
        Dim oControl As Control = Me.Parent.FindControl(Me.HiddenText)
        If oControl IsNot Nothing Then
            If TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputText Then
                strRet = CType(oControl, HtmlControls.HtmlInputText).Value
            ElseIf TypeOf oControl Is System.Web.UI.WebControls.HiddenField Then
                strRet = CType(oControl, HiddenField).Value
            ElseIf TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                strRet = CType(oControl, HtmlControls.HtmlInputHidden).Value
            End If
        End If
        Return strRet
    End Function

    Private Function GetHiddenValue() As String
        Dim strRet As String = ""
        Dim oControl As Control = Me.Parent.FindControl(Me.HiddenValue)
        If oControl IsNot Nothing Then
            If TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputText Then
                strRet = CType(oControl, HtmlControls.HtmlInputText).Value
            ElseIf TypeOf oControl Is System.Web.UI.WebControls.HiddenField Then
                strRet = CType(oControl, HiddenField).Value
            ElseIf TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                strRet = CType(oControl, HtmlControls.HtmlInputHidden).Value

            End If
            'If oControl.GetType.ToString = "System.Web.UI.HtmlControls.HtmlInputText" Then
            'ElseIf oControl.GetType.ToString = "System.Web.UI.WebControls.HiddenField" Then
            'End If
        End If
        Return strRet
    End Function

    Private Sub SetHiddenText(ByVal value As String)
        Dim oControl As Control = Me.Parent.FindControl(Me.HiddenText)
        If oControl IsNot Nothing Then
            If TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputText Then
                CType(oControl, HtmlControls.HtmlInputText).Value = value
            ElseIf TypeOf oControl Is System.Web.UI.WebControls.HiddenField Then
                CType(oControl, HiddenField).Value = value
            ElseIf TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                CType(oControl, HtmlControls.HtmlInputHidden).Value = value
            End If
        End If
    End Sub

    Private Sub SetHiddenValue(ByVal value As String)
        Dim oControl As Control = Me.Parent.FindControl(Me.HiddenValue)
        If oControl IsNot Nothing Then
            If TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputText Then
                CType(oControl, HtmlControls.HtmlInputText).Value = value
            ElseIf TypeOf oControl Is System.Web.UI.WebControls.HiddenField Then
                CType(oControl, HiddenField).Value = value
            ElseIf TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputHidden Then
                CType(oControl, HtmlControls.HtmlInputHidden).Value = value
            End If
        End If
    End Sub

    Public Sub CreateChildControls2()
        Me.CreateChildControls()
    End Sub

End Class