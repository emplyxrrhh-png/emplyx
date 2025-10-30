Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum BotRuleTypeEnum
        <EnumMember()> CopyEmployeePermissions
        <EnumMember()> CopySupervisorPermissions
        <EnumMember()> CopyEmployeeUserFields
        <EnumMember()> CopyCenterCostRole
    End Enum

    Public Enum BotRuleParameterEnum
        <EnumMember()> DestinationEmployee
        <EnumMember()> DestinationSupervisor
        <EnumMember()> DestinationCostCenter
    End Enum

    <DataContract>
    <Serializable>
    Public Class roBotRule

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property IdBot As Integer

        <DataMember>
        Public Property Type As BotRuleTypeEnum

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property IDTemplate As Integer

        <DataMember>
        Public Property Definition As roBotConfig

        <DataMember>
        Public Property IsActive As Boolean

    End Class

End Namespace