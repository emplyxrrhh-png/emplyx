Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum LabAgreeRuleDefinitionAction
        <EnumMember> Move = 0
        <EnumMember> Copy = 1
    End Enum

    Public Enum LabAgreeRuleDefinitionComparation
        <EnumMember> Equal = 0
        <EnumMember> EqualLess = 1
        <EnumMember> EqualMore = 2
        <EnumMember> Less = 3
        <EnumMember> More = 4
    End Enum

    Public Enum LabAgreeRuleDefinitionValueType
        <EnumMember> DirectValue
        <EnumMember> UserFieldValue
        <EnumMember> ConceptValue
    End Enum

    Public Enum LabAgreeRuleDefinitionDif
        <EnumMember> UntilValue = 0
        <EnumMember> All = 1
        <EnumMember> Diff = 2
        <EnumMember> Value = 3
        <EnumMember> UserFieldUntilValue = 4
        <EnumMember> UserFieldValue = 5
    End Enum

    Public Enum LabAgreeScheduleScheduleType
        <EnumMember> Daily
        <EnumMember> Weekly
        <EnumMember> Monthly
        <EnumMember> Annual
    End Enum

    Public Enum LabAgreeScheduleMonthlyType
        <EnumMember> DayAndMonth
        <EnumMember> DayAndStartup
    End Enum

    Public Enum LabAgreeScheduleWeekDay
        <EnumMember> Monday = 1
        <EnumMember> Tuesday = 2
        <EnumMember> Wednesday = 3
        <EnumMember> Thursday = 4
        <EnumMember> Friday = 5
        <EnumMember> Saturday = 6
        <EnumMember> Sunday = 7
    End Enum

    Public Enum LabAgreeValueType
        <EnumMember> None = 0
        <EnumMember> DirectValue = 1
        <EnumMember> UserField = 2
        <EnumMember> CalculatedValue = 3
    End Enum

    Public Enum LabAgreeValueTypeBase
        <EnumMember> DirectValue = 0
        <EnumMember> UserField = 1
    End Enum

    Public Enum LabAgreeExtraHoursConfiguration
        <EnumMember> Disabled = 0
        <EnumMember> ThreeByThree = 1
        <EnumMember> NineAcc = 2
    End Enum

    Public Enum LabAgreeStartupValueExpirationUnit
        <EnumMember> Day = 0
        <EnumMember> Month = 1
    End Enum

    Public Enum LabAgreeStartupValueEnjoymentUnit
        <EnumMember> Day = 0
        <EnumMember> Month = 1
    End Enum

End Namespace