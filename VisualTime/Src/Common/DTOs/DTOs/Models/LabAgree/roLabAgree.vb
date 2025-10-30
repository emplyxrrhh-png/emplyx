Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roLabAgreeEngine

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property Description() As String

        <DataMember()>
        Public Property Telecommuting() As Boolean

        <DataMember()>
        Public Property TelecommutingMandatoryDays() As String
        <DataMember()>
        Public Property PresenceMandatoryDays() As String

        <DataMember()>
        Public Property TelecommutingOptionalDays() As String

        <DataMember()>
        Public Property TelecommutingMaxDays() As Integer

        <DataMember()>
        Public Property TelecommutingMaxPercentage() As Integer

        <DataMember()>
        Public Property TelecommutingPeriodType() As Integer

        <DataMember()>
        Public Property TelecommutingAgreementStart() As Nullable(Of DateTime)

        <DataMember()>
        Public Property TelecommutingAgreementEnd() As Nullable(Of DateTime)

        <DataMember()>
        Public Property LabAgreeAccrualRules() As Generic.List(Of roLabAgreeEngineAccrualRule)

        <DataMember()>
        Public Property StartupValues() As Generic.List(Of roEngineStartupValue)

        <DataMember()>
        Public Property LabAgreeCauseLimitValues() As Generic.List(Of roLabAgreeEngineCauseLimitValues)

        <DataMember()>
        Public Property LabAgreeScheduleRules() As Generic.List(Of roScheduleRule)

        <DataMember()>
        Public Property ExtraHoursConfiguration() As Integer

        <DataMember()>
        Public Property ExtraHoursIDCauseSimples() As String

        <DataMember()>
        Public Property ExtraHoursIDCauseDoubles() As Integer

        <DataMember()>
        Public Property ExtraHoursIDCauseTriples() As Integer

#End Region

    End Class

End Namespace