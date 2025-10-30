Namespace Robotics.Diagnostics

    Public Class roMonitor

        ''' <summary>
        ''' Devuelve el valor del monitor indicado
        ''' </summary>
        ''' <param name="MonitorName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function getMonitorValue(ByVal MonitorName As String, ByRef retValue As Object, ByRef oState As VTDiagCore.wscDiagnosticsState, Optional ByVal params As Generic.List(Of String) = Nothing) As Boolean
            Try
                Select Case MonitorName.Split(".")(0).ToLower
                    Case "process"
                        Select Case MonitorName.Split(".")(1).ToLower
                            Case "run"
                                retValue = roProcessDiagnostics.getProcessRun(params(0), oState)
                            Case "cpu"
                                retValue = roProcessDiagnostics.getProcessCPU(params(0), oState)
                            Case "memory"
                                retValue = roProcessDiagnostics.getProcessMemory(params(0), oState)
                            Case "isvisualtimerunning"
                                retValue = roProcessDiagnostics.getServerRunning(oState)
                            Case "visualtimeprocessinfo"
                                retValue = roProcessDiagnostics.getAllVisualTimeProcessInfo(oState)
                        End Select

                    Case "ddbb"
                        Select Case MonitorName.Split(".")(1).ToLower
                            Case "dbversion"
                                retValue = roSQLDiagnostics.getDBVersion(oState)
                            Case "entries"
                                retValue = roSQLDiagnostics.getDDBBEntries(oState)
                            Case "dailyschedule"
                                retValue = roSQLDiagnostics.getDDBBDailySchedule(oState)
                            Case "size"
                                retValue = roSQLDiagnostics.getDDBBSize(oState)
                            Case "sysrotask"
                                retValue = roSQLDiagnostics.getDDBBSysRoTask(oState)
                            Case "connectionloston24"
                                retValue = roSQLDiagnostics.getDisconnectionTimes(oState)
                        End Select
                    Case "files"
                        Select Case MonitorName.Split(".")(1).ToLower
                            Case "filesize"
                                retValue = roFilesDiagnostics.getFilesSize(params(0), params(1), oState)
                            Case "canwrite"
                                retValue = roFilesDiagnostics.canCreateFile(params(0), params(1), oState)
                        End Select
                    Case "query"
                        Select Case MonitorName.Split(".")(1).ToLower
                            Case "checkemployeescount"
                                retValue = roSQLDiagnostics.GetActiveCountEmployees(oState)
                            Case "notificationscount"
                                retValue = roSQLDiagnostics.GetNotificationsCount(oState)
                            Case "pendingtasks"
                                retValue = roSQLDiagnostics.GetPendingTasks(oState)
                            Case "activesessions"
                                retValue = roSQLDiagnostics.GetActiveSessionsCount(oState)
                            Case "terminalsstatus"
                                retValue = roSQLDiagnostics.GetTerminalsStatus(oState)
                            Case "backgroundtasksstatus"
                                retValue = roSQLDiagnostics.GetBackgroundTasksStatus(oState)
                        End Select
                    Case "system"
                        Select Case MonitorName.Split(".")(1).ToLower
                            Case "cansendemail"
                                retValue = roSystemInfo.canSendEmail(params(0), params(1), params(2), params(3), oState)
                        End Select
                End Select

                If oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.NoError Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try

        End Function

    End Class

End Namespace