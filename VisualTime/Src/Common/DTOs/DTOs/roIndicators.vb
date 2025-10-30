Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum IndicatorCompareType
        <EnumMember> MinorEqual
        <EnumMember> MajorEqual
    End Enum

    <DataContract>
    Public Enum IndicatorsType
        <EnumMember> Attendance = 1
    End Enum

    <DataContract>
    Public Enum IndicatorEvolutionGroupType
        <EnumMember> Day = 1
        <EnumMember> Week = 2
        <EnumMember> Month = 3
        <EnumMember> Year = 4
    End Enum

End Namespace