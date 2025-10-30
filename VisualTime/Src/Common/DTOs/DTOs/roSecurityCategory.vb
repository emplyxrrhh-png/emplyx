Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum CategoryType
        <EnumMember> Prevention
        <EnumMember> Labor
        <EnumMember> Legal
        <EnumMember> Security
        <EnumMember> Quality
        <EnumMember> Planning
        <EnumMember> AttendanceControl
    End Enum

    <DataContract()>
    Public Class roSecurityCategory
        <DataMember()>
        Public Property ID As CategoryType

        <DataMember()>
        Public Property Description As String

        <DataMember()>
        Public Property CategoryLevel As Integer

    End Class

End Namespace