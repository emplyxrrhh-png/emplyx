Imports System.ComponentModel
Imports System.Web.Mvc
Imports System.Web.UI.WebControls
Imports DevExtreme.AspNet.Mvc
Imports Robotics.Base.DTOs
Imports ServiceApi
Imports VTLive40
Imports VT_XU_Base
Imports Xunit

Namespace Unit.Test

    <Collection("DEXController")>
    <CollectionDefinition("DEXController", DisableParallelization:=True)>
    <Category("DEXController")>
    Public Class DEXControllerTest

        Private ReadOnly channelsHelper As ChannelsHelper
        Private ReadOnly permissionsHelper As PermissionsHelper
        Private ReadOnly environtmentHelper As EnvironmentHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            channelsHelper = New ChannelsHelper
            permissionsHelper = New PermissionsHelper
            environtmentHelper = New EnvironmentHelper

        End Sub

        <Fact(DisplayName:="Should load DEX page if complaint channel is active and company exists and not logged in")>
        Sub ShouldLoadDEXPageIfComplaintChannelIsActiveAndCompanyExistsAndNotLoggedIn()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{companyCode, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)
                'Act

                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim actionResult As ViewResult = CType(dexController.Index(companyCode), ViewResult)

                'Assert
                Assert.NotNull(actionResult)
                Assert.Equal("DEX", actionResult.ViewName)

            End Using
        End Sub

        <Fact(DisplayName:="Should load DEX page if complaint channel is active and company exists and logged in in same company")>
        Sub ShouldLoadDEXPageIfComplaintChannelIsActiveAndCompanyExistsAndLoggedInInSameCompany()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()

                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("idi01", New Dictionary(Of String, String) From {{companyCode, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)
                'Act

                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim actionResult As ViewResult = CType(dexController.Index(companyCode), ViewResult)

                'Assert
                Assert.NotNull(actionResult)
                Assert.Equal("DEX", actionResult.ViewName)

            End Using
        End Sub

        <Fact(DisplayName:="Should load InvalidateSession page if complaint channel is active and company exists and logged in another same company")>
        Sub ShouldLoadInvalidateSessionPageIfComplaintChannelIsActiveAndCompanyExistsAndLoggedInAnotherSameCompany()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()

                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("idi02", New Dictionary(Of String, String) From {{companyCode, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(1, "1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)
                'Act

                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim actionResult As ViewResult = CType(dexController.Index(companyCode), ViewResult)

                'Assert
                Assert.NotNull(actionResult)
                Assert.Equal("InvalidateSession", actionResult.ViewName)

            End Using
        End Sub

        <Fact(DisplayName:="Should load NOSession page if complaint channel is inactive and company exists and logged in in same company")>
        Sub ShouldLoadNOSessionPageIfComplaintChannelIsInactiveAndCompanyExistsAndLoggedInInSameCompany()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()

                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("idi01", New Dictionary(Of String, String) From {{companyCode, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim actionResult As ViewResult = CType(dexController.Index(companyCode), ViewResult)

                'Assert
                Assert.NotNull(actionResult)
                Assert.Equal("NoSession", actionResult.ViewName)

            End Using
        End Sub

        <Fact(DisplayName:="Should load NOSession page when wrong id is on url")>
        Sub ShouldLoadNOSessionPageWhenWrongIdIsOnUrl()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim actionResult As ViewResult = CType(dexController.Index(companyCode), ViewResult)

                'Assert
                Assert.NotNull(actionResult)
                Assert.Equal("NoSession", actionResult.ViewName)

            End Using
        End Sub

        <Fact(DisplayName:="Should not validate key if lenght is less than 10")>
        Sub ShouldValidateKeyIfLenghtIs10OrMore()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "1234"
                Dim email As String = ""
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate key if no number is present")>
        Sub ShouldValidateKeyIfNoNumberIsPresent()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "asasasASAS+"
                Dim email As String = ""
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate key if no upper is present")>
        Sub ShouldValidateKeyIfNoUpperIsPresent()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "asasas1234+"
                Dim email As String = ""
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate key if no lower is present")>
        Sub ShouldValidateKeyIfNoLowerIsPresent()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "ASASAS1234+"
                Dim email As String = ""
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should validate key if complexity is correct")>
        Sub ShouldValidateKeyIfComplexityIsCorrect()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = ""
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of Boolean)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate email if arroba is not present")>
        Sub ShouldValidateEmailIfArrobaIsNotPresent()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = "isantaulariamuixicegid.com"
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate email if no point after arroba ")>
        Sub ShouldNotValidateEmailIfNoPointAfterArroba()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = "isantaulariamuixi@cegidcom"
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should validate email if is well informed")>
        Sub ShouldValidateEmailIfIsWellInformed()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = "isantaulariamuixi@cegid.com"
                Dim phone As String = ""

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of Boolean)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should not validate phone if lenght is less than 9")>
        Sub ShouldNotValidatePhoneIfLenghtIsLessThan9()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = "isantaulariamuixi@cegidcom"
                Dim phone As String = "9999"

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of String)(oResult.Data)
            End Using
        End Sub

        <Fact(DisplayName:="Should validate phone if length is correct")>
        Sub ShouldValidatePhoneIfLengthIsCorrect()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim companyCode As String = Guid.NewGuid.ToString()
                Dim key As String = "12345678As+"
                Dim email As String = "isantaulariamuixi@cegid.com"
                Dim phone As String = "999999999"

                permissionsHelper.StubSetIdentity(-1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.StubSetCompany("", New Dictionary(Of String, String) From {{Guid.NewGuid.ToString, "idi01"}, {Guid.NewGuid.ToString, "idi02"}})
                environtmentHelper.InitializeEnvirontment()
                channelsHelper.SetAvailableChannels(-1, "-1#1,2", {"2#3,4", "4#7,8"}, {"3#5,6"}, False)

                'Act
                Dim dexController As DEXController = New VTLive40.DEXController()
                Dim oResult As JsonResult = dexController.ValidateData(companyCode, key, email, phone)

                Assert.NotNull(oResult)
                Assert.IsType(Of Boolean)(oResult.Data)
            End Using
        End Sub

    End Class

End Namespace