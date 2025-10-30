Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Budget
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackBarButtonsRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Dim oPermission As Permission = Permission.None ' Me.GetFeaturePermission("Calendar")
    Dim oPermissionPlan As Permission = Permission.None ' Me.GetFeaturePermission("Calendar.Scheduler")
    Dim oPermissionHigh As Permission = Permission.None ' Me.GetFeaturePermission("Calendar.Highlight")
    Dim oCostCenterPermission As Permission = Permission.None

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js")

        Me.InsertExtraJavascript("DateSelector", "~/AIScheduler/Scripts/DateSelector.js")
        Me.InsertExtraJavascript("Budgets", "~/AIScheduler/Scripts/Budgets.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraCssIncludes("~/AIScheduler/Styles/Budgets.css", Me.Page)

        Dim tmpPassport As roPassportTicket = WLHelperWeb.CurrentPassport

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Calendar") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If
        Me.oPermission = Me.GetFeaturePermission("Calendar")

        If oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Me.oPermissionPlan = Me.GetFeaturePermission("Calendar.Scheduler")
        Me.oPermissionHigh = Me.GetFeaturePermission("Calendar.Highlight")

        If API.LicenseServiceMethods.FeatureIsInstalled("Feature\CostControl") Then
            oCostCenterPermission = Me.GetFeaturePermission("BusinessCenters.Punches")
        End If

        Dim bolMultipleShiftsLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultipleShifts")
        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")
        Dim bolSaasPremium As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")
        Dim sVTEdition As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.Edition"))
        Dim bolTelecommuting As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")

        hdnReportsTitleReads.Value = Me.Language.Translate("ReportsTitleReads", Me.DefaultScope)
        hdnReportsTitleShifts.Value = Me.Language.Translate("ReportsTitleShifts", Me.DefaultScope)

        LoadCalendarTabs(oPermissionPlan)

        If Not IsPostBack Then
            If Not hdnCalendarConfig.Contains("TelecommuteEnabled") Then hdnCalendarConfig.Add("TelecommuteEnabled", bolTelecommuting) Else hdnCalendarConfig("TelecommuteEnabled") = bolTelecommuting
            If Not hdnCalendarConfig.Contains("Language") Then hdnCalendarConfig.Add("Language", WLHelperWeb.CurrentPassport.Language.Key) Else hdnCalendarConfig("Language") = WLHelperWeb.CurrentPassport.Language.Key
            If Not hdnCalendarConfig.Contains("HRScheduling") Then hdnCalendarConfig.Add("HRScheduling", bolHRSchedulingLicense) Else hdnCalendarConfig("HRScheduling") = bolHRSchedulingLicense
            If Not hdnCalendarConfig.Contains("SaasPremium") Then hdnCalendarConfig.Add("SaasPremium", bolSaasPremium) Else hdnCalendarConfig("SaasPremium") = bolSaasPremium
            If Not hdnCalendarConfig.Contains("VTLiveEdition") Then hdnCalendarConfig.Add("VTLiveEdition", sVTEdition) Else hdnCalendarConfig("VTLiveEdition") = sVTEdition

            If Not hdnCalendarConfig.Contains("hdnReportsTitleReads") Then hdnCalendarConfig.Add("hdnReportsTitleReads", Me.hdnReportsTitleReads.ClientID) Else hdnCalendarConfig("hdnReportsTitleReads") = Me.hdnReportsTitleReads.ClientID
            If Not hdnCalendarConfig.Contains("hdnReportsTitleShifts") Then hdnCalendarConfig.Add("hdnReportsTitleShifts", Me.hdnReportsTitleShifts.ClientID) Else hdnCalendarConfig("hdnReportsTitleShifts") = Me.hdnReportsTitleShifts.ClientID

            LoadShiftGroups()

            Dim strOrgChartFilter As String = HelperWeb.GetCookie("Budget_OrgChartFilter")
            If (strOrgChartFilter.Trim <> String.Empty) Then
                'TODO Replace roSecurityNodes For Something
                Dim oNode As roSecurityNode = Nothing 'API.SecurityChartServiceMethods.GetSecurityChartNodeSingle(strOrgChartFilter, Me.Page)
                If oNode IsNot Nothing Then Me.txtOrgChartSelection.Text = oNode.Name
            Else
                Me.txtOrgChartSelection.Text = Me.Language.Translate("OrgChart.NoSelection", Me.DefaultScope)
            End If
        End If
    End Sub

#Region "Ribbon Bar"

    Protected Sub ASPxCallbackPanelBarButtons_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelBarButtons.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackBarButtonsRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "RELOADBARBUTTONS"
                bRet = GetBarButtons(oPermissionPlan, oPermissionHigh, oCostCenterPermission, oParameters.aTab, responseMessage)
                If Not bRet Then
                    ASPxCallbackPanelBarButtons.JSProperties.Add("cpMessage", responseMessage)
                    ASPxCallbackPanelBarButtons.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxCallbackPanelBarButtons.JSProperties.Add("cpResult", "OK")
                End If

                ASPxCallbackPanelBarButtons.JSProperties.Add("cpAction", "RELOADBARBUTTONS")

        End Select
    End Sub

    ''' <summary>
    ''' Carga el Tab superior
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadCalendarTabs(ByRef oPermissionPlan As Permission)
        Try
            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(oPermissionPlan))
            oMainDiv.Controls.Add(oButtonsDiv)

            divCalendarTabs.Controls.Add(oMainDiv)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "Budgets.aspx::LoadCalendarTabs", ex)
        End Try
    End Sub

    Private Function GetBarButtons(ByVal oPermissionPlan As Permission, ByVal oPermissionHigh As Permission, ByVal oCostCenterPermission As Permission, ByVal aTab As Integer, ByRef strMessage As String) As Boolean
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = Nothing
            If aTab = 0 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AISchedule\Budget\Definition", WLHelperWeb.CurrentPassportID)
            ElseIf aTab = 1 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AISchedule\Budget\Schedule", WLHelperWeb.CurrentPassportID)
            ElseIf aTab = 2 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AISchedule\Budget\Definition", WLHelperWeb.CurrentPassportID)
            End If

            Dim destDiv As HtmlGenericControl = roTools.BuildCentralBar(guiActions, "-1", Me.Language, Me.DefaultScope, "Scheduler")
            divBarButtons.Controls.Add(destDiv)

            Return True
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "Budgets.aspx::GetBarButtons", ex)
            strMessage = "Unkown error"
            Return False
        End Try
    End Function

    Private Function CreateTabs(ByRef oPermissionPlan As Permission) As HtmlTable
        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_Definition", Me.Language.Translate("btnReview", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_Calendar", Me.Language.Translate("btnPlanification", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_OrgCalendar", Me.Language.Translate("btnCalendar", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(2))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeBudgetTabs(0);")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeBudgetTabs(1);")
        oTabButtons(2).Attributes.Add("OnClick", "javascript: changeBudgetTabs(2);")

        'Dim actualTab As String = HelperWeb.GetCookie("BudgetType")
        'If actualTab Is Nothing OrElse actualTab = "" Then actualTab = "0"

        'If Not oTabButtons(CInt(actualTab)) Is Nothing Then
        '    oTabButtons(CInt(actualTab)).Attributes("class") = "bTab-active"
        'Else
        '    actualTab = "0"
        '    oTabButtons(CInt(actualTab)).Attributes("class") = "bTab-active"
        'End If

        Return hTableButtons ' Retorna el HTMLTable

    End Function

    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

