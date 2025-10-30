Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum DailyConditionValueType
        <EnumMember()> DirectValue
        <EnumMember()> UserField
        <EnumMember()> ID
    End Enum

    <DataContract()>
    Public Enum DailyConditionCompareType
        <EnumMember()> Equal
        <EnumMember()> Minor
        <EnumMember()> MinorEqual
        <EnumMember()> Major
        <EnumMember()> MajorEqual
        <EnumMember()> Distinct
        <EnumMember()> Between
    End Enum

    <DataContract()>
    Public Enum RuleAction
        <EnumMember()> CarryOver
        <EnumMember()> Plus
        <EnumMember()> CarryOverSingle
    End Enum

    <DataContract()>
    Public Enum RulePart
        <EnumMember()> Part1
        <EnumMember()> Part2
    End Enum

    <DataContract()>
    Public Enum RuleCondition
        <EnumMember()> Condition1
        <EnumMember()> Condition2
        <EnumMember()> Condition3
    End Enum

    <DataContract()>
    Public Enum OperatorCondition
        <EnumMember()> Positive
        <EnumMember()> Negative
    End Enum

    <DataContract()>
    Public Enum DayValidationRule
        <EnumMember()> Anyday_DayValidationRule
        <EnumMember()> Monday_DayValidationRule
        <EnumMember()> Tuesday_DayValidationRule
        <EnumMember()> Wednesday_DayValidationRule
        <EnumMember()> Thursday_DayValidationRule
        <EnumMember()> Friday_DayValidationRule
        <EnumMember()> Saturday_DayValidationRule
        <EnumMember()> Sunday_DayValidationRule
        <EnumMember()> Feast_DayValidationRule
        <EnumMember()> TelecommutingEfective_DayValidationRule
        <EnumMember()> TelecommutingPlanned_DayValidationRule
    End Enum

    <DataContract()>
    Public Enum roXmlLayerKeys
        <EnumMember> Begin
        <EnumMember> Finish
        <EnumMember> MaxTime
        <EnumMember> MaxTimeAction
        <EnumMember> MinTime
        <EnumMember> FloatingBeginUpTo
        <EnumMember> FloatingFinishMinutes
        <EnumMember> Value
        <EnumMember> Action
        <EnumMember> Target
        <EnumMember> MaxBreakTime
        <EnumMember> MaxBreakAction
        <EnumMember> MinBreakTime
        <EnumMember> MinBreakAction
        <EnumMember> NoPunchBreakTime
        <EnumMember> AllowModifyIniHour
        <EnumMember> AllowModifyDuration
        <EnumMember> NotificationForUser
        <EnumMember> NotificationForSupervisor
        <EnumMember> NotificationForUserBeforeTime
        <EnumMember> NotificationForUserAfterTime
        <EnumMember> RealBegin
        <EnumMember> RealFinish
    End Enum

    <DataContract()>
    Public Enum roLayerTypes
        <EnumMember> rolUNDEFINED = 0
        <EnumMember> roLTWorking = 1000
        <EnumMember> roLTMandatory = 1100
        <EnumMember> roLTBreak = 1200
        <EnumMember> roLTPaidTime = 1300
        <EnumMember> roLTUnitFilter = 1400
        <EnumMember> roLTGroupFilter = 1500
        <EnumMember> roLTWorkingMaxMinFilter = 1600
        <EnumMember> roLTDailyTotalsFilter = 1700
    End Enum

    <DataContract()>
    Public Enum ShiftType
        <EnumMember> Normal
        <EnumMember> Vacations
        <EnumMember> NormalFloating
    End Enum

    <DataContract()>
    Public Enum HolidayValueType
        <EnumMember> ExpectedWorkingHours_Value
        <EnumMember> Direct_Value
    End Enum

    <DataContract()>
    Public Enum ApplyScheduleValidationRule
        <EnumMember()> Disabled_ScheduleValidationRule
        <EnumMember()> Cumple_ScheduleValidationRule
        <EnumMember()> NoCumple_ScheduleValidationRule
    End Enum

    <DataContract()>
    Public Class roShiftDailyRule
        Protected _ID As Integer
        Protected _IDShift As Integer
        Protected _Name As String
        Protected _Description As String
        Protected _lstConditions As List(Of roShiftDailyRuleCondition)
        Protected _lstActions As List(Of roShiftDailyRuleAction)
        Protected _Priority As Integer
        Protected _DayValidationRule As DayValidationRule
        Protected _lstPreviousShiftValidationRule As List(Of Integer)
        Protected _ApplyScheduleValidationRule As ApplyScheduleValidationRule
        Protected _lstScheduleRulesValidationRule As List(Of Integer)

        <DataMember()>
        Public Property ID As Integer
            Get
                Return _ID
            End Get
            Set(value As Integer)
                _ID = value
            End Set
        End Property

        <DataMember()>
        Public Property IDShift As Integer
            Get
                Return _IDShift
            End Get
            Set(value As Integer)
                _IDShift = value
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
        Public Property Description As String
            Get
                Return _Description
            End Get
            Set(value As String)
                _Description = value
            End Set
        End Property

        <DataMember()>
        Public Property DayValidationRule As DayValidationRule
            Get
                Return _DayValidationRule
            End Get
            Set(value As DayValidationRule)
                _DayValidationRule = value
            End Set
        End Property


        <DataMember()>
        Public Property PreviousShiftValidationRule As Generic.List(Of Integer)
            Get
                Return _lstPreviousShiftValidationRule
            End Get
            Set(value As Generic.List(Of Integer))
                _lstPreviousShiftValidationRule = value
            End Set
        End Property

        <DataMember()>
        Public Property ScheduleRulesValidationRule As Generic.List(Of Integer)
            Get
                Return _lstScheduleRulesValidationRule
            End Get
            Set(value As Generic.List(Of Integer))
                _lstScheduleRulesValidationRule = value
            End Set
        End Property

        <DataMember()>
        Public Property Conditions As Generic.List(Of roShiftDailyRuleCondition)
            Get
                Return _lstConditions
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleCondition))
                _lstConditions = value
            End Set
        End Property

        <DataMember()>
        Public Property Actions As Generic.List(Of roShiftDailyRuleAction)
            Get
                Return _lstActions
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleAction))
                _lstActions = value
            End Set
        End Property

        <DataMember()>
        Public Property Priority As Integer
            Get
                Return _Priority
            End Get
            Set(value As Integer)
                _Priority = value
            End Set
        End Property

        <DataMember()>
        Public Property ApplyScheduleValidationRule As ApplyScheduleValidationRule
            Get
                Return _ApplyScheduleValidationRule
            End Get
            Set(value As ApplyScheduleValidationRule)
                _ApplyScheduleValidationRule = value
            End Set
        End Property



        Public Sub New()
            _ID = 0
            _IDShift = 0
            _Name = ""
            _Description = ""
            _lstConditions = New Generic.List(Of roShiftDailyRuleCondition)
            _lstActions = New Generic.List(Of roShiftDailyRuleAction)
            _Priority = 0
            _DayValidationRule = DayValidationRule.Anyday_DayValidationRule
            _lstPreviousShiftValidationRule = New Generic.List(Of Integer)
            _ApplyScheduleValidationRule = ApplyScheduleValidationRule.Disabled_ScheduleValidationRule
            _lstScheduleRulesValidationRule = New Generic.List(Of Integer)
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftDailyRuleCondition
        Protected _lstConditionCauses As List(Of roShiftDailyRuleConditionCause)
        Protected _lstConditionTimeZones As List(Of roShiftDailyRuleConditionTimeZone)
        Protected _Compare As DailyConditionCompareType
        Protected _Type As DailyConditionValueType
        Protected _FromValue As String
        Protected _ToValue As String
        Protected _UserField As String
        Protected _lstCompareCauses As List(Of roShiftDailyRuleConditionCause)
        Protected _lstCompareTimeZones As List(Of roShiftDailyRuleConditionTimeZone)

        <DataMember()>
        Public Property ConditionCauses As Generic.List(Of roShiftDailyRuleConditionCause)
            Get
                Return _lstConditionCauses
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleConditionCause))
                _lstConditionCauses = value
            End Set
        End Property

        <DataMember()>
        Public Property ConditionTimeZones As Generic.List(Of roShiftDailyRuleConditionTimeZone)
            Get
                Return _lstConditionTimeZones
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleConditionTimeZone))
                _lstConditionTimeZones = value
            End Set
        End Property

        <DataMember()>
        Public Property Compare As DailyConditionCompareType
            Get
                Return _Compare
            End Get
            Set(value As DailyConditionCompareType)
                _Compare = value
            End Set
        End Property

        <DataMember()>
        Public Property Type As DailyConditionValueType
            Get
                Return _Type
            End Get
            Set(value As DailyConditionValueType)
                _Type = value
            End Set
        End Property

        <DataMember()>
        Public Property FromValue As String
            Get
                Return _FromValue
            End Get
            Set(value As String)
                _FromValue = value
            End Set
        End Property

        <DataMember()>
        Public Property ToValue As String
            Get
                Return _ToValue
            End Get
            Set(value As String)
                _ToValue = value
            End Set
        End Property

        <DataMember()>
        Public Property UserField As String
            Get
                Return _UserField
            End Get
            Set(value As String)
                _UserField = value
            End Set
        End Property

        <DataMember()>
        Public Property CompareCauses As Generic.List(Of roShiftDailyRuleConditionCause)
            Get
                Return _lstCompareCauses
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleConditionCause))
                _lstCompareCauses = value
            End Set
        End Property

        <DataMember()>
        Public Property CompareTimeZones As Generic.List(Of roShiftDailyRuleConditionTimeZone)
            Get
                Return _lstCompareTimeZones
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleConditionTimeZone))
                _lstCompareTimeZones = value
            End Set
        End Property

        Public Sub New()
            _lstConditionCauses = New List(Of roShiftDailyRuleConditionCause)
            _lstConditionTimeZones = New List(Of roShiftDailyRuleConditionTimeZone)
            _Compare = DailyConditionCompareType.Equal
            _Type = DailyConditionValueType.DirectValue
            _FromValue = "00:00"
            _ToValue = "00:00"
            _UserField = ""
            _lstCompareCauses = New List(Of roShiftDailyRuleConditionCause)
            _lstCompareTimeZones = New List(Of roShiftDailyRuleConditionTimeZone)
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftDailyRuleActionCause
        Protected _IDCause As Integer
        Protected _IDCause2 As Integer
        Protected _NameCause As String
        Protected _NameCause2 As String

        <DataMember()>
        Public Property IDCause As Integer
            Get
                Return _IDCause
            End Get
            Set(value As Integer)
                _IDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCause2 As Integer
            Get
                Return _IDCause2
            End Get
            Set(value As Integer)
                _IDCause2 = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _NameCause
            End Get
            Set(value As String)
                _NameCause = value
            End Set
        End Property
        <DataMember()>
        Public Property Name2 As String
            Get
                Return _NameCause2
            End Get
            Set(value As String)
                _NameCause2 = value
            End Set
        End Property

        Public Sub New()
            _IDCause = 0
            _IDCause2 = 0
            _NameCause = ""
            _NameCause2 = ""
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftDailyRuleConditionCause
        Protected _IDCause As Integer
        Protected _Operation As OperatorCondition
        Protected _NameCause As String

        <DataMember()>
        Public Property IDCause As Integer
            Get
                Return _IDCause
            End Get
            Set(value As Integer)
                _IDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property Operation As OperatorCondition
            Get
                Return _Operation
            End Get
            Set(value As OperatorCondition)
                _Operation = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _NameCause
            End Get
            Set(value As String)
                _NameCause = value
            End Set
        End Property

        Public Sub New()
            _IDCause = 0
            _Operation = OperatorCondition.Positive
            _NameCause = ""
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftDailyRuleConditionTimeZone

        Protected _IDTimeZone As Integer
        Protected _NameCause As String

        <DataMember()>
        Public Property IDTimeZone As Integer
            Get
                Return _IDTimeZone
            End Get
            Set(value As Integer)
                _IDTimeZone = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _NameCause
            End Get
            Set(value As String)
                _NameCause = value
            End Set
        End Property

        Public Sub New()
            _IDTimeZone = -1
            _NameCause = ""
        End Sub

    End Class

    <DataContract()>
    Public Class roShiftDailyRuleAction
        Protected _Action As RuleAction
        Protected _CarryOverAction As DailyConditionValueType
        Protected _CarryOverDirectValue As String
        Protected _CarryOverUserFieldValue As String
        Protected _CarryOverConditionPart As RulePart
        Protected _CarryOverConditionNumber As RuleCondition
        Protected _CarryOverActionResult As DailyConditionValueType
        Protected _CarryOverDirectValueResult As String
        Protected _CarryOverUserFieldValueResult As String
        Protected _CarryOverConditionPartResult As RulePart
        Protected _CarryOverConditionNumberResult As RuleCondition
        Protected _CarryOverIDCauseFrom As Integer
        Protected _CarryOverIDCauseTo As Integer

        Protected _PlusIDCause As Integer
        Protected _PlusAction As DailyConditionValueType
        Protected _PlusDirectValue As String
        Protected _PlusUserFieldValue As String
        Protected _PlusConditionPart As RulePart
        Protected _PlusConditionNumber As RuleCondition
        Protected _PlusActionResult As DailyConditionValueType
        Protected _PlusDirectValueResult As String
        Protected _PlusUserFieldValueResult As String
        Protected _PlusConditionPartResult As RulePart
        Protected _PlusConditionNumberResult As RuleCondition
        Protected _PlusActionSign As OperatorCondition

        Protected _lstActionCauses As List(Of roShiftDailyRuleActionCause)
        Protected _CarryOverSingleCause As String

        <DataMember()>
        Public Property Action As RuleAction
            Get
                Return _Action
            End Get
            Set(value As RuleAction)
                _Action = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverAction As DailyConditionValueType
            Get
                Return _CarryOverAction
            End Get
            Set(value As DailyConditionValueType)
                _CarryOverAction = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverDirectValue As String
            Get
                Return _CarryOverDirectValue
            End Get
            Set(value As String)
                _CarryOverDirectValue = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverUserFieldValue As String
            Get
                Return _CarryOverUserFieldValue
            End Get
            Set(value As String)
                _CarryOverUserFieldValue = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverConditionPart As RulePart
            Get
                Return _CarryOverConditionPart
            End Get
            Set(value As RulePart)
                _CarryOverConditionPart = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverConditionNumber As RuleCondition
            Get
                Return _CarryOverConditionNumber
            End Get
            Set(value As RuleCondition)
                _CarryOverConditionNumber = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverActionResult As DailyConditionValueType
            Get
                Return _CarryOverActionResult
            End Get
            Set(value As DailyConditionValueType)
                _CarryOverActionResult = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverDirectValueResult As String
            Get
                Return _CarryOverDirectValueResult
            End Get
            Set(value As String)
                _CarryOverDirectValueResult = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverUserFieldValueResult As String
            Get
                Return _CarryOverUserFieldValueResult
            End Get
            Set(value As String)
                _CarryOverUserFieldValueResult = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverConditionPartResult As RulePart
            Get
                Return _CarryOverConditionPartResult
            End Get
            Set(value As RulePart)
                _CarryOverConditionPartResult = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverConditionNumberResult As RuleCondition

            Get
                Return _CarryOverConditionNumberResult
            End Get
            Set(value As RuleCondition)
                _CarryOverConditionNumberResult = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverIDCauseFrom As Integer

            Get
                Return _CarryOverIDCauseFrom
            End Get
            Set(value As Integer)
                _CarryOverIDCauseFrom = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverIDCauseTo As Integer

            Get
                Return _CarryOverIDCauseTo
            End Get
            Set(value As Integer)
                _CarryOverIDCauseTo = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusIDCause As Integer

            Get
                Return _PlusIDCause
            End Get
            Set(value As Integer)
                _PlusIDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusAction As DailyConditionValueType
            Get
                Return _PlusAction
            End Get
            Set(value As DailyConditionValueType)
                _PlusAction = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusDirectValue As String
            Get
                Return _PlusDirectValue
            End Get
            Set(value As String)
                _PlusDirectValue = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusUserFieldValue As String
            Get
                Return _PlusUserFieldValue
            End Get
            Set(value As String)
                _PlusUserFieldValue = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusConditionPart As RulePart
            Get
                Return _PlusConditionPart
            End Get
            Set(value As RulePart)
                _PlusConditionPart = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusConditionNumber As RuleCondition
            Get
                Return _PlusConditionNumber
            End Get
            Set(value As RuleCondition)
                _PlusConditionNumber = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusActionResult As DailyConditionValueType
            Get
                Return _PlusActionResult
            End Get
            Set(value As DailyConditionValueType)
                _PlusActionResult = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusDirectValueResult As String
            Get
                Return _PlusDirectValueResult
            End Get
            Set(value As String)
                _PlusDirectValueResult = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusUserFieldValueResult As String
            Get
                Return _PlusUserFieldValueResult
            End Get
            Set(value As String)
                _PlusUserFieldValueResult = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusConditionPartResult As RulePart
            Get
                Return _PlusConditionPartResult
            End Get
            Set(value As RulePart)
                _PlusConditionPartResult = value
            End Set
        End Property

        <DataMember()>
        Public Property PlusConditionNumberResult As RuleCondition

            Get
                Return _PlusConditionNumberResult
            End Get
            Set(value As RuleCondition)
                _PlusConditionNumberResult = value
            End Set
        End Property
        <DataMember()>
        Public Property PlusActionSign As OperatorCondition
            Get
                Return _PlusActionSign
            End Get
            Set(value As OperatorCondition)
                _PlusActionSign = value
            End Set
        End Property

        <DataMember()>
        Public Property CarryOverSingleCause As String
            Get
                Return _CarryOverSingleCause
            End Get
            Set(value As String)
                _CarryOverSingleCause = value
            End Set
        End Property

        <DataMember()>
        Public Property ActionCauses As Generic.List(Of roShiftDailyRuleActionCause)
            Get
                Return _lstActionCauses
            End Get
            Set(value As Generic.List(Of roShiftDailyRuleActionCause))
                _lstActionCauses = value
            End Set
        End Property

        Public Sub New()

            _Action = RuleAction.CarryOver
            _CarryOverAction = DailyConditionValueType.DirectValue
            _CarryOverDirectValue = "00:00"
            _CarryOverUserFieldValue = ""
            _CarryOverConditionPart = RulePart.Part1
            _CarryOverConditionNumber = RuleCondition.Condition1
            _CarryOverActionResult = DailyConditionValueType.DirectValue
            _CarryOverDirectValueResult = "00:00"
            _CarryOverUserFieldValueResult = ""
            _CarryOverConditionPartResult = RulePart.Part1
            _CarryOverConditionNumberResult = RuleCondition.Condition1
            _CarryOverIDCauseFrom = 0
            _CarryOverIDCauseTo = 0

            _PlusIDCause = 0
            _PlusAction = DailyConditionValueType.DirectValue
            _PlusDirectValue = "00:00"
            _PlusUserFieldValue = ""
            _PlusConditionPart = RulePart.Part1
            _PlusConditionNumber = RuleCondition.Condition1
            _PlusActionResult = DailyConditionValueType.DirectValue
            _PlusDirectValueResult = "00:00"
            _PlusUserFieldValueResult = ""
            _PlusConditionPartResult = RulePart.Part1
            _PlusConditionNumberResult = RuleCondition.Condition1
            _PlusActionSign = OperatorCondition.Positive

            _CarryOverSingleCause = 0
            _lstActionCauses = New List(Of roShiftDailyRuleActionCause)

        End Sub

    End Class

End Namespace