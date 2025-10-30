Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roLabAgreeEngineAccrualRule

#Region "Properties"

        <DataMember()>
        Public Property IDLabAgree() As Integer

        <DataMember()>
        Public Property IDAccrualRule() As Integer

        <DataMember()>
        Public Property BeginDate() As DateTime

        <DataMember()>
        Public Property EndDate() As DateTime

        <DataMember()>
        Public Property LabAgreeRule() As roLabAgreeEngineRule

#End Region

    End Class

    Public Class roLabAgreeEngineRule

#Region "Properties"

        <DataMember()>
        Public Property ID() As Integer

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property Description() As String

        <DataMember()>
        Public Property Definition() As roLabAgreeEngineRuleDefinition

        <DataMember()>
        Public Property Schedule() As roLabAgreeEngineSchedule

#End Region

    End Class

    Public Class roLabAgreeEngineRuleDefinition

#Region "Properties"

        <DataMember()>
        Public Property Action() As LabAgreeRuleDefinitionAction
        <DataMember()>
        Public Property Comparation() As LabAgreeRuleDefinitionComparation
        <DataMember()>
        Public Property DestiAccrual() As Integer

        <DataMember()>
        Public Property MainAccrual() As Integer

        <DataMember()>
        Public Property ValueType() As LabAgreeRuleDefinitionValueType

        <DataMember()>
        Public Property [Value]() As Double

        <DataMember()>
        Public Property ValueUserField() As Object

        <DataMember()>
        Public Property ValueIDConcept() As Integer

        <DataMember()>
        Public Property Dif() As LabAgreeRuleDefinitionDif

        <DataMember()>
        Public Property Until() As Double

        <DataMember()>
        Public Property UntilUserField() As Object

#End Region

    End Class

    Public Class roLabAgreeEngineSchedule

#Region "Properties"

        <DataMember()>
        Public Property ScheduleType() As LabAgreeScheduleScheduleType

        <DataMember()>
        Public Property MonthlyType() As LabAgreeScheduleMonthlyType

        <DataMember()>
        Public Property Days() As Integer

        <DataMember()>
        Public Property Months() As Integer

        <DataMember()>
        Public Property Day() As Integer

        <DataMember()>
        Public Property Month() As Integer

        <DataMember()>
        Public Property Start() As Integer

        <DataMember()>
        Public Property WeekDay() As LabAgreeScheduleWeekDay

        Public Function IsEqual(ByVal oCompareItem As roLabAgreeEngineSchedule) As Boolean

            Dim bolRet As Boolean = False

            If oCompareItem IsNot Nothing Then
                bolRet = (oCompareItem.ToString() = Me.ToString())
            End If

            Return bolRet

        End Function

#End Region

    End Class

End Namespace