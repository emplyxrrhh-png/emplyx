Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Scheduler_DailyCoveragePlanned
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private intIDGroup As Integer
    Private xDate As Date

    Private _Action_Select As Integer = 0
    Private _Action_Selected As Integer = 1
    Private _Action_Action As Integer = 2

    Private _Coverages_SelectClickIndex As Integer = 0
    Private _Coverages_ActionClickIndex As Integer = 1
    Private _Coverages_selectCellIndex As Integer = 2
    Private _Coverages_ActionButtons() As String = {"imgSelect", "imgSelected", "imgEdit"}
    Private _Coverages_EditCellsIndex() As Integer = {}
    Private _Coverages_EditControls() As String = {}
    Private _Coverages_CaptionControls() As String = {}
    Private _Coverages_EditFields() As String = {}

    Private _CoveragesDetail_EditClickIndex As Integer = 0
    Private _CoveragesDetail_RemoveClickIndex As Integer = 1
    Private _CoveragesDetail_selectCellIndex As Integer = 4
    Private _CoveragesDetail_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _CoveragesDetail_EditCellsIndex() As Integer = {2, 3}
    Private _CoveragesDetail_EditControls() As String = {"IDAssignment_DropDownList", "ExpectedCoverage_TextBox"}
    Private _CoveragesDetail_CaptionControls() As String = {"Assignment_Label", "ExpectedCoverage_Label"}
    Private _CoveragesDetail_EditFields() As String = {"IDAssignment", "ExpectedCoverage"}

    Private _Employees_SelectClickIndex As Integer = 0
    Private _Employees_selectCellIndex As Integer = 1
    Private _Employees_ActionButtons() As String = {"imgSelect", "imgSelected"}
    Private _Employees_EditCellsIndex() As Integer = {}

    'Private _Employees_EditControls() As String = {"IDShift_DropDownList"}
    Private _Employees_EditControls() As String = {}

    Private _Employees_CaptionControls() As String = {"Shift_Label"}
    Private _Employees_EditFields() As String = {"IDShift1"}

    Private oPermission As Permission
    Private bolMultipleShifts As Boolean = False

#End Region

