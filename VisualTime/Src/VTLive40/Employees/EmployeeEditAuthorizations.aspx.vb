Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class EmployeeEditAuthorizations
    Inherits PageBase

    Private Const FeatureAlias As String = "Access.Groups.Assign"

#Region "Declarations"

    Public Enum EditAuthorizationMode
        Employee = 0
        Group = 1
    End Enum

    Private intIDEmployee As Integer
    Private intIDGroup As Integer

    Private oPermission As Permission
    Private bCanEdit As Boolean = True

    Private eFormMode As EditAuthorizationMode = EditAuthorizationMode.Employee

#End Region

#Region "Properties"

    Private Property AuthorizationsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_EmployeesAuthorizations")

            If bolReload OrElse tb Is Nothing Then
                tb = AccessGroupServiceMethods.GetEmployeeAuthorizations(Me.Page, intIDEmployee, intIDGroup, True)

                If tb IsNot Nothing Then
                    tb.Columns.Add(New DataColumn("ID", GetType(Integer)))

                    Dim rowIndex As Integer = 1
                    For Each oRow As DataRow In tb.Rows
                        oRow("ID") = rowIndex
                        rowIndex = rowIndex + 1
                    Next

                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("Employees_EmployeesAuthorizations") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_EmployeesAuthorizations") = value
        End Set
    End Property

    Private Property CurrentGroupPath(Optional ByVal bolReload As Boolean = False) As String
        Get
            Dim oCurrentGoupPath As String = Session("Employees_CurrentGroupPath")

            If bolReload OrElse oCurrentGoupPath Is Nothing Then
                If Me.eFormMode = EditAuthorizationMode.Group Then
                    oCurrentGoupPath = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, intIDGroup, False).Path
                Else
                    oCurrentGoupPath = "0"
                End If

                Session("Employees_CurrentGroupPath") = oCurrentGoupPath
            End If
            Return oCurrentGoupPath
        End Get
        Set(value As String)
            Session("Employees_CurrentGroupPath") = value
        End Set
    End Property

    Private Property AccessGroupsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_EmployeesAccessGroups")

            If bolReload OrElse tb Is Nothing Then
                If (HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1")) Then
                    tb = AccessGroupServiceMethods.GetAccessGroupsByPassportDataTable(Me.Page, WLHelperWeb.CurrentPassportID, False)
                Else
                    tb = AccessGroupServiceMethods.GetAccessGroups(Me.Page)
                End If
                Session("Employees_EmployeesAccessGroups") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_EmployeesAccessGroups") = value
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

        Dim GroupID As String = Request.Params("GroupID")
        If GroupID IsNot Nothing AndAlso GroupID.Length > 0 Then
            Me.intIDGroup = CInt(GroupID)
        End If

        If intIDEmployee > 0 Then
            eFormMode = EditAuthorizationMode.Employee
        ElseIf intIDGroup > 0 Then
            eFormMode = EditAuthorizationMode.Group
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.hdnAccessMode.Value = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("AdvancedAccessMode"))
        Me.bCanEdit = True

        If Me.oPermission > Permission.None Then

            Me.btOK.Visible = (Me.oPermission >= Permission.Write AndAlso bCanEdit)
            Me.btnAddNewAccessGroup.Visible = (Me.oPermission >= Permission.Write AndAlso bCanEdit)
            If (Me.oPermission <= Permission.Write) Or (bCanEdit = False) Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            Me.LoadingPanel.Style("left") = "0px"
            Me.LoadingPanel.Style("top") = "0px"

            If Not Me.IsPostBack Then
                Dim oCurrentPath = Me.CurrentGroupPath(True)
                CreateColumnsAuthorizations()
                BindGridMobility(True)
                Me.hdnAccessRowsCount.Value = AuthorizationsData.Rows.Count
            Else
                BindGridMobility(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        If Me.oPermission >= Permission.Write Then

            If Not Me.GridAccessGroups.IsEditing() Then
                If Me.bCanEdit = False Then
                    Me.MustRefresh = "4"
                    Me.CanClose = True
                Else
                    Dim tbAuthorizations As DataTable = Me.AuthorizationsData

                    If API.EmployeeServiceMethods.SaveAccessAuthorizations(Me.Page, Me.intIDEmployee, Me.intIDGroup, tbAuthorizations) Then
                        Me.MustRefresh = "4"
                        Me.CanClose = True
                    End If

                    Me.AuthorizationsData = tbAuthorizations
                End If
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

#End Region

#Region "Mobility grid"

    Private Sub BindGridMobility(ByVal bolReload As Boolean)
        Me.GridAccessGroups.DataSource = Me.AuthorizationsData(bolReload)
        Me.GridAccessGroups.DataBind()
    End Sub

    Private Sub CreateColumnsAuthorizations()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridAccessGroups.Columns.Clear()
        Me.GridAccessGroups.KeyFieldName = "ID"
        Me.GridAccessGroups.SettingsText.EmptyDataRow = " "
        Me.GridAccessGroups.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridAccessGroups.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDAuthorization"
        GridColumn.FieldName = "IDAuthorization"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridAccessGroups.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "GroupPath"
        GridColumn.FieldName = "GroupPath"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridAccessGroups.Columns.Add(GridColumn)

        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridAccessGroups.Column.AccessGroup", DefaultScope) '"Fecha"
        GridColumnCombo.FieldName = "IDAuthorization"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = AccessGroupsData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridAccessGroups.Columns.Add(GridColumnCombo)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridAccessGroups.Column.AssignedTo", DefaultScope)
        GridColumn.FieldName = "AssignedTo"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = Unit.Pixel(125)
        Me.GridAccessGroups.Columns.Add(GridColumn)

        If Me.oPermission >= Permission.Write AndAlso bCanEdit Then
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
            GridColumnCommand.Width = Unit.Pixel(52)
            VisibleIndex = VisibleIndex + 1

            Me.GridAccessGroups.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Private Sub GridAccessGroups_DataBinding(sender As Object, e As EventArgs) Handles GridAccessGroups.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridAccessGroups.Columns(2)
        oCombo.PropertiesComboBox.DataSource = AccessGroupsData
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Protected Sub GridAccessGroups_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridAccessGroups.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim bFound As Boolean = False
            Dim tb As DataTable = Me.AuthorizationsData()
            Dim cId As Integer = 0
            If (tb.Rows.Count > 0) Then
                For Each oRow As DataRow In tb.Rows
                    If cId <= roTypes.Any2Integer(oRow("ID")) Then cId = roTypes.Any2Integer(oRow("ID")) + 1

                    If Me.eFormMode = EditAuthorizationMode.Group Then
                        If roTypes.Any2Integer(oRow("IDAuthorization")) = roTypes.Any2Integer(e.NewValues("IDAuthorization")) Then
                            bFound = True
                        End If
                    ElseIf Me.eFormMode = EditAuthorizationMode.Employee Then
                        If roTypes.Any2Integer(oRow("IDAuthorization")) = roTypes.Any2Integer(e.NewValues("IDAuthorization")) AndAlso roTypes.Any2String(oRow("GroupPath") = "0") Then
                            bFound = True
                        End If
                    End If
                Next
            End If
            If Not bFound Then
                Dim oNewRow As DataRow = tb.NewRow
                With oNewRow
                    .Item("ID") = cId
                    .Item("IDEmployee") = intIDEmployee
                    .Item("IDAuthorization") = e.NewValues("IDAuthorization")

                    If Me.eFormMode = EditAuthorizationMode.Group Then
                        .Item("GroupPath") = Me.CurrentGroupPath
                    Else
                        .Item("GroupPath") = "0"
                    End If
                End With

                tb.Rows.Add(oNewRow)
                tb.AcceptChanges()
                Me.AuthorizationsData = tb
                e.Cancel = True
                grid.CancelEdit()

                Me.hdnAccessRowsCount.Value = AuthorizationsData.Rows.Count
            Else
                Throw New Exception(Me.Language.Translate("AccessAuthorizations.Error.AlreadyExists", Me.DefaultScope))
            End If
        End If
    End Sub

    Protected Sub GridAccessGroups_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridAccessGroups.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridAccessGroups_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridAccessGroups.RowDeleting
        Dim tb As DataTable = Me.AuthorizationsData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridAccessGroups.KeyFieldName))
        If dr IsNot Nothing Then
            dr.Delete()
            tb.AcceptChanges()
            Me.AuthorizationsData = tb
            e.Cancel = True
        End If
    End Sub

    Private Sub GridAccessGroups_CommandButtonInitialize(sender As Object, e As ASPxGridViewCommandButtonEventArgs) Handles GridAccessGroups.CommandButtonInitialize
        If e.ButtonType = ColumnCommandButtonType.Delete Then
            Dim groupPath As Object = GridAccessGroups.GetRowValues(e.VisibleIndex, "GroupPath")

            If Me.eFormMode = EditAuthorizationMode.Employee Then
                If groupPath <> "0" Then
                    e.Visible = False
                End If
            ElseIf Me.eFormMode = EditAuthorizationMode.Group Then
                If groupPath <> Me.CurrentGroupPath Then
                    e.Visible = False
                End If
            End If
        End If
    End Sub

    Protected Sub GridAccessGroups_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridAccessGroups.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "AssignedTo"
                If e.IsGetData Then
                    If Not IsDBNull(e.GetListSourceFieldValue("GroupPath")) AndAlso e.GetListSourceFieldValue("GroupPath") IsNot Nothing Then
                        e.Value = String.Empty

                        Dim groupPath As String = roTypes.Any2String(e.GetListSourceFieldValue("GroupPath"))

                        If Me.eFormMode = EditAuthorizationMode.Employee Then
                            If groupPath <> "0" Then
                                Dim aGroupsList = groupPath.Split("\")
                                If aGroupsList.Length > 0 Then
                                    Dim iIdLastGroup = roTypes.Any2Integer(aGroupsList(aGroupsList.Length - 1))

                                    e.Value = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, iIdLastGroup, False).Name
                                Else
                                    e.Value = String.Empty
                                End If

                            End If
                        ElseIf Me.eFormMode = EditAuthorizationMode.Group Then
                            If groupPath <> Me.CurrentGroupPath Then
                                Dim aGroupsList = groupPath.Split("\")
                                If aGroupsList.Length > 0 Then
                                    Dim iIdLastGroup = roTypes.Any2Integer(aGroupsList(aGroupsList.Length - 1))

                                    e.Value = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, iIdLastGroup, False).Name
                                Else
                                    e.Value = String.Empty
                                End If
                            End If
                        End If
                    End If
                End If
        End Select
    End Sub

#End Region

End Class