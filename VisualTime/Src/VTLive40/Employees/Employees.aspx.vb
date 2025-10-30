Imports System.Globalization
Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Employees
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class EmployeeCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="oType")>
        Public Type As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class EmployeeSummaryData

        <Runtime.Serialization.DataMember(Name:="taskNames")>
        Public TasksNames As String()

        <Runtime.Serialization.DataMember(Name:="taskValues")>
        Public TasksValues As Double()

        <Runtime.Serialization.DataMember(Name:="taskSummaryName")>
        Public TasksSummaryName As String

        <Runtime.Serialization.DataMember(Name:="centersNames")>
        Public CentersNames As String()

        <Runtime.Serialization.DataMember(Name:="centersValues")>
        Public CentersValues As Double()

        <Runtime.Serialization.DataMember(Name:="centersSummaryName")>
        Public CentersSummaryName As String

        <Runtime.Serialization.DataMember(Name:="workingNames")>
        Public WorkingNames As String()

        <Runtime.Serialization.DataMember(Name:="workingValues")>
        Public WorkingValues As Double()

        <Runtime.Serialization.DataMember(Name:="workingSummaryName")>
        Public WorkingSummaryName As String

        <Runtime.Serialization.DataMember(Name:="absenceNames")>
        Public AbsenceNames As String()

        <Runtime.Serialization.DataMember(Name:="absenceValues")>
        Public AbsenceValues As Double()

        <Runtime.Serialization.DataMember(Name:="absenceSummaryName")>
        Public AbsenceSummaryName As String

        <Runtime.Serialization.DataMember(Name:="accrual")>
        Public AccrualValue As String

        <Runtime.Serialization.DataMember(Name:="punchLocation")>
        Public PunchLocation As String

    End Class

