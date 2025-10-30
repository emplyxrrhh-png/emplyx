Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ValueType
        <EnumMember> DirectValue
        <EnumMember> UserField
        <EnumMember> IDCause
    End Enum

    <DataContract>
    Public Enum ConditionCompareType
        <EnumMember> Equal
        <EnumMember> Minor
        <EnumMember> MinorEqual
        <EnumMember> Major
        <EnumMember> MajorEqual
        <EnumMember> Distinct
    End Enum

    <DataContract>
    Public Enum TypeDayPlanned
        <EnumMember> AllDays = 0
        <EnumMember> Laboral = 1
        <EnumMember> NonLaboral = 2
    End Enum

    <DataContract>
    Public Enum CompositionType
        <EnumMember> Cause = 0
        <EnumMember> Shift = 1
        <EnumMember> Absence = 2
    End Enum

    <DataContract>
    <Serializable>
    Public Class roConceptEngine

        Public Sub New()
            Me.ID = -1
            Me.EmployeesPermission = 0
            Me.EmployeesConditions = New Generic.List(Of Object)
            Me.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
            Me.AutomaticAccrualIDCause = 0
            Me.ExpiredIDCause = 0
            Me.AutomaticAccrualCriteria = New roEngineAutomaticAccrualCriteria
            Me.ExpiredHoursCriteria = New roEngineExpiredHoursCriteria
            Me.Composition = New List(Of roEngineConceptComposition)
        End Sub

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer

        <DataMember>
        Public Property Name() As String

        <DataMember>
        Public Property Description() As String

        <DataMember>
        Public Property Color() As Integer

        <DataMember>
        Public Property IDType() As String

        <DataMember>
        Public Property ShortName() As String

        <DataMember>
        Public Property BeginDate() As DateTime

        <DataMember>
        Public Property Value() As Double

        <DataMember>
        Public Property FinishDate() As DateTime

        <DataMember>
        Public Property ViewInEmployees() As Boolean

        <DataMember>
        Public Property ViewInTerminals() As Nullable(Of Boolean)

        <DataMember>
        Public Property ViewInPays() As Nullable(Of Boolean)

        <DataMember>
        Public Property FixedPay() As Nullable(Of Boolean)

        <DataMember>
        Public Property PayValue() As Nullable(Of Double)

        <DataMember>
        Public Property UsedField() As String

        <DataMember>
        Public Property RoundingBy() As Nullable(Of Double)

        <DataMember>
        Public Property Export() As String

        <DataMember>
        Public Property DefaultQuery() As String

        <DataMember>
        Public Property IsExported() As Nullable(Of Boolean)

        <DataMember>
        Public Property IsIntervaled() As Nullable(Of Boolean)

        <DataMember>
        Public Property RoundConceptBy() As Nullable(Of Double)

        <DataMember>
        Public Property AutomaticAccrualType() As eAutomaticAccrualType

        <DataMember>
        Public Property AutomaticAccrualCriteria() As roEngineAutomaticAccrualCriteria

        <DataMember>
        Public Property ExpiredHoursCriteria() As roEngineExpiredHoursCriteria

        <DataMember>
        Public Property RoundConveptType() As eRoundingType
        <DataMember>
        Public Property IsAbsentiism() As Nullable(Of Boolean)

        <DataMember>
        Public Property ApplyExpiredHours() As Nullable(Of Boolean)

        <DataMember>
        Public Property AbsentiismRewarded() As Nullable(Of Boolean)

        <DataMember>
        Public Property IsAccrualWork() As Nullable(Of Boolean)

        <DataMember>
        Public Property CustomType() As Nullable(Of Boolean)

        <DataMember>
        Public Property ApplyOnHolidaysRequest() As Nullable(Of Boolean)

        <DataMember>
        Public Property Composition() As Generic.List(Of roEngineConceptComposition)

        <DataMember>
        Public Property EmployeesPremission() As Integer

        'Hacer cast a VTUserFields.UserFields.roUserFieldCondition
        <DataMember>
        Public Property EmployeesConditions() As IEnumerable(Of Object)

        <DataMember>
        Public Property AutomaticAccrualIDCause() As Integer

        <DataMember>
        Public Property ExpiredIDCause() As Integer

        <DataMember>
        Public Property EmployeesPermission() As Integer

        <DataMember>
        Public Property PositiveValue() As Double

        <DataMember>
        Public Property NegativeValue() As Double

        <DataMember>
        Public Property AutoApproveRequestsDR() As Boolean

        <DataMember>
        Public Property AdvParameter() As String




#End Region

    End Class

    <DataContract>
    Public Class roEngineAutomaticAccrualCriteria

        Public Sub New()
            Me.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
            Me.FactorType = eFactorType.DirectValue
            Me.UserField = Nothing
            Me.TypeAccrualDay = eAccrualDayType.AllDays
            Me.Shifts = New Generic.List(Of Integer)
            Me.Causes = New Generic.List(Of Integer)
            Me.UserFieldName = String.Empty
        End Sub

        <DataMember>
        Public Property UserFieldName() As String

        <DataMember>
        Public Property UserField() As Object

        <DataMember>
        Public Property TypeAccrualDay() As eAccrualDayType

        <DataMember>
        Public Property FactorType() As eFactorType

        <DataMember>
        Public Property FactorValue() As Double

        <DataMember>
        Public Property TotalCauses() As Integer

        <DataMember>
        Public Property TotalShifts() As Integer

        <DataMember>
        Public Property Shifts() As Generic.List(Of Integer)

        <DataMember>
        Public Property Causes() As Generic.List(Of Integer)

        <DataMember>
        Public Property AutomaticAccrualType() As eAutomaticAccrualType
    End Class

    <DataContract>
    Public Class roEngineExpiredHoursCriteria

        Public Sub New()
            Me.ExpiredHoursType = eExpiredHoursType.NotExpired
            Me.LabAgreementsAffected = New List(Of Integer)
        End Sub

        <DataMember>
        Public Property Value() As Double

        <DataMember>
        Public Property ExpiredHoursType() As eExpiredHoursType

        <DataMember>
        Public Property LabAgreementsAffected() As Generic.List(Of Integer)
    End Class


End Namespace