Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class TaskAssignments
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"

#Region "Declarations"

    Private intIDTask As Integer
    Private xDate As Date

    Private _Action_Edit As Integer = 0
    Private _Action_Remove As Integer = 1
    Private _Action_Accept As Integer = 2
    Private _Action_Cancel As Integer = 3

    Private _Assignments_EditClickIndex As Integer = 0
    Private _Assignments_RemoveClickIndex As Integer = 1
    Private _Assignments_selectCellIndex As Integer = 3
    Private _Assignments_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _Assignments_EditCellsIndex() As Integer = {2}
    Private _Assignments_EditControls() As String = {"IDAssignment_DropDownList"}
    Private _Assignments_CaptionControls() As String = {"Assignment_Label"}
    Private _Assignments_EditFields() As String = {"IDAssignment"}

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property TaskAssignmentsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("TaskAssignments_TaskAssignmentsData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.TasksServiceMethods.GetAssignments(Me, Me.intIDTask, True)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    tb.Rows.Add(tb.NewRow)
                End If

                ViewState("TaskAssignments_TaskAssignmentsData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("TaskAssignments_TaskAssignmentsData") = value
        End Set
    End Property

    Private Property AssignmentsData() As DataView
        Get

            Dim tbCauses As DataTable = ViewState("TaskAssignments_AssignmentsData")
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

                    ViewState("TaskAssignments_AssignmentsData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("TaskAssignments_AssignmentsData") = value.Table
            Else
                ViewState("TaskAssignments_AssignmentsData") = Nothing
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
        If Me.oPermission > Permission.Read Then

            Me.intIDTask = roTypes.Any2Integer(Me.Request("TaskID"))

            Me.tbAddAssignment.Visible = (Me.oPermission >= Permission.Write)
            Me.grdAssignments.Columns(Me._Assignments_selectCellIndex).Visible = (Me.oPermission >= Permission.Write)
            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If
            If Not Me.IsPostBack Then

                Me.TaskAssignmentsData = Nothing
                Me.AssignmentsData = Nothing

                'Dim oAssignment As AssignmentService.roAssignment = AssignmentService.AssignmentServiceMethods.GetAssignment(Me, Me.intIDAssignment, False)
                'Me.lblTitle.Text = Me.lblTitle.Text.Replace("{1}", oAssignment.Name).Replace("{2}", Format(Me.xDate, HelperWeb.GetShortDateFormat))

                Me.LoadData()
            Else
                Me.LoadData()
            End If

            Try
                If Request.Form("__EVENTTARGET").EndsWith(Me._Assignments_ActionButtons(Me._Action_Accept)) Then
                    Me.grdAssignments_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
                ElseIf Request.Form("__EVENTTARGET").EndsWith(Me._Assignments_ActionButtons(Me._Action_Cancel)) Then
                    Me.grdAssignments_EditCancel(Val(Request.Form("__EVENTARGUMENT")))
                End If
            Catch
            End Try

            ' Establecer seguridad
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAddAssignment_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAddAssignment.ServerClick

        If Not Me.EditingData() Then

            If Me.TaskAssignmentsData.Rows.Count = 1 AndAlso HelperWeb.EmptyRow(Me.TaskAssignmentsData.Rows(0)) Then
                Me.TaskAssignmentsData.Rows.RemoveAt(0)
            End If
            Dim oNewRow As DataRow = Me.NewAssignmentData(Me.TaskAssignmentsData)
            Me.TaskAssignmentsData.Rows.Add(oNewRow)
            Me.LoadData()
            ''Me.UpdateContext(2)

            Me.grdAssignments_EditBegin(Me.grdAssignments.Rows.Count - 1)

        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Not Me.EditingData() Then
            Me.SaveData()
        End If

    End Sub

#Region "grdAssignments events"

    Protected Sub grdAssignments_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdAssignments.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                Dim ddlCauses As DropDownList
                ddlCauses = e.Row.Cells(2).FindControl("IDAssignment_DropDownList")
                If ddlCauses IsNot Nothing Then
                    With ddlCauses
                        .DataSource = Me.AssignmentsData()
                        .DataTextField = "Name"
                        .DataValueField = "ID"
                        '.SelectedValue = Nothing
                        .DataBind()
                    End With
                End If

        End Select

    End Sub

    Protected Sub grdAssignments_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdAssignments.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                Dim _EditClickButton As LinkButton = e.Row.Cells(Me._Assignments_EditClickIndex).Controls(0)
                _jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._Assignments_RemoveClickIndex).Controls(0)
                _jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"

                Dim _jsCommand As String

                Dim _EditAcceptButton As HtmlImage = e.Row.Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Accept))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditAcceptButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                If js.StartsWith("javascript:") Then
                    js = "javascrip: if (CheckConvertControls('') == true) { " & js.Substring(CStr("javascript:").Length) & " }"
                End If
                _EditAcceptButton.Attributes("onclick") = js
                _EditAcceptButton.Attributes("style") += "cursor:pointer;"

                Dim _EditCancelButton As HtmlImage = e.Row.Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Cancel))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditCancelButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                _EditCancelButton.Attributes("onclick") = js
                _EditCancelButton.Attributes("style") += "cursor:pointer;"

                Dim oAssignmentData As DataRowView = e.Row.DataItem

                ' Establecemos controles
                Dim ddlAssignments As DropDownList = e.Row.Cells(2).FindControl("IDAssignment_DropDownList")
                Dim lblAssignment As Label = e.Row.Cells(2).FindControl("Assignment_Label")

                If ddlAssignments IsNot Nothing Then
                    If Not IsDBNull(oAssignmentData("IDAssignment")) Then
                        ddlAssignments.SelectedValue = oAssignmentData("IDAssignment")
                        lblAssignment.Text = ddlAssignments.SelectedItem.Text
                    Else
                        ddlAssignments.SelectedValue = 0
                    End If
                End If

                ' Traducir manualmente controles de la grid
                _EditButton.Attributes("title") = Me.Language.Keyword("Button.Edit")
                _RemoveButton.Attributes("title") = Me.Language.Keyword("Button.Delete")
                _EditAcceptButton.Attributes("title") = Me.Language.Keyword("Button.Apply")
                _EditCancelButton.Attributes("title") = Me.Language.Keyword("Button.Cancel")

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdAssignments_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdAssignments.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' Set the Gridview selected index
                _gridView.SelectedIndex = _rowIndex
                ' Bind the Gridview
                _gridView.DataSource = Me.TaskAssignmentsData
                _gridView.DataBind()

                Me.grdAssignments_EditBegin(_rowIndex, _columnIndex)

            Case "RemoveClick"
                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                grdAssignments_Remove(_rowIndex)

        End Select

    End Sub

    Private Sub grdAssignments_EditBegin(ByVal intRowIndex As Integer, Optional ByVal intColIndex As Integer = -1)

        Dim _gridView As GridView = Me.grdAssignments

        Dim oControl As Control
        Dim intCell As Integer
        For intIndex As Integer = 0 To Me._Assignments_EditCellsIndex.Length - 1
            intCell = Me._Assignments_EditCellsIndex(intIndex)
            ' Ocultar los controls no editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Assignments_CaptionControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = False
            ' Clear the attributes from the selected cell to remove the click event
            _gridView.Rows(intRowIndex).Cells(intCell).Attributes.Clear()
            ' Hacer visibles los controles editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Assignments_EditControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = True

        Next

        ' Poner las imágenes de editar y eliminar invisibles
        Dim oEditButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Edit))
        oEditButton.Visible = False
        Dim oRemoveButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Remove))
        oRemoveButton.Visible = False
        ' Poner las imágenes de aceptar y cancelar visibles
        Dim oEditAcceptButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Accept))
        oEditAcceptButton.Visible = True
        Dim oEditCancelButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Assignments_selectCellIndex).FindControl(Me._Assignments_ActionButtons(Me._Action_Cancel))
        oEditCancelButton.Visible = True

    End Sub

    Private Sub grdAssignments_EditAccept(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdAssignments

        Try

            If _gridView.PageCount > 1 Then
                intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
            End If

            ' Insertamos los datos a la tabla
            Dim dt As DataTable = Me.TaskAssignmentsData()
            Dim dr As DataRow = dt.Rows(intRowIndex)
            dr.BeginEdit()

            ' Obtenemos los valores de las columnas editables
            Dim oControl As Control
            Dim intCell As Integer
            For intIndex As Integer = 0 To Me._Assignments_EditCellsIndex.Length - 1

                intCell = Me._Assignments_EditCellsIndex(intIndex)

                oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Assignments_EditControls(intIndex))
                If oControl IsNot Nothing Then

                    If TypeOf oControl Is TextBox Then
                        Dim strValue As String = CType(oControl, TextBox).Text
                        dr(Me._Assignments_EditFields(intIndex)) = strValue
                    ElseIf TypeOf oControl Is DropDownList Then
                        If hdnGrdAssignmentsSelectedAssignment.Value <> -1 Then
                            dr(Me._Assignments_EditFields(intIndex)) = hdnGrdAssignmentsSelectedAssignment.Value
                        Else
                            If Me.AssignmentsData().Table IsNot Nothing AndAlso Me.AssignmentsData().Table.Rows.Count > 0 Then
                                dr(Me._Assignments_EditFields(intIndex)) = Me.AssignmentsData().Table.Rows(0).Item("ID")
                            End If
                        End If
                    ElseIf TypeOf oControl Is HtmlInputText Then
                        Dim strValue As String = CType(oControl, HtmlInputText).Value
                        If CType(oControl, HtmlInputText).Attributes("ConvertControl") = "NumberField" Then
                            strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                        End If
                        dr(Me._Assignments_EditFields(intIndex)) = strValue
                    End If

                End If

            Next

            dr.EndEdit()

            Me.TaskAssignmentsData() = dt

            ' Refrescamos la grid
            _gridView.DataSource = dt
            _gridView.DataBind()
        Catch ex As Exception
            ' Repopulate the GridView
            _gridView.DataSource = Me.TaskAssignmentsData()
            _gridView.DataBind()

        End Try

    End Sub

    Private Sub grdAssignments_EditCancel(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdAssignments

        _gridView.DataSource = Me.TaskAssignmentsData()
        _gridView.DataBind()

    End Sub

    Private Sub grdAssignments_Remove(ByVal intRowIndex As Integer)

        ' Verificamos que no estemos en edición
        If Not Me.EditingData() Then

            If intRowIndex > -1 Then

                Dim Rows() As DataRow = Me.TaskAssignmentsData.Select("", "", DataViewRowState.CurrentRows)
                If Rows.Length > intRowIndex Then
                    Rows(intRowIndex).Delete()
                    Me.TaskAssignmentsData.AcceptChanges()
                End If

                Dim bolEmpty As Boolean = True
                For Each oRow As DataRow In Me.TaskAssignmentsData.Rows
                    If oRow.RowState <> DataRowState.Deleted Then
                        bolEmpty = False
                        Exit For
                    End If
                Next
                If bolEmpty Then
                    Me.TaskAssignmentsData.Rows.Add(Me.TaskAssignmentsData.NewRow)
                End If

                Me.LoadData()

            End If

        End If

    End Sub

#End Region

#End Region

#Region "Methods"

    Private Sub LoadData()

        With Me.grdAssignments
            .DataSourceID = ""
            .DataSource = Me.TaskAssignmentsData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdAssignments)

    End Sub

    Private Function EditingData() As Boolean

        Dim bolEditing As Boolean = False

        Dim oImage As HtmlImage
        For Each oRow As GridViewRow In Me.grdAssignments.Rows
            oImage = oRow.FindControl(Me._Assignments_ActionButtons(Me._Action_Accept))
            If oImage IsNot Nothing Then
                bolEditing = oImage.Visible
                If bolEditing Then Exit For
            End If
        Next

        If bolEditing Then
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("DataEditing.Message", Me.DefaultScope))
        End If

        Return bolEditing

    End Function

    Private Function NewAssignmentData(ByVal tbAssignments As DataTable) As DataRow

        Dim oNew As DataRow = tbAssignments.NewRow
        With oNew
            .Item("IDTask") = Me.intIDTask
            .Item("IDAssignment") = DBNull.Value
        End With
        Return oNew

    End Function

    Private Sub SaveData()

        Dim oAssignments As New Generic.List(Of roTaskAssignment)

        For Each oRow As DataRow In Me.TaskAssignmentsData.Rows
            Dim oEmployeeAssignment As New roTaskAssignment
            If oRow.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("IDAssignment")) Then
                    oEmployeeAssignment.IDTask = oRow("IDTask")
                    oEmployeeAssignment.IDAssignment = oRow("IDAssignment")
                    oAssignments.Add(oEmployeeAssignment)
                End If
            End If
        Next

        If API.TasksServiceMethods.SaveTaskAssignments(Me, Me.intIDTask, oAssignments) Then
            Me.CanClose = True
            Me.MustRefresh = "TaskAssignments"
        Else

        End If

    End Sub

    Protected Sub IDAssignment_DropDownList_TextChanged(sender As Object, e As EventArgs)
        Me.hdnGrdAssignmentsSelectedAssignment.Value = CType(sender, DropDownList).SelectedValue
    End Sub

    Protected Sub IDAssignment_DropDownList_SelectedIndexChanged(sender As Object, e As EventArgs)
        Me.hdnGrdAssignmentsSelectedAssignment.Value = CType(sender, DropDownList).SelectedValue
        Me.grdAssignments_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
    End Sub

#End Region

End Class