#End Region

#Region "Aux Selector Unidades productivas"

    Private Sub LoadShiftGroups()
        cmbProductiveUnits.Items.Clear()
        cmbProductiveUnits.ValueType = GetType(Integer)
        cmbProductiveUnits.Items.Add(Me.Language.Translate("Budget.SelectRow", Me.DefaultScope), 0)

        Dim dTbl As Generic.List(Of roProductiveUnit) = AISchedulingServiceMethods.GetProductiveUnits(Me.Page)
        For Each oPUnit As roProductiveUnit In dTbl
            cmbProductiveUnits.Items.Add(oPUnit.Name, oPUnit.ID)
        Next

        cmbProductiveUnits.SelectedIndex = 0

    End Sub

    Protected Sub ASPxProductiveUnitSelector_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxProductiveUnitSelector.Callback
        Dim productiveUnitId = roTypes.Any2Integer(e.Parameter)
        CreateModesDynamic(productiveUnitId)
    End Sub

    Private Sub CreateModesDynamic(productiveUnitId As Integer)

        Dim oPUnit As roProductiveUnit = AISchedulingServiceMethods.GetProductiveUnitById(Me.Page, productiveUnitId, False)

        If (oPUnit.UnitModes IsNot Nothing AndAlso oPUnit.UnitModes.Count > 0) Then
            For Each oMode As roProductiveUnitMode In oPUnit.UnitModes
                Dim pUnitModeDiv = New HtmlGenericControl("div")

                pUnitModeDiv.Attributes("class") = "pUnitSelector"

                Dim oHTMLColor As String = oMode.HtmlColor
                Dim oHTMLColorText As String = HexConverter(InvertMeAColour(Drawing.ColorTranslator.FromHtml(oMode.HtmlColor)))

                pUnitModeDiv.Attributes("data-IDMode") = oMode.ID
                pUnitModeDiv.Attributes("data-IDProductiveUnit") = productiveUnitId
                pUnitModeDiv.Attributes("data-Name") = oMode.Name
                pUnitModeDiv.Attributes("data-ShortName") = oMode.ShortName

                pUnitModeDiv.Attributes("data-HtmlColor") = oMode.HtmlColor
                pUnitModeDiv.Attributes("data-CostValue") = oMode.CostValue.ToString().Replace(HelperWeb.GetDecimalDigitFormat(), ".")

                pUnitModeDiv.Attributes("title") = oMode.Name
                pUnitModeDiv.InnerHtml = oMode.Name

                pUnitModeDiv.Style.Add("background-color", oHTMLColor)
                pUnitModeDiv.Style.Add("color", oHTMLColorText)

                divPUnitsServer.Controls.Add(pUnitModeDiv)
            Next
        Else
            divPUnitsServer.Controls.Add(EmptyShiftDiv())
        End If
    End Sub

    Private Shared Function HexConverter(c As System.Drawing.Color) As String
        Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
    End Function

    Private Function InvertMeAColour(ColourToInvert As Drawing.Color) As Drawing.Color
        Return Drawing.Color.FromArgb(255 - ColourToInvert.R, 255 - ColourToInvert.G, 255 - ColourToInvert.B)
    End Function

    Protected Function EmptyShiftDiv() As HtmlGenericControl
        Dim shiftDiv = New HtmlGenericControl("div")
        shiftDiv.Attributes("class") = "pUnitSelector maxUnitWidth"
        shiftDiv.InnerHtml = Me.Language.Translate("Budget.SelectBudgtRow", Me.DefaultScope)
        Return shiftDiv
    End Function

#End Region

End Class