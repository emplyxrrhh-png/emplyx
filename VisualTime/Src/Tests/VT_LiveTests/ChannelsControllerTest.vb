Imports System.ComponentModel
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Mvc
Imports Robotics.Base.DTOs
Imports VTLive40
Imports VT_XU_Base
Imports Xunit

Namespace Unit.Test

    <Collection("ChannelsController")>
    <CollectionDefinition("ChannelsController", DisableParallelization:=True)>
    <Category("ChannelsController")>
    Public Class ChannelsControllerTest

        Private ReadOnly channelsHelper As ChannelsHelper
        Private ReadOnly permissionsHelper As PermissionsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            channelsHelper = New ChannelsHelper
            permissionsHelper = New PermissionsHelper

        End Sub

        <Fact(DisplayName:="Should load a list of channels")>
        Sub ShouldLoadListOfChannels()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLoadOptions As New DataSourceLoadOptions
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.NotNull(channelscontroller.GetChannels(oLoadOptions))

            End Using
        End Sub

        <Fact(DisplayName:="Should update a channel if is channel creator")>
        Sub ShouldUpdateAChannelIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(roChannel), channelscontroller.InsertChannel(2, {}, {}, "", "False", "False", {}, ChannelStatusEnum.Draft).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not update complaint channel if dont have complaint rights")>
        Sub ShouldNotUpdateComplaintChannelIfDontHaveComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(String), channelscontroller.InsertChannel(1, {}, {}, "", "False", "False", {}, ChannelStatusEnum.Draft).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not update complaint channel even if have complaint rights")>
        Sub ShouldNotUpdateComplaintChannelEvenIfHaveComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(String), channelscontroller.InsertChannel(1, {}, {}, "", "False", "False", {}, ChannelStatusEnum.Draft).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not update a channel if is not channel creator")>
        Sub ShouldNotUpdateAChannelIfNotIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(String), channelscontroller.InsertChannel(3, {}, {}, "", "False", "False", {}, ChannelStatusEnum.Draft).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should create a channel if no id is provided")>
        Sub ShouldCreateAChannelIfNoIdIsProvided()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(roChannel), channelscontroller.InsertChannel(0, {}, {}, "", "False", "False", {}, ChannelStatusEnum.Draft).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should query a channel if is channel creator")>
        Sub ShouldQueryAChannelIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(roChannel), channelscontroller.GetChannel(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should query complaint channel if has complaint rights")>
        Sub ShouldQueryComplaintChannelIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, True)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(roChannel), channelscontroller.GetChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not query a channel if is not channel creator")>
        Sub ShouldNotQueryAChannelIfNotIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(String), channelscontroller.GetChannel(3).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not query complaint channel if has not complaint rights")>
        Sub ShouldNotQueryComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.IsType(GetType(String), channelscontroller.GetChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should share a channel if is channel creator")>
        Sub ShouldShareAChannelIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.True(channelscontroller.PublishChannel(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should publish complaint channel if has complaint rights")>
        Sub ShouldPublishComplaintChannelIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, True)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.True(channelscontroller.PublishChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not publish a channel if is not channel creator")>
        Sub ShouldNotPublishAChannelIfNotIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.False(channelscontroller.PublishChannel(3).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not publish complaint channel if has not complaint rights")>
        Sub ShouldNotPublishComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.False(channelscontroller.PublishChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should restrict a channel if is channel creator")>
        Sub ShouldRestrictAChannelIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.True(channelscontroller.RestrictChannel(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not restrict complaint channel even if has complaint rights")>
        Sub ShouldNotRestrictComplaintChannelEvenIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, True)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.False(channelscontroller.RestrictChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not restrict a channel if is not channel creator")>
        Sub ShouldNotRestrictAChannelIfNotIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.False(channelscontroller.RestrictChannel(3).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not restrict complaint channel if has not complaint rights")>
        Sub ShouldNotRestrictComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()

                'Assert
                Assert.False(channelscontroller.RestrictChannel(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should delete a channel if is channel creator")>
        Sub ShouldDeleteAChannelIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As HttpStatusCodeResult = channelscontroller.DeleteChannel(2)
                'Assert
                Assert.True(response.StatusCode = 200)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete complaint channel even if has complaint rights")>
        Sub ShouldNotDeleteComplaintChannelEvenIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As HttpStatusCodeResult = channelscontroller.DeleteChannel(1)
                'Assert
                Assert.True(response.StatusCode = 400)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete a channel if is not channel creator")>
        Sub ShouldNotDeleteAChannelIfNotIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As HttpStatusCodeResult = channelscontroller.DeleteChannel(3)
                'Assert
                Assert.True(response.StatusCode = 400)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete complaint channel if has not complaint rights")>
        Sub ShouldNotDeleteComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As HttpStatusCodeResult = channelscontroller.DeleteChannel(1)
                'Assert
                Assert.True(response.StatusCode = 400)

            End Using
        End Sub

        <Fact(DisplayName:="Should set conversation status if is channel creator")>
        Sub ShouldSetConversationStatusIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.SetConversationStatus(3, 1)
                'Assert
                Assert.True(response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not set conversation status if has no channel rights")>
        Sub ShouldNotQueryAConversationIfHasNoChannelRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.SetConversationStatus(5, 1)
                'Assert
                Assert.IsType(GetType(String), response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should set conversation status of complaint channel if has complaint rights")>
        Sub ShouldSetConversationStatusOfComplaintChannelIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, True)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.SetConversationStatus(1, 1)
                'Assert
                Assert.True(response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not set conversation status of complaint channel if has not complaint rights")>
        Sub ShouldNotSetConversationStatusOfComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.SetConversationStatus(1, 1)
                'Assert
                Assert.IsType(GetType(String), response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should add a conversation message if is channel creator")>
        Sub ShouldAddAConversationMessageIfIsChannelCreator()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.AddMessage(3, "message")
                'Assert
                Assert.True(response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not add a conversation message if has no channel rights")>
        Sub ShouldNotAddAConversationMessageIfHasNoChannelRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.AddMessage(5, "message")
                'Assert
                Assert.IsType(GetType(String), response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should add conversation message of complaint channel if has complaint rights")>
        Sub ShouldAddAConversationMessageOfComplaintChannelIfHasComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, True)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.AddMessage(1, "message")
                'Assert
                Assert.True(response.Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not add conversation message of complaint channel if has not complaint rights")>
        Sub ShouldNotAddAConversationMessageOfComplaintChannelIfHasNotComplaintRights()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim channelscontroller As New VTLive40.ChannelsController()
                Dim response As JsonResult = channelscontroller.AddMessage(1, "message")
                'Assert
                Assert.IsType(GetType(String), response.Data)

            End Using
        End Sub

    End Class

End Namespace