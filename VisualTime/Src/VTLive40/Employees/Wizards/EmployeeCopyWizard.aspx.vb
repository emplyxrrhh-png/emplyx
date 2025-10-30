Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Wizards_EmployeeCopyWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome
        frmEmployeeOrigin
        frmEmployeesSelector
        frmUserFields
        frmTerminals
        frmConceptsData
        frmSchedule
        frmRules
        frmLabAgree
        frmAssignments
        frmCenters
        frmFinish
    End Enum

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get

            Dim oFrames As New Generic.List(Of Frame)
            If Me.hdnFrames.Value = "" Then
                Dim oSchPermission = GetFeaturePermission("Calendar.Scheduler")

                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployeeOrigin)
                oFrames.Add(Frame.frmEmployeesSelector)
                oFrames.Add(Frame.frmUserFields)
                If (oSchPermission >= Permission.Write) Then oFrames.Add(Frame.frmSchedule)
                oFrames.Add(Frame.frmLabAgree)
                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") Then oFrames.Add(Frame.frmAssignments)
                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl") Then oFrames.Add(Frame.frmCenters)
                oFrames.Add(Frame.frmFinish)
            Else

                For Each strItem As String In Me.hdnFrames.Value.Split("*")
                    oFrames.Add(strItem)
                Next

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            Me.hdnFrames.Value = ""
            Me.hdnFramesOnlyClient.Value = ""
            If value IsNot Nothing Then
                For Each oItem As Frame In value
                    Me.hdnFrames.Value &= "*" & oItem
                    Select Case oItem
                        Case Frame.frmWelcome, Frame.frmConceptsData, Frame.frmRules, Frame.frmTerminals
                            Me.hdnFramesOnlyClient.Value &= "*1"
                        Case Else
                            Me.hdnFramesOnlyClient.Value &= "*0"
                    End Select
                Next
                If Me.hdnFrames.Value <> "" Then Me.hdnFrames.Value = Me.hdnFrames.Value.Substring(1)
                If Me.hdnFramesOnlyClient.Value <> "" Then Me.hdnFramesOnlyClient.Value = Me.hdnFramesOnlyClient.Value.Substring(1)
            End If
        End Set
    End Property

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("ECW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("ECW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("ECW_iCurrentTask") = value
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
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js", , True)
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000
        Me.LoadUserFields()
        Me.LoadAssignments()
        Me.LoadCenters()
        Me.LoadEmployees()

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Employees", Permission.Admin) Then

                If Not API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultipleShifts") Then
                    ckCopyAlternativeShifts.Checked = False
                    ckCopyAlternativeShifts.Enabled = False
                End If

                Me.Frames = Nothing
                Me.Frames = Me.Frames

                Dim oEmployee As roEmployee = Nothing
                If Me.Request("IDEmployeeSource") <> "" AndAlso IsNumeric(Me.Request("IDEmployeeSource")) Then

                    Me.hdnIDEmployeeSource.Value = Me.Request("IDEmployeeSource")
                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.hdnIDEmployeeSource.Value, False)

                End If

                ' Me.lblWelcome2.Text = Me.lblWelcome2.Text.Replace("{1}", oEmployee.Name)

                Me.SetStepTitles()

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpEmployeeCopyWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpEmployeeCopyWizardGrid")
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

                Me.txtScheduleBeginDate.Date = Now.Date
                Me.txtScheduleEndDate.Date = Now.Date

                Me.oActiveFrame = Frame.frmWelcome
            Else

                WLHelperWeb.RedirectAccessDenied(True)

            End If
        Else

            If Me.Request("action") = "CheckFrame" Then
            Else

                Me.oActiveFrame = Me.FrameByIndex(Me.hdnActiveFrame.Value)
                Dim oDiv As HtmlControl
                For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                    oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                    If n = Me.FrameIndex(Me.oActiveFrame) Then
                        oDiv.Style("display") = "block"
                    Else
                        oDiv.Style("display") = "none"
                    End If
                Next

            End If

        End If

    End Sub

    Private Sub LoadEmployees()

        Me.cmbEmployeeOrigin.Items.Clear()

        Dim oEmployees As DataTable = API.EmployeeServiceMethods.GetAllEmployees(Me, "", "Employees")
        Dim dataview = New DataView(oEmployees)
        dataview.Sort = "EmployeeName ASC"
        oEmployees = dataview.ToTable()
        If oEmployees IsNot Nothing AndAlso oEmployees.Rows.Count > 0 Then
            For Each oEmployee As DataRow In oEmployees.Rows
                Me.cmbEmployeeOrigin.Items.Add(oEmployee("EmployeeName"), oEmployee("IDEmployee"))
            Next

            If Me.cmbEmployeeOrigin.Items.Count > 0 And roTypes.Any2String(Me.cmbEmployeeOrigin.Value) = "" Then
                Me.cmbEmployeeOrigin.SelectedIndex = 0
            End If
        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim oOldFrame As Frame = Me.oActiveFrame

            Dim nCurrentFrame As Integer = Me.FramePos(Me.oActiveFrame) + 1
            'If nCurrentFrame = 4 Then nCurrentFrame = 5 'evitar mostrar frame para copiar permisos ya que su lógica no está implementada todavía
            If Me.Frames.Count > nCurrentFrame Then
                Me.oActiveFrame = Me.Frames(nCurrentFrame)
            End If

            Me.FrameChange(oOldFrame, Me.oActiveFrame)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim oOldFrame As Frame = Me.oActiveFrame

        Dim nCurrentFrame As Integer = Me.FramePos(Me.oActiveFrame) - 1
        'If nCurrentFrame = 4 Then nCurrentFrame = 3 'evitar mostrar frame para copiar permisos ya que su lógica no está implementada todavía

        Me.oActiveFrame = Me.Frames(nCurrentFrame)

        Me.FrameChange(oOldFrame, Me.oActiveFrame)

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
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckFrame(Me.oActiveFrame))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.CopyEmployees()
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

    Private Sub CloseWizard()
        Me.lblWelcome1.Text = Me.Language.Translate("End.EmployeeCopyWelcome1.Text", Me.DefaultScope)
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ""
        Else
            Me.lblWelcome2.Text = Me.Language.Translate("End.Error.EmployeeCopyWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ErrorDescription
            Me.lblWelcome3.ForeColor = Drawing.Color.Red
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)
    End Sub