#Region "Properties"

    Private Property CoveragesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("DailyCoveragePlanned_CoveragesData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.SchedulerServiceMethods.GetDailyCoverageDataTable(Me, Me.intIDGroup, Me.xDate, True)

                tb.Columns.Add(New DataColumn("AssignmentName", GetType(String)))

                If tb IsNot Nothing Then
                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(tb.NewRow)
                    Else
                        Dim oAssignment As roAssignment = Nothing
                        For Each oRow As DataRow In tb.Rows
                            If Not IsDBNull(oRow("IDAssignment")) Then
                                oAssignment = API.AssignmentServiceMethods.GetAssignment(Me, oRow("IDAssignment"), False)
                                If oAssignment IsNot Nothing Then
                                    oRow("AssignmentName") = oAssignment.Name
                                End If
                            End If
                        Next
                    End If
                End If

                ViewState("DailyCoveragePlanned_CoveragesData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("DailyCoveragePlanned_CoveragesData") = value
        End Set
    End Property

    Public Property IDAssignmentSelected() As Integer
        Get
            Return Me.hdnIDAssignmentSel.Value
        End Get
        Set(ByVal value As Integer)
            Me.hdnIDAssignmentSel.Value = value
        End Set
    End Property

    Public Property AssignmentNameSelected() As String
        Get
            Return roTypes.Any2String(ViewState("DailyCoveragePlanned_AssignmentNameSelected"))
        End Get
        Set(ByVal value As String)
            ViewState("DailyCoveragePlanned_AssignmentNameSelected") = value
        End Set
    End Property

    Private Property CoverageDetailData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("DailyCoveragePlanned_CoverageDetailData")
            Dim intIDAssignmentLoaded As Integer = ViewState("DailyCoveragePlanned_IDAssignmentLoaded")

            If bolReload OrElse tb Is Nothing OrElse Me.IDAssignmentSelected <> intIDAssignmentLoaded Then

                intIDAssignmentLoaded = Me.IDAssignmentSelected

                tb = API.SchedulerServiceMethods.GetDailyCoverageAssignmentDetailDataTable(Me, Me.intIDGroup, Me.xDate, intIDAssignmentLoaded)

                If tb IsNot Nothing Then
                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(tb.NewRow)
                    End If
                End If

                ViewState("DailyCoveragePlanned_CoverageDetailData") = tb
                ViewState("DailyCoveragePlanned_IDAssignmentLoaded") = intIDAssignmentLoaded

                ' Reestablecer ínidices selección 'grdCoverageDetail'
                ' ...

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("DailyCoveragePlanned_CoverageDetailData") = value
            If value Is Nothing Then ViewState("DailyCoveragePlanned_IDAssignmentLoaded") = -1
        End Set
    End Property

    Private Property DailyCoverageEmployeesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("DailyCoveragePlanned_DailyCoverageEmployeesData")
            Dim intIDAssignmentLoaded As Integer = ViewState("DailyCoveragePlanned_IDAssignmentLoaded2")

            If bolReload OrElse tb Is Nothing OrElse Me.IDAssignmentSelected <> intIDAssignmentLoaded Then

                intIDAssignmentLoaded = Me.IDAssignmentSelected

                tb = API.EmployeeServiceMethods.GetDailyCoverageEmployees(Me, Me.intIDGroup, intIDAssignmentLoaded, Me.xDate)

                If tb IsNot Nothing Then
                    tb.Columns.Add("Selected", GetType(Boolean))
                    tb.Columns.Add("IDShiftOriginal", GetType(Integer))
                    tb.Columns.Add("ShiftNameOriginal", GetType(String))
                    tb.Columns.Add("IDAssignmentOriginal", GetType(Integer))
                    tb.Columns.Add("AssignmentNameOriginal", GetType(String))
                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(tb.NewRow)
                    End If

                End If

                ViewState("DailyCoveragePlanned_DailyCoverageEmployeesData") = tb
                ViewState("DailyCoveragePlanned_IDAssignmentLoaded2") = intIDAssignmentLoaded

                ' Reestablecer ínidices selección 'grdCoverageDetail'
                ' ...

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("DailyCoveragePlanned_DailyCoverageEmployeesData") = value
            If value Is Nothing Then ViewState("DailyCoveragePlanned_IDAssignmentLoaded2") = -1
        End Set
    End Property

    Private Property ShiftsData() As DataView
        Get

            Dim tbShifts As DataTable = ViewState("DailyCoveragePlanned_ShiftsData")
            Dim dv As DataView = Nothing
            If tbShifts IsNot Nothing Then
                dv = New DataView(tbShifts)
                dv.Sort = "Name ASC"
            End If
            Dim intIDAssignmentLoaded As Integer = ViewState("DailyCoveragePlanned_IDAssignmentLoaded3")

            If dv Is Nothing OrElse Me.IDAssignmentSelected <> intIDAssignmentLoaded Then

                intIDAssignmentLoaded = Me.IDAssignmentSelected

                Dim tb As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me, , , Me.IDAssignmentSelected)
                If tb IsNot Nothing Then
                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    ViewState("DailyCoveragePlanned_ShiftsData") = dv.Table
                    ViewState("DailyCoveragePlanned_IDAssignmentLoaded3") = intIDAssignmentLoaded
                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("DailyCoveragePlanned_ShiftsData") = value.Table
            Else
                ViewState("DailyCoveragePlanned_ShiftsData") = Nothing
            End If
            If value Is Nothing Then ViewState("DailyCoveragePlanned_IDAssignmentLoaded3") = -1
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

            'If strDate.Length >= 10 And IsDate(strDate) Then
            'Me.xDate = New DateTime(Year(roTypes.Any2Time(strDate).Value), Month(roTypes.Any2Time(strDate).Value), Day(roTypes.Any2Time(strDate).Value), 0, 0, 0)
            'End If
            Me.xDate = roTypes.Any2DateTime(strDate)

            Me.bolMultipleShifts = API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultipleShifts")

            Me.grdCoverages.Columns(Me._Coverages_selectCellIndex).Visible = (Me.oPermission >= Permission.Write)

            If Not Me.IsPostBack Then

                Me.CoveragesData = Nothing
                Me.CoverageDetailData = Nothing
                Me.DailyCoverageEmployeesData = Nothing

                Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, Me.intIDGroup, False)
                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{1}", oGroup.Name).Replace("{2}", Format(Me.xDate, HelperWeb.GetShortDateFormat))

                Me.LoadData()
            Else
                Me.LoadData()
            End If

            LoadCombo()
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    ''Protected Sub btAddCoverage_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAddCoverage.ServerClick

    ''    If Not Me.EditingData() Then

    ''        If Me.CoveragesData.Rows.Count = 1 AndAlso HelperWeb.EmptyRow(Me.CoveragesData.Rows(0)) Then
    ''            Me.CoveragesData.Rows.RemoveAt(0)
    ''        End If
    ''        Dim oNewRow As DataRow = Me.NewCoverageData(Me.CoveragesData)
    ''        Me.CoveragesData.Rows.Add(oNewRow)
    ''        Me.LoadData()
    ''        ''Me.UpdateContext(2)

    ''        Me.grdCoverages_EditBegin(Me.grdCoverages.Rows.Count - 1)

    ''    End If

    ''End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        Dim tbEmployees As DataTable = Me.DailyCoverageEmployeesData
        Dim oRow As DataRow

        If Me.cmbShiftsMonday_Value.Value = "" Then
            Exit Sub
        End If

        'Dim StartShift1 As DateTime = Nothing
        'If strStart.Length >= 12 Then StartShift1 = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)

        For n As Integer = 0 To tbEmployees.Rows.Count - 1

            oRow = tbEmployees.Rows(n)
            'Dim ddlShifts As DropDownList = Me.grdEmployees.Rows(n).Cells(2).FindControl("IDShift_DropDownList")

            'If roTypes.Any2Boolean(oRow("Selected")) AndAlso ddlShifts.SelectedIndex >= 0 Then
            If roTypes.Any2Boolean(oRow("Selected")) Then
                API.EmployeeServiceMethods.AssignShift(Me, oRow("IDEmployee"), Me.xDate, CInt(cmbShiftsMonday_Value.Value), 0, 0, 0, Me.GetStartShift(0), Nothing, Nothing, Nothing, oRow("IDAssignment"), True)

                Me.hdnDataChanged.Value = "1"
            End If

        Next

        Me.CoveragesData = Nothing

        Me.LoadData()
        Me.LoadCombo()

        Me.mpxDailyCoverageEmployees.Hide()

        Me.updCoverages.Update()
        Me.updCoveragesDetail.Update()
        Me.updEmployees.Update()

    End Sub

    Protected Sub btRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRefresh.Click

        Me.CoveragesData = Nothing
        Me.CoverageDetailData = Nothing
        Me.DailyCoverageEmployeesData = Nothing

        Me.LoadData()
        Me.LoadCombo()

        Me.mpxDailyCoverageEmployees.Hide()

        Me.updCoverages.Update()
        Me.updCoveragesDetail.Update()
        Me.updEmployees.Update()

    End Sub

