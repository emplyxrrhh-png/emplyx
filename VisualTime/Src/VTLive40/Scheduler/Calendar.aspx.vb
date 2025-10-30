Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Calendar
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

        Me.InsertExtraJavascript("DateSelector", "~/Scheduler/Scripts/DateSelector.js")
        Me.InsertExtraJavascript("CalendarV2", "~/Scheduler/Scripts/CalendarV2.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraCssIncludes("~/Scheduler/Styles/calendarV2.css", Me.Page)

        Dim tmpPassport As roPassportTicket = WLHelperWeb.CurrentPassport

        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Calendar") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If
        Me.oPermission = Me.GetFeaturePermission("Calendar")

        If oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If

        Dim strAux As String = "~/Employee/EmployeeSelector?feature=Calendar&pageName=calendar&config=100&unionType=or&advancedMode=1&advancedFilter=1&allowAll=0&allowNone=0"
        Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermissionPlan = Me.GetFeaturePermission("Calendar.Scheduler")
        Me.oPermissionHigh = Me.GetFeaturePermission("Calendar.Highlight")

        If API.LicenseServiceMethods.FeatureIsInstalled("Feature\CostControl") Then
            oCostCenterPermission = Me.GetFeaturePermission("BusinessCenters.Punches")
        End If

        Dim bolMultipleShiftsLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultipleShifts")
        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")
        Dim bolSaasPremium As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")
        Dim bolTelecommuting As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")
        Dim sVTEdition As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.Edition"))

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

            'Dim oTreeStateGrid = HelperWeb.roSelector_GetTreeState("ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpCalendar")

            Dim selEmployees As String = String.Empty
            Dim selEmployeeFilters As String = String.Empty
            Dim selEmployeeUserFields As String = String.Empty

            If Not hdnCalendarConfig.Contains("EmpSelected") Then hdnCalendarConfig.Add("EmpSelected", selEmployees) Else hdnCalendarConfig("EmpSelected") = selEmployees
            If Not hdnCalendarConfig.Contains("EmpFilters") Then hdnCalendarConfig.Add("EmpFilters", selEmployeeFilters) Else hdnCalendarConfig("EmpFilters") = selEmployeeFilters
            If Not hdnCalendarConfig.Contains("EmpUserFields") Then hdnCalendarConfig.Add("EmpUserFields", selEmployeeUserFields) Else hdnCalendarConfig("EmpUserFields") = selEmployeeUserFields

            LoadShiftGroups()
            If Not Me.IsPostBack And Not Me.IsCallback AndAlso Request.QueryString.Count > 0 Then
                ProcessQueryString()
            End If
        End If
    End Sub

    Protected Sub ProcessQueryString()
        If Not Request.QueryString("idemployee") Is Nothing Then
            Dim idemployee As String = Request.QueryString("idemployee")
            Dim treePathE As String = String.Empty
            If Me.HasFeaturePermissionByEmployee("Employees", Permission.Read, idemployee) Then
                treePathE = roTools.GetEmployeeGroupPath(idemployee)
            End If
            HelperWeb.roSelector_SetSelection("B" & idemployee, treePathE, "ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpCalendar")

            Dim selEmployees As String = "B" & idemployee
            Dim selEmployeeFilters As String = "11110"
            Dim selEmployeeUserFields As String = ""

            If Not hdnCalendarConfig.Contains("EmpSelected") Then hdnCalendarConfig.Add("EmpSelected", selEmployees) Else hdnCalendarConfig("EmpSelected") = selEmployees
            If Not hdnCalendarConfig.Contains("EmpFilters") Then hdnCalendarConfig.Add("EmpFilters", selEmployeeFilters) Else hdnCalendarConfig("EmpFilters") = selEmployeeFilters
            If Not hdnCalendarConfig.Contains("EmpUserFields") Then hdnCalendarConfig.Add("EmpUserFields", selEmployeeUserFields) Else hdnCalendarConfig("EmpUserFields") = selEmployeeUserFields

        ElseIf Not Request.QueryString("idgroup") Is Nothing Then
            Dim idgroup As String = Request.QueryString("idgroup")
            Dim treePath As String = String.Empty
            If Me.HasFeaturePermissionByGroup("Employees", Permission.Read, idgroup) Then
                treePath = roTools.GetGroupPath(idgroup)
            End If
            HelperWeb.roSelector_SetSelection("A" & idgroup, treePath, "ctl00_contentMainBody_PopupSelectorEmployees_ASPxPanel3_objContainerTreeV3_roTrees1EmpCalendar")

            Dim selEmployees As String = "A" & idgroup
            Dim selEmployeeFilters As String = "11110"
            Dim selEmployeeUserFields As String = ""

            If Not hdnCalendarConfig.Contains("EmpSelected") Then hdnCalendarConfig.Add("EmpSelected", selEmployees) Else hdnCalendarConfig("EmpSelected") = selEmployees
            If Not hdnCalendarConfig.Contains("EmpFilters") Then hdnCalendarConfig.Add("EmpFilters", selEmployeeFilters) Else hdnCalendarConfig("EmpFilters") = selEmployeeFilters
            If Not hdnCalendarConfig.Contains("EmpUserFields") Then hdnCalendarConfig.Add("EmpUserFields", selEmployeeUserFields) Else hdnCalendarConfig("EmpUserFields") = selEmployeeUserFields
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
            divCalendarTitle.InnerHtml = ex.Message.ToString & " " & ex.StackTrace.ToString
        End Try
    End Sub

    Private Function GetBarButtons(ByVal oPermissionPlan As Permission, ByVal oPermissionHigh As Permission, ByVal oCostCenterPermission As Permission, ByVal aTab As Integer, ByRef strMessage As String) As Boolean
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = Nothing
            If aTab = 0 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Review", WLHelperWeb.CurrentPassportID)
            Else
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Planification", WLHelperWeb.CurrentPassportID)
            End If

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, "-1", Me.Language, Me.DefaultScope, "Scheduler")
            divBarButtons.InnerHtml = oResponse.BarButtons

            Return True
        Catch ex As Exception
            strMessage = ex.Message.ToString & " " & ex.StackTrace.ToString
            Return False
        End Try
    End Function

    Private Function CreateTabs(ByRef oPermissionPlan As Permission) As HtmlTable
        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_Revision", Me.Language.Translate("btnReview", Me.DefaultScope), "bTab")
        oTabButtons(0).ID = "TABBUTTON_InputOutputs"
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        If oPermissionPlan > Permission.None Then

            oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_Calendar", Me.Language.Translate("btnPlanification", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(1))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
        End If

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeCalendarTabs(0);")
        If oPermissionPlan > Permission.None Then
            oTabButtons(1).Attributes.Add("OnClick", "javascript: changeCalendarTabs(1);")
        End If

        'Dim actualTab As String = HelperWeb.GetCookie("CalendarType")
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

