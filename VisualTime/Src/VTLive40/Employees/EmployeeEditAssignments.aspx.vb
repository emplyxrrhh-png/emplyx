Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class EmployeeAssignments
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.Assignments"

#Region "Declarations"

    Private intIDEmployee As Integer
    Private xDate As Date

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property EmployeeAssignmentsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("EmployeeAssignments_EmployeeAssignmentsData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.EmployeeServiceMethods.GetAssignments(Me, Me.intIDEmployee, True)

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

                Session("EmployeeAssignments_EmployeeAssignmentsData") = tb
            End If

            Return tb
        End Get
        Set(ByVal value As DataTable)
            Session("EmployeeAssignments_EmployeeAssignmentsData") = value
        End Set
    End Property

    Private Property AssignmentsData() As DataView
        Get

            Dim tbCauses As DataTable = Session("EmployeeAssignments_AssignmentsData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me, "Name ASC", False)

                If tb IsNot Nothing Then

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"

                    Session("EmployeeAssignments_AssignmentsData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("EmployeeAssignments_AssignmentsData") = value.Table
            Else
                Session("EmployeeAssignments_AssignmentsData") = Nothing
            End If
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

        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            Me.intIDEmployee = roTypes.Any2Integer(Me.Request("EmployeeID"))

            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If

            If Not Me.IsPostBack Then
                CreateColumnsAssignments()
                Me.EmployeeAssignmentsData = Nothing
                Me.AssignmentsData = Nothing
                BindGridAssignments(True)
            Else
                BindGridAssignments(False)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Not GridAssignments.IsEditing Then
            Me.SaveData()
        End If

    End Sub

#End Region

#Region "Grid Assignments"

    Private Sub BindGridAssignments(ByVal bolReload As Boolean)
        Me.GridAssignments.DataSource = Me.EmployeeAssignmentsData(bolReload)
        Me.GridAssignments.DataBind()
    End Sub

    Private Sub CreateColumnsAssignments()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.GridAssignments.Columns.Clear()
        Me.GridAssignments.KeyFieldName = "ID"
        Me.GridAssignments.SettingsText.EmptyDataRow = " "
        Me.GridAssignments.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission >= Permission.Write Then
            Me.GridAssignments.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridAssignments.FindTitleTemplateControl("btnAddNewAssignment")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        If Me.oPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            VisibleIndex = VisibleIndex + 1
            Me.GridAssignments.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridAssignments.Columns.Add(GridColumn)

        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridAssignments.Column.Assignment", DefaultScope)
        GridColumnCombo.FieldName = "IDAssignment"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = AssignmentsData
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 45
        Me.GridAssignments.Columns.Add(GridColumnCombo)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridAssignments.Column.Suitability", DefaultScope) '"Valor"
        GridColumn.FieldName = "Suitability"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..100>"
        GridColumn.PropertiesTextEdit.MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.DecimalSymbol
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 43
        Me.GridAssignments.Columns.Add(GridColumn)
    End Sub

    Private Sub GridAssignments_DataBinding(sender As Object, e As EventArgs) Handles GridAssignments.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridAssignments.Columns("IDAssignment")
        oCombo.PropertiesComboBox.DataSource = AssignmentsData
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
    End Sub

    Protected Sub GridAssignments_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridAssignments.StartRowEditing

    End Sub

    Protected Sub GridAssignments_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridAssignments.InitNewRow
        Dim tb As DataTable = Me.EmployeeAssignmentsData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If
    End Sub

    Protected Sub GridAssignments_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridAssignments.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.EmployeeAssignmentsData()
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                If e.NewValues("IDAssignment") Is Nothing Then
                    .Item("IDAssignment") = DBNull.Value
                Else
                    .Item("IDAssignment") = e.NewValues("IDAssignment")
                End If
                .Item("Suitability") = e.NewValues("Suitability")
                .Item("IDEmployee") = Me.intIDEmployee
            End With

            tb.Rows.Add(oNewRow)

            Me.EmployeeAssignmentsData = tb
            e.Cancel = True
            grid.CancelEdit()

        End If
    End Sub

    Protected Sub GridAssignments_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles GridAssignments.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridAssignments_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridAssignments.RowUpdating

        Dim tb As DataTable = Me.EmployeeAssignmentsData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridAssignments.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "Suitability"
                    dr.Item("Suitability") = enumerator.Value
                Case "IDAssignment"
                    If enumerator.Value Is Nothing Then
                        dr.Item("IDAssignment") = DBNull.Value
                    Else
                        dr.Item("IDAssignment") = enumerator.Value
                    End If

            End Select

        End While

        Me.EmployeeAssignmentsData = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridAssignments_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridAssignments.RowDeleting
        Dim tb As DataTable = Me.EmployeeAssignmentsData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridAssignments.KeyFieldName))
        dr.Delete()
        Me.EmployeeAssignmentsData = tb
        e.Cancel = True
    End Sub

#End Region

#Region "Methods"

    Private Sub SaveData()

        Dim oAssignments As New Generic.List(Of roEmployeeAssignment)

        For Each oRow As DataRow In Me.EmployeeAssignmentsData.Rows
            Dim oEmployeeAssignment As New roEmployeeAssignment
            If oRow.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("IDAssignment")) Then
                    oEmployeeAssignment.IDEmployee = oRow("IDEmployee")
                    oEmployeeAssignment.IDAssignment = oRow("IDAssignment")
                    oEmployeeAssignment.Suitability = roTypes.Any2Integer(oRow("Suitability"))
                    oAssignments.Add(oEmployeeAssignment)
                End If
            End If
        Next

        If API.EmployeeServiceMethods.SaveEmployeeAssignments(Me, Me.intIDEmployee, oAssignments) Then
            Me.CanClose = True
            Me.MustRefresh = "EmployeeAssignments"
        Else

        End If

    End Sub

    Protected Sub btnAddNewAssignment_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewAssignment", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblDescription_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Keyword("Assignments")
        txtLabel.ToolTip = ""
    End Sub

#End Region

End Class