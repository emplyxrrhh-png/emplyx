Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Wizards_TemplateAssignWizardV2
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="templates")>
        Public Templates As Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate()

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private Enum Frame
        frmWelcome
        frmEmployeeSelector
        frmParameters
        frmResume
    End Enum

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private oActiveFrame As Frame

    Private oEmployeeSelected As roEmployee = Nothing

    Private oPermission As Permission
    Private oCurrentPermission As Permission

    Private bolMultipleShifts As Boolean = False

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("TemplateAssignWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployeeSelector)
                oFrames.Add(Frame.frmParameters)
                oFrames.Add(Frame.frmResume)

                ViewState("TemplateAssignWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("TemplateAssignWizard_Frames") = value
        End Set
    End Property

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("TemplateV2_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("TemplateV2_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("TemplateV2_iCurrentTask") = value
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
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", , True)
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
        Me.InsertExtraJavascript("roTemplateManagementDialog", "~/Scheduler/Scripts/templateManagement.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Scheduler/Styles/TemplateView.css")
        Me.InsertExtraCssIncludes("~/Base/ext-3.4.0/resources/css/ext-all.css")

        Server.ScriptTimeout = 1000
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission >= Permission.Write Then

            Me.bolMultipleShifts = API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultipleShifts")

            Me.LoadTemplates()
            Me.LoadUserFields()

            If Not Me.IsPostBack Then
                Me.LoadShifts(False)
                Dim GroupID As String = Request.Params("GroupID")
                If GroupID IsNot Nothing AndAlso GroupID.Length > 0 AndAlso IsNumeric(GroupID) Then
                    Me.hdnIDGroupSelected.Value = "A" & GroupID
                End If

                If Me.hdnIDGroupSelected.Value <> "" Then
                    Me.oCurrentPermission = Me.GetFeaturePermissionByGroup(FeatureAlias, Me.hdnIDGroupSelected.Value.Substring(1))
                Else
                    Me.oCurrentPermission = Permission.Write
                End If

                If Me.oCurrentPermission >= Permission.Write Then

                    Me.Frames = Nothing

                    Me.SetStepTitles()

                    Me.oActiveFrame = Frame.frmWelcome

                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesEmployeesAssignTemplateWizard")
                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesEmployeesAssignTemplateWizardGrid")

                    Me.hdnEmployeesSelected.Value = ""
                    Me.hdnFilter.Value = ""
                    Me.hdnFilterUser.Value = ""

                    If Me.hdnIDGroupSelected.Value <> "" Then
                        Me.hdnEmployeesSelected.Value = Me.hdnIDGroupSelected.Value
                    End If

                    Me.oActiveFrame = Frame.frmWelcome
                Else

                    WLHelperWeb.RedirectAccessDenied(True)

                End If

                Me.externalform1.PopupFrameExternalForm.CssClassPopupExtenderBackground = "modalBackgroundTransparent"
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
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

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

    Protected Sub btnReload_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnReload.Click
        Me.LoadTemplates()
        If Me.cmbTemplates.Items.Count > 0 Then
            Me.cmbTemplates.SelectedItem = Me.cmbTemplates.Items(0)
        Else
            Me.cmbTemplates.SelectedItem = Nothing
        End If
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Select Case oParameters.Action.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckFrame(Me.oActiveFrame))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.TemplateAssign()
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
            Case "LOADTEMPLATESLIST"
                LoadTemplatesList(oParameters)
            Case "RELOADTEMPLATES"
                LoadTemplatesList(oParameters)
                PerformActionCallback.JSProperties("cpAction") = "DELETETEMPLATE"
            Case "SAVETEMPLATE"
                SaveTemplate(oParameters)
            Case "DELETETEMPLATE"
                DeleteTemplate(oParameters)
        End Select

    End Sub

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        CloseWizard()
    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblWelcome1.Text = Me.Language.Translate("End.Ok.AssignTemplateWelcome1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.AssignTemplateWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblWelcome1.Text = Me.Language.Translate("End.Error.AssignTemplateWelcome1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("End.Error.AssignTemplateWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ErrorDescription
            Me.lblWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Me.btLoop.Visible = True
        Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)
    End Sub

    Private Function TemplateAssign(Optional ByVal _LockedDayAction As LockedDayAction = LockedDayAction.None, Optional ByVal _CoverageDayAction As LockedDayAction = LockedDayAction.None, Optional ByVal _DateLocked As Date = Nothing, Optional ByVal _IDEmployeeLocked As Integer = -1) As Integer

        Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

        Dim xDateLocked As Date
        Dim xIDEmployee As Integer

        If hdnLockedDay.Value <> "" Then xDateLocked = CDate(hdnLockedDay.Value)
        If hdnLockedEmployee.Value <> "" Then xIDEmployee = CInt(hdnLockedEmployee.Value)

        Dim lstEmployeesDest As New Generic.List(Of Integer)
        Dim lstGroups As New Generic.List(Of Integer)

        'obtener todos los empleados de los grupos seleccionados en el arbol v3
        roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployeesDest, lstGroups)
        If lstGroups IsNot Nothing Then
            Dim strFilter As String = Me.hdnFilter.Value
            Dim strFilterUser As String = Me.hdnFilterUser.Value
            Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, lstGroups.ToArray, "Employees", "U", strFilter, strFilterUser)
            lstEmployeesDest.AddRange(tmp)
        End If

        Dim employeeFilter As String = Me.hdnEmployeesSelected.Value & "@" & "Employees" & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

        Dim arrEmployeesDest As New ArrayList
        For Each intID As Integer In lstEmployeesDest
            arrEmployeesDest.Add(intID)
        Next

        'Dim idShift As Integer = roTypes.Any2Integer(Me.cmbAssignShifts.SelectedItem.Value.ToString.Split("_")(0))
        Dim idShift As Integer = Session("cmbAssingShifts")
        Dim isFeast As Boolean = roTypes.Any2Boolean(Me.cmbTemplates.SelectedItem.Value.ToString.Split("_")(1))
        Dim idTemplate As Integer = roTypes.Any2Integer(Me.cmbTemplates.SelectedItem.Value.ToString.Split("_")(0))
        Dim idUserField As String = Me.cmbUserFields.SelectedItem.Text
        Dim txtuserField As String = userFieldValueText.Text

        ' Obtenemos la hora inicial del horario flotante (si lo es)
        Dim xStartShift As DateTime = Me.GetStartShift()

        Dim iTask As Integer = API.LiveTasksServiceMethods.AssignTemplatev2InBackground(Me.Page, idTemplate, arrEmployeesDest, idShift, xStartShift, Me.ckLockDays.Checked, Me.ckKeepBloquedDays.Checked, Me.ckKeepHolidays.Checked, isFeast, employeeFilter, idUserField, txtuserField)

        If iTask > 0 Then
            Audit(iTask)
        End If

        Return iTask
    End Function

    Private Sub Audit(ByVal iTask As Integer)
        Try
            ' Auditoría de consulta de datos

            Dim lstAuditParameterNames As New List(Of String)
            Dim lstAuditParameterValues As New List(Of String)

            lstAuditParameterNames.Add("{iTask}")
            lstAuditParameterValues.Add(iTask)

            lstAuditParameterNames.Add("{Template}")
            lstAuditParameterValues.Add(cmbTemplates.SelectedItem.Value.ToString)
            'lstAuditParameterNames.Add("{Year}")
            'lstAuditParameterValues.Add(cmbYear.Value)
            lstAuditParameterNames.Add("{Group}")
            Dim sGroupName As String = ""
            If CInt(hdnIDGroupSelected.Value.Substring(1)) > 0 Then
                Dim oEmployeeGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, CInt(hdnIDGroupSelected.Value.Substring(1)), False)
                If oEmployeeGroup IsNot Nothing Then
                    sGroupName = oEmployeeGroup.Name
                End If
            End If
            'If Me.chkSubGroups.Checked() Then
            '    sGroupName += "+"
            'End If
            lstAuditParameterValues.Add(sGroupName)
            lstAuditParameterNames.Add("{Shift}")
            Dim sShift As String = ""
            'Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Me, CInt(cmbAssignShifts_Value.Value), False)
            'If oShift IsNot Nothing Then
            '    sShift = oShift.Name
            'End If
            lstAuditParameterValues.Add(sShift)

            lstAuditParameterNames.Add("{KeepBloquedDays}")
            lstAuditParameterValues.Add(ckKeepBloquedDays.Checked.ToString())

            lstAuditParameterNames.Add("{KeepHolidays}")
            lstAuditParameterValues.Add(ckKeepHolidays.Checked.ToString())

            API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tSchedulerHolidays, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub btLoop_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btLoop.Click

        Me.FrameChange(Frame.frmWelcome, Frame.frmEmployeeSelector)
        Me.btLoop.Visible = False

    End Sub