#Region "Aux Selector Horarios"

    Private Sub LoadShiftGroups()
        cmbShiftGroups.Items.Clear()

        Dim dTbl As DataTable = API.ShiftServiceMethods.GetShiftGroupsPlanification(Page)
        For Each grupo As DataRow In dTbl.Rows
            Dim groupName = If(grupo("Name").ToString().Trim().ToUpper().Equals("(NINGUNO)"), Language.Translate("GeneralShift", DefaultScope), grupo("Name"))
            cmbShiftGroups.Items.Add(groupName, grupo("ID"))
        Next

        cmbShiftGroups.SelectedIndex = 0

    End Sub

    Protected Sub ASPxSelectorShiftsCallbackPanelContenido_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxSelectorShiftsCallbackPanelContenido.Callback
        Dim groupId = roTypes.Any2Integer(e.Parameter)
        CreateShiftsDynamic(groupId)
    End Sub

    Private Sub CreateShiftsDynamic(groupId As Integer)
        Dim dTbl As DataTable = Nothing ' API.ShiftServiceMethods.GetShiftsFromGroup(Page, groupId)
        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")

        dTbl = API.ShiftServiceMethods.GetShiftsPlanification(Nothing, groupId)
        Dim shiftsList = dTbl.Select("", "ShiftType")

        If (dTbl.Rows.Count > 0) Then
            Dim actualType = dTbl.Rows(0)("ShiftType")
            For Each shift As DataRow In shiftsList
                If (shift("Name").ToString().Equals("???")) Then Continue For
                Dim shiftType = shift("ShiftType")
                If (Not actualType.Equals(shiftType)) Then
                    divShiftsServer.Controls.Add(EmptyShiftDiv())
                End If
                Dim shiftDiv = New HtmlGenericControl("div")

                'shiftDiv.ID = "Shift_" & shift("ID")
                shiftDiv.Attributes("class") = "shiftSelector"
                shiftDiv.Attributes("draggable") = "true"
                'shiftDiv.Attributes("draggable") = "true"
                Dim auxColor As Drawing.Color = Drawing.ColorTranslator.FromWin32(shift("Color"))
                Dim txtColor As Drawing.Color = InvertMeAColour(auxColor)
                Dim oHTMLColor As String = HexConverter(auxColor)
                Dim oHTMLColorText As String = HexConverter(txtColor)

                shiftDiv.Attributes("data-IDShift") = shift("ID")
                shiftDiv.Attributes("data-Name") = shift("Name")
                shiftDiv.Attributes("data-ShortName") = roTypes.Any2String(shift("ShortName"))
                shiftDiv.Attributes("data-AllowFloatingData") = IIf(roTypes.Any2Boolean(shift("AllowFloatingData")), "1", "0")
                shiftDiv.Attributes("data-AllowComplementary") = IIf(roTypes.Any2Boolean(shift("AllowComplementary")), "1", "0")
                shiftDiv.Attributes("data-AllowAssignments") = IIf(bolHRSchedulingLicense AndAlso (roTypes.Any2Integer(shift("Assignments")) > 0), "1", "0")

                If roTypes.Any2Boolean(shift("IsFloating")) Then
                    shiftDiv.Attributes("data-ShiftType") = 1
                    shiftDiv.Attributes("data-StartHour") = roTypes.Any2DateTime(shift("StartFloating")).ToString("yyyy/MM/dd HH:mm")
                    shiftDiv.Attributes("data-EndHour") = ""
                Else
                    If roTypes.Any2Integer(shift("ShiftType")) = 1 Then
                        shiftDiv.Attributes("data-StartHour") = roTypes.Any2DateTime(shift("StartLimit")).ToString("yyyy/MM/dd HH:mm")
                        shiftDiv.Attributes("data-EndHour") = roTypes.Any2DateTime(shift("EndLimit")).ToString("yyyy/MM/dd HH:mm")
                        shiftDiv.Attributes("data-ShiftType") = 0
                    Else
                        shiftDiv.Attributes("data-ShiftType") = IIf(roTypes.Any2Boolean(shift("AreWorkingDays")), 2, 3)
                        shiftDiv.Attributes("data-StartHour") = DateTime.Now.Date.ToString("yyyy/MM/dd HH:mm")
                        shiftDiv.Attributes("data-EndHour") = ""
                    End If
                End If

                shiftDiv.Attributes("data-ShiftColor") = oHTMLColor
                shiftDiv.Attributes("data-ShiftHours") = roTypes.Any2Time(roTypes.Any2Double(shift("ExpectedWorkingHours"))).Minutes

                shiftDiv.Attributes("data-AdvParameters") = SerializeShiftAdvancedParatemers(roTypes.Any2String(shift("AdvancedParameters")))

                shiftDiv.Attributes("title") = shift("Name")
                shiftDiv.InnerHtml = roTypes.Any2String(shift("ShortName"))

                shiftDiv.Style.Add("background-color", oHTMLColor)
                shiftDiv.Style.Add("color", oHTMLColorText)

                divShiftsServer.Controls.Add(shiftDiv)
                actualType = shiftType
            Next
        End If
    End Sub

    Public Shared Function SerializeShiftAdvancedParatemers(sParameters As String) As String
        Dim oRet As New List(Of roShiftAdvParameters)
        Dim aTemp As String() = {}
        Dim aTemp2 As String() = {}
        Try
            If sParameters.Contains("[") AndAlso sParameters.Contains("]") Then
                aTemp = sParameters.Replace(vbLf, "").Split("]")

                For Each sParameter As String In aTemp
                    If sParameter.Trim.Contains("[") Then
                        aTemp2 = sParameter.Split("=")
                        If aTemp2.Count = 2 Then
                            oRet.Add(New roShiftAdvParameters(aTemp2(0).Trim, aTemp2(1).Replace("[", "").Trim))
                        End If
                    End If
                Next
            End If
        Catch ex As Exception

        End Try
        Return roJSONHelper.SerializeNewtonSoft(oRet.ToArray)
    End Function

    Private Shared Function HexConverter(c As System.Drawing.Color) As String
        Return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2")
    End Function

    Private Function InvertMeAColour(ColourToInvert As Drawing.Color) As Drawing.Color
        Return Drawing.Color.FromArgb(255 - ColourToInvert.R, 255 - ColourToInvert.G, 255 - ColourToInvert.B)
    End Function

    Protected Function EmptyShiftDiv() As HtmlGenericControl
        Dim shiftDiv = New HtmlGenericControl("div")
        shiftDiv.Attributes("class") = "shiftSelector"
        Return shiftDiv
    End Function

#End Region

End Class