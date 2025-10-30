Imports System.ComponentModel
Imports Robotics.Base
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Datalink
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Documents")>
    <CollectionDefinition("Documents", DisableParallelization:=True)>
    <Category("Documents")>
    Public Class DocumentsTest

        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperCommon As CommonHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperDatalink As DatalinkHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperAzure As AzureHelper
        Private ReadOnly helperPassport As PassportHelper

        Sub New()
            helperAdvancedParameters = New AdvancedParametersHelper
            helperCommon = New CommonHelper
            helperBusiness = New BusinessHelper
            helperDatalayer = New DatalayerHelper
            helperDatalink = New DatalinkHelper
            helperEmployee = New EmployeeHelper
            helperAzure = New AzureHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should decrypt document with license key")>
        Sub ShouldDecryptDocumentWithLicenseKey()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperCommon.InitLicense()
                Dim content As Byte() = {1, 2, 3, 4, 5, 6, 7, 8}
                Dim encriptedContent As Byte() = Robotics.VTBase.Extensions.roEncrypt.Encrypt(content)

                'Act
                Dim decriptedContent As Byte() = roEncrypt.Decrypt(encriptedContent)

                'Assert
                Assert.Equal(content, decriptedContent)
            End Using
        End Sub

        <Fact(DisplayName:="Should decrypt document with advanced parameter")>
        Sub ShouldDecryptDocumentWithAdvancedParameter()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim content As Byte() = {1, 2, 3, 4, 5, 6, 7, 8}
                helperCommon.CryptographyHelper("encrypt", "oldKey", "newKey")

                Dim encriptedContent As Byte() = Robotics.VTBase.Extensions.roEncrypt.Encrypt(content)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CustomDecryptKey", "oldKey"}})
                'Act
                helperCommon.Action = "decrypt"
                Dim decriptedContent As Byte() = roEncrypt.Decrypt(encriptedContent)

                'Assert
                Assert.Equal(content, decriptedContent)
            End Using
        End Sub

        <Fact(DisplayName:="Should decrypt document with empty key")>
        Sub ShouldDecryptDocumentWithEmptyKey()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim content As Byte() = {1, 2, 3, 4, 5, 6, 7, 8}
                helperCommon.CryptographyHelper("encrypt", String.Empty, "newKey")

                Dim encriptedContent As Byte() = Robotics.VTBase.Extensions.roEncrypt.Encrypt(content)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CustomDecryptKey", Nothing}})

                'Act
                helperCommon.Action = "decrypt"
                Dim decriptedContent As Byte() = roEncrypt.Decrypt(encriptedContent)

                'Assert
                Assert.Equal(content, decriptedContent)
            End Using
        End Sub

        <Fact(DisplayName:="Should Get All Documents If Parameters Are Not Setted And Are Less Than 10")>
        Sub ShouldGetAllDocumentsIfParametersAreNotSettedAndAreLessThan10()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                helperDatalink.isEmployeeNewStub(2)
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
    New Object() {"Doc2", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
    New Object() {"Doc3", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}
                                                                                                            }
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.Equal(3, documents.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Get 10 Documents If Parameters Are Not Setted And Are More Than 10")>
        Sub ShouldGet10DocumentsIfParametersAreNotSettedAndAreMoreThan10()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim queryContainsTop10 As Boolean = False
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalink.isEmployeeNewStub(2)
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.Contains("TOP 10") Then
                                                                                                            queryContainsTop10 = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc2", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc3", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc4", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc5", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc6", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc7", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc8", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc9", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"},
                                                                                                                New Object() {"Doc10", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}
                                                                                                                }
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(queryContainsTop10, "The query does not contain TOP 10")
                Assert.Equal(10, documents.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Get Documents Ordered By LastStatusChange Asc")>
        Sub ShouldGetDocumentsOrderedByLastStatusChangeAsc()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim queryOrder As Boolean = False
                helperDatalink.isEmployeeNewStub(2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("order by lastupdatetimestamp asc") Then
                                                                                                            queryOrder = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)},
    New Object() {"Doc2", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)},
    New Object() {"Doc3", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}

                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(queryOrder, "The query is not ordered")

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return BioCertificate Documents")>
        Sub ShouldNotReturnBioCertificateDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterBiocertificateDocuments As Boolean = False
                helperDatalink.isEmployeeNewStub(2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(dt.name) <> 'biocertificate'") Then
                                                                                                            filterBiocertificateDocuments = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterBiocertificateDocuments, "Biocertificate documents are not filtered")

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return Communique Documents")>
        Sub ShouldNotReturnCommuniqueDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterCommuniqueDocuments As Boolean = False
                helperDatalink.isEmployeeNewStub(2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idcommunique is null") Then
                                                                                                            filterCommuniqueDocuments = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterCommuniqueDocuments, "Communique documents are not filtered")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee Documents If Type Is Employee")>
        Sub ShouldReturnEmployeeDocumentsIfTypeIsEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterEmployeeDocuments As Boolean = False
                helperDatalink.isEmployeeNewStub(2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idemployee is not null") Then
                                                                                                            filterEmployeeDocuments = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterEmployeeDocuments, "Company documents are not filtered")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Company Documents If Type Is Company")>
        Sub ShouldReturnCompanyDocumentsIfTypeIsCompany()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterCompanyDocuments As Boolean = False
                helperDatalink.isEmployeeNewStub(2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idcompany is not null") Then
                                                                                                            filterCompanyDocuments = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterCompanyDocuments, "Employee documents are not filtered")

            End Using

        End Sub

        <Fact(DisplayName:="Should Filter By Title If Title Is Setted")>
        Sub ShouldFilterByTitleIfTitleIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByTitle As Boolean = False
                Dim title As String = "Doc1"
                helperDatalink.isEmployeeNewStub(2)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(title) like '%" & title.ToLower() & "%'") Then
                                                                                                            filterByTitle = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperAzure.GetDocumentFile()
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Title = title
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTitle, "Documents are not filtered by title")

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Filter By Title If Title Is Not Setted")>
        Sub ShouldNotFilterByTitleIfTitleIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByTitle As Boolean = False
                Dim title As String = "Doc1"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(title) like '%" & title.ToLower() & "%'") Then
                                                                                                            filterByTitle = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTitle, "Documents are filtered by title")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return DocumentTypeMustBeSetted Error If DocumentType Is Not Setted")>
        Sub ShouldReturnDocumentTypeMustBeSettedErrorIfDocumentTypeIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByTitle As Boolean = False
                Dim employee As String = "1"
                helperDatalink.isEmployeeNewStub(2)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.False(ret)
                Assert.True(ReturnCode._DocumentTypeMustBeSetted = returnCode, "Document type must be setted")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return InvalidDocumentType Error If DocumentType Is Invalid")>
        Sub ShouldReturnInvalidDocumentTypeIfDocumentTypeIsInvalid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                'Arrange
                Dim filterByTitle As Boolean = False
                Dim documentType As String = "invalidType"

                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = documentType
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.False(ret)
                Assert.True(ReturnCode._InvalidDocumentType = returnCode, "Document type must be setted")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Documents Of One Employee If Unique Employee ID Is Setted")>
        Sub ShouldReturnDocumentsOfOneEmployeeIfUniqueEmployeeIDIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByEmployee As Boolean = False
                Dim uniqueEmployeeID As String = "1"
                helperDatalink.isEmployeeNewStub()

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in( 1)") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = uniqueEmployeeID
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Documents Of Multiple Employee If Multiple Unique Employee IDs Are Setted")>
        Sub ShouldReturnDocumentsOfMultipleEmployeeIfMultipleUniqueEmployeeIDsAreSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByEmployee As Boolean = False
                Dim uniqueEmployeeID As String = "1;2"
                helperEmployee.GetIDEmployeesFromUserFieldValueStub()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in( 1,2)") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub()
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = uniqueEmployeeID
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Documents Of All Employee If Unique Employee IDs And NIF Are Not Setted")>
        Sub ShouldReturnDocumentsOfAllEmployeeIfUniqueEmployeeIDsAndNIFAreNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim filterByEmployee As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in( 1,2)") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Company Documents If Company Is Setted")>
        Sub ShouldReturnCompanyDocumentsIfCompanyIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim companyName As String = "companyName"
                Dim filterByCompany As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(g.fullgroupname) like '" & companyName.ToLower() & "%'") Then
                                                                                                            filterByCompany = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                documentCriteria.Company = companyName
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByCompany)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents If Company Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfCompanyIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim companyName As String = "companyName"
                Dim filterByCompany As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(g.name) like '%" & companyName.ToLower() & "%'") Then
                                                                                                            filterByCompany = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByCompany)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Documents Updated After Timestamp If Timestamp Is Setted")>
        Sub ShouldReturnDocumentsUpdatedAfterTimestampIfTimestampIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("laststatuschange >= " & Robotics.VTBase.roTypes.Any2Time(timestamp.ToString()).SQLDateTime.ToLower()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Timestamp = timestamp
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTimestamp)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents If Timestamp Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfTimestampIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("laststatuschange >= " & timestamp.ToString()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTimestamp)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents Signed After Timestamp If Update Type Is Signed And Timestamp Is Setted")>
        Sub ShouldReturnAllDocumentsSignedAfterTimestampIfUpdateTypeIsSignedAndTimestampIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("signdate >= " & Robotics.VTBase.roTypes.Any2Time(timestamp.ToString()).SQLDateTime.ToLower()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "signdate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Timestamp = timestamp
                documentCriteria.UpdateType = "Signed"
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTimestamp)

            End Using

        End Sub
        <Fact(DisplayName:="Should Return All Documents If Update Type Is Signed And Timestamp Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfUpdateTypeIsSignedAndTimestampIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("signdate >= " & timestamp.ToString()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "Signed"
                documentCriteria.UniqueEmployeeID = "1"
                ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTimestamp)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents Whose Status Has Changed After Timestamp If Update Type Is StatusChanged And Timestamp Is Setted")>
        Sub ShouldReturnAllDocumentsWhoseStatusHasChangedAfterTimestampIfUpdateTypeIsStatusChangedAndTimestampIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("laststatuschange >= " & Robotics.VTBase.roTypes.Any2Time(timestamp.ToString()).SQLDateTime.ToLower()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Timestamp = timestamp
                documentCriteria.UpdateType = "StatusChanged"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTimestamp)

            End Using

        End Sub
        <Fact(DisplayName:="Should Return All Documents If Update Type Is Status Changed And Timestamp Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfUpdateTypeIsStatusChangedAndTimestampIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("laststatuschange >= " & timestamp.ToString()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "StatusChanged"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTimestamp)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents Which Were Delivered After Timestamp If Update Type Is Delivered And Timestamp Is Setted")>
        Sub ShouldReturnAllDocumentsWhichWereDeliveredAfterTimestampIfUpdateTypeIsDeliveredAndTimestampIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("deliverydate >= " & Robotics.VTBase.roTypes.Any2Time(timestamp.ToString()).SQLDateTime.ToLower()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Timestamp = timestamp
                documentCriteria.UpdateType = "Delivered"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTimestamp)

            End Using

        End Sub
        <Fact(DisplayName:="Should Return All Documents If Update Type Is Delivered And Timestamp Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfUpdateTypeIsDeliveredAndTimestampIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim timestamp As String = New DateTime(2024, 1, 1, 1, 1, 0)
                Dim filterByTimestamp As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("deliverydate >= " & timestamp.ToString()) Then
                                                                                                            filterByTimestamp = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "Delivered"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTimestamp)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents Of One Extension If Extension Is Setted")>
        Sub ShouldReturnAllDocumentsOfOneExtensionIfExtensionIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim extension As String = "pdf"
                Dim filterByExtension As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(documenttype) = '." & extension.ToString().ToLower() & "'") Then
                                                                                                            filterByExtension = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Extension = extension
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByExtension)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents If Extension Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfExtensionIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim extension As String = "pdf"
                Dim filterByExtension As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(documenttype) = '" & extension.ToString().ToLower() & "'") Then
                                                                                                            filterByExtension = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByExtension)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Documents Of One Template If Template Is Setted")>
        Sub ShouldReturnDocumentsOfOneTemplateIfTemplateIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"
                Dim filterByTemplate As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(dt.name) = '" & template.ToString().ToLower() & "'") Then
                                                                                                            filterByTemplate = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.Template = template
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(filterByTemplate)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return All Documents If Template Is Not Setted")>
        Sub ShouldReturnAllDocumentsIfTemplateIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"
                Dim filterByTemplate As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        ' Verifica que la consulta contiene TOP 10
                                                                                                        If strQuery.ToLower().Contains("lower(dt.name) = '" & template.ToString().ToLower() & "'") Then
                                                                                                            filterByTemplate = True
                                                                                                        End If

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = "1"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByTemplate)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Template Name")>
        Sub ShouldReturnDocumentTemplateName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentType.Equals(template))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return IdCompany In Company Documents")>
        Sub ShouldReturnIdCompanyInCompanyDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", 1, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), 1, "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "IdImport", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().CompanyID = "1")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Empty IdCompany In Employee Documents")>
        Sub ShouldReturnEmptyIdCompanyInEmployeeDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().CompanyID = String.Empty)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Absence Info in Documents Related With Day Absemce")>
        Sub ShouldReturnAbsenceInfoInDocumentsRelatedWithDayAbsemce()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos                                                                                                            
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, 1, 0, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)


                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().AbsenceInfo.Id = 1 AndAlso documents.FirstOrDefault().AbsenceInfo.AbsenceType = AbsenceType_Enum.Days.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Absence Info in Documents Related With Hours Absemce")>
        Sub ShouldReturnAbsenceInfoInDocumentsRelatedWithHoursAbsemce()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, 1, 1, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().AbsenceInfo.Id = 1 AndAlso documents.FirstOrDefault().AbsenceInfo.AbsenceType = AbsenceType_Enum.Hours.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return Absence Info in Documents That Are Not Related To Absence")>
        Sub ShouldNotReturnAbsenceInfoInDocumentsThatAreNotRelatedToAbsence()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().AbsenceInfo Is Nothing)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Decode Document Content For Documents Stored On Azure")>
        Sub ShouldReturnDecodeDocumentContentForDocumentsStoredOnAzure()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "DeliveryChannel", 0, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"CustomDecryptKey", Nothing}})
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentData = "AQIDBAUGBwg=")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document For TRASPASOGPA Documents")>
        Sub ShouldReturnDocumentForTRASPASOGPADocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "TRASPASOGPA", 0, New Byte() {72, 101, 108, 108, 111}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentData = "SGVsbG8=")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Decoded Document Content For Documents Stored On DataBase")>
        Sub ShouldReturnDecodedDocumentContentForDocumentsStoredOnDataBase()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentData = "AQIDBAUGBwg=")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Name")>
        Sub ShouldReturnDocumentName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentTitle = "Doc1")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Extension")>
        Sub ShouldReturnDocumentExtension()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentExtension = "jpg")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Delivery Date")>
        Sub ShouldReturnDocumentDeliveryDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DeliveryDate.GetDate() = New Date(2024, 1, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Delivery Channel")>
        Sub ShouldReturnDocumentDeliveryChannel()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DeliveryChannel = "Portal")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Who Deliver a Document")>
        Sub ShouldReturnWhoDeliverADocument()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DeliveredBy = "Admin")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Timestamp If Update Type Is Not Setted")>
        Sub ShouldReturnTimestampIfUpdateActionIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 1, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().Timestamp.GetDate() = New Date(2024, 1, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Timestamp If Update Type Is Signed")>
        Sub ShouldReturnTimestampIfUpdateActionIsSigned()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 1, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "signed"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().Timestamp.GetDate() = New Date(2024, 2, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Timestamp If Update Type Is Status Changed")>
        Sub ShouldReturnTimestampIfUpdateActionIsStatusChanged()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 1, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "statuschanged"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().Timestamp.GetDate() = New Date(2024, 1, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Timestamp If Update Type Is Delivered")>
        Sub ShouldReturnTimestampIfUpdateActionIsDelivered()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().Timestamp.GetDate() = New Date(2024, 1, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Pending Status on Pending Documents")>
        Sub ShouldReturnPendingStatusOnPendingDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 0, "Admin", New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.Type = DocumentStatusType_Enum.Pending.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Validated Status on Validated Documents")>
        Sub ShouldReturnValidatedStatusOnValidatedDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 1, "Admin", New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.Type = DocumentStatusType_Enum.Validated.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Expired Status on Expired Documents")>
        Sub ShouldReturnExpiredStatusOnExpiredDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 2, "Admin", New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.Type = DocumentStatusType_Enum.Expired.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Rejected Status on Rejected Documents")>
        Sub ShouldReturnRejectedStatusOnRejectedDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 3, "Admin", New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.Type = DocumentStatusType_Enum.Rejected.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Invalidated Status on Invalidated Documents")>
        Sub ShouldReturnInvalidatedStatusOnInvalidatedDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, "Admin", New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.Type = DocumentStatusType_Enum.Invalidated.ToString())

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Supervisor Name If Supervisor Has Changed Document Status")>
        Sub ShouldReturnSupervisorNameIfSupervisorHasChangedDocumentStatus()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, 1, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.SupervisorName = "Usuario 1")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Empty If Nobody Has Changed Document Status")>
        Sub ShouldReturnEmptyIfNobodyHasChangedDocumentStatus()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "DocumentExternalId"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.SupervisorName = "")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Begin Date If")>
        Sub ShouldReturnDocumentBeginDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.DocumentBeginDate.GetDate() = New Date(2024, 1, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document End Date")>
        Sub ShouldReturnDocumentEndDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.DocumentEndDate.GetDate() = New Date(2024, 5, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Sign Date If Document Is Signed")>
        Sub ShouldReturnDocumentSignDateIfDocumentIsSigned()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), New Date(2024, 2, 1), 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.DocumentSignedDate.GetDate() = New Date(2024, 2, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Empty Document Sign Date If Document Is Not Signed")>
        Sub ShouldReturnEmptyDocumentSignDateIfDocumentIsNotSigned()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.DocumentSignedDate Is Nothing)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Document Last Status Change Date")>
        Sub ShouldReturnDocumentLastStatusChangeDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()

                'Arrange                
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            ' Simula un conjunto de datos con más de 10 documentos
                                                                                                            Dim values As Object()() = {
                                                                                                                New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)
                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperAzure.GetDocumentFile()
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UpdateType = "delivered"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().DocumentStatusInfo.LastStatusChangeDate.GetDate() = New Date(2024, 3, 1))

            End Using

        End Sub

        <Fact(DisplayName:="Should Return IDEmployee In Employee Documents")>
        Sub ShouldReturnIDEmployeeInEmployeeDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", 1, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "employee"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().EmployeeID = "1")

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Empty IdEmployee In Company Documents")>
        Sub ShouldReturnEmptyIdEmployeeInCompanyDocuments()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1), "test"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate", "DocumentExternalId"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.True(documents.FirstOrDefault().EmployeeID = String.Empty)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Filter By Employee When Get Company Documents And IdEmployee Is Not Setted")>
        Sub ShouldNotFilterByEmployeeWhenGetCompanyDocumentsAndIdEmployeeIsNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"
                Dim filterByEmployee As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in(") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Filter By Employee When Get Company Documents And IdEmployee Is Single")>
        Sub ShouldNotFilterByEmployeeWhenGetCompanyDocumentsAndIdEmployeeIsSingle()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"
                Dim filterByEmployee As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in(") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                documentCriteria.UniqueEmployeeID = 1
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Filter By Employee When Get Company Documents And IdEmployee Is Multiple")>
        Sub ShouldNotFilterByEmployeeWhenGetCompanyDocumentsAndIdEmployeeIsMultiple()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperCommon.InitLicense()
                'Arrange
                Dim template As String = "TemplateName"
                Dim filterByEmployee As Boolean = False

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        If strQuery.ToLower().Contains("d.idemployee in(") Then
                                                                                                            filterByEmployee = True
                                                                                                        End If
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Documents" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"Doc1", "TemplateName", DBNull.Value, DBNull.Value, DBNull.Value, "BlobFileName", "Portal", 5, New Byte() {235, 146, 199, 193, 182, 123, 177, 69, 78, 124, 28, 251, 247, 236, 140, 194}, "jpg", New Date(2024, 1, 1), "Admin", New Date(2024, 3, 1), DBNull.Value, 4, DBNull.Value, New Date(2024, 1, 1), New Date(2024, 5, 1)}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Title", "DocumentType", "IdCompany", "IdDaysAbsence", "IdHoursAbsence", "BlobFileName", "DeliveryChannel", "DocLen", "Document", "Extension", "DeliveryDate", "DeliveredBy", "LastStatusChange", "SignDate", "status", "IdLastStatusSupervisor", "BeginDate", "EndDate"}, values)

                                                                                                        End If

                                                                                                        Return Nothing
                                                                                                    End Function
                helperDatalink.isEmployeeNewStub(2)
                helperPassport.PassportStub(1, helperDatalayer)
                'Act
                Dim dataImport As New roApiDocuments
                Dim documents As Generic.List(Of roDocument)
                Dim errorMsg As String
                Dim returnCode As ReturnCode
                Dim documentCriteria As roDatalinkStandarDocumentCriteria = New roDatalinkStandarDocumentCriteria
                documentCriteria.Type = "company"
                documentCriteria.UniqueEmployeeID = "1;2"
                Dim ret = dataImport.GetDocuments(documents, documentCriteria, errorMsg, returnCode)

                'Assert
                Assert.True(ret)
                Assert.False(filterByEmployee)

            End Using

        End Sub

    End Class

End Namespace