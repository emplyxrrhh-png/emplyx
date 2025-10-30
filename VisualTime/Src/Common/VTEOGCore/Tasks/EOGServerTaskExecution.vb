Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTEOGManager

    Public Class EOGServerTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As BaseTaskResult = roEOGManager.ExecuteTask(oTask)

            Return bolRet
        End Function

#End Region

    End Class

End Namespace