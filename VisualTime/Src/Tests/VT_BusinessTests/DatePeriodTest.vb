Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees.Contract
Imports VT_XU_Base
Imports VT_XU_Common
Imports Xunit

Namespace Unit.Test

    <CollectionDefinition("DatePeriod", DisableParallelization:=True)>
    <Collection("DatePeriod")>
    <Category("DatePeriod")>
    Public Class DatePeriodTest

        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperParameters As ParametersHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperEmployee = New EmployeeHelper
            helperParameters = New ParametersHelper
        End Sub

#Region "Not have contract"

        <Fact(DisplayName:="Should get empty period when user has no contract at requested date")>
        Sub ShouldGetEmptyPeriodWhenUserHasNoContractAtRequestedDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim emptyDate As New DateTime(1900, 1, 1)
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 1, 1), New DateTime(2015, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 1, 1), New DateTime(2015, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.NotNull(lstDates)
                Assert.Equal(True, helperEmployee.SearchForGetLastContractDatesInPeriod AndAlso lstDates(0) = emptyDate AndAlso lstDates(1) = emptyDate)
            End Using
        End Sub

#End Region

#Region "Have contract"

#Region "Day"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is Daily and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsDailyAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Daily, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

#End Region

#Region "Current Week"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is week and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsWeekAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Semanal, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

#End Region

#Region "Current Month"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is month and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsMonthWorkAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Mensual, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should get first day of contract when contract starts after month and accrual type is month")>
        Sub ShouldGetFirstDayOfContractWhenContractStartsAfterMonthAndAccrualTypeIsMonth()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})
                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Mensual, 1, New DateTime(2015, 6, 15), New roContractState(-1))
                'Assert
                Assert.Equal(True, Not helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate AndAlso lstDates(0) = New DateTime(2015, 6, 5))
            End Using
        End Sub

        <Fact(DisplayName:="Should get reference date as end period when accrual type is month")>
        Sub ShouldGetReferenceDateAsEndPeriodWhenAccrualTypeIsMonth()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2013, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})
                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Mensual, 1, New DateTime(2015, 6, 15), New roContractState(-1))
                'Assert
                Assert.Equal(True, Not helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate AndAlso lstDates(1) = New DateTime(2015, 6, 15))
            End Using
        End Sub

        <Fact(DisplayName:="Should get end month date as end period when accrual type is month and raw period requested")>
        Sub ShouldGetEndMonthDateAsEndPeriodWhenAccrualTypeIsMonthAndRawPeriodRequested()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2013, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})
                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Mensual, 1, New DateTime(2015, 6, 15), New roContractState(-1), True)
                'Assert
                Assert.Equal(True, Not helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate AndAlso lstDates(1) = New DateTime(2015, 6, 30))
            End Using
        End Sub

        <Fact(DisplayName:="Should get end contract date as end period when accrual type is month and raw period requested")>
        Sub ShouldGetEndContractDateAsEndPeriodWhenAccrualTypeIsMonthAndRawPeriodRequested()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2013, 6, 5), New DateTime(2015, 6, 20))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})
                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Mensual, 1, New DateTime(2015, 6, 15), New roContractState(-1), True)
                'Assert
                Assert.Equal(True, Not helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate AndAlso lstDates(1) = New DateTime(2015, 6, 20))
            End Using
        End Sub

#End Region

#Region "Last Month"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is LastMonth and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsLastMonthWorkAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.LastMonth, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should get end contract date as end period when accrual type is month and period contains end of contract")>
        Sub ShouldGetEndContractDateAsEndPeriodWhenAccrualTypeIsMonthAndPeriodContainsEndOfContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2010, 6, 5), Now.Date.AddMonths(-1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})
                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.LastMonth, 1, Now.Date, New roContractState(-1), True)
                'Assert
                Assert.Equal(True, Not helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate AndAlso lstDates(1) = Now.Date.AddMonths(-1))
            End Using
        End Sub

#End Region

