Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class RemarksConfig
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Highlight"

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property RemarksData(Optional ByVal bolReload As Boolean = False) As DataView
        Get

            Dim tb As DataTable = Session("Remarks_RemarksData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.SchedulerServiceMethods.GetSchedulerRemarksConfigDataTable(Me, WLHelperWeb.CurrentPassport.ID)

                ViewState("Remarks_RemarksData") = tb
            End If

            Dim dv As New DataView(tb)
            dv.Sort = "Order"

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("Remarks_RemarksData") = value.Table
            Else
                Session("Remarks_RemarksData") = Nothing
            End If

        End Set
    End Property

    Private Property CompareData() As DataView
        Get

            Dim tbCauses As DataTable = Session("Remarks_CompareData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb = New DataTable
                tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                tb.Columns.Add(New DataColumn("Name", GetType(String)))

                For Each CompareItem As Robotics.Base.DTOs.RemarkCompare In System.Enum.GetValues(GetType(Robotics.Base.DTOs.RemarkCompare))
                    Dim oNewRow As DataRow = tb.NewRow
                    oNewRow("ID") = CInt(CompareItem)
                    oNewRow("Name") = Me.Language.Translate("CompareList." & System.Enum.GetName(GetType(Robotics.Base.DTOs.RemarkCompare), CompareItem) & ".Caption", Me.DefaultScope)
                    tb.Rows.Add(oNewRow)
                Next

                tb.AcceptChanges()

                dv = New DataView(tb)
                dv.Sort = "Name ASC"

                Session("Remarks_CompareData") = dv.Table
            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("Remarks_CompareData") = value.Table
            Else
                Session("Remarks_CompareData") = Nothing
            End If
        End Set
    End Property

    Private Property CausesData() As DataView
        Get

            Dim tbCauses As DataTable = Session("Remarks_CausesData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb As DataTable = CausesServiceMethods.GetCauses(Me)

                If tb IsNot Nothing Then

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"

                    Session("Remarks_CausesData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("Remarks_CausesData") = value.Table
            Else
                Session("Remarks_CausesData") = Nothing
            End If
        End Set
    End Property

    Private Property CalendarMode As String
        Get
            Dim oAdvParam As String = ViewState("Remarks_CalendarMode")

            If (oAdvParam Is Nothing) Then
                oAdvParam = HelperSession.AdvancedParametersCache("CalendarMode")
            End If

            ViewState("Remarks_CalendarMode") = oAdvParam
            Return oAdvParam
        End Get
        Set(ByVal value As String)
            If value IsNot Nothing Then
                ViewState("Remarks_CalendarMode") = value
            Else
                ViewState("Remarks_CausesData") = Nothing
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("rgbcolor", "~/Base/Scripts/rgbcolor.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            Me.btnAddRemark.Visible = (Me.oPermission >= Permission.Write)
            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            CreateColumnsRemarks()

            If Not Me.IsPostBack Then

                Me.RemarksData = Nothing
                Me.CausesData = Nothing
                Me.CompareData = Nothing

                BindGridRemarks(True)
            Else
                BindGridRemarks(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click
        Me.SaveData()
    End Sub

#End Region

#Region "Methods"

    Private Sub SaveData()

        Dim oRemarks As New roSchedulerRemarks

        oRemarks.IDPassport = WLHelperWeb.CurrentPassport.ID

        Dim tmpRemarks As New Generic.List(Of Robotics.Base.VTBusiness.Scheduler.roCalendarRemark)
        For Each oRow As DataRow In Me.RemarksData.ToTable.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("IDCause")) Then
                    Dim oRemark As New Robotics.Base.VTBusiness.Scheduler.roCalendarRemark
                    oRemark.IDCause = oRow("IDCause")
                    oRemark.Compare = oRow("Compare")
                    oRemark.Value = oRow("Value")
                    oRemark.Color = oRow("Color")
                    tmpRemarks.Add(oRemark)
                End If
            End If
        Next

        oRemarks.Remarks = tmpRemarks
        If API.SchedulerServiceMethods.SaveSchedulerRemarksConfig(Me, oRemarks) Then

            WLHelperWeb.Context(Nothing, oRemarks.IDPassport, True)  '<-forzar a recuperar el contexto de la BBDD despues de grabar

            Me.CanClose = True
            Me.MustRefresh = "1"
        Else

        End If
    End Sub

#End Region

#Region "Grid Contracts"

    Private Sub BindGridRemarks(ByVal bolReload As Boolean)
        Me.GridRemarks.DataSource = Me.RemarksData(bolReload)
        Me.GridRemarks.DataBind()

        Try
            If Not GridRemarks.JSProperties.ContainsKey("cpActionRO") Then GridRemarks.JSProperties.Add("cpActionRO", "")
        Catch ex As Exception
            GridRemarks.JSProperties("cpActionRO") = ""
        End Try

    End Sub

    Private Sub CreateColumnsRemarks()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnColor As GridViewDataColorEditColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.GridRemarks.Columns.Clear()
        Me.GridRemarks.KeyFieldName = "ID"
        Me.GridRemarks.SettingsText.EmptyDataRow = " "
        Me.GridRemarks.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridRemarks.SettingsEditing.Mode = GridViewEditingMode.Inline

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
            Me.GridRemarks.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridRemarks.Columns.Add(GridColumn)

        'Date
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridRemarks.Column.IDCause", DefaultScope) '"Fecha"
        GridColumnCombo.FieldName = "IDCause"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = Me.CausesData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 40
        Me.GridRemarks.Columns.Add(GridColumnCombo)

        'Date
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridRemarks.Column.Compare", DefaultScope) '"Fecha"
        GridColumnCombo.FieldName = "Compare"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = Me.CompareData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.RequireDataBinding()

        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 15
        Me.GridRemarks.Columns.Add(GridColumnCombo)

        'Valor Hora Editable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.ValueHoraEditable", DefaultScope) '"Valor"
        GridColumn.FieldName = "ValueEditable"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 20
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999..9999>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 60
        Me.GridRemarks.Columns.Add(GridColumn)

        'Date
        GridColumnColor = New GridViewDataColorEditColumn()
        GridColumnColor.Caption = Me.Language.Translate("GridRemarks.Column.Color", DefaultScope) '"Fecha"
        GridColumnColor.FieldName = "ColorEdit"
        GridColumnColor.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnColor.ReadOnly = False
        GridColumnColor.PropertiesColorEdit.EnableCustomColors = True
        GridColumnColor.UnboundType = DevExpress.Data.UnboundColumnType.Object
        GridColumnColor.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnColor.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnColor.Width = 20
        Me.GridRemarks.Columns.Add(GridColumnColor)

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            Dim showButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            showButton.ID = "MoveUPButton"
            showButton.Image.Url = "~/Base/Images/contexticons/stock_up.png"
            showButton.Text = Me.Language.Translate("GridRemarks.Column.UP", DefaultScope) 'Mostrar detalles"

            Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
            employeeButton.ID = "MoveDownButton"
            employeeButton.Image.Url = "~/Base/Images/contexticons/stock_down.png"
            employeeButton.Text = Me.Language.Translate("GridRemarks.Column.DOWN", DefaultScope) 'Ficha empleado"

            GridColumnCommand.CustomButtons.Add(employeeButton)
            GridColumnCommand.CustomButtons.Add(showButton)
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 20
            VisibleIndex = VisibleIndex + 1

            Me.GridRemarks.Columns.Add(GridColumnCommand)

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

            Me.GridRemarks.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Protected Sub gridRunning_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridRemarks.CustomUnboundColumnData

        Select Case e.Column.FieldName
            Case "ValueEditable"
                If e.IsGetData Then
                    e.Value = roTypes.Any2DateTime(e.GetListSourceFieldValue("Value")).ToShortTimeString()
                End If
            Case "ColorEdit"
                If e.IsGetData Then
                    e.Value = Drawing.ColorTranslator.ToHtml(Drawing.ColorTranslator.FromWin32(roTypes.Any2Integer(e.GetListSourceFieldValue("Color"))))
                End If
        End Select

        'If Not e.IsGetData Then Return
        'If e.Column.FieldName = "ValueEditable" Then
        '    If Not e.GetListSourceFieldValue("Value") Is Nothing Then
        '        e.Value = roTypes.Any2DateTime(e.GetListSourceFieldValue("Value")).ToShortTimeString()
        '    End If
        'ElseIf e.Column.FieldName = "ColorEdit" Then
        '    If Not e.GetListSourceFieldValue("Color") Is Nothing Then

        '    End If
        'End If
    End Sub

    Private Sub GridRemarks_DataBinding(sender As Object, e As EventArgs) Handles GridRemarks.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridRemarks.Columns("IDCause")
        oCombo.PropertiesComboBox.DataSource = Me.CausesData
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)

        oCombo = GridRemarks.Columns("Compare")
        oCombo.PropertiesComboBox.DataSource = Me.CompareData
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Protected Sub GridRemarks_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridRemarks.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridRemarks(False)

            GridRemarks.JSProperties("cpActionRO") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridRemarks(True)

            GridRemarks.JSProperties("cpActionRO") = "RELOAD"
        End If
    End Sub

    Protected Sub GridRemarks_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridRemarks.StartRowEditing

    End Sub

    Protected Sub GridRemarks_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridRemarks.InitNewRow
        Dim tb As DataTable = Me.RemarksData.Table
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            e.NewValues("Order") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("Order")) + 1
        Else
            e.NewValues("ID") = 1
            e.NewValues("Order") = 1
        End If

        e.NewValues("IDCause") = 0
        e.NewValues("Compare") = 0
        e.NewValues("Value") = New DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0)
        e.NewValues("Color") = 0

        GridRemarks.JSProperties("cpActionRO") = "INITROW"
    End Sub

    Protected Sub GridRemarks_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridRemarks.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.RemarksData.Table
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("IDCause") = e.NewValues("IDCause")
                .Item("Compare") = e.NewValues("Compare")
                .Item("Value") = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") & " " & e.NewValues("ValueEditable"), Nothing)
                .Item("Color") = Drawing.ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml(e.NewValues("ColorEdit")))
                If (tb.Rows.Count > 0) Then
                    .Item("Order") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("Order")) + 1
                Else
                    .Item("Order") = 1
                End If

            End With

            tb.Rows.Add(oNewRow)

            Me.RemarksData = tb.AsDataView
            e.Cancel = True
            grid.CancelEdit()
        End If

        GridRemarks.JSProperties("cpActionRO") = "ROWINSERTING"
    End Sub

    Protected Sub GridContracts_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridRemarks.RowUpdating

        Dim tb As DataTable = Me.RemarksData.Table
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridRemarks.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDCause"
                    dr.Item("IDCause") = roTypes.Any2Integer(enumerator.Value)
                Case "Compare"
                    dr.Item("Compare") = roTypes.Any2Integer(enumerator.Value)
                Case "ValueEditable"
                    dr.Item("Value") = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy") & " " & enumerator.Value, Nothing)
                Case "ColorEdit"
                    dr.Item("Color") = Drawing.ColorTranslator.ToWin32(Drawing.ColorTranslator.FromHtml(enumerator.Value))
            End Select

        End While

        'Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(dr.Item("Color"))
        'Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)

        Me.RemarksData = tb.AsDataView
        e.Cancel = True
        grid.CancelEdit()

        GridRemarks.JSProperties("cpActionRO") = "ROWUPDATING"
    End Sub

    Protected Sub GridRemarks_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridRemarks.CustomErrorText
        e.ErrorText = roWsUserManagement.SessionObject.States.ContractState.ErrorText
    End Sub

    Protected Sub GridRemarks_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridRemarks.RowDeleting
        Dim tb As DataTable = Me.RemarksData.Table

        Try
            Dim dr As DataRow = tb.Rows.Find(e.Keys(GridRemarks.KeyFieldName))
            dr.Delete()
            tb.AcceptChanges()
            Me.RemarksData = tb.AsDataView
            e.Cancel = True
        Catch ex As Exception

        End Try

        GridRemarks.JSProperties("cpActionRO") = "DELETE"
    End Sub

    Protected Sub gv_CustomCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomCallbackEventArgs) Handles GridRemarks.CustomCallback
        If e.Parameters = "MoveUPButton" OrElse e.Parameters = "MoveDownButton" Then
            Dim gridView As ASPxGridView = CType(sender, ASPxGridView)
            Dim index As Integer = gridView.FocusedRowIndex
            If (e.Parameters = "MoveUPButton" AndAlso index <= 0) OrElse (e.Parameters = "MoveDownButton" AndAlso index >= gridView.VisibleRowCount - 1) Then
                Return
            End If
            Dim id As Integer = CInt(Fix(gridView.GetRowValues(index, "ID")))
            Dim idPos As Integer = CInt(Fix(gridView.GetRowValues(index, "Order")))

            Dim nextIndex As Integer = If(e.Parameters = "MoveUPButton", index - 1, index + 1)
            Dim nextId As Integer = CInt(Fix(gridView.GetRowValues(nextIndex, "ID")))
            Dim nextIdPos As Integer = CInt(Fix(gridView.GetRowValues(nextIndex, "Order")))

            Dim tb As DataTable = Me.RemarksData().Table

            Dim dataRow As DataRow = tb.Rows.Find(id)
            Dim nextDataRow As DataRow = tb.Rows.Find(nextId)

            nextDataRow("Order") = idPos
            dataRow("Order") = nextIdPos

            Me.RemarksData = tb.AsDataView

            GridRemarks.JSProperties("cpActionRO") = e.Parameters
        End If

    End Sub

#End Region

End Class