Imports DevExpress.Web
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Tasks_TaskEmployeeStatus
    Inherits PageBase

#Region "Properties"

    Private Property CurrentEmployees(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim CurrentEmployeesData As DataTable = Session("TaskEmployeeStatus_CurrentEmployees")

            If bolReload OrElse CurrentEmployeesData Is Nothing Then
                CurrentEmployeesData = API.TasksServiceMethods.GetEmployeesWorkingInTaskList(Me.Page, IdTask)
                Session("TaskEmployeeStatus_CurrentEmployees") = CurrentEmployeesData
            End If

            Return CurrentEmployeesData

        End Get

        Set(ByVal value As DataTable)
            Session("TaskEmployeeStatus_CurrentEmployees") = value
        End Set
    End Property

    Private Property PastEmployees(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim PastEmployeesData As DataTable = Session("TaskEmployeeStatus_PastEmployees")

            If bolReload OrElse PastEmployeesData Is Nothing Then
                PastEmployeesData = API.TasksServiceMethods.GetEmployeesWorkedInTaskList(Me.Page, IdTask)
                Session("TaskEmployeeStatus_PastEmployees") = PastEmployeesData
            End If

            Return PastEmployeesData

        End Get

        Set(ByVal value As DataTable)
            Session("TaskEmployeeStatus_PastEmployees") = value
        End Set
    End Property

    Public Property IdTask() As Integer
        Get
            Return Session("TaskEmployeeStatus_IDTask")
        End Get

        Set(ByVal value As Integer)
            Session("TaskEmployeeStatus_IDTask") = value
        End Set
    End Property

#End Region

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then

            CreateColumnsCurrentEmployees()
            CreateColumnsPastEmployees()

            Me.IdTask = roTypes.Any2Integer(Request("TaskId"))

            Me.GridCurrentEmployees.Settings.ShowTitlePanel = True
            Me.GridPastEmployees.Settings.ShowTitlePanel = True
            Me.hdnIDTaskSelected.Value = Me.IdTask

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "window.parent.parent.showLoader(false);", True)
        End If

        BindGridCurrentEmployees(True)
        BindGridPastEmployees(True)
    End Sub

#Region "Current Employees"

    Private Sub BindGridCurrentEmployees(ByVal bolReload As Boolean)
        Me.GridCurrentEmployees.DataSource = Me.CurrentEmployees(bolReload)
        Me.GridCurrentEmployees.DataBind()
    End Sub

    Private Sub CreateColumnsCurrentEmployees()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        'Me.GridDocumentsTracking.SettingsBehavior.ConfirmDelete = True
        Me.GridCurrentEmployees.EnableRowsCache = False
        Me.GridCurrentEmployees.Columns.Clear()
        Me.GridCurrentEmployees.KeyFieldName = "ID"
        Me.GridCurrentEmployees.Settings.VerticalScrollableHeight = 100
        Me.GridCurrentEmployees.SettingsPager.PageSize = 4
        Me.GridCurrentEmployees.SettingsText.EmptyDataRow = Me.Language.Translate("NoEmployeesWorking", DefaultScope)
        Me.GridCurrentEmployees.SettingsText.Title = Me.Language.Translate("NoEmployeesWorking.Title", DefaultScope)
        Me.GridCurrentEmployees.SettingsEditing.Mode = GridViewEditingMode.Inline
        Me.GridCurrentEmployees.SettingsBehavior.AllowSort = False

        'ID (Clave)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridCurrentEmployees.Columns.Add(GridColumn)

        'EmployeeName
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridCurrentEmployees.Column.EmployeeName", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 65
        Me.GridCurrentEmployees.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridCurrentEmployees.Column.PunchDate", DefaultScope)
        GridColumnDate.FieldName = "PunchDateTime"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 49
        Me.GridCurrentEmployees.Columns.Add(GridColumnDate)

        'MovesNew
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

        Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        employeeButton.ID = "ShowPunchEmployee"
        employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Empleado-16x16.gif"
        employeeButton.Text = Me.Language.Translate("GridCurrentEmployees.Column.ShowCurrentPunch", DefaultScope) 'Empleados"

        GridColumnCommand.CustomButtons.Add(employeeButton)
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 18
        VisibleIndex = VisibleIndex + 1

        Me.GridCurrentEmployees.Columns.Add(GridColumnCommand)

    End Sub

#End Region

#Region "Past Employees"

    Private Sub BindGridPastEmployees(ByVal bolReload As Boolean)
        Me.GridPastEmployees.DataSource = Me.PastEmployees(bolReload)
        Me.GridPastEmployees.DataBind()
    End Sub

    Private Sub CreateColumnsPastEmployees()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        'Me.GridDocumentsTracking.SettingsBehavior.ConfirmDelete = True
        Me.GridPastEmployees.EnableRowsCache = False
        Me.GridPastEmployees.Columns.Clear()
        Me.GridPastEmployees.KeyFieldName = "ID"
        Me.GridPastEmployees.Settings.VerticalScrollableHeight = 100
        Me.GridPastEmployees.SettingsPager.PageSize = 4
        Me.GridPastEmployees.SettingsText.EmptyDataRow = Me.Language.Translate("NoEmployeesWorked", DefaultScope)
        Me.GridPastEmployees.SettingsText.Title = Me.Language.Translate("NoEmployeesWorked.Title", DefaultScope)
        Me.GridPastEmployees.SettingsEditing.Mode = GridViewEditingMode.Inline
        Me.GridPastEmployees.SettingsBehavior.AllowSort = False

        'ID (Clave)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridPastEmployees.Columns.Add(GridColumn)

        'EmployeeName
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridPastEmployees.Column.EmployeeName", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 65
        Me.GridPastEmployees.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridPastEmployees.Column.PunchDate", DefaultScope)
        GridColumnDate.FieldName = "PunchDateTime"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 49
        Me.GridPastEmployees.Columns.Add(GridColumnDate)

        'MovesNew
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

        Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        employeeButton.ID = "ShowPunchEmployee"
        employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Empleado-16x16.gif"
        employeeButton.Text = Me.Language.Translate("GridPastEmployees.Column.ShowLastPunch", DefaultScope) 'Fichajes"

        GridColumnCommand.CustomButtons.Add(employeeButton)
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 18
        VisibleIndex = VisibleIndex + 1

        Me.GridPastEmployees.Columns.Add(GridColumnCommand)
    End Sub

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("roTaskStatus", "~/Tasks/Scripts/TaskStatus.js")
    End Sub

End Class