Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roLabAgreeEngineCauseLimitValues

#Region "Properties"

        <DataMember()>
        Public Property IDLabAgree() As Integer

        <DataMember()>
        Public Property IDCauseLimitValue() As Integer

        <DataMember()>
        Public Property BeginDate() As DateTime

        <DataMember()>
        Public Property EndDate() As DateTime

        <DataMember()>
        Public Property CauseLimitValue() As roEngineCauseLimitValue

#End Region

    End Class

    Public Class roEngineCauseLimitValue

#Region "Properties"

        <DataMember()>
        Public Property IDCauseLimitValue() As Integer

        <DataMember()>
        Public Property IDCause() As Integer

        <DataMember()>
        Public Property IDExcessCause() As Integer

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property MaximumAnnualValueType() As LabAgreeValueType

        <DataMember()>
        Public Property MaximumAnnualValue() As Double

        'roUserField
        <DataMember()>
        Public Property MaximumAnnualField() As Object

        <DataMember()>
        Public Property MaximumMonthlyType() As LabAgreeValueType

        <DataMember()>
        Public Property MaximumMonthlyValue() As Double

        'roUserField
        <DataMember()>
        Public Property MaximumMonthlyField() As Object

        <DataMember()>
        Public Property OriginalIDCause() As Integer

#End Region

    End Class

End Namespace