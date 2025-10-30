Imports Robotics.VTBase
Imports VTDiagCore.Robotics.Diagnostics
Imports VTDiagCore.VTDiagCore

Namespace API

    Public NotInheritable Class DiagMTServiceMethods

        Public Shared Function IsAlive() As Boolean

            Dim bolRet As Boolean = True

            Return bolRet

        End Function

        Public Shared Function CanSendEmail(ByVal reciever As String, ByVal header As String, ByVal body As String, ByVal strFileName As String, ByRef strResultMsg As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                Dim params As New Generic.List(Of String)
                params.Add(reciever)
                params.Add(header)
                params.Add(body)
                params.Add(strFileName)

                bolRet = roMonitor.getMonitorValue("system.cansendemail", strResultMsg, oState, params)
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function IsVisualTimeRunning(ByRef bolIsServerRunning As Boolean) As Boolean

            bolIsServerRunning = True
            Return bolIsServerRunning

        End Function

        Public Shared Function GetVisualTimeProcsInfo(ByRef dtProcs As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = True
                dtProcs = Nothing

            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetActiveEmployeeCount(ByRef dtEmployeeCount As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("query.checkEmployeesCount", dtEmployeeCount, oState)

                If Not bolRet Then
                    dtEmployeeCount = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetNotificationsCount(ByRef dtNotificationsCount As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("query.notificationsCount", dtNotificationsCount, oState)

                If Not bolRet Then
                    dtNotificationsCount = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetDBVersion(ByRef dbVersion As Long) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("ddbb.dbVersion", dbVersion, oState)
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function GetConnectionLostDuringLast24Hours(ByRef bolConnectionLost As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = True
                bolConnectionLost = False
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function CanWriteOnSystem(ByRef canWrite As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                canWrite = True
                Return canWrite
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetDailyScheduleStatus(ByRef dtStatus As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("ddbb.dailyschedule", dtStatus, oState)

                If Not bolRet Then
                    dtStatus = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEntriesCount(ByRef eCount As Long) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("ddbb.entries", eCount, oState)
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function GetUserTaskStatus(ByRef dtUserTasks As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("query.pendingtasks", dtUserTasks, oState)

                If Not bolRet Then
                    dtUserTasks = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetActiveSessionsCount(ByRef dtActiveSessionsCount As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                bolRet = roMonitor.getMonitorValue("query.activesessions", dtActiveSessionsCount, oState)

                If Not bolRet Then
                    dtActiveSessionsCount = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetTerminalsStatus(ByRef dtTerminalsStatus As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)

            Try
                Dim dsTerminalsStatus As New DataSet
                bolRet = roMonitor.getMonitorValue("query.terminalsstatus", dtTerminalsStatus, oState)

                If Not bolRet Then
                    dtTerminalsStatus = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetBackgroundTasksStatus(ByRef dtBackgroundTasksStatus As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As VTDiagCore.VTDiagCore.wscDiagnosticsState = New VTDiagCore.VTDiagCore.wscDiagnosticsState(-1)
            Try

                bolRet = roMonitor.getMonitorValue("query.backgroundtasksstatus", dtBackgroundTasksStatus, oState)

                If Not bolRet Then
                    dtBackgroundTasksStatus = Nothing
                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetQueries() As Robotics.Base.DTOs.VTDiag.Query()

            Dim queryService = New QueryService.Service()
            'Return queryService.GetQueries().Select(Function(o) New Robotics.Base.DTOs.VTDiag.Query() With {
            '                                            .Id = o.Id,
            '                                            .Name = o.Name,
            '                                            .Description = o.Description,
            '                                            .Parameters = o.Parameters.Select(Function(p) New Robotics.Base.DTOs.VTDiag.Parameter() With {
            '                                                .Name = p.Name,
            '                                                .Description = p.Description,
            '                                                .Type = p.Type}).ToList()
            '                                            }).ToArray()

            Dim oLst As New Generic.List(Of Robotics.Base.DTOs.VTDiag.Query)
            For Each oQuery As QueryService.Query In queryService.GetQueries()
                Dim oDestQuery As New Robotics.Base.DTOs.VTDiag.Query() With {
                    .Description = oQuery.Description,
                    .Id = oQuery.Id,
                    .Name = oQuery.Name,
                    .Parameters = New List(Of Robotics.Base.DTOs.VTDiag.Parameter)}

                For Each oParam As QueryService.Parameter In oQuery.Parameters

                    oDestQuery.Parameters.Add(New Robotics.Base.DTOs.VTDiag.Parameter With {
                        .Description = oParam.Description,
                        .Name = oParam.Name,
                        .Type = oParam.Type
                                          })
                Next

                oLst.Add(oDestQuery)
            Next

            Return oLst.ToArray
        End Function

        Public Shared Function ExecuteQuery(id As Integer, parameters As IDictionary(Of String, String)) As DataTable
            Dim queryService As New QueryService.Service()

            Dim oQueryParams As New Generic.List(Of QueryService.ParameterValue)

            For Each oDic As String In parameters.Keys
                oQueryParams.Add(New VTDiagCore.VTDiagCore.QueryService.ParameterValue() With {
                    .Name = oDic,
                    .Value = parameters(oDic)
                                 })
            Next

            Return queryService.RunQuery(id, oQueryParams)
        End Function

        Public Shared Sub ChangeVTLiveApiHTTPSEnabled()
            Dim service = New ActionsService.Service()
            service.ChangeVTLiveApiHTTPSEnabled()
        End Sub

        Public Shared Function IsVTLiveApiHTTPSEnabled() As Boolean
            Dim service = New ActionsService.Service()
            Return service.IsVTLiveApiHTTPSEnabled()
        End Function

        Public Shared Sub DeleteTerminalFolder(terminalId As Integer)
            Dim service = New ActionsService.Service()
            service.DeleteTerminalFolder(terminalId)
        End Sub

        Public Shared Function LaunchBroadcasterForTerminal(terminalId As Integer) As Boolean
            Dim service As New ConnectorService.Service()
            Return service.LaunchBroadcasterForTerminal(terminalId)
        End Function

        Public Shared Function RebootTerminal(terminalId As Integer, ByRef errorMessage As String) As Boolean
            Dim service As New ConnectorService.Service()
            Return service.RebootTerminal(terminalId, errorMessage)
        End Function
    End Class

End Namespace