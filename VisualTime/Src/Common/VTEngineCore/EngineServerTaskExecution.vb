Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTEngineManager

    Public Class EngineServerTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = False

            Try
                bolRet = roEngineManager.ExecuteTask(oTask)

            Catch Ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "CEngineServerTaskExecution::ExecuteTask :", Ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

#End Region

    End Class

End Namespace