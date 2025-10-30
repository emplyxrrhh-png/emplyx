Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roApiSession

        Public Sub New()
            LastRequest = DateTime.Now
            SupervisorPortalEnabled = Nothing
            IdIdentity = -1
            IdSupervisor = -1
            IdTerminal = -1
            CurrentPunch = Nothing

            IsRegistered = False
            LoggedIn = False
            Location = String.Empty

            SessionKey = String.Empty
            TerminalSecurityToken = String.Empty
            CurrentToken = String.Empty
        End Sub

        <DataMember>
        Public Property SessionKey As String

        <DataMember>
        Public Property Location As String

        <DataMember>
        Public Property ApplicationSource As roAppSource

        <DataMember>
        Public Property LoginMethod As Integer

        <DataMember>
        Public Property IdTerminal As Integer

        <DataMember>
        Public Property Terminal As Object

        <DataMember>
        Public Property CurrentPunch As roTerminalPunch

        <DataMember>
        Public Property TerminalResult As TerminalBaseResultEnum

        <DataMember>
        Public Property IsRegistered As Boolean

        <DataMember>
        Public Property TerminalSecurityToken As String

        <DataMember>
        Public Property IdIdentity As Integer

        <DataMember>
        Public Property Identity As roPassportTicket

        <DataMember>
        Public Property IdSupervisor As Integer

        <DataMember>
        Public Property Supervisor As roPassportTicket

        <DataMember>
        Public Property LoggedIn As Boolean

        <DataMember>
        Public Property SecurityResult As SecurityResultEnum

        <DataMember>
        Public Property TimeZone As TimeZoneInfo

        <DataMember>
        Public Property Language As String

        <DataMember>
        Public Property LastRequest As DateTime

        <DataMember>
        Public Property SupervisorPortalEnabled As Nullable(Of Boolean)

        <DataMember>
        Public Property LastInfoLoaded As DateTime

        <DataMember>
        Public Property CompanyId As String

        <DataMember>
        Public Property CurrentToken As String

    End Class

End Namespace