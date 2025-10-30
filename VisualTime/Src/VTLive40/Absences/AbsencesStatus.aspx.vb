Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.UsersAdmin
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Absences_AbsencesStatus
    Inherits PageBase

#Region "Properties"

    Private ReadOnly Property GetCauses(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbCauses As DataTable = ViewState("AbsencesStatusFilter_Causes")

            If bolReload OrElse tbCauses Is Nothing Then
                tbCauses = CausesServiceMethods.GetCauses(Me.Page, "")
                ViewState("AbsencesStatusFilter_Causes") = tbCauses
            End If

            Return tbCauses
        End Get
    End Property

    Private ReadOnly Property GetGroups() As Generic.List(Of Integer)
        Get
            Dim Groups As Generic.List(Of Integer) = ViewState("AbsencesStatusFilter_Groups")
            If Groups Is Nothing Then
                Dim tempGroupsTb As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Employees")
                Dim fieldValues As IEnumerable(Of Integer) = tempGroupsTb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("ID")))

                ViewState("AbsencesStatusFilter_Groups") = fieldValues.ToList()
            End If
            Return Groups
        End Get
    End Property

    Private ReadOnly Property GetEmployees() As Generic.List(Of Integer)
        Get
            Dim EmployeesNotAllowed As Generic.List(Of Integer) = ViewState("AbsencesStatusFilter_Employees")
            If EmployeesNotAllowed Is Nothing Then
                Dim tempEmpTb As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Nothing, "", "Employees")
                Dim fieldValues As IEnumerable(Of Integer) = tempEmpTb.AsEnumerable().Select(Function(row) roTypes.Any2Integer(row("IDEmployee")))

                ViewState("AbsencesStatusFilter_Employees") = fieldValues.ToList()
            End If
            Return EmployeesNotAllowed
        End Get
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("SchedulerDates", "~/Scheduler/Scripts/SchedulerDates.js")
        Me.InsertExtraJavascript("AbsencesStatus", "~/Absences/Scripts/AbsencesStatus.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Not IsPostBack AndAlso Not IsCallback Then
            Me.sectionName.InnerText = Me.Language.Translate("AbsencesStatusTitle", Me.DefaultScope)

            LoadCombos()
            CreateColumns()

            Dim oPermission As Permission = Me.GetFeaturePermission("Employees")
            If oPermission < Permission.Write Then
                WLHelperWeb.RedirectAccessDenied(False)
                Exit Sub
            End If

            'NO TOCAR: Cargar propiedades sin postback para añadirlas al viewstate y que esten disponibles mas adelante en callbacks
            Dim tb As DataTable = Me.GetCauses()
            Dim lst As Generic.List(Of Integer) = Me.GetEmployees()
            lst = Me.GetGroups()
            ' FIN

            Session("AbsencesStatusFilter_AbsencesDataTable") = Nothing
            Session("AbsencesStatusFilter_LastEmployeesUsed") = Nothing
            Session("AbsencesStatusFilter_LastDateInfUsed") = Nothing
            Session("AbsencesStatusFilter_LastCausesUsed") = Nothing
            Session("AbsencesStatusFilter_LastDateSupUsed") = Nothing

            txtDateInf.Value = DateTime.Today
            txtDateSup.Value = DateTime.Today

            Me.ListCauses.ValueField = "ID"
            Me.ListCauses.TextField = "Name"
            Me.ListCauses.DataSource = GetCauses(True)
            Me.ListCauses.DataBind()

            hdnAllEmployees.Value = Me.Language.Translate("AllEmployees", DefaultScope)
            hdnAllCauses.Value = Me.Language.Translate("AllCauses", DefaultScope)
            hdnCauseSelectedText.Value = Me.Language.Translate("SelectedCause", DefaultScope)
            txtEmployees.Text = hdnAllEmployees.Value
            txtCauses.Text = hdnAllCauses.Value

            Dim strDateAux As String = HelperWeb.GetShortDateFormat.ToLower
            dtFormatText.Value = strDateAux
            strDateAux = Replace(strDateAux, "ddd", "d")
            strDateAux = Replace(strDateAux, "dd", "d")
            strDateAux = Replace(strDateAux, "mm", "m")
            strDateAux = Replace(strDateAux, "yyyy", "y")
            strDateAux = Replace(strDateAux, "yy", "y")
            strDateAux = Replace(strDateAux, "/", "")
            strDateAux = Replace(strDateAux, "-", "")
            strDateAux = Replace(strDateAux, "d", "0")
            strDateAux = Replace(strDateAux, "m", "1")
            strDateAux = Replace(strDateAux, "y", "2")
            dtFormat.Value = strDateAux

            GetAbsencesStatus(True)

            If Request.QueryString.Count Then
                ProcessQueryString()
            End If

        End If
    End Sub

    Protected Sub ProcessQueryString()
        If Request.QueryString("Status") IsNot Nothing Then
            If Request.QueryString("Status") = "-1" OrElse Request.QueryString("Status").ToLower = "all" Then
                Me.cmbStatus.SelectedIndex = 0
            ElseIf Request.QueryString("Status") = "0" OrElse Request.QueryString("Status").ToLower = "athome" Then
                Me.cmbStatus.SelectedIndex = 1
            ElseIf Request.QueryString("Status") = "1" OrElse Request.QueryString("Status").ToLower = "comethisweek" Then
                Me.cmbStatus.SelectedIndex = 2
            End If
            GetAbsencesStatus(True)
        End If
        If Request.QueryString("DocumentsDelivered") IsNot Nothing Then
            If Request.QueryString("DocumentsDelivered") = "1" OrElse Request.QueryString("DocumentsDelivered").ToLower = "si" Or Request.QueryString("DocumentsDelivered").ToLower = "yes" Then
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AddFilterDocumentsDelivered", "GridAbsencesStatusClient.AutoFilterByColumn('DocumentsDelivered','1');", True)
            Else
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AddFilterDocumentsDelivered", "GridAbsencesStatusClient.AutoFilterByColumn('DocumentsDelivered','0');", True)
            End If
        End If

        If Request.QueryString("IDEmployee") IsNot Nothing Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AddFilterEmployee", "GridAbsencesStatusClient.AutoFilterByColumn('EmployeeName','" & API.EmployeeServiceMethods.GetEmployeeName(Me.Page, roTypes.Any2Integer(Request.QueryString("IDEmployee"))) & "');", True)
        End If

    End Sub

    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "GETINFOSELECTED"
                e.Result = GetInfoSelected()
            Case "REFRESHGRID"
                CreateColumns()
                GetAbsencesStatus(True)
            Case "CLIENTSHOWDETAILS"
                Dim parameters = roJSONHelper.Deserialize(hdnSelectedRowVisibleIndex.Value, GetType(String()))
                Dim curAbsence As sysrovwAbsences = GridAbsencesStatus.GetRow(roTypes.Any2Integer(hdnSelectedRowVisibleIndex.Value))

                Select Case parameters(3)
                    Case "ProgrammedAbsence"
                        e.Result = "1;Employees/EditProgrammedAbsence.aspx?EmployeeID=" & parameters(0) & "&BeginDate=" + Convert.ToDateTime(parameters(1))
                    Case "ProgrammedCause"
                        e.Result = "2;Employees/EditProgrammedIncidence.aspx?EmployeeID=" & parameters(0) & "&BeginDate=" + Convert.ToDateTime(parameters(1)) & "&AbsenceID=" & parameters(2)
                    Case Else
                        e.Result = "3;" & parameters(2)
                End Select
                'If curAbsence.AbsenceType = "ProgrammedAbsence" Then
                '    e.Result = "1;Employees/EditProgrammedAbsence.aspx?EmployeeID=" & parameters(0) & "&BeginDate=" + Convert.ToDateTime(parameters(1))
                'ElseIf curAbsence.AbsenceType = "ProgrammedCause" Then
                '    e.Result = "2;Employees/EditProgrammedIncidence.aspx?EmployeeID=" & parameters(0) & "&BeginDate=" + Convert.ToDateTime(parameters(1)) & "&AbsenceID=" & parameters(2)
                'Else
                '    'e.Result = "3;Requests/EditRequest.aspx?RequestID=" & curAbsence.IDRelatedObject & "&IdTableRow=0"
                '    e.Result = "3;" & parameters(2)
                'End If
            Case "CLIENTEMPLOYEEDETAILS"
                Dim parameters = roJSONHelper.Deserialize(hdnSelectedRowVisibleIndex.Value, GetType(String()))
                Dim curAbsence As sysrovwAbsences = GridAbsencesStatus.GetRow(parameters(0))
                e.Result = parameters(0) & "@" & Configuration.RootUrl
            Case "UPDATECAUSETEXT"
                e.Result = CausesServiceMethods.GetCauseByID(Me.Page, roTypes.Any2Integer(hdnCausesSelected.Value), False).Name
        End Select

    End Sub

    Private Sub LoadCombos()

        Me.cmbStatus.Items.Clear()
        Me.cmbStatus.Items.Add(Me.Language.Translate("Status.All", DefaultScope), "-1")
        Me.cmbStatus.Items.Add(Me.Language.Translate("Status.AtHome", DefaultScope), "0")
        Me.cmbStatus.Items.Add(Me.Language.Translate("Status.ComeThisWeek", DefaultScope), "1")
        Me.cmbStatus.Items.Add(Me.Language.Translate("Status.BetweenEndDate", DefaultScope), "2")
        Me.cmbStatus.Items.Add(Me.Language.Translate("Status.BetweenStartDate", DefaultScope), "3")
        Me.cmbStatus.SelectedIndex = 1

        Me.txtDateInf.Value = DateTime.Today.AddDays(-6)
        Me.txtDateSup.Value = DateTime.Today
    End Sub

#Region "OBTENCION DE DATOS DE LINQ"

    Protected Sub linqAbsencesDataSource_Selecting(ByVal sender As Object, ByVal e As DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs) Handles LinqAbsencesDataSource.Selecting
        'enlazar con datatable en runtime http://www.devexpress.com/Support/Center/p/E168.aspx
        e.QueryableSource = GetAbsencesStatus()
    End Sub

    Private Function GetAbsencesStatus(Optional ByVal bolReload As Boolean = False) As Object

        Dim qry As IQueryable(Of sysrovwAbsences) = Session("AbsencesStatusFilter_AbsencesDataTable")
        Dim LastEmployeesUsed As String = Session("AbsencesStatusFilter_LastEmployeesUsed")
        Dim LastDateInfUsedZZ As Nullable(Of DateTime) = Session("AbsencesStatusFilter_LastDateInfUsed")
        Dim LastDateSupUsedZZ As Nullable(Of DateTime) = Session("AbsencesStatusFilter_LastDateSupUsed")
        Dim LastCausesUsed As String = Session("AbsencesStatusFilter_LastCausesUsed")

        Dim strEmployeeFilters As String = hdnEmployees.Value.Trim & "|" & hdnFilter.Value.Trim & "|" & hdnFilterUser.Value.Trim()

        If bolReload OrElse qry Is Nothing OrElse
           LastEmployeesUsed <> strEmployeeFilters OrElse
           Not LastDateInfUsedZZ.HasValue OrElse LastDateInfUsedZZ.Value <> txtDateInf.Text OrElse
           Not LastDateSupUsedZZ.HasValue OrElse LastDateSupUsedZZ.Value <> txtDateSup.Text OrElse
           LastCausesUsed <> hdnCausesSelected.Value Then

            Dim DateInf As DateTime = txtDateInf.Value
            Dim DateSup As DateTime = txtDateSup.Value

            If DateInf > DateSup Then
                Dim aux As DateTime = DateSup
                DateSup = DateInf
                DateInf = aux
            End If

            Dim strConn As String = API.UserAdminServiceMethods.GetConnectionString(Me, WLHelperWeb.CurrentPassport().ID)
            Dim ContextDB As AbsencesDataContext = New AbsencesDataContext(strConn)
            ContextDB.CommandTimeout = 900

            qry = ContextDB.sysrovwAbsences

            '============ EMPLOYEES
            If LastEmployeesUsed <> strEmployeeFilters OrElse bolReload Then

                Dim lstGroups As New Generic.List(Of Integer)
                Dim lstEmployees As New Generic.List(Of Integer)

                Robotics.Security.roSelector.GetExpandedGroupsAndEmployees(hdnEmployees.Value, lstEmployees, lstGroups)

                If lstGroups.Count > 0 OrElse lstEmployees.Count > 0 Then
                    If lstEmployees.Count = 0 Then lstEmployees.Add(-1)

                    'qry = qry.Where(BuildPredicate(lstEmployees, lstGroups, Me.GetGroups(), Me.GetEmployees()))

                    'Dim provisionalList = qry.ToList()

                    'Dim secondQry = provisionalList.AsQueryable()

                    'qry = secondQry.Where(BuildPredicate(lstEmployees, lstGroups, Me.GetGroups(), Me.GetEmployees()))

                    If hdnFilter.Value <> "" AndAlso hdnFilter.Value.Length = 5 AndAlso hdnFilter.Value <> "11110" Then 'Filtros de Empleados
                        qry = BuildEmployeeFilter(qry)
                    End If

                    Dim tmpList As Generic.List(Of sysrovwAbsences) = qry.ToList()
                    tmpList = tmpList.FindAll(Function(item) (lstEmployees.Contains(item.IDEmployee) AndAlso Me.GetEmployees().Contains(item.IDEmployee)) OrElse
                        (lstGroups.Contains(item.IDGroup) AndAlso Me.GetGroups.Contains(item.IDGroup) AndAlso Me.GetEmployees.Contains(item.IDEmployee)))

                    qry = tmpList.AsQueryable()
                Else

                    'qry = qry.Where(BuildPredicate(lstEmployees, lstGroups, Me.GetGroups(), Me.GetEmployees()))

                    'Dim provisionalList = qry.ToList()

                    'Dim secondQry = provisionalList.AsQueryable()

                    'qry = secondQry.Where(BuildPredicate(lstEmployees, lstGroups, Me.GetGroups(), Me.GetEmployees()))

                    Dim tmpList As Generic.List(Of sysrovwAbsences) = qry.ToList()
                    tmpList = tmpList.FindAll(Function(item) (Me.GetEmployees().Contains(item.IDEmployee)) OrElse
                    (Me.GetGroups.Contains(item.IDGroup) AndAlso Me.GetEmployees.Contains(item.IDEmployee)))

                    qry = tmpList.AsQueryable

                End If

                Session("AbsencesStatusFilter_LastEmployeesUsed") = strEmployeeFilters

            End If
            '============ FIN EMPLOYEES

            '============ JUSTITICACIONES
            If LastCausesUsed <> hdnCausesSelected.Value OrElse bolReload Then
                If hdnCausesSelected.Value <> String.Empty Then
                    Dim arrCauses As String() = hdnCausesSelected.Value.Split(",")
                    If arrCauses.Length > 0 Then
                        qry = qry.Where(Function(a) (arrCauses.Contains(a.IDCause)))
                    End If
                End If
                Session("AbsencesStatusFilter_LastCausesUsed") = hdnCausesSelected.Value
            End If
            '============ FIN JUSTITICACIONES

            '============ ESTADO
            Select Case roTypes.Any2Integer(cmbStatus.SelectedItem.Value)
                Case 0 'AtHome
                    qry = qry.Where(Function(a) (a.Status = "1" AndAlso a.AbsenceType = "ProgrammedAbsence"))

                Case 1 'ComeThisWeek
                    DateInf = FindWeekMonday(DateTime.Now)
                    DateInf = DateInf.AddHours(DateInf.Hour * -1)
                    DateInf = DateInf.AddMinutes(DateInf.Minute * -1)
                    DateInf = DateInf.AddSeconds(DateInf.Second * -1)

                    DateSup = DateInf.AddDays(6).AddHours(24).AddSeconds(-1)

                    txtDateInf.Value = DateInf
                    txtDateSup.Value = DateSup

                    qry = qry.Where(Function(a) (a.FinishDate >= DateInf AndAlso a.FinishDate.Value.AddDays(1) <= DateSup AndAlso a.AbsenceType = "ProgrammedAbsence"))

                Case 2 'BetweenEndDate
                    qry = qry.Where(Function(a) (a.FinishDate >= DateInf AndAlso a.FinishDate <= DateSup))

                Case 3 'BetweenStartDate
                    qry = qry.Where(Function(a) (a.BeginDate >= DateInf AndAlso a.BeginDate <= DateSup))

            End Select
            '============ FIN ESTADO

            Session("AbsencesStatusFilter_AbsencesDataTable") = qry
            Session("AbsencesStatusFilter_LastDateInfUsed") = DateInf
            Session("AbsencesStatusFilter_LastDateSupUsed") = DateSup

        End If

        Return qry

    End Function

    Private Function CheckPermissionOfEmployeeCube(ByVal a As sysrovwAbsences) As Boolean
        Return Not Me.HasFeaturePermissionByEmployeeOnDate("Calendar", Permission.Read, a.IDEmployee, a.BeginDate)
    End Function

    Private Function CheckPermissionOfEmployeesLstEx(ByRef strEmployees As Generic.List(Of String)) As Generic.List(Of String)
        Return Me.HasFeaturePermissionByEmployeeOnDateEx("Calendar", Permission.Read, strEmployees)
    End Function

    Private Function CheckPermissionOfgroupsLst(ByRef strGroups As Generic.List(Of String)) As Generic.List(Of String)
        Return Me.HasFeaturePermissionByGroupEx("Calendar", Permission.Read, strGroups)
    End Function

    Private Function BuildEmployeeFilter(ByVal qryParam As IQueryable(Of sysrovwAbsences)) As IQueryable(Of sysrovwAbsences)

        Dim qryFilter As IQueryable(Of sysrovwAbsences) = qryParam
        Dim lstEmployeesFilter As New Generic.List(Of Integer)

        If Mid(hdnFilter.Value, 1, 1) = "0" Then  'Empleado actual
            qryFilter = qryFilter.Where(Function(item) item.CurrentEmployee <> 1 Or item.EndDateMobility < roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 2, 1) = "0" Then  'Empleado en movimiento
            qryFilter = qryFilter.Where(Function(item) item.EndDateMobility >= roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 3, 1) = "0" Then  'Empleado de baja
            qryFilter = qryFilter.Where(Function(item) item.CurrentEmployee <> 0 Or item.BeginDateMobility > roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If
        If Mid(hdnFilter.Value, 4, 1) = "0" Then  'Empleado futur
            qryFilter = qryFilter.Where(Function(item) item.BeginDate <= roTypes.Any2Time("1/1/2079").ValueDateTime)
        End If

        If Mid(hdnFilter.Value, 5, 1) = "1" AndAlso hdnFilterUser.Value <> String.Empty Then
            Dim sFilterUserSQL As String = HelperWeb.ParseFilterUserFields(hdnFilterUser.Value)
            If sFilterUserSQL <> String.Empty Then
                Dim sLista As String = API.CommonServiceMethods.GetEmployeeListFromFilter(Me, "@SELECT# ID FROM EMPLOYEES WHERE " & sFilterUserSQL)
                For Each item In sLista.Split(",")
                    lstEmployeesFilter.Add(item)
                Next
                If lstEmployeesFilter.Count > 0 Then
                    qryFilter = From AbsencesData In qryFilter Select AbsencesData Where lstEmployeesFilter.Contains(AbsencesData.IDEmployee)
                End If
            End If
        End If

        Return qryFilter

    End Function

    Private Function BuildPredicate(ByVal lstEmployees As Generic.List(Of Integer),
                                    ByVal lstGroups As Generic.List(Of Integer),
                                    ByVal lstGroupsWithPerm As Generic.List(Of Integer),
                                    ByVal lstEmpWithPerm As Generic.List(Of Integer)) As Expressions.Expression(Of System.Func(Of sysrovwAbsences, Boolean))

        'outer 1 and de ID groups
        Dim predicateIDGroup As Expressions.Expression(Of System.Func(Of sysrovwAbsences, Boolean)) = Nothing
        If lstGroups.Count > 0 Then
            predicateIDGroup = PredicateBuilder.True(Of sysrovwAbsences)()
            predicateIDGroup = predicateIDGroup.And((Function(p1 As sysrovwAbsences) lstGroups.Contains(p1.IDGroup) AndAlso lstGroupsWithPerm.Contains(p1.IDGroup) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        Else
            predicateIDGroup = PredicateBuilder.True(Of sysrovwAbsences)()
            predicateIDGroup = predicateIDGroup.And((Function(p1 As sysrovwAbsences) lstGroupsWithPerm.Contains(p1.IDGroup) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        End If

        Dim predicateIDEmployee As Expressions.Expression(Of System.Func(Of sysrovwAbsences, Boolean)) = Nothing
        If lstGroups.Count > 0 Then
            predicateIDEmployee = PredicateBuilder.True(Of sysrovwAbsences)()
            predicateIDEmployee = predicateIDEmployee.And((Function(p1 As sysrovwAbsences) lstEmployees.Contains(p1.IDEmployee) AndAlso lstEmpWithPerm.Contains(p1.IDEmployee)))
        Else
            predicateIDEmployee = PredicateBuilder.True(Of sysrovwAbsences)()
            predicateIDEmployee = predicateIDEmployee.And((Function(p1 As sysrovwAbsences) lstEmpWithPerm.Contains(p1.IDEmployee)))
        End If

        If predicateIDGroup IsNot Nothing OrElse predicateIDEmployee IsNot Nothing Then
            'outer ppal and de employees
            Dim predicate = PredicateBuilder.False(Of sysrovwAbsences)()

            If predicateIDGroup IsNot Nothing Then predicate = predicate.Or(predicateIDGroup)

            If predicateIDEmployee IsNot Nothing Then predicate = predicate.Or(predicateIDEmployee)

            Return predicate
        Else
            Return PredicateBuilder.False(Of sysrovwAbsences)()
        End If

    End Function

#End Region

#Region "Grid Absencias"

    Private Sub CreateColumns()

        Dim GridColumn As GridViewEditDataColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumncheck As GridViewDataCheckColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.GridAbsencesStatus.Columns.Clear()
        Me.GridAbsencesStatus.KeyFieldName = "ID"

        Me.GridAbsencesStatus.Settings.ShowFilterRow = True
        Me.GridAbsencesStatus.Settings.ShowFooter = True
        Me.GridAbsencesStatus.Settings.UseFixedTableLayout = True

        Me.GridAbsencesStatus.SettingsBehavior.AllowSelectSingleRowOnly = True
        Me.GridAbsencesStatus.SettingsBehavior.AllowFocusedRow = True

        Me.GridAbsencesStatus.SettingsPager.PageSize = 18
        Me.GridAbsencesStatus.SettingsPager.ShowEmptyDataRows = True

        'ID (Clave)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "IDEmployee"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "IDRelatedObject"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = False
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'FullGroupName
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.FullGroupName", DefaultScope) 'Nombre
        GridColumn.FieldName = "FullGroupName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumn.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumn.SortIndex = 1
        GridColumn.Width = 25
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'Empleado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.EmployeeName", DefaultScope) 'Nombre
        GridColumn.FieldName = "EmployeeName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumn.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Width = 25
        GridColumn.SortIndex = 2
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'Tipo
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.Type", DefaultScope) 'Fecha"
        GridColumn.FieldName = "AbsenceType"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center

        GridColumn.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumn.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.BeginsWith
        GridColumn.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Settings.ShowInFilterControl = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False

        GridColumn.Width = 20
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'Justificación
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.CauseName", DefaultScope) 'Fecha"
        GridColumn.FieldName = "CauseName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumn.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Width = 25
        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'Estado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.EmployeeStatus", DefaultScope) 'Descripcion"
        GridColumn.FieldName = "Status"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 15

        GridColumn.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumn.Settings.SortMode = DevExpress.XtraGrid.ColumnSortMode.DisplayText
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.BeginsWith
        GridColumn.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Settings.ShowInFilterControl = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False

        Me.GridAbsencesStatus.Columns.Add(GridColumn)

        'DateView
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridViews.Column.StartDate", DefaultScope) 'Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumnDate.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumnDate.Width = 15
        GridColumnDate.SortIndex = 3
        Me.GridAbsencesStatus.Columns.Add(GridColumnDate)

        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridViews.Column.EndDate", DefaultScope) 'Fecha"
        GridColumnDate.FieldName = "FinishDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumnDate.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumnDate.Width = 15
        Me.GridAbsencesStatus.Columns.Add(GridColumnDate)

        GridColumncheck = New GridViewDataCheckColumn
        GridColumncheck.Caption = Me.Language.Translate("GridViews.Column.DocFailed", DefaultScope) 'Fecha"
        GridColumncheck.FieldName = "DocumentsDelivered"
        GridColumncheck.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumncheck.ReadOnly = True
        GridColumncheck.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumncheck.Settings.AllowHeaderFilter = DevExpress.Utils.DefaultBoolean.True
        GridColumncheck.Settings.ShowFilterRowMenu = DevExpress.Utils.DefaultBoolean.False
        GridColumncheck.Width = 15
        Me.GridAbsencesStatus.Columns.Add(GridColumncheck)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

        Dim showButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        showButton.ID = "ShowDetailButton"
        showButton.Image.Url = "~/Base/Images/Grid/edit.png"
        showButton.Text = Me.Language.Translate("AbsenceTypes.ShowDetails", DefaultScope) 'Mostrar detalles"

        Dim employeeButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        employeeButton.ID = "ShowEmployeeButton"
        employeeButton.Image.Url = "~/Base/Images/EmployeeSelector/Empleado-16x16.gif"
        employeeButton.Text = Me.Language.Translate("AbsenceTypes.ShowEmployee", DefaultScope) 'Ficha empleado"

        GridColumnCommand.CustomButtons.Add(employeeButton)
        GridColumnCommand.CustomButtons.Add(showButton)
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 5
        VisibleIndex = VisibleIndex + 1

        Me.GridAbsencesStatus.Columns.Add(GridColumnCommand)
    End Sub

    Protected Sub GridAbsencesStatus_CustomColumnDisplayText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs) Handles GridAbsencesStatus.CustomColumnDisplayText

        If e.Column.FieldName = "Status" Then
            Dim statusField As Integer = roTypes.Any2Integer(e.Value)
            Select Case statusField
                Case -2
                    e.DisplayText = Me.Language.Translate("AbsenceStatus.RequestInProcess", DefaultScope)
                Case -1
                    e.DisplayText = Me.Language.Translate("AbsenceStatus.Requested", DefaultScope)
                Case 0
                    e.DisplayText = Me.Language.Translate("AbsenceStatus.Approved", DefaultScope)
                Case 1
                    e.DisplayText = Me.Language.Translate("AbsenceStatus.InProcess", DefaultScope)
                Case 2
                    e.DisplayText = Me.Language.Translate("AbsenceStatus.Past", DefaultScope)
            End Select
        ElseIf e.Column.FieldName = "AbsenceType" Then
            e.DisplayText = Me.Language.Translate("AbsenceType." & e.Value, DefaultScope)
        End If
    End Sub

    Protected Sub GridAbsencesStatus_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridAbsencesStatus.CustomCallback
        If e.Parameters = "REFRESHGRID" Then
            GetAbsencesStatus(True)
            GridAbsencesStatus.FilterExpression = String.Empty
            GridAbsencesStatus.DataBind()
        End If
    End Sub

    Protected Sub GridAbsencesStatus_HeaderFilterFillItems(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewHeaderFilterEventArgs) Handles GridAbsencesStatus.HeaderFilterFillItems
        If e.Column.FieldName = "Status" Then
            e.Values.Clear()
            e.AddShowAll()
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceStatus.RequestInProcess", DefaultScope), "-2"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceStatus.Requested", DefaultScope), "-1"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceStatus.Approved", DefaultScope), "0"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceStatus.InProcess", DefaultScope), "1"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceStatus.Past", DefaultScope), "2"))

        ElseIf e.Column.FieldName = "AbsenceType" Then
            e.Values.Clear()
            e.AddShowAll()
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceType.RequestAbsence", DefaultScope), "RequestAbsence"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceType.RequestCause", DefaultScope), "RequestCause"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceType.ProgrammedAbsence", DefaultScope), "ProgrammedAbsence"))
            e.Values.Add(New FilterValue(Me.Language.Translate("AbsenceType.ProgrammedCause", DefaultScope), "ProgrammedCause"))

        End If
    End Sub

#End Region

#Region "SELECTOR DE EMPLEADOS Y INCIDENCIAS"

    Private Function GetInfoSelected() As String
        Dim strRet As String = "0; "

        Try
            If Not String.IsNullOrEmpty(Me.hdnEmployees.Value) Then

                Dim DateInf As DateTime = txtDateInf.Value
                Dim DateSup As DateTime = txtDateSup.Value

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                Dim Selection() As String = Me.hdnEmployees.Value.Trim.Split(",")
                If Selection.Length = 1 Then

                    If Selection(0).Substring(0, 1) = "A" Then 'Grupo
                        'obtener el nombre del grupo
                        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, Selection(0).Substring(1), False)
                        If Not oGroup Is Nothing Then
                            'Dim NumEmployees As Integer = GetNumOfEmployees()
                            Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Employees", DateInf, DateSup)
                            strRet = NumEmployees & ";" & oGroup.Name
                        End If

                    ElseIf Selection(0).Substring(0, 1) = "B" Then 'Empleado
                        'obtener el nombre del empleado
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Selection(0).Substring(1), False)
                        If Not oEmployee Is Nothing Then
                            strRet = "1;" & oEmployee.Name
                        End If
                    End If
                Else
                    'Dim NumEmployees As Integer = GetNumOfEmployees()
                    Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Employees", DateInf, DateSup)
                    strRet = NumEmployees & ";" & NumEmployees.ToString & " " & Me.Language.Translate("GridAbsences.EmployeesSelected", DefaultScope)
                End If
            End If
        Catch
            strRet = "0; "
        End Try

        Return strRet

    End Function

