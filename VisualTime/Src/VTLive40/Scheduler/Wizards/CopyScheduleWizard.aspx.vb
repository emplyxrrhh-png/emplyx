Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Forms_CopyScheduleWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("CSW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("CSW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("CSW_iCurrentTask") = value
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
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000
        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oCurrentPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission >= Permission.Write And Me.oCurrentPermission >= Permission.Read Then

            Me.LoadList()

            If Not Me.IsPostBack Then

                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text
                Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
                Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
                Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text

                Me.txtBeginDate.Date = Now.Date
                Me.txtEndDate.Date = Now.Date

                Me.intActivePage = 0

                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.intIDEmployee, False)
                If oEmployee IsNot Nothing Then
                    Me.lblCopyScheduleWelcome2.Text = Me.lblCopyScheduleWelcome2.Text.Replace("{0}", oEmployee.Name)
                    Me.lblStep2Info2.Text = Me.lblStep2Info2.Text.Replace("{0}", oEmployee.Name)
                End If

                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpCopyScheduleWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpCopyScheduleWizardGrid")

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
            Else

                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
                If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
                If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4

            End If

            Me.GroupFilterType.addOPanel(Me.optAllEmployee)
            Me.GroupFilterType.addOPanel(Me.optCondition)
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
                iCurrentTask = Me.CopyShifts()
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

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If Me.optCondition.Checked Then
                    'If Me.ddlUserFields.SelectedIndex = -1 Or Me.txtValue.Text = "" Then
                    '    strMsg = Me.Language.Translate("CheckPage.Page1", Me.DefaultScope)
                    'End If

                    If Me.cmbUserFields_Value.Value = "" Or Me.txtValue.Text = "" Then
                        strMsg = Me.Language.Translate("CheckPage.Page1", Me.DefaultScope)
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

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

                If txtBeginDate.Value Is Nothing OrElse txtEndDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectDates", Me.DefaultScope)
                ElseIf Me.txtBeginDate.Date > Me.txtEndDate.Date Then
                    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1
                Dim strFilter As String = ""
                If Me.optCondition.Checked Then
                    strFilter = "Employees.[" & Me.cmbUserFields_Value.Value & "] LIKE '" & Me.txtValue.Text.Replace("*", "%") & "'"
                End If
                HelperWeb.EmployeeSelector_SetUserFieldsFilter(strFilter)

            Case 2
                ' Desactivar el iframe del selector de grupos
                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True
        End Select

        Select Case intActivePage
            Case 2
                ' Inicializar selección del selector de empleados
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmpCopyScheduleWizard&FeatureAlias=Calendar.Scheduler&PrefixCookie=objContainerTreeV3_treeEmpCopyScheduleWizardGrid&" &
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

        Dim oUserFields As Generic.List(Of roUserField) = API.UserFieldServiceMethods.GetUserFieldList(Me, Types.EmployeeField, True, False, False)
        If oUserFields IsNot Nothing Then
            'With Me.ddlUserFields
            '    For Each oUserField As EmployeeService.roUserField In oUserFields.UserFields
            '        .Items.Add(New ListItem(oUserField.FieldCaption, oUserField.FieldName))
            '    Next
            'End With
            With Me.cmbUserFields
                For Each oUserField As roUserField In oUserFields
                    .AddItem("USR_" & oUserField.FieldName, oUserField.FieldName, "")
                    '.Items.Add(New ListItem(oUserField.FieldCaption, oUserField.FieldName))
                Next
            End With

        End If

        'With Me.ddlTypes
        '    .Items.Add(New ListItem(Me.Language.Translate("ShiftTypes.1", Me.DefaultScope), EmployeeService.ShiftType.PrimaryShift))
        '    .Items.Add(New ListItem(Me.Language.Translate("ShiftTypes.2", Me.DefaultScope), EmployeeService.ShiftType.AlterShift))
        '    .Items.Add(New ListItem(Me.Language.Translate("ShiftTypes.3", Me.DefaultScope), EmployeeService.ShiftType.AllShift))
        'End With

        ' Miramos si tiene la licencia de horarios alternativos
        If Not API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultipleShifts") Then
            ckCopyAlternativeShifts.Checked = False
            ckCopyAlternativeShifts.Enabled = False
        End If

    End Sub

    ''' <summary>
    ''' Retorna de la cadena de selecció sols els codis d'usuari marcats
    ''' </summary>
    ''' <param name="strSelection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function LoadEmployeesSelected(ByVal strSelection As String) As Generic.List(Of String)
        Dim lstEmployeeSelection As Generic.List(Of String) = New Generic.List(Of String)
        If strSelection <> "" Then
            Dim Selection() As String = strSelection.Trim.Split(",")
            If Selection.Length > 0 Then
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" Then
                        Select Case Sel.Substring(0, 1)
                            Case "A" ' Grupo
                                'lstGroupSelection.Add(CInt(Sel.Substring(1)))
                            Case "B" ' Empleado
                                lstEmployeeSelection.Add(CInt(Sel.Substring(1)))
                        End Select
                    End If
                Next
            End If
        End If
        Return lstEmployeeSelection
    End Function

    Private Function GetEmployees(ByVal strIDs As String) As Generic.List(Of Integer)

        Dim EmployeeIDs As New Generic.List(Of Integer)

        Dim intIDEmployee As Integer

        For Each strID As String In strIDs.Split(",")

            If strID.StartsWith("B") Then

                intIDEmployee = strID.Substring(1)
                If Not EmployeeIDs.Contains(intIDEmployee) Then
                    EmployeeIDs.Add(intIDEmployee)
                End If

            ElseIf strID.StartsWith("A") Then

                For Each intIDEmployee In Me.GetEmployeesFromGroup(strID.Substring(1))
                    If Not EmployeeIDs.Contains(intIDEmployee) Then
                        EmployeeIDs.Add(intIDEmployee)
                    End If
                Next

                Dim tbGroups As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Calendar")
                Dim oGroups() As DataRow = tbGroups.Select("(Path LIKE '%\" & strID.Substring(1) & "\%' OR Path LIKE '" & strID.Substring(1) & "\%') AND " &
                                                           "[ID] <> " & strID.Substring(1))
                For Each oRow As DataRow In oGroups
                    For Each intIDEmployee In Me.GetEmployeesFromGroup(oRow("ID"))
                        If Not EmployeeIDs.Contains(intIDEmployee) Then
                            EmployeeIDs.Add(intIDEmployee)
                        End If
                    Next
                Next

            End If
        Next

        Return EmployeeIDs

    End Function

    Private Function GetEmployeesFromGroup(ByVal _IDGroup As Integer) As Generic.List(Of Integer)

        Dim oRet As New Generic.List(Of Integer)

        Dim tb As DataTable = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroupWithType(Me, _IDGroup, "Calendar")
        If tb IsNot Nothing Then
            For Each oRow As DataRow In tb.Rows
                oRet.Add(oRow("IDEmployee"))
            Next
        End If

        Return oRet

    End Function

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Ok.CopyScheduleWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Ok.CopyScheduleWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.Error.CopyScheduleWelcome1.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Error.CopyScheduleWelcome2.Text", Me.DefaultScope)
            Me.lblCopyScheduleWelcome3.Text = ErrorDescription
            Me.lblCopyScheduleWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(4, 0)
    End Sub

    Private Function CopyShifts(Optional ByVal _LockedDayAction As LockedDayAction = LockedDayAction.None,
                           Optional ByVal _CoverageDayAction As LockedDayAction = LockedDayAction.None,
                           Optional ByVal _ShiftPermissionAction As ShiftPermissionAction = ShiftPermissionAction.None,
                           Optional ByVal _DateLocked As Date = Nothing,
                           Optional ByVal _IDEmployeeLocked As Integer = -1) As Integer

        Dim iTask As Integer = -1

        Dim employeesSelected As String = Me.hdnEmployeesSelected.Value & "@" & "Employees" & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

        Dim DateIni As DateTime = txtBeginDate.Date

        Me.lblCopyScheduleWelcome1.Text = Me.Language.Translate("End.CopyScheduleWelcome1.Text", Me.DefaultScope)
        iTask = API.LiveTasksServiceMethods.CopyShiftsInBackground(Me, Me.intIDEmployee, employeesSelected, txtBeginDate.Date, txtEndDate.Date,
                                                             ckCopyMainShifts.Checked, ckCopyAlternativeShifts.Checked, ckCopyHolidays.Checked, ckKeepBloquedDays.Checked, ckKeepHolidays.Checked, True)

        Return iTask
    End Function

#End Region

End Class