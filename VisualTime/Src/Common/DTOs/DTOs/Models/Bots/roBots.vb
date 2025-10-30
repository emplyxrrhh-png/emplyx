Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum BotStatusEnum
        <EnumMember()> Correct
        <EnumMember()> Failed
    End Enum

    <DataContract>
    Public Enum BotTypeEnum
        <EnumMember()> NewEmployee
        <EnumMember()> NewSupervisor
        <EnumMember()> NewCostCenter
    End Enum

    <DataContract>
    <Serializable>
    Public Class roBot

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Type As BotTypeEnum

        <DataMember>
        Public Property Status As BotStatusEnum

        <DataMember>
        Public Property BotRules As Generic.List(Of roBotRule)

        <DataMember>
        Public Property LastExecutionDate As DateTime

    End Class

End Namespace