Imports System.Net.Sockets
Imports System.Threading
Imports Robotics.Base.DTOs
Imports StackExchange.Redis

Public Class roRedisManger

    Private Shared lastReconnectTicks As Long = DateTimeOffset.MinValue.UtcTicks
    Private Shared firstErrorTime As DateTimeOffset = DateTimeOffset.MinValue
    Private Shared previousErrorTime As DateTimeOffset = DateTimeOffset.MinValue
    Private Shared ReadOnly reconnectLock As Object = New Object()

    Public Shared ReadOnly Property ReconnectMinFrequency As TimeSpan
        Get
            Return TimeSpan.FromSeconds(60)
        End Get
    End Property

    Public Shared ReadOnly Property ReconnectErrorThreshold As TimeSpan
        Get
            Return TimeSpan.FromSeconds(30)
        End Get
    End Property

    Public Shared ReadOnly Property RetryMaxAttempts As Integer
        Get
            Return 5
        End Get
    End Property

    Private Shared lazyConnection As Lazy(Of ConnectionMultiplexer) = CreateConnection()

    Public Shared ReadOnly Property Connection As ConnectionMultiplexer
        Get
            Return lazyConnection.Value
        End Get
    End Property

    Private Shared Function CreateConnection() As Lazy(Of ConnectionMultiplexer)
        Return New Lazy(Of ConnectionMultiplexer)(Function()
                                                      Dim oConfigRepo As New Azure.roConfigRepository
                                                      Dim oRedisConf As roAzureConfig = oConfigRepo.GetConfigParameter(roConfigParameter.redis)
                                                      Return ConnectionMultiplexer.Connect(oRedisConf.value)
                                                  End Function)
    End Function

    Private Shared Sub CloseConnection(ByVal oldConnection As Lazy(Of ConnectionMultiplexer))
        If oldConnection Is Nothing Then Return

        Try
            oldConnection.Value.Close()
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Sub ForceReconnect()
        Dim utcNow = DateTimeOffset.UtcNow
        Dim previousTicks As Long = Interlocked.Read(lastReconnectTicks)
        Dim previousReconnectTime = New DateTimeOffset(previousTicks, TimeSpan.Zero)
        Dim elapsedSinceLastReconnect As TimeSpan = utcNow - previousReconnectTime
        If elapsedSinceLastReconnect < ReconnectMinFrequency Then Return

        SyncLock reconnectLock
            utcNow = DateTimeOffset.UtcNow
            elapsedSinceLastReconnect = utcNow - previousReconnectTime

            If firstErrorTime = DateTimeOffset.MinValue Then
                firstErrorTime = utcNow
                previousErrorTime = utcNow
                Return
            End If

            If elapsedSinceLastReconnect < ReconnectMinFrequency Then Return
            Dim elapsedSinceFirstError As TimeSpan = utcNow - firstErrorTime
            Dim elapsedSinceMostRecentError As TimeSpan = utcNow - previousErrorTime
            Dim shouldReconnect As Boolean = elapsedSinceFirstError >= ReconnectErrorThreshold AndAlso elapsedSinceMostRecentError <= ReconnectErrorThreshold
            previousErrorTime = utcNow
            If Not shouldReconnect Then Return
            firstErrorTime = DateTimeOffset.MinValue
            previousErrorTime = DateTimeOffset.MinValue
            Dim oldConnection As Lazy(Of ConnectionMultiplexer) = lazyConnection
            CloseConnection(oldConnection)
            lazyConnection = CreateConnection()
            Interlocked.Exchange(lastReconnectTicks, utcNow.UtcTicks)
        End SyncLock
    End Sub

    Private Shared Function BasicRetry(Of T)(ByVal func As Func(Of T)) As T
        Dim reconnectRetry As Integer = 0
        Dim disposedRetry As Integer = 0

        While True

            Try
                Return func()
            Catch ex As Exception When TypeOf ex Is RedisConnectionException OrElse TypeOf ex Is SocketException
                reconnectRetry += 1
                If reconnectRetry > RetryMaxAttempts Then Throw
                ForceReconnect()
            Catch ex As ObjectDisposedException
                disposedRetry += 1
                If disposedRetry > RetryMaxAttempts Then Throw
            End Try
        End While
    End Function

    Public Shared Function GetDatabase() As IDatabase
        Return BasicRetry(Function() Connection.GetDatabase())
    End Function

    Public Shared Function GetEndPoints() As System.Net.EndPoint()
        Return BasicRetry(Function() Connection.GetEndPoints())
    End Function

    Public Shared Function GetServer(ByVal oEndpoint As Net.EndPoint) As IServer
        Return BasicRetry(Function() Connection.GetServer(oEndpoint))
    End Function

    Public Shared Function GetGlobalDatetimeResetCache() As DateTime
        Dim resetCacheDate As DateTime = DateTime.MinValue

        Try
            If Not roRedisManger.GetDatabase().KeyExists("VT_GC_ResetCache") Then
                resetCacheDate = DateTime.Now.AddDays(-1)
                roRedisManger.GetDatabase().StringSet("VT_GC_ResetCache", resetCacheDate.ToString("yyyyMMddHHmmss"))
            End If

            resetCacheDate = DateTime.ParseExact(roRedisManger.GetDatabase().StringGet("VT_GC_ResetCache").ToString, "yyyyMMddHHmmss", Nothing)
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roRedisManager::Error:Could not retrieve cahce status", ex)
            resetCacheDate = DateTime.Now.AddDays(-1)
        End Try

        Return resetCacheDate
    End Function

End Class