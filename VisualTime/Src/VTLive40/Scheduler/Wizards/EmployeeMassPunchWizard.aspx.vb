Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTSelectorManager
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_EmployeeMassPunchWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Punches.Punches"

#Region "Declarations"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("EMPW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("EMPW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("EMPW_iCurrentTask") = value
        End Set
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("ErrorDescription") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        Dim oParameter As String = HelperSession.AdvancedParametersCache("MassPunchWizardAvailable")
        If Me.oPermission >= Permission.Write AndAlso roTypes.Any2Integer(oParameter) = 1 Then
            If Not Me.IsPostBack Then
                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
                Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text

                Me.intActivePage = 0

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpEmployeeMassPunchWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpEmployeeMassPunchWizardGrid")
                Me.hdnEmployeesSelected.Value = String.Empty
                Me.hdnFilter.Value = "11110"
                Me.hdnFilterUser.Value = String.Empty

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True

                Me.LoadPunchesData()
            Else

                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.divStep2.Style("display") <> "none" Then Me.intActivePage = 2
                If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
                If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4
                If Me.divStep5.Style("display") <> "none" Then Me.intActivePage = 5

            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub LoadPunchesData()
        cmbDetailsType.Items.Clear()
        cmbDetailsType.Enabled = True
        cmbDetailsType.ReadOnly = False
        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.InDet", Me.DefaultScope), Integer.Parse(PunchStatus.Indet_)))

        cmbDetailsType.SelectedIndex = 0
        txtPunchDate.Date = Now
        txtDetailsTime.Text = Date.Now.ToString("HH:mm")

        Dim tbCauses As DataTable = CausesServiceMethods.GetCauses(Me, , True)
        Dim oRow As DataRow = tbCauses.NewRow
        oRow("ID") = 0
        oRow("Name") = ""
        oRow("AllowInputFromReader") = True
        tbCauses.Rows.InsertAt(oRow, 0)

        cmbCause.DataSource = tbCauses
        cmbCause.TextField = "Name"
        cmbCause.ValueField = "ID"
        Try
            cmbCause.DataBind()
            cmbCause.SelectedIndex = 0
        Catch ex As Exception
        End Try

        cmbTerminal.Items.Clear()
        Dim terminalsList As roTerminalList = API.TerminalServiceMethods.GetTerminals(Me)
        For Each terminal As roTerminal In terminalsList.Terminals
            cmbTerminal.Items.Add(New ListEditItem(terminal.Description, terminal.ID))
        Next
        cmbTerminal.SelectedIndex = 0

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
            Me.intActivePage += 1

            'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
            If Me.intActivePage = 1 Then Me.intActivePage += 1

            Me.PageChange(intOldPage, Me.intActivePage)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage
        Me.intActivePage -= 1

        'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
        If Me.intActivePage = 1 Then Me.intActivePage -= 1

        Me.PageChange(intOldPage, Me.intActivePage)

    End Sub

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        CloseWizard()
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckPage(Me.intActivePage))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.InsertEmployeeMassPunch()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                            Case 2
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                                If oTask.ErrorCode <> String.Empty Then
                                    ErrorDescription = oTask.ErrorCode
                                Else
                                    ErrorDescription = String.Empty
                                End If
                            Case 3
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                End If
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If strMsg <> "" Then bolRet = False

            Case 2

                ' Hemos de tener algún empleado seleccionado
                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page2.NoEmployeeSelected", Me.DefaultScope)
                Else
                    If Me.hdnEmployeesSelected.Value.Split(",").Length = 1 Then
                        If Me.hdnEmployeesSelected.Value.Substring(1) = Me.intIDEmployee Then
                            strMsg = Me.Language.Translate("CheckPage.Page2.IncorrectSelection", Me.DefaultScope)
                        End If
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case 3
                If Me.cmbTerminal.SelectedItem Is Nothing OrElse txtPunchDate.Value Is Nothing Or txtDetailsTime.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPunchData", Me.DefaultScope)
                Else
                    Dim xDate As DateTime
                    Dim sDate As String = Me.txtPunchDate.Date.ToString("dd/MM/yyyy") & " " & Me.txtDetailsTime.Text
                    Try
                        xDate = DateTime.ParseExact(sDate, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.CurrentCulture)
                        If xDate > DateTime.Now Then
                            strMsg = Me.Language.Translate("CheckPage.Page3.PunchFutureDate", Me.DefaultScope)
                        End If
                    Catch ex As Exception
                        strMsg = Me.Language.Translate("CheckPage.Page3.PunchFutureDate", Me.DefaultScope)
                    End Try
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case 4
                If cmbDetailsType.SelectedItem Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page4.IncorrectPunchData", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg
            Case 5
                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1

            Case 2
                ' Desactivar el iframe del selector de grupos
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
        End Select

        Select Case intActivePage
            Case 2
                ' Inicializar selección del selector de empleados
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmpEmployeeMassPunchWizard&FeatureAlias=Calendar.Punches&PrefixCookie=objContainerTreeV3_treeEmpEmployeeMassPunchWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeeSelector.Disabled = False
            Case 4
                cmbDetailsType.SelectedItem = cmbDetailsType.Items.FindByValue("0")
                If cmbTerminal.Items(cmbTerminal.SelectedIndex) IsNot Nothing Then

                    Dim oTerminal As roTerminal = API.TerminalServiceMethods.GetTerminal(Me.Page, cmbTerminal.Items(cmbTerminal.SelectedIndex).Value, False)

                    Dim oTerminalReader As roTerminal.roTerminalReader = oTerminal.Readers(0)

                    If oTerminal.Readers.Count > 0 AndAlso oTerminalReader.IDZone.HasValue AndAlso (oTerminalReader.ScopeMode.ToString()).Contains("ACC") Then
                        'Si es un terminal de accesos la dirección me la marca el terminal, sinó dejo que la marque el empleado
                        Dim idZone As Integer = oTerminal.Readers(0).IDZone.Value

                        Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, idZone, False)

                        If oZone.IsWorkingZone Then
                            cmbDetailsType.SelectedItem = cmbDetailsType.Items.FindByValue(roTypes.Any2String(Integer.Parse(PunchStatus.In_)))
                        Else
                            cmbDetailsType.SelectedItem = cmbDetailsType.Items.FindByValue(roTypes.Any2String(Integer.Parse(PunchStatus.Out_)))
                        End If
                        If intActivePage > intOldPage Then
                            intActivePage += 1
                        Else
                            intActivePage -= 1
                        End If

                    End If
                End If
        End Select

        If intActivePage = 5 Then

            Dim lstAuditParameterValues As New List(Of String)
            lstAuditParameterValues.Add(Me.cmbDetailsType.SelectedItem.Text)
            lstAuditParameterValues.Add(Me.cmbTerminal.SelectedItem.Text)
            lstAuditParameterValues.Add(Me.txtPunchDate.Date.ToString("dd/MM/yyyy") & " " & Me.txtDetailsTime.Text)

            Dim strInfoMsg As String = Me.Language.Translate("PageChange.Page5.PunchResume", Me.DefaultScope, lstAuditParameterValues)

            If Me.cmbCause.SelectedItem IsNot Nothing AndAlso roTypes.Any2Integer(Me.cmbCause.SelectedItem.Value) > 0 Then
                Dim lstAuditParameterNames As New List(Of String)
                lstAuditParameterNames.Add(Me.cmbCause.SelectedItem.Text)
                strInfoMsg &= " " & Me.Language.Translate("PageChange.Page5.PunchResumeCause", Me.DefaultScope, lstAuditParameterNames)
            End If

            Me.lblStep5Info2.InnerHtml = strInfoMsg.Replace("<strong>", "<strong style=""font-weight:bold"">")
        End If

        Me.hdnActiveFrame.Value = intActivePage

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If intOldPage = 5 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 5, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 5, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Ok.EmployeeMassPunchWelcome1.Text", Me.DefaultScope)

            If ErrorDescription = String.Empty Then
                Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeMassPunchWelcome2.Text", Me.DefaultScope)
                Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Else
                Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.OkWarning.EmployeeMassPunchWelcome2.Text", Me.DefaultScope)
                Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            End If
            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Orange

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Error.EmployeeMassPunchWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Error.EmployeeMassPunchWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(5, 0)
    End Sub

    Private Function InsertEmployeeMassPunch() As Integer

        Dim iTask As Integer = -1
        Dim lstEmployeesDest As New Generic.List(Of Integer)
        Dim lstGroups As New Generic.List(Of Integer)

        'obtener todos los empleados de los grupos seleccionados en el arbol v3
        roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployeesDest, lstGroups)
        Dim arrEmployeesDest As New ArrayList
        For Each intID As Integer In lstEmployeesDest
            arrEmployeesDest.Add(intID)
        Next

        Dim intIDCause As Integer = -1
        Dim intIDTerminal As Integer = -1

        If cmbCause.SelectedItem IsNot Nothing AndAlso roTypes.Any2Integer(cmbCause.SelectedItem.Value) > 0 Then
            intIDCause = roTypes.Any2Integer(cmbCause.SelectedItem.Value)
        End If
        If cmbTerminal.SelectedItem IsNot Nothing AndAlso roTypes.Any2Integer(cmbTerminal.SelectedItem.Value) > 0 Then
            intIDTerminal = roTypes.Any2Integer(cmbTerminal.SelectedItem.Value)
        End If

        Dim strTime() As String = txtDetailsTime.Text.Split(":")
        Dim xStartDate As New DateTime(txtPunchDate.Date.Year, txtPunchDate.Date.Month, txtPunchDate.Date.Day, roTypes.Any2Integer(strTime(0)), roTypes.Any2Integer(strTime(1)), 0)

        Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.EmployeeMassPunchWelcome1.Text", Me.DefaultScope)
        iTask = API.LiveTasksServiceMethods.MassPunchBackground(Me.Page, Me.hdnEmployeesSelected.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, intIDTerminal, intIDCause, xStartDate, Me.cmbDetailsType.SelectedItem.Value, True)

        If iTask > 0 Then
            Try
                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("ActionMsg")
                Dim msg As String = lblStep5Info2.InnerHtml.Replace("<strong>", "").Replace("<strong style=""font-weight:bold"">", "")
                lstAuditParameterValues.Add(msg)

                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tAssignCause, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
            Catch ex As Exception
            End Try
        End If
        Return iTask
    End Function

#End Region

End Class