Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEOGManager
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class EOGServerTask
        Inherits BaseTask

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New Generic.List(Of roLiveTaskTypes) From {
                roLiveTaskTypes.AssignTemplate,
                roLiveTaskTypes.AssignTemplatev2,
                roLiveTaskTypes.AssignWeekPlan,
                roLiveTaskTypes.EmployeeSecurtiyActions,
                roLiveTaskTypes.CopyAdvancedPlan,
                roLiveTaskTypes.CopyPlan,
                roLiveTaskTypes.MassCause,
                roLiveTaskTypes.MassCopy,
                roLiveTaskTypes.MassPunch,
                roLiveTaskTypes.DeleteOldPhotos,
                roLiveTaskTypes.JustifiedIncidences,
                roLiveTaskTypes.EmployeeMessage,
                roLiveTaskTypes.ManageVisits,
                roLiveTaskTypes.PurgeNotifications,
                roLiveTaskTypes.CompleteTasksAndProjects,
                roLiveTaskTypes.DeleteAccessMovesHistory,
                roLiveTaskTypes.MassProgrammedAbsence,
                roLiveTaskTypes.CopyAdvancedPlanv2,
                roLiveTaskTypes.CopyAdvancedBudgetPlan,
                roLiveTaskTypes.DeleteOldDocuments,
                roLiveTaskTypes.ValidityDocuments,
                roLiveTaskTypes.DocumentTracking,
                roLiveTaskTypes.AIPlannerTask,
                roLiveTaskTypes.CheckCloseDate,
                roLiveTaskTypes.SecurityPermissions,
                roLiveTaskTypes.MassMarkConsents,
                roLiveTaskTypes.GenerateRoboticsPermissions,
                roLiveTaskTypes.AssignCenters,
                roLiveTaskTypes.RecalculatePunchDirection,
                roLiveTaskTypes.MigrateDocsToAzure,
                roLiveTaskTypes.CheckInvalidEntries,
                roLiveTaskTypes.AddReportToDocManager,
                roLiveTaskTypes.CheckScheduleRulesFaults,
                roLiveTaskTypes.KeepAlive,
                roLiveTaskTypes.ConsolidateData,
                roLiveTaskTypes.DeleteOldAudit,
                roLiveTaskTypes.ChangeRequestPermissions,
                roLiveTaskTypes.RemoveExpiredTasks,
                roLiveTaskTypes.ValidateSignStatusDocument,
                roLiveTaskTypes.CheckAutomaticRequests,
                roLiveTaskTypes.BlockInactivePassports,
                roLiveTaskTypes.MassLockDate,
                roLiveTaskTypes.DeleteOldBiometricData,
                roLiveTaskTypes.CheckConcurrenceData,
                roLiveTaskTypes.DeleteOldComplaints,
                roLiveTaskTypes.SynchronizeTerminals,
                roLiveTaskTypes.DeleteOldPunches,
                roLiveTaskTypes.DataMonitoring
            }
        End Function

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim eogManager As New EOGServerTaskExecution()
            Return eogManager.ExecuteTask(oTask)
        End Function


        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean
            If oTaskType = roLiveTaskTypes.AssignTemplate OrElse
                oTaskType = roLiveTaskTypes.AssignTemplatev2 OrElse
                oTaskType = roLiveTaskTypes.AssignWeekPlan OrElse
                oTaskType = roLiveTaskTypes.CopyAdvancedPlan OrElse
                oTaskType = roLiveTaskTypes.CopyAdvancedPlanv2 OrElse
                oTaskType = roLiveTaskTypes.ValidateSignStatusDocument OrElse
                oTaskType = roLiveTaskTypes.CopyPlan OrElse
                oTaskType = roLiveTaskTypes.MassCopy OrElse
                oTaskType = roLiveTaskTypes.JustifiedIncidences OrElse
                oTaskType = roLiveTaskTypes.MassCause OrElse
                oTaskType = roLiveTaskTypes.CompleteTasksAndProjects OrElse
                oTaskType = roLiveTaskTypes.EmployeeMessage OrElse
                oTaskType = roLiveTaskTypes.MassPunch OrElse
                oTaskType = roLiveTaskTypes.MassProgrammedAbsence OrElse
                oTaskType = roLiveTaskTypes.CopyAdvancedBudgetPlan OrElse
                oTaskType = roLiveTaskTypes.RecalculatePunchDirection OrElse
                oTaskType = roLiveTaskTypes.AIPlannerTask OrElse
                oTaskType = roLiveTaskTypes.MassLockDate OrElse
                oTaskType = roLiveTaskTypes.AssignCenters OrElse
                oTaskType = roLiveTaskTypes.EmployeeSecurtiyActions Then
                Return True
            End If

            Return False
        End Function
    End Class

End Namespace