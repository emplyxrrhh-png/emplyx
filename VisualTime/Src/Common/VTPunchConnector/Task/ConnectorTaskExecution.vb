Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTPunchConnector

    Public Class ConnectorTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As New BaseTaskResult With {.Result = True, .Description = String.Empty}

            Dim oManager As New roPunchConnectorManager()
            bolRet = oManager.ExecuteTask(oTask)

            Return bolRet
        End Function

#End Region


    End Class

End Namespace