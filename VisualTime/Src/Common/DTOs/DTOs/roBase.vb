Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum eKeys
        ' IMPORTANTE: Si se cambia o añade algo aqui hacer los cambios respectivos en el NEW de la clase
        <EnumMember> System
        <EnumMember> Config
        <EnumMember> Language
        <EnumMember> Reports
        <EnumMember> Readings
        <EnumMember> Logs
        <EnumMember> LogLevel
        <EnumMember> MaxLogDays
        <EnumMember> PathRoot

        <EnumMember> DefaultLanguage
        <EnumMember> AutomaticBeginJob
        <EnumMember> AuditDays

        <EnumMember> DatabaseServer
        <EnumMember> Database
        <EnumMember> DatabaseUser
        <EnumMember> DatabasePwd

        <EnumMember> ShowTotalPieces
        <EnumMember> AutomaticPreparation
        <EnumMember> AutomaticFinishJob

        <EnumMember> TimeFormat
        <EnumMember> Running

        <EnumMember> PunchPeriodRTIn
        <EnumMember> PunchPeriodRTOut

        <EnumMember> PathProcesses

        <EnumMember> DataLink

        <EnumMember> Documents

        <EnumMember> DatabaseFailover

        <EnumMember> DatabaseEnabledPolling

        <EnumMember> DatabaseEnabledPooling

        <EnumMember> DatabaseMaxPoolSize
    End Enum

    <DataContract>
    Public Enum Parameters
        <EnumMember> StopComms
        <EnumMember> MovMaxHours
        <EnumMember> FirstDate
        <EnumMember> FuncTerminal
        <EnumMember> MonthPeriod
        <EnumMember> YearPeriod
        <EnumMember> JobCounter
        <EnumMember> NumMonthsAccess
        <EnumMember> LastDateAccess
        <EnumMember> MailAccount
        <EnumMember> MailServer
        <EnumMember> ServerPort
        <EnumMember> HideDisabledFeatures
        <EnumMember> PunchPeriodRTIn
        <EnumMember> PunchPeriodRTOut
        <EnumMember> EmployeePassportsIDParent
        <EnumMember> SessionTimeout
        <EnumMember> MailUser
        <EnumMember> MailPWD
        <EnumMember> MailAuthentication
        <EnumMember> MailDomain
        <EnumMember> UseSSL
        <EnumMember> RequiredForbiddenPunch
        <EnumMember> EmployeeFieldCost
        <EnumMember> EvacuationPointUsrField
        <EnumMember> SendPunchesEvery
        <EnumMember> SendPunchesConnector
        <EnumMember> SendPunchesExportFile
        <EnumMember> SendPunchesSeparator
        <EnumMember> SendPunchesEmployeeIdentifier
        <EnumMember> SendPunchesIP
        <EnumMember> SendPunchesPort
        <EnumMember> CommsOffLine
        <EnumMember> CommsOffLineDate
        <EnumMember> TimeFormat
        <EnumMember> EmergencyReportActive
        <EnumMember> EmergencyReportKey
        <EnumMember> EmailUsrField
        <EnumMember> ConnectorDefaultSource
        <EnumMember> ConnectorSourceName
        <EnumMember> ConnectorReadingsName
        <EnumMember> SendPunchesExportLocation
        <EnumMember> PhotosKeepPeriod
        <EnumMember> CloseDateAlert
        <EnumMember> CloseDateAlertPeriod
        <EnumMember> WeekPeriod
        <EnumMember> DocumentsKeepPeriod
        <EnumMember> InitialNotifierDate
        <EnumMember> ActivatedMinBreakTime
        <EnumMember> MinBreakTimeField
        <EnumMember> MinBreakTimeFieldValue
        <EnumMember> MinBreakTimeIDCause
        <EnumMember> MinBreakTime
        <EnumMember> BiometricDataKeepPeriod
        <EnumMember> BlockUser
        <EnumMember> BlockUserPeriod
        <EnumMember> ExternAccessIPs
        <EnumMember> DisableBiometricData
    End Enum

    Public Class IdNamePair
        Public Property Id As Integer
        Public Property Name As String
    End Class

    Public Enum ItemEditionStatus
        NotEdited = 0
        Edited = 1
        Deleted = 2
        [New] = 3
    End Enum

End Namespace