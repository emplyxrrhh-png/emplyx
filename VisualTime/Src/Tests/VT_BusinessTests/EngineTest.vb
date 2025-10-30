Imports System.ComponentModel
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTShiftEngines
Imports VT_XU_Base
Imports VT_XU_Security

Imports Xunit

Namespace Unit.Test

    <Collection("Employee")>
    <CollectionDefinition("Employee", DisableParallelization:=True)>
    <Category("Employee")>
    Public Class ShiftEngineTest

        Private ReadOnly helperPassport As PassportHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperPassport = New PassportHelper
        End Sub

        <Fact(DisplayName:="Should Not Execute Detector If Employee ID Is Empty")>
        Sub ShouldNotExecuteDetectorIfEmployeeIDIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                ' Act
                Dim oDetectorManager As New roDetectorManager(1)
                oDetectorManager.ExecuteBatch(False, False)

                'Assert
                Assert.Equal(oDetectorManager.State.Result, EngineResultEnum.EmployeeRequired)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Execute Incidences If Employee ID Is Empty")>
        Sub ShouldNotExecuteIncidencesIfEmployeeIDIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                ' Act
                Dim oIncidencesManager As New roIncidencesManager(1)
                oIncidencesManager.ExecuteBatch(False, False)

                'Assert
                Assert.Equal(oIncidencesManager.State.Result, EngineResultEnum.EmployeeRequired)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Execute Causes If Employee ID Is Empty")>
        Sub ShouldNotExecuteCausesIfEmployeeIDIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                ' Act
                Dim oCausesManager As New roCausesManager(1)
                oCausesManager.ExecuteBatch(False, False)

                'Assert
                Assert.Equal(oCausesManager.State.Result, EngineResultEnum.EmployeeRequired)

            End Using
        End Sub

        <Fact(DisplayName:="Should Not Execute Accruals If Employee ID Is Empty")>
        Sub ShouldNotExecuteAccrualsIfEmployeeIDIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange
                helperPassport.PassportStub(1, helperDatalayer)
                ' Act
                Dim oAccrualManager As New roAccrualsManager(1)
                oAccrualManager.ExecuteBatch(False, False)

                'Assert
                Assert.Equal(oAccrualManager.State.Result, EngineResultEnum.EmployeeRequired)

            End Using
        End Sub

    End Class

End Namespace