#Region "Properties"

    Private Property ContractsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("EmployeeContracts_EmployeeContractsData")

            If bolReload OrElse tb Is Nothing Then
                tb = API.ContractsServiceMethods.GetContractsByIDEmployee(Me, Me.IdCurrentEmployee, True)

                If tb IsNot Nothing Then
                    tb.Columns.Add(New DataColumn("ID", GetType(Integer)))
                    tb.Columns.Add(New DataColumn("EndDateVisible", GetType(Date)))
                    tb.Columns.Add(New DataColumn("ImagePath", GetType(String)))
                    tb.Columns.Add(New DataColumn("InitialIDContract", GetType(String)))
                End If

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    Dim oRow As DataRow = tb.NewRow
                    oRow("ID") = 1
                    oRow("BeginDate") = New Date(2079, 1, 1)
                    oRow("EndDate") = New Date(2079, 1, 1)
                    oRow("EndDateVisible") = New Date(2079, 1, 1)
                    oRow("IDCard") = -999999
                    oRow("IDLabAgree") = -1
                    oRow("InitialIDContract") = ""
                    oRow("IDEmployee") = Me.IdCurrentEmployee
                    tb.Rows.Add(oRow)
                Else
                    Dim index As Integer = 1
                    For Each oRow As DataRow In tb.Rows
                        oRow("ID") = index
                        oRow("InitialIDContract") = oRow("IDContract")
                        If oRow("EndDate") = New Date(2079, 1, 1) Then
                            oRow("EndDateVisible") = DBNull.Value
                        Else
                            oRow("EndDateVisible") = oRow("EndDate")
                        End If

                        If CDate(oRow("BeginDate")).Date <= Now.Date AndAlso CDate(oRow("EndDate")).Date >= Now.Date Then
                            oRow("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                        Else
                            oRow("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                        End If

                        index = index + 1
                    Next
                End If

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("EmployeeContracts_EmployeeContractsData") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("EmployeeContracts_EmployeeContractsData") = value
        End Set
    End Property

    Private Property LabAgreedData() As DataView
        Get

            Dim tbCauses As DataTable = Session("EmployeeContracts_LabAgreedData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If dv Is Nothing Then

                Dim tb As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)

                If tb IsNot Nothing Then

                    Dim oNewRow As DataRow = tb.NewRow()
                    oNewRow("Name") = ""
                    oNewRow("ID") = -1
                    tb.Rows.Add(oNewRow)

                    dv = New DataView(tb)
                    dv.Sort = "Name ASC"

                    Session("EmployeeContracts_LabAgreedData") = dv.Table

                End If

            End If

            Return dv

        End Get
        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("EmployeeContracts_LabAgreedData") = value.Table
            Else
                Session("EmployeeContracts_LabAgreedData") = Nothing
            End If
        End Set
    End Property

#End Region

#Region "Variables Globals"

    Private Const EmployeeFeatureAlias As String = "Employees"
    Private Const EmployeeFeatureAssignments As String = "Employees.Assignments"
    Private Const CalendarFeatureAlias As String = "Employees"
    Private Const FeatureGroupAlias As String = "Employees.Groups"
    Private Const FeatureMobilityAlias As String = "Employees.GroupMobility"
    Private Const FeatureKpiAlias As String = "KPI.Definition"
    Private Const FeatureCostCenterAlias As String = "Employees.BusinessCenters"
    Private Const FeatureCalendarPlanification As String = "Calendar.Scheduler"
    Private Const FeatureCauses As String = "Calendar.Accruals"
    Private Const FeatureConcepts As String = "Calendar.Accruals"
    Private Const FeatureTasksAnalytics As String = "Tasks.Analytics"
    Private Const FeatureBusinessCentersAnalytics As String = "BusinessCenters.Analytics"
    Private Const FeatureCalendar As String = "Calendar"
    Private Const FeatureContracts As String = "Employees.Contract"

    Private lblIdentifyMethodsInfo As String = ""
    Private lblContractsDescription As String = ""
    Private lblContractsInfo As String = ""

    Private oCurrentContractPermission As Permission       ' Permiso sobre el modulo contratos
    Private oContractPermission As Permission       ' Permiso sobre el modulo contratos
    Private oPermission As Permission               ' Permiso configurado sobre la funcionalidad 'Employees'
    Private oCalendarPermission As Permission               ' Permiso configurado sobre la funcionalidad 'Calendario'
    Private oCurrentPermission As Permission        ' Permiso configurado sobre el empleado actual
    Private oCurrentEmployeeAssignmentsPermission As Permission        ' Permiso configurado sobre el empleado actual
    Private oUserFieldsPermission As Permission     ' Permiso configurado sobre la información de la ficha del empleado actual ('Employees.UserFields.Information')
    Private oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')

    Private oPermissionMobility As Permission
    Private oPermissionGroup As Permission
    Private oPermissionKpi As Permission
    Private oPermissionCostCenter As Permission
    Private oPermissionCalendar As Permission

    Private oPermissionAccrual As Permission
    Private oPermissionCauses As Permission
    Private oPermissionTasks As Permission
    Private oPermissionBusinessCenters As Permission
    Private oPermissionPlanification As Permission

    'Private oAdvancedAccessMode As Boolean = False

    Private isLabAgree As Boolean = False
    Private isDocuments As Boolean = False

    Private bolMultiCompanyLicense As Boolean = False
    Private bolKPIsLicense As Boolean = False
    Private bolCostCentersLicense As Boolean = False

#End Region

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        oCalendarPermission = GetFeaturePermission(CalendarFeatureAlias)

        If Me.IdCurrentEmployee > 0 Then
            oCurrentEmployeeAssignmentsPermission = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAssignments, Me.IdCurrentEmployee)
        Else
            oCurrentEmployeeAssignmentsPermission = Permission.None
        End If

        InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        InsertExtraJavascript("EmployeeGroup", "~/Employees/Scripts/EmployeeGroups.js")
        InsertExtraJavascript("Employees", "~/Employees/Scripts/Employees_v2.js")
        InsertExtraJavascript("GroupsLite", "~/Employees/Scripts/GroupsLite.js")
        InsertExtraJavascript("RoboticsGMaps", "~/Base/Scripts/RoboticsGMaps.js")
        InsertExtraJavascript("Summary", "~/Employees/Scripts/EmployeeSummary.js")
        InsertExtraJavascript("QueryRules", "~/Employees/Scripts/QueryScheduleRules.js")
        InsertExtraJavascript("EmployeeContracts", "~/Employees/Scripts/EmployeeContracts.js")
        InsertExtraJavascript("ContractScheduleRules", "~/Employees/Scripts/ContractScheduleRules.js")
        InsertExtraJavascript("Gmaps", $"https://maps.googleapis.com/maps/api/js?key={MapsKey}&callback=Function.prototype&language={HelperWeb.GetCookie("VTLive_Language")}&loading=async",,, False)

        If Not Me.IsPostBack Then
            HelperSession.DeleteEmployeeGroupsFromApplication()
        End If
        BindGridAssignments(False)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Employees") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTrees1.TreeCaption = Me.Language.Translate("TreeCaptionEmployees", Me.DefaultScope)
        Me.EmployeeURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Employees"

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oContractPermission = Me.GetFeaturePermission(FeatureContracts)
        Me.oCurrentContractPermission = Me.GetFeaturePermissionByEmployee(FeatureContracts, Me.IdCurrentEmployee)

        If Not Me.HasFeaturePermission("Employees", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If
        Dim a As String = Nothing

        If Me.oContractPermission > Permission.None Then
            Me.isLabAgree = isLabAgreeVisible()

            If Not Me.IsPostBack Then
                Me.ContractsData = Nothing
                Me.LabAgreedData = Nothing
            End If

            CreateColumnsContracts()
            BindGridContracts(False)
        End If

        If Not Me.IsPostBack Then

            Dim oTreeStateGrid = HelperWeb.roSelector_GetTreeState("ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpDocs")

            Dim selEmployees As String = oTreeStateGrid.Selected1
            Dim selEmployeeFilters As String = oTreeStateGrid.Filter
            Dim selEmployeeUserFields As String = oTreeStateGrid.UserFieldFilter

            If Not hdnEmployeeDocumentsConfig.Contains("EmpSelected") Then hdnEmployeeDocumentsConfig.Add("EmpSelected", selEmployees) Else hdnEmployeeDocumentsConfig("EmpSelected") = selEmployees
            If Not hdnEmployeeDocumentsConfig.Contains("EmpFilters") Then hdnEmployeeDocumentsConfig.Add("EmpFilters", selEmployeeFilters) Else hdnEmployeeDocumentsConfig("EmpFilters") = selEmployeeFilters
            If Not hdnEmployeeDocumentsConfig.Contains("EmpUserFields") Then hdnEmployeeDocumentsConfig.Add("EmpUserFields", selEmployeeUserFields) Else hdnEmployeeDocumentsConfig("EmpUserFields") = selEmployeeUserFields

            Me.LoadHolidayShiftsCombo(True)
        End If

        If Not Me.IsPostBack AndAlso Not Me.IsCallback Then
            If Request.QueryString.Count Then
                ProcessQueryString()
            End If
            CreateColumnsSuitability(True)
        Else
            CreateColumnsSuitability(False)
        End If


    End Sub

    Private Sub LoadLanguageCombo()
        Dim oLanguages As roPassportLanguage() = API.UserAdminServiceMethods.GetLanguages(Me)
        Dim bolInstalled As Boolean = False
        With Me.cmbLanguage
            .Items.Clear()
            For Each oLanguage As roPassportLanguage In oLanguages
                bolInstalled = IIf(oLanguage.Key = "ESP", True, False)
                If oLanguage.Installed Or bolInstalled Then
                    .Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Language." & oLanguage.Key, Me.DefaultScope), oLanguage.ID))
                End If
            Next
        End With
    End Sub

    Private Sub LoadHolidayShiftsCombo(bOnlyControlledByContractAnnualizedConcept As Boolean)
        divHolidaysSummary.Visible = False
        Me.lblHolidayShift.Text = Me.Language.Translate("HolidaysDetail.Select.HolidaysConcept", DefaultScope)
        cmbHolidayShifts.ValueField = "ID"
        cmbHolidayShifts.TextField = "Name"
        Me.cmbHolidayShifts.DataSource = ShiftServiceMethods.GetHolidayShifts(Me, ,, False, bOnlyControlledByContractAnnualizedConcept)
        Me.cmbHolidayShifts.DataBind()
        If cmbHolidayShifts.Items.Any() Then
            divHolidaysSummary.Visible = True
        End If
    End Sub

    Private Sub LoadHolidaysInfo(ByVal selectedShiftID As Integer)

        Me.GridHolidaysDetail.Visible = True
        Me.GridHolidaysSummary.Visible = True

        Me.GridHolidaysSummary.Columns.Clear()

        Me.GridHolidaysSummary.KeyFieldName = "Type"
        Me.GridHolidaysSummary.SettingsText.EmptyDataRow = " "
        Me.GridHolidaysSummary.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridHolidaysSummary.SettingsBehavior.AllowSort = False

        Dim GridColumn As GridViewDataColumn
        Dim VisibleIndex As Integer = 0

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.Available", DefaultScope)
        GridColumn.FieldName = "Available"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 130
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.ApprovalPending", DefaultScope)
        GridColumn.FieldName = "ApprovalPending"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 130
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.ApprovedNotEnjoyed", DefaultScope)
        GridColumn.FieldName = "ApprovedNotEnjoyed"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 130
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.WillExpires", DefaultScope)
        GridColumn.FieldName = "WillExpires"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 180
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.CanNotEnjoyYet", DefaultScope)
        GridColumn.FieldName = "CanNotEnjoyYet"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 180
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysSummary.Column.ExpectedAccrual", DefaultScope)
        GridColumn.FieldName = "ExpectedAccrual"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 140
        Me.GridHolidaysSummary.Columns.Add(GridColumn)

        Me.GridHolidaysSummary.DataSource = ConceptsServiceMethods.GetHolidaysConceptsSummaryByEmployee(Me, Me.IdCurrentEmployee, selectedShiftID)
        Me.GridHolidaysSummary.DataBind()

        Me.GridHolidaysDetail.Columns.Clear()

        Me.GridHolidaysDetail.KeyFieldName = "Date"
        Me.GridHolidaysDetail.SettingsText.EmptyDataRow = " "
        Me.GridHolidaysDetail.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.GridHolidaysDetail.SettingsBehavior.AllowSort = False


        VisibleIndex = 0
        'Fecha
        GridColumn = New GridViewDataDateColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.Date", DefaultScope)
        GridColumn.FieldName = "Date"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 80
        GridColumn.HeaderStyle.Width = 80
        GridColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Descending
        GridColumn.SortIndex = 0
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Tramo
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.Section", DefaultScope)
        GridColumn.FieldName = "Section"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 160
        GridColumn.HeaderStyle.Width = 160
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Antigüedad
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.YearsAtCompany", DefaultScope)
        GridColumn.FieldName = "YearsAtCompany"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 80
        GridColumn.HeaderStyle.Width = 80
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Días ganados
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.EarnedHolidays", DefaultScope)
        GridColumn.FieldName = "EarnedHolidays"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 95
        GridColumn.HeaderStyle.Width = 95
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Disponibilidad
        GridColumn = New GridViewDataDateColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.HolidaysAvailabilityDate", DefaultScope)
        GridColumn.FieldName = "HolidaysAvailabilityDate"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 95
        GridColumn.HeaderStyle.Width = 95
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Caducidad
        GridColumn = New GridViewDataDateColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.HolidaysExpirationDate", DefaultScope)
        GridColumn.FieldName = "HolidaysExpirationDate"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 80
        GridColumn.HeaderStyle.Width = 80
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Días disfrutados
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.AlreadyTakenHollidays", DefaultScope)
        GridColumn.FieldName = "AlreadyTakenHollidays"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 110
        GridColumn.HeaderStyle.Width = 110
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Días caducados
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.ExpiredHolidays", DefaultScope)
        GridColumn.FieldName = "ExpiredHolidays"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 110
        GridColumn.HeaderStyle.Width = 110
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        'Saldo
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Language.Translate("HolidaysDetail.Column.AccrualValue", DefaultScope)
        GridColumn.FieldName = "AccrualValue"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Width = 80
        GridColumn.HeaderStyle.Width = 80
        Me.GridHolidaysDetail.Columns.Add(GridColumn)

        Me.GridHolidaysDetail.DataSource = ConceptsServiceMethods.GetHolidaysConceptsDetailByEmployee(Me, Me.IdCurrentEmployee, selectedShiftID)
        Me.GridHolidaysDetail.DataBind()
    End Sub

    Protected Sub ProcessQueryString()

        Dim execJs As String = String.Empty

        If Not Request.QueryString("idemployee") Is Nothing Then
            Dim idemployee As String = Request.QueryString("idemployee")

            Dim treePathE As String = String.Empty
            If Me.HasFeaturePermissionByEmployee("Employees", Permission.Read, idemployee) Then
                treePathE = roTools.GetEmployeeGroupPath(idemployee)
            End If
            HelperWeb.roSelector_SetSelection(idemployee, treePathE, "ctl00_contentMainBody_roTrees1",,, "1")

            execJs = "actualType = 'E';actualEmployee =" & idemployee & "; changeTabs(" & roTypes.Any2Integer(Request.QueryString("TabEmployee")) & ");"

        ElseIf Not Request.QueryString("idgroup") Is Nothing Then
            Dim idgroup As String = Request.QueryString("idgroup")
            Dim treePath As String = String.Empty
            If Me.HasFeaturePermissionByGroup("Employees", Permission.Read, idgroup) Then
                treePath = roTools.GetGroupPath(idgroup)
            End If
            HelperWeb.roSelector_SetSelection(idgroup, treePath, "ctl00_contentMainBody_roTrees1",,, "1")
            execJs = "actualType = 'G';actualGrupo=" & idgroup & " ;changeTabs(" & roTypes.Any2Integer(Request.QueryString("TabEmployee")) & ");"
        End If

        If (Request.QueryString("TabEmployee") IsNot Nothing) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ChangeInitialLoadTab", execJs, True)
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Me.LoadLanguageCombo()

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New EmployeeCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "E"
                ProcessEmployeeRequest(oParameters)
            Case "G"
                ProcessGroupRequest(oParameters)
        End Select
    End Sub

    Private Sub ProcessGroupRequest(ByVal oParameters As EmployeeCallbackRequest)

        Me.oPermission = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias, oParameters.ID)
        Me.oCurrentPermission = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias, oParameters.ID)

        If Me.oPermission > Permission.None And oCurrentPermission > Permission.None Then
            Me.oPermissionGroup = Me.GetFeaturePermission(FeatureGroupAlias)
            Me.oPermissionKpi = Me.GetFeaturePermission(FeatureKpiAlias)
            Me.oPermissionCostCenter = Me.GetFeaturePermission(FeatureCostCenterAlias)

            Me.oUserFieldsPermission = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias & ".UserFields.Information", oParameters.ID)
            Me.oUserFieldsAccessPermission(0) = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias & ".UserFields.Information.Low", oParameters.ID)
            Me.oUserFieldsAccessPermission(1) = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias & ".UserFields.Information.Medium", oParameters.ID)
            Me.oUserFieldsAccessPermission(2) = Me.GetFeaturePermissionByGroup(EmployeeFeatureAlias & ".UserFields.Information.High", oParameters.ID)
            Me.bolMultiCompanyLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultiCompany")
            Me.bolKPIsLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\KPIs")
            Me.bolCostCentersLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

            If oParameters.Action = "GETGROUP" Then
                Select Case oParameters.aTab
                    Case 0
                        LoadGroupDataTab0(oParameters)
                    Case 1
                        LoadGroupDataTab1(oParameters)
                End Select

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETGROUP")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpReloadRO", False)
            End If

            If Me.oPermissionGroup < Permission.Write Then
                Me.DisableControls(Me.companyRow.Controls)
            End If
        End If

    End Sub

    Private Sub ProcessEmployeeRequest(ByVal oParameters As EmployeeCallbackRequest)
        Me.IdCurrentEmployee = oParameters.ID

        Me.oPermission = Me.GetFeaturePermission(EmployeeFeatureAlias)

        If oParameters.ID > 0 Then
            oCurrentPermission = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias, oParameters.ID)
            oCurrentEmployeeAssignmentsPermission = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAssignments, oParameters.ID)
        Else
            oCurrentPermission = Permission.Admin
            oCurrentEmployeeAssignmentsPermission = Permission.None
        End If

        CreateColumnsSuitability(False)

        If Me.oPermission > Permission.None And oCurrentPermission > Permission.None Then

            Me.oUserFieldsPermission = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias & ".UserFields.Information", oParameters.ID)
            Me.oUserFieldsAccessPermission(0) = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias & ".UserFields.Information.Low", oParameters.ID)
            Me.oUserFieldsAccessPermission(1) = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias & ".UserFields.Information.Medium", oParameters.ID)
            Me.oUserFieldsAccessPermission(2) = Me.GetFeaturePermissionByEmployee(EmployeeFeatureAlias & ".UserFields.Information.High", oParameters.ID)
            Me.bolCostCentersLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

            If oParameters.Action = "GETEMPLOYEE" Then
                If (oParameters.ID > 0) Then
                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aSelect, Robotics.VTBase.Audit.ObjectType.tEmployee, API.EmployeeServiceMethods.GetEmployeeName(Me.Page, oParameters.ID), New List(Of String), New List(Of String), Me.Page)
                End If

                Select Case oParameters.aTab
                    Case 0
                        LoadEmployeeDataTab0(oParameters)
                    Case 1
                        LoadEmployeeDataTab1(oParameters, False)
                    Case 2
                        LoadEmployeeDataTab2(oParameters)
                    Case 3
                        LoadEmployeeDataTab3(oParameters)
                    Case 4
                        LoadEmployeeDataTab4(oParameters)
                    Case 5
                        LoadEmployeeDataTab5(oParameters)
                    Case 6
                        LoadEmployeeDataTab6(oParameters)
                    Case 7
                        LoadEmployeeDataTab7(oParameters)
                    Case 8
                        LoadEmployeeDataTab8(oParameters)

                End Select
                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETEMPLOYEE")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ElseIf oParameters.Action = "EDITGRID" Then
                LoadEmployeeDataTab1(oParameters, True)

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "EDITGRID")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ElseIf oParameters.Action = "SAVEEMPLOYEEFIELDS" Then
                Dim strError = saveEmployeeGrid(oParameters)
                If strError <> "" Then
                    LoadEmployeeDocsAndOnboardings(Nothing, oParameters)
                Else
                    LoadEmployeeDataTab1(oParameters, False)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETEMPLOYEE")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                End If
            ElseIf oParameters.Action = "SAVEEMPLOYEEASSIGNMENTS" Then

                If SaveEmployeeAssignmentsGrid(oParameters.ID) Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVEEMPLOYEEASSIGNMENTS")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                Else
                    LoadEmployeeDataTab5(oParameters)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVEEMPLOYEEASSIGNMENTS")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", Me.Language.Translate("errorSaveAssignments", DefaultScope))

                End If

            End If
        End If
    End Sub

    Private Sub LoadGroupDataTab0(ByVal oParameters As EmployeeCallbackRequest)
        Try
            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)
            If oGroup Is Nothing Then Exit Sub

            Dim dtblCurrent As DataTable
            Dim nRowControl As DataRow
            Dim strCols() As String = {Me.Language.Translate("ColumnEmployee", Me.DefaultScope), Me.Language.Translate("ColumnDateBegin", Me.DefaultScope), Me.Language.Translate("ColumnDateEnd", Me.DefaultScope)}
            Dim sizeCols() As String = {"350px", "100px", "100px"}
            Dim cssCols() As String = {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

            'Carrega Grid Usuaris Actuals -------------------------------------
            dtblCurrent = Me.CurrentEmployeesData(oParameters.ID)

            Dim tblControlCurrent As DataTable

            If dtblCurrent.Rows.Count > 0 Then
                'Carrego les columnes que no em fan falta
                dtblCurrent.Columns.Remove("GroupName")
                dtblCurrent.Columns.Remove("Path")
                dtblCurrent.Columns.Remove("SecurityFlags")
                dtblCurrent.Columns.Remove("IDGroup")
                'dtblCurrent.Columns.Remove("EndDate")
                dtblCurrent.Columns.Remove("CurrentEmployee")
                dtblCurrent.Columns.Remove("AttControlled")
                dtblCurrent.Columns.Remove("AccControlled")
                dtblCurrent.Columns.Remove("JobControlled")
                dtblCurrent.Columns.Remove("ExtControlled")
                dtblCurrent.Columns.Remove("RiskControlled")
                dtblCurrent.Columns.Remove("FullGroupName")
                dtblCurrent.Columns.Remove("IsTransfer")

                tblControlCurrent = New DataTable
                tblControlCurrent.Columns.Add("NomCamp") 'Nom del Camp
                tblControlCurrent.Columns.Add("NomParam") 'Parametre que es pasara
                tblControlCurrent.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

                nRowControl = tblControlCurrent.NewRow
                nRowControl("NomCamp") = "IDEmployee"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = False
                tblControlCurrent.Rows.Add(nRowControl)

                Dim htmlHGridCurrent As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
                Me.divHeaderGrupActual.Controls.Add(htmlHGridCurrent)

                Dim htmlTGridCurrent As HtmlTable = creaGridLists(dtblCurrent, strCols, sizeCols, tblControlCurrent, True, 1, True)
                Me.divGridGrupActual.Controls.Add(htmlTGridCurrent)
            Else
                Me.divHeaderGrupActual.InnerHtml = Me.Language.Translate("NoEmployees", Me.DefaultScope) '"No hay empleados actualmente"
            End If

            'Carrega Grid Usuaris Futurs -------------------------------------
            Dim dtblFuture As DataTable
            dtblFuture = Me.FutureEmployeesData(oParameters.ID)

            If dtblFuture.Rows.Count > 0 Then
                'Carrego les columnes que no em fan falta
                dtblFuture.Columns.Remove("Path")
                dtblFuture.Columns.Remove("Type")

                Dim tblControlFuture As DataTable = New DataTable
                tblControlFuture.Columns.Add("NomCamp") 'Nom del Camp
                tblControlFuture.Columns.Add("NomParam") 'Parametre que es pasara
                tblControlFuture.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

                nRowControl = tblControlFuture.NewRow
                nRowControl("NomCamp") = "ID"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = False
                tblControlFuture.Rows.Add(nRowControl)

                Dim htmlHGridFuture As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
                Me.divHeaderGrupTransit.Controls.Add(htmlHGridFuture)

                Dim htmlTGridFuture As HtmlTable = creaGridLists(dtblFuture, strCols, sizeCols, tblControlFuture, True)
                Me.divGridGrupTransit.Controls.Add(htmlTGridFuture)
            Else
                Me.divHeaderGrupTransit.InnerHtml = Me.Language.Translate("NoEmployeesTransit", Me.DefaultScope) '"No hay empleados en tránsito en este grupo"
            End If

            'Carrega Grid Usuaris Antics -------------------------------------
            Dim dtblOld As DataTable
            dtblOld = Me.OldEmployeesData(oParameters.ID)

            If dtblOld.Rows.Count > 0 Then
                'Carrego les columnes que no em fan falta
                dtblOld.Columns.Remove("Type")

                Dim tblControlOld As DataTable = New DataTable
                tblControlOld.Columns.Add("NomCamp") 'Nom del Camp
                tblControlOld.Columns.Add("NomParam") 'Parametre que es pasara
                tblControlOld.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

                nRowControl = tblControlOld.NewRow
                nRowControl("NomCamp") = "ID"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = False
                tblControlOld.Rows.Add(nRowControl)
                Dim strCols2() As String = {Me.Language.Translate("ColumnEmployee", Me.DefaultScope), Me.Language.Translate("ColumnDateBegin", Me.DefaultScope), Me.Language.Translate("ColumnDateEnd", Me.DefaultScope)}
                Dim sizeCols2() As String = {"350px", "100px", "100px"}
                Dim cssCols2() As String = {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}
                Dim htmlHGridOld As HtmlTable = creaHeaderLists(strCols2, sizeCols2, cssCols2)
                Me.divHeaderGrupPasat.Controls.Add(htmlHGridOld)

                Dim htmlTGridOld As HtmlTable = creaGridLists(dtblOld, strCols2, sizeCols2, tblControlOld, True)
                Me.divGridGrupPasat.Controls.Add(htmlTGridOld)
            Else
                divHeaderGrupPasat.InnerHtml = Me.Language.Translate("NoEmployeesPast", Me.DefaultScope) '"No han habido empleados en este grupo"
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        End Try

    End Sub

    Private Sub LoadGroupDataTab1(ByVal oParameters As EmployeeCallbackRequest)
        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)
        If oGroup Is Nothing Then Return

        GroupDocumentManagment.SetScope(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
        GroupDocumentManagmentCompany.SetScope(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
        GroupDocumentPendingManagment.LoadAlerts(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
    End Sub

    Private Sub LoadEmployeeDataTab0(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        ' Si el passport actual tiene permisos de lectura o superiores, mostramos el tipo del empleado
        If Me.HasFeaturePermissionByEmployee("Employees.Type", Permission.Read, oParameters.ID) Then
            'Carrega icones Tipus Usuaris ------------------------------
            divTypeEmp.Controls.Add(LoadTypeEmployee(oEmployee))
            If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") Then
                Me.ckProductiveYes.Enabled = False
                Me.ckProductiveNo.Enabled = False
            Else
                Me.ckProductiveYes.Enabled = True
                Me.ckProductiveNo.Enabled = True
                If oEmployee.Type = "J" Then
                    Me.ckProductiveYes.Checked = True
                    Me.ckProductiveNo.Checked = False
                Else
                    Me.ckProductiveYes.Checked = False
                    Me.ckProductiveNo.Checked = True
                End If
            End If

            Me.txtName.Text = oEmployee.Name

            Dim oEmpPassport As roPassportTicket = WLHelperWeb.CurrentPassport(True)
            If oEmpPassport IsNot Nothing Then
                Me.cmbLanguage.SelectedItem = Me.cmbLanguage.Items.FindByValue(oEmpPassport.Language.ID.ToString)
            End If
        Else
            divPending.Visible = False
        End If

        Me.divGoToGenius.Attributes.Add("onclick", "parent.reenviaFrame('/" & ConfigurationManager.AppSettings("RootUrl") & "//Genius', 'MainMenu_Global_MainMenu_Button1', 'Genius', 'Portal\Reports'); return false;")

        If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
            Me.divGoToGenius.Visible = False
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then
            'Me.frmEmployeeScheduleRules.InitControl(oParameters.ID)
        End If

        If roTypes.Any2String(HelperSession.AdvancedParametersCache("EmployeeSummaryEnabled")).ToUpper = "1" Then
            CreateSummaryData(oParameters.ID)
        Else
            divLastPunch.Style("display") = "none"
            divPresenceSummary.Style("display") = "none"
            divEmployeeSummary.Style("display") = "none"
        End If

        If Me.cmbHolidayShifts.SelectedItem Is Nothing Then
            Me.cmbHolidayShifts.SelectedItem = Me.cmbHolidayShifts.Items.FirstOrDefault()
        End If

        If Me.cmbHolidayShifts.SelectedItem IsNot Nothing AndAlso Me.cmbHolidayShifts.SelectedItem.Value > 0 Then
            Me.LoadHolidaysInfo(Me.cmbHolidayShifts.SelectedItem.Value)
        End If

    End Sub

    Private Function GetDays() As DataTable
        Dim dt As DataTable = New DataTable()
        dt.Clear()
        dt.Columns.Add("ID")
        dt.Columns.Add("Name")
        Dim _mon As DataRow = dt.NewRow()
        _mon("ID") = "0"
        _mon("Name") = Me.Language.Translate("Employee.Telecommuting.Monday", Me.DefaultScope)
        Dim _tue As DataRow = dt.NewRow()
        _tue("ID") = "1"
        _tue("Name") = Me.Language.Translate("Employee.Telecommuting.Tuesday", Me.DefaultScope)
        Dim _wed As DataRow = dt.NewRow()
        _wed("ID") = "2"
        _wed("Name") = Me.Language.Translate("Employee.Telecommuting.Wednesday", Me.DefaultScope)
        Dim _thu As DataRow = dt.NewRow()
        _thu("ID") = "3"
        _thu("Name") = Me.Language.Translate("Employee.Telecommuting.Thursday", Me.DefaultScope)
        Dim _fri As DataRow = dt.NewRow()
        _fri("ID") = "4"
        _fri("Name") = Me.Language.Translate("Employee.Telecommuting.Friday", Me.DefaultScope)
        Dim _sat As DataRow = dt.NewRow()
        _sat("ID") = "5"
        _sat("Name") = Me.Language.Translate("Employee.Telecommuting.Saturday", Me.DefaultScope)
        Dim _sun As DataRow = dt.NewRow()
        _sun("ID") = "6"
        _sun("Name") = Me.Language.Translate("Employee.Telecommuting.Sunday", Me.DefaultScope)

        dt.Rows.Add(_mon)
        dt.Rows.Add(_tue)
        dt.Rows.Add(_wed)
        dt.Rows.Add(_thu)
        dt.Rows.Add(_fri)
        dt.Rows.Add(_sat)
        dt.Rows.Add(_sun)

        Return dt
    End Function

    Private Sub LoadEmployeeDataTab1(ByVal oParameters As EmployeeCallbackRequest, ByVal editMode As Boolean)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        LoadEmployeeUserFields(oParameters, editMode, oEmployee)
        LoadEmployeeDocsAndOnboardings(oEmployee, Nothing)

    End Sub

    Private Sub LoadEmployeeDocsAndOnboardings(oEmployee As roEmployee, ByVal oParameters As EmployeeCallbackRequest)

        If oEmployee Is Nothing AndAlso oParameters Is Nothing Then Return

        If oEmployee Is Nothing Then
            oEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
            If oEmployee Is Nothing Then Return
        End If


        Me.dxColorPicker.Color = Drawing.ColorTranslator.FromWin32(oEmployee.HighlightColor)

        'Carrega Grid Documents
        Dim tsAlerts As DocumentAlerts = Nothing

        tsAlerts = DocumentsServiceMethods.GetDocumentationFaults(Me.Page, DocumentType.Employee, oEmployee.ID, -2, ForecastType.Any)

        If tsAlerts IsNot Nothing AndAlso (tsAlerts.DocumentsValidation.Any() OrElse tsAlerts.AbsenteeismDocuments.Any() _
            OrElse tsAlerts.MandatoryDocuments.Any() OrElse tsAlerts.WorkForecastDocuments.Any() OrElse tsAlerts.AccessAuthorizationDocuments.Any()) Then
            Me.imgAlerts.Style("display") = ""
        Else
            Me.imgAlerts.Style("display") = "none"
        End If

        DocumentManagment.SetScope(oEmployee.ID, DocumentType.Employee, -2, ForecastType.Any)

        DocumentPendingManagment.LoadAlerts(oEmployee.ID, DocumentType.Employee, -2, ForecastType.Any)

        'Carrega Tabla Onboarding

        Dim dsOnBoarding = API.ToDoListServiceMethods.GetToDoListTasksByEmployee(oEmployee.ID, Nothing)

        ASPxCallbackPanelContenido.JSProperties.Add("cpOnBoardingResult", roJSONHelper.SerializeNewtonSoft(dsOnBoarding))

        Dim oServerLicense As New roServerLicense
        If oServerLicense.FeatureIsInstalled("Feature\CegidHub") Then
            Me.divOnboarding.Visible = False
        Else
            Me.divOnboarding.Visible = True
        End If

        If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CustomEmployeeColorEnabled")) Then
            highlightDiv.Style("display") = ""
        End If
    End Sub

    Private Sub LoadEmployeeUserFields(oParameters As EmployeeCallbackRequest, editMode As Boolean, oEmployee As roEmployee)
        If editMode Then
            LoadEmployeeGridEdit(oEmployee)
        Else
            'Carrega Grid Usuaris -------------------------------------
            Me.divEmployeeUserFields.Visible = (Me.oUserFieldsPermission > Permission.None)
            If Me.oUserFieldsPermission > Permission.None Then
                Dim dsUsuari As DataSet
                dsUsuari = API.EmployeeServiceMethods.GetUserFieldsDataset(Me, oParameters.ID)

                Dim visible As Boolean = False
                If dsUsuari IsNot Nothing AndAlso dsUsuari.Tables.Count > 0 AndAlso dsUsuari.Tables(0).Rows.Count > 0 Then
                    visible = True
                    Dim dTbl As DataTable = dsUsuari.Tables(0)
                    'dTbl.Columns.Remove("Type")
                    'dTbl.Columns.Remove("FieldName")
                    Dim Columns As String() = {"FieldCaption", "Value"}
                    Dim htmlTGrid As HtmlTable = creaGrid(oParameters.ID, dTbl, Columns)
                    Me.divGrid.Controls.Add(htmlTGrid)

                    visible = (Me.oUserFieldsPermission >= Permission.Write)

                    'Carrega el anchor de editar el grid
                    If Me.oUserFieldsPermission >= Permission.Write Then
                        Me.editGridEmp.Attributes("onclick") = "editGrid('" & oParameters.ID & "')"
                        Me.saveEditGridEmp.Attributes("onclick") = "saveGrid('" & oParameters.ID & "')"
                        Me.cancelEditGridEmp.Attributes("onclick") = "cancelEditGridE('" & oParameters.ID & "')"
                    End If
                Else
                    visible = False
                End If

                Me.btn1Fields.Visible = visible
                Me.btn2Fields.Visible = False
                Me.btn3Fields.Visible = False
            End If
        End If
    End Sub

    Private Sub LoadEmployeeDataTab2(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        'Recupera les descripcions
        Dim strFullGroupName As String = String.Empty
        StateDescriptions(oEmployee.ID, strFullGroupName)

        Dim lockDateType As Boolean
        Dim lockDate = API.EmployeeServiceMethods.GetEmployeeLockDatetoApply(Me, oEmployee.ID, lockDateType, False)
        Me.txtCurrentLockDate.Text = lockDate.ToShortDateString.ToString
        Me.lblLockDate.Text = Me.Language.Translate("Employee.LockDateType.Global", Me.DefaultScope)
        If lockDateType Then
            Me.lblLockDate.Text = Me.Language.Translate("Employee.LockDateType.Custom", Me.DefaultScope)
        End If

        If Me.oPermission >= Permission.Admin Then
            Me.btnEditLockDate.Attributes.Add("onclick", "ShowLockDate('" & oEmployee.ID & "', '" & lockDate & "', '" & lockDateType & "')")
        Else
            Me.btnEditLockDate.Visible = False
        End If

        Me.divContracts.Visible = Me.oCurrentContractPermission >= Permission.Read
        spanContracts.InnerHtml = lblContractsInfo
        lblForgottenRight.Text = Me.Language.Translate("Employee.ForgottenRight.Custom", Me.DefaultScope)
        btnForgottenRightTrue.Value = oEmployee.HasForgottenRight
        btnForgottenRightFalse.Value = Not oEmployee.HasForgottenRight

        BindGridContracts(True)

        frmContractScheduleRules.LoadEmployeeData(oEmployee.ID)

        Me.divMobility.Visible = Me.HasFeaturePermissionByEmployee("Employees.GroupMobility", Permission.Read, oParameters.ID)
        spanMobility.InnerHtml = strFullGroupName
        aMobility.Attributes("onclick") = "ShowMobility('" & oEmployee.ID & "');"

        If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
            Me.divMessages.Visible = False
        End If

        Me.divCost.Visible = Me.HasFeaturePermissionByEmployee(FeatureCostCenterAlias, Permission.Read, oParameters.ID) AndAlso bolCostCentersLicense
        aCost.Attributes("onclick") = "ShowCenters('" & oEmployee.ID & "');"

        aMessage.Attributes("onclick") = "ShowEmployeeMessages('" & oEmployee.ID & "');"

        Me.divRemoveEmployeeData.Visible = Me.HasFeaturePermissionByEmployee("Employees.Contract", Permission.Write, oParameters.ID) AndAlso Me.HasFeaturePermission("Employees.Contract", Permission.Admin)
        aRemoveData.Attributes("onclick") = "ShowRemoveEmployeeData('" & oEmployee.ID & "');"

    End Sub

    Private Sub LoadEmployeeDataTab3(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        'Recupera les descripcions
        Dim strFullGroupName As String = String.Empty
        StateDescriptions(oEmployee.ID, strFullGroupName)

        spanIdentify.InnerHtml = lblIdentifyMethodsInfo
        'aAccruals.Attributes("onclick") = "ShowPubli();"
        Me.divIdentify.Visible = Me.HasFeaturePermissionByEmployee("Employees.IdentifyMethods", Permission.Read, oParameters.ID)
        aIdentifyMethods.Attributes("onclick") = "ShowIdentifyMethods('" & oEmployee.ID & "');"

        'spanState.InnerHtml = ""
        'aState.Attributes("onclick") = "ShowStateInfo('" & oEmployee.ID & "');"
        aEmployeePermissions.Attributes("onclick") = "ShowEmployeePermissions('" & oEmployee.ID & "');"

        Me.divApplications.Visible = Me.HasFeaturePermissionByEmployee("Employees.AllowedApplications", Permission.Read, oParameters.ID)
        aApplications.Attributes("onclick") = "ShowApplicationsEmployee('" & oEmployee.ID & "');"

        Me.divAuthorizations.Visible = HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access")
        aAccessAuthorizations.Attributes("onclick") = "ShowAccessAuthorizations(" & oEmployee.ID & ",-1);"
    End Sub

    Private Sub LoadEmployeeDataTab4(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        'Cargar los grid Ausencias Previstas / Programadas -------------------------
        ' Cargo las ausencias programadas
        Dim dTblAusenciasPrev As DataTable
        dTblAusenciasPrev = Me.ProgrammedAbsencesData(oParameters.ID)

        'Elimina les columnes que no fan falta mostrar al Grid de Ausencias
        dTblAusenciasPrev.Columns.Remove("FinishDate")
        dTblAusenciasPrev.Columns.Remove("MaxLastingDays")
        dTblAusenciasPrev.Columns.Remove("Description")
        dTblAusenciasPrev.Columns.Remove("Name")
        dTblAusenciasPrev.Columns.Remove("RealFinishDate")
        dTblAusenciasPrev.Columns.Remove("RelapsedDate")

        Dim htmlTGridAusencias As HtmlTable

        If dTblAusenciasPrev.Rows.Count = 0 Then
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            Dim nRowControl As DataRow = tblControl.NewRow
            nRowControl("NomCamp") = "IDCause"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "IDEmployee"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "BeginDate"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "HasDocuments"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "DocumentsDelivered"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            dTblAusenciasPrev.Columns.Remove("IDCause")
            dTblAusenciasPrev.Columns.Remove("IDEmployee")
            dTblAusenciasPrev.Columns.Remove("BeginDate")
            Dim oRow As DataRow = dTblAusenciasPrev.NewRow()
            oRow("Literal") = Me.Language.Translate("NoAusences", Me.DefaultScope)
            dTblAusenciasPrev.Rows.Add(oRow)
            htmlTGridAusencias = creaGridAusencias(dTblAusenciasPrev, tblControl, False)
        Else
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            Dim nRowControl As DataRow = tblControl.NewRow
            nRowControl("NomCamp") = "IDCause"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "IDEmployee"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "BeginDate"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "HasDocuments"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "DocumentsDelivered"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            htmlTGridAusencias = creaGridAusencias(dTblAusenciasPrev, tblControl, oPermission >= Permission.Write)
        End If
        If (htmlTGridAusencias IsNot Nothing) Then
            Me.gridAusenciasPrevistas.Controls.Add(htmlTGridAusencias)
        End If

        If oPermission < Permission.Write Then
            tblAddAus.Visible = False
        Else
            tblAddAus.Visible = True
            btnAddAbsence.Attributes("onclick") = "AddNewProgrammedAbsence('" & oParameters.ID & "');"
        End If

        'Cargar los grid Incidencias Previstas
        Dim dTblIncidenciasPrev As DataTable
        dTblIncidenciasPrev = Me.ProgrammedIncidencesData(oParameters.ID)

        'Elimina les columnes que no fan falta mostrar al Grid de Ausencias
        dTblIncidenciasPrev.Columns.Remove("FinishDate")
        dTblIncidenciasPrev.Columns.Remove("BeginTime")
        dTblIncidenciasPrev.Columns.Remove("EndTime")
        dTblIncidenciasPrev.Columns.Remove("Description")
        dTblIncidenciasPrev.Columns.Remove("Name")
        dTblIncidenciasPrev.Columns.Remove("Duration")
        dTblIncidenciasPrev.Columns.Remove("MinDuration")

        Dim htmlTGridIncidencias As HtmlTable

        If dTblIncidenciasPrev.Rows.Count = 0 Then
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            Dim nRowControl As DataRow = tblControl.NewRow
            nRowControl("NomCamp") = "IDCause"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "IDEmployee"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Date"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "ID"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            'Win32
            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Win32"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "HasDocuments"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "DocumentsDelivered"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            dTblIncidenciasPrev.Columns.Remove("IDCause")
            dTblIncidenciasPrev.Columns.Remove("IDEmployee")
            dTblIncidenciasPrev.Columns.Remove("Date")
            dTblIncidenciasPrev.Columns.Remove("ID")
            dTblIncidenciasPrev.Columns.Remove("Win32")
            Dim oRow As DataRow = dTblIncidenciasPrev.NewRow()
            oRow("Literal") = Me.Language.Translate("NoIncidences", Me.DefaultScope)
            dTblIncidenciasPrev.Rows.Add(oRow)
            htmlTGridIncidencias = creaGridIncidencias(dTblIncidenciasPrev, tblControl, False)
        Else
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            Dim nRowControl As DataRow = tblControl.NewRow
            nRowControl("NomCamp") = "IDCause"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "IDEmployee"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Date"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "ID"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            'Win32
            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Win32"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "HasDocuments"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "DocumentsDelivered"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            htmlTGridIncidencias = creaGridIncidencias(dTblIncidenciasPrev, tblControl, oPermission >= Permission.Write)
        End If
        Me.gridIncidenciasPrevistas.Controls.Add(htmlTGridIncidencias)

        If oPermission < Permission.Write Then
            tblAddInc.Visible = False
        Else
            tblAddInc.Visible = True
            btnAddIncidence.Attributes("onclick") = "AddNewProgrammedIncidence('" & oParameters.ID & "');"
        End If

        'Cargar los grid Incidencias Previstas
        Dim lstProgrammedHolidays As Generic.List(Of roProgrammedHoliday)
        lstProgrammedHolidays = Me.ProgrammedHolidaysData(oParameters.ID)

        Dim htmlTGridHolidays As HtmlTable

        If lstProgrammedHolidays.Count = 0 Then
            Dim oEmptyObj As New roProgrammedHoliday
            oEmptyObj.Description = Me.Language.Translate("NoHolidays", Me.DefaultScope)
            lstProgrammedHolidays.Add(oEmptyObj)
            htmlTGridHolidays = creaGridHolidays(lstProgrammedHolidays, oParameters.ID, False)
        Else
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

            Dim nRowControl As DataRow = tblControl.NewRow
            nRowControl("NomCamp") = "IDCause"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "IDEmployee"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Date"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "ID"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            'Win32
            nRowControl = tblControl.NewRow
            nRowControl("NomCamp") = "Win32"
            nRowControl("NomParam") = ""
            nRowControl("Visible") = False
            tblControl.Rows.Add(nRowControl)

            htmlTGridHolidays = creaGridHolidays(lstProgrammedHolidays, oParameters.ID, oPermission >= Permission.Write)
        End If
        Me.gridProgrammedHolidays.Controls.Add(htmlTGridHolidays)

        If oPermission < Permission.Write Then
            tblAddHolidays.Visible = False
        Else
            tblAddHolidays.Visible = True
            btnAddHolidays.Attributes("onclick") = "editGridHolidays(-1," & oParameters.ID & ");"
        End If

        'Cargar los grid Incidencias Previstas
        Dim lstProgrammedOvertimes As Generic.List(Of roProgrammedOvertime)
        lstProgrammedOvertimes = Me.ProgrammedOvertimesData(oParameters.ID)

        Dim htmlTGridOvertimes As HtmlTable

        If lstProgrammedOvertimes.Count = 0 Then
            Dim oEmptyObj As New roProgrammedOvertime
            oEmptyObj.Description = Me.Language.Translate("NoOvertime", Me.DefaultScope)
            lstProgrammedOvertimes.Add(oEmptyObj)
            htmlTGridOvertimes = creaGridOvertimes(lstProgrammedOvertimes, oParameters.ID, False)
        Else
            htmlTGridOvertimes = creaGridOvertimes(lstProgrammedOvertimes, oParameters.ID, oPermission >= Permission.Write)
        End If

        Me.gridProgrammedOvertimes.Controls.Add(htmlTGridOvertimes)

        If oPermission < Permission.Write Then
            tblAddOvertimes.Visible = False
        Else
            tblAddOvertimes.Visible = True
            btnAddOvertime.Attributes("onclick") = "editGridOvertime(-1," & oParameters.ID & ");"
        End If

    End Sub

    Private Sub LoadEmployeeDataTab5(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        'If Not Me.IsPostBack Then
        BindGridAssignments(True)
        'End If

        Me.btnAddNewSuitability.Visible = Me.HasFeaturePermissionByEmployee("Employees.Assignments", Permission.Write, oParameters.ID)

    End Sub

#Region "Grid Suitability"

    Private Sub CreateColumnsSuitability(ByVal bolReload As Boolean)

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim VisibleIndex As Integer = 0

        'general grid settings
        Me.gridSuitability.Columns.Clear()
        Me.gridSuitability.KeyFieldName = "ID"
        Me.gridSuitability.SettingsText.EmptyDataRow = " "
        Me.gridSuitability.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        btnAddNewSuitability.Visible = Me.oCurrentEmployeeAssignmentsPermission >= Permission.Write

        If Me.oCurrentEmployeeAssignmentsPermission >= Permission.Write Then
            Me.gridSuitability.SettingsEditing.Mode = GridViewEditingMode.Inline
        End If

        'Edit button
        If Me.oCurrentEmployeeAssignmentsPermission >= Permission.Write Then
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.Name = "Update"
            GridColumnCommand.ShowEditButton = True
            GridColumnCommand.ShowUpdateButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            Me.gridSuitability.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False 'False
        GridColumn.Width = 40
        Me.gridSuitability.Columns.Add(GridColumn)

        'GridColumn = New GridViewDataTextColumn()
        'GridColumn.FieldName = "IDAssignment"
        'GridColumn.Visible = False
        'Me.gridSuitability.Columns.Add(GridColumn)

        'Puesto
        Dim combo = New GridViewDataComboBoxColumn()
        combo.Caption = Me.Language.Translate("gridSuitability.Column.Assignment", DefaultScope)
        combo.Name = "IDAssignment"
        combo.FieldName = "IDAssignment"
        combo.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        combo.PropertiesComboBox.DataSource = AssignmentsData(bolReload)
        combo.PropertiesComboBox.TextField = "Name"
        combo.PropertiesComboBox.ValueField = "ID"
        combo.PropertiesComboBox.ValueType = GetType(String)
        combo.Width = 80
        combo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        combo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.gridSuitability.Columns.Add(combo)

        '% Idoneidad
        'GridColumn = New GridViewDataTextColumn()
        'GridColumn.Caption = Me.Language.Translate("gridSuitability.Column.Suitability", DefaultScope)
        'GridColumn.FieldName = "Suitability"
        'GridColumn.VisibleIndex = VisibleIndex
        'VisibleIndex = VisibleIndex + 1
        'GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<0..100>"
        'GridColumn.PropertiesTextEdit.MaskSettings.IncludeLiterals = MaskIncludeLiteralsMode.DecimalSymbol
        'GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        'GridColumn.Width = 40
        'Me.gridSuitability.Columns.Add(GridColumn)

        'Cancel button
        If Me.oCurrentEmployeeAssignmentsPermission >= Permission.Write Then
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowCancelButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.Name = "Cancel"
            GridColumnCommand.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            Me.gridSuitability.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Private Sub gridSuitability_DataBinding(sender As Object, e As EventArgs) Handles gridSuitability.DataBinding
        If gridSuitability.Columns.Count > 0 Then
            Dim oCombo As GridViewDataComboBoxColumn = gridSuitability.Columns("IDAssignment")
            oCombo.PropertiesComboBox.DataSource = Me.AssignmentsData(False)
            oCombo.PropertiesComboBox.ValueField = "ID"
            oCombo.PropertiesComboBox.ValueType = GetType(Integer)
            oCombo.PropertiesComboBox.TextField = "Name"
        End If

    End Sub

    Private Sub BindGridAssignments(ByVal bolReload As Boolean)
        Me.gridSuitability.DataSource = Me.EmployeeAssignmentData(bolReload)
        If Me.oCurrentEmployeeAssignmentsPermission <> Permission.None Then
            Me.gridSuitability.DataBind()
        End If

    End Sub

    Protected Sub gridSuitability_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles gridSuitability.InitNewRow
        Dim tb As DataTable = Me.EmployeeAssignmentData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If
    End Sub

    Protected Sub GridAssignments_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles gridSuitability.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If e.NewValues IsNot Nothing Then

            If (e.NewValues("IDAssignment") Is Nothing) Then
                Throw New Exception(Me.Language.Translate("Error.NullAssignment", Me.DefaultScope))
            End If

            Dim tb As DataTable = Me.EmployeeAssignmentData()
            For Each row As DataRow In tb.Rows
                If row("IDAssignment") = e.NewValues("IDAssignment") Then
                    Throw New Exception(Me.Language.Translate("Error.DuplicatedAssignment", Me.DefaultScope))
                End If
            Next

            Dim newID As Integer
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
                .Item("Suitability") = 100
                .Item("IDEmployee") = Me.IdCurrentEmployee
            End With

            tb.Rows.Add(oNewRow)

            Me.EmployeeAssignmentData = tb
            e.Cancel = True
            grid.CancelEdit()

        End If
    End Sub

    Protected Sub GridAssignments_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles gridSuitability.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridAssignments_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles gridSuitability.RowUpdating

        Dim tb As DataTable = Me.EmployeeAssignmentData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(gridSuitability.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        'if changing the job, make sure it is unique
        If e.OldValues("IDAssignment") <> e.NewValues("IDAssignment") Then
            For Each row As DataRow In tb.Rows
                If row("IDAssignment") = e.NewValues("IDAssignment") Then
                    Throw New Exception(Me.Language.Translate("Error.DuplicatedAssignment", Me.DefaultScope))
                End If
            Next
        End If

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()

            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey

                Case "Suitability"
                    dr.Item("Suitability") = 100
                Case "IDAssignment"
                    If enumerator.Value Is Nothing Then
                        dr.Item("IDAssignment") = DBNull.Value
                    Else
                        dr.Item("IDAssignment") = enumerator.Value
                    End If

            End Select

        End While

        Me.EmployeeAssignmentData = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridAssignments_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles gridSuitability.RowDeleting
        Dim tb As DataTable = Me.EmployeeAssignmentData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(gridSuitability.KeyFieldName))
        dr.Delete()
        tb.AcceptChanges()
        Me.EmployeeAssignmentData = tb
        e.Cancel = True
    End Sub

    Private Property IdCurrentEmployee() As Integer
        Get
            Return roTypes.Any2Integer(Session("Employee_IdCurrentEmployee"))
        End Get
        Set(ByVal value As Integer)
            Session("Employee_IdCurrentEmployee") = value
        End Set
    End Property

    ''' <summary>
    ''' només usat per poblar el combobox del grid
    ''' </summary>
    ''' <returns></returns>
    Private Property AssignmentsData(ByVal bolReload As Boolean) As DataView
        Get

            Dim tbCauses As DataTable = Session("EmployeeAssignments_AssignmentsData")
            Dim dv As DataView = Nothing
            If tbCauses IsNot Nothing Then
                dv = New DataView(tbCauses)
                dv.Sort = "Name ASC"
            End If

            If bolReload OrElse dv Is Nothing Then

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

    Private Function SaveEmployeeAssignmentsGrid(ByVal IDEmployee As Integer) As Boolean

        Dim oAssignments As New Generic.List(Of roEmployeeAssignment)

        For Each oRow As DataRow In EmployeeAssignmentData.Rows
            Dim oEmployeeAssignment As New roEmployeeAssignment
            If oRow.RowState <> DataRowState.Deleted Then
                If Not IsDBNull(oRow("IDAssignment")) Then
                    oEmployeeAssignment.IDEmployee = oRow("IDEmployee")
                    oEmployeeAssignment.IDAssignment = oRow("IDAssignment")
                    oEmployeeAssignment.Suitability = 100 'roTypes.Any2Integer(oRow("Suitability"))
                    oAssignments.Add(oEmployeeAssignment)
                End If
            End If
        Next

        Return API.EmployeeServiceMethods.SaveEmployeeAssignments(Me, IDEmployee, oAssignments)

    End Function

#End Region

    Private Sub LoadEmployeeDataTab6(ByVal oParameters As EmployeeCallbackRequest)
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)
        If oEmployee Is Nothing Then Return

        Me.activeNotificationsTable.Controls.Add(Me.CreateActiveNotificationsTableV3(oParameters.ID))
    End Sub

    Private Sub LoadEmployeeDataTab7(ByVal oParameters As EmployeeCallbackRequest)
        'Tab7 removed
    End Sub

    Private Sub LoadEmployeeDataTab8(ByVal oParameters As EmployeeCallbackRequest)
        'Tab8 removed
    End Sub

    ''' <summary>
    ''' Retorna un Datable amb les dades de Ausencies
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProgrammedAbsencesData(ByVal IDEmployee As Integer) As DataTable
        Try
            Dim tb As DataTable
            tb = API.ProgrammedAbsencesServiceMethods.GetProgrammedAbsences(Me, IDEmployee)

            ' Añadir la columna con el literal que define la ausencia
            tb.Columns.Add(New DataColumn("Literal", GetType(String)))
            Dim Params As Generic.List(Of String)
            For Each oRow As DataRow In tb.Rows
                If Not IsDBNull(oRow("IDCause")) Then
                    Params = New Generic.List(Of String)
                    Params.Add(CDate(oRow("BeginDate")).ToShortDateString)
                    Params.Add(CDate(oRow("RealFinishDate")).ToShortDateString)
                    Params.Add(oRow("Name"))
                    If Not IsDBNull(oRow("Description")) AndAlso oRow("Description") <> "" Then
                        Params.Add("(" & oRow("Description") & ")")
                    Else
                        Params.Add("")
                    End If
                    oRow("Literal") = Me.Language.Translate("ProgrammedAbsence.Literal", Me.DefaultScope, Params)
                End If
            Next

            Return tb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un Datable amb les dades de Incidencies previstes
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProgrammedIncidencesData(ByVal IDEmployee As Integer) As DataTable
        Try
            Dim tb As DataTable
            tb = API.ProgrammedCausesServiceMethods.GetProgrammedCauses(Me, IDEmployee)

            ' Añadir la columna con el literal que define la ausencia
            tb.Columns.Add(New DataColumn("Literal", GetType(String)))
            Dim Params As Generic.List(Of String)
            For Each oRow As DataRow In tb.Rows
                If Not IsDBNull(oRow("IDCause")) Then
                    Params = New Generic.List(Of String)
                    Params.Add(CDate(oRow("Date")).ToShortDateString)

                    If IsDBNull(oRow("FinishDate")) Then
                        Params.Add(CDate(oRow("Date")).ToShortDateString)
                    Else
                        Params.Add(CDate(oRow("FinishDate")).ToShortDateString)
                    End If

                    Params.Add(CDate(roTypes.Any2Time(oRow("Duration")).Value).ToShortTimeString)
                    Params.Add(oRow("Name"))
                    If Not IsDBNull(oRow("Description")) AndAlso oRow("Description") <> "" Then
                        Params.Add("(" & oRow("Description") & ")")
                    Else
                        Params.Add("")
                    End If
                    If Not IsDBNull(oRow("BeginTime")) Then
                        Params.Add(CDate(oRow("BeginTime")).ToShortTimeString)
                    Else
                        Params.Add("00:00")
                    End If

                    If Not IsDBNull(oRow("EndTime")) Then
                        Params.Add(CDate(oRow("EndTime")).ToShortTimeString)
                    Else
                        Params.Add("23:59")
                    End If

                    oRow("Literal") = Me.Language.Translate("ProgrammedCause.Literal", Me.DefaultScope, Params)
                End If
            Next

            Return tb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function ProgrammedHolidaysData(ByVal IDEmployee As Integer) As Generic.List(Of roProgrammedHoliday)
        Try
            Dim oLst As Generic.List(Of roProgrammedHoliday) = ProgrammedHolidaysServiceMethods.GetProgrammedHolidaysList(IDEmployee, "", Me.Page, True)

            Return oLst
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function ProgrammedOvertimesData(ByVal IDEmployee As Integer) As Generic.List(Of roProgrammedOvertime)
        Try
            Dim oLst As Generic.List(Of roProgrammedOvertime) = ProgrammedOvertimesServiceMethods.GetProgrammedOvertimesList(IDEmployee, "", Me.Page, True)

            Return oLst
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Recupera les descripcions
    ''' </summary>
    ''' <param name="IdEmployee"></param>
    ''' <param name="strFullGroupName"></param>
    ''' <remarks></remarks>
    Private Sub StateDescriptions(ByVal IdEmployee As Integer, ByRef strFullGroupName As String)
        Try
            ' Mostrar información resumen tipo identificación empleado
            lblIdentifyMethodsInfo = ""
            If Me.HasFeaturePermissionByEmployee("Employees.IdentifyMethods", Permission.Read, IdEmployee) Then
                Dim strMsgId As String = ""
                Dim oEmployeePassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me, IdEmployee, LoadType.Employee)
                If oEmployeePassport IsNot Nothing Then
                    Dim tbMethods As roPassportAuthenticationMethods = oEmployeePassport.AuthenticationMethods
                    Dim oCardMethods As roPassportAuthenticationMethodsRow() = tbMethods.CardRows
                    Dim oBioMethods As roPassportAuthenticationMethodsRow() = tbMethods.BiometricRows
                    Dim oPinMethods As roPassportAuthenticationMethodsRow = tbMethods.PinRow
                    If oCardMethods IsNot Nothing AndAlso oCardMethods.Length > 0 Then strMsgId &= "OrCard"
                    If oBioMethods IsNot Nothing AndAlso oBioMethods.Length > 0 Then strMsgId &= "OrBio"
                    If oPinMethods IsNot Nothing Then strMsgId &= "OrPin"
                    If strMsgId <> "" Then
                        strMsgId = strMsgId.Substring(2)
                    Else
                        strMsgId = "NoPunches"
                    End If
                Else
                    strMsgId = "NoPunches"
                End If
                lblIdentifyMethodsInfo = Me.Language.Translate("MeansAccess.PType." & strMsgId, Me.DefaultScope)
            End If

            ' Cargo el literal de contratos
            lblContractsDescription = ""
            lblContractsInfo = ""
            If Me.HasFeaturePermissionByEmployee("Employees.Contract", Permission.Read, IdEmployee) Then
                Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me, IdEmployee, False)
                If oContract IsNot Nothing Then
                    If API.ContractsServiceMethods.LastError.Result = ContractsResultEnum.ContractNotFound Then
                        lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription.NoContract", Me.DefaultScope)
                    Else
                        If oContract.EndDate.Date = New Date(2079, 1, 1) Then
                            Dim Params As New Generic.List(Of String)
                            Params.Add(oContract.IDContract)
                            Params.Add(oContract.BeginDate.ToShortDateString)
                            lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription", Me.DefaultScope, Params)
                        Else
                            Dim Params As New Generic.List(Of String)
                            Params.Add(oContract.IDContract)
                            Params.Add(oContract.BeginDate.ToShortDateString)
                            Params.Add(Format(oContract.EndDate, HelperWeb.GetShortDateFormat))
                            lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription.EndDate", Me.DefaultScope, Params)
                        End If
                    End If
                    lblContractsInfo = lblContractsDescription
                End If
            End If

            ' Cargo el literal de mobilidad
            If Me.HasFeaturePermissionByEmployee("Employees.GroupMobility", Permission.Read, IdEmployee) Then
                Dim AuxFullGroupName = API.EmployeeServiceMethods.GetCurrentFullGroupName(Me, IdEmployee)
                strFullGroupName = Me.Language.Translate("Employee_Group", Me.DefaultScope) & AuxFullGroupName

            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Function LoadTypeEmployee(ByVal oEmployee As roEmployee) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hRow As New HtmlTableRow
            Dim hCell As HtmlTableCell
            Dim aType As HtmlAnchor

            Select Case oEmployee.Type
                Case Is = "NADA AUN" ' etc.
                Case Else
                    hCell = New HtmlTableCell
                    aType = New HtmlAnchor
                    aType.Attributes("class") = "typeEmpPres"
                    If HelperSession.GetFeatureIsInstalledFromApplication("Version\LiveExpress") Then
                        aType.HRef = "javascript: void(0);"
                        aType.Attributes("onclick") = "ShowEmployeeTypes('" & oEmployee.ID & "');"
                    End If
                    'Me.lblTypeEmp.Text = "Presencia"
                    hCell.Controls.Add(aType)
                    hRow.Cells.Add(hCell)
            End Select

            hTable.Rows.Add(hRow)
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Ausencias
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridAusencias(ByVal dTable As DataTable, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("ProgrammedAbsence", Me.DefaultScope)
            hTCell.ColSpan = 3
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    If editIcons = True Then
                        hTCell.Attributes("style") = "padding: 0px;"
                        strClickEdit = "editGridAus("
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                        hTCell.InnerHtml = "<a href=""javascript: void(0);"" class=""rowSelectable"" onclick=""" & strClickEdit & """>" & dTable.Rows(n)(y).ToString & "</a>"
                    Else
                        hTCell.InnerText = dTable.Rows(n)(y).ToString
                    End If

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cellheader"
                    hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;padding: 0px;cursor:pointer"
                    hTCell.Width = "20px"
                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    If editIcons = True Then
                        strClickEdit = "editGridAus("
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                        hTCell.Attributes("onclick") = strClickEdit
                        '" & dTable.Rows(n)("HasDocuments").ToString & "

                        If roTypes.Any2Integer(dTable.Rows(n)("HasDocuments")) = 1 Then
                            If roTypes.Any2Integer(dTable.Rows(n)("DocumentsDelivered")) = 1 Then
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                            Else
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.png""/>"
                            End If
                        Else
                            hTCell.InnerHtml = ""
                        End If
                    Else
                        If roTypes.Any2Integer(dTable.Rows(n)("HasDocuments")) = 1 Then
                            If roTypes.Any2Integer(dTable.Rows(n)("DocumentsDelivered")) = 1 Then
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                            Else
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.Gif""/>"
                            End If
                        Else
                            hTCell.InnerHtml = ""
                        End If
                    End If

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons = True Then
                        If y = dTable.Columns.Count - 1 Then
                            hTCell = New HtmlTableCell
                            hTCell.Width = "40px"
                            hTCell.Attributes("class") = "GridStyle-cellheader"
                            hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                            Dim hAnchorEdit As New HtmlAnchor
                            Dim hAnchorRemove As New HtmlAnchor

                            strClickEdit = "editGridAus("
                            strClickRemove = "ShowRemoveGridAus("
                            If dTblControls IsNot Nothing Then
                                For Each dRow As DataRow In dTblControls.Rows
                                    strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                    strClickRemove &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                Next
                            End If
                            strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                            strClickRemove = strClickRemove.Substring(0, strClickRemove.Length - 1) & ");"

                            If oPermission > Permission.Read Then
                                hAnchorEdit.Attributes("onclick") = strClickEdit
                                hAnchorEdit.Title = Me.Language.Keyword("Button.Edit")
                                hAnchorEdit.HRef = "javascript: void(0);"
                                hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/edit.png") & """>"

                                hAnchorRemove.Attributes("onclick") = strClickRemove
                                hAnchorRemove.Title = Me.Language.Keyword("Button.Delete")
                                hAnchorRemove.HRef = "javascript: void(0);"
                                hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/remove.png") & """>"
                                hTCell.Controls.Add(hAnchorEdit)
                                hTCell.Controls.Add(hAnchorRemove)

                                hTRow.Cells.Add(hTCell)
                            End If
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Incidencias previstas
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridIncidencias(ByVal dTable As DataTable, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False
            Dim bIsWin32 As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("tabIncidence", Me.DefaultScope)
            hTCell.ColSpan = 3
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                bIsWin32 = False

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dTable.Columns(y).ColumnName = "Win32" AndAlso dTable.Rows(n)("Win32") = "True" Then
                                bIsWin32 = True
                            End If

                            If dRowSel(0).Item("Visible") = False Then
                                Continue For
                            End If
                        End If
                    End If

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    If editIcons = True Then
                        hTCell.Attributes("style") = "padding: 0px;"
                        strClickEdit = "editGridInc("
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                        hTCell.InnerHtml = "<a href=""javascript: void(0);"" class=""rowSelectable"" onclick=""" & strClickEdit & """>" & dTable.Rows(n)(y).ToString & "</a>"
                    Else
                        hTCell.InnerText = dTable.Rows(n)(y).ToString
                    End If

                    If bIsWin32 Then hTCell.Attributes("style") = hTCell.Attributes("style") + "background-color:#ffff80;"

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cellheader"
                    hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;padding: 0px;cursor:pointer"
                    hTCell.Width = "20px"
                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    If editIcons = True Then
                        strClickEdit = "editGridInc("
                        If dTblControls IsNot Nothing Then
                            For Each dRow As DataRow In dTblControls.Rows
                                strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                            Next
                        End If
                        strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                        hTCell.Attributes("onclick") = strClickEdit
                        '" & dTable.Rows(n)("HasDocuments").ToString & "

                        If roTypes.Any2Integer(dTable.Rows(n)("HasDocuments")) = 1 Then
                            If roTypes.Any2Integer(dTable.Rows(n)("DocumentsDelivered")) = 1 Then
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                            Else
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.png""/>"
                            End If
                        Else
                            hTCell.InnerHtml = ""
                        End If
                    Else
                        If roTypes.Any2Integer(dTable.Rows(n)("HasDocuments")) = 1 Then
                            If roTypes.Any2Integer(dTable.Rows(n)("DocumentsDelivered")) = 1 Then
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                            Else
                                hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.Gif""/>"
                            End If
                        Else
                            hTCell.InnerHtml = ""
                        End If
                    End If

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons = True Then
                        If y = dTable.Columns.Count - 1 Then
                            hTCell = New HtmlTableCell
                            hTCell.Width = "40px"
                            hTCell.Attributes("class") = "GridStyle-cellheader"
                            hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                            Dim hAnchorEdit As New HtmlAnchor
                            Dim hAnchorRemove As New HtmlAnchor

                            strClickEdit = "editGridInc("
                            strClickRemove = "ShowRemoveGridInc("
                            If dTblControls IsNot Nothing Then
                                For Each dRow As DataRow In dTblControls.Rows
                                    strClickEdit &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                    strClickRemove &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                Next
                            End If
                            strClickEdit = strClickEdit.Substring(0, strClickEdit.Length - 1) & ");"
                            strClickRemove = strClickRemove.Substring(0, strClickRemove.Length - 1) & ");"

                            If oPermission > Permission.Read Then
                                hAnchorEdit.Attributes("onclick") = strClickEdit
                                hAnchorEdit.Title = Me.Language.Keyword("Button.Edit")
                                hAnchorEdit.HRef = "javascript: void(0);"
                                hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/edit.png") & """>"

                                hAnchorRemove.Attributes("onclick") = strClickRemove
                                hAnchorRemove.Title = Me.Language.Keyword("Button.Delete")
                                hAnchorRemove.HRef = "javascript: void(0);"
                                hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/remove.png") & """>"
                                hTCell.Controls.Add(hAnchorEdit)
                                hTCell.Controls.Add(hAnchorRemove)

                                hTRow.Cells.Add(hTCell)
                            End If
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Incidencias previstas
    ''' </summary>
    ''' <param name="oOvertimesLst"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridOvertimes(ByVal oOvertimesLst As Generic.List(Of roProgrammedOvertime), ByVal intIdEmployee As Integer, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False
            Dim bIsWin32 As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("tabProgrammedOvertimes", Me.DefaultScope)
            hTCell.ColSpan = 3
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To oOvertimesLst.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim oActualOvertime As roProgrammedOvertime = oOvertimesLst(n)

                Dim bOneDay As Boolean = True

                Dim Params = New Generic.List(Of String)

                Params.Add(oActualOvertime.ProgrammedBeginDate.ToShortDateString())

                If oActualOvertime.ProgrammedBeginDate <> oActualOvertime.ProgrammedEndDate Then
                    Params.Add(oActualOvertime.ProgrammedEndDate.ToShortDateString())
                    bOneDay = False
                End If

                Params.Add(CDate(roTypes.Any2Time(oActualOvertime.Duration).Value).ToShortTimeString)
                Params.Add(CausesServiceMethods.GetCauseByID(Me.Page, oActualOvertime.IDCause, False).Name)
                Params.Add(oActualOvertime.Description)
                Params.Add(oActualOvertime.BeginTime.ToShortTimeString())
                Params.Add(oActualOvertime.EndTime.ToShortTimeString())

                Dim strLang As String = String.Empty

                If bOneDay Then
                    strLang = Me.Language.Translate("ProgrammedOvertimes.OneDayLiteral", Me.DefaultScope, Params)
                Else
                    strLang = Me.Language.Translate("ProgrammedOvertimes.MultipleDaysLiteral", Me.DefaultScope, Params)
                End If

                hTCell = New HtmlTableCell

                'Cambia el alternateRow
                hTCell.Attributes("class") = "GridStyle-cell" & altRow & " GridStyle-endcell" & altRow

                If editIcons = True Then
                    hTCell.Attributes("style") = "padding: 0px;"
                    strClickEdit = "editGridOvertime(" & oActualOvertime.ID & "," & intIdEmployee & ");"
                    hTCell.InnerHtml = "<a href=""javascript: void(0);"" class=""rowSelectable"" onclick=""" & strClickEdit & """>" & strLang & "</a>"
                Else
                    If oActualOvertime.ID = 0 Then
                        hTCell.InnerText = oActualOvertime.Description
                    Else
                        hTCell.InnerText = strLang
                    End If
                End If

                If hTCell.InnerText = "" Then hTCell.InnerText = " "

                'Carrega la celda al row
                hTRow.Cells.Add(hTCell)

                hTCell = New HtmlTableCell
                'Cambia el alternateRow
                hTCell.Attributes("class") = "GridStyle-cellheader"
                hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;padding: 0px;cursor:pointer"
                hTCell.Width = "20px"
                'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                If editIcons = True Then
                    strClickEdit = "editGridOvertime(" & oActualOvertime.ID & "," & intIdEmployee & ");"
                    hTCell.Attributes("onclick") = strClickEdit
                    '" & dTable.Rows(n)("HasDocuments").ToString & "

                    If oActualOvertime.HasDocuments Then
                        If oActualOvertime.DocumentsDelivered Then
                            hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                        Else
                            hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.png""/>"
                        End If
                    Else
                        hTCell.InnerHtml = ""
                    End If
                Else
                    If oActualOvertime.HasDocuments Then
                        If oActualOvertime.DocumentsDelivered Then
                            hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_OK.png""/>"
                        Else
                            hTCell.InnerHtml = "<img alt="""" onclick=""" & strClickEdit & """ width=""20px"" height=""20px"" src=""../Employees/Images/documents_KO.Gif""/>"
                        End If
                    Else
                        hTCell.InnerHtml = ""
                    End If
                End If

                If hTCell.InnerText = "" Then hTCell.InnerText = " "

                'Carrega la celda al row
                hTRow.Cells.Add(hTCell)

                'Dibuixem les columnes de les icones d'edicio
                If editIcons = True Then

                    hTCell = New HtmlTableCell
                    hTCell.Width = "40px"
                    hTCell.Attributes("class") = "GridStyle-cellheader"
                    hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                    Dim hAnchorEdit As New HtmlAnchor
                    Dim hAnchorRemove As New HtmlAnchor

                    strClickEdit = "editGridOvertime(" & oActualOvertime.ID & "," & intIdEmployee & ");"
                    strClickRemove = "ShowRemoveOvertimeInc(" & oActualOvertime.ID & ");"

                    If oPermission > Permission.Read Then
                        hAnchorEdit.Attributes("onclick") = strClickEdit
                        hAnchorEdit.Title = Me.Language.Keyword("Button.Edit")
                        hAnchorEdit.HRef = "javascript: void(0);"
                        hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/edit.png") & """>"

                        hAnchorRemove.Attributes("onclick") = strClickRemove
                        hAnchorRemove.Title = Me.Language.Keyword("Button.Delete")
                        hAnchorRemove.HRef = "javascript: void(0);"
                        hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/remove.png") & """>"
                        hTCell.Controls.Add(hAnchorEdit)
                        hTCell.Controls.Add(hAnchorRemove)

                        hTRow.Cells.Add(hTCell)
                    End If
                End If

                hTable.Rows.Add(hTRow)
            Next

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Incidencias previstas
    ''' </summary>
    ''' <param name="oHolidaysLst"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridHolidays(ByVal oHolidaysLst As Generic.List(Of roProgrammedHoliday), ByVal intIdEmployee As Integer, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False
            Dim bIsWin32 As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = Me.Language.Translate("tabProgrammedHolidays", Me.DefaultScope)
            hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To oHolidaysLst.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim oActualHoliday As roProgrammedHoliday = oHolidaysLst(n)

                Dim Params = New Generic.List(Of String)
                Params.Add(oActualHoliday.ProgrammedDate.ToShortDateString())

                Params.Add(CDate(roTypes.Any2Time(oActualHoliday.Duration).Value).ToShortTimeString)
                Params.Add(CausesServiceMethods.GetCauseByID(Me.Page, oActualHoliday.IDCause, False).Name)
                Params.Add(oActualHoliday.Description)
                Params.Add(oActualHoliday.BeginTime.ToShortTimeString())
                Params.Add(oActualHoliday.EndTime.ToShortTimeString())

                Dim strLang As String = String.Empty

                If oActualHoliday.AllDay Then
                    strLang = Me.Language.Translate("ProgrammedHolidays.AllDayLiteral", Me.DefaultScope, Params)
                Else
                    strLang = Me.Language.Translate("ProgrammedHolidays.Literal", Me.DefaultScope, Params)
                End If

                hTCell = New HtmlTableCell

                'Cambia el alternateRow
                hTCell.Attributes("class") = "GridStyle-cell" & altRow & " GridStyle-endcell" & altRow

                If editIcons = True Then
                    hTCell.Attributes("style") = "padding: 0px;"
                    strClickEdit = "editGridHolidays(" & oActualHoliday.ID & "," & intIdEmployee & ");"
                    hTCell.InnerHtml = "<a href=""javascript: void(0);"" class=""rowSelectable"" onclick=""" & strClickEdit & """>" & strLang & "</a>"
                Else
                    If oActualHoliday.ID = 0 Then
                        hTCell.InnerText = oActualHoliday.Description
                    Else
                        hTCell.InnerText = strLang
                    End If
                End If

                If hTCell.InnerText = "" Then hTCell.InnerText = " "

                'Carrega la celda al row
                hTRow.Cells.Add(hTCell)

                'Dibuixem les columnes de les icones d'edicio
                If editIcons = True Then

                    hTCell = New HtmlTableCell
                    hTCell.Width = "40px"
                    hTCell.Attributes("class") = "GridStyle-cellheader"
                    hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                    Dim hAnchorEdit As New HtmlAnchor
                    Dim hAnchorRemove As New HtmlAnchor

                    strClickEdit = "editGridHolidays(" & oActualHoliday.ID & "," & intIdEmployee & ");"
                    strClickRemove = "ShowRemoveHolidaysInc(" & oActualHoliday.ID & ");"

                    If oPermission > Permission.Read Then
                        hAnchorEdit.Attributes("onclick") = strClickEdit
                        hAnchorEdit.Title = Me.Language.Keyword("Button.Edit")
                        hAnchorEdit.HRef = "javascript: void(0);"
                        hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/edit.png") & """>"

                        hAnchorRemove.Attributes("onclick") = strClickRemove
                        hAnchorRemove.Title = Me.Language.Keyword("Button.Delete")
                        hAnchorRemove.HRef = "javascript: void(0);"
                        hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/remove.png") & """>"
                        hTCell.Controls.Add(hAnchorEdit)
                        hTCell.Controls.Add(hAnchorRemove)

                        hTRow.Cells.Add(hTCell)
                    End If
                End If

                hTable.Rows.Add(hTRow)
            Next

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un Datable amb les dades de Puestos
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property EmployeeAssignmentData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("Employee_EmployeeAssignmentsData")

            If bolReload OrElse tb Is Nothing AndAlso Me.IdCurrentEmployee > 0 Then
                tb = API.EmployeeServiceMethods.GetAssignments(Me, Me.IdCurrentEmployee, True)

                'Add a local ID column
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

                'to avoid duplicates
                'Dim columns(0) As DataColumn
                'columns(0) = tb.Columns("IDAssignment")
                ' columns(1) = tb.Columns("IDEmployee")
                'tb.PrimaryKey = columns

                Session("Employee_EmployeeAssignmentsData") = tb
                'Else
                '    tb = New DataTable

                '    Dim index As Integer = 1
                '    For Each oRow As DataRow In tb.Rows
                '        oRow("ID") = index
                '        index = index + 1
                '    Next

                '    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                '    tb.AcceptChanges()

                '    Session("Employee_EmployeeAssignmentsData") = tb
            End If

            Return tb
        End Get
        Set(value As DataTable)
            Session("Employee_EmployeeAssignmentsData") = value
        End Set
    End Property

    ''' <summary>
    ''' Crea un Grid d'estructura de Puestos
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridPuestos(ByVal dTable As DataTable, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            If dTblControls IsNot Nothing Then
                hTRow = New HtmlTableRow

                For Each oRow As DataRow In dTblControls.Rows
                    If oRow("Visible") Then
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cellheader"
                        hTCell.InnerHtml = oRow("NomHeader")
                        hTRow.Cells.Add(hTCell)
                    End If
                Next
                hTable.Rows.Add(hTRow)
            End If

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Or y = 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

#Region "Security V3"

    Private Function CreateActiveNotificationsTableV3(ByVal idEmployee As Integer) As HtmlTable
        Dim hTable As New HtmlTable
        Dim hTRow As HtmlTableRow
        Dim hTCell As HtmlTableCell

        With hTable
            .Border = 0
            .CellPadding = 0
            .CellSpacing = 0
            .Attributes("class") = "FeaturesTableStyle GridFeatures"
        End With

        ' Añadimos fila nombres columnas
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader FeaturesTableStyle-cellheader-noend"
        hTCell.InnerHtml = Me.Language.Translate("Supervisors.Columns.Name", Me.DefaultScope) ' "Funcionalidad"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader"
        hTCell.Attributes("style") = "text-align: right;"
        hTCell.InnerHtml = Me.Language.Translate("Supervisors.Columns.Supervisor", Me.DefaultScope) '"Permiso"
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim notificationsTable As DataTable = API.NotificationServiceMethods.GetNotificationsSupervisor(Me.Page, WLHelperWeb.CurrentPassport.ID, idEmployee)

        Dim oNotificationsRows As Generic.List(Of HtmlTableRow) = Me.GetNotificationsV3(notificationsTable)
        If oNotificationsRows IsNot Nothing Then
            For Each oRow As HtmlTableRow In oNotificationsRows
                hTable.Rows.Add(oRow)
            Next
        End If

        Return hTable
    End Function

    Private Function GetNotificationsV3(ByVal notificationsDT As DataTable) As Generic.List(Of HtmlTableRow)

        Dim oFeatureRows As New Generic.List(Of HtmlTableRow)

        If notificationsDT IsNot Nothing AndAlso notificationsDT.Rows.Count > 0 Then

            ' Obtenemos las funcionalidades del nivel actual (idParentfeature)
            Dim oMainFeature As New Generic.List(Of DataRow)
            For Each oItem As DataRow In notificationsDT.Rows
                'If oItem("IDParent") Is DBNull.Value Then
                oMainFeature.Add(oItem)
                'End If
            Next

            Dim hTRow As HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim divFeature As HtmlGenericControl
            Dim divAnchorFeature As HtmlGenericControl
            Dim divOpenFeature As HtmlGenericControl
            Dim aAnchor As HtmlAnchor
            Dim aAnchorInfo As HtmlAnchor
            Dim iHtmlImg As HtmlImage

            ' Obtenemos traducción tooltip botón mostrar información
            Dim strFeatureInformationButton As String = Me.Language.Translate("Feature.Information.Button", Me.DefaultScope)

            Dim intLevel As Integer = 1
            Dim bolheader As Boolean = False
            For Each oFeatureRow As DataRow In oMainFeature
                ' Pinta la fila con el nombre de la categoría actual y sus permisos

                If oFeatureRow("TypeFeature") = 2 And Not bolheader Then
                    ' Añadimos fila nombres columnas
                    hTRow = New HtmlTableRow

                    hTCell = New HtmlTableCell

                    hTCell.Attributes("class") = "FeaturesTableStyle-cellheader FeaturesTableStyle-cellheader-noend"
                    hTCell.InnerHtml = Me.Language.Translate("Supervisors.Columns.Request", Me.DefaultScope) ' "Funcionalidad"
                    hTRow.Cells.Add(hTCell)

                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "FeaturesTableStyle-cellheader"
                    hTCell.Attributes("style") = "text-align: right;"
                    hTCell.InnerHtml = Me.Language.Translate("Supervisors.Columns.Supervisor", Me.DefaultScope) '"Permiso"
                    hTRow.Cells.Add(hTCell)
                    bolheader = True

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)
                End If

                hTRow = New HtmlTableRow

                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " " & "FeaturesTableStyle-noendcellLevel" & intLevel

                aAnchor = New HtmlAnchor
                With aAnchor
                    .HRef = "javascript:void(0);"
                    .Style("width") = "100%"
                    .Attributes("class") = "FeatureAnchor"
                    .Style("cursor") = "default"
                    .InnerHtml = oFeatureRow("Name")
                End With

                iHtmlImg = New HtmlImage
                With iHtmlImg
                    .Alt = ""
                    .ID = "aFeatureOpenImg" & oFeatureRow("ID")
                    .Style("min-width") = "16px"
                    .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/s.gif")
                End With

                divFeature = New HtmlGenericControl("div")
                divFeature.Style("width") = "100%"

                divOpenFeature = New HtmlGenericControl("div")
                divOpenFeature.Style("float") = "left"
                divOpenFeature.Style("text-align") = "left"
                divOpenFeature.Controls.Add(iHtmlImg)

                divAnchorFeature = New HtmlGenericControl("div")
                divAnchorFeature.Style("float") = "left"
                divAnchorFeature.Style("text-align") = "left"
                divAnchorFeature.Controls.Add(aAnchor)

                divFeature.Controls.Add(divOpenFeature)
                divFeature.Controls.Add(divAnchorFeature)
                hTCell.Controls.Add(divFeature)

                hTRow.Cells.Add(hTCell)

                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " FeaturesTableStyle-cellPermissions"
                hTCell.Attributes("align") = "right"
                hTCell.Style("padding") = "3px"
                hTCell.Style("min-width") = "400px"

                aAnchorInfo = New HtmlAnchor
                With aAnchorInfo
                    .Style("width") = "100%"
                    .Style("cursor") = "default"
                    .Attributes("class") = "FeatureAnchor"
                    .InnerHtml = oFeatureRow("SupervisorName")
                End With

                divFeature = New HtmlGenericControl("div")
                divFeature.Style("width") = "100%"

                divAnchorFeature = New HtmlGenericControl("div")
                divAnchorFeature.Style("float") = "left"
                divAnchorFeature.Style("text-align") = "left"
                divAnchorFeature.Controls.Add(aAnchorInfo)

                divFeature.Controls.Add(divAnchorFeature)
                hTCell.Controls.Add(divFeature)

                hTRow.Cells.Add(hTCell)

                ' Añadimos fila la colección
                oFeatureRows.Add(hTRow)

            Next

        End If

        Return oFeatureRows

    End Function

#End Region

    Private Sub AddChildFeatures(ByRef oFeatureTableRows As Generic.List(Of HtmlTableRow), ByVal allRows As DataTable, ByVal featureRows As Generic.List(Of DataRow), ByVal featureLevel As Integer)
        Dim hTRow As HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim divFeature As HtmlGenericControl
        Dim divAnchorFeature As HtmlGenericControl
        Dim divOpenFeature As HtmlGenericControl
        Dim aAnchor As HtmlAnchor
        Dim aAnchorInfo As HtmlAnchor
        Dim iHtmlImg As HtmlImage

        For Each oFeatureRow As DataRow In featureRows
            ' Pinta la fila con el nombre de la categoría actual y sus permisos
            hTRow = New HtmlTableRow
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & featureLevel & " " & "FeaturesTableStyle-noendcellLevel" & featureLevel

            aAnchor = New HtmlAnchor
            With aAnchor
                .HRef = "javascript:void(0);"
                .Style("width") = "100%"
                .Attributes("class") = "FeatureAnchor"
                If oFeatureRow("IsGroup") Then
                    .Attributes("onclick") = "ShowHideSupervisorsChilds('" & oFeatureRow("ID") & "');"
                Else
                    .Style("cursor") = "default"
                End If
                .InnerHtml = oFeatureRow("Name") ' Me.Language.Translate("Feature." & oFeature.Alias & ".Name", Me.DefaultScope)
            End With

            iHtmlImg = New HtmlImage
            With iHtmlImg
                .Alt = ""
                .ID = "aFeatureOpenImg" & oFeatureRow("ID")
                If oFeatureRow("IsGroup") Then
                    .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/tree/elbow-plus-nl.gif")
                    .Attributes("onclick") = "ShowHideSupervisorsChilds('" & oFeatureRow("ID") & "');"
                    .Style("cursor") = "pointer"
                Else
                    .Style("min-width") = "16px"
                    .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/s.gif")
                End If
            End With

            divFeature = New HtmlGenericControl("div")
            divFeature.Style("width") = "100%"

            divOpenFeature = New HtmlGenericControl("div")
            divOpenFeature.Style("float") = "left"
            divOpenFeature.Style("text-align") = "left"
            divOpenFeature.Controls.Add(iHtmlImg)

            divAnchorFeature = New HtmlGenericControl("div")
            divAnchorFeature.Style("float") = "left"
            divAnchorFeature.Style("text-align") = "left"
            divAnchorFeature.Controls.Add(aAnchor)

            divFeature.Controls.Add(divOpenFeature)
            divFeature.Controls.Add(divAnchorFeature)
            hTCell.Controls.Add(divFeature)

            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & featureLevel & " FeaturesTableStyle-cellPermissions"
            hTCell.Attributes("align") = "right"
            hTCell.Style("padding") = "3px"
            hTCell.Style("min-width") = "400px"

            aAnchorInfo = New HtmlAnchor
            With aAnchorInfo
                .Style("width") = "100%"
                .Style("cursor") = "default"
                .Attributes("class") = "FeatureAnchor"
                .InnerHtml = oFeatureRow("SupervisorName")
            End With

            divFeature = New HtmlGenericControl("div")
            divFeature.Style("width") = "100%"

            divAnchorFeature = New HtmlGenericControl("div")
            divAnchorFeature.Style("float") = "left"
            divAnchorFeature.Style("text-align") = "left"
            divAnchorFeature.Controls.Add(aAnchorInfo)

            divFeature.Controls.Add(divAnchorFeature)
            hTCell.Controls.Add(divFeature)

            hTRow.Cells.Add(hTCell)

            ' Añadimos fila la colección
            oFeatureTableRows.Add(hTRow)

            Dim childFeatures As New Generic.List(Of DataRow)
            For Each oItem As DataRow In allRows.Rows
                If oItem("IDParent") IsNot DBNull.Value AndAlso roTypes.Any2Integer(oItem("IDParent")) = roTypes.Any2Integer(oFeatureRow("ID")) Then
                    childFeatures.Add(oItem)
                End If
            Next

            If childFeatures.Count > 0 Then
                Dim hChildsTable As New HtmlTable
                With hChildsTable
                    .ID = "tbFeatureChilds" & oFeatureRow("ID")
                    .Border = 0
                    .CellPadding = 0
                    .CellSpacing = 0
                    .Attributes("class") = "FeaturesTableStyle GridFeatureChilds"
                End With

                hTRow = New HtmlTableRow
                hTRow.ID = "rowFeatureChilds" & oFeatureRow("ID")
                hTRow.Attributes("style") = "display:none"
                hTCell = New HtmlTableCell
                hTCell.ColSpan = 2
                hTCell.Style("padding-left") = "15px"

                Dim oChildRows As New Generic.List(Of HtmlTableRow)
                AddChildFeatures(oChildRows, allRows, childFeatures, featureLevel + 1)

                For Each oChildRow As HtmlTableRow In oChildRows
                    hChildsTable.Controls.Add(oChildRow)
                Next

                hTCell.Controls.Add(hChildsTable)
                hTRow.Cells.Add(hTCell)

                ' Añadimos fila la colección
                oFeatureTableRows.Add(hTRow)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Crea el Grid de Usuaris TAB General
    ''' </summary>
    ''' <param name="dTable">DataTable amb les dades del usuari</param>
    ''' <param name="dColField">Columna que servira per asignar l'ID al input text i que no es pintara</param>
    ''' <param name="editMode">Si pintara els input text</param>
    ''' <param name="arrErrors">Datatable amb els error que pintara</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGrid(ByVal IDEmployee As Integer, ByVal dTable As DataTable, ByVal ColumnNames() As String, Optional ByVal dColField As String = "",
                              Optional ByVal editMode As Boolean = False, Optional ByVal arrErrors As DataTable = Nothing) As HtmlTable
        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        hTable.Attributes("class") = "GridStyleNext GridEmpleados"

        'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyleNext-cellheader"
        hTCell.Width = "250"
        hTCell.Attributes("style") = "border-right: 0; width: 250px"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Field", Me.DefaultScope) ' "Campo"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyleNext-cellheader"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Value", Me.DefaultScope) '"Valor"
        hTCell.ColSpan = 2
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim fieldIndex As Integer = 0
        Me.hdnEmployeeFieldsIDs.Value = ""

        Dim Categories() As String = API.UserFieldServiceMethods.GetCategories(Me, True)
        If Categories IsNot Nothing Then

            ReDim Preserve Categories(Categories.Length)
            Categories(Categories.Length - 1) = ""

            Dim Rows() As DataRow
            Dim bolEditable As Boolean = False

            Dim intCountHigh As Integer = 0

            For Each strCategory As String In Categories

                ' Obtenemos los campos correspondientes a la categoría
                'Rows = dTable.Select("Category = '" & strCategory & "'" & IIf(strCategory = "", " OR Category IS NULL", ""), "FieldCaption")
                Rows = dTable.Select(String.Format("Category = '{0}'", strCategory.Replace("'", "''")) & IIf(strCategory = "", " OR Category IS NULL", ""), "FieldCaption")

                If Rows.Length > 0 Then

                    ' Pinta la fila con el nombre de la categoría actual
                    hTRow = New HtmlTableRow
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "GridStyleNext-cell3"
                    hTCell.Style("padding") = "3px"
                    hTCell.ColSpan = 3
                    If strCategory <> "" Then
                        hTCell.InnerText = strCategory
                    Else
                        hTCell.InnerText = Me.Language.Translate("UserField.Category.None", Me.DefaultScope)
                    End If
                    hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyleNext-endcell3"
                    hTRow.Cells.Add(hTCell)
                    hTable.Rows.Add(hTRow)

                    ' Bucle por los campos de la categoría actual
                    For Each oRow As DataRow In Rows

                        hTRow = New HtmlTableRow
                        altRow = IIf(altRow = "1", "2", "1")

                        ' Pinta columna nombre campo
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyleNext-cell" & altRow
                        hTCell.Attributes.Add("nowrap", "nowrap")
                        hTCell.Style("padding") = "3px"

                        Dim genericText As New HtmlGenericControl("div")
                        genericText.InnerText = oRow("FieldCaption").ToString
                        genericText.Attributes("class") = "EmployeeGridWidthNext"

                        Dim genericAlert As New HtmlGenericControl("div")
                        genericAlert.Style("width") = "14px"
                        genericAlert.Style("float") = "right"

                        Dim alertImage As New HtmlImage
                        alertImage.Alt = ""
                        alertImage.Src = VirtualPathUtility.ToAbsolute("~/Employees/Images/Alert16.png")
                        alertImage.Width = 14
                        alertImage.Height = 14

                        genericAlert.Controls.Add(alertImage)

                        hTCell.Controls.Add(genericText)

                        If oRow("IsExpired") IsNot DBNull.Value AndAlso oRow("IsExpired") = True Then
                            hTCell.Controls.Add(genericAlert)
                        End If

                        If roTypes.Any2String(oRow("Description")) <> String.Empty Then
                            Dim fieldInfo As New HtmlGenericControl("div")
                            fieldInfo.Style("width") = "14px"
                            fieldInfo.Style("float") = "right"
                            Dim infoImage As New HtmlImage
                            infoImage.Alt = ""
                            infoImage.Src = VirtualPathUtility.ToAbsolute("~/Base/Images/info-icon.png")
                            infoImage.Width = 14
                            infoImage.Height = 14

                            Dim strDescriptionText As String = roTypes.Any2String(oRow("Description"))
                            If strDescriptionText.StartsWith("Info.") Then
                                fieldInfo.Attributes("title") = Me.Language.Translate(strDescriptionText, Me.DefaultScope)
                            Else
                                fieldInfo.Attributes("title") = strDescriptionText
                            End If

                            fieldInfo.Controls.Add(infoImage)

                            hTCell.Controls.Add(fieldInfo)
                        End If

                        hTRow.Cells.Add(hTCell)

                        ' Pinta columna valor
                        Dim strValue As String = roTypes.Any2String(oRow("Value"))

                        Dim intIdDocument As Integer = -1

                        Dim strValueDate As String = Format(CDate(oRow("Date").ToString), HelperWeb.GetShortDateFormat)
                        If CDate(oRow("Date").ToString) = New Date(1900, 1, 1) Then strValueDate = ""

                        Dim strBegin As String = ""
                        Dim strEnd As String = ""
                        Dim strLanguageKey As String = ""
                        Dim xDate As Date
                        Dim LinkField As HtmlAnchor = Nothing

                        Select Case CType(oRow("Type"), FieldTypes)
                            Case FieldTypes.tDecimal
                                If strValue <> "" Then
                                    strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat())
                                End If
                            Case FieldTypes.tDate
                                If oRow("ValueDateTime").ToString <> "" Then
                                    strValue = Format(CDate(oRow("ValueDateTime").ToString), HelperWeb.GetShortDateFormat)
                                End If

                            Case FieldTypes.tTime
                                ''If oRow("ValueDateTime").ToString <> "" Then
                                ''    strValue = Format(CDate(oRow("ValueDateTime").ToString), HelperWeb.GetShortTimeFormat)
                                ''End If

                            Case FieldTypes.tDatePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(0).Substring(0, 4), strValue.Split("*")(0).Substring(5, 2), strValue.Split("*")(0).Substring(8, 2))
                                    strBegin = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(1).Substring(0, 4), strValue.Split("*")(1).Substring(5, 2), strValue.Split("*")(1).Substring(8, 2))
                                    strEnd = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If

                            Case FieldTypes.tTimePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(0).Substring(0, 2), strValue.Split("*")(0).Substring(3, 2), 0)
                                    strBegin = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(1).Substring(0, 2), strValue.Split("*")(1).Substring(3, 2), 0)
                                    strEnd = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If

                            Case FieldTypes.tLink
                                If strValue.Split("*")(0) <> "" Then
                                    strBegin = strValue.Split("*")(0) 'Texto del enlace a mostrar
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1) <> "" Then
                                    strEnd = strValue.Split("*")(1) 'Texto del enlace real
                                End If

                            Case FieldTypes.tDocument
                                If strValue <> "" Then
                                    intIdDocument = roTypes.Any2Integer(strValue)
                                End If

                            Case Else
                        End Select

                        Select Case CType(oRow("AccessLevel"), Integer)
                            Case 0 ' Low
                                bolEditable = (Me.oUserFieldsAccessPermission(0) >= Permission.Write)
                            Case 1 ' Medium
                                bolEditable = (Me.oUserFieldsAccessPermission(1) >= Permission.Write)
                            Case 2 ' High
                                bolEditable = (Me.oUserFieldsAccessPermission(2) >= Permission.Write)
                            Case Else
                                bolEditable = False
                        End Select

                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyleNext-cell" & altRow

                        Dim divShowValue As HtmlGenericControl = Nothing

                        If oRow("AccessLevel") = 2 Then
                            Dim tbValue As New HtmlTable
                            Dim rwValue As New HtmlTableRow
                            Dim clValue As New HtmlTableCell

                            Dim aAnchorShowValue As New HtmlAnchor
                            With aAnchorShowValue
                                .ID = "aShowValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "ShowUserFieldValue(this, '" & intCountHigh & "', " & IDEmployee.ToString & ", '" & oRow("FieldCaption").ToString & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.ShowCaption", Me.DefaultScope) ' "Mostrar valor ..."
                            End With

                            Dim aAnchorHideValue As New HtmlAnchor
                            With aAnchorHideValue
                                .ID = "aHideValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Style("display") = "none"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "HideUserFieldValue(this, '" & intCountHigh & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.HideCaption", Me.DefaultScope) ' "Ocultar valor ..."
                            End With

                            divShowValue = New HtmlGenericControl("div")
                            With divShowValue
                                .ID = "divShowValue_" & intCountHigh.ToString
                                .Attributes("class") = "UserFieldShowHideValueDiv"
                                .Style("text-align") = "left"
                                .Style("width") = "100%"
                                .Style("display") = "none"
                            End With

                            tbValue.Style("width") = "100%"
                            clValue.Controls.Add(aAnchorShowValue)
                            clValue.Controls.Add(divShowValue)
                            rwValue.Cells.Add(clValue)
                            clValue = New HtmlTableCell
                            clValue.Attributes("align") = "right"
                            clValue.Controls.Add(aAnchorHideValue)
                            rwValue.Cells.Add(clValue)
                            tbValue.Rows.Add(rwValue)

                            hTCell.Controls.Add(tbValue)
                            intCountHigh += 1
                        End If

                        If Not editMode Or Not bolEditable Then ' En modo normal

                            hTCell.Style("padding") = "3px"

                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDatePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "DatePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If

                                Case FieldTypes.tTimePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "TimePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If

                                Case FieldTypes.tLink
                                    LinkField = CreaAnchorEnlace(intCountHigh, strBegin, strEnd)
                                Case FieldTypes.tDocument
                                    LinkField = CreaAnchorDocument(intCountHigh, intIdDocument)
                                Case Else
                            End Select

                            If oRow("AccessLevel") <> 2 Then
                                If strValue = "" Then
                                    hTCell.InnerText = " "
                                Else
                                    If LinkField Is Nothing Then
                                        hTCell.InnerHtml = strValue
                                    Else
                                        hTCell.Controls.Add(LinkField)
                                    End If
                                End If
                                ' Si tiene control de histórico muestra tooltip con la fecha de incio de validez
                                If roTypes.Any2Boolean(oRow("History")) And strValueDate <> "" Then
                                    Dim oParams As New Generic.List(Of String)
                                    oParams.Add(Format(CDate(oRow("Date")), HelperWeb.GetShortDateFormat))
                                    hTCell.Attributes.Add("title", Me.Language.Translate("FieldValueDate.Info", Me.DefaultScope, oParams))
                                End If
                            Else

                                If LinkField Is Nothing Then
                                    divShowValue.InnerHtml = strValue
                                Else
                                    divShowValue.Controls.Add(LinkField)
                                End If
                                ' Si tiene control de histórico muestra tooltip con la fecha de incio de validez
                                If roTypes.Any2Boolean(oRow("History")) And strValueDate <> "" Then
                                    Dim oParams As New Generic.List(Of String)
                                    oParams.Add(Format(CDate(oRow("Date")), HelperWeb.GetShortDateFormat))
                                    divShowValue.Attributes.Add("title", Me.Language.Translate("FieldValueDate.Info", Me.DefaultScope, oParams))
                                End If
                            End If

                            hTRow.Cells.Add(hTCell)

                            ' Pinta columna con link a edición histórico valores
                            Dim aAnchorHistory As HtmlAnchor = Nothing
                            If Not editMode And roTypes.Any2Boolean(oRow("History")) Then
                                aAnchorHistory = New HtmlAnchor
                                With aAnchorHistory
                                    .ID = "aHistory_" & intCountHigh.ToString
                                    .HRef = "javascript:void(0);"
                                    .Attributes("class") = "UserFieldShowHideValueAnchor"
                                    .Attributes("onclick") = "ShowUserFieldHistory('" & IDEmployee.ToString & "', '" & oRow("FieldCaption").ToString & "');"
                                    .InnerText = Me.Language.Translate("UserField.History.ShowCaption", Me.DefaultScope) ' "Histórico"
                                End With
                            End If
                            hTCell = New HtmlTableCell
                            hTCell.Attributes("class") = "GridStyleNext-cell" & altRow & " GridStyleNext-cellHistory"
                            hTCell.Attributes.Add("nowrap", "nowrap")
                            hTable.Style("border-left") = "none"
                            hTCell.Style("padding") = "3px"
                            If aAnchorHistory IsNot Nothing Then hTCell.Controls.Add(aAnchorHistory)
                            'If hTCell.InnerText = "" Then hTCell.InnerText = " "
                            If aAnchorHistory IsNot Nothing Then hTCell.Attributes("title") = Me.Language.Translate("UserField.History.ShowTitle", Me.DefaultScope) ' "Mostrar detalle histórico"
                            hTRow.Cells.Add(hTCell)
                        Else ' En modo edición

                            Dim strHtmlError As String = ""
                            Dim strEditCss As String = "textEdit x-form-text x-form-field"
                            hTCell.Style("padding") = "0px"
                            If arrErrors IsNot Nothing Then
                                'Si es comproben els errors
                                Dim dRows() As DataRow = arrErrors.Select("FieldName = '" & oRow(dColField).ToString & "'")
                                If dRows.Length > 0 Then
                                    strValue = roTypes.Any2String(dRows(0).Item("FieldValue"))
                                    strHtmlError = "<br />" &
                                                   "<span style=""padding-left: 5px; color: red"">" & dRows(0).Item("ErrorDescription").ToString & "</span>"
                                    strEditCss &= " textEditError"
                                    If roTypes.Any2String(dRows(0).Item("BeginEnd")) = "BeginPeriod" Then
                                        strBegin = dRows(0).Item("FieldValue")
                                    ElseIf roTypes.Any2String(dRows(0).Item("BeginEnd")) = "EndPeriod" Then
                                        strEnd = dRows(0).Item("FieldValue")
                                    End If
                                End If
                            End If

                            Dim oContainer As Object
                            If oRow("AccessLevel") <> 2 Then
                                oContainer = hTCell
                            Else
                                oContainer = divShowValue
                            End If

                            Dim cnListTable As HtmlTable = Nothing

                            Dim cInstanceNameStr As String = "euf_UserField" & fieldIndex
                            fieldIndex = fieldIndex + 1

                            Dim strOnChangeScript As String = "UserFieldValueChange('" & cInstanceNameStr & "', '" & Format(Now.Date, HelperWeb.GetShortDateFormat) & "');"

                            Dim htmlFieldObject As Object = Nothing

                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDate

                                    Dim dateDev As New DevExpress.Web.ASPxDateEdit
                                    dateDev.Theme = Me.oSelectedTheme
                                    dateDev.ID = cInstanceNameStr
                                    dateDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    dateDev.JSProperties.Add("cpIsEddited", False)
                                    dateDev.JSProperties.Add("cpObjectType", "date")
                                    dateDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    dateDev.ClientSideEvents.DateChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    dateDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    dateDev.Width = 400
                                    dateDev.Date = roTypes.Any2DateTime(strValue)
                                    htmlFieldObject = dateDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tTime

                                    Dim timeDev As New DevExpress.Web.ASPxTextBox
                                    timeDev.Theme = Me.oSelectedTheme
                                    timeDev.ID = cInstanceNameStr
                                    timeDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    timeDev.JSProperties.Add("cpIsEddited", False)
                                    timeDev.JSProperties.Add("cpObjectType", "text")
                                    timeDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    If strValue.StartsWith("00") Then strValue = "0" & strValue.Substring(2)
                                    timeDev.Value = strValue
                                    timeDev.Width = 400
                                    timeDev.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                                    timeDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    timeDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    htmlFieldObject = timeDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tText
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.Theme = Me.oSelectedTheme
                                    textDev.ID = cInstanceNameStr
                                    textDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    textDev.JSProperties.Add("cpIsEddited", False)
                                    textDev.JSProperties.Add("cpObjectType", "text")
                                    textDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    textDev.Text = strValue
                                    textDev.Width = 400
                                    textDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    textDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    htmlFieldObject = textDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tNumeric
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.Theme = Me.oSelectedTheme
                                    textDev.ID = cInstanceNameStr
                                    textDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    textDev.JSProperties.Add("cpIsEddited", False)
                                    textDev.JSProperties.Add("cpObjectType", "text")
                                    textDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    textDev.Text = strValue
                                    textDev.MaskSettings.Mask = "<-9999999..9999999>"
                                    textDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    textDev.Text = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                                    textDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    textDev.Width = 400
                                    htmlFieldObject = textDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tDecimal
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.Theme = Me.oSelectedTheme
                                    textDev.ID = cInstanceNameStr
                                    textDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    textDev.JSProperties.Add("cpIsEddited", False)
                                    textDev.JSProperties.Add("cpObjectType", "text")
                                    textDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    textDev.Text = strValue
                                    textDev.MaskSettings.Mask = "<-99999999999..99999999999>.<000000..999999>"
                                    textDev.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
                                    textDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    textDev.Text = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                                    textDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    textDev.Width = 400
                                    htmlFieldObject = textDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tList
                                    Dim cmbDev As New DevExpress.Web.ASPxComboBox
                                    cmbDev.Theme = Me.oSelectedTheme
                                    cmbDev.ID = cInstanceNameStr
                                    cmbDev.ClearButton.Enabled = True
                                    cmbDev.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
                                    cmbDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    cmbDev.JSProperties.Add("cpIsEddited", False)
                                    cmbDev.JSProperties.Add("cpObjectType", "combo")
                                    cmbDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    Dim oUserFieldInfo As roUserField = API.UserFieldServiceMethods.GetUserField(Me, oRow("FieldCaption").ToString, Types.EmployeeField, False, False)
                                    Dim selIndex As Integer = 0
                                    If oUserFieldInfo IsNot Nothing Then
                                        For Each strItem As String In oUserFieldInfo.ListValues
                                            cmbDev.Items.Add(New DevExpress.Web.ListEditItem(strItem, strItem))
                                            If (strItem = strValue) Then
                                                cmbDev.SelectedIndex = selIndex
                                            End If
                                            selIndex = selIndex + 1
                                        Next
                                    End If
                                    cmbDev.ClientSideEvents.SelectedIndexChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    cmbDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    cmbDev.Width = 400
                                    htmlFieldObject = cmbDev

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                    'Cambio de combobox a tokenbox para habilitar multiselect
                                    'Dim cmbDev As New DevExpress.Web.ASPxTokenBox
                                    'cmbDev.ID = cInstanceNameStr
                                    'cmbDev.Width = "80%"
                                    'cmbDev.ClientInstanceName = cInstanceNameStr & "_Client"
                                    'cmbDev.JSProperties.Add("cpIsEddited", False)
                                    'cmbDev.JSProperties.Add("cpObjectType", "combo")
                                    'cmbDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString)
                                    'Dim oUserFieldInfo As UserFieldService.roUserField = API.UserFieldServiceMethods.GetUserField(Me, oRow("FieldCaption").ToString, UserFieldService.Types.EmployeeField, False)
                                    'If oUserFieldInfo IsNot Nothing Then
                                    '    For Each strItem As String In oUserFieldInfo.ListValues
                                    '        cmbDev.Items.Add(strItem, strItem)
                                    '    Next
                                    '    If (strValue <> String.Empty) Then
                                    '        For Each oSelItem As String In strValue.Split(";")
                                    '            cmbDev.Tokens.Add(oSelItem)
                                    '        Next
                                    '    End If
                                    'End If
                                    'cmbDev.ClientSideEvents.TokensChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"

                                    'htmlFieldObject = cmbDev
                                    'Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_Client#"

                                Case FieldTypes.tDatePeriod
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    Dim Cell1 As New HtmlTableCell
                                    Cell1.Style.Add("padding-right", "3px")
                                    Cell1.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.BeginTitle", Me.DefaultScope)
                                    periodTableRow.Controls.Add(Cell1)

                                    Dim Cell2 As New HtmlTableCell

                                    Dim startDev As New DevExpress.Web.ASPxDateEdit
                                    startDev.Theme = Me.oSelectedTheme
                                    startDev.ID = cInstanceNameStr & "_##BeginPeriod##"
                                    startDev.ClientInstanceName = cInstanceNameStr & "_BeginPeriod_Client"
                                    startDev.JSProperties.Add("cpIsEddited", False)
                                    startDev.JSProperties.Add("cpObjectType", "dateP")
                                    startDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##BeginPeriod##")
                                    startDev.ClientSideEvents.DateChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                    startDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    startDev.Width = 200
                                    If strBegin <> String.Empty Then
                                        startDev.Date = roTypes.Any2DateTime(strBegin)
                                    End If
                                    Cell2.Controls.Add(startDev)
                                    periodTableRow.Controls.Add(Cell2)

                                    Dim Cell3 As New HtmlTableCell
                                    Cell3.Style.Add("padding-right", "3px")
                                    Cell3.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.EndTitle", Me.DefaultScope)
                                    periodTableRow.Controls.Add(Cell3)

                                    Dim Cell4 As New HtmlTableCell
                                    Dim endDev As New DevExpress.Web.ASPxDateEdit
                                    endDev.Theme = Me.oSelectedTheme
                                    endDev.ID = cInstanceNameStr & "_##EndPeriod##"
                                    endDev.ClientInstanceName = cInstanceNameStr & "_EndPeriod_Client"
                                    endDev.JSProperties.Add("cpIsEddited", False)
                                    endDev.JSProperties.Add("cpObjectType", "dateP")
                                    endDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##EndPeriod##")
                                    endDev.ClientSideEvents.DateChanged = "function(s,e){" & strOnChangeScript & ";" & cInstanceNameStr & "_BeginPeriod_Client.cpIsEddited = true; s.cpIsEddited = true; }"
                                    endDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                    endDev.Width = 200
                                    If strEnd <> String.Empty Then
                                        endDev.Date = roTypes.Any2DateTime(strEnd)
                                    End If
                                    Cell4.Controls.Add(endDev)
                                    periodTableRow.Controls.Add(Cell4)

                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_BeginPeriod_Client#"

                                Case FieldTypes.tTimePeriod
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    With periodTableRow
                                        Dim Cell1 As New HtmlTableCell
                                        Cell1.Style.Add("padding-right", "3px")
                                        Cell1.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.BeginTitle", Me.DefaultScope)
                                        .Controls.Add(Cell1)

                                        Dim Cell2 As New HtmlTableCell
                                        Dim startDev As New DevExpress.Web.ASPxTimeEdit
                                        startDev.Theme = Me.oSelectedTheme
                                        startDev.ID = cInstanceNameStr & "_##BeginPeriod##"
                                        startDev.ClientInstanceName = cInstanceNameStr & "_BeginPeriod_Client"
                                        startDev.JSProperties.Add("cpIsEddited", False)
                                        startDev.JSProperties.Add("cpObjectType", "timeP")
                                        startDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##BeginPeriod##")
                                        startDev.EditFormat = DevExpress.Web.EditFormat.Time
                                        startDev.DisplayFormatString = "HH:mm"
                                        startDev.ClientSideEvents.DateChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true;}"
                                        startDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                        startDev.Width = 200
                                        startDev.DateTime = roTypes.Any2DateTime(strBegin)
                                        Cell2.Controls.Add(startDev)
                                        .Controls.Add(Cell2)

                                        Dim Cell3 As New HtmlTableCell
                                        Cell3.Style.Add("padding-right", "3px")
                                        Cell3.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.EndTitle", Me.DefaultScope)
                                        .Controls.Add(Cell3)

                                        Dim Cell4 As New HtmlTableCell
                                        Dim endDev As New DevExpress.Web.ASPxTimeEdit
                                        endDev.Theme = Me.oSelectedTheme
                                        endDev.ID = cInstanceNameStr & "_##EndPeriod##"
                                        endDev.ClientInstanceName = cInstanceNameStr & "_EndPeriod_Client"
                                        endDev.JSProperties.Add("cpIsEddited", False)
                                        endDev.JSProperties.Add("cpObjectType", "timeP")
                                        endDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##EndPeriod##")
                                        endDev.EditFormat = DevExpress.Web.EditFormat.Time
                                        endDev.DisplayFormatString = "HH:mm"
                                        endDev.ClientSideEvents.DateChanged = "function(s,e){" & strOnChangeScript & ";" & cInstanceNameStr & "_BeginPeriod_Client.cpIsEddited = true; s.cpIsEddited = true; }"
                                        endDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                        endDev.Width = 200
                                        endDev.DateTime = roTypes.Any2DateTime(strEnd)
                                        Cell4.Controls.Add(endDev)
                                        .Controls.Add(Cell4)
                                    End With
                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_BeginPeriod_Client#"
                                Case FieldTypes.tLink
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    With periodTableRow
                                        Dim Cell1 As New HtmlTableCell
                                        Cell1.Style.Add("padding-right", "3px")
                                        Cell1.InnerText = Me.Language.Translate("Link.FieldEdit.LinkShow", Me.DefaultScope)
                                        .Controls.Add(Cell1)

                                        Dim Cell2 As New HtmlTableCell
                                        Dim textDev As New DevExpress.Web.ASPxTextBox
                                        textDev.Theme = Me.oSelectedTheme
                                        textDev.ID = cInstanceNameStr & "_##LinkShow##"
                                        textDev.ClientInstanceName = cInstanceNameStr & "_LinkShow_Client"
                                        textDev.JSProperties.Add("cpIsEddited", False)
                                        textDev.JSProperties.Add("cpObjectType", "link")
                                        textDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##LinkShow##")
                                        textDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; s.cpIsEddited = true; }"
                                        textDev.Text = strBegin
                                        textDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                        textDev.Width = 400
                                        Cell2.Controls.Add(textDev)
                                        .Controls.Add(Cell2)

                                        Dim Cell3 As New HtmlTableCell
                                        Cell3.Style.Add("padding-right", "3px")
                                        Cell3.InnerText = Me.Language.Translate("Link.FieldEdit.LinkReal", Me.DefaultScope)
                                        .Controls.Add(Cell3)

                                        Dim Cell4 As New HtmlTableCell
                                        Dim realDev As New DevExpress.Web.ASPxTextBox
                                        realDev.Theme = Me.oSelectedTheme
                                        realDev.ID = cInstanceNameStr & "_##LinkReal##"
                                        realDev.ClientInstanceName = cInstanceNameStr & "_LinkReal_Client"
                                        realDev.JSProperties.Add("cpIsEddited", False)
                                        realDev.JSProperties.Add("cpObjectType", "link")
                                        realDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_##LinkReal##")
                                        realDev.ClientSideEvents.TextChanged = "function(s,e){" & strOnChangeScript & "; " & cInstanceNameStr & "_LinkShow_Client.cpIsEddited = true; s.cpIsEddited = true; }"
                                        realDev.Text = strEnd
                                        realDev.ClientEnabled = Not roTypes.Any2Boolean(oRow("ReadOnly"))
                                        realDev.Width = 400
                                        Cell4.Controls.Add(realDev)
                                        .Controls.Add(Cell4)
                                    End With
                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_LinkShow_Client#"
                                Case FieldTypes.tDocument
                                    Dim aAnchorShowValue As New Label
                                    aAnchorShowValue.Text = Me.Language.Translate("UserFields.DocumentUserField.Edit", Me.DefaultScope)
                                    htmlFieldObject = aAnchorShowValue

                                    Me.hdnEmployeeFieldsIDs.Value &= cInstanceNameStr & "_LinkShow_Client#"
                            End Select

                            Dim endHtmlTable As New HtmlTable
                            endHtmlTable.Style.Add("width", "100%")

                            Dim endHtmlRow As New HtmlTableRow

                            'Dim strInnerHtml As String = "<table style=""width:100%;""><tr>"
                            If htmlFieldObject Is Nothing Then
                                htmlFieldObject = New HtmlGenericControl
                            End If
                            If roTypes.Any2Boolean(oRow("History")) = False Then
                                Dim endHtmlTableCell As New HtmlTableCell
                                endHtmlTableCell.ColSpan = 3
                                endHtmlTableCell.Controls.Add(htmlFieldObject)
                                endHtmlRow.Controls.Add(endHtmlTableCell)
                                'strInnerHtml &= "<td colspan=""3"">" & oContainer.InnerHtml & "</td>"
                            Else
                                Dim endHtmlTableCell As New HtmlTableCell
                                endHtmlTableCell.Controls.Add(htmlFieldObject)

                                Dim endHtmlTableCell2 As New HtmlTableCell
                                endHtmlTableCell2.Align = "right"

                                Dim innerTable As New HtmlTable

                                Dim innerTableRow As New HtmlTableRow

                                Dim innerTableCell As New HtmlTableCell
                                innerTableCell.Style.Add("padding-left", "5px")
                                innerTableCell.InnerText = Me.Language.Translate("FieldValueDate.Label", Me.DefaultScope)

                                Dim innerTableCell2 As New HtmlTableCell
                                innerTableCell2.Style.Add("padding-left", "2px")

                                Dim dateDev As New DevExpress.Web.ASPxDateEdit
                                dateDev.Theme = Me.oSelectedTheme
                                dateDev.ID = cInstanceNameStr & "_@@Date@@"
                                dateDev.ClientInstanceName = cInstanceNameStr & "_Date_Client"
                                dateDev.JSProperties.Add("cpIsEddited", False)
                                dateDev.JSProperties.Add("cpOriginalFieldName", oRow(dColField).ToString & "_@@Date@@")
                                dateDev.ClientSideEvents.DateChanged = "function(s,e){ s.cpIsEddited = true; }"
                                dateDev.AllowNull = False
                                dateDev.Width = 150
                                dateDev.Date = If(strValue <> String.Empty, roTypes.Any2DateTime(strValueDate), DateTime.Now.Date)
                                dateDev.Visible = IIf(roTypes.Any2Boolean(oRow("History")), "true", "false")
                                innerTableCell2.Controls.Add(dateDev)

                                innerTableRow.Controls.Add(innerTableCell)
                                innerTableRow.Controls.Add(innerTableCell2)
                                innerTable.Controls.Add(innerTableRow)

                                endHtmlTableCell2.Controls.Add(innerTable)

                                endHtmlRow.Controls.Add(endHtmlTableCell)
                                endHtmlRow.Controls.Add(endHtmlTableCell2)
                            End If
                            endHtmlTable.Controls.Add(endHtmlRow)
                            oContainer.controls.add(endHtmlTable)
                            'oContainer = endHtmlTable

                            If strHtmlError <> "" Then
                                Dim tbValue As HtmlTable = hTCell.Controls(0)
                                Dim rwValue As New HtmlTableRow
                                Dim clValue As New HtmlTableCell
                                clValue.InnerHtml = strHtmlError
                                rwValue.Cells.Add(clValue)
                                tbValue.Rows.Add(rwValue)

                            End If

                        End If

                        ' Cerramos la fila
                        hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyleNext-endcell" & altRow
                        hTCell.ColSpan = 1
                        hTRow.Cells.Add(hTCell)

                        hTable.Rows.Add(hTRow)

                    Next

                End If

            Next

        End If

        Return hTable

    End Function

    Private Function IsValidFileNameOrPath(ByVal strPath As String) As Boolean

        Dim bRet As Boolean = True

        Try

            If strPath IsNot Nothing Then

                For Each badChar As Char In System.IO.Path.GetInvalidPathChars
                    If InStr(strPath, badChar) > 0 Then
                        bRet = False
                    End If
                Next

                'obtener nombre
                Dim strName As String = System.IO.Path.GetFileName(strPath)
                For Each badChar As Char In System.IO.Path.GetInvalidFileNameChars
                    If InStr(strName, badChar) > 0 Then
                        bRet = False
                    End If
                Next

            End If
        Catch ex As Exception

        End Try

        Return bRet

    End Function

    Private Function CreaAnchorDocument(ByVal intCountHigh As Integer, ByVal intIdDocument As Integer) As HtmlAnchor
        If intIdDocument > 0 Then
            Dim aAnchorShowValue As New HtmlAnchor
            With aAnchorShowValue
                .ID = "UserLink_" & intCountHigh.ToString
                .HRef = "./DocumentVisualize.aspx?DeliveredDocument=" & intIdDocument
                .Attributes("class") = ""
                '.Attributes("ValueLink") = ValueLink
                .Target = "_blank"
                .InnerHtml = Me.Language.Translate("UserField.DownloadDocuments", Me.DefaultScope)
            End With
            Return aAnchorShowValue
        Else
            Return Nothing
        End If
    End Function

    Private Function CreaAnchorEnlace(ByVal intCountHigh As Integer, ByVal ValueLinkShow As String, ByVal ValueLink As String) As HtmlAnchor

        If Not IsValidFileNameOrPath(ValueLink) Then
            Return Nothing
        End If

        If ValueLink <> String.Empty Then
            Dim aAnchorShowValue As New HtmlAnchor
            With aAnchorShowValue
                .ID = "UserLink_" & intCountHigh.ToString
                .HRef = "./EmployeeDownloadFile.aspx?Document=" & ValueLink
                .Attributes("class") = ""
                '.Attributes("ValueLink") = ValueLink
                .Target = "_blank"
                If ValueLinkShow <> String.Empty Then
                    .InnerHtml = ValueLinkShow
                Else
                    .InnerHtml = ValueLink
                End If
            End With
            Return aAnchorShowValue
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Retorna un Datable amb les dades de indicadors
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IndicatorsData(ByVal IDGroup As Integer) As DataTable
        Try
            Dim tb As DataTable
            tb = API.EmployeeGroupsServiceMethods.GetIndicatorsDataset(Me, IDGroup).Tables(0)

            Return tb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Indicadors
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridIndicators(ByVal dTable As DataTable, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            If dTblControls IsNot Nothing Then
                hTRow = New HtmlTableRow

                For Each oRow As DataRow In dTblControls.Rows
                    If oRow("Visible") Then
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cellheader"
                        hTCell.InnerHtml = oRow("NomHeader")
                        hTRow.Cells.Add(hTCell)
                    End If
                Next
                hTable.Rows.Add(hTRow)
            End If

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString

                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Or y = 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un Datatable amb els Usuaris del grup actuals
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CurrentEmployeesData(ByVal IDGroup As Integer) As DataTable
        Try
            Dim tb As DataTable = Nothing
            Dim ds As DataSet = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroup(Me, IDGroup, API.EmployeeGroupsServiceMethods.eEmployeesFromGroup.Current, EmployeeFeatureAlias, , True)
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                tb = ds.Tables(0)
            End If

            Return tb
        Catch ex As Exception
            'TODO: Retornar missatge error
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un Datatable amb els Usuaris en transit
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FutureEmployeesData(ByVal IDGroup As Integer) As DataTable
        Try
            Dim tb As DataTable = Nothing
            Dim ds As DataSet = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroup(Me, IDGroup, API.EmployeeGroupsServiceMethods.eEmployeesFromGroup.Future, EmployeeFeatureAlias)
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                tb = ds.Tables(0)
            End If

            Return tb
        Catch ex As Exception
            'TODO: Retornar missatge error
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un DataTable amb els usuaris pasats
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function OldEmployeesData(ByVal IDGroup As Integer) As DataTable
        Try
            Dim tb As DataTable = Nothing
            Dim ds As DataSet = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroup(Me, IDGroup, API.EmployeeGroupsServiceMethods.eEmployeesFromGroup.Old, EmployeeFeatureAlias, , True)
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                tb = ds.Tables(0)
            End If

            Return tb
        Catch ex As Exception
            'TODO: Retornar missatge error
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea grid de campos
    ''' </summary>
    ''' <param name="dTable">DataTable amb les dades del usuari</param>
    ''' <param name="dColField">Columna que servira per asignar l'ID al input text i que no es pintara</param>
    ''' <param name="editMode">Si pintara els input text</param>
    ''' <param name="arrErrors">Datatable amb els error que pintara</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaFieldsGrid(ByVal dTable As DataTable, ByVal ColumnNames() As String, Optional ByVal dColField As String = "",
                                    Optional ByVal editMode As Boolean = False, Optional ByVal arrErrors As DataTable = Nothing) As HtmlTable
        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        hTable.Attributes("class") = "GridStyle GridEmpleados"

        'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Width = "250"
        hTCell.Attributes("style") = "border-right: 0; width: 250px"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Field", Me.DefaultScope) ' "Campo"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Value", Me.DefaultScope) '"Valor"

        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim Categories() As String = API.UserFieldServiceMethods.GetCategories(Me, True)
        If Categories IsNot Nothing Then

            ReDim Preserve Categories(Categories.Length)
            Categories(Categories.Length - 1) = ""

            Dim Rows() As DataRow
            Dim bolEditable As Boolean = False

            Dim intCountHigh As Integer = 0

            For Each strCategory As String In Categories

                ' Obtenemos los campos correspondientes a la categoría
                Rows = dTable.Select("Category = '" & strCategory.Replace("'", "''") & "'" & IIf(strCategory = "", " OR Category IS NULL", ""), "FieldCaption")

                If Rows.Length > 0 Then

                    ' Pinta la fila con el nombre de la categoría actual
                    hTRow = New HtmlTableRow
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "GridStyle-cell3"
                    hTCell.Style("padding") = "15px"
                    hTCell.ColSpan = 2
                    If strCategory <> "" Then
                        hTCell.InnerText = strCategory
                    Else
                        hTCell.InnerText = Me.Language.Translate("UserField.Category.None", Me.DefaultScope)
                    End If
                    hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell3"
                    hTRow.Cells.Add(hTCell)
                    hTable.Rows.Add(hTRow)

                    ' Bucle por los campos de la categoría actual
                    For Each oRow As DataRow In Rows

                        hTRow = New HtmlTableRow
                        altRow = IIf(altRow = "1", "2", "1")

                        ' Pinta columna nombre campo
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cell" & altRow
                        hTCell.Attributes.Add("nowrap", "nowrap")
                        hTCell.Style("padding") = "3px"
                        hTCell.InnerText = oRow("FieldCaption").ToString
                        If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                        Dim strDescriptionText As String = roTypes.Any2String(oRow("Description"))
                        If strDescriptionText.StartsWith("Info.") Then
                            hTCell.Attributes("title") = Me.Language.Translate(strDescriptionText, Me.DefaultScope)
                        Else
                            hTCell.Attributes("title") = strDescriptionText
                        End If

                        hTRow.Cells.Add(hTCell)

                        ' Pinta columna valor
                        Dim strValue As String = roTypes.Any2String(oRow("Value"))

                        Dim strBegin As String = ""
                        Dim strEnd As String = ""
                        Dim strLanguageKey As String = ""
                        Dim xDate As Date
                        Dim LinkField As HtmlAnchor = Nothing

                        Select Case CType(oRow("Type"), FieldTypes)
                            Case FieldTypes.tDecimal
                                If strValue <> "" Then
                                    strValue = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat())
                                End If
                            Case FieldTypes.tDate
                                If oRow("ValueDateTime").ToString <> "" Then
                                    strValue = Format(CDate(oRow("ValueDateTime").ToString), HelperWeb.GetShortDateFormat)
                                End If

                            Case FieldTypes.tTime
                                If strValue <> "" AndAlso IsNumeric(strValue) Then
                                    strValue = roConversions.ConvertHoursToTime(CDbl(strValue))
                                End If

                            Case FieldTypes.tDatePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(0).Substring(0, 4), strValue.Split("*")(0).Substring(5, 2), strValue.Split("*")(0).Substring(8, 2))
                                    strBegin = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(1).Substring(0, 4), strValue.Split("*")(1).Substring(5, 2), strValue.Split("*")(1).Substring(8, 2))
                                    strEnd = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If

                            Case FieldTypes.tTimePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(0).Substring(0, 2), strValue.Split("*")(0).Substring(3, 2), 0)
                                    strBegin = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(1).Substring(0, 2), strValue.Split("*")(1).Substring(3, 2), 0)
                                    strEnd = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If

                            Case FieldTypes.tLink
                                If strValue.Split("*")(0) <> "" Then
                                    strBegin = strValue.Split("*")(0) 'Texto del enlace a mostrar
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1) <> "" Then
                                    strEnd = strValue.Split("*")(1) 'Texto del enlace real
                                End If

                            Case Else
                        End Select

                        Select Case CType(oRow("AccessLevel"), Integer)
                            Case 0 ' Low
                                bolEditable = (Me.oUserFieldsAccessPermission(0) >= Permission.Write)
                            Case 1 ' Medium
                                bolEditable = (Me.oUserFieldsAccessPermission(1) >= Permission.Write)
                            Case 2 ' High
                                bolEditable = (Me.oUserFieldsAccessPermission(2) >= Permission.Write)
                            Case Else
                                bolEditable = False
                        End Select

                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cell" & altRow

                        Dim divShowValue As HtmlGenericControl = Nothing

                        If oRow("AccessLevel") = 2 Then
                            Dim tbValue As New HtmlTable
                            Dim rwValue As New HtmlTableRow
                            Dim clValue As New HtmlTableCell

                            Dim aAnchorShowValue As New HtmlAnchor
                            With aAnchorShowValue
                                .ID = "aShowValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "ShowUserFieldValue(this, '" & intCountHigh & "','0','" & oRow("FieldCaption").ToString & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.ShowCaption", Me.DefaultScope) ' "Mostrar valor ..."
                            End With

                            Dim aAnchorHideValue As New HtmlAnchor
                            With aAnchorHideValue
                                .ID = "aHideValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Style("display") = "none"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "HideUserFieldValue(this, '" & intCountHigh & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.HideCaption", Me.DefaultScope) ' "Ocultar valor ..."
                            End With

                            divShowValue = New HtmlGenericControl("div")
                            With divShowValue
                                .ID = "divShowValue_" & intCountHigh.ToString
                                .Attributes("class") = "UserFieldShowHideValueDiv"
                                .Style("text-align") = "left"
                                .Style("width") = "100%"
                                .Style("display") = "none"
                            End With

                            tbValue.Style("width") = "100%"
                            clValue.Controls.Add(aAnchorShowValue)
                            clValue.Controls.Add(divShowValue)
                            rwValue.Cells.Add(clValue)
                            clValue = New HtmlTableCell
                            clValue.Attributes("align") = "right"
                            clValue.Controls.Add(aAnchorHideValue)
                            rwValue.Cells.Add(clValue)
                            tbValue.Rows.Add(rwValue)

                            hTCell.Controls.Add(tbValue)
                            intCountHigh += 1
                        End If

                        If Not editMode Or Not bolEditable Then ' En modo normal

                            hTCell.Style("padding") = "3px"

                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDatePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "DatePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If

                                Case FieldTypes.tTimePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "TimePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If

                                Case FieldTypes.tLink
                                    LinkField = CreaAnchorEnlace(intCountHigh, strBegin, strEnd)

                                Case Else
                            End Select

                            If oRow("AccessLevel") <> 2 Then
                                If strValue = "" Then
                                    hTCell.InnerText = " "
                                Else
                                    If LinkField Is Nothing Then
                                        hTCell.InnerHtml = strValue
                                    Else
                                        hTCell.Controls.Add(LinkField)
                                    End If
                                End If
                            Else

                                If LinkField Is Nothing Then
                                    divShowValue.InnerHtml = strValue
                                Else
                                    divShowValue.Controls.Add(LinkField)
                                End If
                            End If

                            hTRow.Cells.Add(hTCell)
                        Else ' En modo edición

                            Dim strHtmlError As String = ""
                            Dim strEditCss As String = "textEdit x-form-text x-form-field"
                            hTCell.Style("padding") = "0px"
                            If arrErrors IsNot Nothing Then
                                'Si es comproben els errors
                                Dim dRows() As DataRow = arrErrors.Select("FieldName = '" & oRow(dColField).ToString & "'")
                                If dRows.Length > 0 Then
                                    strValue = roTypes.Any2String(dRows(0).Item("FieldValue"))
                                    strHtmlError = "<br />" &
                                                   "<span style=""padding-left: 5px; color: red"">" & dRows(0).Item("ErrorDescription").ToString & "</span>"
                                    strEditCss &= " textEditError"
                                    If roTypes.Any2String(dRows(0).Item("BeginEnd")) = "BeginPeriod" Then
                                        strBegin = dRows(0).Item("FieldValue")
                                    ElseIf roTypes.Any2String(dRows(0).Item("BeginEnd")) = "EndPeriod" Then
                                        strEnd = dRows(0).Item("FieldValue")
                                    End If
                                End If
                            End If

                            Dim oContainer As Object
                            If oRow("AccessLevel") <> 2 Then
                                oContainer = hTCell
                            Else
                                oContainer = divShowValue
                            End If

                            Dim htmlFieldObject As Object = Nothing

                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDate

                                    Dim dateDev As New DevExpress.Web.ASPxDateEdit
                                    dateDev.ID = oRow(dColField).ToString
                                    dateDev.Theme = Me.oSelectedTheme
                                    dateDev.Width = 400
                                    dateDev.Date = roTypes.Any2DateTime(strValue)
                                    htmlFieldObject = dateDev
                                    'oContainer.InnerHtml = "<input type=""text"" id=""" & oRow(dColField).ToString & """ value=""" & strValue & """ style=""width:75px;"" ConvertControl=""DatePicker"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tTime
                                    Dim timeDev As New DevExpress.Web.ASPxTextBox
                                    timeDev.ID = oRow(dColField).ToString
                                    timeDev.Text = strValue
                                    timeDev.Theme = Me.oSelectedTheme
                                    timeDev.Width = 400
                                    timeDev.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                                    htmlFieldObject = timeDev
                                    'oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """ style=""width:99%"" onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""TextField"" CCregex=""/^([-]?[0-9]?[0-9]?[0-9]?[0-9]):([0-5][0-9])$/"" CCmaxLength=""7"" CCtime=""false"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tText
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.ID = oRow(dColField).ToString
                                    textDev.Text = strValue
                                    textDev.Theme = Me.oSelectedTheme
                                    textDev.Width = 400
                                    htmlFieldObject = textDev
                                    'oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""TextField"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tNumeric
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.ID = oRow(dColField).ToString
                                    textDev.Text = strValue
                                    textDev.Theme = Me.oSelectedTheme
                                    textDev.Width = 400
                                    textDev.MaskSettings.Mask = "<0..9999999>"
                                    textDev.Text = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                                    htmlFieldObject = textDev
                                    'oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""NumberField"" CCallowDecimals=""false"" CCmaxValue=""2147483647"" CCminValue=""-2147483648"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tDecimal
                                    Dim textDev As New DevExpress.Web.ASPxTextBox
                                    textDev.ID = oRow(dColField).ToString
                                    textDev.Text = strValue
                                    textDev.MaskSettings.Mask = "<-99999999999..99999999999>.<000..999>"
                                    textDev.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
                                    textDev.Text = strValue.Replace(".", HelperWeb.GetDecimalDigitFormat)
                                    htmlFieldObject = textDev
                                    'oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue.Replace(HelperWeb.GetDecimalDigitFormat, ".") & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""NumberField"" CCallowDecimals=""true"" CCdecimalPrecision=""3"" CCmaxValueText=""16"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tList
                                    Dim cmbDev As New DevExpress.Web.ASPxComboBox
                                    cmbDev.ID = oRow(dColField).ToString
                                    Dim oUserFieldInfo As roUserField = API.UserFieldServiceMethods.GetUserField(Me, oRow("FieldCaption").ToString, Types.GroupField, False, False)
                                    Dim selIndex As Integer = 0
                                    If oUserFieldInfo IsNot Nothing Then
                                        For Each strItem As String In oUserFieldInfo.ListValues
                                            cmbDev.Items.Add(New DevExpress.Web.ListEditItem(strItem, strItem))
                                            If (strItem = strValue) Then
                                                cmbDev.SelectedIndex = selIndex
                                            End If
                                            selIndex = selIndex + 1
                                        Next
                                    End If
                                    cmbDev.Theme = Me.oSelectedTheme
                                    cmbDev.Width = 400
                                    cmbDev.ClientInstanceName = oRow(dColField).ToString

                                    htmlFieldObject = cmbDev
                                Case FieldTypes.tDatePeriod
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    Dim Cell1 As New HtmlTableCell
                                    Cell1.Style.Add("padding-right", "3px")
                                    Cell1.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.BeginTitle", Me.DefaultScope)
                                    periodTableRow.Controls.Add(Cell1)

                                    Dim Cell2 As New HtmlTableCell

                                    Dim startDev As New DevExpress.Web.ASPxDateEdit
                                    startDev.ID = oRow(dColField).ToString & "_##BeginPeriod##"
                                    startDev.Theme = Me.oSelectedTheme
                                    startDev.Width = 200
                                    If strBegin <> String.Empty Then
                                        startDev.Date = roTypes.Any2DateTime(strBegin)
                                    End If
                                    Cell2.Controls.Add(startDev)
                                    periodTableRow.Controls.Add(Cell2)

                                    Dim Cell3 As New HtmlTableCell
                                    Cell3.Style.Add("padding-right", "3px")
                                    Cell3.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.EndTitle", Me.DefaultScope)
                                    periodTableRow.Controls.Add(Cell3)

                                    Dim Cell4 As New HtmlTableCell
                                    Dim endDev As New DevExpress.Web.ASPxDateEdit
                                    endDev.ID = oRow(dColField).ToString & "_##EndPeriod##"
                                    endDev.Theme = Me.oSelectedTheme
                                    endDev.Width = 200
                                    If strEnd <> String.Empty Then
                                        endDev.Date = roTypes.Any2DateTime(strEnd)
                                    End If
                                    Cell4.Controls.Add(endDev)
                                    periodTableRow.Controls.Add(Cell4)

                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                                Case FieldTypes.tTimePeriod
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    With periodTableRow
                                        Dim Cell1 As New HtmlTableCell
                                        Cell1.Style.Add("padding-right", "3px")
                                        Cell1.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.BeginTitle", Me.DefaultScope)
                                        .Controls.Add(Cell1)

                                        Dim Cell2 As New HtmlTableCell
                                        Dim startDev As New DevExpress.Web.ASPxTimeEdit
                                        startDev.ID = oRow(dColField).ToString & "_##BeginPeriod##"
                                        startDev.EditFormat = DevExpress.Web.EditFormat.Time
                                        startDev.DisplayFormatString = "HH:mm"
                                        startDev.Theme = Me.oSelectedTheme
                                        startDev.Width = 200
                                        startDev.DateTime = roTypes.Any2DateTime(strBegin)
                                        Cell2.Controls.Add(startDev)
                                        .Controls.Add(Cell2)

                                        Dim Cell3 As New HtmlTableCell
                                        Cell3.Style.Add("padding-right", "3px")
                                        Cell3.InnerText = Me.Language.Translate("DatePeriod.FieldEdit.EndTitle", Me.DefaultScope)
                                        .Controls.Add(Cell3)

                                        Dim Cell4 As New HtmlTableCell
                                        Dim endDev As New DevExpress.Web.ASPxTimeEdit
                                        endDev.ID = oRow(dColField).ToString & "_##EndPeriod##"
                                        endDev.EditFormat = DevExpress.Web.EditFormat.Time
                                        endDev.DisplayFormatString = "HH:mm"
                                        endDev.Theme = Me.oSelectedTheme
                                        endDev.Width = 200
                                        endDev.DateTime = roTypes.Any2DateTime(strEnd)
                                        Cell4.Controls.Add(endDev)
                                        .Controls.Add(Cell4)
                                    End With
                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                                Case FieldTypes.tLink
                                    Dim periodTable As New HtmlTable

                                    Dim periodTableRow As New HtmlTableRow

                                    With periodTableRow
                                        Dim Cell1 As New HtmlTableCell
                                        Cell1.Style.Add("padding-right", "3px")
                                        Cell1.InnerText = Me.Language.Translate("Link.FieldEdit.LinkShow", Me.DefaultScope)
                                        .Controls.Add(Cell1)

                                        Dim Cell2 As New HtmlTableCell
                                        Dim textDev As New DevExpress.Web.ASPxTextBox
                                        textDev.ID = oRow(dColField).ToString & "_##LinkShow##"
                                        textDev.Text = strBegin
                                        textDev.Theme = Me.oSelectedTheme
                                        textDev.Width = 200
                                        Cell2.Controls.Add(textDev)
                                        .Controls.Add(Cell2)

                                        Dim Cell3 As New HtmlTableCell
                                        Cell3.Style.Add("padding-right", "3px")
                                        Cell3.InnerText = Me.Language.Translate("Link.FieldEdit.LinkReal", Me.DefaultScope)
                                        .Controls.Add(Cell3)

                                        Dim Cell4 As New HtmlTableCell
                                        Dim realDev As New DevExpress.Web.ASPxTextBox
                                        realDev.ID = oRow(dColField).ToString & "_##LinkReal##"
                                        realDev.Text = strEnd
                                        realDev.Theme = Me.oSelectedTheme
                                        realDev.Width = 200
                                        Cell4.Controls.Add(realDev)
                                        .Controls.Add(Cell4)
                                    End With
                                    periodTable.Controls.Add(periodTableRow)
                                    htmlFieldObject = periodTable

                            End Select

                            Dim endHtmlTable As New HtmlTable
                            endHtmlTable.Style.Add("width", "100%")

                            Dim endHtmlRow As New HtmlTableRow

                            'Dim strInnerHtml As String = "<table style=""width:100%;""><tr>"
                            If htmlFieldObject Is Nothing Then
                                htmlFieldObject = New HtmlGenericControl
                            End If

                            Dim endHtmlTableCell As New HtmlTableCell
                            endHtmlTableCell.ColSpan = 3
                            endHtmlTableCell.Controls.Add(htmlFieldObject)
                            endHtmlRow.Controls.Add(endHtmlTableCell)

                            endHtmlTable.Controls.Add(endHtmlRow)
                            oContainer.controls.add(endHtmlTable)
                            If strHtmlError <> "" Then
                                Dim tbValue As HtmlTable = hTCell.Controls(0)
                                Dim rwValue As New HtmlTableRow
                                Dim clValue As New HtmlTableCell
                                clValue.InnerHtml = strHtmlError
                                rwValue.Cells.Add(clValue)
                                tbValue.Rows.Add(rwValue)
                            End If
                        End If

                        ' Cerramos la fila
                        hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                        hTRow.Cells.Add(hTCell)

                        hTable.Rows.Add(hTRow)

                    Next

                End If

            Next

        End If

        Return hTable

    End Function

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n) '"GridStyle-cellheader"
                hTCell.InnerHtml = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerText = " "
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            'Celda d'espai per edicio
            hTCell = New HtmlTableCell
            hTCell.InnerText = " "
            hTCell.Width = "40px"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Ausencias
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridLists(ByVal dTable As DataTable, Optional ByVal nomCols() As String = Nothing, Optional ByVal sizeCols() As String = Nothing, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False, Optional ByVal colSpanCol As Integer = 1, Optional ByVal moveIcon As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickShow As String = ""         'Href onclick Mode edicio
            Dim strClickMove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            'hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'Bucle als registres

            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim colsizeint As Integer = -1
                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    colsizeint += 1
                    hTCell = New HtmlTableCell
                    hTCell.Width = sizeCols(colsizeint)

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    hTCell.Style("display") = "table-cell"

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString
                    If IsDate(dTable.Rows(n)(y)) Then
                        hTCell.InnerText = Format(dTable.Rows(n)(y), HelperWeb.GetShortDateFormat)
                        If hTCell.InnerText = "01/01/2079" Then hTCell.InnerText = ""
                        'hTCell.Style("text-align") = "center"
                    End If
                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons = True Then
                        If y = dTable.Columns.Count - 1 Then
                            hTCell = New HtmlTableCell
                            hTCell.Width = "40px"
                            hTCell.Attributes("class") = "GridStyle-cellheader"
                            hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                            Dim hAnchorEdit As New HtmlAnchor
                            Dim hAnchorRemove As New HtmlAnchor

                            strClickShow = "cargaEmpleado2("
                            strClickMove = "ShowMoveCurrentEmployee("
                            If dTblControls IsNot Nothing Then
                                For Each dRow As DataRow In dTblControls.Rows
                                    strClickShow &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                    strClickMove &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                Next
                            End If
                            strClickShow = strClickShow.Substring(0, strClickShow.Length - 1) & ");"
                            strClickMove = strClickMove.Substring(0, strClickMove.Length - 1) & ");"

                            hAnchorEdit.Attributes("onclick") = strClickShow
                            hAnchorEdit.Title = Me.Language.Translate("ViewEmployee", Me.DefaultScope) '"Ver empleado"
                            hAnchorEdit.HRef = "javascript: void(0);"
                            hAnchorEdit.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/showemp.png") & """>"

                            hAnchorRemove.Attributes("onclick") = strClickMove
                            hAnchorRemove.Title = Me.Language.Translate("MoveEmployee", Me.DefaultScope) '"Mover empleado"
                            hAnchorRemove.HRef = "javascript: void(0);"
                            hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/move.png") & """>"

                            hTCell.Controls.Add(hAnchorEdit)
                            If moveIcon = True Then
                                'Comprobamos si se tiene permisos sobre el empleado
                                Me.oPermissionMobility = Me.GetFeaturePermissionByEmployee(FeatureMobilityAlias, dTable.Rows(n)(0).ToString)

                                If oPermissionMobility > Permission.Read Then
                                    hTCell.Controls.Add(hAnchorRemove)
                                End If
                            End If

                            hTRow.Cells.Add(hTCell)
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerHtml = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try
    End Function

    ''' <summary>
    ''' Carrega el Grid "GENERAL" en Mode "edicio"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadEmployeeGridEdit(ByVal oEmployee As roEmployee)
        Try
            'Carrega Grid Usuaris
            Dim dsUsuari As DataSet
            dsUsuari = API.EmployeeServiceMethods.GetUserFieldsDataset(Me, oEmployee.ID)

            Dim dTbl As DataTable = dsUsuari.Tables(0)
            'dTbl.Columns.Remove("Type")
            Dim Columns() As String = {"FieldCaption", "Value"}
            Dim htmlTGrid As HtmlTable = creaGrid(oEmployee.ID, dTbl, Columns, dTbl.Columns(1).ColumnName, True)
            Me.divGrid.Controls.Add(htmlTGrid)

            Me.btn1Fields.Visible = False
            Me.btn2Fields.Visible = True
            Me.btn3Fields.Visible = True

            Me.saveEditGridEmp.Attributes("onclick") = "saveGrid('" & oEmployee.ID & "')"
            Me.cancelEditGridEmp.Attributes("onclick") = "cancelEditGridE('" & oEmployee.ID & "')"
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    Private Function saveEmployeeGrid(ByVal oParameters As EmployeeCallbackRequest) As String
        ''Try

        'Dim oEmployeeService As EmployeeService.wsEmployeeWse = WebServices.EmployeeService

        Dim oUserField As roEmployeeUserField
        Dim intEmployeeID As Integer = oParameters.ID
        Dim bolSaveData As Boolean
        Dim dRowError As DataRow

        'Taula per acumular els errors
        Dim errTable As DataTable = New DataTable
        errTable.Columns.Add("FieldName")
        errTable.Columns.Add("FieldValue")
        errTable.Columns.Add("BeginEnd")
        errTable.Columns.Add("ErrorDescription")

        'Carrega Grid Usuaris per comprobar el tipus de columna
        Dim dsUsuari As DataSet
        dsUsuari = API.EmployeeServiceMethods.GetUserFieldsDataset(Me, intEmployeeID)
        Dim dTbl As DataTable = dsUsuari.Tables(0)

        ' Guardo los UserFields del employee actual
        bolSaveData = True
        Dim strFieldName As String

        Dim oFieldParameters() As String = oParameters.resultClientAction.Split("&")

        For Each cVars As String In oFieldParameters
            If cVars = String.Empty Then Continue For

            Dim fieldData() As String = cVars.Split("=")
            strFieldName = fieldData(0).Substring(4)

            If strFieldName.ToString.EndsWith("_@@Date@@") Then Continue For
            If strFieldName.ToString.EndsWith("_I") Then
                strFieldName = strFieldName.Substring(0, strFieldName.Length - 2)
            End If
            If strFieldName.ToString.EndsWith("_VI") Then Continue For
            If strFieldName.ToString.EndsWith("_DDDWS") Then Continue For
            If strFieldName.ToString.EndsWith("_LDeletedItems") Then Continue For
            If strFieldName.ToString.EndsWith("_LInsertedItems") Then Continue For
            If strFieldName.ToString.EndsWith("_LCustomCallback") Then Continue For
            If strFieldName.ToString.EndsWith("_Raw") Then Continue For
            If strFieldName.ToString.EndsWith("_DDD_C_STATE") Then Continue For
            If strFieldName.ToString.EndsWith("_DDD_C_FNPWS") Then Continue For

            If cVars.ToString.StartsWith("USR_") Then

                oUserField = New roEmployeeUserField
                oUserField.Definition = New roUserField

                oUserField.FieldName = strFieldName
                If (fieldData.Length > 1) Then
                    oUserField.FieldValue = Server.UrlDecode(fieldData(1).Trim())
                End If

                Dim bolIsBeginPeriod As Boolean = False
                Dim bolIsEndPeriod As Boolean = False

                If strFieldName.EndsWith("_##BeginPeriod##") Then
                    bolIsBeginPeriod = True
                    oUserField.FieldName = strFieldName.Substring(0, strFieldName.Length - CStr("_##BeginPeriod##").Length)
                ElseIf strFieldName.EndsWith("_##EndPeriod##") Then
                    bolIsEndPeriod = True
                    oUserField.FieldName = strFieldName.Substring(0, strFieldName.Length - CStr("_##EndPeriod##").Length)
                End If

                If strFieldName.EndsWith("_##LinkShow##") Then
                    bolIsBeginPeriod = True
                    oUserField.FieldName = strFieldName.Substring(0, strFieldName.Length - CStr("_##LinkShow##").Length)
                ElseIf strFieldName.EndsWith("_##LinkReal##") Then
                    bolIsEndPeriod = True
                    oUserField.FieldName = strFieldName.Substring(0, strFieldName.Length - CStr("_##LinkReal##").Length)
                End If

                ' Obtenemos el valor del campo fecha (control de histórico de valores)
                Dim strFieldDate As String = ""

                Dim dateFieldName As String = "USR_" & oUserField.FieldName & "_@@Date@@="
                If oParameters.resultClientAction.Contains(dateFieldName) Then
                    Dim startPos As Integer = oParameters.resultClientAction.IndexOf(dateFieldName)
                    Dim dateVal As String = oParameters.resultClientAction.Substring(startPos + dateFieldName.Length)
                    If dateVal <> "" AndAlso dateVal.IndexOf("&") <> -1 Then
                        strFieldDate = dateVal.Substring(0, dateVal.IndexOf("&"))
                    Else
                        strFieldDate = dateVal
                    End If
                End If

                If strFieldDate <> "" Then
                    oUserField._Date = Date.ParseExact(strFieldDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture)
                Else
                    oUserField._Date = roTypes.CreateDateTime(1900, 1, 1)
                End If

                'Busca el FieldType dintre de la BBDD
                Dim dRow As DataRow() = dTbl.Select("FieldName = '" & oUserField.FieldName & "'")
                If dRow.Length > 0 Then
                    oUserField.Definition.FieldType = dRow(0).Item("Type")
                Else ' Si no el troba, possa String (0)
                    oUserField.Definition.FieldType = 0
                End If

                Select Case oUserField.Definition.FieldType

                    Case FieldTypes.tDate
                        Dim parsedDate As DateTime
                        Dim dateFormat As String = "dd/MM/yyyy"

                        If DateTime.TryParseExact(oUserField.FieldValue, dateFormat, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, parsedDate) Then
                            oUserField.FieldValue = parsedDate.Date
                        Else
                            If oUserField.FieldValue = "" Then
                                oUserField.FieldValue = Nothing
                            Else
                                'Graba a la taula de errors
                                dRowError = errTable.NewRow
                                dRowError("FieldName") = oUserField.FieldName
                                dRowError("FieldValue") = oUserField.FieldValue
                                dRowError("ErrorDescription") = Me.Language.Translate("InvalidDate", Me.DefaultScope) '"No es una fecha valida"
                                errTable.Rows.Add(dRowError)
                                bolSaveData = False
                            End If
                        End If

                    Case FieldTypes.tNumeric
                        If IsNumeric(oUserField.FieldValue) Then
                            oUserField.FieldValue = CInt(oUserField.FieldValue)
                        Else
                            If oUserField.FieldValue = "" Then
                                oUserField.FieldValue = Nothing
                            Else
                                'Graba a la taula de errors
                                dRowError = errTable.NewRow
                                dRowError("FieldName") = oUserField.FieldName
                                dRowError("FieldValue") = oUserField.FieldValue
                                dRowError("ErrorDescription") = Me.Language.Translate("InvalidNumber", Me.DefaultScope) '"No es un número válido"
                                errTable.Rows.Add(dRowError)
                                bolSaveData = False
                            End If
                        End If

                    Case FieldTypes.tDecimal
                        If IsNumeric(oUserField.FieldValue) Then
                            oUserField.FieldValue = oUserField.FieldValue.ToString.Replace(".", HelperWeb.GetDecimalDigitFormat())
                        Else
                            If oUserField.FieldValue = "" Then
                                oUserField.FieldValue = Nothing
                            Else
                                'Graba a la taula de errors
                                dRowError = errTable.NewRow
                                dRowError("FieldName") = oUserField.FieldName
                                dRowError("FieldValue") = oUserField.FieldValue
                                dRowError("ErrorDescription") = Me.Language.Translate("InvalidDecimal", Me.DefaultScope) '"No es un número decimal válido"
                                errTable.Rows.Add(dRowError)
                                bolSaveData = False
                            End If
                        End If

                    Case FieldTypes.tList
                        If oUserField.FieldValue IsNot Nothing AndAlso oUserField.FieldValue <> "" Then
                            ' Verifica que el valor exista en la lista
                            Dim oUserFieldInfo As roUserField = API.UserFieldServiceMethods.GetUserField(Me, oUserField.FieldName, Types.EmployeeField, False, False)
                            If oUserFieldInfo IsNot Nothing Then
                                Dim bolNotExist As Boolean = True
                                For Each strItem As String In oUserFieldInfo.ListValues
                                    If strItem = oUserField.FieldValue Then
                                        bolNotExist = False
                                        Exit For
                                    End If
                                Next
                                If bolNotExist Then
                                    oUserField.FieldValue = ""
                                    'Graba a la taula de errors
                                    dRowError = errTable.NewRow
                                    dRowError("FieldName") = oUserField.FieldName
                                    dRowError("FieldValue") = oUserField.FieldValue
                                    dRowError("ErrorDescription") = Me.Language.Translate("InvalidListValue", Me.DefaultScope) '"No es un valor de la lista
                                    errTable.Rows.Add(dRowError)
                                    bolSaveData = False
                                End If
                            End If
                        End If

                    Case FieldTypes.tDatePeriod
                        Dim parsedDate As DateTime
                        Dim dateFormat As String = "dd/MM/yyyy"

                        If oUserField.FieldValue = "" OrElse DateTime.TryParseExact(oUserField.FieldValue, dateFormat, System.Globalization.CultureInfo.CurrentCulture, DateTimeStyles.None, parsedDate) Then

                            If oUserField.FieldValue.ToString <> "" Then oUserField.FieldValue = Format(parsedDate.Date, "yyyy/MM/dd")
                            ' Leemos el valor actual del campo
                            Dim strOriginalValue As String = dRow(0).Item("Value").ToString
                            If bolIsBeginPeriod Then
                                oUserField.FieldValue &= "*"
                                If strOriginalValue.Split("*").Length > 1 Then
                                    oUserField.FieldValue &= strOriginalValue.Split("*")(1)
                                End If
                            ElseIf bolIsEndPeriod Then
                                oUserField.FieldValue = strOriginalValue.Split("*")(0) & "*" & oUserField.FieldValue
                            End If
                            If oUserField.FieldValue = "" Or oUserField.FieldValue = "*" Then oUserField.FieldValue = Nothing
                            dRow(0).Item("Value") = oUserField.FieldValue
                        Else
                            'Graba a la taula de errors
                            dRowError = errTable.NewRow
                            dRowError("FieldName") = oUserField.FieldName
                            dRowError("BeginEnd") = IIf(bolIsBeginPeriod, "BeginPeriod", "EndPeriod")
                            dRowError("FieldValue") = oUserField.FieldValue
                            dRowError("ErrorDescription") = Me.Language.Translate("InvalidDatePeriod", Me.DefaultScope) '"No es una fecha valida"
                            errTable.Rows.Add(dRowError)
                            bolSaveData = False
                        End If

                    Case FieldTypes.tTimePeriod
                        If oUserField.FieldValue = "" OrElse IsDate(oUserField.FieldValue) Then

                            If oUserField.FieldValue <> "" Then oUserField.FieldValue = Format(CDate(oUserField.FieldValue), "HH:mm")

                            ' Leemos el valor actual del campo
                            Dim strOriginalValue As String = dRow(0).Item("Value").ToString
                            If bolIsBeginPeriod Then
                                oUserField.FieldValue &= "*"
                                If strOriginalValue.Split("*").Length > 1 Then
                                    oUserField.FieldValue &= strOriginalValue.Split("*")(1)
                                End If
                            ElseIf bolIsEndPeriod Then
                                oUserField.FieldValue = strOriginalValue.Split("*")(0) & "*" & oUserField.FieldValue
                            End If
                            If oUserField.FieldValue = "" Or oUserField.FieldValue = "*" Then oUserField.FieldValue = Nothing
                            dRow(0).Item("Value") = oUserField.FieldValue
                        Else
                            'Graba a la taula de errors
                            dRowError = errTable.NewRow
                            dRowError("FieldName") = oUserField.FieldName
                            dRowError("BeginEnd") = IIf(bolIsBeginPeriod, "BeginPeriod", "EndPeriod")
                            dRowError("FieldValue") = oUserField.FieldValue
                            dRowError("ErrorDescription") = Me.Language.Translate("InvalidTimePeriod", Me.DefaultScope) '"No es una fecha valida"
                            errTable.Rows.Add(dRowError)
                            bolSaveData = False
                        End If

                    Case FieldTypes.tLink
                        If oUserField.FieldValue <> "" Then
                            ' Leemos el valor actual del campo
                            Dim strOriginalValue As String = dRow(0).Item("Value").ToString
                            If bolIsBeginPeriod Then
                                oUserField.FieldValue &= "*"
                                If strOriginalValue.Split("*").Length > 1 Then
                                    oUserField.FieldValue &= strOriginalValue.Split("*")(1)
                                End If
                            ElseIf bolIsEndPeriod Then
                                oUserField.FieldValue = strOriginalValue.Split("*")(0) & "*" & oUserField.FieldValue
                            End If
                            If oUserField.FieldValue = "" Or oUserField.FieldValue = "*" Then oUserField.FieldValue = Nothing
                            dRow(0).Item("Value") = oUserField.FieldValue
                        End If
                End Select

                If bolSaveData AndAlso Not API.EmployeeServiceMethods.SaveUserField(Me, intEmployeeID, oUserField.FieldName, oUserField.FieldValue, oUserField._Date, True) Then
                    bolSaveData = False
                    'Graba a la taula de errors
                    dRowError = errTable.NewRow
                    dRowError("FieldName") = oUserField.FieldName
                    dRowError("FieldValue") = oUserField.FieldValue
                    dRowError("ErrorDescription") = roWsUserManagement.SessionObject.States.EmployeeUserFieldState.ErrorText
                    errTable.Rows.Add(dRowError)
                End If
            End If
        Next

        Dim Columns As String() = {"FieldCaption", "Value"}

        'Crea el grid, en mode edicio o normal segons errors
        If Not bolSaveData Then
            'Amb errors
            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oParameters.ID, False)

            If oEmployee Is Nothing Then Return ""

            'Carrega Grid Usuaris
            Dim dsEmp As DataSet
            dsEmp = API.EmployeeServiceMethods.GetUserFieldsDataset(Me, oParameters.ID)

            Dim dTblEmp As DataTable = dsEmp.Tables(0)

            'dTblEmp.Columns.Remove("Type")
            Dim htmlTGrid As HtmlTable = creaGrid(oParameters.ID, dTblEmp, Columns, dTbl.Columns("FieldName").ColumnName, True, errTable)
            Me.divGrid.Controls.Add(htmlTGrid)
            Me.btn1Fields.Visible = False
            Me.btn2Fields.Visible = True
            Me.btn3Fields.Visible = True
            Me.saveEditGridEmp.Attributes("onclick") = "saveGrid('" & oEmployee.ID & "')"
            Me.cancelEditGridEmp.Attributes("onclick") = "cancelEditGridE('" & oEmployee.ID & "')"
            Return "ERROR"
        Else
            Return ""
        End If
    End Function

    Private Sub LoadDateRangeCombos(ByVal idEmployee As Integer)

        Dim actualContractInfo As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim currentAnnualPeriodInfo As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim actualMonthPeriod As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim lastMonthPeriod As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim weekPeriod As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim actualAnnualPeriod As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"
        Dim lastAnnualPeriod As String = " (" & Language.Translate("SummaryDateRange.Without_Data", DefaultScope) & ")"

        Dim oActiveWorkPeriod As List(Of Date) = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.Contrato, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(1).Year <> 1900 Then actualContractInfo = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.ContractAnnualized, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then currentAnnualPeriodInfo = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.Mensual, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then actualMonthPeriod = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.LastMonth, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then lastMonthPeriod = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.Semanal, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then weekPeriod = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.Anual, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then actualAnnualPeriod = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        oActiveWorkPeriod = API.ContractsServiceMethods.GetDatesOfEmployeePeriodByContractInDate(Me, SummaryType.LastYear, idEmployee, DateTime.Now.Date)
        If oActiveWorkPeriod IsNot Nothing AndAlso oActiveWorkPeriod(0).Year <> 1900 Then lastAnnualPeriod = $" ({oActiveWorkPeriod(0).ToShortDateString} - {oActiveWorkPeriod(1).ToShortDateString})"

        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Contrato", DefaultScope) & actualContractInfo, SummaryType.Contrato)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.LastMonth", DefaultScope) & lastMonthPeriod, SummaryType.LastMonth)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.LastYear", DefaultScope) & lastAnnualPeriod, SummaryType.LastYear)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Semanal", DefaultScope) & weekPeriod, SummaryType.Semanal)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Mensual", DefaultScope) & actualMonthPeriod, SummaryType.Mensual)
        cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.Anual", DefaultScope) & actualAnnualPeriod, SummaryType.Anual)
        'cmbSummaryPeriod.Items.Add(Language.Translate("SummaryDateRange.AnualWork", DefaultScope) & currentAnnualPeriodInfo, SummaryType.ContractAnnualized) 'Quitamos año laboral (PBI:1549431)

        Dim summaryPeriod As SummaryType = WLHelperWeb.Context(Request).SummaryType

        cmbSummaryPeriod.SelectedItem = cmbSummaryPeriod.Items.FindByValue(summaryPeriod.ToString())
    End Sub

    Private Function LastPunchResume(lastPunch As roLastPunchSummary) As String

        Dim params As New List(Of String)
        params.Add(lastPunch.PunchDateTime)
        If (String.IsNullOrEmpty(lastPunch.PunchTerminal) AndAlso String.IsNullOrEmpty(lastPunch.PunchZone)) Then
            Return Language.Translate("Summary.PunchInfoOnlyDate", DefaultScope, params)
        End If

        If (Not String.IsNullOrEmpty(lastPunch.PunchTerminal) AndAlso String.IsNullOrEmpty(lastPunch.PunchZone)) Then
            params.Add(lastPunch.PunchTerminal)
            Return Language.Translate("Summary.PunchInfoDateTerminal", DefaultScope, params)
        End If

        If (String.IsNullOrEmpty(lastPunch.PunchTerminal) AndAlso Not String.IsNullOrEmpty(lastPunch.PunchZone)) Then
            params.Add(lastPunch.PunchZone)
            Return Language.Translate("Summary.PunchInfoDateZone", DefaultScope, params)
        End If

        params.Add(lastPunch.PunchTerminal)
        params.Add(lastPunch.PunchZone)

        Return Language.Translate("Summary.PunchInfoAll", DefaultScope, params)

    End Function

    Private Function PlanificationResume(planification As roPlanificationSummary, lastPunch As roLastPunchSummary) As String

        Select Case planification.EmployeeState
            Case EmployeeState.Unplanned
                Return Language.Translate("Summary.Unplanned", DefaultScope)
            Case EmployeeState.Vacation
                Return Language.Translate("Summary.Vacations", DefaultScope)
            Case EmployeeState.ProgrammedAbsence
                Return Language.Translate("StateInfo.ScheduledAbsence", DefaultScope, planification.Params.ToList())
            Case EmployeeState.ProgrammedCause
                Return Language.Translate("StateInfo.ScheduledCause", DefaultScope, planification.Params.ToList())
                'Case EmployeeState.PresenceIn
                '    If (lastPunch.PunchType = PunchTypeEnum._TASK) Then
                '        Dim params As New List(Of String)
                '        params.Add(lastPunch.TaskName)
                '        params.Add(lastPunch.PunchDateTime.Value.Date)
                '        params.Add(lastPunch.PunchDateTime.Value.ToString("HH:mm"))
                '        Return Language.Translate("StateInfo.Task", DefaultScope, params)
                '    End If
        End Select

        Return String.Empty

    End Function

    Public Sub CreateSummaryData(idEmployee As Integer)
        oPermissionAccrual = Me.GetFeaturePermission(FeatureConcepts)
        oPermissionCauses = Me.GetFeaturePermission(FeatureCauses)
        oPermissionTasks = Me.GetFeaturePermission(FeatureTasksAnalytics)
        oPermissionBusinessCenters = Me.GetFeaturePermission(FeatureBusinessCentersAnalytics)
        oPermissionPlanification = GetFeaturePermission(FeatureCalendarPlanification)
        oPermissionCalendar = GetFeaturePermission(FeatureCalendar)

        Dim summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Me, idEmployee, Date.Now, SummaryType.Anual, SummaryType.Anual, SummaryType.Anual, SummaryType.Anual, SummaryRequestType.Punch)
        Dim summaryJson As New EmployeeSummaryData
        LoadDateRangeCombos(idEmployee)
        Dim strOnClick As String = String.Empty
        'Div para ver la edición de fichajes
        If (summary.employeeLastPunch IsNot Nothing) Then
            strOnClick = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                         "url = url + '&EmployeeID=" & idEmployee & "&Date=" & Format(summary.employeeLastPunch.PunchDateTime, "dd/MM/yyyy") & "'; " &
                         "parent.ShowExternalForm2(url, 1400 , 620, '', '', false, false, false);"
        Else
            strOnClick = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                         "url = url + '&EmployeeID=" & idEmployee & "&Date=" & Format(Now, "dd/MM/yyyy") & "'; " &
                         "parent.ShowExternalForm2(url, 1400 , 620, '', '', false, false, false);"
        End If

