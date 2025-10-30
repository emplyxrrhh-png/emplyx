Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Groups
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

    Private lblIdentifyMethodsInfo As String = ""
    Private lblContractsDescription As String = ""
    Private lblContractsInfo As String = ""

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

    Private bolMultiCompanyLicense As Boolean = False
    Private bolKPIsLicense As Boolean = False
    Private bolCostCentersLicense As Boolean = False

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.OverrrideDefaultScope = "Employees"
        oCalendarPermission = GetFeaturePermission(CalendarFeatureAlias)

        InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        InsertExtraJavascript("EmployeeGroup", "~/Employees/Scripts/EmployeeGroups.js")
        InsertExtraJavascript("Employees", "~/Employees/Scripts/Employees_v2.js")
        InsertExtraJavascript("Groups", "~/Employees/Scripts/Groups_v2.js")
        InsertExtraJavascript("RoboticsGMaps", "~/Base/Scripts/RoboticsGMaps.js")
        InsertExtraJavascript("Summary", "~/Employees/Scripts/EmployeeSummary.js")
        InsertExtraJavascript("QueryRules", "~/Employees/Scripts/QueryScheduleRules.js")
        InsertExtraJavascript("Gmaps", $"https://maps.googleapis.com/maps/api/js?key={MapsKey}&callback=Function.prototype&language={HelperWeb.GetCookie("VTLive_Language")}&loading=async",,, False)

        If Not Me.IsPostBack Then
            HelperSession.DeleteEmployeeGroupsFromApplication()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Employees") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreeGroups1.TreeCaption = Me.Language.Translate("TreeCaptionGroups", Me.DefaultScope)

        Me.EmployeeURI.Value = Request.ApplicationPath + "#/" + Configuration.RootUrl + "/Employees/Employees"

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Not Me.HasFeaturePermission("Employees", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Not Me.IsPostBack And Not Me.IsCallback Then
            If Request.QueryString.Count Then
                ProcessQueryString()
            End If
        End If
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

            execJs = "actualType = 'E';changeTabs(" & roTypes.Any2Integer(Request.QueryString("TabEmployee")) & ");"

        ElseIf Not Request.QueryString("idgroup") Is Nothing Then
            Dim idgroup As String = Request.QueryString("idgroup")
            Dim treePath As String = String.Empty
            If Me.HasFeaturePermissionByGroup("Employees", Permission.Read, idgroup) Then
                treePath = roTools.GetGroupPath(idgroup)
            End If
            HelperWeb.roSelector_SetSelection(idgroup, treePath, "ctl00_contentMainBody_roTrees1",,, "1")
            execJs = "actualType = 'G';changeTabs(" & roTypes.Any2Integer(Request.QueryString("TabEmployee")) & ");"
        End If

        If (Request.QueryString("TabEmployee") IsNot Nothing) Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ChangeInitialLoadTab", execJs, True)
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New EmployeeCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "G"
                LoadInitialCombosData()
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
                        LoadGroupDataTab0(oParameters, False)
                    Case 1
                        LoadGroupDataTab1(oParameters)
                    Case 2
                        LoadGroupDataTab2(oParameters)
                    Case 3
                        LoadGroupDataTab3(oParameters)
                    Case 4
                        LoadGroupDataTab4(oParameters)
                    Case 5
                        LoadGroupDataTab5(oParameters)
                    Case 6
                        LoadGroupDataTab6(oParameters)
                End Select

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETGROUP")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpReloadRO", False)
            ElseIf oParameters.Action = "EDITGRID" Then
                LoadGroupDataTab0(oParameters, True)

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "EDITGRID")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ElseIf oParameters.Action = "SAVECOMPANYFIELDS" Then
                Dim strError = saveCompanyFieldsGrid(oParameters)
                If strError <> "" Then
                Else
                    LoadGroupDataTab0(oParameters, False)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETGROUP")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                End If
            ElseIf oParameters.Action = "SAVEGROUP" Then
                Dim strError As String = ""
                If oParameters.aTab = 0 Then
                    strError = saveGroup(oParameters)
                Else
                    strError = saveCostCenters(oParameters)
                End If
                If strError = "" Then
                    If oParameters.aTab = 0 Then
                        LoadGroupDataTab0(oParameters, False)
                    Else
                        LoadGroupDataTab3(oParameters)
                    End If

                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETGROUP")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpReloadRO", True)
                Else
                    Dim actualValue As String = txtDescription.Text
                    LoadGroupDataTab0(oParameters, False)
                    txtDescription.Text = actualValue
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVEGROUP")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", strError)
                End If
            End If

            If Me.oPermissionGroup < Permission.Write Then
                Me.DisableControls(Me.companyRow.Controls)
            End If
        End If

    End Sub

    Private Sub LoadInitialCombosData()
        Me.cmbWorkingZone.Items.Clear()
        Me.cmbWorkingZone.ValueType = GetType(Integer)
        Dim zonesDT As DataTable = API.ZoneServiceMethods.GetZones(Me)
        Me.cmbWorkingZone.Items.Add("", -1)
        If zonesDT IsNot Nothing AndAlso zonesDT.Rows.Count > 0 Then
            For Each oRow As DataRow In zonesDT.Rows
                Me.cmbWorkingZone.Items.Add(oRow("Name"), oRow("ID"))
            Next
        End If

        Me.cmbCostCenter.Items.Clear()
        Me.cmbCostCenter.ValueType = GetType(Integer)
        Dim CentersDT As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, WLHelperWeb.CurrentPassport.ID, True)
        Me.cmbCostCenter.Items.Add(Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope), 0)
        If CentersDT IsNot Nothing AndAlso CentersDT.Rows.Count > 0 Then
            For Each oRow As DataRow In CentersDT.Rows
                Me.cmbCostCenter.Items.Add(oRow("Name"), oRow("ID"))
            Next
        End If

    End Sub

    Private Sub LoadGroupDataTab0(ByVal oParameters As EmployeeCallbackRequest, ByVal isEdit As Boolean)
        Try
            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)

            If oGroup IsNot Nothing Then
                Me.txtDescription.Value = oGroup.DescriptionGroup
                Me.txtExport.Value = oGroup.Export
                Me.txtNameCompany.Text = oGroup.Name

                If roTypes.Any2String(HelperSession.AdvancedParametersCache("Customization")).ToLower = "taif" AndAlso Not HasFeaturePermission("Employees.Groups", Permission.Admin) Then
                    descriptionGroupRow.Style("display") = "none"
                End If

                If oGroup.IDZoneWorkingTime.HasValue Then
                    Me.cmbWorkingZone.SelectedItem = Me.cmbWorkingZone.Items.FindByValue(oGroup.IDZoneWorkingTime.Value)
                Else
                    Dim IDParentWorkingZone As Integer = -1
                    Dim IDParentNonWorkingZone As Integer = -1
                    API.EmployeeGroupsServiceMethods.GetGroupZones(Me, oGroup.ID, IDParentWorkingZone, IDParentNonWorkingZone)

                    If IDParentWorkingZone > -1 Then
                        Dim listEditItem As DevExpress.Web.ListEditItem = Me.cmbWorkingZone.Items.FindByValue(IDParentWorkingZone)

                        If listEditItem IsNot Nothing Then
                            Dim newItem As New DevExpress.Web.ListEditItem(listEditItem.Text & ("*"), -2)
                            Me.cmbWorkingZone.Items.Add(newItem)
                            Me.cmbWorkingZone.SelectedItem = newItem
                        End If
                    Else
                        Me.cmbWorkingZone.SelectedItem = Nothing
                    End If
                End If

                If oGroup.ID.ToString = oGroup.Path Then ' Es un grupo raiz
                    Me.txtCloseDate.Value = oGroup.CloseDate

                    ' Si es un grupo de nivel zero , cargamos la grid con los campo de la fcha de empresa
                    'If oGroup.ID.ToString = oGroup.Path AndAlso Me.bolMultiCompanyLicense Then
                    If isEdit Then
                        LoadCompanyGridEdit(oGroup)
                    Else
                        If oGroup.ID.ToString = oGroup.Path Then

                            Dim dsUserFields As DataSet
                            dsUserFields = API.EmployeeGroupsServiceMethods.GetUserFieldsDataset(Me, oParameters.ID)

                            If dsUserFields IsNot Nothing AndAlso dsUserFields.Tables(0).Rows.Count > 0 Then
                                Dim dTbl As DataTable = dsUserFields.Tables(0)
                                'dTbl.Columns.Remove("Type")
                                'dTbl.Columns.Remove("FieldName")
                                Dim Columns As String() = {"FieldCaption", "Value"}
                                Dim htmlTGrid As HtmlTable = creaFieldsGrid(dTbl, Columns)
                                Me.divCompanyFields.Controls.Add(htmlTGrid)

                                'Carrega el anchor de editar el grid
                                Me.editFieldsGrid.Attributes("onclick") = "editGroupFieldsGrid('" & oParameters.ID & "')"
                                Me.saveFieldsGrid.Attributes("onclick") = "saveGroupFieldsGrid('" & oParameters.ID & "')"
                                Me.cancelFieldsGrid.Attributes("onclick") = "cancelGroupFieldsGrid('" & oParameters.ID & "')"

                                Dim oPermUFields As Permission = Me.GetFeaturePermission("Employees.UserFields.Information")

                                If oPermUFields < Permission.Write Then
                                    Me.btn1FieldsCompany.Visible = False
                                End If
                            Else
                                Me.btn1FieldsCompany.Visible = False
                            End If

                            Me.btn2FieldsCompany.Visible = False
                            Me.btn3FieldsCompany.Visible = False
                            'Else
                            'actualTab = 0
                        End If
                    End If

                    If HelperSession.AdvancedParametersCache("Customization").ToUpper = "TAIF" Then
                        Me.divCompanyUserFields.Visible = (Me.oUserFieldsPermission > Permission.None)
                        divCompanyCloseDate.Visible = oCalendarPermission > Permission.Write
                    Else
                        Me.txtCloseDate.Value = Nothing
                        Me.divCompanyCloseDate.Visible = False
                    End If
                Else
                    Me.txtCloseDate.Value = Nothing
                    Me.divCompanyCloseDate.Visible = False
                    Me.divCompanyUserFields.Visible = False
                End If

                If roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("MultiTimeZoneEnabled")) Then
                    Me.divMultiTimezone.Style("display") = ""
                Else
                    Me.divMultiTimezone.Style("display") = "none"
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub LoadGroupDataTab1(ByVal oParameters As EmployeeCallbackRequest)
        Try
            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)

            If oGroup IsNot Nothing Then
                Dim dtblCurrent As DataTable
                Dim nRowControl As DataRow
                Dim strCols As String() = {Me.Language.Translate("ColumnEmployee", Me.DefaultScope), Me.Language.Translate("ColumnDateBegin", Me.DefaultScope), Me.Language.Translate("ColumnDateEnd", Me.DefaultScope)}
                Dim sizeCols As String() = {"350px", "100px", "100px"}
                Dim cssCols As String() = {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

                'PPR Aviso de PRL si hay datos de la ficha inconsistentes
                Me.divAdviceCompany.Visible = False
                Me.lblAdviceCompany.Text = String.Empty

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
                    Dim strCols2 As String() = {Me.Language.Translate("ColumnEmployee", Me.DefaultScope), Me.Language.Translate("ColumnDateBegin", Me.DefaultScope), Me.Language.Translate("ColumnDateEnd", Me.DefaultScope)}
                    Dim sizeCols2 As String() = {"350px", "100px", "100px"}
                    Dim cssCols2 As String() = {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}
                    Dim htmlHGridOld As HtmlTable = creaHeaderLists(strCols2, sizeCols2, cssCols2)
                    Me.divHeaderGrupPasat.Controls.Add(htmlHGridOld)

                    Dim htmlTGridOld As HtmlTable = creaGridLists(dtblOld, strCols2, sizeCols2, tblControlOld, True)
                    Me.divGridGrupPasat.Controls.Add(htmlTGridOld)
                Else
                    divHeaderGrupPasat.InnerHtml = Me.Language.Translate("NoEmployeesPast", Me.DefaultScope) '"No han habido empleados en este grupo"
                End If
            End If

        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        End Try

    End Sub

    Private Sub LoadGroupDataTab3(ByVal oParameters As EmployeeCallbackRequest)
        Try

            optHere.Checked = False
            optOneCost.Checked = False

            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)
            If oGroup Is Nothing Then Exit Sub

            If oGroup.IDCenter.HasValue Then
                ' Si tiene asignado un centro
                Dim listEditItemCenter As DevExpress.Web.ListEditItem = Me.cmbCostCenter.Items.FindByValue(oGroup.IDCenter)
                If listEditItemCenter Is Nothing Then

                    Me.cmbCostCenter.SelectedItem = Nothing
                    ' Si no esta en la lista hay que añadirlo porque el usuario actual no tiene permisos sobre ese centro
                    ' pero otro usuario lo ha asignado
                    Dim oCenter As roBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, oGroup.IDCenter, False)

                    If oCenter IsNot Nothing Then
                        Dim newItem As New DevExpress.Web.ListEditItem(oCenter.Name & ("*"), oCenter.ID)
                        Me.cmbCostCenter.Items.Add(newItem)
                        Me.cmbCostCenter.SelectedItem = newItem
                    End If
                Else
                    Me.cmbCostCenter.SelectedItem = Me.cmbCostCenter.Items.FindByValue(oGroup.IDCenter)
                End If
                optOneCost.Checked = True
            Else
                ' hay que mirar el centro de coste heredado
                Dim IDParenCenter As Integer = -1
                API.EmployeeGroupsServiceMethods.GetGroupCenters(Me, oGroup.ID, IDParenCenter)

                If IDParenCenter > -1 Then
                    Dim listEditItem As DevExpress.Web.ListEditItem = Me.cmbCostCenter.Items.FindByValue(IDParenCenter)

                    If listEditItem IsNot Nothing Then
                        Dim newItem As New DevExpress.Web.ListEditItem(listEditItem.Text & (""), -2)
                        Me.cmbCostCenter.Items.Add(newItem)
                        Me.cmbCostCenter.SelectedItem = newItem
                        optHere.Checked = True
                    Else
                        Me.cmbCostCenter.SelectedItem = Nothing
                        ' Si no esta en la lista hay que añadirlo porque el usuario actual no tiene permisos sobre ese centro
                        ' pero otro usuario lo ha asignado
                        Dim oCenter As roBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, IDParenCenter, False)

                        If oCenter IsNot Nothing Then
                            Dim newItem As New DevExpress.Web.ListEditItem(oCenter.Name & ("*"), -2)
                            Me.cmbCostCenter.Items.Add(newItem)
                            Me.cmbCostCenter.SelectedItem = newItem
                            optHere.Checked = True
                        End If

                    End If
                Else
                    Me.cmbCostCenter.SelectedItem = Nothing
                End If
            End If

            If Me.cmbCostCenter.SelectedItem Is Nothing Then
                optHere.Checked = True
                optOneCost.Checked = False
                Me.cmbCostCenter.SelectedItem = Me.cmbCostCenter.Items.FindByValue(0)
            End If

            If Not optOneCost.Checked Then
                lblHereDesc.Text = Me.cmbCostCenter.SelectedItem.Text
            Else
                lblHereDesc.Text = ""
            End If

            If oPermissionCostCenter > Permission.None Then
                divCostopt1.Visible = True
                cmbCostCenter.Visible = True
            Else
                divCostopt1.Visible = False
                cmbCostCenter.Visible = False
            End If

            optHere.Enabled = False
            optOneCost.Enabled = False
            cmbCostCenter.Enabled = False
            If oPermissionCostCenter > Permission.Read AndAlso oPermissionGroup > Permission.Read AndAlso GetFeaturePermission(EmployeeFeatureAlias) = Permission.Admin Then
                cmbCostCenter.Enabled = True
                optHere.Enabled = True
                optOneCost.Enabled = True
            End If

            If bolCostCentersLicense Then
                divCostDetail.Style("display") = ""
                divCostDetailNoLicense.Style("display") = "none"
            Else
                divCostDetail.Style("display") = "none"
                divCostDetailNoLicense.Style("display") = ""
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        End Try

    End Sub

    Private Sub LoadGroupDataTab2(ByVal oParameters As EmployeeCallbackRequest)

        'Cargar los grid de kpis
        Dim dTblIndicators As DataTable
        dTblIndicators = Me.IndicatorsData(oParameters.ID)

        'Elimina les columnes que no fan falta mostrar al Grid de Puestos

        Dim htmlTGridIndicators As HtmlTable

        If dTblIndicators.Rows.Count = 0 Then
            Dim oRow As DataRow = dTblIndicators.NewRow()
            oRow("Name") = Me.Language.Translate("NoIndicators", Me.DefaultScope)
            dTblIndicators.Rows.Add(oRow)

            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...
            tblControl.Columns.Add("NomHeader")

            Dim nRowCtrl As DataRow = tblControl.NewRow
            nRowCtrl("NomCamp") = "Name"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = True
            nRowCtrl("NomHeader") = Me.Language.Translate("Indicators", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "Description"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = True
            nRowCtrl("NomHeader") = Me.Language.Translate("Description", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "IDGroup"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = False
            nRowCtrl("NomHeader") = Me.Language.Translate("IDGroup", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "IDIndicator"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = False
            nRowCtrl("NomHeader") = Me.Language.Translate("IDIndicator", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            htmlTGridIndicators = creaGridIndicators(dTblIndicators, tblControl, False)
        Else
            Dim tblControl As DataTable = New DataTable
            tblControl.Columns.Add("NomCamp") 'Nom del Camp
            tblControl.Columns.Add("NomParam") 'Parametre que es pasara
            tblControl.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...
            tblControl.Columns.Add("NomHeader")

            Dim nRowCtrl As DataRow = tblControl.NewRow
            nRowCtrl("NomCamp") = "Name"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = True
            nRowCtrl("NomHeader") = Me.Language.Translate("Indicators", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "Description"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = True
            nRowCtrl("NomHeader") = Me.Language.Translate("Description", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "IDGroup"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = False
            nRowCtrl("NomHeader") = Me.Language.Translate("IDGroup", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            nRowCtrl = tblControl.NewRow
            nRowCtrl("NomCamp") = "IDIndicator"
            nRowCtrl("NomParam") = ""
            nRowCtrl("Visible") = False
            nRowCtrl("NomHeader") = Me.Language.Translate("IDIndicator", Me.DefaultScope)
            tblControl.Rows.Add(nRowCtrl)

            htmlTGridIndicators = creaGridIndicators(dTblIndicators, tblControl, True)
        End If

        Me.gridIndicators.Controls.Add(htmlTGridIndicators)

        If tblAddInd.Visible = True Then
            If Me.bolKPIsLicense = False OrElse oPermissionKpi < Permission.Write OrElse oPermissionGroup < Permission.Write Then
                tblAddInd.Visible = False
            Else
                tblAddInd.Visible = True
                btnAddIndicator.Attributes("onclick") = "EditIndicators('" & oParameters.ID & "');"
            End If
        End If
    End Sub

    Private Sub LoadGroupDataTab4(ByVal oParameters As EmployeeCallbackRequest)
        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)
        If oGroup Is Nothing Then Exit Sub

        CompanyDocumentManagment.SetScope(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
        CompanyDocumentPendingManagment.LoadAlerts(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
    End Sub

    Private Sub LoadGroupDataTab6(ByVal oParameters As EmployeeCallbackRequest)
        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, True)
        If oGroup Is Nothing Then Exit Sub

        GroupDocumentManagment.SetScope(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
        GroupDocumentPendingManagment.LoadAlerts(oGroup.ID, DocumentType.Company, -2, ForecastType.Any)
    End Sub

    Private Sub LoadGroupDataTab5(ByVal oParameters As EmployeeCallbackRequest)
        aCompanyAccessAuthorizations.Attributes("onclick") = "ShowAccessAuthorizations(-1," & oParameters.ID & ");"
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
                    hTCell.Style("padding") = "3px"
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
                        If oRow("FieldCaption").ToString.StartsWith("_translate_") Then
                            hTCell.InnerText = Me.Language.Translate(oRow("FieldCaption").ToString, Me.DefaultScope)
                        Else
                            hTCell.InnerText = oRow("FieldCaption").ToString
                        End If
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
                            'Dim hAnchorRemove As New HtmlAnchor

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

                            'hAnchorRemove.Attributes("onclick") = strClickMove
                            'hAnchorRemove.Title = Me.Language.Translate("MoveEmployee", Me.DefaultScope) '"Mover empleado"
                            'hAnchorRemove.HRef = "javascript: void(0);"
                            'hAnchorRemove.InnerHtml = "<img src=""" & Me.Page.ResolveUrl("~/Base/Images/Grid/move.png") & """>"

                            hTCell.Controls.Add(hAnchorEdit)
                            If moveIcon = True Then
                                'Comprobamos si se tiene permisos sobre el empleado
                                Me.oPermissionMobility = Me.GetFeaturePermissionByEmployee(FeatureMobilityAlias, dTable.Rows(n)(0).ToString)

                                'If oPermissionMobility > Permission.Read Then
                                '    hTCell.Controls.Add(hAnchorRemove)
                                'End If
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

    Private Function saveCostCenters(ByVal oParameters As EmployeeCallbackRequest) As String
        Try
            Dim strErrorInfo As String = ""
            Dim strResponse As String = ""

            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, False)
            If oGroup Is Nothing Then Return "ERROR"

            If optHere.Checked Then
                oGroup.IDCenter = Nothing
            Else
                If Me.cmbCostCenter.SelectedItem IsNot Nothing AndAlso Me.cmbCostCenter.SelectedItem.Value > 0 Then
                    oGroup.IDCenter = Me.cmbCostCenter.SelectedItem.Value
                Else
                    oGroup.IDCenter = Nothing
                End If

            End If

            If API.EmployeeGroupsServiceMethods.SaveGroup(Me, oGroup, True) > 0 Then
                strErrorInfo = ""
            Else
                strErrorInfo = API.EmployeeGroupsServiceMethods.LastErrorText
            End If

            If strErrorInfo <> "" Then
                strResponse = "MESSAGE" &
                            "TitleKey=Error.SaveGroup.Title&" +
                            "DescriptionText=" + strErrorInfo + "&" +
                            "Option1TextKey=Error.SaveGroup.Option1Text&" +
                            "Option1DescriptionKey=Error.SaveGroup.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                            "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If

            Return strResponse
        Catch ex As Exception
            Return "ERROR"
        End Try

    End Function

    Private Function saveGroup(ByVal oParameters As EmployeeCallbackRequest) As String
        Try
            Dim strErrorInfo As String = ""
            Dim strResponse As String = ""

            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, False)
            If oGroup Is Nothing Then Return "ERROR"

            oGroup.Name = txtNameCompany.Text
            oGroup.DescriptionGroup = txtDescription.Text
            oGroup.Export = txtExport.Text

            If roTypes.Any2String(HelperSession.AdvancedParametersCache("Customization")).ToLower = "taif" Then
                If oGroup.ID.ToString = oGroup.Path Then
                    oGroup.CloseDate = Me.txtCloseDate.Value
                Else
                    oGroup.CloseDate = Nothing
                End If

                If oGroup.CloseDate IsNot Nothing AndAlso oGroup.CloseDate < API.ConnectorServiceMethods.GetFirstDate(Me.Page) Then
                    strErrorInfo = Me.Language.Translate("Error.CloseDate", Me.DefaultScope)
                End If
            Else
                oGroup.CloseDate = Nothing
            End If

            If strErrorInfo = String.Empty Then
                If Me.cmbWorkingZone.SelectedItem IsNot Nothing AndAlso Me.cmbWorkingZone.SelectedItem.Value > -1 Then
                    oGroup.IDZoneWorkingTime = Me.cmbWorkingZone.SelectedItem.Value
                Else
                    oGroup.IDZoneWorkingTime = Nothing
                End If

                If API.EmployeeGroupsServiceMethods.SaveGroup(Me, oGroup, True) > 0 Then
                    strErrorInfo = ""
                Else
                    strErrorInfo = API.EmployeeGroupsServiceMethods.LastErrorText
                End If
            End If

            If strErrorInfo <> "" Then
                strResponse = "MESSAGE" &
                            "TitleKey=Error.SaveGroup.Title&" +
                            "DescriptionText=" + strErrorInfo + "&" +
                            "Option1TextKey=Error.SaveGroup.Option1Text&" +
                            "Option1DescriptionKey=Error.SaveGroup.Option1Description&" +
                            "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                            "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If

            Return strResponse
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function

    Private Function saveCompanyFieldsGrid(ByVal oParameters As EmployeeCallbackRequest) As String
        Try
            Dim oUserField As roGroupUserField
            Dim intGroupID As Integer
            Dim oState As New Robotics.Base.VTBusiness.Group.roGroupState
            Dim bolSaveData As Boolean
            Dim dRowError As DataRow

            'Taula per acumular els errors
            Dim errTable As DataTable = New DataTable
            errTable.Columns.Add("FieldName")
            errTable.Columns.Add("FieldValue")
            errTable.Columns.Add("BeginEnd")
            errTable.Columns.Add("ErrorDescription")

            intGroupID = oParameters.ID

            'Carrega Grid Usuaris per comprobar el tipus de columna
            Dim dsUserFields As DataSet
            dsUserFields = API.EmployeeGroupsServiceMethods.GetUserFieldsDataset(Me, intGroupID)
            Dim dTbl As DataTable = dsUserFields.Tables(0)

            ' Guardo los UserFields del grupo actual
            bolSaveData = True
            Dim strFieldName As String

            Dim oFieldParameters() As String = oParameters.resultClientAction.Split("&")

            For Each cVars As String In oFieldParameters
                If cVars = String.Empty Then Continue For

                Dim fieldData() As String = cVars.Split("=")
                strFieldName = fieldData(0).Substring(4)

                If strFieldName.ToString.EndsWith("_@@Date@@_I") Then Continue For
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

                    oUserField = New roGroupUserField
                    oUserField.Definition = New roUserField

                    oUserField.FieldName = strFieldName
                    oUserField.Definition.Type = Types.GroupField
                    If (fieldData.Length > 1) Then
                        oUserField.FieldValue = fieldData(1)
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

                    'Busca el FieldType dintre de la BBDD
                    Dim dRow As DataRow() = dTbl.Select("FieldName = '" & oUserField.FieldName & "'")
                    If dRow.Length > 0 Then
                        oUserField.Definition.FieldType = dRow(0).Item("Type")
                        ''oUserField.FieldCaption = dRow(0).Item("FieldCaption")
                    Else ' Si no el troba, possa String (0)
                        oUserField.Definition.FieldType = 0
                    End If

                    Select Case oUserField.Definition.FieldType
                        Case FieldTypes.tDate
                            If IsDate(oUserField.FieldValue) Then
                                oUserField.FieldValue = CDate(oUserField.FieldValue).Date
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

                        Case FieldTypes.tTime
                            If oUserField.FieldValue IsNot Nothing AndAlso oUserField.FieldValue <> "" Then
                                If CStr(oUserField.FieldValue).IndexOf(":") >= 0 Then
                                    oUserField.FieldValue = roConversions.ConvertTimeToHours(CStr(oUserField.FieldValue))
                                Else
                                    'Graba a la taula de errors
                                    dRowError = errTable.NewRow
                                    dRowError("FieldName") = oUserField.FieldName
                                    dRowError("FieldValue") = oUserField.FieldValue
                                    dRowError("ErrorDescription") = Me.Language.Translate("InvalidTime", Me.DefaultScope) '"No es una fecha valida"
                                    errTable.Rows.Add(dRowError)
                                    bolSaveData = False
                                End If
                            Else
                                oUserField.FieldValue = Nothing
                            End If
                            ''If IsDate(oUserField.FieldValue) Then
                            ''    oUserField.FieldValue = CDate(oUserField.FieldValue)
                            ''    oUserField.FieldValue = New Date(1900, 1, 1, CDate(oUserField.FieldValue).Hour, CDate(oUserField.FieldValue).Minute, 0)
                            ''Else
                            ''    If oUserField.FieldValue = "" Then
                            ''        oUserField.FieldValue = Nothing
                            ''    Else
                            ''        'Graba a la taula de errors
                            ''        dRowError = errTable.NewRow
                            ''        dRowError("FieldName") = oUserField.FieldName
                            ''        dRowError("FieldValue") = oUserField.FieldValue
                            ''        dRowError("ErrorDescription") = Me.Language.Translate("InvalidTime", Me.DefaultScope) '"No es una fecha valida"
                            ''        errTable.Rows.Add(dRowError)
                            ''        bolSaveData = False
                            ''    End If
                            ''End If

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
                                oUserField.FieldValue = CDbl(oUserField.FieldValue.ToString.Replace(".", HelperWeb.GetDecimalDigitFormat()))
                                'oUserField.FieldValue = CDbl(oUserField.FieldValue)
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
                                Dim oUserFieldInfo As roUserField = API.UserFieldServiceMethods.GetUserField(Me, oUserField.FieldName, Types.GroupField, False, True)
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
                            If oUserField.FieldValue = "" OrElse IsDate(oUserField.FieldValue) Then

                                If oUserField.FieldValue.ToString <> "" Then oUserField.FieldValue = Format(CDate(oUserField.FieldValue), "yyyy/MM/dd")
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

                                ' '' ''Else
                                ' '' ''    'Graba a la taula de errors
                                ' '' ''    dRowError = errTable.NewRow
                                ' '' ''    dRowError("FieldName") = oUserField.FieldName
                                ' '' ''    dRowError("BeginEnd") = IIf(bolIsBeginPeriod, "BeginPeriod", "EndPeriod")
                                ' '' ''    dRowError("FieldValue") = oUserField.FieldValue
                                ' '' ''    dRowError("ErrorDescription") = Me.Language.Translate("InvalidLink", Me.DefaultScope) '"No es una fecha valida"
                                ' '' ''    errTable.Rows.Add(dRowError)
                                ' '' ''    bolSaveData = False
                            End If

                    End Select

                    If bolSaveData AndAlso oUserField.FieldName <> String.Empty Then
                        If Not API.EmployeeGroupsServiceMethods.SaveUserField(Me, intGroupID, oUserField.FieldName, oUserField.FieldValue, True) Then
                            bolSaveData = False
                            'Graba a la taula de errors
                            dRowError = errTable.NewRow
                            dRowError("FieldName") = oUserField.FieldName
                            dRowError("FieldValue") = oUserField.FieldValue
                            dRowError("ErrorDescription") = roWsUserManagement.SessionObject.States.EmployeeGroupUserFieldState.ErrorText
                            errTable.Rows.Add(dRowError)
                        End If
                    End If

                End If

            Next

            Dim Columns As String() = {"FieldCaption", "Value"}

            'Crea el grid, en mode edicio o normal segons errors
            If Not bolSaveData Then

                Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oParameters.ID, False)

                If oGroup Is Nothing Then Return ""

                'Carrega Grid Usuaris
                dsUserFields = API.EmployeeGroupsServiceMethods.GetUserFieldsDataset(Me, oParameters.ID)

                Dim dTblEmp As DataTable = dsUserFields.Tables(0)

                'dTblEmp.Columns.Remove("Type")
                Dim htmlTGrid As HtmlTable = creaFieldsGrid(dTblEmp, Columns, dTbl.Columns("FieldName").ColumnName, True, errTable)
                Me.divCompanyFields.Controls.Add(htmlTGrid)
                'Carrega el anchor de editar el grid
                Me.btn1FieldsCompany.Visible = False
                Me.btn2FieldsCompany.Visible = True
                Me.btn3FieldsCompany.Visible = True

                Me.editFieldsGrid.Attributes("onclick") = "editGroupFieldsGrid('" & oGroup.ID & "')"
                Me.saveFieldsGrid.Attributes("onclick") = "saveGroupFieldsGrid('" & oGroup.ID & "')"
                Me.cancelFieldsGrid.Attributes("onclick") = "cancelGroupFieldsGrid('" & oGroup.ID & "')"

                Return "ERROR"
            Else
                Return ""
            End If
        Catch ex As Exception
            Dim strResponse As String

            strResponse = "MESSAGE" &
                          "TitleKey=SaveGroupUserFields.Error.Title&" +
                          "DescriptionText=" + ex.Message + "&" +
                          "Option1TextKey=SaveGroupUserFields.Error.Option1Text&" +
                          "Option1DescriptionKey=SaveGroupUserFields.Error.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

            Response.Write(strResponse)

        End Try

        Return ""
    End Function

    ''' <summary>
    ''' Carrega el Grid "GENERAL" en Mode "edicio"
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadCompanyGridEdit(ByVal oGroup As roGroup)

        Try
            If oGroup Is Nothing Then Exit Sub

            'Carrega Grid Usuaris
            Dim dsUserFields As DataSet
            dsUserFields = API.EmployeeGroupsServiceMethods.GetUserFieldsDataset(Me, oGroup.ID)

            Dim dTbl As DataTable = dsUserFields.Tables(0)
            'dTbl.Columns.Remove("Type")
            Dim Columns() As String = {"FieldCaption", "Value"}
            Dim htmlTGrid As HtmlTable = creaFieldsGrid(dTbl, Columns, dTbl.Columns(1).ColumnName, True)
            Me.divCompanyFields.Controls.Add(htmlTGrid)

            'Carrega el anchor de editar el grid
            Me.btn1FieldsCompany.Visible = False
            Me.btn2FieldsCompany.Visible = True
            Me.btn3FieldsCompany.Visible = True
            Me.editFieldsGrid.Attributes("onclick") = "editGroupFieldsGrid('" & oGroup.ID & "')"
            Me.saveFieldsGrid.Attributes("onclick") = "saveGroupFieldsGrid('" & oGroup.ID & "')"
            Me.cancelFieldsGrid.Attributes("onclick") = "cancelGroupFieldsGrid('" & oGroup.ID & "')"
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

End Class