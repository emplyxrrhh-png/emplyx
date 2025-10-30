Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs
Imports Xunit
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports System.Data.Common
Imports VT_XU_Datalink

Namespace Unit.Test

    <Collection("DailyCauses")>
    <CollectionDefinition("DailyCauses", DisableParallelization:=True)>
    <Category("DailyCause")>
    Public Class DailyCausesTest

        Private ReadOnly helper As DailyCausesHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperWeb As WebHelper
        Private ReadOnly helperEmployees As EmployeeHelper
        Private ReadOnly helperDatalink As DatalinkHelper
        Private ReadOnly helperBusiness As BusinessHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helper = New DailyCausesHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDatalayer = New DatalayerHelper
            helperPassport = New PassportHelper
            helperBase = New BaseHelper
            helperWeb = New WebHelper
            helperEmployees = New EmployeeHelper
            helperDatalink = New DatalinkHelper
            helperBusiness = New BusinessHelper
        End Sub


        <Fact(DisplayName:="Should create or update daily cause with manual equals true if manual is empty")>
        Sub ShouldCreateOrUpdateDailyCauseWithManualEqualsTrueIfManualIsEmpty()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "60"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helper.SaveDailyCauseSpy()
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(2)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                Robotics.DataLayer.Fakes.ShimAccessHelper.ExecuteSqlStringroBaseConnection =
                    Function(strSQL As String, oConn As roBaseConnection)
                        Return True
                    End Function
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.CreateOrUpdateDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, helper.SaveDailyCauseCalled AndAlso helper.ManualDailyCause)

            End Using

        End Sub

        <Fact(DisplayName:="Should delete manual daily cause if value is zero")>
        Sub ShouldDeleteManualDailyCauseIfValueIsZero()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "0"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(2)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                helper.LoadWithParamsSpy(5)
                helper.DeleteDailyCauseSpy()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.DeleteDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, helper.DeleteDailyCauseCalled AndAlso helper.ManualDailyCause)

            End Using

        End Sub

        <Fact(DisplayName:="Should delete manual causes if value is zero")>
        Sub ShouldDeleteManualCausesIfValueIsZero()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "0"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(5)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                helper.LoadWithParamsSpy(0)
                helper.DeleteDailyCauseSpy()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.DeleteDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, helper.DeleteDailyCauseCalled AndAlso helper.ManualDailyCause)

            End Using

        End Sub

        <Fact(DisplayName:="Should delete manual causes created or updated by users")>
        Sub ShouldDeleteManualCausesCreatedOrUpdatedByUsers()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "0"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(5)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                helper.LoadWithParamsSpy(5)
                helper.DeleteDailyCauseSpy()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.DeleteDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, helper.DeleteDailyCauseCalled AndAlso helper.ManualDailyCause AndAlso Not helper.CheckUser)

            End Using

        End Sub

        <Fact(DisplayName:="Should delete manual causes with zero value if value is zero")>
        Sub ShouldDeleteManualCausesWithZeroValueIfValueIsZero()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "0"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(2)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                helper.LoadWithParamsSpy(0)
                helper.DeleteDailyCauseSpy()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.DeleteDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, helper.DeleteDailyCauseCalled AndAlso helper.ManualDailyCause)

            End Using

        End Sub

        <Fact(DisplayName:="Should return error if try to delete not manual cause or cause not exists")>
        Sub ShouldReturnErrorIfTryToDeleteNotManualCauseOrCauseNotExists()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oCause As roDatalinkDailyCause = New roDatalinkDailyCause
                oCause.CauseDate = Now
                oCause.ShortCauseName = "test"
                oCause.Value = "0"
                oCause.UniqueEmployeeID = "1"
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "sysroUserFields" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"FieldName", "FieldValue"}, {})
                                                                                         ElseIf tableName = "dbo.GetAllEmployeeUserFieldValue" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"IDContract", "IDEmployee", "BeginDate", "EndDate", "IDCard", "Enterprise", "Telecommuting", "TelecommutingMandatoryDays", "PresenceMandatoryDays", "TelecommutingOptionalDays", "TelecommutingMaxDays", "TelecommutingMaxPercentage", "PeriodType", "TelecommutingAgreementStart", "TelecommutingAgreementEnd", "EndContractReason"}, New Object()() {New Object() {1, 1, DateTime.Now.AddYears(-1), New DateTime(2079, 1, 1), 0, "", False, "", "", "", 0, 0, 0, Nothing, Nothing, ""}})
                                                                                         End If

                                                                                     End Function
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM EmployeeContracts", 1}})




                Fakes.ShimDbCommand.AllInstances.ExecuteNonQueryAsync = Function(x) Task.FromResult(1)
                helperPassport.PassportStub(1, helperDatalayer)
                helperDatalayer.StartTransaction()
                helperDatalink.isEmployeeNewStub(2)
                helperBusiness.GetEmployeeLockDatetoApplyStub(Date.Now.AddDays(-1))
                helper.GetCauseByShortName()
                helper.DeleteDailyCauseSpy()
                CommonHelper.InitTask()

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                externAccessInstance.DeleteDailyCause(oCause, iResult)

                ' Assert
                Assert.Equal(True, Not helper.DeleteDailyCauseCalled AndAlso iResult = ReturnCode._ManualCauseNotExists)

            End Using

        End Sub

        <Fact(DisplayName:="Should return all users daily causes if employee is not seted")>
        Sub ShouldReturnAllUsersDailyCausesIfEmployeeIsNotSeted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
                                                                                                                        New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
                                                                                                                        New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
                                                                                                                        New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = ""

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return daily causes for multiple users")>
        Sub ShouldReturnDailyCausesForMultipleUsers()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1;2"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return daily causes for single user")>
        Sub ShouldReturnDailyCausesForSingleUser()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return all users daily causes by timestamp if employee is not seted")>
        Sub ShouldReturnAllUsersDailyCausesByTimestampIfEmployeeIsNotSeted()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.Timestamp = New Date(2024, 1, 1)
                oCriteria.UniqueEmployeeID = ""

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return daily causes by timestamp for multiple users")>
        Sub ShouldReturnDailyCausesByTimestampForMultipleUsers()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.Timestamp = New Date(2024, 1, 1)
                oCriteria.UniqueEmployeeID = "1;2"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return daily causes by timestamp for single user")>
        Sub ShouldReturnDailyCausesByTimestampForSingleUser()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.Timestamp = New Date(2024, 1, 1)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should return rounded daily cause value for repeating decimal numbers when get causes by timestamp")>
        Sub ShouldReturnRoundedDailyCauseValueForRepeatingDecimalNumbersWhenGetCausesByTimestamp()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 2.333333, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.Timestamp = New Date(2024, 1, 1)
                oCriteria.UniqueEmployeeID = "1;2"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.FirstOrDefault().CauseValue = 140)

            End Using

        End Sub

        <Fact(DisplayName:="Should return rounded daily cause value for repeating decimal numbers when get causes")>
        Sub ShouldReturnRoundedDailyCauseValueForRepeatingDecimalNumbersWhenGetCauses()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 4.066667, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()

                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1;2"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.FirstOrDefault().CauseValue = 244)

            End Using

        End Sub

        <Fact(DisplayName:="Should return ID daily incidence for daily causes for single user")>
        Sub ShouldReturnIDDailyIncidenceForDailyCausesForSingleUser()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).Incidence = 101)

            End Using

        End Sub

        <Fact(DisplayName:="Should return daily incidence info for daily causes for single user with Add Related incidence Active")>
        Sub ShouldReturnDailyIncidenceInfoForDailyCausesForSingleUserWithAddRelatedIncidenceActive()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If
                                                                                                        If tableName = "DailyIncidences" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"101", "2024-04-15", 1001, 1, 10.5, "2024-04-15", "2024-04-15", "Zona 1"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDIncidence", "Date", "IDType", "IDZone", "total", "BeginTime", "EndTime", "ZoneName"}, values)
                                                                                                        End If


                                                                                                    End Function


                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"
                oCriteria.AddRelatedIncidence = True


                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).IncidenceData IsNot Nothing)

            End Using

        End Sub

        <Fact(DisplayName:="Should not return daily incidence info for daily causes for single user with Add Related incidence no active")>
        Sub ShouldNotReturnDailyIncidenceInfoForDailyCausesForSingleUserWithAddRelatedIncidenceNoActive()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If


                                                                                                    End Function


                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"
                oCriteria.AddRelatedIncidence = False


                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).IncidenceData Is Nothing)

            End Using

        End Sub

        <Fact(DisplayName:="Should return equivalence code for daily causes")>
        Sub ShouldReturnEquivalenceCodeForDailyCauses()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, "Equivalence Code"}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).CauseEquivalenceCode = "Equivalence Code")

            End Using

        End Sub

        <Fact(DisplayName:="Should return equivalence code for daily causes with empty equivalence code")>
        Sub ShouldReturnEquivalenceCodeForDailyCausesWithEmptyEquivalenceCode()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).CauseValue = 630 AndAlso oCausesResponse.Causes(0).CauseEquivalenceCode = "")

            End Using

        End Sub

        <Fact(DisplayName:="Should return equivalence code for daily causes when equivalence code is nothing")>
        Sub ShouldReturnEquivalenceCodeForDailyCausesWhenEquivalenceCodeIsNothing()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, Nothing}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.EndCausePeriod = New Date(2024, 1, 1)
                oCriteria.StartCausePeriod = New Date(2024, 1, 31)
                oCriteria.UniqueEmployeeID = "1"

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes(0).CauseValue = 630 AndAlso oCausesResponse.Causes(0).CauseEquivalenceCode = Nothing)

            End Using

        End Sub

        <Fact(DisplayName:="Should take into account time when returning causes with timestamp criteria")>
        Sub ShouldTakeIntoAccountTimeWhenReturningCausesWithTimestampCriteria()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" AndAlso strQuery.IndexOf("DailySchedule.TimestampEngine >= CONVERT(SMALLDATETIME,'2024/07/24 10:00 ',120) and DailySchedule.TimestampEngine <=CONVERT(SMALLDATETIME,'2024/07/24 11:00 ',120)") > 0 Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.ShowChangesInPeriod = True
                oCriteria.UniqueEmployeeID = "1"
                oCriteria.StartCausePeriod = New DateTime(2024, 7, 24, 10, 0, 0)
                oCriteria.EndCausePeriod = New DateTime(2024, 7, 24, 11, 0, 0)

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

        <Fact(DisplayName:="Should take into account time when returning causes without timestamp criteria")>
        Sub ShouldTakeIntoAccountTimeWhenReturningCausesWithoutTimestampCriteria()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                Dim cCode As String = "test"
                Dim iResult As ReturnCode = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                helperBusiness.AdvancedParameterStub(New Dictionary(Of String, String) From {{"ImportPrimaryKeyUserField", "1"}})
                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "DailyCauses" AndAlso strQuery.IndexOf("DailySchedule.TimestampEngine >= CONVERT(SMALLDATETIME,'2024/07/24 10:00 ',120) and DailySchedule.TimestampEngine <=CONVERT(SMALLDATETIME,'2024/07/24 11:00 ',120)") = -1 Then
                                                                                                            Dim values As Object()() = {
    New Object() {"123456789", "ABC123", "XYZ789", 1001, 10.5, "ShortName1", "Name1", "2024-04-15", 1, 101, ""},
    New Object() {"987654321", "DEF456", "UVW123", 1002, 20.3, "ShortName2", "Name2", "2024-04-14", 0, 102, ""},
    New Object() {"555555555", "GHI789", "QRS456", 1003, 15.7, "ShortName3", "Name3", "2024-04-13", 1, 103, ""}}
                                                                                                            Return helperDatalayer.CreateDataTableMock({"IDCause", "NIF", "IdImport", "idemployee", "total", "ShortName", "Name", "Date", "Manual", "IDRelatedIncidence", "Export"}, values)


                                                                                                        End If

                                                                                                    End Function
                helperEmployees.GetIDEmployeesFromUserFieldValueStub()
                Dim oCriteria As New roDatalinkStandarDailyCauseCriteria
                oCriteria.ShowChangesInPeriod = False
                oCriteria.UniqueEmployeeID = "1"
                oCriteria.StartCausePeriod = New DateTime(2024, 7, 24, 10, 0, 0)
                oCriteria.EndCausePeriod = New DateTime(2024, 7, 24, 11, 0, 0)

                'Act
                Dim externAccessInstance As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess = Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess.GetInstance(True, cCode)
                Dim oCausesResponse As New roDatalinkStandarCauseResponse
                externAccessInstance.GetCauses(oCriteria, oCausesResponse)

                ' Assert
                Assert.Equal(True, oCausesResponse.Causes.Count = 3)

            End Using

        End Sub

    End Class

End Namespace