#End Region

    Protected Sub btnExportToXls_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExportToXls.Click
        Me.ASPxGridViewExporter1.GridViewID = Me.GridAbsencesStatus.ID
        Me.ASPxGridViewExporter1.WriteXlsxToResponse(True)
    End Sub

    Private Function FindWeekMonday(ByVal oDate As Date) As Date
        Dim rest As Integer = 0
        If oDate.DayOfWeek = 0 Then
            rest = 6
        Else
            rest = oDate.DayOfWeek - 1
        End If
        Return oDate.AddDays(rest * -1)
    End Function

    Protected Sub ASPxGridViewExporter1_RenderBrick(sender As Object, e As ASPxGridViewExportRenderingEventArgs) Handles ASPxGridViewExporter1.RenderBrick
        Dim dataColumn As DevExpress.Web.GridViewDataColumn = TryCast(e.Column, DevExpress.Web.GridViewDataColumn)
        If e.RowType = DevExpress.Web.GridViewRowType.Data AndAlso dataColumn IsNot Nothing Then
            Select Case dataColumn.FieldName
                Case "AbsenceType"
                    Select Case e.TextValue
                        Case "ProgrammedCause"
                            e.TextValue = Me.Language.Translate("AbsenceType.ProgrammedCause", DefaultScope)

                        Case "ProgrammedAbsence"
                            e.TextValue = Me.Language.Translate("AbsenceType.ProgrammedAbsence", DefaultScope)

                        Case "RequestAbsence"
                            e.TextValue = Me.Language.Translate("AbsenceType.RequestAbsence", DefaultScope)

                        Case "RequestCause"
                            e.TextValue = Me.Language.Translate("AbsenceType.RequestCause", DefaultScope)
                    End Select
            End Select
        End If
    End Sub

End Class