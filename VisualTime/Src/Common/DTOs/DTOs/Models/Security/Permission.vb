Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Valid permissions.
    ''' </summary>
    <DataContract>
    Public Enum Permission
        <EnumMember> None = 0
        <EnumMember> Read = 3
        <EnumMember> Write = 6
        <EnumMember> Admin = 9
    End Enum

End Namespace