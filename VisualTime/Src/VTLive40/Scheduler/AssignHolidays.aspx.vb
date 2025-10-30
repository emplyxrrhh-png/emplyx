Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Scheduler_AssignHolidays
    Inherits PageBase

    Private Const FeatureAlias As String = "Calendar.Scheduler"

#Region "Declarations"

    Private intIDEmployee As Integer
    Private xDate As Date

    Private strCellsAssign As String

    Private _Action_Select As Integer = 0
    Private _Action_Selected As Integer = 1

    Private _Shifts_SelectClickIndex As Integer = 0
    Private _Shifts_selectCellIndex As Integer = 1
    Private _Shifts_ActionButtons() As String = {"imgSelect", "imgSelected"}

    Private oPermission As Permission

#End Region

#Region "Properties"

    Public Property ProcessedCells() As String
        Get
            Return roTypes.Any2String(ViewState("AddHolidays_ProcessedCells"))
        End Get
        Set(ByVal value As String)
            ViewState("AddHolidays_ProcessedCells") = value
        End Set
    End Property

    Public Property IDShiftSelected() As Integer
        Get
            Return roTypes.Any2Integer(ViewState("AddHolidays_IDShiftSelected"))
        End Get
        Set(ByVal value As Integer)
            ViewState("AddHolidays_IDShiftSelected") = value
        End Set
    End Property

    Private Property ShiftsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = ViewState("AddHolidays_ShiftsData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.ShiftServiceMethods.GetHolidayShifts(Me.Page, -1, False)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    tb.Rows.Add(tb.NewRow)
                End If

                ViewState("AddHolidays_ShiftsData") = tb

                ' Reestablecer ínidices selección 'grdScheduler'
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("AddHolidays_ShiftsData") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes()

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Read Then

            Me.intIDEmployee = roTypes.Any2Integer(Me.Request("IDEmployee"))
            Dim strDate As String = roTypes.Any2String(Request("SelectedDate")) ' formato dd/MM/yyyy
            If strDate.Length >= 10 And IsDate(strDate) Then
                Me.xDate = New DateTime(Year(roTypes.Any2Time(strDate).Value), Month(roTypes.Any2Time(strDate).Value), Day(roTypes.Any2Time(strDate).Value), 0, 0, 0)
                'Me.xDate = New DateTime(CInt(strDate.Substring(6, 4)), CInt(strDate.Substring(3, 2)), CInt(strDate.Substring(0, 2)), 0, 0, 0)
            End If

            Me.strCellsAssign = Request("cellsSelected")

            Me.btAccept.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
            End If
            If Not Me.IsPostBack Then

                Me.ShiftsData = Nothing

                Dim tbPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, Me.intIDEmployee, Me.xDate)

                Dim strEmployeeName As String = ""
                Dim strAssignmentName As String = ""
                Dim strShiftName As String = ""
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.intIDEmployee, False)
                If oEmployee IsNot Nothing Then strEmployeeName = oEmployee.Name

                Me.LoadData()

            End If

            ''Try
            ''    If Request.Form("__EVENTTARGET").EndsWith(Me._Employees_ActionButtons(Me._Action_Selected)) Then
            ''        Me.grdEmployees_EditAccept(Val(Request.Form("__EVENTARGUMENT")))
            ''    End If
            ''Catch
            ''End Try

            AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click
        If Me.treeShifts.SelectedNode IsNot Nothing AndAlso Me.treeShifts.SelectedNode.ChildNodes.Count = 0 Then
            If Me.treeShifts.SelectedNode.Value.Contains("*") Then
                Me.IDShiftSelected = Me.treeShifts.SelectedNode.Value.Split("*")(0)
                Me.SaveData()
            End If
        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Dim stopType As Integer = -1
        If strButtonKey.Split("_").Length >= 2 Then
            Select Case strButtonKey.Split("_")(1)
                Case "0"
                    stopType = 0
                Case "1"
                    stopType = 1
                Case "2"
                    stopType = 2
                Case "4"
                    stopType = 4
            End Select
        End If

        Dim oAction1 As LockedDayAction = LockedDayAction.None
        If strButtonKey.Split("_").Length >= 3 Then
            Dim strResponse As String = strButtonKey.Split("_")(2)
            Select Case strResponse
                Case "replacefirst"
                    oAction1 = LockedDayAction.ReplaceFirst
                Case "replaceall"
                    oAction1 = LockedDayAction.ReplaceAll
                Case "noreplacefirst"
                    oAction1 = LockedDayAction.NoReplaceFirst
                Case "noreplaceall"
                    oAction1 = LockedDayAction.NoReplaceAll
            End Select
        End If

        Dim oAction2 As LockedDayAction = LockedDayAction.None
        Select Case strButtonKey.Split("_")(0)
            Case "replacefirst"
                oAction2 = LockedDayAction.ReplaceFirst
            Case "replaceall"
                oAction2 = LockedDayAction.ReplaceAll
            Case "noreplacefirst"
                oAction2 = LockedDayAction.NoReplaceFirst
            Case "noreplaceall"
                oAction2 = LockedDayAction.NoReplaceAll
        End Select

        Dim oAction3 As ShiftPermissionAction = ShiftPermissionAction.None
        Select Case strButtonKey.Split("_")(0)
            Case "continueall"
                oAction3 = ShiftPermissionAction.ContinueAll
            Case "continueandstop"
                oAction3 = ShiftPermissionAction.ContinueAndStop
            Case "stopall"
                oAction3 = ShiftPermissionAction.StopAll
        End Select

        If (stopType = 4 AndAlso Me.strCellsAssign = String.Empty) Then
            Me.CanClose = True
            Me.MustRefresh = "AddHoliday"
        Else
            Me.SaveData(oAction1, oAction2, oAction3)
        End If

    End Sub

