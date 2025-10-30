Imports System.Runtime.Serialization

Namespace Robotics.ExternalSystems.DataLink.RoboticsExternAccess

    Public Enum CalendarAsciiColumns
        <EnumMember> NIF
        <EnumMember> NIF_Letter
        <EnumMember> ShiftKey
        <EnumMember> PlanDate
        <EnumMember> ImportPrimaryKey

        <EnumMember> Layer1StartTime
        <EnumMember> Layer1EndTime
        <EnumMember> Layer1OrdinaryHours
        <EnumMember> Layer1ComplementaryHours

        <EnumMember> Layer2StartTime
        <EnumMember> Layer2EndTime
        <EnumMember> Layer2OrdinaryHours
        <EnumMember> Layer2ComplementaryHours
        <EnumMember> CanTelecommute
        <EnumMember> TelecommutingStatus
        <EnumMember> TelecommuteForced
    End Enum

    Public Interface IDatalinkCalendar

        Function GetEmployeeColumnsDefinition(ByRef ColumnsVal As String(), ByRef ColumnsPos As Integer()) As Boolean

    End Interface

End Namespace