Imports System.ComponentModel
Imports Robotics
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.AccessPeriod
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Camera
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTBusiness.DiningRoom
Imports Robotics.Base.VTBusiness.EventScheduler
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTBusiness.Indicator
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.SQLServerHint
Imports Robotics.Security
Imports Robotics.Web.Base
Imports VT_XU_Base
Imports VT_XU_Security
Imports Xunit
Imports VTLiveApi
Imports System.Globalization
Imports Robotics.Base.VTSelectorManager

Namespace Unit.Test

    <CollectionDefinition("BusinessSupport", DisableParallelization:=True)>
    <Collection("BusinessSupport")>
    <Category("BusinessSupport")>
    Public Class BusinessSupportTest
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperPassport As PassportHelper
        Sub New()
            helperDataLayer = New DatalayerHelper()
            helperPassport = New PassportHelper()
        End Sub

        <Fact(DisplayName:="Must be empty if No intersection exists")>
        Public Sub TestNoIntersection()
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 8, 1)
            Dim endPeriodB As New Date(2023, 8, 15)

            Dim result As List(Of Date) = roBusinessSupport.GetPeriodsIntersection(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            Assert.Empty(result)
        End Sub

        <Fact(DisplayName:="Should give full intersection (beginPeriodB-endPeriodB)")>
        Public Sub TestFullIntersection()
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 5)
            Dim endPeriodB As New Date(2023, 7, 10)

            Dim result As List(Of Date) = roBusinessSupport.GetPeriodsIntersection(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            Dim expectedDates() As Date = {New Date(2023, 7, 5), New Date(2023, 7, 10)}
            Assert.Equal(expectedDates, result)
        End Sub

        <Fact(DisplayName:="Should give partial intersection at Start (beginPeriodB-endPeriodA)")>
        Public Sub TestPartialOverlapAtStart()
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 20)
            Dim beginPeriodB As New Date(2023, 7, 10)
            Dim endPeriodB As New Date(2023, 7, 25)

            Dim result As List(Of Date) = roBusinessSupport.GetPeriodsIntersection(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            Dim expectedDates() As Date = {New Date(2023, 7, 10), New Date(2023, 7, 20)}
            Assert.Equal(expectedDates, result)
        End Sub

        <Fact(DisplayName:="Should give partial intersection at Start (beginPeriodA-endPeriodB)")>
        Public Sub TestPartialOverlapAtEnd()
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 25)
            Dim beginPeriodB As New Date(2023, 6, 25)
            Dim endPeriodB As New Date(2023, 7, 20)

            Dim result As List(Of Date) = roBusinessSupport.GetPeriodsIntersection(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            Dim expectedDates() As Date = {New Date(2023, 7, 1), New Date(2023, 7, 20)}
            Assert.Equal(expectedDates, result)
        End Sub

        <Fact(DisplayName:="Should Return Original Period When No Overlap Between Original And Modified And Original Is Prior Than Modified")>
        Public Sub ShouldReturnOriginalPeriodWhenNoOverlapBetweenOriginalAndModifiedAndOriginalIsPriorThanModified()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 8, 1)
            Dim endPeriodB As New Date(2023, 8, 15)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 1)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, beginPeriodA)
            Assert.Equal(result(0).EndDateTimePeriod.Date, endPeriodA)

        End Sub

        <Fact(DisplayName:="Should Return Original Period When No Overlap Between Original And Modified And Original Is Later Than Modified")>
        Public Sub ShouldReturnOriginalPeriodWhenNoOverlapBetweenThisAndModifiedAndOriginalIsLaterThanModified()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 8, 1)
            Dim endPeriodB As New Date(2023, 8, 15)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 1)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, beginPeriodA)
            Assert.Equal(result(0).EndDateTimePeriod.Date, endPeriodA)

        End Sub

        <Fact(DisplayName:="Should Return Original Period Except Last Date When Overlap Includes Only Last Original End Date")>
        Public Sub ShouldReturnOriginalPeriodExceptLastDateWhenOverlapIncludesOnlyLastOriginalEndDate()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 15)
            Dim endPeriodB As New Date(2023, 7, 30)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 1)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, beginPeriodA)
            Assert.Equal(result(0).EndDateTimePeriod.Date, endPeriodA.AddDays(-1))

        End Sub

        <Fact(DisplayName:="Should Return One Period Starting On Begin Period Date And Ending The Day Before New Period Start When Only Start Date Is Modified And New Start Is Some Days After Original Start")>
        Public Sub ShouldReturnOnePeriodStartingOnBeginPeriodDateAndEndingTheDayBeforeNewPeriodStartWhenOnlyStartDateIsModifiedAndNewStartIsSomeDaysAfterOriginalStart()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 3)
            Dim endPeriodB As New Date(2023, 7, 15)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 1)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, beginPeriodA)
            Assert.Equal(result(0).EndDateTimePeriod.Date, beginPeriodB.AddDays(-1))

        End Sub


        <Fact(DisplayName:="Should Return One Period Starting On The Day After Modified Period End And Ending On Original Period End When Only End Is Modified And New End Is Before Original End")>
        Public Sub ShouldReturnOnePeriodStartingOnTheDayAfterModifiedPeriodEndAndEndingOnOriginalPeriodEndWhenOnlyEndIsModifiedAndNewEndIsBeforeOriginalEnd()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 1)
            Dim endPeriodB As New Date(2023, 7, 12)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 1)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, endPeriodB.AddDays(1))
            Assert.Equal(result(0).EndDateTimePeriod.Date, endPeriodA)

        End Sub

        <Fact(DisplayName:="Should Return Two Periods When Modified Period Is Fully Inside Original Period And Both Begin And End Date Are Modified")>
        Public Sub ShouldReturnTwoPeriodsWhenModifiedPeriodIsFullyInsideOriginalPeriodAndBothBeginAndEndDateAreModified()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 2)
            Dim endPeriodB As New Date(2023, 7, 14)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 2)
            Assert.Equal(result(0).BeginDateTimePeriod.Date, beginPeriodA)
            Assert.Equal(result(0).EndDateTimePeriod.Date, beginPeriodB.AddDays(-1))
            Assert.Equal(result(1).BeginDateTimePeriod.Date, endPeriodB.AddDays(1))
            Assert.Equal(result(1).EndDateTimePeriod.Date, endPeriodA)
        End Sub

        <Fact(DisplayName:="Should Return No Periods When Modified Period Fully Includes Original Period")>
        Public Sub ShouldReturnNoPeriodsWhenModifiedPeriodFullyIncludesOriginalPeriod()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 6, 30)
            Dim endPeriodB As New Date(2023, 7, 16)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 0)

        End Sub

        <Fact(DisplayName:="Should Return No Periods When Period Is Not Modified")>
        Public Sub ShouldReturnNoPeriodsWhenPeriodIsNotModified()
            ' Arrange
            Dim beginPeriodA As New Date(2023, 7, 1)
            Dim endPeriodA As New Date(2023, 7, 15)
            Dim beginPeriodB As New Date(2023, 7, 1)
            Dim endPeriodB As New Date(2023, 7, 15)

            ' Act
            Dim result As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(beginPeriodA, endPeriodA, beginPeriodB, endPeriodB)

            ' Assert
            Assert.Equal(result.Count, 0)

        End Sub

        <Fact(DisplayName:="Should fill 1 group when strSelectionList with 1 group is provided")>
        Public Sub ShouldFill1GroupWhenStrSelectionListWith1GroupIsProvided()
            Dim strSelection As String = "A160,B1,B3,B2"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstGroups.Count, 1)
        End Sub

        <Fact(DisplayName:="Should fill 0 groups when strSelectionList with 0 groups is provided")>
        Public Sub ShouldFill0GroupsWhenStrSelectionListWith0GroupsIsProvided()
            Dim strSelection As String = "B566"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstGroups.Count, 0)
        End Sub

        <Fact(DisplayName:="Should fill 3 groups when strSelectionList with 3 groups is provided")>
        Public Sub ShouldFill3GroupsWhenStrSelectionListWith3GroupsIsProvided()
            Dim strSelection As String = "A566,A31,A56,B31,321,42"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstGroups.Count, 3)
        End Sub

        <Fact(DisplayName:="Should fill 1 employee when strSelectionList with 1 employee is provided")>
        Public Sub ShouldFill1EmployeeWhenStrSelectionListWith1EmployeeIsProvided()
            Dim strSelection As String = "A160,B1,A123"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstEmployees.Count, 1)
        End Sub

        <Fact(DisplayName:="Should fill 0 employees when strSelectionList with 0 employees is provided")>
        Public Sub ShouldFill0EmployeesWhenStrSelectionListWith0EmployeesIsProvided()
            Dim strSelection As String = "A566"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstEmployees.Count, 0)
        End Sub

        <Fact(DisplayName:="Should fill 3 employees when strSelectionList with 3 employees is provided")>
        Public Sub ShouldFill3EmployeesWhenStrSelectionListWith3EmployeesIsProvided()
            Dim strSelection As String = "A566,A31,A56,B31,B321,B42"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstEmployees.Count, 3)
        End Sub

        <Fact(DisplayName:="Should fill 0 employees and 0 groups when strSelectionList is empty")>
        Public Sub ShouldFill0EmployeesAnd0GroupsWhenStrSelectionListIsEmpty()
            Dim strSelection As String = ""
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstEmployees.Count, 0)
            Assert.Equal(lstGroups.Count, 0)
        End Sub

        <Fact(DisplayName:="Should fill employees and groups when strSelectionList with employees and groups is provided")>
        Public Sub ShouldFillEmployeesAndGroupsWhenStrSelectionListWithEmployeesAndGroupsIsProvided()
            Dim strSelection As String = "A566,A31,A56,B31,B321"
            Dim lstEmployees As New List(Of Integer)
            Dim lstGroups As New List(Of Integer)

            roSelectorManager.ExtractIdsFromSelectionString(strSelection, lstEmployees, lstGroups)

            Assert.Equal(lstEmployees.Count, 2)
            Assert.Equal(lstGroups.Count, 3)
        End Sub

        <Fact(DisplayName:="Should return end of contract if end of period is greater than end of contract and the contract has finished ")>
        Public Sub ShouldReturnEndOfContractIfEndOfPeriodIsGreaterThanEndOfContractAndTheContractHasFinished()
            ' Arrange
            Dim contractDates As New List(Of Date) From {New Date(2022, 1, 1), New Date(2023, 12, 31)}
            Dim beginPeriod As New Date(2024, 1, 1)
            Dim endPeriod As New Date(2025, 12, 31)
            ' Act
            roBusinessSupport.SetPeriodWithContracts(contractDates, beginPeriod, endPeriod, "Y")
            ' Assert
            Assert.Equal(endPeriod, contractDates(1))
        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetDesktopAlerts_EmployeesWithNonJustifiedPunches")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetDesktopAlerts_EmployeesWithNonJustifiedPunches()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonJustifiedPunches.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonJustifiedPunches)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)


            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetDesktopAlerts_EmployeesWithNonReliablePunches")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetDesktopAlerts_EmployeesWithNonReliablePunches()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonReliablePunches.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesWithNonReliablePunches)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)


            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetDesktopAlerts_EmployeesThatShouldBePresent")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetDesktopAlerts_EmployeesThatShouldBePresent()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesThatShouldBePresent.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_EmployeesThatShouldBePresent)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)


            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass LoadRowsByCalendar")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassLoadRowsByCalendar()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.LoadRowsByCalendar.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.LoadRowsByCalendar)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetEmployeesStatus")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetEmployeesStatus()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesStatus.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesStatus)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetRequestNextSupervisorsNames")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetRequestNextSupervisorsNames()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetRequestNextSupervisorsNames.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestNextSupervisorsNames)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetEmployeesDashboardStatus")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetEmployeesDashboardStatus()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesDashboardStatus.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesDashboardStatus)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetUserFieldsUsedInShifts")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetUserFieldsUsedInShifts()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetUserFieldsUsedInShifts.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetUserFieldsUsedInShifts)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetAllEmployees")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetAllEmployees()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetAllEmployees.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetAllEmployees)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetDesktopAlerts_PendingRequests")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetDesktopAlerts_PendingRequests()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetDesktopAlerts_PendingRequests.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDesktopAlerts_PendingRequests)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetDirectSupervisorNotification")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetDirectSupervisorNotification()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetDirectSupervisorNotification.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetDirectSupervisorNotification)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetRequestsSupervisor")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetRequestsSupervisor()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetRequestsSupervisor.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestsSupervisor)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass DataLink_ExportRequests")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassDataLink_ExportRequests()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.DataLink_ExportRequests.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.DataLink_ExportRequests)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass ApproveRefuse")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassApproveRefuse()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.ApproveRefuse.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.ApproveRefuse)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetRequestPassportLevelOfAuthority")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetRequestPassportLevelOfAuthority()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetRequestPassportLevelOfAuthority.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestPassportLevelOfAuthority)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass HasPermissionOverRequest")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassHasPermissionOverRequest()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.HasPermissionOverRequest.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.HasPermissionOverRequest)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetForecastDocumentationFaultAlerts")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetForecastDocumentationFaultAlerts()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetForecastDocumentationFaultAlerts.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetForecastDocumentationFaultAlerts)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetRequestsDashboardResume")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetRequestsDashboardResume()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetRequestsDashboardResume.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestsDashboardResume)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150 string when pass GetEmployeeInfo")>
        Public Sub ShouldReturnSQLHintQUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150WhenPassGetEmployeeInfo()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeeInfo.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeeInfo)
                ' Assert
                Assert.Contains($"{SQLHint.QUERY_OPTIMIZER_COMPATIBILITY_LEVEL_150.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint empty string when pass GetEmployeeListByContract")>
        Public Sub ShouldReturnSQLHintEmptyWhenPassGetEmployeeInfo()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeeListByContract.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeeListByContract)
                ' Assert
                Assert.Equal("", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint empty string when pass GetEmployeeList")>
        Public Sub ShouldReturnSQLHintEmptyWhenPassGetEmployeeList()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeeList.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeeList)
                ' Assert
                Assert.Equal("", strHint)

            End Using

        End Sub
        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetEmployeesSQLWithPermissions")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetEmployeesSQLWithPermissions()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesSQLWithPermissions.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesSQLWithPermissions)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint FORCE_LEGACY_CARDINALITY_ESTIMATION string when pass GetEmployeesSQLWithoutPermissions")>
        Public Sub ShouldReturnSQLHintFORCE_LEGACY_CARDINALITY_ESTIMATIONWhenPassGetEmployeesSQLWithoutPermissions()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesSQLWithoutPermissions.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesSQLWithoutPermissions)
                ' Assert
                Assert.Contains($"{SQLHint.FORCE_LEGACY_CARDINALITY_ESTIMATION.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetEmployeesFromGroupWithPermissions")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetEmployeesFromGroupWithPermissions()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesFromGroupWithPermissions.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesFromGroupWithPermissions)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)

            End Using

        End Sub

        <Fact(DisplayName:="Should return SQLHint ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS string when pass GetEmployeesFromGroupWithTypeWithPermissions")>
        Public Sub ShouldReturnSQLHintASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERSWhenPassGetEmployeesFromGroupWithTypeWithPermissions()
            Dim strHint As String = String.Empty

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {SQLServerHint.SelectHinted.GetEmployeesFromGroupWithTypeWithPermissions.ToString, ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                strHint = SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeesFromGroupWithTypeWithPermissions)
                ' Assert
                Assert.Contains($"{SQLHint.ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS.ToString}", strHint)

            End Using

        End Sub
        <Fact(DisplayName:="Should Return True If IsXSSSafe pass Boolean Variable With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassBooleanVariableWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                bolResul = DataLayer.roSupport.IsXSSSafe(True)


                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass string Variable With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassStringVariableWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                bolResul = DataLayer.roSupport.IsXSSSafe("abcdefghgas")


                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass date Variable With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassDateVariableWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)

                ' Act
                bolResul = DataLayer.roSupport.IsXSSSafe(New Date(Now.Year, Now.Month, 1))
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roShift Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroShiftClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oShift As New roShift(1, New roShiftState(1))
                oShift.ID = 1
                oShift.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oShift)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roLabAgree Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroLabAgreeClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roLabAgree(1, New roLabAgreeState(1))
                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roGeniusScheduler Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroGeniusSchedulerClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roGeniusScheduler()
                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roBot Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroBotClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roBot()
                oClass.Id = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roBotRule Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroBotRuleClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roBotRule()
                oClass.Id = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roAccessGroup Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroAccessGroupClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roAccessGroup()
                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roAccessPeriod Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroAccessPeriodClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roAccessPeriod

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roAssignment Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroAssignmentClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roAssignment

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roBusinessCenter Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroBusinessCenterClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roBusinessCenter

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roCamera Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroCameraClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roCamera

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roCause Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroCauseClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roCause

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roConcept Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroConceptClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roConcept

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roConceptGroup Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroConceptGroupClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roConceptGroup

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roDiningRoom Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroDiningRoomClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roDiningRoom

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roDiningRoomTurn Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroDiningRoomTurnClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roDiningRoomTurn

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roEventScheduler Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroEventSchedulerClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roEventScheduler

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roGroup Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroGroupClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roGroup

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roIndicator Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroIndicatorClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roIndicator

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roProgrammedAbsence Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroProgrammedAbsenceClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roProgrammedAbsence

                oClass.IdAbsence = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roProgrammedCause Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroProgrammedCauseClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roProgrammedCause

                oClass.AbsenceID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roRemark Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroRemarkClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roRemark

                oClass.ID = 1
                oClass.Text = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roTask Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroTaskClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roTask

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roTaskTemplate Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroTaskTemplateClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roTaskTemplate

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roTerminal Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroTerminalClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roTerminal

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roZone Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroZoneClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roZone

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roChannel Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroChannelClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roChannel

                oClass.Id = 1
                oClass.Title = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roCommunique Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroCommuniqueClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roCommunique

                oClass.Id = 1
                oClass.Subject = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roDocumentTemplate Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroDocumentTemplateClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roDocumentTemplate

                oClass.Id = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roDocument Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroDocumentClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roDocument

                oClass.Id = 1
                oClass.Title = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roContract Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroContractClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roContract

                oClass.IDContract = "1"
                oClass.IDEmployee = 1

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roEmployee Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroEmployeeClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roEmployee

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roProgrammedHoliday Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroProgrammedHolidayClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roProgrammedHoliday

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roProgrammedOvertime Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroProgrammedOvertimeClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roProgrammedOvertime

                oClass.ID = 1
                oClass.Description = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roNotification Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroNotificationClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roNotification

                oClass.ID = 1
                oClass.Name = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass ReportPlannedExecution Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassReportPlannedExecutionClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New ReportPlannedExecution

                oClass.Id = 1
                oClass.CreatorName = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass Report Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassReportClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New Report

                oClass.Id = 1
                oClass.CreatorName = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roRequest Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroRequestClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roRequest

                oClass.ID = 1
                oClass.Comments = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roSurvey Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroSurveyClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roSurvey

                oClass.Id = 1
                oClass.Content = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roToDoList Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroToDoListClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roToDoList

                oClass.Id = 1
                oClass.Comments = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roToDoTask Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroToDoTaskClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roToDoTask

                oClass.Id = 1
                oClass.TaskName = "Test"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roEmployeeUserField Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroEmployeeUserFieldClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roEmployeeUserField

                oClass.FieldName = "test"
                oClass.FieldValue = "1"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roUserField Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroUserFieldClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roUserField

                oClass.FieldName = "test"
                oClass.Id = 1

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roBusinessCenterFieldDefinition Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroBusinessCenterFieldDefinitionClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roBusinessCenterFieldDefinition

                oClass.Name = "test"
                oClass.ID = 1

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass roTaskField Class With Valid Data")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassroTaskFieldClassWithValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roTaskField

                oClass.FieldName = "test"
                oClass.FieldValue = "1"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return False If IsXSSSafe pass roTaskField Class With not Valid Data")>
        Public Sub ShouldReturnFalseIfIsXSSSafePassroTaskFieldClassWithNotValidData()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                Dim oClass As New roTaskField

                oClass.FieldName = "'><img src=x onerror=prompt(document.cookie)>"
                oClass.FieldValue = "1"

                bolResul = DataLayer.roSupport.IsXSSSafe(oClass)
                ' Assert
                Assert.Equal(bolResul, False)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass Nothing Object")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassNothingObject()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                bolResul = DataLayer.roSupport.IsXSSSafe(Nothing)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return True If IsXSSSafe pass DBNull value")>
        Public Sub ShouldReturnTrueIfIsXSSSafePassDBNulValue()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                helperDataLayer.StartTransaction()
                helperPassport.PassportStub(1, helperDataLayer)

                'Arrange
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                Dim tMock As New DataTableMock With {.columns = {"ParameterName", "Value"},
                    .values = New Object()() {New Object() {"", ""}}}
                dDataTStub.Add("sysroLiveAdvancedParameters", tMock)
                helperDataLayer.CreateDataTableStub(dDataTStub)


                ' Act
                bolResul = DataLayer.roSupport.IsXSSSafe(DBNull.Value)
                ' Assert
                Assert.Equal(bolResul, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Version History With Ten Versions")>
        Public Sub ShouldReturnVersionHistoryWithTenVersions()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                'Arrange
                Dim result As Boolean = False
                Dim versionHistory As String()
                Dim currentVersion As String = ""
                Dim currentVersionDate As String = ""

                ' Act
                Dim resp As (Boolean, String, String, String())
                resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value
                result = resp.Item1
                currentVersion = resp.Item2
                currentVersionDate = resp.Item3
                versionHistory = resp.Item4

                ' Assert
                Assert.Equal(result AndAlso versionHistory.Length = 10, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Version History With Ten Versions And Current Version Should Be Included And Should Be The First One")>
        Public Sub ShouldReturnVersionHistoryWithTenVersionsAndCurrentVersionShouldBeIncludedAndShouldBeTheFirstOne()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                'Arrange
                Dim result As Boolean = False
                Dim versionHistory As String()
                Dim currentVersion As String = ""
                Dim currentVersionDate As String = ""

                ' Act
                Dim resp As (Boolean, String, String, String())
                resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value
                result = resp.Item1
                currentVersion = resp.Item2
                currentVersionDate = resp.Item3
                versionHistory = resp.Item4

                Dim currentVersionIncluded As Boolean = (versionHistory.ToList.First = $"{currentVersion} ({currentVersionDate})")

                ' Assert
                Assert.Equal(result AndAlso currentVersionIncluded, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Version History And Versions Should Be Properly Ordered")>
        Public Sub ShouldReturnVersionHistoryAndVersionsShouldBeProperlyOrdered()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                'Arrange
                Dim result As Boolean = False
                Dim versionHistory As String()
                Dim currentVersion As String = ""
                Dim currentVersionDate As String = ""

                ' Act
                Dim resp As (Boolean, String, String, String())
                resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value
                result = resp.Item1
                currentVersion = resp.Item2
                currentVersionDate = resp.Item3
                versionHistory = resp.Item4

                Dim versionDigit1 As Integer
                Dim versionDigit2 As Integer
                Dim versionDigit3 As Integer
                Dim versionDigit4 As Integer
                Dim versionNumberString As String
                Dim versionNumberInteger As Integer

                Dim tempVersionHistory As New List(Of Integer)
                For Each version As String In versionHistory
                    versionNumberString = version.Split(" ")(0)
                    versionDigit1 = Integer.Parse(versionNumberString.Split(".")(0))
                    versionDigit2 = Integer.Parse(versionNumberString.Split(".")(1))
                    versionDigit3 = Integer.Parse(versionNumberString.Split(".")(2))
                    versionDigit4 = Integer.Parse(versionNumberString.Split(".")(3))
                    versionNumberInteger = versionDigit1 * 1000000 + versionDigit2 * 10000 + versionDigit3 * 100 + versionDigit4
                    tempVersionHistory.Add(versionNumberInteger)
                Next

                'Compruebo si la lista está ordenada desdendentemente
                Dim ordered = tempVersionHistory.SequenceEqual(tempVersionHistory.OrderByDescending(Function(x) x))


                ' Assert
                Assert.Equal(result AndAlso ordered, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Version History And All Dates Has Proper Format", Skip:="FailsWhenExecutingPipe")>
        Public Sub ShouldReturnVersionHistoryAndAllDatesHasProperFormat()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                'Arrange
                Dim result As Boolean = False
                Dim versionHistory As String()
                Dim currentVersion As String = ""
                Dim currentVersionDate As String = ""

                ' Act
                Dim resp As (Boolean, String, String, String())
                resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value
                result = resp.Item1
                currentVersion = resp.Item2
                currentVersionDate = resp.Item3
                versionHistory = resp.Item4

                Dim versionDateString As String
                Dim allDatesProperlyFormatted As Boolean = True

                Dim counter As Integer = 0
                While allDatesProperlyFormatted AndAlso counter < versionHistory.Length
                    versionDateString = versionHistory(counter).Split(" ")(1).Replace("(", "").Replace(")", "")
                    allDatesProperlyFormatted = Date.TryParseExact(versionDateString, "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, Nothing)
                    counter += 1
                End While

                ' Assert
                Assert.Equal(result AndAlso allDatesProperlyFormatted, True)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Version History And Versions Dates Should Be Properly Ordered", Skip:="FailsWhenExecutingPipe")>
        Public Sub ShouldReturnVersionHistoryAndVersionsDatesShouldBeProperlyOrdered()
            Dim bolResul As Boolean = False

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()

                'Arrange
                Dim result As Boolean = False
                Dim versionHistory As String()
                Dim currentVersion As String = ""
                Dim currentVersionDate As String = ""

                ' Act
                Dim resp As (Boolean, String, String, String())
                resp = VTLiveApi.LicenseMethods.VersionInfo(currentVersion, currentVersionDate).Value
                result = resp.Item1
                currentVersion = resp.Item2
                currentVersionDate = resp.Item3
                versionHistory = resp.Item4

                Dim versionDateString As String
                Dim versionDateDate As Date

                Dim tempVersionHistory As New List(Of Date)
                For Each version As String In versionHistory
                    versionDateString = version.Split(" ")(1).Replace("(", "").Replace(")", "")
                    Date.TryParseExact(versionDateString, "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, versionDateDate)
                    tempVersionHistory.Add(versionDateDate)
                Next

                'Compruebo si la lista está ordenada desdendentemente
                Dim ordered = tempVersionHistory.SequenceEqual(tempVersionHistory.OrderByDescending(Function(x) x))

                ' Assert
                Assert.Equal(result AndAlso ordered, True)

            End Using

        End Sub

    End Class

End Namespace