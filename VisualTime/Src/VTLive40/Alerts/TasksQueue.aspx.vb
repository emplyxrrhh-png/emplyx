Imports DevExpress.Web
Imports DevExpress.Web.Data
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class TasksQueue
    Inherits PageBase

    Private Const FeatureAlias As String = "Administration.TasksQueue"

    Private Property WaitingTasks(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim solutions As Object = Session("TasksQueue_WaitingTasks")

            If bolReload OrElse solutions Is Nothing Then
                solutions = API.LiveTasksServiceMethods.GetReportsTaksStatus(Me.Page, roLiveTaskStatus.Stopped, If(Me.oPermission = Permission.Admin, False, True))

                If solutions IsNot Nothing Then
                    solutions.PrimaryKey = New DataColumn() {solutions.Columns("ID")}
                    solutions.AcceptChanges()
                End If

                Session("TasksQueue_WaitingTasks") = solutions
            End If
            Return solutions
        End Get
        Set(value As DataTable)
            Session("TasksQueue_WaitingTasks") = value
        End Set
    End Property

    Private Property CompletedTasks(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim solutions As Object = Session("TasksQueue_CompletedTasks")

            If bolReload OrElse solutions Is Nothing Then
                solutions = API.LiveTasksServiceMethods.GetReportsTaksStatus(Me.Page, roLiveTaskStatus.Finished, If(Me.oPermission = Permission.Admin, False, True))

                If solutions IsNot Nothing Then
                    solutions.PrimaryKey = New DataColumn() {solutions.Columns("ID")}
                    solutions.AcceptChanges()
                End If
                Session("TasksQueue_CompletedTasks") = solutions
            End If
            Return solutions
        End Get
        Set(value As DataTable)
            Session("TasksQueue_CompletedTasks") = value
        End Set
    End Property

    Private Property RunningTasks(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim solutions As Object = Session("TasksQueue_RunningTasks")

            If bolReload OrElse solutions Is Nothing Then
                solutions = API.LiveTasksServiceMethods.GetReportsTaksStatus(Me.Page, roLiveTaskStatus.Running, If(Me.oPermission = Permission.Admin, False, True))

                If solutions IsNot Nothing Then
                    solutions.PrimaryKey = New DataColumn() {solutions.Columns("ID")}
                    solutions.AcceptChanges()
                End If

                Session("TasksQueue_RunningTasks") = solutions
            End If
            Return solutions
        End Get
        Set(value As DataTable)
            Session("TasksQueue_RunningTasks") = value
        End Set
    End Property

    Private Function LoadUsersCombo(Optional ByVal bolReload As Boolean = False) As DataView

        Dim tb As DataTable = Session("TasksQueue_AuditUserNames")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.UserAdminServiceMethods.GetAuditPassports(Me.Page)
            If tb IsNot Nothing Then
                Session("TasksQueue_AuditUserNames") = tb
                dv = New DataView(tb)
                dv.Sort = "UserName ASC"
            End If
            Return dv
        Else
            dv = New DataView(tb)
            dv.Sort = "UserName ASC"
            Return dv
        End If
    End Function

#Region "Declarations"

    Private oPermission As Permission

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Options", "~/Options/Scripts/Options.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("tasksqueue", "~/Alerts/Scripts/TasksQueue.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Si el passport actual no tiene permisso de lectura, rediriguimos a pàgina de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If
        Me.IsSaas.Value = "True"
        Me.GeniusURI.Value = "/" + Configuration.RootUrl + "//Genius"
        Me.AccessURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Access/Analytics"
        Me.ScheduleAnalyticURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Scheduler/AnalyticsScheduler"
        Me.CostCenterAnalyticURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Scheduler/AnalyticsCostControl"
        Me.TaskAnalyticURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Tasks/Analytics"

        If Not Me.IsPostBack Then
            CreateColumnsGrids()

            BindGrids(True)
            LoadUsersCombo(True)
            Me.SetPermissions()
        Else
            BindGrids(False)
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub CreateColumnsGrids()
        CreateColumnsRunningGrid()
        CreateColumnsCompletedGrid()
        CreateColumnsWaitingGrid()
    End Sub

    Private Sub BindGrids(ByVal bolReload As Boolean)
        Me.gridRunning.DataSource = Me.RunningTasks(bolReload)
        Me.gridRunning.DataBind()

        Me.gridCompleted.DataSource = Me.CompletedTasks(bolReload)
        Me.gridCompleted.DataBind()

        Me.gridWaiting.DataSource = Me.WaitingTasks(bolReload)
        Me.gridWaiting.DataBind()
    End Sub

    Private Sub CreateColumnsRunningGrid()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.gridRunning.Columns.Clear()
        Me.gridRunning.KeyFieldName = "ID"
        Me.gridRunning.SettingsText.EmptyDataRow = " "
        Me.gridRunning.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.gridRunning.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.gridRunning.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Action", DefaultScope)
        GridColumn.FieldName = "ActionType"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridRunning.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Name", DefaultScope)
        GridColumn.FieldName = "ActionName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridRunning.Columns.Add(GridColumn)

        'User
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridSolutions.Column.User", DefaultScope)
        GridColumnCombo.FieldName = "IDPassport"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        GridColumnCombo.PropertiesComboBox.TextField = "UserName"
        GridColumnCombo.PropertiesComboBox.ValueField = "UserID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(String)
        GridColumnCombo.PropertiesComboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 35
        Me.gridRunning.Columns.Add(GridColumnCombo)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridSolutions.Column.CreationDate", DefaultScope)
        GridColumnDate.FieldName = "TimeStamp"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        'GridColumnDate.SortIndex = 0
        'GridColumnDate.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        Me.gridRunning.Columns.Add(GridColumnDate)

        'Hora
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.StartTime", DefaultScope)
        GridColumn.FieldName = "iHour"
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
        Me.gridRunning.Columns.Add(GridColumn)
    End Sub

    Private Sub CreateColumnsCompletedGrid()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim CustomButton As GridViewCommandColumnCustomButton
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.gridCompleted.Columns.Clear()
        Me.gridCompleted.KeyFieldName = "ID"
        Me.gridCompleted.SettingsText.EmptyDataRow = " "
        Me.gridCompleted.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.gridCompleted.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "TaskID"
        GridColumn.FieldName = "TaskID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "AnalyticType"
        GridColumn.FieldName = "AnalyticType"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ViewID"
        GridColumn.FieldName = "ViewID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Action", DefaultScope)
        GridColumn.FieldName = "ActionType"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Name", DefaultScope)
        GridColumn.FieldName = "ActionName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Name", DefaultScope)
        GridColumn.FieldName = "Action"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.gridCompleted.Columns.Add(GridColumn)

        'User
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridSolutions.Column.User", DefaultScope)
        GridColumnCombo.FieldName = "IDPassport"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        GridColumnCombo.PropertiesComboBox.TextField = "UserName"
        GridColumnCombo.PropertiesComboBox.ValueField = "UserID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(String)
        GridColumnCombo.PropertiesComboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 35
        Me.gridCompleted.Columns.Add(GridColumnCombo)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridSolutions.Column.CreationDate", DefaultScope)
        GridColumnDate.FieldName = "TimeStamp"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        Me.gridCompleted.Columns.Add(GridColumnDate)

        'Hora
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.StartTime", DefaultScope)
        GridColumn.FieldName = "iHour"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Width = 30
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.EndTime", DefaultScope)
        GridColumn.FieldName = "eHour"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Width = 30
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.EndStatus", DefaultScope)
        GridColumn.FieldName = "EndStatus"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridCompleted.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridSolutions.Column.DeleteDate", DefaultScope)
        GridColumnDate.FieldName = "DeleteDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.UnboundType = DevExpress.Data.UnboundColumnType.DateTime
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        Me.gridCompleted.Columns.Add(GridColumnDate)

        If Me.oPermission > Permission.Read Then
            'Command Column Ficheros Adjuntos
            GridColumnCommand = New GridViewCommandColumn(Me.Language.Translate("GridSolutions.Column.DownloadFile", DefaultScope))
            GridColumnCommand.ShowClearFilterButton = False
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.Width = 50
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Visible = True

            CustomButton = New GridViewCommandColumnCustomButton() 'Descargar fichero
            CustomButton.ID = "SeeAttachFileButton"
            CustomButton.Image.Url = "Images/AttachmentSee.png"
            CustomButton.Image.ToolTip = Me.Language.Translate("GridSolutions.Column.DownloadFile", DefaultScope)
            CustomButton.Image.Height = New Unit(16)
            CustomButton.Image.Width = New Unit(16)
            CustomButton.Image.SpriteProperties.Left = New Unit(5)
            GridColumnCommand.CustomButtons.Add(CustomButton)

            GridColumnCommand.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            Me.gridCompleted.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Private Sub CreateColumnsWaitingGrid()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnImage As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.gridWaiting.Columns.Clear()
        Me.gridWaiting.KeyFieldName = "ID"
        Me.gridWaiting.SettingsText.EmptyDataRow = " "
        Me.gridWaiting.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.gridWaiting.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.gridWaiting.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Action", DefaultScope)
        GridColumn.FieldName = "ActionType"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridWaiting.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridSolutions.Column.Name", DefaultScope)
        GridColumn.FieldName = "ActionName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 30
        Me.gridWaiting.Columns.Add(GridColumn)

        'User
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridSolutions.Column.User", DefaultScope)
        GridColumnCombo.FieldName = "IDPassport"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        GridColumnCombo.PropertiesComboBox.TextField = "UserName"
        GridColumnCombo.PropertiesComboBox.ValueField = "UserID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(String)
        GridColumnCombo.PropertiesComboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 35
        Me.gridWaiting.Columns.Add(GridColumnCombo)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridSolutions.Column.CreationDate", DefaultScope)
        GridColumnDate.FieldName = "TimeStamp"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        'GridColumnDate.SortIndex = 0
        'GridColumnDate.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        Me.gridWaiting.Columns.Add(GridColumnDate)

        'Delete
        GridColumnImage = New GridViewCommandColumn()
        GridColumnImage.Caption = Me.Language.Translate("GridSolutions.Column.Delete", DefaultScope)
        GridColumnImage.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.ShowDeleteButton = False
        GridColumnImage.ShowEditButton = False
        GridColumnImage.ShowCancelButton = False
        GridColumnImage.ShowUpdateButton = False
        GridColumnImage.Width = 20
        VisibleIndex = VisibleIndex + 1
        Me.gridWaiting.Columns.Add(GridColumnImage)
    End Sub

    Protected Sub gridCompleted_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles gridCompleted.CustomUnboundColumnData
        ProcessUnboundEvent(sender, e)
    End Sub

    Protected Sub gridWaiting_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles gridWaiting.CustomUnboundColumnData
        ProcessUnboundEvent(sender, e)
    End Sub

    Protected Sub gridRunning_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles gridRunning.CustomUnboundColumnData
        ProcessUnboundEvent(sender, e)
    End Sub

    Private Sub ProcessUnboundEvent(ByRef sender As Object, ByRef e As ASPxGridViewColumnDataEventArgs)

        'GridColumn = New GridViewDataTextColumn()
        'GridColumn.Caption = "AnalyticType"
        'GridColumn.FieldName = "AnalyticType"
        'GridColumn.ReadOnly = True
        'GridColumn.Visible = False
        'Me.gridCompleted.Columns.Add(GridColumn)

        'GridColumn = New GridViewDataTextColumn()
        'GridColumn.Caption = "ViewID"
        'GridColumn.FieldName = "ViewID"
        'GridColumn.ReadOnly = True
        'GridColumn.Visible = False
        'Me.gridCompleted.Columns.Add(GridColumn)

        Select Case e.Column.FieldName
            Case "ActionType"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("Action")) AndAlso e.GetListSourceFieldValue("Action") IsNot Nothing Then
                        If e.GetListSourceFieldValue("Action").ToString().ToUpper = "REPORT" Then
                            e.Value = Me.Language.Translate("GridSolutions.ActionType.Report", Me.DefaultScope)
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "IMPORT" Then
                            e.Value = Me.Language.Translate("GridSolutions.ActionType.Import", Me.DefaultScope)
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "EXPORT" Then
                            e.Value = Me.Language.Translate("GridSolutions.ActionType.Export", Me.DefaultScope)
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "ANALYTICSTASK" Then
                            e.Value = Me.Language.Translate("GridSolutions.ActionType.Analytic", Me.DefaultScope)
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "REPORTTASKDX" Then
                            e.Value = Me.Language.Translate("GridSolutions.ActionType.Report", Me.DefaultScope)
                        End If
                    End If
                End If
            Case "ViewID"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("Name")) AndAlso e.GetListSourceFieldValue("Name") IsNot Nothing Then
                        If e.GetListSourceFieldValue("Action").ToString().ToUpper = "ANALYTICSTASK" Then
                            Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(e.GetListSourceFieldValue("Name")))
                            e.Value = roTypes.Any2Integer(oCollection("IdView"))
                        Else
                            e.Value = ""
                        End If
                    End If
                End If
            Case "AnalyticType"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("Name")) AndAlso e.GetListSourceFieldValue("Name") IsNot Nothing Then
                        If e.GetListSourceFieldValue("Action").ToString().ToUpper = "ANALYTICSTASK" Then
                            Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(e.GetListSourceFieldValue("Name")))
                            e.Value = roTypes.Any2Integer(oCollection("analyticType"))
                        Else
                            e.Value = ""
                        End If
                    End If
                End If
            Case "ActionName"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("Name")) AndAlso e.GetListSourceFieldValue("Name") IsNot Nothing Then
                        If e.GetListSourceFieldValue("Action").ToString().ToUpper = "IMPORT" OrElse e.GetListSourceFieldValue("Action").ToString().ToUpper = "EXPORT" Then
                            Try
                                Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(e.GetListSourceFieldValue("Name")))
                                If oCollection.Exists("IDImport") Then
                                    e.Value = API.DataLinkServiceMethods.GetImportGuide(Me.Page, oCollection("IDImport"), False).Name
                                ElseIf oCollection.Exists("IDExport") Then
                                    Select Case roTypes.Any2Integer(oCollection("IDExport"))
                                        Case 0
                                            e.Value = Me.Language.Translate("ExportStandardXML", DefaultScope)
                                        Case 8000
                                            e.Value = Me.Language.Translate("ExportSageMurano", DefaultScope)
                                        Case Else
                                            e.Value = API.DataLinkServiceMethods.GetExportGuide(Me.Page, oCollection("IDExport"), False).Name
                                    End Select
                                End If
                            Catch ex As Exception
                                e.Value = "Name not found"
                            End Try
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "PLANNEDREPORT" Then
                            Dim strTarget = Me.Language.Translate("Report.Target", DefaultScope)

                            e.Value = e.GetListSourceFieldValue("Name").Replace("@@TARGET@@", strTarget)

                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "ANALYTICSTASK" Then
                            Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(e.GetListSourceFieldValue("Name")))

                            Dim analyticType As Integer = roTypes.Any2Integer(oCollection("analyticType"))
                            Dim analyticView As Integer = roTypes.Any2Integer(oCollection("IdView"))
                            If oCollection("GeniusViewName") IsNot Nothing Then
                                e.Value = oCollection("GeniusViewName")
                            Else
                                e.Value = Me.Language.Translate("Analytic.Name." & analyticType & "." & analyticView, DefaultScope)
                            End If
                        ElseIf e.GetListSourceFieldValue("Action").ToString().ToUpper = "REPORTTASKDX" Then
                            Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(e.GetListSourceFieldValue("Name")))
                            Dim iReportID As Integer = roTypes.Any2Integer(oCollection("ReportId"))
                            Dim oReport = API.ReportServiceMethods.GetReportById(iReportID, Robotics.Base.ReportPermissionTypes.Read, Me.Page)
                            If oReport IsNot Nothing Then
                                e.Value = oReport.Name
                            Else
                                e.Value = "N/A"
                            End If
                        Else
                            e.Value = roTypes.Any2String(e.GetListSourceFieldValue("Name"))
                        End If
                    End If
                End If
            Case "iHour"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("ExecutionDate")) Then
                        If e.GetListSourceFieldValue("ExecutionDate") Is Nothing Then
                            e.Value = String.Empty
                        Else
                            e.Value = CType(e.GetListSourceFieldValue("ExecutionDate"), Date).ToString("HH:mm:ss")
                        End If
                    End If
                End If
            Case "eHour"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("EndDate")) Then
                        If e.GetListSourceFieldValue("EndDate") Is Nothing Then
                            e.Value = String.Empty
                        Else
                            e.Value = CType(e.GetListSourceFieldValue("EndDate"), Date).ToString("HH:mm:ss")
                        End If
                    End If
                End If
            Case "EndStatus"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("Status")) AndAlso e.GetListSourceFieldValue("Status") IsNot Nothing Then
                        If roTypes.Any2Integer(e.GetListSourceFieldValue("Status")) = 3 Then
                            e.Value = Me.Language.Translate("GridSolutions.Status.EndWithErrors", Me.DefaultScope)
                        ElseIf roTypes.Any2Integer(e.GetListSourceFieldValue("Status")) = 2 Then
                            e.Value = Me.Language.Translate("GridSolutions.Status.EndOk", Me.DefaultScope)
                        End If
                    End If
                End If
            Case "DeleteDate"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("TimeStamp")) AndAlso e.GetListSourceFieldValue("TimeStamp") IsNot Nothing Then
                        Dim oAdv As String = HelperSession.AdvancedParametersCache("ReportsPersistOnSystem")
                        If oAdv <> String.Empty Then
                            e.Value = CType(e.GetListSourceFieldValue("TimeStamp"), Date).AddDays(roTypes.Any2Integer(oAdv)).ToString("dd/MM/yyyy")
                        End If
                    End If
                End If
        End Select
    End Sub

    Protected Sub gridCompletedg_CustomButtonInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomButtonEventArgs) Handles gridCompleted.CustomButtonInitialize
        If e.ButtonID = "SeeAttachFileButton" Then
            Dim IDPassport As Integer = roTypes.Any2Integer(gridCompleted.GetRowValues(e.VisibleIndex, "IDPassport"))
            Dim params = gridCompleted.GetRowValues(e.VisibleIndex, "Name")
            Dim isDownloadBI As Boolean = False
            If Not IsDBNull(params) AndAlso params IsNot Nothing Then
                If gridCompleted.GetRowValues(e.VisibleIndex, "Action").ToString().ToUpper = "ANALYTICSTASK" Then
                    Dim oCollection As New Robotics.VTBase.roCollection(roTypes.Any2String(params))
                    isDownloadBI = roTypes.Any2Boolean(oCollection("DownloadBI")) And HelperSession.GetFeatureIsInstalledFromApplication("Feature\BIIntegration")
                End If
            End If
            If WLHelperWeb.CurrentPassport Is Nothing OrElse IDPassport <> WLHelperWeb.CurrentPassport.ID OrElse roTypes.Any2Integer(gridCompleted.GetRowValues(e.VisibleIndex, "Status")) = 3 OrElse isDownloadBI Then
                e.Visible = DevExpress.Utils.DefaultBoolean.False
            End If
        End If
    End Sub

    Private Sub SetPermissions()
        If Me.oPermission = Permission.None Then
            Me.TABBUTTON_TasksQueueInfo.Style("display") = "none"
            Me.tbLicenceInfo.Visible = False
        ElseIf Me.oPermission < Permission.Write Then
            Me.DisableControls(Me.tbLicenceInfo.Controls)
        End If
    End Sub

