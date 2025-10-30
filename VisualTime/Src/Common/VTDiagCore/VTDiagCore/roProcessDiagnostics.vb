Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Robotics.Diagnostics

    Public Class roProcessDiagnostics

        Public Shared AvailableProcess As String() = {
            "roProcAccruals",
            "roProcBroadcaster",
            "roProcCauses",
            "roProcCollector",
            "roProcCommServer",
            "roProcCommsNET",
            "roProcCommsRXF",
            "roProcDataImport",
            "roProcDetector",
            "roProcEOGServer",
            "roProcHRActualScheduler",
            "roProcHRPlannedScheduler",
            "roProcIncidences",
            "roProcJobAccruals",
            "roProcMachineAccruals",
            "roProcJobMoves",
            "roProcJobPieces",
            "roProcJobTime",
            "roProcMovesNet",
            "roProcNotificationServer",
            "roProcNotifier",
            "roProcPunchConnector",
            "roProcReportingServerNet",
            "roProcReportServer",
            "roProcReportPriorityServer",
            "roProcTaskAccruals",
            "sqlservr",
            "VTServer",
            "VTConsole"
        }

        Public Shared Function getAllVisualTimeProcessInfo(ByRef oState As VTDiagCore.wscDiagnosticsState) As DataTable
            Try
                Dim dtProcess As New DataTable

                dtProcess.Columns.Add(New DataColumn("ProcessName", GetType(String)))
                dtProcess.Columns.Add(New DataColumn("IsRunning", GetType(String)))
                dtProcess.Columns.Add(New DataColumn("ProcessInfo", GetType(String)))

                Dim procs As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcesses()

                Dim oRow As DataRow

                For Each strProcessName As String In AvailableProcess

                    If processShouldBeRunning(strProcessName) Or strProcessName.StartsWith("ro") = False Then
                        Dim selProcess As System.Diagnostics.Process = Nothing

                        For Each oProc As System.Diagnostics.Process In procs
                            If oProc.ProcessName = strProcessName Then
                                selProcess = oProc
                                Exit For
                            End If
                        Next

                        oRow = dtProcess.NewRow
                        oRow("ProcessName") = strProcessName
                        If selProcess IsNot Nothing Then
                            If selProcess.Responding Then
                                oRow("IsRunning") = "Running"
                            Else
                                oRow("IsRunning") = "Not Responding"
                            End If

                            oRow("ProcessInfo") = "Physical memory usage: " & Format((selProcess.WorkingSet64 / 1024 / 1024), "0.00") & "Mb // Total processor Time:" & selProcess.TotalProcessorTime.ToString("dd\.hh\:mm\:ss\:FFFF")
                        Else
                            oRow("IsRunning") = "Stopped"
                            oRow("ProcessInfo") = ""
                        End If
                        dtProcess.Rows.Add(oRow)
                    End If
                Next

                Return dtProcess
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                Return Nothing
            End Try

        End Function

        Private Shared Function processShouldBeRunning(ByVal strProcessName As String) As Boolean
            Dim bolRet As Boolean = False
            Dim oLicencse As New roServerLicense
            Select Case strProcessName
                Case "roProcAccruals"
                    Return True
                Case "roProcBroadcaster"
                    Return True
                Case "roProcCauses"
                    Return True
                Case "roProcCollector"
                    Return True
                Case "roProcCommServer"
                    Return True
                Case "roProcCommsNET"
                    Return True
                Case "roProcCommsRXF"
                    Return True
                Case "roProcDataImport"
                    Return oLicencse.FeatureIsInstalled("Forms\DataLink")
                Case "roProcDetector"
                    Return True
                Case "roProcEOGServer"
                    Return True
                Case "roProcHRActualScheduler"
                    Return oLicencse.FeatureIsInstalled("Feature\HRScheduling")
                Case "roProcHRPlannedScheduler"
                    Return oLicencse.FeatureIsInstalled("Feature\HRScheduling")
                Case "roProcIncidences"
                    Return True
                Case "roProcJobAccruals"
                    Return True
                Case "roProcMachineAccruals"
                    Return True
                Case "roProcJobMoves"
                    Return True
                Case "roProcJobPieces"
                    Return True
                Case "roProcJobTime"
                    Return True
                Case "roProcMovesNet"
                    Return True
                Case "roProcNotificationServer"
                    Return True
                Case "roProcNotifier"
                    Return True
                Case "roProcPunchConnector"
                    Return oLicencse.FeatureIsInstalled("Feature\PunchConnector")
                Case "roProcReportingServerNet"
                    Return True
                Case "roProcReportServer"
                    Return True
                Case "roProcReportPriorityServer"
                    Return True
                Case "roProcTaskAccruals"
                    Return oLicencse.FeatureIsInstalled("Feature\Productiv")

            End Select

            Return bolRet
        End Function

        Public Shared Function getServerRunning(ByRef oState As VTDiagCore.wscDiagnosticsState) As Boolean
            Try
                Dim proc As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName("VTServer")
                Dim oSettings As New roSettings()
                Dim obj As Object = oSettings.GetVTSetting(eKeys.Running)

                If proc.Length > 0 AndAlso roTypes.Any2String(obj).ToUpper = "TRUE" Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                Return 0
            End Try
        End Function

        Public Shared Function getProcessRun(ByVal Name As String, ByRef oState As VTDiagCore.wscDiagnosticsState) As Boolean
            Try
                Dim proc As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName(Name)
                Return proc.Length > 0
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                Return False
            End Try

        End Function

        Public Shared Function getProcessCPU(ByVal Name As String, ByRef oState As VTDiagCore.wscDiagnosticsState) As Double
            Try
                Dim proc As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName(Name)
                If proc.Length > 0 Then

                    Dim PC As PerformanceCounter = New PerformanceCounter
                    PC.CategoryName = "Process"
                    PC.CounterName = "% Processor Time"
                    PC.InstanceName = Name

                    Dim privMemory As Single = PC.NextValue()
                    Threading.Thread.Sleep(1000)
                    Return PC.NextValue()
                Else
                    oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                    Return 0
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                Return 0
            End Try
        End Function

        Public Shared Function getProcessMemory(ByVal Name As String, ByRef oState As VTDiagCore.wscDiagnosticsState) As Double
            Try
                Dim proc As System.Diagnostics.Process() = System.Diagnostics.Process.GetProcessesByName(Name)
                If proc.Length > 0 Then
                    Dim PC As PerformanceCounter = New PerformanceCounter
                    PC.CategoryName = "Process"
                    PC.CounterName = "Working Set - Private"
                    PC.InstanceName = Name

                    Dim privMemory As Single = PC.NextValue() / 1024 / 1024
                    Threading.Thread.Sleep(1000)
                    Return PC.NextValue() / 1024 / 1024
                Else
                    oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                    Return 0
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.ProcessAccessError
                Return 0
            End Try
        End Function

    End Class

End Namespace