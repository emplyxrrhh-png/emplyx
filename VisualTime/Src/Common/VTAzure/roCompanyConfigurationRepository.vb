Imports System.Linq
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports Robotics.Base.DTOs

Public Class roCompanyConfigurationRepository
    Implements IroCompanyConfigurationRepository
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
            VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "VTAzure::roCompanyConfigurationRepository::New::", ex)

            xMongoClient = Nothing
            xMongoDB = Nothing
        End Try

    End Sub


    Public Function GetCompanyConfiguration(IdCompany As String) As roCompanyConfiguration Implements IroCompanyConfigurationRepository.GetCompanyConfiguration
        If xMongoClient IsNot Nothing Then
            Dim xCompanies As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("companies")
            Dim filter = Builders(Of BsonDocument).Filter.Eq(Of String)("_id", IdCompany.Trim.ToLower)

            Dim doc As BsonDocument = xCompanies.FindSync(Of BsonDocument)(filter).FirstOrDefault()

            If doc IsNot Nothing Then
                Dim config As New roCompanyConfiguration()
                config.companyname = doc.GetValue("companyname", "").AsString
                config.dbconnectionstring = doc.GetValue("dbconnectionstring", "").AsString
                config.readdbconnectionstring = doc.GetValue("readdbconnectionstring", "").AsString
                config.Id = doc.GetValue("_id", "").AsString
                config.license = doc.GetValue("license", "").AsString
                config.netEngine = doc.GetValue("netEngine", False).ToBoolean()

                'If doc.Contains("timezone") Then
                '    config.timezone = doc.GetValue("timezone", "Romance Standard Time").AsString
                'Else
                '    config.timezone = "Romance Standard Time"
                'End If


                Return config
            End If
        End If

        Return New roCompanyConfiguration With {
                .companyname = "",
                .dbconnectionstring = "",
                .Id = "",
                .license = "",
                .netEngine = False
                }

    End Function

    Public Function GetCompanies() As roCompanyConfiguration() Implements IroCompanyConfigurationRepository.GetCompanies
        If xMongoClient IsNot Nothing Then
            Dim xCompanies As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("companies")
            Dim filter = Builders(Of BsonDocument).Filter.Ne(Of String)("dbconnectionstring", "")
            Dim docs = xCompanies.FindSync(Of BsonDocument)(filter).ToList()

            Dim resultList As New List(Of roCompanyConfiguration)()

            For Each doc As BsonDocument In docs
                Dim config As New roCompanyConfiguration()
                config.companyname = doc.GetValue("companyname", "").AsString
                config.dbconnectionstring = doc.GetValue("dbconnectionstring", "").AsString
                config.readdbconnectionstring = doc.GetValue("readdbconnectionstring", "").AsString
                config.Id = doc.GetValue("_id", "").AsString
                config.license = doc.GetValue("license", "").AsString
                config.netEngine = doc.GetValue("netEngine", False).ToBoolean()

                'If doc.Contains("timezone") Then
                '    config.timezone = doc.GetValue("timezone", "Romance Standard Time").AsString
                'Else
                '    config.timezone = "Romance Standard Time"
                'End If

                resultList.Add(config)
            Next

            Return resultList.ToArray()
        Else
            Return New roCompanyConfiguration() {}
        End If
    End Function

End Class