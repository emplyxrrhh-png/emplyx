Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ZoneTelecommutingType
        <EnumMember> Telecommuting = 1
        <EnumMember> Presence = 0
        <EnumMember> AskUser = 2
    End Enum

End Namespace