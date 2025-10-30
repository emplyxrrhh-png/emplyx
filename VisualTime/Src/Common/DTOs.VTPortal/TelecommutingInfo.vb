Imports System.Runtime.Serialization

Namespace DTOs

    <DataContract()>
    Public Class TelecommutingInfo

        Public Sub New()
            TCToday = ""
            TCTomorrow = ""
            WorkcenterStatus = ""
            NoWorkToday = ""
            NoWorkTomorrow = ""
            Status = 0
            CurrentAgreementPercentageUsed = -1
            MaxPercentage = -1
            MaxDays = -1
            CurrentAgreementDaysUsed = -1
            Period = -1
            ByPercentage = False
            PeriodStart = Date.Now
            PeriodEnd = Date.Now
            TelecommutePlannedHours = 0
            TotalWorkingPlannedHours = 0
        End Sub

        <DataMember()>
        Public Property TCToday As String
        <DataMember()>
        Public Property TCTomorrow As String
        <DataMember()>
        Public Property NoWorkToday As String
        <DataMember()>
        Public Property NoWorkTomorrow As String
        <DataMember()>
        Public Property WorkcenterNameToday As String
        <DataMember()>
        Public Property WorkcenterNameTomorrow As String
        <DataMember()>
        Public Property WorkcenterStatus As String
        <DataMember()>
        Public Property WorkcenterStatusTomorrow As String
        <DataMember()>
        Public Property Status As Long
        <DataMember()>
        Public Property CurrentAgreementPercentageUsed As Integer
        <DataMember()>
        Public Property MaxPercentage As Integer
        <DataMember()>
        Public Property CurrentAgreementDaysUsed As Integer
        <DataMember()>
        Public Property MaxDays As Integer
        <DataMember()>
        Public Property Period As Integer
        <DataMember()>
        Public Property ByPercentage As Boolean
        <DataMember()>
        Public Property PeriodStart As Date
        <DataMember()>
        Public Property PeriodEnd As Date
        <DataMember()>
        Public Property TelecommutePlannedHours As Integer
        <DataMember()>
        Public Property TotalWorkingPlannedHours As Integer

    End Class

End Namespace