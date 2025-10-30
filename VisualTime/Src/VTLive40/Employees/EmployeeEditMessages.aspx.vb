Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class EmployeeEditMessages
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees"

#Region "Declarations"

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private bCanEdit As Boolean = True

#End Region

#Region "Properties"

    Private Property MessagesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_EmployeesMessages")

            If bolReload OrElse tb Is Nothing Then
                tb = API.EmployeeServiceMethods.GetTerminalMessages(Me.Page, intIDEmployee)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("Employees_EmployeesMessages") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_EmployeesMessages") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.bCanEdit = oPermission > Permission.Read
        If Me.oPermission > Permission.None Then

            Me.LoadingPanel.Style("left") = "0px"
            Me.LoadingPanel.Style("top") = "0px"

            If Not Me.IsPostBack Then
                CreateColumnsMessages()
                BindGridMessages(True)
            Else
                BindGridMessages(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

#End Region

#Region "Messages grid"

    Private Sub BindGridMessages(ByVal bolReload As Boolean)
        Me.GridMessages.DataSource = Me.MessagesData(bolReload)
        Me.GridMessages.DataBind()
    End Sub

    Private Sub CreateColumnsMessages()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridMessages.Columns.Clear()
        Me.GridMessages.KeyFieldName = "ID"
        Me.GridMessages.SettingsText.EmptyDataRow = " "
        Me.GridMessages.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridMessages.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridMessages.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridMessages.Column.Message", DefaultScope) '"Valor"
        GridColumn.FieldName = "Message"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridMessages.Columns.Add(GridColumn)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridMessages.Column.SeenOn", DefaultScope) '"Valor"
        GridColumnDate.FieldName = "LastTimeShown"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridMessages.Columns.Add(GridColumnDate)

        If bCanEdit Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = False
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = Unit.Pixel(26)
            VisibleIndex = VisibleIndex + 1

            Me.GridMessages.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Protected Sub GridMessages_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridMessages.RowDeleting
        Dim tb As DataTable = Me.MessagesData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridMessages.KeyFieldName))
        If dr IsNot Nothing Then

            If API.EmployeeServiceMethods.DeleteTerminalMessages(Me.Page, dr("IDEmployee"), dr("Message"), dr("ID")) Then
                dr.Delete()
                tb.AcceptChanges()
            Else
                Throw New Exception(Me.Language.Translate("Error.DeleteMessage", Me.DefaultScope))
            End If
            Me.MessagesData = tb
            e.Cancel = True
        End If
    End Sub

#End Region

End Class