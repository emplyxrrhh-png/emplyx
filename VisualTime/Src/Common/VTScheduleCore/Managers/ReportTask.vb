Imports System.Data
Imports ReportGenerator
Imports ReportGenerator.Services
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTNotificationsCore.Notifications
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTScheduleManager

    Public Class ReportTaskManager
        Inherits BaseTask

        Private ReadOnly _reportExecutionService As IReportExecutionService
        Private ReadOnly _reportStorageService As IReportStorageService
        Private ReadOnly _reportGeneratorService As IReportGeneratorService

        Public Sub New(reportExecutionService As IReportExecutionService, reportStorageService As IReportStorageService, reportGeneratorService As IReportGeneratorService)
            _reportExecutionService = reportExecutionService
            _reportStorageService = reportStorageService
            _reportGeneratorService = reportGeneratorService
        End Sub

        Protected Overrides Function ExecuteTaskFromManager(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim eogManager As New TaskManager.ReportTaskExecution(_reportExecutionService, _reportStorageService, _reportGeneratorService)
            Return eogManager.ExecuteTask(oTask)
        End Function

        Protected Overrides Function GetProcessTaskTypes() As List(Of roLiveTaskTypes)
            Return New List(Of roLiveTaskTypes) From {
                    roLiveTaskTypes.ReportTaskDX,
                    roLiveTaskTypes.GenerateReportsDxTasks
                }
        End Function

        Protected Overrides Function NeedToKeepTask(ByVal oTaskType As roLiveTaskTypes) As Boolean
            If oTaskType = roLiveTaskTypes.ReportTaskDX Then
                Return True
            End If

            Return False
        End Function

    End Class
End Namespace
