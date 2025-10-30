Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace Robotics.Diagnostics

    Public Class roSQLDiagnostics

        Public Shared Function getDisconnectionTimes(ByRef oState As VTDiagCore.wscDiagnosticsState) As Boolean
            Try
                Dim oSettings As New roSettings
                Dim systemPath As String = oSettings.GetVTSetting(eKeys.System)

                Dim path As String = IO.Path.Combine(systemPath, "ErrorLog_" + Now.ToString("yyyyMMdd") + ".DAT")
                If IO.File.Exists(path) Then
                    Dim sr As IO.StreamReader = New IO.StreamReader(path)
                    Dim existsDisconnection As Boolean = False

                    Do While sr.Peek() >= 0
                        Dim logLine As String = sr.ReadLine()

                        'TODO parlar amb lluís
                        If logLine.ToLower.Contains("could not open table") Then
                            existsDisconnection = True
                        End If

                        If existsDisconnection Then
                            Exit Do
                        End If
                    Loop
                    sr.Close()

                    Return existsDisconnection
                Else
                    Return False
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.FileAccessError
                Return False
            End Try
        End Function

        Public Shared Function getDBVersion(ByRef oState As VTDiagCore.wscDiagnosticsState) As Long
            Dim bolRet As Boolean = False

            Try
                Dim sql As String = "@SELECT# Data from sysroParameters where ID = 'DBVERSION'"
                Return AccessHelper.ExecuteScalar(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return 0
            End Try
        End Function

        Public Shared Function GetActiveCountEmployees(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                Dim sql As String = "@SELECT# t1.TotalGH,t2.TotalProd from " &
                                    "(@SELECT# COUNT(IDEmployee) AS TotalGH From EmployeeContracts WHERE EndDate > getDate() AND BeginDate<= getDate()) t1, " &
                                    "(@SELECT# COUNT(Employees.ID) AS TotalProd From Employees,EmployeeContracts WHERE Employees.ID = EmployeeContracts.IDEmployee AND EmployeeContracts.EndDate > getDate() AND EmployeeContracts.BeginDate<=getDate() AND Employees.Type ='J')t2"

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function GetNotificationsCount(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                Dim sql As String = "@SELECT# t1.TotalRequest, t2.TotalSupervisor, t3.TotalNotifications from " &
                                    "(@SELECT# COUNT(*) AS TotalRequest FROM Requests WHERE Status in(0,1)) t1, " &
                                    "(@SELECT# COUNT(*) AS TotalSupervisor FROM sysroNotificationTasks INNER JOIN Notifications ON sysroNotificationTasks.IDNotification = Notifications.ID and Notifications.AllowPortal = 1 WHERE sysroNotificationTasks.IsReaded = 1)t2, " &
                                    "(@SELECT# COUNT(*) AS TotalNotifications FROM sysroNotificationTasks INNER JOIN Notifications ON sysroNotificationTasks.IDNotification = Notifications.ID and Notifications.AllowMail = 1 WHERE sysroNotificationTasks.Executed = 1)t3"

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function GetPendingTasks(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                Dim sql As String = "@SELECT# * FROM sysroTasks ORDER BY DateCreated ASC"

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function GetActiveSessionsCount(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                'Dim sql As String = "@SELECT# t1.LiveSessions,t2.LivePortalSessions, t3.SupervisorPortalSessions, t4.BloquedUsers from " & _
                '                    "(@SELECT# COUNT(*) AS LiveSessions FROM sysroPassports_Sessions WHERE ApplicationName like '%VTLive%') t1, " & _
                '                    "(@SELECT# COUNT(*) AS LivePortalSessions FROM sysroPassports_Sessions WHERE ApplicationName like '%VTAnywhere%')t2, " & _
                '                    "(@SELECT# COUNT(*) AS SupervisorPortalSessions FROM sysroPassports_Sessions WHERE ApplicationName like '%SupervisorPortal%')t3, " & _
                '                    "(@SELECT# COUNT(*) AS BloquedUsers FROM sysroPassports_AuthenticationMethods WHERE Method = 1 and BloquedAccessApp = 1)t4 "

                Dim sql As String = "@SELECT# ApplicationName, COUNT(*) AS ActiveSessions FROM sysroPassports_Sessions WHERE InvalidationCode = 0 group by ApplicationName " &
                                    "UNION " &
                                    "@SELECT# 'Usuarios bloqueados' as ApplicationName, COUNT(*) AS ActiveSessions FROM sysroPassports_AuthenticationMethods WHERE Method = 1 and BloquedAccessApp = 1 "

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function GetTerminalsStatus(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                Dim sql As String = "@SELECT# Terminals.ID,Terminals.Type,Terminals.Location,Terminals.LastStatus,Terminals.LastUpdate, " &
                                    " (@SELECT# count(*) from TerminalsSyncTasks where IDTerminal = ID) AS TerminalSyncTasks FROM terminals " &
                                    "WHERE Type <> 'LivePortal' AND Type <> 'Virtual' "

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function GetBackgroundTasksStatus(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Dim bolRet As DataTable = Nothing

            Try
                Dim sql As String = "@SELECT# * from( " &
                                    "@SELECT# ID, 'Informe' AS Type, CASE WHEN Status = 0 THEN 'En cola' WHEN Status = 1 AND UploadFile = '' THEN 'En curso' WHEN Status = 1 AND UploadFile <> '' And UploadFile not like '%ReportsCR%' Then 'En cola' END AS Estado, ReportName AS Name, '' AS Perfil, TimeStamp AS LastExecution from sysroReportTasks where (Status = 0 OR (Status = 1 and UploadFile not like '%ReportsCR%')) and TimeStamp > DATEADD(dd,-1,getdate()) " &
                                    "UNION " &
                                    "@SELECT# ReportsScheduler.ID, 'Informe planificado' AS Type, CASE WHEN State = 4 THEN 'Error' WHEN State = 2 THEN 'En curso' WHEN State = 0 Then 'En cola' END AS Estado, ReportsScheduler.Name as Name, sysroReportProfile.Name AS Perfil, LastExecution AS LastExecution  from ReportsScheduler inner join sysroReportProfile on sysroReportProfile.ID = ReportsScheduler.Profile  where State <> 3 and NextDateTimeExecuted <= getdate() " &
                                    "UNION " &
                                    "@SELECT# ID, 'Tarea' AS Type, CASE WHEN Status = 3 THEN 'Error' WHEN Status = 1 THEN 'En curso' WHEN Status = 0 Then 'En cola' END AS Estado, Action AS Name, '' AS Perfil, ExecutionDate AS LastExecution  from sysroLiveTasks  where Status <> 2 and ExecutionDate > DATEADD(dd,-1,getdate()) " &
                                    ") data " &
                                    "order by data.Type "

                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function getDDBBEntries(ByRef oState As VTDiagCore.wscDiagnosticsState) As Double
            Try
                Dim sql As String = "@SELECT# count(*) from entries where DateTime<dateadd(d,-2,getdate())"
                Return AccessHelper.ExecuteScalar(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return 0
            End Try
        End Function

        Public Shared Function getDDBBDailySchedule(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Try
                Dim sql As String = "@SELECT#  " &
                                    "CASE WHEN Status  = 0 THEN '0 - Sin procesar(Detector)' " &
                                    "WHEN Status  >= 40  and Status < 45 THEN '40 - Incidences' " &
                                    "WHEN Status  >= 45  and Status < 50 THEN '45 - Incidences' " &
                                    "WHEN Status  >= 50  and Status < 55  THEN '50 - Causes' " &
                                    "WHEN Status  >= 55  and Status < 60  THEN '55 - Causes' " &
                                    "WHEN Status  >= 60  and Status < 65  THEN '60 - Accruals' " &
                                    "WHEN Status  >= 65  and Status < 70 THEN '65 - Accruals' END AS Action, " &
                                    "count(*) AS Counter " &
                                    "from DailySchedule where date<getdate() and status < 70 Group By Status "
                Return AccessHelper.CreateDataTable(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return Nothing
            End Try
        End Function

        Public Shared Function getDDBBSize(ByRef oState As VTDiagCore.wscDiagnosticsState) As Double
            Try
                Dim sql As String = "@SELECT# sum(size * 8 / 1024.00) AS size_in_mb FROM sys.database_files where physical_name like '%mdf'"
                Return AccessHelper.ExecuteScalar(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return 0
            End Try
        End Function

        Public Shared Function getDDBBSysRoTask(ByRef oState As VTDiagCore.wscDiagnosticsState) As Double
            Try
                Dim sql As String = "@SELECT# count(*) from sysroTasks"
                Return AccessHelper.ExecuteScalar(sql)
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ConnectionError
                Return 0
            End Try
        End Function

    End Class

End Namespace