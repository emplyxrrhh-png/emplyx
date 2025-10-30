Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Scheduler_DailyCoverage
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private intIDGroup As Integer
    Private xDate As Date

    Private _Action_Edit As Integer = 0
    Private _Action_Remove As Integer = 1
    Private _Action_Accept As Integer = 2
    Private _Action_Cancel As Integer = 3

    Private _Coverages_EditClickIndex As Integer = 0
    Private _Coverages_RemoveClickIndex As Integer = 1
    Private _Coverages_selectCellIndex As Integer = 4
    Private _Coverages_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _Coverages_EditCellsIndex() As Integer = {2, 3}
    Private _Coverages_EditControls() As String = {"IDAssignment_DropDownList", "ExpectedCoverage_TextBox"}
    Private _Coverages_CaptionControls() As String = {"Assignment_Label", "ExpectedCoverage_Label"}
    Private _Coverages_EditFields() As String = {"IDAssignment", "ExpectedCoverage"}

    Private oPermission As Permission

#End Region

#Region "Properties"

    Private Property CoveragesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("DailyCoverage_CoveragesData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.SchedulerServiceMethods.GetDailyCoverageDataTable(Me, Me.intIDGroup, Me.xDate, True)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    tb.Rows.Add(tb.NewRow)
                End If

                ViewState("DailyCoverage_CoveragesData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("DailyCoverage_CoveragesData") = value
        End Set
    End Property

    Private Property AssignmentData() As DataView
        Get

            Dim tbCauses As DataTable = ViewState("DailyCoverage_AssignmentsData")
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

                    ViewState("DailyCoverage_AssignmentsData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("DailyCoverage_AssignmentsData") = value.Table
            Else
                ViewState("DailyCoverage_AssignmentsData") = Nothing
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

            Me.intIDGroup = roTypes.Any2Integer(Me.Request("IDGroup"))
            Dim strDate As String = roTypes.Any2String(Request("CoverageDate")) ' formato dd/MM/yyyy
            If strDate.Length >= 10 Then Me.xDate = New DateTime(CInt(strDate.Substring(6, 4)), CInt(strDate.Substring(3, 2)), CInt(strDate.Substring(0, 2)), 0, 0, 0)

            Me.tbAddCoverage.Visible = (Me.oPermission >= Permission.Write)
            Me.grdCoverages.Columns(Me._Coverages_selectCellIndex).Visible = (Me.oPermission >= Permission.Write)
            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If
            If Not Me.IsPostBack Then

                Me.CoveragesData = Nothing
                Me.AssignmentData = Nothing

                Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, Me.intIDGroup, False)
                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{1}", oGroup.Name).Replace("{2}", Format(Me.xDate, HelperWeb.GetShortDateFormat))

                Me.LoadData()

            End If

            Try
                If Request.Form("__EVENTTARGET").EndsWith(Me._Coverages_ActionButtons(Me._Action_Accept)) Then
                    Me.grdCoverages_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
                ElseIf Request.Form("__EVENTTARGET").EndsWith(Me._Coverages_ActionButtons(Me._Action_Cancel)) Then
                    Me.grdCoverages_EditCancel(Val(Request.Form("__EVENTARGUMENT")))
                End If
            Catch
            End Try

            ' Establecer seguridad
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAddCoverage_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAddCoverage.ServerClick

        If Not Me.EditingData() Then

            If Me.CoveragesData.Rows.Count = 1 AndAlso HelperWeb.EmptyRow(Me.CoveragesData.Rows(0)) Then
                Me.CoveragesData.Rows.RemoveAt(0)
            End If
            Dim oNewRow As DataRow = Me.NewCoverageData(Me.CoveragesData)
            Me.CoveragesData.Rows.Add(oNewRow)
            Me.LoadData()
            ''Me.UpdateContext(2)

            Me.grdCoverages_EditBegin(Me.grdCoverages.Rows.Count - 1)

        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Not Me.EditingData() Then
            Me.SaveData()
        End If

    End Sub

#Region "grdCoverages events"

    Protected Sub grdCoverages_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdCoverages.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                Dim ddlCauses As DropDownList
                ddlCauses = e.Row.Cells(2).FindControl("IDAssignment_DropDownList")
                If ddlCauses IsNot Nothing Then
                    With ddlCauses
                        .DataSource = Me.AssignmentData()
                        .DataTextField = "Name"
                        .DataValueField = "ID"
                        .SelectedValue = Nothing
                        .DataBind()
                    End With
                End If

        End Select

    End Sub

    Protected Sub grdCoverages_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdCoverages.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                Dim _EditClickButton As LinkButton = e.Row.Cells(Me._Coverages_EditClickIndex).Controls(0)
                _jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._Coverages_RemoveClickIndex).Controls(0)
                _jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"

                Dim _jsCommand As String

                Dim _EditAcceptButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Accept))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditAcceptButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                If js.StartsWith("javascript:") Then
                    js = "javascrip: if (CheckConvertControls('') == true) { " & js.Substring(CStr("javascript:").Length) & " }"
                End If
                _EditAcceptButton.Attributes("onclick") = js
                _EditAcceptButton.Attributes("style") += "cursor:pointer;"

                Dim _EditCancelButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Cancel))
                _jsCommand = ClientScript.GetPostBackClientHyperlink(_EditCancelButton, "")
                js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                _EditCancelButton.Attributes("onclick") = js
                _EditCancelButton.Attributes("style") += "cursor:pointer;"

                Dim oCoverageData As DataRowView = e.Row.DataItem

                ' Establecemos controles
                Dim ddlAssignments As DropDownList = e.Row.Cells(2).FindControl("IDAssignment_DropDownList")
                Dim lblAssignment As Label = e.Row.Cells(2).FindControl("Assignment_Label")
                Dim lblExpectedCoverage As Label = e.Row.Cells(3).FindControl("ExpectedCoverage_Label")
                Dim txtExpectedCoverage As HtmlInputText = e.Row.Cells(3).FindControl("ExpectedCoverage_TextBox")

                If ddlAssignments IsNot Nothing Then
                    If Not IsDBNull(oCoverageData("IDAssignment")) Then
                        ddlAssignments.SelectedValue = oCoverageData("IDAssignment")
                        lblAssignment.Text = ddlAssignments.SelectedItem.Text
                    Else
                        ddlAssignments.SelectedValue = 0
                    End If
                End If

                If lblExpectedCoverage IsNot Nothing Then
                    If Not IsDBNull(oCoverageData("ExpectedCoverage")) Then
                        lblExpectedCoverage.Text = CStr(oCoverageData("ExpectedCoverage")).Replace(HelperWeb.GetDecimalDigitFormat, ".")
                    Else
                        lblExpectedCoverage.Text = "0"
                    End If
                End If
                If txtExpectedCoverage IsNot Nothing Then
                    txtExpectedCoverage.Value = lblExpectedCoverage.Text
                End If

                ' Traducir manualmente controles de la grid
                _EditButton.Attributes("title") = Me.Language.Keyword("Button.Edit")
                _RemoveButton.Attributes("title") = Me.Language.Keyword("Button.Delete")
                _EditAcceptButton.Attributes("title") = Me.Language.Keyword("Button.Apply")
                _EditCancelButton.Attributes("title") = Me.Language.Keyword("Button.Cancel")

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdCoverages_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdCoverages.RowCommand

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
                _gridView.DataSource = Me.CoveragesData
                _gridView.DataBind()

                Me.grdCoverages_EditBegin(_rowIndex, _columnIndex)

            Case "RemoveClick"
                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                grdCoverages_Remove(_rowIndex)

        End Select

    End Sub

    Private Sub grdCoverages_EditBegin(ByVal intRowIndex As Integer, Optional ByVal intColIndex As Integer = -1)

        Dim _gridView As GridView = Me.grdCoverages

        Dim oControl As Control
        Dim intCell As Integer
        For intIndex As Integer = 0 To Me._Coverages_EditCellsIndex.Length - 1
            intCell = Me._Coverages_EditCellsIndex(intIndex)
            ' Ocultar los controls no editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Coverages_CaptionControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = False
            ' Clear the attributes from the selected cell to remove the click event
            _gridView.Rows(intRowIndex).Cells(intCell).Attributes.Clear()
            ' Hacer visibles los controles editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Coverages_EditControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = True

        Next

        '_gridView.Columns(Me._Entries_selectCellIndex).HeaderStyle.Width = "50"
        '_gridView.Columns(Me._Entries_selectCellIndex).ItemStyle.Width = "50"

        ' Poner las imágenes de editar y eliminar invisibles
        Dim oEditButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Edit))
        oEditButton.Visible = False
        Dim oRemoveButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Remove))
        oRemoveButton.Visible = False
        ' Poner las imágenes de aceptar y cancelar visibles
        Dim oEditAcceptButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Accept))
        oEditAcceptButton.Visible = True
        Dim oEditCancelButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Cancel))
        oEditCancelButton.Visible = True

    End Sub

    Private Sub grdCoverages_EditAccept(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdCoverages

        Try

            If _gridView.PageCount > 1 Then
                intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
            End If

            ' Insertamos los datos a la tabla
            Dim dt As DataTable = Me.CoveragesData()
            Dim dr As DataRow = dt.Rows(intRowIndex)
            dr.BeginEdit()

            ' Obtenemos los valores de las columnas editables
            Dim oControl As Control
            Dim intCell As Integer
            For intIndex As Integer = 0 To Me._Coverages_EditCellsIndex.Length - 1

                intCell = Me._Coverages_EditCellsIndex(intIndex)

                oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Coverages_EditControls(intIndex))
                If oControl IsNot Nothing Then

                    If TypeOf oControl Is TextBox Then
                        Dim strValue As String = CType(oControl, TextBox).Text
                        dr(Me._Coverages_EditFields(intIndex)) = strValue
                    ElseIf TypeOf oControl Is DropDownList Then
                        dr(Me._Coverages_EditFields(intIndex)) = CType(oControl, DropDownList).SelectedValue
                    ElseIf TypeOf oControl Is HtmlInputText Then
                        Dim strValue As String = CType(oControl, HtmlInputText).Value
                        If CType(oControl, HtmlInputText).Attributes("ConvertControl") = "NumberField" Then
                            strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                        End If
                        dr(Me._Coverages_EditFields(intIndex)) = strValue
                    End If

                End If

            Next

            dr.EndEdit()

            Me.CoveragesData() = dt

            ' Refrescamos la grid
            _gridView.DataSource = dt
            _gridView.DataBind()
        Catch ex As Exception
            ' Repopulate the GridView
            _gridView.DataSource = Me.CoveragesData()
            _gridView.DataBind()

        End Try

    End Sub

    Private Sub grdCoverages_EditCancel(ByVal intRowIndex As Integer)

        Dim _gridView As GridView = Me.grdCoverages

        If _gridView.PageCount > 1 Then
            intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
        End If

        ' Insertamos los datos a la tabla
        Dim dt As DataTable = Me.CoveragesData()
        Dim dr As DataRow = dt.Rows(intRowIndex)
        If IsDBNull(dr("IDAssignment")) Then dr.Delete()
        dt.AcceptChanges()

        _gridView.DataSource = Me.CoveragesData()
        _gridView.DataBind()

    End Sub

    Private Sub grdCoverages_Remove(ByVal intRowIndex As Integer)

        ' Verificamos que no estemos en edición
        If Not Me.EditingData() Then

            If intRowIndex > -1 Then

                Dim Rows() As DataRow = Me.CoveragesData.Select("", "", DataViewRowState.CurrentRows)
                If Rows.Length > intRowIndex Then
                    Rows(intRowIndex).Delete()
                End If
                Me.CoveragesData.AcceptChanges()

                'Dim oRow As DataRow = Me.EntriesData.Rows(intRowIndex)

                'If Me.EntriesData.Rows(intRowIndex).RowState <> DataRowState.Added Then
                '    oRow.Delete()
                'Else
                '    Me.EntriesData.Rows.RemoveAt(intRowIndex)
                'End If

                Dim bolEmpty As Boolean = True
                For Each oRow As DataRow In Me.CoveragesData.Rows
                    If oRow.RowState <> DataRowState.Deleted Then
                        bolEmpty = False
                        Exit For
                    End If
                Next
                If bolEmpty Then
                    Me.CoveragesData.Rows.Add(Me.CoveragesData.NewRow)
                End If

                Me.LoadData()

            End If

        End If

    End Sub