#Region "Year"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is anual and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsAnualAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should get First day of year from parameters when type is Annual and has contract")>
        Sub ShouldGetFirstDayOfYearFromParametersWhenTypeIsAnualAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "2"}, {"YearPeriod", "3"}, {"WeekPeriod", "4"}})

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(2, lstDates(0).Day)
            End Using
        End Sub

        <Fact(DisplayName:="Should get First day of contract as Initial date when period contains contract and type is Annual and has contract")>
        Sub ShouldGetFirstDayOfContractAsInitialDateWhenPeriodContainsContractAndTypeIsAnualAndHasContract()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2023, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "1"}, {"WeekPeriod", "1"}})

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, New DateTime(2023, 7, 5), New roContractState(-1))

                'Assert
                Assert.Equal(New DateTime(2023, 6, 5), lstDates(0))
            End Using
        End Sub

        <Fact(DisplayName:="Should get reference date as end date when period is annual")>
        Sub ShouldGetReferenceDateAsEndDateWhenPeriodIsAnnual()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2010, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "1"}, {"WeekPeriod", "1"}})

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, Now.Date.AddDays(1), New roContractState(-1))

                'Assert
                Assert.Equal(Now.Date.AddDays(1), lstDates(1))
            End Using
        End Sub

        <Fact(DisplayName:="Should get last day of contract as final date when period is annual and contains end contract and raw period is requested")>
        Sub ShouldGetLastDayOfContractAsFinalDateWhenPeriodIsAnnualAndContainsEndContractAndRawPeriodIsRequested()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2010, 6, 5), New DateTime(2023, 3, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "1"}, {"WeekPeriod", "1"}})

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, New DateTime(2023, 2, 1), New roContractState(-1), True)

                'Assert
                Assert.Equal(New DateTime(2023, 3, 1), lstDates(1))
            End Using
        End Sub

        <Fact(DisplayName:="Should get RequestedDate as EndDate when period contains RequestDate and type is Annual and has contract")>
        Sub ShouldGetRequestedDateAsEndDateWhenPeriodContainsRequestDateAndTypeIsAnualAndHasContract()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperParameters.ParameterStub(New Dictionary(Of String, String) From {{"MonthPeriod", "1"}, {"YearPeriod", "1"}, {"WeekPeriod", "1"}})

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Anual, 1, New DateTime(2023, 7, 5), New roContractState(-1))

                'Assert
                Assert.Equal(New DateTime(2023, 7, 5), lstDates(1))
            End Using
        End Sub

#End Region

#Region "Last Year"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is LastYear and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsLastYearAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.LastYear, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

#End Region

#Region "Anual Work"

        <Fact(DisplayName:="Should call GetDatesOfContractInDate when type is annualwork and has contract")>
        Sub ShouldCallGetDatesOfContractInDateWhenTypeIsAnnualWorkAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(True, helperEmployee.SearchForGetDatesOfContractInDate)
            End Using
        End Sub

        <Fact(DisplayName:="Should get begin contract date as start period date when type is AnnualWork and has contract")>
        Sub ShouldGetBeginContractDateAsStartPeriodDateWhenTypeIsAnnualWorkAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim beginContractDate As DateTime = New DateTime(2015, 6, 5)
                Dim endContractDate As DateTime = New DateTime(2079, 1, 1)
                Dim requestedDate As DateTime = New DateTime(2017, 10, 6)

                helperEmployee.GetLastContractDatesInPeriodStub(beginContractDate, endContractDate)
                helperEmployee.GetDatesOfContractInDateFake(beginContractDate, endContractDate)

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, 1, requestedDate, New roContractState(-1))

                'Assert
                Assert.Equal(True, lstDates(0).Day = beginContractDate.Day AndAlso lstDates(0).Month = beginContractDate.Month And lstDates(0).Year = beginContractDate.Year)
            End Using
        End Sub

        <Fact(DisplayName:="Should get RequestDate as end period date when type is AnnualWork and has contract and StartPeriod is less than a day before")>
        Sub ShouldGetRequestDateAsEndPeriodDateWhenTypeIsAnnualWorkAndHasContractAndStartPeriodIsLessThanADayBefore()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim beginContractDate As DateTime = New DateTime(2015, 6, 5)
                Dim endContractDate As DateTime = New DateTime(2079, 1, 1)
                Dim requestedDate As DateTime = New DateTime(2017, 10, 6)

                helperEmployee.GetLastContractDatesInPeriodStub(beginContractDate, endContractDate)
                helperEmployee.GetDatesOfContractInDateFake(beginContractDate, endContractDate)

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, 1, requestedDate, New roContractState(-1))

                'Assert
                Assert.Equal(True, lstDates(1).Day = requestedDate.Day AndAlso lstDates(1).Month = requestedDate.Month And lstDates(1).Year = requestedDate.Year)
            End Using
        End Sub

        <Fact(DisplayName:="Should get reference date as end period date when type is AnnualWork and reference date is between period")>
        Sub ShouldGetReferenceDateAsEndPeriodDateWhenTypeIsAnnualWorkAndReferenceDateIsBetweenPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim emptyDate As New DateTime(1900, 1, 1)
                Dim referenceDate As Date = New DateTime(Now.Year, 6, 1)
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2023, 1, 1), New DateTime(2079, 1, 1))
                Dim dBeginContrat As Date = New DateTime(Now.Year, 1, 1)
                Dim dEndContract As Date = New DateTime(Now.Year, 12, 31)
                helperEmployee.GetDatesOfContractInDateFake(dBeginContrat, dEndContract)

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.ContractAnnualized, 1, referenceDate, New roContractState(-1))

                'Assert
                Assert.NotNull(lstDates)
                Assert.Equal(True, lstDates(0) = New DateTime(Now.Year, 1, 1) AndAlso lstDates(1) = referenceDate)
            End Using
        End Sub

