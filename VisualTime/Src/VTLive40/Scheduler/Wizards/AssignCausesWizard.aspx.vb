Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_AssignCausesWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Punches"

#Region "Declarations"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("ACW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("ACW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("ACW_iCurrentTask") = value
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

            If Not Me.IsPostBack Then

                Me.optCompletedDays.Checked = True

                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
                Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
                Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text

                Me.txtBeginDate.Date = Now.Date
                Me.txtEndDate.Date = Now.Date

                Me.intActivePage = 0

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpAssignCausesWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpAssignCausesWizardGrid")

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
            Else

                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
                If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
                If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4

            End If

            If Me.intActivePage = 3 Then
                Me.LoadList()
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
                iCurrentTask = Me.AssignCauses()
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

                If Me.optCompletedDays.Checked = True Then
                    If txtBeginDate.Value Is Nothing OrElse txtEndDate.Value Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectDates", Me.DefaultScope)
                    ElseIf Me.txtBeginDate.Date > Me.txtEndDate.Date Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                    End If
                Else
                    If txtBeginDatePartial.Value Is Nothing OrElse txtEndDatePartial.Value Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectDates", Me.DefaultScope)
                    ElseIf Me.txtBeginDatePartial.Date > Me.txtEndDatePartial.Date Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                    ElseIf Me.txtBeginDatePartial.Date = Me.txtEndDatePartial.Date And Me.txtBeginTime.DateTime > Me.txtEndTime.DateTime Then
                        strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case 4
                If cmbCause.SelectedItem Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page4.IncorrectCause", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep4Error.Text = strMsg
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
                                       "PrefixTree=treeEmpAssignCausesWizard&FeatureAlias=Calendar.Punches&PrefixCookie=objContainerTreeV3_treeEmpAssignCausesWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeeSelector.Disabled = False

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

        If intOldPage = 4 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 4, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 4, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub LoadList()

        ' Miramos si tiene la licencia de horarios alternativos

        cmbCause.Items.Clear()

        Dim dTbl As DataTable = Nothing

        If optCompletedDays.Checked Then
            dTbl = CausesServiceMethods.GetCausesShortList(Nothing, True)
        Else
            dTbl = CausesServiceMethods.GetCauses(Nothing, "InputPermissions IN (0, 2) ", True)
        End If

        For Each dRow As DataRow In dTbl.Rows
            cmbCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
        Next

        If cmbCause.Items.Count > 0 Then
            cmbCause.Items(0).Selected = True
        End If

    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Ok.AssignCausesWelcome1.Text", Me.DefaultScope)
            If ErrorDescription = String.Empty Then
                Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Ok.AssignCausesWelcome2.Text", Me.DefaultScope)
                Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Else
                Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.OkWarning.AssignCausesWelcome2.Text", Me.DefaultScope)
                Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            End If

            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Orange
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Error.AssignCausesWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Error.AssignCausesWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(4, 0)
    End Sub

    Private Function AssignCauses() As Integer

        Dim iTask As Integer = -1

        Dim employeesSelected As String = Me.hdnEmployeesSelected.Value & "@" & "Employees" & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

        Dim DateIni As DateTime = txtBeginDate.Date
        Dim DateFIn As DateTime = txtEndDate.Date
        Dim intIDCause As Integer = 0

        If Not cmbCause.SelectedItem Is Nothing Then
            intIDCause = cmbCause.SelectedItem.Value
        End If

        If Not Me.optCompletedDays.Checked Then
            DateIni = New DateTime(Me.txtBeginDatePartial.Date.Year, Me.txtBeginDatePartial.Date.Month, Me.txtBeginDatePartial.Date.Day, Me.txtBeginTime.DateTime.Hour, Me.txtBeginTime.DateTime.Minute, 0)
            DateFIn = New DateTime(Me.txtEndDatePartial.Date.Year, Me.txtEndDatePartial.Date.Month, Me.txtEndDatePartial.Date.Day, Me.txtEndTime.DateTime.Hour, Me.txtEndTime.DateTime.Minute, 0)
        End If

        Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.AssignCausesWelcome1.Text", Me.DefaultScope)

        iTask = API.LiveTasksServiceMethods.AssignCauseInBackground(Me, employeesSelected, DateIni, DateFIn, intIDCause, Me.optCompletedDays.Checked, True)

        Return iTask
    End Function

#End Region

End Class