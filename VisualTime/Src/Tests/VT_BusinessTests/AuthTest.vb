Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports VT_XU_Base
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <CollectionDefinition("Auth", DisableParallelization:=True)>
    <Collection("Auth")>
    <Category("Auth")>
    Public Class ValidateCredentialsTest

        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperSecurity As SecurityHelper
        Private ReadOnly helperPassport As PassportHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperDatalayer = New DatalayerHelper
            helperSecurity = New SecurityHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should clean username when password method and it starts with '.\'")>
        Sub ShouldCleanUsernameWhenItStartsWithDotBarAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()
                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, ".\username", "password", False, "", False, New roSecurityState(-1))
                'Assert

                Assert.Equal("username", helperSecurity.CredentialUsed)
            End Using
        End Sub

        <Fact(DisplayName:="Should add default domain to username when AD is configured and password method")>
        Sub ShouldAddDefaultDomainToUsernameWhenADdomainIsConfiguredAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()
                helperDatalayer.ExecuteScalarStub("VTLive.AD.DefaultDomain", "domain")

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "username", "password", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.Equal("domain\username", helperSecurity.CredentialUsed)
            End Using
        End Sub

        <Fact(DisplayName:="Should validate against AD when password method AND is AD credential")>
        Sub ShouldValidateAgainstADWhenCredentialIsADAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "\username", "password", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.True(helperSecurity.AuthenticateCount > 0 AndAlso helperSecurity.AuthenticationADCount > 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not validate against AD when username not contains '\' and password method ")>
        Sub ShouldNotValidateAgainstADWhenUsernameNotContainsSlashAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "username", "password", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.True(helperSecurity.AuthenticateCount > 0 AndAlso helperSecurity.AuthenticationADCount = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should overwrite validation when password method and is SSO and credential is AD")>
        Sub ShouldOverwriteValidationWhenCredentialIsADAndIsSSOAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()
                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "\username", "password", False, "", True, New roSecurityState(-1))

                'Assert
                Assert.True(helperSecurity.AuthenticateCount > 0 AndAlso helperSecurity.AuthenticationADCount = 0)
            End Using
        End Sub

        <Fact(DisplayName:="Should validate against DB when password method and NOT is SSO and credential NOT is AD")>
        Sub ShouldValidateAgainstDBWhenCredentialNotIsADAndNotIsSSOAndMethodIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "username", "password", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.True(helperSecurity.AuthenticateCount > 0 AndAlso helperSecurity.AuthenticationADCount = 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should load passport when correct credentials using password method and NOT is SSO and credential NOT is AD AND validate against DB")>
        Sub ShouldLoadPassportWhenWhenCredentialNotIsADAndNotIsSSOAndMethodIsPasswordAndValidatesAgainstDB()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "username", "password", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.NotNull(oValidatedPassport)
            End Using
        End Sub

        <Fact(DisplayName:="Should not load passport when wrong credentials using password method and NOT is SSO and credential NOT is AD AND validate against DB")>
        Sub ShouldNotLoadPassportWhenWhenCredentialNotIsADAndNotIsSSOAndMethodIsPasswordAndValidatesAgainstDB()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Password, "username", "wrongpassword", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.Null(oValidatedPassport)

            End Using
        End Sub

        <Fact(DisplayName:="Should validate against DB when another method than password")>
        Sub ShouldValidateAgainstDBWhenMethodNotIsPassword()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Biometry, "finger1", "fingerbiometry", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.True(helperSecurity.AuthenticateCount > 0)

            End Using
        End Sub

        <Fact(DisplayName:="Should load passport when another method than password and correct credentials")>
        Sub ShouldLoadPassportWhenMethodNotIsPasswordAndCorrectCredentials()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Biometry, "finger1", "fingerbiometry", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.NotNull(oValidatedPassport)
            End Using
        End Sub

        <Fact(DisplayName:="Should not load passport when another method than password and correct credentials")>
        Sub ShouldNotLoadPassportWhenMethodNotIsPasswordAndCorrectCredentials()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                helperSecurity.AuthenticateSpy()
                helperSecurity.AuthenticateADStub()

                'Act
                Dim oValidatedPassport As roPassportTicket = roPassportManager.ValidateCredentials(AuthenticationMethod.Biometry, "finger1", "wrongfingerbiometry", False, "", False, New roSecurityState(-1))

                'Assert
                Assert.Null(oValidatedPassport)
            End Using
        End Sub

    End Class

End Namespace