#End Region

#Region "Next Annual Work"

        <Fact(DisplayName:="Should get next annual work period when user employee has same contract in current annual work period")>
        Sub ShouldGetNextAnnualWorkPeriodWhenUserEmployeeHasSameContractInCurrentAnnualWorkPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim emptyDate As New DateTime(1900, 1, 1)
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2023, 1, 1), New DateTime(2079, 1, 1))
                Dim lstPeriods As New List(Of List(Of Date))
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.Year, 1, 1), New DateTime(Now.Year, 12, 31)})
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.AddYears(1).Year, 1, 1), New DateTime(Now.AddYears(1).Year, 12, 31)})
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods)
                helperEmployee.GetDatesOfContractInDateFake(New DateTime(2023, 1, 1), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.NextContractAnnualizedPeriod, 1, New DateTime(Now.Year, 6, 1), New roContractState(-1))

                'Assert
                Assert.NotNull(lstDates)
                Assert.Equal(True, lstDates(0) = New DateTime(Now.AddYears(1).Year, 1, 1) AndAlso lstDates(1) = New DateTime(Now.AddYears(1).Year, 12, 31))
            End Using
        End Sub

        <Fact(DisplayName:="Should return empty period if current and next annual work period are correlative but corresponds to different contracts")>
        Sub ShouldReturnEmptyPeriodIfCurrentAndNextAnnualWorkPeriodAreCorrelativeButCorrespondsToDiferenteContracts()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim emptyDate As New DateTime(1900, 1, 1)
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(Now.AddYears(1).Year, 1, 1), New DateTime(Now.AddYears(1).Year, 12, 31))
                Dim lstPeriods As New List(Of List(Of Date))
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.Year, 1, 1), New DateTime(Now.Year, 12, 31)})
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.AddYears(1).Year, 1, 1), New DateTime(Now.AddYears(1).Year, 12, 31)})
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods)
                helperEmployee.GetDatesOfContractInDateFake(New DateTime(Now.Year, 1, 1), New DateTime(Now.Year, 12, 31))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.NextContractAnnualizedPeriod, 1, Now.Date, New roContractState(-1))

                'Assert
                Assert.NotNull(lstDates)
                Assert.Equal(True, lstDates(0).Year = 1900)
            End Using
        End Sub

        <Fact(DisplayName:="Should return end contract date as end period if current and next annual work period belongs to same contract and contract ends in next work period")>
        Sub ShouldReturnEndContractDateAsEndPeriodIfCurrentAndNextAnnualWorkPeriodBelongsToSameContractAndContractEndsInNextWorkPeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(Now.AddYears(1).Year, 1, 1), New DateTime(Now.AddYears(1).Year, 6, 1))
                Dim lstPeriods As New List(Of List(Of Date))
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.Year, 1, 1), New DateTime(Now.Year, 12, 31)})
                lstPeriods.Add(New List(Of Date) From {New DateTime(Now.AddYears(1).Year, 1, 1), New DateTime(Now.AddYears(1).Year, 12, 31)})
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateStub(lstPeriods)
                helperEmployee.GetDatesOfContractInDateFake(New DateTime(2000, 1, 1), New DateTime(Now.AddYears(1).Year, 6, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.NextContractAnnualizedPeriod, 1, Now.Date, New roContractState(-1))

                'Assert
                Assert.NotNull(lstDates)
                Assert.Equal(True, lstDates(1) = New DateTime(Now.AddYears(1).Year, 6, 1))
            End Using
        End Sub

#End Region

#Region "Current Contract"

        <Fact(DisplayName:="Should not call GetDatesOfAnnualWorkPeriodsInDate when type is contract and has contract")>
        Sub ShouldNotCallGetDatesOfAnnualWorkPeriodsInDateWhenTypeIsContractAndHasContract()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperEmployee.GetLastContractDatesInPeriodStub(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfContractInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))
                helperEmployee.GetDatesOfAnnualWorkPeriodsInDateFake(New DateTime(2015, 6, 5), New DateTime(2079, 1, 1))

                'Act
                Dim lstDates As List(Of DateTime) = roBusinessSupport.GetPeriodDatesByContractAtDate(SummaryType.Contrato, 1, DateTime.Now.Date, New roContractState(-1))

                'Assert
                Assert.Equal(False, helperEmployee.SearchForGetDatesOfAnnualWorkPeriodsInDate)
            End Using
        End Sub

#End Region

        'TODO ISM:Check more period test
    End Class

#End Region

End Namespace