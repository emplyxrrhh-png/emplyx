Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Enum eNotificationType
        <EnumMember> Empty = 0
        <EnumMember> Before_Begin_Contract = 1
        <EnumMember> Before_Finish_Contract = 2
        <EnumMember> Before_Begin_Programmed_Absence = 3
        <EnumMember> Begin_Programmed_Absence = 4
        <EnumMember> Before_Finish_Programmed_Absence = 5
        <EnumMember> Finish_Programmed_Absence_and_dont_work_later = 6
        <EnumMember> Cut_Programmed_Absence = 7
        <EnumMember> Punch_with_any_Cause = 8
        <EnumMember> Invalid_Access = 9
        <EnumMember> Error_Messages = 10
        <EnumMember> Terminal_Disconnected = 11
        <EnumMember> Access_Dennied_Framework_Security = 12
        <EnumMember> Employee_Absence_On_Coverage = 13
        <EnumMember> Under_Coverage = 14
        <EnumMember> Employee_Not_Arrived_or_Late = 15
        <EnumMember> End_Period_Employee = 16
        <EnumMember> End_Period_Enterprise = 17
        <EnumMember> Concept_Not_Reached = 18
        <EnumMember> Day_With_Unmatched_Time_Record = 19
        <EnumMember> Day_with_Unreliable_Time_Record = 20
        <EnumMember> Non_Justified_Incident = 21
        <EnumMember> IDCard_Not_Assigned_To_Employee = 22
        <EnumMember> Task_Close_to_Finish = 23
        <EnumMember> Task_Close_to_Start = 24
        <EnumMember> Task_Exceeding_Planned_Time = 25
        <EnumMember> Task_Exceeding_Finished_Date = 26
        <EnumMember> Request_Holidays = 27
        <EnumMember> Request_Shift_Change = 28
        <EnumMember> Task_exceeding_Started_Date = 29
        <EnumMember> Task_With_ALerts = 30
        <EnumMember> Kpi_Limit_OverTaken = 31
        <EnumMember> Absence_Document_Pending = 32
        <EnumMember> Advice_For_Document_Not_Delivered = 33
        <EnumMember> Document_Not_Delivered = 34
        <EnumMember> Advice_For_Day_With_Unmatched_Time_Record = 35
        <EnumMember> Advice_For_temporary_blocked_account = 36
        <EnumMember> Advice_For_permanent_blocked_account = 37
        <EnumMember> Advice_For_validation_code = 38
        <EnumMember> Advice_For_New_password = 39
        <EnumMember> Request_Pending = 40
        <EnumMember> Employee_Present_With_Expired_Documents = 41
        <EnumMember> Punch_WithTimezone_Not_Reliable = 42
        <EnumMember> Mobility_Update = 43
        <EnumMember> Mobility_Execution = 44
        <EnumMember> Visit_Update = 45
        <EnumMember> Advice_For_concept_value_exceeded = 46
        <EnumMember> Advice_For_concept_value_Not_reached = 47
        <EnumMember> Requests_Result = 48
        <EnumMember> Document_delivered = 49
        <EnumMember> Document_Pending = 50
        <EnumMember> Assign_Shift = 51
        <EnumMember> Document_Validation_Action = 52
        <EnumMember> Document_Pending_For_Employee = 53
        <EnumMember> Productive_Unit_Under_Coverage = 54
        <EnumMember> Advice_For_Password_Recover = 55
        <EnumMember> Advice_For_Punch_During_ProgrammedAbsence = 56
        <EnumMember> InvalidPortalConsents = 57
        <EnumMember> InvalidDesktopConsents = 58
        <EnumMember> InvalidTerminalConsents = 59
        <EnumMember> InvalidVisitsConsents = 60
        <EnumMember> LinkImportExecuted = 61
        <EnumMember> LabAgree_Max_Exceeded = 62
        <EnumMember> LabAgree_Min_Reached = 63
        <EnumMember> Tasks_Request_complete = 64
        <EnumMember> Advice_Holiday_During_ProgrammedAbsence = 65
        <EnumMember> Advice_ScheduleRule_Indictment = 66
        <EnumMember> Request_Impersonation = 67
        <EnumMember> Punches_In_LockDate = 68
        <EnumMember> Employee_WithoutMask = 69
        <EnumMember> PendingCauseEmployeeAdvice = 70
        <EnumMember> SchedulerAnalyticExecuted = 71
        <EnumMember> Employee_Without_Exit = 72
        <EnumMember> Employee_WithHigh_Temperature = 73
        <EnumMember> Telecommuting_Change_For_Employee = 75
        <EnumMember> Absence_Canceled_By_User = 76
        <EnumMember> OvertimeBreach = 77
        <EnumMember> HoursAbsenceBreach = 78
        <EnumMember> BreakStart = 79
        <EnumMember> BreakFinish = 80
        <EnumMember> BreakNotTaken = 81
        <EnumMember> TelecommuteAgreement = 82
        <EnumMember> PunchBeforeStart = 83
        <EnumMember> EmployeeNotArrivedBeforeStartLimit = 84
        <EnumMember> GeniusFinished = 85
        <EnumMember> CapacityReached = 86
        <EnumMember> NonSupervisedDepartments = 87
        <EnumMember> Advice_For_Send_Username = 88
        <EnumMember> NewMessage_FromEmployee_InChannel = 89
        <EnumMember> Advice_For_NewConversation = 90
        <EnumMember> Document_rejected = 91
        <EnumMember> ScheduledCommuniquee = 92

    End Enum

    Public Enum eBreachControlType
        <EnumMember> Both = 0
        <EnumMember> OnlyMinimumNotReached = 1
        <EnumMember> OnlyMaximumExceeded = 2
    End Enum

    Public Enum eTypeNotification
        <EnumMember> Type_Mail = 0
        <EnumMember> Type_Portal = 1
        <EnumMember> Type_All = 2
        <EnumMember> Type_Desktop = 3
    End Enum

    <DataContract>
    Public Enum MessageScope
        <EnumMember> Subject
        <EnumMember> Body
    End Enum

    <DataContract()>
    Public Class roNotificationType

        Public Sub New()
            Id = 0
            Name = ""
            System = 0
        End Sub

        ''' <summary>
        ''' Resultado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Id As Integer

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property System As Integer

        <DataMember>
        Public Property IDCategory As CategoryType

    End Class

    <DataContract()>
    Public Class roNotificationLanguage

        Public Sub New()
            NotificationType = eNotificationType.Absence_Document_Pending
            LanguageKey = "ESP"
            Scenarios = {}
        End Sub

        ''' <summary>
        ''' Resultado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NotificationType As eNotificationType

        <DataMember()>
        Public Property LanguageKey As String

        <DataMember()>
        Public Property Scenarios As NotificationLanguageScenario()

    End Class

    <DataContract()>
    Public Class NotificationLanguageScenario

        Public Sub New()
            IDScenario = 0
            Name = String.Empty
            Subject = String.Empty
            Body = String.Empty
            NotificationLanguageKey = String.Empty
            SubjectParameters = {}
            BodyParameters = {}
        End Sub

        <DataMember()>
        Public Property IDScenario As Integer

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property Subject As String

        <DataMember()>
        Public Property SubjectParameters As NotificationLanguageParam()

        <DataMember()>
        Public Property Body As String

        <DataMember()>
        Public Property BodyParameters As NotificationLanguageParam()

        <DataMember()>
        Public Property NotificationLanguageKey As String

    End Class

    <DataContract()>
    Public Class NotificationLanguageParam

        Public Sub New()
            IDParameter = 0
            ParameterLanguageKey = String.Empty
            Name = String.Empty
        End Sub

        <DataMember()>
        Public Property IDParameter As Integer

        <DataMember()>
        Public Property ParameterLanguageKey As String

        <DataMember()>
        Public Property Name As String

    End Class

End Namespace