#Region "Ultimo Fichaje"

        'Dibujo el último fichaje
        If (oPermissionCalendar > Permission.None) Then
            If (summary.employeeLastPunch IsNot Nothing) Then
                Dim divImage As New HtmlGenericControl("div")
                divImage.Attributes("class") = "lastPunchContainer"
                divImage.Style("background-image") = Page.ResolveUrl("~/Base/Images/Employees/Presencia.png")
                divImage.Style("width") = "60px"
                divImage.Style("height") = "48px"
                divImage.Style("background-repeat") = "no-repeat"
                Dim divImageStatus As New HtmlGenericControl("div")
                divImageStatus.Attributes("class") = If(summary.employeeLastPunch.PunchPresenceType = PunchSummaryType._IN, "pf-status-active", "pf-status-inactive")
                divImage.Controls.Add(divImageStatus)
                'divSummaryPunch.Controls.Add(divImage)

                Dim divStatus As New HtmlGenericControl("div")
                divStatus.Attributes("class") = "lastPunchContainer"
                divStatus.Style("width") = "410px"
                divStatus.Style("margin-left") = "50px"

                Dim divPresStatus As New HtmlGenericControl("div")
                Dim spanPresStatus As New HtmlGenericControl("span")
                spanPresStatus.Style("font-weight") = "bold"
                spanPresStatus.InnerHtml = If(summary.employeeLastPunch.PunchPresenceType = PunchSummaryType._IN, Language.Translate("Summary.In", DefaultScope), Language.Translate("Summary.Out", DefaultScope))
                divPresStatus.Controls.Add(spanPresStatus)

                Dim divPresStatusDesc As New HtmlGenericControl("div")
                divPresStatusDesc.InnerHtml = LastPunchResume(summary.employeeLastPunch)
                divStatus.Controls.Add(divPresStatus)
                divStatus.Controls.Add(divPresStatusDesc)

                divSummaryPunch.Controls.Add(divImage)
                divSummaryPunch.Controls.Add(divStatus)
                'Valido si tiene imagen del fichajes
                If (Not String.IsNullOrEmpty(summary.employeeLastPunch.PunchLocation)) Then
                    Dim divLocationContainer As New HtmlGenericControl("div")
                    divLocationContainer.Attributes("class") = "lastPunchContainer"
                    divLocationContainer.Style("width") = "300px"

                    'div con el mapa
                    Dim divPunchLocation As New HtmlGenericControl("div")
                    divPunchLocation.Attributes("id") = "divPunchMap"

                    'Div con el texto
                    Dim divLabelLocation As New HtmlGenericControl("div")
                    Dim spanLocationLabel As New HtmlGenericControl("span")
                    Dim params As New List(Of String)
                    params.Add(summary.employeeLastPunch.PunchLocationZone)
                    spanLocationLabel.InnerHtml = Language.Translate("Summary.SummaryLocationLabel", DefaultScope, params)
                    divLabelLocation.Controls.Add(spanLocationLabel)

                    summaryJson.PunchLocation = summary.employeeLastPunch.PunchLocation
                    divLocationContainer.Controls.Add(divLabelLocation)
                    divLocationContainer.Controls.Add(divPunchLocation)
                    divSummaryPunch.Controls.Add(divLocationContainer)
                    divSummaryPunch.Style("min-height") = "100px"
                    divSummaryPunch.Attributes("coord0") = summary.employeeLastPunch.PunchLocation.Split(",")(0)
                    divSummaryPunch.Attributes("coord1") = summary.employeeLastPunch.PunchLocation.Split(",")(1)
                Else
                    divSummaryPunch.Attributes("coord0") = ""
                    divSummaryPunch.Attributes("coord1") = ""
                End If

                If (Me.GetFeaturePermission("Employees.UserFields.Information.High") > Permission.None AndAlso summary.employeeLastPunch.HasPhoto) Then
                    Dim divImageContainer As New HtmlGenericControl("div")
                    divImageContainer.Attributes("class") = "lastPunchContainer"
                    divImageContainer.Style("width") = "300px"

                    'Div con el label del fichaje
                    Dim divLabelCapture As New HtmlGenericControl("div")
                    Dim spanImageLabel As New HtmlGenericControl("span")
                    spanImageLabel.InnerHtml = Language.Translate("Summary.SummaryImageLabel", DefaultScope)
                    divLabelCapture.Controls.Add(spanImageLabel)

                    Dim divLoadImage As New HtmlGenericControl("div")
                    divLoadImage.ID = "dvSummaryLoadPunch"
                    divLoadImage.ClientIDMode = ClientIDMode.Static
                    divLoadImage.Style("width") = "200px"
                    divLoadImage.Style("hight") = "110px"

                    Dim loadImage As New HtmlImage()
                    With loadImage
                        .Alt = Me.Language.Translate("Summary.loadPunch", Me.DefaultScope)
                        .Attributes.Add("title", Me.Language.Translate("Summary.loadPunch", Me.DefaultScope))
                        .Style("height") = "20px"
                        .Style("width") = "20px"
                        .Style("display") = "block"
                        .Style("padding-top") = "40px"
                        .Style("margin") = "auto"
                        .Style("cursor") = "pointer"
                        .Attributes.Add("onclick", "auditAndShowPunchImage()")
                        .Attributes.Add("attr-urlPunch", "../Scheduler/LoadImageCapture.aspx?IdPunch=" & summary.employeeLastPunch.IdPunch.ToString & "&NewParam=" & Now.TimeOfDay.Seconds.ToString)
                        .Src = "~/Base/images/icoRefresh.png"
                        .ID = "imgLoadEmployeePunch"
                        .ClientIDMode = ClientIDMode.Static
                    End With
                    divLoadImage.Controls.Add(loadImage)

                    'Div con la imagen del fichaje
                    Dim divPunchImage As New HtmlGenericControl("div")
                    divPunchImage.Style("width") = "200px"
                    divPunchImage.Style("display") = "none"
                    divPunchImage.ID = "dvEmployeePunch"
                    divPunchImage.ClientIDMode = ClientIDMode.Static

                    Dim captureImage As New HtmlImage()
                    With captureImage
                        .Alt = String.Empty
                        .Style("height") = "120px"
                        .Style("width") = "150px"
                        .Src = ""
                        .ID = "imgEmployeePunch"
                        .ClientIDMode = ClientIDMode.Static
                    End With
                    divPunchImage.Controls.Add(captureImage)

                    divImageContainer.Controls.Add(divLabelCapture)
                    divImageContainer.Controls.Add(divPunchImage)
                    divImageContainer.Controls.Add(divLoadImage)
                    divSummaryPunch.Style("min-height") = "120px"
                    divSummaryPunch.Controls.Add(divImageContainer)
                End If
                Dim divSeePunch As New HtmlGenericControl("div")
                divSeePunch.InnerHtml = Language.Translate("Summary.SeePunch", DefaultScope)
                divSeePunch.Attributes("class") = "summaryLink"
                divSeePunch.Attributes("onclick") = If(String.IsNullOrEmpty(strOnClick), String.Empty, strOnClick)
                divStatus.Controls.Add(divSeePunch)
            Else
                divLastPunch.Style("display") = "none"
            End If
        Else
            divLastPunch.Style("display") = "none"
        End If

