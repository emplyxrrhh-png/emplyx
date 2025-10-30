Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.LabAgree
Imports Robotics.Base.VTLabAgrees.LabAgree
Imports Robotics.Base.VTUserFields.UserFields
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("LabAgree")>
    <CollectionDefinition("LabAgree", DisableParallelization:=True)>
    <Category("LabAgree")>
    Public Class LabAgreeTest

        Private ReadOnly helper As EmployeeHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperPassport As PassportHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helper = New EmployeeHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDatalayer = New DatalayerHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should Not Apply Accrual Rule If Task Date Not Between Accrual Rule Period")>
        Sub ShouldNotApplyAccrualRuleIfTaskDateNotBetweenAccrualRulePeriod()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(-1))
                Dim oLabAgreeAccrualRule As New roLabAgreeEngineAccrualRule
                oLabAgreeAccrualRule.BeginDate = Now.Date.AddDays(-100)
                oLabAgreeAccrualRule.EndDate = Now.Date.AddDays(-99)

                ' Act
                Dim bolRet As Boolean = oLabAgreeManager.AccrualRuleApplyOnDate(1, oLabAgreeAccrualRule, Now.Date, Nothing)

                'Assert
                Assert.Equal(bolRet.ToString, "False")

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Apply Accrual Rule If definition is empty")>
        Sub ShouldNotApplyAccrualRuleIfDefinitionIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(-1))
                Dim oLabAgreeAccrualRule As New roLabAgreeEngineAccrualRule
                oLabAgreeAccrualRule.BeginDate = Now.Date.AddDays(-1)
                oLabAgreeAccrualRule.EndDate = Now.Date.AddDays(2)
                oLabAgreeAccrualRule.LabAgreeRule = New roLabAgreeEngineRule

                ' Act
                Dim bolRet As Boolean = oLabAgreeManager.AccrualRuleApplyOnDate(1, oLabAgreeAccrualRule, Now.Date, Nothing)

                'Assert
                Assert.Equal(bolRet.ToString, "False")

            End Using
        End Sub

        <Fact(DisplayName:="Should Apply Accrual Rule If Schedule type is Daily and Task date is now")>
        Sub ShouldApplyAccrualRuleIfScheduletypeIsDailyAndTaskDateisNow()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(-1))
                Dim oLabAgreeAccrualRule As New roLabAgreeEngineAccrualRule
                oLabAgreeAccrualRule.BeginDate = Now.Date.AddDays(-1)
                oLabAgreeAccrualRule.EndDate = Now.Date.AddDays(2)
                oLabAgreeAccrualRule.LabAgreeRule = New roLabAgreeEngineRule
                oLabAgreeAccrualRule.LabAgreeRule.Schedule = New roLabAgreeEngineSchedule
                oLabAgreeAccrualRule.LabAgreeRule.Schedule.ScheduleType = LabAgreeScheduleScheduleType.Daily
                oLabAgreeAccrualRule.LabAgreeRule.Schedule.Days = 1

                ' Act
                Dim bolRet As Boolean = oLabAgreeManager.AccrualRuleApplyOnDate(1, oLabAgreeAccrualRule, Now.Date, Nothing)

                'Assert
                Assert.Equal(bolRet.ToString, "True")

            End Using
        End Sub

        <Fact(DisplayName:="Should Apply Accrual Rule If Schedule type is Annual on 02/08 and Task date has the same day and month")>
        Sub ShouldApplyAccrualRuleIfScheduletypeIsAnnualon0208AndTaskDatehasTheSameDayAndMonth()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLabAgreeManager As New roLabAgreeManager(New roLabAgreeManagerState(-1))
                Dim oLabAgreeAccrualRule As New roLabAgreeEngineAccrualRule
                oLabAgreeAccrualRule.BeginDate = New DateTime(2023, 8, 2).AddDays(-100)
                oLabAgreeAccrualRule.EndDate = New DateTime(2023, 8, 2).AddDays(100)
                oLabAgreeAccrualRule.LabAgreeRule = New roLabAgreeEngineRule
                oLabAgreeAccrualRule.LabAgreeRule.Schedule = New roLabAgreeEngineSchedule
                oLabAgreeAccrualRule.LabAgreeRule.Schedule.ScheduleType = LabAgreeScheduleScheduleType.Annual
                oLabAgreeAccrualRule.LabAgreeRule.Schedule.Day = 2
                oLabAgreeAccrualRule.LabAgreeRule.Schedule.Month = 8

                ' Act
                Dim bolRet As Boolean = oLabAgreeManager.AccrualRuleApplyOnDate(1, oLabAgreeAccrualRule, New DateTime(2023, 8, 2), Nothing)

                'Assert
                Assert.Equal(bolRet.ToString, "True")

            End Using
        End Sub

        <Fact(DisplayName:="Should not validate Startup value if name is empty")>
        Sub ShouldNotValidateStartupValueIfNameIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim oStartupValue As New roStartupValue()
                oStartupValue.Name = ""
                Dim intIDLabAgree As Integer = 1

                ' Act
                Dim bolRet As Boolean = oStartupValue.Validate(intIDLabAgree)

                'Assert
                Assert.Equal(LabAgreeResultEnum.StartupValueNameInvalid.ToString, oStartupValue.State.Result.ToString)

            End Using
        End Sub

        <Fact(DisplayName:="Should not validate Startup value if Concept is empty")>
        Sub ShouldNotValidateStartupValueIfConceptIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim oStartupValue As New roStartupValue()
                oStartupValue.Name = "Test"
                oStartupValue.IDConcept = 0
                Dim intIDLabAgree As Integer = 1

                ' Act
                Dim bolRet As Boolean = oStartupValue.Validate(intIDLabAgree)

                'Assert
                Assert.Equal(LabAgreeResultEnum.StartupValueIDConceptEmpty.ToString, oStartupValue.State.Result.ToString)

            End Using
        End Sub

        <Fact(DisplayName:="Should not validate Startup value if Field Name is empty when it is Field type")>
        Sub ShouldNotValidateStartupValueIfFieldNameIsEmptyWhenisFieldType()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim oStartupValue As New roStartupValue()
                oStartupValue.Name = "Test"
                oStartupValue.IDConcept = 1
                oStartupValue.StartValueType = LabAgreeValueType.UserField
                oStartupValue.StartUserField = New roUserField
                oStartupValue.StartUserField.FieldName = ""
                Dim intIDLabAgree As Integer = 1

                ' Act
                Dim bolRet As Boolean = oStartupValue.Validate(intIDLabAgree)

                'Assert
                Assert.Equal(LabAgreeResultEnum.StartupValueStartUserFieldEmpty.ToString, oStartupValue.State.Result.ToString)

            End Using
        End Sub

        <Fact(DisplayName:="Should not validate Startup value if Field Name is empty when it is Calculated type")>
        Sub ShouldNotValidateStartupValueIfFieldNameIsEmptyWhenisCalculatedType()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim oStartupValue As New roStartupValue()
                oStartupValue.Name = "Test"
                oStartupValue.IDConcept = 1
                oStartupValue.StartValueType = LabAgreeValueType.CalculatedValue
                oStartupValue.StartValueBaseType = LabAgreeValueTypeBase.UserField
                oStartupValue.StartUserField = New roUserField
                oStartupValue.StartUserField.FieldName = ""
                Dim intIDLabAgree As Integer = 1

                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"DefaultQuery", "Y"}, {"ApplyExpiredHours", False}})

                ' Act
                Dim bolRet As Boolean = oStartupValue.Validate(intIDLabAgree)

                'Assert
                Assert.Equal(LabAgreeResultEnum.StartupValueStartUserFieldEmpty.ToString, oStartupValue.State.Result.ToString)

            End Using
        End Sub

    End Class

End Namespace