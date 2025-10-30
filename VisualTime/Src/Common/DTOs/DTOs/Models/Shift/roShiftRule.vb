Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum ShiftRuleType
        <EnumMember> Bonus = 0
        <EnumMember> Adv = 1
        <EnumMember> Simple = 2
        <EnumMember> Daily = 3
    End Enum

    <DataContract()>
    Public Enum ShiftRuleLimit
        <EnumMember> Anytime = 0
        <EnumMember> More = 1
        <EnumMember> Less = 2
        <EnumMember> Between = 3
    End Enum

    <DataContract()>
    Public Enum eShiftRuleValueType
        <EnumMember> DirectValue
        <EnumMember> UserFieldValue
    End Enum

    <DataContract>
    <Serializable>
    Public Class roShiftEngineRule

#Region "Properties"

        <DataMember()>
        Public Property IDShift() As Integer
        <DataMember()>
        Public Property ID() As Integer
        <DataMember()>
        Public Property Type() As ShiftRuleType
        <DataMember()>
        Public Property IDIncidence() As Integer
        <DataMember()>
        Public Property IDZone() As Integer
        <DataMember()>
        Public Property ConditionValueType() As eShiftRuleValueType
        <DataMember()>
        Public Property FromTime() As Nullable(Of DateTime)
        <DataMember()>
        Public Property ToTime() As Nullable(Of DateTime)
        <DataMember()>
        Public Property FromValueUserFieldName() As String
        <DataMember()>
        Public Property ToValueUserFieldName() As String
        <DataMember()>
        Public Property BetweenValueUserFieldName() As String
        <DataMember()>
        Public Property IDCause() As Integer
        <DataMember()>
        Public Property ActionValueType() As eShiftRuleValueType
        <DataMember()>
        Public Property MaxTime() As Nullable(Of DateTime)
        <DataMember()>
        Public Property MaxValueUserFieldName() As String
        <DataMember()>
        Public Property Aux() As String

#End Region

    End Class

End Namespace