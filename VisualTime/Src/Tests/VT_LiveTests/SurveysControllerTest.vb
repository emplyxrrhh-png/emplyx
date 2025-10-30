Imports System.ComponentModel
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Mvc
Imports Robotics.Base.DTOs
Imports VTLive40
Imports VT_XU_Base
Imports Xunit

Namespace Unit.Test

    <Collection("SurveysController")>
    <CollectionDefinition("SurveysController", DisableParallelization:=True)>
    <Category("SurveysController")>
    Public Class SurveysControllerTest

        Private ReadOnly surveysHelper As SurveysHelper
        Private ReadOnly permissionsHelper As PermissionsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            surveysHelper = New SurveysHelper
            permissionsHelper = New PermissionsHelper

        End Sub

        <Fact(DisplayName:="Should load a list of surveys")>
        Sub ShouldLoadListOfSurveys()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLoadOptions As New DataSourceLoadOptions
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.NotNull(surveysController.GetSurveys(oLoadOptions))

            End Using
        End Sub

        <Fact(DisplayName:="Should load a surveys that owns")>
        Sub ShouldLoadSurveysThatOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurvey(1)

                'Assert
                Assert.NotNull(result)
                Assert.Equal(CType(result.Data, roSurvey).CreatedBy, 1)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load a surveys that not owns")>
        Sub ShouldNotLoadSurveysThatNotOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurvey(2)

                'Assert
                Assert.NotNull(result)
                Assert.Equal(CType(result.Data, roSurvey).CreatedBy, 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should load surveys responses of a survey that owns")>
        Sub ShouldLoadSurveysResponsesOfASurveyThatOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurveyResponses(1)

                'Assert
                Assert.NotNull(result)
                Assert.True(surveysHelper.SurveyResponsesLoaded)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load surveys responses of a survey that not owns")>
        Sub ShouldNotLoadSurveysResponsesOfASurveyThatNotOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurveyResponses(2)

                'Assert
                Assert.NotNull(result)
                Assert.False(surveysHelper.SurveyResponsesLoaded)

            End Using
        End Sub

        <Fact(DisplayName:="Should load surveys responses filtered by idEmployee of a survey that owns")>
        Sub ShouldLoadSurveysResponsesFilteredByIdEmployeeOfASurveyThatOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurveyResponsesByIdEmployee(1, {1, 2})

                'Assert
                Assert.NotNull(result)
                Assert.True(surveysHelper.SurveyResponsesLoaded)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load surveys responses filtered by idemployee of a survey that not owns")>
        Sub ShouldNotLoadSurveysResponsesFilteredByIdEmployeeOfASurveyThatNotOwns()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As JsonResult = surveysController.GetSurveyResponsesByIdEmployee(2, {1, 2})

                'Assert
                Assert.NotNull(result)
                Assert.False(surveysHelper.SurveyResponsesLoaded)

            End Using
        End Sub

        <Fact(DisplayName:="Should create a survey copy if is survey author")>
        Sub ShouldCreateASurveyCopyIfIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.True(surveysController.CopySurvey(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not create a survey copy if not is survey author")>
        Sub ShouldNotCreateASurveyCopyIfNotIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.False(surveysController.CopySurvey(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should insert a survey if is survey author")>
        Sub ShouldInsertASurveyIfIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.IsType(GetType(roSurvey), surveysController.InsertSurvey(1, {}, {}, "", "", "", "", False, "2024-01-01", "", {}, 0, SurveyStatusEnum.Draft, "Simple").Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not insert a survey if is not survey author")>
        Sub ShouldNotInsertASurveyIfNotIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.False(surveysController.InsertSurvey(2, {}, {}, "", "", "", "", False, "2024-01-01", "", {}, 0, SurveyStatusEnum.Draft, "Simple").Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should create a survey if no id is provided")>
        Sub ShouldCreateASurveyIfNoIdIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.IsType(GetType(roSurvey), surveysController.InsertSurvey(1, {}, {}, "", "", "", "", False, "2024-01-01", "", {}, 0, SurveyStatusEnum.Draft, "Simple").Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should publish a survey if is survey author")>
        Sub ShouldPublishASurveyIfIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.True(surveysController.SendSurvey(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not publish survey if not is survey author")>
        Sub ShouldNotPublishSurveyIfNotIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()

                'Assert
                Assert.False(surveysController.SendSurvey(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should update a survey if is survey author")>
        Sub ShouldUpdateASurveyIfIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As HttpStatusCodeResult = surveysController.UpdateSurvey(1, "{}")
                'Assert
                Assert.True(result.StatusCode = 200)

            End Using
        End Sub

        <Fact(DisplayName:="Should not update survey if not is survey author")>
        Sub ShouldNotUpdateSurveyIfNotIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As HttpStatusCodeResult = surveysController.UpdateSurvey(2, "{}")

                'Assert
                Assert.True(result.StatusCode = 400)

            End Using
        End Sub

        <Fact(DisplayName:="Should delete a survey if is survey author")>
        Sub ShouldDeleteASurveyIfIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As HttpStatusCodeResult = surveysController.DeleteSurvey(1)
                'Assert
                Assert.True(result.StatusCode = 200)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete survey if not is survey author")>
        Sub ShouldNotDeleteSurveyIfNotIsSurveyAuthor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                surveysHelper.SetAvailableSurveys(1, {1, 3}, {2})

                'Act
                Dim surveysController As New VTLive40.SurveysController()
                Dim result As HttpStatusCodeResult = surveysController.DeleteSurvey(2)

                'Assert
                Assert.True(result.StatusCode = 400)

            End Using
        End Sub

    End Class

End Namespace