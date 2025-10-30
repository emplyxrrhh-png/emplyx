Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Scheduler_AddCoverage
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private intIDEmployee As Integer
    Private xDate As Date

    Private _Action_Select As Integer = 0
    Private _Action_Selected As Integer = 1

    Private _Employees_SelectClickIndex As Integer = 0
    Private _Employees_selectCellIndex As Integer = 1
    Private _Employees_ActionButtons() As String = {"imgSelect", "imgSelected"}
    Private _Employees_EditCellsIndex() As Integer = {3}
    Private _Employees_EditControls() As String = {"IDShift_DropDownList"}
    Private _Employees_CaptionControls() As String = {"Shift_Label"}
    Private _Employees_EditFields() As String = {"IDShift1"}

    Private oPermission As Permission

#End Region

#Region "Properties"

    Public Property IDAssignment() As Integer
        Get
            Return roTypes.Any2Integer(ViewState("AddCoverage_IDAssignment"))
        End Get
        Set(ByVal value As Integer)
            ViewState("AddCoverage_IDAssignment") = value
        End Set
    End Property

    Public Property IDShift() As Integer
        Get
            Return roTypes.Any2Integer(ViewState("AddCoverage_IDShift"))
        End Get
        Set(ByVal value As Integer)
            ViewState("AddCoverage_IDShift") = value
        End Set
    End Property

    Public Property AssignmentName() As String
        Get
            Return roTypes.Any2String(ViewState("AddCoverage_AssignmentName"))
        End Get
        Set(ByVal value As String)
            ViewState("AddCoverage_AssignmentName") = value
        End Set
    End Property

    Public Property IDEmployeeSelected() As Integer
        Get
            Return roTypes.Any2Integer(ViewState("AddCoverage_IDEmployeeSelected"))
        End Get
        Set(ByVal value As Integer)
            ViewState("AddCoverage_IDEmployeeSelected") = value
        End Set
    End Property

    Public Property IDShiftSelected() As Integer
        Get
            Return roTypes.Any2Integer(ViewState("AddCoverage_IDShiftSelected"))
        End Get
        Set(ByVal value As Integer)
            ViewState("AddCoverage_IDShiftSelected") = value
        End Set
    End Property

    Private Property EmployeesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("AddCoverage_EmployeesData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.EmployeeServiceMethods.GetCoverageEmployees(Me, Me.intIDEmployee, Me.xDate)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    tb.Rows.Add(tb.NewRow)
                End If

                ViewState("AddCoverage_EmployeesData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("AddCoverage_EmployeesData") = value
        End Set
    End Property

    Private Property ShiftsData() As DataView
        Get

            Dim tbShifts As DataTable = ViewState("AddCoverage_ShiftsData")
            Dim dv As DataView = Nothing
            If tbShifts IsNot Nothing Then
                dv = New DataView(tbShifts)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then
                Dim tb As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me, , , Me.IDAssignment)
                If tb IsNot Nothing Then
                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"
                    ViewState("AddCoverage_ShiftsData") = dv.Table
                End If
            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                ViewState("AddCoverage_ShiftsData") = value.Table
            Else
                ViewState("AddCoverage_ShiftsData") = Nothing
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

            Me.intIDEmployee = roTypes.Any2Integer(Me.Request("IDEmployee"))
            Dim strDate As String = roTypes.Any2String(Request("CoverageDate")) ' formato dd/MM/yyyy
            If strDate.Length >= 10 And IsDate(strDate) Then
                Me.xDate = New DateTime(Year(roTypes.Any2Time(strDate).Value), Month(roTypes.Any2Time(strDate).Value), Day(roTypes.Any2Time(strDate).Value), 0, 0, 0)
                'Me.xDate = New DateTime(CInt(strDate.Substring(6, 4)), CInt(strDate.Substring(3, 2)), CInt(strDate.Substring(0, 2)), 0, 0, 0)
            End If

            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If
            If Not Me.IsPostBack Then

                Me.EmployeesData = Nothing
                Me.ShiftsData = Nothing

                Dim tbPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, Me.intIDEmployee, Me.xDate)
                If tbPlan IsNot Nothing AndAlso tbPlan.Rows.Count > 0 Then
                    Me.IDAssignment = roTypes.Any2Integer(tbPlan.Rows(0).Item("IDAssignment"))
                    Me.AssignmentName = roTypes.Any2String(tbPlan.Rows(0).Item("AssignmentName"))
                    Me.IDShift = roTypes.Any2Integer(tbPlan.Rows(0).Item("IDShift1"))
                End If

                ' Verificamos que el puesto del empleado a cubrir esté en la cobertura del día
                Dim bolValidAssignment As Boolean = False
                Dim intIDGroup As Integer = -1

                Dim oDailyCoverage As roDailyCoverageAssignment = API.SchedulerServiceMethods.GetDailyCoverageAssignmentFromEmployeeDate(Me, Me.intIDEmployee, Me.xDate)
                If oDailyCoverage IsNot Nothing Then
                    If oDailyCoverage.IDAssignment = Me.IDAssignment Then
                        bolValidAssignment = True
                    End If
                End If

                ''Dim tbMobilities As DataTable = API.EmployeeServiceMethods.GetMobilities(Me, Me.intIDEmployee)
                ''Dim oRowsSelect() As DataRow = tbMobilities.Select("BeginDate <= '" & Format(Me.xDate, "yyyy/MM/dd") & "' AND EndDate >= '" & Format(Me.xDate, "yyyy/MM/dd") & "'")
                ''If oRowsSelect.Length = 1 Then
                ''    intIDGroup = roTypes.Any2Integer(oRowsSelect(0).Item("IDGroup"))
                ''End If
                ''Dim oDailyCoverage As SchedulerSvc.roDailyCoverage = SchedulerService.SchedulerServiceMethods.GetDailyCoverage(Me, intIDGroup, Me.xDate, False)
                ''For Each oDailyAssignment As SchedulerSvc.roDailyCoverageAssignment In oDailyCoverage.CoverageAssignments
                ''    If oDailyAssignment.IDAssignment = Me.IDAssignment Then
                ''        bolValidAssignment = True
                ''        Exit For
                ''    End If
                ''Next

                If Not bolValidAssignment Then Me.hdnIncorrectAssignment.Value = "1"

                Dim strEmployeeName As String = ""
                Dim strAssignmentName As String = ""
                Dim strShiftName As String = ""
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.intIDEmployee, False)
                If oEmployee IsNot Nothing Then strEmployeeName = oEmployee.Name
                Dim oAssignment As roAssignment = API.AssignmentServiceMethods.GetAssignment(Me, Me.IDAssignment, False)
                If oAssignment IsNot Nothing Then strAssignmentName = oAssignment.Name
                Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Me, Me.IDShift, False)
                If oShift IsNot Nothing Then strShiftName = oShift.Name

                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{1}", strEmployeeName)
                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{2}", Format(Me.xDate, HelperWeb.GetShortDateFormat))
                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{3}", strAssignmentName)
                Me.lblTitle.Text = Me.lblTitle.Text.Replace("{4}", strShiftName)

                Me.LoadData()

            End If

            ''Try
            ''    If Request.Form("__EVENTTARGET").EndsWith(Me._Employees_ActionButtons(Me._Action_Selected)) Then
            ''        Me.grdEmployees_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
            ''    End If
            ''Catch
            ''End Try

            AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.ExistItemSelected() Then
            Me.SaveData()
        End If

    End Sub

    Protected Sub btIncorrectAssignment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btIncorrectAssignment.Click

        HelperWeb.ShowOptionMessage(Me, Me.Language.Translate("AddCoverage.IncorrectAssignment.Title", Me.DefaultScope), Me.Language.Translate("AddCoverage.IncorrectAssignment.Description", Me.DefaultScope),
                                     Me.Language.Keyword("Button.Accept"), "IncorrectAssignment", "", "Close();", True, False,
                                     "", "", "", "", False, False, "", "", "", "", False, False, "", "", "", "", False, False, "", HelperWeb.MsgBoxIcons.AlertIcon)

        Me.hdnIncorrectAssignment.Value = "0"

    End Sub

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

                Dim _jsSelect As String
                'Dim js As String

                Dim _SelectClickButton As LinkButton = e.Row.Cells(Me._Employees_SelectClickIndex).Controls(0)
                _jsSelect = ClientScript.GetPostBackClientHyperlink(_SelectClickButton, "")

                Dim _SelectButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Select))
                _SelectButton.Attributes("onclick") = _jsSelect
                _SelectButton.Attributes("style") += "cursor:pointer;"

                'Dim _jsCommand As String

                Dim _SelectedButton As HtmlImage = e.Row.Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Selected))
                ''_jsCommand = ClientScript.GetPostBackClientHyperlink(_EditAcceptButton, "")
                ''js = _jsCommand.Insert(_jsCommand.Length - 2, e.Row.RowIndex)
                ''If js.StartsWith("javascript:") Then
                ''    js = "javascrip: if (CheckConvertControls('') == true) { " & js.Substring(CStr("javascript:").Length) & " }"
                ''End If
                ''_EditAcceptButton.Attributes("onclick") = js
                ''_EditAcceptButton.Attributes("style") += "cursor:pointer;"

                Dim oCoverageData As DataRowView = e.Row.DataItem

                ' Establecemos controles
                Dim ddlShifts As DropDownList = e.Row.Cells(2).FindControl("IDShift_DropDownList")
                Dim lblShift As Label = e.Row.Cells(2).FindControl("Shift_Label")

                If ddlShifts IsNot Nothing Then
                    If Not IsDBNull(oCoverageData("IDShift1")) Then
                        ddlShifts.SelectedValue = oCoverageData("IDShift1")
                        lblShift.Text = roTypes.Any2String(oCoverageData("ShiftName"))
                    Else
                        ddlShifts.SelectedValue = Me.IDShift
                    End If
                End If

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

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
                ' Set the Gridview selected index
                _gridView.SelectedIndex = _rowIndex
                ' Bind the Gridview
                _gridView.DataSource = Me.EmployeesData
                _gridView.DataBind()

                ''Me.IDEmployeeSelected = Me.EmployeesData.Rows(_rowIndex).Item("IDEmployee")

                Me.grdEmployees_EditBegin(_rowIndex, _columnIndex)

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

        Dim oEmployeeData As DataRow = Me.EmployeesData.Rows(intRowIndex)
        If IsDBNull(oEmployeeData("IDAssignment")) OrElse roTypes.Any2Integer(oEmployeeData("IDAssignment")) <> Me.IDAssignment Then

            _gridView.Rows(intRowIndex).Cells(4).Text = Me.AssignmentName
            _gridView.Rows(intRowIndex).Cells(4).ForeColor = Drawing.Color.Red

        End If

        '_gridView.Columns(Me._Entries_selectCellIndex).HeaderStyle.Width = "50"
        '_gridView.Columns(Me._Entries_selectCellIndex).ItemStyle.Width = "50"

        ' Poner las imágenes de editar y eliminar invisibles
        Dim oEditButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Select))
        oEditButton.Visible = False
        ' Poner las imágenes de aceptar y cancelar visibles
        Dim oEditAcceptButton As HtmlImage = _gridView.Rows(intRowIndex).Cells(Me._Employees_selectCellIndex).FindControl(Me._Employees_ActionButtons(Me._Action_Selected))
        oEditAcceptButton.Visible = True

    End Sub

    ''Private Sub grdEmployees_EditAccept(ByVal intRowIndex As Integer)

    ''    Dim _gridView As GridView = Me.grdEmployees

    ''    Try

    ''        If _gridView.PageCount > 1 Then
    ''            intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
    ''        End If

    ''        ' Insertamos los datos a la tabla
    ''        Dim dt As DataTable = Me.CoveragesData()
    ''        Dim dr As DataRow = dt.Rows(intRowIndex)
    ''        dr.BeginEdit()

    ''        ' Obtenemos los valores de las columnas editables
    ''        Dim oControl As Control
    ''        Dim intCell As Integer
    ''        For intIndex As Integer = 0 To Me._Employees_EditCellsIndex.Length - 1

    ''            intCell = Me._Employees_EditCellsIndex(intIndex)

    ''            oControl = _gridView.Rows(intRowIndex).Cells(intCell).FindControl(Me._Employees_EditControls(intIndex))
    ''            If oControl IsNot Nothing Then

    ''                If TypeOf oControl Is TextBox Then
    ''                    Dim strValue As String = CType(oControl, TextBox).Text
    ''                    dr(Me._Employees_EditFields(intIndex)) = strValue
    ''                ElseIf TypeOf oControl Is DropDownList Then
    ''                    dr(Me._Employees_EditFields(intIndex)) = CType(oControl, DropDownList).SelectedValue
    ''                ElseIf TypeOf oControl Is HtmlInputText Then
    ''                    Dim strValue As String = CType(oControl, HtmlInputText).Value
    ''                    If CType(oControl, HtmlInputText).Attributes("ConvertControl") = "NumberField" Then
    ''                        strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
    ''                    End If
    ''                    dr(Me._Employees_EditFields(intIndex)) = strValue
    ''                End If

    ''            End If

    ''        Next

    ''        dr.EndEdit()

    ''        Me.CoveragesData() = dt

    ''        ' Refrescamos la grid
    ''        _gridView.DataSource = dt
    ''        _gridView.DataBind()

    ''    Catch ex As Exception
    ''        ' Repopulate the GridView
    ''        _gridView.DataSource = Me.CoveragesData()
    ''        _gridView.DataBind()

    ''    End Try

    ''End Sub

    ''Private Sub grdEmployees_EditCancel(ByVal intRowIndex As Integer)

    ''    Dim _gridView As GridView = Me.grdEmployees

    ''    If _gridView.PageCount > 1 Then
    ''        intRowIndex = (_gridView.PageIndex * _gridView.PageSize) + intRowIndex
    ''    End If

    ''    ' Insertamos los datos a la tabla
    ''    Dim dt As DataTable = Me.CoveragesData()
    ''    Dim dr As DataRow = dt.Rows(intRowIndex)
    ''    If IsDBNull(dr("IDAssignment")) Then dr.Delete()
    ''    dt.AcceptChanges()

    ''    _gridView.DataSource = Me.CoveragesData()
    ''    _gridView.DataBind()

    ''End Sub

