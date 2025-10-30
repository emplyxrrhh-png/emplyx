Imports System.Diagnostics
Imports System.Globalization
Imports System.Threading
Imports System.Web
Imports Robotics.Base.DTOs

<Serializable()>
Public Class roTrace

    Private Shared _traceInstance As roTrace = Nothing

#Region "Enumeraciones"

    Public Enum TraceType
        roDebug = 0
        roInfo = 1
        roDisabled = 2
    End Enum

    Public Enum TraceResult
        Init = 0
        Ok = 1
        [Error] = 2
        NotExists = 3
        NotProcessed = 4
        NotConfigured = 5
        OkWithErrors = 6
    End Enum

    Public Enum o11yTraceKeys
        ElapsedProcessStart
        ElapsedSQLTime
        ElapsedSqlCount
        ExtraContent
        Id
        GroupId
        Result
        DescriptionMsg
    End Enum

#End Region

    'Implementamos el patrón SINGLETON para asegurar que sólo mantengo una fichero de log para todos los procesos
    Private gComputerName As String

    Private oTelemetryClient As roTelemetryClient = Nothing

    Public Shared ReadOnly Property GetInstance(Optional ByVal className As String = "") As roTrace
        Get
            If (_traceInstance Is Nothing) Then
                _traceInstance = New roTrace(className)
            End If
            Return _traceInstance
        End Get
    End Property

    Public Sub New(ByVal fileName As String, ByVal className As String)
        Me.New(className)
    End Sub

    Public Sub New(ByVal className As String)
        Try
            oTelemetryClient = roTelemetryClient.GetInstance()
        Catch
            'do nothing 
        End Try

        gComputerName = Environment.MachineName
    End Sub

    Public Overridable Sub TraceMessage(ByVal type As TraceType, ByVal result As TraceResult, Optional ByVal message As String = "")
        Try
            Dim gLogLevel As TraceType = roConstants.GetDefaultTraceLevel
            If gLogLevel = TraceType.roDisabled Then Return

            If result = TraceResult.Init Then Return

            If oTelemetryClient IsNot Nothing Then
                oTelemetryClient.TraceMessage(type, result, message, gLogLevel, gComputerName)
            Else
                oTelemetryClient = roTelemetryClient.GetInstance()
            End If
        Catch
            'do nothing 
        End Try
    End Sub


    Public Sub AddTraceDescription(ByVal message As String)
        Try
            If roTypes.Any2Boolean(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace")) Then Return


            Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
            If _traceInfo Is Nothing Then Return

            If _traceInfo.ContainsKey(o11yTraceKeys.DescriptionMsg.ToString()) Then
                _traceInfo(o11yTraceKeys.DescriptionMsg.ToString()) = $"{_traceInfo(o11yTraceKeys.DescriptionMsg.ToString())} {message}"
            Else
                _traceInfo.Add(o11yTraceKeys.DescriptionMsg.ToString(), message)
            End If

            roTelemetryInfo.GetInstance().UpdateTraceInfo(_traceInfo)
        Catch
            'do nothing 
        End Try
    End Sub

    Public Sub AddTraceEvent(ByVal message As String, Optional ByVal type As TraceType = TraceType.roDebug)
        Try
            If roTypes.Any2Boolean(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace")) Then Return

            Dim gLogLevel As TraceType = roConstants.GetDefaultTraceLevel
            If gLogLevel = TraceType.roDisabled Then Return

            Dim bolApplyLog As Boolean = (CInt(type) >= CInt(gLogLevel))

            If bolApplyLog Then
                Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()
                Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()

                Dim nowTicks As Long = DateTime.UtcNow.Ticks
                Dim processTicks As Long = nowTicks
                Dim oldSqlTime As Double = 0
                Dim sqlTime As Double = 0

                Dim sqlExecute As Integer = 0
                Dim oldSqlExecute As Integer = 0

                If _traceInfo.ContainsKey(o11yTraceKeys.ElapsedProcessStart.ToString()) Then
                    processTicks = roTypes.Any2Long(_traceInfo(o11yTraceKeys.ElapsedProcessStart.ToString()))
                Else
                    If _o11yInfo.ContainsKey(roLog.o11yKeys.ProcessStart.ToString()) Then
                        processTicks = roTypes.Any2Long(_o11yInfo(roLog.o11yKeys.ProcessStart.ToString()))
                    Else
                        processTicks = nowTicks
                    End If
                End If
                _traceInfo(o11yTraceKeys.ElapsedProcessStart.ToString()) = nowTicks.ToString()

                If _o11yInfo.ContainsKey(roLog.o11yKeys.SqlTime.ToString()) Then
                    sqlTime = roTypes.Any2Double(_o11yInfo(roLog.o11yKeys.SqlTime.ToString()), CultureInfo.InvariantCulture)
                End If

                If _traceInfo.ContainsKey(o11yTraceKeys.ElapsedSQLTime.ToString()) Then
                    oldSqlTime = roTypes.Any2Double(_traceInfo(o11yTraceKeys.ElapsedSQLTime.ToString()), CultureInfo.InvariantCulture)
                Else
                    oldSqlTime = sqlTime
                End If
                _traceInfo(o11yTraceKeys.ElapsedSQLTime.ToString()) = sqlTime.ToString(CultureInfo.InvariantCulture)


                If _o11yInfo.ContainsKey(roLog.o11yKeys.SqlCount.ToString()) Then
                    sqlExecute = roTypes.Any2Integer(_o11yInfo(roLog.o11yKeys.SqlCount.ToString()))
                End If


                If _traceInfo.ContainsKey(o11yTraceKeys.ElapsedSqlCount.ToString()) Then
                    oldSqlExecute = roTypes.Any2Long(_traceInfo(o11yTraceKeys.ElapsedSqlCount.ToString()))
                Else
                    oldSqlExecute = sqlExecute
                End If
                _traceInfo(o11yTraceKeys.ElapsedSqlCount.ToString()) = sqlExecute.ToString()


                Dim diferenciaTicks As Long = Math.Abs(nowTicks - CType(processTicks, Long))
                Dim diferenciaTiempo As TimeSpan = TimeSpan.FromTicks(diferenciaTicks)

                Dim extraContext As Dictionary(Of Integer, String)

                If _traceInfo.ContainsKey(o11yTraceKeys.ExtraContent.ToString()) Then
                    extraContext = roJSONHelper.DeserializeNewtonSoft(_traceInfo(o11yTraceKeys.ExtraContent.ToString()), GetType(Dictionary(Of Integer, String)))
                Else
                    extraContext = New Dictionary(Of Integer, String)
                End If

                Dim jsonObject = New With {
                    .Message = message,
                    .ProcessTime = Math.Round(diferenciaTiempo.TotalSeconds, 4),
                    .SqlTime = Math.Round((sqlTime - oldSqlTime), 4),
                    .SqlCount = (sqlExecute - oldSqlExecute)
                }

                extraContext.Add(extraContext.Count + 1, roJSONHelper.SerializeNewtonSoft(jsonObject))

                If _traceInfo.ContainsKey(o11yTraceKeys.ExtraContent.ToString()) Then
                    _traceInfo(o11yTraceKeys.ExtraContent.ToString()) = roJSONHelper.SerializeNewtonSoft(extraContext)
                Else
                    _traceInfo.Add(o11yTraceKeys.ExtraContent.ToString(), roJSONHelper.SerializeNewtonSoft(extraContext))
                End If

                roTelemetryInfo.GetInstance().UpdateTraceInfo(_traceInfo)
            End If

        Catch
            'do nothing 
        End Try
    End Sub

    Public Sub InitTraceEvent()
        Try
            If roTypes.Any2Boolean(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace")) Then Return

            Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
            Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()

            Dim nowTicks As Long = DateTime.UtcNow.Ticks
            Dim sqlTime As Double = 0
            Dim sqlExecute As Integer = 0

            _traceInfo(o11yTraceKeys.ElapsedProcessStart.ToString()) = nowTicks.ToString()

            If _o11yInfo.ContainsKey(roLog.o11yKeys.SqlTime.ToString()) Then
                sqlTime = roTypes.Any2Double(_o11yInfo(roLog.o11yKeys.SqlTime.ToString()).Replace(",", "."), CultureInfo.InvariantCulture)
            End If
            _traceInfo(o11yTraceKeys.ElapsedSQLTime.ToString()) = sqlTime.ToString(CultureInfo.InvariantCulture)

            If _o11yInfo.ContainsKey(roLog.o11yKeys.SqlCount.ToString()) Then
                sqlExecute = roTypes.Any2Integer(_o11yInfo(roLog.o11yKeys.SqlCount.ToString()))
            End If

            _traceInfo(o11yTraceKeys.ElapsedSqlCount.ToString()) = sqlExecute.ToString()


            roTelemetryInfo.GetInstance().UpdateTraceInfo(_traceInfo)
        Catch
            'do nothing 
        End Try
    End Sub

    Public Sub AddTraceInfo(taskId As String, actionName As String, companyName As String, Optional groupTraceId As String = "")
        Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
        Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()

        If _o11yInfo Is Nothing OrElse _traceInfo Is Nothing Then Return

        If Not _traceInfo.ContainsKey(o11yTraceKeys.Id.ToString()) Then
            _traceInfo.Add(o11yTraceKeys.Id.ToString(), Guid.NewGuid.ToString)
        End If

        If String.IsNullOrEmpty(groupTraceId) Then
            groupTraceId = _traceInfo(o11yTraceKeys.Id.ToString())
        End If

        If _traceInfo.ContainsKey(o11yTraceKeys.GroupId.ToString()) Then
            _traceInfo(o11yTraceKeys.GroupId.ToString()) = groupTraceId
        Else
            _traceInfo.Add(o11yTraceKeys.GroupId.ToString(), groupTraceId)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.Task.ToString()) Then
            _o11yInfo(roLog.o11yKeys.Task.ToString()) = taskId
        Else
            _o11yInfo.Add(roLog.o11yKeys.Task.ToString(), taskId)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.Action.ToString()) Then
            _o11yInfo(roLog.o11yKeys.Action.ToString()) = actionName
        Else
            _o11yInfo.Add(roLog.o11yKeys.Action.ToString(), actionName)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.Company.ToString()) Then
            _o11yInfo(roLog.o11yKeys.Company.ToString()) = companyName
        Else
            _o11yInfo.Add(roLog.o11yKeys.Company.ToString(), companyName)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.ProcessStart.ToString()) Then
            _o11yInfo(roLog.o11yKeys.ProcessStart.ToString()) = DateTime.UtcNow.Ticks
        Else
            _o11yInfo.Add(roLog.o11yKeys.ProcessStart.ToString(), DateTime.UtcNow.Ticks)
        End If

        roTelemetryInfo.GetInstance().UpdateO11yInfo(_o11yInfo)
        roTelemetryInfo.GetInstance().UpdateTraceInfo(_traceInfo)

    End Sub

    Public Function GetCurrentTraceGroup() As String
        Dim _traceInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetTraceInfo()

        If _traceInfo Is Nothing Then Return String.Empty

        If _traceInfo.ContainsKey(o11yTraceKeys.Id.ToString()) Then
            Return _traceInfo(o11yTraceKeys.Id.ToString())
        Else
            Return String.Empty
        End If
    End Function

    Public Sub AddTerminalInfo(serialNumber As String, model As String, companyName As String)
        Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
        If _o11yInfo Is Nothing Then Return

        If _o11yInfo.ContainsKey(roLog.o11yKeys.SN.ToString()) Then
            _o11yInfo(roLog.o11yKeys.SN.ToString()) = serialNumber
        Else
            _o11yInfo.Add(roLog.o11yKeys.SN.ToString(), serialNumber)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.Model.ToString()) Then
            _o11yInfo(roLog.o11yKeys.Model.ToString()) = model
        Else
            _o11yInfo.Add(roLog.o11yKeys.Model.ToString(), model)
        End If

        If _o11yInfo.ContainsKey(roLog.o11yKeys.Company.ToString()) Then
            _o11yInfo(roLog.o11yKeys.Company.ToString()) = companyName
        Else
            _o11yInfo.Add(roLog.o11yKeys.Company.ToString(), companyName)
        End If

        roTelemetryInfo.GetInstance().UpdateO11yInfo(_o11yInfo)
    End Sub

End Class
