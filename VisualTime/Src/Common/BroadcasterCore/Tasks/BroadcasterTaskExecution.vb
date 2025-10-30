Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Public Class BroadcasterTaskExecution

#Region "Declarations - Constructor"

    Public Sub New()

    End Sub

#End Region

#Region "Methods"

    Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
        Dim bolRet As New BaseTaskResult With {.Result = True, .Description = String.Empty}

        Select Case UCase(oTask.Action)
            Case roLiveTaskTypes.BroadcasterTask.ToString().ToUpper
                bolRet = BroadcasterTaskExecution.ExecuteBroadcaster(oTask)
        End Select

        Return bolRet
    End Function

    Public Shared Function ExecuteBroadcaster(ByVal oTask As roLiveTask) As BaseTaskResult
        Dim bolRet As Boolean = True
        ' Leer parametros de la tarea y ejecutar broadcaster para todos los terminales o para 1 en concreto.

        Try

            Dim bDisableBroadcaster As Boolean = roTypes.Any2Boolean(roCacheManager.GetInstance.GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Main.DisableBroadcaster"))
            If Not bDisableBroadcaster Then
                Dim oBroadcasterManager As BroadcasterManager = New BroadcasterManager()

                If oTask.Parameters.Exists("IDTerminal") Then
                    'TerminalId EXISTS: LAUNCH THE BROADCASTER
                    oBroadcasterManager.RunBroadcaster(roTypes.Any2Integer(oTask.Parameters("IDTerminal")),
                                                            If(oTask.Parameters.Exists("TerminalsTask"), roTypes.Any2String(oTask.Parameters("TerminalsTask")), String.Empty),
                                                            If(oTask.Parameters.Exists("IDEmployees"), roTypes.Any2Integer(oTask.Parameters("IDEmployees")), 0),
                                                            If(oTask.Parameters.Exists("OnlyTask"), roTypes.Any2Boolean(oTask.Parameters("OnlyTask")), False),
                                                            If(oTask.Parameters.Exists("IDFinger"), roTypes.Any2Integer(oTask.Parameters("IDFinger")), False))
                Else
                    'TerminalId DOES NOT EXIST: MUST LAUNCH THE BROADCASTER FOR ALL THE TERMINALS
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CBroadcasterServer::ExecuteTask:: Launching Broadcast for all terminals due to task -> " & oTask.Parameters.XML)
                    Dim bState As roTerminalState = New roTerminalState(-1)
                    Dim oTerminals As New roTerminalList()
                    oTerminals.LoadData("type not in ('LivePortal' , 'NFC' , 'Virtual', 'masterASP', 'Suprema', 'Time Gate')  AND Enabled = 1", bState)

                    For Each oTerminal As roTerminal In oTerminals.Terminals
                        Try
                            Dim oParameters As New roCollection
                            oParameters.Add("IDTerminal", oTerminal.ID)

                            Dim taskId As Integer = roLiveTask.CreateLiveTask(roLiveTaskTypes.BroadcasterTask, oParameters, New roLiveTaskState())

                            If taskId = -1 Then bolRet = False
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "CBroadcasterServer::ExecuteTask::", ex)
                        End Try
                    Next
                End If
            Else
                bolRet = True
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "CBroadcasterServer::Error::", ex)
        End Try

        Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
    End Function

#End Region

End Class