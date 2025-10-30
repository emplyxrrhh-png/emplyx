Imports System.Linq
Imports Azure.Messaging.ServiceBus.Administration
Imports MongoDB.Bson
Imports MongoDB.Driver
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class roAzureTerminalRepository
    Implements IroTerminalRepository
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
            xMongoClient = Nothing
            xMongoDB = Nothing
        End Try

    End Sub

    Public Function GetTerminals() As roTerminalRegister() Implements IroTerminalRepository.GetTerminals
        If xMongoClient IsNot Nothing Then
            Dim xTerminals As IMongoCollection(Of roTerminalRegister) = xMongoDB.GetCollection(Of roCompanyConfiguration)("terminals")
            Dim result As roTerminalRegister() = xTerminals.Find(Function(comp) comp.Id IsNot String.Empty).ToList().ToArray

            Return result
        Else
            Return New roTerminalRegister() {}
        End If
    End Function

    Public Function GetTerminalCompanyConfiguration(TerminalSerialNumber As String) As roTerminalRegister Implements IroTerminalRepository.GetTerminalCompanyConfiguration
        If xMongoClient IsNot Nothing Then
            Dim result As roTerminalRegister = Nothing
            Dim xTerminals As IMongoCollection(Of roTerminalRegister) = xMongoDB.GetCollection(Of roTerminalRegister)("terminals")
            Dim terminalResult As roTerminalRegister = xTerminals.Find(Function(doc) doc.Id Is TerminalSerialNumber).ToList().FirstOrDefault()

            If Not terminalResult Is Nothing Then
                Dim xCompanies As IMongoCollection(Of roCompanyConfiguration) = xMongoDB.GetCollection(Of roCompanyConfiguration)("companies")
                If xCompanies.Find(Function(comp) comp.Id Is terminalResult.companyname).ToList().Count = 1 Then
                    Return terminalResult
                End If
            End If
            ' Si llego aquí o no encontré terminal registrado, o la empresa configurada no existe
            Return New roTerminalRegister With {
                .companyname = "",
                .model = "",
                .Id = ""
            }
        Else
            Return New roTerminalRegister With {
                    .companyname = "",
                    .model = "",
                    .Id = ""
                }
        End If
    End Function

    Public Function AddTerminalToCompany(TerminalSerialNumber As String, CompanyName As String, TerminalModel As String) As String Implements IroTerminalRepository.AddTerminalToCompany
        Dim strResult As String = String.Empty
        If xMongoClient IsNot Nothing Then
            Dim result As New roTerminalRegister With {
                .companyname = CompanyName,
                .Id = TerminalSerialNumber,
                .model = TerminalModel,
                .enabled = True
                }
            Dim xTerminals As IMongoCollection(Of roTerminalRegister) = xMongoDB.GetCollection(Of roTerminalRegister)("terminals")

            Dim terminalAlreadyExists As Boolean = False
            Dim tmpTerminal As roTerminalRegister = xTerminals.Find(Function(doc) doc.Id Is TerminalSerialNumber).ToList().FirstOrDefault()
            If tmpTerminal IsNot Nothing Then
                terminalAlreadyExists = True
                strResult = "El terminal indicado ya existe en la empresa:" & tmpTerminal.companyname
            Else
                Dim xCompanies As IMongoCollection(Of roCompanyConfiguration) = xMongoDB.GetCollection(Of roCompanyConfiguration)("companies")
                If xCompanies.Find(Function(comp) comp.Id Is CompanyName).ToList().Count = 1 Then
                    Try
                        xTerminals.InsertOne(result)
                        strResult = "OK"
                    Catch ex As Exception
                        strResult = "No se ha podido insertar en la tabla de terminales"
                    End Try
                Else
                    strResult = "No existe la empresa indicada"
                End If
            End If
        Else
            strResult = "Conexión con mongoDB perdida"
        End If

        If strResult <> "OK" Then
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roAzureTerminal::" & strResult)
        End If

        Return strResult
    End Function

    Public Function DeleteTerminalFromCompany(TerminalSerialNumber As String, CompanyName As String) As String Implements IroTerminalRepository.DeleteTerminalFromCompany
        Dim strResult As String = String.Empty
        If xMongoClient IsNot Nothing Then
            Dim bCompanyFound = False
            Dim xTerminals As IMongoCollection(Of roTerminalRegister) = xMongoDB.GetCollection(Of roTerminalRegister)("terminals")

            Dim terminalAlreadyExists As Boolean = False

            Dim filterId = Builders(Of roTerminalRegister).Filter.Eq(Of String)("_id", TerminalSerialNumber)
            Dim filterCompany = Builders(Of roTerminalRegister).Filter.Eq(Of String)("companyname", CompanyName.ToLower)
            Dim fullFilter As FilterDefinition(Of roTerminalRegister) = Builders(Of roTerminalRegister).Filter.And({filterId, filterCompany})

            Dim tmpTerminal As roTerminalRegister = xTerminals.Find(fullFilter).ToList().FirstOrDefault '.Find(fullFilter).FirstOrDefault()
            If tmpTerminal IsNot Nothing Then
                Dim xCompanies As IMongoCollection(Of roCompanyConfiguration) = xMongoDB.GetCollection(Of roCompanyConfiguration)("companies")
                If xCompanies.Find(Function(comp) comp.Id Is CompanyName).ToList().Count = 1 Then
                    Try
                        If tmpTerminal.companyname.ToLower = CompanyName.ToLower Then

                            xTerminals.DeleteOne(fullFilter)
                            strResult = "OK"
                        Else
                            strResult = "No se ha podido borrar el terminal"
                        End If
                    Catch ex As Exception
                        strResult = "No se ha podido borrar el terminal"
                    End Try
                Else
                    strResult = "No existe la empresa indicada"
                End If
            Else
                strResult = "El terminal indicado no existe en la empresa:" & CompanyName
            End If
        Else
            strResult = "Conexión con mongoDB perdida"
        End If

        If strResult <> "OK" Then
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roAzureTerminal::" & strResult)
        End If

        Return strResult
    End Function

    Public Function UpdateTerminalToCompany(TerminalSerialNumber As String, CompanyName As String, enabled As Boolean) As String Implements IroTerminalRepository.UpdateTerminalToCompany
        Dim strResult As String = String.Empty
        If xMongoClient IsNot Nothing Then
            Dim xTerminals As IMongoCollection(Of roTerminalRegister) = xMongoDB.GetCollection(Of roTerminalRegister)("terminals")

            Dim terminalAlreadyExists As Boolean = False
            Dim tmpTerminal As roTerminalRegister = xTerminals.Find(Function(doc) doc.Id Is TerminalSerialNumber).ToList().FirstOrDefault()
            If tmpTerminal IsNot Nothing Then
                terminalAlreadyExists = True
                Dim xCompanies As IMongoCollection(Of roCompanyConfiguration) = xMongoDB.GetCollection(Of roCompanyConfiguration)("companies")
                If xCompanies.Find(Function(comp) comp.Id Is CompanyName).ToList().Count = 1 Then
                    Try
                        If tmpTerminal.companyname.ToLower = CompanyName.ToLower Then
                            ' Filtro para localizar el documento (por ID, por ejemplo)
                            Dim oParam As New roAzureConfig With {
                                .id = TerminalSerialNumber,
                                .value = enabled.ToString
                            }
                            Dim filter = Builders(Of roTerminalRegister).Filter.Eq(Of String)("_id", oParam.id)
                            Dim update = Builders(Of roTerminalRegister).Update.Set(Of Boolean)("enabled", roTypes.Any2Boolean(oParam.value))
                            Dim options As New UpdateOptions() With {.IsUpsert = False}

                            Dim result = xTerminals.UpdateOne(filter, update, options)
                            strResult = "OK"
                        Else
                            strResult = "No se ha podido actualizar el terminal"
                        End If
                    Catch ex As Exception
                        strResult = "No se ha podido actualizar el terminal"
                    End Try
                Else
                    strResult = "No existe la empresa indicada"
                End If
            Else
                strResult = "El terminal indicado no existe en la empresa:" & CompanyName
            End If
        Else
            strResult = "Conexión con mongoDB perdida"
        End If

        If strResult <> "OK" Then
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roAzureTerminal::" & strResult)
        End If

        Return strResult
    End Function

End Class