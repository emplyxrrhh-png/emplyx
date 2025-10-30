Imports Robotics.Base.DTOs

Public Class roNotificationTaskFactory

    Public Function GetNotificationTaskManager(ByVal sGUID As String, ByVal notificationType As eNotificationType) As roNotificationTaskManager
        Dim oManager As roNotificationTaskManager = Nothing

        Select Case notificationType
            Case eNotificationType.Before_Begin_Contract
                oManager = New roNotificationTask_Before_Begin_Contract_Manager(sGUID)
            Case eNotificationType.Before_Finish_Contract
                oManager = New roNotificationTask_Before_Finish_Contract_Manager(sGUID)
            Case eNotificationType.Before_Begin_Programmed_Absence
                oManager = New roNotificationTask_Before_Begin_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Begin_Programmed_Absence
                oManager = New roNotificationTask_Begin_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Before_Finish_Programmed_Absence
                oManager = New roNotificationTask_Before_Finish_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Finish_Programmed_Absence_and_dont_work_later
                oManager = New roNotificationTask_Finish_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Cut_Programmed_Absence
                oManager = New roNotificationTask_Cut_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Punch_with_any_Cause
                oManager = New roNotificationTask_Punch_With_Cause_Manager(sGUID)
            Case eNotificationType.Invalid_Access
                oManager = New roNotificationTask_Invalid_Access_Manager(sGUID)
            Case eNotificationType.Terminal_Disconnected
                oManager = New roNotificationTask_Terminal_Disconnected_Manager(sGUID)
            Case eNotificationType.Employee_Absence_On_Coverage
                oManager = New roNotificationTask_Employee_Absence_OnCoverage_Manager(sGUID)
            Case eNotificationType.Under_Coverage
                oManager = New roNotificationTask_Under_Coverage_Manager(sGUID)
            Case eNotificationType.Employee_Not_Arrived_or_Late
                oManager = New roNotificationTask_Employee_LateArrival_Manager(sGUID)
            Case eNotificationType.End_Period_Employee
                oManager = New roNotificationTask_EndPeriodEmployee_Manager(sGUID)
            Case eNotificationType.End_Period_Enterprise
                oManager = New roNotificationTask_EndPeriodEnterprise_Manager(sGUID)
            Case eNotificationType.Concept_Not_Reached
                oManager = New roNotificationTask_ConceptNotReached_Manager(sGUID)
            Case eNotificationType.Day_With_Unmatched_Time_Record
                oManager = New roNotificationTask_Day_With_Unmatched_Time_Record_Manager(sGUID)
            Case eNotificationType.Day_with_Unreliable_Time_Record
                oManager = New roNotificationTask_Day_with_Unreliable_Time_Record_Manager(sGUID)
            Case eNotificationType.Non_Justified_Incident
                oManager = New roNotificationTask_Non_Justified_Incident_Manager(sGUID)
            Case eNotificationType.IDCard_Not_Assigned_To_Employee
                oManager = New roNotificationTask_IDCard_Not_Assigned_To_Employee_Manager(sGUID)
            Case eNotificationType.Task_Close_to_Finish
                oManager = New roNotificationTask_Task_Close_to_Finish_Manager(sGUID)
            Case eNotificationType.Task_Close_to_Start
                oManager = New roNotificationTask_Task_Close_to_Start_Manager(sGUID)
            Case eNotificationType.Task_Exceeding_Planned_Time
                oManager = New roNotificationTask_Task_Exceeding_Planned_Time_Manager(sGUID)
            Case eNotificationType.Task_Exceeding_Finished_Date
                oManager = New roNotificationTask_Task_Exceeding_Finish_Date_Manager(sGUID)
            Case eNotificationType.Request_Holidays
                oManager = New roNotificationTask_Request_Holidays_Manager(sGUID)
            Case eNotificationType.Request_Shift_Change
                oManager = New roNotificationTask_Request_Shift_Change_Manager(sGUID)
            Case eNotificationType.Task_exceeding_Started_Date
                oManager = New roNotificationTask_Task_exceeding_Started_Date_Manager(sGUID)
            Case eNotificationType.Task_With_ALerts
                oManager = New roNotificationTask_Task_With_ALerts_Manager(sGUID)
            Case eNotificationType.Kpi_Limit_OverTaken
                oManager = New roNotificationTask_Kpi_Limit_OverTaken_Manager(sGUID)
            Case eNotificationType.Absence_Document_Pending
                oManager = New roNotificationTask_Absence_Document_Pending_Manager(sGUID)
            Case eNotificationType.Advice_For_Document_Not_Delivered
                oManager = New roNotificationTask_Advice_For_Document_Not_Delivered_Manager(sGUID)
            Case eNotificationType.Document_Not_Delivered
                oManager = New roNotificationTask_Document_Not_Delivered_Manager(sGUID)
            Case eNotificationType.Advice_For_Day_With_Unmatched_Time_Record
                oManager = New roNotificationTask_Advice_For_Day_With_Unmatched_Time_Record_Manager(sGUID)
            Case eNotificationType.Advice_For_temporary_blocked_account
                oManager = New roNotificationTask_Advice_For_temporary_blocked_account_Manager(sGUID)
            Case eNotificationType.Advice_For_permanent_blocked_account
                oManager = New roNotificationTask_Advice_For_permanent_blocked_account_Manager(sGUID)
            Case eNotificationType.Advice_For_validation_code
                oManager = New roNotificationTask_Advice_For_validation_code_Manager(sGUID)
            Case eNotificationType.Advice_For_New_password
                oManager = New roNotificationTask_Advice_For_New_password_Manager(sGUID)
            Case eNotificationType.Request_Pending
                oManager = New roNotificationTask_Request_Pending_Manager(sGUID)
            Case eNotificationType.Employee_Present_With_Expired_Documents
                oManager = New roNotificationTask_Employee_Present_With_Expired_Documents_Manager(sGUID)
            Case eNotificationType.Punch_WithTimezone_Not_Reliable
                oManager = New roNotificationTask_Punch_WithTimezone_Not_Reliable_Manager(sGUID)
            Case eNotificationType.Mobility_Update
                oManager = New roNotificationTask_Mobility_Update_Manager(sGUID)
            Case eNotificationType.Mobility_Execution
                oManager = New roNotificationTask_Mobility_Execution_Manager(sGUID)
            Case eNotificationType.Visit_Update
                oManager = New roNotificationTask_Visit_Update_Manager(sGUID)
            Case eNotificationType.Advice_For_concept_value_exceeded
                oManager = New roNotificationTask_Advice_For_concept_value_exceeded_Manager(sGUID)
            Case eNotificationType.Advice_For_concept_value_Not_reached
                oManager = New roNotificationTask_Advice_For_concept_value_Not_reached_Manager(sGUID)
            Case eNotificationType.Requests_Result
                oManager = New roNotificationTask_Requests_Result_Manager(sGUID)
            Case eNotificationType.Document_delivered
                oManager = New roNotificationTask_Document_delivered_Manager(sGUID)
            Case eNotificationType.Document_Pending
                oManager = New roNotificationTask_Document_Pending_Manager(sGUID)
            Case eNotificationType.PendingCauseEmployeeAdvice
                oManager = New roPendingCauseEmployeeAdvice(sGUID)
            Case eNotificationType.Assign_Shift
                oManager = New roNotificationTask_Assign_Shift_Manager(sGUID)
            Case eNotificationType.Document_Validation_Action
                oManager = New roNotificationTask_Document_Validation_Action_Manager(sGUID)
            Case eNotificationType.Document_rejected
                oManager = New roNotificationTask_Document_rejected_Manager(sGUID)
            Case eNotificationType.Document_Pending_For_Employee
                oManager = New roNotificationTask_Document_Pending_For_Employee_Manager(sGUID)
            Case eNotificationType.Productive_Unit_Under_Coverage
                oManager = New roNotificationTask_Productive_Unit_Under_Coverage_Manager(sGUID)
            Case eNotificationType.Advice_For_Password_Recover
                oManager = New roNotificationTask_Advice_For_Password_Recover_Manager(sGUID)
            Case eNotificationType.Advice_For_Punch_During_ProgrammedAbsence
                oManager = New roNotificationTask_Advice_For_Punch_During_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.InvalidPortalConsents
                oManager = New roNotificationTask_InvalidPortalConsents_Manager(sGUID)
            Case eNotificationType.InvalidDesktopConsents
                oManager = New roNotificationTask_InvalidDesktopConsents_Manager(sGUID)
            Case eNotificationType.InvalidTerminalConsents
                oManager = New roNotificationTask_InvalidTerminalConsents_Manager(sGUID)
            Case eNotificationType.InvalidVisitsConsents
                oManager = New roNotificationTask_InvalidVisitsConsents_Manager(sGUID)
            Case eNotificationType.LinkImportExecuted
                oManager = New roNotificationTask_LinkImportExecuted_Manager(sGUID)
            Case eNotificationType.LabAgree_Max_Exceeded
                oManager = New roNotificationTask_LabAgree_Max_Exceeded_Manager(sGUID)
            Case eNotificationType.LabAgree_Min_Reached
                oManager = New roNotificationTask_LabAgree_Min_Reached_Manager(sGUID)
            Case eNotificationType.Tasks_Request_complete
                oManager = New roNotificationTask_Tasks_Request_Complete_Manager(sGUID)
            Case eNotificationType.Advice_Holiday_During_ProgrammedAbsence
                oManager = New roNotificationTask_Advice_Holiday_During_ProgrammedAbsence_Manager(sGUID)
            Case eNotificationType.Advice_ScheduleRule_Indictment
                oManager = New roNotificationTask_Advice_ScheduleRule_Indictment_Manager(sGUID)
            Case eNotificationType.Request_Impersonation
                oManager = New roNotificationTask_Request_Impersonation_Manager(sGUID)
            Case eNotificationType.Punches_In_LockDate
                oManager = New roNotificationTask_Punches_In_LockDate_Manager(sGUID)
            Case eNotificationType.Employee_WithoutMask
                oManager = New roNotificationTask_Employee_WithoutMask_Manager(sGUID)
            Case eNotificationType.SchedulerAnalyticExecuted
                oManager = New roNotificationTask_SchedulerAnalyticExecuted_Manager(sGUID)
            Case eNotificationType.Employee_Without_Exit
                oManager = New roNotificationTask_Employee_Without_Exit_Manager(sGUID)
            Case eNotificationType.Employee_WithHigh_Temperature
                oManager = New roNotificationTask_Employee_WithHigh_Temperature_Manager(sGUID)
            Case eNotificationType.Telecommuting_Change_For_Employee
                oManager = New roNotificationTask_Telecommuting_Change_For_Employee_Manager(sGUID)
            Case eNotificationType.Absence_Canceled_By_User
                oManager = New roNotificationTask_Absence_Canceled_By_User_Manager(sGUID)
            Case eNotificationType.OvertimeBreach
                oManager = New roNotificationTask_OvertimeBreach_Manager(sGUID)
            Case eNotificationType.HoursAbsenceBreach
                oManager = New roNotificationTask_HoursAbsenceBreach_Manager(sGUID)
            Case eNotificationType.BreakStart
                oManager = New roNotificationTask_BreakStart_Manager(sGUID)
            Case eNotificationType.BreakFinish
                oManager = New roNotificationTask_BreakFinish_Manager(sGUID)
            Case eNotificationType.BreakNotTaken
                oManager = New roNotificationTask_BreakNotTaken_Manager(sGUID)
            Case eNotificationType.TelecommuteAgreement
                oManager = New roNotificationTask_TelecommuteAgreement_Manager(sGUID)
            Case eNotificationType.PunchBeforeStart
                oManager = New roNotificationTask_PunchBeforeStart_Manager(sGUID)
            Case eNotificationType.EmployeeNotArrivedBeforeStartLimit
                oManager = New roNotificationTask_EmployeeNotArrivedBeforeStartLimit_Manager(sGUID)
            Case eNotificationType.GeniusFinished
                oManager = New roNotificationTask_GeniusFinished_Manager(sGUID)
            Case eNotificationType.CapacityReached
                oManager = New roNotificationTask_CapacityReached_Manager(sGUID)
            Case eNotificationType.Advice_For_Send_Username
                oManager = New roNotificationTask_Advice_For_Send_username_Manager(sGUID)
            Case eNotificationType.Error_Messages,
                 eNotificationType.Access_Dennied_Framework_Security
                oManager = Nothing 'No se soporta en ha
            Case eNotificationType.NewMessage_FromEmployee_InChannel
                oManager = New roNotificationTask_Message_FromEmployee_InChannel(sGUID)
            Case eNotificationType.Advice_For_NewConversation
                oManager = New roNotificationTask_Advice_For_New_Conversation(sGUID)
            Case eNotificationType.ScheduledCommuniquee
                oManager = New NotificationTaskScheduledCommuniqueManager(sGUID)
            Case Else
                oManager = Nothing
        End Select

        'If oManager IsNot Nothing AndAlso oNotificationTaskRow IsNot Nothing Then
        '    oManager.Load(oNotificationTaskRow)
        'End If

        Return oManager
    End Function

End Class