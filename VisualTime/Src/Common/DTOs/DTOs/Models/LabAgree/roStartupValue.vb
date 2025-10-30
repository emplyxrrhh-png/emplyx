Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roEngineStartupValue

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer

        <DataMember()>
        Public Property IDConcept() As Integer

        <DataMember()>
        Public Property CalculatedType() As Integer

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property ScalingFieldValues() As Generic.List(Of roEngineScalingValues)

        <DataMember()>
        Public Property ScalingUserField() As String

        <DataMember()>
        Public Property ScalingCoefficientUserField() As String

        <DataMember()>
        Public Property StartValueType() As LabAgreeValueType

        <DataMember()>
        Public Property StartValue() As Double

        <DataMember()>
        Public Property StartValueBaseType() As LabAgreeValueTypeBase

        <DataMember()>
        Public Property TotalPeriodBaseType() As LabAgreeValueTypeBase

        <DataMember()>
        Public Property AccruedValueType() As LabAgreeValueTypeBase

        <DataMember()>
        Public Property StartValueBase() As Double

        <DataMember()>
        Public Property TotalPeriodBase() As Double

        <DataMember()>
        Public Property AccruedValue() As Double

        'roUserField
        <DataMember()>
        Public Property StartUserField() As Object

        'roUserField
        <DataMember()>
        Public Property StartUserFieldBase() As Object

        'roUserField
        <DataMember()>
        Public Property StartUserFieldTotalPeriodBase() As Object

        'roUserField
        <DataMember()>
        Public Property StartUserFieldAccruedValue() As Object

        <DataMember()>
        Public Property MaximumValueType() As LabAgreeValueType

        <DataMember()>
        Public Property MaximumValue() As Double

        'roUserField
        <DataMember()>
        Public Property MaximumUserField() As Object

        <DataMember()>
        Public Property MinimumValueType() As LabAgreeValueType

        <DataMember()>
        Public Property MinimumValue() As Double

        'roUserField
        <DataMember()>
        Public Property MinimumUserField() As Object

        <DataMember()>
        Public Property ApplyEndCustomPeriod() As Boolean

        'roUserField
        <DataMember()>
        Public Property EndCustomPeriodUserField() As Object

        <DataMember()>
        Public Property OriginalIDConcept() As Integer

        <DataMember()>
        Public Property RoundingType() As Integer

        <DataMember()>
        Public Property NewContractException() As Boolean

        'Generic.List(Of roUserFieldCondiition)
        <DataMember()>
        Public Property NewContractExceptionCriteria() As IEnumerable(Of Object)

        <DataMember()>
        Public Property Expiration() As roEngineStartupValueExpirationRule

        <DataMember()>
        Public Property Enjoyment() As roEngineStartupValueEnjoymentRule



#End Region

    End Class

    <DataContract>
    <Serializable()>
    Public Class roEngineScalingValues

        <DataMember>
        Public Property UserField() As String

        <DataMember>
        Public Property AccumValue() As String
    End Class

    <DataContract()>
    <Serializable()>
    Public Class roEngineStartupValueExpirationRule
        Private intExpiration As Integer = 0
        Private eExpirationUnit As LabAgreeStartupValueExpirationUnit = LabAgreeStartupValueExpirationUnit.Day

        <DataMember()>
        Public Property ExpireAfter() As Integer
            Get
                Return Me.intExpiration
            End Get
            Set(ByVal value As Integer)
                Me.intExpiration = value
            End Set
        End Property

        <DataMember()>
        Public Property Unit() As LabAgreeStartupValueExpirationUnit
            Get
                Return Me.eExpirationUnit
            End Get
            Set(ByVal value As LabAgreeStartupValueExpirationUnit)
                Me.eExpirationUnit = value
            End Set
        End Property

    End Class

    <DataContract()>
    <Serializable()>
    Public Class roEngineStartupValueEnjoymentRule
        Private intStartEnjoyment As Integer = 0
        Private eEnjoymentUnit As LabAgreeStartupValueEnjoymentUnit = LabAgreeStartupValueEnjoymentUnit.Day

        <DataMember()>
        Public Property StartAfter() As Integer
            Get
                Return Me.intStartEnjoyment
            End Get
            Set(ByVal value As Integer)
                Me.intStartEnjoyment = value
            End Set
        End Property

        <DataMember()>
        Public Property Unit() As LabAgreeStartupValueEnjoymentUnit
            Get
                Return Me.eEnjoymentUnit
            End Get
            Set(ByVal value As LabAgreeStartupValueEnjoymentUnit)
                Me.eEnjoymentUnit = value
            End Set
        End Property
    End Class


End Namespace