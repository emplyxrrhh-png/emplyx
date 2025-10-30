Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Wizards_MultiAbsencesWizard
    Inherits PageBase

#Region "Declarations"

    Private intEmployeeID As Integer

    Private Enum Frame
        frmWelcome '0
        frmEmployees '1
        frmAbsences '2
        frmResume '2
    End Enum

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("MAW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("MAW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("MAW_iCurrentTask") = value
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

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("MultiAbsencesWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployees)
                oFrames.Add(Frame.frmAbsences)
                oFrames.Add(Frame.frmResume)

                ViewState("MultiAbsencesWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("MultiAbsencesWizard_Frames") = value
        End Set
    End Property

    Private ReadOnly Property FreezeDate() As Nullable(Of Date)
        Get

            Dim oDate As Nullable(Of Date)

            If ViewState("NewMultiAbsencesWizard_FreezeDate") = Nothing Then

                Dim oParameters As roParameters = API.ConnectorServiceMethods.GetParameters(Me)
                If oParameters IsNot Nothing Then
                    Dim oParams As New roCollection(oParameters.ParametersXML)
                    Dim auxDate As Object
                    Try
                        auxDate = oParams.Item(oParameters.ParametersNames(Parameters.FirstDate))
                    Catch ex As Exception
                        auxDate = Nothing
                    End Try
                    If auxDate IsNot Nothing AndAlso IsDate(auxDate) Then
                        ViewState("NewMultiAbsencesWizard_FreezeDate") = CType(auxDate, Date).ToShortDateString()
                    Else
                        ViewState("NewMultiAbsencesWizard_FreezeDate") = String.Empty
                    End If
                End If

                Dim strDate As String = ViewState("NewMultiAbsencesWizard_FreezeDate")
                Dim Fecha As Date
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            Else

                Dim Fecha As Date
                Dim strDate As String = ViewState("NewMultiAbsencesWizard_FreezeDate")
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            End If

            Return oDate

        End Get
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

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Employees", Permission.Admin) Then
                Me.Frames = Nothing

                Me.SetStepTitles()

                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizardGrid")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityGroupWizard")

                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

                Me.oActiveFrame = Frame.frmWelcome
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

        If Not Me.IsPostBack Then
            LoadCausesCombo()
            Me.txtBeginDate.Value = Date.Now.Date
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

    Private Sub LoadCausesCombo()
        Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page, True)

        Dim dvCauses As New DataView(tb)
        dvCauses.RowFilter = "IsHoliday = 0 AND DayType = 0 AND CustomType=0"
        dvCauses.Sort = "Name"
        tb = dvCauses.ToTable

        cmbCausesList.Value = Nothing
        cmbCausesList.DataSource = tb
        cmbCausesList.TextField = "Name"
        cmbCausesList.ValueField = "Id"
        cmbCausesList.ValueType = GetType(Integer)
        cmbCausesList.DataBind()
    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim oOldFrame As Frame = Me.oActiveFrame
        Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) - 1)

        Me.FrameChange(oOldFrame, Me.oActiveFrame)

    End Sub

    Protected Function AssignMassiveAbsences(ByVal iType As Integer) As Integer
        Dim ret As Integer

        'Dim dBeginTime As DateTime = txtNormalHourBegin.DateTime
        'Dim dEndTime As DateTime = txtNormalHourEnd.DateTime

        Dim dBeginTime As DateTime = New DateTime(1899, 12, 30, txtNormalHourBegin.DateTime.Hour, txtNormalHourBegin.DateTime.Minute, 0)
        Dim dEndTime As DateTime = New DateTime(1899, 12, 30, txtNormalHourEnd.DateTime.Hour, txtNormalHourEnd.DateTime.Minute, 0)

        Dim dEndDate As DateTime = New DateTime
        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim strErrorInfo As String = ""

            Dim selectedDate As Date = Date.Now.Date

        End If

        Dim massType As eRequestType = eRequestType.PlannedCauses

        If iType = 1 Then 'dies
            massType = eRequestType.PlannedAbsences
            dEndDate = txtEndDate.Date.Date
            If chkMaxDays.Checked Then dEndDate = Date.MinValue
        Else
            dEndDate = txtEndDate.Date.Date
            If iType = 2 Then massType = eRequestType.PlannedOvertimes
            If dBeginTime > dEndTime Then dEndTime = dEndTime.AddDays(1)
        End If

        ret = API.LiveTasksServiceMethods.MassProgrammedAbsenceBackground(Me.Page, Me.hdnEmployeesSelected.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value,
                                                                                       cmbCausesList.SelectedItem.Value,
                                                                                       txtBeginDate.Date.Date, dEndDate,
                                                                                       txtDescription.Text,
                                                                                       If(((txtMinDuration.Text = "") Or (iType = 1)), 0, roTypes.Any2Time(txtMinDuration.Text).NumericValue),
                                                                                       If((txtDuration.Text = "") Or (iType = 1), 0, roTypes.Any2Time(txtDuration.Text).NumericValue),
                                                                                       Me.txtMaxDays.Value,
                                                                                       dBeginTime, dEndTime,
                                                                                       massType, True)

        If ret = -1 Then
            Me.CanClose = False
            Me.MustRefresh = "0"
        End If

        Return ret

    End Function

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

            Case Wizards_MultiAbsencesWizard.Frame.frmEmployees
                If Me.hdnEmployeesSelected.Value.Trim = "" Then
                    strMsg = Me.Language.Translate("CheckPage.NoEmployeeSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_MultiAbsencesWizard.Frame.frmAbsences
                Dim selectedDate As Date = Date.Now.Date

                If txtBeginDate.Date = Date.MinValue Then
                    strMsg = Me.Language.Translate("CheckPage.InitialDateError", Me.DefaultScope) '"Debe indicar una fecha inicial"
                End If

                If strMsg = String.Empty Then
                    If chkMaxDays.Checked Then
                        If roTypes.Any2Integer(txtMaxDays.Text) = 0 Then
                            strMsg = Me.Language.Translate("CheckPage.WorkingDaysError", Me.DefaultScope)
                        End If
                    Else
                        If txtEndDate.Date = Date.MinValue OrElse txtEndDate.Date < txtBeginDate.Date Then
                            strMsg = Me.Language.Translate("CheckPage.EndDateError", Me.DefaultScope)
                        End If
                    End If
                End If

                If strMsg = String.Empty Then
                    If Me.cmbCausesList.SelectedItem Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.CauseNotSelected", Me.DefaultScope)
                    Else
                        Dim tb As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page, True)

                        Dim dvCauses As New DataView(tb)
                        dvCauses.RowFilter = "Id = " & Me.cmbCausesList.SelectedItem.Value.ToString()
                        dvCauses.Sort = "Name"
                        tb = dvCauses.ToTable

                        If tb.Rows.Count = 1 Then
                            Dim bWorkingMode As Boolean = roTypes.Any2Boolean(tb.Rows(0)("WorkingType"))
                            If bWorkingMode AndAlso opOvertimeHours.Checked = False Then
                                strMsg = Me.Language.Translate("CheckPage.NotWorkingCause", Me.DefaultScope)
                            ElseIf Not bWorkingMode AndAlso opOvertimeHours.Checked Then
                                strMsg = Me.Language.Translate("CheckPage.NotAbsenceCause", Me.DefaultScope)
                            End If
                        End If

                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            'Case Wizards_NewMultiAbsencesWizard.Frame.frmGroup

            Case Wizards_MultiAbsencesWizard.Frame.frmEmployees
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True
            Case Wizards_MultiAbsencesWizard.Frame.frmAbsences

        End Select

        Select Case oActiveFrame
            Case Wizards_MultiAbsencesWizard.Frame.frmEmployees
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeMultiEmployeeMobilityEmployeesWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)

                Me.ifEmployeesSelector.Disabled = False
            Case Wizards_MultiAbsencesWizard.Frame.frmAbsences

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
            Me.btNext.Visible = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) <Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub SetStepTitles()

        Dim oLabel As Label = New Label
        oLabel.Text = ""
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

    Protected Sub cmbCausesList_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCausesList.SelectedIndexChanged
        Dim IDCause As Integer = roTypes.Any2Integer(cmbCausesList.Value)
        Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Me.Page, IDCause, False)

        If oCause IsNot Nothing AndAlso oCause.MaxProgrammedAbsence > 0 Then
            chkMaxDays.Checked = True
            chkEndDate.Checked = False
            txtMaxDays.Value = oCause.MaxProgrammedAbsence
            'txtBeginDate.Value = Nothing
        ElseIf oCause IsNot Nothing AndAlso oCause.Absence_MaxDays.HasValue Then
            chkMaxDays.Checked = True
            chkEndDate.Checked = False
            txtMaxDays.Value = oCause.Absence_MaxDays
            'txtBeginDate.Value = Nothing
        Else
            chkMaxDays.Checked = False
            chkEndDate.Checked = True
            txtMaxDays.Value = 0
        End If

        If opAbsenceDays.Checked() = True Then

            txtNormalHourBegin.ClientEnabled = False
            txtNormalHourEnd.ClientEnabled = False
            txtMinDuration.ClientEnabled = False
            txtDuration.ClientEnabled = False
        Else
            txtNormalHourBegin.ClientEnabled = True
            txtNormalHourEnd.ClientEnabled = True
            txtMinDuration.ClientEnabled = True
            txtDuration.ClientEnabled = True
        End If

    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", True)
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                                PerformActionCallback.JSProperties.Add("cpActionMsg", "")
                            Case 2
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                                PerformActionCallback.JSProperties.Add("cpActionMsg", "OK")
                            Case 3
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                        PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                End If
            Case Else
                If strParameter.Trim.ToUpperInvariant.StartsWith("PERFORM_ACTION") Then
                    PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                    iCurrentTask = Me.AssignMassiveAbsences(roTypes.Any2Integer(strParameter.Trim.ToUpperInvariant.Replace("PERFORM_ACTION#", "")))
                End If

        End Select

    End Sub

    Private Sub btnActionResult_Click(sender As Object, e As EventArgs) Handles btnActionResult.Click

        If Not ErrorExists Then
            Me.lblWelcome1.Text = Me.Language.Translate("MultiAbsencesWizard.Title.Text", Me.DefaultScope,,, "Asistente para introducción masiva de ausencias")
            Me.lblWelcome2.Text = Me.Language.Translate("MultiAbsencesWizard.FinishedOk.Text", Me.DefaultScope,,, "Ausencias creadas correctamente")
            Me.lblWelcome3.Text = Me.Language.Translate("MultiAbsencesWizard.ClickCloseToContinue.Text", Me.DefaultScope,,, "Para continuar, haga clic en Cerrar.")
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)
        Else

            Me.lblWelcome1.Text = Me.Language.Translate("MultiAbsencesWizard.Title.Text", Me.DefaultScope,,, "Asistente para introducción masiva de ausencias")
            Me.lblWelcome2.Text = ErrorDescription
            Me.lblWelcome3.Text = Me.Language.Translate("MultiAbsencesWizard.ClickCloseToContinue.Text", Me.DefaultScope,,, "Para continuar, haga clic en Cerrar.")
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

        End If

    End Sub

#End Region

End Class