Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum eRoundingType
        <EnumMember> Round_UP = 0
        <EnumMember> Round_Down = 1
        <EnumMember> Round_Near = 2
    End Enum

    <DataContract>
    Public Enum eCauseType
        <EnumMember> ProductiveIn = 0
        <EnumMember> ProductiveOut = 1
        <EnumMember> InProductiveIn = 2
        <EnumMember> InProductiveOut = 3
    End Enum

    <DataContract>
    Public Enum eJustifyPeriodType
        <EnumMember> DontJustify = 0
        <EnumMember> JustifyPeriod = 1
    End Enum

    <DataContract>
    Public Enum eAutomaticEquivalenceType
        <EnumMember> DeactivatedType = 0
        <EnumMember> ExpectedWorkingHoursType = 1
        <EnumMember> FieldType = 2
        <EnumMember> DirectValueType = 3
    End Enum

    <DataContract>
    Public Enum eCauseRequest
        <EnumMember> All = 0
        <EnumMember> ProgrammedAbsence = 1
        <EnumMember> ProgrammedCause = 2
        <EnumMember> ProgrammedOvertime = 3
        <EnumMember> PlannedHolidays = 4
        <EnumMember> ExternalWork = 5
        <EnumMember> Leaves = 6
    End Enum

    '<DataContract>
    'Public Enum eRoundingType
    '    <EnumMember> Round_UP = 0
    '    <EnumMember> Round_Down = 1
    '    <EnumMember> Round_Near = 2
    'End Enum

    <DataContract>
    Public Enum eAutomaticAccrualType
        <EnumMember> DeactivatedType = 0
        <EnumMember> DaysType = 1
        <EnumMember> HoursType = 2
    End Enum

    <DataContract>
    Public Enum eFactorType
        <EnumMember> DirectValue = 0
        <EnumMember> UserField = 1
    End Enum

    <DataContract>
    Public Enum eAccrualDayType
        <EnumMember> AllDays = 0
        <EnumMember> SomeDays = 1
    End Enum

    <DataContract>
    Public Enum eExpiredHoursType
        <EnumMember> NotExpired = 0
        <EnumMember> DaysType = 1
        <EnumMember> MonthType = 2
    End Enum

    <DataContract>
    Public Enum eEnjoymentRuleType
        <EnumMember> NoRule = 0
        <EnumMember> Days = 1
        <EnumMember> Months = 2
    End Enum

End Namespace