Imports System.ComponentModel
Imports Moq
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit
Imports System.Data.Common

Namespace Unit.Test

    <Collection("Group")>
    <CollectionDefinition("Group", DisableParallelization:=True)>
    <Category("Group")>
    Public Class GroupTest

        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly groupsHelper As GroupsHelper
        Private ReadOnly helperBusiness As BusinessHelper
        Private ReadOnly employeeHelper As EmployeeHelper
        Private ReadOnly helperPassport As PassportHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperDatalayer = New DatalayerHelper
            groupsHelper = New GroupsHelper
            helperBusiness = New BusinessHelper()
            employeeHelper = New EmployeeHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should delete group without associated communiques")>
        Sub ShouldDeleteGroupWithAssociatedCommuniques()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                groupsHelper.Initialize(helperDatalayer, 0)

                Dim group As Robotics.Base.VTBusiness.Group.roGroup = New Robotics.Base.VTBusiness.Group.roGroup()
                group.ID = 1
                group.Name = "Test Group"
                group.Path = "Test Path\Test"

                'Act
                Dim deleted As Boolean = group.Delete(False)

                'Assert
                Assert.True(deleted)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete group whose owner exists")>
        Sub ShouldNotDeleteGroupWhoseOwnerExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                groupsHelper.Initialize(helperDatalayer, 1)

                Dim group As Robotics.Base.VTBusiness.Group.roGroup = New Robotics.Base.VTBusiness.Group.roGroup()
                group.ID = 1
                group.Name = "Test Group"
                group.Path = "Test Path\Test"

                'Act
                Dim deleted As Boolean = group.Delete(False)

                'Assert
                Assert.False(deleted)

            End Using
        End Sub

        <Fact(DisplayName:="Should delete group that is the recipient of a communiquee")>
        Sub ShouldDeleteGroupThatIsTheRecipientOfACommuniquee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                groupsHelper.Initialize(helperDatalayer, 0)


                Dim group As Robotics.Base.VTBusiness.Group.roGroup = New Robotics.Base.VTBusiness.Group.roGroup()
                group.ID = 1
                group.Name = "Test Group"
                group.Path = "Test Path\Test"

                'Act
                Dim deleted As Boolean = group.Delete(False)

                'Assert
                Assert.True(deleted AndAlso groupsHelper.DeleteCommuniqueeGroupCalled)

            End Using
        End Sub


        <Fact(DisplayName:="Should create or update employee if all levels are setted")>
        Sub ShouldCreateOrUpdateEmployeeIfAllLevelsAreSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.Nivel0 = "test"
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel8 = "Level 8"
                oEmp.Nivel9 = "Level 9"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if levels are not setted and only has one company")>
        Sub ShouldCreateOrUpdateEmployeeIfLevelsAreNotSettedAndOnlyHasOneCompany()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if levels are not setted and has more than one company")>
        Sub ShouldCreateOrUpdateEmployeeIfLevelsAreNotSettedAndHasMoreThanOneCompany()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {"1"}, New Object() {"2"}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level 1 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level2 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel2IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel2 = "Level 2"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level3 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel3IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel3 = "Level 3"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level4 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel4IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel4 = "Level 4"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level5 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel5IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel5 = "Level 5"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level6 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level1 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel1IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level3 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel3IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel3 = "Level 3"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level4 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel4IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel4 = "Level 4"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level5 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel5IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel5 = "Level 5"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level6 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level2 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel2IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level4 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel4IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel4 = "Level 4"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level5 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel5IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel5 = "Level 5"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level6 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level3 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel3IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level5 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel5IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel5 = "Level 5"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level6 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level4 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel4IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 and level5 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4AndLevel5IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level5 is not setted and level6 is setted")>
        Sub ShouldReturnErrorIfLevel5IsNotSettedAndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level5 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel5IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level5 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel5IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level5 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel5IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level5 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel5IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 and level5 and level6 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4AndLevel5AndLevel6IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level6 is not setted and level7 is setted")>
        Sub ShouldReturnErrorIfLevel6IsNotSettedAndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level6 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel6IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level6 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel6IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level6 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel6IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 and level5 and level6 and level7 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4AndLevel5AndLevel6AndLevel7IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level7 is not setted and level8 is setted")>
        Sub ShouldReturnErrorIfLevel7IsNotSettedAndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level7 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel7IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level7 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel7IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 and level5 and level6 and level7 and level8 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4AndLevel5AndLevel6AndLevel7AndLevel8IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel8 = "Level 8"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level8 is not setted and level9 is setted")>
        Sub ShouldReturnErrorIfLevel8IsNotSettedAndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level8 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel8IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub

        <Fact(DisplayName:="Should create or update employee if level1 and level 2 and level3 and level4 and level5 and level6 and level7 and level8 and level9 is setted")>
        Sub ShouldCreateOrUpdateEmployeeIfLevel1AndLevel2AndLevel3AndLevel4AndLevel5AndLevel6AndLevel7AndLevel8AndLevel9IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel8 = "Level 8"
                oEmp.Nivel9 = "Level 9"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._OK)

            End Using
        End Sub

        <Fact(DisplayName:="Should return error if level9 is not setted and level10 is setted")>
        Sub ShouldReturnErrorIfLevel9IsNotSettedAndLevel10IsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmp As RoboticsExternAccess.roDatalinkStandarEmployee = New RoboticsExternAccess.roDatalinkStandarEmployee()
                oEmp.NombreEmpleado = "test"
                oEmp.UniqueEmployeeID = "1234"
                oEmp.IDContract = "1234"
                oEmp.LabAgreeName = "test"
                oEmp.StartContractDate = Now.Date
                oEmp.NifEmpleado = "12345678A"
                oEmp.EnabledVTDesktop = False
                oEmp.Nivel0 = "Level 0"
                oEmp.Nivel1 = "Level 1"
                oEmp.Nivel2 = "Level 2"
                oEmp.Nivel3 = "Level 3"
                oEmp.Nivel4 = "Level 4"
                oEmp.Nivel5 = "Level 5"
                oEmp.Nivel6 = "Level 6"
                oEmp.Nivel7 = "Level 7"
                oEmp.Nivel8 = "Level 8"
                oEmp.Nivel10 = "Level 10"
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
                                                                                         ElseIf tableName = "Groups" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function
                Fakes.ShimAccessHelper.CreateDataTableStringString = Function(ByVal strQuery As String, ByVal strTableName As String) As DataTable
                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                         If tableName = "Employees" Then
                                                                             Return New DataTable()
                                                                         End If

                                                                     End Function
                helperDatalayer.CreateCommandStub()

                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                employeeHelper.SaveEmployeeSpy()
                employeeHelper.SaveEmployeeUserField()
                employeeHelper.SaveUserField()
                employeeHelper.GetCompanyByName()
                employeeHelper.SaveGroupUserField()
                employeeHelper.SaveMobility()
                employeeHelper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidGroup)

            End Using
        End Sub
    End Class

End Namespace