#End Region

#End Region

#Region "Methods"

    Private Sub LoadData()

        With Me.grdCoverages
            .DataSourceID = ""
            .DataSource = Me.CoveragesData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdCoverages)

    End Sub

    Private Function EditingData() As Boolean

        Dim bolEditing As Boolean = False

        Dim oImage As HtmlImage
        For Each oRow As GridViewRow In Me.grdCoverages.Rows
            oImage = oRow.FindControl(Me._Coverages_ActionButtons(Me._Action_Accept))
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

    Private Function NewCoverageData(ByVal tbCoverages As DataTable) As DataRow

        Dim oNew As DataRow = tbCoverages.NewRow
        With oNew
            .Item("IDGroup") = Me.intIDGroup
            .Item("Date") = Me.xDate
            .Item("IDAssignment") = DBNull.Value
            .Item("ExpectedCoverage") = 1
        End With
        Return oNew

    End Function

    Private Sub SaveData()

        Dim oCoverages As New roDailyCoverage

        oCoverages.IDGroup = Me.intIDGroup
        oCoverages.CoverageDate = Me.xDate

        Dim oCoverage As roDailyCoverageAssignment = Nothing
        For Each oRow As DataRow In Me.CoveragesData.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("IDAssignment")) Then
                    oCoverage = New roDailyCoverageAssignment
                    oCoverage.IDAssignment = oRow("IDAssignment")
                    oCoverage.ExpectedCoverage = oRow("ExpectedCoverage")

                    If oCoverages.CoverageAssignments Is Nothing Then oCoverages.CoverageAssignments = New List(Of roDailyCoverageAssignment)
                    oCoverages.CoverageAssignments.Add(oCoverage)
                End If
            End If
        Next

        If API.SchedulerServiceMethods.SaveTeoricDailyCoverage(Me, oCoverages) Then
            Me.CanClose = True
            Me.MustRefresh = "DailyCoverageTeoric"
        Else

        End If
    End Sub

#End Region

End Class