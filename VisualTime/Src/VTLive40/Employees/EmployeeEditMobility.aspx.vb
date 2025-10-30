Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class EmployeeEditMobility
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.GroupMobility"

#Region "Declarations"

    Private intIDEmployee As Integer

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property EditMobilityData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("EmployeeEditMobility_EditMobilityData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.EmployeeServiceMethods.GetMobilities(Me, Me.intIDEmployee)

                If tb IsNot Nothing Then
                    tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                End If

                If Not tb Is Nothing Then

                    Dim index As Integer = 1
                    For Each oRow As DataRow In tb.Rows
                        oRow("ID") = index
                        index = index + 1
                    Next

                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("EmployeeEditMobility_EditMobilityData") = tb

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("EmployeeEditMobility_EditMobilityData") = value
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

        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission > Permission.None Then

            Me.btOK.Visible = (Me.oPermission >= Permission.Write)
            Me.btnAddNewHMobility.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            Me.LoadingPanel.Style("left") = "0px"
            Me.LoadingPanel.Style("top") = "0px"

            If Not Me.IsPostBack Then
                CreateColumnsMobility()

                Me.GroupSelectorFrame.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=100&TreesMultiSelect=000&TreesOnlyGroups=100&TreeFunction=parent.GroupSelected&FilterFloat=false&" &
                                                                                                                    "PrefixTree=treeGroupEmployeeEditMobility&AdvancedFilterVisible=false&" &
                                                                                                                    "FeatureAlias=Employees&FeatureType=A")
                HelperWeb.roSelector_Initialize("roChildSelectorW_treeGroupEmployeeEditMobility")
                BindGridMobility(True)
            Else
                BindGridMobility(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        If Me.oPermission >= Permission.Write Then

            If Not Me.GridMobility.IsEditing() Then

                Dim intInvalidRow As Integer

                Dim tbMobilities As DataTable = Me.EditMobilityData

                If API.EmployeeServiceMethods.ValidateMobilities(Me, Me.intIDEmployee, tbMobilities, intInvalidRow) Then

                    If API.EmployeeServiceMethods.SaveMobilities(Me, Me.intIDEmployee, tbMobilities) Then

                        Me.MustRefresh = "4"
                        Me.CanClose = True

                    End If

                    Me.EditMobilityData = tbMobilities

                End If

            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

#End Region

#Region "Mobility grid"

    Private Sub BindGridMobility(ByVal bolReload As Boolean)
        Me.GridMobility.DataSource = Me.EditMobilityData(bolReload)
        Me.GridMobility.DataBind()
    End Sub

    Protected Sub btnAddNewHMobility_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewMobility", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblMobilitiesCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdMobilities.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Private Sub CreateColumnsMobility()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnCheckBox As GridViewDataCheckColumn

        Dim VisibleIndex As Integer = 0

        Me.GridMobility.Columns.Clear()
        Me.GridMobility.KeyFieldName = "ID"
        Me.GridMobility.SettingsText.EmptyDataRow = " "
        Me.GridMobility.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission >= Permission.Write Then
            Me.GridMobility.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridMobility.FindTitleTemplateControl("btnAddNewHMobility")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

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
            GridColumnCommand.Width = 18
            VisibleIndex = VisibleIndex + 1
            Me.GridMobility.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridMobility.Columns.Add(GridColumn)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDGroup"
        GridColumn.FieldName = "IDGroup"
        GridColumn.ReadOnly = False
        'GridColumn.Visible = False
        GridColumn.Width = 0
        Me.GridMobility.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridMobility.Column.Date", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 45
        Me.GridMobility.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridMobility.Column.Group", DefaultScope) '"Valor"
        GridColumn.FieldName = "FullGroupName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.PropertiesTextEdit.ClientSideEvents.GotFocus = "function(s,e){ShowGroupSelector()}"
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 300
        Me.GridMobility.Columns.Add(GridColumn)

        If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("TransferAvailable")) Then
            GridColumnCheckBox = New GridViewDataCheckColumn
            GridColumnCheckBox.Caption = Me.Language.Translate("GridMobility.Column.Transfer", DefaultScope) '"Valor"
            GridColumnCheckBox.FieldName = "IsTransfer"
            GridColumnCheckBox.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            GridColumnCheckBox.ReadOnly = False
            GridColumnCheckBox.Width = 20
            Me.GridMobility.Columns.Add(GridColumnCheckBox)
        End If

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
            GridColumnCommand.Width = 18
            VisibleIndex = VisibleIndex + 1
            Me.GridMobility.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Protected Sub GridMobility_CommandButtonInitialize(sender As Object, e As ASPxGridViewCommandButtonEventArgs) Handles GridMobility.CommandButtonInitialize
        Dim tb As DataTable = Me.EditMobilityData()

        If CountActiveRows(tb) = 1 AndAlso e.ButtonType = ColumnCommandButtonType.Delete Then
            e.Visible = False
        End If

    End Sub

    Protected Sub GridMobility_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridMobility.StartRowEditing

    End Sub

    Protected Sub GridMobility_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridMobility.InitNewRow
        Dim tb As DataTable = Me.EditMobilityData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If
        e.NewValues("BeginDate") = Date.Now.Date
    End Sub

    Protected Sub GridMobility_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridMobility.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.EditMobilityData()
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("BeginDate") = e.NewValues("BeginDate")
                .Item("FullGroupName") = API.EmployeeGroupsServiceMethods.GetGroup(Me, e.NewValues("IDGroup"), False).FullGroupName
                .Item("IDGroup") = e.NewValues("IDGroup")
                If e.NewValues("IsTransfer") Is Nothing Then
                    .Item("IsTransfer") = False
                Else
                    .Item("IsTransfer") = e.NewValues("IsTransfer")
                End If

            End With

            tb.Rows.Add(oNewRow)

            Me.EditMobilityData = tb
            e.Cancel = True
            grid.CancelEdit()

        End If
    End Sub

    Protected Sub GridMobility_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles GridMobility.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridMobility_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridMobility.RowUpdating

        Dim tb As DataTable = Me.EditMobilityData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridMobility.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "BeginDate"
                    dr.Item("BeginDate") = enumerator.Value
                Case "FullGroupName"

                Case "IDGroup"
                    dr.Item("IDGroup") = enumerator.Value
                    dr.Item("FullGroupName") = API.EmployeeGroupsServiceMethods.GetGroup(Me, enumerator.Value, False).FullGroupName
                Case "IsTransfer"
                    dr.Item("IsTransfer") = enumerator.Value
            End Select

        End While

        Me.EditMobilityData = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridMobility_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridMobility.RowDeleting
        Dim tb As DataTable = Me.EditMobilityData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridMobility.KeyFieldName))
        dr.Delete()
        tb.AcceptChanges()
        Me.EditMobilityData = tb
        e.Cancel = True
    End Sub

    Private Function CountActiveRows(ByVal dData As DataTable) As Integer
        Dim iActiveRows As Integer = 0

        For Each oRow As DataRow In dData.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                iActiveRows = iActiveRows + 1
            End If
        Next

        Return iActiveRows
    End Function

#End Region

End Class