#End Region

#Region "Planificación"

        'Dibujo Planificación
        'summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Me, idEmployee, Date.Now, EmployeeService.SummaryType.Anual, EmployeeService.SummaryType.Anual, EmployeeService.SummaryType.Anual, EmployeeService.SummaryType.Anual, EmployeeService.SummaryRequestType.Schedule)

        If (oPermissionPlanification > Permission.None) Then
            'If (summary.employeePlanification IsNot Nothing AndAlso summary.employeePlanification.Shift IsNot Nothing AndAlso summary.employeePlanification.Shift.Length > 0) Then
            If summary.employeePlanification.EmployeeState <> EmployeeState.Unplanned Then
                Dim planText = PlanificationResume(summary.employeePlanification, summary.employeeLastPunch)
                If (Not String.IsNullOrEmpty(planText)) Then
                    Dim divActualPlanification As New HtmlGenericControl("div")
                    Dim spanActualPlanificationLabel As New HtmlGenericControl("span")
                    spanActualPlanificationLabel.InnerHtml = Language.Translate("Summary.StateLabel", DefaultScope)

                    Dim divActualPlanificationValue As New HtmlGenericControl("b")
                    divActualPlanificationValue.InnerHtml = " " + planText.ToUpper()
                    divActualPlanification.Controls.Add(spanActualPlanificationLabel)
                    divActualPlanification.Controls.Add(divActualPlanificationValue)

                    divPlanification.Controls.Add(divActualPlanification)
                End If
            End If

            If (Not String.IsNullOrEmpty(summary.employeePlanification.Shift)) Then
                'Horario
                Dim divActualShiftRow As New HtmlGenericControl("div")

                Dim spanActualShiftLabel As New HtmlGenericControl("span")
                spanActualShiftLabel.InnerHtml = Language.Translate("Summary.ShiftLabel", DefaultScope)
                Dim divActualShiftValue As New HtmlGenericControl("b")
                divActualShiftValue.InnerHtml = " " + summary.employeePlanification.Shift
                divActualShiftRow.Controls.Add(spanActualShiftLabel)
                divActualShiftRow.Controls.Add(divActualShiftValue)
                divPlanification.Controls.Add(divActualShiftRow)
            End If


            If (Not String.IsNullOrEmpty(summary.employeePlanification.Asiggment)) Then

                Dim divActualAsiggmentRow As New HtmlGenericControl("div")
                Dim spanActualAsiggmentLabel As New HtmlGenericControl("span")
                spanActualAsiggmentLabel.InnerHtml = Language.Translate("Summary.AssignmentLabel", DefaultScope)
                Dim divActualAsiggmentValue As New HtmlGenericControl("b")
                divActualAsiggmentValue.InnerHtml = " " + summary.employeePlanification.Asiggment
                divActualAsiggmentRow.Controls.Add(spanActualAsiggmentLabel)
                divActualAsiggmentRow.Controls.Add(divActualAsiggmentValue)
                divPlanification.Controls.Add(divActualAsiggmentRow)
            End If
            'Nuevo div para ver calendario y fichajes

            Dim divSeeCalendar As New HtmlGenericControl("div")
            Dim divSeeCalendarLabel As New HtmlGenericControl("div")
            divSeeCalendarLabel.Attributes("class") = "splitDivLeft"
            divSeeCalendarLabel.Attributes("class") = "summaryLink"
            divSeeCalendarLabel.Style("padding-top") = "0px"
            divSeeCalendarLabel.InnerHtml = Language.Translate("Summary.SeeCalendar", DefaultScope)
            divSeeCalendarLabel.Attributes("onclick") = "showEmpAnnualDetail('" & idEmployee & "');"
            divSeeCalendar.Controls.Add(divSeeCalendarLabel)

            Dim divSeePunch As New HtmlGenericControl("div")
            Dim divSeePunchLabel As New HtmlGenericControl("div")
            divSeePunchLabel.Attributes("class") = "splitDivLeft"
            divSeePunchLabel.Attributes("class") = "summaryLink"
            divSeePunchLabel.Style("padding-top") = "0px"
            divSeePunchLabel.InnerHtml = Language.Translate("Summary.SeePunch", DefaultScope)
            divSeePunchLabel.Attributes("onclick") = If(String.IsNullOrEmpty(strOnClick), String.Empty, strOnClick)
            divSeePunch.Controls.Add(divSeePunchLabel)

            divPlanificationLinks.Controls.Add(divSeeCalendar)
            divPlanificationLinks.Controls.Add(divSeePunch)
            'If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then
            '    Dim divSeeScheduleRules As New HtmlGenericControl("div")
            '    divSeeScheduleRules.Attributes("class") = "divRow"
            '    Dim divSeeScheduleRulesLabel As New HtmlGenericControl("div")
            '    divSeeScheduleRulesLabel.Attributes("class") = "splitDivLeft"
            '    divSeeScheduleRulesLabel.Attributes("class") = "summaryLink"
            '    divSeeScheduleRulesLabel.Style("padding-top") = "0px"
            '    divSeeScheduleRulesLabel.InnerHtml = Language.Translate("Summary.SeeScheduleRules", DefaultScope)
            '    divSeeScheduleRulesLabel.Attributes("onclick") = "showActualIdContractScheduleRules();"
            '    divSeeScheduleRules.Controls.Add(divSeeScheduleRulesLabel)

            '    divPlanificationLinks.Controls.Add(divSeeScheduleRules)
            'End If
            '<div style=""width: 100%; padding-left: 155px;""><a href=""javascript: void(0);"" class=""icoEmpViewAnnualDetail"" onclick=""showEmpAnnualDetail('" & oSchedulerEmployee.IDEmployee & "');""></a></div>


            'Else
            'divPresenceSummary.Style("display") = "none"
            'End If
        Else
            divPresenceSummary.Style("display") = "none"
        End If