#Region "grdCoverages events"

    Protected Sub grdCoverages_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdCoverages.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

        End Select

    End Sub

    Protected Sub grdCoverages_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdCoverages.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.DataRow

                Dim oCoverageData As DataRowView = e.Row.DataItem

                ' Asignar eventos
                Dim _jsSelect As String
                Dim _jsAction As String

                Dim _SelectClickButton As LinkButton = e.Row.Cells(Me._Coverages_SelectClickIndex).Controls(0)
                _jsSelect = ClientScript.GetPostBackClientHyperlink(_SelectClickButton, "")

                ''_jsSelect = _jsSelect.Insert(_jsSelect.Length - 2, e.Row.RowIndex)
                ''e.Row.Cells(2).Attributes("onclick") = _jsSelect
                ''e.Row.Cells(2).Attributes("style") += "cursor:pointer;"

                Dim _ActionClickButton As LinkButton = e.Row.Cells(Me._Coverages_ActionClickIndex).Controls(0)
                _jsAction = ClientScript.GetPostBackClientHyperlink(_ActionClickButton, "")

                Dim _SelectButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Select))
                _SelectButton.Attributes("onclick") = _jsSelect
                _SelectButton.Attributes("style") += "cursor:pointer;"

                Dim _SelectedButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Selected))

                Dim _ActionButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Action))
                If Me.oPermission >= Permission.Write Then
                    _ActionButton.Attributes("onclick") = "ShowDailyCoverageAddPlan(" & roTypes.Any2Integer(oCoverageData("IDAssignment")) & ");"
                    _ActionButton.Attributes("style") += "cursor:pointer;"
                End If

                ' Establecemos controles
                If roTypes.Any2Double(oCoverageData("ExpectedCoverage")) > roTypes.Any2Double(oCoverageData("PlannedCoverage")) Then
                    e.Row.Cells(5).BackColor = Drawing.Color.Red
                    For Each oControl As Control In e.Row.Cells(4).Controls
                        Try
                            CType(oControl, Object).BackColor = Drawing.Color.Red
                        Catch
                        End Try
                    Next
                End If

                ''If Me.IDAssignmentSelected = roTypes.Any2Integer(oCoverageData("IDAssignment")) Then
                ''    ' Poner la imagen de seleccionar invisible
                ''    Dim oEditButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Select))
                ''    oEditButton.Visible = False
                ''    ' Poner la imágen de seleccionado visible
                ''    Dim oEditAcceptButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Selected))
                ''    oEditAcceptButton.Visible = True
                ''Else
                ''    Dim oEditButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Select))
                ''    oEditButton.Visible = True
                ''    Dim oEditAcceptButton As HtmlImage = e.Row.Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Selected))
                ''    oEditAcceptButton.Visible = False
                ''End If

                ' Traducir manualmente controles de la grid
                _SelectButton.Attributes("title") = Me.Language.Translate("grdCoverages.SelectButton.Title", Me.DefaultScope)
                _SelectedButton.Attributes("title") = Me.Language.Translate("grdCoverages.SelectedButton.Title", Me.DefaultScope)
                _ActionButton.Attributes("title") = Me.Language.Translate("Action.Plan", Me.DefaultScope)

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdCoverages_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdCoverages.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "SelectClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' Set the Gridview selected index
                _gridView.SelectedIndex = _rowIndex
                ' Bind the Gridview
                _gridView.DataSource = Me.CoveragesData
                _gridView.DataBind()

                Dim bolChangeAssignment As Boolean = False
                If Me.IDAssignmentSelected <> Me.CoveragesData.Rows(_rowIndex).Item("IDAssignment") Then
                    bolChangeAssignment = True
                End If
                Me.IDAssignmentSelected = Me.CoveragesData.Rows(_rowIndex).Item("IDAssignment")

                Me.AssignmentNameSelected = Me.CoveragesData.Rows(_rowIndex).Item("AssignmentName")

                Me.LoadCombo(bolChangeAssignment)

                Me.LoadDetailData()

                ' Poner la imagen de seleccionar invisible
                Dim oEditButton As HtmlImage = _gridView.Rows(_rowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Select))
                oEditButton.Visible = False
                ' Poner la imágen de seleccionado visible
                Dim oEditAcceptButton As HtmlImage = _gridView.Rows(_rowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Selected))
                oEditAcceptButton.Visible = True
                ' Poner la imagen de acción visible
                Dim oActionButton As HtmlImage = _gridView.Rows(_rowIndex).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Action))
                oActionButton.Visible = True

                Me.updCoveragesDetail.Update()
                Me.updEmployees.Update()

            Case "ActionClick"

                ' '' Get the row index
                ''Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' '' Parse the event argument (added in RowDataBound) to get the selected column index
                ''Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' '' Set the Gridview selected index
                ''_gridView.SelectedIndex = _rowIndex
                ' '' Bind the Gridview
                ''_gridView.DataSource = Me.CoveragesData
                ''_gridView.DataBind()

                ''Me.grdCoverages_EditBegin(_rowIndex, _columnIndex)

        End Select

    End Sub

