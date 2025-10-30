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

    <Collection("Employee")>
    <CollectionDefinition("Employee", DisableParallelization:=True)>
    <Category("Employee")>
    Public Class EmployeeTest

        Private ReadOnly helper As EmployeeHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperBusiness As BusinessHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helper = New EmployeeHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDatalayer = New DatalayerHelper
            helperPassport = New PassportHelper
            helperBusiness = New BusinessHelper()
        End Sub

        <Fact(DisplayName:="Should Try To Find Employee By NIF If Not Found By PrimaryKey")>
        Sub ShouldTryToFindEmployeeByNIFIfNotFoundByPrimaryKey()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.GetIDEmployeesFromUserFieldValueSpy()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportSecondaryKeyIsNIF", "1"}, {"ImportPrimaryKeyUserField", "Id. Importacion"}})
                helperDatalayer.ExecuteScalarStub("sysroVisualtimeID", "Identificador usuario")

                'Act
                Dim oDatalinkImport As roEmployeeImport = New roEmployeeImport()
                Dim isEmployee As Boolean = oDatalinkImport.isEmployeeNew("115", "12345678X", Nothing)

                'Assert
                Assert.Equal(helper.NifSearchCounter, 1)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Find Employee By NIF If Not Found By UserId")>
        Sub ShouldNotTryToFindEmployeeByNIFIfNotFoundByUserId()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.GetIDEmployeesFromUserFieldValueSpy()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportSecondaryKeyIsNIF", "1"}, {"ImportPrimaryKeyUserField", "Identificador usuario"}})
                helperDatalayer.ExecuteScalarStub("sysroVisualtimeID", "Identificador usuario")

                'Act
                Dim oDatalinkImport As roEmployeeImport = New roEmployeeImport()
                Dim isEmployee As Boolean = oDatalinkImport.isEmployeeNew("115", "12345678X", Nothing)

                'Assert
                Assert.Equal(helper.NifSearchCounter, 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Find Employee By NIF If Search By Empty UserId")>
        Sub ShouldNotTryToFindEmployeeByNIFIfSearchByEmptyUserId()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.GetIDEmployeesFromUserFieldValueSpy()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportSecondaryKeyIsNIF", "1"}, {"ImportPrimaryKeyUserField", "Identificador usuario"}})
                helperDatalayer.ExecuteScalarStub("sysroVisualtimeID", "Identificador usuario")

                'Act
                Dim oDatalinkImport As roEmployeeImport = New roEmployeeImport()
                Dim isEmployee As Boolean = oDatalinkImport.isEmployeeNew("", "12345678X", Nothing)

                'Assert
                Assert.Equal(helper.NifSearchCounter, 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Find Employee By NIF If Found By PrimaryKey")>
        Sub ShouldNotTryToFindEmployeeByNIFIfFoundByPrimaryKey()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange

                helper.AddSearchEmployeeCounterByNifSpy()
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportSecondaryKeyIsNIF", "1"}, {"ImportPrimaryKeyUserField", "Id. Importacion"}})
                helperDatalayer.ExecuteScalarStub("sysroVisualtimeID", "Identificador usuario")

                'Act
                Dim oDatalinkImport As roEmployeeImport = New roEmployeeImport()
                Dim isEmployee As Boolean = oDatalinkImport.isEmployeeNew("115", "12345678X", Nothing)

                'Assert
                Assert.Equal(helper.NifSearchCounter, 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Return Employee Without Active Contracts If OnlyActiveContracts Is True")>
        Sub ShouldNotReturnEmployeeWithoutActiveContractsIfOnlyActiveContractsIsTrue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(False, True)

                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of DataLink.RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, True, False, "", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.Equal(0, oEmployees.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee With Active Contract If OnlyActiveContracts Is True")>
        Sub ShouldReturnEmployeeWithActiveContractsIfOnlyActiveContractsIsTrue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)

                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, True, False, "", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.Equal(1, oEmployees.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee Without Active Contract If OnlyActiveContracts Is False")>
        Sub ShouldReturnEmployeeWithOutActiveContractsIfOnlyActiveContractsIsFalse()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, True)

                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, False, False, "", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.Equal(2, oEmployees.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return Employee By ID If Not Exists")>
        Sub ShouldNotReturnEmployeeByIDIfNotExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)

                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, False, False, "115", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.Equal(0, oEmployees.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee By ID If Exists")>
        Sub ShouldReturnEmployeeByIDIfExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)

                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "26"}})

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, False, False, "26", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK)
                Assert.Equal(1, oEmployees.Count)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee By ID If Exists On GetEmployeeById Function")>
        Sub ShouldReturnEmployeeByIDIfExistsOnGetEmployeeByIdFunction()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)

                'Act
                Dim oDataExport As New roApiEmployees
                Dim strErrorMsg As String
                Dim oEmployee As RoboticsExternAccess.roEmployee = oDataExport.GetEmployeeById("116", strErrorMsg)

                'Assert
                Assert.NotEqual(Nothing, oEmployee)
                Assert.Equal("26", oEmployee.ID)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return Employee By ID If Not Exists On GetEmployeeById Function")>
        Sub ShouldNotReturnEmployeeByIDIfNotExistsOnGetEmployeeByIdFunction()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)

                'Act
                Dim oDataExport As New roApiEmployees
                Dim strErrorMsg As String
                Dim oEmployee As RoboticsExternAccess.roEmployee = oDataExport.GetEmployeeById("2", strErrorMsg)

                'Assert
                Assert.Equal(Nothing, oEmployee)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee with All Contracts On GetEmployeeById Function")>
        Sub ShouldReturnEmployeewithContractsOnGetEmployeeByIdFunction()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)
                Dim numOfContracts = 3
                Dim sIdEmployee = "116"
                helper.GetContractsByIDEmployeeStub(sIdEmployee, numOfContracts)

                'Act
                Dim oDataExport As New roApiEmployees
                Dim strErrorMsg As String
                Dim oEmployee As RoboticsExternAccess.roEmployee = oDataExport.GetEmployeeById(sIdEmployee, strErrorMsg)

                'Assert
                Assert.NotEqual(Nothing, oEmployee)
                Assert.Equal(numOfContracts, oEmployee.Contracts.Length)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Return Employee with 0 Contracts When Has Contracts On GetEmployeeById Function")>
        Sub ShouldReturnEmployeewith0ContractsWhenHasContractsOnGetEmployeeByIdFunction()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helper.EmployeesAndContractsTableStub(True, False)
                Dim numOfContracts = 2
                Dim sIdEmployee = "116"
                helper.GetContractsByIDEmployeeStub(sIdEmployee, numOfContracts)

                'Act
                Dim oDataExport As New roApiEmployees
                Dim strErrorMsg As String
                Dim oEmployee As RoboticsExternAccess.roEmployee = oDataExport.GetEmployeeById(sIdEmployee, strErrorMsg)

                'Assert
                Assert.NotEqual(Nothing, oEmployee)
                Assert.NotEqual(0, oEmployee.Contracts.Length)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Validate Contract If data is empty on ValidateContract function")>
        Sub ShouldNotvalidateEmployeeContractIfIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                Dim oContract As New roContract(New roContractState(-1), "")

                oContract.IDContract = ""

                'Act
                Dim bolret As Boolean = oContract.ValidateContract()

                'Assert
                Assert.Equal(ContractsResultEnum.InvalidIDContract.ToString, oContract.State.Result.ToString)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Validate Contract If Exists Another Employee With Same IDContract")>
        Sub ShouldNotvalidateEmployeeContractIfExistsAnotherEmployeeWithSameIDContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helper.ContractLoadStub("1234", 12)
                Dim oContract As New roContract(New roContractState(-1), "1234")
                oContract.IDEmployee = "1"

                'Act
                Dim bolret As Boolean = oContract.ValidateContract()

                'Assert
                Assert.Equal(ContractsResultEnum.InvalidIDContract.ToString, oContract.State.Result.ToString)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Validate Contract If EndDate Is Smaller Than BeginDate")>
        Sub ShouldNotvalidateEmployeeContractIfEndDateIsSmallerThanBeginDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                Dim oContract As New roContract(New roContractState(-1), "1234")
                oContract.IDEmployee = "1"
                oContract.BeginDate = Now.Date
                oContract.EndDate = Now.Date.AddDays(-1)
                'Act
                Dim bolret As Boolean = oContract.ValidateContract()

                'Assert
                Assert.Equal(ContractsResultEnum.InvalidDates.ToString, oContract.State.Result.ToString)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Contract If BeginDate Is Smaller Than Freeze Date")>
        Sub ShouldNotvalidateEmployeeContractIfBeginDateIsSmallerThanFreezeDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                Dim oContract As New roContract(New roContractState(-1), "1234")
                oContract.IDEmployee = "1"
                oContract.BeginDate = Now.Date.AddYears(-20)
                oContract.EndDate = Now.Date.AddDays(-1)

                helper.GetEmployeeLockDatetoApplyStub(Now.Date)

                'Act
                Dim bolret As Boolean = oContract.ValidateContract()

                'Assert
                Assert.Equal(ContractsResultEnum.ContractInFreezeDate.ToString, oContract.State.Result.ToString)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Contract If Date Period Interval Overlaps With An Existing One")>
        Sub ShouldNotvalidateEmployeeContractIfDatePeriodIntervalOverlapsWithAnExistingOne()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                Dim oContractStub As New roContract(New roContractState(-1), "1234")
                oContractStub.BeginDate = Now.Date.AddDays(-1)
                oContractStub.EndDate = Now.Date.AddDays(101)
                helper.GetContractsByIdEmployeeStub(oContractStub)

                Dim oContract As New roContract(New roContractState(-1), "1234")
                oContract.IDEmployee = "1"
                oContract.BeginDate = Now.Date
                oContract.EndDate = Now.Date.AddDays(100)

                'Act
                Dim bolret As Boolean = oContract.ValidateContract()

                'Assert
                Assert.Equal(ContractsResultEnum.InvalidDateInterval.ToString, oContract.State.Result.ToString)

            End Using
        End Sub






        <Fact(DisplayName:="Should create employee with PhotoRequiered Without Value if RequirePunchWithPhoto isn´t specified")>
        Sub ShouldCreateEmployeeWithPhotoRequieredTrueIfRequirePunchWithPhotoIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.UpdatePassportPhotoRequieredCalled = False AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LocationRequiered True if RequirePunchWithLocation is true")>
        Sub ShouldCreateEmployeeWithLocationRequieredTrueIfRequirePunchWithLocationIsTrue()

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
                oEmp.RequirePunchWithGeolocation = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.LocationRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with PhotoRequiered False if RequirePunchWithPhoto is false")>
        Sub ShouldCreateEmployeeWithPhotoRequieredFalseIfRequirePunchWithPhotoIsFalse()

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
                oEmp.RequirePunchWithPhoto = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.PhotoRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update PhotoRequiered For Employee If RequirePunchWithPhoto Isnt Specified")>
        Sub ShouldNotUpdatePhotoRequieredForEmployeeIfRequirePunchWithPhotoIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.PhotoRequiered = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set PhotoRequiered True For Employee If RequirePunchWithPhoto Is True")>
        Sub ShouldSetPhotoRequieredTrueForEmployeeIfRequirePunchWithPhotoIsTrue()

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
                oEmp.RequirePunchWithPhoto = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.PhotoRequiered = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.PhotoRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set PhotoRequiered False For Employee If RequirePunchWithPhoto Is False")>
        Sub ShouldSetPhotoRequieredFalseForEmployeeIfRequirePunchWithPhotoIsFalse()

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
                oEmp.RequirePunchWithPhoto = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.PhotoRequiered = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.PhotoRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LocationRequiered True if RequirePunchWithLocation isn´t specified")>
        Sub ShouldCreateEmployeeWithLocationRequieredTrueIfRequirePunchWithLocationIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.UpdatePassportLocationRequieredCalled = False AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with PhotoRequiered True if RequirePunchWithPhoto is true")>
        Sub ShouldCreateEmployeeWithPhotoRequieredTrueIfRequirePunchWithPhotoIsTrue()

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
                oEmp.RequirePunchWithPhoto = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportPhotoRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.PhotoRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LocationRequiered False if RequirePunchWithLocation is false")>
        Sub ShouldCreateEmployeeWithLocationRequieredFalseIfRequirePunchWithLocationIsFalse()

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
                oEmp.RequirePunchWithGeolocation = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.LocationRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update LocationRequiered For Employee If RequirePunchWithLocation Isnt Specified")>
        Sub ShouldNotUpdateLocationRequieredForEmployeeIfRequirePunchWithLocationIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LocationRequiered = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set LocationRequiered True For Employee If RequirePunchWithLocation Is True")>
        Sub ShouldSetLocationRequieredTrueForEmployeeIfRequirePunchWithLocationIsTrue()

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
                oEmp.RequirePunchWithGeolocation = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LocationRequiered = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.LocationRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set LocationRequiered False For Employee If RequirePunchWithLocation Is False")>
        Sub ShouldSetLocationRequieredFalseForEmployeeIfRequirePunchWithLocationIsFalse()

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
                oEmp.RequirePunchWithPhoto = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LocationRequiered = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLocationRequiered()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.PhotoRequiered AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should not delete contract if it is the last one")>
        Sub ShouldNotDeleteContractIfItIsTheLastOne()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helper.ContractLoadStub("1234", 26)
                Dim oState As New roContractState(-1)
                Dim oContractStub As New roContract(oState, "1234")
                oContractStub.BeginDate = Now.Date.AddDays(-1)
                oContractStub.EndDate = Now.Date.AddDays(101)
                helper.GetContractsByIdEmployeeStub(oContractStub)

                'Act
                oContractStub.Delete()

                'Assert
                Assert.Equal(ContractsResultEnum.LastContractDeleteError.ToString, oContractStub.State.Result.ToString)

            End Using

        End Sub

        <Fact(DisplayName:="Should delete contract if there is more than one")>
        Sub ShouldDeleteContractIfifThereIsMoreThanOne()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlWithoutTimeOutString =
                                                       Function(ByVal strQuery As String)
                                                           If strQuery.ToLower().Trim().Contains("delete") Then
                                                               Return True
                                                           End If
                                                       End Function

                helper.ContractLoadStub("1234", 26)
                Dim oState As New roContractState(-1)
                Dim oContractStub As New roContract(oState, "1234")
                oContractStub.BeginDate = Now.Date.AddDays(-1)
                oContractStub.EndDate = Now.Date.AddDays(101)

                helper.EmployeesAndContractsTableStub(True, False)
                Dim numOfContracts = 3
                Dim sIdEmployee = "26"
                helper.GetContractsByIDEmployeeStub(sIdEmployee, numOfContracts)

                helperDatalayer.CreateCommandStub()
                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)

                'Act
                Dim bolret = oContractStub.Delete()

                'Assert
                Assert.Equal(True.ToString, bolret.ToString)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTDesktop False if EnabledVTDesktop is false")>
        Sub ShouldCreateEmployeeWithEnabledVTDesktopFalseIfEnabledVTDesktopIsFalse()

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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTDesktop AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTDesktop True if EnabledVTDesktop is true")>
        Sub ShouldCreateEmployeeWithEnabledVTDesktopTrueIfEnabledVTDesktopIsTrue()

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
                oEmp.EnabledVTDesktop = True
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTDesktop AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTDesktop True if EnabledVTDesktop Is Not Specified")>
        Sub ShouldCreateEmployeeWithEnabledVTDesktopTrueIfEnabledVTDesktopIsNotSpecified()

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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTDesktop AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update EnabledVTDesktop For Employee If EnabledVTDesktop Isnt Specified")>
        Sub ShouldNotUpdateEnabledVTDesktopForEmployeeIfEnabledVTDesktopIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTDesktop = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTDesktop True For Employee If EnabledVTDesktop Is True")>
        Sub ShouldSetEnabledVTDesktopTrueForEmployeeIfEnabledVTDesktopIsTrue()

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
                oEmp.EnabledVTDesktop = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTDesktop = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTDesktop AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTDesktop False For Employee If EnabledVTDesktop Is False")>
        Sub ShouldSetEnabledVTDesktopFalseForEmployeeIfEnabledVTDesktopIsFalse()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTDesktop = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTDesktop AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTVisits False if EnabledVTVisits is false")>
        Sub ShouldCreateEmployeeWithEnabledVTVisitsFalseIfEnabledVTVisitsIsFalse()

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
                oEmp.EnabledVTVisits = False
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTVisits AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTVisits True if EnabledVTVisits is true")>
        Sub ShouldCreateEmployeeWithEnabledVTVisitsTrueIfEnabledVTVisitsIsTrue()

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
                oEmp.EnabledVTVisits = True
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTVisits AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTVisits True if EnabledVTVisits Is Not Specified")>
        Sub ShouldCreateEmployeeWithEnabledVTVisitsTrueIfEnabledVTVisitsIsNotSpecified()

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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTVisits AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update EnabledVTVisits For Employee If EnabledVTVisits Isnt Specified")>
        Sub ShouldNotUpdateEnabledVTVisitsForEmployeeIfEnabledVTVisitsIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTVisits = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTVisits True For Employee If EnabledVTVisits Is True")>
        Sub ShouldSetEnabledVTVisitsTrueForEmployeeIfEnabledVTVisitsIsTrue()

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
                oEmp.EnabledVTVisits = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTVisits = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTVisits AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTVisits False For Employee If EnabledVTVisits Is False")>
        Sub ShouldSetEnabledVTVisitsFalseForEmployeeIfEnabledVTVisitsIsFalse()

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
                oEmp.EnabledVTVisits = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTVisits = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTVisits()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTVisits AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortalApp False if EnabledVTPortalApp is false")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalAppFalseIfEnabledVTPortalAppIsFalse()

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
                oEmp.EnabledVTPortalApp = False
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTPortalApp AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortalApp True if EnabledVTPortalApp is true")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalAppTrueIfEnabledVTPortalAppIsTrue()

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
                oEmp.EnabledVTPortalApp = True
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortalApp AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortalApp True if EnabledVTPortalApp Is Not Specified")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalAppTrueIfEnabledVTPortalAppIsNotSpecified()

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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortalApp AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update EnabledVTPortalApp For Employee If EnabledVTPortalApp Isnt Specified")>
        Sub ShouldNotUpdateEnabledVTPortalAppForEmployeeIfEnabledVTPortalAppIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTPortalApp = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTPortalApp True For Employee If EnabledVTPortalApp Is True")>
        Sub ShouldSetEnabledVTPortalAppTrueForEmployeeIfEnabledVTPortalAppIsTrue()

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
                oEmp.EnabledVTPortalApp = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTDesktop = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortalApp AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTPortalApp False For Employee If EnabledVTPortalApp Is False")>
        Sub ShouldSetEnabledVTPortalAppFalseForEmployeeIfEnabledVTPortalAppIsFalse()

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
                oEmp.EnabledVTPortalApp = False
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTPortalApp = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortalApp()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTPortalApp AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortal False if EnabledVTPortal is false")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalFalseIfEnabledVTPortalIsFalse()

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
                oEmp.EnabledVTPortal = False
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTPortal AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortal True if EnabledVTPortal is true")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalTrueIfEnabledVTPortalIsTrue()

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
                oEmp.EnabledVTPortal = True
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortal AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with EnabledVTPortal True if EnabledVTPortal Is Not Specified")>
        Sub ShouldCreateEmployeeWithEnabledVTPortalTrueIfEnabledVTPortalIsNotSpecified()

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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortal AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update EnabledVTPortal For Employee If EnabledVTPortal Isnt Specified")>
        Sub ShouldNotUpdateEnabledVTPortalForEmployeeIfEnabledVTPortalIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTPortal = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTPortal True For Employee If EnabledVTPortal Is True")>
        Sub ShouldSetEnabledVTPortalTrueForEmployeeIfEnabledVTPortalIsTrue()

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
                oEmp.EnabledVTPortal = True
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTPortal = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.EnabledVTPortal AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set EnabledVTPortal False For Employee If EnabledVTPortal Is False")>
        Sub ShouldSetEnabledVTPortalFalseForEmployeeIfEnabledVTPortalIsFalse()

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
                oEmp.EnabledVTPortal = False
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.EnabledVTPortal = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTPortal()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.EnabledVTPortal AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LoginWithoutContract False if LoginWithoutContract is false")>
        Sub ShouldCreateEmployeeWithLoginWithoutContractFalseIfLoginWithoutContractIsFalse()

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
                oEmp.LoginWithoutContract = False
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.LoginWithoutContract AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LoginWithoutContract True if LoginWithoutContract is true")>
        Sub ShouldCreateEmployeeWithLoginWithoutContractTrueIfLoginWithoutContractIsTrue()

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
                oEmp.LoginWithoutContract = True
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.LoginWithoutContract AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with LoginWithoutContract False if LoginWithoutContract Is Not Specified")>
        Sub ShouldCreateEmployeeWithLoginWithoutContractFalseIfLoginWithoutContractIsNotSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, Not helperPassport.LoginWithoutContract AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Not Update LoginWithoutContract For Employee If LoginWithoutContract Isnt Specified")>
        Sub ShouldNotUpdateLoginWithoutContractForEmployeeIfLoginWithoutContractIsntSpecified()

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
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LoginWithoutContract = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.True(helper.SaveEmployeeCalled AndAlso Not helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set LoginWithoutContract True For Employee If LoginWithoutContract Is True")>
        Sub ShouldSetLoginWithoutContractTrueForEmployeeIfLoginWithoutContractIsTrue()

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
                oEmp.LoginWithoutContract = True
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LoginWithoutContract = False
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helperPassport.LoginWithoutContract AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should Set LoginWithoutContract False For Employee If LoginWithoutContract Is False")>
        Sub ShouldSetLoginWithoutContractFalseForEmployeeIfLoginWithoutContractIsFalse()

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
                oEmp.LoginWithoutContract = False
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LoginWithoutContract = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperPassport.UpdatePassportLoginWithoutContract()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helperPassport.LoginWithoutContract AndAlso helper.SaveEmployeeCalled AndAlso helperPassport.UpdatePassportCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee with image if photo is setted")>
        Sub ShouldCreateEmployeeWithImageIfPhotoIsSetted()

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
                oEmp.UserPhoto = "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"
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
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {1}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helper.UpdateEmployeeImageCalled AndAlso helper.Image.Length > 0 AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should create employee without image if photo is not setted")>
        Sub ShouldCreateEmployeeWithoutImageIfPhotoIsNotSetted()

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
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {1}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, Not helper.UpdateEmployeeImageCalled AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should set employee image if photo is setted and employee is updated")>
        Sub ShouldSetEmployeeImageIfPhotoIsSettedAndEmployeeIsUpdated()

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
                oEmp.UserPhoto = "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"
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LoginWithoutContract = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, helper.UpdateEmployeeImageCalled AndAlso helper.Image.Length > 0 AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should not set employee image if photo is not setted and employee is updated")>
        Sub ShouldNotSetEmployeeImageIfPhotoIsNotSettedAndEmployeeIsUpdated()

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
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, Not helper.UpdateEmployeeImageCalled AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should not set employee image if photo is empty and employee is updated")>
        Sub ShouldNotSetEmployeeImageIfPhotoIsEmptyAndEmployeeIsUpdated()

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
                oEmp.UserPhoto = ""
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, Not helper.UpdateEmployeeImageCalled AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should update employee photo if photo is setted")>
        Sub ShouldUpdateEmployeePhotoIfPhotoIsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmpPhoto As RoboticsExternAccess.roDatalinkStandardPhoto = New RoboticsExternAccess.roDatalinkStandardPhoto()
                oEmpPhoto.UniqueEmployeeID = "1234"
                oEmpPhoto.NifEmpleado = "12345678A"
                oEmpPhoto.PhotoData = "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"
                Dim cCode As String = "test"
                Dim returnMsg As String = ""
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployeePhoto(oEmpPhoto, "", iResult, returnMsg)

                ' Assert
                Assert.Equal(True, helper.UpdateEmployeeImageCalled AndAlso helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should not update employee photo if photo is not setted")>
        Sub ShouldNotUpdateEmployeePhotoIfPhotoIsNotSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmpPhoto As RoboticsExternAccess.roDatalinkStandardPhoto = New RoboticsExternAccess.roDatalinkStandardPhoto()
                oEmpPhoto.UniqueEmployeeID = "1234"
                oEmpPhoto.NifEmpleado = "12345678A"
                Dim cCode As String = "test"
                Dim returnMsg As String = ""
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployeePhoto(oEmpPhoto, "", iResult, returnMsg)

                ' Assert
                Assert.Equal(True, Not helper.UpdateEmployeeImageCalled AndAlso Not helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should not update employee photo if photo is empty")>
        Sub ShouldNotUpdateEmployeePhotoIfPhotoIsEmpty()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmpPhoto As RoboticsExternAccess.roDatalinkStandardPhoto = New RoboticsExternAccess.roDatalinkStandardPhoto()
                oEmpPhoto.UniqueEmployeeID = "1234"
                oEmpPhoto.NifEmpleado = "12345678A"
                oEmpPhoto.PhotoData = ""
                Dim cCode As String = "test"
                Dim returnMsg As String = ""
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployeePhoto(oEmpPhoto, "", iResult, returnMsg)

                ' Assert
                Assert.Equal(True, Not helper.UpdateEmployeeImageCalled AndAlso Not helper.SaveEmployeeCalled)

            End Using

        End Sub

        <Fact(DisplayName:="Should return InvalidPhotoData error when create user with invalid photo")>
        Sub ShouldReturnInvalidPhotoDataErrorWhenCreateUserWithInvalidPhoto()

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
                oEmp.UserPhoto = "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"
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
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {1}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidPhotoData)

            End Using

        End Sub

        <Fact(DisplayName:="Should return InvalidPhotoData error when update user with invalid photo")>
        Sub ShouldReturnInvalidPhotoDataErrorWhenUpdateUserWithInvalidPhoto()

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
                oEmp.UserPhoto = "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"
                Dim cCode As String = "test"
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 0
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                passport.LoginWithoutContract = True
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidPhotoData)

            End Using

        End Sub

        <Fact(DisplayName:="Should return Invalid Photo Data if invalid photo is setted")>
        Sub ShouldReturnInvalidPhotoDataIfInvalidPhotoIsSetted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oEmpPhoto As RoboticsExternAccess.roDatalinkStandardPhoto = New RoboticsExternAccess.roDatalinkStandardPhoto()
                oEmpPhoto.UniqueEmployeeID = "1234"
                oEmpPhoto.NifEmpleado = "12345678A"
                oEmpPhoto.PhotoData = "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"
                Dim cCode As String = "test"
                Dim returnMsg As String = ""
                Dim iResult As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helper.UpdateEmployeePhoto()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployeePhoto(oEmpPhoto, "", iResult, returnMsg)

                ' Assert
                Assert.Equal(True, iResult = ReturnCode._InvalidPhotoData)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Employee Plate If Exists")>
        Sub ShouldReturnEmployeePlateIfExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helper.EmployeesAndContractsTableStub(True, False)
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "26"}})
                Dim passport As roPassport = New roPassport()
                Dim oAuthentication As New roPassportAuthenticationMethodsRow()
                oAuthentication.Method = AuthenticationMethod.Plate
                oAuthentication.Credential = "1234ASD"
                Dim tmpAuthentication As New Generic.List(Of roPassportAuthenticationMethodsRow)
                tmpAuthentication.Add(oAuthentication)
                passport.AuthenticationMethods.PlateRows = tmpAuthentication.ToArray()
                helperPassport.PassportStub(1, helperDatalayer, passport)

                'Act
                Dim oDataExport As New roApiEmployees
                Dim oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee)
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetEmployees(oEmployees, False, False, "26", "", "", strErrorMsg, returnCode, Nothing)

                'Assert
                Assert.True(returnCode = ReturnCode._OK)
                Assert.Equal("1234ASD", oEmployees.FirstOrDefault().AuthenticationMethods.FirstOrDefault().Credential)

            End Using

        End Sub

        <Fact(DisplayName:="Should set employee user field value if user field name is not empty when create employee")>
        Sub ShouldSetEmployeeUserFieldValueIfUserFieldNameIsNotEmptyWhenCreateEmployee()

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
                oEmp.UserFields = New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() {New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = "test", .UserFieldValue = "test", .UserFieldValueDate = New Date(2024, 7, 16)}}
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helper.SavedUserField.FieldName = "test" AndAlso helper.SavedUserField.ListValuesString.IndexOf("test") >= 0)

            End Using

        End Sub

        <Fact(DisplayName:="Should not set employee user field value if user field name is empty when create employee")>
        Sub ShouldNotSetEmployeeUserFieldValueIfUserFieldNameIsEmptyWhenCreateEmployee()

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
                oEmp.UserFields = New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() {New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = "", .UserFieldValue = "test", .UserFieldValueDate = New Date(2024, 7, 16)}}
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPassport.Update()
                helperPassport.UpdatePassportEnabledVTDesktop()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helper.SavedUserField.FieldName = "" AndAlso helper.SavedUserField.ListValuesString.IndexOf("test") >= 0)

            End Using

        End Sub

        <Fact(DisplayName:="Should set employee user field value if user field name is not empty when update employee")>
        Sub ShouldSetEmployeeUserFieldValueIfUserFieldNameIsNotEmptyWhenUpdateEmployee()

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
                oEmp.UserFields = New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() {New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = "test", .UserFieldValue = "test", .UserFieldValueDate = New Date(2024, 7, 16)}}
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 1
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helper.SavedUserField.FieldName = "test" AndAlso helper.SavedUserField.ListValuesString.IndexOf("test") >= 0)

            End Using

        End Sub

        <Fact(DisplayName:="Should not set employee user field value if user field name is empty when update employee")>
        Sub ShouldNotSetEmployeeUserFieldValueIfUserFieldNameIsEmptyWhenUpdateEmployee()

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
                oEmp.UserFields = New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() {New RoboticsExternAccess.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = "", .UserFieldValue = "test", .UserFieldValueDate = New Date(2024, 7, 16)}}
                Dim cCode As String = "test"
                Dim iResult As RoboticsExternAccess.Core.DTOs.ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                Dim iNewEmpId As Integer = 1
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IdEmployee"}, New Object()() {New Object() {1}})
                                                                                         ElseIf tableName = "Employees" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Contador"}, New Object()() {New Object() {DBNull.Value}})
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
                helper.SaveEmployeeSpy()
                helper.SaveEmployeeUserField()
                helper.SaveUserField()
                helper.GetCompanyByName()
                helper.SaveGroupUserField()
                helper.SaveMobility()
                helper.SaveContract()
                helper.GetEmployee()
                Dim passport As roPassport = New roPassport()
                helperPassport.PassportStub(1, helperDatalayer, passport)
                helperPassport.Update()
                helperDatalayer.StartTransaction()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateEmployee(oEmp, iResult, iNewEmpId)

                ' Assert
                Assert.Equal(False, helper.SavedUserField.FieldName = "" AndAlso helper.SavedUserField.ListValuesString.IndexOf("test") >= 0)

            End Using

        End Sub

    End Class

End Namespace