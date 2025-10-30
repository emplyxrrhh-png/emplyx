Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roShiftEngine

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property Description() As String
        <DataMember()>
        Public Property Color() As Integer

        <DataMember()>
        Public Property ExpectedWorkingHours() As Single

        <DataMember()>
        Public Property IsObsolete() As Boolean
        <DataMember()>
        Public Property IsTemplate() As Boolean
        <DataMember()>
        Public Property StartLimit() As DateTime
        <DataMember()>
        Public Property EndLimit() As DateTime
        <DataMember()>
        Public Property ManualLimit() As Boolean
        <DataMember()>
        Public Property ShortName() As String
        <DataMember()>
        Public Property TypeShift() As String
        <DataMember()>
        Public Property IDGroup() As Integer
        <DataMember()>
        Public Property IDCenter() As Integer
        <DataMember()>
        Public Property WebVisible() As Boolean
        <DataMember()>
        Public Property ApplyCenterOnAbsence() As Boolean
        <DataMember()>
        Public Property WebLaboral() As Boolean
        <DataMember()>
        Public Property IDConceptBalance() As Integer
        <DataMember()>
        Public Property IDConceptRequestNextYear() As Integer
        <DataMember()>
        Public Property IDCauseHolidays() As Integer
        <DataMember()>
        Public Property AllowComplementary() As Boolean
        <DataMember()>
        Public Property AreWorkingDays() As Boolean
        <DataMember()>
        Public Property StartDate() As Nullable(Of Date)

        <DataMember()>
        Public Property Layers() As Generic.List(Of roShiftEngineLayer)
        <DataMember()>
        Public Property TimeZones() As Generic.List(Of roShiftEngineTimeZone)

        <DataMember()>
        Public Property SimpleRules() As Generic.List(Of roShiftEngineRule)

        <DataMember()>
        Public Property DailyRules() As Generic.List(Of roShiftDailyRule)
        <DataMember()>
        Public Property AdvancedParameters() As String
        <DataMember()>
        Public Property ShiftType() As ShiftType
        <DataMember()>
        Public Property StartFloating() As Nullable(Of DateTime)
        <DataMember()>
        Public Property Assignments() As Generic.List(Of roShiftEngineAssignment)
        <DataMember()>
        Public Property VisibilityPermissions() As Integer
        <DataMember()>
        Public Property BreakHours() As Single
        <DataMember()>
        Public Property AllowFloatingData() As Boolean
        <DataMember()>
        Public Property ExportName() As String
        <DataMember()>
        Public Property TypeHolidayValue() As HolidayValueType

        <DataMember()>
        Public Property HolidayValue() As Double

        <DataMember()>
        Public Property DailyFactor() As Double

        <DataMember()>
        Public Property EnableNotifyExit() As Boolean

        <DataMember()>
        Public Property CompleteExitAt() As Integer

        <DataMember()>
        Public Property EnableCompleteExit() As Boolean

        <DataMember()>
        Public Property NotifyEmployeeExitAt() As Integer

#End Region

    End Class

End Namespace