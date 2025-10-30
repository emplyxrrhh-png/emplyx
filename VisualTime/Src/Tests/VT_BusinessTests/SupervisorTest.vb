Imports System.ComponentModel
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin
Imports VT_XU_Base
Imports VT_XU_Datalink
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Supervisor")>
    <CollectionDefinition("Supervisor", DisableParallelization:=True)>
    <Category("Supervisor")>
    Public Class SupervisorTest

        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperEmployees As EmployeeHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperDatalink As DatalinkHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly helperGroups As GroupsHelper

        Sub New()
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDatalayer = New DatalayerHelper
            helperEmployees = New EmployeeHelper
            helperPassport = New PassportHelper
            helperBase = New BaseHelper
            helperDatalink = New DatalinkHelper
            helperGroups = New GroupsHelper
            helperBusiness = New BusinessHelper
        End Sub

        <Fact(DisplayName:="Should create a supervisor with the role defined in the advanced parameter when importing a supervisor")>
        Sub ShouldCreateSupervisorWithRoleDefinedInAdvancedParameterWhenImportingSupervisor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                SpreadsheetLight.Fakes.ShimSLDocument.ConstructorStream = Function(a As SpreadsheetLight.SLDocument, stream As System.IO.Stream) New SpreadsheetLight.Fakes.ShimSLDocument()
                SpreadsheetLight.Fakes.ShimSLDocument.AllInstances.GetSheetNames = Function(a As SpreadsheetLight.SLDocument) New List(Of String) From {"Sheet1"}

                helperBase.InitLanguage()
                helperDatalink.GenerateSupervisorImportStub("GroupName", "71", "1")
                helperPassport.GetFeatureListRoles(New roGroupFeature() {New roGroupFeature() With {.ID = 1, .Name = "Admin"}})
                helperDatalayer.StartTransaction()
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperEmployees.GetEmployee()
                helperGroups.GetGroupStub(1)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"Custom.PNET.SupervisorImport", "71-Admin"}, {"Custom.PNET.SupervisorImportCategoriesInfo", "[{'IDCategory': 0, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 1, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 2, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 3, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 4, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 5, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 6, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }]"}})
                helperPassport.GetPassportFeaturesStub(New Generic.List(Of Feature))
                helperPassport.SavePassportSpy()
                helperPassport.GetPassportsByRole()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)

                'Act
                Dim oDataLinkState As New Robotics.Base.VTBusiness.Common.roDataLinkState()
                Dim oImportFile As Byte() = {1, 2, 0, 3, 4, 0, 5}
                Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                oDataLinkImport.ImportSupervisorsExcel()
                'Assert
                Assert.True(helperPassport.SavedRole = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Should create a supervisor with the categories defined in the advanced parameter when importing a supervisor")>
        Sub ShouldCreateSupervisorWithCategoriesDefinedInAdvancedParameterWhenImportingSupervisor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                SpreadsheetLight.Fakes.ShimSLDocument.ConstructorStream = Function(a As SpreadsheetLight.SLDocument, stream As System.IO.Stream) New SpreadsheetLight.Fakes.ShimSLDocument()
                SpreadsheetLight.Fakes.ShimSLDocument.AllInstances.GetSheetNames = Function(a As SpreadsheetLight.SLDocument) New List(Of String) From {"Sheet1"}

                helperBase.InitLanguage()
                helperDatalink.GenerateSupervisorImportStub("GroupName", "71", "1")
                helperPassport.GetFeatureListRoles(New roGroupFeature() {New roGroupFeature() With {.ID = 1, .Name = "Admin"}})
                helperDatalayer.StartTransaction()
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperEmployees.GetEmployee()
                helperGroups.GetGroupStub(1)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"Custom.PNET.SupervisorImport", "71-Admin"}, {"Custom.PNET.SupervisorImportCategoriesInfo", "[{'IDCategory': 0, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 1, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 2, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 3, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 4, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 5, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 6, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }]"}})
                helperPassport.GetPassportFeaturesStub(New Generic.List(Of Feature))
                helperPassport.SavePassportSpy()
                helperPassport.GetPassportsByRole()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)

                'Act
                Dim oDataLinkState As New Robotics.Base.VTBusiness.Common.roDataLinkState()
                Dim oImportFile As Byte() = {1, 2, 0, 3, 4, 0, 5}
                Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                oDataLinkImport.ImportSupervisorsExcel()
                'Assert                
                Assert.True(helperPassport.SavedCategories.CategoryRows.Length = 7 AndAlso helperPassport.SavedCategories.CategoryRows(0).IDCategory = 0 AndAlso helperPassport.SavedCategories.CategoryRows(0).LevelOfAuthority = 3 AndAlso helperPassport.SavedCategories.CategoryRows(0).ShowFromLevel = 2)
            End Using
        End Sub

        <Fact(DisplayName:="Should not create a supervisor without the responsable type defined in the advanced parameter when importing a supervisor")>
        Sub ShouldNotCreateSupervisorWithoutResponsibleTypeDefinedInAdvancedParameterWhenImportingSupervisor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                SpreadsheetLight.Fakes.ShimSLDocument.ConstructorStream = Function(a As SpreadsheetLight.SLDocument, stream As System.IO.Stream) New SpreadsheetLight.Fakes.ShimSLDocument()
                SpreadsheetLight.Fakes.ShimSLDocument.AllInstances.GetSheetNames = Function(a As SpreadsheetLight.SLDocument) New List(Of String) From {"Sheet1"}

                helperBase.InitLanguage()
                helperDatalink.GenerateSupervisorImportStub("GroupName", "72", "1")
                helperPassport.GetFeatureListRoles(New roGroupFeature() {New roGroupFeature() With {.ID = 1, .Name = "Admin"}})
                helperDatalayer.StartTransaction()
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                helperPassport.PassportStub(1, helperDatalayer)
                helperEmployees.GetEmployee()
                helperGroups.GetGroupStub(1)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"Custom.PNET.SupervisorImport", "71-Admin"}, {"Custom.PNET.SupervisorImportCategoriesInfo", "[{'IDCategory': 0, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 1, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 2, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 3, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 4, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 5, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 6, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }]"}})
                helperPassport.GetPassportFeaturesStub(New Generic.List(Of Feature))
                helperPassport.Update()
                helperPassport.GetPassportsByRole()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)

                'Act
                Dim oDataLinkState As New Robotics.Base.VTBusiness.Common.roDataLinkState()
                Dim oImportFile As Byte() = {1, 2, 0, 3, 4, 0, 5}
                Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                oDataLinkImport.ImportSupervisorsExcel()
                'Assert                
                Assert.True(Not helperPassport.UpdatePassportCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should assign group to supervisor with role defined in the advanced parameter when importing a supervisor")>
        Sub ShouldAssignGroupToSupervisorWithRoleDefinedInAdvancedParameterWhenImportingSupervisor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                SpreadsheetLight.Fakes.ShimSLDocument.ConstructorStream = Function(a As SpreadsheetLight.SLDocument, stream As System.IO.Stream) New SpreadsheetLight.Fakes.ShimSLDocument()
                SpreadsheetLight.Fakes.ShimSLDocument.AllInstances.GetSheetNames = Function(a As SpreadsheetLight.SLDocument) New List(Of String) From {"Sheet1"}
                helperBase.InitLanguage()

                'Robotics.Security.Base.Fakes.ShimroBusinessState.AllInstances.LanguageGet = Function(a As Robotics.Security.Base.roBusinessState) New Robotics.VTBase.Fakes.ShimroLanguage()

                helperDatalink.GenerateSupervisorImportStub("GroupName", "71", "1")
                helperPassport.GetFeatureListRoles(New roGroupFeature() {New roGroupFeature() With {.ID = 1, .Name = "Admin"}})
                helperDatalayer.StartTransaction()
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oPassport As roPassport = New roPassport()
                oPassport.IsSupervisor = True
                oPassport.IDGroupFeature = 1
                helperPassport.PassportStub(1, helperDatalayer, oPassport)
                helperEmployees.GetEmployee()
                helperGroups.GetGroupStub(1)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"Custom.PNET.SupervisorImport", "71-Admin"}, {"Custom.PNET.SupervisorImportCategoriesInfo", "[{'IDCategory': 0, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 1, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 2, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 3, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 4, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 5, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 6, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }]"}})
                helperPassport.GetPassportFeaturesStub(New Generic.List(Of Feature))
                helperPassport.SavePassportSpy()
                helperPassport.GetPassportsByRole()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)


                'Act
                Dim oDataLinkState As New Robotics.Base.VTBusiness.Common.roDataLinkState()
                Dim oImportFile As Byte() = {1, 2, 0, 3, 4, 0, 5}
                Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                oDataLinkImport.ImportSupervisorsExcel()
                'Assert
                Assert.True(helperPassport.SavedGroups.GroupRows.Count = 1 AndAlso helperPassport.SavedGroups.GroupRows(0).IDGroup = 1)
            End Using
        End Sub

        <Fact(DisplayName:="Should unassign group to supervisor with role defined in the advanced parameter when importing supervisors")>
        Sub ShouldUnassignGroupToSupervisorWithRoleDefinedInAdvancedParameterWhenImportingSupervisors()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                SpreadsheetLight.Fakes.ShimSLDocument.ConstructorStream = Function(a As SpreadsheetLight.SLDocument, stream As System.IO.Stream) New SpreadsheetLight.Fakes.ShimSLDocument()
                SpreadsheetLight.Fakes.ShimSLDocument.AllInstances.GetSheetNames = Function(a As SpreadsheetLight.SLDocument) New List(Of String) From {"Sheet1"}
                helperBase.InitLanguage()


                'Robotics.Security.Base.Fakes.ShimroBusinessState.AllInstances.LanguageGet = Function(a As Robotics.Security.Base.roBusinessState) New Robotics.VTBase.Fakes.ShimroLanguage()

                helperDatalink.GenerateSupervisorImportStub("GroupName", "71", "1")
                helperPassport.GetFeatureListRoles(New roGroupFeature() {New roGroupFeature() With {.ID = 1, .Name = "Admin"}})
                helperDatalayer.StartTransaction()
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oPassport As roPassport = New roPassport()
                oPassport.IsSupervisor = True
                oPassport.IDGroupFeature = 1
                Dim oPassportGroup As roPassportGroupRow = New roPassportGroupRow()
                oPassportGroup.IDGroup = 1
                oPassportGroup.IDPassport = 1
                oPassport.Groups = New roPassportGroups()
                oPassport.Groups.GroupRows = New roPassportGroupRow() {}
                ReDim oPassport.Groups.GroupRows(oPassport.Groups.GroupRows.Length + 1)
                oPassport.Groups.GroupRows(0) = oPassportGroup
                oPassportGroup = New roPassportGroupRow()
                oPassportGroup.IDGroup = 2
                oPassportGroup.IDPassport = 1
                oPassport.Groups.GroupRows(1) = oPassportGroup
                oPassport.Groups.idPassport = 1
                oPassport.ID = 1
                helperPassport.PassportStub(1, helperDatalayer, oPassport)
                helperEmployees.GetEmployee()
                helperGroups.GetGroupStub(1)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"Custom.PNET.SupervisorImport", "71-Admin"}, {"Custom.PNET.SupervisorImportCategoriesInfo", "[{'IDCategory': 0, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 1, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 2, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 3, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 4, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 5, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }, {'IDCategory': 6, 'LevelOfAuthority': 3, 'ShowFromLevel': 2 }]"}})
                helperPassport.GetPassportFeaturesStub(New Generic.List(Of Feature))
                helperPassport.SavePassportSpy()
                helperPassport.GetPassportsByRole(oPassport)
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)


                'Act
                Dim oDataLinkState As New Robotics.Base.VTBusiness.Common.roDataLinkState()
                Dim oImportFile As Byte() = {1, 2, 0, 3, 4, 0, 5}
                Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                oDataLinkImport.ImportSupervisorsExcel()
                'Assert
                Assert.True(helperPassport.SavedGroups.GroupRows.Count = 1 AndAlso helperPassport.SavedGroups.GroupRows(0).IDGroup = 1)
            End Using
        End Sub
    End Class
End Namespace