#End Region

        Dim bNoData As Boolean = True

        If (oPermissionAccrual = Permission.None) Then
            divAccrualsSummary.Attributes("permission") = "0"
        Else
            divAccrualsSummary.Attributes("permission") = "1"
            bNoData = False
        End If

        If (oPermissionCauses = Permission.None) Then
            divCausesSummary.Attributes("permission") = "0"
        Else
            divCausesSummary.Attributes("permission") = "1"
            bNoData = False
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") = False OrElse oPermissionTasks = Permission.None Then
            divTasksSummary.Attributes("permission") = "0"
        Else
            divTasksSummary.Attributes("permission") = "1"
            bNoData = False
        End If

        If (Not bolCostCentersLicense OrElse oPermissionBusinessCenters = Permission.None) Then
            divBussinessCentersSummary.Attributes("permission") = "0"
        Else
            divBussinessCentersSummary.Attributes("permission") = "1"
            bNoData = False
        End If

        If bNoData Then
            noDataRow.Style("display") = ""
        End If

        ASPxCallbackPanelContenido.JSProperties.Add("cp_EmployeeSummary", roJSONHelper.SerializeNewtonSoft(summaryJson))
    End Sub

#Region "Grid Contracts"

    Private Sub BindGridContracts(ByVal bolReload As Boolean)
        Me.GridContracts.DataSource = Me.ContractsData(bolReload)
        Me.GridContracts.DataBind()
        If Me.GridContracts.IsEditing() Then
            Me.GridContracts.CancelEdit()
        End If

        Try
            If Not GridContracts.JSProperties.ContainsKey("cpActionRO") Then GridContracts.JSProperties.Add("cpActionRO", "")
        Catch ex As Exception
            GridContracts.JSProperties("cpActionRO") = ""
        End Try

    End Sub

    Protected Sub btnAddNewContract_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewContract", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub lblContractsCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.Text = Me.Language.Translate("grdContracts.Title", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Private Sub GridContracts_Init(sender As Object, e As EventArgs) Handles GridContracts.Init
        Dim toolbar As GridViewToolbar = CType(GridContracts.Toolbars(0), GridViewToolbar)

        CType(toolbar.Items.FindByName("New"), GridViewToolbarItem).Enabled = (Me.GetFeaturePermissionByEmployee(FeatureContracts, Me.IdCurrentEmployee) >= Permission.Write)
        CType(toolbar.Items.FindByName("New"), GridViewToolbarItem).Text = Me.Language.Translate("grdContracts.Title", DefaultScope)
    End Sub

    Private Sub CreateColumnsContracts()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnImage As GridViewDataImageColumn
        Dim CustomButton As GridViewCommandColumnCustomButton

        Dim VisibleIndex As Integer = 0

        Me.GridContracts.Columns.Clear()
        Me.GridContracts.KeyFieldName = "ID"
        Me.GridContracts.SettingsText.EmptyDataRow = " "
        Me.GridContracts.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Dim uiControl As DevExpress.Web.ASPxButton = CType(Me.GridContracts.FindTitleTemplateControl("btnAddNewContract"), DevExpress.Web.ASPxButton)

        If Me.oContractPermission >= Permission.Write Then
            Me.GridContracts.SettingsEditing.Mode = GridViewEditingMode.Inline
        ElseIf Me.oContractPermission <= Permission.Read Then
            Me.GridContracts.SettingsDataSecurity.AllowEdit = False
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        If Me.oContractPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = True

            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 16
            VisibleIndex = VisibleIndex + 1

            CustomButton = New GridViewCommandColumnCustomButton()
            CustomButton.ID = "UpdateContractRow"
            CustomButton.Image.Url = "~/Base/Images/Grid/save.png"
            CustomButton.Image.ToolTip = " " 'Me.Language.Translate("GridContracts.Column.SaveRow", DefaultScope)
            CustomButton.Image.Height = New Unit(16)
            CustomButton.Image.Width = New Unit(16)
            CustomButton.Visibility = GridViewCustomButtonVisibility.EditableRow
            GridColumnCommand.CustomButtons.Add(CustomButton)

            If Me.isLabAgree Then
                CustomButton = New GridViewCommandColumnCustomButton()
                CustomButton.ID = "EditScheduleRowsRow"
                CustomButton.Image.Url = "~/Base/Images/Grid/Attachment.png"
                CustomButton.Image.ToolTip = Me.Language.Translate("GridContracts.Column.EditLabAgreeScheduleRules", DefaultScope)
                CustomButton.Image.Height = New Unit(16)
                CustomButton.Image.Width = New Unit(16)
                GridColumnCommand.CustomButtons.Add(CustomButton)
            End If

            Me.GridContracts.Columns.Add(GridColumnCommand)
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridContracts.Columns.Add(GridColumn)

        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "ImagePath"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.PropertiesImage.ImageWidth = 10
        GridColumnImage.PropertiesImage.ImageHeight = 10
        GridColumnImage.EditFormSettings.Visible = DevExpress.Utils.DefaultBoolean.False
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        Me.GridContracts.Columns.Add(GridColumnImage)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.IDContract", DefaultScope) '"Valor"
        GridColumn.FieldName = "IDContract"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 20
        Me.GridContracts.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridContracts.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "BeginDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        Me.GridContracts.Columns.Add(GridColumnDate)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridContracts.Column.EndDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "EndDateVisible"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        Me.GridContracts.Columns.Add(GridColumnDate)

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.Enterprise", DefaultScope) '"Valor"
        GridColumn.FieldName = "Enterprise"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridContracts.Columns.Add(GridColumn)

        If Me.isLabAgree Then
            'Date
            GridColumnCombo = New GridViewDataComboBoxColumn()
            GridColumnCombo.Caption = Me.Language.Translate("GridContracts.Column.LabAgree", DefaultScope) '"Fecha"
            GridColumnCombo.FieldName = "IDLabAgree"
            GridColumnCombo.VisibleIndex = VisibleIndex
            GridColumnCombo.PropertiesComboBox.DataSource = LabAgreedData
            GridColumnCombo.PropertiesComboBox.TextField = "Name"
            GridColumnCombo.PropertiesComboBox.ValueField = "ID"
            GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
            GridColumnCombo.PropertiesComboBox.RequireDataBinding()

            VisibleIndex = VisibleIndex + 1
            GridColumnCombo.ReadOnly = Not isLabAgree
            GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
            GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
            GridColumnCombo.Width = 35
            Me.GridContracts.Columns.Add(GridColumnCombo)
        End If

        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.EndContractReason", DefaultScope)
        GridColumn.FieldName = "EndContractReason"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridContracts.Columns.Add(GridColumn)

        If Me.oContractPermission >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = False

            GridColumnCommand.ShowCancelButton = True
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 10
            VisibleIndex = VisibleIndex + 1

            CustomButton = New GridViewCommandColumnCustomButton()
            CustomButton.ID = "DeleteContractRow"
            CustomButton.Image.Url = "~/Base/Images/Grid/remove.png"
            CustomButton.Image.ToolTip = " " 'Me.Language.Translate("GridContracts.Column.DeleteRow", DefaultScope)
            CustomButton.Image.Height = New Unit(16)
            CustomButton.Image.Width = New Unit(16)
            GridColumnCommand.CustomButtons.Add(CustomButton)

            Me.GridContracts.Columns.Add(GridColumnCommand)
        End If
    End Sub

    Private Sub GridContracts_DataBinding(sender As Object, e As EventArgs) Handles GridContracts.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridContracts.Columns("IDLabAgree")
        If oCombo IsNot Nothing Then
            oCombo.PropertiesComboBox.DataSource = LabAgreedData
            oCombo.PropertiesComboBox.TextField = "Name"
            oCombo.PropertiesComboBox.ValueField = "ID"
            oCombo.PropertiesComboBox.ValueType = GetType(Integer)
        End If
    End Sub

    Protected Sub GridContracts_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridContracts.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridContracts(False)

            GridContracts.JSProperties("cpActionRO") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridContracts(True)

            GridContracts.JSProperties("cpActionRO") = "RELOAD"
        End If
    End Sub

    Protected Sub GridContracts_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles GridContracts.StartRowEditing

    End Sub

    Protected Sub GridContracts_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridContracts.InitNewRow
        Dim tb As DataTable = Me.ContractsData()
        If (tb.Rows.Count > 0) Then
            e.NewValues("ID") = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
        Else
            e.NewValues("ID") = 1
        End If

        e.NewValues("BeginDate") = New Date
        e.NewValues("EndDate") = New Date(2079, 1, 1)
        e.NewValues("IDCard") = -999999
        e.NewValues("IDLabAgree") = -1

        GridContracts.JSProperties("cpActionRO") = "INITROW"
    End Sub

    Protected Sub GridContracts_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridContracts.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim newID As Integer

            Dim tb As DataTable = Me.ContractsData()
            If (tb.Rows.Count > 0) Then
                newID = roTypes.Any2Integer(tb.Rows(tb.Rows.Count - 1)("ID")) + 1
            Else
                newID = 1
            End If

            Dim oNewRow As DataRow = tb.NewRow
            With oNewRow
                .Item("ID") = newID
                .Item("IDEmployee") = Me.IdCurrentEmployee
                .Item("IDContract") = roTypes.Any2String(e.NewValues("IDContract")).Trim
                .Item("BeginDate") = e.NewValues("BeginDate")
                If e.NewValues("EndDateVisible") IsNot Nothing Then
                    .Item("EndDateVisible") = e.NewValues("EndDateVisible")
                    .Item("EndDate") = e.NewValues("EndDateVisible")
                Else
                    .Item("EndDateVisible") = DBNull.Value
                    .Item("EndDate") = New Date(2079, 1, 1)
                End If

                If e.NewValues("Enterprise") IsNot Nothing Then
                    .Item("Enterprise") = e.NewValues("Enterprise")
                Else
                    .Item("Enterprise") = String.Empty
                End If

                If e.NewValues("IDLabAgree") Is Nothing Then
                    .Item("IdLabAgree") = DBNull.Value
                Else
                    .Item("IDLabAgree") = e.NewValues("IDLabAgree")
                End If

                If e.NewValues("EndContractReason") IsNot Nothing Then
                    .Item("EndContractReason") = e.NewValues("EndContractReason")
                Else
                    .Item("EndContractReason") = String.Empty
                End If

                If CDate(.Item("BeginDate")).Date <= Now.Date AndAlso CDate(.Item("EndDate")).Date >= Now.Date Then
                    .Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                Else
                    .Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                End If
            End With

            Dim oContract As New roContract
            oContract.BeginDate = oNewRow("BeginDate")
            oContract.EndDate = oNewRow("EndDate")
            oContract.IDContract = oNewRow("IDContract")
            oContract.Enterprise = oNewRow("Enterprise")
            oContract.IDCard = -999999
            oContract.IDEmployee = oNewRow("IDEmployee")
            oContract.OriginalIDContract = "#######"
            oContract.EndContractReason = oNewRow("EndContractReason")

            If oNewRow("IDLabAgree") IsNot Nothing AndAlso Not IsDBNull(oNewRow("IDLabAgree")) AndAlso oNewRow("IDLabAgree") <> -1 Then
                Dim oLabAgree As New roLabAgree
                oLabAgree.ID = oNewRow("IDLabAgree")
                oContract.LabAgree = oLabAgree
            Else
                oContract.LabAgree = Nothing
            End If

            If API.ContractsServiceMethods.SaveContract(Me, oContract, True) Then
                oNewRow("InitialIDContract") = oContract.IDContract
                tb.Rows.Add(oNewRow)
                Me.ContractsData = tb
                e.Cancel = True
                grid.CancelEdit()
            Else
                Throw New Exception(roWsUserManagement.SessionObject.States.ContractState.ErrorText)
            End If

        End If

        GridContracts.JSProperties("cpActionRO") = "ROWINSERTING"
    End Sub

    Protected Sub GridContracts_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridContracts.RowUpdating

        Dim tb As DataTable = Me.ContractsData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridContracts.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDContract"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDContract") = roTypes.Any2String(enumerator.Value).Trim
                    Else
                        dr.Item("IDContract") = String.Empty
                    End If
                Case "BeginDate"
                    dr.Item("BeginDate") = enumerator.Value
                    If CDate(dr.Item("BeginDate")).Date <= Now.Date AndAlso CDate(dr.Item("EndDate")).Date >= Now.Date Then
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                    Else
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                    End If
                Case "EndDateVisible"
                    If enumerator.Value Is Nothing Then
                        dr.Item("EndDateVisible") = DBNull.Value
                        dr.Item("EndDate") = New Date(2079, 1, 1)
                    Else
                        dr.Item("EndDateVisible") = enumerator.Value
                        dr.Item("EndDate") = enumerator.Value
                    End If

                    If CDate(dr.Item("BeginDate")).Date <= Now.Date AndAlso CDate(dr.Item("EndDate")).Date >= Now.Date Then
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Play.gif")
                    Else
                        dr.Item("ImagePath") = Me.Page.ResolveUrl("~/Base/Images/Grid/Stop.gif")
                    End If

                Case "Enterprise"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("Enterprise") = enumerator.Value
                    Else
                        dr.Item("Enterprise") = String.Empty
                    End If

                Case "IDLabAgree"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDLabAgree") = enumerator.Value
                    Else
                        dr.Item("IDLabAgree") = -1
                    End If

                Case "EndContractReason"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("EndContractReason") = enumerator.Value
                    Else
                        dr.Item("EndContractReason") = String.Empty
                    End If

            End Select

        End While

        Dim oContract As New roContract
        oContract.BeginDate = dr.Item("BeginDate")
        oContract.EndDate = dr.Item("EndDate")

        If dr.Item("IDContract") <> String.Empty Then
            oContract.IDContract = dr.Item("IDContract")
        End If

        oContract.Enterprise = dr.Item("Enterprise")
        oContract.IDCard = roTypes.Any2Long(dr.Item("IDCard"))
        oContract.IDEmployee = dr.Item("IDEmployee")

        If Not IsDBNull(dr.Item("InitialIDContract")) Then
            oContract.OriginalIDContract = dr.Item("InitialIDContract")
        Else
            oContract.OriginalIDContract = "#######"
        End If

        If Not IsDBNull(dr.Item("Telecommuting")) Then
            oContract.Telecommuting = dr.Item("Telecommuting")
        Else
            oContract.Telecommuting = Nothing
        End If

        If Not IsDBNull(dr("TelecommutingAgreementStart")) Then
            oContract.TelecommutingAgreementStart = roTypes.Any2DateTime(dr("TelecommutingAgreementStart"))
        Else
            oContract.TelecommutingAgreementStart = Nothing
        End If
        If Not IsDBNull(dr("TelecommutingAgreementEnd")) Then
            oContract.TelecommutingAgreementEnd = roTypes.Any2DateTime(dr("TelecommutingAgreementEnd"))
        Else
            oContract.TelecommutingAgreementEnd = Nothing
        End If

        oContract.TelecommutingMandatoryDays = roTypes.Any2String(dr("TelecommutingMandatoryDays"))
        oContract.PresenceMandatoryDays = roTypes.Any2String(dr("PresenceMandatoryDays"))
        oContract.TelecommutingOptionalDays = roTypes.Any2String(dr("TelecommutingOptionalDays"))
        oContract.TelecommutingMaxDays = roTypes.Any2Integer(dr("TelecommutingMaxDays"))
        oContract.TelecommutingMaxPercentage = roTypes.Any2Integer(dr("TelecommutingMaxPercentage"))
        oContract.TelecommutingPeriodType = roTypes.Any2Integer(dr("PeriodType"))

        If dr.Item("IDLabAgree") IsNot Nothing AndAlso Not IsDBNull(dr.Item("IDLabAgree")) AndAlso dr.Item("IDLabAgree") <> -1 Then
            Dim oLabAgree As New roLabAgree
            oLabAgree.ID = dr.Item("IDLabAgree")
            oContract.LabAgree = oLabAgree
        Else
            oContract.LabAgree = Nothing
        End If

        oContract.EndContractReason = dr.Item("EndContractReason")

        If API.ContractsServiceMethods.SaveContract(Me, oContract, True) Then
            dr.Item("InitialIDContract") = oContract.IDContract
            Me.ContractsData = tb
            e.Cancel = True
            grid.CancelEdit()
        Else
            oContract.State.Result = ContractsResultEnum.InvalidIDContract
            'Throw New Exception("Mojon") 'roWsUserManagement.SessionObject.States.ContractState.ErrorText)
        End If

        GridContracts.JSProperties("cpActionRO") = "ROWUPDATING"
    End Sub

    Protected Sub GridContracts_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridContracts.CustomErrorText
        e.ErrorText = roWsUserManagement.SessionObject.States.ContractState.ErrorText
    End Sub

    Protected Sub GridContracts_CommandButtonInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCommandButtonEventArgs) Handles GridContracts.CommandButtonInitialize
        Dim tb As DataTable = Me.ContractsData()

        If CountActiveRows(tb) = 1 AndAlso e.ButtonType = ColumnCommandButtonType.Delete Then
            e.Visible = False
        End If

        If e.ButtonType = DevExpress.Web.ColumnCommandButtonType.Update Then
            e.Visible = False
        End If

        GridContracts.JSProperties("cpActionRO") = "CUSTOMBUTTON"
    End Sub

    Protected Sub GridContracts_CustomButtonInitialize(sender As Object, e As ASPxGridViewCustomButtonEventArgs) Handles GridContracts.CustomButtonInitialize
        Dim tb As DataTable = Me.ContractsData()

        If e.IsEditingRow AndAlso e.ButtonID = "DeleteContractRow" Then
            e.Visible = DevExpress.Utils.DefaultBoolean.False
            e.Enabled = False
        End If

        If CountActiveRows(tb) = 1 AndAlso e.ButtonID = "DeleteContractRow" Then
            e.Visible = DevExpress.Utils.DefaultBoolean.False
            e.Enabled = False
        End If

        GridContracts.JSProperties("cpActionRO") = "CUSTOMBUTTON"
    End Sub

    Protected Sub GridContracts_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridContracts.RowDeleting
        Dim tb As DataTable = Me.ContractsData()
        'Dim i As String = GridIncidences.FindVisibleIndexByKeyValue(e.Keys(GridIncidences.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridContracts.KeyFieldName))

        If tb.Rows.Count > 1 Then
            If API.ContractsServiceMethods.DeleteContract(Me, dr("IDContract"), True) Then
                dr.Delete()
                tb.AcceptChanges()
                Me.ContractsData = tb
                e.Cancel = True
            Else
                Throw New Exception(roWsUserManagement.SessionObject.States.ContractState.ErrorText)
            End If
        End If

        GridContracts.JSProperties("cpActionRO") = "DELETE"
    End Sub

#End Region

#Region "Methods"

    Private Function CountActiveRows(ByVal dData As DataTable) As Integer
        Dim iActiveRows As Integer = 0

        For Each oRow As DataRow In dData.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                iActiveRows = iActiveRows + 1
            End If
        Next

        Return iActiveRows
    End Function

    Public Function isLabAgreeVisible() As Boolean
        ' Miramos si hay la licencia de convenios activada
        Dim oPermission As Permission = Permission.None
        ' Miramos si el passport actual tiene permisos para visualizar los convenios del empleado
        If Me.HasFeaturePermissionByEmployee("LabAgree", Permission.Read, Me.IdCurrentEmployee) Then
            ' Miramos si el passport actual tiene permisos para mofdificar la asignación del convenio para el empleado actual.
            oPermission = Me.GetFeaturePermissionByEmployee("LabAgree.EmployeeAssign", Me.IdCurrentEmployee)
        End If

        Return oPermission >= Permission.Read
    End Function


#End Region

End Class