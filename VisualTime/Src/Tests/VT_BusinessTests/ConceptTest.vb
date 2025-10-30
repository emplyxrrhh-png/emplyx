Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.VTBase
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Concept")>
    <CollectionDefinition("Concept", DisableParallelization:=True)>
    <Category("Concept")>
    Public Class ConceptTest

        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperConcepts As ConceptsHelper
        Private ReadOnly helperPassport As PassportHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperDatalayer = New DatalayerHelper
            helperEmployee = New EmployeeHelper
            helperConcepts = New ConceptsHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Name Is Empty")>
        Sub ShouldNotValidateConceptIfNameIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Act
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                Dim validation = concept.Validate()

                'Assert
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.NameCannotBeNull))
            End Using

        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Name Is Blank")>
        Sub ShouldNotValidateConceptIfNameIsBlank()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Act
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = ""
                Dim validation = concept.Validate()

                'Assert
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.NameCannotBeNull))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Name Exists And BolCheckNames Is True")>
        Sub ShouldNotValidateConceptIfNameExistsAndBolCheckNamesIsTrue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 1}})
                helperDatalayer.CreateCommandStub()
                ' Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.NameAlreadyExist))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Short Name Exists And BolCheckNames Is True")>
        Sub ShouldNotValidateConceptIfShortNameExistsAndBolCheckNamesIsTrue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 1}})
                helperDatalayer.CreateCommandStub()

                ' Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ShortNameAlreadyExist))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Begin Date Is Greater Than Finish Date")>
        Sub ShouldNotValidateConceptIfBeginDateIsGreaterThanFinishDate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}})
                helperDatalayer.CreateCommandStub()
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 20)
                concept.FinishDate = New Date(2023, 7, 19)
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.InvalidDateInterval))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is Showed On Terminal And There Are Four Concepts For Terminal")>
        Sub ShouldNotValidateConceptIfIsShowedOnTerminalAndThereAreFourConceptsForTerminal()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}})
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub("FROM Concepts WHERE ViewInTerminals = 1", 4)
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.TooManyTerminals))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is View In Pays And Used Field Is Empty")>
        Sub ShouldNotValidateConceptIfIsViewInPaysAndUsedFieldIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = True
                concept.UsedField = Nothing
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.InvalidUsedField))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is View In Pays And Used Field Is Blank")>
        Sub ShouldNotValidateConceptIfIsViewInPaysAndUsedFieldIsBlank()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = True
                concept.UsedField = ""
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.InvalidUsedField))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is Accrual Work And Exists Other Accrual Work")>
        Sub ShouldNotValidateConceptIfIsAccrualWorkAndExistsOtherAccrualWork()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.AccrualWorkExists))
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is Not Accrual Work And Exists Other Accrual Work")>
        Sub ShouldValidateConceptIfIsNotAccrualWorkAndExistsOtherAccrualWork()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = False
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is Accrual Work And Not Exists Other Accrual Work")>
        Sub ShouldValidateConceptIfIsAccrualWorkAndNotExistsOtherAccrualWork()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = False
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is DaysType Accrual Type And Automatic Accrual Criteria Is Nothing")>
        Sub ShouldNotValidateConceptIfIsAutomaticAccrualTypeAndDaysTypeAccrualCriteriaIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DaysType
                concept.AutomaticAccrualCriteria = Nothing
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.AutomaticAccrualCriteriaDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is DeactivatedType Accrual Type And Automatic Accrual Criteria Is Nothing")>
        Sub ShouldValidateConceptIfIsDeactivatedTypeAccrualTypeAndAutomaticAccrualCriteriaIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.AutomaticAccrualCriteria = Nothing
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is DaysType Accrual Type And Automatic Accrual Criteria Is Not Nothing")>
        Sub ShouldValidateConceptIfIsDaysTypeAccrualTypeAndAutomaticAccrualCriteriaIsNotNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DaysType
                Dim criteria As roAutomaticAccrualCriteria = New roAutomaticAccrualCriteria()
                Dim userField As roUserField = New roUserField()
                userField.FieldName = "Test"
                criteria.UserField = userField
                concept.AutomaticAccrualCriteria = criteria
                concept.AutomaticAccrualIDCause = 1
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is HoursType Accrual Type And Automatic Accrual Criteria Is Not Nothing")>
        Sub ShouldValidateConceptIfIsHoursTypeAccrualTypeAndAutomaticAccrualCriteriaIsNotNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.HoursType
                Dim criteria As roAutomaticAccrualCriteria = New roAutomaticAccrualCriteria()
                Dim userField As roUserField = New roUserField()
                userField.FieldName = "Test"
                criteria.UserField = userField
                concept.AutomaticAccrualCriteria = criteria
                concept.AutomaticAccrualIDCause = 1
                Dim causesList As List(Of Integer) = New List(Of Integer)()
                causesList.Add(1)
                causesList.Add(2)
                concept.AutomaticAccrualCriteria.Causes = causesList
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is HoursType Accrual Type And Automatic Accrual Criteria Is Nothing")>
        Sub ShouldNotValidateConceptIfIsAutomaticAccrualTypeAndHoursTypeAccrualCriteriaIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.HoursType
                concept.AutomaticAccrualCriteria = Nothing
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.AutomaticAccrualCriteriaDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is HoursType Accrual Type And FactorType Is UserField And UserField Is Nothing")>
        Sub ShouldNotValidateConceptIfIsAutomaticAccrualTypeAndFactorTypeIsUserFieldAndUserFieldIsNothing()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.HoursType
                Dim automaticAccrualCriteria = New roAutomaticAccrualCriteria()
                automaticAccrualCriteria.FactorType = eFactorType.UserField
                automaticAccrualCriteria.UserField = Nothing
                concept.AutomaticAccrualCriteria = automaticAccrualCriteria
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.AutomaticAccrualCriteriaDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Is HoursType Accrual Type And AutomaticAccrualIDCause Is Zero")>
        Sub ShouldNotValidateConceptIfIsAutomaticAccrualTypeAndAutomaticAccrualIDCauseIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.HoursType
                Dim automaticAccrualCriteria = New roAutomaticAccrualCriteria()
                automaticAccrualCriteria.FactorType = eFactorType.UserField
                Dim userField As roUserField = New roUserField(-1)
                userField.FieldName = "Prueba"
                automaticAccrualCriteria.UserField = userField
                concept.AutomaticAccrualCriteria = automaticAccrualCriteria
                concept.AutomaticAccrualIDCause = 0
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.AutomaticAccrualCriteriaDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours IS True And Concept Type Is Yearly")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndConceptTypeIsYearly()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "Y"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And Concept Type Is Monthly")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndConceptTypeIsMonthly()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "M"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And ConceptType Is Weekly")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndConceptTypeIsWeekly()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "W"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And Concept Type Is By Laboral Year")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndConceptTypeIsByLaboralYear()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "L"
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And Expired Id Cause Is Zero")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndExpiredIdCauseIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROçM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "C"
                concept.ExpiredIDCause = 0
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And ExpiredHoursCriteria Is Zero")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndExpiredHoursCriteriaIsZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "C"
                concept.ExpiredIDCause = 1
                concept.ExpiredHoursCriteria.oValue = 0
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And ExpiredHoursCriteria Is Less Than Zero")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndExpiredHoursCriteriaIsLessThanZero()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "C"
                concept.ExpiredIDCause = 1
                concept.ExpiredHoursCriteria.oValue = -1
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And Has Startup Values")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndHasStartupValues()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}, {"FROM StartupValues WHERE IDConcept =", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "C"
                concept.ExpiredIDCause = 1
                concept.ExpiredHoursCriteria.oValue = 1
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply Expired Hours Is True And Has Accrual Rules")>
        Sub ShouldNotValidateConceptIfApplyExpiredHoursIsTrueAndHasAccrualRules()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}, {"FROM StartupValues WHERE IDConcept =", 0}, {"FROM AccrualsRules  INNER JOIN LabAgreeAccrualsRules", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = True
                concept.DefaultQuery = "C"
                concept.ExpiredIDCause = 1
                concept.ExpiredHoursCriteria.oValue = 1
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.ExpiredHoursDataError))
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Validate Concept If Apply On Holidays Request Is True And Is Weekly Concept")>
        Sub ShouldNotValidateConceptIfApplyOnHolidaysRequestIsTrueAndIsWeeklyConcept()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer, Nothing)
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}, {"FROM Concepts WHERE IsAccrualWork = 1", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.IsAccrualWork = True
                concept.AutomaticAccrualType = eAutomaticAccrualType.DeactivatedType
                concept.ApplyExpiredHours = False
                concept.DefaultQuery = "W"
                concept.ApplyOnHolidaysRequest = True
                Dim validation As Boolean = concept.Validate()
                Assert.True(Not validation AndAlso concept.State.Result.Equals(ConceptResultEnum.DefaultQueryMustBeAnnual))
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Name Has Value")>
        Sub ShouldValidateConceptIfNameHasValue()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If BolCheckNames Is False And Exists Other Concept With Same Name")>
        Sub ShouldValidateConceptIfBolCheckNamesIsFalseAndExistsOtherConceptWithSameName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 1}, {"FROM Concepts WHERE ShortName", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                Dim validation As Boolean = concept.Validate(False)
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If BolCheckNames Is True And Not Exists Other Concept With Same Name")>
        Sub ShouldValidateConceptIfBolCheckNamesIsTrueAndNotExistsOtherConceptWithSameName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If BolCheckNames Is False And Exists Other Concept With Same Short Name")>
        Sub ShouldValidateConceptIfBolCheckNamesIsFalseAndExistsOtherConceptWithSameShortName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                Dim validation As Boolean = concept.Validate(False)
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If BolCheckNames Is True And Not Exists Other Concept With Same Short Name")>
        Sub ShouldValidateConceptIfBolCheckNamesIsTrueAndNotExistsOtherConceptWithSameShortName()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is Showed On Terminal Is False And There Are Four Concepts For Terminal")>
        Sub ShouldNotValidateConceptIfIsShowedOnTerminalIsFalseAndThereAreFourConceptsForTerminal()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 4}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = False
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is Showed On Terminal Is True And There Are Less Than Four Concepts For Terminal")>
        Sub ShouldNotValidateConceptIfIsShowedOnTerminalIsTrueAndThereAreLessThanFourConceptsForTerminal()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 3}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is View In Pays Is False And Used Field Is Empty")>
        Sub ShouldValidateConceptIfIsViewInPaysIsFalseAndUsedFieldIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.UsedField = Nothing
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is View In Pays Is True And Used Field Has Value")>
        Sub ShouldValidateConceptIfIsViewInPaysIsTrueAndUsedFieldIsNotEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = True
                concept.UsedField = "Test"
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Validate Concept If Is View In Pays Is False And Used Field Is Blank")>
        Sub ShouldValidateConceptIfIsViewInPaysIsFalseAndUsedFieldIsBlank()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperDatalayer.CreateCommandStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Concepts WHERE Name", 0}, {"FROM Concepts WHERE ShortName", 0}, {"FROM Concepts WHERE ViewInTerminals = 1", 1}})
                'Act
                Dim conceptState As roConceptState = New roConceptState(-1)
                Dim concept As roConcept = New roConcept(1, conceptState)
                concept.Name = "Concept 1"
                concept.BeginDate = New Date(2023, 7, 19)
                concept.FinishDate = New Date(2023, 7, 20)
                concept.ViewInTerminals = True
                concept.ViewInPays = False
                concept.UsedField = ""
                Dim validation As Boolean = concept.Validate()
                Assert.True(validation)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property On GetAccrualEmployeeSummary When Accrual DefaultQuery Is M")>
        Sub ShouldNotGetYearWorkPeriodPropertyOnGetAccrualEmployeeSummaryWhenAccrualDefaultQueryIsM()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Mensual
                Dim defaultQuery = "M"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetMonthAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property On GetAccrualEmployeeSummary When Accrual DefaultQuery Is W")>
        Sub ShouldNotGetYearWorkPeriodPropertyOnGetAccrualEmployeeSummaryWhenAccrualDefaultQueryIsW()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Semanal
                Dim defaultQuery = "W"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetWeekAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property On GetAccrualEmployeeSummary When Accrual DefaultQuery Is Y")>
        Sub ShouldNotGetYearWorkPeriodPropertyOnGetAccrualEmployeeSummaryWhenAccrualDefaultQueryIsY()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Anual
                Dim defaultQuery = "Y"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetAnnualAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property On GetAccrualEmployeeSummary When Accrual DefaultQuery Is C")>
        Sub ShouldNotGetYearWorkPeriodPropertyOnGetAccrualEmployeeSummaryWhenAccrualDefaultQueryIsC()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Contrato
                Dim defaultQuery = "C"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetContractAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is Anual on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsAnualOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Anual
                Dim defaultQuery = "Y"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetAnnualAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is LastYear on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsLastYearOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.LastYear
                Dim defaultQuery = "Y"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetAnnualAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is LastMonth on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsLastMonthOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.LastMonth
                Dim defaultQuery = "M"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetMonthAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is Month on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsMonthOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Mensual
                Dim defaultQuery = "M"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetMonthAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is Semanal on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsSemanalOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Semanal
                Dim defaultQuery = "W"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetWeekAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is Contrato on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsContratoOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.Contrato
                Dim defaultQuery = "C"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetContractAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Get YearWorkPeriod Property when type is ContractAnnualized on GetAccrualEmployeeSummary")>
        Sub ShouldNotGetYearWorkPeriodPropertyWhenTypeIsContractAnnualizedOnGetAccrualEmployeeSummary()

            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim now As DateTime = New DateTime(2023, 7, 31)
                Dim lstDates As New Generic.List(Of DateTime)
                lstDates.Add(New DateTime(2023, 3, 5))
                lstDates.Add(now)
                Dim accrualType = SummaryType.ContractAnnualized
                Dim defaultQuery = "L"
                Dim conceptsTableMock As System.Data.DataTable = helperConcepts.FillMockWithAccrualsByType(2, accrualType, defaultQuery)

                helperConcepts.GetAnnualWorkAccrualsStub(conceptsTableMock)
                helperEmployee.GetPeriodDatesByContractAtDateStub(lstDates)

                'Act
                Dim dt As List(Of roAccrualsSummary) = roEmployeeSummaryManager.GetAccrualEmployeeSummary(1, now, accrualType, Nothing, New roEmployeeState(-1))

                'Assert
                Assert.Equal(2, dt.Count)
                Assert.Equal(Nothing, dt(0).YearWorkPeriod)
                Assert.Equal(Nothing, dt(1).YearWorkPeriod)
            End Using
        End Sub
    End Class

End Namespace