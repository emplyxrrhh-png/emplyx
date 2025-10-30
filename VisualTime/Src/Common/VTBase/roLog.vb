Imports System.Diagnostics
Imports System.Globalization
Imports System.Threading
Imports System.Web
Imports Robotics.Base.DTOs

<Serializable()>
Public Class roLog

    Private Shared _logInstance As roLog = Nothing
    Private Shared _logCommsInstance As roLog = Nothing

#Region "Enumeraciones"

    Public Enum EventType
        roDebug = 0
        roInfo = 1
        roWarning = 2
        roError = 3
        roCritic = 4
        roDisabled = 5
    End Enum

    Public Enum ProcessTimeUnit
        milliseconds = 0
        seconds = 1
        minuted = 2
        hours = 3
        nonspecified = 4
    End Enum


    Public Enum o11yKeys
        ProcessTime
        ProcessTimeUnit
        SqlTime
        SqlOpen
        SqlUnit
        SqlCount
        ProcessStart
        Task
        Company
        Action
        SN
        Model
    End Enum

#End Region

    'Implementamos el patrón SINGLETON para asegurar que sólo mantengo una fichero de log para todos los procesos
    Private gComputerName As String

    Private oTelemetryClient As roTelemetryClient = Nothing

    Public Shared ReadOnly Property GetInstance(Optional ByVal ClassName As String = "") As roLog
        Get
            If (_logInstance Is Nothing) Then
                _logInstance = New roLog(ClassName)
            End If
            Return _logInstance
        End Get
    End Property

    Public Sub New(ByVal FileName As String, ByVal ClassName As String)
        Me.New(ClassName)
    End Sub

    Public Sub New(ByVal ClassName As String)
        Try
            oTelemetryClient = roTelemetryClient.GetInstance()
        Catch
            'do nothing 
        End Try

        gComputerName = Environment.MachineName
    End Sub





#Region "Log"

    Public Overridable Sub logMessage(ByVal type As EventType, ByVal logmsg As String, Optional customColumns As Dictionary(Of String, String) = Nothing)
        Try
            Dim gLogLevel As EventType = roConstants.GetDefaultLogLevel
            If gLogLevel = EventType.roDisabled Then Return

            If oTelemetryClient IsNot Nothing Then
                oTelemetryClient.LogMessage(type, logmsg, gLogLevel, gComputerName, customColumns)
            Else
                oTelemetryClient = roTelemetryClient.GetInstance()
            End If
        Catch
            'do nothing 
        End Try
    End Sub

    Public Overridable Sub logMessage(ByVal type As EventType, ByVal logmsg As String, ByVal Ex As Exception, Optional customColumns As Dictionary(Of String, String) = Nothing)
        Me.logMessage(type, logmsg & " Unexpected error: " & Ex.Message.Replace(vbCrLf, Chr(13)) & " " & Ex.StackTrace.Replace(vbCrLf, Chr(13)), customColumns)
    End Sub

    Public Overridable Sub logMessage(ByVal type As EventType, ByVal logmsg As String, ByVal Ex As System.Data.Common.DbException, Optional customColumns As Dictionary(Of String, String) = Nothing)

        Me.logMessage(type, logmsg & " DbException error: " & Ex.Message.Replace(vbCrLf, Chr(13)) & " " & Ex.StackTrace.Replace(vbCrLf, Chr(13)), customColumns)
    End Sub

    Public Overridable Sub logMessage(ByVal type As EventType, ByVal logmsg As String, ByVal Ex As System.Net.Sockets.SocketException, Optional customColumns As Dictionary(Of String, String) = Nothing)
        Me.logMessage(type, logmsg & " SocketException error: " & Ex.Message.Replace(vbCrLf, Chr(13)) & " " & Ex.StackTrace.Replace(vbCrLf, Chr(13)), customColumns)
    End Sub

    Public Sub logSystemMessage(ByVal type As EventType, ByVal logmsg As String, Optional ByVal Ex As Exception = Nothing, Optional forceWrite As Boolean = False, Optional customColumns As Dictionary(Of String, String) = Nothing)

        Try
            If Ex IsNot Nothing Then
                logmsg &= logmsg & " Unexpected error: " & Ex.Message.Replace(vbCrLf, Chr(13)) & " " & Ex.StackTrace.Replace(vbCrLf, Chr(13))
            End If

            If oTelemetryClient IsNot Nothing Then
                oTelemetryClient.LogSystemMessage(type, logmsg, forceWrite, customColumns)
            End If
        Catch
            'do nothing    
        End Try

    End Sub