#End Region

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Dim stopType As Integer = -1
        If strButtonKey.Split("_").Length >= 2 Then
            Select Case strButtonKey.Split("_")(1)
                Case "0"
                    stopType = 0
                Case "1"
                    stopType = 1
            End Select
        End If

        Dim oActionOther As LockedDayAction = LockedDayAction.None
        If strButtonKey.Split("_").Length >= 3 Then
            Dim strResponse As String = strButtonKey.Split("_")(2)
            Select Case strResponse
                Case "ReplaceFirst"
                    oActionOther = LockedDayAction.ReplaceFirst
                Case "ReplaceAll"
                    oActionOther = LockedDayAction.ReplaceAll
                Case "NoReplaceFirst"
                    oActionOther = LockedDayAction.NoReplaceFirst
                Case "NoReplaceAll"
                    oActionOther = LockedDayAction.NoReplaceAll
            End Select
        End If

        Dim oAction As LockedDayAction = LockedDayAction.None
        Select Case strButtonKey.Split("_")(0)
            Case "ReplaceFirst"
                oAction = LockedDayAction.ReplaceFirst
            Case "ReplaceAll"
                oAction = LockedDayAction.ReplaceAll
            Case "NoReplaceFirst"
                oAction = LockedDayAction.NoReplaceFirst
            Case "NoReplaceAll"
                oAction = LockedDayAction.NoReplaceAll
        End Select

        If stopType = 0 Then
            Me.SaveData(oAction, oActionOther)
        ElseIf stopType = 1 Then
            Me.SaveData(oActionOther, oAction)
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadData()

        With Me.grdEmployees
            .DataSourceID = ""
            .DataSource = Me.EmployeesData
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdEmployees)

    End Sub

    Private Function ExistItemSelected() As Boolean

        Dim bolItemSelected As Boolean = False

        Dim oImage As HtmlImage
        For Each oRow As GridViewRow In Me.grdEmployees.Rows
            oImage = oRow.FindControl(Me._Employees_ActionButtons(Me._Action_Selected))
            If oImage IsNot Nothing Then
                bolItemSelected = oImage.Visible
                If bolItemSelected Then

                    Me.IDEmployeeSelected = Me.EmployeesData.Rows(oRow.RowIndex).Item("IDEmployee")

                    Dim intCell As Integer = Me._Employees_EditCellsIndex(0)
                    Dim oControl As Control = oRow.Cells(intCell).FindControl(Me._Employees_EditControls(0))
                    If oControl IsNot Nothing Then

                        If TypeOf oControl Is TextBox Then
                            Dim strValue As String = CType(oControl, TextBox).Text
                            Me.IDShiftSelected = strValue
                        ElseIf TypeOf oControl Is DropDownList Then
                            Me.IDShiftSelected = CType(oControl, DropDownList).SelectedValue
                        ElseIf TypeOf oControl Is HtmlInputText Then
                            Dim strValue As String = CType(oControl, HtmlInputText).Value
                            If CType(oControl, HtmlInputText).Attributes("ConvertControl") = "NumberField" Then
                                strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                            End If
                            Me.IDShiftSelected = strValue
                        End If

                    End If

                    Exit For
                End If
            End If
        Next

        If bolItemSelected Then
            bolItemSelected = (Me.IDEmployeeSelected > 0)
        End If

        If Not bolItemSelected Then
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("NoItemSelected.Message", Me.DefaultScope))
        ElseIf Me.IDShiftSelected <= 0 Then
            bolItemSelected = False
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("NoShiftSelected.Message", Me.DefaultScope))
        End If

        Return bolItemSelected

    End Function

    Private Sub SaveData(Optional ByVal _LockedDayAction1 As LockedDayAction = LockedDayAction.None, Optional ByVal _LockedDayAction2 As LockedDayAction = LockedDayAction.None)

        Dim intIDEmployeeLocked As Integer = -1

        If Not API.EmployeeServiceMethods.AddEmployeeCoverage(Me, Me.IDEmployeeSelected, Me.intIDEmployee, Me.xDate, _LockedDayAction1, _LockedDayAction2, intIDEmployeeLocked, Me.IDShiftSelected, True) Then

            If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then

                Dim stopType As Integer = 0
                Dim strLangTag As String = "DailyScheduleLockedDay"
                Dim oActionOther As LockedDayAction = LockedDayAction.None
                If intIDEmployeeLocked = Me.intIDEmployee Then
                    stopType = 0
                    oActionOther = _LockedDayAction2
                ElseIf intIDEmployeeLocked = Me.IDEmployeeSelected Then
                    stopType = 1
                    oActionOther = _LockedDayAction1
                End If

                If stopType = 0 Then
                    hdnLockedMsg.Value = _LockedDayAction1.ToString
                Else
                    hdnLockedMsg.Value = _LockedDayAction2.ToString
                End If

                Dim strActionOther As String = ""
                Select Case oActionOther
                    Case LockedDayAction.None
                        strActionOther = "None"
                    Case LockedDayAction.ReplaceFirst
                        strActionOther = "ReplaceFirst"
                    Case LockedDayAction.ReplaceAll
                        strActionOther = "ReplaceAll"
                    Case LockedDayAction.NoReplaceFirst
                        strActionOther = "NoReplaceFirst"
                    Case LockedDayAction.NoReplaceAll
                        strActionOther = "NoReplaceAll"
                End Select

                Dim oParams As New Generic.List(Of String)
                ' Buscamos el nombre del empleado que tiene un día bloqueado para mostrarlo por pantalla
                Dim oEmployeeLocked As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, intIDEmployeeLocked, False)
                If oEmployeeLocked IsNot Nothing Then
                    oParams.Add(oEmployeeLocked.Name)
                Else
                    oParams.Add(hdnLockedEmployee.Value)
                End If
                oParams.Add(Format(Me.xDate, HelperWeb.GetShortDateFormat))

                HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard"), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                             Me.Language.Translate(strLangTag & ".ReplaceFirst.Title", "CopyScheduleWizard"), "ReplaceFirst_" & stopType.ToString & "_" & strActionOther, Me.Language.Translate(strLangTag & ".ReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                             Me.Language.Translate(strLangTag & ".NoReplaceFirst.Title", "CopyScheduleWizard"), "NoReplaceFirst_" & stopType.ToString & "_" & strActionOther, Me.Language.Translate(strLangTag & ".NoReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                             "", "", "", "", False, True, "", "", "", "", False, True, "", HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.QuestionIcon))
                Exit Sub

            End If
        Else
            Me.CanClose = True
            Me.MustRefresh = "AddCoverage"
        End If

    End Sub

#End Region

End Class