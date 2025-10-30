Imports System.ComponentModel
Imports Azure
Imports Azure.Storage.Blobs
Imports Robotics
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTAnalyticsManager
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports VT_XU_Base
Imports VT_XU_Common
Imports Xunit

Namespace Unit.Test

    <Collection("Genius")>
    <CollectionDefinition("Genius", DisableParallelization:=True)>
    <Category("Genius")>
    Public Class GeniusTest
        Private ReadOnly helperGenius As GeniusHelper
        Private ReadOnly helperAzure As AzureHelper
        Private ReadOnly helperBase As BaseHelper
        Private ReadOnly hekperAzure As AzureHelper
        Private ReadOnly helperTask As TaskHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperGenius = New GeniusHelper()
            helperAzure = New AzureHelper()
            helperBase = New BaseHelper()
            helperAzure = New AzureHelper()
            helperTask = New TaskHelper()
        End Sub

        <Fact(DisplayName:="Genius result should return the same number of lines than executed stored procedure for parallelized BI execution")>
        Sub GeniusResultShouldReturnTheSameNumberOfLinesThanExecutedStoredProcedureForParallelizedBIExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oDBResults As DataTable = helperGenius.FillMockForDBResults(100)
                Dim xAzureContext As New Robotics.Azure.RoAzureSupport
                helperBase.InitJSONSerializers()
                helperAzure.InitAzureConfiguration()

                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()
                                                                                                                                                                                                End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.CreateIfNotExistsBlobHttpHeadersIDictionaryOfStringStringCancellationToken = Function()
                                                                                                                                                                     End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.AppendBlockStreamAppendBlobAppendBlockOptionsCancellationToken = Function()
                                                                                                                                                         End Function
                Dim lockObject As New Object()

                Dim writedLines As Integer = 0
                System.IO.Fakes.ShimTextWriter.AllInstances.WriteLineString = Function()
                                                                                  SyncLock lockObject
                                                                                      writedLines = writedLines + 1
                                                                                  End SyncLock
                                                                              End Function

                'Act
                xAzureContext.Any2GeniusParalellized(oDBResults, "test", Nothing, Nothing, DTOs.roLiveQueueTypes.analyticsbi, "test", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "test")
                'Assert
                Assert.Equal(writedLines, 102)
            End Using
        End Sub

        <Fact(DisplayName:="Genius result should return the same number of lines than executed stored procedure for non parallelized BI execution")>
        Sub GeniusResultShouldReturnTheSameNumberOfLinesThanExecutedStoredProcedureForNonParallelizedBIExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oDBResults As DataTable = helperGenius.FillMockForDBResults(100)
                Dim xAzureContext As New Robotics.Azure.RoAzureSupport
                helperBase.InitJSONSerializers()
                helperAzure.InitAzureConfiguration()


                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()

                                                                                                                                                                                                End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.CreateIfNotExistsBlobHttpHeadersIDictionaryOfStringStringCancellationToken = Function()

                                                                                                                                                                     End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.AppendBlockStreamAppendBlobAppendBlockOptionsCancellationToken = Function()

                                                                                                                                                         End Function
                helperAzure.UploadStream2BlobInCompanyContainer()
                Dim lockObject As New Object()

                Dim writedLines As Integer = 0
                System.IO.Fakes.ShimTextWriter.AllInstances.WriteLineString = Function()
                                                                                  SyncLock lockObject
                                                                                      writedLines = writedLines + 1
                                                                                  End SyncLock
                                                                              End Function

                'Act
                xAzureContext.Any2Genius(oDBResults, "test", Nothing, Nothing, DTOs.roLiveQueueTypes.analyticsbi, "test", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, "test")
                'Assert
                Assert.Equal(writedLines, 102)
            End Using
        End Sub

        <Fact(DisplayName:="Genius result should return the same number of lines than executed stored procedure for parallelized and non BI execution")>
        Sub GeniusResultShouldReturnTheSameNumberOfLinesThanExecutedStoredProcedureForParallelizedAndNonBIExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oDBResults As DataTable = helperGenius.FillMockForDBResults(1000)
                Dim xAzureContext As New Robotics.Azure.RoAzureSupport
                helperBase.InitJSONSerializers()
                helperAzure.InitAzureConfiguration()

                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()

                                                                                                                                                                                                End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.CreateIfNotExistsBlobHttpHeadersIDictionaryOfStringStringCancellationToken = Function()

                                                                                                                                                                     End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.AppendBlockStreamAppendBlobAppendBlockOptionsCancellationToken = Function()

                                                                                                                                                         End Function
                Dim lockObject As New Object()

                Dim writedLines As Integer = 0
                System.IO.Fakes.ShimTextWriter.AllInstances.WriteLineString = Function()
                                                                                  SyncLock lockObject
                                                                                      writedLines = writedLines + 1
                                                                                  End SyncLock
                                                                              End Function

                'Act
                xAzureContext.Any2GeniusParalellized(oDBResults, "test", Nothing, Nothing, DTOs.roLiveQueueTypes.analytics, "test", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
                'Assert
                Assert.Equal(writedLines, 1002)
            End Using
        End Sub

        <Fact(DisplayName:="Genius result should return the same number of lines than executed stored procedure for non parallelized and non BI execution")>
        Sub GeniusResultShouldReturnTheSameNumberOfLinesThanExecutedStoredProcedureForNonParallelizedAndNonBIExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oDBResults As DataTable = helperGenius.FillMockForDBResults(1000)
                Dim xAzureContext As New Robotics.Azure.RoAzureSupport
                helperBase.InitJSONSerializers()
                helperAzure.InitAzureConfiguration()

                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()

                                                                                                                                                                                                End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.CreateIfNotExistsBlobHttpHeadersIDictionaryOfStringStringCancellationToken = Function()

                                                                                                                                                                     End Function
                Azure.Storage.Blobs.Specialized.Fakes.ShimAppendBlobClient.AllInstances.AppendBlockStreamAppendBlobAppendBlockOptionsCancellationToken = Function()

                                                                                                                                                         End Function
                Dim lockObject As New Object()

                Dim writedLines As Integer = 0
                System.IO.Fakes.ShimTextWriter.AllInstances.WriteLineString = Function()
                                                                                  SyncLock lockObject
                                                                                      writedLines = writedLines + 1
                                                                                  End SyncLock
                                                                              End Function
                helperAzure.UploadStream2Blob()
                xAzureContext.Any2Genius(oDBResults, "test", Nothing, Nothing, DTOs.roLiveQueueTypes.analytics, "test", Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing)
                'Assert
                Assert.Equal(writedLines, 1004)
            End Using
        End Sub

        <Fact(DisplayName:="Should find genius' BI execution when report's name has special characters")>
        Sub ShouldFindGeniusBIExecutionWhenReportsNameHasSpecialCharacters()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helperAzure.InitAzureConfiguration()
                helperTask.Load("00. FICHAJES (Prueba)")
                helperTask.Save()
                helperAzure.ListFiles()
                helperGenius.GetGeniusExecutionById()
                helperGenius.GenerateDBCube()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tGenius)
                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()
                                                                                                                                                                                                End Function
                helperAzure.GetFileSaSTokenWithURI()
                helperGenius.UpdateGeniusExecution()
                'Act
                Dim taskExecution As AnalyticsServerTaskExecution = New AnalyticsServerTaskExecution()
                Dim row As roLiveTask = New roLiveTask(1, New roLiveTaskState(1))
                taskExecution.ExecuteTask(row)
                'Assert
                Assert.Equal(helperGenius.GeniusViewName, "00. FICHAJES (Prueba)/00. FICHAJES (Prueba)_3")
            End Using
        End Sub

        <Fact(DisplayName:="Should find genius' BI execution when report's name hasn't special characters")>
        Sub ShouldFindGeniusBIExecutionWhenReportsNameHasNotSpecialCharacters()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helperAzure.InitAzureConfiguration()
                helperTask.Load("FICHAJES")
                helperTask.Save()
                helperAzure.ListFiles()
                helperGenius.GetGeniusExecutionById()
                helperGenius.GenerateDBCube()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tGenius)
                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()
                                                                                                                                                                                                End Function
                helperAzure.GetFileSaSTokenWithURI()
                helperGenius.UpdateGeniusExecution()
                'Act
                Dim taskExecution As AnalyticsServerTaskExecution = New AnalyticsServerTaskExecution()
                Dim row As roLiveTask = New roLiveTask(1, New roLiveTaskState(1))
                taskExecution.ExecuteTask(row)
                'Assert
                Assert.Equal(helperGenius.GeniusViewName, "FICHAJES/FICHAJES_3")
            End Using
        End Sub

        <Fact(DisplayName:="Should find genius' BI execution when report's name has special characters and it is the first execution")>
        Sub ShouldFindGeniusBIExecutionWhenReportsNameHasSpecialCharactersAndItIsTheFirstExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helperAzure.InitAzureConfiguration()
                helperTask.Load("00. FICHAJES (Prueba)")
                helperTask.Save()
                helperAzure.ListFiles(True)
                helperGenius.GetGeniusExecutionById()
                helperGenius.GenerateDBCube()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tGenius)
                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()
                                                                                                                                                                                                End Function
                helperAzure.GetFileSaSTokenWithURI()
                helperGenius.UpdateGeniusExecution()
                'Act
                Dim taskExecution As AnalyticsServerTaskExecution = New AnalyticsServerTaskExecution()
                Dim row As roLiveTask = New roLiveTask(1, New roLiveTaskState(1))
                taskExecution.ExecuteTask(row)
                'Assert
                Assert.Equal(helperGenius.GeniusViewName, "00. FICHAJES (Prueba)/00. FICHAJES (Prueba)_1")
            End Using
        End Sub

        <Fact(DisplayName:="Should find genius' BI execution when report's name hasn't special characters and it is first execution")>
        Sub ShouldFindGeniusBIExecutionWhenReportsNameHasNotSpecialCharactersAndItIsFirstExecution()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helperAzure.InitAzureConfiguration()
                helperTask.Load("FICHAJES")
                helperTask.Save()
                helperAzure.ListFiles(True)
                helperGenius.GetGeniusExecutionById()
                helperGenius.GenerateDBCube()
                helperBase.AuditSpy(Robotics.VTBase.Audit.Action.aInsert, Robotics.VTBase.Audit.ObjectType.tGenius)
                Azure.Storage.Blobs.Fakes.ShimBlobContainerClient.AllInstances.CreateIfNotExistsPublicAccessTypeIDictionaryOfStringStringBlobContainerEncryptionScopeOptionsCancellationToken = Function()
                                                                                                                                                                                                End Function
                helperAzure.GetFileSaSTokenWithURI()
                helperGenius.UpdateGeniusExecution()
                'Act
                Dim taskExecution As AnalyticsServerTaskExecution = New AnalyticsServerTaskExecution()
                Dim row As roLiveTask = New roLiveTask(1, New roLiveTaskState(1))
                taskExecution.ExecuteTask(row)
                'Assert
                Assert.Equal(helperGenius.GeniusViewName, "FICHAJES/FICHAJES_1")
            End Using
        End Sub

    End Class

End Namespace