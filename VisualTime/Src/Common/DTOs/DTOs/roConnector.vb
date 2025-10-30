Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum TasksType
        <EnumMember> MOVES
        <EnumMember> EMPLOYEEJOBTIME
        <EnumMember> ENTRIES
        <EnumMember> TEAMJOBTIME
        <EnumMember> MACHINEJOBMOVES
        <EnumMember> DAILYCAUSES
        <EnumMember> DAILYSCHEDULE
        <EnumMember> SHIFTS
        <EnumMember> BROADCASTER
        <EnumMember> PROGRAMMEDABSENCES
        <EnumMember> TERMINALSIRENS
        <EnumMember> CONCEPTS
        <EnumMember> CAUSES
        <EnumMember> EMERGENCYREPORTS
        <EnumMember> MANUALREPORTS
        <EnumMember> HRSCHEDULER
        <EnumMember> TASKS
        <EnumMember> BROADCASTER_ONLINE
        <EnumMember> INCIDENCES
        <EnumMember> DAILYINCIDENCES
        <EnumMember> LABAGREES
        ' ...
    End Enum

    <DataContract>
    Public Enum TaskStatus
        <EnumMember> FINISHED
        <EnumMember> INPROCESS
    End Enum

    <DataContract>
    Public Enum CacheAction
        <EnumMember> InsertOrUpdate
        <EnumMember> Delete
    End Enum

End Namespace