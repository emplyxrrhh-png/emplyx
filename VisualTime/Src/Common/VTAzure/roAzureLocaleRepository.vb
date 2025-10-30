Imports System.Linq
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports Robotics.Base.DTOs

Public Class roAzureLocaleRepository
    Implements IroAzureLocaleRepository
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

    Public Function GetLocaleByKey(key As String) As roAzureLocale Implements IroAzureLocaleRepository.GetLocaleByKey
        If xMongoClient IsNot Nothing Then
            Dim xLocales As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("locales")
            Dim filter = Builders(Of BsonDocument).Filter.Eq(Of String)("key", key.Trim.ToLower)

            Dim doc As BsonDocument = xLocales.FindSync(Of BsonDocument)(filter).FirstOrDefault()

            If doc IsNot Nothing Then
                Dim config As New roAzureLocale() With {
                    .id = doc.GetValue("_id", -1).AsInt32,
                    .key = doc.GetValue("key", "").AsString,
                    .culture = doc.GetValue("culture", "").AsString,
                    .parameters = doc.GetValue("parameters", "").AsString
                }
                Return config
            End If
        End If

        Return New roAzureLocale With {
                    .id = 0,
                    .key = "ESP",
                    .culture = "es-ES",
                    .parameters = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ExtLanguage"" type=""8"">sp</Item><Item key=""ExtDatePickerFormat"" type=""8"">d/m/Y</Item><Item key=""ExtDatePickerStartDay"" type=""8"">1</Item></roCollection>"
                }

    End Function

    Public Function GetLocaleById(id As Integer) As roAzureLocale Implements IroAzureLocaleRepository.GetLocaleById
        If xMongoClient IsNot Nothing Then
            Dim xLocales As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("locales")
            Dim filter = Builders(Of BsonDocument).Filter.Eq(Of Integer)("_id", id)

            Dim doc As BsonDocument = xLocales.FindSync(Of BsonDocument)(filter).FirstOrDefault()

            If doc IsNot Nothing Then
                Dim config As New roAzureLocale() With {
                    .id = doc.GetValue("_id", -1).AsInt32,
                    .key = doc.GetValue("key", "").AsString,
                    .culture = doc.GetValue("culture", "").AsString,
                    .parameters = doc.GetValue("parameters", "").AsString
                }
                Return config
            End If
        End If

        Return New roAzureLocale With {
                    .id = 0,
                    .key = "ESP",
                    .culture = "es-ES",
                    .parameters = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ExtLanguage"" type=""8"">sp</Item><Item key=""ExtDatePickerFormat"" type=""8"">d/m/Y</Item><Item key=""ExtDatePickerStartDay"" type=""8"">1</Item></roCollection>"
                }

    End Function

    Public Function GetLocales() As roAzureLocale() Implements IroAzureLocaleRepository.GetLocales
        If xMongoClient IsNot Nothing Then
            Dim xLocales As IMongoCollection(Of BsonDocument) = xMongoDB.GetCollection(Of BsonDocument)("locales")
            Dim filter = Builders(Of BsonDocument).Filter.Eq(Of Integer)("enabled", 1)
            Dim docs = xLocales.FindSync(Of BsonDocument)(filter).ToList()

            Dim resultList As New List(Of roAzureLocale)()

            For Each doc As BsonDocument In docs
                Dim config As New roAzureLocale() With {
                    .id = doc.GetValue("_id", -1).AsInt32,
                    .key = doc.GetValue("key", "").AsString,
                    .culture = doc.GetValue("culture", "").AsString,
                    .parameters = doc.GetValue("parameters", "").AsString
                }

                resultList.Add(config)
            Next

            Return resultList.ToArray()
        Else
            Return New roAzureLocale() {}
        End If
    End Function

End Class