Imports System.ComponentModel
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports VTLiveApi
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports VT_XU_Datalink
Imports Xunit
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkDailyCause
Imports Robotics.ExternalSystems.DataLink

Namespace Unit.Test

    <Collection("TimeGate")>
    <CollectionDefinition("TimeGate", DisableParallelization:=True)>
    <Category("TimeGate")>
    Public Class TimeGateApiTest
        Private ReadOnly helperWeb As WebHelper
        Private ReadOnly helperTerminals As TerminalsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperWeb = New WebHelper
            helperTerminals = New TerminalsHelper
        End Sub

        <Fact(DisplayName:="Should update timegate status when obtains timegate configuration")>
        Function ShouldUpdateTimeGateStatusWhenObtainsTimeGateConfiguration()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperTerminals.GetTimeGateConfiguration(True)
                helperTerminals.SetTimeGateStatus()

                'Act
                Dim timeGateService As New VTPortalWeb.TimeGate
                Dim response As roGenericResponse(Of Robotics.Base.DTOs.Timegate) = timeGateService.Initialize()

                'Assert
                Assert.True(helperTerminals.setTimeGateStatusCalled)
            End Using
        End Function

        <Fact(DisplayName:="Should not update timegate status when cannot obtains timegate configuration")>
        Function ShouldNotUpdateTimeGateStatusWhenCannotObtainsTimeGateConfiguration()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                helperWeb.HttpContextStub()
                helperTerminals.GetTimeGateConfiguration(False)
                helperTerminals.SetTimeGateStatus()

                'Act
                Dim timeGateService As New VTPortalWeb.TimeGate
                Dim response As roGenericResponse(Of Robotics.Base.DTOs.Timegate) = timeGateService.Initialize()

                'Assert
                Assert.False(helperTerminals.setTimeGateStatusCalled)
            End Using
        End Function
    End Class
End Namespace
