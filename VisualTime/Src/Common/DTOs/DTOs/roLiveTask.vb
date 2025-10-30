Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum roLiveAnalyticType
        <EnumMember> Schedule = 0
        <EnumMember> CostCenter = 1
        <EnumMember> Access = 2
        <EnumMember> Tasks = 3
        <EnumMember> KeepAlive = 4
        <EnumMember> Audit = 5
    End Enum

    <DataContract>
    Public Enum roLiveTaskStatus
        <EnumMember> All
        <EnumMember> Stopped
        <EnumMember> Running
        <EnumMember> Finished
    End Enum

    Public Enum roLiveEngineTaskStatus
        <EnumMember> Pending
        <EnumMember> Running
        <EnumMember> Completed
    End Enum

    <DataContract>
    Public Enum roLiveTaskTypes
        <EnumMember> CopyPlan
        <EnumMember> AssignWeekPlan
        <EnumMember> AssignTemplate
        <EnumMember> CopyAdvancedPlan
        <EnumMember> MassCopy
        <EnumMember> Import
        <EnumMember> Export
        <EnumMember> MassCause
        <EnumMember> ExportPunch
        <EnumMember> StartPunchConnector
        <EnumMember> StopPunchConnector
        <EnumMember> JustifiedIncidences
        <EnumMember> EmployeeMessage
        <EnumMember> MassPunch
        <EnumMember> AssignCenters
        <EnumMember> DeleteOldPhotos
        <EnumMember> DeleteOldBiometricData
        <EnumMember> CheckCloseDate
        <EnumMember> ManageVisits
        <EnumMember> PurgeNotifications
        <EnumMember> CompleteTasksAndProjects
        <EnumMember> CustomExport
        <EnumMember> SecurityPermissions
        <EnumMember> CopyAdvancedPlanv2
        <EnumMember> DeleteOldDocuments
        <EnumMember> ValidityDocuments
        <EnumMember> DocumentTracking
        <EnumMember> CheckAbsenceTrackingLegacy
        <EnumMember> AssignTemplatev2
        <EnumMember> CopyAdvancedBudgetPlan
        <EnumMember> ReportTask
        <EnumMember> AnalyticsTask
        <EnumMember> DeleteAccessMovesHistory
        <EnumMember> AIPlannerTask
        <EnumMember> MassProgrammedAbsence
        <EnumMember> MassMarkConsents
        <EnumMember> GenerateRoboticsPermissions
        <EnumMember> ConsolidateData
        <EnumMember> RecalculatePunchDirection
        <EnumMember> CheckScheduleRulesFaults
        <EnumMember> KeepAlive
        <EnumMember> MassLockDate
        <EnumMember> MigrateDocsToAzure
        <EnumMember> ReportTaskDX
        <EnumMember> AddReportToDocManager
        <EnumMember> CheckInvalidEntries
        <EnumMember> SynchronizeTerminals
        <EnumMember> RunEngine
        <EnumMember> RunEngineEmployee
        <EnumMember> UpdateEngineCache
        <EnumMember> RecalculateRequestStatus
        <EnumMember> BroadcasterTask
        <EnumMember> ChangeRequestPermissions
        <EnumMember> RemoveExpiredTasks
        <EnumMember> CheckDaywithunmatchedtimerecord_OBSOLETE
        <EnumMember> ValidateSignStatusDocument
        <EnumMember> CacheControl
        <EnumMember> SendEmail
        <EnumMember> CheckAutomaticRequests
        <EnumMember> GenerateNotifications
        <EnumMember> SendNotifications
        <EnumMember> GenerateDatalinkTasks
        <EnumMember> GenerateAnalyticsTasks
        <EnumMember> GenerateReportsDxTasks
        <EnumMember> SendPushNotification
        <EnumMember> PunchConnectorTask
        <EnumMember> DeleteOldAudit
        <EnumMember> DeleteOldPunches
        <EnumMember> BlockInactivePassports
        <EnumMember> EmployeeSecurtiyActions
        <EnumMember> CheckConcurrenceData
        <EnumMember> CTAIMA
        <EnumMember> DeleteOldComplaints
        <EnumMember> DataMonitoring
        <EnumMember> Suprema
        <EnumMember> ResetCompanyCache
    End Enum

    <DataContract>
    Public Enum roLiveQueueTypes
        <EnumMember> analytics
        <EnumMember> reports
        <EnumMember> documents
        <EnumMember> eog
        <EnumMember> datalink
        <EnumMember> broadcaster
        <EnumMember> test
        <EnumMember> scheduler
        <EnumMember> engine
        <EnumMember> audit
        <EnumMember> logbook
        <EnumMember> email
        <EnumMember> notifications
        <EnumMember> pushnotifications
        <EnumMember> upgrade
        <EnumMember> connector
        <EnumMember> punchphotos
        <EnumMember> vtlive
        <EnumMember> vtportal
        <EnumMember> terminals
        <EnumMember> visits
        <EnumMember> vtliveapi
        <EnumMember> analyticsbi
        <EnumMember> pnlink
        <EnumMember> dinner
    End Enum

    <DataContract>
    Public Enum roLiveDatalinkFolders
        <EnumMember> import
        <EnumMember> export
        <EnumMember> templates
        <EnumMember> punches
        <EnumMember> connector
        <EnumMember> custom
        <EnumMember> certificates
    End Enum

    <DataContract>
    Public Enum roPatternTypes
        <EnumMember> none
        <EnumMember> asterisc
        <EnumMember> dateandtime
    End Enum

End Namespace