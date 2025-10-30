Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class CardsCorrector
    Inherits PageBase

#Region "Properties"

    Private Property EntriesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("Entries_EntriesData")

            If bolReload OrElse tb Is Nothing Then
                tb = API.PunchServiceMethods.GetInvalidPunches(Me)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("Entries_EntriesData") = tb
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("Entries_EntriesData") = value
        End Set
    End Property

    Private Function GetJustCausesData() As DataView
        Dim dv As DataView = Nothing
        Dim tb As DataTable = CausesServiceMethods.GetCauses(Me.Page, "")
        If tb IsNot Nothing Then
            dv = New DataView(tb)
            dv.Sort = "Name ASC"
        End If
        Return dv
    End Function

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.IdentifyMethods", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If
        If Not Me.IsPostBack Then
            CreateColumnsEntries()
            Me.EntriesData = Nothing
            BindGridEntries(True)
        Else
            BindGridEntries(False)
        End If

    End Sub

    Private Sub BindGridEntries(ByVal bolReload As Boolean)
        Me.GridEntries.DataSource = Me.EntriesData(bolReload)
        Me.GridEntries.DataBind()
    End Sub

    Private Sub CreateColumnsEntries()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim CustomButton As GridViewCommandColumnCustomButton

        Dim VisibleIndex As Integer = 0

        Me.GridEntries.Settings.ShowTitlePanel = True
        Me.GridEntries.Columns.Clear()
        Me.GridEntries.KeyFieldName = "ID"
        Me.GridEntries.SettingsText.EmptyDataRow = " "
        Me.GridEntries.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridEntries.SettingsCommandButton.DeleteButton.Image.ToolTip = " "
        Me.GridEntries.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridEntries.SettingsCommandButton.EditButton.Image.ToolTip = " "
        Me.GridEntries.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridEntries.SettingsCommandButton.UpdateButton.Image.ToolTip = " "
        Me.GridEntries.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridEntries.SettingsCommandButton.CancelButton.Image.ToolTip = " "
        Me.GridEntries.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridEntries.SettingsEditing.Mode = GridViewEditingMode.Inline
        Me.GridEntries.SettingsPager.PageSize = 20

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridEntries.Columns.Add(GridColumn)

        'Value
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridEntries.Column.IDCredential", DefaultScope) '"Valor"
        GridColumn.FieldName = "IDCredentialStr"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 43
        Me.GridEntries.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridEntries.Column.DateTime", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "DateTime"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 30
        Me.GridEntries.Columns.Add(GridColumnDate)

        'Hora
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridEntries.Column.Time", DefaultScope)
        GridColumn.FieldName = "Hour"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        'GridColumn.SortIndex = 1
        'GridColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        GridColumn.Width = 30
        Me.GridEntries.Columns.Add(GridColumn)

        'IDTerminal
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridEntries.Column.IDTerminal", DefaultScope) '"Valor"
        GridColumn.FieldName = "IDTerminal"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 20
        Me.GridEntries.Columns.Add(GridColumn)

        'Tipo
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridEntries.Column.Type", DefaultScope) '"Valor"
        GridColumn.FieldName = "Type"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 20
        Me.GridEntries.Columns.Add(GridColumn)

        ''Cause
        'GridColumnCombo = New GridViewDataComboBoxColumn
        'GridColumnCombo.Caption = Me.Language.Translate("GridEntries.Column.TypeData", DefaultScope) '"Valor"
        'GridColumnCombo.FieldName = "TypeData"
        'GridColumnCombo.VisibleIndex = VisibleIndex
        'GridColumnCombo.PropertiesComboBox.TextField = "Name"
        'GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        'GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        'GridColumnCombo.PropertiesComboBox.DataSource = Me.GetJustCausesData()
        'VisibleIndex = VisibleIndex + 1
        'GridColumnCombo.ReadOnly = False
        'GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumnCombo.Width = 43
        'Me.GridEntries.Columns.Add(GridColumnCombo)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 36
        VisibleIndex = VisibleIndex + 1

        'CustomButton = New GridViewCommandColumnCustomButton()
        'CustomButton.ID = "DeleteEntrieRow"
        'CustomButton.Image.Url = "~/Base/Images/Grid/remove.png"
        'CustomButton.Image.ToolTip = Me.Language.Translate("GridDocumentsTracking.Column.DeleteRow", DefaultScope)
        'CustomButton.Image.Height = New Unit(16)
        'CustomButton.Image.Width = New Unit(16)
        'GridColumnCommand.CustomButtons.Add(CustomButton)

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "AssignCardRow"
        CustomButton.Image.Url = "~/Employees/Images/Empleado-16x16.gif"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridDocumentsTracking.Column.AssignCard", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        Me.GridEntries.Columns.Add(GridColumnCommand)

    End Sub

    Protected Sub GridEntries_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridEntries.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "Hour"
                If Not IsDBNull(e.GetListSourceFieldValue("DateTime")) Then
                    e.Value = CType(e.GetListSourceFieldValue("DateTime"), Date).ToString("HH:mm:ss")
                End If
            Case "IDCredentialStr"
                If Not IsDBNull(e.GetListSourceFieldValue("IDCredential")) Then
                    e.Value = CType(e.GetListSourceFieldValue("IDCredential"), String)
                End If
        End Select

    End Sub

    Protected Sub GridEntries_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridEntries.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridEntries(False)
        ElseIf e.Parameters = "RELOAD" Then
            BindGridEntries(True)
        End If
    End Sub

    Protected Sub GridEntries_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridEntries.StartRowEditing

    End Sub

    Protected Sub GridEntries_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridEntries.RowUpdating

        Dim tb As DataTable = Me.EntriesData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridEntries.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDCredential"
                    dr.Item("IDCredential") = enumerator.Value
            End Select

        End While

        Me.EntriesData = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridEntries_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridEntries.RowDeleting
        Dim tb As DataTable = Me.EntriesData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridEntries.KeyFieldName))
        dr.Delete()
        Me.EntriesData = tb
        e.Cancel = True
    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        ' Verificamos que no estemos en edición
        If Not Me.GridEntries.IsEditing Then

            ' Marcamos las filas cómo inválidas y borramos las posibles filas vacías
            Dim RemoveRowsIndex As New ArrayList
            Dim oRow As DataRow
            For n As Integer = 0 To Me.EntriesData.Rows.Count - 1
                oRow = Me.EntriesData.Rows(n)
                If oRow.RowState <> DataRowState.Deleted Then
                    If Not HelperWeb.EmptyRow(oRow) Then
                        'oRow("InvalidRead") = False
                    Else
                        RemoveRowsIndex.Add(n)
                    End If
                End If
            Next
            For Each n As Integer In RemoveRowsIndex
                Me.EntriesData.Rows.RemoveAt(n)
            Next

            ' Grabar cambios en tabla 'Punches'
            Dim bolSaved As Boolean = API.PunchServiceMethods.SaveInvalidPunches(Me, Me.EntriesData)

            Me.CanClose = bolSaved

        End If

    End Sub

#End Region

End Class