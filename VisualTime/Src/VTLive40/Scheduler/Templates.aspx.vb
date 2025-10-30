Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Templates
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Highlight"

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property IDTemplate() As Integer
        Get
            Dim intID As Integer = 0
            If ViewState("TemplateDetail_IDTemplate") Is Nothing OrElse Not IsNumeric(ViewState("TemplateDetail_IDTemplate")) OrElse CInt(ViewState("TemplateDetail_IDTemplate")) <= 0 Then
                If Me.cmbTemplates.ItemsValue.Count > 0 Then
                    If Not IsNumeric(Me.cmbTemplates.SelectedValue) OrElse CInt(Me.cmbTemplates.SelectedValue) <= 0 Then
                        intID = Me.cmbTemplates.ItemsValue(0)
                    Else
                        intID = Me.cmbTemplates.SelectedValue
                    End If
                Else
                    intID = 0
                End If
                Me.IDTemplate = intID
            Else
                intID = ViewState("TemplateDetail_IDTemplate")
            End If
            Return intID
        End Get
        Set(ByVal value As Integer)
            ViewState("TemplateDetail_IDTemplate") = value
            Me.cmbTemplates.SelectedValue = value
        End Set
    End Property

    Private Property NonLaboralDaysMonths(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("TemplateDetail_TemplateMonthData")

            If bolReload OrElse tb Is Nothing Then

                tb = New DataTable
                tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                tb.Columns.Add(New DataColumn("Name", GetType(String)))

                For n As Integer = 1 To 12
                    Dim oNewRow As DataRow = tb.NewRow
                    oNewRow("ID") = n
                    oNewRow("Name") = Me.Language.Keyword("month." & n.ToString)
                    tb.Rows.Add(oNewRow)
                Next

                tb.AcceptChanges()

                Session("TemplateDetail_TemplateMonthData") = tb
            End If

            Return tb
        End Get
        Set(value As DataTable)

        End Set
    End Property

    Private Property TemplateDetailData(Optional ByVal bolReload As Boolean = False) As DataView
        Get

            Dim tb As DataTable = Session("TemplateDetail_TemplateDetailData")

            If bolReload OrElse tb Is Nothing Then

                tb = New DataTable
                tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                tb.Columns.Add(New DataColumn("IDTemplate", GetType(Integer)))
                tb.Columns.Add(New DataColumn("ScheduleDate", GetType(Integer)))
                tb.Columns.Add(New DataColumn("ScheduleDateMonth", GetType(Integer)))
                tb.Columns.Add(New DataColumn("ScheduleDateSort", GetType(Date)))
                tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}

                Dim oTemplate As roScheduleTemplate = API.SchedulerServiceMethods.GetScheduleTemplate(Me, Me.IDTemplate(), True)
                If oTemplate IsNot Nothing Then
                    If oTemplate.ScheduleDates IsNot Nothing Then
                        Dim oRow As DataRow
                        Dim tmpID As Integer = 1
                        For Each oDate As roScheduleTemplateDate In oTemplate.ScheduleDates
                            oRow = tb.NewRow
                            oRow("ID") = tmpID
                            oRow("IDTemplate") = oTemplate.ID
                            oRow("ScheduleDate") = oDate.ScheduleDate.Day
                            oRow("ScheduleDateMonth") = oDate.ScheduleDate.Month
                            oRow("ScheduleDateSort") = oDate.ScheduleDate
                            tb.Rows.Add(oRow)
                            tmpID = tmpID + 1
                        Next
                    End If
                End If

                tb.AcceptChanges()

                Session("TemplateDetail_TemplateDetailData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Dim dv As New DataView(tb)
            dv.Sort = "ScheduleDateSort"

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("TemplateDetail_TemplateDetailData") = value.Table
            Else
                Session("TemplateDetail_TemplateDetailData") = Nothing
            End If

        End Set
    End Property

    Private ReadOnly Property TemplateDetailDataHasChanges() As Boolean
        Get
            Dim bolRet As Boolean = False
            Dim tb As DataTable = Me.TemplateDetailData.Table
            If tb.DataSet Is Nothing Then
                Dim ds As New DataSet
                ds.Tables.Add(tb)
            End If
            If tb IsNot Nothing AndAlso tb.DataSet IsNot Nothing AndAlso tb.DataSet.HasChanges Then
                For Each oRow As DataRow In tb.Rows
                    Select Case oRow.RowState
                        Case DataRowState.Added
                            ' Verificar que no sea una fila sin valores
                            If Not HelperWeb.EmptyRow(oRow) Then
                                bolRet = True
                                Exit For
                            End If
                        Case DataRowState.Deleted, DataRowState.Modified
                            bolRet = True
                            Exit For
                    End Select
                Next
            End If
            Return bolRet
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("rgbColor", "~/Base/Scripts/rgbcolor.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            Me.LoadTemplates()

            CreateColumnsContracts()
            If Not Me.IsPostBack Then

                Me.TemplateDetailData = Nothing

                Dim TemplateID As String = Request.Params("TemplateID")
                If TemplateID IsNot Nothing AndAlso TemplateID.Length > 0 Then
                    Me.IDTemplate = CInt(TemplateID)
                Else
                    Me.IDTemplate = 0
                End If

                'Me.LoadData()
                BindGridNonLaboralDays(True)
            Else
                Me.IDTemplate = Me.IDTemplate
                BindGridNonLaboralDays(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btClose.Click
        Me.CanClose = True
    End Sub

    Protected Sub btSaves_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSaves.Click
        Me.SaveData()
    End Sub

    'Protected Sub btUndo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btUndo.Click

    '    Me.TemplateDetailData = Nothing

    '    Me.LoadData()

    '    Me.hdnChanged.Value = "0"

    'End Sub

#End Region

#Region "Methods"

    Private Sub LoadTemplates()

        Me.cmbTemplates.ClearItems()

        Dim oTemplates As Generic.List(Of roScheduleTemplate) = API.SchedulerServiceMethods.GetScheduleTemplates(Me)
        If oTemplates IsNot Nothing Then
            For Each oTemplate As roScheduleTemplate In oTemplates
                Me.cmbTemplates.AddItem(oTemplate.Name.ToString, oTemplate.ID.ToString, "$get('" & Me.hdnIDTemplateSelected.ClientID & "').value = $get('" & cmbTemplates_Value.ClientID & "').value; ButtonClick($get('" & Me.btRefresh.ClientID & "'));")
            Next
            'If Me.ddlProfiles.Items.Count > 0 Then Me.ddlProfiles.SelectedIndex = 0
            If Me.cmbTemplates.ItemsText.Count > 0 And Me.cmbTemplates.Value = "" Then
                Me.cmbTemplates_Value.Value = Me.cmbTemplates.ItemsValue(0)
                Me.cmbTemplates.SelectedValue = Me.cmbTemplates.ItemsValue(0)
            End If

        End If

    End Sub

    'Private Function NewDetailData(ByVal tbTemplateDetail As DataTable) As DataRow

    '    Dim oNew As DataRow = tbTemplateDetail.NewRow
    '    With oNew
    '        .Item("IDTemplate") = Me.IDTemplate
    '        .Item("ScheduleDate") = DBNull.Value
    '        .Item("ScheduleDateSort") = New Date(2079, 1, 1)
    '    End With
    '    Return oNew

    'End Function

    Private Function SaveData() As Boolean

        Dim bolRet As Boolean = False

        Dim oTemplate As roScheduleTemplate = API.SchedulerServiceMethods.GetScheduleTemplate(Me, Me.IDTemplate(), False)

        Dim oScheduleDates As New Generic.List(Of roScheduleTemplateDate)

        Dim oRow As DataRowView
        For n As Integer = 0 To Me.TemplateDetailData.Count - 1
            oRow = Me.TemplateDetailData.Item(n)
            If oRow.Row.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("ScheduleDateSort")) Then
                    Dim oSchedulerTemplateDate As New roScheduleTemplateDate
                    oSchedulerTemplateDate.ScheduleDate = CDate(oRow("ScheduleDateSort"))
                    oSchedulerTemplateDate.Description = ""
                    oScheduleDates.Add(oSchedulerTemplateDate)
                End If
            End If
        Next

        oTemplate.ScheduleDates = oScheduleDates

        If API.SchedulerServiceMethods.SaveScheduleTemplate(Me, oTemplate, True, WLHelperWeb.CurrentPassport.ID) Then
            bolRet = True
            Me.hdnChanged.Value = "0"
            Me.MustRefresh = "1"
            Me.TemplateDetailData.Table.AcceptChanges()
            Me.CanClose = True
        Else

        End If

        Return bolRet

    End Function

#End Region

#Region "Grid Contracts"

    Private Sub BindGridNonLaboralDays(ByVal bolReload As Boolean)
        Me.GridNonLaboralDays.DataSource = Me.TemplateDetailData(bolReload)
        Me.GridNonLaboralDays.DataBind()

        Try
            If Not GridNonLaboralDays.JSProperties.ContainsKey("cpActionRO") Then GridNonLaboralDays.JSProperties.Add("cpActionRO", "")
        Catch ex As Exception
            GridNonLaboralDays.JSProperties("cpActionRO") = ""
        End Try

    End Sub

    Private Sub CreateColumnsContracts()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.GridNonLaboralDays.Columns.Clear()
        Me.GridNonLaboralDays.KeyFieldName = "ID"
        Me.GridNonLaboralDays.SettingsText.EmptyDataRow = " "
        Me.GridNonLaboralDays.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridNonLaboralDays.SettingsEditing.Mode = GridViewEditingMode.Inline

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = True

            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = True

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 16
            VisibleIndex = VisibleIndex + 1
            Me.GridNonLaboralDays.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridNonLaboralDays.Columns.Add(GridColumn)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDTemplate"
        GridColumn.FieldName = "IDTemplate"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridNonLaboralDays.Columns.Add(GridColumn)

        'Clave
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = "ScheduleDateSort"
        GridColumnDate.FieldName = "ScheduleDateSort"
        GridColumnDate.ReadOnly = False
        GridColumnDate.Visible = False
        Me.GridNonLaboralDays.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridNonLaboralDays.Column.ScheduleDate", DefaultScope) '"Valor"
        GridColumn.FieldName = "ScheduleDate"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridNonLaboralDays.Columns.Add(GridColumn)

        'Date
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridNonLaboralDays.Column.ScheduleDateMonth", DefaultScope) '"Fecha"
        GridColumnCombo.FieldName = "ScheduleDateMonth"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = Me.NonLaboralDaysMonths
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 35
        Me.GridNonLaboralDays.Columns.Add(GridColumnCombo)

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = False

            GridColumnCommand.ShowCancelButton = True
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            VisibleIndex = VisibleIndex + 1

            Me.GridNonLaboralDays.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Private Sub GridNonLaboralDays_DataBinding(sender As Object, e As EventArgs) Handles GridNonLaboralDays.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridNonLaboralDays.Columns("ScheduleDateMonth")
        oCombo.PropertiesComboBox.DataSource = Me.NonLaboralDaysMonths
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Protected Sub GridNonLaboralDays_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridNonLaboralDays.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridNonLaboralDays(False)

            GridNonLaboralDays.JSProperties("cpActionRO") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridNonLaboralDays(True)

            GridNonLaboralDays.JSProperties("cpActionRO") = "RELOAD"
        End If
    End Sub

    Protected Sub GridNonLaboralDays_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridNonLaboralDays.StartRowEditing

    End Sub

    Protected Sub GridNonLaboralDays_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridNonLaboralDays.InitNewRow
        Dim tb As DataTable = Me.TemplateDetailData().Table
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If

        e.NewValues("IDTemplate") = Me.IDTemplate()
        e.NewValues("ScheduleDate") = 1
        e.NewValues("ScheduleDateMonth") = 1
        e.NewValues("ScheduleDateSort") = New Date(2000, 1, 1)

        GridNonLaboralDays.JSProperties("cpActionRO") = "INITROW"
    End Sub

    Protected Sub GridNonLaboralDays_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridNonLaboralDays.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.TemplateDetailData().Table
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("IDTemplate") = Me.IDTemplate()
                .Item("ScheduleDate") = roTypes.Any2String(e.NewValues("ScheduleDate")).Trim
                .Item("ScheduleDateMonth") = e.NewValues("ScheduleDateMonth")
                .Item("ScheduleDateSort") = New DateTime(2000, e.NewValues("ScheduleDateMonth"), e.NewValues("ScheduleDate"), 0, 0, 0)
            End With

            tb.Rows.Add(oNewRow)

            Me.TemplateDetailData = tb.AsDataView
            e.Cancel = True
            grid.CancelEdit()
        End If

        GridNonLaboralDays.JSProperties("cpActionRO") = "ROWINSERTING"
    End Sub

    Protected Sub GridContracts_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridNonLaboralDays.RowUpdating

        Dim tb As DataTable = Me.TemplateDetailData().Table
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridNonLaboralDays.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "ScheduleDate"
                    dr.Item("ScheduleDate") = roTypes.Any2String(enumerator.Value).Trim
                Case "ScheduleDateMonth"
                    dr.Item("ScheduleDateMonth") = enumerator.Value
            End Select

        End While
        dr.Item("ScheduleDateSort") = New DateTime(2000, dr.Item("ScheduleDateMonth"), dr.Item("ScheduleDate"), 0, 0, 0)

        Me.TemplateDetailData = tb.AsDataView
        e.Cancel = True
        grid.CancelEdit()

        GridNonLaboralDays.JSProperties("cpActionRO") = "ROWUPDATING"
    End Sub

    Protected Sub GridNonLaboralDays_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridNonLaboralDays.CustomErrorText
        e.ErrorText = roWsUserManagement.SessionObject.States.ContractState.ErrorText
    End Sub

    Protected Sub GridNonLaboralDays_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridNonLaboralDays.RowDeleting
        Dim tb As DataTable = Me.TemplateDetailData().Table

        Try
            Dim dr As DataRow = tb.Rows.Find(e.Keys(GridNonLaboralDays.KeyFieldName))
            dr.Delete()
            tb.AcceptChanges()
            Me.TemplateDetailData = tb.AsDataView
            e.Cancel = True
        Catch ex As Exception

        End Try

        GridNonLaboralDays.JSProperties("cpActionRO") = "DELETE"
    End Sub

#End Region

End Class