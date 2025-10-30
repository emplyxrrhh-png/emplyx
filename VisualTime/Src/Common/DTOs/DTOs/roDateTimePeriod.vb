Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum TypePeriodEnum
        <EnumMember> PeriodOther = 0
        <EnumMember> PeriodTomorrow = 1
        <EnumMember> PeriodToday = 2
        <EnumMember> PeriodYesterday = 3
        <EnumMember> PeriodCurrentWeek = 4
        <EnumMember> PeriodLastWeek = 5
        <EnumMember> PeriodCurrentMonth = 6
        <EnumMember> PeriodLastMonth = 7
        <EnumMember> PeriodCurrentYear = 8
        <EnumMember> PeriodNextWeek = 9
        <EnumMember> PeriodNextMonth = 10
        <EnumMember> PeriodNMonthsAgoFromDay = 11
    End Enum

    <DataContract>
    Public Class roDateTimePeriod
        <DataMember()>
        Public Property TypePeriod As TypePeriodEnum
        <DataMember()>
        Public Property BeginDateTimePeriod As DateTime
        <DataMember()>
        Public Property EndDateTimePeriod As DateTime
    End Class

End Namespace