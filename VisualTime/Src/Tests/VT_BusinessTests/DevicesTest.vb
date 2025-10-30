Imports System.ComponentModel
Imports VT_XU_Base
Imports VT_XU_Security
Imports Robotics.Base.VTTerminals
Imports Robotics.Comms.Base
Imports Xunit
Imports VT_XU_Common

Namespace Unit.Test

    <Collection("Terminals")>
    <CollectionDefinition("Terminals", DisableParallelization:=True)>
    <Category("Terminals")>
    Public Class TerminalsTest
        Private ReadOnly helperAdvancedParameters As AdvancedParametersHelper
        Private ReadOnly helperPassport As PassportHelper
        Private ReadOnly helperDataLayer As DatalayerHelper
        Private ReadOnly helperTerminals As TerminalsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperPassport = New PassportHelper
            helperAdvancedParameters = New AdvancedParametersHelper
            helperDataLayer = New DatalayerHelper
            helperTerminals = New TerminalsHelper
        End Sub

        <Fact(DisplayName:="Version Code for mx9 devices should be between five and six characters long")>
        Sub VersionCodeForMx9DevicesShouldBeBetweenFiveAndSixCharactersLong()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange

                ' Act
                Dim strVersionCode As String = Robotics.Base.VTTerminals.roTerminalsHelper.GetVersionCode("3.0.5")

                'Assert
                Assert.True(strVersionCode.Length >= 5 AndAlso strVersionCode.Length <= 6)
            End Using
        End Sub

        <Fact(DisplayName:="Old mx9 version code should be considered 2.0.5")>
        Sub OldMx9VersionCodeShouldBeConsidered205()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange

                ' Act
                Dim strVersionCode As String = Robotics.Base.VTTerminals.roTerminalsHelper.GetVersionCode("1.0")

                'Assert
                Assert.Equal(strVersionCode, "20005")
            End Using
        End Sub

        <Fact(DisplayName:="Unknown sync tasks should be ignored on mx9 devices")>
        Sub UnknownSyncTasksShouldBeIgnoredOnMx9Devices()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange
                helperAdvancedParameters.AdvancedParameterCacheStub(New Dictionary(Of String, String) From {{"Terminals.MaxSyncTasksRetries", "100"}})
                Dim tMock As New DataTableMock
                Dim dDataTStub As New Dictionary(Of String, DataTableMock)
                'Datos del canal 1
                tMock = New DataTableMock With {.columns = {"ID", "Task", "IDEmployee", "IDFinger", "Parameter1", "Parameter2", "TaskData", "TaskRetries"},
                    .values = New Object()() {New Object() {"1", "addmock", "0", "0", "", "", "", "0"}}}
                dDataTStub.Add("TerminalsSyncTasks", tMock)
                Dim dtTerminalsSyncTasks As DataTable = helperDataLayer.CreateDataTableStubExt(dDataTStub)

                ' Act
                Dim oTerminalsManager As roTerminalsManager = New roTerminalsManager(Nothing)
                Dim oSyncTask As roTerminalsSyncTasks = New roTerminalsSyncTasks(1)
                oSyncTask.LoadNext()

                'Assert
                Assert.True(oSyncTask.Task = roTerminalsSyncTasks.SyncActions.none)
            End Using
        End Sub

        <Fact(DisplayName:="Should Enable BatchMode When BatchSize Greater Than 0")>
        Public Sub ShouldEnableBatchModeWhenBatchSizeGreaterThan0()
            ' Arrange
            Dim syncTasks As New roTerminalsSyncTasks(1)

            ' Act
            syncTasks.SetBatchSize(10)

            ' Assert
            Assert.Equal(roTerminalsSyncTasks.eDbWorkMode.Batch, syncTasks.WorkMode)
        End Sub

        <Fact(DisplayName:="Should Enable DirectMode When BatchSize Equals Or Less Than 0")>
        Public Sub ShouldEnableDirectModeWhenBatchSizeEqualsOrLessThan0()
            ' Arrange
            Dim syncTasks As New roTerminalsSyncTasks(1)

            ' Act
            syncTasks.SetBatchSize(0)

            ' Assert
            Assert.Equal(roTerminalsSyncTasks.eDbWorkMode.Direct, syncTasks.WorkMode)
        End Sub

        <Fact(DisplayName:="Should Not Call ExecuteSql When Adding Task And Batch Mode Enabled")>
        Public Sub ShouldNotCallExecuteSqlWhenAddingTaskAndBatchModeEnabled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, 0,,,,,, False)

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = helperDataLayer.SqlExecuteString.None)
            End Using
        End Sub

        <Fact(DisplayName:="Should Call ExecuteSql When Adding Task And Direct Mode Enabled")>
        Public Sub ShouldCallExecuteSqlWhenAddingTaskAndDirectModeEnabled()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(0)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, 0,,,,,, False)

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = helperDataLayer.SqlExecuteString.BroadcasterAddTask)
            End Using
        End Sub

        <Fact(DisplayName:="Should Not Try To Delete Related Tasks Via ExecuteSql When Adding Task And Batch Mode Enabled And No Prior Tasks")>
        Public Sub ShouldNotTryToDeleteRelatedTasksViaExecuteSqlWhenAddingTaskAndBatchModeEnabledAndNoPriorTasks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, 0,,,,,, True)

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlWasCalled = helperDataLayer.SqlExecuteString.None)
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Related Tasks Via ExecuteSql When Adding Task And Direct Mode Enabled And No Prior Tasks")>
        Public Sub ShouldTryToDeleteRelatedTasksViaExecuteSqlWhenAddingTaskAndDirectModeEnabledAndNoPriorTasks()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(0)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, 0,,,,,, True)

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing AddEmploye And DelEmployee Tasks When Adding AddEmploye Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingAddEmployeAndDelEmployeeTasksWhenAddingAddEmployeTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(0)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployee, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployee")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployee")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing AddEmploye And DelEmployee Tasks When Adding DelEmploye Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingAddEmployeAndDelEmployeeTasksWhenAddingDelEmployeTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployee, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployee")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployee")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding DelAllEmployes Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllEmployesTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployees, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delallemployees")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployee")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployee")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delallemployeetimezones")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding AddCard Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddCardTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addcard, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addcard")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delcard")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding DelCard Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelCardTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delcard, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addcard")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delcard")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding DelAllCards Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllCardsTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallcards, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%card%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addbio Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddBioTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addbio, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbio")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbio")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding DelBio Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelBioTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delbio, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbio")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbio")))
            End Using
        End Sub
        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding DelAllBios Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllBiosTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbios, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbio")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbio")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delallbios")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addbiodataface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddBioDataFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodataface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbiodataface")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbiodataface")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delbiodataface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelBioDataFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delbiodataface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbiodataface")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbiodataface")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallbiodataface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllBioDataFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodataface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%biodataface")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addbiodatapalm Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddBioDataPalmTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addbiodatapalm, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addbiodatapalm")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delbiodatapalm")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallbiodatapalm Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllBioDataPalmTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallbiodatapalm, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%biodatapalm")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addface")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delface")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addface")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delface")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallface Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllFaceTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallface, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%face%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addphoto Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddPhotoTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addphoto, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addphoto")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delphoto")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delphoto Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelPhotoTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delphoto, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addphoto")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delphoto")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallphotos Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllPhotosTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallphotos, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%photo%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addemployeegroup Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddEmployeeGroupTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployeegroup, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployeegroup")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployeegroup")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delemployeegroup Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelEmployeeGroupTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeegroup, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployeegroup")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployeegroup")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallemployeegroup Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllEmployeeGroupTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeegroup, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%employeegroup%")))
            End Using
        End Sub
        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding adddocument Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddDocumentTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.adddocument, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("adddocument")))
            End Using
        End Sub
        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding deldocument Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelDocumentTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.deldocument, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("deldocument")))
            End Using
        End Sub
        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delalldocuments Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllDocumentsTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delalldocuments, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%document%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addtimezone Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddTimeZoneTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addtimezone, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addtimezone")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("deltimezone")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding deltimezone Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelTimeZoneTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.deltimezone, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addtimezone")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("deltimezone")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delalltimezones Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllTimeZonesTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delalltimezones, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addtimezone")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("deltimezone")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delalltimezones")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding addemployeeaccesslevel Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingAddEmployeeAccessLevelTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployeeaccesslevel")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delemployeeaccesslevel Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelEmployeeAccessLevelTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployeeaccesslevel")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployeeaccesslevel")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallemployeeaccesslevel Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllEmployeeAccessLevelTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%employeeaccesslevel%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding setterminalconfig Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingSetTerminalConfigTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.setterminalconfig, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("setterminalconfig")))
            End Using
        End Sub


        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delemployeetimezones Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelEmployeeTimeZonesTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delemployeetimezones, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("addemployeetimezones")))
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("delemployeetimezones")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding delallemployeetimezones Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingDelAllEmployeeTimeZonesTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.delallemployeetimezones, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("%employeetimezones%")))
            End Using
        End Sub

        <Fact(DisplayName:="Should Try To Delete Existing Related Tasks When Adding setsirens Task And Prior Tasks Exists")>
        Public Sub ShouldTryToDeleteExistingRelatedTasksWhenAddingSetSirensTaskAndPriorTasksExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                ' Arrange
                Dim syncTasks As New roTerminalsSyncTasks(1)
                helperDataLayer.ExecuteSqlSpy()

                ' Act
                syncTasks.SetBatchSize(10)
                syncTasks.addSyncTask(roTerminalsSyncTasks.SyncActions.setsirens, 0,,,,,, True)
                syncTasks.PersistTasksToDatabase()

                ' Assert
                Assert.True(helperDataLayer.ExecuteSqlHistory.Any(Function(item) item.SqlExecuteString = helperDataLayer.SqlExecuteString.BroadcasterDeleteTask AndAlso item.SqlString.Contains("setsirens")))
            End Using
        End Sub

        <Fact(DisplayName:="Should update time gate Last Action when update time gate status")>
        Sub ShouldUpdateTimeGateLastActionWhenUpdateTimeGateStatus()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange                
                Dim terminal As Robotics.Base.VTBusiness.Terminal.roTerminal = helperTerminals.GetTerminalBySerialNumber()
                helperTerminals.Load()
                helperTerminals.PutValues()
                helperTerminals.Save()
                helperTerminals.ReturnFields()
                ' Act
                Robotics.Base.VTPortal.VTPortal.TerminalsHelper.SetTimeGateStatus("serialNumber", "apkversion", terminalState)

                'Assert
                Assert.True(helperTerminals.updatedLastAction > Now.AddHours(-1))
            End Using
        End Sub

        <Fact(DisplayName:="Should update time gate Firm Version when update time gate status")>
        Sub ShouldUpdateTimeGateFirmVersionWhenUpdateTimeGateStatus()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                ' Arrange                
                Dim terminal As Robotics.Base.VTBusiness.Terminal.roTerminal = helperTerminals.GetTerminalBySerialNumber()
                helperTerminals.Load()
                helperTerminals.PutValues()
                helperTerminals.Save()
                helperTerminals.ReturnFields()
                helperTerminals.UpdateStatus()
                ' Act
                Robotics.Base.VTPortal.VTPortal.TerminalsHelper.SetTimeGateStatus("serialNumber", "apkversion", terminalState)

                'Assert
                Assert.True(helperTerminals.statusUpdated)
            End Using
        End Sub

    End Class

End Namespace