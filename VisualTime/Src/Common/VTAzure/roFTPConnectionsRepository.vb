Imports System.Linq
Imports MongoDB.Driver
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class roFTPConnectionsRepository
    Implements IroFTPConnectionsRepository
    Private xMongoClient As MongoClient = Nothing
    Private xMongoDB As IMongoDatabase = Nothing

    Sub New()
        Dim strCosmoDB As String = ""
        Dim strDBName As String = ""
        ' O lo recupera de config o esta hardcoded
        Try
            strCosmoDB = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.ConnectionString"))
            strDBName = VTBase.roTypes.Any2String(VTBase.roConstants.GetConfigurationParameter("CosmosDB.DBName"))

            xMongoClient = New MongoClient(strCosmoDB)
            xMongoDB = xMongoClient.GetDatabase(strDBName)
        Catch ex As Exception
            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "VTAzure::roFTPConnectionsRepository::New::", ex)

            xMongoClient = Nothing
            xMongoDB = Nothing
        End Try

    End Sub

    Public Function GetFTPConnection(ByVal IdCompany As String) As roFTPConnection Implements IroFTPConnectionsRepository.GetFTPConnection
        Try
            If xMongoDB IsNot Nothing Then
                Dim xConnections As IMongoCollection(Of roFTPConnection) = xMongoDB.GetCollection(Of roFTPConnection)("ftp_connections")
                Dim ftpConnection As roFTPConnection = xConnections.Find(Function(conn) conn.Id = IdCompany.ToLower()).FirstOrDefault()
                Return ftpConnection
            Else
                Return New roFTPConnection("", New FTPInfo("", "", "", "", ""), "", "", "")
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "VTAzure::roFTPConnectionsRepository::GetFTPConnection::", ex)
            xMongoClient = Nothing
            xMongoDB = Nothing
            Return New roFTPConnection("", New FTPInfo("", "", "", "", ""), "", "", "")
        End Try

    End Function

    Public Function GetConnections() As List(Of roFTPConnection) Implements IroFTPConnectionsRepository.GetConnections
        Try

            If xMongoClient IsNot Nothing Then
                Dim xFTPConfs As IMongoCollection(Of roFTPConnection) = xMongoDB.GetCollection(Of roFTPConnection)("ftp_connections")
                Dim result As roFTPConnection() = xFTPConfs.Find(Function(comp) comp.Id IsNot String.Empty).ToList().ToArray

                Return result.ToList
            Else
                Return New List(Of roFTPConnection)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "VTAzure::roFTPConnectionsRepository::GetConnections::", ex)
            xMongoClient = Nothing
            xMongoDB = Nothing
            Return New List(Of roFTPConnection)()
        End Try
    End Function

End Class