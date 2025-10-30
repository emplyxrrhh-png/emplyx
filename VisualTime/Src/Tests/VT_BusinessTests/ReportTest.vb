Imports System.ComponentModel
Imports ReportGenerator.Services
Imports ReportGenerator.Support
Imports Robotics.Base
Imports Robotics.DataLayer
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Reports")>
    <CollectionDefinition("Reports", DisableParallelization:=True)>
    <Category("Reports")>
    Public Class ReportTest

        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperConcept As ConceptsHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperPermission As PermissionHelper

        Public Sub New()
            helperDatalayer = New DatalayerHelper()
            helperEmployee = New EmployeeHelper()
            helperConcept = New ConceptsHelper()
            helperPassport = New PassportHelper()
            helperPermission = New PermissionHelper()
        End Sub

        <Fact(DisplayName:="Should clean TMP table before execution")>
        Sub ShouldCleanTmpTableBeforeExecution()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{" AND StartupValue=1  AND CarryOver=1 AND IDConcept = ", 0}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "dbo.EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"groupname", "Fullgroupname", "IDEmployee", "idGroup", "EmployeeName", "IDContract", "BeginDateContract", "EndDateContract"}, New Object()() {New Object() {"Group 1", "Group 1 Full Name", 1, "1", "User 1", "1", New Date(2020, 2, 12), New DateTime(2079, 1, 1)}})
                                                                                         ElseIf tableName = "Concepts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID", "Name", "DefaultQuery"}, New Object()() {New Object() {1, "Year holiday concept", "Y"}})
                                                                                         ElseIf tableName = "AccrualsRules" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Shifts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function

                helperConcept.VacationsResumeQuerySpy()
                'Act
                Dim tmpTM As TemporalTablesManager = New TemporalTablesManager()
                Dim parametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {
                New ReportParameter With {.Type = "Robotics.Base.passportIdentifier", .Value = 0},
New ReportParameter With {.Type = "System.DateTime", .Value = New Date(2023, 8, 8)}}

                Dim originalParametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {
                New ReportParameter With {.Type = "Robotics.Base.employeesSelector", .Value = "B1229@11110@"},
                New ReportParameter With {.Type = "Robotics.Base.6passportIdentifier", .Value = 0},
New ReportParameter With {.Type = "System.DateTime", .Value = New Date(2023, 8, 8)}}
                tmpTM.ExecuteTask("TMPHOLIDAYSCONTROLBYCONTRACTV2", 1, parametersList.ToList(), originalParametersList.ToList())
                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.DeleteTmpHolidaysControlBycontract)
            End Using
        End Sub

        <Fact(DisplayName:="Should execute report at now if the selected date belongs to the current year for annual accruals")>
        Sub ShouldExecuteReportAtNowIfTheSelectedDateBelongsToTheCurrentYearForAnnualAccruals()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{" AND StartupValue=1  AND CarryOver=1 AND IDConcept = ", 0}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "dbo.EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"groupname", "Fullgroupname", "IDEmployee", "idGroup", "EmployeeName", "IDContract", "BeginDateContract", "EndDateContract"}, New Object()() {New Object() {"Group 1", "Group 1 Full Name", 1, "1", "User 1", "1", New Date(2020, 2, 12), New DateTime(2079, 1, 1)}})
                                                                                         ElseIf tableName = "Concepts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID", "Name", "DefaultQuery"}, New Object()() {New Object() {1, "Year holiday concept", "Y"}})
                                                                                         ElseIf tableName = "AccrualsRules" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Shifts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function

                helperConcept.VacationsResumeQuerySpy()
                'Act
                Dim tmpTM As TemporalTablesManager = New TemporalTablesManager()

                Dim parametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {
                New ReportParameter With {.Type = "Robotics.Base.passportIdentifier", .Value = 0},
                New ReportParameter With {.Type = "Robotics.Base.employeesSelector", .Value = {New Object() {1229}}},
                New ReportParameter With {.Type = "System.DateTime", .Value = New Date(DateTime.Now.Year, 8, 8)}}

                Dim originalParametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {}
                tmpTM.ExecuteTask("TMPHOLIDAYSCONTROLBYCONTRACTV2", 1, parametersList.ToList(), originalParametersList.ToList())
                'Assert
                Assert.True(helperConcept.VacationsResumeQueryCalled4CurrentDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should execute report at end of the last year if the selected date belongs to the last year for annual accruals")>
        Sub ShouldExecuteReportAtEndOfTheLastYearIfTheSelectedDateBelongsToTheLastYearForAnnualAccruals()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.ExecuteSqlSpy()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{" AND StartupValue=1  AND CarryOver=1 AND IDConcept = ", 0}})
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "dbo.EmployeeContracts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"groupname", "Fullgroupname", "IDEmployee", "idGroup", "EmployeeName", "IDContract", "BeginDateContract", "EndDateContract"}, New Object()() {New Object() {"Group 1", "Group 1 Full Name", 1, "1", "User 1", "1", New Date(2020, 2, 12), New DateTime(2079, 1, 1)}})
                                                                                         ElseIf tableName = "Concepts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID", "Name", "DefaultQuery"}, New Object()() {New Object() {1, "Year holiday concept", "Y"}})
                                                                                         ElseIf tableName = "AccrualsRules" Then
                                                                                             Return New DataTable()
                                                                                         ElseIf tableName = "Shifts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If

                                                                                     End Function

                helperConcept.VacationsResumeQuerySpy()
                'Act
                Dim tmpTM As TemporalTablesManager = New TemporalTablesManager()

                Dim parametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {
                New ReportParameter With {.Type = "Robotics.Base.passportIdentifier", .Value = 0},
                New ReportParameter With {.Type = "Robotics.Base.employeesSelector", .Value = {New Object() {1229}}},
                New ReportParameter With {.Type = "System.DateTime", .Value = DateTime.Now.Date.AddYears(-1)}}

                Dim originalParametersList As IEnumerable(Of ReportParameter) = New List(Of ReportParameter) From {}
                tmpTM.ExecuteTask("TMPHOLIDAYSCONTROLBYCONTRACTV2", 1, parametersList.ToList(), originalParametersList.ToList())
                'Assert
                Assert.True(helperConcept.VacationsResumeQueryCalled4EndOfLastYear)
            End Using
        End Sub





        <Fact(DisplayName:="Should get employees from BD when period are not provided and employees are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldGetEmployeesFromBDWhenPeriodAreNotProvidedAndEmployeesAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.CreateExecuteSQLSpy("\bsysrovwsecurity_permissionoveremployeeandfeature\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("B1229@11110@", 1, Nothing, Nothing)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.QueryUsersWithPermissions)
            End Using
        End Sub

        <Fact(DisplayName:="Should get employees from BD when period are provided and employees are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldGetEmployeesFromBDWhenPeriodAreProvidedAndEmployeesAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                Dim startDate As DateTime = New DateTime(2022, 2, 12)
                Dim endDate As DateTime = New DateTime(2023, 2, 11)
                helperDatalayer.CreateExecuteSQLSpy("\bsysrovwsecurity_permissionoveremployeeandfeature\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("B1229@11110@", 1, startDate, endDate)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.QueryUsersWithPermissions)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employees from BD when period are not provided and only groups are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldNotGetEmployeesFromBDWhenPeriodAreNotProvidedAndOnlyGroupsAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.CreateDataTableSpy("\bselect\b.*\bemployees\b.*\weblogin_getpermissionoveremployee\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("A12@11110@", 1, Nothing, Nothing)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled <> DatalayerHelper.CreateDataTableString.GeneralCreateDatatable)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employees from BD when period are provided and only groups are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldGetEmployeesFromBDWhenPeriodAreProvidedAndOnlyGroupsAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                Dim startDate As DateTime = New DateTime(2022, 2, 12)
                Dim endDate As DateTime = New DateTime(2023, 2, 11)
                helperDatalayer.CreateDataTableSpy("\bselect\b.*\bemployees\b.*\weblogin_getpermissionoveremployee\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("A12@11110@", 1, startDate, endDate)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled <> DatalayerHelper.CreateDataTableString.GeneralCreateDatatable)
            End Using
        End Sub

        <Fact(DisplayName:="Should get employees from groups from BD when period are not provided and only groups are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldGetEmployeesFromGroupsFromBDWhenPeriodAreNotProvidedAndOnlyGroupsAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.CreateExecuteSQLSpy("\bsysrovwsecurity_permissionoveremployeeandfeature\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("A123@11110@", 1, Nothing, Nothing)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.QueryUsersWithPermissions)
            End Using
        End Sub

        <Fact(DisplayName:="Should get employees from groups from BD when period are provided and only groups are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldGetEmployeesFromGroupsFromBDWhenPeriodAreProvidedAndOnlyGroupsAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                Dim startDate As DateTime = New DateTime(2022, 2, 12)
                Dim endDate As DateTime = New DateTime(2023, 2, 11)
                helperDatalayer.CreateExecuteSQLSpy("\bsysrovwsecurity_permissionoveremployeeandfeature\b")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("A123@11110@", 1, startDate, endDate)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.QueryUsersWithPermissions)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employees from groups when period are not provided and only employees are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldNotGetEmployeesFromGroupsWhenPeriodAreNotProvidedAndOnlyEmployeesAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperDatalayer.CreateDataTableWithoutTimeoutsSpy("id from groups where id = ")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("B333@11110@", 1, Nothing, Nothing)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled <> DatalayerHelper.CreateDataTableString.GeneralCreateDatatable)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employees from groups when period are provided and only employees are selected on Report Services GetEmployeesFromEncodedQuery")>
        Sub ShouldNotGetEmployeesFromGroupsWhenPeriodAreProvidedAndOnlyEmployeesAreSelectedOnReportServicesGetEmployeesFromEncodedQuery()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                Dim startDate As DateTime = New DateTime(2022, 2, 12)
                Dim endDate As DateTime = New DateTime(2023, 2, 11)
                helperDatalayer.CreateDataTableWithoutTimeoutsSpy("id from groups where id = ")
                helperPassport.PassportStub(1, helperDatalayer)

                'Act
                Dim service = New VisualTimeQueryDecoderService()
                service.GetEmployeesFromEncodedQuery("B123@11110@", 1, startDate, endDate)

                'Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled <> DatalayerHelper.CreateDataTableString.GeneralCreateDatatable)
            End Using
        End Sub

        <Fact(DisplayName:="Should list report if I created the report")>
        Sub ShouldListReportIfCreatedByMe()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)

                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.CreatorPassportId = 1

                ' Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)

                ' Assert
                Assert.True(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should list report with RequiredFunctionalities null if user doesn't have permission over feature x")>
        Sub ShouldListReportsWithRequiredFunctionalitiesNullIfUserDontHavePermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(False)

                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = Nothing

                ' Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)

                ' Assert
                Assert.True(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should list report with RequiredFunctionalities null if user has permission over feature x")>
        Sub ShouldListReportsWithRequiredFunctionalitiesNullIfUserHasPermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(True)
                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = Nothing

                'Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)
                'Assert
                Assert.True(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should list reports with empty RequiredFunctionalities if user dont has permission over feature x")>
        Sub ShouldListReportsWithEmptyRequieredFeatureIfUserDontHasPermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(False)
                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = ""

                'Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)
                'Assert
                Assert.True(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should list reports with RequiredFunctionalities empty feature if user has permission over feature x")>
        Sub ShouldListReportsWithRequiredFunctionalitiesEmptyFeatureIfUserHasPermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(True)
                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = ""

                'Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)
                'Assert
                Assert.True(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should not list reports with x requiered feature if user dont has permission over feature x")>
        Sub ShouldNotListReportsWithXRequiredFunctionalitiesIfUserDoesntHavePermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(False)
                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = "U:Calendar=Read"

                'Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)
                'Assert
                Assert.False(hasPermission)
            End Using
        End Sub

        <Fact(DisplayName:="Should list reports with x requiered feature if user has permission over feature x")>
        Sub ShouldListReportsWithXRequieredFeatureIfUserHasPermissionOverFeatureX()
            'Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperPassport.PassportStub(1, helperDatalayer)
                helperPermission.HasPermissionFake(True)
                Dim report As New Report()
                report.Name = "Report with Calendar Feature"
                report.RequiredFunctionalities = "U:Calendar=Read"

                'Act
                Dim permissionService As IReportPermissionService = New ReportPermissionServiceV3()
                Dim hasPermission As Boolean = permissionService.GetPermissionOverReportAction(1, report, ReportPermissionTypes.Read)
                'Assert
                Assert.True(hasPermission)
            End Using
        End Sub

    End Class

End Namespace