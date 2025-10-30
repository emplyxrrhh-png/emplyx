Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum eEmployeeState
        <EnumMember> EmpNormalState
        <EnumMember> EmpMoveState
        <EnumMember> EmpRemoveState
        <EnumMember> EmpAddState
    End Enum

    <DataContract>
    Public Enum PresenceStatus
        <EnumMember> Inside          ' Presente
        <EnumMember> Outside         ' Ausente
    End Enum

    <DataContract>
    Public Enum LockedDayAction
        <EnumMember> None
        <EnumMember> ReplaceFirst
        <EnumMember> NoReplaceFirst
        <EnumMember> ReplaceAll
        <EnumMember> NoReplaceAll
    End Enum

    <DataContract>
    Public Enum ShiftPermissionAction
        <EnumMember> None
        <EnumMember> ContinueAndStop
        <EnumMember> ContinueAll
        <EnumMember> StopAll
    End Enum

    <DataContract>
    Public Enum EmployeeStatusEnum
        <EnumMember> Current = 1
        <EnumMember> Movility = 2
        <EnumMember> Old = 3
        <EnumMember> Future = 4
    End Enum

    <DataContract>
    Public Enum ActionShiftType
        <EnumMember> PrimaryShift
        <EnumMember> AlterShift
        <EnumMember> AllShift
        <EnumMember> HolidayShift
    End Enum

    <DataContract>
    Public Enum TelecommuteAgreementSource
        <EnumMember> Contract = 0
        <EnumMember> LabAgree = 1
    End Enum

    <DataContract()>
    Public Class roEmployeeLockDateInfo
        <DataMember>
        Public Property EmployeeLockDateType As Boolean

        <DataMember>
        Public Property EmployeeLockDate As DateTime
    End Class

    <DataContract()>
    Public Class roEmployeeCopyPlanResult
        <DataMember>
        Public Property CopyPlanResult As Boolean

        <DataMember>
        Public Property IDEmployeeLocked As Integer

        <DataMember>
        Public Property EmployeeLockDate As DateTime

    End Class

End Namespace