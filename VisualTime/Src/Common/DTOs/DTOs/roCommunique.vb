Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum CommuniqueStatusEnum
        <EnumMember()> Draft
        <EnumMember()> Online
        <EnumMember()> Expired
        <EnumMember()> Cancelled
    End Enum

    <DataContract>
    Public Class roCommunique

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Subject As String

        <DataMember>
        Public Property MandatoryRead As Boolean

        <DataMember>
        Public Property IdCompany As Integer

        <DataMember>
        Public Property CreatedBy As roPassportWithPhoto

        <DataMember>
        Public Property CreatedOn As DateTime

        <DataMember>
        Public Property SentOn As DateTime

        <DataMember>
        Public Property Message As String

        <DataMember>
        Public Property AllowedResponses As String()

        <DataMember>
        Public Property AllowChangeResponse As Boolean

        <DataMember>
        Public Property ResponsePercentageLimit As Integer

        <DataMember>
        Public Property ExpirationDate As DateTime

        <DataMember>
        Public Property PlanificationDate As DateTime

        <DataMember>
        Public Property Employees As Integer()

        <DataMember>
        Public Property Groups As Integer()

        <DataMember>
        Public Property Documents As roDocument()

        <DataMember>
        Public Property Archived As Boolean

        <DataMember>
        Public Property Status As CommuniqueStatusEnum

        Public Sub New()
            Id = -1
            CreatedOn = DateTime.Now
            AllowedResponses = {}
            SentOn = DateSerial(1970, 1, 1)
            ExpirationDate = DateSerial(2079, 1, 1)
            PlanificationDate = DateSerial(2000, 1, 1)
        End Sub

    End Class

    <DataContract>
    Public Class roCommuniqueWithStatistics

        <DataMember>
        Public Property Communique As roCommunique

        <DataMember>
        Public Property EmployeeCommuniqueStatus As roCommuniqueStatusForEmployee()

    End Class

    <DataContract>
    Public Class roCommuniqueStatusForEmployee
        <DataMember>
        Public Property IdEmployee As Integer

        <DataMember>
        Public Property EmployeeName As String

        <DataMember>
        Public Property Read As Boolean

        <DataMember>
        Public Property ReadTimeStamp As DateTime

        <DataMember>
        Public Property AnswerRequired As Boolean

        <DataMember>
        Public Property Answered As Boolean

        <DataMember>
        Public Property Answer As String

        <DataMember>
        Public Property AnswerTimeStamp As DateTime?

        Public Sub New()
            AnswerTimeStamp = DateSerial(1970, 1, 1)
            ReadTimeStamp = DateSerial(1970, 1, 1)
        End Sub

    End Class

    <DataContract>
    Public Class roCommuniqueResponse

        Public Sub New()
            Communique = New roCommunique
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Communique As roCommunique

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roCommuniqueStandarResponse

        Public Sub New()
            Result = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roCommuniqueListResponse

        Public Sub New()
            Communiques = {}
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Communiques As roCommunique()

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roCommuniqueStatusResponse

        Public Sub New()
            Status = New roCommuniqueWithStatistics
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Status As roCommuniqueWithStatistics

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roEmployeeCommuniquesStatusResponse

        Public Sub New()
            Status = {}
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Status As roCommuniqueWithStatistics()

        <DataMember>
        Public Property oState As roWsState
    End Class

End Namespace