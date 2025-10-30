Imports System.Linq
Imports System.Runtime.Caching
Imports MongoDB.Driver
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class roServiceConfigurationRepository
    Implements IroServiceConfigurationRepository

    Private Const MasterDatabase As String = "robotics"
    Private Shared lockobject As New Object
    Private Shared memoryCache As MemoryCache = MemoryCache.Default

    Private xMongoClient As MongoClient = Nothing
    Private xMongoDB As IMongoDatabase = Nothing

    Private strConnectionString As String
    Private strDBName As String
    Private sServiceConfigurationType As roLiveQueueTypes

#Region "Declarations - Constructor"

    Public Sub New(ByVal _QueueType As roLiveQueueTypes)
        Me.sServiceConfigurationType = _QueueType
        Me.strConnectionString = roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.ConnectionString"))
        Me.strDBName = roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.DBName"))

    End Sub

    Public Shared Sub RebootCache()
        SyncLock lockobject

            Dim items As Array
            items = System.Enum.GetNames(GetType(roLiveQueueTypes))
            Dim item As String
            For Each item In items
                If memoryCache.Contains(item) Then memoryCache.Remove(item)
            Next

        End SyncLock
    End Sub

#End Region

#Region "Methods"

    Private Sub InitMongoDBConnection()
        Try
            ' Acceso a la BBDD de CosmoDB MongoDB
            xMongoClient = New MongoClient(Me.strConnectionString)
            If Not String.IsNullOrEmpty(strDBName) Then
                xMongoDB = xMongoClient.GetDatabase(Me.strDBName)
            Else
                xMongoDB = xMongoClient.GetDatabase(MasterDatabase)
            End If
        Catch ex As Exception
            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "VTAzure::roServiceConfigurationRepository::InitMongoDBConnection::", ex)
            xMongoClient = Nothing
            xMongoDB = Nothing
        End Try
    End Sub

    Public Function GetServiceConfiguration() As roServiceConfiguration Implements IroServiceConfigurationRepository.GetServiceConfiguration

        Dim oConf As New roServiceConfiguration With {
                            .Id = sServiceConfigurationType.ToString.ToLower,
                            .servicebusconnectionstring = String.Empty,
                            .servicebusqueuename = String.Empty,
                            .storageconnectionstring = String.Empty,
                            .storageblobcontainer = String.Empty,
                            .loglevel = CInt(roLog.EventType.roInfo),
                            .tracelevel = CInt(roTrace.TraceType.roInfo)
                        }

        If memoryCache.Contains(sServiceConfigurationType.ToString()) Then
            oConf = CType(memoryCache(sServiceConfigurationType.ToString()), roServiceConfiguration)
        Else
            If xMongoClient Is Nothing Then InitMongoDBConnection()

            Try
                If xMongoClient IsNot Nothing Then
                    Dim xServiceconfiguration As IMongoCollection(Of roServiceConfiguration) = xMongoDB.GetCollection(Of roServiceConfiguration)("serviceconfiguration")
                    oConf = xServiceconfiguration.Find(Function(comp) comp.Id Is sServiceConfigurationType.ToString.ToLower).ToList().FirstOrDefault()

                    If oConf Is Nothing Then
                        oConf = New roServiceConfiguration With {
                            .Id = sServiceConfigurationType.ToString.ToLower,
                            .servicebusconnectionstring = String.Empty,
                            .servicebusqueuename = String.Empty,
                            .storageconnectionstring = String.Empty,
                            .storageblobcontainer = String.Empty,
                            .loglevel = CInt(roLog.EventType.roInfo),
                            .tracelevel = CInt(roTrace.TraceType.roInfo)
                        }

                        If oConf.Id = "vtlive" OrElse oConf.Id = "vtliveapi" OrElse oConf.Id = "terminals" OrElse oConf.Id = "visits" OrElse oConf.Id = "vtportal" Then
                            Try
                                xServiceconfiguration.InsertOne(oConf)
                            Catch ex As Exception
                                roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "VTAzure::roServiceConfigurationRepository::Error adding missing configuration to serviceconfiguration::" & sServiceConfigurationType.ToString, ex)
                            End Try
                        End If
                    End If

                    memoryCache.Set(sServiceConfigurationType.ToString, oConf, DateTimeOffset.Now.AddDays(1))
                End If
            Catch ex As Exception
                VTBase.roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "VTAzure::roServiceConfigurationRepository::Could not query serviceconfiguration::" & sServiceConfigurationType.ToString, ex)
            End Try

        End If

        Return oConf
    End Function

#End Region

End Class