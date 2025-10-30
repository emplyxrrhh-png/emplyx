Imports System.ComponentModel
Imports System.Text.Json.Nodes
Imports VT_XU_Base
Imports VTLive40
Imports Xunit

Namespace Unit.Test

    <Collection("CommuniqueController")>
    <CollectionDefinition("CommuniqueController", DisableParallelization:=True)>
    <Category("CommuniqueController")>
    Public Class CommuniqueControllerTest

        Private ReadOnly communiqueHelper As CommuniquesHelper
        Private ReadOnly permissionsHelper As PermissionsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            communiqueHelper = New CommuniquesHelper
            permissionsHelper = New PermissionsHelper

        End Sub

        <Fact(DisplayName:="Should see communique statidistics if is owner")>
        Sub ShouldSeeCommuniqueStadisticsIfIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                communiqueHelper.SetAvailableCommuniques(1, {1, 3}, {2})

                'Act
                Dim communiqueController As New VTLive40.CommuniqueController()
                Dim cObject As JsonObject = JsonObject.Parse(communiqueController.GetCommunique(1))

                'Assert
                Assert.NotNull(cObject)
                Assert.True(CInt(cObject.Item("Communique")("Id")) > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should not see communique statidistics if not is owner")>
        Sub ShouldNotSeeCommuniqueStadisticsIfNotIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                communiqueHelper.SetAvailableCommuniques(1, {1, 3}, {2})

                'Act
                Dim communiqueController As New VTLive40.CommuniqueController()
                Dim cObject As JsonObject = JsonObject.Parse(communiqueController.GetCommunique(2))

                'Assert
                Assert.NotNull(cObject)
                Assert.True(CInt(cObject.Item("Communique")("Id")) = -1)

            End Using
        End Sub

    End Class

End Namespace