#End Region

#Region "grdEmployees events"

    Protected Sub grdEmployees_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdEmployees.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                Dim ddlShifts As DropDownList = e.Row.Cells(3).FindControl("IDShift_DropDownList")
                If ddlShifts IsNot Nothing Then
                    With ddlShifts
                        .DataSource = Me.ShiftsData()
                        .DataTextField = "Name"
                        .DataValueField = "ID"
                        .SelectedValue = Nothing
                        .DataBind()
                    End With
                End If

        End Select

    End Sub

    Protected Sub grdEmployees_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdEmployees.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _SelectClickButton As LinkButton = e.Row.Cells(Me._Employees_SelectClickIndex).Controls(0)
                Dim _jsSelect As String = ClientScript.GetPostBackClientHyperlink(_SelectClickButton, "")

                Dim _SelectButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Select))
                _SelectButton.Attributes("onclick") = _jsSelect
                _SelectButton.Attributes("style") += "cursor:pointer;"

                Dim _SelectedButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Selected))
                _SelectedButton.Attributes("onclick") = _jsSelect
                _SelectedButton.Attributes("style") += "cursor:pointer;"

                ''Dim _jsCommand As String
                ''_jsCommand = ClientScript.GetPostBackClientHyperlink(_EditAcceptButton, "")
                ''js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                ''If js.StartsWith("javascript:") Then
                ''    js = "javascrip: if (CheckConvertControls('') == true) { " & js.Substring(CStr("javascript:").Length) & " }"
                ''End If
                ''_EditAcceptButton.Attributes("onclick") = js
                ''_EditAcceptButton.Attributes("style") += "cursor:pointer;"

                Dim oEmployeeData As DataRowView = e.Row.DataItem

                ' Establecemos controles
                Dim ddlShifts As DropDownList = e.Row.Cells(2).FindControl("IDShift_DropDownList")
                Dim lblShift As Label = e.Row.Cells(2).FindControl("Shift_Label")
                Dim oEditButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Select))
                Dim oEditAcceptButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Selected))

                If ddlShifts IsNot Nothing Then
                    If Not IsDBNull(oEmployeeData("IDShift1")) Then
                        ddlShifts.SelectedValue = oEmployeeData("IDShift1")
                        lblShift.Text = roTypes.Any2String(oEmployeeData("ShiftName"))
                    Else
                        ddlShifts.SelectedIndex = -1
                    End If
                End If

                If Not IsDBNull(oEmployeeData("IDAssignmentOriginal")) AndAlso roTypes.Any2Integer(oEmployeeData("IDAssignment")) <> roTypes.Any2Integer(oEmployeeData("IDAssignmentOriginal")) Then
                    e.Row.Cells(4).ForeColor = Drawing.Color.Red
                End If

                If CBool(roTypes.Any2Boolean(oEmployeeData("Selected"))) Then
                    Dim oControl As Control
                    Dim intCell As Integer
                    For intIndex As Integer = 0 To Me._Employees_EditCellsIndex.Length - 1
                        intCell = Me._Employees_EditCellsIndex(intIndex)
                        ' Ocultar los controls no editables
                        oControl = e.Row.Cells(intCell).FindControl(Me._Employees_CaptionControls(intIndex))
                        If oControl IsNot Nothing Then oControl.Visible = False
                        ' Clear the attributes from the selected cell to remove the click event
                        e.Row.Cells(intCell).Attributes.Clear()
                        ' Hacer visibles los controles editables
                        oControl = e.Row.Cells(intCell).FindControl(Me._Employees_EditControls(intIndex))
                        If oControl IsNot Nothing Then oControl.Visible = True
                    Next
                End If

                ' Poner las imágenes de editar y eliminar invisibles
                oEditButton.Visible = Not CBool(roTypes.Any2Boolean(oEmployeeData("Selected")))
                ' Poner las imágenes de aceptar y cancelar visibles
                oEditAcceptButton.Visible = CBool(roTypes.Any2Boolean(oEmployeeData("Selected")))

                ' Traducir manualmente controles de la grid
                _SelectButton.Attributes("title") = Me.Language.Translate("grdEmployees.SelectButton.Title", Me.DefaultScope)
                _SelectedButton.Attributes("title") = Me.Language.Translate("grdEmployees.SelectedButton.Title", Me.DefaultScope)

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdEmployees_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdEmployees.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "SelectClick"

                Dim oEmployeeData As DataRow = Nothing

                For n As Integer = 0 To _gridView.Rows.Count - 1
                    oEmployeeData = Me.DailyCoverageEmployeesData.Rows(n)
                    If roTypes.Any2Boolean(oEmployeeData.Item("Selected")) Then
                        Dim ddlShifts As DropDownList = _gridView.Rows(n).Cells(2).FindControl("IDShift_DropDownList")
                        oEmployeeData("IDShift1") = ddlShifts.SelectedValue
                        oEmployeeData("ShiftName") = oEmployeeData("ShiftName")
                    End If
                Next

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' Set the Gridview selected index
                _gridView.SelectedIndex = _rowIndex

                oEmployeeData = Me.DailyCoverageEmployeesData.Rows(_rowIndex)

                oEmployeeData("Selected") = Not CBool(roTypes.Any2Boolean(oEmployeeData("Selected")))
                If CBool(oEmployeeData("Selected")) Then
                    oEmployeeData("IDShiftOriginal") = oEmployeeData("IDShift1")
                    oEmployeeData("ShiftNameOriginal") = oEmployeeData("ShiftName")
                    oEmployeeData("IDAssignmentOriginal") = oEmployeeData("IDAssignment")
                    oEmployeeData("AssignmentNameOriginal") = oEmployeeData("AssignmentName")
                    oEmployeeData("IDAssignment") = Me.IDAssignmentSelected
                    oEmployeeData("AssignmentName") = Me.AssignmentNameSelected
                Else
                    oEmployeeData("IDShift1") = oEmployeeData("IDShiftOriginal")
                    oEmployeeData("ShiftName") = oEmployeeData("ShiftNameOriginal")
                    oEmployeeData("IDAssignment") = oEmployeeData("IDAssignmentOriginal")
                    oEmployeeData("AssignmentName") = oEmployeeData("AssignmentNameOriginal")
                    oEmployeeData("IDShiftOriginal") = DBNull.Value
                    oEmployeeData("ShiftNameOriginal") = DBNull.Value
                    oEmployeeData("IDAssignmentOriginal") = DBNull.Value
                    oEmployeeData("AssignmentNameOriginal") = DBNull.Value
                End If

                ' Bind the Gridview
                _gridView.DataSource = Me.DailyCoverageEmployeesData
                _gridView.DataBind()

                Me.DailyCoverageEmployeesData = Me.DailyCoverageEmployeesData

                ''Me.IDEmployeeSelected = Me.EmployeesData.Rows(_rowIndex).Item("IDEmployee")
                If CBool(oEmployeeData("Selected")) Then
                    Me.grdEmployees_EditBegin(_rowIndex, _columnIndex)
                End If

                Me.mpxDailyCoverageEmployees.Show()

        End Select

    End Sub

    Private Sub grdEmployees_EditBegin(ByVal intRowIndex As Integer, Optional ByVal intColIndex As Integer = -1)

        Dim _gridView As GridView = Me.grdEmployees

        Dim oControl As Control
        Dim intCell As Integer
        For intIndex As Integer = 0 To Me._Employees_EditCellsIndex.Length - 1
            intCell = Me._Employees_EditCellsIndex(intIndex)
            ' Ocultar los controls no editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Employees_CaptionControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = False
            ' Clear the attributes from the selected cell to remove the click event
            _gridView.Rows(intRowIndex).Cells(intCell).Attributes.Clear()
            ' Hacer visibles los controles editables
            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Employees_EditControls(intIndex))
            If oControl IsNot Nothing Then oControl.Visible = True

        Next

        Dim oEmployeeData As DataRow = Me.DailyCoverageEmployeesData.Rows(intRowIndex)

        If IsDBNull(oEmployeeData("IDAssignmentOriginal")) OrElse roTypes.Any2Integer(oEmployeeData("IDAssignmentOriginal")) <> Me.IDAssignmentSelected Then

            _gridView.Rows(intRowIndex).Cells(4).Text = Me.AssignmentNameSelected
            _gridView.Rows(intRowIndex).Cells(4).ForeColor = Drawing.Color.Red

        End If

        '_gridView.Columns(Me._Entries_selectCellIndex).HeaderStyle.Width = "50"
        '_gridView.Columns(Me._Entries_selectCellIndex).ItemStyle.Width = "50"

        ' Poner las imágenes de editar y eliminar invisibles
        Dim oEditButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Select))
        oEditButton.Visible = Not CBool(Me.DailyCoverageEmployeesData.Rows(intRowIndex).Item("Selected"))
        ' Poner las imágenes de aceptar y cancelar visibles
        Dim oEditAcceptButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Selected))
        oEditAcceptButton.Visible = CBool(Me.DailyCoverageEmployeesData.Rows(intRowIndex).Item("Selected"))

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

        Me.IDAssignmentSelected = -1
        If Me.CoveragesData.Rows.Count > 0 Then

            Me.IDAssignmentSelected = roTypes.Any2Integer(Me.CoveragesData.Rows(0).Item("IDAssignment"))
            Me.AssignmentNameSelected = roTypes.Any2String(Me.CoveragesData.Rows(0).Item("AssignmentName"))

            ' Poner la imagen de seleccionar invisible
            Dim oEditButton As HtmlImage = Me.grdCoverages.Rows(0).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Select))
            oEditButton.Visible = False
            ' Poner la imágen de seleccionado visible
            Dim oEditAcceptButton As HtmlImage = Me.grdCoverages.Rows(0).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Selected))
            oEditAcceptButton.Visible = True
            ' Poner la imagen de acción visible
            Dim oActionButton As HtmlImage = Me.grdCoverages.Rows(0).Cells(Me._Coverages_selectCellIndex).FindControl(Me._Coverages_ActionButtons(Me._Action_Action))
            oActionButton.Visible = True

        End If

        Me.LoadDetailData()

    End Sub

    Private Function GetStartShift(ByVal intDayIndex As Integer) As DateTime

        Dim xRet As DateTime = Nothing

        Dim hdnIsFloating() As HtmlInputHidden = {Me.hdnIsFloatingMonday}
        Dim cmbStartFloating_Value() As HtmlInputHidden = {Me.cmbStartFloatingMonday_Value}
        Dim txtStartFloating() As DevExpress.Web.ASPxTimeEdit = {Me.txtStartFloatingMonday}

        If hdnIsFloating(intDayIndex).Value = "1" Then

            Select Case cmbStartFloating_Value(intDayIndex).Value
                Case "0"
                    xRet = New DateTime(1899, 12, 29, txtStartFloating(intDayIndex).DateTime.Hour, txtStartFloating(intDayIndex).DateTime.Minute, 0)
                Case "1"
                    xRet = New DateTime(1899, 12, 30, txtStartFloating(intDayIndex).DateTime.Hour, txtStartFloating(intDayIndex).DateTime.Minute, 0)
                Case "2"
                    xRet = New DateTime(1899, 12, 31, txtStartFloating(intDayIndex).DateTime.Hour, txtStartFloating(intDayIndex).DateTime.Minute, 0)
            End Select
        End If

        Return xRet

    End Function

    Private Sub LoadDetailData()

        With Me.grdCoverageDetail
            .DataSourceID = ""
            .DataSource = Me.CoverageDetailData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdCoverageDetail)

        With Me.grdEmployees
            .DataSourceID = ""
            .DataSource = Me.DailyCoverageEmployeesData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdEmployees)

        Dim oMsgParams As New Generic.List(Of String)
        oMsgParams.Add(Me.AssignmentNameSelected)
        Me.lblDailyCoverageEmployeesTitle.Text = Me.Language.Translate("DailyCoverageEmployees.Title", Me.DefaultScope, oMsgParams)

    End Sub

    Private Sub LoadCombo(Optional ByVal bolChangeAssignment As Boolean = False)
        Dim tbShifts As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me, , , Me.IDAssignmentSelected)

        Dim oWeekCombos() As Robotics.WebControls.roComboBox = {Me.cmbShiftsMonday}
        For Each oCmb As Robotics.WebControls.roComboBox In oWeekCombos
            oCmb.ClearItems()
        Next

        Dim strScript As String = ""
        For Each oRow As DataRow In tbShifts.Rows
            If Me.bolMultipleShifts Then
                If roTypes.Any2Boolean(oRow("IsFloating")) Then
                    strScript = "ShowStartShift('tdStartShiftMonday', '" & Me.txtStartFloatingMonday.ClientID & "','" & Me.cmbStartFloatingMonday.ClientID & "','" & Me.cmbStartFloatingMonday_Text.ClientID & "','" & Me.cmbStartFloatingMonday_Value.ClientID & "'"
                    If Not IsDBNull(oRow("StartFloating")) Then
                        strScript &= ",'" & Format(CDate(oRow("StartFloating")), "yyyyMMddHHmm") & "'"
                    Else
                        strScript &= ",null"
                    End If
                    strScript &= ",'" & Me.hdnIsFloatingMonday.ClientID & "');"
                Else
                    strScript = "HideStartShift('tdStartShiftMonday','" & Me.hdnIsFloatingMonday.ClientID & "');"
                End If
            End If
            Me.cmbShiftsMonday.AddItem(oRow("Name"), CStr(oRow("ID")), strScript)
        Next

        Dim oWeekFloatingCombos() As Robotics.WebControls.roComboBox = {Me.cmbStartFloatingMonday}
        For Each oCmb As Robotics.WebControls.roComboBox In oWeekFloatingCombos
            oCmb.ClearItems()
            oCmb.AddItem(Me.Language.Translate("StartFloating.ShiftDay", "Shift"), "1", "")
            oCmb.AddItem(Me.Language.Translate("StartFloating.ShiftAfter", "Shift"), "2", "")
            oCmb.AddItem(Me.Language.Translate("StartFloating.ShiftBefore", "Shift"), "0", "")
        Next

        If bolChangeAssignment Then
            Me.cmbShiftsMonday_Value.Value = ""
            Me.cmbShiftsMonday_Text.Value = ""
        End If

    End Sub

#End Region

End Class