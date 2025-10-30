Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum CalendarView
        <EnumMember()> Review
        <EnumMember()> Planification
    End Enum

    Public Enum CalendarDetailLevel
        <EnumMember()> Daily
        <EnumMember()> Detail_15
        <EnumMember()> Detail_30
        <EnumMember()> Detail_60
    End Enum

    <DataContract()>
    Public Enum ShiftTypeEnum
        <EnumMember()> Normal
        <EnumMember()> NormalFloating
        <EnumMember()> Holiday_Working
        <EnumMember()> Holiday_NoWorking
    End Enum

    <DataContract()>
    Public Enum EmployeeStatusOnDayEnum
        <EnumMember()> Ok
        <EnumMember()> NoContract
        <EnumMember()> InOtherDepartment
    End Enum

    <DataContract()>
    Public Enum DailyHourTypeEnum
        <EnumMember()> Untyped
        <EnumMember()> Mandatory
        <EnumMember()> Flexible
        <EnumMember()> Complementary
    End Enum

    <DataContract()>
    Public Enum CalendarStatusEnum
        <EnumMember()> OK
        <EnumMember()> KO
        <EnumMember()> WARNING
    End Enum

    <DataContract()>
    Public Enum CalendarCoverageDayStatus
        <EnumMember()> OK
        <EnumMember()> KO
        <EnumMember()> OVERLOAD
        <EnumMember()> WITHOUTCOVERAGE
    End Enum

    <DataContract()>
    Public Enum CalendarErrorResultDayEnum
        <EnumMember()> NoContract
        <EnumMember()> FreezingDate
        <EnumMember()> PermissionDenied
        <EnumMember()> ShiftWithoutPermission
        <EnumMember()> ShiftNotExist
        <EnumMember()> InvalidStartFloating
        <EnumMember()> InvalidShiftBase
        <EnumMember()> InvalidAreWorkingDay
        <EnumMember()> InvalidComplementaryData
        <EnumMember()> InvalidFloatingData
        <EnumMember()> EmployeeDoesNotExist
        <EnumMember()> DateOutOfScope
        <EnumMember()> GenericInvalidData
        <EnumMember()> UnknownEmployee
        <EnumMember()> UnknownError
        <EnumMember()> GenericError
        <EnumMember()> InvalidShiftDefinition
        <EnumMember()> InvalidAssignmentData
        <EnumMember()> ExistProgrammedHoliday
    End Enum

    <DataContract()>
    Public Enum RemarkCompare
        <EnumMember()> Equal
        <EnumMember()> Minor
        <EnumMember()> MinorEqual
        <EnumMember()> Major
        <EnumMember()> MajorEqual
        <EnumMember()> Distinct
    End Enum

    <DataContract()>
    Public Enum ScheduleRuleType
        <EnumMember()> OneShiftOneDay
        <EnumMember()> RestBetweenShifts
        <EnumMember()> MinMaxFreeLabourDaysInPeriod
        <EnumMember()> MinMaxShiftsInPeriod
        <EnumMember()> MinWeekendsInPeriod
        <EnumMember()> MinMaxExpectedHours
        <EnumMember()> TwoShiftSequence
        <EnumMember()> MinMax2ShiftSequenceOnEmployee
        <EnumMember()> MaxHolidays
        <EnumMember()> WorkOnFestive
        <EnumMember()> WorkOnWeekend
        <EnumMember()> MaxNotScheduled
        <EnumMember()> MinMaxDaysSequence
        <EnumMember()> MinMaxExpectedHoursInPeriod
        <EnumMember()> MinMaxShiftsSequence
        <EnumMember()> Custom
    End Enum

    <DataContract()>
    Public Enum ScheduleRuleScope
        <EnumMember()> Day
        <EnumMember()> Week
        <EnumMember()> Month
        <EnumMember()> Year
        <EnumMember()> Selection
        <EnumMember()> Always
    End Enum

    <DataContract()>
    Public Enum ScheduleRuleDayType
        <EnumMember()> NotLaborable
        <EnumMember()> Laborable
    End Enum

    <DataContract()>
    Public Enum ScheduleRuleBaseType
        <EnumMember()> System
        <EnumMember()> User
        <EnumMember()> Custom
    End Enum

    <DataContract()>
    Public Enum CalendarPunchTypeEnum
        <EnumMember()> _NOTDEFINDED = 0
        <EnumMember()> _IN = 1
        <EnumMember()> _OUT = 2
    End Enum

    <DataContract()>
    Public Enum TelecommutingTypeEnum
        <EnumMember()> _AtOffice = 0
        <EnumMember()> _AtHome = 1
    End Enum

    <DataContract()>
    Public Enum TelecommutingPeriodType
        <EnumMember()> _Week = 0
        <EnumMember()> _Month = 1
        <EnumMember()> _Trimester = 2
    End Enum

    <DataContract()>
    Public Enum TelecommutingMaxType
        <EnumMember()> _Days = 0
        <EnumMember()> _Percentage = 1
    End Enum

    <DataContract()>
    Public Enum TelecommutingCheckChangeResult
        <EnumMember()> _Direct = 0
        <EnumMember()> _Request = 1
        <EnumMember()> _NoSchedule = 2
        <EnumMember()> _NoAgreement = 3
        <EnumMember()> _ErrorChecking = 4
    End Enum

    <DataContract()>
    Public Class roScheduleRulesTypesResponse

        Public Sub New()
            Rules = {}
            oState = New roWsState()
        End Sub

        <DataMember()>
        Public Property Rules As Integer()
        <DataMember()>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roScheduleRulesResponse

        Public Sub New()
            Rules = {}
            oState = New roWsState()
        End Sub

        <DataMember()>
        Public Property Rules As roScheduleRule()
        <DataMember()>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roScheduleRulesStdResponse

        Public Sub New()
            Result = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property oState As roWsState

        <DataMember>
        Public Property Result As Boolean

    End Class

    <DataContract(),
    KnownType(GetType(roScheduleRule_OneShiftOneDay)),
    KnownType(GetType(roScheduleRule_RestBetweenShifts)),
    KnownType(GetType(roScheduleRule_MinMaxFreeLabourDaysInPeriod)),
    KnownType(GetType(roScheduleRule_MinMaxShiftsInPeriod)),
    KnownType(GetType(roScheduleRule_MinWeekendsInPeriod)),
    KnownType(GetType(roScheduleRule_MinMaxExpectedHours)),
    KnownType(GetType(roScheduleRule_2ShiftSequence)),
    KnownType(GetType(roScheduleRule_MinMax2ShiftSequenceOnEmployee)),
    KnownType(GetType(roScheduleRule_MaxHolidays)),
    KnownType(GetType(roScheduleRule_WorkOnFestive)),
    KnownType(GetType(roScheduleRule_WorkOnWeekend)),
    KnownType(GetType(roScheduleRule_MaxNotScheduled)),
    KnownType(GetType(roScheduleRule_MinMaxDaysSequence)),
    KnownType(GetType(roScheduleRule_MinMaxExpectedHoursInPeriod)),
    KnownType(GetType(roScheduleRule_MinMaxShiftsSequence)),
    KnownType(GetType(roScheduleRule_Custom))>
    <Serializable>
    Public Class roScheduleRule
        <DataMember()>
        Public Property Id As Integer
        <DataMember()>
        Public Property IDRule As ScheduleRuleType
        <DataMember()>
        Public Property RuleName As String
        <DataMember()>
        Public Property RuleDescription As String
        <DataMember()>
        Public Property IdLabAgree As Integer
        <DataMember()>
        Public Property IdContract As String
        <DataMember()>
        Public Property Enabled As Boolean
        <DataMember()>
        Public Property Weight As Integer
        <DataMember()>
        Public Property RuleType As ScheduleRuleBaseType
        <DataMember()>
        Public Property Scope As ScheduleRuleScope
        <DataMember()>
        Public Property ScopePeriods As Integer
        <DataMember()>
        Public Property DayDepth As Integer
        <DataMember()>
        Public Property BeginPeriod As Date
        <DataMember()>
        Public Property EndPeriod As Date

        Public Sub New()
            Me.ScopePeriods = 1
            Me.BeginPeriod = DateSerial(1970, 1, 1)
            Me.EndPeriod = DateSerial(1970, 1, 1)
        End Sub

    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_OneShiftOneDay
        Inherits roScheduleRule
        <DataMember()>
        Public Property Type As ShiftSequenceType
        <DataMember()>
        Public Property ReferenceDate As Date
        <DataMember()>
        Public Property CurrentDayShifts As Integer()
        <DataMember()>
        Public Property PreviousDayShifts As Integer()
        <DataMember()>
        Public Property TypeNextDays As ShiftSequenceType
        <DataMember()>
        Public Property NextDayShifts As Integer()
        <DataMember()>
        Public Property RestHours As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_RestBetweenShifts
        Inherits roScheduleRule
        <DataMember()>
        Public Property RestHours As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxFreeLabourDaysInPeriod
        Inherits roScheduleRule
        <DataMember()>
        Public Property MinimumRestDays As Integer
        <DataMember()>
        Public Property MaximumRestDays As Integer
        <DataMember()>
        Public Property MinimumLabourDays As Integer
        <DataMember()>
        Public Property MaximumLabourDays As Integer
        <DataMember()>
        Public Property DaysType As DaysType

        Public Sub New()
            DaysType = DaysType.Free
            MinimumLabourDays = -1
            MaximumLabourDays = -1
            MinimumRestDays = 0
            MaximumRestDays = -1
        End Sub

    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxShiftsInPeriod
        Inherits roScheduleRule
        <DataMember()>
        Public Property CurrentDayShifts As Integer()
        <DataMember()>
        Public Property Maximum As Integer
        <DataMember()>
        Public Property Minimum As Integer
        <DataMember()>
        Public Property LogicOr As Boolean
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinWeekendsInPeriod
        Inherits roScheduleRule

        <DataMember()>
        Public Property Minimum As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxExpectedHours
        Inherits roScheduleRule
        <DataMember()>
        Public Property MinimumWorkingHours As Integer
        <DataMember()>
        Public Property MinimumEmployeeField As String
        <DataMember()>
        Public Property MaximumWorkingHours As Integer
        <DataMember()>
        Public Property MaximumEmployeeField As String
        <DataMember()>
        Public Property MaximumWorkingHoursFork As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_2ShiftSequence
        Inherits roScheduleRule
        <DataMember()>
        Public Property Type As ShiftSequenceType
        <DataMember()>
        Public Property CurrentDayShifts As Integer()
        <DataMember()>
        Public Property PreviousDayShifts As Integer()
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMax2ShiftSequenceOnEmployee
        Inherits roScheduleRule
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MaxHolidays
        Inherits roScheduleRule

        <DataMember()>
        Public Property MaximumHoliDays As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_WorkOnFestive
        Inherits roScheduleRule
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_WorkOnWeekend
        Inherits roScheduleRule

        <DataMember()>
        Public Property LabourDaysIndex As String
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MaxNotScheduled
        Inherits roScheduleRule

        <DataMember()>
        Public Property MaximumNotScheduledDays As Integer
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxDaysSequence
        Inherits roScheduleRule
        <DataMember()>
        Public Property MinimumDays As Integer
        <DataMember()>
        Public Property MaximumDays As Integer
        <DataMember()>
        Public Property DaysType As DaysType

        Public Sub New()
            MinimumDays = -1
            MaximumDays = 0
            DaysType = DaysType.Labour
        End Sub

    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxShiftsSequence
        Inherits roScheduleRule
        <DataMember()>
        Public Property Minimum As Integer
        <DataMember()>
        Public Property MinimumEmployeeField As String
        <DataMember()>
        Public Property Maximum As Integer
        <DataMember()>
        Public Property MaximumEmployeeField As String
        <DataMember()>
        Public Property Shifts As String()

        Public Sub New()
            Minimum = -1
            Maximum = 0
        End Sub

    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_Custom
        Inherits roScheduleRule
    End Class

    <DataContract()>
    <Serializable>
    Public Class roScheduleRule_MinMaxExpectedHoursInPeriod
        Inherits roScheduleRule
        <DataMember()>
        Public Property MaximumWorkingHours As Integer
        <DataMember()>
        Public Property MaximumEmployeeField As String
        <DataMember()>
        Public Property MinimumWorkingHours As Integer
        <DataMember()>
        Public Property MinimumEmployeeField As String
    End Class

    <DataContract()>
    Public Enum ShiftSequenceType
        <EnumMember()> Unwanted 'Cuando se planifica el último horario de la secuencia, los días anteriores NO pueden planificarse con los horarios de la secuencia
        <EnumMember()> Wanted 'Cuando se planifica el último horario de la secuencia, los días anteriores deben necesariamente planificarse con los horarios de la secuencia
    End Enum

    <DataContract()>
    Public Enum DaysType
        <EnumMember()> Free
        <EnumMember()> Labour
    End Enum

    <DataContract()>
    <Serializable>
    Public Class roCalendarRemark
        Protected _idCause As Integer
        Protected _oCompare As RemarkCompare
        Protected _xValue As DateTime
        Protected _intColor As Integer
        Protected _strCalendarNotJustified As String

        <DataMember()>
        Public Property IdCause As Integer
            Get
                Return _idCause
            End Get
            Set(value As Integer)
                _idCause = value
            End Set
        End Property

        <DataMember()>
        Public Property Comparison As RemarkCompare
            Get
                Return _oCompare
            End Get
            Set(value As RemarkCompare)
                _oCompare = value
            End Set
        End Property

        <DataMember()>
        Public Property Value As DateTime
            Get
                Return _xValue
            End Get
            Set(value As DateTime)
                _xValue = value
            End Set
        End Property

        <DataMember()>
        Public Property Color As Integer
            Get
                Return _intColor
            End Get
            Set(value As Integer)
                _intColor = value
            End Set
        End Property

        Public Sub New()
            _oCompare = RemarkCompare.Equal
            _idCause = -1
            _xValue = New DateTime(1900, 1, 1, 0, 0, 0)
            _intColor = 0
        End Sub

    End Class

    <DataContract()>
    <Serializable>
    Public Class roCalendarPassportConfig
        Protected _lstCalendarRemarks As roCalendarRemark()
        Protected _strCalendarAccrual As String
        Protected _strCalendarHolidays As String
        Protected _strCalendarWorking As String
        Protected _strCalendarOvertime As String
        Protected _strCalendarNotJustified As String

        <DataMember()>
        Public Property CalendarRemarks As roCalendarRemark()
            Get
                Return _lstCalendarRemarks
            End Get
            Set(value As roCalendarRemark())
                _lstCalendarRemarks = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarAccrual As String
            Get
                Return _strCalendarAccrual
            End Get
            Set(value As String)
                _strCalendarAccrual = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarHolidays As String
            Get
                Return _strCalendarHolidays
            End Get
            Set(value As String)
                _strCalendarHolidays = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarWorking As String
            Get
                Return _strCalendarWorking
            End Get
            Set(value As String)
                _strCalendarWorking = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarOvertime As String
            Get
                Return _strCalendarOvertime
            End Get
            Set(value As String)
                _strCalendarOvertime = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarNotJustified As String
            Get
                Return _strCalendarNotJustified
            End Get
            Set(value As String)
                _strCalendarNotJustified = value
            End Set
        End Property

        Public Sub New()
            _lstCalendarRemarks = {}
            _strCalendarAccrual = String.Empty
            _strCalendarHolidays = String.Empty
            _strCalendarNotJustified = String.Empty
            _strCalendarOvertime = String.Empty
            _strCalendarWorking = String.Empty
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendar
        Protected _lstCalendarHeader As roCalendarHeader
        Protected _lstCalendarData As roCalendarRow()
        Protected _dateFirstDay As Date
        Protected _dateLastDay As Date
        Protected _dateFreezingDate As Date
        Protected _lstCalendarShift As List(Of roCalendarShift)

        <DataMember()>
        Public Property CalendarHeader As roCalendarHeader
            Get
                Return _lstCalendarHeader
            End Get
            Set(value As roCalendarHeader)
                _lstCalendarHeader = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarData As roCalendarRow()
            Get
                Return _lstCalendarData
            End Get
            Set(value As roCalendarRow())
                _lstCalendarData = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarShift As Generic.List(Of roCalendarShift)
            Get
                Return _lstCalendarShift
            End Get
            Set(value As Generic.List(Of roCalendarShift))
                _lstCalendarShift = value
            End Set
        End Property

        <DataMember()>
        Public Property FirstDay As Date
            Get
                Return _dateFirstDay
            End Get
            Set(value As Date)
                _dateFirstDay = value
            End Set
        End Property

        <DataMember()>
        Public Property LastDay As Date
            Get
                Return _dateLastDay
            End Get
            Set(value As Date)
                _dateLastDay = value
            End Set
        End Property

        <DataMember()>
        Public Property FreezingDate As Date
            Get
                Return _dateFreezingDate
            End Get
            Set(value As Date)
                _dateFreezingDate = value
            End Set
        End Property

        Public Sub New()
            _lstCalendarHeader = Nothing
            _lstCalendarData = Nothing
            _dateFirstDay = Nothing
            _dateFirstDay = Nothing
            _dateFreezingDate = Nothing
            _lstCalendarShift = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarHeader

        Protected _EmployeeHeaderData As roCalendarHeaderCell
        Protected _SummaryHeaderData As roCalendarHeaderCell
        Protected _lstPeriodHeaderData As roCalendarHeaderCell()
        Protected _lstPeriodCoverageData As roCalendarCoverageDay()
        Protected _lstPeriodSeatingCapacityData As roCalendarSeatingCapacityDay()

        <DataMember()>
        Public Property EmployeeHeaderData As roCalendarHeaderCell
            Get
                Return _EmployeeHeaderData
            End Get
            Set(value As roCalendarHeaderCell)
                _EmployeeHeaderData = value
            End Set
        End Property

        <DataMember()>
        Public Property SummaryHeaderData As roCalendarHeaderCell
            Get
                Return _SummaryHeaderData
            End Get
            Set(value As roCalendarHeaderCell)
                _SummaryHeaderData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodHeaderData As roCalendarHeaderCell()
            Get
                Return _lstPeriodHeaderData
            End Get
            Set(value As roCalendarHeaderCell())
                _lstPeriodHeaderData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodCoverageData As roCalendarCoverageDay()
            Get
                Return _lstPeriodCoverageData
            End Get
            Set(value As roCalendarCoverageDay())
                _lstPeriodCoverageData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodSeatingCapacityData As roCalendarSeatingCapacityDay()
            Get
                Return _lstPeriodSeatingCapacityData
            End Get
            Set(value As roCalendarSeatingCapacityDay())
                _lstPeriodSeatingCapacityData = value
            End Set
        End Property

        Public Sub New()
            _EmployeeHeaderData = Nothing
            _SummaryHeaderData = Nothing
            _lstPeriodHeaderData = Nothing
            _lstPeriodCoverageData = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarCoverageDay
        Protected _IDGroup As Double
        Protected _Date As Date
        Protected _lstAssignmentData As roCalendarAssignmentCoverageDay()
        Protected _PlannedStatus As CalendarCoverageDayStatus
        Protected _ActualStatus As CalendarCoverageDayStatus
        Protected _ActualProcessed As Boolean
        Protected _PlannedProcessed As Boolean

        <DataMember()>
        Public Property AssignmentData As roCalendarAssignmentCoverageDay()
            Get
                Return _lstAssignmentData
            End Get
            Set(value As roCalendarAssignmentCoverageDay())
                _lstAssignmentData = value
            End Set
        End Property

        <DataMember()>
        Public Property PlannedStatus As CalendarCoverageDayStatus
            Get
                Return _PlannedStatus
            End Get
            Set(value As CalendarCoverageDayStatus)
                _PlannedStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property ActualStatus As CalendarCoverageDayStatus
            Get
                Return _ActualStatus
            End Get
            Set(value As CalendarCoverageDayStatus)
                _ActualStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property ActualProcessed As Boolean
            Get
                Return _ActualProcessed
            End Get
            Set(value As Boolean)
                _ActualProcessed = value
            End Set
        End Property

        <DataMember()>
        Public Property PlannedProcessed As Boolean
            Get
                Return _PlannedProcessed
            End Get
            Set(value As Boolean)
                _PlannedProcessed = value
            End Set
        End Property

        <DataMember()>
        Public Property IDGroup As Double
            Get
                Return _IDGroup
            End Get
            Set(value As Double)
                _IDGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property CoverageDate As Date
            Get
                Return _Date
            End Get
            Set(value As Date)
                _Date = value
            End Set
        End Property

        Public Sub New()
            _Date = New Date(1970, 1, 1)
            _IDGroup = 0
            _ActualStatus = CalendarCoverageDayStatus.OK
            _PlannedStatus = CalendarCoverageDayStatus.OK
            _lstAssignmentData = {}
            _ActualProcessed = False
            _PlannedProcessed = False

        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarAssignmentCoverageDay
        Protected _ID As Double
        Protected _ExpectedCoverage As Double
        Protected _PlannedCoverage As Double
        Protected _ActualCoverage As Double
        Protected _Name As String
        Protected _ShortName As String

        <DataMember()>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        <DataMember()>
        Public Property ShortName As String
            Get
                Return _ShortName
            End Get
            Set(value As String)
                _ShortName = value
            End Set
        End Property

        <DataMember()>
        Public Property ID As Double
            Get
                Return _ID
            End Get
            Set(value As Double)
                _ID = value
            End Set
        End Property

        <DataMember()>
        Public Property Expected As Double
            Get
                Return _ExpectedCoverage
            End Get
            Set(value As Double)
                _ExpectedCoverage = value
            End Set
        End Property

        <DataMember()>
        Public Property Planned As Double
            Get
                Return _PlannedCoverage
            End Get
            Set(value As Double)
                _PlannedCoverage = value
            End Set
        End Property

        <DataMember()>
        Public Property Actual As Double
            Get
                Return _ActualCoverage
            End Get
            Set(value As Double)
                _ActualCoverage = value
            End Set
        End Property

        Public Sub New()
            _ID = 0
            _ActualCoverage = 0
            _PlannedCoverage = 0
            _ExpectedCoverage = 0
            _Name = ""
            _ShortName = ""

        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarSeatingCapacityDay
        Protected _lstCapacities As roCalendarCapacities()

        <DataMember()>
        Public Property Capacities As roCalendarCapacities()
            Get
                Return _lstCapacities
            End Get
            Set(value As roCalendarCapacities())
                _lstCapacities = value
            End Set
        End Property

        Public Sub New()
            _lstCapacities = {}
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarCapacities
        Protected _sZoneName As String
        Protected _iCurrentSeating As Integer
        Protected _iMaxSeatingCapacity As Integer
        Protected _bZoneCapacityVisible As Boolean

        <DataMember()>
        Public Property ZoneName As String
            Get
                Return _sZoneName
            End Get
            Set(value As String)
                _sZoneName = value
            End Set
        End Property

        <DataMember()>
        Public Property CurrentSeating As Integer
            Get
                Return _iCurrentSeating
            End Get
            Set(value As Integer)
                _iCurrentSeating = value
            End Set
        End Property

        <DataMember()>
        Public Property MaxSeatingCapacity As Integer
            Get
                Return _iMaxSeatingCapacity
            End Get
            Set(value As Integer)
                _iMaxSeatingCapacity = value
            End Set
        End Property

        <DataMember()>
        Public Property ZoneCapacityVisible As Boolean
            Get
                Return _bZoneCapacityVisible
            End Get
            Set(value As Boolean)
                _bZoneCapacityVisible = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roCalendarHeaderCell
        Protected _strRow1 As String
        Protected _strRow2 As String
        Protected _strBackColor As String

        <DataMember()>
        Public Property Row1Text As String
            Get
                Return _strRow1
            End Get
            Set(value As String)
                _strRow1 = value
            End Set
        End Property

        <DataMember()>
        Public Property Row2Text As String
            Get
                Return _strRow2
            End Get
            Set(value As String)
                _strRow2 = value
            End Set
        End Property

        <DataMember()>
        Public Property BackColor As String
            Get
                Return _strBackColor
            End Get
            Set(value As String)
                _strBackColor = value
            End Set
        End Property

        Public Sub New()
            _strRow1 = String.Empty
            _strRow2 = String.Empty
            _strBackColor = String.Empty
        End Sub

    End Class

    ''' <summary>
    ''' Información de la franja de un horario
    ''' </summary>
    <DataContract()>
    Public Class roCalendarShiftLayersDefinition
        Protected _dLayerStartTime As DateTime
        Protected _dLayerOrdinaryHours As Double
        Protected _dLayerComplementaryHours As Double
        Protected _dLayerDuration As Double
        Protected _dLayerID As Integer
        Protected _ExistLayerStartTime As Boolean
        Protected _ExistLayerDuration As Boolean

        <DataMember()>
        Public Property LayerStartTime As DateTime
            Get
                Return _dLayerStartTime
            End Get
            Set(value As DateTime)
                _dLayerStartTime = value
                _ExistLayerStartTime = True
            End Set
        End Property

        <DataMember()>
        Public Property ExistLayerStartTime As Boolean
            Get
                Return _ExistLayerStartTime
            End Get
            Set(value As Boolean)

            End Set
        End Property

        <DataMember()>
        Public Property LayerOrdinaryHours As Double
            Get
                Return _dLayerOrdinaryHours
            End Get
            Set(value As Double)
                _dLayerOrdinaryHours = value
            End Set
        End Property

        <DataMember()>
        Public Property LayerDuration As Double
            Get
                Return _dLayerDuration
            End Get
            Set(value As Double)
                _dLayerDuration = value
                _ExistLayerDuration = True
            End Set
        End Property

        <DataMember()>
        Public Property ExistLayerDuration As Boolean
            Get
                Return _ExistLayerDuration
            End Get
            Set(value As Boolean)

            End Set
        End Property

        <DataMember()>
        Public Property LayerID As Integer
            Get
                Return _dLayerID
            End Get
            Set(value As Integer)
                _dLayerID = value
            End Set
        End Property

        <DataMember()>
        Public Property LayerComplementaryHours As Double
            Get
                Return _dLayerComplementaryHours
            End Get
            Set(value As Double)
                _dLayerComplementaryHours = value
            End Set
        End Property

        Public Sub New()
            _dLayerComplementaryHours = -1
            _dLayerOrdinaryHours = -1
            _dLayerID = 0
            _dLayerDuration = -1
            _dLayerStartTime = New Date(1900, 1, 1)
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarRow

        Protected _EmployeeData As roCalendarRowEmployeeData
        Protected _SummaryData As roCalendarRowSummaryData
        Protected _PeriodData As roCalendarRowPeriodData
        Protected _Pos As Integer

        Public Sub New()
            _EmployeeData = Nothing
            _SummaryData = Nothing
            _PeriodData = Nothing
            _Pos = 0
        End Sub

        <DataMember()>
        Public Property EmployeeData As roCalendarRowEmployeeData
            Get
                Return _EmployeeData
            End Get
            Set(value As roCalendarRowEmployeeData)
                _EmployeeData = value
            End Set
        End Property

        <DataMember()>
        Public Property SummaryData As roCalendarRowSummaryData
            Get
                Return _SummaryData
            End Get
            Set(value As roCalendarRowSummaryData)
                _SummaryData = value
            End Set
        End Property

        <DataMember()>
        Public Property PeriodData As roCalendarRowPeriodData
            Get
                Return _PeriodData
            End Get
            Set(value As roCalendarRowPeriodData)
                _PeriodData = value
            End Set
        End Property

        <DataMember()>
        Public Property Pos As Integer
            Get
                Return _Pos
            End Get
            Set(value As Integer)
                _Pos = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roCalendarRowEmployeeData
        Protected _intIDEmployee As Integer
        Protected _intIDGroup As Integer
        Protected _strEmployeeName As String
        Protected _strGroupName As String
        Protected _strBackgroundColor As String
        Protected _intPermission As Integer
        Protected _lAssig As roCalendarAssignmentData()
        Protected _dateFreezingDate As Date
        Protected _CanTelecommute As Boolean
        Protected _TeleCommuteDays As String

        'TODO: Protected _lstSchedulingPositions() 'NombreCorto / Color

        Public Sub New()
            _strEmployeeName = String.Empty
            _strGroupName = String.Empty
            _strBackgroundColor = String.Empty
            _intIDEmployee = -1
            _intIDGroup = -1
            _intPermission = 0
            _lAssig = Nothing
            _dateFreezingDate = Nothing
            _CanTelecommute = False
            _TeleCommuteDays = String.Empty
        End Sub

        <DataMember()>
        Public Property FreezingDate As Date
            Get
                Return _dateFreezingDate
            End Get
            Set(value As Date)
                _dateFreezingDate = value
            End Set
        End Property

        <DataMember()>
        Public Property Assignments As roCalendarAssignmentData()
            Get
                Return _lAssig
            End Get
            Set(value As roCalendarAssignmentData())
                _lAssig = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return _intIDEmployee
            End Get
            Set(value As Integer)
                _intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property IDGroup As Integer
            Get
                Return _intIDGroup
            End Get
            Set(value As Integer)
                _intIDGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property Permission As Integer
            Get
                Return _intPermission
            End Get
            Set(value As Integer)
                _intPermission = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeeName As String
            Get
                Return _strEmployeeName
            End Get
            Set(value As String)
                _strEmployeeName = value
            End Set
        End Property

        <DataMember()>
        Public Property GroupName As String
            Get
                Return _strGroupName
            End Get
            Set(value As String)
                _strGroupName = value
            End Set
        End Property

        <DataMember()>
        Public Property BackgroundColor As String
            Get
                Return _strBackgroundColor
            End Get
            Set(value As String)
                _strBackgroundColor = value
            End Set
        End Property

        <DataMember()>
        Public Property CanTelecommute As Boolean
            Get
                Return _CanTelecommute
            End Get
            Set(value As Boolean)
                _CanTelecommute = value
            End Set
        End Property

        <DataMember()>
        Public Property TeleCommuteDays As String
            Get
                Return _TeleCommuteDays
            End Get
            Set(value As String)
                _TeleCommuteDays = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roCalendarAssignmentData
        Protected _ID As Double
        Protected _Name As String
        Protected _ShortName As String
        Protected _Color As String
        Protected _Suit As Double

        <DataMember()>
        Public Property ID As Double
            Get
                Return _ID
            End Get
            Set(value As Double)
                _ID = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        <DataMember()>
        Public Property ShortName As String
            Get
                Return _ShortName
            End Get
            Set(value As String)
                _ShortName = value
            End Set
        End Property

        <DataMember()>
        Public Property Color As String
            Get
                Return _Color
            End Get
            Set(value As String)
                _Color = value
            End Set
        End Property

        <DataMember()>
        Public Property Suit As Double
            Get
                Return _Suit
            End Get
            Set(value As Double)
                _Suit = value
            End Set
        End Property

        Public Sub New()
            _ID = 0
            _Name = ""
            _ShortName = ""
            _Color = ""
            _Suit = 0
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarAssignmentCellData
        Protected _ID As Double
        Protected _Name As String
        Protected _ShortName As String
        Protected _Color As String
        Protected _Cover As Double

        <DataMember()>
        Public Property ID As Double
            Get
                Return _ID
            End Get
            Set(value As Double)
                _ID = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        <DataMember()>
        Public Property ShortName As String
            Get
                Return _ShortName
            End Get
            Set(value As String)
                _ShortName = value
            End Set
        End Property

        <DataMember()>
        Public Property Color As String
            Get
                Return _Color
            End Get
            Set(value As String)
                _Color = value
            End Set
        End Property

        <DataMember()>
        Public Property Cover As Double
            Get
                Return _Cover
            End Get
            Set(value As Double)
                _Cover = value
            End Set
        End Property

        Public Sub New()
            _ID = -1
            _Name = ""
            _ShortName = ""
            _Color = ""
            _Cover = 0
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarRowSummaryData
        Protected _iAccrual As Double
        Protected _iAccrualHolidays As Double
        Protected _Alerts As roCalendarRowDayAlerts
        Protected _plannedHours As roPlannedHours
        Protected _holidaysResume As roHolidaysResume

        <DataMember()>
        Public Property Accrual As Double
            Get
                Return _iAccrual
            End Get
            Set(value As Double)
                _iAccrual = value
            End Set
        End Property

        <DataMember()>
        Public Property AccrualHolidays As Double
            Get
                Return _iAccrualHolidays
            End Get
            Set(value As Double)
                _iAccrualHolidays = value
            End Set
        End Property

        ''' <summary>
        ''' Alertas referidas al día de hoy (independientemente del periodo visualizado)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Alerts As roCalendarRowDayAlerts
            Get
                Return _Alerts
            End Get
            Set(value As roCalendarRowDayAlerts)
                _Alerts = value
            End Set
        End Property

        <DataMember()>
        Public Property PlannedHours As roPlannedHours
            Get
                Return _plannedHours
            End Get
            Set(value As roPlannedHours)
                _plannedHours = value
            End Set
        End Property

        <DataMember()>
        Public Property HolidayResume As roHolidaysResume
            Get
                Return _holidaysResume
            End Get
            Set(value As roHolidaysResume)
                _holidaysResume = value
            End Set
        End Property

        Public Sub New()
            _iAccrual = 0
            _iAccrualHolidays = 0
            _Alerts = Nothing
            _plannedHours = New roPlannedHours
            _holidaysResume = New roHolidaysResume
        End Sub

    End Class

    <DataContract()>
    Public Class roHolidaysResume

        Public Sub New()
            Done = 0
            Requested = 0
            Pending = 0
        End Sub

        <DataMember()>
        Public Property Done As Double

        <DataMember()>
        Public Property Requested As Double

        <DataMember()>
        Public Property Pending As Double
    End Class

    <DataContract()>
    Public Class roPlannedHours

        Public Sub New()
            YearTotal = 0
            AccruedToDate = 0
        End Sub

        <DataMember()>
        Public Property YearTotal As Double

        <DataMember()>
        Public Property AccruedToDate As Double
    End Class

    ''' <summary>
    ''' Representa toda la información sobre la planificación de un empleado para un periodo de fechas
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowPeriodData

        ''' <summary>Infomación de planificación del empleado para un día</summary>
        Protected _lstDayData As roCalendarRowDayData()

        Public Sub New()
            _lstDayData = Nothing
        End Sub

        ''' <summary>
        ''' Devuelve o establece un array de elementos con la información diaria de planificación de un empleado. Cada elemento contiene la información de un día del periodo de fechas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DayData As roCalendarRowDayData()
            Get
                Return _lstDayData
            End Get
            Set(value As roCalendarRowDayData())
                _lstDayData = value
            End Set
        End Property

    End Class

    ''' <summary>
    ''' Información de planificación de un día para un empleado
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowDayData

        Protected _Date As Date
        Protected _ShiftUsed As roCalendarRowShiftData
        Protected _MainShift As roCalendarRowShiftData
        Protected _AltShift1 As roCalendarRowShiftData
        Protected _AltShift2 As roCalendarRowShiftData
        Protected _AltShift3 As roCalendarRowShiftData
        Protected _ShiftBase As roCalendarRowShiftData
        Protected _PreviousShift As roCalendarRowShiftData
        Protected _isLocked As Boolean
        Protected _isFeast As Boolean
        Protected _FeastDescription As String
        Protected _isHoliday As Boolean
        Protected _canBeModified As Boolean
        Protected _eEmployeeStatusOnDay As EmployeeStatusOnDayEnum
        Protected _Alerts As roCalendarRowDayAlerts
        Protected _strFlagColor As String
        Protected _strRemarks As String 'En pantalla no sería necesaria hasta entrar en edición, pero para la importación exportación puede ser necesario
        Protected _lstHourData As roCalendarRowHourData() ' Para vista diaria (una para cada media hora)
        Protected _HasChanged As Boolean
        Protected _IncidenceData As roCalendarRowIncidenceData
        Protected _AssignmentData As roCalendarAssignmentCellData
        Protected _AllowAssignment As Boolean
        Protected _IDDailyBudgetPosition As Long
        Protected _ProductiveUnit As String
        Protected _lstPunchData As roCalendarRowPunchData() ' Fichajes del dia
        Protected _RequestedShift As String
        Protected _TelecommutingExpected As Boolean?
        Protected _TelecommutingOptional As Boolean?
        Protected _CanTelecommute As Boolean?
        Protected _TelecommuteForced As Boolean
        Protected _EmployeeWorkcenter As String
        Protected _ZoneName As String
        Protected _TelecommutingMandatoryDays As String
        Protected _PresenceMandatoryDays As String
        Protected _TelecommutingOptionalDays As String
        Protected _TelecommutingMaxDays As Integer
        Protected _TelecommutingMaxPercentage As Integer
        Protected _TelecommutingPeriodType As Integer
        Protected _Timestamp As DateTime

        ''' <summary>
        ''' Fecha a la que se refiere el resto de la información de planificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PlanDate As Date
            Get
                Return _Date
            End Get
            Set(value As Date)
                _Date = value
            End Set
        End Property
        ''' <summary>
        ''' Información del horario planificado para el día y el empleado en cuestión
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftUsed As roCalendarRowShiftData
            Get
                Return _ShiftUsed
            End Get
            Set(value As roCalendarRowShiftData)
                _ShiftUsed = value
            End Set
        End Property

        ''' <summary>
        ''' Información resumen sobre incidencias de gestión horaria del empleado en el día en cuestión
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IncidenceData As roCalendarRowIncidenceData
            Get
                Return _IncidenceData
            End Get
            Set(value As roCalendarRowIncidenceData)
                _IncidenceData = value
            End Set
        End Property

        ''' <summary>
        ''' Información resumen sobre planificación de puestos del empleado para una fecha
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AssigData As roCalendarAssignmentCellData
            Get
                Return _AssignmentData
            End Get
            Set(value As roCalendarAssignmentCellData)
                _AssignmentData = value
            End Set
        End Property

        ''' <summary>
        ''' Información del horario principal planificado para un empleado en un día concreto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MainShift As roCalendarRowShiftData
            Get
                Return _MainShift
            End Get
            Set(value As roCalendarRowShiftData)
                _MainShift = value
            End Set
        End Property

        ''' <summary>
        ''' Información del primer horario alternativo planificado para un empleado en un día concreto (sólo disponible si se dispone de licencia de Flexibilidad Horaria)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AltShift1 As roCalendarRowShiftData
            Get
                Return _AltShift1
            End Get
            Set(value As roCalendarRowShiftData)
                _AltShift1 = value
            End Set
        End Property

        ''' <summary>
        ''' Información del segundo horario alternativo planificado para un empleado en un día concreto (sólo disponible si se dispone de licencia de Flexibilidad Horaria)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AltShift2 As roCalendarRowShiftData
            Get
                Return _AltShift2
            End Get
            Set(value As roCalendarRowShiftData)
                _AltShift2 = value
            End Set
        End Property

        ''' <summary>
        ''' Información del tercer horario alternativo planificado para un empleado en un día concreto (sólo disponible si se dispone de licencia de Flexibilidad Horaria)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AltShift3 As roCalendarRowShiftData
            Get
                Return _AltShift3
            End Get
            Set(value As roCalendarRowShiftData)
                _AltShift3 = value
            End Set
        End Property

        ''' <summary>
        ''' Información del tercer horario base planificado para un empleado en un día concreto. Siempre que se planifica un horario de tipo festsivo, debe existir un horario base laboral definido
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftBase As roCalendarRowShiftData
            Get
                Return _ShiftBase
            End Get
            Set(value As roCalendarRowShiftData)
                _ShiftBase = value
            End Set
        End Property

        ''' <summary>
        ''' Información del horario que habñia planificado antes del actual
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PreviousShift As roCalendarRowShiftData
            Get
                Return _PreviousShift
            End Get
            Set(value As roCalendarRowShiftData)
                _PreviousShift = value
            End Set
        End Property

        ''' <summary>
        ''' Información resumen de estado de un empleado para un día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Alerts As roCalendarRowDayAlerts
            Get
                Return _Alerts
            End Get
            Set(value As roCalendarRowDayAlerts)
                _Alerts = value
            End Set
        End Property
        ''' <summary>
        ''' Indica si el día está bloqueado (es decir, no es modificable)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Locked As Boolean
            Get
                Return _isLocked
            End Get
            Set(value As Boolean)
                _isLocked = value
            End Set
        End Property

        ''' <summary>
        ''' Indica si el día es festivo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Feast As Boolean
            Get
                Return _isFeast
            End Get
            Set(value As Boolean)
                _isFeast = value
            End Set
        End Property

        ''' <summary>
        ''' Indica descripcion  del día festivo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FeastDescription As String
            Get
                Return _FeastDescription
            End Get
            Set(value As String)
                _FeastDescription = value
            End Set
        End Property

        ''' <summary>
        ''' Se puede asignar un puesto al empleado ese día (sólo disponible con licencia de Scheduling)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AllowAssignment As Boolean
            Get
                Return _AllowAssignment
            End Get
            Set(value As Boolean)
                _AllowAssignment = value
            End Set
        End Property

        <DataMember()>
        Public Property HasChanged As Boolean
            Get
                Return _HasChanged
            End Get
            Set(value As Boolean)
                _HasChanged = value
            End Set
        End Property
        <DataMember()>
        Public Property IsHoliday As Boolean
            Get
                Return _isHoliday
            End Get
            Set(value As Boolean)
                _isHoliday = value
            End Set
        End Property

        <DataMember()>
        Public Property IDDailyBudgetPosition As Long
            Get
                Return _IDDailyBudgetPosition
            End Get
            Set(value As Long)
                _IDDailyBudgetPosition = value
            End Set
        End Property

        <DataMember()>
        Public Property ProductiveUnit As String
            Get
                Return _ProductiveUnit
            End Get
            Set(value As String)
                _ProductiveUnit = value
            End Set
        End Property

        <DataMember()>
        Public Property RequestedShift As String
            Get
                Return _RequestedShift
            End Get
            Set(value As String)
                _RequestedShift = value
            End Set
        End Property

        <DataMember()>
        Public Property CanBeModified As Boolean
            Get
                Return _canBeModified
            End Get
            Set(value As Boolean)
                _canBeModified = value
            End Set
        End Property

        <DataMember()>
        Public Property Remarks As String
            Get
                Return _strRemarks
            End Get
            Set(value As String)
                _strRemarks = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeeStatusOnDay As EmployeeStatusOnDayEnum
            Get
                Return _eEmployeeStatusOnDay
            End Get
            Set(value As EmployeeStatusOnDayEnum)
                _eEmployeeStatusOnDay = value
            End Set
        End Property

        <DataMember()>
        Public Property FlagColor As String
            Get
                Return _strFlagColor
            End Get
            Set(value As String)
                _strFlagColor = value
            End Set
        End Property

        ''' <summary>
        ''' Infomación a nivel de horas sobre el horario planificado para el empleado en un día.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HourData As roCalendarRowHourData()
            Get
                Return _lstHourData
            End Get
            Set(value As roCalendarRowHourData())
                _lstHourData = value
            End Set
        End Property

        ''' <summary>
        ''' Lista de fichajes de presencia del dia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchData As roCalendarRowPunchData()
            Get
                Return _lstPunchData
            End Get
            Set(value As roCalendarRowPunchData())
                _lstPunchData = value
            End Set
        End Property

        ''' <summary>
        ''' Flag que indica a pantalla si el empleado puede teletrabajar ese día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanTelecommute As Nullable(Of Boolean)
            Get
                Return _CanTelecommute
            End Get
            Set(value As Nullable(Of Boolean))
                _CanTelecommute = value
            End Set
        End Property

        ''' <summary>
        ''' Flag que llega desde pantalla, e indica si debo guardar el estado de teletrabajo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommuteForced As Boolean
            Get
                Return _TelecommuteForced
            End Get
            Set(value As Boolean)
                _TelecommuteForced = value
            End Set
        End Property

        ''' <summary>
        ''' Si tiene planificado teletrabajo, o es obligatorio que teletrabaje por su acuerdo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingExpected As Nullable(Of Boolean)
            Get
                Return _TelecommutingExpected
            End Get
            Set(value As Nullable(Of Boolean))
                _TelecommutingExpected = value
            End Set
        End Property

        ''' <summary>
        ''' Si el empleado puede elegir si ese día trabaja presencialmente o teletrabaja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingOptional As Nullable(Of Boolean)
            Get
                Return _TelecommutingOptional
            End Get
            Set(value As Nullable(Of Boolean))
                _TelecommutingOptional = value
            End Set
        End Property

        ''' <summary>
        ''' Días de la semana (lunes=1...domingo=7) en los que debo teletrabajar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingMandatoryDays As String
            Get
                Return _TelecommutingMandatoryDays
            End Get
            Set(value As String)
                _TelecommutingMandatoryDays = value
            End Set
        End Property

        ''' <summary>
        ''' Días de la semana (lunes=1...domingo=7) en los que debo trabajar presencialmente
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PresenceMandatoryDays As String
            Get
                Return _PresenceMandatoryDays
            End Get
            Set(value As String)
                _PresenceMandatoryDays = value
            End Set
        End Property

        ''' <summary>
        ''' Días de la semana (lunes=1...domingo=7) en los que puedo teletrabajar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingOptionalDays As String
            Get
                Return _TelecommutingOptionalDays
            End Get
            Set(value As String)
                _TelecommutingOptionalDays = value
            End Set
        End Property

        ''' <summary>
        ''' Máximo numero de días de teletrabajo en el periodo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingMaxDays As Integer
            Get
                Return _TelecommutingMaxDays
            End Get
            Set(value As Integer)
                _TelecommutingMaxDays = value
            End Set
        End Property

        ''' <summary>
        ''' Máximo numero de días de teletrabajo en el periodo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TelecommutingMaxPercentage As Integer
            Get
                Return _TelecommutingMaxPercentage
            End Get
            Set(value As Integer)
                _TelecommutingMaxPercentage = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingPeriodType As Integer
            Get
                Return _TelecommutingPeriodType
            End Get
            Set(value As Integer)
                _TelecommutingPeriodType = value
            End Set
        End Property

        <DataMember()>
        Public Property Timestamp As DateTime
            Get
                Return _Timestamp
            End Get
            Set(value As DateTime)
                _Timestamp = value
            End Set
        End Property

        <DataMember()>
        Public Property Workcenter As String
            Get
                Return _EmployeeWorkcenter
            End Get
            Set(value As String)
                _EmployeeWorkcenter = value
            End Set
        End Property

        <DataMember()>
        Public Property ZoneName As String
            Get
                Return _ZoneName
            End Get
            Set(value As String)
                _ZoneName = value
            End Set
        End Property

        Public Sub New()
            _Date = Date.Now.Date
            _ShiftUsed = Nothing
            _MainShift = Nothing
            _AltShift1 = Nothing
            _AltShift2 = Nothing
            _AltShift3 = Nothing
            _ShiftBase = Nothing
            _isLocked = False
            _isFeast = False
            _FeastDescription = ""
            _isHoliday = False
            _canBeModified = False
            _Alerts = Nothing
            _strFlagColor = String.Empty
            _strRemarks = String.Empty
            _eEmployeeStatusOnDay = EmployeeStatusOnDayEnum.Ok
            _lstHourData = Nothing
            _lstPunchData = Nothing
            _IncidenceData = Nothing
            _AssignmentData = Nothing
            _AllowAssignment = False
            IDDailyBudgetPosition = 0
            _TelecommuteForced = False
            _Timestamp = New DateTime(1970, 1, 1, 0, 0, 0)
        End Sub

    End Class

    ''' <summary>
    ''' Información de un fichaje
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowPunchData
        Protected _ID As Long
        Protected _ActualType As CalendarPunchTypeEnum
        Protected _DateTime As DateTime

        ''' <summary>
        ''' Identificador del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID As Long
            Get
                Return _ID
            End Get
            Set(value As Long)
                _ID = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha/Hora del fichaje
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property DateTimePunch As DateTime
            Get
                Return _DateTime
            End Get
            Set(value As DateTime)
                _DateTime = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ActualType As CalendarPunchTypeEnum
            Get
                Return _ActualType
            End Get
            Set(value As CalendarPunchTypeEnum)
                _ActualType = value
            End Set
        End Property

        Public Sub New()
            _ID = -1
            _ActualType = CalendarPunchTypeEnum._NOTDEFINDED
            _DateTime = Nothing
        End Sub

    End Class

    ''' <summary>
    ''' Información de un horario
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowShiftData
        Protected _id As Integer
        Protected _strShortName As String
        Protected _strExport As String
        Protected _strName As String
        Protected _strDescription As String
        Protected _iPlannedHours As Double
        Protected _iBreakHours As Double
        Protected _strColor As String
        Protected _datStartHour As DateTime
        Protected _datEndHour As DateTime
        Protected _ShiftLayersDefinition As roCalendarShiftLayersDefinition()
        Protected _eType As ShiftTypeEnum
        Protected _ExistComplementaryData As Boolean
        Protected _ExistFloatingData As Boolean
        Protected _ShiftLayers As Integer
        Protected _AdvParameters As roShiftAdvParameters()
        Protected _WhoToNotifyBefore As Integer
        Protected _WhoToNotifyAfter As Integer
        Protected _NotifyEmployeeBeforeAt As Integer
        Protected _NotifyEmployeeAfterAt As Integer
        Protected _EnableNotifyBefore As Boolean
        Protected _EnableNotifyAfter As Boolean

        ''' <summary>
        ''' Identificador del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ID
            Get
                Return _id
            End Get
            Set(value)
                _id = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre corto del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShortName
            Get
                Return _strShortName
            End Get
            Set(value)
                _strShortName = value
            End Set
        End Property

        ''' <summary>
        ''' Equivalencia del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Export
            Get
                Return _strExport
            End Get
            Set(value)
                _strExport = value
            End Set
        End Property
        ''' <summary>
        ''' Nombre del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name
            Get
                Return _strName
            End Get
            Set(value)
                _strName = value
            End Set
        End Property
        ''' <summary>
        ''' Nombre del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description
            Get
                Return _strDescription
            End Get
            Set(value)
                _strDescription = value
            End Set
        End Property
        ''' <summary>
        ''' Color asignado al horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Color As String
            Get
                Return _strColor
            End Get
            Set(value As String)
                _strColor = value
            End Set
        End Property
        ''' <summary>
        ''' Número de franjas del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftLayers As Integer
            Get
                Return _ShiftLayers
            End Get
            Set(value As Integer)
                _ShiftLayers = value
            End Set
        End Property

        ''' <summary>
        ''' Horas planificadas (para totalizadores en pantalla)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property PlannedHours As Double
            Get
                Return _iPlannedHours
            End Get
            Set(value As Double)
                _iPlannedHours = value
            End Set
        End Property

        ''' <summary>
        ''' Horas de descanso
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property BreakHours As Double
            Get
                Return _iBreakHours
            End Get
            Set(value As Double)
                _iBreakHours = value
            End Set
        End Property

        ''' <summary>
        ''' Hora de inicio para horarios flotantes, si aplica
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StartHour As DateTime
            Get
                Return _datStartHour
            End Get
            Set(value As DateTime)
                _datStartHour = value
            End Set
        End Property

        ''' <summary>
        ''' Hora fin del horario
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property EndHour As DateTime
            Get
                Return _datEndHour
            End Get
            Set(value As DateTime)
                _datEndHour = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de horario: 0=Normal, 1=Flotante, 2=Festivo aplicables a días laborables a efectos de contabilización de vacaciones, 3=Festivo aplicable a días naturales a efectos de contabilización de vacaciones
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Type As ShiftTypeEnum
            Get
                Return _eType
            End Get
            Set(value As ShiftTypeEnum)
                _eType = value
            End Set
        End Property

        ''' <summary>
        ''' Día con información sobre horarios que incluyen horas complemetarias
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ExistComplementaryData As Boolean
            Get
                Return _ExistComplementaryData
            End Get
            Set(value As Boolean)
                _ExistComplementaryData = value
            End Set
        End Property

        ''' <summary>
        ''' Día con información sobre horarios que incluyen definición flotante
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ExistFloatingData As Boolean
            Get
                Return _ExistFloatingData
            End Get
            Set(value As Boolean)
                _ExistFloatingData = value
            End Set
        End Property

        ''' <summary>
        ''' Array con la definición de las distintas franjas de un horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShiftLayersDefinition As roCalendarShiftLayersDefinition()
            Get
                Return _ShiftLayersDefinition
            End Get
            Set(value As roCalendarShiftLayersDefinition())
                _ShiftLayersDefinition = value
            End Set
        End Property

        <DataMember()>
        Public Property AdvancedParameters As roShiftAdvParameters()
            Get
                Return _AdvParameters
            End Get
            Set(value As roShiftAdvParameters())
                _AdvParameters = value
            End Set
        End Property

        ''' <summary>
        ''' A quién notificar si se ficha antes la jornada
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property WhoToNotifyBefore As Integer
            Get
                Return _WhoToNotifyBefore
            End Get
            Set(value As Integer)
                _WhoToNotifyBefore = value
            End Set
        End Property

        ''' <summary>
        ''' A quién notificar si aun no se ha fichado después de X tiempo desde el inicio de la jornada
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property WhoToNotifyAfter As Integer
            Get
                Return _WhoToNotifyAfter
            End Get
            Set(value As Integer)
                _WhoToNotifyAfter = value
            End Set
        End Property

        ''' <summary>
        ''' Tiempo X antes la jornada que no se permite fichar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property NotifyEmployeeBeforeAt As Integer
            Get
                Return _NotifyEmployeeBeforeAt
            End Get
            Set(value As Integer)
                _NotifyEmployeeBeforeAt = value
            End Set
        End Property

        ''' <summary>
        ''' Tiempo X que ha pasado desde la hora de inicio de la jornada y aún no se ha fichado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property NotifyEmployeeAfterAt As Integer
            Get
                Return _NotifyEmployeeAfterAt
            End Get
            Set(value As Integer)
                _NotifyEmployeeAfterAt = value
            End Set
        End Property

        ''' <summary>
        ''' Habilita la notificación de recordatorio de fichaje antes de inico de jornada
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property EnableNotifyBefore As Boolean
            Get
                Return _EnableNotifyBefore
            End Get
            Set(value As Boolean)
                _EnableNotifyBefore = value
            End Set
        End Property

        ''' <summary>
        ''' Habilita la notificación de fichaje después de X tiempo del incio de la jornada
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property EnableNotifyAfter As Boolean
            Get
                Return _EnableNotifyAfter
            End Get
            Set(value As Boolean)
                _EnableNotifyAfter = value
            End Set
        End Property

        Public Sub New()
            _id = -1
            _iPlannedHours = 0
            _iBreakHours = 0
            _strShortName = String.Empty
            _strName = String.Empty
            _strColor = String.Empty
            _datStartHour = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            _datEndHour = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)
            _eType = ShiftTypeEnum.Normal
            _ShiftLayersDefinition = Nothing
            _ExistComplementaryData = False
            _ExistFloatingData = False
            _WhoToNotifyBefore = 0
            _WhoToNotifyAfter = 0
            _NotifyEmployeeBeforeAt = 0
            _NotifyEmployeeAfterAt = 0
            _EnableNotifyBefore = False
            _EnableNotifyAfter = False
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftAdvParameters
        Protected _parametername As String
        Protected _parametervalue As String
        <DataMember()>
        Public Property Name As String
            Get
                Return _parametername
            End Get
            Set(value As String)
                _parametername = value
            End Set
        End Property
        <DataMember()>
        Public Property Value As String
            Get
                Return _parametervalue
            End Get
            Set(value As String)
                _parametervalue = value
            End Set
        End Property

        Public Sub New(spatametername As String, sparametervalue As String)
            _parametername = spatametername
            _parametervalue = sparametervalue
        End Sub

    End Class

    ''' <summary>
    ''' Información resumen de estado de un empleado para un día
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowDayAlerts
        Protected _onAbsenceDays As Boolean
        Protected _onAbsenceDaysInfo As String
        Protected _onAbsenceHours As Boolean
        Protected _onAbsenceHoursInfo As String
        Protected _onHolidays As Boolean
        Protected _onHolidaysHours As Boolean
        Protected _onHolidaysHoursInfo As String
        Protected _onOvertimesHours As Boolean
        Protected _onOvertimesHoursInfo As String

        Protected _isUnexpectedlyAbsent As Boolean
        Protected _Indictments As roCalendarScheduleIndictment()

        ''' <summary>
        ''' Empleado en ausencia por días en el día de hoy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property OnAbsenceDays As Boolean
            Get
                Return _onAbsenceDays
            End Get
            Set(value As Boolean)
                _onAbsenceDays = value
            End Set
        End Property

        <DataMember()>
        Public Property OnAbsenceDaysInfo As String
            Get
                Return _onAbsenceDaysInfo
            End Get
            Set(value As String)
                _onAbsenceDaysInfo = value
            End Set
        End Property

        ''' <summary>
        ''' Empleado en ausencia por horas en el día de hoy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property OnAbsenceHours As Boolean
            Get
                Return _onAbsenceHours
            End Get
            Set(value As Boolean)
                _onAbsenceHours = value
            End Set
        End Property

        <DataMember()>
        Public Property OnAbsenceHoursInfo As String
            Get
                Return _onAbsenceHoursInfo
            End Get
            Set(value As String)
                _onAbsenceHoursInfo = value
            End Set
        End Property

        ''' <summary>
        ''' Empleado de vacaciones el día de hoy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property OnHolidays As Boolean
            Get
                Return _onHolidays
            End Get
            Set(value As Boolean)
                _onHolidays = value
            End Set
        End Property

        ''' <summary>
        ''' Empleado de vacaciones por horas
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property OnHolidaysHours As Boolean
            Get
                Return _onHolidaysHours
            End Get
            Set(value As Boolean)
                _onHolidaysHours = value
            End Set
        End Property

        <DataMember()>
        Public Property OnHolidaysHoursInfo As String
            Get
                Return _onHolidaysHoursInfo
            End Get
            Set(value As String)
                _onHolidaysHoursInfo = value
            End Set
        End Property

        ''' <summary>
        ''' Empleado realizando horas de exceso
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property OnOvertimesHours As Boolean
            Get
                Return _onOvertimesHours
            End Get
            Set(value As Boolean)
                _onOvertimesHours = value
            End Set
        End Property

        <DataMember()>
        Public Property OnOvertimesHoursInfo As String
            Get
                Return _onOvertimesHoursInfo
            End Get
            Set(value As String)
                _onOvertimesHoursInfo = value
            End Set
        End Property

        ''' <summary>
        ''' Empleado ausente sin motivo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property UnexpectedlyAbsent As Boolean
            Get
                Return _isUnexpectedlyAbsent
            End Get
            Set(value As Boolean)
                _isUnexpectedlyAbsent = value
            End Set
        End Property

        ''' <summary>
        ''' Array con los indicadores de planificacion
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Indictments As roCalendarScheduleIndictment()
            Get
                Return _Indictments
            End Get
            Set(value As roCalendarScheduleIndictment())
                _Indictments = value
            End Set
        End Property

        Public Sub New()
            _onAbsenceDays = False
            _onAbsenceDaysInfo = ""

            _onAbsenceHours = False
            _onAbsenceHoursInfo = ""
            _onHolidays = False
            _isUnexpectedlyAbsent = False
            _onHolidaysHours = False
            _onHolidaysHoursInfo = ""
            _onOvertimesHours = False
            _onOvertimesHoursInfo = ""
            _Indictments = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarRowHourData
        Protected _DailyHourType As DailyHourTypeEnum
        Protected _inHoursAbsence As Boolean
        Protected _inHoursHoliday As Boolean
        Protected _inHoursOvertime As Boolean

        <DataMember()>
        Public Property DailyHourType As DailyHourTypeEnum
            Get
                Return _DailyHourType
            End Get
            Set(value As DailyHourTypeEnum)
                _DailyHourType = value
            End Set
        End Property
        <DataMember()>
        Public Property IsHoursAbsence As Boolean
            Get
                Return _inHoursAbsence
            End Get
            Set(value As Boolean)
                _inHoursAbsence = value
            End Set
        End Property

        <DataMember()>
        Public Property IsHoursHoliday As Boolean
            Get
                Return _inHoursHoliday
            End Get
            Set(value As Boolean)
                _inHoursHoliday = value
            End Set
        End Property

        <DataMember()>
        Public Property IsHoursOvertime As Boolean
            Get
                Return _inHoursOvertime
            End Get
            Set(value As Boolean)
                _inHoursOvertime = value
            End Set
        End Property

        Public Sub New()
            _DailyHourType = DailyHourTypeEnum.Untyped
            _inHoursAbsence = False
            _inHoursHoliday = False
            _inHoursOvertime = False
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarShiftAssignmentData
        Protected _IDAssig As Double
        Protected _Cover As Double

        <DataMember()>
        Public Property IDAssig As Double
            Get
                Return _IDAssig
            End Get
            Set(value As Double)
                _IDAssig = value
            End Set
        End Property
        <DataMember()>
        Public Property Cover As Double
            Get
                Return _Cover
            End Get
            Set(value As Double)
                _Cover = value
            End Set
        End Property

        Public Sub New()
            _Cover = 0
            _IDAssig = 0
        End Sub

    End Class

    ''' <summary>
    ''' Proporciona información de resumen del día sobre incidencias de gestión horaria del empleado para el día en concreto
    ''' </summary>
    <DataContract()>
    Public Class roCalendarRowIncidenceData
        Protected _dblAbsence As Double
        Protected _dblNormalWork As Double
        Protected _dblOverWorking As Double

        Protected _bRemark1 As Boolean
        Protected _bRemark2 As Boolean
        Protected _bRemark3 As Boolean

        ''' <summary>
        ''' Total de horas de ausencia del empleado en el día. Corresponde al valor del saldo configurado como saldo de horas de ausencia.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Absence As Double
            Get
                Return _dblAbsence
            End Get
            Set(value As Double)
                _dblAbsence = value
            End Set
        End Property

        ''' <summary>
        ''' Total de horas trabajadas del empleado en el día. Corresponde al valor del saldo configurado como saldo de horas trabajadas.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NormalWork As Double
            Get
                Return _dblNormalWork
            End Get
            Set(value As Double)
                _dblNormalWork = value
            End Set
        End Property

        ''' <summary>
        ''' Total de horas extras del empleado en el día. Corresponde al valor del saldo configurado como saldo de horas extras.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property OverWorking As Double
            Get
                Return _dblOverWorking
            End Get
            Set(value As Double)
                _dblOverWorking = value
            End Set
        End Property

        ''' <summary>
        ''' Indica si el empleado tiene alguna incidencia sin justificar ese día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Remark1 As Boolean
            Get
                Return _bRemark1
            End Get
            Set(value As Boolean)
                _bRemark1 = value
            End Set
        End Property
        ''' <summary>
        ''' Indica si el empleado tiene alguna incidencia de ausencia ese día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Remark2 As Boolean
            Get
                Return _bRemark2
            End Get
            Set(value As Boolean)
                _bRemark2 = value
            End Set
        End Property
        ''' <summary>
        ''' Indica si el empleado tiene alguna incidencia de horas extras ese día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Remark3 As Boolean
            Get
                Return _bRemark3
            End Get
            Set(value As Boolean)
                _bRemark3 = value
            End Set
        End Property

        Public Sub New()
            _dblNormalWork = 0
            _dblAbsence = 0
            _dblOverWorking = 0
            _bRemark1 = False
            _bRemark2 = False
            _bRemark3 = False
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarIndictmentResult
        Protected _intStatus As CalendarStatusEnum
        Protected _Calendar As roCalendar

        <DataMember()>
        Public Property Status As CalendarStatusEnum
            Get
                Return _intStatus
            End Get
            Set(value As CalendarStatusEnum)
                _intStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property Calendar As roCalendar
            Get
                Return _Calendar
            End Get
            Set(value As roCalendar)
                _Calendar = value
            End Set
        End Property

        Public Sub New()
            _intStatus = CalendarStatusEnum.OK
            _Calendar = Nothing
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarResult
        Protected _intStatus As CalendarStatusEnum
        Protected _lstCalendarDataResult As roCalendarDataDayError()

        <DataMember()>
        Public Property Status As CalendarStatusEnum
            Get
                Return _intStatus
            End Get
            Set(value As CalendarStatusEnum)
                _intStatus = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarDataResult As roCalendarDataDayError()
            Get
                Return _lstCalendarDataResult
            End Get
            Set(value As roCalendarDataDayError())
                _lstCalendarDataResult = value
            End Set
        End Property

        Public Sub New()
            _intStatus = CalendarStatusEnum.OK
            _lstCalendarDataResult = {}
        End Sub

    End Class

    <DataContract()>
    Public Class roCalendarDataDayError
        Protected _intIDEmployee As Integer
        Protected _Date As Date
        Protected _ErrorCode As CalendarErrorResultDayEnum
        Protected _Errortext As String

        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return _intIDEmployee
            End Get
            Set(value As Integer)
                _intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorDate As Date
            Get
                Return _Date
            End Get
            Set(value As Date)
                _Date = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorCode As CalendarErrorResultDayEnum
            Get
                Return _ErrorCode
            End Get
            Set(value As CalendarErrorResultDayEnum)
                _ErrorCode = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorText As String
            Get
                Return _Errortext
            End Get
            Set(value As String)
                _Errortext = value
            End Set
        End Property

        Public Sub New()
            _intIDEmployee = 0
            _Date = Now.Date
            _ErrorCode = CalendarErrorResultDayEnum.NoContract
        End Sub

    End Class

    <DataContract()>
    Public Class roBreakLayerDefinition

        <DataMember()>
        Public Property Start As DateTime

        <DataMember()>
        Public Property Finish As DateTime

    End Class

    <DataContract()>
    Public Class roCalendarShift
        Dim _intIDShift As Integer

        Dim _isFloating As Boolean
        Dim _dWorkingHours As Double
        Dim _xStartFloating As DateTime
        Dim _xEndFloating As DateTime

        Dim _allowFloating As Boolean
        Dim _allowComplementary As Boolean

        Dim _strName As String
        Dim _strShortName As String
        Dim _strColor As String
        Dim _type As ShiftTypeEnum

        Dim _intIDLayer1 As Integer
        Dim _intIDLayer2 As Integer
        Dim _intCountLayers As Integer
        Dim _bolAllowComplementary1 As Boolean
        Dim _bolAllowComplementary2 As Boolean
        Dim _bolAllowModifyIniHour1 As Boolean
        Dim _bolAllowModifyDuration1 As Boolean
        Dim _bolAllowModifyIniHour2 As Boolean
        Dim _bolAllowModifyDuration2 As Boolean
        Dim _bolHasLayer1FixedEnd As Boolean
        Dim _bolHasLayer2FixedEnd As Boolean
        Dim _dBreakHours As Double
        Dim _xStartLayer1 As DateTime
        Dim _xStartLayer2 As DateTime
        Dim _xEndLayer1 As DateTime
        Dim _xEndLayer2 As DateTime
        Dim _lstCalendarShiftAssignmentData As roCalendarShiftAssignmentData()
        Dim _advancedParameters As roShiftAdvParameters()
        Dim _breakLayers As roBreakLayerDefinition()

        Public Sub New()
            _intIDShift = 0
            _isFloating = False
            _dWorkingHours = 0
            _allowComplementary = False
            _allowFloating = False
            _xStartFloating = Date.MinValue
            _xEndFloating = Date.MinValue

            _type = ShiftTypeEnum.Normal

            _strName = ""
            _strShortName = ""
            _intIDLayer1 = -1
            _intIDLayer2 = -1
            _intCountLayers = 0
            _bolAllowComplementary1 = False
            _bolAllowComplementary2 = False
            _bolAllowModifyIniHour1 = False
            _bolAllowModifyDuration1 = False
            _bolAllowModifyIniHour2 = False
            _bolAllowModifyDuration2 = False
            _bolHasLayer1FixedEnd = False
            _bolHasLayer2FixedEnd = False
            _dBreakHours = 0

            _xStartLayer1 = Date.MinValue
            _xStartLayer2 = Date.MinValue
            _xEndLayer1 = Date.MinValue
            _xEndLayer2 = Date.MinValue

            _lstCalendarShiftAssignmentData = Nothing
            _advancedParameters = {}
        End Sub

        <DataMember()>
        Public Property BreakLayers As roBreakLayerDefinition()
            Get
                Return _breakLayers
            End Get
            Set(value As roBreakLayerDefinition())
                _breakLayers = value
            End Set
        End Property

        <DataMember()>
        Public Property AdvancedParameters As roShiftAdvParameters()
            Get
                Return _advancedParameters
            End Get
            Set(value As roShiftAdvParameters())
                _advancedParameters = value
            End Set
        End Property

        <DataMember()>
        Public Property Assignments As roCalendarShiftAssignmentData()
            Get
                Return _lstCalendarShiftAssignmentData
            End Get
            Set(value As roCalendarShiftAssignmentData())
                _lstCalendarShiftAssignmentData = value
            End Set
        End Property

        <DataMember()>
        Public Property Type As ShiftTypeEnum
            Get
                Return _type
            End Get
            Set(value As ShiftTypeEnum)
                _type = value
            End Set
        End Property

        <DataMember()>
        Public Property IDShift As Integer
            Get
                Return _intIDShift
            End Get
            Set(value As Integer)
                _intIDShift = value
            End Set
        End Property

        <DataMember()>
        Public Property IsFloating As Boolean
            Get
                Return _isFloating
            End Get
            Set(value As Boolean)
                _isFloating = value
            End Set
        End Property

        <DataMember()>
        Public Property WorkingHours As Double
            Get
                Return _dWorkingHours
            End Get
            Set(value As Double)
                _dWorkingHours = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowFloating As Boolean
            Get
                Return _allowFloating
            End Get
            Set(value As Boolean)
                _allowFloating = value
            End Set
        End Property

        <DataMember()>
        Public Property StartFloating As DateTime
            Get
                Return _xStartFloating
            End Get
            Set(value As DateTime)
                _xStartFloating = value
            End Set
        End Property

        <DataMember()>
        Public Property EndFloating As DateTime
            Get
                Return _xEndFloating
            End Get
            Set(value As DateTime)
                _xEndFloating = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowComplementary As Boolean
            Get
                Return _allowComplementary
            End Get
            Set(value As Boolean)
                _allowComplementary = value
            End Set
        End Property

        <DataMember()>
        Public Property Color As String
            Get
                Return _strColor
            End Get
            Set(value As String)
                _strColor = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _strName
            End Get
            Set(value As String)
                _strName = value
            End Set
        End Property

        <DataMember()>
        Public Property ShortName As String
            Get
                Return _strShortName
            End Get
            Set(value As String)
                _strShortName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDLayer1 As Integer
            Get
                Return _intIDLayer1
            End Get
            Set(value As Integer)
                _intIDLayer1 = value
            End Set
        End Property

        <DataMember()>
        Public Property IDLayer2 As Integer
            Get
                Return _intIDLayer2
            End Get
            Set(value As Integer)
                _intIDLayer2 = value
            End Set
        End Property

        <DataMember()>
        Public Property CountLayers As Integer
            Get
                Return _intCountLayers
            End Get
            Set(value As Integer)
                _intCountLayers = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowComplementary1 As Boolean
            Get
                Return _bolAllowComplementary1
            End Get
            Set(value As Boolean)
                _bolAllowComplementary1 = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowComplementary2 As Boolean
            Get
                Return _bolAllowComplementary2
            End Get
            Set(value As Boolean)
                _bolAllowComplementary2 = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowModifyIniHour1 As Boolean
            Get
                Return _bolAllowModifyIniHour1
            End Get
            Set(value As Boolean)
                _bolAllowModifyIniHour1 = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowModifyIniHour2 As Boolean
            Get
                Return _bolAllowModifyIniHour2
            End Get
            Set(value As Boolean)
                _bolAllowModifyIniHour2 = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowModifyDuration1 As Boolean
            Get
                Return _bolAllowModifyDuration1
            End Get
            Set(value As Boolean)
                _bolAllowModifyDuration1 = value
            End Set
        End Property

        <DataMember()>
        Public Property AllowModifyDuration2 As Boolean
            Get
                Return _bolAllowModifyDuration2
            End Get
            Set(value As Boolean)
                _bolAllowModifyDuration2 = value
            End Set
        End Property

        <DataMember()>
        Public Property HasLayer1FixedEnd As Boolean
            Get
                Return _bolHasLayer1FixedEnd
            End Get
            Set(value As Boolean)
                _bolHasLayer1FixedEnd = value
            End Set
        End Property

        <DataMember()>
        Public Property HasLayer2FixedEnd As Boolean
            Get
                Return _bolHasLayer2FixedEnd
            End Get
            Set(value As Boolean)
                _bolHasLayer2FixedEnd = value
            End Set
        End Property

        <DataMember()>
        Public Property BreakHours As Double
            Get
                Return _dBreakHours
            End Get
            Set(value As Double)
                _dBreakHours = value
            End Set
        End Property

        <DataMember()>
        Public Property StartLayer1 As DateTime
            Get
                Return _xStartLayer1
            End Get
            Set(value As DateTime)
                _xStartLayer1 = value
            End Set
        End Property

        <DataMember()>
        Public Property StartLayer2 As DateTime
            Get
                Return _xStartLayer2
            End Get
            Set(value As DateTime)
                _xStartLayer2 = value
            End Set
        End Property

        <DataMember()>
        Public Property EndLayer1 As DateTime
            Get
                Return _xEndLayer1
            End Get
            Set(value As DateTime)
                _xEndLayer1 = value
            End Set
        End Property

        <DataMember()>
        Public Property EndLayer2 As DateTime
            Get
                Return _xEndLayer2
            End Get
            Set(value As DateTime)
                _xEndLayer2 = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roCalendarScheduleIndictment
        Protected _intID As Integer
        Protected _intIDEmployee As Integer
        Protected _intIDScheduleRule As Integer
        Protected _strRuleName As String
        Protected _RuleName As String
        Protected _DateBegin As Date
        Protected _DateEnd As Date
        Protected _Dates() As Date
        Protected _ErrorText As String = String.Empty

        <DataMember()>
        Public Property ID As Integer
            Get
                Return _intID
            End Get
            Set(value As Integer)
                _intID = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return _intIDEmployee
            End Get
            Set(value As Integer)
                _intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property IDScheduleRule As Integer
            Get
                Return _intIDScheduleRule
            End Get
            Set(value As Integer)
                _intIDScheduleRule = value
            End Set
        End Property

        <DataMember()>
        Public Property RuleName As String
            Get
                Return _strRuleName
            End Get
            Set(value As String)
                _strRuleName = value
            End Set
        End Property

        <DataMember()>
        Public Property DateBegin As Date
            Get
                Return _DateBegin
            End Get
            Set(value As Date)
                _DateBegin = value
            End Set
        End Property

        <DataMember()>
        Public Property DateEnd As Date
            Get
                Return _DateEnd
            End Get
            Set(value As Date)
                _DateEnd = value
            End Set
        End Property

        <DataMember()>
        Public Property ErrorText As String
            Get
                Return _ErrorText
            End Get
            Set(value As String)
                _ErrorText = value
            End Set
        End Property

        <DataMember()>
        Public Property Dates As Date()
            Get
                Return _Dates
            End Get
            Set(value As Date())
                _Dates = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roCalendarResponse

        Public Sub New()
            Calendar = Nothing
            oState = Nothing
            CalendarResult = New roCalendarResult
        End Sub

        <DataMember>
        Public Property Calendar As roCalendar

        <DataMember>
        Public Property oState As roWsState

        <DataMember>
        Public Property CalendarResult As roCalendarResult

    End Class

    <DataContract()>
    Public Class roCalendarIndictmentResultResponse
        <DataMember>
        Public Property CalendarIndictment As roCalendarIndictmentResult
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarCoverageDaysResponse
        <DataMember>
        Public Property CalendarCoverageDays As roCalendarCoverageDay()
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarPassportConfigResponse
        <DataMember>
        Public Property CalendarPassportConfig As roCalendarPassportConfig
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarRowDayDataResponse
        <DataMember>
        Public Property CalendarDay As roCalendarRowDayData
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarRowHourDataResponse
        <DataMember>
        Public Property CalendarRowHourData As roCalendarRowHourData()
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarCoverageDayResponse
        <DataMember>
        Public Property CalendarCoverageDay As roCalendarCoverageDay
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roCalendarFile
        <DataMember>
        Public Property XlsByteArray As Byte()
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roExcelCalendar
        <DataMember>
        Public Property Calendar As roCalendar

        <DataMember>
        Public Property oState As roWsState

        <DataMember>
        Public Property CalendarResult As roCalendarResult

        <DataMember>
        Public Property FileNameError As String
    End Class

    <DataContract()>
    Public Class roCalendarShiftResponse
        <DataMember>
        Public Property CalendarShift As roCalendarShift
        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract()>
    Public Class roHolidayResumeInfo
        <DataMember>
        Public Property BeginPeriod As DateTime
        <DataMember>
        Public Property EndPeriod As DateTime

        <DataMember>
        Public Property Done As Double
        <DataMember>
        Public Property Lasting As Double
        <DataMember>
        Public Property Disponible As Double
        <DataMember>
        Public Property Pending As Double

        <DataMember>
        Public Property ExpiredDays As Double

        <DataMember>
        Public Property DaysWithoutEnjoyment As Double

    End Class

End Namespace