#End Region

#Region "Methods"

    Protected Function CopyEmployees() As Integer
        Dim iTask As Integer = -1

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim lstUserFieldsNoHistory As New Generic.List(Of String)
            Dim lstUserFieldsHistory As New Generic.List(Of String)
            Dim lstAssignments As New Generic.List(Of String)
            Dim lstCenters As New Generic.List(Of String)

            If chkAssignments.Checked AndAlso Me.hdnAssignments.Value <> "" Then
                For Each strFieldName As String In Me.hdnAssignments.Value.ToString.Split(",")
                    If strFieldName <> "" Then
                        lstAssignments.Add(strFieldName)
                    End If
                Next
            Else
                lstAssignments.Clear()
            End If

            If chkCenters.Checked AndAlso Me.hdnCenters.Value <> "" Then
                For Each strFieldName As String In Me.hdnCenters.Value.ToString.Split("@")
                    If strFieldName <> "" Then
                        lstCenters.Add(strFieldName)
                    End If
                Next
            Else
                lstCenters.Clear()
            End If

            If Me.chkUserFields.Checked Then
                If Me.hdnUserFields.Value <> "" Then
                    For Each strFieldName As String In Me.hdnUserFields.Value.ToString.Split(",")
                        If strFieldName <> "" Then
                            lstUserFieldsNoHistory.Add(strFieldName)
                        End If
                    Next
                Else
                    lstUserFieldsNoHistory.Clear()
                End If

                If Me.hdnUserFieldsHistory.Value <> "" Then
                    For Each strValue As String In Me.hdnUserFieldsHistory.Value.ToString.Split(",")
                        lstUserFieldsHistory.Add(strValue)
                        If lstUserFieldsNoHistory.Contains(strValue.Split("=")(0)) Then
                            lstUserFieldsNoHistory.Remove(strValue.Split("=")(0))
                        End If
                    Next
                Else
                    lstUserFieldsHistory.Clear()
                End If
            Else
                lstUserFieldsHistory.Clear()
                lstUserFieldsNoHistory.Clear()
            End If

            Dim employeesSelected As String = Me.hdnEmployeesSelected.Value & "@" & "Employees" & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

            Dim startDate As DateTime = Now
            Dim endDate As DateTime = Now

            If Me.txtScheduleBeginDate.Value IsNot Nothing Then startDate = Me.txtScheduleBeginDate.Date
            If Me.txtScheduleEndDate.Value IsNot Nothing Then endDate = Me.txtScheduleEndDate.Date

            iTask = API.LiveTasksServiceMethods.CopyEmployeesBackground(Me, roTypes.Any2Integer(Me.cmbEmployeeOrigin.Value), employeesSelected, chkAssignments.Checked, chkUserFields.Checked, lstUserFieldsHistory, lstUserFieldsNoHistory, chkLabAgree.Checked, lstAssignments,
                                                                                   Me.chkSchedule.Checked, startDate, endDate,
                                                                                    ckCopyMainShifts.Checked, ckCopyAlternativeShifts.Checked, ckCopyHolidays.Checked, ckKeepBloquedDays.Checked, ckKeepHolidays.Checked, chkCenters.Checked, lstCenters, True)
        End If

        Return iTask
    End Function

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
            Case Wizards_EmployeeCopyWizard.Frame.frmEmployeeOrigin
                If cmbEmployeeOrigin.SelectedIndex < 0 Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_EmployeeCopyWizard.Frame.frmEmployeesSelector

                ' Hemos de tener algún grupo seleccionado
                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case Wizards_EmployeeCopyWizard.Frame.frmUserFields
                If Me.chkUserFields.Checked Then
                    Dim arrUserFields(-1) As String
                    Dim arrUserFieldsHistory(-1) As String

                    If Me.hdnUserFields.Value <> "" Then arrUserFields = Me.hdnUserFields.Value.ToString.Split(",")
                    If Me.hdnUserFieldsHistory.Value <> "" Then arrUserFieldsHistory = Me.hdnUserFieldsHistory.Value.ToString.Split(",")

                    'Comprovem que si hi han camps de la fitxa amb historic, hagin triat algun valor
                    If arrUserFieldsHistory.Length > 0 Then
                        For n As Integer = LBound(arrUserFieldsHistory) To UBound(arrUserFieldsHistory)
                            If arrUserFieldsHistory(n) <> "" Then
                                Dim strValue As String = arrUserFieldsHistory(n).Split("=")(1)
                                If strValue = "" Then
                                    strMsg = Me.Language.Translate("CheckPage.UserFieldHistoryRequired", Me.DefaultScope)
                                    Exit For
                                End If
                            End If
                        Next
                    End If

                    'Si no hi ha camps de la fitxa, donem missatge error
                    If arrUserFields.Length = 0 Then
                        strMsg = Me.Language.Translate("CheckPage.UserFieldRequired", Me.DefaultScope)
                    End If

                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case Wizards_EmployeeCopyWizard.Frame.frmAssignments
                If Me.chkAssignments.Checked Then
                    Dim arrAssignments(-1) As String

                    If Me.hdnAssignments.Value <> "" Then arrAssignments = Me.hdnAssignments.Value.ToString.Split(",")

                    'Si no hi ha camps de la fitxa, donem missatge error
                    If arrAssignments.Length = 0 Then
                        strMsg = Me.Language.Translate("CheckPage.AssignmentRequired", Me.DefaultScope)
                    End If

                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep9Error.Text = strMsg

            Case Wizards_EmployeeCopyWizard.Frame.frmCenters
                If Me.chkCenters.Checked Then
                    Dim arrCenters(-1) As String

                    If Me.hdnCenters.Value <> "" Then arrCenters = Me.hdnCenters.Value.ToString.Split(",")

                    'Si no hi ha camps de la fitxa, donem missatge error
                    If arrCenters.Length = 0 Then
                        strMsg = Me.Language.Translate("CheckPage.CenterRequired", Me.DefaultScope)
                    End If

                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep10Error.Text = strMsg
            Case Wizards_EmployeeCopyWizard.Frame.frmSchedule
                If Me.chkSchedule.Checked Then
                    If Me.txtScheduleBeginDate.Value Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.BeginCopyScheduleIncorrect", Me.DefaultScope)
                    ElseIf Me.txtScheduleEndDate.Value Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.EndCopyScheduleIncorrect", Me.DefaultScope)
                    ElseIf Me.txtScheduleBeginDate.Date > Me.txtScheduleEndDate.Date Then
                        strMsg = Me.Language.Translate("CheckPage.CopySchedulePeriodIncorrect", Me.DefaultScope)
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep6Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmEmployeesSelector
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

        End Select

        Select Case oActiveFrame
            Case Frame.frmEmployeesSelector
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmpEmployeeCopyWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmpEmployeeCopyWizard&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)

                Me.ifEmployeesSelector.Disabled = False

            Case Frame.frmUserFields

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
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Info")
            If oLabel IsNot Nothing Then oLabel.Text = Me.hdnSetpInfo.Text
        Next

    End Sub

    Private Sub LoadUserFields()
        Try

            'Dim oUserFieldsPermission As Permission     ' Permiso configurado sobre la información de la ficha del empleado actual ('Employees.UserFields.Information')

            'oUserFieldsPermission = Me.GetFeaturePermission("Employees.UserFields.Information")

            'If (oUserFieldsPermission >= Permission.Read) Then
            Dim dTUserField As New HtmlTable ' Taula
            Dim dtRowUF As HtmlTableRow ' Files
            Dim dtCellUF As HtmlTableCell ' Celdes

            Dim imgUF As HtmlImage 'Imatge (~/Images/UserField 16.png)
            Dim hUF As HtmlAnchor 'Anchor
            Dim chkUF As HtmlInputCheckBox
            'Dim cmbUF As Robotics.WebControls.roComboBox

            Dim cmbDev As DevExpress.Web.ASPxComboBox

            Dim imgAlertUF As HtmlImage 'Imatge alerta si el camp llença recalcul, etc.

            Dim oUserFields As Generic.List(Of roUserField) = API.UserFieldServiceMethods.GetUserFieldList(Me, Types.EmployeeField, True, True, False)
            If oUserFields IsNot Nothing Then
                For Each oUserField As roUserField In oUserFields
                    If Not oUserField.ReadOnly Then
                        dtRowUF = New HtmlTableRow
                        dtCellUF = New HtmlTableCell

                        'Celda Imatge
                        imgUF = New HtmlImage
                        imgUF.ID = "img" & oUserField.FieldName
                        imgUF.Src = Page.ResolveUrl("~/Employees/Images/UserField 16.png")
                        dtCellUF.Controls.Add(imgUF)
                        dtRowUF.Cells.Add(dtCellUF)

                        'Celda Checkbox
                        dtCellUF = New HtmlTableCell
                        chkUF = New HtmlInputCheckBox
                        chkUF.ID = "chk" & oUserField.FieldName
                        chkUF.Attributes("name") = "chkUserFields"
                        chkUF.Attributes("onclick") = "retrieveChecks();"
                        dtCellUF.Controls.Add(chkUF)
                        dtRowUF.Cells.Add(dtCellUF)

                        'Celda anchor
                        dtCellUF = New HtmlTableCell
                        hUF = New HtmlAnchor
                        hUF.HRef = "javascript: void(0);"
                        hUF.InnerHtml = oUserField.FieldName
                        hUF.Style("width") = "180px"
                        hUF.Attributes("onclick") = "CheckLinkClick('chkUserFields_" & chkUF.ClientID & "');"
                        dtCellUF.Controls.Add(hUF)
                        dtRowUF.Cells.Add(dtCellUF)

                        'Si te historic
                        If oUserField.History = True Then
                            'Columna combobox
                            dtCellUF = New HtmlTableCell
                            hUF = New HtmlAnchor

                            'cmbUF = New Robotics.WebControls.roComboBox
                            'cmbUF.ID = "cmb" & oUserField.FieldName
                            'cmbUF.ChildsVisible = 3
                            'cmbUF.AddItem(Me.Language.Translate("History.NowAndFuture", DefaultScope), "1", "retrieveChecks();")
                            'cmbUF.AddItem(Me.Language.Translate("History.OnlyFuture", DefaultScope), "2", "retrieveChecks();")
                            'cmbUF.DocTypeEnabled = True
                            'dtCellUF.Controls.Add(cmbUF)

                            cmbDev = New DevExpress.Web.ASPxComboBox
                            cmbDev.ID = "cmbDev_" & oUserField.FieldName
                            cmbDev.Items.Add(New DevExpress.Web.ListEditItem("", "0"))
                            cmbDev.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("History.NowAndFuture", DefaultScope), "1"))
                            cmbDev.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("History.OnlyFuture", DefaultScope), "2"))
                            cmbDev.ClientInstanceName = "cmbDevClient_" & oUserField.FieldName
                            cmbDev.ClientSideEvents.SelectedIndexChanged = "retrieveChecks"
                            cmbDev.SelectedIndex = 0
                            dtCellUF.Controls.Add(cmbDev)

                            dtRowUF.Cells.Add(dtCellUF)

                            'Afegim missatge sols si es vol donar avis
                        ElseIf oUserField.InProcess Then
                            dtCellUF = New HtmlTableCell
                            imgAlertUF = New HtmlImage
                            imgAlertUF.Src = Page.ResolveUrl("~/Images/Alert16.png")
                            imgAlertUF.Attributes("title") = Me.Language.Translate("History.IsInUseProcess", DefaultScope)
                            dtCellUF.Controls.Add(imgAlertUF)
                            dtRowUF.Cells.Add(dtCellUF)
                        Else
                            dtCellUF = New HtmlTableCell
                            dtCellUF.ColSpan = 2
                            dtCellUF.InnerText = " "
                            dtRowUF.Cells.Add(dtCellUF)
                        End If

                        dTUserField.Rows.Add(dtRowUF)
                    End If

                Next

            End If
            panUserFields.Controls.Add(dTUserField)
            'End If
        Catch ex As Exception
            Dim lblError As New HtmlGenericControl("SPAN")
            lblError.InnerHtml = ex.Message & " " & ex.StackTrace
            panUserFields.Controls.Add(lblError)
        End Try

    End Sub

    Private Sub LoadCenters()
        Try

            Dim dTCenter As New HtmlTable ' Taula
            Dim dtRowUF As HtmlTableRow ' Files
            Dim dtCellUF As HtmlTableCell ' Celdes

            Dim imgUF As HtmlImage 'Imatge (~/Images/UserField 16.png)
            Dim hUF As HtmlAnchor 'Anchor
            Dim chkUF As HtmlInputCheckBox ' Checkbox

            If Me.hdnIDEmployeeSource.Value = "" Then
                Exit Sub
            End If

            Dim oCenters As DataTable = API.TasksServiceMethods.GetEmployeeBusinessCentersDataTable(Me, Me.hdnIDEmployeeSource.Value, False)

            If oCenters IsNot Nothing Then
                For Each oCenter As DataRow In oCenters.Rows
                    dtRowUF = New HtmlTableRow
                    dtCellUF = New HtmlTableCell

                    'Celda Imatge
                    imgUF = New HtmlImage
                    imgUF.ID = "img" & oCenter("Name")
                    imgUF.Src = Page.ResolveUrl("~/Employees/Images/UserField 16.png")
                    dtCellUF.Controls.Add(imgUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    'Celda Checkbox
                    dtCellUF = New HtmlTableCell
                    chkUF = New HtmlInputCheckBox
                    chkUF.ID = "chk" & oCenter("Name")
                    chkUF.Attributes("name") = "chkAssignment"
                    chkUF.Attributes("onclick") = "retrieveChecksCenters();"
                    dtCellUF.Controls.Add(chkUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    'Celda anchor
                    dtCellUF = New HtmlTableCell
                    hUF = New HtmlAnchor
                    hUF.HRef = "javascript: void(0);"
                    hUF.InnerHtml = oCenter("Name")
                    hUF.Style("width") = "180px"
                    hUF.Attributes("onclick") = "CheckLinkClick('chkCenters_" & chkUF.ClientID & "');"
                    dtCellUF.Controls.Add(hUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    dtCellUF = New HtmlTableCell
                    dtCellUF.ColSpan = 2
                    dtCellUF.InnerText = " "
                    dtRowUF.Cells.Add(dtCellUF)

                    dTCenter.Rows.Add(dtRowUF)
                Next

            End If
            panCenters.Controls.Add(dTCenter)
        Catch ex As Exception
            Dim lblError As New HtmlGenericControl("SPAN")
            lblError.InnerHtml = ex.Message & " " & ex.StackTrace
            panCenters.Controls.Add(lblError)
        End Try

    End Sub

    Private Sub LoadAssignments()
        Try

            Dim dTAssigment As New HtmlTable ' Taula
            Dim dtRowUF As HtmlTableRow ' Files
            Dim dtCellUF As HtmlTableCell ' Celdes

            Dim imgUF As HtmlImage 'Imatge (~/Images/UserField 16.png)
            Dim hUF As HtmlAnchor 'Anchor
            Dim chkUF As HtmlInputCheckBox ' Checkbox

            'Dim oAssignments As Generic.List(Of AssignmentService.roAssignment) = AssignmentService.AssignmentServiceMethods.GetAssignments(Me, "Name", False)

            If Me.hdnIDEmployeeSource.Value = "" Then
                Exit Sub
            End If

            Dim oAssignments As DataTable = API.EmployeeServiceMethods.GetAssignments(Me, Me.hdnIDEmployeeSource.Value, False)

            If oAssignments IsNot Nothing Then
                For Each oAssignment As DataRow In oAssignments.Rows
                    dtRowUF = New HtmlTableRow
                    dtCellUF = New HtmlTableCell

                    'Celda Imatge
                    imgUF = New HtmlImage
                    imgUF.ID = "img" & oAssignment("Name")
                    imgUF.Src = Page.ResolveUrl("~/Employees/Images/UserField 16.png")
                    dtCellUF.Controls.Add(imgUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    'Celda Checkbox
                    dtCellUF = New HtmlTableCell
                    chkUF = New HtmlInputCheckBox
                    chkUF.ID = "chk" & oAssignment("Name")
                    chkUF.Attributes("name") = "chkAssignment"
                    chkUF.Attributes("onclick") = "retrieveChecksAssignments();"
                    dtCellUF.Controls.Add(chkUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    'Celda anchor
                    dtCellUF = New HtmlTableCell
                    hUF = New HtmlAnchor
                    hUF.HRef = "javascript: void(0);"
                    hUF.InnerHtml = oAssignment("Name")
                    hUF.Style("width") = "180px"
                    hUF.Attributes("onclick") = "CheckLinkClick('chkAssignments_" & chkUF.ClientID & "');"
                    dtCellUF.Controls.Add(hUF)
                    dtRowUF.Cells.Add(dtCellUF)

                    dtCellUF = New HtmlTableCell
                    dtCellUF.ColSpan = 2
                    dtCellUF.InnerText = " "
                    dtRowUF.Cells.Add(dtCellUF)

                    dTAssigment.Rows.Add(dtRowUF)
                Next

            End If
            panAssignments.Controls.Add(dTAssigment)
        Catch ex As Exception
            Dim lblError As New HtmlGenericControl("SPAN")
            lblError.InnerHtml = ex.Message & " " & ex.StackTrace
            panAssignments.Controls.Add(lblError)
        End Try

    End Sub

#End Region

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class