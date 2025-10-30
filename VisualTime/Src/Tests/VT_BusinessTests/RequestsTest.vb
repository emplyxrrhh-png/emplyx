Imports System.ComponentModel
Imports Moq
Imports System.Data.Common
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTRequests.Requests
Imports VT_XU_Base
Imports VT_XU_Common
Imports Xunit
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Base.VTDailyRecord
Imports Robotics.Base
Imports System.Web.WebPages
Imports VT_XU_Security
Imports VTLiveApi
Imports Robotics.Base.VTBusiness
Imports Robotics.VTBase

Namespace Unit.Test

    <Collection("Requests")>
    <CollectionDefinition("Requests", DisableParallelization:=True)>
    <Category("Requess")>
    Public Class RequestsTest

        Private ReadOnly helperEmployees As EmployeeHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperRequest As RequestsHelper
        Private ReadOnly helperTerminal As TerminalsHelper
        Private ReadOnly helperPunch As PunchHelper
        Private ReadOnly helperSecurity As SecurityHelper
        Private ReadOnly helperNotification As NotificationsHelper
        Private ReadOnly helperBusiness As BusinessHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperEmployees = New EmployeeHelper
            helperDatalayer = New DatalayerHelper
            helperPunches = New PunchHelper
            helperBase = New BaseHelper
            helperPassport = New PassportHelper
            helperRequest = New RequestsHelper
            helperTerminal = New TerminalsHelper
            helperPunch = New PunchHelper
            helperSecurity = New SecurityHelper
            helperNotification = New NotificationsHelper
            helperBusiness = New BusinessHelper
        End Sub

        <Fact(DisplayName:="Should Check Annual Laboral Period When Enjoy Holidays Rule Configured As N Months", Skip:="To be done ...")>
        Sub ShouldCheckAnnualLaboralPeriodWhenEnjoyHolidaysRuleConfiguredAsNMonths()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = Now.Date
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, Now.Date, Nothing)}
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1902, 9, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1902, 9, 1)
                helperEmployees.GetDatesOfAnnualWorkPeriodsInDateSpy()

                'Act
                Dim ostate As New roRequestState
                roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.True(helperEmployees.GetDatesOfAnnualWorkPeriodsInDateCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Check Annual Laboral Period When Enjoy Holidays Rule Configured As Period Of Dates")>
        Sub ShouldNotCheckAnnualLaboralPeriodWhenEnjoyHolidaysRuleConfiguredAsPeriodOfDates()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = Now.Date
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, Now.Date, Nothing)}
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1901, 1, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1901, 12, 31)
                helperEmployees.GetPeriodDatesByContractAtDateSpy()

                'Act
                Dim ostate As New roRequestState
                roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.True(Not helperEmployees.GetPeriodDatesByContractAtDateCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Prevent Holiday Request When Annual Laboral Shift Used And Enjoy Rule Configured As N Months And Requested Date Exceeds This Limit")>
        Sub ShouldPreventHolidayRequestWhenAnnualLaboralShiftUsedAndEnjoyRuleConfiguredAsNMonthsAndRequestedDateExceedsThisLimit()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = Now.Date
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, Now.Date.AddMonths(2), Nothing)}
                Dim nMonthsForEnjoy As Integer = 1
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1902, nMonthsForEnjoy, 1) '1 month for enjoyment
                oRequestRuleDefinition.EndPeriod = New DateTime(1902, nMonthsForEnjoy, 1)
                Dim lstPeriods As New List(Of List(Of Date))
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.Year, Now.Month, 1), (New DateTime(Now.Year, Now.Month, 1)).AddYears(1).AddDays(-1)})
                helperEmployees.GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods)

                'Act
                Dim ostate As New roRequestState
                Dim canEnjoy As Boolean = True
                canEnjoy = roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.False(canEnjoy)
            End Using
        End Sub

        <Fact(DisplayName:="Should Allow Holiday Request When Annual Laboral Shift Used And Enjoy Rule Configured As N Months And Requested Date Not Exceed This limit", Skip:="To be done ...")>
        Sub ShouldAllowHolidayRequestWhenAnnualLaboralShiftUsedAndEnjoyRuleConfiguredAsNMonthsAndRequestedDateIsNearLimit()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = Now.Date
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, Now.Date.AddMonths(2), Nothing)}
                Dim nMonthsForEnjoy As Integer = 5
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1902, nMonthsForEnjoy, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1902, nMonthsForEnjoy, 1)
                Dim lstPeriods As New List(Of List(Of Date))
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.Year, Now.Month, 1), (New DateTime(Now.Year, Now.Month, 1)).AddYears(1).AddDays(-1)})
                helperEmployees.GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods)

                'Act
                Dim ostate As New roRequestState
                Dim canEnjoy As Boolean = True
                canEnjoy = roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.True(canEnjoy)
            End Using
        End Sub

        <Fact(DisplayName:="Should Prevent Holiday Request When Annual Shift Used And Enjoy Rule Configured As Period Of Dates And Requested Date Is Out Of This period")>
        Sub ShouldPreventHolidayRequestWhenAnnualShiftUsedAndEnjoyRuleConfiguredAsPeriodOfDatesAndRequestedDateIsOutOfThisPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = Now.Date
                Dim requestedDate As Date = Now.Date.AddMonths(4)
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, requestedDate, Nothing)}
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1901, Now.Date.AddMonths(-1).Month, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1902, requestedDate.AddDays(-1).Month, requestedDate.AddDays(-1).Day)

                'Act
                Dim ostate As New roRequestState
                Dim canEnjoy As Boolean = True
                canEnjoy = roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.False(canEnjoy)
            End Using
        End Sub

        <Fact(DisplayName:="Should Allow Holiday Request When Annual Shift Used And Enjoy Rule Configured As Period Of Dates Of Current Year And Requested Date Is Inside This period")>
        Sub ShouldAllowHolidayRequestWhenAnnualShiftUsedAndEnjoyRuleConfiguredAsPeriodOfDatesOfCurrentYearAndRequestedDateIsInsideThisPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange

                Dim checkDate As DateTime = New DateTime(DateTime.Now.Year, 8, 1, 0, 0, 0)

                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = checkDate.Date
                Dim requestedDate As Date = checkDate.Date.AddMonths(4)
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, requestedDate, Nothing)}
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1900, checkDate.AddMonths(-1).Month, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1900, requestedDate.Month, requestedDate.Day)

                'Act
                Dim ostate As New roRequestState
                Dim canEnjoy As Boolean = True
                canEnjoy = roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.True(canEnjoy)
            End Using
        End Sub

        <Fact(DisplayName:="Should Allow Holiday Request When Annual Shift Used And Enjoy Rule Configured As Period Of Dates Of Next Year And Requested Date Is Inside This Future period")>
        Sub ShouldAllowHolidayRequestWhenAnnualShiftUsedAndEnjoyRuleConfiguredAsPeriodOfDatesOfNextYearAndRequestedDateIsInsideThisFuturePeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()

                Dim checkDate As DateTime = New DateTime(DateTime.Now.Year, 8, 1, 0, 0, 0)
                'Arrange
                Dim oRequest As New roRequest
                oRequest.IDEmployee = 1
                oRequest.ID = 1
                oRequest.Date1 = checkDate.Date
                Dim requestedDate As Date = checkDate.Date.AddYears(1).AddMonths(4)
                oRequest.RequestDays = New List(Of roRequestDay) From {New roRequestDay(1, requestedDate, Nothing)}
                Dim oRequestRuleDefinition As New roRequestRuleDefinition
                oRequestRuleDefinition.BeginPeriod = New DateTime(1901, checkDate.Date.AddYears(1).AddMonths(-1).Month, 1)
                oRequestRuleDefinition.EndPeriod = New DateTime(1901, requestedDate.Month, requestedDate.Day)

                'Act
                Dim ostate As New roRequestState
                Dim canEnjoy As Boolean = True
                canEnjoy = roRequestRuleManager.ValidatePeriodEnjoynment(oRequest, oRequestRuleDefinition, ostate)

                'Assert
                Assert.True(canEnjoy)
            End Using
        End Sub

        <Fact(DisplayName:="Should check if passing requested day on annual work concept returns any pending days calculating vacations resume ")>
        Sub ShouldCheckifPassingRequestedDayonAnnualWorkConceptReturnsAnyPendingdays()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts", "L"}})
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteReaderStub()

                'Act
                Dim intDone As Double
                Dim intPending As Double
                Dim intLasting As Double
                Dim intDisponible As Double

                Dim lstRequestDates = New List(Of Date)
                lstRequestDates.Add(Now.Date)

                roBusinessSupport.VacationsResumeQuery(1, 1, Now.Date, Now.Date, Now.AddDays(5), Now.Date, intDone, intPending, intLasting, intDisponible, New roRequestState, 0, 0,, lstRequestDates)

                'Assert
                Assert.True(intPending > 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should check if default query is L return list of Summary Holidays by period")>
        Sub ShouldCheckifDefaultQueryIsLReturnListofSummaryHolidaysbyPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "DailyAccruals" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"BeginPeriod", "EndPeriod", "Total", "StartupValueTotal", "ExpiredDate", "StartEnjoymentDate"}, New Object()() {New Object() {Now.Date, Now.Date, 1, 1, Now.Date, Now.Date}})
                                                                                         End If
                                                                                     End Function

                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts", "L"}})
                helperDatalayer.CreateCommandStub()

                'Act
                Dim intDone As Double
                Dim intPending As Double
                Dim intLasting As Double
                Dim intDisponible As Double
                Dim xBeginPeriod As Date = Now.Date.AddYears(-5)
                Dim xEndPeriod As Date = Now.Date.AddYears(5)

                Dim lstHolidaysSummaryByPeriod As New Generic.List(Of roHolidaysSummaryByPeriod)
                roBusinessSupport.GetAvailableHolidays(1, 1, xBeginPeriod, xEndPeriod, New roRequestState, "L", , lstHolidaysSummaryByPeriod)

                'Assert
                Assert.True(lstHolidaysSummaryByPeriod.Count > 0)
            End Using
        End Sub


        <Fact(DisplayName:="Should check if default query is L return list of Holidays pending days")>
        Sub ShouldCheckifDefaultQueryIsLReturnListofHolidaysPendingDays()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                         Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                         If tableName = "Requests" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"Date"}, New Object()() {New Object() {Now.Date}})
                                                                                         ElseIf tableName = "Shifts" Then
                                                                                             Return helperDatalayer.CreateDataTableMock({"ID"}, New Object()() {New Object() {1}})
                                                                                         End If
                                                                                     End Function

                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"@SELECT# ISNULL(DefaultQuery , 'Y') FROM Concepts", "L"}})
                helperDatalayer.CreateCommandStub()

                'Act
                Dim intPending As Double
                Dim xEndPeriod As Date = Now.Date.AddYears(5)

                Dim lstHolidaysPendingDays As New Generic.List(Of roHolidaysDay)
                intPending = roBusinessSupport.GetPendingApprovalHollidays(1, 1, Nothing, xEndPeriod, True, New roRequestState, lstHolidaysPendingDays)

                'Assert
                Assert.True(lstHolidaysPendingDays.Count > 0)
            End Using
        End Sub


        <Fact(DisplayName:="Should save punches with not reliable cause on daily record request")>
        Sub ShouldSavePunchesWithNotReliableCauseOnDailyRecordRequest()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()

                'Act
                Dim dailyRecord As roDailyRecord = New roDailyRecord
                dailyRecord.RecordDate = Now.Date
                Dim dailyRecordPunchIn As roDailyRecordPunch = New roDailyRecordPunch
                dailyRecordPunchIn.IdDailyRecord = 1
                dailyRecordPunchIn.IdEmployee = 1
                dailyRecordPunchIn.DateTime = New DateTime(2025, 1, 1, 8, 0, 0)
                dailyRecordPunchIn.Type = PunchTypeEnum._IN
                Dim dailyRecordPunchOut As roDailyRecordPunch = New roDailyRecordPunch
                dailyRecordPunchOut.IdDailyRecord = 1
                dailyRecordPunchOut.IdEmployee = 1
                dailyRecordPunchOut.DateTime = New DateTime(2025, 1, 1, 9, 0, 0)
                dailyRecord.Punches = {dailyRecordPunchIn, dailyRecordPunchOut}
                dailyRecordPunchOut.Type = PunchTypeEnum._OUT
                Dim oDailyRecordManager As roDailyRecordManager = New roDailyRecordManager(New roDailyRecordState(1))
                Dim saved As Boolean = oDailyRecordManager.SaveDailyRecord(dailyRecord, requestDate, acceptWarning, oSupervisor, True)

                'Assert
                Assert.True(saved AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.DailyRecord.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with not reliable cause on forgotten punch request")>
        Sub ShouldSavePunchWithNotReliableCauseOnForgottenPunchRequest()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = 1200
                oPunchTest.IDEmployee = 1
                oPunchTest.ShiftDate = New DateTime(2023, 1, 1)
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Type = DTOs.PunchTypeEnum._IN
                oPunchTest.ActualType = DTOs.PunchTypeEnum._IN
                helperPunch.PunchesTableFromAdapterStub(oPunchTest)
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(2)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, New Object()() {New Object() {1, DBNull.Value, 1, DBNull.Value, DBNull.Value, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, False, False, 1, DBNull.Value}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForbiddenPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,, )

                'Assert
                Assert.True(response.Result AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.ForgottenPunch.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch remarks on forgotten punch request if remarks are setted")>
        Sub ShouldSavePunchRemarksOnForgottenPunchRequestIfRemarksAreSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = 1200
                oPunchTest.IDEmployee = 1
                oPunchTest.ShiftDate = New DateTime(2023, 1, 1)
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Type = DTOs.PunchTypeEnum._IN
                oPunchTest.ActualType = DTOs.PunchTypeEnum._IN
                helperPunch.PunchesTableFromAdapterStub(oPunchTest)
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(2)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, New Object()() {New Object() {1, DBNull.Value, 1, DBNull.Value, DBNull.Value, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, False, False, 1, DBNull.Value}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForbiddenPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,, "Forgotten punch remarks")

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals("Forgotten punch remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch without remarks on forgotten punch request if remarks are not setted")>
        Sub ShouldSavePunchWithoutRemarksOnForgottenPunchRequestIfRemarksAreNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = 1200
                oPunchTest.IDEmployee = 1
                oPunchTest.ShiftDate = New DateTime(2023, 1, 1)
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Type = DTOs.PunchTypeEnum._IN
                oPunchTest.ActualType = DTOs.PunchTypeEnum._IN
                helperPunch.PunchesTableFromAdapterStub(oPunchTest)
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(2)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, New Object()() {New Object() {1, DBNull.Value, 1, DBNull.Value, DBNull.Value, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, False, False, 1, DBNull.Value}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForbiddenPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,,)

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with not reliable cause on cost center forgotten punch request")>
        Sub ShouldSavePunchWithNotReliableCauseOnCostCenterForgottenPunchRequest()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(12)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForgottenCostCenterPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,, )

                'Assert
                Assert.True(response.Result AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.CostCenterForgottenPunch.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with remarks on cost center forgotten punch request if remarks are setted")>
        Sub ShouldSavePunchWithRemarksOnCostCenterForgottenPunchRequestIfRemarksAreSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(12)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForgottenCostCenterPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,, "Cost center punch remarks")

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals("Cost center punch remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch without remarks on cost center forgotten punch request if remarks are not setted")>
        Sub ShouldSavePunchWithoutRemarksOnCostCenterForgottenPunchRequestIfRemarksAreNotSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(12)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ForgottenCostCenterPunch, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, Nothing, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,,)

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with not reliable cause on work resume punch request")>
        Sub ShouldSavePunchWithNotReliableCauseOnWorkResumePunchRequest()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(15)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act
                Dim resumeInfo As ExternalWorkResume = New ExternalWorkResume
                resumeInfo.DateTime = New DateTime(2025, 1, 1, 8, 0, 0)
                resumeInfo.IdCause = 1
                resumeInfo.Description = "External work resume"
                resumeInfo.Direction = "in"
                Dim resumeInfoArray As ExternalWorkResume() = {resumeInfo}
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ExternalWorkWeekResume, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, resumeInfoArray, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,, )

                'Assert
                Assert.True(response.Result AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.ExternalWork.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with remarks on work resume punch request with remarks")>
        Sub ShouldSavePunchWithRemarksOnWorkResumePunchRequestWithRemarks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(15)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act
                Dim resumeInfo As ExternalWorkResume = New ExternalWorkResume
                resumeInfo.DateTime = New DateTime(2025, 1, 1, 8, 0, 0)
                resumeInfo.IdCause = 1
                resumeInfo.Description = "External work resume"
                resumeInfo.Direction = "in"
                resumeInfo.Remark = "External work remarks"
                Dim resumeInfoArray As ExternalWorkResume() = {resumeInfo}
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ExternalWorkWeekResume, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, resumeInfoArray, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,,)

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals("External work remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch without remarks on work resume punch request without remarks")>
        Sub ShouldSavePunchWithoutRemarksOnWorkResumePunchRequestWithoutRemarks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(15)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Punches" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "IDCredential", "Type", "ActualType", "InvalidType", "ShiftDate", "DateTime", "IDTerminal", "IDReader", "IDZone", "TypeData", "TypeDetails", "Location", "LocationZone", "FullAddress", "TimeZone", "Exported", "IP", "IsNotReliable", "Action", "IDPassport", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "TimeStamp", "CRC", "MaskAlert", "TemperatureAlert", "VerificationType", "Workcenter", "InTelecommute", "HasPhoto", "PhotoOnAzure", "Source", "IDRequest"}, {})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act
                Dim resumeInfo As ExternalWorkResume = New ExternalWorkResume
                resumeInfo.DateTime = New DateTime(2025, 1, 1, 8, 0, 0)
                resumeInfo.IdCause = 1
                resumeInfo.Description = "External work resume"
                resumeInfo.Direction = "in"
                Dim resumeInfoArray As ExternalWorkResume() = {resumeInfo}
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.SaveRequest(eRequestType.ExternalWorkWeekResume, New roPassportTicket(), 1, "2025-03-14 00:33:00.000", "", "", "", "", 1, -1, "", Nothing, "in", False, "", "", "", "", "", "", "", "", False, "", False, resumeInfoArray, Nothing, TimeZoneInfo.Local, New roRequestState(1), Nothing, Nothing, Nothing, Nothing, False,,,)

                'Assert
                Assert.True(response.Result AndAlso helperPunch.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with reliable cause on task forgotten punch request")>
        Sub ShouldSavePunchWithReliableCauseOnTaskForgottenPunchRequest()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperDatalayer.DbCommandMock4Save()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(10)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                helperRequest.ForgottenTaskRequestFromAdapterStub()
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Tasks.Requests.Forgotten", 1850, 1)
                helperRequest.GetRequestApprovals()
                helperRequest.GetRequestDays()
                helperRequest.GetApprovalStatus()
                helperBusiness.GetEmployeeLockDatetoApplyStub(New Date(2024, 1, 1))
                helperNotification.GenerateNotificationTaskMock()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Requests" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "RequestType", "RequestDate", "Comments", "Status", "StatusLevel", "Date1", "Date2", "FieldName", "FieldValue", "IDCause", "IDCenter", "Hours", "IDShift", "IDTask1", "IDTask2", "CompletedTask", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "NextLevelPassports", "StartShift", "IDEmployeeExchange", "FromTime", "ToTime", "NotReaded", "AutomaticValidation", "ValidationDate", "RejectedDate", "CRC"}, New Object()() {New Object() {1, 10, New Date(2025, 1, 1), "", 1, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, "", "", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, 1, DBNull.Value, False, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, DBNull.Value, DBNull.Value, DBNull.Value, 1}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim request As roRequest = New roRequest
                request.ID = 1
                request.RequestType = eRequestType.ForbiddenTaskPunch
                Dim passportTicket As roPassportTicket = New roPassportTicket
                passportTicket.ID = 1
                Dim oLng As New roLanguage()
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.ApproveRefuseRequest(request, passportTicket, New roRequestState(1), oLng, True, False)

                'Assert
                Assert.True(response.Result AndAlso helperRequest.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.TaskForgottenPunch.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch with remarks on task forgotten punch request with remarks")>
        Sub ShouldSavePunchWithRemarksOnTaskForgottenPunchRequestWithRemarks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperDatalayer.DbCommandMock4Save()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(10)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                helperRequest.ForgottenTaskRequestFromAdapterStub()
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Tasks.Requests.Forgotten", 1850, 1)
                helperRequest.GetRequestApprovals()
                helperRequest.GetRequestDays()
                helperRequest.GetApprovalStatus()
                helperBusiness.GetEmployeeLockDatetoApplyStub(New Date(2024, 1, 1))
                helperNotification.GenerateNotificationTaskMock()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Requests" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "RequestType", "RequestDate", "Comments", "Status", "StatusLevel", "Date1", "Date2", "FieldName", "FieldValue", "IDCause", "IDCenter", "Hours", "IDShift", "IDTask1", "IDTask2", "CompletedTask", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "NextLevelPassports", "StartShift", "IDEmployeeExchange", "FromTime", "ToTime", "NotReaded", "AutomaticValidation", "ValidationDate", "RejectedDate", "CRC"}, New Object()() {New Object() {1, 10, New Date(2025, 1, 1), "Task forgotten punch remarks", 1, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, "", "", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, 1, DBNull.Value, False, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, DBNull.Value, DBNull.Value, DBNull.Value, 1}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim request As roRequest = New roRequest
                request.ID = 1
                request.RequestType = eRequestType.ForbiddenTaskPunch
                Dim passportTicket As roPassportTicket = New roPassportTicket
                passportTicket.ID = 1
                Dim oLng As New roLanguage()
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.ApproveRefuseRequest(request, passportTicket, New roRequestState(1), oLng, True, False)

                'Assert
                Assert.True(response.Result AndAlso helperRequest.RemarksSaved.Equals("Task forgotten punch remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should save punch without remarks on task forgotten punch request without remarks")>
        Sub ShouldSavePunchWithoutRemarksOnTaskForgottenPunchRequestWithoutRemarks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                'Arrange
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperRequest.SaveWithParams()
                helperTerminal.Load()
                helperDatalayer.DbCommandMock4Save()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()
                helperSecurity.GetEmployeePermissions(10)
                helperTerminal.GetCurrentTerminalDatetime()
                helperEmployees.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployees.GetEmployee()
                helperTerminal.GetEmployeeTerminals()
                helperTerminal.PutValues()
                helperTerminal.ReturnFields()
                helperPunch.GetLastPunchPres()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperNotification.DeleteIncompletePunchNotificationIfExist()
                helperRequest.ForgottenTaskRequestFromAdapterStub()
                helperSecurity.GetSupervisorPermissionOverFeature(1, "Tasks.Requests.Forgotten", 1850, 1)
                helperRequest.GetRequestApprovals()
                helperRequest.GetRequestDays()
                helperRequest.GetApprovalStatus()
                helperBusiness.GetEmployeeLockDatetoApplyStub(New Date(2024, 1, 1))
                helperNotification.GenerateNotificationTaskMock()
                Fakes.ShimAccessHelper.CreateDataTableStringListOfCommandParameterroBaseConnectionStringBooleanNullableOfInt32 = Function(ByVal strQuery As String, ByVal lstParameters As List(Of CommandParameter), ByVal oConnection As roBaseConnection, ByVal strTableName As String, ByVal bUseCache As Boolean, ByVal iTimeoutSeconds As Nullable(Of Integer)) As DataTable
                                                                                                                                     Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)


                                                                                                                                     If tableName = "Requests" Then
                                                                                                                                         Return helperDatalayer.CreateDataTableMock({"IdEmployee", "RequestType", "RequestDate", "Comments", "Status", "StatusLevel", "Date1", "Date2", "FieldName", "FieldValue", "IDCause", "IDCenter", "Hours", "IDShift", "IDTask1", "IDTask2", "CompletedTask", "Field1", "Field2", "Field3", "Field4", "Field5", "Field6", "NextLevelPassports", "StartShift", "IDEmployeeExchange", "FromTime", "ToTime", "NotReaded", "AutomaticValidation", "ValidationDate", "RejectedDate", "CRC"}, New Object()() {New Object() {1, 10, New Date(2025, 1, 1), DBNull.Value, 1, DBNull.Value, New Date(2025, 1, 1), DBNull.Value, "", "", DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, 1, DBNull.Value, False, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, True, DBNull.Value, DBNull.Value, DBNull.Value, 1}})
                                                                                                                                     End If


                                                                                                                                 End Function
                'Act                
                Dim request As roRequest = New roRequest
                request.ID = 1
                request.RequestType = eRequestType.ForbiddenTaskPunch
                Dim passportTicket As roPassportTicket = New roPassportTicket
                passportTicket.ID = 1
                Dim oLng As New roLanguage()
                Dim response = Robotics.Base.VTPortal.VTPortal.RequestsHelper.ApproveRefuseRequest(request, passportTicket, New roRequestState(1), oLng, True, False)

                'Assert
                Assert.True(response.Result AndAlso helperRequest.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub
    End Class

End Namespace