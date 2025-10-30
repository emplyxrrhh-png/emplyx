Imports System.Linq
Imports System.Runtime.Caching
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Enum roConfigParameter
    email
    sms
    redis
    firebase
    keyvalut
    cegidid
    showaichatbot
    showuserpilot
    sqlsecurityversion
    accountbiometriccertificate
    uidpulse
End Enum

Public Class roConfigRepository
    Implements IroConfigRepository

    Private Const MasterDatabase As String = "robotics"
    Private Shared lockobject As New Object
    Private Shared memoryCache As MemoryCache = MemoryCache.Default

    Private _strCosmoDB As String = ""
    Private _strDBName As String = ""

    Private xMongoClient As MongoClient = Nothing
    Private xMongoDB As IMongoDatabase = Nothing

    Sub New()
        _strCosmoDB = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.ConnectionString"))
        _strDBName = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.DBName"))

    End Sub


    Public Shared Sub RebootCache()
        SyncLock lockobject

            Dim items As Array
            items = System.Enum.GetNames(GetType(roConfigParameter))
            Dim item As String
            For Each item In items
                If memoryCache.Contains("config_" & item) Then memoryCache.Remove("config_" & item)
            Next

        End SyncLock
    End Sub

    Private Sub InitMongoDBConnection()
        Try
            ' Acceso a la BBDD de CosmoDB MongoDB
            xMongoClient = New MongoClient(_strCosmoDB)
            If Not String.IsNullOrEmpty(_strDBName) Then
                xMongoDB = xMongoClient.GetDatabase(_strDBName)
            Else
                xMongoDB = xMongoClient.GetDatabase(MasterDatabase)
            End If
        Catch ex As Exception
            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "VTAzure::roServiceConfigurationRepository::InitMongoDBConnection::", ex)
            xMongoClient = Nothing
            xMongoDB = Nothing
        End Try
    End Sub

    Public Function GetConfigParameter(eParameter As roConfigParameter) As roAzureConfig Implements IroConfigRepository.GetConfigParameter
        Dim oConf As New roAzureConfig With {
                .id = eParameter.ToString,
                .value = ""
            }

        If memoryCache.Contains("config_" & eParameter.ToString()) Then
            oConf = CType(memoryCache("config_" & eParameter.ToString()), roAzureConfig)
        Else
            If xMongoClient Is Nothing Then InitMongoDBConnection()

            Try
                If xMongoClient IsNot Nothing Then
                    Dim xParams As IMongoCollection(Of roAzureConfig) = xMongoDB.GetCollection(Of roAzureConfig)("configuration")
                    oConf = xParams.Find(Function(comp) comp.id Is (eParameter.ToString)).ToList().FirstOrDefault()

                    memoryCache.Set("config_" & eParameter.ToString, oConf, DateTimeOffset.Now.AddDays(1))
                End If
            Catch ex As Exception
                VTBase.roLog.GetInstance().logSystemMessage(roLog.EventType.roError, "VTAzure::roConfigRepository::Could not query serviceconfiguration::" & eParameter.ToString, ex)
            End Try
        End If

        Return oConf
    End Function

    Public Function SaveConfigParameter(oParam As roAzureConfig) As Boolean Implements IroConfigRepository.SaveConfigParameter
        If xMongoClient IsNot Nothing Then
            Dim bSaved As Boolean = True

            Dim xParams As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("configuration")

            Dim update = Builders(Of BsonDocument).Update.Set(Of String)("value", oParam.value)
            Dim filter = Builders(Of BsonDocument).Filter.Eq(Of String)("_id", oParam.id)
            Dim options As New UpdateOptions() With {.IsUpsert = True}

            Dim updateResult As UpdateResult = xParams.UpdateOne(filter, update, options)

            If updateResult.IsAcknowledged Then
                memoryCache.Set("config_" & oParam.id, oParam, DateTimeOffset.Now.AddDays(1))
            End If

            Return updateResult.IsAcknowledged
        Else
            Return False
        End If
    End Function

End Class