#End Region

#Region "Methods"

    Public Sub LoadData()

        Dim tbGroups As DataTable = API.ShiftServiceMethods.GetShiftGroupsPlanification(Me)

        Dim tbShifts As DataTable

        Dim oGroupNode As TreeNode
        Dim oShiftNode As TreeNode
        Dim strName As String
        For Each oGroup As DataRow In tbGroups.Rows

            If oGroup("ID") = 0 Then
                strName = Me.Language.Translate("Shifts.ShiftsMainGroup.literal", Me.DefaultScope)
            ElseIf Not IsDBNull(oGroup("Name")) Then
                strName = oGroup("Name")
            Else
                strName = ""
            End If
            oGroupNode = New TreeNode(strName, oGroup("ID"), "Images/TABLE_PROPERTIES_CLOCK_16.GIF")

            tbShifts = API.ShiftServiceMethods.GetHolidayShifts(Me.Page, oGroup("ID"))

            Dim strID As String = ""
            For Each oShift As DataRow In tbShifts.Rows
                strID = oShift("ID") & "*"
                oShiftNode = New TreeNode(oShift("Name"), strID, "Images/TABLE_PROPERTIES_CLOCK_16.GIF")
                oGroupNode.ChildNodes.Add(oShiftNode)
            Next

            Me.treeShifts.Nodes.Add(oGroupNode)

        Next

    End Sub

    Private Sub SaveData(Optional ByVal action1 As LockedDayAction = LockedDayAction.None,
                         Optional ByVal action2 As LockedDayAction = LockedDayAction.None,
                         Optional ByVal action3 As ShiftPermissionAction = ShiftPermissionAction.None)

        Dim result As Boolean = True

        If (Me.strCellsAssign = String.Empty) Then
            Dim dtPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, Me.intIDEmployee, Me.xDate)

            If dtPlan.Rows.Count > 0 Then

                Dim xStartShift1 As Date
                Dim xStartShift2 As Date
                Dim xStartShift3 As Date
                Dim xStartShift4 As Date
                If Not IsDBNull(dtPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtPlan.Rows(0)("StartShift1")
                If Not IsDBNull(dtPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtPlan.Rows(0)("StartShift2")
                If Not IsDBNull(dtPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtPlan.Rows(0)("StartShift3")
                If Not IsDBNull(dtPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtPlan.Rows(0)("StartShift4")

                Dim IDShift1 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift1"))
                Dim IDShift2 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift2"))
                Dim IDShift3 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift3"))
                Dim IDShift4 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift4"))

                Dim IDAssignment As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IDAssignment"))

                result = Me.AssignShift(Me.xDate, Me.intIDEmployee, IDShift1, IDShift2, IDShift3, IDShift4, IDAssignment, xStartShift1, xStartShift2, xStartShift3, xStartShift4, action1, action2, action3)
            Else
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.intIDEmployee, False)
                Dim oParams As New Generic.List(Of String)
                If (oEmployee IsNot Nothing) Then
                    oParams.Add(oEmployee.Name)
                Else
                    oParams.Add("Desconcido")
                End If
                oParams.Add(Format(Me.xDate, HelperWeb.GetShortDateFormat))

                HelperWeb.ShowOptionMessage(Me, Me.Language.Translate("NoBaseShiftAssigned.Title", "CopyScheduleWizard"), Me.Language.Translate("NoBaseShiftAssigned.Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate("NoBaseShiftAssigned.Continue.Title", "CopyScheduleWizard"), "Continue_4", Me.Language.Translate("NoBaseShiftAssigned.Continue.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", "", "", "", False, True, "", "", "", "", False, True, "", "", "", "", False, True, "", HelperWeb.MsgBoxIcons.InformationIcon)
                result = False
            End If
        Else
            Dim Cells() As String = strCellsAssign.Split(",")

            If action3 <> ShiftPermissionAction.StopAll Then
                For Each Cell As String In Cells
                    If Not ProcessedCells.Contains(Cell) Then

                        Dim strEmployee As String = Cell.Split("_")(0)
                        Dim strDate As String = Cell.Split("_")(1)

                        Dim arrDate As String() = strDate.Split("/")
                        Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

                        Dim dtPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, roTypes.Any2Integer(strEmployee), dDate)

                        If dtPlan.Rows.Count > 0 Then

                            Dim xStartShift1 As Date
                            Dim xStartShift2 As Date
                            Dim xStartShift3 As Date
                            Dim xStartShift4 As Date
                            If Not IsDBNull(dtPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtPlan.Rows(0)("StartShift1")
                            If Not IsDBNull(dtPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtPlan.Rows(0)("StartShift2")
                            If Not IsDBNull(dtPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtPlan.Rows(0)("StartShift3")
                            If Not IsDBNull(dtPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtPlan.Rows(0)("StartShift4")

                            Dim IDShift1 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift1"))
                            Dim IDShift2 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift2"))
                            Dim IDShift3 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift3"))
                            Dim IDShift4 As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IdShift4"))

                            Dim IDAssignment As Integer = roTypes.Any2Integer(dtPlan.Rows(0)("IDAssignment"))

                            result = Me.AssignShift(dDate, roTypes.Any2Integer(strEmployee), IDShift1, IDShift2, IDShift3, IDShift4, IDAssignment, xStartShift1, xStartShift2, xStartShift3, xStartShift4, action1, action2, action3)

                            If result Then

                                If ProcessedCells <> String.Empty Then Me.ProcessedCells = Me.ProcessedCells & ","
                                Me.ProcessedCells = Me.ProcessedCells & Cell

                                If action1 <> LockedDayAction.None Then
                                    If action1 <> LockedDayAction.NoReplaceAll And action1 <> LockedDayAction.ReplaceAll Then
                                        action1 = LockedDayAction.None
                                    End If
                                End If
                                If action2 <> LockedDayAction.None Then
                                    If action2 <> LockedDayAction.NoReplaceAll And action1 <> LockedDayAction.ReplaceAll Then
                                        action2 = LockedDayAction.None
                                    End If
                                End If
                                If action3 <> ShiftPermissionAction.None Then
                                    If action3 <> ShiftPermissionAction.ContinueAll And action1 <> ShiftPermissionAction.StopAll Then
                                        action3 = ShiftPermissionAction.None
                                    End If
                                End If
                            Else
                                Exit For
                            End If
                        End If
                    End If
                Next
            End If
        End If

        If result Then
            Me.CanClose = True
            Me.MustRefresh = "AddHoliday"
        Else
            Exit Sub
        End If

    End Sub

    Private Function AssignShift(ByVal selDate As Date, ByVal idEmployee As Integer, ByVal idShift1 As Integer, ByVal idShift2 As Integer, ByVal idShift3 As Integer, ByVal idShift4 As Integer, ByVal idAssignment As Integer,
                                 ByVal startShift1 As Date, ByVal startShift2 As Date, ByVal startShift3 As Date, ByVal startShift4 As Date,
                                 Optional ByVal action1 As LockedDayAction = LockedDayAction.None,
                                 Optional ByVal action2 As LockedDayAction = LockedDayAction.None,
                                 Optional ByVal action3 As ShiftPermissionAction = ShiftPermissionAction.None) As Boolean
        Dim saved As Boolean = False

        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, idEmployee, False)
        Dim oParams As New Generic.List(Of String)
        If (oEmployee IsNot Nothing) Then
            oParams.Add(oEmployee.Name)
        Else
            oParams.Add("Desconcido")
        End If
        oParams.Add(Format(selDate, HelperWeb.GetShortDateFormat))

        If Not API.EmployeeServiceMethods.AssignShift(Me.Page, idEmployee, selDate, Me.IDShiftSelected, idShift2, idShift3, idShift4,
                                                                  startShift1, startShift2, startShift3, startShift4, idAssignment,
                                                                  action1, action2, True, action3) Then
            Dim strLangTag As String = ""
            Dim stopType As Integer = 0
            Dim requestFunc As String = ""

            If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                stopType = 0
                strLangTag = "DailyScheduleLockedDay"
            ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                stopType = 1
                strLangTag = "DailyScheduleCoverageDay"
            ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                stopType = 2
                strLangTag = "DailyScheduleShiftWithoutPermission"
            ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoBaseShiftAssigned Then
                stopType = 4
                strLangTag = "NoBaseShiftAssigned"
            ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoWorkingDay Then
                stopType = 4
                strLangTag = "NoWorkingDay"
            End If

            If (stopType = 2) Then
                If (Me.strCellsAssign = String.Empty) Then
                    HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard", oParams), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate(strLangTag & ".ContinueAll.Title", "CopyScheduleWizard"), "ReplaceFirst_" & stopType.ToString & "_" & "continueall", Me.Language.Translate(strLangTag & ".ContinueAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".StopAll.Title", "CopyScheduleWizard"), "NoReplaceAll_" & stopType.ToString & "_" & "stopall", Me.Language.Translate(strLangTag & ".StopAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", "", "", "", False, True, "", "", "", "", False, True, "", HelperWeb.MsgBoxIcons.QuestionIcon)
                Else
                    HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard", oParams), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate(strLangTag & ".ReplaceFirst.Title", "CopyScheduleWizard"), "ReplaceFirst_" & stopType.ToString & "_" & "replacefirst", Me.Language.Translate(strLangTag & ".ReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".ReplaceAll.Title", "CopyScheduleWizard"), "ReplaceAll_" & stopType.ToString & "_" & "replaceall", Me.Language.Translate(strLangTag & ".ReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".NoReplaceFirst.Title", "CopyScheduleWizard"), "NoReplaceFirst_" & stopType.ToString & "_" & "noreplacefirst", Me.Language.Translate(strLangTag & ".NoReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".NoReplaceAll.Title", "CopyScheduleWizard"), "NoReplaceAll_" & stopType.ToString & "_" & "noreplaceall", Me.Language.Translate(strLangTag & ".NoReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", HelperWeb.MsgBoxIcons.QuestionIcon)
                End If

                saved = False
            ElseIf (stopType = 1 Or stopType = 0) Then
                If (Me.strCellsAssign = String.Empty) Then
                    HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard", oParams), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate(strLangTag & ".ReplaceAll.Title", "CopyScheduleWizard"), "ReplaceAll_" & stopType.ToString & "_" & "replaceall", Me.Language.Translate(strLangTag & ".ReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".NoReplaceAll.Title", "CopyScheduleWizard"), "NoReplaceAll_" & stopType.ToString & "_" & "noreplaceall", Me.Language.Translate(strLangTag & ".NoReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", "", "", "", False, True, "", "", "", "", False, True, "", HelperWeb.MsgBoxIcons.QuestionIcon)
                Else
                    HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard"), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate(strLangTag & ".ReplaceFirst.Title", "CopyScheduleWizard"), "ReplaceFirst_" & stopType.ToString & "_" & "replacefirst", Me.Language.Translate(strLangTag & ".ReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".ReplaceAll.Title", "CopyScheduleWizard"), "ReplaceAll_" & stopType.ToString & "_" & "replaceall", Me.Language.Translate(strLangTag & ".ReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".NoReplaceFirst.Title", "CopyScheduleWizard"), "NoReplaceFirst_" & stopType.ToString & "_" & "noreplacefirst", Me.Language.Translate(strLangTag & ".NoReplaceFirst.Description", "CopyScheduleWizard"), "", False, True,
                                                 Me.Language.Translate(strLangTag & ".NoReplaceAll.Title", "CopyScheduleWizard"), "NoReplaceAll_" & stopType.ToString & "_" & "noreplaceall", Me.Language.Translate(strLangTag & ".NoReplaceAll.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", HelperWeb.MsgBoxIcons.QuestionIcon)
                End If

                saved = False
            Else
                HelperWeb.ShowOptionMessage(Me, Me.Language.Translate(strLangTag & ".Title", "CopyScheduleWizard"), Me.Language.Translate(strLangTag & ".Description", "CopyScheduleWizard", oParams),
                                                 Me.Language.Translate(strLangTag & ".Continue.Title", "CopyScheduleWizard"), "Continue_" & stopType.ToString, Me.Language.Translate(strLangTag & ".Continue.Description", "CopyScheduleWizard"), "", False, True,
                                                 "", "", "", "", False, True, "", "", "", "", False, True, "", "", "", "", False, True, "", HelperWeb.MsgBoxIcons.InformationIcon)
                If Me.strCellsAssign <> String.Empty Then
                    saved = True
                Else
                    saved = False
                End If

            End If
        Else
            saved = True
        End If

        Return saved
    End Function

    Private Function RetMsgResponseShift(ByVal MsgResponse As String) As ShiftPermissionAction
        Select Case MsgResponse.ToUpper
            Case "NONE"
                Return ShiftPermissionAction.None
            Case "CONTINUEANDSTOP"
                Return ShiftPermissionAction.ContinueAndStop
            Case "CONTINUEALL"
                Return ShiftPermissionAction.ContinueAll
            Case "STOPALL"
                Return ShiftPermissionAction.StopAll
            Case Else
                Return ShiftPermissionAction.None
        End Select
    End Function

    Private Function RetMsgResponse(ByVal MsgResponse As String) As LockedDayAction
        Select Case MsgResponse.ToUpper
            Case "NONE"
                Return LockedDayAction.None
            Case "NOREPLACEALL"
                Return LockedDayAction.NoReplaceAll
            Case "NOREPLACEFIRST"
                Return LockedDayAction.NoReplaceFirst
            Case "REPLACEALL"
                Return LockedDayAction.ReplaceAll
            Case "REPLACEFIRST"
                Return LockedDayAction.ReplaceFirst
            Case Else
                Return LockedDayAction.None
        End Select
    End Function

#End Region

End Class