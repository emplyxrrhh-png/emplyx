Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Class roEngineConceptComposition

        <DataMember>
        Public Property ID() As Integer

        <DataMember>
        Public Property IDConcept() As Integer

        <DataMember>
        Public Property IDCause() As Integer

        <DataMember>
        Public Property Conditions() As New Generic.List(Of roEngineConceptCondition)

        <DataMember>
        Public Property FactorType() As ValueType

        <DataMember>
        Public Property FactorValue() As Double

        <DataMember>
        Public Property FactorUserField() As String

        <DataMember>
        Public Property IDShift() As Integer

        <DataMember>
        Public Property IDType() As CompositionType

        <DataMember>
        Public Property TypeDayPlanned() As TypeDayPlanned

        <DataMember>
        Public Property CompositionUserField() As String

        <DataMember>
        Public Property AdvParameter() As String

    End Class

    <DataContract>
    Public Class roEngineConceptCondition
        <DataMember>
        Public Property IDCauses() As New Generic.List(Of roEngineConceptConditionCause)

        <DataMember>
        Public Property Compare() As ConditionCompareType

        <DataMember>
        Public Property Value_Type() As ValueType

        <DataMember>
        Public Property Value_Direct() As DateTime

        <DataMember>
        Public Property Value_IDCause() As Integer

        <DataMember>
        Public Property Value_UserField() As String

    End Class

    <DataContract>
    Public Class roEngineConceptConditionCause

        <DataMember>
        Public Property IDCause As Integer
        <DataMember>
        Public Property Operation As Char

    End Class

End Namespace