#End Region

#Region "Templates Manager"

    Private Sub DeleteTemplate(ByVal oParameters As ObjectCallbackRequest)
        Dim oTemplatesList As Generic.List(Of Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate) = Nothing

        Dim rError As New roJSON.JSONError(False, "")

        Try
            If API.SchedulerServiceMethods.DeleteScheduleTemplate(Me.Page, oParameters.Templates(0).ID, True) Then
                oTemplatesList = API.SchedulerServiceMethods.GetScheduleTemplatesv2(Me.Page)
            End If

            If oTemplatesList Is Nothing Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject().States.SchedulerState.ErrorText)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message)
        Finally
            PerformActionCallback.JSProperties.Add("cpAction", "DELETETEMPLATE")

            If rError.Error = False Then
                PerformActionCallback.JSProperties.Add("cpResult", "OK")
                PerformActionCallback.JSProperties.Add("cpTemplatesLst", roJSONHelper.SerializeNewtonSoft(oTemplatesList.ToArray))
            Else
                PerformActionCallback.JSProperties.Add("cpResult", "KO")
                PerformActionCallback.JSProperties.Add("cpErrorRO", rError)
                PerformActionCallback.JSProperties.Add("cpMessage", rError.ErrorText)
            End If
        End Try
    End Sub

    Private Sub SaveTemplate(ByVal oParameters As ObjectCallbackRequest)
        Dim oTemplatesList As Generic.List(Of Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate) = Nothing

        Dim rError As New roJSON.JSONError(False, "")

        Try
            If API.SchedulerServiceMethods.SaveScheduleTemplate(Me.Page, oParameters.Templates(0), True, WLHelperWeb.CurrentPassport.ID) Then
                oTemplatesList = API.SchedulerServiceMethods.GetScheduleTemplatesv2(Me.Page)
            End If

            If oTemplatesList Is Nothing Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject().States.SchedulerState.ErrorText)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message)
        Finally
            PerformActionCallback.JSProperties.Add("cpAction", "SAVETEMPLATE")

            If rError.Error = False Then
                PerformActionCallback.JSProperties.Add("cpResult", "OK")
                PerformActionCallback.JSProperties.Add("cpTemplatesLst", roJSONHelper.SerializeNewtonSoft(oTemplatesList.ToArray))
            Else
                PerformActionCallback.JSProperties.Add("cpResult", "KO")
                PerformActionCallback.JSProperties.Add("cpErrorRO", rError)
                PerformActionCallback.JSProperties.Add("cpMessage", rError.ErrorText)
            End If
        End Try
    End Sub

    Private Sub LoadTemplatesList(ByVal oParameters As ObjectCallbackRequest)

        Dim oTemplatesList As New Generic.List(Of Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate)

        Dim rError As New roJSON.JSONError(False, "")

        Try
            oTemplatesList = API.SchedulerServiceMethods.GetScheduleTemplatesv2(Me.Page)

            If oTemplatesList Is Nothing Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject().States.SchedulerState.ErrorText)
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message)
        Finally
            PerformActionCallback.JSProperties.Add("cpAction", "LOADTEMPLATESLIST")

            If rError.Error = False Then
                PerformActionCallback.JSProperties.Add("cpResult", "OK")
                PerformActionCallback.JSProperties.Add("cpTemplatesLst", roJSONHelper.SerializeNewtonSoft(oTemplatesList.ToArray))
            Else
                PerformActionCallback.JSProperties.Add("cpResult", "KO")
                PerformActionCallback.JSProperties.Add("cpErrorRO", rError)
                PerformActionCallback.JSProperties.Add("cpMessage", rError.ErrorText)
            End If

        End Try

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
            Case Wizards_TemplateAssignWizardV2.Frame.frmEmployeeSelector
                ' Hemos de tener algún grupo seleccionado

                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_TemplateAssignWizardV2.Frame.frmParameters
                'Try
                '    Dim xDateYear As New Date(roTypes.Any2Integer(cmbYear.Value), 1, 1)
                'Catch
                '    strMsg = Me.Language.Translate("CheckPage.YearIncorrect", Me.DefaultScope)
                'End Try
                'If strMsg <> "" Then bolRet = False
                If bolRet Then
                    If Me.cmbTemplates Is Nothing OrElse Me.cmbTemplates.SelectedItem Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.TemplateRequired", Me.DefaultScope)
                    End If
                    If strMsg <> "" Then bolRet = False
                End If

                If Me.cmbAssignShifts.SelectedItem Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.ShiftRequired", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False

                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmEmployeeSelector
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

        End Select

        Select Case oActiveFrame

            Case Frame.frmEmployeeSelector
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmployeesEmployeesAssignTemplateWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmployeesEmployeesAssignTemplateWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeesSelector.Disabled = False

            Case Frame.frmParameters
                Me.cmbStartFloating.SelectedItem = Nothing

            Case Frame.frmResume
                If oOldFrame <> Frame.frmResume Then
                    Dim xStartShift As DateTime = Nothing
                    If Me.hdnIsFloating.Value = "1" Then
                        xStartShift = Me.GetStartShift()
                    End If
                    Session("cmbAssingShifts") = Me.cmbAssignShifts.SelectedItem.Value.ToString.Split("_")(0)
                    Dim strShiftName As String = API.ShiftServiceMethods.FloatingShiftName(Me, Me.cmbAssignShifts.SelectedItem.Value.ToString.Split("_")(0), xStartShift)
                    If strShiftName = "???" Then
                        strShiftName = "Ninguno"
                    End If
                    Me.lblResumeAll.Text = Me.Language.Translate("lblResumeAll", Me.DefaultScope).ToString.Replace("{1}", strShiftName).Replace("{2}", cmbTemplates.SelectedItem.Text)
                End If
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

        Dim oLabel As Label
        Dim strStep As String = ""
        For n As Integer = 1 To System.Enum.GetValues(GetType(Frame)).Length - 1
            If n > 1 Then
                strStep = Me.hdnStepTitle2.Text.Replace("{0}", Me.FramePos(Me.FrameByIndex(n)))
                strStep = strStep.Replace("{1}", Me.Frames.Count - 1)
            End If
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Title")
            oLabel.Text = Me.hdnStepTitle.Text & strStep
        Next

    End Sub

    Private Sub LoadTemplates()

        Me.cmbTemplates.Items.Clear()

        Dim oTemplates As Generic.List(Of Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate) = API.SchedulerServiceMethods.GetScheduleTemplatesv2(Me)

        If oTemplates IsNot Nothing Then
            For Each oTemplate As Robotics.Base.VTBusiness.Scheduler.roScheduleTemplate In oTemplates
                'Me.cmbTemplates.AddItem(oTemplate.Name.ToString, oTemplate.ID.ToString, "ButtonClick($get('" & Me.btRefreshYearView.ClientID & "'));")
                Me.cmbTemplates.Items.Add(oTemplate.Name.ToString, oTemplate.ID.ToString & "_" & If(oTemplate.FeastTemplate, "1", "0"))
            Next

            If Me.cmbTemplates.Items.Count > 0 And roTypes.Any2String(Me.cmbTemplates.Value) = "" Then
                Me.cmbTemplates.SelectedIndex = 0
            End If
        End If

    End Sub

    Private Sub LoadUserFields()

        Me.cmbUserFields.Items.Clear()

        Dim oUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType = 0 AND Used=1 and History=0", False)

        Me.cmbUserFields.Items.Add("", 0)

        If oUserFields IsNot Nothing Then

            Dim dRows() As DataRow = oUserFields.Select("", "FieldName")

            For Each dRow As DataRow In dRows
                If dRow("Used") Then
                    cmbUserFields.Items.Add(dRow("FieldName"), dRow("ID"))
                End If
            Next

            If Me.cmbUserFields.Items.Count > 0 And roTypes.Any2String(Me.cmbUserFields.Value) = "" Then
                Me.cmbUserFields.SelectedIndex = 0
            End If
        End If

    End Sub

    Private Sub LoadShifts(ByVal addEmpty As Boolean)

        Me.cmbAssignShifts.Items.Clear()

        If addEmpty = True Then
            Me.cmbAssignShifts.Items.Add(Me.Language.Translate("cmbNone", Me.DefaultScope), "0_0_189912310000")
        End If

        Dim tbShifts As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me)
        If tbShifts IsNot Nothing Then
            Dim strScript As String = ""
            For Each oRow As DataRow In tbShifts.Rows

                If Not roTypes.Any2Boolean(oRow("AllowComplementary")) AndAlso Not roTypes.Any2Boolean(oRow("AllowFloatingData")) AndAlso Not roTypes.Any2String(oRow("AdvancedParameters")) = "Starter=[1]" Then

                    Dim oValue As String = oRow("ID") & "_0_189912310000"

                    If Me.bolMultipleShifts Then
                        If roTypes.Any2Boolean(oRow("IsFloating")) Then
                            If Not IsDBNull(oRow("StartFloating")) Then
                                oValue = oRow("ID") & "_1_" & Format(CDate(oRow("StartFloating")), "yyyyMMddHHmm")
                            Else
                                oValue = oRow("ID") & "_1_189912310000"
                            End If
                        End If
                    End If

                    Me.cmbAssignShifts.Items.Add(oRow("Name"), oValue)
                End If
            Next
        End If

        'If addEmpty = False Then
        '    Me.cmbAssignShifts.SelectedIndex = 0
        'Else
        '    Me.cmbAssignShifts.SelectedIndex = -1
        'End If
        Me.cmbStartFloating.Items.Clear()
        Me.cmbStartFloating.Items.Add(Me.Language.Translate("StartFloating.ShiftDay", "Shift"), "1")
        Me.cmbStartFloating.Items.Add(Me.Language.Translate("StartFloating.ShiftAfter", "Shift"), "2")
        Me.cmbStartFloating.Items.Add(Me.Language.Translate("StartFloating.ShiftBefore", "Shift"), "0")

    End Sub

    Private Function GetStartShift() As DateTime

        Dim xRet As DateTime = Nothing

        If Me.cmbStartFloating.SelectedItem IsNot Nothing Then
            Select Case roTypes.Any2Integer(Me.cmbStartFloating.SelectedItem.Value)
                Case 0
                    xRet = New DateTime(1899, 12, 29, Me.txtStartFloating.DateTime.Hour, Me.txtStartFloating.DateTime.Minute, 0)
                Case 1
                    xRet = New DateTime(1899, 12, 30, Me.txtStartFloating.DateTime.Hour, Me.txtStartFloating.DateTime.Minute, 0)
                Case 2
                    xRet = New DateTime(1899, 12, 31, Me.txtStartFloating.DateTime.Hour, Me.txtStartFloating.DateTime.Minute, 0)
            End Select

        End If

        Return xRet

    End Function

    Private Sub cmbAssignShifts_Callback(sender As Object, e As CallbackEventArgsBase) Handles cmbAssignShifts.Callback
        If e.Parameter = "AddEmpty" Then
            LoadShifts(True)
        Else
            LoadShifts(False)
        End If

    End Sub

#End Region

End Class