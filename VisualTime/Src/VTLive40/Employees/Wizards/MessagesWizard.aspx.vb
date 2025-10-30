Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Forms_MessagesWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees"
    Private Const IMAGE_INCIDENCE As String = "~/Reports/Images/Information_16.png"

#Region "Declarations"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Messages_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Messages_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Messages_iCurrentTask") = value
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
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js", , True)
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js", , True)

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Write Then

            If Not Me.IsPostBack AndAlso Not IsCallback Then

                Me.FreezingDatePage = API.ConnectorServiceMethods.GetFirstDate(Me.Page)

                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
                Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text

                Me.intActivePage = 0

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpIncidencesWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpIncidencesWizardGrid")

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
            Else

                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
                If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3

            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
            Me.intActivePage += 1

            'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
            If Me.intActivePage = 1 Then Me.intActivePage += 1

            Me.PageChange(intOldPage, Me.intActivePage)
        Else

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
                PerformActionCallback.JSProperties.Add("cpErrorMessageKey", Me.lblStep3Error.Text)
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.SaveEmployeeMessages()
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

    Private Function SaveEmployeeMessages() As Integer

        Dim iTask As Integer = -1

        Me.lblWelcome1.Text = Me.Language.Translate("End.CreatingMessagesWelcome1.Text", Me.DefaultScope)

        Dim employeesSelected As String = Me.hdnEmployeesSelected.Value & "@" & "Employees" & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

        Dim strScheduleStr As String = "1@1@1"

        If Me.chkSendOnDate.Checked Then
            If Me.ckFixedValue.Checked Then
                strScheduleStr = "A@" & Me.txtOnFixedDate.Date.Day & "@" & Me.txtOnFixedDate.Date.Month
            ElseIf Me.ckUserField.Checked Then
                strScheduleStr = "A@" & Me.cmbUserField.SelectedItem.Value & "@"
            End If

            If Me.ckSendOnPeriod.Checked Then
                strScheduleStr = strScheduleStr & "@" & Me.txtInitPeriod.DateTime.ToString("HH:mm") & "@" & Me.txtEndPeriod.DateTime.ToString("HH:mm")
            Else
                strScheduleStr = strScheduleStr & "@00:00@23:59"
            End If
        End If

        iTask = API.LiveTasksServiceMethods.CreateMessageForEmployees(Me.Page, employeesSelected, txtMessage.Text, strScheduleStr, True)

        Return iTask
    End Function

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
                If txtMessage.Text Is Nothing OrElse txtMessage.Text = String.Empty Then
                    strMsg = "CheckPage.Page3.IncorrectMessage"
                End If

                If Me.chkSendOnDate.Checked Then
                    If Me.ckFixedValue.Checked AndAlso Me.txtOnFixedDate.Value Is Nothing Then
                        strMsg = "CheckPage.Page3.SelectAFixedDate"
                    ElseIf Me.ckUserField.Checked AndAlso Me.cmbUserField.SelectedItem Is Nothing Then
                        strMsg = "CheckPage.Page3.SelectUserfield"
                    ElseIf Me.ckSendOnPeriod.Checked AndAlso Me.txtInitPeriod.DateTime > Me.txtEndPeriod.DateTime Then
                        strMsg = "CheckPage.Page3.ValidPeriod"
                    End If
                End If

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
                                       "PrefixTree=treeEmpIncidencesWizard&FeatureAlias=Calendar.JustifyIncidences&PrefixCookie=objContainerTreeV3_treeEmpIncidencesWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeeSelector.Disabled = False

                Dim oEmployeeTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("ctl00_contentMainBody_roTrees1")
                Dim oLockTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpIncidencesWizardGrid")
                oLockTreeState.Selected1 = oEmployeeTreeState.Selected1
                HelperWeb.roSelector_SetTreeState(oLockTreeState)
            Case 3
                cmbUserField.Items.Clear()
                Dim tbFactorUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType =2 AND Used=1", False)
                For Each oRow As DataRow In tbFactorUserFields.Select("", "FieldName")
                    cmbUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
                Next

                If (intOldPage < intActivePage) Then
                    Me.chkSendOnNextPunch.Checked = True
                    Me.chkSendOnDate.Checked = False
                    Me.ckFixedValue.Checked = True
                    Me.ckFixedValue.ClientEnabled = True

                    Me.ckUserField.Checked = False
                    Me.ckUserField.ClientEnabled = False

                    Me.txtOnFixedDate.ClientEnabled = False
                    Me.cmbUserField.ClientEnabled = False

                    Me.ckSendOnPeriod.ClientEnabled = False
                    Me.txtInitPeriod.DateTime = DateTime.Now.Date
                    Me.txtEndPeriod.DateTime = DateTime.Now.Date.AddDays(1).AddSeconds(-1)
                    Me.txtInitPeriod.ClientEnabled = False
                    Me.txtEndPeriod.ClientEnabled = False

                End If
        End Select

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

        If intOldPage = 3 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0 And intActivePage < 3, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 3, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 3, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If
    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblWelcome1.Text = Me.Language.Translate("End.Ok.MessagesWizard1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.MessagesWizard2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblWelcome1.Text = Me.Language.Translate("End.Error.MessagesWizard1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("End.Error.MessagesWizard2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ErrorDescription
            Me.lblWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(3, 0)
    End Sub

#End Region

    Public Property FreezingDatePage() As Date
        Get
            Return CDate(ViewState("Incidences_FreezingDate"))
        End Get
        Set(ByVal value As Date)
            ViewState("Incidences_FreezingDate") = value
        End Set
    End Property

    Private Function CheckPeriodOfFreezing(ByVal xDate As Date) As Boolean
        Dim bolRet As Boolean = False

        If (Me.FreezingDatePage >= xDate) Then
            bolRet = True
        End If

        If Not bolRet Then
            ' Verificamos que no estemos intentando modificar un día futuro.
            bolRet = Not (xDate <= Now.Date)
        End If

        Return bolRet
    End Function

    Private Function CheckPeriodOfFreezingPage(ByVal xDate As Date) As Boolean
        Dim bolFreeze As Boolean = False
        bolFreeze = CheckPeriodOfFreezing(xDate)
        Return bolFreeze
    End Function

End Class