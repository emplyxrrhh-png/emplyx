Imports System.Configuration
Imports System.Globalization
Imports System.Reflection
Imports System.Threading
Imports System.Web
Imports FxResources
Imports Microsoft.ApplicationInsights
Imports Robotics.Base.DTOs
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports Robotics.VTBase.roLog
Imports Robotics.VTBase.roTrace

Public Enum CustomInfoKeys
    IsSystem
    InstanceName
    Type
    Company
    Source
    Version
End Enum

Public Class roTelemetryClient
    Private oTelemetryConfiguration As Extensibility.TelemetryConfiguration = Nothing
    Private oTelemetryClient As TelemetryClient = Nothing

    Private Shared oClient As roTelemetryClient = Nothing

    Public Shared ReadOnly Property GetInstance() As roTelemetryClient
        Get
            If oClient Is Nothing Then
                oClient = New roTelemetryClient()
            End If

            Return oClient
        End Get

    End Property

    Public Sub New()
        Dim oConf As New Extensibility.TelemetryConfiguration()

        Dim telemetryKey As String = roTypes.Any2String(ConfigurationManager.AppSettings.Get("ApplicationInsights.Key"))
        If telemetryKey = String.Empty Then telemetryKey = roTypes.Any2String(System.Environment.GetEnvironmentVariable("ApplicationInsights.Key"))

        oConf.ConnectionString = "InstrumentationKey=" & telemetryKey
        oTelemetryClient = New TelemetryClient(oConf)
    End Sub

    Public Function TraceMessage(ByVal type As roTrace.TraceType, ByVal result As TraceResult, ByVal logmsg As String, ByVal gLogLevel As roTrace.TraceType, ByVal gComputerName As String) As Boolean
        Try
            If oTelemetryClient Is Nothing Then Return True

            Dim oSeverityLevel As DataContracts.SeverityLevel = DataContracts.SeverityLevel.Information
            Select Case type
                Case roTrace.TraceType.roDebug : oSeverityLevel = DataContracts.SeverityLevel.Verbose
                Case roTrace.TraceType.roInfo : oSeverityLevel = DataContracts.SeverityLevel.Information
            End Select

            ' Enviamos el log en caso que la configuración general así lo indique
            Dim bolApplyLog As Boolean = (CInt(type) >= CInt(gLogLevel))

            If bolApplyLog Then
                Dim currentVersion As Version = Assembly.GetExecutingAssembly().GetName().Version

                Dim insightsProperties As New Dictionary(Of String, String) From {
                    {CustomInfoKeys.IsSystem.ToString(), 0},
                    {CustomInfoKeys.InstanceName.ToString(), gComputerName},
                    {CustomInfoKeys.Type.ToString(), "trace"},
                    {CustomInfoKeys.Version.ToString(), currentVersion.ToString()}
                }

                Try
                    If VTBase.roConstants.IsMultitenantServiceEnabled Then
                        Dim oFuncName As String = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName"))
                        Dim sCompanyName As String = roTypes.Any2String(Thread.GetDomain().GetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_company"))
                        If sCompanyName = String.Empty Then sCompanyName = "FUNCTION"

                        insightsProperties.Add(CustomInfoKeys.Company.ToString(), sCompanyName)
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), oFuncName)
                    Else
                        Dim sAppName As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.LogFileName))
                        Dim sCompanyName As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))
                        If sCompanyName = String.Empty Then If sCompanyName = String.Empty Then sCompanyName = "WEBAPP"

                        insightsProperties.Add(CustomInfoKeys.Company.ToString(), sCompanyName)
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), sAppName)

                    End If

                    insightsProperties = AddProcessO11yProperties(insightsProperties)

                    insightsProperties = AddTracesContext(insightsProperties)

                    If insightsProperties.ContainsKey(roTrace.o11yTraceKeys.Result.ToString()) Then
                        insightsProperties(roTrace.o11yTraceKeys.Result.ToString()) = result.ToString()
                    Else
                        insightsProperties.Add(roTrace.o11yTraceKeys.Result.ToString(), result.ToString())
                    End If

                    Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                    If _traceInfo.ContainsKey(roTrace.o11yTraceKeys.DescriptionMsg.ToString()) Then
                        logmsg = $"{logmsg}. {_traceInfo(roTrace.o11yTraceKeys.DescriptionMsg.ToString())}"
                    End If

                Catch ex As Exception
                    gComputerName = "EXCEPTION"
                End Try

                oTelemetryClient.TrackTrace(logmsg, oSeverityLevel, insightsProperties)
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function LogMessage(ByVal type As roLog.EventType, ByVal logmsg As String, ByVal gLogLevel As roLog.EventType, ByVal gComputerName As String, customColumns As Dictionary(Of String, String)) As Boolean
        Try
            If oTelemetryClient Is Nothing Then Return True

            Dim oSeverityLevel As DataContracts.SeverityLevel = DataContracts.SeverityLevel.Information
            Select Case type
                Case roLog.EventType.roDebug : oSeverityLevel = DataContracts.SeverityLevel.Verbose
                Case roLog.EventType.roInfo : oSeverityLevel = DataContracts.SeverityLevel.Information
                Case roLog.EventType.roWarning : oSeverityLevel = DataContracts.SeverityLevel.Warning
                Case roLog.EventType.roError : oSeverityLevel = DataContracts.SeverityLevel.Error
                Case roLog.EventType.roCritic : oSeverityLevel = DataContracts.SeverityLevel.Critical
            End Select

            ' Enviamos el log en caso que la configuración general así lo indique
            Dim bolApplyLog As Boolean = (CInt(type) >= CInt(gLogLevel))

            If bolApplyLog Then
                Dim currentVersion As Version = Assembly.GetExecutingAssembly().GetName().Version
                Dim insightsProperties As New Dictionary(Of String, String) From {
                    {CustomInfoKeys.IsSystem.ToString(), 0},
                    {CustomInfoKeys.InstanceName.ToString(), gComputerName},
                    {CustomInfoKeys.Type.ToString(), "log"},
                    {CustomInfoKeys.Version.ToString(), currentVersion.ToString()}
                }

                Try
                    If VTBase.roConstants.IsMultitenantServiceEnabled Then
                        Dim oFuncName As String = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName"))
                        Dim sCompanyName As String = roTypes.Any2String(Thread.GetDomain().GetData(Thread.CurrentThread.ManagedThreadId.ToString() & "_company"))
                        If sCompanyName = String.Empty Then sCompanyName = "FUNCTION"

                        insightsProperties.Add(CustomInfoKeys.Company.ToString(), sCompanyName)
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), oFuncName)
                    Else
                        Dim sAppName As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.LogFileName))
                        Dim sCompanyName As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId))
                        If sCompanyName = String.Empty Then If sCompanyName = String.Empty Then sCompanyName = "WEBAPP"

                        insightsProperties.Add(CustomInfoKeys.Company.ToString(), sCompanyName)
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), sAppName)

                    End If

                    insightsProperties = AddProcessO11yProperties(insightsProperties)
                    insightsProperties = AddCustomO11yProperties(customColumns, insightsProperties)

                Catch ex As Exception
                    gComputerName = "EXCEPTION"
                End Try

                oTelemetryClient.TrackTrace(logmsg, oSeverityLevel, insightsProperties)
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function LogSystemMessage(ByVal type As roLog.EventType, ByVal logmsg As String, ByVal forceWrite As Boolean, customColumns As Dictionary(Of String, String)) As Boolean
        Try
            If oTelemetryClient Is Nothing Then Return True

            Dim gLogLevel As EventType = roConstants.GetDefaultLogLevel
            If Not forceWrite AndAlso gLogLevel = EventType.roDisabled Then Return True


            Dim oSeverityLevel As DataContracts.SeverityLevel = DataContracts.SeverityLevel.Information
            Select Case type
                Case roLog.EventType.roDebug : oSeverityLevel = DataContracts.SeverityLevel.Verbose
                Case roLog.EventType.roInfo : oSeverityLevel = DataContracts.SeverityLevel.Information
                Case roLog.EventType.roWarning : oSeverityLevel = DataContracts.SeverityLevel.Warning
                Case roLog.EventType.roError : oSeverityLevel = DataContracts.SeverityLevel.Error
                Case roLog.EventType.roCritic : oSeverityLevel = DataContracts.SeverityLevel.Critical
            End Select


            ' Enviamos el log en caso que la configuración general así lo indique
            If forceWrite OrElse (CInt(type) >= CInt(gLogLevel)) Then
                Dim currentVersion As Version = Assembly.GetExecutingAssembly().GetName().Version

                Dim gComputerName As String = Environment.MachineName
                Dim insightsProperties As New Dictionary(Of String, String) From {
                    {CustomInfoKeys.IsSystem.ToString(), 1},
                    {CustomInfoKeys.InstanceName.ToString(), gComputerName},
                    {CustomInfoKeys.Type.ToString(), "log"},
                    {CustomInfoKeys.Version.ToString(), currentVersion.ToString()}
                }

                Try
                    If VTBase.roConstants.IsMultitenantServiceEnabled Then
                        Dim oFuncName As String = roTypes.Any2String(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_AppName"))
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), oFuncName)
                    Else
                        Dim sAppName As String = VTBase.roTypes.Any2String(VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.AppName))
                        insightsProperties.Add(CustomInfoKeys.Source.ToString(), sAppName)
                    End If

                Catch ex As Exception
                    gComputerName = "EXCEPTION"
                End Try

                'insightsProperties = AddProcessO11yProperties(insightsProperties)
                'insightsProperties = AddCustomO11yProperties(customColumns, insightsProperties)

                oTelemetryClient.TrackTrace(logmsg, oSeverityLevel, insightsProperties)
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Shared Function AddCustomO11yProperties(customColumns As Dictionary(Of String, String), insightsProperties As Dictionary(Of String, String)) As Dictionary(Of String, String)
        If customColumns IsNot Nothing Then
            For Each kvp In customColumns
                If Not insightsProperties.ContainsKey(kvp.Key) Then
                    insightsProperties.Add(kvp.Key, kvp.Value)
                Else
                    insightsProperties(kvp.Key) = kvp.Value
                End If
            Next
        End If

        Return insightsProperties
    End Function

    Private Shared Function AddProcessO11yProperties(insightsProperties As Dictionary(Of String, String)) As Dictionary(Of String, String)
        Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
        If _o11yInfo IsNot Nothing Then
            For Each kvp In _o11yInfo

                If kvp.Key = o11yKeys.ProcessStart.ToString() Then
                    Dim processTicks As Long = roTypes.Any2Long(kvp.Value)
                    Dim diferenciaTicks As Long = Math.Abs(DateTime.UtcNow.Ticks - CType(processTicks, Long))
                    Dim diferenciaTiempo As TimeSpan = TimeSpan.FromTicks(diferenciaTicks)

                    If Not insightsProperties.ContainsKey(o11yKeys.ProcessTime.ToString()) Then
                        insightsProperties.Add(o11yKeys.ProcessTime.ToString(), diferenciaTiempo.TotalSeconds.ToString("F4", CultureInfo.InvariantCulture))
                    Else
                        insightsProperties(o11yKeys.ProcessTime.ToString()) = diferenciaTiempo.TotalSeconds.ToString("F4", CultureInfo.InvariantCulture)
                    End If

                    If Not insightsProperties.ContainsKey(o11yKeys.ProcessTimeUnit.ToString()) Then
                        insightsProperties.Add(o11yKeys.ProcessTimeUnit.ToString(), ProcessTimeUnit.seconds.ToString())
                    End If

                Else
                    If Not insightsProperties.ContainsKey(kvp.Key) Then
                        insightsProperties.Add(kvp.Key, kvp.Value)
                    Else
                        insightsProperties(kvp.Key) = kvp.Value
                    End If
                End If
            Next
        End If

        Return insightsProperties
    End Function

    Private Shared Function AddTracesContext(insightsProperties As Dictionary(Of String, String)) As Dictionary(Of String, String)
        Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
        If _traceInfo Is Nothing Then Return insightsProperties

        Dim contentValue As String = ""
        If _traceInfo.ContainsKey(roTrace.o11yTraceKeys.ExtraContent.ToString()) Then
            Dim extraContext As Dictionary(Of Integer, String) = roJSONHelper.DeserializeNewtonSoft(_traceInfo(roTrace.o11yTraceKeys.ExtraContent.ToString()), GetType(Dictionary(Of Integer, String)))
            contentValue = String.Join($",{vbNewLine}", extraContext.Values)
        End If

        If insightsProperties.ContainsKey(roTrace.o11yTraceKeys.ExtraContent.ToString()) Then
            insightsProperties(roTrace.o11yTraceKeys.ExtraContent.ToString()) = $"[{contentValue}]"
        Else
            insightsProperties.Add(roTrace.o11yTraceKeys.ExtraContent.ToString(), $"[{contentValue}]")
        End If


        If Not _traceInfo.ContainsKey(roTrace.o11yTraceKeys.Id.ToString()) Then
            Dim traceID As String = System.Guid.NewGuid.ToString()
            _traceInfo.Add(roTrace.o11yTraceKeys.Id.ToString(), traceID)
            _traceInfo.Add(roTrace.o11yTraceKeys.GroupId.ToString(), traceID)
        End If


        If insightsProperties.ContainsKey(roTrace.o11yTraceKeys.Id.ToString()) Then
            insightsProperties(roTrace.o11yTraceKeys.Id.ToString()) = _traceInfo(roTrace.o11yTraceKeys.Id.ToString())
        Else
            insightsProperties.Add(roTrace.o11yTraceKeys.Id.ToString(), _traceInfo(roTrace.o11yTraceKeys.Id.ToString()))
        End If

        If insightsProperties.ContainsKey(roTrace.o11yTraceKeys.GroupId.ToString()) Then
            insightsProperties(roTrace.o11yTraceKeys.GroupId.ToString()) = _traceInfo(roTrace.o11yTraceKeys.GroupId.ToString())
        Else
            insightsProperties.Add(roTrace.o11yTraceKeys.GroupId.ToString(), _traceInfo(roTrace.o11yTraceKeys.GroupId.ToString()))
        End If

        Return insightsProperties
    End Function

End Class