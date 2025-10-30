Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("VTPortal")>
    <CollectionDefinition("VTPortal", DisableParallelization:=True)>
    <Category("VTPortal")>
    Public Class VTPortalApiTest
        Private ReadOnly helperWeb As WebHelper
        Private ReadOnly helperPassport As PassportHelper
        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperWeb = New WebHelper
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Set timezone is called during user login")>
        Function SetTimezoneIsCalledDuringUserLogin()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperPassport.LoginProcessSpys()
                helperPassport.InitPassportCredentials(New roPassportTicket() With {
                    .ID = 1,
                    .Name = "pep",
                    .Description = "",
                    .IsSupervisor = True
                })
                'Act
                Dim oState As New Robotics.Security.Base.roSecurityState(-1)
                Dim response As LoginResult = Robotics.Base.VTPortal.VTPortal.SecurityHelper.Login("pep", "pep", "ESP", "1.0.0", "1.0.0", "", "Madrid", "123", False, roAppSource.VTPortal, oState, "", True)

                'Assert
                Assert.True(helperPassport.SetTimezone)
            End Using
        End Function
    End Class
End Namespace
