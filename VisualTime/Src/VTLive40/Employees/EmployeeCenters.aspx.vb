Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class EmployeeCenters
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.BusinessCenters"

#Region "Declarations"

    Private intIDEmployee As Integer

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property CentersData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_EmployeesCenters")

            If bolReload OrElse tb Is Nothing Then
                tb = API.TasksServiceMethods.GetEmployeeBusinessCentersDataTable(Me, intIDEmployee, False, False)

                If tb IsNot Nothing Then
                    tb.Columns.Add(New DataColumn("EndDateVisible", GetType(Date)))
                End If

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim index As Integer = 1
                    For Each oRow As DataRow In tb.Rows
                        If oRow("EndDate") = New Date(2079, 1, 1) Then
                            oRow("EndDateVisible") = DBNull.Value
                        Else
                            oRow("EndDateVisible") = oRow("EndDate")
                        End If
                    Next
                End If

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("IDCenter"), tb.Columns("BeginDate")}
                    tb.AcceptChanges()
                End If

                Session("Employees_EmployeesCenters") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_EmployeesCenters") = value
        End Set
    End Property

    Private Property ListCentersData(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_Centers")

            If bolReload OrElse tb Is Nothing Then
                tb = API.TasksServiceMethods.GetBusinessCenters(Me, bolCheckStatus)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("Employees_Centers") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_Centers") = value
        End Set
    End Property

    Private Property ListCentersDataDetail(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Employees_CentersDetail")

            If bolReload OrElse tb Is Nothing Then
                tb = API.TasksServiceMethods.GetBusinessCenters(Me, bolCheckStatus)
                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("Employees_CentersDetail") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Employees_CentersDetail") = value
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

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission > Permission.None Then

            Me.btOK.Visible = (Me.oPermission >= Permission.Admin)
            Me.btnAddNewCenter.Visible = (Me.oPermission >= Permission.Admin)
            If (Me.oPermission <= Permission.Write) Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            Me.LoadingPanel.Style("left") = "0px"
            Me.LoadingPanel.Style("top") = "0px"

            If Not Me.IsPostBack Then
                CreateColumnsCenters()
                BindGridCenter(True)
                Dim dtVisibleCenters As DataTable = Me.ListCentersDataDetail(True, True)
            Else
                BindGridCenter(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        If Me.oPermission >= Permission.Admin Then

            If Not Me.GridCenters.IsEditing() Then

                Dim tbCenters As DataTable = Me.CentersData

                If API.TasksServiceMethods.SaveEmployeeBusinessCenters(Me, Me.intIDEmployee, tbCenters) Then
                    Me.MustRefresh = "4"
                    Me.CanClose = True
                End If

                Me.CentersData = tbCenters
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

#End Region

#Region "Centers grid"

    Private Sub BindGridCenter(ByVal bolReload As Boolean)
        Me.GridCenters.DataSource = Me.CentersData(bolReload)
        Me.GridCenters.DataBind()
    End Sub

    Private Sub CreateColumnsCenters()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridCenters.Columns.Clear()
        Me.GridCenters.KeyFieldName = "IDCenter;BeginDate"
        Me.GridCenters.SettingsText.EmptyDataRow = " "
        Me.GridCenters.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridCenters.SettingsEditing.Mode = GridViewEditingMode.Inline
        Me.GridCenters.SettingsBehavior.AllowEllipsisInText = True

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDCenter"
        GridColumn.FieldName = "IDCenter"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridCenters.Columns.Add(GridColumn)

        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridCenters.Column.Center", DefaultScope)
        GridColumnCombo.FieldName = "IDCenter"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = ListCentersData(True, False)
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        'GridColumnCombo.Width = New Unit(35, UnitType.Percentage)
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridCenters.Columns.Add(GridColumnCombo)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridCenters.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = New Unit(100, UnitType.Pixel)
        Me.GridCenters.Columns.Add(GridColumnDate)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridCenters.Column.EndDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "EndDateVisible"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = New Unit(100, UnitType.Pixel)
        Me.GridCenters.Columns.Add(GridColumnDate)

        If Me.oPermission >= Permission.Admin Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = New Unit(50, UnitType.Pixel)
            VisibleIndex = VisibleIndex + 1

            Me.GridCenters.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Private Sub GridCenters_DataBinding(sender As Object, e As EventArgs) Handles GridCenters.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridCenters.Columns(1)
        oCombo.PropertiesComboBox.DataSource = ListCentersData
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Protected Sub GridCenters_CellEditorInitialize(sender As Object, e As ASPxGridViewEditorEventArgs) Handles GridCenters.CellEditorInitialize
        If e.Column.FieldName = "IDCenter" Then

            Dim tb As DataTable = Me.CentersData()
            If e.VisibleIndex >= 0 Then
                Dim dRow As DataRow = tb.Rows.Find(e.KeyValue.ToString().Split("|"))

                Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                cmb.Font.Size = 8
                cmb.Focus()

                If Not dRow Is Nothing Then
                    Dim dtVisibleCenters As DataTable = Me.ListCentersDataDetail(, True)

                    Dim removeItems As New Generic.List(Of ListEditItem)
                    For Each oItem As ListEditItem In cmb.Items

                        Dim oRows() As DataRow = dtVisibleCenters.Select("ID = " & oItem.Value)

                        If oRows.Length = 0 Then
                            If roTypes.Any2Integer(dRow("IDCenter")) <> oItem.Value Then
                                removeItems.Add(oItem)
                            End If
                        End If
                    Next

                    For Each oDeleteItem As ListEditItem In removeItems
                        cmb.Items.Remove(oDeleteItem)
                    Next
                End If
            Else
                Try
                    Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                    cmb.Font.Size = 8
                    cmb.Focus()

                    Dim dtVisibleCenters As DataTable = Me.ListCentersDataDetail(, True)

                    Dim removeItems As New Generic.List(Of ListEditItem)
                    For Each oItem As ListEditItem In cmb.Items

                        Dim oRows() As DataRow = dtVisibleCenters.Select("ID = " & oItem.Value)

                        If oRows.Length = 0 Then
                            removeItems.Add(oItem)
                        End If
                    Next

                    For Each oDeleteItem As ListEditItem In removeItems
                        cmb.Items.Remove(oDeleteItem)
                    Next
                Catch ex As Exception

                End Try
            End If
        End If

    End Sub

    Private Sub ValidateRow(ByVal tb As DataTable, ByVal row As OrderedDictionary, ByRef bFound As Boolean)
        For Each oRow As DataRow In tb.Rows
            If roTypes.Any2Integer(oRow("IDCenter")) = roTypes.Any2Integer(row("IDCenter")) Then
                Dim datBegindate As Date
                Dim datEndDate As Date
                datBegindate = oRow("BeginDate")
                datEndDate = oRow("EndDate")

                If row("BeginDate") >= datBegindate And row("BeginDate") <= datEndDate Then
                    bFound = True
                    Exit For
                End If

                If row("EndDateVisible") >= datBegindate And row("EndDateVisible") <= datEndDate Then
                    bFound = True
                    Exit For
                End If

                If row("BeginDate") <= datBegindate And row("EndDateVisible") >= datBegindate Then
                    bFound = True
                    Exit For
                End If

                If row("BeginDate") <= datEndDate And row("EndDateVisible") >= datEndDate Then
                    bFound = True
                    Exit For
                End If
            End If
        Next
    End Sub

    Protected Sub GridCenters_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridCenters.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim bFound As Boolean = False
            Dim tb As DataTable = Me.CentersData()
            If (tb.Rows.Count > 0) Then
                ValidateRow(tb, e.NewValues, bFound)
            End If
            If Not bFound Then
                If e.NewValues("IDCenter") Is Nothing Then
                    Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.EmptyData", Me.DefaultScope))
                End If
                If e.NewValues("BeginDate") Is Nothing Then
                    Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.EmptyData", Me.DefaultScope))
                End If

                If Not e.NewValues("EndDateVisible") Is Nothing Then
                    If e.NewValues("BeginDate") > e.NewValues("EndDateVisible") Then
                        Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.InValidData", Me.DefaultScope))
                    End If
                End If

                Dim oNewRow As DataRow = tb.NewRow
                With oNewRow
                    .Item("IDEmployee") = intIDEmployee
                    .Item("IDCenter") = e.NewValues("IDCenter")
                    .Item("BeginDate") = e.NewValues("BeginDate")
                    If e.NewValues("EndDateVisible") IsNot Nothing Then
                        .Item("EndDateVisible") = e.NewValues("EndDateVisible")
                        .Item("EndDate") = e.NewValues("EndDateVisible")
                    Else
                        .Item("EndDateVisible") = DBNull.Value
                        .Item("EndDate") = New Date(2079, 1, 1)
                    End If
                End With

                tb.Rows.Add(oNewRow)
                tb.AcceptChanges()
                Me.CentersData = tb
                e.Cancel = True
                grid.CancelEdit()
            Else
                Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.AlreadyExists", Me.DefaultScope))
            End If
        End If
    End Sub

    Protected Sub GridCenters_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridCenters.CustomErrorText
        e.ErrorText = e.Exception.Message
    End Sub

    Protected Sub GridCenters_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridCenters.RowDeleting
        Dim tb As DataTable = Me.CentersData()
        Dim pk As List(Of Object) = New List(Of Object)()
        pk.Add(e.Keys(GridCenters.KeyFieldName.ToString().Split(";")(0)))
        pk.Add(e.Keys(GridCenters.KeyFieldName.ToString().Split(";")(1)))
        Dim dr As DataRow = tb.Rows.Find(pk.ToArray())
        If dr IsNot Nothing Then
            dr.Delete()
            tb.AcceptChanges()
            Me.CentersData = tb
            e.Cancel = True
        End If
    End Sub

#End Region

    Protected Sub GridCenters_InitNewRow(sender As Object, e As Data.ASPxDataInitNewRowEventArgs) Handles GridCenters.InitNewRow

        e.NewValues("BeginDate") = Date.Now.Date
        e.NewValues("EndDate") = New Date(2079, 1, 1)
    End Sub

    Protected Sub GridCenters_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs) Handles GridCenters.RowUpdating
        Dim tb As DataTable = Me.CentersData()
        Dim pk As List(Of Object) = New List(Of Object)()
        pk.Add(e.Keys(GridCenters.KeyFieldName.ToString().Split(";")(0)))
        pk.Add(e.Keys(GridCenters.KeyFieldName.ToString().Split(";")(1)))
        Dim dr As DataRow = tb.Rows.Find(pk.ToArray())
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim bFound As Boolean = False
        Dim tb2Validate As DataTable
        tb2Validate = tb.Copy()
        Dim currentDr As DataRow = tb2Validate.Rows.Find(pk.ToArray())
        tb2Validate.Rows.Remove(currentDr)
        ValidateRow(tb2Validate, e.NewValues, bFound)
        If Not bFound Then
            Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
            enumerator.Reset()
            While enumerator.MoveNext()

                Dim currentkey As String = ""
                If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
                Select Case currentkey
                    Case "IDCenter"
                        If enumerator.Value IsNot Nothing Then
                            dr.Item("IDCenter") = enumerator.Value
                        Else
                            dr.Item("IDCenter") = String.Empty
                        End If
                    Case "BeginDate"
                        If enumerator.Value IsNot Nothing Then
                            dr.Item("BeginDate") = enumerator.Value
                        Else
                            dr.Item("BeginDate") = Date.Now.Date
                        End If

                    Case "EndDateVisible"
                        If enumerator.Value Is Nothing Then
                            dr.Item("EndDateVisible") = DBNull.Value
                            dr.Item("EndDate") = New Date(2079, 1, 1)
                        Else
                            dr.Item("EndDateVisible") = enumerator.Value
                            dr.Item("EndDate") = enumerator.Value
                        End If
                End Select

            End While

            If e.NewValues("IDCenter") Is Nothing Then
                Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.EmptyData", Me.DefaultScope))
            End If
            If e.NewValues("BeginDate") Is Nothing Then
                Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.EmptyData", Me.DefaultScope))
            End If

            If Not e.NewValues("EndDateVisible") Is Nothing Then
                If e.NewValues("BeginDate") > e.NewValues("EndDateVisible") Then
                    Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.InValidData", Me.DefaultScope))
                End If
            End If
        Else
            Throw New Exception(Me.Language.Translate("EmployeeCenters.Error.AlreadyExists", Me.DefaultScope))
        End If

        e.Cancel = True
        grid.CancelEdit()
        BindGridCenter(False)

    End Sub

End Class