#End Region

    Private Sub gridCompleted_DataBinding(sender As Object, e As EventArgs) Handles gridCompleted.DataBinding

        '  gridCompleted.DataSource = CompletedTasks(True)

        Dim oCombo As GridViewDataComboBoxColumn = gridCompleted.Columns("IDPassport")
        oCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        oCombo.PropertiesComboBox.TextField = "UserName"
        oCombo.PropertiesComboBox.ValueField = "UserID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)

    End Sub

    Private Sub gridRunning_DataBinding(sender As Object, e As EventArgs) Handles gridRunning.DataBinding
        ' gridRunning.DataSource = RunningTasks(True)

        Dim oCombo As GridViewDataComboBoxColumn = gridRunning.Columns("IDPassport")
        oCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        oCombo.PropertiesComboBox.TextField = "UserName"
        oCombo.PropertiesComboBox.ValueField = "UserID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)

    End Sub

    Private Sub gridWaiting_DataBinding(sender As Object, e As EventArgs) Handles gridWaiting.DataBinding
        '  gridWaiting.DataSource = WaitingTasks(True)

        Dim oCombo As GridViewDataComboBoxColumn = gridWaiting.Columns("IDPassport")
        oCombo.PropertiesComboBox.DataSource = LoadUsersCombo()
        oCombo.PropertiesComboBox.TextField = "UserName"
        oCombo.PropertiesComboBox.ValueField = "UserID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Private Sub gridWaiting_RowDeleting(sender As Object, e As ASPxDataDeletingEventArgs) Handles gridWaiting.RowDeleting
        Dim tb As DataTable = Me.WaitingTasks()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(gridWaiting.KeyFieldName))
        If dr IsNot Nothing Then
            If API.LiveTasksServiceMethods.DeleteLiveTask(Me.Page, dr("TaskID"), dr("Action")) Then
                dr.Delete()
                tb.AcceptChanges()
            Else
                Throw New Exception(Me.Language.Translate("Error.DeleteMessage", Me.DefaultScope))
            End If
            Me.WaitingTasks = tb
            e.Cancel = True
        End If
    End Sub

    Private Sub gridWaiting_CommandButtonInitialize(sender As Object, e As ASPxGridViewCommandButtonEventArgs) Handles gridWaiting.CommandButtonInitialize

        Dim row = gridWaiting.GetRow(e.VisibleIndex)

        If DirectCast(row, System.Data.DataRowView).Row("Action") = "Import" Then
            e.Visible = False
        ElseIf DirectCast(row, System.Data.DataRowView).Row("Action") = "Export" Then
            e.Visible = False
        Else
            e.Visible = True
        End If

    End Sub

    Private Sub gridCompleted_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles gridCompleted.CustomCallback
        If e.Parameters.ToUpper = "REFRESH" Then
            gridCompleted.DataSource = CompletedTasks(True)
            BindGrids(False)
        End If
    End Sub

    Private Sub GridRunning_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles gridRunning.CustomCallback
        If e.Parameters.ToUpper = "REFRESH" Then
            gridRunning.DataSource = RunningTasks(True)
            BindGrids(False)
        End If
    End Sub

    Private Sub gridWaiting_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles gridWaiting.CustomCallback
        If e.Parameters.ToUpper = "REFRESH" Then
            gridWaiting.DataSource = WaitingTasks(True)
            BindGrids(False)
        End If
    End Sub

End Class