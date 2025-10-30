Imports Robotics.Base.Analytics.Manager

Public Class roWebServicesStates
    Private oSecurityState As Robotics.Base.DTOs.roWsState = Nothing
    'Private oAuditState As AuditService.wscAuditState = Nothing

    Private oLabAgreeState As Robotics.Base.DTOs.roWsState = Nothing
    Private oPunchState As Robotics.Base.DTOs.roWsState = Nothing

    Private oSchedulerState As Robotics.Base.DTOs.roWsState = Nothing
    Private oUserAdminSecurityState As Robotics.Base.DTOs.roWsState = Nothing
    Private oWscState As Robotics.Base.DTOs.roWsState = Nothing
    Private oTaskTemplateState As Robotics.Base.DTOs.roWsState = Nothing
    Private oTerminalState As Robotics.Base.DTOs.roWsState = Nothing
    Private oGroupFeatureState As Robotics.Base.DTOs.roWsState
    Private oSecurityChartNodeState As Robotics.Base.DTOs.roWsState = Nothing

    Private oDocumentState As Robotics.Base.DTOs.roWsState = Nothing
    Private oProgrammedHolidayState As Robotics.Base.DTOs.roWsState = Nothing
    Private oProgrammedOvertimeState As Robotics.Base.DTOs.roWsState = Nothing
    Private oSDKState As Robotics.Base.DTOs.roWsState = Nothing
    Private oProgrammedCauseState As Robotics.Base.DTOs.roWsState = Nothing
    Private oProgrammedAbsenceState As Robotics.Base.DTOs.roWsState = Nothing
    Private oProductiveUnitState As Robotics.Base.DTOs.roWsState = Nothing
    Private oBudgetState As Robotics.Base.DTOs.roWsState = Nothing
    Private oBudgetRowState As Robotics.Base.DTOs.roWsState = Nothing
    Private oBudgetRowPeriodState As Robotics.Base.DTOs.roWsState = Nothing
    Private oScheduleRulesState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCalendarV2State As Robotics.Base.DTOs.roWsState = Nothing
    Private oCalendarShiftV2State As Robotics.Base.DTOs.roWsState = Nothing
    Private oCalendarPeriodCoverageState As Robotics.Base.DTOs.roWsState = Nothing
    Private oSecurityV2State As Robotics.Base.DTOs.roWsState = Nothing
    Private oDatalinkGuideState As Robotics.Base.DTOs.roWsState = Nothing
    Private oAccessGroupState As Robotics.Base.DTOs.roWsState = Nothing
    Private oAccessMoveState As Robotics.Base.DTOs.roWsState = Nothing
    Private oAccessPeriodState As Robotics.Base.DTOs.roWsState = Nothing
    Private oAssignmentState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCameraState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCaptureState As Robotics.Base.DTOs.roWsState = Nothing
    Private oZoneState As Robotics.Base.DTOs.roWsState = Nothing
    Private oAuditState As Robotics.Base.DTOs.roWsState = Nothing
    Private oConnectorState As Robotics.Base.DTOs.roWsState = Nothing
    Private oIndicatorState As Robotics.Base.DTOs.roWsState = Nothing
    Private oNotificationState As Robotics.Base.DTOs.roWsState = Nothing
    Private oReportState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCostCenterState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCauseState As Robotics.Base.DTOs.roWsState = Nothing
    Private oMoveState As Robotics.Base.DTOs.roWsState = Nothing
    Private oIncidenceState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCommonState As Robotics.Base.DTOs.roWsState = Nothing
    Private oUserTaskState As Robotics.Base.DTOs.roWsState = Nothing
    Private oEventSchedulerState As Robotics.Base.DTOs.roWsState = Nothing
    Private oDocumentAbsenceState As Robotics.Base.DTOs.roWsState = Nothing
    Private oSecurityOptionState As Robotics.Base.DTOs.roWsState = Nothing
    Private oDinningRoomState As Robotics.Base.DTOs.roWsState = Nothing
    Private oLiveTaskState As Robotics.Base.DTOs.roWsState = Nothing
    Private oContractState As Robotics.Base.DTOs.roWsState = Nothing
    Private oEmployeeState As Robotics.Base.DTOs.roWsState = Nothing
    Private oEmployeeUserFieldState As Robotics.Base.DTOs.roWsState = Nothing
    Private oShiftState As Robotics.Base.DTOs.roWsState = Nothing
    Private oDataLinkBaseState As Robotics.Base.DTOs.roWsState = Nothing
    Private oLicenseState As Robotics.Base.DTOs.roWsState = Nothing
    Private oTaskState As Robotics.Base.DTOs.roWsState = Nothing
    Private oTaskFieldState As Robotics.Base.DTOs.roWsState = Nothing
    Private oBusinessCenterState As Robotics.Base.DTOs.roWsState = Nothing
    Private oPortalBaseState As Robotics.Base.DTOs.roWsState = Nothing
    Private oRequestState As Robotics.Base.DTOs.roWsState = Nothing
    Private oConceptsState As Robotics.Base.DTOs.roWsState = Nothing
    Private oUserFieldState As Robotics.Base.DTOs.roWsState = Nothing
    Private oUserTaskFieldState As Robotics.Base.DTOs.roWsState = Nothing
    Private oEmployeeGroupState As Robotics.Base.DTOs.roWsState = Nothing
    Private oEmployeeGroupUserFieldState As Robotics.Base.DTOs.roWsState = Nothing
    Private oCommuniqueState As Robotics.Base.DTOs.roWsState = Nothing
    Private oSecurityV3State As Robotics.Base.DTOs.roWsState = Nothing
    Private oToDoListState As Robotics.Base.VTToDoLists.roToDoListState = Nothing
    Private oSurveyState As Robotics.Base.VTSurveys.roSurveyState = Nothing
    Private oChannelState As Robotics.Base.VTChannels.roChannelState = Nothing
    Private oConversationState As Robotics.Base.VTChannels.roConversationState = Nothing
    Private oMessageState As Robotics.Base.VTChannels.roMessageState = Nothing
    Private oLogBookState As Robotics.Base.VTChannels.roLogBookState = Nothing
    Private oAnalyticsState As roAnalyticState = Nothing
    Private oBotState As Robotics.Base.VTBots.roBotState = Nothing
    Private oCollectiveState As Robotics.Base.VTCollectives.roCollectiveState = Nothing
    Private oRulesState As Robotics.Base.VTRules.roRulesState = Nothing
    Private oWebLinkState As Robotics.Base.VTWebLinks.roWebLinksManagerState = Nothing

    Public Property SecurityV3State() As Robotics.Base.DTOs.roWsState
        Get
            If oSecurityV3State Is Nothing Then oSecurityV3State = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSecurityV3State
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSecurityV3State = value
        End Set
    End Property

    Public Property AnalyticsState() As roAnalyticState
        Get
            If oAnalyticsState Is Nothing Then oAnalyticsState = New roAnalyticState()
            Return oAnalyticsState
        End Get
        Set(value As roAnalyticState)
            oAnalyticsState = value
        End Set
    End Property

    Public Property DatalinkGuideState() As Robotics.Base.DTOs.roWsState
        Get
            If oDatalinkGuideState Is Nothing Then oDatalinkGuideState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oDatalinkGuideState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oDatalinkGuideState = value
        End Set
    End Property

    Public Property ZoneState() As Robotics.Base.DTOs.roWsState
        Get
            If oZoneState Is Nothing Then oZoneState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oZoneState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oZoneState = value
        End Set
    End Property

    Public Property EventSchedulerState() As Robotics.Base.DTOs.roWsState
        Get
            If oEventSchedulerState Is Nothing Then oEventSchedulerState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oEventSchedulerState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oEventSchedulerState = value
        End Set
    End Property

    Public Property IncidenceState() As Robotics.Base.DTOs.roWsState
        Get
            If oIncidenceState Is Nothing Then oIncidenceState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oIncidenceState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oIncidenceState = value
        End Set
    End Property

    Public Property CostCenterState() As Robotics.Base.DTOs.roWsState
        Get
            If oCostCenterState Is Nothing Then oCostCenterState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCostCenterState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCostCenterState = value
        End Set
    End Property

    Public Property NotificationState() As Robotics.Base.DTOs.roWsState
        Get
            If oNotificationState Is Nothing Then oNotificationState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oNotificationState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oNotificationState = value
        End Set
    End Property

    Public Property ReportState() As Robotics.Base.DTOs.roWsState
        Get
            If oReportState Is Nothing Then oReportState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oReportState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oReportState = value
        End Set
    End Property

    Public Property ConnectorState() As Robotics.Base.DTOs.roWsState
        Get
            If oConnectorState Is Nothing Then oConnectorState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oConnectorState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oConnectorState = value
        End Set
    End Property

    Public Property UsertaskState() As Robotics.Base.DTOs.roWsState
        Get
            If oUserTaskState Is Nothing Then oUserTaskState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oUserTaskState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oUserTaskState = value
        End Set
    End Property

    Public Property UserFieldState() As Robotics.Base.DTOs.roWsState
        Get
            If oUserFieldState Is Nothing Then oUserFieldState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oUserFieldState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oUserFieldState = value
        End Set
    End Property

    Public Property UserTaskFieldState() As Robotics.Base.DTOs.roWsState
        Get
            If oUserTaskFieldState Is Nothing Then oUserTaskFieldState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oUserTaskFieldState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oUserTaskFieldState = value
        End Set
    End Property

    Public Property TerminalState() As Robotics.Base.DTOs.roWsState
        Get
            If oTerminalState Is Nothing Then oTerminalState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oTerminalState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oTerminalState = value
        End Set
    End Property

    Public Property WscState() As Robotics.Base.DTOs.roWsState
        Get
            If oWscState Is Nothing Then oWscState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oWscState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oWscState = value
        End Set
    End Property

    Public Property UserAdminSecurityState() As Robotics.Base.DTOs.roWsState
        Get
            If oUserAdminSecurityState Is Nothing Then oUserAdminSecurityState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oUserAdminSecurityState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oUserAdminSecurityState = value
        End Set
    End Property

    Public Property MoveState() As Robotics.Base.DTOs.roWsState
        Get
            If oMoveState Is Nothing Then oMoveState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oMoveState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oMoveState = value
        End Set
    End Property

    Public Property CaptureState() As Robotics.Base.DTOs.roWsState
        Get
            If oCaptureState Is Nothing Then oCaptureState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCaptureState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCaptureState = value
        End Set
    End Property

    'Public Property EmployeeGroupState() As roGroupState
    '    Get
    '        If oEmployeeGroupState Is Nothing Then oEmployeeGroupState = New roGroupState
    '        Return oEmployeeGroupState
    '    End Get
    '    Set(value As roGroupState)
    '        oEmployeeGroupState = value
    '    End Set
    'End Property

    Public Property EmployeeGroupState() As Robotics.Base.DTOs.roWsState
        Get
            If oEmployeeGroupState Is Nothing Then oEmployeeGroupState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oEmployeeGroupState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oEmployeeGroupState = value
        End Set
    End Property

    Public Property EmployeeGroupUserFieldState() As Robotics.Base.DTOs.roWsState
        Get
            If oEmployeeGroupUserFieldState Is Nothing Then oEmployeeGroupUserFieldState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oEmployeeGroupUserFieldState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oEmployeeGroupUserFieldState = value
        End Set
    End Property

    Public Property CauseState() As Robotics.Base.DTOs.roWsState
        Get
            If oCauseState Is Nothing Then oCauseState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCauseState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCauseState = value
        End Set
    End Property

    Public Property DocumentAbsenceState() As Robotics.Base.DTOs.roWsState
        Get
            If oDocumentAbsenceState Is Nothing Then oDocumentAbsenceState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oDocumentAbsenceState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oDocumentAbsenceState = value
        End Set
    End Property

    Public Property CameraState() As Robotics.Base.DTOs.roWsState
        Get
            If oCameraState Is Nothing Then oCameraState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCameraState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCameraState = value
        End Set
    End Property

    Public Property SchedulerState() As Robotics.Base.DTOs.roWsState

        Get
            If oSchedulerState Is Nothing Then oSchedulerState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSchedulerState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSchedulerState = value
        End Set

    End Property

    Public Property AccessGroupState() As Robotics.Base.DTOs.roWsState
        Get
            If oAccessGroupState Is Nothing Then oAccessGroupState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oAccessGroupState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oAccessGroupState = value
        End Set
    End Property

    Public Property LiveTaskState() As Robotics.Base.DTOs.roWsState
        Get
            If oLiveTaskState Is Nothing Then oLiveTaskState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oLiveTaskState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oLiveTaskState = value
        End Set
    End Property

    Public Property ContractState() As Robotics.Base.DTOs.roWsState
        Get
            If oContractState Is Nothing Then oContractState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oContractState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oContractState = value
        End Set
    End Property

    Public Property IndicatorState() As Robotics.Base.DTOs.roWsState
        Get
            If oIndicatorState Is Nothing Then oIndicatorState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oIndicatorState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oIndicatorState = value
        End Set
    End Property

    Public Property AccessPeriodState() As Robotics.Base.DTOs.roWsState
        Get
            If oAccessPeriodState Is Nothing Then oAccessPeriodState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oAccessPeriodState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oAccessPeriodState = value
        End Set
    End Property

    Public Property AssignmentState() As Robotics.Base.DTOs.roWsState
        Get
            If oAssignmentState Is Nothing Then oAssignmentState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oAssignmentState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oAssignmentState = value
        End Set
    End Property

    Public Property ShiftState() As Robotics.Base.DTOs.roWsState
        Get
            If oShiftState Is Nothing Then oShiftState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oShiftState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oShiftState = value
        End Set
    End Property

    Public Property TaskState() As Robotics.Base.DTOs.roWsState
        Get
            If oTaskState Is Nothing Then oTaskState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oTaskState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oTaskState = value
        End Set
    End Property

    Public Property TaskFieldState() As Robotics.Base.DTOs.roWsState
        Get
            If oTaskFieldState Is Nothing Then oTaskFieldState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oTaskFieldState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oTaskFieldState = value
        End Set
    End Property

    Public Property BusinessCenterState() As Robotics.Base.DTOs.roWsState
        Get
            If oBusinessCenterState Is Nothing Then oBusinessCenterState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oBusinessCenterState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oBusinessCenterState = value
        End Set
    End Property

    Public Property TaskTemplateState() As Robotics.Base.DTOs.roWsState
        Get
            If oTaskTemplateState Is Nothing Then oTaskTemplateState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oTaskTemplateState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oTaskTemplateState = value
        End Set
    End Property

    Public Property WebLinksState() As Robotics.Base.DTOs.roWsState
        Get
            If oBusinessCenterState Is Nothing Then oBusinessCenterState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oBusinessCenterState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oBusinessCenterState = value
        End Set
    End Property

    Public Property SecurityOptionState() As Robotics.Base.DTOs.roWsState
        Get
            If oSecurityOptionState Is Nothing Then oSecurityOptionState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSecurityOptionState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSecurityOptionState = value
        End Set
    End Property

    Public Property PortalBaseState() As Robotics.Base.DTOs.roWsState
        Get
            If oPortalBaseState Is Nothing Then oPortalBaseState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oPortalBaseState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oPortalBaseState = value
        End Set
    End Property

    Public Property EmployeeState() As Robotics.Base.DTOs.roWsState
        Get
            If oEmployeeState Is Nothing Then oEmployeeState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oEmployeeState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oEmployeeState = value
        End Set
    End Property

    Public Property EmployeeUserFieldState() As Robotics.Base.DTOs.roWsState
        Get
            If oEmployeeUserFieldState Is Nothing Then oEmployeeUserFieldState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oEmployeeUserFieldState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oEmployeeUserFieldState = value
        End Set
    End Property

    Public Property ConceptsState() As Robotics.Base.DTOs.roWsState
        Get
            If oConceptsState Is Nothing Then oConceptsState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oConceptsState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oConceptsState = value
        End Set
    End Property

    Public Property ProgrammedAbsenceState() As Robotics.Base.DTOs.roWsState
        Get
            If oProgrammedAbsenceState Is Nothing Then oProgrammedAbsenceState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oProgrammedAbsenceState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oProgrammedAbsenceState = value
        End Set
    End Property

    Public Property DinningRoomState() As Robotics.Base.DTOs.roWsState
        Get
            If oDinningRoomState Is Nothing Then oDinningRoomState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oDinningRoomState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oDinningRoomState = value
        End Set
    End Property

    'Public Property DataLinkState() As DataLinkService.roDataLinkState
    '    Get
    '        If oDataLinkState Is Nothing Then oDataLinkState = New DataLinkService.roDataLinkState
    '        Return oDataLinkState
    '    End Get
    '    Set(value As DataLinkService.roDataLinkState)
    '        oDataLinkState = value
    '    End Set
    'End Property

    Public Property DataLinkBaseState() As Robotics.Base.DTOs.roWsState
        Get
            If oDataLinkBaseState Is Nothing Then oDataLinkBaseState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oDataLinkBaseState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oDataLinkBaseState = value
        End Set
    End Property

    Public Property RequestState() As Robotics.Base.DTOs.roWsState
        Get
            If oRequestState Is Nothing Then oRequestState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oRequestState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oRequestState = value
        End Set
    End Property

    Public Property ProgrammedCauseState() As Robotics.Base.DTOs.roWsState
        Get
            If oProgrammedCauseState Is Nothing Then oProgrammedCauseState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oProgrammedCauseState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oProgrammedCauseState = value
        End Set
    End Property

    Public Property CommonState() As Robotics.Base.DTOs.roWsState
        Get
            If oCommonState Is Nothing Then oCommonState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCommonState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCommonState = value
        End Set
    End Property

    Public Property PunchState() As Robotics.Base.DTOs.roWsState
        Get
            If oPunchState Is Nothing Then oPunchState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oPunchState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oPunchState = value
        End Set
    End Property

    Public Property LabAgreeState() As Robotics.Base.DTOs.roWsState
        Get
            If oLabAgreeState Is Nothing Then oLabAgreeState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oLabAgreeState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oLabAgreeState = value
        End Set
    End Property

    Public Property SecurityState() As Robotics.Base.DTOs.roWsState
        Get
            If oSecurityState Is Nothing Then oSecurityState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSecurityState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSecurityState = value
        End Set
    End Property

    Public Property AccessMoveState() As Robotics.Base.DTOs.roWsState
        Get
            If oAccessMoveState Is Nothing Then oAccessMoveState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oAccessMoveState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oAccessMoveState = value
        End Set
    End Property

    Public Property AuditState() As Robotics.Base.DTOs.roWsState
        Get
            If oAuditState Is Nothing Then oAuditState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oAuditState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oAuditState = value
        End Set
    End Property

    Public Property GroupFeatureState() As Robotics.Base.DTOs.roWsState
        Get
            If oGroupFeatureState Is Nothing Then oGroupFeatureState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oGroupFeatureState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oGroupFeatureState = value
        End Set
    End Property

    Public Property CalendarV2State() As Robotics.Base.DTOs.roWsState
        Get
            If oCalendarV2State Is Nothing Then oCalendarV2State = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCalendarV2State
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCalendarV2State = value
        End Set
    End Property

    Public Property SecurityV2State() As Robotics.Base.DTOs.roWsState
        Get
            If oSecurityV2State Is Nothing Then oSecurityV2State = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSecurityV2State
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSecurityV2State = value
        End Set
    End Property

    Public Property CalendarPeriodCoverageState() As Robotics.Base.DTOs.roWsState
        Get
            If oCalendarPeriodCoverageState Is Nothing Then oCalendarPeriodCoverageState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCalendarPeriodCoverageState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCalendarPeriodCoverageState = value
        End Set
    End Property

    Public Property CalendarShiftV2State() As Robotics.Base.DTOs.roWsState
        Get
            If oCalendarShiftV2State Is Nothing Then oCalendarShiftV2State = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCalendarShiftV2State
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCalendarShiftV2State = value
        End Set
    End Property

    Public Property DocumentState() As Robotics.Base.DTOs.roWsState
        Get
            If oDocumentState Is Nothing Then oDocumentState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oDocumentState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oDocumentState = value
        End Set
    End Property

    Public Property ProgrammedHolidayState() As Robotics.Base.DTOs.roWsState
        Get
            If oProgrammedHolidayState Is Nothing Then oProgrammedHolidayState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oProgrammedHolidayState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oProgrammedHolidayState = value
        End Set
    End Property

    Public Property ProgrammedOvertimeState() As Robotics.Base.DTOs.roWsState
        Get
            If oProgrammedOvertimeState Is Nothing Then oProgrammedOvertimeState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oProgrammedOvertimeState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oProgrammedOvertimeState = value
        End Set
    End Property

    Public Property SDKState() As Robotics.Base.DTOs.roWsState
        Get
            If oSDKState Is Nothing Then oSDKState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oSDKState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oSDKState = value
        End Set
    End Property

    Public Property ProductiveUnitState() As Robotics.Base.DTOs.roWsState
        Get
            If oProductiveUnitState Is Nothing Then oProductiveUnitState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oProductiveUnitState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oProductiveUnitState = value
        End Set
    End Property

    Public Property BudgetState() As Robotics.Base.DTOs.roWsState
        Get
            If oBudgetState Is Nothing Then oBudgetState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oBudgetState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oBudgetState = value
        End Set
    End Property

    Public Property BudgetRowState() As Robotics.Base.DTOs.roWsState
        Get
            If oBudgetRowState Is Nothing Then oBudgetRowState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oBudgetRowState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oBudgetRowState = value
        End Set
    End Property

    Public Property BudgetRowPeriodState() As Robotics.Base.DTOs.roWsState
        Get
            If oBudgetRowPeriodState Is Nothing Then oBudgetRowPeriodState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oBudgetRowPeriodState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oBudgetRowPeriodState = value
        End Set
    End Property

    Public Property ScheduleRulesState() As Robotics.Base.DTOs.roWsState
        Get
            If oScheduleRulesState Is Nothing Then oScheduleRulesState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oScheduleRulesState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oScheduleRulesState = value
        End Set
    End Property

    Public Property LicenseState() As Robotics.Base.DTOs.roWsState
        Get
            If oLicenseState Is Nothing Then oLicenseState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oLicenseState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oLicenseState = value
        End Set
    End Property

    Public Property CommuniqueState() As Robotics.Base.DTOs.roWsState
        Get
            If oCommuniqueState Is Nothing Then oCommuniqueState = New Robotics.Base.DTOs.roWsState With {.Result = 0}
            Return oCommuniqueState
        End Get
        Set(value As Robotics.Base.DTOs.roWsState)
            oCommuniqueState = value
        End Set
    End Property

    Public Property ToDoListState() As Robotics.Base.VTToDoLists.roToDoListState
        Get
            If oToDoListState Is Nothing Then oToDoListState = New Robotics.Base.VTToDoLists.roToDoListState With {.Result = 0}
            Return oToDoListState
        End Get
        Set(value As Robotics.Base.VTToDoLists.roToDoListState)
            oToDoListState = value
        End Set
    End Property

    Public Property SurveyState() As Robotics.Base.VTSurveys.roSurveyState
        Get
            If oSurveyState Is Nothing Then oSurveyState = New Robotics.Base.VTSurveys.roSurveyState With {.Result = 0}
            Return oSurveyState
        End Get
        Set(value As Robotics.Base.VTSurveys.roSurveyState)
            oSurveyState = value
        End Set
    End Property

    Public Property ChannelState() As Robotics.Base.VTChannels.roChannelState
        Get
            If oChannelState Is Nothing Then oChannelState = New Robotics.Base.VTChannels.roChannelState With {.Result = 0}
            Return oChannelState
        End Get
        Set(value As Robotics.Base.VTChannels.roChannelState)
            oChannelState = value
        End Set
    End Property

    Public Property BotState() As Robotics.Base.VTBots.roBotState
        Get
            If oBotState Is Nothing Then oBotState = New Robotics.Base.VTBots.roBotState With {.Result = 0}
            Return oBotState
        End Get
        Set(value As Robotics.Base.VTBots.roBotState)
            oBotState = value
        End Set
    End Property

    Public Property CollectiveState() As Robotics.Base.VTCollectives.roCollectiveState
        Get
            If oCollectiveState Is Nothing Then oCollectiveState = New Robotics.Base.VTCollectives.roCollectiveState With {.Result = 0}
            Return oCollectiveState
        End Get
        Set(value As Robotics.Base.VTCollectives.roCollectiveState)
            oCollectiveState = value
        End Set
    End Property

    Public Property RulesState() As Robotics.Base.VTRules.roRulesState
        Get
            If oRulesState Is Nothing Then oRulesState = New Robotics.Base.VTRules.roRulesState With {.Result = 0}
            Return oRulesState
        End Get
        Set(value As Robotics.Base.VTRules.roRulesState)
            oRulesState = value
        End Set
    End Property

    Public Property ConversationState() As Robotics.Base.VTChannels.roConversationState
        Get
            If oConversationState Is Nothing Then oConversationState = New Robotics.Base.VTChannels.roConversationState With {.Result = 0}
            Return oConversationState
        End Get
        Set(value As Robotics.Base.VTChannels.roConversationState)
            oConversationState = value
        End Set
    End Property

    Public Property MessageState() As Robotics.Base.VTChannels.roMessageState
        Get
            If oMessageState Is Nothing Then oMessageState = New Robotics.Base.VTChannels.roMessageState With {.Result = 0}
            Return oMessageState
        End Get
        Set(value As Robotics.Base.VTChannels.roMessageState)
            oMessageState = value
        End Set
    End Property

    Public Property LogBookState() As Robotics.Base.VTChannels.roLogBookState
        Get
            If oLogBookState Is Nothing Then oLogBookState = New Robotics.Base.VTChannels.roLogBookState With {.Result = 0}
            Return oLogBookState
        End Get
        Set(value As Robotics.Base.VTChannels.roLogBookState)
            oLogBookState = value
        End Set
    End Property

End Class