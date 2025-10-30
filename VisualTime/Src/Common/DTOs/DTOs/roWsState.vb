Imports System.Runtime.Serialization
Imports System.Web

Namespace Robotics.Base.DTOs

    <DataContract,
    KnownType(GetType(TerminalBaseResultEnum)),
    KnownType(GetType(CrystalReportsResultEnum)),
    KnownType(GetType(SecurityResultEnum)),
    KnownType(GetType(GroupFeatureResultEnum)),
    KnownType(GetType(SecurityNodeResultEnum)),
    KnownType(GetType(WscResultEnum)),
    KnownType(GetType(PunchResultEnum)),
    KnownType(GetType(UserFieldResultEnum)),
    KnownType(GetType(EmployeeResultEnum)),
    KnownType(GetType(DocumentResultEnum)),
    KnownType(GetType(HolidayResultEnum)),
    KnownType(GetType(OvertimeResultEnum)),
    KnownType(GetType(SDKResultEnum)),
    KnownType(GetType(ProductiveUnitResultEnum)),
    KnownType(GetType(BudgetResultEnum)),
    KnownType(GetType(BudgetRowResultEnum)),
    KnownType(GetType(ScheduleRulesResultEnum)),
    KnownType(GetType(BudgetRowPeriodDataResultEnum)),
    KnownType(GetType(CalendarV2ResultEnum)),
    KnownType(GetType(CalendarShiftResultEnum)),
    KnownType(GetType(CalendarPeriodCoverageResultEnum)),
    KnownType(GetType(DataLinkGuideResultEnum)),
    KnownType(GetType(AccessGroupResultEnum)),
    KnownType(GetType(SchedulerResultEnum)),
    KnownType(GetType(ActivityResultEnum)),
    KnownType(GetType(AccessMoveResultEnum)),
    KnownType(GetType(AccessPeriodResultEnum)),
    KnownType(GetType(AssignmentResultEnum)),
    KnownType(GetType(CameraResultEnum)),
    KnownType(GetType(CaptureResultEnum)),
    KnownType(GetType(ZoneResultEnum)),
    KnownType(GetType(AuditResultEnum)),
    KnownType(GetType(IndicatorResultEnum)),
    KnownType(GetType(NotificationResultEnum)),
    KnownType(GetType(CostCenterResultEnum)),
    KnownType(GetType(BusinessCenterResultEnum)),
    KnownType(GetType(BusinessCenterZoneResultEnum)),
    KnownType(GetType(CauseResultEnum)),
    KnownType(GetType(MoveResultEnum)),
    KnownType(GetType(IncidenceResultEnum)),
    KnownType(GetType(EventSchedulerResultEnum)),
    KnownType(GetType(PassportConsentResultEnum)),
    KnownType(GetType(AdvancedParameterResultEnum)),
    KnownType(GetType(UserTaskResultEnum)),
    KnownType(GetType(SecurityOptionsResultEnum)),
    KnownType(GetType(DocumentAbsenceResultEnum)),
    KnownType(GetType(ShiftResultEnum)),
    KnownType(GetType(ConnectorResultEnum)),
    KnownType(GetType(LiveTasksResultEnum)),
    KnownType(GetType(ContractsResultEnum)),
    KnownType(GetType(LabAgreeResultEnum)),
    KnownType(GetType(DiningRoomResultEnum)),
    KnownType(GetType(TaskResultEnum)),
    KnownType(GetType(EmployeeTaskResultEnum)),
    KnownType(GetType(TaskFieldResultEnum)),
    KnownType(GetType(ProgrammedCausesResultEnum)),
    KnownType(GetType(ProgrammedAbsencesResultEnum)),
    KnownType(GetType(DataLinkResultEnum)),
    KnownType(GetType(GuiStateResultEnum)),
    KnownType(GetType(RequestResultEnum)),
    KnownType(GetType(GroupResultEnum)),
    KnownType(GetType(ConceptResultEnum)),
    KnownType(GetType(ReportResultEnum)),
    KnownType(GetType(CalendarRowHourDataResultEnum)),
    KnownType(GetType(CommuniqueResultEnum)),
    KnownType(GetType(EngineResultEnum))>
    Public Class roWsState

        Public Sub New()
            Me.IDPassport = -1
            Me.SessionID = String.Empty
            Me.ClientAddress = String.Empty
            Me.Result = String.Empty
            Me.ErrorText = String.Empty
            Me.ErrorDetail = String.Empty
            Me.ErrorNumber = 0
            Me.ReturnCode = String.Empty
            Me.Context = Nothing
            Me.AppSource = roAppSource.unknown
            Me.AppType = roAppType.Unknown
        End Sub

        <DataMember>
        Public Property IDPassport As Integer

        <DataMember>
        Public Property SessionID As String

        <DataMember>
        Public Property ClientAddress As String

        <DataMember>
        Public Property Result As Object

        <DataMember>
        Public Property ErrorText As String

        <DataMember>
        Public Property ErrorDetail As String

        <DataMember>
        Public Property ErrorNumber As Integer

        <DataMember>
        Public Property ReturnCode As String

        Public Property Context As HttpContext

        <DataMember>
        Public Property AppSource As roAppSource
        <DataMember>
        Public Property AppType As roAppType

    End Class

    Public Class roWsTerminalState

        Public Sub New()
            Me.IDTerminal = -1
            Me.SessionID = String.Empty
            Me.ClientAddress = String.Empty
            Me.Result = 0
            Me.ErrorText = String.Empty
            Me.ErrorDetail = String.Empty
            Me.ErrorNumber = 0
            Me.ReturnCode = String.Empty
        End Sub

        <DataMember>
        Public Property IDTerminal As Integer

        <DataMember>
        Public Property SessionID As String

        <DataMember>
        Public Property ClientAddress As String

        <DataMember>
        Public Property Result As Integer

        <DataMember>
        Public Property ErrorText As String

        <DataMember>
        Public Property ErrorDetail As String

        <DataMember>
        Public Property ErrorNumber As Integer

        <DataMember>
        Public Property ReturnCode As String

        <DataMember>
        Public Property AppSource As roAppSource
        <DataMember>
        Public Property AppType As roAppType
    End Class

    <DataContract>
    Public Enum VTLiveLogLevel
        <EnumMember> None = 0
        <EnumMember> Coded = 1
        <EnumMember> Info = 2
        <EnumMember> Warning = 3
        <EnumMember> Fail = 4
        <EnumMember> Debug = 5
    End Enum

    <DataContract>
    Public Enum CrystalReportsResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> ErrorRequestPrinters
    End Enum

    <DataContract>
    Public Enum TerminalBaseResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> Siren_TerminalNotExist
        <EnumMember> Reader_ActivityRequired
        <EnumMember> InvalidConfiguration
        <EnumMember> Reader_InvalidConfiguration
        <EnumMember> Reader_InvalidOutputDuration
        <EnumMember> Reader_InvalidInvalidOutputDuration
        <EnumMember> Reader_InvalidCustomButtonConfiguration
        <EnumMember> Reader_InvalidAccessZone
        <EnumMember> LivePortalTerminalAlreadyExist
        <EnumMember> Reader_InvalidAccessZoneOut
        <EnumMember> ReleAlreadyUsed
        <EnumMember> InvalidRegistrationCode
        <EnumMember> SecurityTokenNotValid
        <EnumMember> TerminalDoesNotExists
        <EnumMember> ServerStopped
        <EnumMember> IncompatibleTerminalsExists
        <EnumMember> Reader_InvalidZonesForTimegate
        <EnumMember> Reader_InvalidCostCenter
    End Enum

    <DataContract>
    Public Enum SecurityResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> PassportInactive
        <EnumMember> PassportAuthenticationIncorrect
        <EnumMember> AuthenticationMethodsIncorrect
        <EnumMember> AuthenticationMethodPasswordIncorrect
        <EnumMember> AuthenticationMethodsIntegratedSecurityIncorrect
        <EnumMember> AuthenticationMethodsCardIncorrect
        <EnumMember> AuthenticationMethodsBiometryIncorrect
        <EnumMember> AuthenticationMethodsPinIncorrect
        <EnumMember> AuthenticationMethodsPlateIncorrect
        <EnumMember> IsUsersAdminForCurrentUser
        <EnumMember> MaxCurrentSessionsExceeded
        <EnumMember> UpdateSessionError
        <EnumMember> DeleteSessionError
        <EnumMember> ServerStopped
        <EnumMember> MaxCurrentSessionsNotAvailable
        <EnumMember> LevelAuthorityPassportAndPassportParentWithout '<-  "el passport no tiene nivel de mando y su passportParent tampoco
        <EnumMember> LevelAuthorityPassportParentWithout '<-  "el passportParent no tiene nivel de mando
        <EnumMember> LevelAuthorityPassportParentTooHight '<- el mando del passport no puede ser inferior al mando de su padre"
        <EnumMember> LevelAuthorityPassportChildsTooLow '<-Alguno de los hijos tiene nivel de mando inferior al de este passport"
        <EnumMember> BloquedAccessApp
        <EnumMember> TemporayBloqued
        <EnumMember> IsExpired
        <EnumMember> InvalidApp
        <EnumMember> GeneralBlockAccess
        <EnumMember> InvalidClientLocation
        <EnumMember> NeedTemporaryKeyRequest
        <EnumMember> NeedTemporaryKeyRequestExpired
        <EnumMember> InvalidVersionAPP
        <EnumMember> PassportDoesNotExists
        <EnumMember> SessionExpired
        <EnumMember> NeedMailRequest
        <EnumMember> NeedTemporaryKeyRequestRobotics
        <EnumMember> NeedTemporaryKeyRequestExpiredRobotics
        <EnumMember> AlreadyLoggedinInOtherLocation
        <EnumMember> SessionInvalidatedByPermissions
        <EnumMember> SessionInvalidatedOtherUserWithSameSession
        <EnumMember> SupervisorHasFiltersDefined
        <EnumMember> SecurityTokenNotValid
        <EnumMember> RoboticsUserAllowed
        <EnumMember> InvalidRoboticsAccountFormat
        <EnumMember> AuthenticationMethodsNFCIncorrect
        <EnumMember> XSSvalidationError
        ' ...
    End Enum

    <DataContract>
    Public Enum WscResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> PassportInactive
        <EnumMember> PassportAuthenticationIncorrect
        <EnumMember> AuthenticationMethodsIncorrect
        <EnumMember> AuthenticationMethodPasswordIncorrect
        <EnumMember> AuthenticationMethodsIntegratedSecurityIncorrect
        <EnumMember> AuthenticationMethodsCardIncorrect
        <EnumMember> AuthenticationMethodsBiometryIncorrect
        <EnumMember> AuthenticationMethodsPinIncorrect
        <EnumMember> AuthenticationMethodsPlateIncorrect
        <EnumMember> AuthenticationMethodsNFCIncorrect
    End Enum

    <DataContract>
    Public Enum PunchResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidChangeStateTime
    End Enum

    <DataContract>
    Public Enum CalendarShiftResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
    End Enum

    <DataContract>
    Public Enum ScheduleRulesResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
        <EnumMember> CannotDeleteScheduleRuleInUse
        <EnumMember> CannotDisableScheduleRuleInUse
    End Enum

    <DataContract>
    Public Enum SDKResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SQLError
    End Enum

    <DataContract>
    Public Enum DocumentResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidName
        <EnumMember> EmptyName
        <EnumMember> InvalidShortName
        <EnumMember> EmptyShortName
        <EnumMember> TemplateInUse
        <EnumMember> EmployeeRequired
        <EnumMember> CompanyRequired
        <EnumMember> NoPermissionOverEmployee
        <EnumMember> NoPermissionOverGroup
        <EnumMember> InvalidMobileNumber
        <EnumMember> EmptyFile
        <EnumMember> ErrorUploadingDocumentToSign
        <EnumMember> PDFDocumentRequired
        <EnumMember> NumberOfSignedDocumentsExceeded
        <EnumMember> ContractRequired
        <EnumMember> EmployeeRequiredPatternAlt1
        <EnumMember> XSSvalidationError
        <EnumMember> InvalidA3PayrollFormat
        <EnumMember> A3PayrollDocumentEmpty
        <EnumMember> A3MoreThanOneTemplate
        <EnumMember> ExternalIdDuplicated
        <EnumMember> ErrorDeletingDocument
    End Enum

    <DataContract>
    Public Enum HolidayResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidDateTimeInterval
        <EnumMember> InvalidDate
        <EnumMember> AnotherAbsenceExistInDate
        <EnumMember> AnotherHolidayExistInDate
        <EnumMember> DateOutOfContract
        <EnumMember> InFreezeDate
        <EnumMember> InHolidayPlanification
        <EnumMember> AnotherOvertimeExistInDate
        <EnumMember> NotAllowedCause
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum OvertimeResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidDateTimeInterval
        <EnumMember> InvalidDate
        <EnumMember> AnotherAbsenceExistInDate
        <EnumMember> AnotherHolidayExistInDate
        <EnumMember> DateOutOfContract
        <EnumMember> InFreezeDate
        <EnumMember> InHolidayPlanification
        <EnumMember> InvalidDuration
        <EnumMember> AnotherExistInDay
        <EnumMember> NotAllowedCause
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ProductiveUnitResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
        <EnumMember> InvalidName
        <EnumMember> InvalidShortName
        <EnumMember> UPExistinBudget
        <EnumMember> UP_ModeExistinBudget
        <EnumMember> UP_Mode_Position_InvalidQuantity
        <EnumMember> UP_Mode_Position_InvalidAssignment
        <EnumMember> UP_Mode_Position_InvalidShift
    End Enum

    <DataContract>
    Public Enum BudgetResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
        <EnumMember> PermissionDenied
    End Enum

    <DataContract>
    Public Enum BudgetRowResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
    End Enum

    <DataContract>
    Public Enum BudgetRowPeriodDataResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidData
    End Enum

    <DataContract>
    Public Enum CalendarV2ResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
        <EnumMember> RowDayData_EmployeeNotExist
        <EnumMember> RowDayData_PlannedDayNotExist
        <EnumMember> RowDayData_ShiftNotExist
        <EnumMember> RowDayData_ShiftBaseRequired
        <EnumMember> RowDayData_PermissionDeniedOverEmployee
        <EnumMember> RowDayData_EmployeeWithoutContractOnDate
        <EnumMember> RowDayData_ShiftBaseShouldBeWorking
        <EnumMember> RowDayData_AssignmentDataInvalid
    End Enum

    <DataContract>
    Public Enum CalendarPeriodCoverageResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
    End Enum

    <DataContract>
    Public Enum PassportConsentResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
    End Enum

    <DataContract>
    Public Enum AccessGroupResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoDeleteforEmployeesAssigned
        <EnumMember> EventSchedulerAssigned
        <EnumMember> ShortNameEmpty
        <EnumMember> ShortNameAlreadyExists
        <EnumMember> NameToLong
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ConnectorResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
    End Enum

    <DataContract>
    Public Enum SchedulerResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidIDPassport
        <EnumMember> InvalidScheduler
        <EnumMember> RemarksConfiguration
        <EnumMember> InvalidScheduleTemplateName
        <EnumMember> DailyScheduleLockedDay
        <EnumMember> DailyCoverageAssignmentRepited
        <EnumMember> DailyCoverageAssignmentInvalidExpectedCoverage
        <EnumMember> DailyScheduleCoverageDay
        <EnumMember> NoEmployeesAffected
    End Enum

    <DataContract>
    Public Enum CompareTypeEnum
        <EnumMember> Equal
        <EnumMember> Minor
        <EnumMember> MinorEqual
        <EnumMember> Major
        <EnumMember> MajorEqual
        <EnumMember> Distinct
        <EnumMember> Contains
        <EnumMember> NotContains
        <EnumMember> StartWith
        <EnumMember> EndWidth
    End Enum

    <DataContract>
    Public Enum AnalyticsResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> ErrorRequestPrinters
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum FieldTypesEnum
        <EnumMember> tText
        <EnumMember> tNumeric
        <EnumMember> tDate
        <EnumMember> tDecimal
        <EnumMember> tTime
        <EnumMember> tList
        <EnumMember> tDatePeriod
        <EnumMember> tTimePeriod
        <EnumMember> tLink
        <EnumMember> tDocument
    End Enum

    <DataContract>
    Public Enum AccessLevelsEnum
        <EnumMember> aLow
        <EnumMember> aMedium
        <EnumMember> aHigh
    End Enum

    <DataContract>
    Public Enum TypesEnum
        <EnumMember> EmployeeField
        <EnumMember> GroupField
        <EnumMember> TaskField
        <EnumMember> TaskEmployeeField
    End Enum

    <DataContract>
    Public Enum AccessValidationEnum
        <EnumMember> None
        <EnumMember> Required
        <EnumMember> Warning
    End Enum

    <DataContract>
    Public Enum CompareValueTypeEnum
        <EnumMember> DirectValue
        <EnumMember> CurrentDate
    End Enum

    <DataContract>
    Public Enum ActivityResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidName
        <EnumMember> InvalidActivityID
        <EnumMember> InvalidGroupID
        <EnumMember> GroupIDRepited
        <EnumMember> ActivityAssignedInTerminalReader

    End Enum

    <DataContract>
    Public Enum AccessMoveResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError

    End Enum

    <DataContract>
    Public Enum AccessPeriodResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> AccessPeriodInUse
        <EnumMember> InvalidName
        <EnumMember> NameAlreadyExist
        <EnumMember> DuplicatedPeriodsDays
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum AssignmentResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NameCannotBeNull
        <EnumMember> NameAlreadyExist
        <EnumMember> UsedInEmployeeAssignments
        <EnumMember> UsedInShiftAssignments
        <EnumMember> UsedInSchedulerAssignments
        <EnumMember> UsedInCoverageAssignments
        <EnumMember> ShortNameCannotBeNull
        <EnumMember> ShortNameAlreadyExist
        <EnumMember> ShortNameToLong
        <EnumMember> ExportAlreadyExist
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ReportResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
    End Enum

    <DataContract>
    Public Enum CameraResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> CameraInTerminalReaders
        <EnumMember> CameraInZones
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum CaptureResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError

    End Enum

    <DataContract>
    Public Enum ZoneResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> ZoneInTerminalReaders
        <EnumMember> ZoneInGroupPermissions
        <EnumMember> ZonePlaneInZone
        <EnumMember> InvalidName
        <EnumMember> NameAlreadyExist
        <EnumMember> OverlapedInactiviy
        <EnumMember> ZoneInBusinessCenter
        <EnumMember> ZoneIpRestrictionChange
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum AuditResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidPassport

    End Enum

    <DataContract>
    Public Enum IndicatorResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NameCannotBeNull
        <EnumMember> NameAlreadyExist
        <EnumMember> IDFirstConceptCannotBeNull
        <EnumMember> IDSecondConceptCannotBeNull
        <EnumMember> DesiredValueCannotBeNull
        <EnumMember> LimitValueCannotBeNull
        <EnumMember> ConceptsTypeDiffer
        <EnumMember> UsedInGroupAssignments
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum CauseResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidName
        <EnumMember> NameAlreadyExist
        <EnumMember> InvalidShortName
        <EnumMember> ShortNameAlreadeyExist
        <EnumMember> InvalidRoundingBy
        <EnumMember> InvalidReaderInputCode
        <EnumMember> InvalidMaxProgrammedAbsence
        <EnumMember> CauseUsedInShift
        <EnumMember> CauseUsedInAccrual
        <EnumMember> CauseUsedInProgrammedAbsence
        <EnumMember> CauseUsedInProgrammedCause
        <EnumMember> CauseUsedInPunchWithIncidence
        <EnumMember> ReaderInputCodeExistent
        <EnumMember> InvalidJustifyPeriodDates
        <EnumMember> CauseUsedInRequest
        <EnumMember> NumberOfCausesExceeded
        <EnumMember> UserFieldEmpty
        <EnumMember> CauseUsedinCauses
        <EnumMember> CauseUsedInProgrammedHoliday
        <EnumMember> ConceptBalanceAnnualQueryRequired
        <EnumMember> CauseUsedInProgrammedOvertime
        <EnumMember> AutomaticEquivalenceDataInvalid
        <EnumMember> CauseUsedInEquivalenceData
        <EnumMember> ChangeTypeNotAllowed
        <EnumMember> TypeDoesNotAllowDocumentTracking
        <EnumMember> CauseUsedInRequestRule
        <EnumMember> CauseUsedInLabAgree
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum NotificationResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NoSelectedType
        <EnumMember> IncorrectConditions
        <EnumMember> ConditionsIncorrectDaysBefore
        <EnumMember> ConditionsNoIDCause
        <EnumMember> ConditionsIncorrectDaysNoWorking
        <EnumMember> IncorrectSelectionDestination
        <EnumMember> NotificacionNameAlreadyExist
        <EnumMember> WithoutName
        <EnumMember> ConditionsNoIDConcept
        <EnumMember> ConditionsNoCompareType
        <EnumMember> ConditionsNoTargetTypeConcept
        <EnumMember> ConditionsNoTargetConcept
        <EnumMember> NotEnoughInformation
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum CostCenterResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError

    End Enum

    <DataContract>
    Public Enum WebLinksResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError

    End Enum

    <DataContract>
    Public Enum BusinessCenterResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NameCannotBeNull
        <EnumMember> NameAlreadyExist
        <EnumMember> UsedOnPassports
        <EnumMember> UsedOnTasks
        <EnumMember> UsedOnGroups
        <EnumMember> UsedOnEmployees
        <EnumMember> UsedOnPunches
        <EnumMember> UsedOnDailyCauses
        <EnumMember> UsedOnShifts
        <EnumMember> NonExistentZone
        <EnumMember> NonExistentEmployee
        <EnumMember> InvalidFormat
        <EnumMember> InvalidDate
        <EnumMember> UsedOnProductiveUnit
        <EnumMember> XSSvalidationError
        <EnumMember> UsedOnTerminals
    End Enum

    <DataContract>
    Public Enum BusinessCenterZoneResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoDeleteforEmployeesAssigned
    End Enum

    <DataContract>
    Public Enum MoveResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidChangeStateTime
    End Enum

    <DataContract>
    Public Enum IncidenceResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
    End Enum

    Public Enum EventSchedulerResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidName
        <EnumMember> InvalidShortName
        <EnumMember> InvalidDate
        <EnumMember> DuplicateDate
        <EnumMember> DuplicateName
        <EnumMember> DuplicateShortName
        <EnumMember> EmptyAuthorizations
        <EnumMember> InvalidEventID
        <EnumMember> InvalidGroupID
        <EnumMember> GroupIDRepited
        <EnumMember> InvalidAuthorizationDate
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum DiningRoomResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> DiningRoomNameRequired
        <EnumMember> DiningRoomNameAlreadyExist
        <EnumMember> DiningRoomNotEmpty
        <EnumMember> ErrorGeneratingNewID
        <EnumMember> IncorrectValue
        <EnumMember> InvalidBeginDate
        <EnumMember> InvalidFinishDate
        <EnumMember> InvalidPeriod
        <EnumMember> DaysOfWeekEmpty
        <EnumMember> ErrorSavingData
        <EnumMember> UserFieldEmpty
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ServiceApiResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NoCredentials
    End Enum

    <DataContract>
    Public Enum AdvancedParameterResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
    End Enum

    <DataContract>
    Public Enum UserTaskResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
    End Enum

    <DataContract>
    Public Enum SecurityOptionsResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ErrorGeneratingNewID
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> PermanenBlockAccessError
        <EnumMember> InvalidKeyRequierementValues
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum DailyRecordPunchesResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> InvalidSequence
        <EnumMember> PunchesNumberShouldBeEven
        <EnumMember> PunchesListCantBeEmpty
        <EnumMember> PunchesOverlaped
        <EnumMember> PunchRepeated
    End Enum

    Public Enum DocumentAbsenceResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidName
        <EnumMember> NameAlreadyExist
        <EnumMember> InvalidShortName
        <EnumMember> ShortNameAlreadeyExist
        <EnumMember> DocumentUsedInAbsenceTracking
    End Enum

    <DataContract>
    Public Enum EmployeeResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidEmployeeName
        <EnumMember> InvalidIDGroup
        <EnumMember> IDGroupNotExist
        <EnumMember> InvalidDateInterval
        <EnumMember> DuplicateUserField
        <EnumMember> InvalidUserField
        <EnumMember> InvalidUserdFieldType
        <EnumMember> InvalidSchedule
        <EnumMember> EmployeeNotExist
        <EnumMember> InvalidEmployeeType
        <EnumMember> InvalidAccessGroup
        <EnumMember> MobilityBadBeginDate
        <EnumMember> MobilityInvalidBeginDate
        <EnumMember> MobilityNoGroup
        <EnumMember> MobilityDifferentContractDate
        <EnumMember> MobilityDuplicateStartDate
        <EnumMember> MobilityDuplicateGroup
        <EnumMember> MobilityInvalidData
        <EnumMember> EmployeeNoContract
        <EnumMember> ShiftInvalidPrimaryShift
        <EnumMember> ShiftFullAlterShift
        <EnumMember> ShiftAlreadyAssigned
        <EnumMember> InvalidWeekShiftsList
        <EnumMember> DailyCausesInvalidData
        <EnumMember> EmployeeHaveJobMoves
        <EnumMember> EmployeeHaveDailyJobAccruals
        <EnumMember> EmployeeHaveTeamJobMoves
        <EnumMember> EmployeeNoActiveContract
        <EnumMember> InPeriodOfFreezing
        <EnumMember> InvalidSchedulerRemarksConfiguration
        <EnumMember> AccessDenied
        <EnumMember> DailyScheduleLockedDay
        <EnumMember> DailyScheduleCoverageDay
        <EnumMember> AssignmentInvalidSuitability
        <EnumMember> AssignmentRepited
        <EnumMember> AssignmentUsedOnScheduler
        <EnumMember> EmployeeCoveredIsCovering
        <EnumMember> EmployeeCoveredAlreadyCovered
        <EnumMember> EmployeeCoveredNoAssignment
        <EnumMember> DailyScheduleCoveredNotExist
        <EnumMember> EmployeeCoverageAlreadyCovering
        <EnumMember> EmployeeCoverageIsCovered
        <EnumMember> EmployeeCoverageEmployeeAssignmentIncompatibility
        <EnumMember> EmployeeCoverageShiftAssignmentIncompatibility
        <EnumMember> EmployeePlanNotExist
        <EnumMember> ShiftWithoutPermission
        <EnumMember> FileWithoutPermissionToRead
        <EnumMember> FileNotExists
        <EnumMember> FileInvalidName
        <EnumMember> FileNotEspecified
        <EnumMember> NoBaseShiftAssigned
        <EnumMember> NoWorkingDay
        <EnumMember> LastMobilidityDeleteError
        <EnumMember> XSSvalidationError
    End Enum

    Public Enum ShiftResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NameAlreadyExist
        <EnumMember> ErrorSavingShiftData
        <EnumMember> ErrorGeneratingNewID
        <EnumMember> ErrorOnMarkShiftAsObsolete
        <EnumMember> ErrorOnChangeObsoleteName
        <EnumMember> ErrorOnChangeExportName
        <EnumMember> ShiftRule_InvalidIncidence
        <EnumMember> ShiftRule_InvalidZone
        <EnumMember> ShiftRule_InvalidCause
        <EnumMember> ShiftLayer_LayerIDExistent
        <EnumMember> ShiftTimeZone_TimeZoneNotExist
        <EnumMember> ShiftTimeZone_TimePeriodIncorrect
        <EnumMember> FlexibleLayer_InvalidBegin
        <EnumMember> FlexibleLayer_InvalidFinish
        <EnumMember> FlexibleLayer_InvalidPeriod
        <EnumMember> FlexibleLayer_Collision
        <EnumMember> MandatoryLayer_InvalidBegin
        <EnumMember> Mandatory_InvalidFinish
        <EnumMember> MandatoryLayer_InvalidPeriod
        <EnumMember> MandatoryLayer_InvalidFloatingBegin
        <EnumMember> MandatoryLayer_FinishDoesNotMatchFloatingFinish
        <EnumMember> MandatoryLayer_FloatingFinish_Without_FloatingBegin
        <EnumMember> MandatoryLayer_Collision
        <EnumMember> BreakLayer_InvalidBegin
        <EnumMember> BreakLayer_InvalidFinish
        <EnumMember> BreakLayer_InvalidPeriod
        <EnumMember> BreakLayer_InvalidMinBreak
        <EnumMember> BreakLayer_InvalidMaxBreak
        <EnumMember> BreakLayer_NoParentMandatoryLayer
        <EnumMember> BreakLayer_Collision
        <EnumMember> PaidLayer_InvalidBegin
        <EnumMember> PaidLayer_InvalidFinish
        <EnumMember> PaidLayer_InvalidPeriod
        <EnumMember> PaidLayer_InvalidTarget
        <EnumMember> PaidLayer_InvalidValue
        <EnumMember> PaidLayer_NoParentBreakLayer
        <EnumMember> Filter_InvalidAction
        <EnumMember> Filter_InvalidTarget
        <EnumMember> Filter_InvalidValue
        <EnumMember> Filter_InvalidBegin
        <EnumMember> Filter_InvalidFinish
        <EnumMember> Filter_InvalidPeriod
        <EnumMember> TimeZone_ErrorNameIncorrect
        <EnumMember> TimeZone_AssignInZones
        <EnumMember> TimeZone_OverlappedZones
        <EnumMember> NameRequired
        <EnumMember> ShiftGroupNameRequired
        <EnumMember> ShiftGroupNameAlreadyExist
        <EnumMember> ShiftGroupNotEmpty
        <EnumMember> ShiftRule_ConditionUserFieldRequired
        <EnumMember> ShiftRule_ActionUserFieldRequired
        <EnumMember> ShiftRule_InvalidFromUserFieldType
        <EnumMember> ShiftRule_InvalidToUserFieldType
        <EnumMember> ShiftRule_InvalidBetweenUserFieldType
        <EnumMember> ShiftRule_InvalidActionUserFieldType
        <EnumMember> ShiftRule_InvalidConditionUserFields
        <EnumMember> StartFloatingRequired
        <EnumMember> TypeLockedShiftPlanned
        <EnumMember> VacationsShiftNotEmpty
        <EnumMember> ShiftAssignmentRepited
        <EnumMember> ShiftAssignmentInvalidCoverage
        <EnumMember> ShiftAssignmentAssigned
        <EnumMember> ShiftWithoutPermission
        <EnumMember> UserFieldEmpty
        <EnumMember> CollectivesEmpty
        <EnumMember> ShiftPlannedInFreezeDate
        <EnumMember> MandatoryLayer_InvalidFlexibleLayer
        <EnumMember> ShiftLayerCount
        <EnumMember> TypeLockedShiftPlannedBreakHours
        <EnumMember> TypeLockedShiftFloatingData
        <EnumMember> ExportNameAlreadyExist
        <EnumMember> ShiftLayerNumber
        <EnumMember> InvalidExpectedWorkingHours
        <EnumMember> ShiftRule_InvalidData
        <EnumMember> FlexibleLayer_LayerInsideMandatory
        <EnumMember> PatternPunchesShouldBeEven
        <EnumMember> PatternPunchesBadSequence
        <EnumMember> PatternPunchesOverlaped
        <EnumMember> PatternHasRepeatedPunch
        <EnumMember> HolidayConceptsQueryNotEqual
        <EnumMember> XSSvalidationError
        <EnumMember> ShiftInRule
        <EnumMember> ShiftInDailyRule
    End Enum

    Public Enum UserFieldResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidUserdFieldType
        <EnumMember> InvalidAccessLevel
        <EnumMember> UserFieldUsedInProcess
        <EnumMember> HistoryValues
        <EnumMember> UserFieldNameDuplicated
        <EnumMember> UserFieldNamePrivate
        <EnumMember> InPeriodOfFreezing
        <EnumMember> AccessDenied
        <EnumMember> InvalidName
        <EnumMember> ReadOnlyField
        <EnumMember> XSSvalidationError
        <EnumMember> UniqueValueAlreadyExists
        <EnumMember> UserFieldUsedAsIdForTimeGate
        <EnumMember> InvalidUniqueFieldType
    End Enum

    <DataContract>
    Public Enum TaskResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ErrorGeneratingNewID
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NameCannotBeNull
        <EnumMember> NameAlreadyExist
        <EnumMember> ShortNameCannotBeNull
        <EnumMember> ShortNameAlreadyExist
        <EnumMember> NegativeTime
        <EnumMember> CenterCannotBeNull
        <EnumMember> ProjectTemplateNotEmpty
        <EnumMember> WorkingInTask
        <EnumMember> BarCodeAlreadyExist
        <EnumMember> AssignmentRepited
        <EnumMember> AssignmentInOtherTask
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum EmployeeTaskResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoDeleteforEmployeesAssigned
    End Enum

    <DataContract>
    Public Enum GroupTaskResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoDeleteforGroupsAssigned
    End Enum

    <DataContract>
    Public Enum TaskFieldResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> TaskFieldCannotBeNull
        <EnumMember> TaskFieldUsedInTasks
    End Enum

    <DataContract>
    Public Enum ProgrammedAbsencesResultEnum

        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidDateInterval
        <EnumMember> AnotherExistInDateInterval
        <EnumMember> DateOutOfContract
        <EnumMember> ExistPunchesInBeginDate
        <EnumMember> InFreezeDate
        <EnumMember> ExistTrackingDays
        <EnumMember> AnotherHolidayExistInDate
        <EnumMember> AnotherOvertimeExistInDate
        <EnumMember> NotAllowedCause
        <EnumMember> XSSvalidationError

    End Enum

    <DataContract>
    Public Enum ProgrammedCausesResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidDateTimeInterval
        <EnumMember> InvalidDate
        <EnumMember> AnotherExistInDate
        <EnumMember> AnotherAbsenceExistInDate
        <EnumMember> DateOutOfContract
        <EnumMember> InFreezeDate
        <EnumMember> InvalidDuration
        <EnumMember> ExistTrackingDays
        <EnumMember> AnotherHolidayExistInDate
        <EnumMember> AnotherOvertimeinDate
        <EnumMember> NotAllowedCause
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum DataLinkResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoSheets
        <EnumMember> NoRegisters
        <EnumMember> InvalidColumns

        <EnumMember> InvalidEmployee
        <EnumMember> InvalidGroup
        <EnumMember> InvalidContract
        <EnumMember> ExpiredContract
        <EnumMember> InvalidCard
        <EnumMember> InvalidLogin
        <EnumMember> InvalidLanguage
        <EnumMember> InvalidMovility
        <EnumMember> InvalidUserGroup

        <EnumMember> InvalidAssignment

        <EnumMember> InvalidProfileMask
        <EnumMember> InvalidProfileType
        <EnumMember> InvalidMode
        <EnumMember> InvalidExportFileType
        <EnumMember> InvalidDelimiter
        <EnumMember> InvalidProfileName
        <EnumMember> InvalidDestination
        <EnumMember> InvalidImportFileName
        <EnumMember> InvalidExportFileName
        <EnumMember> InvalidStartCalculDay
        <EnumMember> InvalidStartHour
        <EnumMember> InvalidExcelFile
        <EnumMember> InvalidASCIIFile
        <EnumMember> InvalidXMLFile
        <EnumMember> InvalidCoverage
        <EnumMember> SomeRegistersNotImported
        <EnumMember> SomeRegistersAreInvalidFormat
        <EnumMember> FreezeDateException
        <EnumMember> InvalidCause
        <EnumMember> CalculateError
        <EnumMember> FormatColumnIsWrong
        <EnumMember> NumMaxEmployeesExceded
        <EnumMember> AuthorizationError
        <EnumMember> InvalidLabAgree
        <EnumMember> SomeUserFieldsNotSaved
        <EnumMember> FutureDate
        <EnumMember> FieldDataIncorrect

        <EnumMember> FieldCIFNotExists
        <EnumMember> FieldIberper_ComedorNotExists
        <EnumMember> FileIbermaticaIniNotFound
        <EnumMember> FileIbermaticaIniIncomplete
        <EnumMember> NoRegistersWithDetail
        <EnumMember> ErrorProcessingRowWithRBSScript
        <EnumMember> ErrorProcessingImportFileWithRBSScript
        <EnumMember> ErrorProcessingExportFileWithRBSScript

        <EnumMember> InvalidShift
        <EnumMember> InvalidData

        <EnumMember> InvalidAccrual
        <EnumMember> IllegalStatement
        <EnumMember> InvalidDocumentType
        <EnumMember> InvalidDocumentData
        <EnumMember> InvalidPhotoData
        <EnumMember> UnexistingDocumentTemplate

        <EnumMember> ErrorSavingDocument
        <EnumMember> DocumentNotDeliverable
        <EnumMember> DocumentTooBig
        <EnumMember> EmployeeDocumentAlreadyExists

        <EnumMember> NoDataSourceFile
        <EnumMember> NoPassportSpecified
        <EnumMember> PendingWSExport
        <EnumMember> UnableToRecoverExcelProfile
        <EnumMember> ErrorSendingExportResultFile
        <EnumMember> InvalidContractHistory

        <EnumMember> NoContracts
        <EnumMember> NoLabAgreesDefined

        <EnumMember> HasPunchesAfterContractEndDate

        <EnumMember> NoExportGuide
        <EnumMember> NoEmployeeSuitable
        <EnumMember> NoExportTask
        <EnumMember> NoExportParameters

        <EnumMember> NotAllowedChangeContractBeginDate

        <EnumMember> InvalidParentGroup

        <EnumMember> CTAIMAError
        <EnumMember> InvalidPinLength
        <EnumMember> MandatoryParameterNotDefined
        <EnumMember> InvalidRole
        <EnumMember> ContractDataProtected
        <EnumMember> NewEmployeeCannotBeSourceOfPlanning
        <EnumMember> ManualCauseNotExists
        <EnumMember> PGPEncryptionFailed
        <EnumMember> IVECOInvalidPeriod
        <EnumMember> IVECOInvalidBeginPeriod
        <EnumMember> IVECOEmptyEmployees
        <EnumMember> SupremaErrorGettingPunches

        <EnumMember> UnexistentDocument
        <EnumMember> ErrorDeletingDocument
        <EnumMember> ExternalIdDuplicated
        <EnumMember> DocumentDeletedButExternalIdStillExists
        <EnumMember> ErrorRecoveringData
    End Enum

    <DataContract>
    Public Enum DataLinkExportProfile
        <EnumMember> Accruals = 1
        <EnumMember> ProgrammedAbsenceLegacy = 2
        <EnumMember> DinningRoomLegacy = 3
        <EnumMember> PunchesLegacy = 4
        <EnumMember> TasksLegacy = 5
        <EnumMember> Employees = 6
        <EnumMember> CostCentersLegacy = 7
        <EnumMember> Punches = 8
        <EnumMember> DailyCauses = 9
        <EnumMember> EmployeeTasksLegacy = 10
        <EnumMember> PlanningLegacy = 11
        <EnumMember> Planning = 12
        <EnumMember> Requests = 13
        <EnumMember> Absences = 14
        <EnumMember> Tasks = 15
        <EnumMember> CostCenters = 16
        <EnumMember> DinningRoom = 17
        <EnumMember> Argal = 18
        <EnumMember> CustomAttendanceProductivity = 19 'Taberna del Cura
        <EnumMember> CustomIberper = 20 'Iberper        
        <EnumMember> IvecoMDOMadridD = 21 'MDO Madrid Diaria (ID 20015)
        <EnumMember> IvecoMDOMadridM = 22 'MDO Madrid Mensual (ID 20016) 
        <EnumMember> IvecoMDOLogisticMadridD = 23 'MDO Logistic Madrid Diaria (ID 20017)
        <EnumMember> IvecoMDOLogisticMadridM = 24 'MDO Logistic Madrid Mensual (ID 20018)
        <EnumMember> IvecoMDOMadridMWorkanalysis = 25 'MDO Madrid Mensual - Workanalysis (ID 20019).
        <EnumMember> IvecoMDOLogisticMWorkanalysis = 26 'MDO Logistic Mensual - Workanalysis (ID 20020).
        <EnumMember> IvecoMDOValladolidM = 27 'MDO Valladolid Mensual (ID 20014)
        <EnumMember> IvecoMDOValladolidD = 28 'MDO Valladolid- Diaria (ID 20013)
        <EnumMember> IvecoPlusPresenciaMadrid = 29 'Plus presencia- MADRID (ID 10016, 10017)
        <EnumMember> IvecoPlusPresenciaValladolid = 30 'Plus presencia- Valladolid 
        <EnumMember> IvecoWorkanalysisReportM = 31 'WORKANALISIS – REPORT MENSUAL (ID 10022)
        <EnumMember> IvecoWorkanalysisHoursSection = 32 'Workanalysis - Horas trabajadas diarias por sección  (ID 10021)
        <EnumMember> IvecoWorkanalysisHoursEmployee = 33 'Workanalysis - Horas trabajadas diarias por empleado   (ID 10020)
        <EnumMember> IvecoConguallo = 34 'Expotacion Conguallo (ID 10024)
        <EnumMember> IvecoJobHistory = 35 'Exportación Historial de Puestos  (ID 8995)
        <EnumMember> IvecoCategoryChange = 36 'Exportación HISTORIAL CAMBIO DE CATEGORIA (ID 8997)        
        <EnumMember> LivenDailyCauses = 37 'Exportación justificaciones diarias(ID 9880)
        <EnumMember> RosRocaDynamics = 38 'Exportación a dynamics (ID 9880)
        <EnumMember> TisvolPrimas = 39 'Exportación Calculo de Primas (10026)
        <EnumMember> TisvolPrimasSummary = 40 'Exportación Calculo de Primas Summary (10027)
        <EnumMember> TisvolPunches = 41 'Exportación Fichajes Tisvol (10028)
    End Enum

    <DataContract>
    Public Enum DataLinkImportProfile
        <EnumMember> Employees = 20
        <EnumMember> Planning = 21
        <EnumMember> Absences = 22
        <EnumMember> DailyCauses = 23
        <EnumMember> Tasks = 25
        <EnumMember> CostCenters = 26
        <EnumMember> Supervisors = 27
        <EnumMember> VSLWorkSheets = 12
    End Enum

    <DataContract>
    Public Enum DataLinkResultFileType
        <EnumMember> Excel = 1
        <EnumMember> Text = 2
    End Enum

    <DataContract>
    Public Enum ContractsResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> InvalidIDContract
        <EnumMember> InvalidDateInterval
        <EnumMember> ContractNotFound
        <EnumMember> InvalidCardID
        <EnumMember> InvalidDates
        <EnumMember> CardIDRepited
        <EnumMember> InvalidAuthenticationMethods
        <EnumMember> ContractActiveWithMovements
        <EnumMember> ContractInFreezeDate
        <EnumMember> ProgrammedAbsencesOutOfContracts
        <EnumMember> LabAgreeEmpty
        <EnumMember> LastContractDeleteError
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum LiveTasksResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> ActionEmpty
        <EnumMember> ParametersEmpty
        <EnumMember> TaskNotExecuted
        <EnumMember> CouldNotQueryStatus
        <EnumMember> FileNotFound
    End Enum

    <DataContract>
    Public Enum LabAgreeResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NameAlreadyExist
        <EnumMember> NameCannotBeNull
        <EnumMember> InvalidDateInterval
        <EnumMember> InvalidUsedField
        <EnumMember> UsedInEmployeeContracts
        <EnumMember> StartupValueNameInvalid
        <EnumMember> StartupValueNameAlreadyExists
        <EnumMember> StartupValueIDConceptEmpty
        <EnumMember> StartupValueIDConceptAlreadyExists
        <EnumMember> StartupValueUsedInLabAgree
        <EnumMember> StartupValueMaximumUserFieldEmpty
        <EnumMember> StartupValueStartUserFieldEmpty
        <EnumMember> StartupValueMinimumUserFieldEmpty
        <EnumMember> StartupValueNoUserFieldsSelected
        <EnumMember> LabAgreeAccrualsRulesUsedInLabAgree
        <EnumMember> LabAgreeRulesNameCannotBeNull
        <EnumMember> LabAgreeRulesIDAlreadyExists
        <EnumMember> LabAgreeRulesInFreezingDate
        <EnumMember> LabAgreeRulesBeginDateInFreezePeriod
        <EnumMember> LabAgreeRulesEndDateInFreezePeriod
        <EnumMember> LabAgreeRulesDuplicatedInEdition
        <EnumMember> OnlyOneLabAgreeAllowed
        <EnumMember> IncorrectDate
        <EnumMember> CauseLimitValueNameInvalid
        <EnumMember> CauseLimitValueNameAlreadyExists
        <EnumMember> CauseLimitValueIDCauseEmpty
        <EnumMember> CauseLimitValueIDCauseAlreadyExists
        <EnumMember> CauseLimitValueUsedInLabAgree
        <EnumMember> CauseLimitValueMaximumAnnualUserFieldEmpty
        <EnumMember> CauseLimitValueMaximumMonthlyUserFieldEmpty
        <EnumMember> CauseLimitValueNoUserFieldsSelected
        <EnumMember> LabAgreeCauseLimitEndDateInFreezePeriod
        <EnumMember> LabAgreeCauseLimitBeginDateInFreezePeriod
        <EnumMember> LabAgreeCauseLimitDuplicatedInEdition
        <EnumMember> RequestRuleNameAlreadyExist
        <EnumMember> RequestRuleWithoutName
        <EnumMember> RequestRuleReasonDuplicated
        <EnumMember> LabAgreeRulesConceptIncompatibleAction
        <EnumMember> UsedInConcept
        <EnumMember> UsedInCauseDocument
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum GuiStateResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
    End Enum

    <DataContract>
    Public Enum RequestResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> NoDeleteBecauseNotPending
        <EnumMember> IncorrectDates
        <EnumMember> NoApprovePermissions
        <EnumMember> UserFieldNoRequestVisible ' El campo de la ficha no es visible para solicitudes
        <EnumMember> NoApproveRefuseLevelOfAuthorityRequired ' No se puede aprobar/denegar la solicitud ya que un nivel de mando de rango superior ya la ha aprobado/denegado
        <EnumMember> UserFieldValueSaveError ' No se ha podido guardar el valor del campo de la ficha
        <EnumMember> InvalidPassport
        <EnumMember> ChangeShiftError ' No se ha podido cambiar la planificación del horario
        <EnumMember> VacationsOrPermissionsError ' No se ha podido planificar el periodo  de vacaciones
        <EnumMember> ExistsLockedDaysInPeriod ' Hay días bloqueados en el periodo a planificar
        <EnumMember> ForbiddenPunchError ' Error al validar el fichaje olvidado
        <EnumMember> JustifyPunchError ' Error al validar la justificación del fichaje
        <EnumMember> RequestMoveNotExist ' El fichaje relacionado con la solicitud no existe o se ha modificado
        <EnumMember> RequestMoveTooMany  ' Hay más de un fichaje relacionado con la solicitud
        <EnumMember> PlannedAbsencesError ' Error al validar la solicitud de ausencia prolongada
        <EnumMember> PlannedCausesError ' Error al validar la solicitud de incidencia prevista
        <EnumMember> ExternalWorkResumePartError ' Error al validar la solicitud de parte de trabajo externo
        <EnumMember> UserFieldRequired ' Campo de la ficha requerido
        <EnumMember> PunchDateTimeRequired ' Fecha y hora del fichaje requerida
        <EnumMember> CauseRequired ' Justificación requerida
        <EnumMember> DateRequired ' Fecha requerida
        <EnumMember> HoursRequired ' Horas requeridas
        <EnumMember> ShiftRequired ' Horario requerido
        <EnumMember> RequestRepited ' Solicitud repetida (ya existe una solicitud con el mismo tipo de solicitud y el mismo empleado en los dos últimos segundos)
        <EnumMember> PunchExist ' Ya existe un fichaje con la misma fecha y hora
        <EnumMember> StartShiftRequired ' Inicio de horario flotante requerido
        <EnumMember> PlannedCausesOverlapped 'Solicitudes de Horas de Ausencia solapadas
        <EnumMember> PlannedAbsencesOverlapped 'Solicitudes de Dias de ausencia solapadas
        <EnumMember> TaskRequiered ' Tarea requerida
        <EnumMember> NotEnoughConceptBalance ' No tiene saldo de vacaciones suficiente
        <EnumMember> CostCenterRequiered ' Centro de coste obligatorio
        <EnumMember> ForgottenCostCenterPunchError ' Error al validar el fichaje olvidado de centro de coste
        <EnumMember> PlannedHolidaysError ' Error al validar la solicitud de vacaciones por horas
        <EnumMember> PlannedHolidaysOverlapped 'Solicitudes de rpevision de vacaciones/permisos por horas solapadas
        <EnumMember> AnotherHolidayExistInDate
        <EnumMember> AnotherAbsenceExistInDate
        <EnumMember> InHolidayPlanification
        <EnumMember> VacationsOrPermissionsOverlapped
        <EnumMember> NeedConfirmation
        <EnumMember> RequestRuleError
        <EnumMember> PlannedOvertimesOverlapped
        <EnumMember> AnotherOvertimeExistInDate
        <EnumMember> PlannedOvertimesError
        <EnumMember> ExternalWorkWeekResumeError
        <EnumMember> IncorrectActualType
        <EnumMember> FreezeDateException
        <EnumMember> ExchangeEmployeeRequired
        <EnumMember> DateOutOfContract
        <EnumMember> SaveCalendarException
        <EnumMember> CalendarGenerateIndictments
        <EnumMember> CompensationRequired
        <EnumMember> EmployeeNotSuitableForShiftExchange
        <EnumMember> BlockedDayOnShiftExchange
        <EnumMember> RequestPendingOnShiftExchange
        <EnumMember> NoPermissionOnShiftExchange
        <EnumMember> OnAbsenceOnShiftExchange
        <EnumMember> OnHolidaysOnShiftExchange
        <EnumMember> ApplicantCantCoverEmployeeOnShiftExchange
        <EnumMember> EmployeeCantCoverApplicantOnShiftExchange
        <EnumMember> NoAssignmentOnShiftExchange
        <EnumMember> WrongAssignmentOnShiftExchange
        <EnumMember> IndictmentOnShiftExchange
        <EnumMember> NoPunchesForDailyRecord
        <EnumMember> DailyRecordPunchesError
        <EnumMember> XSSvalidationError
    End Enum

    Public Enum SecurityNodeResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> EmptyName
        <EnumMember> HasChildren
        <EnumMember> NotExistAdminGroup
        <EnumMember> NotExistAdminPassport
        <EnumMember> InvalidSecurityMode
        <EnumMember> InvalidProductiveCenter
        <EnumMember> UsedInBudget
        <EnumMember> MigrateErrorExceptionsOnSystem
    End Enum

    <DataContract>
    Public Enum GroupResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> GroupIsNotEmpty
        <EnumMember> GroupNotEmpty
        <EnumMember> IsFirstGroupCannotRemove
        <EnumMember> GroupUsedInActivity
        <EnumMember> GroupCannotBeNull
        <EnumMember> IndicatorCannotBeNull
        <EnumMember> NoAttRestrictionAllowed
        <EnumMember> InvalidProductiveCenter
        <EnumMember> UsedInBudget
        <EnumMember> CantDeleteGroupCommuniquee
        <EnumMember> CantDeleteGroupSurvey
        <EnumMember> ExportNameNotUnique
        <EnumMember> GroupNotExists
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ConceptResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> UsedByAccrualRules
        <EnumMember> UsedByEmployeeConceptAnnualLimits
        <EnumMember> UsedByReportsGroups
        <EnumMember> NameAlreadyExist
        <EnumMember> NameCannotBeNull
        <EnumMember> ShortNameAlreadyExist
        <EnumMember> InvalidDateInterval
        <EnumMember> TooManyTerminals
        <EnumMember> TooManyTerminals232
        <EnumMember> InvalidUsedField
        <EnumMember> CompositionFactorValueInvalid
        <EnumMember> CompositionFactorIDCauseNotAllowed
        <EnumMember> CompositionFactorUserFieldInvalidDataType
        <EnumMember> CompositionFactorUserFieldRequired
        <EnumMember> CompositionConditionIDCauseRequired
        <EnumMember> CompositionConditionIDCauseInvalid
        <EnumMember> CompositionConditionDirectValueInvalid
        <EnumMember> CompositionConditionUserFieldValueInvalidDataType
        <EnumMember> CompositionConditionUserFieldValueRequired
        <EnumMember> CompositionConditionIDCauseValueInvalid
        <EnumMember> CompositionConditionIDCausesMustBeDiferent
        <EnumMember> GroupNameCannotBeNull
        <EnumMember> GroupNameAlreadyExist
        <EnumMember> NotEmptyConcepts
        <EnumMember> AccrualWorkExists
        <EnumMember> UsedByInddicators
        <EnumMember> NumberOfConceptsExceeded
        <EnumMember> UsedByCausesonProgrammedHolidays
        <EnumMember> AutomaticAccrualCriteriaDataError
        <EnumMember> ExpiredHoursDataError
        <EnumMember> DefaultQueryMustBeAnnual
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum GroupFeatureResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> EmptyName
        <EnumMember> ManagerGroupNotDelete
        <EnumMember> SupervisorAssigned
        <EnumMember> ExistSameName
        <EnumMember> XSSvalidationError
        <EnumMember> EmptyExternalId
        <EnumMember> ExistSameExternalId
    End Enum

    <DataContract>
    Public Enum CalendarRowHourDataResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InValidData
    End Enum

    <DataContract>
    Public Enum CommuniqueResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidName
        <EnumMember> EmptyMessage
        <EnumMember> EmployeesRequired
        <EnumMember> UnexpectedCommuniqueStatusForEmployee
        <EnumMember> ErrorSettingEmployees
        <EnumMember> ErrorSettingGroups
        <EnumMember> ErrorSavingDocuments
        <EnumMember> ErrorLoadingCommunique
        <EnumMember> ErrorSettingAnswer
        <EnumMember> ErrorSettingReadMark
        <EnumMember> AnswerCannotBeChanged
        <EnumMember> SubjectRequired
        <EnumMember> CanNotDeleteReadCommuniques
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum EngineResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> SqlError
        <EnumMember> EmployeeRequired
        <EnumMember> ErrorGettingProgrammedCauses
        <EnumMember> ErrorGettingProgrammedHolidays
        <EnumMember> ErrorGettingProgrammedOvertimes
        <EnumMember> ErrorGettingIncidences
        <EnumMember> ErrorRemovingPreviousUnjustifiedCauses
        <EnumMember> ErrorRemovingUnchangedIncidences
        <EnumMember> ErrorApplyingAllShiftRules
        <EnumMember> ErrorApplyingProgrammedAbsences
        <EnumMember> ErrorApplyingProgrammedHolidays
        <EnumMember> CouldNotAnalyzeProgrammedAbsence
        <EnumMember> ErrorApplyingHolidaysRules
        <EnumMember> ErrorApplyingDefaultCauses
        <EnumMember> ErrorApplyingDefaultCenters
        <EnumMember> ErrorExecutingPendingSQL
        <EnumMember> ErrorPreprocessingDaysForAccrualsRules
        <EnumMember> ErrorLoadingEmployeeAccrualsRules
        <EnumMember> ErrorMarkingAccrualsRulesNextDates
        <EnumMember> ErrorApplyingDailyConceptsRules
        <EnumMember> ErrorDeletingEquivalences
        <EnumMember> ErrorApplyingEquivalences
        <EnumMember> ErrorInsertingEquivalences
        <EnumMember> ErrorUpdatingEquivalences
        <EnumMember> ErrorDoingAccruals
        <EnumMember> ErrorDeletingManualCauses
        <EnumMember> ErrorDoingAutomaticAccruals
        <EnumMember> ErrorDoingShiftDailyRules
        <EnumMember> ErrorDoingCausesLimits
        <EnumMember> ErrorDoReplaceCauseValue
        <EnumMember> ErrorDoingRules
        <EnumMember> ErrorDoingExpiredDateRules
        <EnumMember> ErrorCreatingOrUpdatingDailyCause
        <EnumMember> ErrorDoingSingleDayRules
        <EnumMember> ErrorDeletingDailyAccruals
        <EnumMember> ErrorInsertingStartupValue
        <EnumMember> ErrorDoingStartupValues
        <EnumMember> ErrorSettingNotifications1001
        <EnumMember> ErrorSettingNotifications1002
        <EnumMember> ErrorSettingNotifications65
        <EnumMember> ErrorInsertingEmployeeMaximumLimits
        <EnumMember> ErrorApplyingBreakRules
        <EnumMember> ErrorApplyingGratificationRule
        <EnumMember> ErrorUpdatingStatus
        <EnumMember> ErrorSavingData
        <EnumMember> ErrorGettingCausesSum
        <EnumMember> AccrualsCacheModified
        <EnumMember> ErrorApplyPunchedCauses
        <EnumMember> AutomaticApprovalDailyRecord
        <EnumMember> ErrorApplyingLatamOverTime
        <EnumMember> ErrorSetPeriodAnnualWork
        <EnumMember> ErrorUpdatingPeriodAnnualWork


    End Enum

    <DataContract>
    Public Enum ToDoListResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidName
        <EnumMember> ErrorCreatingOrUpdatingToDoList
        <EnumMember> ErrorCreatingOrUpdatingToDoTask
        <EnumMember> ErrorDeletingToDoTasks
        <EnumMember> ErrorDeletingToDoList
        <EnumMember> TargetListAlreadyHasTasks
        <EnumMember> ErrorInitializingTasks
        <EnumMember> ErrorCloningToDoList
        <EnumMember> ErrorCloningToDoListTasks
        <EnumMember> SourceListNotFound
        <EnumMember> TargetListNotFound
        <EnumMember> ErrorRecoveringToDoTask
        <EnumMember> ErrorRecoveringToDoListTasks
        <EnumMember> ErrorRecoveringToDoList
        <EnumMember> ErrorRecoveringAllToDoLists
        <EnumMember> ErrorRecoveringOnBoarding
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum SurveyResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidName
        <EnumMember> ErrorCreatingOrUpdatingSurvey
        <EnumMember> ErrorSettingSurveyEmployees
        <EnumMember> ErrorSettingSurveyGroups
        <EnumMember> ErrorDeletingSurvey
        <EnumMember> ErrorCloningSurvey
        <EnumMember> SourceListNotFound
        <EnumMember> TargetListNotFound
        <EnumMember> ErrorRecoveringSurvey
        <EnumMember> ErrorRecoveringAllSurveys
        <EnumMember> ErrorSavingSurveyResponse
        <EnumMember> ErrorRecoveringAllSurveyResponses
        <EnumMember> ErrorRecoveringAllSurveyTemplates
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ChannelResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ErrorCreatingOrUpdatingChannel
        <EnumMember> ErrorSettingChannelEmployees
        <EnumMember> ErrorSettingChannelGroups
        <EnumMember> ErrorSettingChannelSupervisors
        <EnumMember> ErrorDeletingChannel
        <EnumMember> ErrorRecoveringChannel
        <EnumMember> ErrorRecoveringAllChannels
        <EnumMember> ErrorRecoveringNewMessagesInChannel
        <EnumMember> ErrorRecoveringOpenConversationsInChannel
        <EnumMember> NoPermission
        <EnumMember> XSSvalidationError
    End Enum

    <DataContract>
    Public Enum ConversationResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> InvalidName
        <EnumMember> ErrorCreatingConversation
        <EnumMember> ErrorConversationAlreadyExists
        <EnumMember> ErrorRecoveringConversation
        <EnumMember> ErrorRecoveringAllConversations
        <EnumMember> NoPermission
        <EnumMember> ErrorNewConversationShouldHaveFirstMessage
        <EnumMember> ErrorRecoveringNewMessagesInConversation
        <EnumMember> ErrorUpdatingConversationState
        <EnumMember> MessageRequiredToCreateConversation
        <EnumMember> InitialMessageCanNotBeEmpty
        <EnumMember> ConversationShouldBeCreatedByAnEmployee
        <EnumMember> ConversationShouldBelongToAChannel
        <EnumMember> ChannelShouldBePublishedToCreateConversations
        <EnumMember> ComplainantsShouldProvidePassword
        <EnumMember> ComplaintsShouldBeAnonymous
    End Enum

    <DataContract>
    Public Enum MessageResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> ErrorCreatingMessage
        <EnumMember> ErrorRecoveringMessage
        <EnumMember> ErrorRecoveringAllMessages
        <EnumMember> ErrorMessageAlreadyExists
        <EnumMember> NoPermission
        <EnumMember> ConversationClosedOrDismissed
        <EnumMember> MessageCannotBeRegisteredOnLogBook
    End Enum

    <DataContract>
    Public Enum LogBookResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> ErrorRecoveringMessage
        <EnumMember> ErrorRecoveringLogBook
        <EnumMember> ErrorSavingLogBook
        <EnumMember> NoPermission
    End Enum

    <DataContract>
    Public Enum DailyRecordResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> ErrorLoadingDailyRecord
        <EnumMember> ErrorLoadingDailyRecordCalendar
        <EnumMember> ErrorSavingDailyRecordRequest
        <EnumMember> ErrorSavingDailyRecordPunch
        <EnumMember> FutureDailyRecordNotAllowed
        <EnumMember> ErrorDeletingIncompletePunchNotification
        <EnumMember> ErrorReorderingDayPunches
        <EnumMember> ErrorAprovingDailyRecordMadeBySupervisor
        <EnumMember> InFrozenPeriod
        <EnumMember> DailyRecordAlreadyExistsOnDate
        <EnumMember> DailyRecordDoesNotExists
        <EnumMember> UnconsistentDailyRecordPunches
        <EnumMember> DailyRecordMustContainPunches
        <EnumMember> DailyRecordOddNumberOfPunches
        <EnumMember> DailyRecordPunchesOverlaped
        <EnumMember> DailyRecordHasRepeatedPunches
    End Enum

    Public Enum BotResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> NoPermission
        <EnumMember> ErrorRecoveringBot
        <EnumMember> ErrorRecoveringAllBots
        <EnumMember> ErrorCreatingOrUpdatingBot
        <EnumMember> ErrorDeletingBot
        <EnumMember> NameAlreadyExists
        <EnumMember> ErrorExecutingBot
        <EnumMember> BotRuleNameAlreadyExists
        <EnumMember> BotRuleSameTypeAlreadyExist
        <EnumMember> XSSvalidationError
    End Enum

    Public Enum BotRuleResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> NoPermission
        <EnumMember> ErrorRecoveringBotRule
        <EnumMember> NameAlreadyExists
        <EnumMember> ErrorCreatingOrUpdatingBotRule
        <EnumMember> ErrorExecutingBotRule
        <EnumMember> SameTypeAlreadyExist
        <EnumMember> XSSvalidationError
    End Enum

    Public Enum CollectiveResult
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> NoPermission
        <EnumMember> ErrorRecoveringCollective
        <EnumMember> NameAlreadyExists
        <EnumMember> ErrorCreatingOrUpdatingCollective
        <EnumMember> XSSvalidationError
        <EnumMember> ErrorDeletingCollective
        <EnumMember> ErrorRecoveringAllCollectives
        <EnumMember> ErrorRecoveringEmployeeUserfields
        <EnumMember> ErorrGeneratingHavingClause
        <EnumMember> ErrorFilterExpressionContainsInvalidFields
        <EnumMember> CollectiveNameCanNotBeEmpty
        <EnumMember> CollectiveFilterCanNotBeEmpty
        <EnumMember> ErrorRecoveringCollectiveEmployees
        <EnumMember> ErrorRecoveringCollectiveDefinition
        <EnumMember> BeginDefinitionInFreezeDate
        <EnumMember> CollectiveUsedInProcess
        <EnumMember> NonElegibleUserfieldUsed
        <EnumMember> CollectiveDefinitionOverlapped
        <EnumMember> ErrorValidatingCollectiveDefinition
    End Enum

    Public Enum RulesResult
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> NoPermission
    End Enum
End Namespace