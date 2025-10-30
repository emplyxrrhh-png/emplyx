Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Enum ResultAction
        <EnumMember> NotUsedAction      ' Sin uso
        <EnumMember> RequiredAction     ' Obligatorio
        <EnumMember> OptionalAction     ' Opcional
    End Enum

    <DataContract>
    <Serializable>
    Public Class roStart
        <DataMember>
        Public Property Id As Integer

    End Class

    Public Class roStartResponse
        Public Property result As Object

        Public Property errorText As String

    End Class

    <DataContract()>
    Public Class AlertSummaryDetail
        <DataMember()>
        Public Count As Integer
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ImageSrc As String
    End Class

    <DataContract()>
    Public Class AlertSummary

        Public Sub New()
            Alerts = {}
        End Sub

        <DataMember()>
        Public Alerts As AlertSummaryDetail()

    End Class

    <DataContract()>
    Public Class RequestSummaryDetail
        <DataMember()>
        Public Count As Integer
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ImageSrc As String
    End Class

    <DataContract()>
    Public Class RequestSummary

        Public Sub New()
            Requests = {}
        End Sub

        <DataMember()>
        Public Requests As RequestSummaryDetail()

    End Class

    <DataContract()>
    Public Class TelecommutingSummaryDetail
        <DataMember()>
        Public Count As Integer
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ImageSrc As String
    End Class

    <DataContract()>
    Public Class TelecommutingSummary

        Public Sub New()
            Employees = {}
        End Sub

        <DataMember()>
        Public Employees As TelecommutingSummaryDetail()

    End Class

    <DataContract()>
    Public Class EmployeeSummaryDetail
        <DataMember()>
        Public Count As Integer
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ImageSrc As String
    End Class

    <DataContract()>
    Public Class EmployeeSummary

        Public Sub New()
            Employees = {}
        End Sub

        <DataMember()>
        Public Employees As EmployeeSummaryDetail()

    End Class

    <DataContract()>
    Public Class AbsenceSummaryDetail
        <DataMember()>
        Public Count As Integer
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ImageSrc As String
    End Class

    <DataContract()>
    Public Class AbsenceSummary

        Public Sub New()
            Absences = {}
        End Sub

        <DataMember()>
        Public Absences As AbsenceSummaryDetail()
    End Class

End Namespace