Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Security_Wizards_PassportSecurityActions
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome '0
        frmSelector '1
        frmOptions '2
    End Enum

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("PSA_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("PSA_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("PSA_iCurrentTask") = value
        End Set
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("PSA_ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("PSA_ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("PSA_ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("PSA_ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("PSA_ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("PSA_ErrorDescription") = value
        End Set
    End Property

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("EmployeesGroupWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmSelector)
                oFrames.Add(Frame.frmOptions)

                ViewState("EmployeesGroupWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("EmployeesGroupWizard_Frames") = value
        End Set
    End Property

#End Region

    Protected Sub frmSecurityActions_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSecurityActions.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
    End Sub

    Protected Sub frmSecurityActions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSecurityActions.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            Me.opResetPassword.Checked = True

            If Me.HasFeaturePermission("Administration.Security", Permission.Admin) AndAlso
               Me.HasFeaturePermission("Employees.IdentifyMethods", Permission.Write) Then

                'Me.WizardType = "e" 'roTypes.Any2String(Me.Request("WizardType"))

                'lblStep2InfoEmployees.Visible = (WizardType = "e")
                'lblStep2InfoUsers.Visible = (WizardType = "u")
                'ifEmployeesSelector.Visible = (WizardType = "e")
                'ifUsersSelector.Visible = (WizardType = "u")

                Me.Frames = Nothing

                Me.SetStepTitles()

                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesPassportSecurityActions")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesPassportSecurityActionsGrid")
                'HelperWeb.roSelector_Initialize("objContainerTree_treeUsersPassportSecurityActions")

                Me.oActiveFrame = Frame.frmWelcome

                Me.hdnEmployeesSelected.Value = ""
                'Me.hdnUsersSelected.Value = ""

                HelperWeb.roSelector_SetSelection("", "", "objContainerTreeV3_treeEmployeesPassportSecurityActions")
                HelperWeb.roSelector_SetSelection("", "", "objContainerTreeV3_treeEmployeesPassportSecurityActionsGrid")
                'HelperWeb.roSelector_SetSelection("", "", "objContainerTree_treeUsersPassportSecurityActions")
            Else
                WLHelperWeb.RedirectAccessDenied(True)
            End If
        Else

            Dim oDiv As HtmlControl
            For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                If oDiv.Style("display") <> "none" Then
                    Me.oActiveFrame = Me.FrameByIndex(n)
                    Exit For
                End If
            Next

        End If

    End Sub

    Private Sub LoadEmployeePermissions()

        Me.cmbEmployeePermissions.Items.Clear()
        Me.cmbEmployeeApplications.Items.Clear()

        Dim oEmployees As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Me, "", "Employees")

        If oEmployees IsNot Nothing AndAlso oEmployees.Rows.Count > 0 Then

            oEmployees.DefaultView.Sort = "EmployeeName ASC"

            For Each oEmployee As DataRowView In oEmployees.DefaultView
                Me.cmbEmployeePermissions.Items.Add(oEmployee("EmployeeName"), oEmployee("IDEmployee"))
                Me.cmbEmployeeApplications.Items.Add(oEmployee("EmployeeName"), oEmployee("IDEmployee"))
            Next

            If Me.cmbEmployeePermissions.Items.Count > 0 And roTypes.Any2String(Me.cmbEmployeePermissions.Value) = "" Then
                Me.cmbEmployeePermissions.SelectedIndex = 0
            End If
            If Me.cmbEmployeeApplications.Items.Count > 0 And roTypes.Any2String(Me.cmbEmployeeApplications.Value) = "" Then
                Me.cmbEmployeeApplications.SelectedIndex = 0
            End If
        End If

    End Sub

#Region "Events"

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click
        If Me.CheckFrame(Me.oActiveFrame) Then
            Dim oOldFrame As Frame = Me.oActiveFrame
            If Me.Frames.Count > Me.FramePos(Me.oActiveFrame) + 1 Then
                Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) + 1)
            End If
            Me.FrameChange(oOldFrame, Me.oActiveFrame)
        End If
    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click
        Dim oOldFrame As Frame = Me.oActiveFrame
        Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) - 1)
        Me.FrameChange(oOldFrame, Me.oActiveFrame)
    End Sub

#End Region

