Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.DataLayer

Namespace VTDiagCore.ConnectorService

    Public Class Service

        Public Function LaunchBroadcasterForTerminal(ByVal IDTerminal As Integer) As Boolean
            Try
                Dim oParams As New roCollection($"<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""Command"" type=""8"">RESET_TERMINAL</Item><Item key=""IDTerminal"" type=""2"">{IDTerminal}</Item></roCollection>")
                roConnector.InitTask(TasksType.BROADCASTER, oParamsAux:=oParams)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
        Public Function RebootTerminal(ByVal IDTerminal As Integer, ByRef errorMessage As String) As Boolean
            Try
                Dim sql As String = $"@SELECT# Type FROM Terminals WHERE ID = {IDTerminal}"
                Dim type As String = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
                If type.ToUpper.Contains("RX") Then
                    sql = $"IF NOT EXISTS(@SELECT# * FROM TerminalsSyncTasks WHERE Task = 'reboot' AND IDTerminal = {IDTerminal} AND TaskSent IS NULL)
                                      @INSERT# INTO TerminalsSyncTasks (Task, Idterminal, TaskDate) VALUES ('reboot', {IDTerminal}, GETDATE())"
                    Return AccessHelper.ExecuteSql(sql)
                Else
                    errorMessage = "El terminal no existe o no es de la familia rx"
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Class

End Namespace