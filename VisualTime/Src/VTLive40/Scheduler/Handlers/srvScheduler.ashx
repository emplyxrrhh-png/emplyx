<%@ WebHandler Language="VB" Class="srvScheduler" %>

Imports System
Imports System.Web
Imports System.Text
Imports System.Data
Imports System.Runtime.Serialization
Imports Robotics.Web.Base
Imports Robotics.VTBase
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTBusiness.Scheduler
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Shift


Public Class srvScheduler
    Inherits handlerBase

    Private Const FeatureAlias As String = "Calendar"
    Private Const FeatureAliasPlan As String = "Calendar.Scheduler"

    Private mDateStart As Date        'Data Inici
    Private mDateEnd As Date          'Data Fin

    Private oPermission As Permission
    Private oPermissionPlan As Permission

    Private bolHRScheduling As Boolean = False

    Private Structure SchedulerOptionsStruct
        Dim IDGroup As Integer
        Dim IDEmployee As Integer
        Dim TypeView As Integer
        Dim IncludeSubGroups As Boolean
        Dim UserFieldName As String
        Dim UserFieldValue As String
    End Structure

    'Solo se utiliza para vacaciones
    Public Property ProcessedCells() As String
        Get
            Return roTypes.Any2String(Me.Session("RemoveHolidays_ProcessedCells"))
        End Get
        Set(ByVal value As String)
            Session("RemoveHolidays_ProcessedCells") = value
        End Set
    End Property


    Public Overrides Sub SetActionsNotRequieredUpdateSession()
        FeaturesNotRequiereUpdateSession = {"getDayPlan", "getDayHeaderPlan", "getDayHeadersPlan"}
    End Sub

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermissionPlan = Me.GetFeaturePermission(FeatureAliasPlan)

        Me.bolHRScheduling = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")

        If Me.oPermission > Permission.None Then
            Select Case Request("action")
                Case "getScheduler" ' Retorna el calendari principal

                Case "getSchedulerDiaryParts" ' Retorna el calendari principal
                    LoadSchedulerDiaryParts()
                Case "getSchedulerPlanification" ' Retorna el calendari principal

                Case "getIDGroupByEmployee" 'Retorna el ID del Grup de l'usuari
                    getIDGroupByEmployee()
                Case "getPeriodDescription" 'Retorna el texte descriptiu del Periode
                    getPeriodDescription()
                Case "savePlan" ' Graba el Horari
                    If Me.oPermissionPlan > Permission.Read Then savePlan()
                Case "savePlan2" ' Graba el Horari
                    If Me.oPermissionPlan > Permission.Read Then savePlan2()
                Case "assignPlan" ' Asigna el horario
                    If Me.oPermissionPlan > Permission.Read Then AssignPlan()
                Case "copyPlan" ' Copia la planificacio (sols 1 dia)
                    If Me.oPermissionPlan > Permission.Read Then copyPlan()
                Case "copyPlan2" ' Copia la planificacio (sols 1 dia)
                    If Me.oPermissionPlan > Permission.Read Then copyPlan2()
                Case "pasteMultiPlan" ' Copia la planificacio (sols 1 dia)
                    If Me.oPermissionPlan > Permission.Read Then pasteMultiPlan()
                Case "copySpecialPlan" ' Enganxat Especial (sols 1 dia)
                    If Me.oPermissionPlan > Permission.Read Then copySpecialPlan()
                Case "copySpecialMultiPlan" ' Enganxat Especial (sols 1 dia)
                    If Me.oPermissionPlan > Permission.Read Then copySpecialMultiPlan()
                Case "applyMultiPlan"
                    If Me.oPermissionPlan > Permission.Read Then applyMultiPlan()
                Case "applyMultiPlanAlt"
                    If Me.oPermissionPlan > Permission.Read Then applyMultiPlan(True)
                Case "exchangeShifts"
                    If Me.oPermissionPlan > Permission.Read Then exchangeShifts2()
                Case "lockDays"
                    If Me.oPermissionPlan > Permission.Read Then lockDays(True)
                Case "unlockDays"
                    If Me.oPermissionPlan > Permission.Read Then lockDays(False)
                Case "lockDaysMulti"
                    If Me.oPermissionPlan > Permission.Read Then lockDaysMulti(True)
                Case "unlockDaysMulti"
                    If Me.oPermissionPlan > Permission.Read Then lockDaysMulti(False)
                Case "DisassignMultiPlanAlt"
                    If Me.oPermissionPlan > Permission.Read Then DisassignMultiPlanAlt()
                Case "getDayPlan"
                    If Me.oPermissionPlan >= Permission.Read Then getDayPlan()
                Case "getDayHeaderPlan"
                    If Me.oPermissionPlan >= Permission.Read Then getDayHeaderPlan()
                Case "getDayHeadersPlan"
                    If Me.oPermissionPlan >= Permission.Read Then getDayHeadersPlan()
                Case "deleteTeoricCoverage"
                    If Me.oPermissionPlan > Permission.Read Then DeleteTeoricCoverage()
                Case "getEmployeeShiftAssignments"
                    If Me.oPermissionPlan >= Permission.Read Then getEmployeeShiftAssignments()
                Case "removeEmployeeCoverage" ' Elimina una cobertura
                    If Me.oPermissionPlan > Permission.Read Then RemoveEmployeeCoverage()
                Case "pasteTeoricCoverage"
                    If Me.oPermissionPlan > Permission.Read Then pasteTeoricCoverage()
                Case "pasteTeoricCoverageMulti"
                Case "pasteTeoricCoverageSpecial"
                Case "removeEmployeeHolidays"
                    If Me.oPermissionPlan > Permission.Read Then RemoveEmployeeHolidays()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
            End Select

        End If
    End Sub

    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Employees\Employees", WLHelperWeb.CurrentPassportID)

            If sID = 0 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Review", WLHelperWeb.CurrentPassportID)
                Dim guiActionsPlanification = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Planification", WLHelperWeb.CurrentPassportID)
                For Each action As roGuiAction In guiActionsPlanification
                    guiActions.Add(action)
                Next
            ElseIf sID = 1 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Planification", WLHelperWeb.CurrentPassportID)
                Dim guiActionsReview = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Calendar\Review", WLHelperWeb.CurrentPassportID)
                For Each action As roGuiAction In guiActionsReview
                    guiActions.Add(action)
                Next
            ElseIf sID = 2 Then
                guiActions = PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftControl\Scheduler\calendar", WLHelperWeb.CurrentPassportID)
            End If

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Scheduler")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Converteix una Data en format String (dd/MM/yyyy) a un camp Date
    ''' </summary>
    ''' <param name="dateString">String amb format (dd/MM/yyyy)</param>
    ''' <returns>Un Camp Date</returns>
    ''' <remarks></remarks>
    Private Function convertDate(ByVal dateString As String) As Date
        Try
            Dim arrDateString() As String = dateString.Split("/")
            Dim dReturn As Date = New Date(arrDateString(2), arrDateString(1), arrDateString(0))
            Return dReturn
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Converteix una Data JAVASCRIPT en format String (MM/dd/yyyy) a un camp Date
    ''' </summary>
    ''' <param name="dateString">String amb format (dd/MM/yyyy)</param>
    ''' <returns>Un Camp Date</returns>
    ''' <remarks></remarks>
    Private Function convertJSDateMDY(ByVal dateString As String) As Date
        Try
            Dim arrDateString() As String = dateString.Split("/")
            Dim dReturn As Date = New Date(arrDateString(2), arrDateString(0) + 1, arrDateString(1))
            Return dReturn
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Converteix una Data JAVASCRIPT en format String (MM/dd/yyyy) a un camp Date
    ''' </summary>
    ''' <returns>Un Camp Date</returns>
    ''' <remarks></remarks>
    Private Function convertToJSDateMDY(ByVal oDate As Date) As String
        Try
            Return (oDate.Month - 1) & "/" & oDate.Day & "/" & oDate.Year
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Private Function FindWeekMonday(ByVal oDate As Date) As Date
        Dim rest As Integer = 0
        If oDate.DayOfWeek = 0 Then
            rest = 6
        Else
            rest = oDate.DayOfWeek - 1
        End If
        Return oDate.AddDays(rest * -1)
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

    Private Sub AssignPlan()

        Dim strResponse As String = ""

        Try
            Dim IDEmployee As Integer = Request("IDEmployee")
            Dim IDShift1 As Integer = Request("IDShift1")
            Dim IDShift2 As Integer = Request("IDShift2")
            Dim IDShift3 As Integer = Request("IDShift3")
            Dim IDShift4 As Integer = Request("IDShift4")

            Dim StartShift1 As DateTime = Nothing
            Dim StartShift2 As DateTime = Nothing
            Dim StartShift3 As DateTime = Nothing
            Dim StartShift4 As DateTime = Nothing
            Dim strStart As String = roTypes.Any2String(Request("StartShift1"))
            If strStart.Length >= 12 Then StartShift1 = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
            strStart = roTypes.Any2String(Request("StartShift2"))
            If strStart.Length >= 12 Then StartShift2 = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
            strStart = roTypes.Any2String(Request("StartShift3"))
            If strStart.Length >= 12 Then StartShift3 = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
            strStart = roTypes.Any2String(Request("StartShift4"))
            If strStart.Length >= 12 Then StartShift4 = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)

            Dim IDAssignment As Integer = roTypes.Any2Integer(Request("IDAssignment"))

            Dim MsgResponse As String = Request("MsgResponse")
            Dim oResponse As LockedDayAction = RetMsgResponse(MsgResponse)

            Dim MsgResponseCoverage As String = Request("MsgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = RetMsgResponse(MsgResponseCoverage)

            Dim MsgResponseShift As String = Request("MsgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                Dim arrDate() As String = Request("DestDate").Split("/")
                Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

                Dim bolRemoveCoverage As Boolean = (oResponseCoverage <> LockedDayAction.None)

                If API.EmployeeServiceMethods.AssignShift(Nothing, IDEmployee, dDate, IDShift1, IDShift2, IDShift3, IDShift4, StartShift1, StartShift2, StartShift3, StartShift4,
                                                                      IDAssignment, oResponse, oResponseCoverage, True, oResponseShift) Then
                    If Not bolRemoveCoverage Then
                        Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmployee, dDate)
                        If dtblPlan IsNot Nothing AndAlso dtblPlan.Rows.Count > 0 Then
                            IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                            IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                            IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                            IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                            StartShift1 = Nothing
                            StartShift2 = Nothing
                            StartShift3 = Nothing
                            StartShift4 = Nothing
                            If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then StartShift1 = dtblPlan.Rows(0)("StartShift1")
                            If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then StartShift2 = dtblPlan.Rows(0)("StartShift2")
                            If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then StartShift3 = dtblPlan.Rows(0)("StartShift3")
                            If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then StartShift4 = dtblPlan.Rows(0)("StartShift4")
                            IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                        End If
                        strResponse = "OK"
                    Else
                        strResponse = "OK"
                    End If
                Else
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                        strResponse = "STOP0"
                    ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                        strResponse = "STOP1"
                    ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                        strResponse = "STOP2"
                    Else
                        strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                    End If
                End If
            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    Private Sub lockDays(ByVal oLock As Boolean)
        Dim strResponse As String = ""
        Try
            Dim IDEmployee As Integer = Request("IDEmployee")
            Dim arrDate() As String = Request("DestDate").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            If API.EmployeeServiceMethods.LockDaysList(Nothing, IDEmployee, dDate, dDate, oLock) Then
                strResponse = "OK"
            Else
                strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    Private Sub lockDaysMulti(ByVal oLock As Boolean)
        Dim strResponse As String = ""
        Try
            Dim bolRet As Boolean = True
            Dim strCellsAssign As String = Request("CellsAssign")
            Dim Cells() As String = strCellsAssign.Split(",")

            For Each Cell As String In Cells
                Dim strEmployee As String = Cell.Split("_")(0)
                Dim strDate As String = Cell.Split("_")(1)

                Dim arrDate() As String = strDate.Split("/")
                Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

                If Not Robotics.Web.Base.API.EmployeeServiceMethods.LockDaysList(Nothing, strEmployee, dDate, dDate, oLock) Then
                    bolRet = False
                End If

                If bolRet = False Then Exit For
            Next

            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Copia el plan (multiple)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub pasteMultiPlan()
        Dim errDetail As String = ""
        Dim strRender As String = ""
        Dim strResponse As String = ""

        Try

            Dim TypeCopy As String = Request("TypeCopy")
            Dim strListShifts() As String = Request("Shifts").Split("|")

            Dim IdEmployeeDest As String = Request("IdEmployeeDest")
            Dim DateStart As Date = convertDate(Request("DateTarget"))
            Dim DateEnd As Date

            Dim totEmpsViewed() As String = Request("TotalEmpsViewed").Split(",")

            Dim strRetDateLocked As String = Request("retDateTarget")
            Dim xDateLocked As Date = Nothing
            If strRetDateLocked <> "" Then xDateLocked = convertDate(strRetDateLocked)

            Dim strRetIDEmployee As String = Request("retIdEmployee")
            Dim xIDEmployee As Integer = Nothing
            If strRetIDEmployee <> "" Then xIDEmployee = CInt(strRetIDEmployee)

            Dim arrCells As New Generic.List(Of srvScheduler.SchedulerCell)
            Dim oColor As String = ""
            Dim sCell As srvScheduler.SchedulerCell
            Dim bolGradient As Boolean = False
            Dim strColorFrom As String = ""
            Dim strColorTo As String = ""

            Dim oListShifts As New Generic.List(Of String)
            Dim oEmployees As New Generic.List(Of Integer)

            Dim strMsgResponse As String = Request("msgResponse")
            Dim oResponse As LockedDayAction = Me.RetMsgResponse(strMsgResponse)

            Dim strMsgResponseCoverage As String = Request("msgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = Me.RetMsgResponse(strMsgResponseCoverage)

            Dim MsgResponseShift As String = Request("msgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = Me.RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                If TypeCopy.ToLower = "vertical" Then
                    Dim totEmpsInt() As Integer = Nothing 'Converteix a integers el arraylist
                    For n As Integer = 0 To totEmpsViewed.Length - 1
                        ReDim Preserve totEmpsInt(n)
                        totEmpsInt(n) = CInt(totEmpsViewed(n).ToString)
                    Next

                    DateEnd = DateStart

                    Dim EmpsDest() As Integer = Nothing 'Usuaris desti
                    Dim lenStartCopy As Integer = 0
                    Dim lenEmpstoCopy As Integer = strListShifts.Length - 1
                    Dim startFlag As Boolean = False

                    For n As Integer = 0 To totEmpsInt.Length - 1
                        If totEmpsInt(n) = CInt(IdEmployeeDest) Then
                            ReDim Preserve EmpsDest(lenStartCopy)
                            EmpsDest(lenStartCopy) = CInt(totEmpsInt(n).ToString)
                            startFlag = True
                            lenStartCopy += 1
                        ElseIf startFlag = True And totEmpsInt(n) <> IdEmployeeDest Then
                            If lenStartCopy <= lenEmpstoCopy Then
                                ReDim Preserve EmpsDest(lenStartCopy)
                                EmpsDest(lenStartCopy) = totEmpsInt(n)
                                lenStartCopy += 1
                            Else
                                Exit For
                            End If
                        ElseIf startFlag = False Then
                        End If
                    Next

                    For Each strList As String In strListShifts
                        oListShifts.Add(strList)
                    Next

                    For Each strEmp As Integer In EmpsDest
                        oEmployees.Add(strEmp)
                    Next

                    If API.EmployeeServiceMethods.CopyListShiftsEmployees(Nothing, oListShifts, oEmployees, DateStart, DateEnd, ActionShiftType.AllShift,
                                                                                      oResponse, oResponseCoverage, oResponseShift, xDateLocked, xIDEmployee, False, True) = False Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                            'LOCK VERTICAL

                            Dim intStopType As Integer = 0
                            Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                                Case EmployeeResultEnum.DailyScheduleLockedDay
                                    intStopType = 0
                                Case EmployeeResultEnum.DailyScheduleCoverageDay
                                    intStopType = 1
                                Case EmployeeResultEnum.ShiftWithoutPermission
                                    intStopType = 2
                            End Select

                            Dim nCount As Integer = 0
                            For nEmp As Integer = 0 To oEmployees.Count - 1
                                Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, oEmployees(nEmp), DateStart)
                                If dtblPlan.Rows.Count > 0 Then
                                    strRender = ""
                                    Dim xStartShift1 As DateTime = Nothing
                                    Dim xStartShift2 As DateTime = Nothing
                                    Dim xStartShift3 As DateTime = Nothing
                                    Dim xStartShift4 As DateTime = Nothing
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                    strRender = ""
                                    sCell = New srvScheduler.SchedulerCell(oEmployees(nEmp), DateStart, strRender)
                                    sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                    sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.StartShift1 = xStartShift1
                                    sCell.StartShift2 = xStartShift2
                                    sCell.StartShift3 = xStartShift3
                                    sCell.StartShift4 = xStartShift4
                                    sCell.ShiftColor1 = oColor
                                    sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                    sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                    sCell.Gradient = bolGradient
                                    sCell.ColorFrom = strColorFrom
                                    sCell.ColorTo = strColorTo
                                    sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                    arrCells.Add(sCell)
                                End If
                                nCount += 1
                            Next

                            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, xIDEmployee, False)

                            Dim oParams As New Generic.List(Of String)
                            If oEmployee IsNot Nothing Then
                                oParams.Add(oEmployee.Name)
                            Else
                                oParams.Add(CInt(xIDEmployee))
                            End If

                            If intStopType = 0 Or intStopType = 1 Then
                                oParams.Add(Format(xDateLocked, HelperWeb.GetShortDateFormat))
                            Else
                                oParams.Add("seleccionado")
                            End If

                            Dim strMsgRes As String = ""
                            Select Case intStopType
                                Case 0
                                    strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                                Case 1
                                    strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                                Case 2
                                    strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                            End Select

                            Dim oMsg As SchedulerCellMessage
                            If intStopType = 0 Or intStopType = 1 Then
                                oMsg = New SchedulerCellMessage(strMsgRes, xIDEmployee, oEmployee.Name, Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, arrCells)
                            Else
                                oMsg = New SchedulerCellMessage(strMsgRes, xIDEmployee, oEmployee.Name, "seleccionado", arrCells)
                            End If

                            strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                            strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
                        Else
                            strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                    Else ' TRUE
                        Dim nCount As Integer = 0
                        For nEmp As Integer = 0 To oEmployees.Count - 1
                            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, oEmployees(nEmp), DateStart)
                            If dtblPlan.Rows.Count > 0 Then
                                strRender = ""
                                Dim xStartShift1 As DateTime = Nothing
                                Dim xStartShift2 As DateTime = Nothing
                                Dim xStartShift3 As DateTime = Nothing
                                Dim xStartShift4 As DateTime = Nothing
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                strRender = ""
                                sCell = New srvScheduler.SchedulerCell(oEmployees(nEmp), DateStart, strRender)
                                sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                sCell.StartShift1 = xStartShift1
                                sCell.StartShift2 = xStartShift2
                                sCell.StartShift3 = xStartShift3
                                sCell.StartShift4 = xStartShift4
                                sCell.ShiftColor1 = oColor
                                sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                sCell.Gradient = bolGradient
                                sCell.ColorFrom = strColorFrom
                                sCell.ColorTo = strColorTo
                                sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                arrCells.Add(sCell)
                            End If
                            nCount += 1
                        Next

                        strResponse = "OK" & roJSONHelper.Serialize(arrCells)
                    End If

                ElseIf TypeCopy.ToLower = "horizontal" Then ' -----------------------------------------------
                    For Each strList As String In strListShifts
                        oListShifts.Add(strList)
                    Next

                    DateEnd = DateStart.AddDays(strListShifts.Length - 1)
                    'xDateLocked = DateStart
                    If API.EmployeeServiceMethods.CopyListShifts(Nothing, oListShifts, IdEmployeeDest, DateStart, DateEnd, ActionShiftType.AllShift,
                                                                             oResponse, oResponseCoverage, oResponseShift, xDateLocked, False, True) = False Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                            'LOCK HORIZONTAL

                            Dim intStopType As Integer = 0
                            Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                                Case EmployeeResultEnum.DailyScheduleLockedDay
                                    intStopType = 0
                                Case EmployeeResultEnum.DailyScheduleCoverageDay
                                    intStopType = 1
                                Case EmployeeResultEnum.ShiftWithoutPermission
                                    intStopType = 2
                            End Select

                            Dim nCount As Integer = 0
                            For Each oShift As String In oListShifts
                                Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IdEmployeeDest, DateStart.AddDays(nCount))
                                If dtblPlan.Rows.Count > 0 Then
                                    strRender = ""
                                    Dim xStartShift1 As DateTime = Nothing
                                    Dim xStartShift2 As DateTime = Nothing
                                    Dim xStartShift3 As DateTime = Nothing
                                    Dim xStartShift4 As DateTime = Nothing
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                    strRender = ""
                                    sCell = New srvScheduler.SchedulerCell(IdEmployeeDest, DateStart.AddDays(nCount), strRender)
                                    sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                    sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                    sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                    sCell.StartShift1 = xStartShift1
                                    sCell.StartShift2 = xStartShift2
                                    sCell.StartShift3 = xStartShift3
                                    sCell.StartShift4 = xStartShift4
                                    sCell.ShiftColor1 = oColor
                                    sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                    sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                    sCell.Gradient = bolGradient
                                    sCell.ColorFrom = strColorFrom
                                    sCell.ColorTo = strColorTo
                                    sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                    arrCells.Add(sCell)
                                End If
                                nCount += 1
                            Next

                            Dim oParams As New Generic.List(Of String)
                            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IdEmployeeDest, False)

                            If intStopType = 0 Or intStopType = 1 Then
                                If oEmployee IsNot Nothing Then
                                    oParams.Add(oEmployee.Name)
                                Else
                                    oParams.Add(CInt(xIDEmployee))
                                End If
                            Else
                                oParams.Add("origen")
                            End If

                            oParams.Add(Format(xDateLocked, HelperWeb.GetShortDateFormat))
                            Dim strMsgRes As String = ""
                            Select Case intStopType
                                Case 0
                                    strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                                Case 1
                                    strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                                Case 2
                                    strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                            End Select

                            Dim oMsg As SchedulerCellMessage

                            If intStopType = 0 Or intStopType = 1 Then
                                oMsg = New SchedulerCellMessage(strMsgRes, IdEmployeeDest, oEmployee.Name, Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, arrCells)
                            Else
                                oMsg = New SchedulerCellMessage(strMsgRes, IdEmployeeDest, "origen", Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, arrCells)
                            End If

                            strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                            strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
                        Else
                            strResponse = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                    Else ' TRUE
                        Dim nCount As Integer = 0
                        For Each oShift As String In oListShifts
                            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IdEmployeeDest, DateStart.AddDays(nCount))
                            If dtblPlan.Rows.Count > 0 Then
                                strRender = ""
                                Dim xStartShift1 As DateTime = Nothing
                                Dim xStartShift2 As DateTime = Nothing
                                Dim xStartShift3 As DateTime = Nothing
                                Dim xStartShift4 As DateTime = Nothing
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                strRender = ""
                                sCell = New srvScheduler.SchedulerCell(IdEmployeeDest, DateStart.AddDays(nCount), strRender)
                                sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                sCell.StartShift1 = xStartShift1
                                sCell.StartShift2 = xStartShift2
                                sCell.StartShift3 = xStartShift3
                                sCell.StartShift4 = xStartShift4
                                sCell.ShiftColor1 = oColor
                                sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                sCell.Gradient = bolGradient
                                sCell.ColorFrom = strColorFrom
                                sCell.ColorTo = strColorTo
                                sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                arrCells.Add(sCell)
                            End If
                            nCount += 1
                        Next

                        strResponse = "OK" & roJSONHelper.Serialize(arrCells)
                    End If

                End If

            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub


    ''' <summary>
    ''' Intercanviar horarios entre 2 planes
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub exchangeShifts()
        Try
            Dim bolRet As Boolean = True
            Dim strErrMsg As String = ""

            Dim IDEmp1 As String = Request("IDEmployee1")
            Dim IDEmp2 As String = Request("IDEmployee2")

            Dim strDate1() As String = Request("DatePlan1").Split("/")
            Dim strDate2() As String = Request("DatePlan2").Split("/")
            Dim dateSel1 As Date = New Date(strDate1(2), strDate1(1), strDate1(0))
            Dim dateSel2 As Date = New Date(strDate2(2), strDate2(1), strDate2(0))

            Dim dtblPlan1 As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp1, dateSel1)
            Dim dtblPlan2 As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp2, dateSel2)

            If dtblPlan1.Rows.Count > 0 And dtblPlan2.Rows.Count > 0 Then

                Dim intIDShift1 As Integer = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift1"))
                Dim intIDShift2 As Integer = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift2"))
                Dim intIDShift3 As Integer = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift3"))
                Dim intIDShift4 As Integer = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift4"))
                If intIDShift1 = 0 Then intIDShift1 = -1
                If intIDShift2 = 0 Then intIDShift2 = -1
                If intIDShift3 = 0 Then intIDShift3 = -1
                If intIDShift4 = 0 Then intIDShift4 = -1
                Dim xStartShift1 As DateTime = Nothing
                Dim xStartShift2 As DateTime = Nothing
                Dim xStartShift3 As DateTime = Nothing
                Dim xStartShift4 As DateTime = Nothing
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan1.Rows(0)("StartShift1")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan1.Rows(0)("StartShift2")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan1.Rows(0)("StartShift3")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan1.Rows(0)("StartShift4")
                Dim intIDAssignment As Integer = 0
                If Not IsDBNull(dtblPlan1.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan1.Rows(0)("IDAssignment")

                If Not API.EmployeeServiceMethods.AssignShift(Nothing, IDEmp2, dateSel2, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                                          LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                        strErrMsg = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                    Else
                        strErrMsg = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                    End If
                    bolRet = False
                End If

                If bolRet Then
                    intIDShift1 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift1"))
                    intIDShift2 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift2"))
                    intIDShift3 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift3"))
                    intIDShift4 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift4"))
                    If intIDShift1 = 0 Then intIDShift1 = -1
                    If intIDShift2 = 0 Then intIDShift2 = -1
                    If intIDShift3 = 0 Then intIDShift3 = -1
                    If intIDShift4 = 0 Then intIDShift4 = -1
                    xStartShift1 = Nothing
                    xStartShift2 = Nothing
                    xStartShift3 = Nothing
                    xStartShift4 = Nothing
                    If Not IsDBNull(dtblPlan2.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan2.Rows(0)("StartShift1")
                    If Not IsDBNull(dtblPlan2.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan2.Rows(0)("StartShift2")
                    If Not IsDBNull(dtblPlan2.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan2.Rows(0)("StartShift3")
                    If Not IsDBNull(dtblPlan2.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan2.Rows(0)("StartShift4")
                    intIDAssignment = 0
                    If Not IsDBNull(dtblPlan2.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan2.Rows(0)("IDAssignment")

                    If Not API.EmployeeServiceMethods.AssignShift(Nothing, IDEmp1, dateSel1, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                                              LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                            strErrMsg = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                        Else
                            strErrMsg = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                        bolRet = False
                    End If
                End If
            Else
                bolRet = False
                strErrMsg = Me.Language.Translate("TwoSchedShiftsNeeded", DefaultScope)
            End If

            If bolRet Then
                Response.Write("OK")
            Else
                Response.Write(strErrMsg)
            End If

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Intercanviar horarios entre 2 planes
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub exchangeShifts2()
        Dim strRet As String = ""
        Try

            Dim bolRet As Boolean = True
            Dim strErrMsg As String = ""

            Dim IDEmp1 As String = Request("IDEmployee1")
            Dim IDEmp2 As String = Request("IDEmployee2")

            Dim strDate1() As String = Request("DatePlan1").Split("/")
            Dim strDate2() As String = Request("DatePlan2").Split("/")

            Dim dateSel1 As Date = New Date(strDate1(2), strDate1(1), strDate1(0))
            Dim dateSel2 As Date = New Date(strDate2(2), strDate2(1), strDate2(0))

            Dim dtblPlan1 As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp1, dateSel1)
            Dim dtblPlan2 As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp2, dateSel2)

            Dim xStartShift1 As DateTime = Nothing
            Dim xStartShift2 As DateTime = Nothing
            Dim xStartShift3 As DateTime = Nothing
            Dim xStartShift4 As DateTime = Nothing

            If dtblPlan1.Rows.Count > 0 And dtblPlan2.Rows.Count > 0 Then

                Dim intIDShift1 As Integer
                Dim intIDShift2 As Integer
                Dim intIDShift3 As Integer
                Dim intIDShift4 As Integer

                'Permisos sobre horarios. Primero la segunda planificacion
                intIDShift1 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift1"))
                intIDShift2 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift2"))
                intIDShift3 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift3"))
                intIDShift4 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift4"))
                If Not API.ShiftServiceMethods.ShiftsAreAllowed(Nothing, intIDShift1, intIDShift2, intIDShift3, intIDShift4) Then
                    strErrMsg = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                    bolRet = False
                End If

                If bolRet Then

                    intIDShift1 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift1"))
                    intIDShift2 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift2"))
                    intIDShift3 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift3"))
                    intIDShift4 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift4"))
                    If intIDShift1 = 0 Then intIDShift1 = -1
                    If intIDShift2 = 0 Then intIDShift2 = -1
                    If intIDShift3 = 0 Then intIDShift3 = -1
                    If intIDShift4 = 0 Then intIDShift4 = -1
                    xStartShift1 = Nothing
                    xStartShift2 = Nothing
                    xStartShift3 = Nothing
                    xStartShift4 = Nothing
                    If Not IsDBNull(dtblPlan1.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan1.Rows(0)("StartShift1")
                    If Not IsDBNull(dtblPlan1.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan1.Rows(0)("StartShift2")
                    If Not IsDBNull(dtblPlan1.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan1.Rows(0)("StartShift3")
                    If Not IsDBNull(dtblPlan1.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan1.Rows(0)("StartShift4")
                    Dim intIDAssignment As Integer = 0
                    If Not IsDBNull(dtblPlan1.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan1.Rows(0)("IDAssignment")

                    If Not IsDBNull(dtblPlan1.Rows(0)("IsHolidays")) Then
                        If roTypes.Any2Boolean(dtblPlan1.Rows(0)("IsHolidays")) Then
                            If Not IsDBNull(dtblPlan1.Rows(0)("IDShiftBase")) Then intIDShift1 = dtblPlan1.Rows(0)("IDShiftBase")
                            If Not IsDBNull(dtblPlan1.Rows(0)("StartShiftBase")) Then xStartShift1 = dtblPlan1.Rows(0)("StartShiftBase")
                            If Not IsDBNull(dtblPlan1.Rows(0)("IDAssignmentBase")) Then intIDAssignment = dtblPlan1.Rows(0)("IDAssignmentBase")
                        End If
                    End If



                    If Not API.EmployeeServiceMethods.AssignShift(Nothing, IDEmp2, dateSel2, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                                              LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                            strErrMsg = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                        Else
                            strErrMsg = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                        bolRet = False
                    End If

                    If bolRet Then
                        intIDShift1 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift1"))
                        intIDShift2 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift2"))
                        intIDShift3 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift3"))
                        intIDShift4 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift4"))
                        If intIDShift1 = 0 Then intIDShift1 = -1
                        If intIDShift2 = 0 Then intIDShift2 = -1
                        If intIDShift3 = 0 Then intIDShift3 = -1
                        If intIDShift4 = 0 Then intIDShift4 = -1
                        xStartShift1 = Nothing
                        xStartShift2 = Nothing
                        xStartShift3 = Nothing
                        xStartShift4 = Nothing
                        If Not IsDBNull(dtblPlan2.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan2.Rows(0)("StartShift1")
                        If Not IsDBNull(dtblPlan2.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan2.Rows(0)("StartShift2")
                        If Not IsDBNull(dtblPlan2.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan2.Rows(0)("StartShift3")
                        If Not IsDBNull(dtblPlan2.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan2.Rows(0)("StartShift4")
                        intIDAssignment = 0
                        If Not IsDBNull(dtblPlan2.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan2.Rows(0)("IDAssignment")

                        If Not IsDBNull(dtblPlan2.Rows(0)("IsHolidays")) Then
                            If roTypes.Any2Boolean(dtblPlan2.Rows(0)("IsHolidays")) Then
                                If Not IsDBNull(dtblPlan2.Rows(0)("IDShiftBase")) Then intIDShift1 = dtblPlan2.Rows(0)("IDShiftBase")
                                If Not IsDBNull(dtblPlan2.Rows(0)("StartShiftBase")) Then xStartShift1 = dtblPlan2.Rows(0)("StartShiftBase")
                                If Not IsDBNull(dtblPlan2.Rows(0)("IDAssignmentBase")) Then intIDAssignment = dtblPlan2.Rows(0)("IDAssignmentBase")
                            End If
                        End If

                        If Not API.EmployeeServiceMethods.AssignShift(Nothing, IDEmp1, dateSel1, intIDShift1, intIDShift2, intIDShift3, intIDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4, intIDAssignment,
                                                                                  LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                            If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                                strErrMsg = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                            Else
                                strErrMsg = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                            End If
                            bolRet = False
                        End If
                    End If
                End If
            Else
                bolRet = False
                strErrMsg = Me.Language.Translate("TwoSchedShiftsNeeded", DefaultScope)
            End If

            If bolRet Then

                dtblPlan1 = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp1, dateSel1)
                dtblPlan2 = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IDEmp2, dateSel2)

                Dim arrCells As New Generic.List(Of srvScheduler.SchedulerCell)
                Dim oColor1 As String = String.Empty
                Dim oColor2 As String = String.Empty
                Dim bolGradient As Boolean = False
                Dim strColorFrom As String = ""
                Dim strColorTo As String = ""

                xStartShift1 = Nothing
                xStartShift2 = Nothing
                xStartShift3 = Nothing
                xStartShift4 = Nothing
                If Not IsDBNull(dtblPlan2.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan2.Rows(0)("StartShift1")
                If Not IsDBNull(dtblPlan2.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan2.Rows(0)("StartShift2")
                If Not IsDBNull(dtblPlan2.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan2.Rows(0)("StartShift3")
                If Not IsDBNull(dtblPlan2.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan2.Rows(0)("StartShift4")
                Dim sCell1 As srvScheduler.SchedulerCell = Nothing

                sCell1.IDShift1 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift1"))
                sCell1.IDShift2 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift2"))
                sCell1.IDShift3 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift3"))
                sCell1.IDShift4 = roTypes.Any2Integer(dtblPlan2.Rows(0)("IdShift4"))
                sCell1.StartShift1 = xStartShift1
                sCell1.StartShift2 = xStartShift2
                sCell1.StartShift3 = xStartShift3
                sCell1.StartShift4 = xStartShift4
                sCell1.ShiftColor1 = oColor1
                sCell1.IDAssignment = roTypes.Any2Integer(dtblPlan2.Rows(0)("IDAssignment"))
                sCell1.Gradient = bolGradient
                sCell1.ColorFrom = strColorFrom
                sCell1.ColorTo = strColorTo
                sCell1.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan2.Rows(0)("CoverageIDEmployee"))

                xStartShift1 = Nothing
                xStartShift2 = Nothing
                xStartShift3 = Nothing
                xStartShift4 = Nothing
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan1.Rows(0)("StartShift1")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan1.Rows(0)("StartShift2")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan1.Rows(0)("StartShift3")
                If Not IsDBNull(dtblPlan1.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan1.Rows(0)("StartShift4")
                Dim sCell2 As New srvScheduler.SchedulerCell(IDEmp1, dateSel1, "")
                sCell2.IDShift1 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift1"))
                sCell2.IDShift2 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift2"))
                sCell2.IDShift3 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift3"))
                sCell2.IDShift4 = roTypes.Any2Integer(dtblPlan1.Rows(0)("IdShift4"))
                sCell2.StartShift1 = xStartShift1
                sCell2.StartShift2 = xStartShift2
                sCell2.StartShift3 = xStartShift3
                sCell2.StartShift4 = xStartShift4
                sCell2.ShiftColor1 = oColor2
                sCell2.IDAssignment = roTypes.Any2Integer(dtblPlan1.Rows(0)("IDAssignment"))
                sCell2.Gradient = bolGradient
                sCell2.ColorFrom = strColorFrom
                sCell2.ColorTo = strColorTo
                sCell2.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan1.Rows(0)("CoverageIDEmployee"))

                arrCells.Add(sCell1)
                arrCells.Add(sCell2)

                strRet = "OK" & roJSONHelper.Serialize(arrCells)
            Else
                strRet = strErrMsg
            End If

        Catch ex As Exception
            strRet = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strRet)
    End Sub

    ''' <summary>
    ''' Asignación multiple de calendarios por dia / empleado
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub applyMultiPlan(Optional ByVal bolAlterShift As Boolean = False)
        Dim strResponse As String = ""
        Try
            Dim bolRet As Boolean = True
            Dim strCellsAssign As String = Request("CellsAssign")
            Dim strShift As String = Request("ApplyShift")
            Dim xStartShift As DateTime = Nothing
            Dim strStart As String = roTypes.Any2String(Request("ApplyStartShift"))
            If strStart.Length >= 12 Then xStartShift = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
            Dim intIDAssignment As Integer = roTypes.Any2Integer(Request("ApplyAssignment"))

            Dim strEmployee As String = String.Empty
            Dim strDate As String

            Dim arrDate() As String
            Dim dDate As Date

            Dim bolFind As Boolean = False

            Dim arrCells As New Generic.List(Of srvScheduler.SchedulerCell)
            Dim strRender As String = ""
            Dim sCell As srvScheduler.SchedulerCell
            Dim oColor As String = ""
            Dim bolGradient As Boolean = False
            Dim strColorFrom As String = ""
            Dim strColorTo As String = ""

            Dim xStartShift1 As DateTime = Nothing
            Dim xStartShift2 As DateTime = Nothing
            Dim xStartShift3 As DateTime = Nothing
            Dim xStartShift4 As DateTime = Nothing

            Dim bolAssign As Boolean = False

            Dim Cells() As String = strCellsAssign.Split(",")

            Dim strRetIDEmployee As String = Request("retIDEmployee")
            Dim strRetDate As String = Request("retDateDest")

            Dim strMsgResponse As String = Request("msgResponse")
            Dim oResponse As LockedDayAction = Me.RetMsgResponse(strMsgResponse)

            Dim strMsgResponseCoverage As String = Request("msgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = Me.RetMsgResponse(strMsgResponseCoverage)

            Dim MsgResponseShift As String = Request("msgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = Me.RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                For Each Cell As String In Cells
                    'TODO: Comprobar els horaris alternatius abans de assignar el calendari
                    strEmployee = Cell.Split("_")(0)
                    strDate = Cell.Split("_")(1)

                    arrDate = strDate.Split("/")
                    dDate = New Date(arrDate(2), arrDate(1), arrDate(0))

                    If strRetIDEmployee <> "" And strRetDate <> "" And bolFind = False Then
                        If strEmployee = strRetIDEmployee Then
                            If strRetDate = strDate Then
                                bolFind = True
                            Else
                                Continue For
                            End If
                        Else
                            Continue For
                        End If
                        If bolFind = True Then
                            If Not bolAlterShift Then
                                bolAssign = API.EmployeeServiceMethods.AssignShift(Nothing, strEmployee, dDate, strShift, 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, intIDAssignment,
                                                                                               oResponse, oResponseCoverage, True, oResponseShift)
                            Else
                                bolAssign = API.EmployeeServiceMethods.AssignAlterShift(Nothing, strEmployee, dDate, strShift, xStartShift, oResponse)
                                ' Si devuelve false, verificamos si és porqué ya están todos los horarios alternativos informados o el horario ya esté asignado, y desmarcamos el error para que continue con el proceso.
                                If Not bolAssign And
                                   (roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftFullAlterShift Or
                                    roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftAlreadyAssigned) Then
                                    bolAssign = True
                                End If
                            End If
                            If Not bolAssign Then
                                bolRet = False
                            Else
                                Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                                If dtblPlan.Rows.Count > 0 Then
                                    strRender = ""
                                    xStartShift1 = Nothing
                                    xStartShift2 = Nothing
                                    xStartShift3 = Nothing
                                    xStartShift4 = Nothing
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                    strRender = ""
                                    sCell = New srvScheduler.SchedulerCell(strEmployee, strDate, strRender)
                                    sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                    sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                    sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                    sCell.StartShift1 = xStartShift1
                                    sCell.StartShift2 = xStartShift2
                                    sCell.StartShift3 = xStartShift3
                                    sCell.StartShift4 = xStartShift4
                                    sCell.ShiftColor1 = oColor
                                    sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                    sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                    sCell.Gradient = bolGradient
                                    sCell.ColorFrom = strColorFrom
                                    sCell.ColorTo = strColorTo
                                    sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                    arrCells.Add(sCell)
                                End If

                                If oResponse = LockedDayAction.ReplaceAll Or oResponse = LockedDayAction.NoReplaceAll Then
                                Else
                                    oResponse = LockedDayAction.None
                                End If
                            End If
                        End If
                    Else
                        If Not bolAlterShift Then
                            bolAssign = API.EmployeeServiceMethods.AssignShift(Nothing, strEmployee, dDate, strShift, 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, intIDAssignment, oResponse,
                                                                                           oResponseCoverage, True, oResponseShift)

                            ' Si devuelve false, verificamos que no sea causa de ser un horario de vacaciones aplicandolo donde no se debe
                            If Not bolAssign And
                                (roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoBaseShiftAssigned Or
                                 roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoWorkingDay) Then
                                bolAssign = True
                            End If
                        Else
                            bolAssign = API.EmployeeServiceMethods.AssignAlterShift(Nothing, strEmployee, dDate, strShift, xStartShift, oResponse)
                            ' Si devuelve false, verificamos si és porqué ya están todos los horarios alternativos informados o el horario ya esté asignado, y desmarcamos el error para que continue con el proceso.
                            If Not bolAssign And
                               (roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftFullAlterShift Or
                                roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftAlreadyAssigned Or
                                roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoBaseShiftAssigned Or
                                roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoWorkingDay) Then
                                bolAssign = True
                            End If
                        End If
                        If Not bolAssign Then
                            bolRet = False
                        Else

                            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                            If dtblPlan.Rows.Count > 0 Then
                                strRender = ""
                                xStartShift1 = Nothing
                                xStartShift2 = Nothing
                                xStartShift3 = Nothing
                                xStartShift4 = Nothing
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                strRender = ""
                                sCell = New srvScheduler.SchedulerCell(strEmployee, strDate, strRender)
                                sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                sCell.StartShift1 = xStartShift1
                                sCell.StartShift2 = xStartShift2
                                sCell.StartShift3 = xStartShift3
                                sCell.StartShift4 = xStartShift4
                                sCell.ShiftColor1 = oColor
                                sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                sCell.Gradient = bolGradient
                                sCell.ColorFrom = strColorFrom
                                sCell.ColorTo = strColorTo
                                sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                arrCells.Add(sCell)
                            End If

                            If oResponse = LockedDayAction.ReplaceAll Or oResponse = LockedDayAction.NoReplaceAll Then
                            Else
                                oResponse = LockedDayAction.None
                            End If
                        End If
                    End If

                    If bolRet = False Then Exit For
                Next

                If bolRet Then
                    strResponse = "OK" & roJSONHelper.Serialize(arrCells)
                Else
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                        Dim intStopType As Integer = 0
                        Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                            Case EmployeeResultEnum.DailyScheduleLockedDay
                                intStopType = 0
                            Case EmployeeResultEnum.DailyScheduleCoverageDay
                                intStopType = 1
                            Case EmployeeResultEnum.ShiftWithoutPermission
                                intStopType = 2
                        End Select

                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, CInt(strEmployee), False)

                        Dim oParams As New Generic.List(Of String)
                        If oEmployee IsNot Nothing Then
                            oParams.Add(oEmployee.Name)
                        Else
                            oParams.Add(CInt(strEmployee))
                        End If
                        oParams.Add(Format(dDate, HelperWeb.GetShortDateFormat))
                        Dim strMsgRes As String = ""
                        Select Case intStopType
                            Case 0
                                strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                            Case 1
                                strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                            Case 2
                                strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                        End Select

                        Dim oMsg As New SchedulerCellMessage(strMsgRes, strEmployee, oEmployee.Name, Format(dDate, HelperWeb.GetShortDateFormat).ToString, arrCells)
                        strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)
                    Else
                        strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                    End If

                End If

            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub


    ''' <summary>
    ''' Copia el Plan (sols una celda)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub copyPlan()
        Try

            Dim IdEmployeeOri As String = Request("IdEmployeeOri")
            Dim IdEmployeeDest As String = Request("IdEmployeeDest")

            Dim strDatePlanOrig() As String = Request("DatePlanOrig").Split("/")
            Dim DatePlanOrig As Date = New Date(strDatePlanOrig(2), strDatePlanOrig(1), strDatePlanOrig(0))

            Dim strDatePlanDest() As String = Request("DatePlanDest").Split("/")
            Dim DatePlanDest As Date = New Date(strDatePlanDest(2), strDatePlanDest(1), strDatePlanDest(0))

            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IdEmployeeOri, DatePlanOrig)
            If dtblPlan.Rows.Count > 0 Then
                Dim xStartShift1 As DateTime = Nothing
                Dim xStartShift2 As DateTime = Nothing
                Dim xStartShift3 As DateTime = Nothing
                Dim xStartShift4 As DateTime = Nothing
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                Dim intIDAssignment As Integer = 0
                If Not IsDBNull(dtblPlan.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan.Rows(0)("IDAssignment")
                If Not API.EmployeeServiceMethods.AssignShift(Nothing, IdEmployeeDest, DatePlanDest, roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1")), roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2")),
                                                                          roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3")), roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4")), xStartShift1, xStartShift2,
                                                                          xStartShift3, xStartShift4, intIDAssignment,
                                                                          LockedDayAction.None, True, LockedDayAction.None, ShiftPermissionAction.None) Then
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                        Response.Write(Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope))
                    Else
                        Response.Write(Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText)
                    End If
                Else
                    Response.Write("OK")
                End If
            End If
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Copia el Plan (sols una celda)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub copyPlan2()
        Try

            Dim IdEmployeeOri As String = Request("IdEmployeeOri")
            Dim IdEmployeeDest As String = Request("IdEmployeeDest")

            Dim strDatePlanOrig() As String = Request("DatePlanOrig").Split("/")
            Dim DatePlanOrig As Date = New Date(strDatePlanOrig(2), strDatePlanOrig(1), strDatePlanOrig(0))

            Dim strDatePlanDest() As String = Request("DatePlanDest").Split("/")
            Dim DatePlanDest As Date = New Date(strDatePlanDest(2), strDatePlanDest(1), strDatePlanDest(0))

            Dim oListShifts As New Generic.List(Of String)
            oListShifts.Add("***")

            Dim dateLocked As Date = Nothing
            If API.EmployeeServiceMethods.CopyListShifts(Nothing, oListShifts, IdEmployeeDest, DatePlanDest, DatePlanDest.AddDays(oListShifts.Count), ActionShiftType.PrimaryShift,
                                                                     LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, dateLocked, False, True) Then

            End If

            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, IdEmployeeOri, DatePlanOrig)
            If dtblPlan.Rows.Count > 0 Then
                Dim xStartShift1 As DateTime = Nothing
                Dim xStartShift2 As DateTime = Nothing
                Dim xStartShift3 As DateTime = Nothing
                Dim xStartShift4 As DateTime = Nothing
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                Dim intIDAssignment As Integer = 0
                If Not IsDBNull(dtblPlan.Rows(0)("IDAssignment")) Then intIDAssignment = dtblPlan.Rows(0)("IDAssignment")
                If Not API.EmployeeServiceMethods.AssignShift(Nothing, IdEmployeeDest, DatePlanDest, roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1")), roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2")),
                                                                          roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3")), roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4")), xStartShift1, xStartShift2,
                                                                          xStartShift3, xStartShift4, intIDAssignment, LockedDayAction.None, LockedDayAction.None,
                                                                          True, ShiftPermissionAction.None) Then
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                        Response.Write(Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope))
                    Else
                        Response.Write(Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText)
                    End If
                Else
                    Response.Write("OK")
                End If
            End If
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub


    ''' <summary>
    ''' Escriu el calendari a la data/usuari corresponent
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub savePlan()
        Try
            'TODO: Comprobar els horaris alternatius abans de assignar el calendari
            Dim arrDate() As String = Request("DatePlan").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            Dim xStartShift As DateTime = Nothing
            Dim strStart As String = roTypes.Any2String(Request("StartShift"))
            If strStart.Length >= 12 Then xStartShift = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)

            If Not API.EmployeeServiceMethods.AssignShift(Nothing, Request("IDEmployee"), dDate, Request("Shift"), 0, 0, 0, xStartShift, Nothing, Nothing, Nothing,
                                                                      roTypes.Any2Integer(Request("Assignment")),
                                                                      LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                    Response.Write(Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope))
                Else
                    Response.Write(Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText)
                End If
            Else
                Response.Write("OK")
            End If

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Escriu el calendari a la data/usuari corresponent
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub savePlan2()
        Try
            'TODO: Comprobar els horaris alternatius abans de assignar el calendari
            Dim arrDate() As String = Request("DatePlan").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            Dim xStartShift As DateTime = Nothing
            Dim strStart As String = roTypes.Any2String(Request("StartShift"))
            If strStart.Length >= 12 Then xStartShift = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)

            Dim strResponse As String = ""
            If Not API.EmployeeServiceMethods.AssignShift(Nothing, Request("IDEmployee"), dDate, Request("Shift"), 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, roTypes.Any2Integer(Request("Assignment")),
                                                                     LockedDayAction.None, LockedDayAction.None, True, ShiftPermissionAction.None) Then
                If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                    strResponse = Me.Language.Translate("ShiftWithoutPermission.Description", DefaultScope)
                Else
                    strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                End If
            Else
                strResponse = "OK"
            End If

            Response.Write(strResponse)

        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    ''' <summary>
    ''' Retorna la Descripció del Periode seleccionat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub getPeriodDescription()
        Try
            CalculateDaysPeriods()
            Dim dateCaption As String = Me.Language.Translate("PeriodFrom", Me.DefaultScope) & Format(mDateStart, HelperWeb.GetShortDateFormat)
            dateCaption &= Me.Language.Translate("PeriodTo", Me.DefaultScope) & Format(mDateEnd, HelperWeb.GetShortDateFormat)
            Response.Write(dateCaption)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Sub getIDGroupByEmployee()
        Try
            Dim roMob As roMobility = Robotics.Web.Base.API.EmployeeServiceMethods.GetCurrentMobility(Nothing, Request("IDEmployee"))
            Response.Clear()
            Response.ClearContent()
            Response.Write(roMob.IdGroup.ToString)
        Catch ex As Exception
            Response.Write("ERROR: " & ex.Message & " " & ex.StackTrace)
        End Try
    End Sub



    Private Function createDayCell(ByRef oSchedulerEmpDay As roDailySchedule, ByVal dAct As Date, ByVal arrDetail() As String, ByVal strRemark As String,
                                   ByVal cView As Integer, ByVal cssRemark As String, ByVal cssG1 As String, ByVal cssG2 As String) As String
        Dim strOutPut As String = ""
        Dim strTypeView As String = ""

        Try
            strTypeView = "RowTypeView" & cView.ToString

            Dim detPunches(-1) As String

            If cView <> 2 Then
                For Each oStr As String In arrDetail
                    Dim oStrSplit() As String = oStr.Split(",")
                    If Trim(oStrSplit(0)) <> "" Then
                        ReDim Preserve detPunches(detPunches.Length)
                        detPunches(detPunches.Length - 1) = Trim(oStrSplit(0))
                    End If
                    If oStrSplit.Length > 1 Then
                        If Trim(oStrSplit(1)) <> "" Then
                            ReDim Preserve detPunches(detPunches.Length)
                            detPunches(detPunches.Length - 1) = Trim(oStrSplit(1))
                        End If
                    End If
                Next
            Else
                detPunches = arrDetail.Clone()
            End If

            strOutPut &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">"
            'Si es tindra de pintar el div amagat o no
            If detPunches.Length = 2 Then
                strOutPut &= "<tr>"
                strOutPut &= "<td class=""RowCell " & strTypeView & """>"
                strOutPut &= "<a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_1"" href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & " " & cssRemark & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & vbCrLf
                strOutPut &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">" & vbCrLf
                strOutPut &= "  <tr>" & vbCrLf
                strOutPut &= "      <td>" & vbCrLf
                If strRemark <> "" Then
                    strOutPut &= "          <div class=""RemarkCSS_" & strTypeView & """>" & strRemark & "</div>" & vbCrLf
                Else
                    strOutPut &= "          <div class=""RemarkCSS_" & strTypeView & """>&nbsp;</div>" & vbCrLf
                End If
                strOutPut &= "      </td>" & vbCrLf
                strOutPut &= "      <td>" & vbCrLf
                strOutPut &= "          <div style=""font-size:9px;"" class=""RemarkCSSText_" & strTypeView & """>" & detPunches(0) & "</div>" & vbCrLf
                strOutPut &= "      </td>" & vbCrLf
                strOutPut &= "  </tr>" & vbCrLf
                strOutPut &= "</table>" & vbCrLf


                strOutPut &= "</a>"
                strOutPut &= "</tr>" & vbCrLf
                strOutPut &= "<tr>" & vbCrLf
                strOutPut &= "  <td>" & vbCrLf
                strOutPut &= "      <a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_2"" href=""javascript: void(0);"" style=""font-size:9px;"" class=""RowCell2 " & cssG2 & " " & strTypeView & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & "&nbsp;&nbsp;" & detPunches(1) & "</a>"
                strOutPut &= "  </td>" & vbCrLf
                strOutPut &= "  <td></td>" & vbCrLf
                strOutPut &= "</tr>" & vbCrLf

            ElseIf detPunches.Length = 1 Then
                strOutPut &= "<tr>"
                strOutPut &= "<td class=""RowCell " & strTypeView & """>"
                strOutPut &= "<a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_1"" href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & " " & cssRemark & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & vbCrLf
                strOutPut &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">" & vbCrLf
                strOutPut &= "  <tr>" & vbCrLf
                strOutPut &= "      <td>" & vbCrLf
                If strRemark <> "" Then
                    strOutPut &= "          <div class=""RemarkCSS_" & strTypeView & """>" & strRemark & "</div>" & vbCrLf
                Else
                    strOutPut &= "          <div class=""RemarkCSS_" & strTypeView & """>&nbsp;</div>" & vbCrLf
                End If
                strOutPut &= "      </td>" & vbCrLf
                strOutPut &= "      <td>" & vbCrLf
                strOutPut &= "          <div class=""RemarkCSSText_" & strTypeView & """>" & detPunches(0) & "</div>" & vbCrLf
                strOutPut &= "      </td>" & vbCrLf
                strOutPut &= "  </tr>" & vbCrLf
                strOutPut &= "</table>" & vbCrLf


                strOutPut &= "</a>"
                strOutPut &= "</tr>" & vbCrLf
                strOutPut &= "<tr>" & vbCrLf
                strOutPut &= "  <td>" & vbCrLf
                strOutPut &= "      <a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_2"" href=""javascript: void(0);"" class=""RowCell2 " & cssG2 & " " & strTypeView & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">&nbsp;</a>"
                strOutPut &= "  </td>" & vbCrLf
                strOutPut &= "  <td></td>" & vbCrLf
                strOutPut &= "</tr>" & vbCrLf
            ElseIf detPunches.Length > 2 Then
                'Si son mes de 2 linies
                strOutPut &= "<tr>"
                strOutPut &= "<td class=""RowCell " & strTypeView & """>"

                'Div especific
                Dim nameDivHd As String = "hiddenRowsCell" & cView & "_" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy")
                Dim strDiv As String = "<div id=""" & nameDivHd & """ style=""display: none;position: relative; margin-top: -42px;border: solid 2px #76859F; margin-left: -2px;"" onmouseout=""moHideDiv('" & nameDivHd & "');"">"

                For nDet As Integer = 0 To detPunches.Length - 1
                    If nDet > 1 Then
                        strDiv &= "<a id=""div_" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_" & nDet + 1 & """ href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & """>" & detPunches(nDet) & "</a>"
                    Else
                        If nDet = 0 Then
                            If strRemark <> "" Then
                                strOutPut &= "<a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_1"" href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & " " & cssRemark & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & vbCrLf
                                strOutPut &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">" & vbCrLf
                                strOutPut &= "  <tr>" & vbCrLf
                                strOutPut &= "      <td>" & vbCrLf
                                strOutPut &= "          <div class=""RemarkCSS_" & strTypeView & """>" & strRemark & "</div>" & vbCrLf
                                strOutPut &= "      </td>" & vbCrLf
                                strOutPut &= "      <td>" & vbCrLf
                                strOutPut &= "          <div class=""RemarkCSSText_" & strTypeView & """>" & detPunches(0) & "</div>" & vbCrLf
                                strOutPut &= "      </td>" & vbCrLf
                                strOutPut &= "  </tr>" & vbCrLf
                                strOutPut &= "</table>" & vbCrLf
                                strOutPut &= "</a>"
                            Else
                                strOutPut &= "<a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_1"" href=""javascript: void(0);"" style=""font-size:9px;"" class=""RowCell " & cssG1 & " " & strTypeView & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & detPunches(0) & "&nbsp;&nbsp;&nbsp;&nbsp;" & "</a>"
                            End If

                            strOutPut &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: white;"">" & vbCrLf
                            strOutPut &= "  <tr>" & vbCrLf
                            strOutPut &= "      <td>" & vbCrLf
                            strOutPut &= "          <a id=""" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_2"" href=""javascript: void(0);"" style=""font-size:9px;"" class=""tblShowMoreRegs" & cView & """ onclick=""selectedRow(this);"" onDblClick=""ShowMoves();"">" & detPunches(1) & "</a>" & vbCrLf

                            strOutPut &= "      </td>" & vbCrLf
                            strOutPut &= "      <td valign=""bottom"">" & vbCrLf
                            strOutPut &= "          <a href=""javascript: void(0)"" class=""aBtHiddenRow"" style=""overflow: hidden;"" onmouseover=""moShowDiv('" & nameDivHd & "');""></a>" & vbCrLf
                            strOutPut &= "      </td>" & vbCrLf
                            strOutPut &= "  </tr>" & vbCrLf
                            strOutPut &= "</table>"

                            strDiv &= "<a id=""div_" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_1"" href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & """>" & detPunches(0) & "</a>"
                            strDiv &= "<a id=""div_" & oSchedulerEmpDay.IDEmployee & "_" & Format(dAct, "dd/MM/yyyy") & "_2"" href=""javascript: void(0);"" class=""RowCell " & cssG1 & " " & strTypeView & """>" & detPunches(1) & "</a>"

                        End If
                    End If
                Next
                strDiv &= "</div>"

                'Carrega el div amagat per mostrar a traves de la fletxa
                strOutPut &= strDiv
                strOutPut &= "</td>" & vbCrLf
                strOutPut &= "</tr>" & vbCrLf

            End If
            strOutPut &= "</tr></table>"

        Catch ex As Exception
            strOutPut = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Return strOutPut
    End Function


    Private Sub CalculateDaysPeriods()
        Me.mDateStart = roTypes.Any2DateTime(Request("DateStart"))
        Me.mDateEnd = roTypes.Any2DateTime(Request("DateEnd"))
    End Sub

    Private Sub CalculateDays()
        Select Case Request("TypeDate")
            Case "Today"
                'Hoy
                mDateStart = Now
                mDateEnd = Now
            Case "Yesterday"
                mDateStart = Now.AddDays(-1)
                mDateEnd = Now.AddDays(-1)
            Case "CurrentWeek"
                If Now.DayOfWeek = 0 Then
                    'Si es diumenge, tira enrera 7 dies
                    mDateStart = Now.AddDays(-7)
                    mDateEnd = mDateStart.AddDays(6)
                Else
                    mDateStart = Now.AddDays((Now.DayOfWeek * -1) + 1)
                    mDateEnd = mDateStart.AddDays(6)
                End If
            Case "LastWeek"
                If Now.DayOfWeek = 0 Then
                    'Si es diumenge, tira enrera 7 dies
                    mDateStart = Now.AddDays(-14)
                    mDateEnd = mDateStart.AddDays(6)
                Else
                    mDateStart = Now.AddDays((Now.DayOfWeek * -1) + 1)
                    mDateStart = mDateStart.AddDays(-7)
                    mDateEnd = mDateStart.AddDays(6)
                End If
            Case "CurrentMonth"
                mDateStart = New Date(Now.Year, Now.Month, 1) '"01/" & Now.Month & "/" & Now.Year
                mDateEnd = mDateStart.AddMonths(1)
                mDateEnd = mDateEnd.AddDays(-1)
            Case "LastMonth"
                mDateStart = Now.AddMonths(-1)
                mDateStart = New Date(mDateStart.Year, mDateStart.Month, 1) '"01/" & mDateStart.Month & "/" & mDateStart.Year
                mDateEnd = mDateStart.AddMonths(1)
                mDateEnd = mDateEnd.AddDays(-1)
            Case "CurrentYear"
                mDateStart = New Date(Now.Year, 1, 1) ' "01/01/" & Now.Year
                mDateEnd = Now.AddDays(-1)
        End Select
        'mDateStart = Now.AddDays(-31)  ' Data Inici consulta
        'mDateEnd = DateTime.Now 'Data fin consulta
    End Sub

    ''' <summary>
    ''' Retorna el estil corresponent per pintar les celdes del calendari en Ausencies programades i Causes
    ''' </summary>
    ''' <param name="dTbl">DataTable de ProgrammedAbsences</param>
    ''' <param name="ActualDate">Data Actual</param>
    ''' <returns>Retorna tlrb (top-left-right-bottom) 1-actiu, 0-inactiu</returns>
    ''' <remarks></remarks>
    Private Function CheckProgrammedCausesAndAbsences(ByVal dTbl As DataTable, ByVal ActualDate As Date) As String
        Try
            Dim xBeginDate As Date 'Data Inici del Row
            Dim xFinishDate As Date 'Data Final del Row

            'Per saber que es te de pintar de les celdes
            Dim topPaint As Boolean = False
            Dim leftPaint As Boolean = False
            Dim rightPaint As Boolean = False
            Dim bottomPaint As Boolean = False

            'Si la taula no conte registres
            If dTbl IsNot Nothing And dTbl.Rows.Count > 0 Then
                'Filtrat sols per buscar registres entre les dates
                Dim dRows() As DataRow = dTbl.Select("(BeginDate <= '" & Format(ActualDate, "yyyy/MM/dd") & "' AND " &
                                                                        "FinishDate >= '" & Format(ActualDate, "yyyy/MM/dd") & "')")

                For Each oRow As DataRow In dTbl.Rows

                    xBeginDate = CDate(oRow("BeginDate"))

                    If Not IsDBNull(oRow("FinishDate")) Then
                        xFinishDate = CDate(oRow("FinishDate"))
                    Else
                        xFinishDate = CDate(oRow("BeginDate")).AddDays(CInt(oRow("MaxLastingDays")) - 1)
                    End If

                    If ActualDate.Date >= xBeginDate And ActualDate.Date <= xFinishDate Then

                        topPaint = True
                        bottomPaint = True

                        If ActualDate.Date = xBeginDate.Date Then
                            leftPaint = True
                        End If

                        If ActualDate.Date = xFinishDate.Date Then
                            rightPaint = True
                        End If

                        Exit For

                    End If

                Next
            End If

            Dim strReturn As String = ""
            If topPaint = True Then strReturn &= "1" Else strReturn &= "0"
            If leftPaint = True Then strReturn &= "1" Else strReturn &= "0"
            If rightPaint = True Then strReturn &= "1" Else strReturn &= "0"
            If bottomPaint = True Then strReturn &= "1" Else strReturn &= "0"
            Return strReturn
        Catch ex As Exception
            Throw ex
            'Return "0000"
        End Try
    End Function

    ''' <summary>
    ''' Retorna el estil corresponent per pintar les celdes del calendari en Ausencies programades i Causes
    ''' </summary>
    ''' <param name="dTbl">DataTable de ProgrammedAbsences</param>
    ''' <param name="ActualDate">Data Actual</param>
    ''' <returns>Retorna tlrb (top-left-right-bottom) 1-actiu, 0-inactiu</returns>
    ''' <remarks></remarks>
    Private Function CheckProgrammedCauses(ByVal dTbl As DataTable, ByVal ActualDate As Date) As String
        Try
            'Si la taula no conte registres
            If dTbl IsNot Nothing And dTbl.Rows.Count > 0 Then
                'Filtrat sols per buscar registres entre les dates
                Dim dRows() As DataRow = dTbl.Select("Date = '" & Format(ActualDate, "yyyy/MM/dd") & "'")
                If dRows.Length > 0 Then
                    Return "1111"
                Else
                    Return "0000"
                End If
            Else
                Return "0000"
            End If

        Catch ex As Exception
            Throw ex
            'Return "0000"
        End Try
    End Function

    ''' <summary>
    ''' Carga el combo per navegar per els subgrups
    ''' </summary>
    ''' <returns>Retorna un Div</returns>
    ''' <remarks></remarks>
    Private Function CargaCombo(ByVal IdGroup As Integer, ByVal bolPlanificationView As Boolean, ByVal bolShowCoverage As Boolean, ByVal CoverageView As Integer, Optional ByVal IncludeSubGroups As Boolean = False) As String
        Try
            Dim roGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, IdGroup, False)

            Dim grupUp As String = ""
            'Recupera el Grup superior
            If roGroup.Path <> roGroup.ID.ToString Then
                Dim Path() As String = roGroup.Path.Split("\")
                If Path.Length > 1 Then
                    grupUp = Path(Path.Length - 2)
                End If
            End If

            Dim dTblSubGroups As DataTable = API.EmployeeGroupsServiceMethods.GetChildGroups(Nothing, IdGroup, "Calendar", "U")

            Dim strRet As String = ""
            strRet = "<div id=""divCalGroupsSchedule"" class=""divCalGroups"
            If Me.bolHRScheduling And bolShowCoverage And IdGroup <> -1 Then strRet &= " SelectorCoverage"
            If Me.bolHRScheduling And bolPlanificationView And IdGroup <> -1 Then strRet &= " CoverageHeaderMenu"
            strRet &= """ "
            strRet &= ">"
            strRet &= "<table width=""100%"" border=""0"" cellpadding=""0"" cellspacing=""0"">"
            strRet &= "<tr>"
            strRet &= "<td width=""16px"">"
            If grupUp <> "" Then
                strRet &= "<a href=""javascript: void(0);"" class=""navUp"" title=""" & Me.Language.Translate("UpLevel", Me.DefaultScope) & """ onclick=""NavigateUp('1');"">&nbsp;</a>"
            Else
                strRet &= "&nbsp;"
            End If

            strRet &= "</td>"
            strRet &= "<td align=""center""><a href=""javascript: void(0);"" class=""groupSelector"">" & roGroup.Name & "</a></td>"

            If dTblSubGroups.Rows.Count > 0 Then
                'Si hi han grups
                If Me.bolHRScheduling Then _
                    strRet &= "<td width=""16px""><a id=""aIncludeSubGroups"" href=""javascript: void(0);"" class=""" & IIf(IncludeSubGroups, "navCollapse", "navExpand") & """  title=""" & Me.Language.Translate(IIf(IncludeSubGroups, "CollapseSelector", "ExpandSelector") & ".Title", Me.DefaultScope) & """ onclick=""ChangeIncludeSubGroups(IncludeSubGroups, true);"" titleExpand=""" & Me.Language.Translate("ExpandSelector.Title", Me.DefaultScope) & """ titleCollapse=""" & Me.Language.Translate("CollapseSelector.Title", Me.DefaultScope) & """ >&nbsp;</a>"
                strRet &= "<td width=""16px""><a href=""javascript: void(0);"" class=""navDown""  title=""" & Me.Language.Translate("NavigateDown", Me.DefaultScope) & """ onmouseover=""document.getElementById('cmbNavGroups').style.display='';"" onmouseout=""document.getElementById('cmbNavGroups').style.display='none';"">&nbsp;</a>"
                strRet &= "<div id=""cmbNavGroups"" class=""RoundCorner"" style=""position: absolute; width: 200px; height: 150px;border: solid 1px #76859F; background-color: white; display: none; overflow-y: auto;overflow-x: hidden; z-index:99999"" onmouseover=""this.style.display='';"" onmouseout=""this.style.display='none';"">"
                'Aqui van els sub-grups
                strRet &= "<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">"
                Dim SubGroupsRows() As DataRow = dTblSubGroups.Select("", "Name")
                For Each dRow As DataRow In SubGroupsRows
                    strRet &= "<tr><td><a href=""javascript: void(0)"" class=""selCmbNavGroup"" onclick=""NavigateDown('1','" & dRow("ID") & "')"">" & dRow("Name") & "</a></td></tr>"
                Next
                strRet &= "</table>"
            Else
                strRet &= "<td width=""20px"">&nbsp;"
            End If

            strRet &= "</div>"
            strRet &= "</td>"
            strRet &= "</tr>"

            If Me.bolHRScheduling And bolPlanificationView And IdGroup <> -1 Then

                strRet &= "<tr id=""trSelectorCoverage"" style=""display: " & IIf(bolShowCoverage, "", "none") & ";""><td colspan=""4"" align=""center"">"
                strRet &= "<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""coverageSelector"" >"
                strRet &= "<tr>"
                strRet &= "<td align=""center""><a id=""aCoverageView"" href=""javascript: void(0);"" class=""groupSelector"">" & Me.Language.Translate("CoverageView." & CoverageView.ToString & ".SelectorTitle", Me.DefaultScope) & "</a></td>"
                strRet &= "<td width=""16px""><a href=""javascript: void(0);"" class=""navSelector""  title=""" & Me.Language.Translate("CoverageView.SelectorTitle", Me.DefaultScope) & """ onmouseover=""document.getElementById('cmbCoverageView').style.display='';"" onmouseout=""document.getElementById('cmbCoverageView').style.display='none';"">&nbsp;</a>"
                strRet &= "<div id=""cmbCoverageView"" style=""position: absolute; width: 150px; height: 75px;border: solid 1px #dddddd; background-color: white; display: none; overflow-y: auto;overflow-x: hidden; z-index:99999"" onmouseover=""this.style.display='';"" onmouseout=""this.style.display='none';"">"
                strRet &= "<table width=""100%"" border=""0"" cellspacing=""0"" cellpadding=""0"">"
                strRet &= "<tr><td><a href=""javascript: void(0)"" class=""selCmbNavGroup"" onclick=""document.getElementById('aCoverageView').innerHTML=this.innerHTML; CoverageView=0; LoadDayHeadersPlan(false); "">" & Me.Language.Translate("CoverageView.0.SelectorTitle", Me.DefaultScope) & "</a></td></tr>"
                strRet &= "<tr><td><a href=""javascript: void(0)"" class=""selCmbNavGroup"" onclick=""document.getElementById('aCoverageView').innerHTML=this.innerHTML; CoverageView=1; LoadDayHeadersPlan(false); "">" & Me.Language.Translate("CoverageView.1.SelectorTitle", Me.DefaultScope) & "</a></td></tr>"
                strRet &= "<tr><td><a href=""javascript: void(0)"" class=""selCmbNavGroup"" onclick=""document.getElementById('aCoverageView').innerHTML=this.innerHTML; CoverageView=2; LoadDayHeadersPlan(false); "">" & Me.Language.Translate("CoverageView.2.SelectorTitle", Me.DefaultScope) & "</a></td></tr>"
                strRet &= "</table></div>"
                strRet &= "</tr>"
                strRet &= "</table>"
                strRet &= "</td></tr>"

            End If

            strRet &= "</table>"
            strRet &= "</div>"

            Return strRet

        Catch ex As Exception
            Return "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try
    End Function

    Private Sub LoadSchedulerDiaryParts()

    End Sub





    Private Shared Function CheckColortoHex(ByVal oHTMLColor As String) As String
        If oHTMLColor = "White" Then Return "#FFFFFF"
        If oHTMLColor = "Black" Then Return "#000000"
        Return oHTMLColor
    End Function

    ''' <summary>
    ''' Crea el Grid de Usuaris TAB General
    ''' </summary>
    ''' <param name="dTable">DataTable amb les dades del usuari</param>
    ''' <param name="dColField">Columna que servira per asignar l'ID al input text i que no es pintara</param>
    ''' <param name="editMode">Si pintara els input text</param>
    ''' <param name="arrErrors">Datatable amb els error que pintara</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGrid(ByVal dTable As DataTable, Optional ByVal dColField As String = "", Optional ByVal editMode As Boolean = False, Optional ByVal arrErrors As DataTable = Nothing) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Width = "250"
            hTCell.Attributes("style") = "border-right: 0; width: 250px"
            hTCell.InnerHtml = "Campo"
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.InnerHtml = "Valor"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Si es la columna del id
                    If dTable.Columns(y).ColumnName = dColField Then Continue For
                    hTCell = New HtmlTableCell

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow

                    If (y = 2 And editMode = True) Then
                        hTCell.Style("padding") = "0px"
                        If arrErrors IsNot Nothing Then
                            'Si es comproben els errors
                            Dim dRows() As DataRow = arrErrors.Select("FieldName = '" & dTable.Rows(n)(dColField).ToString & "'")
                            If dRows.Length > 0 Then
                                hTCell.InnerHtml = "<input type=""text"" style=""width: 100%; color: #666666; font-size: 11px; background-color: #ffffff; border: solid 1px #7D8BA3; padding-left: 2px; background-color: #FEFE9B;"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dRows(0).Item("FieldValue") & """ /><br />" &
                                "<span style=""padding-left: 5px; color: red"">" & dRows(0).Item("ErrorDescription").ToString & "</span>"
                            Else
                                hTCell.InnerHtml = "<input type=""text"" style=""width: 100%; color: #666666; font-size: 11px; background-color: #ffffff; border: solid 1px #7D8BA3; padding-left: 2px;"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dTable.Rows(n)(y).ToString & """ />"
                            End If
                        Else
                            hTCell.InnerHtml = "<input type=""text"" style=""width: 100%; color: #666666; font-size: 11px; background-color: #ffffff; border: solid 1px #7D8BA3; padding-left: 2px;"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dTable.Rows(n)(y).ToString & """ />"
                        End If

                    Else
                        hTCell.Style("padding") = "3px"
                        hTCell.InnerText = dTable.Rows(n)(y).ToString
                    End If
                    If hTCell.InnerHtml = "" Then hTCell.InnerHtml = "&nbsp;"
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable

        Catch ex As Exception
            Dim hTableError As New HtmlTable
            Dim hTableCellError As New HtmlTableCell
            Dim hTableRowError As New HtmlTableRow
            hTableCellError.InnerHtml = ex.Message.ToString
            hTableRowError.Cells.Add(hTableCellError)
            hTableError.Rows.Add(hTableRowError)
            Return hTableError
        End Try
    End Function

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell


            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n) '"GridStyle-cellheader"
                hTCell.InnerHtml = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerHtml = "&nbsp;"
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            'Celda d'espai per edicio
            hTCell = New HtmlTableCell
            hTCell.InnerHtml = "&nbsp;"
            hTCell.Width = "40px"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable

        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Crea un Grid d'estructura de Ausencias
    ''' </summary>
    ''' <param name="dTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaGridLists(ByVal dTable As DataTable, Optional ByVal nomCols() As String = Nothing, Optional ByVal sizeCols() As String = Nothing, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal editIcons As Boolean = False, Optional ByVal colSpanCol As Integer = 1, Optional ByVal moveIcon As Boolean = False) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickShow As String = ""         'Href onclick Mode edicio
            Dim strClickMove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            'hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'Bucle als registres

            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim colsizeint As Integer = -1
                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar 
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    colsizeint += 1
                    hTCell = New HtmlTableCell
                    hTCell.Width = sizeCols(colsizeint)

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    hTCell.Style("display") = "table-cell"

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString
                    If hTCell.InnerText = "" Then hTCell.InnerHtml = "&nbsp;"
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)

                    'Dibuixem les columnes de les icones d'edicio
                    If editIcons = True Then
                        If y = dTable.Columns.Count - 1 Then
                            hTCell = New HtmlTableCell
                            hTCell.Width = "40px"
                            hTCell.Attributes("class") = "GridStyle-cellheader"
                            hTCell.Attributes("style") = "border: 0; background-color: #E8EEF7;border-right: solid 1px #D7D7D7;"
                            Dim hAnchorEdit As New HtmlAnchor
                            Dim hAnchorRemove As New HtmlAnchor

                            strClickShow = "cargaEmpleado("
                            strClickMove = "ShowMoveCurrentEmployee("
                            If dTblControls IsNot Nothing Then
                                For Each dRow As DataRow In dTblControls.Rows
                                    strClickShow &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                    strClickMove &= "'" & dTable.Rows(n)(dRow(0)).ToString & "',"
                                Next
                            End If
                            strClickShow = strClickShow.Substring(0, strClickShow.Length - 1) & ");"
                            strClickMove = strClickMove.Substring(0, strClickMove.Length - 1) & ");"

                            hAnchorEdit.Attributes("onclick") = strClickShow
                            hAnchorEdit.Title = Me.Language.Translate("ViewEmployee", DefaultScope)
                            hAnchorEdit.HRef = "javascript: void(0);"
                            hAnchorEdit.InnerHtml = "<img src=""" & System.Web.VirtualPathUtility.ToAbsolute("~/Base/Images/Grid/showemp.png") & """>"

                            hAnchorRemove.Attributes("onclick") = strClickMove
                            hAnchorRemove.Title = Me.Language.Translate("MoveEmployee", DefaultScope)
                            hAnchorRemove.HRef = "javascript: void(0);"
                            hAnchorRemove.InnerHtml = "<img src=""" & System.Web.VirtualPathUtility.ToAbsolute("~/Base/Images/Grid/move.png") & """>"

                            hTCell.Controls.Add(hAnchorEdit)
                            If moveIcon = True Then
                                hTCell.Controls.Add(hAnchorRemove)
                            End If

                            hTRow.Cells.Add(hTCell)
                        End If
                    End If
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Genera automaticament HtmlButtons
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="Transparente">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlButton(ByVal Name As String, ByVal Text As String, ByVal Transparente As Boolean) As HtmlButton
        Dim obutton As New HtmlButton
        obutton.ID = Name
        obutton.InnerText = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Cambia el color de la font (blanc o negre) segons el backgroundcolor
    ''' </summary>
    ''' <param name="bg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IdealTextColor(ByVal bg As System.Drawing.Color) As System.Drawing.Color
        Dim nThreshold As Integer = 105
        Dim bgDelta As Integer = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + (bg.B * 0.114))
        Dim forecolor As System.Drawing.Color
        If (255 - bgDelta < nThreshold) Then
            forecolor = System.Drawing.Color.Black
        Else
            forecolor = System.Drawing.Color.White
        End If
        Return forecolor
    End Function

    ''' <summary>
    ''' Enganxat Especial (simple 1 dia)
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub copySpecialPlan()

        Dim strResponse As String = ""

        Try
            Dim idEmployeeOri As String = Request("IdEmployeeOri")
            Dim DatePlanOrig As Date = convertDate(Request("DatePlanOrig"))
            Dim idEmployeeDest As String = Request("IdEmployeeDest")
            Dim DatePlanDestIni As Date = Request("DatePlanDestIni")
            Dim DatePlanDestFin As Date = Request("DatePlanDestFin")

            Dim strRetDateLocked As String = Request("retDateTarget")
            Dim xDateLocked As Date = Nothing
            If strRetDateLocked <> "" Then xDateLocked = convertDate(strRetDateLocked)

            Dim strMsgResponse As String = Request("msgResponse")
            Dim oResponse As LockedDayAction = Me.RetMsgResponse(strMsgResponse)

            Dim strMsgResponseCoverage As String = Request("msgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = Me.RetMsgResponse(strMsgResponseCoverage)

            Dim MsgResponseShift As String = Request("msgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = Me.RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                If DatePlanDestIni > DatePlanDestFin Then
                    Response.Write(Me.Language.Translate("DateBeginEndIncorrect", Me.DefaultScope)) 'Fecha fin mayor que inicio
                    Exit Sub
                End If

                ' Obtener la configuración de horarios almacenada en la variable de sessión                    
                If API.EmployeeServiceMethods.CopyShifts(Nothing, idEmployeeOri, idEmployeeDest, DatePlanOrig, DatePlanOrig, DatePlanDestIni, DatePlanDestFin,
                                                                     ActionShiftType.AllShift, oResponse, oResponseCoverage, oResponseShift, xDateLocked, False, True) = True Then
                    strResponse = "OK"
                Else
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                        Dim intStopType As Integer = 0
                        Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                            Case EmployeeResultEnum.DailyScheduleLockedDay
                                intStopType = 0
                            Case EmployeeResultEnum.DailyScheduleCoverageDay
                                intStopType = 1
                            Case EmployeeResultEnum.ShiftWithoutPermission
                                intStopType = 2
                        End Select

                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, idEmployeeDest, False)

                        Dim oParams As New Generic.List(Of String)
                        If oEmployee IsNot Nothing Then
                            oParams.Add(oEmployee.Name)
                        Else
                            oParams.Add(CInt(idEmployeeDest))
                        End If
                        oParams.Add(Format(xDateLocked, HelperWeb.GetShortDateFormat))
                        Dim strMsgRes As String = ""
                        Select Case intStopType
                            Case 0
                                strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                            Case 1
                                strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                            Case 2
                                strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                        End Select

                        Dim oMsg As New SchedulerCellMessage(strMsgRes, idEmployeeDest, oEmployee.Name, Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, Nothing)
                        strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)

                    ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                        strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
                    Else
                        strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                    End If

                End If

            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub

    ''' <summary>
    ''' Copia el plan (multiple) (Pegado Especial)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub copySpecialMultiPlan()

        Dim strResponse As String = ""

        Try

            Dim TypeCopy As String = Request("TypeCopy")

            Dim DateIni As Date = Request("dateIni")
            Dim DateFin As Date = Request("dateFin")

            Dim strRetDateLocked As String = Request("retDateTarget")
            Dim xDateLocked As Date = Nothing
            If strRetDateLocked <> "" Then xDateLocked = convertDate(strRetDateLocked)

            Dim strRetIDEmployee As String = Request("retIdEmployee")
            Dim xIDEmployee As Integer = Nothing
            If strRetIDEmployee <> "" Then xIDEmployee = CInt(strRetIDEmployee)

            Dim strMsgResponse As String = Request("msgResponse")
            Dim oResponse As LockedDayAction = Me.RetMsgResponse(strMsgResponse)

            Dim strMsgResponseCoverage As String = Request("msgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = Me.RetMsgResponse(strMsgResponseCoverage)

            Dim MsgResponseShift As String = Request("msgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = Me.RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                If TypeCopy = "vertical" Then

                    Dim totEmpsViewed() As String = Request("TotalEmpsViewed").Split(",")

                    Dim EmpstoCopy() As String = Request("EmpstoCopy").Split(",")
                    Dim EmpstoCopyInt() As Integer = Nothing 'Converteix a integers el arraylist
                    For n As Integer = 0 To EmpstoCopy.Length - 1
                        ReDim Preserve EmpstoCopyInt(n)
                        EmpstoCopyInt(n) = CInt(EmpstoCopy(n).ToString)
                    Next

                    Dim DatetoCopy As Date = convertDate(Request("DatetoCopy"))
                    Dim IdEmployeeDest As String = Request("IdEmployeeDest")
                    Dim DatePlanDest As Date = convertDate(Request("DatePlanDest"))

                    Dim EmpsDest() As Integer = Nothing 'Usuaris desti
                    Dim lenStartCopy As Integer = 0
                    Dim lenEmpstoCopy As Integer = EmpstoCopy.Length - 1
                    Dim startFlag As Boolean = False

                    For n As Integer = 0 To totEmpsViewed.Length - 1
                        'Si el usuari es el desti, comença a llegir cap abaix
                        If totEmpsViewed(n) = IdEmployeeDest Then
                            ReDim Preserve EmpsDest(lenStartCopy)
                            EmpsDest(lenStartCopy) = CInt(totEmpsViewed(n).ToString)
                            startFlag = True
                            lenStartCopy += 1
                        ElseIf startFlag = True And totEmpsViewed(n) <> IdEmployeeDest Then
                            If lenStartCopy <= lenEmpstoCopy Then
                                ReDim Preserve EmpsDest(lenStartCopy)
                                EmpsDest(lenStartCopy) = CInt(totEmpsViewed(n).ToString)
                                lenStartCopy += 1
                            Else
                                Exit For
                            End If
                        ElseIf startFlag = False Then
                        End If
                    Next

                    If API.EmployeeServiceMethods.CopyShifts(Nothing, EmpstoCopyInt, EmpsDest, DatetoCopy, DateIni, DateFin, ActionShiftType.AllShift, oResponse,
                                                                         oResponseCoverage, oResponseShift, xDateLocked, xIDEmployee, False, True) = False Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                            Dim oEmployee As roEmployee = Nothing

                            Dim intStopType As Integer = 0
                            Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                                Case EmployeeResultEnum.DailyScheduleLockedDay
                                    intStopType = 0
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, xIDEmployee, False)

                                Case EmployeeResultEnum.DailyScheduleCoverageDay
                                    intStopType = 1
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, xIDEmployee, False)

                                Case EmployeeResultEnum.ShiftWithoutPermission
                                    intStopType = 2
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, xIDEmployee, False)

                            End Select

                            Dim oParams As New Generic.List(Of String)
                            If oEmployee IsNot Nothing Then
                                oParams.Add(oEmployee.Name)
                            Else
                                oParams.Add(CInt(xIDEmployee))
                            End If
                            oParams.Add(Format(xDateLocked, HelperWeb.GetShortDateFormat))
                            Dim strMsgRes As String = ""
                            Select Case intStopType
                                Case 0
                                    strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                                Case 1
                                    strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                                Case 2
                                    strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                            End Select

                            Dim oMsg As New SchedulerCellMessage(strMsgRes, IdEmployeeDest, oEmployee.Name, Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, Nothing)
                            strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
                        Else
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                    Else
                        strResponse = "OK"
                    End If

                ElseIf TypeCopy = "horizontal" Then

                    Dim EmpstoCopy As Integer = CInt(Request("EmpstoCopy"))
                    Dim DatetoCopy() As String = Request("DatetoCopy").Split(",")
                    Dim IdEmployeeDest As String = Request("IdEmployeeDest")
                    Dim DatePlanDest As Date = convertDate(Request("DatePlanDest"))

                    'Recupero data inicial, data final
                    Dim dOri As Date = convertDate(DatetoCopy(0))
                    Dim dDest As Date = convertDate(DatetoCopy(DatetoCopy.Length - 1))

                    If API.EmployeeServiceMethods.CopyShifts(Nothing, EmpstoCopy, IdEmployeeDest, dOri, dDest, DateIni, DateFin, ActionShiftType.AllShift, oResponse,
                                                                         oResponseCoverage, oResponseShift, xDateLocked, False, True) = False Then
                        If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                           roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                            'LOCK HORIZONTAL

                            Dim intStopType As Integer = 0
                            Dim oEmployee As roEmployee = Nothing

                            Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                                Case EmployeeResultEnum.DailyScheduleLockedDay
                                    intStopType = 0
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IdEmployeeDest, False)

                                Case EmployeeResultEnum.DailyScheduleCoverageDay
                                    intStopType = 1
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IdEmployeeDest, False)

                                Case EmployeeResultEnum.ShiftWithoutPermission
                                    intStopType = 2
                                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, EmpstoCopy, False)
                            End Select

                            Dim oParams As New Generic.List(Of String)
                            If oEmployee IsNot Nothing Then
                                oParams.Add(oEmployee.Name)
                            Else
                                oParams.Add(CInt(xIDEmployee))
                            End If
                            oParams.Add(Format(xDateLocked, HelperWeb.GetShortDateFormat))
                            Dim strMsgRes As String = ""
                            Select Case intStopType
                                Case 0
                                    strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                                Case 1
                                    strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                                Case 2
                                    strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                            End Select

                            Dim oMsg As New SchedulerCellMessage(strMsgRes, IdEmployeeDest, oEmployee.Name, Format(xDateLocked, HelperWeb.GetShortDateFormat).ToString, Nothing)
                            strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                        ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
                        Else
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                        End If
                    Else
                        strResponse = "OK"
                    End If

                End If

            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub


    ''' <summary>
    ''' Desasigna horarios alternativos de varios dia / empleado 
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DisassignMultiPlanAlt()
        Dim strResponse As String = ""
        Try
            Dim bolRet As Boolean = True
            Dim strCellsAssign As String = Request("CellsAssign")
            Dim Cells() As String = strCellsAssign.Split(",")

            Dim strRetIDEmployee As String = Request("retIDEmployee")
            Dim strRetDate As String = Request("retDateDest")

            Dim strEmployee As String = String.Empty
            Dim strDate As String

            Dim arrDate() As String
            Dim dDate As Date

            Dim bolFind As Boolean = False

            Dim arrCells As New Generic.List(Of srvScheduler.SchedulerCell)
            Dim strRender As String = ""
            Dim sCell As srvScheduler.SchedulerCell
            Dim oColor As String = ""
            Dim bolGradient As Boolean = False
            Dim strColorFrom As String = ""
            Dim strColorTo As String = ""

            Dim xStartShift1 As DateTime = Nothing
            Dim xStartShift2 As DateTime = Nothing
            Dim xStartShift3 As DateTime = Nothing
            Dim xStartShift4 As DateTime = Nothing

            Dim intIDAssignment As Integer = 0
            Dim bolAssign As Boolean = False

            Dim strMsgResponse As String = Request("msgResponse")
            Dim oResponse As LockedDayAction = Me.RetMsgResponse(strMsgResponse)

            Dim strMsgResponseCoverage As String = Request("msgResponseCoverage")
            Dim oResponseCoverage As LockedDayAction = Me.RetMsgResponse(strMsgResponseCoverage)

            Dim MsgResponseShift As String = Request("msgResponseShift")
            Dim oResponseShift As ShiftPermissionAction = Me.RetMsgResponseShift(MsgResponseShift)

            If oResponseShift <> ShiftPermissionAction.StopAll Then

                For Each Cell As String In Cells
                    'TODO: Comprobar els horaris alternatius abans de assignar el calendari
                    strEmployee = Cell.Split("_")(0)
                    strDate = Cell.Split("_")(1)

                    arrDate = strDate.Split("/")
                    dDate = New Date(arrDate(2), arrDate(1), arrDate(0))

                    If strRetIDEmployee <> "" And strRetDate <> "" And bolFind = False Then

                        If strEmployee = strRetIDEmployee Then
                            If strRetDate = strDate Then
                                bolFind = True
                            Else
                                Continue For
                            End If
                        Else
                            Continue For
                        End If
                        If bolFind = True Then

                            Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                            If dtblPlan.Rows.Count > 0 Then

                                xStartShift1 = Nothing
                                xStartShift2 = Nothing
                                xStartShift3 = Nothing
                                xStartShift4 = Nothing
                                If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                intIDAssignment = 0

                                bolAssign = API.EmployeeServiceMethods.AssignShift(Nothing, strEmployee, dDate, roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1")), -1, -1, -1, xStartShift1,
                                                                                               Nothing, Nothing, Nothing, intIDAssignment, oResponse, oResponseCoverage, True, oResponseShift)
                                If Not bolAssign Then
                                    bolRet = False
                                Else

                                    dtblPlan = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                                    If dtblPlan.Rows.Count > 0 Then

                                        xStartShift1 = Nothing
                                        xStartShift2 = Nothing
                                        xStartShift3 = Nothing
                                        xStartShift4 = Nothing
                                        If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                        If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                        If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                        If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                        intIDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))

                                        strRender = ""
                                        sCell = New srvScheduler.SchedulerCell(strEmployee, strDate, strRender)
                                        sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                        sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                        sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                        sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                        sCell.StartShift1 = xStartShift1
                                        sCell.StartShift2 = xStartShift2
                                        sCell.StartShift3 = xStartShift3
                                        sCell.StartShift4 = xStartShift4
                                        sCell.ShiftColor1 = oColor
                                        sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                        sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                        sCell.Gradient = bolGradient
                                        sCell.ColorFrom = strColorFrom
                                        sCell.ColorTo = strColorTo
                                        sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                        arrCells.Add(sCell)

                                    End If

                                    If oResponse = LockedDayAction.ReplaceAll Or oResponse = LockedDayAction.NoReplaceAll Then
                                    Else
                                        oResponse = LockedDayAction.None
                                    End If

                                End If


                            End If

                        End If

                    Else

                        Dim dtblPlan As DataTable = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                        If dtblPlan.Rows.Count > 0 Then

                            xStartShift1 = Nothing
                            xStartShift2 = Nothing
                            xStartShift3 = Nothing
                            xStartShift4 = Nothing
                            If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                            intIDAssignment = 0

                            bolAssign = Robotics.Web.Base.API.EmployeeServiceMethods.AssignShift(Nothing, strEmployee, dDate, roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1")), -1, -1, -1,
                                                                                                             xStartShift1, Nothing, Nothing, Nothing, intIDAssignment, oResponse, oResponseCoverage, True, oResponseShift)
                            If Not bolAssign Then
                                bolRet = False
                            Else

                                dtblPlan = Robotics.Web.Base.API.EmployeeServiceMethods.GetPlan(Nothing, strEmployee, dDate)
                                If dtblPlan.Rows.Count > 0 Then

                                    xStartShift1 = Nothing
                                    xStartShift2 = Nothing
                                    xStartShift3 = Nothing
                                    xStartShift4 = Nothing
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift1")) Then xStartShift1 = dtblPlan.Rows(0)("StartShift1")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift2")) Then xStartShift2 = dtblPlan.Rows(0)("StartShift2")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift3")) Then xStartShift3 = dtblPlan.Rows(0)("StartShift3")
                                    If Not IsDBNull(dtblPlan.Rows(0)("StartShift4")) Then xStartShift4 = dtblPlan.Rows(0)("StartShift4")
                                    intIDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                    strRender = ""
                                    sCell = New srvScheduler.SchedulerCell(strEmployee, strDate, strRender)
                                    sCell.IDShift1 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift1"))
                                    sCell.IDShift2 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift2"))
                                    sCell.IDShift3 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift3"))
                                    sCell.IDShift4 = roTypes.Any2Integer(dtblPlan.Rows(0)("IdShift4"))
                                    sCell.StartShift1 = xStartShift1
                                    sCell.StartShift2 = xStartShift2
                                    sCell.StartShift3 = xStartShift3
                                    sCell.StartShift4 = xStartShift4
                                    sCell.ShiftColor1 = oColor
                                    sCell.Locked = roTypes.Any2Boolean(dtblPlan.Rows(0)("LockedDay"))
                                    sCell.IDAssignment = roTypes.Any2Integer(dtblPlan.Rows(0)("IDAssignment"))
                                    sCell.Gradient = bolGradient
                                    sCell.ColorFrom = strColorFrom
                                    sCell.ColorTo = strColorTo
                                    sCell.CoverageIDEmployee = roTypes.Any2Integer(dtblPlan.Rows(0)("CoverageIDEmployee"))
                                    arrCells.Add(sCell)

                                End If

                                If oResponse = LockedDayAction.ReplaceAll Or oResponse = LockedDayAction.NoReplaceAll Then
                                Else
                                    oResponse = LockedDayAction.None
                                End If

                            End If
                        End If

                    End If

                    If bolRet = False Then Exit For
                Next

                If bolRet Then
                    strResponse = "OK" & roJSONHelper.Serialize(arrCells)
                Else
                    If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Or
                       roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then

                        Dim intStopType As Integer = 0
                        Select Case roWsUserManagement.SessionObject.States.EmployeeState.Result
                            Case EmployeeResultEnum.DailyScheduleLockedDay
                                intStopType = 0
                            Case EmployeeResultEnum.DailyScheduleCoverageDay
                                intStopType = 1
                            Case EmployeeResultEnum.ShiftWithoutPermission
                                intStopType = 2
                        End Select

                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, CInt(strEmployee), False)

                        Dim oParams As New Generic.List(Of String)
                        If oEmployee IsNot Nothing Then
                            oParams.Add(oEmployee.Name)
                        Else
                            oParams.Add(CInt(strEmployee))
                        End If
                        oParams.Add(Format(dDate, HelperWeb.GetShortDateFormat))
                        Dim strMsgRes As String = ""
                        Select Case intStopType
                            Case 0
                                strMsgRes = Me.Language.Translate("DailyScheduleLockedDay.Description", DefaultScope, oParams)
                            Case 1
                                strMsgRes = Me.Language.Translate("DailyScheduleCoverageDay.Description", DefaultScope, oParams)
                            Case 2
                                strMsgRes = Me.Language.Translate("DailyScheduleShiftWithoutPermission.Description", DefaultScope, oParams)
                        End Select

                        Dim oMsg As New SchedulerCellMessage(strMsgRes, strEmployee, oEmployee.Name, Format(dDate, HelperWeb.GetShortDateFormat).ToString, arrCells)
                        strResponse = "STOP" & intStopType.ToString & roJSONHelper.Serialize(oMsg)
                    Else
                        strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                    End If

                End If

            Else
                strResponse = "OK"
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    Private Sub getDayPlan()
        Dim strResponse As String = ""
        Try
            Dim IDEmployee As Integer = Request("IDEmployee")

            Dim arrDate() As String = Request("PlanDate").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            Dim tbPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Nothing, IDEmployee, dDate)
            If tbPlan IsNot Nothing AndAlso tbPlan.Rows.Count > 0 Then
                strResponse = "OK" & roTypes.Any2Integer(tbPlan.Rows(0)("IDShift1")) & "~" & roTypes.Any2String(tbPlan.Rows(0)("Name1")) & "~" &
                                     roTypes.Any2Integer(tbPlan.Rows(0)("IDShift2")) & "~" & roTypes.Any2String(tbPlan.Rows(0)("Name2")) & "~" &
                                     roTypes.Any2Integer(tbPlan.Rows(0)("IDShift3")) & "~" & roTypes.Any2String(tbPlan.Rows(0)("Name3")) & "~" &
                                     roTypes.Any2Integer(tbPlan.Rows(0)("IDShift4")) & "~" & roTypes.Any2String(tbPlan.Rows(0)("Name4")) & "~"
            Else
                strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Devuelve el nombre del horario indicando la hora de inicio del flotante.
    ''' </summary>
    ''' <param name="_StartFloating"></param>
    ''' <returns></returns>
    ''' <remarks>Si el horario no es flotante, devuelve el nombre del horario.</remarks>
    Private Function FloatingShiftName(ByVal _ShiftName As String, ByVal _StartFloating As DateTime) As String

        Dim strRet As String = _ShiftName

        If _StartFloating <> Nothing Then

            strRet = "[" & Format(_StartFloating, "HH:mm")
            If _StartFloating < New DateTime(1899, 12, 30, 0, 0, 0) Then
                ' Día anterior al del horario
                strRet &= "-"
            ElseIf _StartFloating > New DateTime(1899, 12, 30, 23, 59, 59) Then
                ' Día posterior al del horario
                strRet &= "+"
            End If
            strRet &= "] " & _ShiftName

        End If

        Return strRet

    End Function

    Private Function CreateCoverageCell(ByVal IdGroup As Integer, ByRef oDailyCoverage As roDailyCoverage, ByVal CoverageView As Integer, ByVal TypeView As Integer, ByVal tbAssignments As DataTable) As String

        Dim strRet As String = ""

        ' Generamos los atributos de la dotación
        Dim strAttributes As String = ""
        If oDailyCoverage.CoverageAssignments IsNot Nothing Then
            strAttributes = "coveragescount=""" & oDailyCoverage.CoverageAssignments.Count & """ "
            For i As Integer = 0 To oDailyCoverage.CoverageAssignments.Count - 1
                strAttributes &= "idassignment" & i.ToString & "=""" & oDailyCoverage.CoverageAssignments(i).IDAssignment.ToString() & """ expected" & i.ToString & "=""" & oDailyCoverage.CoverageAssignments(i).ExpectedCoverage.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".") & """ "
            Next
        Else
            strAttributes = "coveragescount=""" & 0 & """ "
        End If

        ' Generamos la visualización de los datos
        Dim strInfo As String = ""
        Dim strFullInfo As String = ""

        Dim strAssignmentDesc As String = ""
        Dim strCoverage As String = ""
        Dim strCoverageTooltip As String = ""
        Dim strAssignmentCss As String = ""
        Dim strDetailColor As String = ""

        Dim strTableCss As String = ""
        Select Case CoverageView
            Case 0 : strTableCss = "TeoricCoverageView"
            Case 1 : strTableCss = "PlannedCoverageViewOk"
            Case 2 : strTableCss = "ActualCoverageViewOk"
        End Select

        Dim nameDivHd As String = "hiddenCoverageCell" & TypeView & "_" & IdGroup & "_" & Format(oDailyCoverage.CoverageDate, "dd/MM/yyyy") & "_" & TypeView.ToString

        Dim oParams As Generic.List(Of String)

        Dim intCoverageIndex As Integer = 0

        If oDailyCoverage.CoverageAssignments IsNot Nothing Then

            For i As Integer = 0 To oDailyCoverage.CoverageAssignments.Count - 1

                ' Obtenemos el nombre del puesto
                strAssignmentDesc = "??"
                Dim oRows() As DataRow = tbAssignments.Select("ID=" & oDailyCoverage.CoverageAssignments(i).IDAssignment.ToString)
                If oRows.Length = 1 Then
                    Select Case TypeView
                        Case 0
                            strAssignmentDesc = roTypes.Any2String(oRows(0).Item("ShortName"))
                        Case 1, 2
                            strAssignmentDesc = roTypes.Any2String(oRows(0).Item("Name"))
                    End Select
                End If

                strCoverage = ""
                strCoverageTooltip = ""
                strAssignmentCss = ""
                strDetailColor = ""
                oParams = New Generic.List(Of String)

                Select Case CoverageView
                    Case 0 ' Vista teórica                                    
                        oParams.Add(oDailyCoverage.CoverageAssignments(i).ExpectedCoverage.ToString)
                        oParams.Add(strAssignmentDesc)
                        strCoverage = Me.Language.Translate("ExpectedCoverage." & TypeView.ToString & ".Teoric", Me.DefaultScope, oParams)
                        strCoverageTooltip = Me.Language.Translate("ExpectedCoverage.Teoric.Tooltip", Me.DefaultScope, oParams)
                        strAssignmentCss = "TeoricCoverageDetailView"

                    Case 1 ' Vista planificada
                        If oDailyCoverage.CoverageAssignments(i).ForcedCoveraged.HasValue AndAlso oDailyCoverage.CoverageAssignments(i).ForcedCoveraged.Value Then
                            oParams.Add(strAssignmentDesc)
                            strCoverage = Me.Language.Translate("PlannedCoverage." & TypeView.ToString & ".Forced", Me.DefaultScope, oParams)
                            strCoverageTooltip = Me.Language.Translate("PlannedCoverage.Forced.Tooltip", Me.DefaultScope, oParams)
                            strAssignmentCss = "PlannedCoverageDetailViewForced"
                            If strTableCss <> "PlannedCoverageViewError" Then strTableCss = "PlannedCoverageViewForced"
                        Else
                            If oDailyCoverage.CoverageAssignments(i).ExpectedCoverage > oDailyCoverage.CoverageAssignments(i).PlannedCoverage Then
                                oParams.Add(strAssignmentDesc)
                                strCoverage = Me.Language.Translate("PlannedCoverage." & TypeView.ToString & ".Missing", Me.DefaultScope, oParams)
                                strCoverageTooltip = Me.Language.Translate("PlannedCoverage.Missing.Tooltip", Me.DefaultScope, oParams)
                                strAssignmentCss = "PlannedCoverageDetailViewError"
                                strTableCss = "PlannedCoverageViewError"
                            ElseIf oDailyCoverage.CoverageAssignments(i).ExpectedCoverage < oDailyCoverage.CoverageAssignments(i).PlannedCoverage Then
                                oParams.Add(strAssignmentDesc)
                                strCoverage = Me.Language.Translate("PlannedCoverage." & TypeView.ToString & ".Over", Me.DefaultScope, oParams)
                                strCoverageTooltip = Me.Language.Translate("PlannedCoverage.Over.Tooltip", Me.DefaultScope, oParams)
                                strAssignmentCss = "PlannedCoverageDetailViewOver"
                            End If
                        End If

                    Case 2 ' Vista real
                        If oDailyCoverage.CoverageAssignments(i).ForcedCoveraged.HasValue AndAlso oDailyCoverage.CoverageAssignments(i).ForcedCoveraged.Value Then
                            oParams.Add(strAssignmentDesc)
                            strCoverage = Me.Language.Translate("ActualCoverage." & TypeView.ToString & ".Forced", Me.DefaultScope, oParams)
                            strCoverageTooltip = Me.Language.Translate("ActualCoverage.Forced.Tooltip", Me.DefaultScope, oParams)
                            strAssignmentCss = "ActualCoverageDetailViewForced"
                            If strTableCss <> "ActualCoverageViewError" Then strTableCss = "ActualCoverageViewForced"
                        Else
                            If oDailyCoverage.CoverageAssignments(i).ExpectedCoverage > oDailyCoverage.CoverageAssignments(i).ActualCoverage Then
                                oParams.Add(strAssignmentDesc)
                                strCoverage = Me.Language.Translate("ActualCoverage." & TypeView.ToString & ".Missing", Me.DefaultScope, oParams)
                                strCoverageTooltip = Me.Language.Translate("ActualCoverage.Missing.Tooltip", Me.DefaultScope, oParams)
                                strAssignmentCss = "ActualCoverageDetailViewError"
                                strTableCss = "ActualCoverageViewError"
                            ElseIf oDailyCoverage.CoverageAssignments(i).ExpectedCoverage < oDailyCoverage.CoverageAssignments(i).ActualCoverage Then
                                oParams.Add(strAssignmentDesc)
                                strCoverage = Me.Language.Translate("ActualCoverage." & TypeView.ToString & ".Over", Me.DefaultScope, oParams)
                                strCoverageTooltip = Me.Language.Translate("ActualCoverage.Over.Tooltip", Me.DefaultScope, oParams)
                                strAssignmentCss = "ActualCoverageDetailViewOver"
                            End If
                        End If

                End Select

                If strAssignmentCss <> "" Then strAssignmentCss = "class=""" & strAssignmentCss & """"

                If strCoverage <> "" Then

                    Select Case TypeView
                        Case 0 : If strCoverage.Length > 9 Then strCoverage = strCoverage.Substring(0, 6) & "..."
                        Case 1 : If strCoverage.Length > 14 Then strCoverage = strCoverage.Substring(0, 11) & "..."
                        Case 2 : If strCoverage.Length > 17 Then strCoverage = strCoverage.Substring(0, 14) & "..."
                    End Select

                    If intCoverageIndex < 2 Then
                        If intCoverageIndex = 0 Then
                            strInfo &= "<tr><td colspan=""2"" align=""left""><span " & strAssignmentCss & " title=""" & strCoverageTooltip & """ >" & strCoverage & "</span></td>"
                        ElseIf intCoverageIndex = 1 Then
                            strInfo &= "<tr><td colspan=""@@colspan@@"" align=""left""><span " & strAssignmentCss & " title=""" & strCoverageTooltip & """ >" & strCoverage & "</span></td>"
                            strInfo &= "@@HiddenButton@@"
                        End If
                        strInfo &= "</tr>"
                    End If
                    strFullInfo &= "<tr><td align=""left""><span " & strAssignmentCss & " title=""" & strCoverageTooltip & """ >" & strCoverage & "</span></td></tr>"

                    intCoverageIndex += 1

                End If

            Next
        End If

        If intCoverageIndex > 2 Then
            strInfo = strInfo.Replace("@@colspan@@", "1")
            strInfo = strInfo.Replace("@@HiddenButton@@", "<td valign=""bottom"" align=""right"" >" &
                                                          "<table border=""0"" cellpadding=""0"" cellspacing=""0""><tr><td class=""aBtHiddenRow"" style=""overflow: hidden; width:15px;"" onmouseover=""ShowCoverageDetail('" & nameDivHd & "');"" onmouseout=""HideCoverageDetail();""></td></tr></table>" &
                                                          "</td>")
        Else
            strInfo = strInfo.Replace("@@colspan@@", "2")
            strInfo = strInfo.Replace("@@HiddenButton@@", "")
        End If

        If strTableCss.EndsWith("Alert") Then
            strInfo = strInfo.Replace("<span ", "<span style=""color: Black;"" ")
            strFullInfo = strFullInfo.Replace("<span ", "<span style=""color: Black;"" ")
        End If

        If strInfo = "" Then
            If oDailyCoverage.CoverageAssignments IsNot Nothing AndAlso oDailyCoverage.CoverageAssignments.Count > 0 Then
                Select Case CoverageView
                    Case 0
                        strCoverage = ""
                    Case 1
                        strCoverage = Me.Language.Translate("PlannedCoverage." & TypeView.ToString & ".Ok", Me.DefaultScope)
                        strCoverageTooltip = Me.Language.Translate("PlannedCoverage.Ok.Tooltip", Me.DefaultScope)
                    Case 2
                        strCoverage = Me.Language.Translate("ActualCoverage." & TypeView.ToString & ".Ok", Me.DefaultScope)
                        strCoverageTooltip = Me.Language.Translate("ActualCoverage.Ok.Tooltip", Me.DefaultScope)
                End Select
                If strCoverage <> "" Then
                    strInfo = "<tr><td colspan=""2"" align=""center""><span class=""PlannedCoverageDetailViewOk"" title=""" & strCoverageTooltip & """ >" & strCoverage & "</span></td>"
                End If
            Else
                strTableCss = ""
            End If
        End If

        strInfo = "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" >" & strInfo & "</table>"
        strFullInfo = "<table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" >" & strFullInfo & "</table>"

        Dim strDiv As String = ""
        If oDailyCoverage.CoverageAssignments IsNot Nothing AndAlso oDailyCoverage.CoverageAssignments.Count > 2 Then
            Dim strDivWidth As String = ""
            Select Case TypeView
                Case 2 : strDivWidth = "width: 118px;"
                Case 1 : strDivWidth = "width: 90px;"
                Case 0 : strDivWidth = "width: 50px;"
            End Select
            strDiv &= "<div id=""" & nameDivHd & """  style=""display: none;position: relative; margin-top: -42px;border: solid 2px #76859F; margin-left: 0px; " & strDivWidth & " "" "
            strDiv &= IIf(strTableCss <> "", "class=""" & strTableCss & """", "")
            strDiv &= " onmouseover=""ShowCoverageDetail('" & nameDivHd & "');"" onmouseout=""HideCoverageDetail();"""
            strDiv &= ">"
            strDiv &= strFullInfo
            strDiv &= "</div>"
        End If

        Dim strTypeView As String = ""
        If TypeView = 0 Then strTypeView = "RowTypeView0"
        If TypeView = 1 Then strTypeView = "RowTypeView1"
        If TypeView = 2 Then strTypeView = "RowTypeView2"

        strRet = "<a id=""" & IdGroup & "_" & Format(oDailyCoverage.CoverageDate, "dd/MM/yyyy") & "_" & TypeView.ToString & "_Coverage"" href=""javascript: void(0);"" class=""FixedHeaderCellUCoverage " & strTypeView & " CoverageMenu" & IIf(strTableCss <> "", " " & strTableCss, "") & """ onclick=""selectedCoverageU(this);"" " & strAttributes & " >" & strInfo & "</a>"
        strRet &= strDiv

        Return strRet

    End Function

    Private Function CreateDivHeader(ByVal IdGroup As Integer, ByVal xDate As Date, ByRef oDailyCoverage As roDailyCoverage, ByRef tbAssignments As DataTable) As String

        Dim strRet As String = ""

        Dim intTypeView As Integer = roTypes.Any2Integer(Request("TypeView"))                    'Tipo de Vista
        Dim bolShowCoverage As Boolean = (roTypes.Any2String(Request("ShowCoverage")).ToLower = "true") ' Mostrar u ocultar barra de dotaciones
        Dim CoverageView As Integer = roTypes.Any2Integer(Request("CoverageView"))                      ' Tipo de estado a mostrar en la barra de dotaciones (0- teórico, 1- planificado, 2- real)

        bolShowCoverage = (Me.bolHRScheduling And bolShowCoverage And IdGroup <> -1 And IdGroup <> -999) ' Sólo mostrar las dotaciones si hay un grupo actual en la selección 

        Dim strDates As String
        Dim strDt0 As String, strDt1 As String, strDt2 As String
        Dim strStDt0 As String, strStDt1 As String, strStDt2 As String

        'Filtrar format dates segons vista
        strDt0 = Format(xDate, "dddd").Substring(0, 3) & "<br/>" & Format(xDate, "dd") & vbCrLf
        strDt1 = Format(xDate, "dddd").Substring(0, 3) & "<br/>" & Format(xDate, HelperWeb.GetMonthAndDayDateFormat)
        strDt2 = Format(xDate, "dddd") & "<br/>" & Format(xDate, HelperWeb.GetShortDateFormat)

        strStDt0 = "<div class=""DateDet0"" style=""display:none;"">" & strDt0 & "</div>"
        strStDt1 = "<div class=""DateDet1"" style=""display:none;"">" & strDt1 & "</div>"
        strStDt2 = "<div class=""DateDet2"" style=""display:none;"">" & strDt2 & "</div>"

        Select Case intTypeView
            Case 2
                strStDt2 = "<div class=""DateDet2"" style=""display:;"">" & strDt2 & "</div>"
            Case 1
                strStDt1 = "<div class=""DateDet1"" style=""display:;"">" & strDt1 & "</div>"
            Case 0
                strStDt0 = "<div class=""DateDet0"" style=""display:;"">" & strDt0 & "</div>"
        End Select
        strDates = strStDt0 & strStDt1 & strStDt2


        Dim strCoverages As String
        Dim strCoverage0 As String, strCoverage1 As String, strCoverage2 As String
        Dim strStCoverage0 As String, strStCoverage1 As String, strStCoverage2 As String

        strCoverage0 = Me.CreateCoverageCell(IdGroup, oDailyCoverage, CoverageView, 0, tbAssignments)
        strCoverage1 = Me.CreateCoverageCell(IdGroup, oDailyCoverage, CoverageView, 1, tbAssignments)
        strCoverage2 = Me.CreateCoverageCell(IdGroup, oDailyCoverage, CoverageView, 2, tbAssignments)

        strStCoverage0 = "<div class=""DateDet0"" style=""display:none;"">" & strCoverage0 & "</div>"
        strStCoverage1 = "<div class=""DateDet1"" style=""display:none;"">" & strCoverage1 & "</div>"
        strStCoverage2 = "<div class=""DateDet2"" style=""display:none;"">" & strCoverage2 & "</div>"

        Select Case intTypeView
            Case 0
                strStCoverage0 = "<div class=""DateDet0"">" & strCoverage0 & "</div>"
            Case 1
                strStCoverage1 = "<div class=""DateDet1"" >" & strCoverage1 & "</div>"
            Case 2
                strStCoverage2 = "<div class=""DateDet2"" >" & strCoverage2 & "</div>"
        End Select
        strCoverages = strStCoverage0 & strStCoverage1 & strStCoverage2

        Dim strProcessingCoverageCss As String = ""
        If oDailyCoverage.CoverageAssignments IsNot Nothing Then
            Select Case CoverageView
                Case 1 ' Vista dotación planificada
                    For Each oDailyAssignment As roDailyCoverageAssignment In oDailyCoverage.CoverageAssignments
                        If oDailyAssignment.PlannedStatus <> 100 Then
                            strProcessingCoverageCss = "CoverageProcessing"
                            Exit For
                        End If
                    Next
                Case 2 ' Vista dotación real
                    If xDate.Date <= Now.Date Then
                        For Each oDailyAssignment As roDailyCoverageAssignment In oDailyCoverage.CoverageAssignments
                            If oDailyAssignment.ActualStatus <> 100 Then
                                strProcessingCoverageCss = "CoverageProcessing"
                                Exit For
                            End If
                        Next
                    End If
            End Select
        End If

        Dim strTypeView As String = ""
        Select Case intTypeView
            Case 0 : strTypeView = "RowTypeView0"
            Case 1 : strTypeView = "RowTypeView1"
            Case 2 : strTypeView = "RowTypeView2"
        End Select

        Dim strCssCoverageHeaderMenu As String = " CoverageHeaderMenu"
        If IdGroup = -1 Or IdGroup = -999 Then strCssCoverageHeaderMenu = ""

        strRet = "<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""height: 100%;"">" &
                     "<tr><td><a href=""javascript: void(0);"" class=""FixedHeaderCellU " & strTypeView & strCssCoverageHeaderMenu & IIf(strProcessingCoverageCss = "", """", IIf(bolShowCoverage, " " & strProcessingCoverageCss & """", """") & " CoverageProcessingCss=""" & strProcessingCoverageCss & """ ") & " >" & strDates & "</a></td></tr>" &
                     "<tr Content=""CoverageResume"" style=""display: " & IIf(bolShowCoverage, "", "none") & ";"">" &
                        "<td id=""td_" & IdGroup & "_" & Format(oDailyCoverage.CoverageDate, "dd/MM/yyyy") & """>" & strCoverages & "</td>" &
                     "</tr>" &
                     "</table>"

        Return strRet

    End Function

    Private Sub getDayHeaderPlan()

        Dim strResponse As String = ""

        Try

            Dim IDGroup As Integer = roTypes.Any2Integer(Request("ID"))
            Dim xDate As Date = convertDate(Request("Date"))

            Dim oDailyCoverage As roDailyCoverage = API.SchedulerServiceMethods.GetDailyCoverage(Nothing, IDGroup, xDate, False)
            Dim tbAssignments As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Nothing, "", False)

            strResponse = "OK" & Me.CreateDivHeader(IDGroup, xDate, oDailyCoverage, tbAssignments)

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub

    Private Sub getDayHeadersPlan()

        Dim strResponse As String = ""

        Try

            Dim IDGroup As Integer = roTypes.Any2Integer(Request("ID"))

            CalculateDaysPeriods()

            Dim lstDailyCoverages As Generic.List(Of roDailyCoverage) = API.SchedulerServiceMethods.GetDailyCoverages(Nothing, IDGroup, mDateStart, mDateEnd, False)
            Dim tbAssignments As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Nothing, "", False)

            Dim arrCells As New Generic.List(Of SchedulerHeaderCell)
            Dim oSchedulerHeaderCell As SchedulerHeaderCell

            For Each oDailyCoverage As roDailyCoverage In lstDailyCoverages
                oSchedulerHeaderCell = New SchedulerHeaderCell(oDailyCoverage.IDGroup, oDailyCoverage.CoverageDate, Me.CreateDivHeader(IDGroup, oDailyCoverage.CoverageDate, oDailyCoverage, tbAssignments))
                arrCells.Add(oSchedulerHeaderCell)
            Next

            strResponse = "OK" & roJSONHelper.Serialize(arrCells)

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub

    Private Sub DeleteTeoricCoverage()

        Dim strResponse As String = ""

        Try

            Dim IDGroup As Integer = roTypes.Any2Integer(Request("ID"))
            Dim xDate As Date = convertDate(Request("Date"))

            Dim bolRet As Boolean = API.SchedulerServiceMethods.DeleteDailyCoverage(Nothing, IDGroup, xDate)
            If bolRet Then

                Dim oDailyCoverage As roDailyCoverage = API.SchedulerServiceMethods.GetDailyCoverage(Nothing, IDGroup, xDate, False)
                Dim tbAssignments As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Nothing, "", False)

                Dim arrCells As New Generic.List(Of SchedulerHeaderCell)
                Dim oSchedulerHeaderCell As SchedulerHeaderCell

                oSchedulerHeaderCell = New SchedulerHeaderCell(oDailyCoverage.IDGroup, oDailyCoverage.CoverageDate, Me.CreateDivHeader(IDGroup, oDailyCoverage.CoverageDate, oDailyCoverage, tbAssignments))
                arrCells.Add(oSchedulerHeaderCell)

                strResponse = "OK" & roJSONHelper.Serialize(arrCells)

            Else
                strResponse = roWsUserManagement.SessionObject.States.SchedulerState.ErrorText
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub

    Private Sub getEmployeeShiftAssignments()

        Dim strResponse As String = ""

        Try

            Dim intIDEmployee As Integer = roTypes.Any2Integer(Request("IDEmployee"))
            Dim intIDShift As Integer = roTypes.Any2Integer(Request("IDShift"))

            If intIDEmployee > 0 Then
                Dim oAssignments As Generic.List(Of roAssignment) = API.SchedulerServiceMethods.GetEmployeAndShiftAssignments(Nothing, intIDEmployee, intIDShift)
                If roWsUserManagement.SessionObject.States.SchedulerState.Result = SchedulerResultEnum.NoError Then

                    strResponse = "OK"
                    If oAssignments.Count > 0 Then
                        For Each oAssignment As roAssignment In oAssignments
                            strResponse &= oAssignment.ID.ToString & "*" & oAssignment.Name & "~"
                        Next
                        strResponse = strResponse.Substring(0, strResponse.Length - 1)
                        ''Else
                        ''    strResponse = "ERROR: " & Me.Language.Translate("EmployeAndShiftAssignments.NoAssignments", Me.DefaultScope)
                    End If

                End If

            Else

                Dim oShift As roShift = API.ShiftServiceMethods.GetShift(Nothing, intIDShift, False)
                If roWsUserManagement.SessionObject.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If oShift.Assignments IsNot Nothing Then

                        Dim tbAssignments As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Nothing, "", False)
                        Dim oRows() As DataRow

                        strResponse = "OK"
                        If oShift.Assignments.Count > 0 Then
                            For Each oAssignment As roShiftAssignment In oShift.Assignments
                                oRows = tbAssignments.Select("ID = " & oAssignment.IDAssignment.ToString, "")
                                If oRows.Length = 1 Then
                                    strResponse &= oAssignment.IDAssignment.ToString & "*" & roTypes.Any2String(oRows(0).Item("Name")) & "~"
                                End If
                            Next
                            strResponse = strResponse.Substring(0, strResponse.Length - 1)
                        End If

                    End If
                End If

            End If

            If strResponse = "" Then
                strResponse = roWsUserManagement.SessionObject.States.SchedulerState.ErrorText
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)

    End Sub





    Private Sub RemoveEmployeeHolidays()
        Dim strResponse As String = ""
        Try
            Dim IDEmployee As Integer = Request("IDEmployee")
            Dim MsgResponseCovered As String = Request("MsgResponseCovered")
            Dim MsgResponseCoverage As String = Request("MsgResponseCoverage")
            Dim MsgResponsePermission As String = Request("MsgResponsePermission")

            Dim iResponseCovered As LockedDayAction = RetMsgResponse(MsgResponseCovered)
            Dim iResponseCoverage As LockedDayAction = RetMsgResponse(MsgResponseCoverage)
            Dim iResponsePermission As ShiftPermissionAction = RetMsgResponseShift(MsgResponsePermission)

            Dim arrDate() As String = Request("DestDate").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            Dim strCellsAssign As String = Request("cellsSelected")

            Dim intEmployeeType As Integer = 0
            Dim intIDEmployeeLocked As Integer = -1
            Dim oEmployee As roEmployee = Nothing

            Dim runningDate As String = ""

            Dim bContinue As Boolean = True
            If (strCellsAssign = String.Empty) Then
                Dim dtPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Nothing, IDEmployee, dDate)
                runningDate = Format(dDate, HelperWeb.GetShortDateFormat())
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

                    oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IDEmployee, False)
                    If API.EmployeeServiceMethods.AssignShift(Nothing, IDEmployee, dDate, -1, IDShift2, IDShift3, IDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                                      IDAssignment, iResponseCovered, iResponseCoverage, True, iResponsePermission) Then
                        strResponse = "OK"
                    Else
                        bContinue = False
                    End If
                End If

            Else
                Dim Cells() As String = strCellsAssign.Split(",")

                If iResponsePermission <> ShiftPermissionAction.StopAll Then
                    For Each Cell As String In Cells
                        If Not ProcessedCells.Contains(Cell) Then
                            Dim strEmployee As String = Cell.Split("_")(0)
                            Dim strDate As String = Cell.Split("_")(1)

                            arrDate = strDate.Split("/")
                            dDate = New Date(arrDate(2), arrDate(1), arrDate(0))
                            runningDate = Format(dDate, HelperWeb.GetShortDateFormat())

                            Dim dtPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Nothing, roTypes.Any2Integer(strEmployee), dDate)

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

                                oEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IDEmployee, False)
                                If API.EmployeeServiceMethods.AssignShift(Nothing, roTypes.Any2Integer(strEmployee), dDate, -1, IDShift2, IDShift3, IDShift4, xStartShift1, xStartShift2, xStartShift3, xStartShift4,
                                                                          IDAssignment, iResponseCovered, iResponseCoverage, True, iResponsePermission) Then
                                    strResponse = "OK"
                                    If ProcessedCells <> String.Empty Then Me.ProcessedCells = Me.ProcessedCells & ","
                                    Me.ProcessedCells = Me.ProcessedCells & Cell
                                    If iResponseCovered <> LockedDayAction.None Then
                                        If iResponseCovered <> LockedDayAction.NoReplaceAll And iResponseCovered <> LockedDayAction.ReplaceAll Then
                                            iResponseCovered = LockedDayAction.None
                                        End If
                                    End If
                                    If iResponseCoverage <> LockedDayAction.None Then
                                        If iResponseCoverage <> LockedDayAction.NoReplaceAll And iResponseCoverage <> LockedDayAction.ReplaceAll Then
                                            iResponseCoverage = LockedDayAction.None
                                        End If
                                    End If
                                    If iResponsePermission <> ShiftPermissionAction.None Then
                                        If iResponsePermission <> ShiftPermissionAction.ContinueAll And iResponsePermission <> ShiftPermissionAction.StopAll Then
                                            iResponsePermission = ShiftPermissionAction.None
                                        End If
                                    End If
                                Else
                                    bContinue = False
                                    intIDEmployeeLocked = roTypes.Any2Integer(strEmployee)
                                    If Not bContinue Then Exit For
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            If Not bContinue Then


                If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                    If (strCellsAssign <> String.Empty) Then
                        strResponse = "STOP01"
                    Else
                        strResponse = "STOP00"
                    End If
                    If oEmployee IsNot Nothing Then strResponse &= oEmployee.Name
                    strResponse = strResponse & "_" & runningDate
                ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                    If (strCellsAssign <> String.Empty) Then
                        strResponse = "STOP11"
                    Else
                        strResponse = "STOP10"
                    End If
                    If oEmployee IsNot Nothing Then strResponse &= oEmployee.Name
                    strResponse = strResponse & "_" & runningDate
                ElseIf roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Then
                    If (strCellsAssign <> String.Empty) Then
                        strResponse = "STOP21"
                    Else
                        strResponse = "STOP20"
                    End If
                    If oEmployee IsNot Nothing Then strResponse &= oEmployee.Name
                    strResponse = strResponse & "_" & runningDate
                Else
                    strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                End If
            Else
                Me.ProcessedCells = ""
            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    Private Sub RemoveEmployeeCoverage()
        Dim strResponse As String = ""
        Try
            Dim IDEmployee As Integer = Request("IDEmployee")
            Dim MsgResponseCovered As String = Request("MsgResponseCovered")
            Dim MsgResponseCoverage As String = Request("MsgResponseCoverage")

            Dim iResponseCovered As LockedDayAction = RetMsgResponse(MsgResponseCovered)
            Dim iResponseCoverage As LockedDayAction = RetMsgResponse(MsgResponseCoverage)

            Dim arrDate() As String = Request("DestDate").Split("/")
            Dim dDate As Date = New Date(arrDate(2), arrDate(1), arrDate(0))

            Dim intEmployeeType As Integer = 0
            Dim intIDEmployeeLocked As Integer = -1

            If API.EmployeeServiceMethods.RemoveEmployeeCoverage(Nothing, IDEmployee, dDate, iResponseCovered, iResponseCoverage, intEmployeeType, intIDEmployeeLocked, True) Then
                strResponse = "OK"
            Else
                If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                    If intEmployeeType = 0 Then
                        If IDEmployee = intIDEmployeeLocked Then
                            strResponse = "STOP0"
                        Else
                            strResponse = "STOP1"
                        End If
                    Else
                        If IDEmployee = intIDEmployeeLocked Then
                            strResponse = "STOP1"
                        Else
                            strResponse = "STOP0"
                        End If
                    End If
                    If intIDEmployeeLocked > 0 Then
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, intIDEmployeeLocked, False)
                        If oEmployee IsNot Nothing Then strResponse &= oEmployee.Name
                    End If
                Else
                    strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                End If
            End If
        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    Private Function GetHexColor(ByVal colorObj As System.Drawing.Color) As String
        Return "#" & Hex(colorObj.R).PadLeft(2, "0") & Hex(colorObj.G).PadLeft(2, "0") & Hex(colorObj.B).PadLeft(2, "0")
    End Function

    Private Sub pasteTeoricCoverage()
        Dim strResponse As String = ""
        Try
            Dim IdGroup As Integer = Request("IDGroup")
            Dim strListCoverages() As String = Request("Coverages").Split("|") 'IDAssignment~ExpectedCoverage*IDAssignment~ExpectedCoverage*...|IDAssignment~ExpectedCoverage*IDAssignment~ExpectedCoverage*...|...
            Dim arrDate() As String = Request("DestDate").Split("/")
            Dim xDestDate As DateTime = New DateTime(arrDate(2), arrDate(1), arrDate(0))
            Dim xDestDateEnd As DateTime = Nothing

            If roTypes.Any2String(Request("DestDateEnd")) <> "" Then
                arrDate = Request("DestDateEnd").Split("/")
                xDestDateEnd = New DateTime(arrDate(2), arrDate(1), arrDate(0))
            End If

            If strListCoverages IsNot Nothing AndAlso strListCoverages.Length > 0 Then

                If xDestDateEnd = Nothing Then
                    xDestDateEnd = xDestDate.AddDays(strListCoverages.Length - 1)
                End If

                Dim tbAssignments As DataTable = API.AssignmentServiceMethods.GetAssignmentsDataTable(Nothing, "", False)

                Dim arrCells As New Generic.List(Of SchedulerHeaderCell)
                Dim oSchedulerHeaderCell As SchedulerHeaderCell

                Dim xDateAct As DateTime = xDestDate
                Dim intCoverageIndex As Integer = 0
                Dim strListAssignments() As String

                While xDateAct <= xDestDateEnd

                    strListAssignments = strListCoverages(intCoverageIndex).Split("*") ' IDAssignment~ExpectedCoverage*...

                    Dim oDailyCoverage As New roDailyCoverage
                    oDailyCoverage.IDGroup = IdGroup
                    oDailyCoverage.CoverageDate = xDateAct
                    oDailyCoverage.CoverageAssignments = New List(Of roDailyCoverageAssignment)


                    Dim intIDAssignment As Integer = 0
                    Dim dblExpectedCoverage As Double = 0
                    Dim bolAssignments As Boolean = False


                    For n As Integer = 0 To strListAssignments.Length - 1
                        If strListAssignments(n) <> "" AndAlso strListAssignments(n).Split("~").Length > 1 Then
                            intIDAssignment = roTypes.Any2Integer(strListAssignments(n).Split("~")(0))
                            If IsNumeric(strListAssignments(n).Split("~")(1).Replace(".", HelperWeb.GetDecimalDigitFormat())) Then
                                dblExpectedCoverage = CDbl(strListAssignments(n).Split("~")(1).Replace(".", HelperWeb.GetDecimalDigitFormat()))
                            Else
                                dblExpectedCoverage = 0
                            End If
                            If intIDAssignment > 0 Then
                                Dim tmpCoverage As New roDailyCoverageAssignment
                                tmpCoverage.IDGroup = IdGroup
                                tmpCoverage.IDAssignment = intIDAssignment
                                tmpCoverage.ExpectedCoverage = dblExpectedCoverage
                                oDailyCoverage.CoverageAssignments.Add(tmpCoverage)
                                bolAssignments = True
                            End If
                        End If
                    Next

                    If bolAssignments Then
                        If API.SchedulerServiceMethods.SaveTeoricDailyCoverage(Nothing, oDailyCoverage) Then

                            oSchedulerHeaderCell = New SchedulerHeaderCell(oDailyCoverage.IDGroup, oDailyCoverage.CoverageDate, Me.CreateDivHeader(IdGroup, oDailyCoverage.CoverageDate, oDailyCoverage, tbAssignments))
                            arrCells.Add(oSchedulerHeaderCell)

                        Else
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.SchedulerState.ErrorText
                            Exit While
                        End If
                    Else

                        If API.SchedulerServiceMethods.DeleteDailyCoverage(Nothing, oDailyCoverage.IDGroup, oDailyCoverage.CoverageDate) Then
                        Else
                            strResponse = Robotics.Web.Base.roWsUserManagement.SessionObject.States.SchedulerState.ErrorText
                            Exit While
                        End If
                    End If

                    xDateAct = xDateAct.AddDays(1)
                    intCoverageIndex = (intCoverageIndex + 1) Mod strListCoverages.Length

                End While

                If strResponse = "" Then
                    strResponse = "OK" & roJSONHelper.Serialize(arrCells)
                End If

            End If

        Catch ex As Exception
            strResponse = "ERROR: " & ex.Message & " " & ex.StackTrace
        End Try

        Response.Write(strResponse)
    End Sub

    <DataContract()>
    Public Class SchedulerCellMessage
        Private m_IDEmployee As Integer
        Private m_EmployeeName As String
        Private m_DateDest As String
        Private m_Render As Generic.List(Of SchedulerCell)
        Private m_Message As String

        Public Sub New()
        End Sub

        Public Sub New(ByVal xMessage As String,
                        ByVal xIDEmployee As Integer,
                        ByVal xEmployeeName As String,
                        ByVal xDateDest As String,
                        ByVal xRender As Generic.List(Of SchedulerCell))
            m_Message = xMessage
            m_IDEmployee = xIDEmployee
            m_EmployeeName = xEmployeeName
            m_DateDest = xDateDest
            m_Render = xRender
        End Sub

        <DataMember(Name:="Employee")>
        Public Property IDEmployee() As Integer
            Get
                Return m_IDEmployee
            End Get
            Set(ByVal value As Integer)
                m_IDEmployee = value
            End Set
        End Property

        <DataMember(Name:="Date")>
        Public Property Datedest() As String
            Get
                Return m_DateDest
            End Get
            Set(ByVal value As String)
                m_DateDest = value
            End Set
        End Property

        <DataMember(Name:="Render")>
        Public Property Render() As Generic.List(Of SchedulerCell)
            Get
                Return m_Render
            End Get
            Set(ByVal value As Generic.List(Of SchedulerCell))
                m_Render = value
            End Set
        End Property

        <DataMember(Name:="Message")>
        Public Property Message() As String
            Get
                Return m_Message
            End Get
            Set(ByVal value As String)
                m_Message = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class SchedulerCell
        Private m_IDEmployee As Integer
        Private m_DateDest As String
        Private m_InnerHTML As String

        Private m_IDShift1 As Integer
        Private m_IDShift2 As Integer
        Private m_IDShift3 As Integer
        Private m_IDShift4 As Integer

        Private m_StartShift1 As DateTime
        Private m_StartShift2 As DateTime
        Private m_StartShift3 As DateTime
        Private m_StartShift4 As DateTime

        Private m_ShiftColor1 As String
        Private m_ShiftColor2 As String
        Private m_ShiftColor3 As String
        Private m_ShiftColor4 As String

        Private m_Locked As Boolean

        Private m_IDAssignment As Integer

        Private m_Gradient As Boolean
        Private m_ColorFrom As String
        Private m_ColorTo As String

        Private m_CoverageIDEmployee As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal xIDEmployee As Integer, ByVal xDateDest As String, ByVal xInnerHTML As String)
            m_IDEmployee = xIDEmployee
            m_DateDest = xDateDest
            m_InnerHTML = xInnerHTML
        End Sub

        <DataMember(Name:="IDEmployee")>
        Public Property IDEmployee() As Integer
            Get
                Return m_IDEmployee
            End Get
            Set(ByVal value As Integer)
                m_IDEmployee = value
            End Set
        End Property

        <DataMember(Name:="DateDest")>
        Public Property Datedest() As String
            Get
                Return m_DateDest
            End Get
            Set(ByVal value As String)
                m_DateDest = value
            End Set
        End Property

        <DataMember(Name:="IDShift1")>
        Public Property IDShift1() As Integer
            Get
                Return m_IDShift1
            End Get
            Set(ByVal value As Integer)
                m_IDShift1 = value
            End Set
        End Property

        <DataMember(Name:="IDShift2")>
        Public Property IDShift2() As Integer
            Get
                Return m_IDShift2
            End Get
            Set(ByVal value As Integer)
                m_IDShift2 = value
            End Set
        End Property
        <DataMember(Name:="IDShift3")>
        Public Property IDShift3() As Integer
            Get
                Return m_IDShift3
            End Get
            Set(ByVal value As Integer)
                m_IDShift3 = value
            End Set
        End Property
        <DataMember(Name:="IDShift4")>
        Public Property IDShift4() As Integer
            Get
                Return m_IDShift4
            End Get
            Set(ByVal value As Integer)
                m_IDShift4 = value
            End Set
        End Property

        Public Property StartShift1() As DateTime
            Get
                Return Me.m_StartShift1
            End Get
            Set(ByVal value As DateTime)
                Me.m_StartShift1 = value
            End Set
        End Property
        Public Property StartShift2() As DateTime
            Get
                Return Me.m_StartShift2
            End Get
            Set(ByVal value As DateTime)
                Me.m_StartShift2 = value
            End Set
        End Property
        Public Property StartShift3() As DateTime
            Get
                Return Me.m_StartShift3
            End Get
            Set(ByVal value As DateTime)
                Me.m_StartShift3 = value
            End Set
        End Property
        Public Property StartShift4() As DateTime
            Get
                Return Me.m_StartShift4
            End Get
            Set(ByVal value As DateTime)
                Me.m_StartShift4 = value
            End Set
        End Property

        <DataMember(Name:="StartShift1String")>
        Public Property StartShift1String() As String
            Get
                If Me.m_StartShift1 <> Nothing Then
                    Return Format(Me.m_StartShift1, "yyyyMMddHHmm")
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" AndAlso value.Length >= 12 Then
                    Me.m_StartShift1 = New DateTime(CInt(value.Substring(0, 4)), CInt(value.Substring(4, 2)), CInt(value.Substring(6, 2)), CInt(value.Substring(8, 2)), CInt(value.Substring(10, 2)), 0)
                Else
                    Me.m_StartShift1 = Nothing
                End If
            End Set
        End Property
        <DataMember(Name:="StartShift2String")>
        Public Property StartShift2String() As String
            Get
                If Me.m_StartShift2 <> Nothing Then
                    Return Format(Me.m_StartShift2, "yyyyMMddHHmm")
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" AndAlso value.Length >= 12 Then
                    Me.m_StartShift2 = New DateTime(CInt(value.Substring(0, 4)), CInt(value.Substring(4, 2)), CInt(value.Substring(6, 2)), CInt(value.Substring(8, 2)), CInt(value.Substring(10, 2)), 0)
                Else
                    Me.m_StartShift2 = Nothing
                End If
            End Set
        End Property
        <DataMember(Name:="StartShift3String")>
        Public Property StartShift3String() As String
            Get
                If Me.m_StartShift3 <> Nothing Then
                    Return Format(Me.m_StartShift3, "yyyyMMddHHmm")
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" AndAlso value.Length >= 12 Then
                    Me.m_StartShift3 = New DateTime(CInt(value.Substring(0, 4)), CInt(value.Substring(4, 2)), CInt(value.Substring(6, 2)), CInt(value.Substring(8, 2)), CInt(value.Substring(10, 2)), 0)
                Else
                    Me.m_StartShift3 = Nothing
                End If
            End Set
        End Property
        <DataMember(Name:="StartShift4String")>
        Public Property StartShift4String() As String
            Get
                If Me.m_StartShift4 <> Nothing Then
                    Return Format(Me.m_StartShift4, "yyyyMMddHHmm")
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                If value <> "" AndAlso value.Length >= 12 Then
                    Me.m_StartShift4 = New DateTime(CInt(value.Substring(0, 4)), CInt(value.Substring(4, 2)), CInt(value.Substring(6, 2)), CInt(value.Substring(8, 2)), CInt(value.Substring(10, 2)), 0)
                Else
                    Me.m_StartShift4 = Nothing
                End If
            End Set
        End Property

        <DataMember(Name:="ShiftColor1")>
        Public Property ShiftColor1() As String
            Get
                Return m_ShiftColor1
            End Get
            Set(ByVal value As String)
                m_ShiftColor1 = value
            End Set
        End Property

        <DataMember(Name:="ShiftColor2")>
        Public Property ShiftColor2() As String
            Get
                Return m_ShiftColor2
            End Get
            Set(ByVal value As String)
                m_ShiftColor2 = value
            End Set
        End Property

        <DataMember(Name:="ShiftColor3")>
        Public Property ShiftColor3() As String
            Get
                Return m_ShiftColor3
            End Get
            Set(ByVal value As String)
                m_ShiftColor3 = value
            End Set
        End Property
        <DataMember(Name:="ShiftColor4")>
        Public Property ShiftColor4() As String
            Get
                Return m_ShiftColor4
            End Get
            Set(ByVal value As String)
                m_ShiftColor4 = value
            End Set
        End Property

        <DataMember(Name:="Locked")>
        Public Property Locked() As Boolean
            Get
                Return m_Locked
            End Get
            Set(ByVal value As Boolean)
                m_Locked = value
            End Set
        End Property

        <DataMember(Name:="IDAssignment")>
        Public Property IDAssignment() As Integer
            Get
                Return m_IDAssignment
            End Get
            Set(ByVal value As Integer)
                m_IDAssignment = value
            End Set
        End Property

        <DataMember(Name:="Gradient")>
        Public Property Gradient() As Boolean
            Get
                Return m_Gradient
            End Get
            Set(ByVal value As Boolean)
                m_Gradient = value
            End Set
        End Property

        <DataMember(Name:="ColorFrom")>
        Public Property ColorFrom() As String
            Get
                Return m_ColorFrom
            End Get
            Set(ByVal value As String)
                m_ColorFrom = value
            End Set
        End Property

        <DataMember(Name:="ColorTo")>
        Public Property ColorTo() As String
            Get
                Return m_ColorTo
            End Get
            Set(ByVal value As String)
                m_ColorTo = value
            End Set
        End Property

        <DataMember(Name:="CoverageIDEmployee")>
        Public Property CoverageIDEmployee() As Integer
            Get
                Return m_CoverageIDEmployee
            End Get
            Set(ByVal value As Integer)
                m_CoverageIDEmployee = value
            End Set
        End Property

        <DataMember(Name:="TextHTML")>
        Public Property InnerHTML() As String
            Get
                Return m_InnerHTML
            End Get
            Set(ByVal value As String)
                m_InnerHTML = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class SchedulerPeriod
        Private mDateCaption As String
        Private mSelectorType As String
        Private mSelectorInit As String
        Private mSelectorDatePos As String
        Private mOtherType As String
        Private mStartDate As String
        Private mEndDate As String
        Private mToDays As String

        Public Sub New()

        End Sub

        Public Sub New(ByVal xDateCaption As String, ByVal xSelectorType As String, ByVal xSelectorInit As String, ByVal xSelectorDatePos As String, ByVal xOtherType As String,
                       ByVal xStartDate As String, ByVal xEndDate As String, ByVal xToDays As String)
            mDateCaption = xDateCaption
            mSelectorType = xSelectorType
            mSelectorInit = xSelectorInit
            mSelectorDatePos = xSelectorDatePos
            mOtherType = xOtherType
            mStartDate = xStartDate
            mEndDate = xEndDate
            mToDays = xToDays
        End Sub

        <DataMember()>
        Public Property DateCaption() As String
            Get
                Return mDateCaption
            End Get
            Set(ByVal value As String)
                mDateCaption = value
            End Set
        End Property

        <DataMember()>
        Public Property SelectorType() As String
            Get
                Return mSelectorType
            End Get
            Set(ByVal value As String)
                mSelectorType = value
            End Set
        End Property
        <DataMember()>
        Public Property SelectorInit() As String
            Get
                Return mSelectorInit
            End Get
            Set(ByVal value As String)
                mSelectorInit = value
            End Set
        End Property
        <DataMember()>
        Public Property SelectorDatePos() As String
            Get
                Return mSelectorDatePos
            End Get
            Set(ByVal value As String)
                mSelectorDatePos = value
            End Set
        End Property
        <DataMember()>
        Public Property OtherType() As String
            Get
                Return mOtherType
            End Get
            Set(ByVal value As String)
                mOtherType = value
            End Set
        End Property
        <DataMember()>
        Public Property StartDate() As String
            Get
                Return mStartDate
            End Get
            Set(ByVal value As String)
                mStartDate = value
            End Set
        End Property
        <DataMember()>
        Public Property EndDate() As String
            Get
                Return mEndDate
            End Get
            Set(ByVal value As String)
                mEndDate = value
            End Set
        End Property
        <DataMember()>
        Public Property ToDays() As String
            Get
                Return mToDays
            End Get
            Set(ByVal value As String)
                mToDays = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class SchedulerHeaderCell

        Private m_IDGroup As Integer
        Private m_Date As String
        Private m_InnerHTML As String

        Public Sub New(ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _InnerHTML As String)
            m_IDGroup = _IDGroup
            m_Date = Format(_Date, "dd/MM/yyyy")
            m_InnerHTML = _InnerHTML
        End Sub

        <DataMember(Name:="IDGroup")>
        Public Property IDGroup() As Integer
            Get
                Return m_IDGroup
            End Get
            Set(ByVal value As Integer)
                m_IDGroup = value
            End Set
        End Property

        <DataMember(Name:="HeaderDate")>
        Public Property HeaderDate() As String
            Get
                Return Me.m_Date
            End Get
            Set(ByVal value As String)
                Me.m_Date = value
            End Set
        End Property

        <DataMember(Name:="TextHTML")>
        Public Property InnerHTML() As String
            Get
                Return m_InnerHTML
            End Get
            Set(ByVal value As String)
                m_InnerHTML = value
            End Set
        End Property

    End Class

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
