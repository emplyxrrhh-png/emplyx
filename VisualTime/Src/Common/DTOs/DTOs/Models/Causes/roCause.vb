Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Enum TypeRequestEnum
        <EnumMember> AtBegin
        <EnumMember> AtEnd
        <EnumMember> EveryDays
        <EnumMember> EveryWeeks
        <EnumMember> EveryMonths
        <EnumMember> EveryFlexible1
        <EnumMember> EveryFlexible2
    End Enum

    <DataContract>
    <Serializable>
    Public Class roCauseEngine
        Private strRoundingType As String

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer

        <DataMember>
        Public Property Name() As String

        <DataMember>
        Public Property RoundingBy() As Decimal

        <DataMember>
        Public Property CostFactor() As Decimal

        <DataMember>
        Public Property MaxTimeToForecast() As Decimal

        <DataMember>
        Public Property RoundingType() As eRoundingType
            Get
                Select Case strRoundingType
                    Case "+"
                        Return eRoundingType.Round_UP
                    Case "-"
                        Return eRoundingType.Round_Down
                    Case "~"
                        Return eRoundingType.Round_Near
                    Case Else
                        Return eRoundingType.Round_UP
                End Select
            End Get
            Set(ByVal value As eRoundingType)
                Select Case value
                    Case eRoundingType.Round_UP
                        strRoundingType = "+"
                    Case eRoundingType.Round_Down
                        strRoundingType = "-"
                    Case eRoundingType.Round_Near
                        strRoundingType = "~"
                End Select
            End Set
        End Property

        <DataMember>
        Public Property StringRoundingType() As String

        <DataMember>
        Public Property AllowInputFromReader() As Boolean

        <DataMember>
        Public Property ReaderInputcode() As Integer

        <DataMember>
        Public Property WorkingType() As Boolean

        <DataMember>
        Public Property Description() As String

        <DataMember>
        Public Property Color() As Integer

        <DataMember>
        Public Property ShortName() As String

        <DataMember>
        Public Property StartsProgrammedAbsence() As Boolean

        <DataMember>
        Public Property MaxProgrammedAbsence() As Integer

        <DataMember>
        Public Property AbsenceMandatoryDays() As Integer

        <DataMember>
        Public Property RoundingByDailyScope() As Boolean

        <DataMember>
        Public Property CauseType() As eCauseType

        <DataMember>
        Public Property PunchCloseProgrammedAbsence() As Boolean

        <DataMember>
        Public Property VisibilityPermissions() As Integer

        <DataMember>
        Public Property InputPermissions() As Integer

        <DataMember>
        Public Property AutomaticEquivalenceType() As eAutomaticEquivalenceType

        <DataMember>
        Public Property AutomaticEquivalenceCriteria() As roEngineAutomaticEquivalenceCriteria

        <DataMember>
        Public Property AutomaticEquivalenceIDCause() As Integer

        <DataMember>
        Public Property ApplyJustifyPeriod() As Boolean

        <DataMember>
        Public Property JustifyPeriodStart() As Nullable(Of Integer)

        <DataMember>
        Public Property JustifyPeriodEnd() As Nullable(Of Integer)

        <DataMember>
        Public Property JustifyPeriodType() As Nullable(Of eJustifyPeriodType)

        <DataMember>
        Public Property ApplyAbsenceOnHolidays() As Boolean

        <DataMember>
        Public Property ApplyWorkDaysOnConcept() As Boolean

        <DataMember>
        Public Property Documents() As Generic.List(Of roEngineCauseDocument)

        <DataMember>
        Public Property Absence_MaxDays() As Nullable(Of Integer)

        <DataMember>
        Public Property Absence_BetweenMax() As Nullable(Of DateTime)

        <DataMember>
        Public Property Absence_BetweenMin() As Nullable(Of DateTime)

        <DataMember>
        Public Property Absence_DurationMax() As Nullable(Of DateTime)

        <DataMember>
        Public Property Absence_DurationMin() As Nullable(Of DateTime)

        <DataMember>
        Public Property TraceDocumentsAbsences() As Boolean

        <DataMember>
        Public Property Export() As String

        <DataMember>
        Public Property BusinessCenter() As String

        <DataMember>
        Public Property ExternalWork() As Boolean

        <DataMember>
        Public Property IDConceptBalance() As Integer

        <DataMember>
        Public Property IsHoliday() As Boolean

        <DataMember>
        Public Property DayType() As Boolean

        <DataMember>
        Public Property CustomType() As Boolean

        <DataMember>
        Public Property RequestAvailability() As String
        <DataMember>
        Public Property IDCategory() As CategoryType

        <DataMember>
        Public Property MinLevelOfAuthority() As Integer

        <DataMember>
        Public Property ApprovedAtLevel() As Integer

#End Region

    End Class

    Public Class roEngineAutomaticEquivalenceCriteria
        <DataMember>
        Public Property UserField() As Object 'VTUserFields.UserFields.roUserField

        <DataMember>
        Public Property FactorValue() As Double

        <DataMember>
        Public Property AutomaticEquivalenceType() As eAutomaticEquivalenceType
    End Class

End Namespace