Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Web.Base
Imports Robotics.WebControls

Partial Class Base_WebUserControls_roChildSelector
    Inherits UserControlBase

    Private m_cookiePrefix As String = "" 'Prefix per possar davant les cookies

    Dim Criteris(,) As String = {
                                    {"Criteria.Equal", "="},
                                    {"Criteria.Major", ">"},
                                    {"Criteria.MajororEqual", ">="},
                                    {"Criteria.Minor", "<"},
                                    {"Criteria.MinororEqual", "<="},
                                    {"Criteria.Different", "<>"},
                                    {"Criteria.StartsWith", "*"},
                                    {"Criteria.Contains", "**"}
                                    }

    Protected mJSFilterShow As String = "filterFloatVisible('" & Me.ClientID & "');"
    Protected AdvancedFilterDisplay As String = ""

    Protected m_TreeClientID As String = ""

#Region "Properties"

    ''' <summary>
    ''' Filtre Avançat flotant
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property FilterFloat() As Boolean
        Get
            If ViewState("FilterFloat") Is Nothing Then
                Return True
            Else
                Return ViewState("FilterFloat")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("FilterFloat") = value
            If value = True Then
                mJSFilterShow = "filterFloatVisible('" & Me.ClientID & "');"
            Else
                mJSFilterShow = "filterEmbeddedVisible('" & Me.ClientID & "');"
            End If
        End Set
    End Property

    ''' <summary>
    ''' Prefix dels Tree carregats
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PrefixTree() As String
        Get
            If ViewState("PrefixTree") Is Nothing Then
                Return ""
            Else
                Return ViewState("PrefixTree")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("PrefixTree") = value
        End Set
    End Property

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property TreeContainer() As PlaceHolder
        Get
            Return ContentTrees
        End Get
    End Property

    ''' <summary>
    ''' Enllaç dels filtres amb els arbres
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property TreesBehaviorID() As String
        Get
            If ViewState("TreesBehaviorID") Is Nothing Then
                Return ""
            Else
                Return ViewState("TreesBehaviorID")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("TreesBehaviorID") = value
        End Set
    End Property

    ''' <summary>
    ''' Botones de filtro visibles (1111)
    ''' </summary>
    Public Property FiltersVisible() As String
        Get
            If ViewState("FiltersVisible") Is Nothing Then
                Return "1111"
            Else
                Return ViewState("FiltersVisible")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("FiltersVisible") = value
        End Set
    End Property

    ''' <summary>
    ''' Tag de idioma para mostrar tooltip filtro 1
    ''' </summary>
    Public Property Filter1LanguageKey() As String
        Get
            If ViewState("Filter1LanguageKey") Is Nothing Then
                Return "ttFilter1"
            Else
                Return ViewState("Filter1LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter1LanguageKey") = value
        End Set
    End Property

    ''' <summary>
    ''' Tag de idioma para mostrar tooltip filtro 2
    ''' </summary>
    Public Property Filter2LanguageKey() As String
        Get
            If ViewState("Filter2LanguageKey") Is Nothing Then
                Return "ttFilter2"
            Else
                Return ViewState("Filter2LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter2LanguageKey") = value
        End Set
    End Property

    ''' <summary>
    ''' Tag de idioma para mostrar tooltip filtro 3
    ''' </summary>
    Public Property Filter3LanguageKey() As String
        Get
            If ViewState("Filter3LanguageKey") Is Nothing Then
                Return "ttFilter3"
            Else
                Return ViewState("Filter3LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter3LanguageKey") = value
        End Set
    End Property

    ''' <summary>
    ''' Tag de idioma para mostrar tooltip filtro 4
    ''' </summary>
    Public Property Filter4LanguageKey() As String
        Get
            If ViewState("Filter4LanguageKey") Is Nothing Then
                Return "ttFilter4"
            Else
                Return ViewState("Filter4LanguageKey")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter4LanguageKey") = value
        End Set
    End Property

    ''' <summary>
    ''' class del botón del filtro 1
    ''' </summary>
    Public Property Filter1Class() As String
        Get
            If ViewState("Filter1Class") Is Nothing Then
                Return "icoFilter1"
            Else
                Return ViewState("Filter1Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter1Class") = value
        End Set
    End Property

    Public Property Filter2Class() As String
        Get
            If ViewState("Filter2Class") Is Nothing Then
                Return "icoFilter2"
            Else
                Return ViewState("Filter2Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter2Class") = value
        End Set
    End Property

    Public Property Filter3Class() As String
        Get
            If ViewState("Filter3Class") Is Nothing Then
                Return "icoFilter3"
            Else
                Return ViewState("Filter3Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter3Class") = value
        End Set
    End Property

    Public Property Filter4Class() As String
        Get
            If ViewState("Filter4Class") Is Nothing Then
                Return "icoFilter4"
            Else
                Return ViewState("Filter4Class")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("Filter4Class") = value
        End Set
    End Property

    Public Property AdvancedFilterVisible() As Boolean
        Get
            If ViewState("AdvancedFilterVisible") Is Nothing Then
                Return True
            Else
                Return ViewState("AdvancedFilterVisible")
            End If
        End Get
        Set(ByVal value As Boolean)
            ViewState("AdvancedFilterVisible") = value
            Me.AdvancedFilterDisplay = IIf(value, "", "none")
        End Set
    End Property

    Public Property AfterSelectFilterFuncion() As String
        Get
            If ViewState("AfterSelectFilterFuncion") Is Nothing Then
                hdnAfterSelectFilterFuncion.Value = ""
                Return ""
            Else
                Return ViewState("AfterSelectFilterFuncion")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("AfterSelectFilterFuncion") = value
            hdnAfterSelectFilterFuncion.Value = value
        End Set
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim cacheManager As New NoCachePageBase
        cacheManager.InsertExtraJavascript("moment", "~/Base/Scripts/moment.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("momenttz", "~/Base/Scripts/moment-tz.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roDate", "~/Base/Scripts/Live/roDateManager.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("sugarlitejs", "~/Base/globalize/sugar.lite.min.js", Me.Parent.Page)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Try
        IsScriptManagerInParent()

        ' Mostrar botones filtros
        Me.icoFilt1.Style.Add("display", IIf(Me.FiltersVisible.Substring(0, 1) = "1", "", "none"))
        Me.icoFilt2.Style.Add("display", IIf(Me.FiltersVisible.Substring(1, 1) = "1", "", "none"))
        Me.icoFilt3.Style.Add("display", IIf(Me.FiltersVisible.Substring(2, 1) = "1", "", "none"))
        Me.icoFilt4.Style.Add("display", IIf(Me.FiltersVisible.Substring(3, 1) = "1", "", "none"))

        Me.icoFilt5.Style.Add("display", IIf(Me.AdvancedFilterVisible, "", "none"))
        Me.icoFiltAdv.Style.Add("display", IIf(Me.AdvancedFilterVisible, "", "none"))

        Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState(Me.ClientID & "_" & Me.TreesBehaviorID)

        If Me.AdvancedFilterVisible Then

            If FilterFloat = True Then
                mJSFilterShow = "filterFloatVisible('" & Me.ClientID & "');"
                CreateFilterFloat()
            Else
                mJSFilterShow = "filterEmbeddedVisible('" & Me.ClientID & "','" & PrefixTree & "');"
                CreateFilterEmbedd()
            End If

            'Carrega els filtres actuals (recuperats per cookies)
            RetrieveActualFilters(oTreeState)
        Else
            mJSFilterShow = ""
        End If

        ' Inicializar los filtros a aplicar
        If oTreeState.Filter <> "" AndAlso oTreeState.Filter.Length >= 5 Then
            Me.icoFilt1.Attributes("class") = Me.Filter1Class & " roCanHide " & IIf(oTreeState.Filter.Substring(0, 1) = "1", "icoPressed", "icoUnPressed")
            Me.icoFilt2.Attributes("class") = Me.Filter2Class & " roCanHide " & IIf(oTreeState.Filter.Substring(1, 1) = "1", "icoPressed", "icoUnPressed")
            Me.icoFilt3.Attributes("class") = Me.Filter3Class & " roCanHide " & IIf(oTreeState.Filter.Substring(2, 1) = "1", "icoPressed", "icoUnPressed")
            Me.icoFilt4.Attributes("class") = Me.Filter4Class & " roCanHide " & IIf(oTreeState.Filter.Substring(3, 1) = "1", "icoPressed", "icoUnPressed")
            Me.icoFilt5.Attributes("class") = Me.icoFilt5.Attributes("class").Split(" ")(0) & " roCanHide " & IIf(oTreeState.Filter.Substring(4, 1) = "1", "icoPressed", "icoUnPressed")
            Me.icoFiltAdv.Attributes("class") = Me.icoFiltAdv.Attributes("class").Split(" ")(0) & " roCanHide " & IIf(oTreeState.UserFieldFilter <> String.Empty, "icoPressed", "icoUnPressed")
        End If

        'Traduccions Icones filtrat
        Me.icoFilt1.Title = Me.Language.Translate(Me.Filter1LanguageKey, "roSelector")
        Me.icoFilt2.Title = Me.Language.Translate(Me.Filter2LanguageKey, "roSelector")
        Me.icoFilt3.Title = Me.Language.Translate(Me.Filter3LanguageKey, "roSelector")
        Me.icoFilt4.Title = Me.Language.Translate(Me.Filter4LanguageKey, "roSelector")
        Me.icoFilt5.Title = Me.Language.Translate("ttFilter5", "roSelector")
        Me.icoFiltAdv.Title = Me.Language.Translate("AdvancedFilter", "roSelector")

        'Recupera el ClientID dels arbres Trees
        If Me.TreesBehaviorID <> "" Then
            Dim objTree As Object = Me.ContentTrees.FindControl(Me.TreesBehaviorID)
            If objTree IsNot Nothing Then
                m_TreeClientID = objTree.ClientID
            End If
        End If

        'Activa els Onclick per els filtres (client)
        Me.icoFilt1.Attributes("onclick") = "UpdTreeFilter(this,'" & Me.ClientID & "','" & TreesBehaviorID & "');"
        Me.icoFilt2.Attributes("onclick") = "UpdTreeFilter(this,'" & Me.ClientID & "','" & TreesBehaviorID & "');"
        Me.icoFilt3.Attributes("onclick") = "UpdTreeFilter(this,'" & Me.ClientID & "','" & TreesBehaviorID & "');"
        Me.icoFilt4.Attributes("onclick") = "UpdTreeFilter(this,'" & Me.ClientID & "','" & TreesBehaviorID & "');"
        Me.icoFilt5.Attributes("onclick") = "UpdTreeFilter(this,'" & Me.ClientID & "','" & TreesBehaviorID & "');"
        Me.icoFiltAdv.Attributes("onclick") = Me.mJSFilterShow

        If Me.AfterSelectFilterFuncion <> "" Then
            Me.icoFilt1.Attributes("onclick") &= Me.AfterSelectFilterFuncion
            Me.icoFilt2.Attributes("onclick") &= Me.AfterSelectFilterFuncion
            Me.icoFilt3.Attributes("onclick") &= Me.AfterSelectFilterFuncion
            Me.icoFilt4.Attributes("onclick") &= Me.AfterSelectFilterFuncion
            Me.icoFilt5.Attributes("onclick") &= Me.AfterSelectFilterFuncion
            Me.icoFiltAdv.Attributes("onclick") &= Me.AfterSelectFilterFuncion
        End If

        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New NoCachePageBase
        cacheManager.InsertExtraJavascript("roChildSelector", "~/Base/Scripts/roChildSelector.js", Me.Parent.Page)
        Return True
    End Function

    ''' <summary>
    ''' Crea filtre en el area del Tree
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateFilterEmbedd()
        'Try
        Dim hTable As HtmlTable = New HtmlTable
        Dim hRow As HtmlTableRow
        Dim hCell As HtmlTableCell

        hTable.CellPadding = 0
        hTable.CellSpacing = 0
        hTable.Border = 0
        hTable.Width = "auto"
        hTable.Attributes("style") = "padding: 2px;"

        hRow = New HtmlTableRow
        hCell = New HtmlTableCell
        hCell.ColSpan = 3
        hCell.VAlign = "top"
        hCell.Attributes("style") = "width: auto;"

        Dim hTable2 As New HtmlTable
        Dim hRow2 As New HtmlTableRow
        Dim hCell2 As New HtmlTableCell

        hTable2.Border = 0
        hTable2.Width = "100%"
        hTable2.Height = "100%"

        'hCell2.Height = "160px"
        Dim hDivFilterContainer As New HtmlGenericControl("div")
        hDivFilterContainer.ID = "dvContainer"
        hDivFilterContainer.Attributes("style") = "border: solid 1px silver; width: auto; height: 200px; overflow: auto;"
        Me.createEmbeddFilterFields(hDivFilterContainer)
        hCell2.Controls.Add(hDivFilterContainer)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hRow2 = New HtmlTableRow
        hCell2 = New HtmlTableCell

        hCell2.Attributes("style") = "padding: 5px; height:10px;"
        hCell2.Align = "right"

        Dim hTable3 As New HtmlTable
        Dim hRow3 As New HtmlTableRow
        Dim hCell3 As New HtmlTableCell

        hTable3.Width = "100%"
        hTable3.Border = 0
        hTable3.Height = "auto"
        hCell3.Align = "left"

        Dim hAnchor As New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "ClearUserFieldFilter('" & Me.ClientID & "');"
        hAnchor.InnerText = Me.Language.Translate("Button.Clear", "roSelector")
        hCell3.Controls.Add(hAnchor)
        hRow3.Cells.Add(hCell3)

        hCell3 = New HtmlTableCell
        hCell3.Align = "right"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "SaveUserFieldFilter(""" & Me.ClientID & """, """ & Me.PrefixTree & """,""" & mJSFilterShow & """);"
        hAnchor.InnerText = Me.Language.Keyword("Button.Accept")
        hCell3.Controls.Add(hAnchor)

        Dim hSpan As New HtmlGenericControl("span")
        hSpan.InnerText = " | "
        hCell3.Controls.Add(hSpan)

        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = mJSFilterShow
        hAnchor.InnerText = Me.Language.Keyword("Button.Cancel")
        hCell3.Controls.Add(hAnchor)

        hRow3.Cells.Add(hCell3)
        hTable3.Rows.Add(hRow3)

        hCell2.Controls.Add(hTable3)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hCell.Controls.Add(hTable2)
        hRow.Cells.Add(hCell)
        hTable.Rows.Add(hRow)

        Me.divFiltreAvan.Controls.Add(hTable)
        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    ''' <summary>
    ''' Crea els camps del filtre (estructura flotant)
    ''' </summary>
    ''' <param name="hDiv">Div on anira inclos el contingut generat</param>
    ''' <remarks></remarks>
    Private Sub CreateFloatFilterFields(ByRef hDiv As HtmlGenericControl)
        Try
            Dim hTable As New HtmlTable
            Dim hRow As HtmlTableRow
            Dim hCell As HtmlTableCell

            'Carreguem el combo principal (Noms de Camp)
            Dim hSelect As roComboBox

            Dim hText As HtmlInputText

            Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
            Dim dRows() As DataRow = dTbl.Select(Nothing, "FieldName")

            'Capcelera
            hRow = New HtmlTableRow
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Field", "roSelector") '"Campo"
            hRow.Cells.Add(hCell)
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Criteria", "roSelector") '"Criterio"
            hRow.Cells.Add(hCell)
            hCell = New HtmlTableCell
            hCell.InnerText = Me.Language.Translate("Criteria.Value", "roSelector") '"Valor"
            hRow.Cells.Add(hCell)
            hTable.Rows.Add(hRow)

            'Creem les files de criteris (5 per omisió)
            For n As Integer = 1 To 5
                hRow = New HtmlTableRow
                hCell = New HtmlTableCell

                hSelect = New roComboBox
                hSelect.AutoResizeChildsWidth = True
                hSelect.DocTypeEnabled = False
                hSelect.ChildsWidth = "200px"
                hSelect.ID = "UserFieldFilter" & n.ToString
                For Each dRow As DataRow In dRows
                    If dRow("Used") = False Then Continue For
                    hSelect.AddItem(dRow("FieldName"), "Usr_" & dRow("FieldName") & "|" & dRow("FieldType"), "")
                Next

                'Afegeix el combo de camps d'usuari
                hCell.Controls.Add(hSelect)
                hRow.Cells.Add(hCell)

                'Select de criteris
                hCell = New HtmlTableCell
                hSelect = New roComboBox
                hSelect.ID = "CriteriaFilter" & n.ToString

                For nCrit As Integer = Criteris.GetLowerBound(0) To Criteris.GetUpperBound(0)
                    hSelect.AddItem(Me.Language.Translate(Criteris(nCrit, 0), "roSelector"), Criteris(nCrit, 1), "")
                Next

                'Afegeix el combo de criteris
                hCell.Controls.Add(hSelect)
                hRow.Cells.Add(hCell)

                'Afegeix el camp de busqueda
                hCell = New HtmlTableCell
                hText = New HtmlInputText
                hText.ID = "ValueFilter" & n.ToString
                hText.Attributes("class") = "textClass"
                hText.Style("width") = "170px"
                hText.Value = ""

                'Afegeix els inputs de busqueda
                hCell.Controls.Add(hText)
                hRow.Cells.Add(hCell)

                hTable.Rows.Add(hRow)

                'Afegim la fila de Y o O (AND OR)
                If n <> 5 Then
                    hRow = New HtmlTableRow
                    hCell = New HtmlTableCell
                    hCell.ColSpan = 3
                    Dim hTableYO As New HtmlTable
                    Dim hRowYO As New HtmlTableRow
                    Dim hCellYO As New HtmlTableCell
                    Dim hOption As New HtmlInputRadioButton
                    hOption.ID = "OptionAND" & n
                    hOption.Name = "AndAndOr" & n
                    hOption.Value = "AND"
                    hOption.Checked = True
                    hOption.Attributes("class") = "inputRadioClass"
                    hCellYO.Controls.Add(hOption)
                    Dim lblAndOr As Label = New Label 'Etiqueta
                    lblAndOr.Text = "Y"
                    lblAndOr.CssClass = "inputRadioClass"
                    hCellYO.Controls.Add(lblAndOr)
                    hRowYO.Cells.Add(hCellYO)

                    hCellYO = New HtmlTableCell
                    hOption = New HtmlInputRadioButton
                    hOption.ID = "OptionOR" & n
                    hOption.Name = "AndAndOr" & n
                    hOption.Value = "OR"
                    hOption.Attributes("class") = "inputRadioClass"
                    hCellYO.Controls.Add(hOption)
                    lblAndOr = New Label ' Etiqueta
                    lblAndOr.Text = "O"
                    lblAndOr.CssClass = "inputRadioClass"
                    hCellYO.Controls.Add(lblAndOr)
                    hRowYO.Cells.Add(hCellYO)

                    hTableYO.Rows.Add(hRowYO)
                    hCell.Controls.Add(hTableYO)
                    hRow.Cells.Add(hCell)
                    hTable.Rows.Add(hRow)
                End If

            Next

            hDiv.Controls.Add(hTable)
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

    ''' <summary>
    ''' Crea els camps del filtre (estructura embebida)
    ''' </summary>
    ''' <param name="hDiv">DIV que contindra el contingut</param>
    ''' <remarks></remarks>
    Private Sub createEmbeddFilterFields(ByRef hDiv As HtmlGenericControl)
        'Try
        Dim colAlternate As String = "#E0E5EF"

        Dim hTable As New HtmlTable
        hTable.CellPadding = 0
        hTable.CellSpacing = 0
        Dim hRow As HtmlTableRow
        Dim hCell As HtmlTableCell

        'Carreguem el combo principal (Noms de Camp)
        Dim hCombo As Robotics.WebControls.roComboBox
        'Dim hListItem As ListItem

        Dim hText As HtmlInputText

        Dim dTbl As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "", False)
        Dim dRows() As DataRow = dTbl.Select(Nothing, "FieldName")

        'Capcelera
        hRow = New HtmlTableRow
        hCell = New HtmlTableCell
        hCell.Style("padding") = "2px"
        hCell.BgColor = colAlternate
        hCell.InnerText = Me.Language.Translate("Criteria.Field", "roSelector") '"Campo"
        hRow.Cells.Add(hCell)
        hCell = New HtmlTableCell
        hCell.Style("padding") = "2px"
        hCell.BgColor = colAlternate
        hCell.InnerText = Me.Language.Translate("Criteria.Criteria", "roSelector") '"Criterio"
        hRow.Cells.Add(hCell)
        'hCell = New HtmlTableCell
        'hCell.InnerText = Me.Language.Translate("Criteria.Value", "roSelector") '"Valor"
        'hRow.Cells.Add(hCell)
        hTable.Rows.Add(hRow)

        'Creem les files de criteris (5 per omisió)
        For n As Integer = 1 To 5
            hRow = New HtmlTableRow
            hCell = New HtmlTableCell
            hCell.BgColor = colAlternate
            hCell.Style("padding") = "2px"

            hCombo = New Robotics.WebControls.roComboBox
            hCombo.ID = "UserFieldFilter" & n.ToString
            hCombo.ParentWidth = "150px"
            hCombo.ChildsVisible = 4
            hCombo.ClearItems()
            hCombo.AddItem("", "")
            For Each dRow As DataRow In dRows
                If dRow("Used") = False Then Continue For
                hCombo.AddItem(dRow("FieldName"), "Usr_" & dRow("FieldName") & "|" & dRow("FieldType"), "")
                'hListItem.Value = "Usr_" & dRow("FieldName") & "|" & dRow("FieldType")
            Next

            'Afegeix el combo de camps d'usuari
            'hCell.Controls.Add(hSelect)
            hCell.Controls.Add(hCombo)
            hRow.Cells.Add(hCell)

            'Select de criteris
            hCell = New HtmlTableCell
            hCell.BgColor = colAlternate
            hCell.Style("padding") = "2px"
            'hSelect = New HtmlSelect
            'hSelect.ID = "CriteriaFilter" & n.ToString
            'hSelect.Attributes("class") = "comboClass"
            ''hSelect.Attributes("onchange") = "alert(this.value);"

            'For nCrit As Integer = Criteris.GetLowerBound(0) To Criteris.GetUpperBound(0)
            '    hListItem = New ListItem
            '    hListItem.Text = Me.Language.Translate(Criteris(nCrit, 0), "roSelector")
            '    hListItem.Value = Criteris(nCrit, 1)
            '    hSelect.Items.Add(hListItem)
            'Next

            ''Afegeix el combo de criteris
            'hCell.Controls.Add(hSelect)

            hCombo = New Robotics.WebControls.roComboBox
            hCombo.ID = "CriteriaFilter" & n.ToString
            hCombo.AutoResizeChildsWidth = "true"
            hCombo.ParentWidth = "100px"
            hCombo.ChildsVisible = 6
            hCombo.ClearItems()
            For nCrit As Integer = Criteris.GetLowerBound(0) To Criteris.GetUpperBound(0)
                hCombo.AddItem(Me.Language.Translate(Criteris(nCrit, 0), "roSelector"), Criteris(nCrit, 1), "")
            Next

            'Afegeix el combo de criteris
            hCell.Controls.Add(hCombo)
            hRow.Cells.Add(hCell)

            'Afegeix el camp de busqueda
            hCell = New HtmlTableCell
            hCell.Style("padding") = "2px"
            hCell.BgColor = colAlternate
            hText = New HtmlInputText
            hText.ID = "ValueFilter" & n.ToString
            hText.Attributes("class") = "textClass"
            hText.Style("width") = "170px"
            hText.Value = ""

            hTable.Rows.Add(hRow)
            hRow = New HtmlTableRow

            hCell = New HtmlTableCell
            hCell.Style("padding") = "2px"
            hCell.BgColor = colAlternate
            hCell.InnerText = Me.Language.Translate("Criteria.Value", "roSelector") & " " '"Valor"
            hCell.Width = "40px"
            hRow.Cells.Add(hCell)

            'Afegeix els inputs de busqueda
            hCell.Controls.Add(hText)
            hCell.ColSpan = 2
            hRow.Cells.Add(hCell)

            hTable.Rows.Add(hRow)

            'Afegim la fila de Y o O (AND OR)
            If n <> 5 Then
                hRow = New HtmlTableRow
                hCell = New HtmlTableCell
                hCell.Style("padding") = "2px"
                hCell.BgColor = colAlternate
                hCell.ColSpan = 3
                Dim hTableYO As New HtmlTable
                Dim hRowYO As New HtmlTableRow
                Dim hCellYO As New HtmlTableCell
                Dim hOption As New HtmlInputRadioButton
                hOption.ID = "OptionAND" & n
                hOption.Name = "AndAndOr" & n
                hOption.Value = "AND"
                hOption.Checked = True
                hOption.Attributes("class") = "inputRadioClass"
                hCellYO.Controls.Add(hOption)
                Dim lblAndOr As Label = New Label 'Etiqueta
                lblAndOr.Text = "Y"
                lblAndOr.CssClass = "inputRadioClass"
                hCellYO.Controls.Add(lblAndOr)
                hRowYO.Cells.Add(hCellYO)

                hCellYO = New HtmlTableCell
                hCell.BgColor = colAlternate
                hOption = New HtmlInputRadioButton
                hOption.ID = "OptionOR" & n
                hOption.Name = "AndAndOr" & n
                hOption.Value = "OR"
                hOption.Attributes("class") = "inputRadioClass"
                hCellYO.Controls.Add(hOption)
                lblAndOr = New Label ' Etiqueta
                lblAndOr.Text = "O"
                lblAndOr.CssClass = "inputRadioClass"
                hCellYO.Controls.Add(lblAndOr)
                hRowYO.Cells.Add(hCellYO)

                hTableYO.Rows.Add(hRowYO)
                hCell.Controls.Add(hTableYO)
                hRow.Cells.Add(hCell)
                hTable.Rows.Add(hRow)
            End If
            If colAlternate = "#FFFFFF" Then
                colAlternate = "#E0E5EF"
            Else
                colAlternate = "#FFFFFF"
            End If
        Next

        hDiv.Controls.Add(hTable)

        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    ''' <summary>
    ''' Crea filtre flotant
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateFilterFloat()
        'Try

        Dim hDivTb As New HtmlGenericControl("div")
        hDivTb.Attributes("class") = "RoundCornerFrame roundCorner bodyPopup"
        hDivTb.Attributes("style") = "width: auto; *width: 470px;"

        'TAULA 2 (Contenidora dels camps)
        Dim hTable2 As New HtmlTable
        Dim hRow2 As New HtmlTableRow
        Dim hCell2 As New HtmlTableCell
        hTable2.Border = 0

        Dim hDiv As New HtmlGenericControl("div")
        CreateFloatFilterFields(hDiv)

        hCell2.Controls.Add(hDiv)
        hRow2.Cells.Add(hCell2) '<TD> amb DIV Controls
        hTable2.Rows.Add(hRow2) '<TR> Div Controls

        hRow2 = New HtmlTableRow
        hCell2 = New HtmlTableCell

        'TAULA 3 (Botonera FOOTER)
        Dim hTable3 As New HtmlTable

        Me.CreateButtonFooter(hTable3)
        hCell2.Controls.Add(hTable3)
        hRow2.Cells.Add(hCell2)
        hTable2.Rows.Add(hRow2)

        hDivTb.Controls.Add(hTable2)

        Me.divFiltreAvan_Float.Controls.Add(hDivTb)

        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    ''' <summary>
    ''' Crea els botons del peu (borrar, Aceptar, Cancelar)
    ''' </summary>
    ''' <param name="hTable3">HtmlTable a omplir</param>
    ''' <remarks></remarks>
    Private Sub CreateButtonFooter(ByRef hTable3 As HtmlTable)
        Dim hRow3 As New HtmlTableRow
        Dim hCell3 As New HtmlTableCell
        Dim hAnchor As New HtmlAnchor

        hTable3.Width = "100%"
        hTable3.Border = 0
        hTable3.BorderColor = "blue"
        hTable3.Height = "10px"

        hCell3.Align = "left"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "ClearUserFieldFilter('" & Me.ClientID & "');"
        hAnchor.InnerText = Me.Language.Translate("Button.Clear", "roSelector")
        hCell3.Controls.Add(hAnchor)
        hRow3.Cells.Add(hCell3)

        hCell3 = New HtmlTableCell
        hCell3.Align = "right"
        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = "SaveUserFieldFilter(""" & Me.ClientID & """,""" & Me.PrefixTree & """,""" & mJSFilterShow & """);"
        hAnchor.InnerText = Me.Language.Keyword("Button.Accept")
        hCell3.Controls.Add(hAnchor)

        Dim hSpan As New HtmlGenericControl("span")
        hSpan.InnerText = " | "
        hCell3.Controls.Add(hSpan)

        hAnchor = New HtmlAnchor
        hAnchor.HRef = "javascript: void(0);"
        hAnchor.Attributes("onclick") = mJSFilterShow
        hAnchor.InnerText = Me.Language.Keyword("Button.Cancel")
        hCell3.Controls.Add(hAnchor)

        hRow3.Cells.Add(hCell3)
        hTable3.Rows.Add(hRow3)

    End Sub

    ''' <summary>
    ''' Crea el Script per controlar el Filtre avançat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateScriptFilter()

        '' Generamos el script para guardar los filtros
        'Dim hSelectField As roComboBox
        'Dim hSelectCond As roComboBox
        'Dim hTextValue As HtmlInputText
        'Dim hOptionAND As HtmlInputRadioButton

        'Dim strAndOr As String = "AND"

        '' Obtenemos los filtros actuales
        'Dim strUFFilter As String = ""
        'If HttpContext.Current.Request.Cookies(Me.ClientID & "_Selector_UserFieldFilter") IsNot Nothing Then
        '    strUFFilter = HttpContext.Current.Request.Cookies(Me.ClientID & "_Selector_UserFieldFilter").Value
        'End If
        'If strUFFilter <> "" Then
        '    ' Mostramos filtro actual
        '    Dim Filters() As String = strUFFilter.Split(Chr(127))
        '    Dim Parts() As String
        '    For i = 0 To Filters.Length - 1

        '        If Filters(i) <> "" Then

        '            Parts = Filters(i).Split("~")

        '            ' Campo
        '            hSelect = htable.FindControl("UserFieldFilter" & i + 1)
        '            For j As Integer = 0 To hSelect.ItemsValue.Count - 1
        '                If hSelect.ItemsValue(j).ToString.ToLower = Parts(0).ToLower Then
        '                    'hSelect.Items(j).Selected = True
        '                    hSelect.SelectedValue = hSelect.ItemsValue(j)
        '                    Exit For
        '                End If
        '            Next
        '            ' Condición
        '            hSelect = hTable.FindControl("CriteriaFilter" & i + 1)
        '            For j As Integer = 0 To hSelect.ItemsValue.Count - 1
        '                If hSelect.ItemsValue(j).ToString = Parts(1) Then
        '                    'hSelect.Items(j).Selected = True
        '                    hSelect.SelectedValue = hSelect.ItemsValue(j)
        '                    Exit For
        '                End If
        '            Next
        '            ' Valor
        '            hText = hTable.FindControl("ValueFilter" & i + 1)
        '            hText.Value = Parts(2)

        '            If Parts(3) = "AND" Then
        '                CType(hTable.FindControl("OptionAND" & i + 1), HtmlInputRadioButton).Checked = True
        '            Else
        '                CType(hTable.FindControl("OptionOR" & i + 1), HtmlInputRadioButton).Checked = True
        '            End If

        '        End If

        '    Next

        'End If
        ' '' End Embbed

    End Sub

    ''' <summary>
    ''' Recupera els filtres actuals i els carrega al formulari de criteris
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub RetrieveActualFilters(ByVal oTreeState As roTreeState)
        Dim hSelect As roComboBox
        Dim hText As HtmlInputText

        '' Obtenemos los filtros actuales
        Dim strUFFilter As String = oTreeState.UserFieldFilter

        If strUFFilter <> "" Then
            strUFFilter = HttpUtility.UrlDecode(strUFFilter)

            ' Mostramos filtro actual
            Dim Filters() As String = strUFFilter.Split(Chr(127))
            Dim Parts() As String
            Dim strFieldAux As String = ""

            For i As Integer = 0 To Filters.Length - 1

                If Filters(i) <> "" Then

                    Parts = Filters(i).Split("~")

                    'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                    strFieldAux = Parts(0).Split("|")(1)
                    If strFieldAux.StartsWith("(") And strFieldAux.EndsWith(")") Then
                        Parts(0) = Parts(0).Split("|")(0) & "|" & strFieldAux.Substring(1, strFieldAux.Length - 2)
                    End If

                    ' Campo
                    'hSelect = htable.FindControl("UserFieldFilter" & i + 1)
                    hSelect = CType(Me.FindControl("UserFieldFilter" & i + 1), roComboBox)
                    For j As Integer = 0 To hSelect.ItemsValue.Count - 1
                        If hSelect.ItemsValue(j).ToString.ToLower = Parts(0).ToLower Then
                            'hSelect.Items(j).Selected = True
                            hSelect.SelectedValue = hSelect.ItemsValue(j)
                            hSelect.Value = hSelect.ItemsValue(j)
                            Exit For
                        End If
                    Next

                    ' Condición
                    'hSelect = htable.FindControl("CriteriaFilter" & i + 1)
                    hSelect = CType(Me.FindControl("CriteriaFilter" & i + 1), roComboBox)
                    For j As Integer = 0 To hSelect.ItemsValue.Count - 1
                        If hSelect.ItemsValue(j) = Parts(1) Then
                            'hSelect.Items(j).Selected = True
                            hSelect.SelectedValue = hSelect.ItemsValue(j)
                            hSelect.Value = hSelect.ItemsValue(j)
                            Exit For
                        End If
                    Next

                    ' Valor
                    hText = Me.FindControl("ValueFilter" & i + 1)
                    'Desproteger valor (problemas al codificar/decodificar) le quitamos parentesis
                    If Parts(2).StartsWith("(") And Parts(2).EndsWith(")") Then
                        hText.Value = Parts(2).Substring(1, Parts(2).Length - 2)
                    Else
                        hText.Value = Parts(2)
                    End If

                    If Parts(3) = "AND" Then
                        CType(Me.FindControl("OptionAND" & i + 1), HtmlInputRadioButton).Checked = True
                    Else
                        CType(Me.FindControl("OptionOR" & i + 1), HtmlInputRadioButton).Checked = True
                    End If

                End If

            Next

        End If

    End Sub

End Class