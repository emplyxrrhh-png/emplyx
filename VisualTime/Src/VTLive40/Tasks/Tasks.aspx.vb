Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Tasks_Tasks
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"
    Private oPermission As Permission
    Private Const FeatureEmployees As String = ""
    Private bolCentesimalFormatProductiV As Boolean = False
    Public ReportDefaultVersion As Integer = 1
    Public DefaultRootURL As String = String.Empty

    <Runtime.Serialization.DataContract()>
    Private Class TaskStatistics

        <Runtime.Serialization.DataMember(Name:="idTask")>
        Public IdTask As Integer

        <Runtime.Serialization.DataMember(Name:="idView")>
        Public TypeView As TaskStatisticsViewEnum

        <Runtime.Serialization.DataMember(Name:="idGroup")>
        Public TypeGroup As TaskStatisticsGroupByEnum

        <Runtime.Serialization.DataMember(Name:="beginDate")>
        Public BeginDate As String

        <Runtime.Serialization.DataMember(Name:="endDate")>
        Public EndDate As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class TaskDetails

        <Runtime.Serialization.DataMember(Name:="idTask")>
        Public IdTask As Integer

        <Runtime.Serialization.DataMember(Name:="timeWorked")>
        Public TimeWorked As Double

        <Runtime.Serialization.DataMember(Name:="idView")>
        Public TypeView As TaskStatisticsViewEnum

        <Runtime.Serialization.DataMember(Name:="idGroup")>
        Public TypeGroup As TaskStatisticsGroupByEnum

        <Runtime.Serialization.DataMember(Name:="beginDate")>
        Public BeginDate As String

        <Runtime.Serialization.DataMember(Name:="endDate")>
        Public EndDate As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="gridAlerts")>
        Public gridAlerts As AlertDetails()

        <Runtime.Serialization.DataMember(Name:="gridAssigments")>
        Public gridAssigments As AssigmentDetails()

        <Runtime.Serialization.DataMember(Name:="gridFields")>
        Public gridFields As FieldDetails()

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AlertDetails

        <Runtime.Serialization.DataMember(Name:="id")>
        Public id As String

        <Runtime.Serialization.DataMember(Name:="employeename")>
        Public EmployeeName As String

        <Runtime.Serialization.DataMember(Name:="idemployee")>
        Public EmployeeID As String

        <Runtime.Serialization.DataMember(Name:="datetime")>
        Public DateTime As String

        <Runtime.Serialization.DataMember(Name:="comment")>
        Public Comment As String

        <Runtime.Serialization.DataMember(Name:="isreaded")>
        Public IsReaded As String

        <Runtime.Serialization.DataMember(Name:="readed")>
        Public Readed As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AssigmentDetails

        <Runtime.Serialization.DataMember(Name:="assignmentname")>
        Public AssignmentName As String

        <Runtime.Serialization.DataMember(Name:="idassignment")>
        Public IDAssignment As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class FieldDetails

        <Runtime.Serialization.DataMember(Name:="idfield")>
        Public idfield As String

        <Runtime.Serialization.DataMember(Name:="fieldname")>
        Public fieldname As String

        <Runtime.Serialization.DataMember(Name:="value")>
        Public value As String

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")

        Me.InsertExtraJavascript("Task", "~/Tasks/Scripts/Task.js")
        'Me.InsertExtraJavascript("TaskData", "~/Tasks/Scripts/TaskData.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        ' Obtenemos el formato de los campos de tiempo
        bolCentesimalFormatProductiV = HelperSession.AdvancedParametersCache("CentesimalFormatProductiV").Equals("1")

        ' Formateamos los campos de tiempo a HH:MM o centesimal en funcion del parametro avanzado
        SetFieldsFormat()

        Me.ReportDefaultVersion = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.DefaultReportsVersions"))
        Me.DefaultRootURL = Configuration.RootUrl

        oPermission = Me.GetFeaturePermission(FeatureAlias)
        If oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If oPermission <= Permission.Read Then
            Me.hdnModeEdit.Value = "false"
        Else
            Me.hdnModeEdit.Value = "true"
        End If

        If Not IsPostBack AndAlso Not IsCallback Then
            CreateColumns()

            'Inicializamos el selector de empleados
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpDetailTask")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpDetailTaskGrid")

            LoadTaskDataTab()

            'CreateColorPickerTable()
            LoadCombos()
            If oPermission >= Permission.Write Then
                Me.lblEmpSelect.Attributes.Add("OnClick", "javascript: TaskDetailShowSelector();")
                Me.ASPxbtnSetActive.Attributes.Add("OnClick", "javascript: hasChanges(true);")
                Me.ASPxbtnSetCanceled.Attributes.Add("OnClick", "javascript: hasChanges(true);")
                Me.ASPxbtnSetEnded.Attributes.Add("OnClick", "javascript: hasChanges(true);")
            End If

            ASPxButton2.ToolTip = Me.Language.Translate("CaptionGrid.BackToGrid", Me.DefaultScope)

            If Not Request.QueryString("IDTask") Is Nothing Then
                Me.IDLoadTask.Value = Request.QueryString("IDTask")
            End If

        End If

    End Sub

    Private Sub SetFieldsFormat()
        '
        ' Formateamos los campos de tipo tiempo
        '
        txtTimeChangedRequirements.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtEmployeeTime.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtMaterialTime.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtForecastErrorTime.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtTeamTime.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtOtherTime.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"
        txtNonProductiveTimeIncidence.ClientSideEvents.TextChanged = "function(s,e){ refreshTotalDesviations(" & IIf(bolCentesimalFormatProductiV, "1", "0") & ",'" & HelperWeb.GetDecimalDigitFormat() & "');hasChanges(true);}"

        If bolCentesimalFormatProductiV Then
            Me.txtInitialTime.MaxLength = 9
            Me.txtInitialTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtInitialTime.MaskSettings.Mask = "<0..9999>.<0000..9999>"

            Me.txtTimeChangedRequirements.MaxLength = 10
            Me.txtTimeChangedRequirements.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtTimeChangedRequirements.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtEmployeeTime.MaxLength = 10
            Me.txtEmployeeTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtEmployeeTime.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtMaterialTime.MaxLength = 10
            Me.txtMaterialTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtMaterialTime.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtForecastErrorTime.MaxLength = 10
            Me.txtForecastErrorTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtForecastErrorTime.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtTeamTime.MaxLength = 10
            Me.txtTeamTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtTeamTime.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtOtherTime.MaxLength = 10
            Me.txtOtherTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtOtherTime.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtNonProductiveTimeIncidence.MaxLength = 10
            Me.txtNonProductiveTimeIncidence.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtNonProductiveTimeIncidence.MaskSettings.Mask = "<-9999..9999>.<0000..9999>"

            Me.txtTotalDesviationHours.MaxLength = 11
            Me.txtTotalDesviationHours.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
            Me.txtTotalDesviationHours.MaskSettings.Mask = "<-99999..99999>.<0000..9999>"
        Else
            Me.txtInitialTime.MaxLength = 7
            Me.txtInitialTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtInitialTime.MaskSettings.Mask = "<0..9999>:<00..59>"

            Me.txtTimeChangedRequirements.MaxLength = 8
            Me.txtTimeChangedRequirements.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtTimeChangedRequirements.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtEmployeeTime.MaxLength = 8
            Me.txtEmployeeTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtEmployeeTime.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtMaterialTime.MaxLength = 8
            Me.txtMaterialTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtMaterialTime.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtForecastErrorTime.MaxLength = 8
            Me.txtForecastErrorTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtForecastErrorTime.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtTeamTime.MaxLength = 8
            Me.txtTeamTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtTeamTime.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtOtherTime.MaxLength = 8
            Me.txtOtherTime.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtOtherTime.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtNonProductiveTimeIncidence.MaxLength = 8
            Me.txtNonProductiveTimeIncidence.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtNonProductiveTimeIncidence.MaskSettings.Mask = "<-9999..9999>:<00..59>"

            Me.txtTotalDesviationHours.MaxLength = 9
            Me.txtTotalDesviationHours.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.All
            Me.txtTotalDesviationHours.MaskSettings.Mask = "<-99999..99999>:<00..59>"

        End If

    End Sub

    Private Sub LoadCombos()

        'Me.cmbView.ClearItems()
        Me.cmbView.Items.Clear()
        Me.cmbView.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskView.Time", DefaultScope), "0"))
        Me.cmbView.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskView.Desvios", DefaultScope), "1"))
        Me.cmbView.SelectedIndex = 0

        'Me.cmbView.AddItem(Me.Language.Translate("TaskView.Time", DefaultScope), "0", "UpdateStatisticsView(0);")
        'Me.cmbView.AddItem(Me.Language.Translate("TaskView.Desvios", DefaultScope), "1", "UpdateStatisticsView(1);")
        'Me.cmbView.SelectedIndex = 0

        Me.cmbGroupBy.Items.Clear()
        Me.cmbGroupBy.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskGroupby.Employee", DefaultScope), "0"))
        Me.cmbGroupBy.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskGroupby.Date", DefaultScope), "1"))
        Me.cmbGroupBy.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskGroupby.Group", DefaultScope), "2"))
        Me.cmbGroupBy.SelectedIndex = 0

        'Me.cmbGroupBy.ClearItems()
        'Me.cmbGroupBy.AddItem(Me.Language.Translate("TaskGroupby.Employee", DefaultScope), "0", "UpdateStatisticsGroupBy(0);")
        'Me.cmbGroupBy.AddItem(Me.Language.Translate("TaskGroupby.Date", DefaultScope), "1", "UpdateStatisticsGroupBy(1);")
        'Me.cmbGroupBy.AddItem(Me.Language.Translate("TaskGroupby.Group", DefaultScope), "2", "UpdateStatisticsGroupBy(2);")
        'Me.cmbGroupBy.SelectedIndex = 0

        Me.txtGroupByDateInf.Date = DateTime.Today.AddDays(-6)
        Me.txtGroupByDateSup.Date = DateTime.Today

        Me.cmbGroup.Items.Clear()
        Me.cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TaskView.NoCenter", DefaultScope), "0"))

        Dim IdPassport As Integer = WLHelperWeb.CurrentPassport().ID

        Dim dTblGroups As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, IdPassport, True)
        If dTblGroups.Rows.Count > 0 Then
            For Each dRow As DataRow In dTblGroups.Rows
                If dRow("ID").ToString <> "0" Then
                    Me.cmbGroup.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID").ToString))
                End If
            Next
        End If

    End Sub

    Private Sub CreateColumns()

        Dim GridColumn As GridViewDataColumn
        Dim GridColumnImage As GridViewDataImageColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnProgress As GridViewDataProgressBarColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridTareas.TotalSummary.Clear()
        Me.GridTareas.Columns.Clear()
        Me.GridTareas.KeyFieldName = "ID"

        Me.GridTareas.SettingsResizing.ColumnResizeMode = ColumnResizeMode.NextColumn

        Me.GridTareas.Settings.ShowFilterRow = True
        Me.GridTareas.Settings.ShowFooter = True
        Me.GridTareas.Settings.UseFixedTableLayout = True

        Me.GridTareas.SettingsBehavior.AllowSelectSingleRowOnly = True
        Me.GridTareas.SettingsBehavior.AllowFocusedRow = True

        Me.GridTareas.SettingsPager.PageSize = 18
        Me.GridTareas.SettingsPager.ShowEmptyDataRows = True

        'Me.GridTareas.Settings.ShowVerticalScrollBar = True
        'Me.GridTareas.Settings.VerticalScrollableHeight = 0

        Me.GridTareas.ClientSideEvents.RowDblClick = "function(s, e) {GetTaskDetails(s,e);}"
        Me.GridTareas.ClientSideEvents.RowFocusing = "function(s, e) {UpdateActionsOnFocusing(s,e);}"

        Me.GridTareas.ClientSideEvents.Init = "function(s, e) { FocuseOnFirstRow(s,e); }"
        'Me.GridTareas.ClientSideEvents.EndCallback = "function(s, e) { FocuseOnFirstRow(s,e); }"

        'ID
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.ID", DefaultScope)
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 30
        Me.GridTareas.Columns.Add(GridColumn)

        'Status
        GridColumnImage = New DevExpress.Web.GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("TaskGrid.Column.State", DefaultScope) '"Estado"
        GridColumnImage.FieldName = "ImgStatus"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 50
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 19
        GridColumnImage.PropertiesImage.ImageWidth = 19
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridTareas.Columns.Add(GridColumnImage)

        'Alertas
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Alerts", DefaultScope)
        GridColumn.FieldName = "Alerts"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 50
        Me.GridTareas.Columns.Add(GridColumn)

        'Centro
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Center", DefaultScope) '"Centro"
        GridColumn.FieldName = "Center"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 65
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridTareas.Columns.Add(GridColumn)

        'Proyecto
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Project", DefaultScope) '"Proyecto"
        GridColumn.FieldName = "Project"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 75
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridTareas.Columns.Add(GridColumn)

        'Name
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Task", DefaultScope) '"Tarea"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 150
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridTareas.Columns.Add(GridColumn)

        'ShortName
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.ShortName", DefaultScope) '"Abrev."
        GridColumn.FieldName = "ShortName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.CellStyle.Wrap = DevExpress.Utils.DefaultBoolean.False
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'GridColumn.Width = 30
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridTareas.Columns.Add(GridColumn)

        'Duracion
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Duration", DefaultScope) ' "Duración"
        GridColumn.FieldName = "Duration"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 70
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridTareas.Columns.Add(GridColumn)

        'Worked
        GridColumn = New DevExpress.Web.GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Worked", DefaultScope) '"Trabajado"
        GridColumn.FieldName = "Worked"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 70
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        Me.GridTareas.Columns.Add(GridColumn)

        'Progress
        GridColumnProgress = New DevExpress.Web.GridViewDataProgressBarColumn
        GridColumnProgress.Caption = Me.Language.Translate("TaskGrid.Column.Progress", DefaultScope) ' "Progreso"
        GridColumnProgress.FieldName = "Progress"
        GridColumnProgress.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnProgress.ReadOnly = True
        GridColumnProgress.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnProgress.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'GridColumnProgress.Width = 90
        GridColumnProgress.PropertiesProgressBar.IndicatorStyle.BackColor = Drawing.Color.LightGray

        Me.GridTareas.Columns.Add(GridColumnProgress)

        'FechaIni y hora ini
        GridColumnDate = New DevExpress.Web.GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("TaskGrid.Column.ExpectedStartDate", DefaultScope) '"Inicio"
        GridColumnDate.FieldName = "ExpectedStartDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 75
        Me.GridTareas.Columns.Add(GridColumnDate)

        'FechaFin y hora fin
        GridColumnDate = New DevExpress.Web.GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("TaskGrid.Column.ExpectedEndDate", DefaultScope)  '"Fin"
        GridColumnDate.FieldName = "ExpectedEndDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 75
        Me.GridTareas.Columns.Add(GridColumnDate)

        'Tag
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Tag", DefaultScope) '"Tag"
        GridColumn.FieldName = "Tag"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.GridTareas.Columns.Add(GridColumn)

        'Empleados
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Employees", DefaultScope) ' "Empleados"
        GridColumn.FieldName = "Employees"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        'GridColumn.Width = 70
        Me.GridTareas.Columns.Add(GridColumn)

        'Empleados Actauales
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

        Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        employeeButton.ID = "ShowTaskEmployeeStatus"
        employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Empleado-16x16.gif"
        employeeButton.Text = Me.Language.Translate("TaskGrid.Column.ShowTaskStatus", DefaultScope) 'Empleados"

        GridColumnCommand.CustomButtons.Add(employeeButton)
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 30
        VisibleIndex = VisibleIndex + 1
        Me.GridTareas.Columns.Add(GridColumnCommand)

        'Prioridad
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.Priority", DefaultScope) '"Prio."
        GridColumn.FieldName = "Priority"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Right
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 30
        Me.GridTareas.Columns.Add(GridColumn)

        'Id Tarea Previa
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("TaskGrid.Column.ActivationTask", DefaultScope) '"Prev."
        GridColumn.FieldName = "ActivationTask"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 30
        Me.GridTareas.Columns.Add(GridColumn)

        'color
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = Me.Language.Translate("TaskGrid.Column.Color", DefaultScope) '"Color"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridTareas.Columns.Add(GridColumn)

        'ColorTaskPrev
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = "ColorTaskPrev"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridTareas.Columns.Add(GridColumn)

        'NameTaskPrev
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = "NameTaskPrev"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridTareas.Columns.Add(GridColumn)

        'NameTaskPrev
        GridColumn = New DevExpress.Web.GridViewDataTextColumn()
        GridColumn.FieldName = "Exceeded"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridTareas.Columns.Add(GridColumn)

        'TOTALES
        Dim TotalItem As DevExpress.Web.ASPxSummaryItem

        TotalItem = New DevExpress.Web.ASPxSummaryItem()
        TotalItem.FieldName = "ID"
        TotalItem.ShowInColumn = "Name"
        TotalItem.DisplayFormat = Me.Language.Translate("TaskGrid.Column.Tasks", DefaultScope) & " {0:n0}" ' "Tareas: {0:n0}"
        TotalItem.SummaryType = DevExpress.Data.SummaryItemType.Count
        Me.GridTareas.TotalSummary.Add(TotalItem)

        TotalItem = New DevExpress.Web.ASPxSummaryItem()
        TotalItem.FieldName = "Duration"
        TotalItem.ShowInColumn = "Duration"
        TotalItem.DisplayFormat = "{0:n4}"
        TotalItem.SummaryType = DevExpress.Data.SummaryItemType.Sum
        Me.GridTareas.TotalSummary.Add(TotalItem)

        TotalItem = New DevExpress.Web.ASPxSummaryItem()
        TotalItem.FieldName = "Worked"
        TotalItem.ShowInColumn = "Worked"
        TotalItem.DisplayFormat = "{0:n4}"
        TotalItem.SummaryType = DevExpress.Data.SummaryItemType.Sum
        Me.GridTareas.TotalSummary.Add(TotalItem)

    End Sub

    Protected Sub btnShowPendiente_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.ToolTip = Me.Language.Translate("btnShowPendienteTooltip", Me.DefaultScope)
        txtLabel.Image.ToolTip = Me.Language.Translate("btnShowPendienteTooltip", Me.DefaultScope)
    End Sub

    Protected Sub btnShowEnded_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.ToolTip = Me.Language.Translate("btnShowEndedTooltip", Me.DefaultScope)
        txtLabel.Image.ToolTip = Me.Language.Translate("btnShowEndedTooltip", Me.DefaultScope)
        txtLabel.Checked = True
    End Sub

    Protected Sub btnShowCancelled_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.ToolTip = Me.Language.Translate("btnShowCancelledTooltip", Me.DefaultScope)
        txtLabel.Image.ToolTip = Me.Language.Translate("btnShowCancelledTooltip", Me.DefaultScope)
        txtLabel.Checked = True
    End Sub

    Protected Sub btnShowConfirmation_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.ToolTip = Me.Language.Translate("btnShowConfirmationTooltip", Me.DefaultScope)
        txtLabel.Image.ToolTip = Me.Language.Translate("btnShowConfirmationTooltip", Me.DefaultScope)
    End Sub

    Protected Sub GridTareas_CustomColumnDisplayText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs) Handles GridTareas.CustomColumnDisplayText
        If e.Column.FieldName = "Duration" Or e.Column.FieldName = "Worked" Then
            If e.Value IsNot System.DBNull.Value And e.Value IsNot Nothing Then
                If Not bolCentesimalFormatProductiV Then
                    e.DisplayText = roConversions.ConvertHoursToTime(roTypes.Any2Double(e.Value))
                Else
                    e.DisplayText = Format(roTypes.Any2Double(e.Value), "0.0000")
                End If

            End If

        ElseIf e.Column.FieldName = "ExpectedStartDate" Or e.Column.FieldName = "ExpectedEndDate" Then
            If e.Value IsNot System.DBNull.Value And e.Value IsNot Nothing Then
                Dim AuxDate As Date = e.Value
                e.DisplayText = Globalization.CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(AuxDate.ToString("dddd", Globalization.CultureInfo.CurrentUICulture) & ", " &
                                                                                                AuxDate.ToShortDateString & " " & AuxDate.ToShortTimeString)
            End If

        ElseIf e.Column.FieldName = "Progress" Then
            Dim x As GridViewDataProgressBarColumn = CType(e.Column, GridViewDataProgressBarColumn)
            If roTypes.Any2Integer(GridTareas.GetRowValues(e.VisibleIndex, "Exceeded")) = 1 Then
                x.PropertiesProgressBar.IndicatorStyle.BackColor = Drawing.Color.Red
            Else
                x.PropertiesProgressBar.IndicatorStyle.BackColor = Drawing.Color.LightGray
            End If
        End If

        If e.Column.FieldName = "Employees" Then
            If e.Value IsNot System.DBNull.Value And e.Value IsNot Nothing Then
                Try
                    Dim aux As String = ""
                    aux = roTypes.String2Item(e.Value, 0, "@")
                    aux = aux & " " & Me.Language.Translate("TaskGrid.Column.ActiveEmployees", DefaultScope) & vbNewLine
                    aux = aux & ", " & roTypes.String2Item(e.Value, 1, "@") & " " & Me.Language.Translate("TaskGrid.Column.RecentEmployees", DefaultScope)
                    e.DisplayText = aux
                Catch ex As Exception

                End Try
            End If
        End If

    End Sub

    Protected Sub GridTareas_CustomUnboundColumnData(ByVal sender As Object, ByVal e As ASPxGridViewColumnDataEventArgs) Handles GridTareas.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "ImgStatus"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("Status") IsNot System.DBNull.Value Then
                        Select Case CType(e.GetListSourceFieldValue("Status"), Integer)
                            Case 0
                                e.Value = "Pendiente.png"
                            Case 1
                                e.Value = "Finalizada.png"
                            Case 2
                                e.Value = "Cancelada.png"
                            Case 3
                                e.Value = "PendienteConfirmacion.png"
                            Case Else
                                e.Value = "Pendiente.png"
                        End Select
                    Else
                        e.Value = "Pendiente.png"
                    End If
                End If
        End Select
    End Sub

    Protected Sub GridTareas_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewTableDataCellEventArgs) Handles GridTareas.HtmlDataCellPrepared
        If e.DataColumn.FieldName = "ID" Then
            If e.GetValue("Color") IsNot System.DBNull.Value And e.GetValue("Color") IsNot Nothing Then
                'obtener color de fondo para el IdTarea
                e.Cell.BackColor = System.Drawing.ColorTranslator.FromWin32(e.GetValue("Color"))
                Dim bcolor As Drawing.Color = Drawing.Color.FromArgb(e.GetValue("Color"))
                Dim fcolor As Drawing.Color = Drawing.Color.FromArgb(255 - bcolor.R, 255 - bcolor.G, 255 - bcolor.B)
                e.Cell.ForeColor = fcolor
            Else
                e.Cell.BackColor = System.Drawing.Color.White
                Dim bcolor As Drawing.Color = Drawing.Color.FromArgb(e.GetValue("Color"))
                Dim fcolor As Drawing.Color = Drawing.Color.FromArgb(255 - System.Drawing.Color.White.R, 255 - System.Drawing.Color.White.G, 255 - System.Drawing.Color.White.B)
                e.Cell.ForeColor = fcolor
            End If

        ElseIf e.DataColumn.FieldName = "ActivationTask" Then
            If e.GetValue("ColorTaskPrev") IsNot System.DBNull.Value And e.GetValue("ColorTaskPrev") IsNot Nothing Then
                e.Cell.BackColor = System.Drawing.ColorTranslator.FromWin32(e.GetValue("ColorTaskPrev"))
                Dim bcolor As Drawing.Color = Drawing.Color.FromArgb(e.GetValue("ColorTaskPrev"))
                Dim fcolor As Drawing.Color = Drawing.Color.FromArgb(255 - bcolor.R, 255 - bcolor.G, 255 - bcolor.B)
                e.Cell.ForeColor = fcolor
            Else
                e.Cell.BackColor = System.Drawing.Color.White
                Dim bcolor As Drawing.Color = Drawing.Color.FromArgb(e.GetValue("ColorTaskPrev"))
                Dim fcolor As Drawing.Color = Drawing.Color.FromArgb(255 - System.Drawing.Color.White.R, 255 - System.Drawing.Color.White.G, 255 - System.Drawing.Color.White.B)
                e.Cell.ForeColor = fcolor
            End If

            'NameTaskPrev
            If e.GetValue("NameTaskPrev") IsNot System.DBNull.Value And e.GetValue("NameTaskPrev") IsNot Nothing Then
                e.Cell.Attributes.Add("title", e.GetValue("NameTaskPrev").ToString())
            End If
        ElseIf e.DataColumn.FieldName = "Alerts" Then
            'Si tiene alertas fondo amarillo
            Dim bcolor As Drawing.Color
            If roTypes.Any2Double(e.GetValue("Alerts")) > 0 Then
                e.Cell.BackColor = Drawing.Color.Yellow
                bcolor = Drawing.Color.Yellow
            Else
                e.Cell.BackColor = Drawing.Color.White
                bcolor = Drawing.Color.White
            End If

            Dim fcolor As Drawing.Color = Drawing.Color.FromArgb(255 - bcolor.R, 255 - bcolor.G, 255 - bcolor.B)
            e.Cell.ForeColor = fcolor

        ElseIf e.DataColumn.FieldName <> "ImgStatus" And e.DataColumn.FieldName <> "Employees" Then
            If e.CellValue IsNot System.DBNull.Value And e.CellValue IsNot Nothing Then
                e.Cell.Attributes.Add("title", e.CellValue.ToString())
            End If
        End If

    End Sub

    Protected Sub LinqServerModeDataSource1_Selecting(ByVal sender As Object, ByVal e As DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs) Handles LinqServerModeDataSource1.Selecting
        'enlazar con datatable en runtime http://www.devexpress.com/Support/Center/p/E168.aspx
        e.QueryableSource = GetTasks()
    End Sub

    Private Function GetTasks() As IQueryable

        Dim qry As IQueryable = Nothing

        Try

            Dim sAux As String = (Me.hdnFilterStatus.Value & "1111").Substring(0, 4)
            Dim VerPendientes As Boolean = (sAux(0) = "1")
            Dim VerFinalizadas As Boolean = (sAux(1) = "1")
            Dim VerCanceladas As Boolean = (sAux(2) = "1")
            Dim VerPendientesConfirmacion As Boolean = (sAux(3) = "1")

            Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport()
            If oPassport IsNot Nothing Then

                Dim oList() As Integer = Nothing
                oList = API.TasksServiceMethods.GetBusinessCenterByPassport(Me, oPassport.ID, False)

                Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, oPassport.ID)
                Dim ContextDB As TasksDataContext = New TasksDataContext(strConn)
                ContextDB.CommandTimeout = 900

                If oPermission >= Permission.Read Then
                    qry = From task In ContextDB.sysroCurrentTasksStatus Select task Where ((VerPendientes AndAlso task.Status = 0) Or
                               (VerFinalizadas AndAlso task.Status = 1) Or (VerCanceladas AndAlso task.Status = 2) Or (VerPendientesConfirmacion AndAlso task.Status = 3)) And task.ID > 0 And oList.Contains(task.IDCenter)
                Else
                    qry = From task In ContextDB.sysroCurrentTasksStatus Select task Where task.ID = -1
                End If

            End If
        Catch ex As Exception
        End Try

        Return qry

    End Function

    '#####  ======================================
    'ESTA FUNCION SE EJECUTA EN CALLBACK POR LO TANTO NO HAY DISPONIBLE NINGUN CONTROL DENTRO DE ESTA FUNCION NI EN NINGUNA DE LAS FUNCIONES QUE UTILICE
    '#####  ======================================
    Protected Sub ASPxCallbackPanel2_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanel2.Callback
        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskDetails As New TaskDetails()
        oTaskDetails = roJSONHelper.Deserialize(strParameters, oTaskDetails.GetType())

        Dim intProgress As Integer = 0
        Dim intExceeded As Integer = 0
        Dim dblEmployees As Double = 0

        Me.hdnFieldsASP.Value = LoadTaskDataX(oTaskDetails.IdTask, oTaskDetails.TimeWorked, intProgress, intExceeded, dblEmployees)

        If oTaskDetails.TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
            If oTaskDetails.BeginDate <> String.Empty And oTaskDetails.EndDate <> String.Empty Then
                Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup, oTaskDetails.BeginDate, oTaskDetails.EndDate)
            Else
                Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
            End If
        Else
            Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
        End If

    End Sub

    Protected Sub ASPxCallbackPanel3_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanel3.Callback
        ' Si se utiliza
        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskStatistics As New TaskStatistics()
        oTaskStatistics = roJSONHelper.Deserialize(strParameters, oTaskStatistics.GetType())

        If oTaskStatistics.TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
            If oTaskStatistics.BeginDate <> String.Empty And oTaskStatistics.EndDate <> String.Empty Then
                Me.hdnFieldsStatisticsSmall.Value = LoadStatisticsDataX(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup, oTaskStatistics.BeginDate, oTaskStatistics.EndDate)
            Else
                Me.hdnFieldsStatisticsSmall.Value = LoadStatisticsDataX(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup)
            End If
        Else
            Me.hdnFieldsStatisticsSmall.Value = LoadStatisticsDataX(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup)
        End If

    End Sub

    Protected Sub ASPxCallbackPanel4_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanel4.Callback
        ' Si se utiliza para cargar grid de atributos de la tarea
        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskDetails As New TaskDetails()
        oTaskDetails = roJSONHelper.Deserialize(strParameters, oTaskDetails.GetType())

        Me.hdnTaskFieldsTask.Value = LoadTaskUserFields(oTaskDetails.IdTask)
    End Sub

    'RECIBE CALLBACK PARA LLENAR EL GRAFICO
    Protected Sub ASPxCallbackPanelGraf_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelGraf.Callback
        ' Si se utiliza, sirve para cargar grafico

        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskStatistics As New TaskStatistics()
        oTaskStatistics = roJSONHelper.Deserialize(strParameters, oTaskStatistics.GetType())

        If oTaskStatistics.TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
            If oTaskStatistics.BeginDate <> String.Empty And oTaskStatistics.EndDate <> String.Empty Then
                LoadStatisticsGrafic(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup, oTaskStatistics.BeginDate, oTaskStatistics.EndDate)
            Else
                LoadStatisticsGrafic(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup)
            End If
        Else
            LoadStatisticsGrafic(oTaskStatistics.IdTask, oTaskStatistics.TypeView, oTaskStatistics.TypeGroup)
        End If
    End Sub

    Private Sub LoadStatisticsGrafic(ByVal intIDTask As Integer, ByVal ViewType As TaskStatisticsViewEnum, ByVal TaskStatisticsGroupBy As TaskStatisticsGroupByEnum,
                                     Optional ByVal BeginDate As DateTime = Nothing, Optional ByVal EndDate As DateTime = Nothing)

        Dim id As Integer = 0
        ''''Dim sConcept As String = ""
        Dim sTotal As String = ""
        Dim nTotal As Double = 0

        Me.Grafico.Series.Clear()

        Dim SerieEmpleado As New DevExpress.XtraCharts.Series(Me.Language.Translate("Chart.Series.Employee", DefaultScope), DevExpress.XtraCharts.ViewType.Pie3D)

        Dim tbStatistics As DataTable = TasksServiceMethods.GetTaskStatistics(Me, intIDTask, ViewType, TaskStatisticsGroupBy, BeginDate, EndDate)
        For Each row As DataRow In tbStatistics.Rows

            If ViewType = TaskStatisticsViewEnum._WorkingTime And TaskStatisticsGroupBy = TaskStatisticsGroupByEnum._ByDate Then
                id += 1
                ''''sConcept = DateTime.Parse(row(0)).ToShortDateString

            ElseIf ViewType = TaskStatisticsViewEnum._Diversions Then
                id = roTypes.Any2String(row(0))
                '' ''Select Case roTypes.Any2String(row(0))
                '' ''    Case 1
                '' ''        sConcept = Me.Language.Translate("Diversions.InitialTime", DefaultScope)
                '' ''    Case 2
                '' ''        sConcept = Me.Language.Translate("Diversions.TimeChangedRequirements", DefaultScope)
                '' ''    Case 3
                '' ''        sConcept = Me.Language.Translate("Diversions.ForecastErrorTime", DefaultScope)
                '' ''    Case 4
                '' ''        sConcept = Me.Language.Translate("Diversions.NonProductiveTimeIncidence", DefaultScope)
                '' ''    Case 5
                '' ''        sConcept = Me.Language.Translate("Diversions.EmployeeTime", DefaultScope)
                '' ''    Case 6
                '' ''        sConcept = Me.Language.Translate("Diversions.TeamTime", DefaultScope)
                '' ''    Case 7
                '' ''        sConcept = Me.Language.Translate("Diversions.MaterialTime", DefaultScope)
                '' ''    Case 8
                '' ''        sConcept = Me.Language.Translate("Diversions.OtherTime", DefaultScope)
                '' ''End Select
            Else
                id += 1
                ''''sConcept = roTypes.Any2String(row(0))
            End If

            sTotal = roConversions.ConvertHoursToTime(roTypes.Any2Double(row(1)))
            nTotal = roTypes.Any2Double(row(1))

            ''''SerieEmpleado.Points.Add(New DevExpress.XtraCharts.SeriesPoint(sConcept, nTotal))
            SerieEmpleado.Points.Add(New DevExpress.XtraCharts.SeriesPoint(id.ToString, nTotal))

        Next

        SerieEmpleado.Label.TextPattern = "{VP:c2}"

        SerieEmpleado.LegendTextPattern = "{A}"

        SerieEmpleado.Label.ResolveOverlappingMode = DevExpress.XtraCharts.ResolveOverlappingMode.Default

        Me.Grafico.Series.Add(SerieEmpleado)
        Me.Grafico.Legend.Visibility = True

    End Sub

    'Funcion ejecutada bajo CallBack
    Private Function LoadStatisticsDataX(ByVal IdTask As Integer, Optional ByVal TypeView As TaskStatisticsViewEnum = TaskStatisticsViewEnum._WorkingTime,
                                         Optional ByVal TypeGroup As TaskStatisticsGroupByEnum = TaskStatisticsGroupByEnum._ByEmployee,
                                         Optional ByVal BeginDate As DateTime = Nothing, Optional ByVal EndDate As DateTime = Nothing) As String

        Dim strRetStats As String = String.Empty
        Dim strAux As String

        Dim id As Integer = 0
        Dim sConcept As String = String.Empty
        Dim sTotal As String

        Dim SumTotal As Double = 0
        Try

            strAux = String.Empty
            Dim tbStatistics As DataTable = TasksServiceMethods.GetTaskStatistics(Me, IdTask, TypeView, TypeGroup, BeginDate, EndDate)
            For Each row As DataRow In tbStatistics.Rows
                Dim employeeId As Integer = -1

                If TypeView = TaskStatisticsViewEnum._WorkingTime And TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
                    id += 1
                    sConcept = DateTime.Parse(row(0)).ToShortDateString

                ElseIf TypeView = TaskStatisticsViewEnum._Diversions Then
                    id = roTypes.Any2String(row(0))
                    Select Case id
                        Case 1
                            sConcept = Me.Language.Translate("Diversions.InitialTime", DefaultScope)
                        Case 2
                            sConcept = Me.Language.Translate("Diversions.TimeChangedRequirements", DefaultScope)
                        Case 3
                            sConcept = Me.Language.Translate("Diversions.ForecastErrorTime", DefaultScope)
                        Case 4
                            sConcept = Me.Language.Translate("Diversions.NonProductiveTimeIncidence", DefaultScope)
                        Case 5
                            sConcept = Me.Language.Translate("Diversions.EmployeeTime", DefaultScope)
                        Case 6
                            sConcept = Me.Language.Translate("Diversions.TeamTime", DefaultScope)
                        Case 7
                            sConcept = Me.Language.Translate("Diversions.MaterialTime", DefaultScope)
                        Case 8
                            sConcept = Me.Language.Translate("Diversions.OtherTime", DefaultScope)
                    End Select
                ElseIf TypeView = TaskStatisticsViewEnum._WorkingTime And TypeGroup = TaskStatisticsGroupByEnum._ByGroup Then
                    id += 1
                    sConcept = roTypes.Any2String(row(0))
                Else
                    id += 1
                    sConcept = roTypes.Any2String(row(0))
                    employeeId = roTypes.Any2Integer(row(2))
                End If

                If Not bolCentesimalFormatProductiV Then
                    sTotal = roConversions.ConvertHoursToTime(roTypes.Any2Double(row(1)))
                Else
                    sTotal = Format(roTypes.Any2Double(row(1)), "0.0000")
                End If

                SumTotal += roTypes.Any2Double(row(1))

                strAux &= "{fields:[{ field: 'id', value: '" & id & "' }, " &
                                   "{ field: 'concept', value: '" & sConcept.Replace("'", "\'") & "' }, " &
                                   "{ field: 'employeeId', value: '" & employeeId & "' }, " &
                                   "{ field: 'total', value: '" & sTotal & "'}]},"
            Next

            If Not bolCentesimalFormatProductiV Then
                sTotal = roConversions.ConvertHoursToTime(SumTotal)
            Else
                sTotal = Format(roTypes.Any2Double(SumTotal), "0.0000")
            End If

            strAux &= "{fields:[{ field: 'id', value: '' }, " &
                               "{ field: 'concept', value: 'TOTAL' }, " &
                               "{ field: 'employeeId', value: '-1' }, " &
                               "{ field: 'total', value: '" & sTotal & "'}]},"

            If strAux.EndsWith(",") Then strAux = strAux.Substring(0, strAux.Length - 1)

            strRetStats = strAux
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strRetStats = rError.toJSON
        End Try

        Return strRetStats

    End Function

    'Funcion ejecutada bajo CallBack
    Private Function LoadTaskDataX(ByVal IdTask As Integer, ByVal nTimeWorked As Double, ByRef intProgress As Integer, ByRef intExceeded As Integer, ByRef dblEmployees As Double, Optional ByVal CCurrentTask As roTask = Nothing) As String

        Dim strRetTask As String = String.Empty
        Dim strAux As String = String.Empty
        Dim nAux As Integer = 0
        Dim isNegative As Boolean = False

        Try

            Dim CurrentTask As roTask
            If CCurrentTask Is Nothing Then
                If IdTask < 0 Then
                    CurrentTask = New roTask()
                Else
                    CurrentTask = TasksServiceMethods.GetTaskByID(Me, IdTask, True)
                    If (nTimeWorked = -1) Then
                        nTimeWorked = API.TasksServiceMethods.GetTaskWorkedTime(Me.Page, IdTask)
                    End If
                End If
            Else
                CurrentTask = CCurrentTask
            End If

            If CurrentTask Is Nothing Then Return strRetTask

            Dim oJSONFields As New Generic.List(Of JSONFieldItem)

            'TODO: Afegir tema read-only???

            Dim bDisableGroup As Boolean = False
            Dim bolDisableControls As Boolean = False
            If oPermission <= Permission.Read Then
                'Me.DisableControls()
                bDisableGroup = True
                bolDisableControls = True
                Me.hdnModeEdit.Value = "false"
            Else
                Me.hdnModeEdit.Value = "true"
            End If

            'ID As Integer

            'Name As String
            Me.txtName.Value = CurrentTask.Name
            Me.txtName.Enabled = Not bolDisableControls

            'Project As String
            Me.txtProject.Value = CurrentTask.Project
            Me.txtProject.Enabled = Not bolDisableControls

            ' Center
            Me.cmbGroup.SelectedItem = Me.cmbGroup.Items.FindByValue(CurrentTask.IDCenter.ToString)
            If cmbGroup.SelectedItem Is Nothing Then
                ' Si no esta en la lista hay que añadirlo porque el centro actualmente esta inactivo
                Dim oCenter As roBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, CurrentTask.IDCenter, False)

                If oCenter IsNot Nothing Then
                    Dim newItem As New DevExpress.Web.ListEditItem(oCenter.Name & ("*"), oCenter.ID)
                    Me.cmbGroup.Items.Add(newItem)
                    Me.cmbGroup.SelectedItem = newItem
                Else
                    cmbGroup.SelectedItem = cmbGroup.Items.FindByValue(CType(0, Short))
                End If
            End If
            Me.cmbGroup.Enabled = Not bolDisableControls

            'Color As Integer
            Me.dxColorPicker.Color = System.Drawing.ColorTranslator.FromWin32(CurrentTask.Color)
            Me.dxColorPicker.Enabled = Not bolDisableControls

            'ShortName As String
            Me.txtShortName.Value = roTypes.Any2String(CurrentTask.ShortName)
            Me.txtShortName.Enabled = Not bolDisableControls

            'Description As String
            Me.txtDescription.Value = roTypes.Any2String(CurrentTask.Description)
            Me.txtDescription.Enabled = Not bolDisableControls

            'Tag As String
            Me.txtTag.Value = roTypes.Any2String(CurrentTask.Tag)
            Me.txtTag.Enabled = Not bolDisableControls

            'BarCode As String
            Me.txtBarCode.Value = roTypes.Any2String(CurrentTask.BarCode)
            Me.txtBarCode.Enabled = Not bolDisableControls

            'Priority As Integer
            Me.TrackBarPriority.Value = CurrentTask.Priority.ToString
            Me.TrackBarPriority.Enabled = Not bolDisableControls

            '************************************************

            If Not bolCentesimalFormatProductiV Then
                Me.txtTimeChangedRequirements.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.TimeChangedRequirements)).Replace("00:", "0:")
            Else
                Me.txtTimeChangedRequirements.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.TimeChangedRequirements)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtTimeChangedRequirements.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtEmployeeTime.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.EmployeeTime)).Replace("00:", "0:")
            Else
                Me.txtEmployeeTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.EmployeeTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtEmployeeTime.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtMaterialTime.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.MaterialTime)).Replace("00:", "0:")
            Else
                Me.txtMaterialTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.MaterialTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtMaterialTime.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtForecastErrorTime.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.ForecastErrorTime)).Replace("00:", "0:")
            Else
                Me.txtForecastErrorTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.ForecastErrorTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtForecastErrorTime.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtTeamTime.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.TeamTime)).Replace("00:", "0:")
            Else
                Me.txtTeamTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.TeamTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtTeamTime.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtOtherTime.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.OtherTime)).Replace("00:", "0:")
            Else
                Me.txtOtherTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.OtherTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtOtherTime.Enabled = Not bolDisableControls

            If Not bolCentesimalFormatProductiV Then
                Me.txtNonProductiveTimeIncidence.Value = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.NonProductiveTimeIncidence)).Replace("00:", "0:")
            Else
                Me.txtNonProductiveTimeIncidence.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.NonProductiveTimeIncidence)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtNonProductiveTimeIncidence.Enabled = Not bolDisableControls

            Dim doubleTotalHours As Double = 0
            doubleTotalHours = roTypes.Any2Double(CurrentTask.TimeChangedRequirements) + roTypes.Any2Double(CurrentTask.ForecastErrorTime) + roTypes.Any2Double(CurrentTask.NonProductiveTimeIncidence)
            doubleTotalHours = doubleTotalHours + roTypes.Any2Double(CurrentTask.EmployeeTime) + roTypes.Any2Double(CurrentTask.TeamTime) + roTypes.Any2Double(CurrentTask.MaterialTime) + roTypes.Any2Double(CurrentTask.OtherTime)
            If Not bolCentesimalFormatProductiV Then
                Me.txtTotalDesviationHours.Value = roConversions.ConvertHoursToTime(doubleTotalHours).Replace("00:", "0:")
            Else
                Me.txtTotalDesviationHours.Value = roTypes.Any2String(roTypes.Any2Double(doubleTotalHours)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If
            Me.txtTotalDesviationHours.Enabled = Not bolDisableControls

            'TimeRemarks As String
            Me.txtTimeRemarks.Value = roTypes.Any2String(CurrentTask.TimeRemarks)
            Me.txtTimeRemarks.Enabled = Not bolDisableControls

            'Status As Byte
            Me.ASPxbtnSetActive.Checked = False

            Me.ASPxbtnSetEnded.Checked = False

            Me.ASPxbtnSetCanceled.Checked = False

            Me.ASPxbtnSetPendienteVal.Checked = False

            Me.ASPxbtnSetPendienteVal.Visible = False

            Select Case CurrentTask.Status
                Case TaskStatusEnum._ON
                    nAux = 0
                    ASPxbtnSetActive.Checked = True
                Case TaskStatusEnum._COMPLETED
                    nAux = 1
                    Me.ASPxbtnSetEnded.Checked = True
                Case TaskStatusEnum._CANCELED
                    nAux = 2
                    Me.ASPxbtnSetCanceled.Checked = True
                Case TaskStatusEnum._PENDING
                    nAux = 3
                    Me.ASPxbtnSetPendienteVal.Checked = True
                    Me.ASPxbtnSetPendienteVal.Visible = True
                Case Else
                    ASPxbtnSetActive.Checked = True
            End Select

            Me.ASPxbtnSetActive.Enabled = Not bolDisableControls
            Me.ASPxbtnSetEnded.Enabled = Not bolDisableControls
            Me.ASPxbtnSetCanceled.Enabled = Not bolDisableControls
            Me.ASPxbtnSetPendienteVal.Enabled = Not bolDisableControls

            ''*********************************************************************************

            If CurrentTask.ExpectedStartDate.HasValue Then
                'Fecha Ini
                Me.txtExpectedStartDate.Date = CurrentTask.ExpectedStartDate.Value

                'Hora Ini
                Me.txtExpectedStartTime.Text = Format(CurrentTask.ExpectedStartDate.Value, HelperWeb.GetShortTimeFormat)
            Else
                'Fecha Ini
                Me.txtExpectedStartDate.Value = Nothing

                'Hora Ini
                Me.txtExpectedStartTime.Text = ""
            End If

            Me.txtExpectedStartDate.Enabled = Not bolDisableControls
            Me.txtExpectedStartTime.Enabled = Not bolDisableControls

            'ExpectedEndDate As System.Nullable(Of Date)
            If CurrentTask.ExpectedEndDate.HasValue Then
                'Fecha Fin
                Me.txtExpectedEndDate.Date = CurrentTask.ExpectedEndDate.Value

                'Hora Fin
                Me.txtExpectedEndTime.Text = Format(CurrentTask.ExpectedEndDate.Value, HelperWeb.GetShortTimeFormat)
            Else
                'Fecha Ini
                Me.txtExpectedEndDate.Value = Nothing

                'Hora Fin
                Me.txtExpectedEndTime.Text = ""
            End If

            Me.txtExpectedEndDate.Enabled = Not bolDisableControls
            Me.txtExpectedEndTime.Enabled = Not bolDisableControls

            If bDisableGroup = False And oPermission < Permission.Admin Then
                bDisableGroup = True
            End If

            If Not bolCentesimalFormatProductiV Then
                strAux = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.InitialTime)).Replace("00:", "0:")
                Me.txtInitialTime.Value = strAux
            Else
                strAux = roConversions.ConvertHoursToTime(roTypes.Any2Double(CurrentTask.InitialTime)).Replace("00:", "0:")
                Me.txtInitialTime.Value = roTypes.Any2String(roTypes.Any2Double(CurrentTask.InitialTime)).Replace(".", HelperWeb.GetDecimalDigitFormat)
            End If

            If nTimeWorked > 0 Then
                Me.txtInitialTime.Enabled = False
            Else
                Me.txtInitialTime.Enabled = Not bolDisableControls
            End If

            Me.optClosingByDate.Checked = CurrentTask.TypeClosing = TaskTypeClosingEnum._ATDATE
            Me.optClosingAllways.Checked = CurrentTask.TypeClosing = TaskTypeClosingEnum._UNDEFINED

            Me.optClosingByDate.Enabled = Not bolDisableControls
            Me.optClosingAllways.Enabled = Not bolDisableControls

            If CurrentTask.ClosingDate.HasValue Then
                'Fecha
                Me.txtClosingDate.Date = CurrentTask.ClosingDate.Value
                'Hora
                Me.txtClosingTime.Text = Format(CurrentTask.ClosingDate.Value, HelperWeb.GetShortTimeFormat)
            Else
                'Fecha
                Me.txtClosingDate.Value = Nothing
                'Hora
                Me.txtClosingTime.Text = ""
            End If

            Me.txtClosingDate.Enabled = Not bolDisableControls
            Me.txtClosingTime.Enabled = Not bolDisableControls

            Select Case CurrentTask.TypeActivation
                Case TaskTypeActivationEnum._ALWAYS
                    nAux = 0
                Case TaskTypeActivationEnum._ATDATE
                    nAux = 1
                Case TaskTypeActivationEnum._ATFINISHTASK
                    nAux = 2
                Case TaskTypeActivationEnum._ATRUNTASK
                    nAux = 3
                Case Else
                    nAux = 0
            End Select
            Me.optActivAllways.Checked = CurrentTask.TypeActivation = TaskTypeActivationEnum._ALWAYS
            Me.optActivByDate.Checked = CurrentTask.TypeActivation = TaskTypeActivationEnum._ATDATE
            Me.optActivByEndTask.Checked = CurrentTask.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK
            Me.optActivByIniTask.Checked = CurrentTask.TypeActivation = TaskTypeActivationEnum._ATRUNTASK

            Me.optActivAllways.Enabled = Not bolDisableControls
            Me.optActivByDate.Enabled = Not bolDisableControls
            Me.optActivByEndTask.Enabled = Not bolDisableControls
            Me.optActivByIniTask.Enabled = Not bolDisableControls

            Select Case CurrentTask.TypeActivation
                Case TaskTypeActivationEnum._ATFINISHTASK
                    strAux = TasksServiceMethods.GetNameTask(Me, roTypes.Any2Integer(CurrentTask.ActivationTask))
                    Me.txtEndTask.Text = strAux
                    Me.txtIniTask.Text = ""
                    Me.hdnActivationTask.Value = roTypes.Any2Integer(CurrentTask.ActivationTask)
                Case TaskTypeActivationEnum._ATRUNTASK
                    strAux = TasksServiceMethods.GetNameTask(Me, roTypes.Any2Integer(CurrentTask.ActivationTask))
                    Me.txtEndTask.Text = ""
                    Me.txtIniTask.Text = strAux
                    Me.hdnActivationTask.Value = roTypes.Any2Integer(CurrentTask.ActivationTask)
                Case Else
                    Me.txtEndTask.Text = ""
                    Me.txtIniTask.Text = ""
                    Me.hdnActivationTask.Value = ""
            End Select

            If CurrentTask.ActivationDate.HasValue Then
                'Fecha
                Me.txtActivationDate.Date = CurrentTask.ActivationDate.Value
                'Hora
                Me.txtActivationTime.Text = Format(CurrentTask.ActivationDate.Value, HelperWeb.GetShortTimeFormat)
            Else
                'Fecha
                Me.txtActivationDate.Value = Nothing

                'Hora
                Me.txtActivationTime.Text = ""
            End If

            Me.txtActivationDate.Enabled = Not bolDisableControls
            Me.txtActivationTime.Enabled = Not bolDisableControls

            Me.optTypeCollabAny.Checked = CurrentTask.TypeCollaboration = TaskTypeCollaborationEnum._ANY
            Me.optTypeCollabFirst.Checked = CurrentTask.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST

            'ModeCollaboration As System.Nullable(Of Byte) (--TaskModeCollaborationEnum--)
            Me.optColabAllEmp.Checked = CurrentTask.ModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
            Me.optColabOnlyOneEmp.Checked = CurrentTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME

            'TypeAuthorization As System.Nullable(Of Byte) (--TaskTypeAuthorizationEnum--)
            Me.optAutEmpAll.Checked = (CurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE Or CurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._ASSIGNMENTEMPLOYEES)
            Me.optAutEmpSelect.Checked = CurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES

            Me.optTypeCollabAny.Enabled = Not bolDisableControls
            Me.optTypeCollabFirst.Enabled = Not bolDisableControls
            Me.optColabAllEmp.Enabled = Not bolDisableControls
            Me.optColabOnlyOneEmp.Enabled = Not bolDisableControls
            Me.optAutEmpAll.Enabled = Not bolDisableControls
            Me.optAutEmpSelect.Enabled = Not bolDisableControls

            'Progreso
            Dim nTotalTime As Double
            nTotalTime = roTypes.Any2Double(CurrentTask.InitialTime) + roTypes.Any2Double(CurrentTask.TimeChangedRequirements) + roTypes.Any2Double(CurrentTask.ForecastErrorTime) +
                        roTypes.Any2Double(CurrentTask.NonProductiveTimeIncidence) + roTypes.Any2Double(CurrentTask.EmployeeTime) + roTypes.Any2Double(CurrentTask.TeamTime) +
                        roTypes.Any2Double(CurrentTask.MaterialTime) + roTypes.Any2Double(CurrentTask.OtherTime)

            nTotalTime = Math.Round(nTotalTime, 4)

            strAux = roConversions.ConvertHoursToTime(nTotalTime)

            If nTimeWorked = -1 Then
                intProgress = 0
            Else
                nAux = IIf(nTotalTime > 0, Math.Round((nTimeWorked / nTotalTime) * 100, 0), 0)
                If nAux > 100 Then nAux = 100
                intProgress = nAux
            End If

            'Info estado de la tarea
            strAux = ""
            If CurrentTask.StartDate.HasValue Then
                'Tarea comenzada
                strAux = Me.Language.Translate("TaskStatus.BeginWorkTheDay", DefaultScope) & Format(CurrentTask.StartDate.Value, HelperWeb.GetShortDateFormat) & Me.Language.Translate("TaskStatus.AtTime", DefaultScope) & Format(CurrentTask.StartDate.Value, HelperWeb.GetShortTimeFormat) & "."

                If CurrentTask.EndDate.HasValue Then
                    If CurrentTask.Status = TaskStatusEnum._COMPLETED Then
                        strAux &= Me.Language.Translate("TaskStatus.CompletedDay", DefaultScope) & Format(CurrentTask.EndDate.Value, HelperWeb.GetShortDateFormat) & Me.Language.Translate("TaskStatus.AtTime", DefaultScope) & Format(CurrentTask.EndDate.Value, HelperWeb.GetShortTimeFormat) & "."

                    ElseIf CurrentTask.Status = TaskStatusEnum._CANCELED Then
                        strAux &= Me.Language.Translate("TaskStatus.CanceledDay", DefaultScope) & Format(CurrentTask.EndDate.Value, HelperWeb.GetShortDateFormat) & Me.Language.Translate("TaskStatus.AtTime", DefaultScope) & Format(CurrentTask.EndDate.Value, HelperWeb.GetShortTimeFormat) & "."
                    End If
                End If
            Else
                If CurrentTask.Status = TaskStatusEnum._ON Then
                    'Tarea NO comenzada
                    strAux = Me.Language.Translate("TaskStatus.NotBeginWork", DefaultScope) '"No se ha empezado a trabajar en la tarea."
                End If
            End If
            Me.lblTaskInfo.Text = strAux
            Me.icoTaskInfo.Visible = strAux.Length > 0

            'Status
            strAux = ""
            Select Case CurrentTask.Status
                Case TaskStatusEnum._ON
                    strAux = Me.Language.Translate("lblTaskInfoState.ON.Text", Me.DefaultScope)
                Case TaskStatusEnum._COMPLETED
                    strAux = Me.Language.Translate("lblTaskInfoState.COMPLETED.Text", Me.DefaultScope)
                Case TaskStatusEnum._CANCELED
                    strAux = Me.Language.Translate("lblTaskInfoState.CANCELED.Text", Me.DefaultScope)
                Case TaskStatusEnum._PENDING
                    strAux = Me.Language.Translate("lblTaskInfoState.PENDING.Text", Me.DefaultScope)
            End Select
            Me.lblTaskInfoState.Text = strAux
            Me.Img2.Visible = strAux.Length > 0

            strAux = ""
            If CurrentTask.IDEmployeeUpdateStatus IsNot Nothing Then
                Dim dt As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Me.Page, "ID = " & CurrentTask.IDEmployeeUpdateStatus, "Employees")
                If (dt IsNot Nothing AndAlso dt.Rows.Count = 1) Then
                    strAux = Me.Language.Translate("lblTaskClosedInfo.Completed.Text", Me.DefaultScope) & dt.Rows(0).Item("EmployeeName")
                    If CurrentTask.UpdateStatusDate IsNot Nothing Then
                        strAux = strAux & Me.Language.Translate("lblTaskClosedInfo.CompletedDate.Text", Me.DefaultScope) & Format(CurrentTask.UpdateStatusDate.Value, HelperWeb.GetShortDateFormat)
                        strAux = strAux & Me.Language.Translate("lblTaskClosedInfo.CompletedHour.Text", Me.DefaultScope) & Format(CurrentTask.UpdateStatusDate.Value, HelperWeb.GetShortTimeFormat)
                    End If
                End If
            End If

            Me.lblTaskClosedInfo.Text = strAux
            Me.icoTaskClosedInfo.Visible = strAux.Length > 0

            'Info tiempo restante de tarea
            strAux = ""
            If CurrentTask.Status = TaskStatusEnum._ON And CurrentTask.ExpectedEndDate.HasValue Then
                Dim ts As TimeSpan = CurrentTask.ExpectedEndDate.Value - Date.Now
                If Date.Compare(CurrentTask.ExpectedEndDate, Date.Now) > 0 Then
                    strAux = Me.Language.Translate("TaskStatus.LeftDays", DefaultScope) & ts.Days & Me.Language.Translate("TaskStatus.DaysAnd", DefaultScope) & ts.Hours & Me.Language.Translate("TaskStatus.HoursAndTeoricEndDate", DefaultScope) & Format(CurrentTask.ExpectedEndDate.Value, HelperWeb.GetShortDateFormat) & ")."
                Else
                    If Math.Abs(ts.Days()) > 0 And Math.Abs(ts.Hours()) > 0 Then
                        strAux = Me.Language.Translate("TaskStatus.Exceeded", DefaultScope) & Math.Abs(ts.Days()) & Me.Language.Translate("TaskStatus.DaysAnd", DefaultScope) & Math.Abs(ts.Hours) & Me.Language.Translate("TaskStatus.HoursAndTeoricEndDate", DefaultScope) & Format(CurrentTask.ExpectedEndDate.Value, HelperWeb.GetShortDateFormat) & ")."
                    Else
                        If Math.Abs(ts.Days()) > 0 And Math.Abs(ts.Hours()) = 0 Then
                            strAux = Me.Language.Translate("TaskStatus.Exceeded", DefaultScope) & Math.Abs(ts.Days()) & Me.Language.Translate("TaskStatus.DaysToTeoricEndDate", DefaultScope) & Format(CurrentTask.ExpectedEndDate.Value, HelperWeb.GetShortDateFormat) & ")."
                        Else
                            If Math.Abs(ts.Days()) = 0 And Math.Abs(ts.Hours()) > 0 Then
                                strAux = Me.Language.Translate("TaskStatus.Exceeded", DefaultScope) & Math.Abs(ts.Hours) & Me.Language.Translate("TaskStatus.HoursToTeoricEndDate", DefaultScope) & Format(CurrentTask.ExpectedEndDate.Value, HelperWeb.GetShortDateFormat) & ")."
                            End If
                        End If
                    End If
                End If
            End If
            lblTaskInfo2.Text = strAux
            Me.icoTaskInfo2.Visible = strAux.Length > 0

            'Total de horas trabajadas
            Dim paramsList As New Generic.List(Of String)

            If bolCentesimalFormatProductiV Then
                paramsList.Add(roTypes.Any2String(nTimeWorked).Replace(".", HelperWeb.GetDecimalDigitFormat))
                paramsList.Add(roTypes.Any2String(nTotalTime).Replace(".", HelperWeb.GetDecimalDigitFormat))
                paramsList.Add(roTypes.Any2String(nTotalTime - nTimeWorked).Replace(".", HelperWeb.GetDecimalDigitFormat))
            Else
                paramsList.Add(roConversions.ConvertHoursToTime(nTimeWorked))
                paramsList.Add(roConversions.ConvertHoursToTime(nTotalTime))
                paramsList.Add(roConversions.ConvertHoursToTime(nTotalTime - nTimeWorked))
            End If
            strAux = Me.Language.Translate("TaskStatus.CurrentTime", DefaultScope, paramsList)
            lblTaskInfo3.Text = strAux
            Me.icoTaskInfo3.Visible = strAux.Length > 0

            'obtener empleados de la tarea
            Dim strEmployeesTask As String = String.Empty
            If CurrentTask.Employees IsNot Nothing Then
                For Each item In CurrentTask.Employees
                    strEmployeesTask &= item.ID & ","
                Next
                If strEmployeesTask.EndsWith(",") Then strEmployeesTask = strEmployeesTask.Substring(0, strEmployeesTask.Length - 1)
            End If

            'obtener grupos de la tarea
            Dim strGroupsTask As String = String.Empty
            If CurrentTask.Groups IsNot Nothing Then
                For Each item In CurrentTask.Groups
                    strGroupsTask &= item.ID & ","
                Next
                If strGroupsTask.EndsWith(",") Then strGroupsTask = strGroupsTask.Substring(0, strGroupsTask.Length - 1)
            End If

            LoadEmployeeGroupsToSelector(strEmployeesTask, strGroupsTask)

            'Obtener cantidad total de empleados asignados a la tarea
            Dim nEmployees As Double = TasksServiceMethods.GetEmployeesFromTask(Me, CurrentTask.ID, String.Empty, String.Empty)

            dblEmployees = nEmployees

            'tiempo trabajado excedido del teorico
            If nTimeWorked > nTotalTime Then
                intExceeded = 1
            Else
                intExceeded = 0
            End If

            Me.btnAddTaskAssigment.Visible = Not bolDisableControls
            Me.btnAddUserFieldsTaskTemplate.Visible = Not bolDisableControls

            strRetTask = ""
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strRetTask = rError.toJSON
        End Try

        Return strRetTask

    End Function

    'Funcion ejecutada bajo CallBack
    Private Function LoadTaskUserFields(ByVal IdTask As Integer) As String
        Dim strRetTask As String = ""
        Try
            Dim oJTaskFieldsTask As New Generic.List(Of Object)
            Dim oJFTaskFieldsTask As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            Dim dt As DataTable = API.TasksServiceMethods.GetTaskFieldsDataTable(Me.Page, IdTask)

            If (dt IsNot Nothing) Then
                For Each cRow As DataRow In dt.Rows
                    oJFTaskFieldsTask = New Generic.List(Of JSONFieldItem)
                    oJFTaskFieldsTask.Add(New JSONFieldItem("IDField", roTypes.Any2String(cRow("IDField")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskFieldsTask.Add(New JSONFieldItem("FieldName", roTypes.Any2String(cRow("FieldName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskFieldsTask.Add(New JSONFieldItem("Value", roTypes.Any2String(cRow("Value")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    Dim strAction As String = ""
                    Select Case roTypes.Any2Integer(cRow("Action"))
                        Case ActionTypes.aCreate
                            strAction = Me.Language.Translate("aCreate", Me.DefaultScope)
                        Case ActionTypes.aBegin
                            strAction = Me.Language.Translate("aBegin", Me.DefaultScope)
                        Case ActionTypes.tChange
                            strAction = Me.Language.Translate("tChange", Me.DefaultScope)
                        Case ActionTypes.tComplete
                            strAction = Me.Language.Translate("tComplete", Me.DefaultScope)
                    End Select
                    oJFTaskFieldsTask.Add(New JSONFieldItem("Action", strAction, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJTaskFieldsTask.Add(oJFTaskFieldsTask)
                Next
            End If

            strJSONGroups &= "{ ""taskFields"": [ "
            For Each oObj As Object In oJTaskFieldsTask
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "] }"
            strRetTask = strJSONGroups
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strRetTask = rError.toJSON
        End Try

        Return strRetTask

    End Function

    Private Function LoadTaskAssignments(ByVal IdTask As Integer) As String
        Dim strRetTask As String = ""
        Try
            Dim oJTaskAssignmentsTask As New Generic.List(Of Object)
            Dim oJFTaskAssignmentsTask As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            Dim dt As DataTable = API.TasksServiceMethods.GetAssignments(Me.Page, IdTask, True)

            If (dt IsNot Nothing) Then
                For Each cRow As DataRow In dt.Rows
                    oJFTaskAssignmentsTask = New Generic.List(Of JSONFieldItem)
                    oJFTaskAssignmentsTask.Add(New JSONFieldItem("AssignmentName", roTypes.Any2String(cRow("Name")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAssignmentsTask.Add(New JSONFieldItem("IDAssignment", roTypes.Any2String(cRow("IDAssignment")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJTaskAssignmentsTask.Add(oJFTaskAssignmentsTask)
                Next
            End If

            strJSONGroups &= "{ ""taskAssignments"": [ "
            For Each oObj As Object In oJTaskAssignmentsTask
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "] }"
            strRetTask = strJSONGroups

            Dim bDisable As Boolean = True
            If oPermission > Permission.Read Then
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strRetTask = rError.toJSON
        End Try

        Return strRetTask

    End Function

    Private Function LoadTaskAlerts(ByVal IdTask As Integer) As String
        Dim strRetTask As String = ""
        Try
            Dim oJTaskAlertsTask As New Generic.List(Of Object)
            Dim oJFTaskAlertsTask As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            Dim dt As DataTable = API.TasksServiceMethods.GetTaskAlertsDataTable(Me.Page, IdTask)

            If (dt IsNot Nothing) Then
                For Each cRow As DataRow In dt.Rows
                    oJFTaskAlertsTask = New Generic.List(Of JSONFieldItem)
                    oJFTaskAlertsTask.Add(New JSONFieldItem("ID", roTypes.Any2String(cRow("ID")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAlertsTask.Add(New JSONFieldItem("EmployeeName", roTypes.Any2String(cRow("EmployeeName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAlertsTask.Add(New JSONFieldItem("IDEmployee", roTypes.Any2String(cRow("IDEmployee")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAlertsTask.Add(New JSONFieldItem("DateTime", roTypes.Any2String(cRow("DateTime")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAlertsTask.Add(New JSONFieldItem("Comment", roTypes.Any2String(cRow("Comment")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskAlertsTask.Add(New JSONFieldItem("Readed", roTypes.Any2Boolean(cRow("IsReaded")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    Dim strReaded As String = ""
                    Select Case roTypes.Any2Boolean(cRow("IsReaded"))
                        Case True
                            strReaded = Me.Language.Translate("aReaded", Me.DefaultScope)
                        Case False
                            strReaded = Me.Language.Translate("aNotReaded", Me.DefaultScope)
                    End Select
                    oJFTaskAlertsTask.Add(New JSONFieldItem("IsReaded", strReaded, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJTaskAlertsTask.Add(oJFTaskAlertsTask)
                Next
            End If

            strJSONGroups &= "{ ""taskAlerts"": [ "
            For Each oObj As Object In oJTaskAlertsTask
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "] }"
            strRetTask = strJSONGroups
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            strRetTask = rError.toJSON
        End Try

        Return strRetTask

    End Function

    '*****************************************************************************************
    'Carga empleados y grupos en la cookie con los valores obtenidos de empleados y grupos
    '*****************************************************************************************
    Private Sub LoadEmployeeGroupsToSelector(ByVal strEmployees As String, ByVal strGroups As String)

        Try

            Dim strItems As String = String.Empty

            If strEmployees <> String.Empty Or strGroups <> String.Empty Then

                Dim bAgregarItem As Boolean = False

                Dim tmpLista As New Generic.List(Of String)

                If strGroups <> "" And strGroups <> "-1" Then 'construir cadena con los grupos
                    For Each strGroup As String In strGroups.Split(",")
                        'ppr --> para nuevo arbol v3 en la lista de grupos obtenida del perfil hay que eliminar los grupos a los que no tenga permiso el usuario
                        If FeatureEmployees <> "" Then
                            If Me.HasFeaturePermissionByGroup(FeatureEmployees, Permission.Read, strGroup, "U") Then
                                strItems &= "A" & strGroup & ","
                            End If
                        Else
                            strItems &= "A" & strGroup & ","
                        End If
                    Next
                End If
                If strEmployees <> "" And strEmployees <> "-1" Then 'construir cadena con los empleados
                    For Each strEmployee As String In strEmployees.Split(",")
                        'ppr --> para nuevo arbol v3 en la lista de empleados obtenida del perfil hay que eliminar los empleados a los que no tenga permiso el usuario
                        If FeatureEmployees <> "" Then
                            If Me.HasFeaturePermissionByEmployee(FeatureEmployees, Permission.Read, strEmployee, "U") Then
                                strItems &= "B" & strEmployee & ","
                            End If
                        Else
                            strItems &= "B" & strEmployee & ","
                        End If
                    Next
                End If

                If strItems.EndsWith(",") Then strItems = strItems.Substring(0, strItems.Length - 1)

            End If

            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpDetailTaskGrid")
            oTreeState.Selected1 = strItems
            HelperWeb.roSelector_SetTreeState(oTreeState)
        Catch
        End Try
    End Sub

    Private Sub LoadTaskDataTab()
        Me.tbButtons.InnerHtml = CreaBarraEines()

        Me.TABBUTTON_06.Visible = False
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then
            Me.TABBUTTON_06.Visible = True
        End If

    End Sub

    Private Function CreaBarraEines() As String
        Try
            Dim strContent As String

            strContent = "<table border=""0"" class=""tbtop"" cellpadding=""0"" cellspacing=""0"">"
            strContent &= "<tr>"
            strContent &= "<td style="" align=""center"">"
            'Contingut dels botons
            strContent &= "<table><tr>"
            If oPermission = Permission.Admin Then
                strContent &= "<td width=""42px""><div id=""tip1"" style=""position: absolute; margin-left: -15px; margin-top: 35px; display: none; z-index: 990; "">" &
                CreateToolTipVertical(Me.Language.Translate("tbAddNewTask", Me.DefaultScope)) &
                "</div><a href=""javascript: void(0);"" style=""padding-bottom:5"" class=""btnTbAdd2 ButtonBarVertical"" onmouseover=""showTbTip('tip1');"" onmouseout=""hideTbTip('tip1');"" onclick=""ShowCreateTaskWizard();""></a></td>"

                strContent &= "<td id=""deleteTaskCell"" width=""42px""><div id=""tip2"" style=""position: absolute; margin-left: -20px; margin-top: 35px; display: none; z-index: 990; "">" &
                CreateToolTipVertical(Me.Language.Translate("tbDelTask", Me.DefaultScope)) &
                "</div><a href=""javascript: void(0);"" style=""padding-bottom:5"" class=""btnTbDel2 ButtonBarVertical"" onmouseover=""showTbTip('tip2');"" onmouseout=""hideTbTip('tip2');"" onclick=""ShowRemoveTask();""></a></td>"

                strContent &= "<td width=""42px""><div id=""tip5"" style=""position: absolute; margin-left: -110px; margin-top: 35px; display: none; z-index: 990; "">" &
                CreateToolTipVertical(Me.Language.Translate("tbTbCompleteTaskWizard", Me.DefaultScope)) &
                "</div><a href=""javascript: void(0);"" style=""padding-bottom:5"" class=""btnTbLaunchWizardCompleteTasks ButtonBarVertical"" onmouseover=""showTbTip('tip5');"" onmouseout=""hideTbTip('tip5');"" onclick=""LaunchTaskCompleteWizard();""></a></td>"

                strContent &= "<td width=""42px""><div id=""tip3"" style=""position: absolute; margin-left: -110px; margin-top: 35px; display: none; z-index: 990; "">" &
                CreateToolTipVertical(Me.Language.Translate("tbTbLaunchWizardTemplate", Me.DefaultScope)) &
                "</div><a href=""javascript: void(0);"" style=""padding-bottom:5"" class=""btnTbLaunchWizardTemplate ButtonBarVertical"" onmouseover=""showTbTip('tip3');"" onmouseout=""hideTbTip('tip3');"" onclick=""LaunchTaskTemplateWizard();""></a></td>"
            Else
                strContent &= "<td width=""22px"">&nbsp;</td>"
                strContent &= "<td width=""22px"">&nbsp;</td>"
                strContent &= "<td width=""22px"">&nbsp;</td>"
            End If
            strContent &= "</tr></table>"
            strContent &= "</td>"
            strContent &= "</tr>"
            strContent &= "</table>"
            Return strContent
        Catch ex As Exception
            Return ex.Message.ToString
        End Try
    End Function

    Private Function CreateToolTipVertical(ByVal ToolTipText As String) As String

        Dim strTip As String = String.Empty

        Dim strEstilo = " style=""border:solid 1px #A8A8A8; border-radius: 5px; -ms-border-radius: 5px; -moz-border-radius: 5px; -webkit-border-radius: 5px; -khtml-border-radius: 5px; padding:8px; white-space: nowrap; background-color: #FFFFCC; 	color: #2D4155;"""

        Try
            strTip = "<div" & strEstilo & ">"
            strTip &= ToolTipText
            strTip &= "</div>"
        Catch ex As Exception
            strTip = String.Empty
        End Try

        Return strTip

    End Function

    Protected Sub ASPxbtnExportGrid_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Me.ASPxGridViewExporter1.GridViewID = Me.GridTareas.ID
        Me.ASPxGridViewExporter1.WriteXlsxToResponse(True)
    End Sub

    Protected Sub ASPxCallbackPanel5_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanel5.Callback
        ' Si se utiliza sirve para cargar las alertas
        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskDetails As New TaskDetails()
        oTaskDetails = roJSONHelper.Deserialize(strParameters, oTaskDetails.GetType())

        Me.hdnTaskAlertsTask.Value = LoadTaskAlerts(oTaskDetails.IdTask)
    End Sub

    Protected Sub ASPxCallbackPanel6_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanel6.Callback
        ' Si se utiliza sirve para cargar los puestos asignados
        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskDetails As New TaskDetails()
        oTaskDetails = roJSONHelper.Deserialize(strParameters, oTaskDetails.GetType())

        Me.hdnTaskAssignmentsTask.Value = LoadTaskAssignments(oTaskDetails.IdTask)

    End Sub

    Protected Sub ASPxCallbackpanelDetailsTask_Callback(sender As Object, e As CallbackEventArgsBase) Handles ASPxCallbackpanelDetailsTask.Callback
        ' Si se utiliza
        ' carga datos generales tarea

        Dim strParameters As String = roTypes.Any2String(e.Parameter)
        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        strParameters = Server.UrlDecode(strParameters)
        Dim oTaskDetails As New TaskDetails()
        oTaskDetails = roJSONHelper.Deserialize(strParameters, oTaskDetails.GetType())

        Dim intProgress As Integer = 0
        Dim intExceeded As Integer = 0
        Dim dblEmployees As Double = 0

        If oTaskDetails.Action = "LOADTASK" Then
            LoadTaskDataX(oTaskDetails.IdTask, oTaskDetails.TimeWorked, intProgress, intExceeded, dblEmployees)

            If oTaskDetails.TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
                If oTaskDetails.BeginDate <> String.Empty And oTaskDetails.EndDate <> String.Empty Then
                    Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup, oTaskDetails.BeginDate, oTaskDetails.EndDate)
                Else
                    Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
                End If
            Else
                Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
            End If

            ASPxCallbackpanelDetailsTask.JSProperties.Add("cpResult", "OK")
            ASPxCallbackpanelDetailsTask.JSProperties.Add("cpAction", "LOADTASK")
            ASPxCallbackpanelDetailsTask.JSProperties.Add("cpID", oTaskDetails.IdTask)
        End If

        If oTaskDetails.Action = "SAVETASK" Then
            Dim oCurrentTask As roTask = Nothing

            bRet = SaveTaskDataX(oTaskDetails, responseMessage, oCurrentTask)
            If Not bRet Then
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpMessage", responseMessage)
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpAction", "SAVETASK")
                LoadTaskDataX(-1, oTaskDetails.TimeWorked, intProgress, intExceeded, dblEmployees, oCurrentTask)
            Else
                LoadTaskDataX(oCurrentTask.ID, oTaskDetails.TimeWorked, intProgress, intExceeded, dblEmployees)

                If oTaskDetails.TypeGroup = TaskStatisticsGroupByEnum._ByDate Then
                    If oTaskDetails.BeginDate <> String.Empty And oTaskDetails.EndDate <> String.Empty Then
                        Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup, oTaskDetails.BeginDate, oTaskDetails.EndDate)
                    Else
                        Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
                    End If
                Else
                    Me.hdnFieldsStatistics.Value = LoadStatisticsDataX(oTaskDetails.IdTask, oTaskDetails.TypeView, oTaskDetails.TypeGroup)
                End If
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpResult", "OK")
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpAction", "LOADTASK")
                ASPxCallbackpanelDetailsTask.JSProperties.Add("cpID", oCurrentTask.ID)
            End If

        End If

        ASPxCallbackpanelDetailsTask.JSProperties.Add("cpGridsJSON", Me.hdnFieldsStatistics.Value)
        ASPxCallbackpanelDetailsTask.JSProperties.Add("cpProgress", roTypes.Any2String(intProgress))
        ASPxCallbackpanelDetailsTask.JSProperties.Add("cpExceeded", roTypes.Any2String(intExceeded))
        ASPxCallbackpanelDetailsTask.JSProperties.Add("cpSelectedElements", roTypes.Any2String(dblEmployees))

    End Sub

    ''' <summary>
    ''' Graba la tarea
    ''' </summary>
    ''' <remarks></remarks>
    Private Function SaveTaskDataX(ByVal oTaskDetails As TaskDetails, ByRef ErrorInfo As String, ByRef oCurrentTask As roTask) As Boolean
        Dim bolret As Boolean = False

        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope)
                Return False
            End If

            Dim IsNegative As Boolean = False
            Dim bolIsNew As Boolean = False
            If oTaskDetails.IdTask = 0 Or oTaskDetails.IdTask = -1 Then
                oCurrentTask = New roTask()
                oCurrentTask.ID = -1
                oCurrentTask.Passport = WLHelperWeb.CurrentPassport.ID
            Else
                oCurrentTask = TasksServiceMethods.GetTaskByID(Nothing, oTaskDetails.IdTask, False)
            End If

            If oCurrentTask Is Nothing Then Return False

            Dim nAux As Integer = 0
            Dim sAux As String = String.Empty
            Dim AuxDate As Date = Nothing
            Dim nHour As Integer = 0
            Dim nMinute As Integer = 0

            Dim taskFieldsUsedIDs As New Generic.List(Of Integer)
            Dim taskFieldsUsedValues As New Generic.List(Of String)

            Dim taskAlertsIDs As New Generic.List(Of Integer)
            Dim taskAlertsValues As New Generic.List(Of Boolean)

            Dim taskAssigmentsIDs As New Generic.List(Of Integer)

            oCurrentTask.Name = Me.txtName.Text
            oCurrentTask.ShortName = Me.txtShortName.Text
            oCurrentTask.Description = Me.txtDescription.Text
            oCurrentTask.Color = Drawing.ColorTranslator.ToWin32(Me.dxColorPicker.Color)
            oCurrentTask.Project = Me.txtProject.Text
            oCurrentTask.IDCenter = roTypes.Any2Integer(cmbGroup.SelectedItem.Value)
            oCurrentTask.Tag = Me.txtTag.Text
            oCurrentTask.BarCode = Me.txtBarCode.Text
            oCurrentTask.Priority = roTypes.Any2Integer(TrackBarPriority.Value)

            If Not Me.txtExpectedStartDate.Date = Nothing Then
                Dim oDateTime As New DateTime
                oDateTime = Me.txtExpectedStartDate.Date
                If Not Me.txtExpectedStartTime.Value = Nothing Then
                    oDateTime = roTypes.Any2Time(roTypes.Any2Time(Me.txtExpectedStartDate.Date).NumericValue + roTypes.Any2Time(Me.txtExpectedStartTime.Value).NumericValue).Value
                End If
                oCurrentTask.ExpectedStartDate = oDateTime
            Else
                oCurrentTask.ExpectedStartDate = Nothing
            End If

            If Not Me.txtExpectedEndDate.Date = Nothing Then
                Dim oDateTime As New DateTime
                oDateTime = Me.txtExpectedEndDate.Date
                If Not Me.txtExpectedEndTime.Value = Nothing Then
                    oDateTime = roTypes.Any2Time(roTypes.Any2Time(Me.txtExpectedEndDate.Date).NumericValue + roTypes.Any2Time(Me.txtExpectedEndTime.Value).NumericValue).Value
                End If
                oCurrentTask.ExpectedEndDate = oDateTime
            Else
                oCurrentTask.ExpectedEndDate = Nothing
            End If

            Dim aux As String = ""
            Dim auxCentesimal As Double = 0

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtInitialTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.InitialTime = auxCentesimal
            Else
                aux = Me.txtInitialTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.InitialTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtTimeChangedRequirements.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.TimeChangedRequirements = auxCentesimal
            Else
                aux = Me.txtTimeChangedRequirements.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.TimeChangedRequirements = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtForecastErrorTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.ForecastErrorTime = auxCentesimal
            Else
                aux = Me.txtForecastErrorTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.ForecastErrorTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))

            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtNonProductiveTimeIncidence.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.NonProductiveTimeIncidence = auxCentesimal
            Else
                aux = Me.txtNonProductiveTimeIncidence.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.NonProductiveTimeIncidence = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtEmployeeTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.EmployeeTime = auxCentesimal
            Else
                aux = Me.txtEmployeeTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.EmployeeTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtTeamTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.TeamTime = auxCentesimal
            Else
                aux = Me.txtTeamTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.TeamTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtMaterialTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.MaterialTime = auxCentesimal
            Else
                aux = Me.txtMaterialTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.MaterialTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If bolCentesimalFormatProductiV Then
                auxCentesimal = roTypes.Any2Double(roTypes.Any2String(Me.txtOtherTime.Value).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                oCurrentTask.OtherTime = auxCentesimal
            Else
                aux = Me.txtOtherTime.Text
                IsNegative = False
                If Mid$(aux, 1, 1) = "-" Then
                    IsNegative = True
                    aux = Mid$(aux, 2)
                End If
                oCurrentTask.OtherTime = roTypes.Any2Time(aux).NumericValue * (IIf(IsNegative, -1, 1))
            End If

            If Me.optTypeCollabFirst.Checked Then
                oCurrentTask.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST
            Else
                oCurrentTask.TypeCollaboration = TaskTypeCollaborationEnum._ANY
            End If

            If Me.optActivAllways.Checked Then
                oCurrentTask.TypeActivation = TaskTypeActivationEnum._ALWAYS
            End If

            If Me.optActivByDate.Checked Then
                oCurrentTask.TypeActivation = TaskTypeActivationEnum._ATDATE
                If Not Me.txtActivationDate.Date = Nothing Then
                    Dim oDateTime As New DateTime
                    oDateTime = Me.txtActivationDate.Date
                    If Not Me.txtActivationTime.Value = Nothing Then
                        oDateTime = roTypes.Any2Time(roTypes.Any2Time(Me.txtActivationDate.Date).NumericValue + roTypes.Any2Time(Me.txtActivationTime.Value).NumericValue).Value
                    End If
                    oCurrentTask.ActivationDate = oDateTime
                Else
                    oCurrentTask.ActivationDate = Nothing
                    oCurrentTask.TypeActivation = TaskTypeActivationEnum._ALWAYS
                End If

            End If

            If Me.optActivByEndTask.Checked Then
                oCurrentTask.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK
            End If

            If Me.optActivByIniTask.Checked Then
                oCurrentTask.TypeActivation = TaskTypeActivationEnum._ATRUNTASK
            End If

            If Me.optActivByEndTask.Checked Or Me.optActivByIniTask.Checked Then
                oCurrentTask.ActivationTask = roTypes.Any2Integer(Me.hdnActivationTask.Value)
            Else
                oCurrentTask.ActivationTask = 0
            End If

            If oCurrentTask.ActivationTask = 0 And (Me.optActivByEndTask.Checked Or Me.optActivByIniTask.Checked) Then
                oCurrentTask.TypeActivation = TaskTypeActivationEnum._ALWAYS
            End If

            If Me.optClosingByDate.Checked Then
                oCurrentTask.TypeClosing = TaskTypeClosingEnum._ATDATE
                If Not Me.txtClosingDate.Date = Nothing Then
                    Dim oDateTime As New DateTime
                    oDateTime = Me.txtClosingDate.Date
                    If Not Me.txtClosingTime.Value = Nothing Then
                        oDateTime = roTypes.Any2Time(roTypes.Any2Time(Me.txtClosingDate.Date).NumericValue + roTypes.Any2Time(Me.txtClosingTime.Value).NumericValue).Value
                    End If
                    oCurrentTask.ClosingDate = oDateTime
                Else
                    oCurrentTask.ClosingDate = Nothing
                    oCurrentTask.TypeClosing = TaskTypeClosingEnum._UNDEFINED
                End If
            Else
                oCurrentTask.TypeClosing = TaskTypeClosingEnum._UNDEFINED
            End If

            If Me.optAutEmpAll.Checked Then
                oCurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
            Else
                oCurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
            End If

            If Me.optColabAllEmp.Checked Then
                oCurrentTask.ModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
            Else
                oCurrentTask.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME
            End If

            oCurrentTask.TimeRemarks = Me.txtTimeRemarks.Text

            oCurrentTask.Status = TaskStatusEnum._ON

            If ASPxbtnSetEnded.Checked Then
                oCurrentTask.Status = TaskStatusEnum._COMPLETED
            End If

            If ASPxbtnSetCanceled.Checked Then
                oCurrentTask.Status = TaskStatusEnum._CANCELED
            End If

            If Me.ASPxbtnSetPendienteVal.Visible Then
                If ASPxbtnSetPendienteVal.Checked Then
                    oCurrentTask.Status = TaskStatusEnum._PENDING
                End If
            End If

            If Not oTaskDetails.gridAlerts Is Nothing Then
                For Each oAlert As AlertDetails In oTaskDetails.gridAlerts
                    taskAlertsIDs.Add(roTypes.Any2Integer(oAlert.id))
                    taskAlertsValues.Add(IIf(oAlert.Readed.ToLower = "true", True, False))
                Next
            End If

            If Not oTaskDetails.gridFields Is Nothing Then
                For Each oField As FieldDetails In oTaskDetails.gridFields
                    taskFieldsUsedIDs.Add(oField.idfield)
                    taskFieldsUsedValues.Add(oField.value)
                Next
            End If

            If Not oTaskDetails.gridAssigments Is Nothing Then
                For Each oAssignment As AssigmentDetails In oTaskDetails.gridAssigments
                    taskAssigmentsIDs.Add(roTypes.Any2Integer(oAssignment.IDAssignment))
                Next
            End If

            'Obtener empleados y grupos seleccionados en la tarea
            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpDetailTaskGrid")
            If oTreeState.Selected1 <> String.Empty Then

                Dim oArrEmployees As New Generic.List(Of roEmployeeTaskDescription)
                Dim oArrGroups As New Generic.List(Of roGroupTaskDescription)

                Dim arr() As String = oTreeState.Selected1.Split(",")
                For Each item As String In arr
                    If item.IndexOf("A") = 0 Then
                        oArrGroups.Add(New roGroupTaskDescription() With {.ID = item.Substring(1)})
                    ElseIf item.IndexOf("B") = 0 Then
                        oArrEmployees.Add(New roEmployeeTaskDescription() With {.ID = item.Substring(1)})
                    End If
                Next

                oCurrentTask.Employees = oArrEmployees
                oCurrentTask.Groups = oArrGroups
            Else
                oCurrentTask.Employees = Nothing
                oCurrentTask.Groups = Nothing
                If oCurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._ASSIGNMENTEMPLOYEES Or oCurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES Then
                    oCurrentTask.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
                End If

            End If

            'Guardar Task
            If TasksServiceMethods.SaveTask(Me, oCurrentTask, True) = True Then
                Dim tFields As New Generic.List(Of roTaskField)
                Dim bolSaved As Boolean = True

                Dim fieldsDT As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField, "")

                For index As Integer = 0 To taskFieldsUsedIDs.Count - 1
                    Dim definitionRow As DataRow = Nothing
                    For Each cRow As DataRow In fieldsDT.Rows
                        If (roTypes.Any2Integer(cRow("ID")) = taskFieldsUsedIDs(index)) Then
                            definitionRow = cRow
                        End If
                    Next
                    If (definitionRow IsNot Nothing) Then
                        Dim tField As New roTaskField
                        tField.IDTask = oCurrentTask.ID
                        tField.IDField = definitionRow("ID")
                        tField.Type = definitionRow("Type")
                        tField.FieldName = definitionRow("Name")
                        tField.Action = definitionRow("Action")
                        tField.FieldValue = taskFieldsUsedValues(index)
                        tFields.Add(tField)
                    End If
                Next

                If (API.TasksServiceMethods.SaveTaskFields(Me.Page, oCurrentTask.ID, tFields) = False) Then
                    bolSaved = False
                End If

                Dim tAlerts As New Generic.List(Of roTaskAlert)
                Dim alertsDT As DataTable = API.TasksServiceMethods.GetTaskAlertsDataTable(Me.Page, oCurrentTask.ID)

                For index As Integer = 0 To taskAlertsIDs.Count - 1
                    Dim definitionRow As DataRow = Nothing
                    For Each cRow As DataRow In alertsDT.Rows
                        If (roTypes.Any2Integer(cRow("ID")) = taskAlertsIDs(index)) Then
                            definitionRow = cRow
                        End If
                    Next
                    If (definitionRow IsNot Nothing) Then
                        Dim tAlert As New roTaskAlert

                        tAlert.ID = definitionRow("ID")
                        tAlert.IDTask = oCurrentTask.ID
                        tAlert.Comment = definitionRow("Comment")
                        tAlert.DateTime = definitionRow("datetime")
                        tAlert.IDEmployee = definitionRow("idemployee")
                        tAlert.IsReaded = taskAlertsValues(index)
                        tAlerts.Add(tAlert)
                    End If
                Next

                If (API.TasksServiceMethods.SaveTaskAlerts(Me.Page, oCurrentTask.ID, tAlerts) = False) Then
                    bolSaved = False
                End If

                Dim tAssigments As New Generic.List(Of roTaskAssignment)
                Dim AssigmentsDT As DataTable = API.TasksServiceMethods.GetAssignments(Me.Page, oCurrentTask.ID, False)

                For index As Integer = 0 To taskAssigmentsIDs.Count - 1
                    Dim definitionRow As DataRow = Nothing
                    For Each cRow As DataRow In AssigmentsDT.Rows
                        If (roTypes.Any2Integer(cRow("IDAssignment")) = taskAssigmentsIDs(index)) Then
                            definitionRow = cRow
                        End If
                    Next
                    If (definitionRow IsNot Nothing) Then
                        Dim tAssigment As New roTaskAssignment

                        tAssigment.IDAssignment = definitionRow("IDAssignment")
                        tAssigment.IDTask = oCurrentTask.ID
                        tAssigments.Add(tAssigment)
                    End If
                Next

                If (API.TasksServiceMethods.SaveTaskAssignments(Me.Page, oCurrentTask.ID, tAssigments) = False) Then
                    bolSaved = False
                End If

                If bolSaved Then
                    bolret = True
                Else
                    ErrorInfo = TasksServiceMethods.LastErrorText
                    bolret = False
                End If
            Else
                ErrorInfo = TasksServiceMethods.LastErrorText
                bolret = False
            End If
        Catch ex As Exception
            ErrorInfo = ex.Message.ToString & " " & ex.StackTrace
            bolret = False
        End Try

        Return bolret
    End Function

End Class