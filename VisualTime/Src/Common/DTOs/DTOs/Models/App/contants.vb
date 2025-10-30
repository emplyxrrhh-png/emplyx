Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum roAppSource
        <EnumMember> unknown
        <EnumMember> mx9
        <EnumMember> VTPortal
        <EnumMember> VTPortalApp
        <EnumMember> VTPortalWeb
        <EnumMember> VTLive
        <EnumMember> VTLiveApi
        <EnumMember> Visits
        <EnumMember> TerminalsPushServer
        <EnumMember> TimeGate
    End Enum

    <DataContract>
    Public Enum roAppType
        <EnumMember> Unknown

        <EnumMember> VTLive
        <EnumMember> VTLiveApi
        <EnumMember> VTPortal
        <EnumMember> VTVisits
        <EnumMember> TerminalsPushServer

        <EnumMember> roAnalyticsFunctions
        <EnumMember> roBackgroundFunctions
        <EnumMember> roBroadcasterFunction
        <EnumMember> roDatalinkFunctions
        <EnumMember> roEngineFunctions
        <EnumMember> roFTPSyncFunction
        <EnumMember> roMailFunction
        <EnumMember> roNotificationsFunction
        <EnumMember> roPunchConnectorFunctions
        <EnumMember> roPushNotificationsFunction
        <EnumMember> roReportFunctions
        <EnumMember> roSCFunction
        <EnumMember> roScheduleFunctions

    End Enum

    Public Enum GlobalAsaxParameter
        MaxConcurrentSessions
        ServerTimeout
        CurrentIdPassport
        CompanyId
        ClientCompanyId
        SystemPassportID
        AppName
        RequestGUID
        LogFileName
        LogLevel
        TraceLevel
        LoggedInPassportTicket
    End Enum


End Namespace