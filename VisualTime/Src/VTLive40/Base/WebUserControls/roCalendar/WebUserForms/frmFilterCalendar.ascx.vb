Imports DevExpress.Web
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Web.Base

Partial Class WebUserForms_frmFilterCalendar
    Inherits UserControlBase

    Public Property WorkMode As roCalendar.roCalendarWorkMode
        Get
            If ViewState("roAdvFilterCalendarWorkMode") Is Nothing Then
                Return False
            Else
                Return ViewState("roAdvFilterCalendarWorkMode")
            End If
        End Get
        Set(value As roCalendar.roCalendarWorkMode)
            ViewState("roAdvFilterCalendarWorkMode") = value
        End Set
    End Property

    Private Function GetAssignmentsDataAll(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session(Me.WorkMode.ToString & "_FilterOptions") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            If Me.WorkMode = roCalendar.roCalendarWorkMode.roCalendar Then
                oNewRow = returnTb.NewRow
                With oNewRow
                    .Item("ID") = 0
                    .Item("Name") = Me.Language.Translate("FilterCalendar.NoAssignment", Me.DefaultScope)
                End With
                returnTb.Rows.Add(oNewRow)

                Dim lstAssign As List(Of roAssignment) = API.AssignmentServiceMethods.GetAssignments(Me.Page, "", False)
                If lstAssign IsNot Nothing Then
                    For Each oRow As roAssignment In lstAssign
                        oNewRow = returnTb.NewRow
                        With oNewRow
                            .Item("ID") = oRow.ID
                            .Item("Name") = oRow.Name
                        End With
                        returnTb.Rows.Add(oNewRow)
                    Next
                End If
            Else
                Dim oLst = API.AISchedulingServiceMethods.GetProductiveUnits(Me.Page)

                For Each oUnit In oLst
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oUnit.ID
                        .Item("Name") = oUnit.Name
                    End With

                    returnTb.Rows.Add(oNewRow)
                Next

            End If

            Session(Me.WorkMode.ToString & "_FilterOptions") = returnTb
            dv = New DataView(returnTb)
            dv.Sort = "Name ASC"

            Return dv
        Else
            Dim dtReturn As DataTable = Session(Me.WorkMode.ToString & "_FilterOptions")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Public Sub InitControl(ByVal parentWorkMode As roCalendar.roCalendarWorkMode)
        Me.WorkMode = parentWorkMode
    End Sub

    Private Sub WebUserForms_frmFilterCalendar_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.grdAssignments.ClientInstanceName = Me.ClientID & "_grdAssignmentsClient"
        Me.rbPlannedView.ClientInstanceName = Me.ClientID & "_rbPlannedView"
        Me.rbRealView.ClientInstanceName = Me.ClientID & "_rbRealView"

        CreateColumnsAssignments()

        If Not IsPostBack Then
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") = True Then
                BindGridAssignments(True)
            Else
                BindGridAssignments(False)
            End If
        Else
            BindGridAssignments(False)
        End If

        Me.lblAssignmentDescription.Text = Me.Language.Translate("lblAssignmentDescription." & Me.WorkMode.ToString(), Me.DefaultScope)
        Me.lblFilterTitle.Text = Me.Language.Translate("lblFilterTitle." & Me.WorkMode.ToString(), Me.DefaultScope)

    End Sub

    Private Sub BindGridAssignments(Optional ByVal bolReload As Boolean = False)
        Me.grdAssignments.DataSource = Me.GetAssignmentsDataAll(bolReload)
        Me.grdAssignments.DataBind()
    End Sub

    Private Sub CreateColumnsAssignments()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.grdAssignments.Columns.Clear()
        Me.grdAssignments.KeyFieldName = "ID"
        Me.grdAssignments.SettingsText.EmptyDataRow = " "
        Me.grdAssignments.Settings.ShowFilterRow = True

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.grdAssignments.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ShowApplyFilterButton = False
        GridColumnCommand.ShowClearFilterButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.AllPages
        GridColumnCommand.ShowSelectCheckbox = True

        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = Unit.Percentage(10)
        VisibleIndex = VisibleIndex + 1
        Me.grdAssignments.Columns.Add(GridColumnCommand)

        'Nombre
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridCauses.Column." & Me.WorkMode.ToString() & ".Name", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = Unit.Percentage(90)
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        Me.grdAssignments.Columns.Add(GridColumn)

    End Sub

End Class