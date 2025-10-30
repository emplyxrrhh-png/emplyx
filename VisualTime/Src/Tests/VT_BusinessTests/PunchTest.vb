Imports System.ComponentModel
Imports System.Drawing
Imports Robotics.Base
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.DataLayer
Imports VT_XU_Base
Imports VT_XU_Common
Imports VT_XU_Security
Imports Xunit

Namespace Unit.Test

    <Collection("Punch")>
    <CollectionDefinition("Punch", DisableParallelization:=True)>
    <Category("Punch")>
    Public Class PunchTest
        Private ReadOnly helper As PunchHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperEmployee As EmployeeHelper
        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperAzure As AzureHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperPunch As PunchHelper
        Private ReadOnly helperAccess As AccessTestHelper

        Sub New()
            helper = New PunchHelper
            helperBase = New BaseHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDatalayer = New DatalayerHelper
            helperEmployee = New EmployeeHelper
            helperAzure = New AzureHelper
            helperPassport = New PassportHelper
            helperPunch = New PunchHelper
            helperAccess = New AccessTestHelper
        End Sub

        <Fact(DisplayName:="Should Calculate Attendance Punch ActualType On New Punch If Type Is Auto")>
        Sub ShouldCalculateAttPunchActualTypeIfTypeIsAuto()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.CalculateTypeSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._AUTO

                oPunch.Save(, True)

                ' Assert
                Assert.True(helperBase.AuditWasCalled AndAlso helperPunch.RecalcTypeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Calculate Attendance Punch ActualType On New Punch If Type Is Not Auto")>
        Sub ShouldNotCalculateAttPunchActualTypeOnNewPunchIfTypeIsNotAuto()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                ' Verify that save punch finalize
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)

                helperPunch.CalculateTypeSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN

                oPunch.Save(, True)

                ' Assert
                Assert.True(helperBase.AuditWasCalled AndAlso Not helperPunch.RecalcTypeCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Notify When Employee Punch Comes With Temperature Alert")>
        Sub ShouldNotifyWhenEmployeePunchComesWithTemperatureAlert()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperPunch.TemperatureNotificationSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.TemperatureAlert = True
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperPunch.TemperatureNotificationCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Notify When Employee Punch Comes With Mask Alert")>
        Sub ShouldNotifyWhenEmployeePunchComesWithMaskAlert()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperPunch.MaskNotificationSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.MaskAlert = True
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperPunch.MaskNotificationCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Launch Engine Calcs When New Attendance Employee Punch Is Saved")>
        Sub ShouldLaunchEngineCalcsWhenNewAttendanceEmployeePunchIsSaved()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.EngineLaunchedSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperPunch.EngineLaunchedCalled AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Launch Engine Calcs When Non Attendance New Employee Punch Is Saved")>
        Sub ShouldNotLaunchEngineCalcsWhenNonAttendanceNewEmployeePunchIsSaved()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperPunch.EngineLaunchedSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._DR
                oPunch.Save(, True)
                ' Assert
                Assert.True(Not helperPunch.EngineLaunchedCalled AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Find Employee By Credential On New Punch If IDEmployee Not Provided And IDCredential Provided")>
        Sub ShouldTryToFindEmployeeByCredentialOnNewPunchIfIDEmployeeNotProvidedAndIDCredentialProvided()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperEmployee.GetEmployeeIDByIDCardSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDCredential = 121212121212
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperEmployee.EmployeeSearchedByCard AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Find Employee By Credential On New Punch If IDEmployee Not Provided And IDCredential Neither Provided")>
        Sub ShouldNotTryToFindEmployeeByCredentialOnNewPunchIfIDEmployeeNotProvidedAndIDCredentialNeitherProvided()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperEmployee.GetEmployeeIDByIDCardSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDCredential = 0
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Save(, True)
                ' Assert
                Assert.True(Not helperEmployee.EmployeeSearchedByCard AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Find Employee By Credential On New Punch If IDEmployee Is Provided Even If IDCredential Is Provided")>
        Sub ShouldNotTryToFindEmployeeByCredentialOnNewPunchIfIDEmployeeIsProvidedEvenIfIDCredentialIsProvided()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperEmployee.GetEmployeeIDByIDCardSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.IDCredential = 1212121212
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Save(, True)
                ' Assert
                Assert.True(Not helperEmployee.EmployeeSearchedByCard AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Shoul Upload Punch Picture To Azure If Picture Provideded")>
        Sub ShouldUploadPunchPictureToAzureIfPictureProvideded()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tMove)
                helperAzure.UploadPunchPhoto2AzureSpy(Nothing, True)
                helperDatalayer.ExecuteSqlSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Capture = New Bitmap(1, 1)
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperAzure.UploadPunchPhoto2AzureWasCalled AndAlso helperBase.AuditWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Shoul Delete Existing Punch Picture From Database And Upload To Azure")>
        Sub ShouldDeleteExistingPunchPictureFromDatabaseAndUploadToAzure()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = 1200
                oPunchTest.IDEmployee = 1
                oPunchTest.ShiftDate = New DateTime(2023, 1, 1)
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Type = DTOs.PunchTypeEnum._IN
                oPunchTest.ActualType = DTOs.PunchTypeEnum._IN
                helper.PunchesTableFromAdapterStub(oPunchTest)
                helperDatalayer.DbCommandMock()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMove)
                helperAzure.UploadPunchPhoto2AzureSpy(Nothing, True)
                helperDatalayer.ExecuteSqlSpy()

                ' Act
                Dim oPunch As New Punch.roPunch
                oPunch.ID = 1200
                oPunch.IDEmployee = 1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Capture = New Bitmap(1, 1)
                oPunch.PhotoOnAzure = False
                oPunch.Save(, True)
                ' Assert
                Assert.True(helperDatalayer.ExecuteSqlWasCalled = DatalayerHelper.SqlExecuteString.DeletePunchesCaptures AndAlso
                            helperBase.AuditWasCalled AndAlso
                            helperAzure.UploadPunchPhoto2AzureWasCalled)
            End Using
        End Sub

        <Fact(DisplayName:="Should give last punch zone id if doesnt have IDZone and Productive")>
        Sub ShouldGiveLastPunchZoneIdIfProductivePunch()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Punches where Datetime", 1337}})
                helperDatalayer.DbCommandMock()

                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = -1
                oPunchTest.ActualType = DTOs.PunchTypeEnum._TASK
                oPunchTest.Type = DTOs.PunchTypeEnum._TASK
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Save(, True)
                ' Assert
                Assert.Equal(oPunchTest.IDZone, 1337)
            End Using
        End Sub

        <Fact(DisplayName:="Should alert people capacity if punch zone is not default and has capacity and supervisor")>
        Sub ShouldAlertActualPunchPeopleCapacityIfPunchZoneIsNotDefault()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperAccess.InitZones()
                helper.GenerateNotificationStub()
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = -1
                oPunchTest.IDEmployee = 1
                oPunchTest.IDZone = 1337
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Save(, True)
                ' Assert
                Assert.True(helper.ExecuteSqlWasCalled = helper.SqlExecuteString.SendCapacityZoneNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Should not alert people capacity if punch zone has default zone")>
        Sub ShouldNotAlertActualPunchPeopleCapacityIfPunchZoneIsNotDefault()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helper.GenerateNotificationStub()
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = -1
                oPunchTest.IDEmployee = 1
                oPunchTest.IDZone = 255
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Save(, True)
                ' Assert
                Assert.True(helper.ExecuteSqlWasCalled = helper.SqlExecuteString.None)
            End Using
        End Sub

        <Fact(DisplayName:="Should alert people capacity if oldest punch zone is not default and has capacity and supervisor")>
        Sub ShouldAlertActualPunchPeopleCapacityIfOldestPunchZoneIsNotDefault()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperAccess.InitZones()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Punches where Datetime", 1337}})
                helper.GenerateNotificationStub()

                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = -1
                oPunchTest.IDEmployee = 1
                oPunchTest.IDZone = 255
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Save(, True)
                ' Assert
                Assert.True(helper.ExecuteSqlWasCalled = helper.SqlExecuteString.SendCapacityZoneNotification)
            End Using
        End Sub

        <Fact(DisplayName:="Should not alert people capacity if oldest punch zone default")>
        Sub ShouldNotAlertExitPeopleCapacityIfPumchZoneIsNotDefault()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helper.GenerateNotificationStub()
                helperDatalayer.ExecuteScalarStub(New Dictionary(Of String, Object) From {{"FROM Punches where Datetime", 255}})
                helper.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                Dim oPunchTest As New Punch.roPunch
                oPunchTest.ID = -1
                oPunchTest.IDEmployee = 1
                oPunchTest.IDZone = 255
                oPunchTest.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunchTest.Save(, True)
                ' Assert
                Assert.True(helper.ExecuteSqlWasCalled = helper.SqlExecuteString.None)
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Punch With Remarks If Remarks Are Setted")>
        Sub ShouldSavePunchWithRemarksIfRemarksAreSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN
                oPunch.Remarks = "Punch remarks"

                saved = oPunch.Save(, True)

                ' Assert
                Assert.True(saved AndAlso helperPunch.RemarksSaved.Equals("Punch remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Punch Without Remarks If Remarks Are Not Setted")>
        Sub ShouldSavePunchWithoutRemarksIfRemarksAreNotSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN

                saved = oPunch.Save(, True)

                ' Assert
                Assert.True(saved AndAlso helperPunch.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Punch With Not Reliable Cause If It Is Setted")>
        Sub ShouldSavePunchWithNotReliableCauseIfItIsSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN
                oPunch.NotReliableCause = DTOs.NotReliableCause.CostCenterForgottenPunch.ToString()

                saved = oPunch.Save(, True)

                ' Assert
                Assert.True(saved AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.CostCenterForgottenPunch.ToString()))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Punch Without Not Reliable Cause If It Is Not Setted")>
        Sub ShouldSavePunchWithoutNotReliableCauseIfItIsNotSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim oPunch As New Punch.roPunch
                oPunch.ID = -1
                oPunch.DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                oPunch.Type = DTOs.PunchTypeEnum._IN

                saved = oPunch.Save(, True)

                ' Assert
                Assert.True(saved AndAlso helperPunch.NotReliableCauseSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Cost Center Punch With Remarks If Remarks Are Setted")>
        Sub ShouldSaveCostCenterPunchWithRemarksIfRemarksAreSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim _IDEmployee As Integer = 1
                Dim _InputDateTime As DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                Dim _IDTerminal As Integer = 1
                Dim _IDCostCenter As Integer = 1
                Dim _InputCapture As Image = New Bitmap(1, 1)
                Dim _Punch As Punch.roPunch = New Punch.roPunch
                Dim _InputType As DTOs.PunchTypeEnum = DTOs.PunchTypeEnum._IN
                Dim _State As Punch.roPunchState = New Punch.roPunchState
                Dim _Lat As Double = 1.0
                Dim _Lon As Double = 1.0
                Dim _LocationZone As String = "Location zone"
                Dim _FullAddress As String = "Full address"
                Dim _TimeZone As String = "Time zone"
                Dim _comments As String = "Punch remarks"
                saved = Punch.roPunch.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone,, _comments)

                ' Assert
                Assert.True(saved AndAlso helperPunch.RemarksSaved.Equals("Punch remarks"))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Cost Center Punch Without Remarks If Remarks Are Not Setted")>
        Sub ShouldSaveCostCenterPunchWithoutRemarksIfRemarksAreNotSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim _IDEmployee As Integer = 1
                Dim _InputDateTime As DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                Dim _IDTerminal As Integer = 1
                Dim _IDCostCenter As Integer = 1
                Dim _InputCapture As Image = New Bitmap(1, 1)
                Dim _Punch As Punch.roPunch = New Punch.roPunch
                Dim _InputType As DTOs.PunchTypeEnum = DTOs.PunchTypeEnum._IN
                Dim _State As Punch.roPunchState = New Punch.roPunchState
                Dim _Lat As Double = 1.0
                Dim _Lon As Double = 1.0
                Dim _LocationZone As String = "Location zone"
                Dim _FullAddress As String = "Full address"
                Dim _TimeZone As String = "Time zone"
                saved = Punch.roPunch.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, _comments)

                ' Assert
                Assert.True(saved AndAlso helperPunch.RemarksSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Cost Center Punch Without Not Reliable Cause If Not Reliable Cause Is Not Setted")>
        Sub ShouldSaveCostCenterPunchWithoutNotReliableCauseIfNotReliableCauseIsNotSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim _IDEmployee As Integer = 1
                Dim _InputDateTime As DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                Dim _IDTerminal As Integer = 1
                Dim _IDCostCenter As Integer = 1
                Dim _InputCapture As Image = New Bitmap(1, 1)
                Dim _Punch As Punch.roPunch = New Punch.roPunch
                Dim _InputType As DTOs.PunchTypeEnum = DTOs.PunchTypeEnum._IN
                Dim _State As Punch.roPunchState = New Punch.roPunchState
                Dim _Lat As Double = 1.0
                Dim _Lon As Double = 1.0
                Dim _LocationZone As String = "Location zone"
                Dim _FullAddress As String = "Full address"
                Dim _TimeZone As String = "Time zone"
                saved = Punch.roPunch.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, _comments)

                ' Assert
                Assert.True(saved AndAlso helperPunch.NotReliableCauseSaved.Equals(String.Empty))
            End Using
        End Sub

        <Fact(DisplayName:="Should Save Cost Center Punch With Not Reliable Cause If Not Reliable Cause Is Setted")>
        Sub ShouldSaveCostCenterPunchWithNotReliableCauseIfNotReliableCauseIsSetted()
            ' Arrange
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                helperPassport.PassportStub(1, helperDatalayer)
                helperPunch.EmptyPunchesTableFromAdapterStub()
                helperDatalayer.DbCommandMock()

                ' Act
                Dim saved As Boolean = False
                Dim _IDEmployee As Integer = 1
                Dim _InputDateTime As DateTime = New DateTime(2023, 1, 1, 8, 0, 0)
                Dim _IDTerminal As Integer = 1
                Dim _IDCostCenter As Integer = 1
                Dim _InputCapture As Image = New Bitmap(1, 1)
                Dim _Punch As Punch.roPunch = New Punch.roPunch
                Dim _InputType As DTOs.PunchTypeEnum = DTOs.PunchTypeEnum._IN
                Dim _State As Punch.roPunchState = New Punch.roPunchState
                Dim _Lat As Double = 1.0
                Dim _Lon As Double = 1.0
                Dim _LocationZone As String = "Location zone"
                Dim _FullAddress As String = "Full address"
                Dim _TimeZone As String = "Time zone"
                saved = Punch.roPunch.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCapture, _Punch, _InputType, True, _State, _Lat, _Lon, _LocationZone, _FullAddress, _TimeZone, , _comments, DTOs.NotReliableCause.CostCenterForgottenPunch.ToString())

                ' Assert
                Assert.True(saved AndAlso helperPunch.NotReliableCauseSaved.Equals(DTOs.NotReliableCause.CostCenterForgottenPunch.ToString()))
            End Using
        End Sub

    End Class

End Namespace