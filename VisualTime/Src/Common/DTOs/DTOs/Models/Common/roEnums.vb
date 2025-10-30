Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs.Enums

    <DataContract>
    <Serializable>
    Public Enum ResultAction
        <EnumMember> NotUsedAction      ' Sin uso
        <EnumMember> RequiredAction     ' Obligatorio
        <EnumMember> OptionalAction     ' Opcional
    End Enum

    Public Enum eViewOptionType

        <EnumMember> None = 0
        <EnumMember> InputText = 1
        <EnumMember> InputDate = 2
        <EnumMember> InputNumber = 3
        <EnumMember> RadioButton = 4
        <EnumMember> CheckBox = 5
        <EnumMember> SelectOption = 6
        <EnumMember> TextArea = 7
        <EnumMember> ColorBox = 8
        <EnumMember> DataGrid = 9
        <EnumMember> Button = 10
        <EnumMember> TagBox = 11
        <EnumMember> DateBox = 12
        <EnumMember> InputHoursAndMinutes = 13
        <EnumMember> FileManager = 14

    End Enum

    Public Enum eWeekDay
        <EnumMember> Monday = 1
        <EnumMember> Tuesday = 2
        <EnumMember> Wednesday = 3
        <EnumMember> Thursday = 4
        <EnumMember> Friday = 5
        <EnumMember> Saturday = 6
        <EnumMember> Sunday = 7
    End Enum

    <DataContract()>
    Public Enum TimeScope
        <EnumMember()> PerDay
        <EnumMember()> PerWeek
        <EnumMember()> PerMonth
        <EnumMember()> PerYear
        <EnumMember()> PerSelection
        <EnumMember()> PerAlways
        <EnumMember()> PerRequest
        <EnumMember()> PerWeekDay
    End Enum

    <DataContract>
    Public Enum DayType
        <EnumMember> LABORABLE
        <EnumMember> NONLABORABLE
        <EnumMember> FESTIVE
    End Enum

    <DataContract>
    Public Enum UserContractType
        <EnumMember> FIX
        <EnumMember> [PARTIAL]
        <EnumMember> DISCONTINUOUS
    End Enum

    <DataContract()>
    Public Enum ShiftSequenceType
        <EnumMember()> Unwanted 'Cuando se planifica el último horario de la secuencia, los días anteriores NO pueden planificarse con los horarios de la secuencia
        <EnumMember()> Wanted 'Cuando se planifica el último horario de la secuencia, los días anteriores deben necesariamente planificarse con los horarios de la secuencia
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
    Public Enum ScheduleRuleBaseType
        <EnumMember()> System
        <EnumMember()> User
        <EnumMember()> Custom
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

End Namespace