#Region "Methods"

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = 0
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                intRet = n
                Exit For
            End If
        Next
        Return intRet
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal Frame As Frame) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case Frame

            Case Frame.frmSelector

                'If Me.WizardType = "e" Then
                'Comprobar si hay algún empleado seleccionado

                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

                'Else
                ''Comprobar si hay algún usuario seleccionado
                'If Me.hdnUsersSelected.Value = "" Then
                '    strMsg = Me.Language.Translate("CheckPage.IncorrectUsersSelected", Me.DefaultScope)
                'End If
                'If strMsg <> "" Then bolRet = False
                'Me.lblStep1Error.Text = strMsg
                'End If

            Case Frame.frmOptions

                'Comprobar acciones de seguridad marcadas
                If 1 = 2 Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectOptionsSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame

            Case Frame.frmSelector
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

                Me.LoadEmployeePermissions()

                ' En el caso que sea seguridad modo v2 no visualziamos el frame de asignar a grupo de usuario

                Me.opPermissions.Visible = False
                Me.opPermissions.Visible = True

            Case Frame.frmOptions

        End Select

        Select Case oActiveFrame

            Case Frame.frmSelector

                'If Me.WizardType = "e" Then

                Dim strRelative As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                            "PrefixTree=treeEmployeesPassportSecurityActions&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmployeesPassportSecurityActionsGrid&" &
                                            "AfterSelectFuncion=parent.EmployeesSelected"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strRelative)
                Me.ifEmployeesSelector.Disabled = False
            Case Frame.frmOptions

        End Select

        Me.hdnActiveFrame.Value = Me.FrameIndex(oActiveFrame)

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oOldFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If

        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oActiveFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(Me.FramePos(oActiveFrame) > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub SetStepTitles()

        'Dim oLabel As Label
        'Dim strStep As String = ""
        'For n As Integer = 1 To System.Enum.GetValues(GetType(Frame)).Length - 1
        '    If n > 1 Then
        '        strStep = Me.hdnStepTitle2.Text.Replace("{0}", Me.FramePos(Me.FrameByIndex(n)))
        '        strStep = strStep.Replace("{1}", Me.Frames.Count - 1)
        '    End If
        '    oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Title")
        '    oLabel.Text = Me.hdnStepTitle.Text & strStep
        'Next

    End Sub

#End Region

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        Me.lblWelcomeEmployees.Text = Me.Language.Translate("End.EmployeesGroupWelcome1.Text", Me.DefaultScope)

        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeesGroupWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ""
        Else
            Me.lblWelcome2.Text = "" 'Me.Language.Translate("End.Error.EmployeesGroupWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ErrorDescription
            Me.lblWelcome3.ForeColor = Drawing.Color.Red
        End If

        Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckFrame(Me.oActiveFrame))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.PerformSecurityAction()
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

    Protected Function PerformSecurityAction() As Integer
        Dim iTask As Integer = -1

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim strErrorInfo As String = ""
            Dim strEmployeeFilter As String = Me.hdnEmployeesSelected.Value
            Dim strFilters As String = String.Empty
            Dim iSourceEmployee As Integer = -1
            Dim bLockAccess As Boolean = False

            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmployeesPassportSecurityActions")
            If oTreeState.Filter <> "" AndAlso oTreeState.Filter.Length >= 5 Then
                strFilters = oTreeState.Filter
            End If

            Dim strUserFieldFilters As String = oTreeState.UserFieldFilter
            Dim iAction As Integer = -1
            If Me.opLockAccount.Checked Then
                iAction = 1
                bLockAccess = True
            ElseIf Me.opResetPassword.Checked Then
                iAction = 2
                bLockAccess = False
            ElseIf Me.opUnlockAccount.Checked Then
                iAction = 3
                bLockAccess = False
            ElseIf Me.opPermissions.Checked Then
                iAction = 4
                bLockAccess = False
                iSourceEmployee = cmbEmployeePermissions.SelectedItem.Value
            ElseIf Me.opApplications.Checked Then
                iAction = 5
                bLockAccess = False
                iSourceEmployee = cmbEmployeeApplications.SelectedItem.Value
            ElseIf Me.opSendUsername.Checked Then
                iAction = 6
                bLockAccess = False
            ElseIf Me.opSendPin.Checked Then
                iAction = 7
                bLockAccess = False
            End If

            iTask = API.LiveTasksServiceMethods.SecurityActionsInBackground(Me, iAction, strEmployeeFilter, "Employees", strFilters, strUserFieldFilters, bLockAccess, iSourceEmployee, True)

        End If

        Return iTask
    End Function

    Private Sub SendFile()

        Dim lstResult() As String = Session("PassportSecurityActionsResetPassportFile")
        Dim bfile() As Byte = lstResult.SelectMany(Function(s) Text.Encoding.ASCII.GetBytes(s)).ToArray()
        If bfile IsNot Nothing AndAlso bfile.Length > 0 Then

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()
            Response.ContentType = "text/plain"
            Response.AddHeader("content-disposition", "attachment;filename=ResetPaswordResult.txt")

            Response.OutputStream.Write(bfile, 0, bfile.Length)
            Response.Flush()

            Try
                Response.End()
            Catch ex As Exception

            End Try

        End If

    End Sub

End Class