#End Region

    Public Sub AddSqlProcessTime(processTime As Double, unit As ProcessTimeUnit)

        If roTypes.Any2Boolean(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace")) Then Return

        If unit = ProcessTimeUnit.nonspecified Then Return

        If unit <> ProcessTimeUnit.seconds Then
            processTime = ConvertToSeconds(processTime, unit)
        End If

        Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
        If _o11yInfo Is Nothing Then Return

        If _o11yInfo.ContainsKey(o11yKeys.SqlTime.ToString()) Then
            _o11yInfo(o11yKeys.SqlTime.ToString()) = (roTypes.Any2Double(_o11yInfo(o11yKeys.SqlTime.ToString()), CultureInfo.InvariantCulture) + processTime).ToString("F4", CultureInfo.InvariantCulture)
        Else
            _o11yInfo.Add(o11yKeys.SqlTime.ToString(), processTime.ToString("F4", CultureInfo.InvariantCulture))
        End If

        If Not _o11yInfo.ContainsKey(o11yKeys.SqlUnit.ToString()) Then
            _o11yInfo.Add(o11yKeys.SqlUnit.ToString(), unit.ToString())
        End If

        If _o11yInfo.ContainsKey(o11yKeys.SqlCount.ToString()) Then
            _o11yInfo(o11yKeys.SqlCount.ToString()) = (roTypes.Any2Integer(_o11yInfo(o11yKeys.SqlCount.ToString())) + 1).ToString()
        Else
            _o11yInfo.Add(o11yKeys.SqlCount.ToString(), 1)
        End If

        roTelemetryInfo.GetInstance().UpdateO11yInfo(_o11yInfo)
    End Sub

    Public Sub AddSqlOpenTime(timeToOpen As Double)

        If roTypes.Any2Boolean(Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_ExcludeFromTrace")) Then Return

        Dim _o11yInfo As Dictionary(Of String, String) = roTelemetryInfo.GetInstance().GetO11yInfo()
        If _o11yInfo Is Nothing Then Return

        If _o11yInfo.ContainsKey(o11yKeys.SqlOpen.ToString()) Then
            _o11yInfo(o11yKeys.SqlOpen.ToString()) = (roTypes.Any2Double(_o11yInfo(o11yKeys.SqlOpen.ToString()), CultureInfo.InvariantCulture) + timeToOpen).ToString("F4", CultureInfo.InvariantCulture)
        Else
            _o11yInfo.Add(o11yKeys.SqlOpen.ToString(), timeToOpen.ToString("F4", CultureInfo.InvariantCulture))
        End If

        roTelemetryInfo.GetInstance().UpdateO11yInfo(_o11yInfo)
    End Sub

    Private Shared Function ConvertToSeconds(value As Double, unit As ProcessTimeUnit) As Double
        Select Case unit
            Case ProcessTimeUnit.milliseconds
                Return value / 1000 ' Convertir de milisegundos a segundos
            Case ProcessTimeUnit.seconds
                Return value ' Ya está en segundos
            Case ProcessTimeUnit.minuted
                Return value * 60 ' Convertir de minutos a segundos
            Case ProcessTimeUnit.hours
                Return value * 3600 ' Convertir de horas a segundos
            Case Else
                Return 0
        End Select
    End Function

    Public Shared Function AddProcessTime(processTime As Double, unit As ProcessTimeUnit) As Dictionary(Of String, String)
        If Not [Enum].IsDefined(GetType(ProcessTimeUnit), unit) Then
            unit = ProcessTimeUnit.nonspecified
        End If
        Return New Dictionary(Of String, String) From {{o11yKeys.ProcessTime.ToString(), $"{processTime.ToString()}"}, {o11yKeys.ProcessTimeUnit.ToString(), $"{unit}"}}
    End Function
End Class
