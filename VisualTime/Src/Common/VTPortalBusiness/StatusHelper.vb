Imports System.ComponentModel
Imports System.Security.Cryptography
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTChannels
Imports Robotics.Base.VTCommuniques
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTRequests
Imports Robotics.Base.VTSurveys
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTPortal

    Public Class StatusHelper

        Public Shared NULL_DATE As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        Public Shared Function GetStatusFromState(ByVal oState As SecurityResultEnum) As Integer
            Dim result As Integer = ErrorCodes.OK

            Select Case oState
                Case SecurityResultEnum.BloquedAccessApp
                    result = ErrorCodes.LOGIN_BLOCKED_ACCESS_APP
                Case SecurityResultEnum.TemporayBloqued
                    result = ErrorCodes.LOGIN_TEMPORANY_BLOQUED
                Case SecurityResultEnum.GeneralBlockAccess
                    result = ErrorCodes.LOGIN_GENERAL_BLOCK_ACCESS
                Case SecurityResultEnum.InvalidClientLocation
                    result = ErrorCodes.LOGIN_INVALID_CLIENT_LOCATION
                Case SecurityResultEnum.InvalidVersionAPP
                    result = ErrorCodes.LOGIN_INVALID_VERSION_APP
                Case SecurityResultEnum.InvalidApp
                    result = ErrorCodes.LOGIN_INVALID_APP
                Case SecurityResultEnum.ServerStopped
                    result = ErrorCodes.SERVER_NOT_RUNNING
                Case SecurityResultEnum.SecurityTokenNotValid
                    result = ErrorCodes.GENERAL_ERROR_InvalidSecurityToken
                Case SecurityResultEnum.MaxCurrentSessionsExceeded
                    result = ErrorCodes.GENERAL_ERROR_MaxCurrentSessionsExceeded

            End Select

            Return result
        End Function

        Public Shared Function GetCurrentTerminalDatetime(ByVal oTerminal As Terminal.roTerminal, ByVal clientTimeZone As TimeZoneInfo) As DateTime
            Try
                Dim punchDateTime As Date = oTerminal.GetCurrentDateTime()
                If roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("MultiTimeZoneEnabled", New AdvancedParameter.roAdvancedParameterState).Value) Then
                    punchDateTime = TimeZoneInfo.ConvertTime(punchDateTime, clientTimeZone)
                End If

                Return punchDateTime
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetCurrentTerminalDatetime")

                Return Date.Now
            End Try
        End Function

        Public Shared Function GetEmployeesCurrentStatus(ByVal idPassport As Integer) As DataTable
            Dim employeeResult As DataTable = Nothing
            Dim oLogState As New roBusinessState("Common.BaseState", "")

            Try
                employeeResult = roBusinessSupport.GetEmployeesStatus(idPassport, oLogState)

                Return employeeResult
            Catch ex As Exception
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeesCurrentStatus")
                Return employeeResult
            End Try
        End Function

        Public Shared Function UpdateStatus(ByVal oEmployee As Employee.roEmployee, ByVal oTerminal As Terminal.roTerminal, ByVal clientTimeZone As TimeZoneInfo) As DataTable
            Try
                Dim dReturn As New DataTable("EmployeeStatus")
                dReturn.Columns.Add("EmployeeName", GetType(String))
                dReturn.Columns.Add("ServerDateTime", GetType(Date))
                dReturn.Columns.Add("PunchDateTime", GetType(Date))
                dReturn.Columns.Add("PresenceStatus", GetType(String))
                dReturn.Columns.Add("PunchStatus", GetType(String))
                dReturn.Columns.Add("LastPunchDateTime", GetType(Date))
                dReturn.Columns.Add("LastPunch", GetType(Object))
                dReturn.Columns.Add("DifferenceDate", GetType(Date))

                Dim dRow As DataRow = dReturn.NewRow

                dRow("EmployeeName") = oEmployee.Name
                Dim punchDateTime As Date = oTerminal.GetCurrentDateTime()
                dRow("ServerDateTime") = punchDateTime

                If roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("MultiTimeZoneEnabled", New AdvancedParameter.roAdvancedParameterState).Value) Then
                    punchDateTime = TimeZoneInfo.ConvertTime(punchDateTime, clientTimeZone)
                End If
                dRow("PunchDateTime") = punchDateTime
                Dim oPunchStatus As PunchStatus
                Dim oPresenceStatus As PresenceStatus
                Dim oLastPunchDateTime As Date
                Dim oLastPunch As New Punch.roPunch
                Dim oPresenceMinutes As Integer

                oPresenceStatus = VTBusiness.Scheduler.roScheduler.GetPresenceStatusEx(oEmployee.ID, DateTime.Now, oPunchStatus, oLastPunchDateTime, oLastPunch, oPresenceMinutes, New Employee.roEmployeeState)

                'Calcul de diferencies de temps
                Select Case oPresenceStatus
                    Case PresenceStatus.Inside
                        dRow("PresenceStatus") = "Inside"
                    Case PresenceStatus.Outside
                        dRow("PresenceStatus") = "Outside"
                End Select
                If oLastPunch.ID = -1 Then
                    dRow("LastPunchDateTime") = dRow("ServerDateTime")
                Else
                    dRow("LastPunchDateTime") = PunchHelper.CorrectPunchDateTime(oLastPunch)
                End If
                dRow("LastPunch") = oLastPunch
                Select Case oPunchStatus
                    Case PunchStatus.In_
                        dRow("PunchStatus") = "In"
                    Case PunchStatus.Indet_
                        dRow("PunchStatus") = "Indet"
                    Case PunchStatus.Out_
                        dRow("PunchStatus") = "Out"
                End Select
                dReturn.Rows.Add(dRow)
                Return dReturn
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::UpdateStatus")

                Return Nothing
            End Try
        End Function

        Public Shared Function PunchesEnabled(ByVal idPassport As Integer, ByVal terminal As Terminal.roTerminal) As Boolean
            Dim bolRet As Boolean = False
            Try
                If terminal IsNot Nothing AndAlso terminal.Readers.Count > 0 Then
                    ' Verificamos que el comportamiento del lector sea de presencia
                    Dim oReader As Terminal.roTerminal.roTerminalReader = terminal.Readers(0)
                    ' Verificamos que el comportamiento del lector sea de presencia
                    bolRet = (oReader.ScopeMode.ToString().IndexOf("TA") >= 0)

                    If bolRet Then
                        Dim oParameters As New roParameters("OPTIONS")
                        Dim sOffline As String
                        sOffline = roTypes.Any2String(oParameters.Parameter(Parameters.CommsOffLine))
                        bolRet = (sOffline <> "1")
                    End If
                End If
                Return (bolRet AndAlso SecurityHelper.GetFeaturePermission(idPassport, "Punches.Punches", "E") > Permission.Read)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::PunchesEnabled")

                Return False
            End Try
        End Function

        Public Shared Function GetSupervisorStatus(ByVal oPassportTicket As roPassportTicket, ByVal timeZone As TimeZoneInfo, ByVal bOnlyNotifications As Boolean) As UserStatus
            Dim lrret As New UserStatus

            Try

                lrret.LastPunchId = -1
                Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(-1, "LIVEPORTAL", New Terminal.roTerminalState(oPassportTicket.ID))
                Dim dServerDateTime As Date = DateTime.Now
                Dim oTerminal As Terminal.roTerminal = Nothing

                If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                    dServerDateTime = oTerminals(0).GetCurrentDateTime()
                    oTerminal = oTerminals(0)
                End If

                Dim dReturn As New DataTable("EmployeeStatus")
                dReturn.Columns.Add("EmployeeName", GetType(String))
                dReturn.Columns.Add("ServerDateTime", GetType(Date))
                dReturn.Columns.Add("PunchDateTime", GetType(Date))
                dReturn.Columns.Add("PresenceStatus", GetType(String))
                dReturn.Columns.Add("PunchStatus", GetType(String))
                dReturn.Columns.Add("LastPunchDateTime", GetType(Date))
                dReturn.Columns.Add("LastPunch", GetType(Object))
                dReturn.Columns.Add("DifferenceDate", GetType(Date))

                Dim dRow As DataRow = dReturn.NewRow

                dRow("EmployeeName") = oPassportTicket.Name
                Dim punchDateTime As Date = oTerminal.GetCurrentDateTime() 'AppGeneral.CurrentTerminal.ID
                dRow("ServerDateTime") = punchDateTime

                If roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("MultiTimeZoneEnabled", New AdvancedParameter.roAdvancedParameterState).Value) Then
                    punchDateTime = TimeZoneInfo.ConvertTime(punchDateTime, timeZone)
                End If
                dRow("PunchDateTime") = punchDateTime
                dRow("PresenceStatus") = "Inside"
                dRow("PunchStatus") = "Indet"
                dRow("LastPunchDateTime") = dRow("ServerDateTime")

                dReturn.Rows.Add(dRow)

                'Carga hora
                lrret.ServerDate = dReturn.Rows(0)("ServerDateTime")

                'Nom usuari
                lrret.EmployeeName = dReturn.Rows(0)("EmployeeName")
                'Tiempo desde la ultima ausencia
                lrret.LastPunchDate = dReturn.Rows(0)("LastPunchDateTime")
                ' Estado de presencia del empleado inside/outside
                lrret.PresenceStatus = dReturn.Rows(0)("PresenceStatus")

                ' Verificamos si el empleado actual tiene permisos para fichar con el terminal y si el terminal es de tipo presencia
                lrret.PunchesEnabled = False
                lrret.ProductiVEnabled = False
                lrret.TaskTitle = ""
                lrret.TaskId = -1
                lrret.ProductiVEnabled = False
                lrret.CanCompleteTask = False
                lrret.HasCompletePermission = False
                lrret.CostCenterId = -1
                lrret.CostCenterEnabled = False
                lrret.CostCenterName = ""

                lrret.ScheduleStatus = New EmployeeAlerts
                If bOnlyNotifications Then
                    lrret.SupervisorStatus = StatusHelper.LoadSupervisorAlerts(oPassportTicket)
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetSupervisorStatus")

                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function LoadSupervisorAlerts(ByVal oPassportTicket As roPassportTicket) As SupervisorAlerts
            Dim oSupervisorAlerts As New SupervisorAlerts

            Dim oState As New Notifications.roNotificationState(oPassportTicket.ID)

            oSupervisorAlerts = Notifications.roNotification.GetDesktopAlertsForPortal(oState)

            If oState.Result = NotificationResultEnum.NoError Then

                Dim oDocState As New VTDocuments.roDocumentState(oPassportTicket.ID)
                Dim oDocManager As New VTDocuments.roDocumentManager(oDocState)
                oSupervisorAlerts.DocumentAlerts = oDocManager.GetDocumentationFaultAlerts(-1, DocumentType.Employee, 0)

                oSupervisorAlerts.Status = ErrorCodes.OK
            Else
                oSupervisorAlerts.Status = ErrorCodes.GENERAL_ERROR
            End If

            Return oSupervisorAlerts
        End Function

        Public Shared Function LoadSupervisorAlertsByType(ByVal oPassportTicket As roPassportTicket, ByVal idAlertType As Integer) As SupervisorAlertsTypeDetail
            Dim oSupervisorAlerts As New SupervisorAlertsTypeDetail

            Dim oState As New Notifications.roNotificationState(oPassportTicket.ID)

            Dim alertsDS As DataSet = Notifications.roNotification.GetDesktopAlertsDetails(idAlertType, oState)

            If oState.Result = NotificationResultEnum.NoError Then

                If (alertsDS IsNot Nothing AndAlso alertsDS.Tables.Count > 0) Then
                    Dim oAlertsLst As New Generic.List(Of DTOs.DesktopAlertDetail)

                    For Each oRow In alertsDS.Tables(0).Rows

                        Dim oAlert As New DTOs.DesktopAlertDetail With {
                            .Name = roTypes.Any2String(oRow("Column1Detail")),
                            .Id = roTypes.Any2Integer(oRow("Id")),
                            .Description = roTypes.Any2String(oRow("Column2Detail"))
                            }

                        oAlertsLst.Add(oAlert)
                    Next

                    oSupervisorAlerts.Alerts = oAlertsLst.ToArray
                End If

                oSupervisorAlerts.Status = ErrorCodes.OK
            Else
                oSupervisorAlerts.Alerts = {}
                oSupervisorAlerts.Status = ErrorCodes.GENERAL_ERROR
            End If

            Return oSupervisorAlerts
        End Function

        Public Shared Function GetEmployee(ByVal idEmployee As Integer) As Employee.roEmployee
            Try
                Return Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployee")
                Return New Employee.roEmployee
            End Try
        End Function

        Public Shared Function GetEmployeeScheduleStatus(ByVal oPassportTicket As roPassportTicket, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo) As EmployeeStatus
            Dim lrret As New EmployeeStatus

            Try
                Dim oEmployeePassport As roPassportTicket = roPassportManager.GetPassportTicket(idEmployee, LoadType.Employee)
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)


                lrret = GetEmployeeScheduleDetails(oEmployeePassport, oLanguage, idEmployee, timeZone)

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeScheduleStatus")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetSupervisorScheduleStatus(ByVal oPassportTicket As roPassportTicket, ByVal idPassport As Integer, ByVal timeZone As TimeZoneInfo) As EmployeeStatus
            Dim lrret As New EmployeeStatus

            Try
                Dim oUserPassport As roPassportTicket = roPassportManager.GetPassportTicket(idPassport, LoadType.Passport)
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)


                lrret = GetSupervisorScheduleDetails(oUserPassport, oLanguage, idPassport, timeZone)

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetSupervisorScheduleStatus")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeNotifications(ByVal oPassportTicket As roPassportTicket, ByVal idEmployee As Integer, ByVal bLoadSupervisorAlerts As Boolean, ByVal timeZone As TimeZoneInfo) As EmployeeNotifications
            Dim lrret As New EmployeeNotifications

            Try
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)


                lrret.ScheduleStatus = GetEmployeeAlerts(oPassportTicket, oLanguage, idEmployee, False)

                If bLoadSupervisorAlerts Then
                    lrret.SupervisorStatus = StatusHelper.LoadSupervisorAlerts(oPassportTicket)
                End If

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeNotifications")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetSupervisorNotifications(ByVal oPassportTicket As roPassportTicket, ByVal idpassport As Integer, ByVal bLoadSupervisorAlerts As Boolean, ByVal timeZone As TimeZoneInfo) As EmployeeNotifications
            Dim lrret As New EmployeeNotifications

            Try
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)


                lrret.ScheduleStatus = New EmployeeAlerts()

                If bLoadSupervisorAlerts Then
                    lrret.SupervisorStatus = StatusHelper.LoadSupervisorAlerts(oPassportTicket)
                End If

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetSupervisorNotifications")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function


        Public Shared Function GetEmployeeNotificationStatus(ByVal oPassportTicket As roPassportTicket, ByVal idEmployee As Integer, ByVal bLoadSupervisorAlerts As Boolean, ByVal timeZone As TimeZoneInfo) As EmployeeNotificationStatus
            Dim lrret As New EmployeeNotificationStatus

            Try
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)


                Dim oParameter As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.VTPortalApiVersion.ToString(), New AdvancedParameter.roAdvancedParameterState)
                Dim ApiVersion = roTypes.Any2Integer(oParameter.Value)


                lrret.HasStatusAlerts = False
                lrret.HasMandatoryCommuniques = False

                If ApiVersion > 10 Then
                    Dim oCommuniqueManager As New roCommuniqueManager()
                    If oCommuniqueManager.HasEmployeeCommuniquesWithAlert(idEmployee, False, False) Then
                        lrret.HasMandatoryCommuniques = True
                        lrret.HasStatusAlerts = True
                    Else
                        lrret.HasMandatoryCommuniques = False
                        lrret.HasStatusAlerts = EmployeeHasAlerts(oPassportTicket.ID, oPassportTicket.IDEmployee)
                    End If
                Else
                    lrret.HasStatusAlerts = EmployeeHasAlerts(oPassportTicket.ID, oPassportTicket.IDEmployee)
                End If

                lrret.HasSupervisorAlerts = False
                If oPassportTicket.IsSupervisor Then lrret.HasSupervisorAlerts = SupervisorHasAlerts(oPassportTicket.ID)


                lrret.Status = ErrorCodes.OK

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeNotificationStatus")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetSupervisorNotificationStatus(ByVal oPassportTicket As roPassportTicket, ByVal idpassport As Integer, ByVal bLoadSupervisorAlerts As Boolean, ByVal timeZone As TimeZoneInfo) As EmployeeNotificationStatus
            Dim lrret As New EmployeeNotificationStatus

            Try
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)

                lrret.HasStatusAlerts = False
                lrret.HasSupervisorAlerts = SupervisorHasAlerts(oPassportTicket.ID)

                lrret.Status = ErrorCodes.OK

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetSupervisorNotificationStatus")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function



        Public Shared Function GetEmployeeScheduleDetails(ByVal oEmployeePassport As roPassportTicket, ByVal oLanguage As roLanguage, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo) As EmployeeStatus
            Dim lrret As New EmployeeStatus

            Try
                Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)

                'Comprova si s'ha d'afegir un marcatge oblidat
                Dim oPunch As Punch.roPunch = Nothing
                Dim oPunchStatus As PunchStatus
                Dim oSeqStatus As PunchSeqStatus

                Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState(oEmployeePassport.ID))
                Dim dServerDateTime As Date = DateTime.Now
                Dim oTerminal As Terminal.roTerminal = Nothing

                If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                    dServerDateTime = oTerminals(0).GetCurrentDateTime()
                    oTerminal = oTerminals(0)
                End If

                If Not Punch.roPunch.DoSequencePunch(idEmployee, dServerDateTime, oTerminal.ID, 1, -1, Nothing, oPunch, oPunchStatus, oSeqStatus, False, New Punch.roPunchState) Then
                    lrret.Status = ErrorCodes.PUNCH_ERROR_DO_SEQUENCE
                End If

                lrret.LastPunchId = oPunch.ID
                Dim dTbl As DataTable = VTPortal.StatusHelper.UpdateStatus(oEmployee, oTerminal, timeZone)

                'Carga hora
                lrret.ServerDate = dTbl.Rows(0)("ServerDateTime")

                'Nom usuari
                lrret.EmployeeName = dTbl.Rows(0)("EmployeeName")
                'Tiempo desde la ultima ausencia
                lrret.LastPunchDate = dTbl.Rows(0)("LastPunchDateTime")
                'Dirección del último fichaje
                lrret.LastPunchDirection = dTbl.Rows(0)("PunchStatus")
                ' Estado de presencia del empleado inside/outside
                lrret.PresenceStatus = dTbl.Rows(0)("PresenceStatus")

                ' Verificamos si el empleado actual tiene permisos para fichar con el terminal y si el terminal es de tipo presencia
                lrret.PunchesEnabled = VTPortal.StatusHelper.PunchesEnabled(oEmployeePassport.ID, oTerminal)

                Try
                    lrret.ReadMode = VTPortal.SecurityHelper.HasReadModeEnabled(oEmployeePassport.IDEmployee)
                Catch ex As Exception
                    lrret.ReadMode = False
                End Try

                If oEmployee.Type = "J" AndAlso VTPortal.SecurityHelper.GetFeaturePermission(oEmployeePassport.ID, "TaskPunches.Punches", "E") >= Permission.Read Then

                    lrret.HasCompletePermission = VTPortal.SecurityHelper.HasFeaturePermission(oEmployeePassport.ID, "TaskPunches.Complete", Permission.Write, "E")

                    lrret.ProductiVEnabled = True

                    Dim oState As New VTUserFields.UserFields.roUserFieldState(oEmployeePassport.ID)
                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(oState, oPunchState)
                    Dim iLimit As Integer = 200
                    lrret.AvailableTasks = Punch.roPunch.GetAvailableTasksByEmployee(oEmployeePassport.IDEmployee, oPunchState, iLimit)

                    If lrret.AvailableTasks <= iLimit Then lrret.AvailableTasks = 199 'Thanks Alper!

                    oPunch.IDEmployee = idEmployee
                    Dim lastPunchID As Integer
                    Dim lastPunchDate As DateTime = DateTime.Now

                    oPunch.GetLastPunchTask(PunchTypeEnum._TASK, lastPunchDate, lastPunchID)

                    If lastPunchID <> -1 Then
                        Dim oTask As Task.roTask = Task.roTask.GetLastTaskByEmployee(idEmployee, New Task.roTaskState)
                        lrret.LastTaskDate = lastPunchDate
                        lrret.TaskTitle = oTask.Project & ":" & oTask.Name
                        lrret.TaskDescription = oTask.Description
                        lrret.TaskId = oTask.ID

                        Dim usersWorking As Double = Task.roTask.GetEmployeesWorkingInTask(oTask.ID, New Task.roTaskState)
                        If (usersWorking > 1) Then
                            lrret.CanCompleteTask = False
                        Else
                            lrret.CanCompleteTask = True
                        End If
                    Else
                        lrret.TaskTitle = ""
                        lrret.TaskDescription = ""
                        lrret.TaskId = -1
                        lrret.CanCompleteTask = False
                    End If
                Else
                    lrret.TaskTitle = ""
                    lrret.TaskDescription = ""
                    lrret.TaskId = -1
                    lrret.ProductiVEnabled = False
                    lrret.CanCompleteTask = False
                    lrret.HasCompletePermission = False
                End If

                Dim oServerLicense As New roServerLicense()
                If oServerLicense.FeatureIsInstalled("Feature\CostControl") AndAlso lrret.PunchesEnabled Then
                    lrret.CostCenterEnabled = True
                    Dim idCostCenter As Integer = -1

                    lrret.CostCenterName = BusinessCenter.roBusinessCenter.GetEmployeeWorkingCostCenter(New BusinessCenter.roBusinessCenterState(oEmployeePassport.ID), idEmployee, idCostCenter)
                    lrret.CostCenterId = idCostCenter
                Else
                    lrret.CostCenterId = -1
                    lrret.CostCenterEnabled = False
                    lrret.CostCenterName = ""
                End If

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeScheduleDetails")

                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function


        Public Shared Function GetSupervisorScheduleDetails(ByVal oPassportTicket As roPassportTicket, ByVal oLanguage As roLanguage, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo) As EmployeeStatus
            Dim lrret As New EmployeeStatus

            Try

                lrret.LastPunchId = -1
                Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(-1, "LIVEPORTAL", New Terminal.roTerminalState(oPassportTicket.ID))
                Dim dServerDateTime As Date = DateTime.Now
                Dim oTerminal As Terminal.roTerminal = Nothing

                If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                    dServerDateTime = oTerminals(0).GetCurrentDateTime()
                    oTerminal = oTerminals(0)
                End If

                Dim dReturn As New DataTable("EmployeeStatus")
                dReturn.Columns.Add("EmployeeName", GetType(String))
                dReturn.Columns.Add("ServerDateTime", GetType(Date))
                dReturn.Columns.Add("PunchDateTime", GetType(Date))
                dReturn.Columns.Add("PresenceStatus", GetType(String))
                dReturn.Columns.Add("PunchStatus", GetType(String))
                dReturn.Columns.Add("LastPunchDateTime", GetType(Date))
                dReturn.Columns.Add("LastPunch", GetType(Object))
                dReturn.Columns.Add("DifferenceDate", GetType(Date))

                Dim dRow As DataRow = dReturn.NewRow

                dRow("EmployeeName") = oPassportTicket.Name
                Dim punchDateTime As Date = oTerminal.GetCurrentDateTime() 'AppGeneral.CurrentTerminal.ID
                dRow("ServerDateTime") = punchDateTime

                If roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("MultiTimeZoneEnabled", New AdvancedParameter.roAdvancedParameterState).Value) Then
                    punchDateTime = TimeZoneInfo.ConvertTime(punchDateTime, timeZone)
                End If
                dRow("PunchDateTime") = punchDateTime
                dRow("PresenceStatus") = "Inside"
                dRow("PunchStatus") = "Indet"
                dRow("LastPunchDateTime") = dRow("ServerDateTime")

                dReturn.Rows.Add(dRow)

                'Carga hora
                lrret.ServerDate = dReturn.Rows(0)("ServerDateTime")

                'Nom usuari
                lrret.EmployeeName = dReturn.Rows(0)("EmployeeName")
                'Tiempo desde la ultima ausencia
                lrret.LastPunchDate = dReturn.Rows(0)("LastPunchDateTime")
                ' Estado de presencia del empleado inside/outside
                lrret.PresenceStatus = dReturn.Rows(0)("PresenceStatus")


                ' Verificamos si el empleado actual tiene permisos para fichar con el terminal y si el terminal es de tipo presencia
                lrret.PunchesEnabled = False
                lrret.ProductiVEnabled = False
                lrret.TaskTitle = ""
                lrret.TaskId = -1
                lrret.ProductiVEnabled = False
                lrret.CanCompleteTask = False
                lrret.HasCompletePermission = False
                lrret.CostCenterId = -1
                lrret.CostCenterEnabled = False
                lrret.CostCenterName = ""
                lrret.Status = ErrorCodes.OK

            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetSupervisorScheduleDetails")

                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function



        Public Shared Function GetEmployeeStatus(ByVal oPassportTicket As roPassportTicket, ByVal idEmployee As Integer, ByVal bLoadSupervisorAlerts As Integer, ByVal timeZone As TimeZoneInfo, ByVal bLoadOnlyAlerts As Boolean) As UserStatus
            Dim lrret As New UserStatus

            Try
                Dim oEmployeePassport As roPassportTicket = roPassportManager.GetPassportTicket(idEmployee, LoadType.Employee)
                Dim oLanguage = New roLanguage()
                oLanguage.SetLanguageReference("ProcessNotificationServer", oPassportTicket.Language.Key)

                If bLoadOnlyAlerts Then
                    lrret.ScheduleStatus = GetEmployeeAlerts(oPassportTicket, oLanguage, idEmployee, bLoadSupervisorAlerts)

                    If bLoadSupervisorAlerts Then
                        lrret.SupervisorStatus = StatusHelper.LoadSupervisorAlerts(oPassportTicket)
                    End If
                Else
                    lrret = GetEmployeeDetails(oEmployeePassport, oLanguage, idEmployee, timeZone)

                    Dim oParameter As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.VTPortalApiVersion.ToString(), New AdvancedParameter.roAdvancedParameterState)
                    Dim ApiVersion = roTypes.Any2Integer(oParameter.Value)

                    If ApiVersion > 10 Then

                        Dim oCommuniqueManager As New roCommuniqueManager()
                        lrret.Communiques = oCommuniqueManager.GetAllEmployeeCommuniquesWithStatus(idEmployee, False, False).ToArray

                        For Each oCommunique In lrret.Communiques
                            oCommunique.Communique.Message = oCommunique.Communique.Message.Replace(vbCr, "<br>").Replace(vbLf, "<br>")
                        Next
                    End If

                    If ApiVersion > 14 Then
                        Try
                            Dim oSurveyManager As New roSurveyManager()
                            Dim surveys = oSurveyManager.GetAllSurveys(idEmployee, False)

                            If surveys IsNot Nothing AndAlso surveys.Count > 0 Then
                                lrret.Surveys = surveys.ToArray
                            Else
                                lrret.Surveys = {}
                            End If
                        Catch ex As Exception
                            lrret.Surveys = {}
                        End Try
                    Else
                        lrret.Surveys = {}
                    End If

                    If ApiVersion > 18 Then
                        Try
                            Dim zones = New Zones()

                            zones = VTPortal.ZonesHelper.GetListAllZones(idEmployee)

                            If zones IsNot Nothing AndAlso zones.ListZones.Count > 0 Then
                                lrret.Zones = zones
                            Else
                                lrret.Zones = New Zones()
                            End If
                        Catch ex As Exception
                            lrret.Zones = New Zones()
                        End Try
                    Else
                        lrret.Zones = New Zones()
                    End If
                End If

                lrret.Channels = {}
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeStatus")
                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeDetails(ByVal oEmployeePassport As roPassportTicket, ByVal oLanguage As roLanguage, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo) As UserStatus
            Dim lrret As New UserStatus

            Try
                Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)

                'Comprova si s'ha d'afegir un marcatge oblidat
                Dim oPunch As Punch.roPunch = Nothing
                Dim oPunchStatus As PunchStatus
                Dim oSeqStatus As PunchSeqStatus

                Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState(oEmployeePassport.ID))
                Dim dServerDateTime As Date = DateTime.Now
                Dim oTerminal As Terminal.roTerminal = Nothing

                If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                    dServerDateTime = oTerminals(0).GetCurrentDateTime()
                    oTerminal = oTerminals(0)
                End If

                If Not Punch.roPunch.DoSequencePunch(idEmployee, dServerDateTime, oTerminal.ID, 1, -1, Nothing, oPunch, oPunchStatus, oSeqStatus, False, New Punch.roPunchState) Then
                    lrret.Status = ErrorCodes.PUNCH_ERROR_DO_SEQUENCE
                End If

                lrret.LastPunchId = oPunch.ID

                Select Case oSeqStatus
                    Case PunchSeqStatus.Max_SeqOK 'Primera entrada de la jornada
                        lrret.FirstEntrance = True
                    Case PunchSeqStatus.Max_SeqERR 'Primera entrada de la jornada, marcaje olvidado
                        lrret.ForbiddenPunch = True
                    Case PunchSeqStatus.NoSequence
                End Select

                Dim dTbl As DataTable = VTPortal.StatusHelper.UpdateStatus(oEmployee, oTerminal, timeZone)

                'Carga hora
                lrret.ServerDate = dTbl.Rows(0)("ServerDateTime")

                'Nom usuari
                lrret.EmployeeName = dTbl.Rows(0)("EmployeeName")
                'Tiempo desde la ultima ausencia
                lrret.LastPunchDate = dTbl.Rows(0)("LastPunchDateTime") 'TimeZoneInfo.ConvertTime(dTbl.Rows(0)("LastPunchDateTime"), session.TimeZone, TimeZoneInfo.Local)
                ' Estado de presencia del empleado inside/outside
                lrret.PresenceStatus = dTbl.Rows(0)("PresenceStatus")

                ' Verificamos si el empleado actual tiene permisos para fichar con el terminal y si el terminal es de tipo presencia
                lrret.PunchesEnabled = VTPortal.StatusHelper.PunchesEnabled(oEmployeePassport.ID, oTerminal)

                If lrret.PunchesEnabled AndAlso lrret.ForbiddenPunch Then

                    Dim oParameters As New roParameters("OPTIONS")
                    Dim bolRequirePunch As Object = oParameters.Parameter(Parameters.RequiredForbiddenPunch)
                    Dim bolShowMsgbox As Boolean = False
                    If bolRequirePunch IsNot Nothing Then
                        If CBool(bolRequirePunch) = True Then
                            bolShowMsgbox = True
                        End If
                    Else
                        bolShowMsgbox = True
                    End If
                    If bolShowMsgbox Then
                        Dim strLastMoveInfo As String = ""

                        Dim oLastPunch As Punch.roPunch = VTBusiness.Punch.roPunch.GetLastPunchPres(idEmployee, New Employee.roEmployeeState)
                        If oLastPunch IsNot Nothing Then
                            lrret.ForbiddenPunchDate = oLastPunch.DateTime
                        End If
                        lrret.Status = ErrorCodes.PUNCH_ERROR_FORBIDDEN
                    End If
                End If

                Dim oServerLicense As New roServerLicense()

                ' Datos de teletrabajo relativos al día de hoy
                Dim bEmployeeHasTelecommuteAgreementOnDate As Boolean = False
                Dim sEmployeeTelecommuteMandatoryDays As String = String.Empty
                Dim sEmployeeTelecommuteOptionalDays As String = String.Empty
                Dim iEmployeeTelecommuteMaxDays As Integer = 0
                Dim iEmployeeTelecommuteMaxPercentage As Integer = 0
                Dim iEmployeeTelecommutePeriodType As Integer = 0

                Employee.roEmployee.GetEmployeeTelecommutingDataOnDate(Now.Date, oEmployee.ID, New Employee.roEmployeeState(-1), bEmployeeHasTelecommuteAgreementOnDate, sEmployeeTelecommuteMandatoryDays, sEmployeeTelecommuteOptionalDays, iEmployeeTelecommuteMaxDays, iEmployeeTelecommuteMaxPercentage, iEmployeeTelecommutePeriodType)

                lrret.Telecommuting = bEmployeeHasTelecommuteAgreementOnDate
                lrret.TelecommutingDays = sEmployeeTelecommuteMandatoryDays.Trim

                lrret.TelecommutingExpected = False
                lrret.WorkCenterName = ""

                Try
                    lrret.ReadMode = VTPortal.SecurityHelper.HasReadModeEnabled(oEmployeePassport.IDEmployee)
                Catch ex As Exception
                    lrret.ReadMode = False
                End Try

                If oEmployee.Type = "J" AndAlso VTPortal.SecurityHelper.GetFeaturePermission(oEmployeePassport.ID, "TaskPunches.Punches", "E") >= Permission.Read Then

                    lrret.HasCompletePermission = VTPortal.SecurityHelper.HasFeaturePermission(oEmployeePassport.ID, "TaskPunches.Complete", Permission.Write, "E")

                    lrret.ProductiVEnabled = True

                    Dim oState As New VTUserFields.UserFields.roUserFieldState(oEmployeePassport.ID)
                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(oState, oPunchState)
                    Dim iLimit As Integer = 200
                    lrret.AvailableTasks = Punch.roPunch.GetAvailableTasksByEmployee(oEmployeePassport.IDEmployee, oPunchState, iLimit)

                    If lrret.AvailableTasks <= iLimit Then lrret.AvailableTasks = 199 'Thanks Alper!

                    oPunch.IDEmployee = idEmployee
                    Dim lastPunchID As Integer
                    Dim lastPunchDate As DateTime = DateTime.Now

                    oPunch.GetLastPunchTask(PunchTypeEnum._TASK, lastPunchDate, lastPunchID)

                    If lastPunchID <> -1 Then
                        Dim oTask As Task.roTask = Task.roTask.GetLastTaskByEmployee(idEmployee, New Task.roTaskState)
                        lrret.LastTaskDate = lastPunchDate
                        lrret.TaskTitle = oTask.Project & ":" & oTask.Name
                        lrret.TaskDescription = oTask.Description
                        lrret.TaskId = oTask.ID

                        Dim usersWorking As Double = Task.roTask.GetEmployeesWorkingInTask(oTask.ID, New Task.roTaskState)
                        If (usersWorking > 1) Then
                            lrret.CanCompleteTask = False
                        Else
                            lrret.CanCompleteTask = True
                        End If
                    Else
                        lrret.TaskTitle = ""
                        lrret.TaskDescription = ""
                        lrret.TaskId = -1
                        lrret.CanCompleteTask = False
                    End If
                Else
                    lrret.TaskTitle = ""
                    lrret.TaskDescription = ""
                    lrret.TaskId = -1
                    lrret.ProductiVEnabled = False
                    lrret.CanCompleteTask = False
                    lrret.HasCompletePermission = False
                End If

                If oServerLicense.FeatureIsInstalled("Feature\CostControl") AndAlso lrret.PunchesEnabled Then
                    lrret.CostCenterEnabled = True
                    Dim idCostCenter As Integer = -1

                    lrret.CostCenterName = BusinessCenter.roBusinessCenter.GetEmployeeWorkingCostCenter(New BusinessCenter.roBusinessCenterState(oEmployeePassport.ID), idEmployee, idCostCenter)
                    lrret.CostCenterId = idCostCenter
                Else
                    lrret.CostCenterId = -1
                    lrret.CostCenterEnabled = False
                    lrret.CostCenterName = ""
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeStatus")

                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function EmployeeHasAlerts(ByVal idpassport As Integer, ByVal idemployee As Integer) As Boolean

            Try
                Dim lrret As EmployeeAlerts = Notifications.roNotification.GetPortalAlerts(idemployee, New Notifications.roNotificationState(idpassport), Nothing, True)
                If lrret.RequestAlerts.Any() OrElse lrret.ExpiredDocAlert IsNot Nothing OrElse lrret.IncompletePunches.Any() OrElse lrret.ForecastDocuments.Any() Then Return True

                Dim oParameter As New AdvancedParameter.roAdvancedParameter(AdvancedParameterType.VTPortalApiVersion.ToString(), New AdvancedParameter.roAdvancedParameterState)
                Dim ApiVersion = roTypes.Any2Integer(oParameter.Value)

                If ApiVersion > 14 Then
                    Dim oSurveyManager As New roSurveyManager()
                    If oSurveyManager.HasEmployeeSurveysWithAlert(idemployee, False) Then Return True
                End If


                Dim oChannelState As New roChannelState(idpassport)
                Dim oChannelManager As New roChannelManager(oChannelState)
                If oChannelManager.HasChannelsWithMessages(idemployee) Then Return True


                Dim oDocManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(idpassport))
                lrret.SignDocuments = oDocManager.GetPortalAlerts_PendingSignDocumentsByEmployee(idemployee, DocumentType.Employee)
                If lrret.SignDocuments.Any() Then Return True

                lrret.TrackingDocuments = oDocManager.GetForecastDocumentationFaultAlerts(idemployee, DocumentType.Employee,, ForecastType.Leave, False)
                If lrret.TrackingDocuments.Any() Then Return True

                Dim oTmpLst As New Generic.List(Of DocumentAlert)
                oTmpLst.AddRange(oDocManager.GetForecastDocumentationFaultAlerts(idemployee, DocumentType.Employee,, ForecastType.AnyAbsence, False))
                If oTmpLst.Any() Then Return True

                oTmpLst.AddRange(oDocManager.GetForecastDocumentationFaultAlerts(idemployee, DocumentType.Employee,, ForecastType.OverWork, False))
                If oTmpLst.Any() Then Return True

                oTmpLst.AddRange(oDocManager.GetPortalAlerts_UndeliveredDocumentsByEmployee(idemployee, DocumentType.Employee))
                If oTmpLst.Any() Then Return True

                oTmpLst.AddRange(oDocManager.GetAccessAuthorizationDocumentationFaultAlerts(idemployee, DocumentType.Employee,, False))
                If oTmpLst.Any() Then Return True

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTPortal::StatusHelper::EmployeeHasAlerts", ex)
            End Try

            Return False
        End Function

        Public Shared Function SupervisorHasAlerts(ByVal idpassport As Integer) As Boolean
            Try

                Dim oState As New Notifications.roNotificationState(idpassport)

                Dim oSupervisorAlerts As SupervisorAlerts = Notifications.roNotification.GetDesktopAlertsForPortal(oState,, True)
                If oSupervisorAlerts.Alerts.Any() Then Return True


                Dim oDocState As New VTDocuments.roDocumentState(idpassport)
                Dim oDocManager As New VTDocuments.roDocumentManager(oDocState)
                oSupervisorAlerts.DocumentAlerts = oDocManager.GetDocumentationFaultAlerts(-1, DocumentType.Employee, 0,,,,,, True)
                If oSupervisorAlerts.DocumentAlerts.GpaAlerts.Any() OrElse oSupervisorAlerts.DocumentAlerts.AbsenteeismDocuments.Any() OrElse oSupervisorAlerts.DocumentAlerts.AccessAuthorizationDocuments.Any() OrElse oSupervisorAlerts.DocumentAlerts.WorkForecastDocuments.Any() Then Return True

                Return False

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTPortal::StatusHelper::SupervisorHasAlerts", ex)
            End Try

            Return False
        End Function


        Public Shared Function GetEmployeeAlerts(ByVal oPassportTicket As roPassportTicket, ByVal oLanguage As roLanguage, ByVal idEmployee As Integer, ByVal bLoadSupervisorAlerts As Integer) As EmployeeAlerts
            Dim lrret As New EmployeeAlerts

            Try
                lrret = Notifications.roNotification.GetPortalAlerts(idEmployee, New Notifications.roNotificationState(oPassportTicket.ID), Nothing)
                If lrret.RequestAlerts.Length > 0 Then
                    Dim oReqState As New Requests.roRequestState(oPassportTicket.ID)
                    For Each oRequest In lrret.RequestAlerts
                        Dim oReq As Requests.roRequest = New Requests.roRequest(oRequest.IdRequest, oReqState)
                        oRequest.Description = oReq.RequestInfo(False)
                        Select Case oRequest.Status
                            Case 0
                                oRequest.AlertSubject = oLanguage.Translate("Notification.Portal.Request.Pending.Subject", "Notification")
                            Case 1
                                oRequest.AlertSubject = oLanguage.Translate("Notification.Portal.Request.Running.Subject", "Notification")
                            Case 2
                                oRequest.AlertSubject = oLanguage.Translate("Notification.Portal.Request.Approved.Subject", "Notification")
                            Case 3
                                oRequest.AlertSubject = oLanguage.Translate("Notification.Portal.Request.Denied.Subject", "Notification")
                        End Select
                    Next
                End If

                If lrret.IncompletePunches.Length > 0 Then
                    For Each oPunchRequest In lrret.IncompletePunches
                        oPunchRequest.AlertSubject = oLanguage.Translate("Notification.Portal.IncompletePunch.Subject", "Notification")
                        oLanguage.ClearUserTokens()
                        oLanguage.AddUserToken(oPunchRequest.DateTime.ToShortDateString)
                        oPunchRequest.AlertDescription = oLanguage.Translate("Notification.Portal.IncompletePunch.Body", "Notification")
                    Next
                End If

                If lrret.ExpiredDocAlert IsNot Nothing Then
                    lrret.ExpiredDocAlert.AlertSubject = oLanguage.Translate("Notification.Portal.ExpiredDoc.Subject", "Notification")
                    oLanguage.ClearUserTokens()
                    oLanguage.AddUserToken(lrret.ExpiredDocAlert.DateTime.ToShortDateString)
                    lrret.ExpiredDocAlert.AlertDescription = oLanguage.Translate("Notification.Portal.ExpiredDoc.Body", "Notification")
                End If

                Dim oDocManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(oPassportTicket.ID))
                lrret.TrackingDocuments = oDocManager.GetForecastDocumentationFaultAlerts(idEmployee, DocumentType.Employee,, ForecastType.Leave, False)
                Dim oTmpLst As New Generic.List(Of DocumentAlert)
                oTmpLst.AddRange(oDocManager.GetForecastDocumentationFaultAlerts(idEmployee, DocumentType.Employee,, ForecastType.AnyAbsence, False))
                oTmpLst.AddRange(oDocManager.GetForecastDocumentationFaultAlerts(idEmployee, DocumentType.Employee,, ForecastType.OverWork, False))
                oTmpLst.AddRange(oDocManager.GetPortalAlerts_UndeliveredDocumentsByEmployee(idEmployee, DocumentType.Employee))
                oTmpLst.AddRange(oDocManager.GetAccessAuthorizationDocumentationFaultAlerts(idEmployee, DocumentType.Employee,, False))

                lrret.SignDocuments = oDocManager.GetPortalAlerts_PendingSignDocumentsByEmployee(idEmployee, DocumentType.Employee)
                lrret.ForecastDocuments = oTmpLst.ToArray
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::GetEmployeeStatus")

                lrret.Status = ErrorCodes.GENERAL_ERROR
            End Try

            Return lrret
        End Function

        Public Shared Function UploadUserPhoto(ByVal oPassport As roPassportTicket, ByVal fUploadedFile As Web.HttpPostedFile) As StdResponse
            Dim tCount As New StdResponse
            Try

                Dim file As IO.Stream = fUploadedFile.InputStream()
                Dim fileData As Byte() = New Byte(file.Length - 1) {}
                file.Read(fileData, 0, file.Length)
                If oPassport.IDEmployee IsNot Nothing Then
                    tCount.Result = VTBusiness.Common.roBusinessSupport.SetUserPhoto(oPassport.IDEmployee, fileData)
                    tCount.Status = ErrorCodes.OK
                Else
                    tCount.Status = ErrorCodes.REQUEST_ERROR_InvalidPassport
                End If
            Catch ex As Exception

                tCount.Result = False
                tCount.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::StatusHelper::UploadUserPhoto")
            End Try
            Return tCount
        End Function

    End Class

End Namespace