Imports System.IO
Imports System.Net
Imports System.Threading.Tasks
Imports Renci.SshNet
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Namespace VTStorageFTPsynchronization

    Public Class roStorageFTPsynchronizationExportManager
        Inherits roStorageFTPsynchronizationManager

        Public Sub Move2FTP(ByVal oFTPConnection As roFTPConnection)
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::ExportFile::Start Export " & oFTPConnection.Id)

            Try
                Dim filesInFolder As List(Of String) = RoAzureSupport.ListAllFiles(roLiveQueueTypes.datalink, oFTPConnection.storageFolder, True, oFTPConnection.client)
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::ExportFile::" & filesInFolder.Count & "files for export")

                For Each fileName As String In filesInFolder
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::ExportFile::Exporting file " & fileName)
                    Dim hasExported As Boolean = ExportFile(oFTPConnection, fileName).Result

                    If hasExported Then
                        RoAzureSupport.RenameFileInCompanyContainer(fileName, BAK_FOLDER & fileName, oFTPConnection.storageFolder, roLiveQueueTypes.datalink)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::ExportFile:: An error occurred while exporting to FTP")
                    End If
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::Export::" & ex.Message)
            End Try

            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::End Export " & oFTPConnection.Id)
        End Sub

        Private Function ExportFile(ByVal oFTPConnection As roFTPConnection, ByVal AzureFileBlobName As String) As Task(Of Boolean)
            Dim hasExported As Boolean = False

            Try

                If oFTPConnection.ftps IsNot Nothing Then
                    Dim client As SftpClient = connect2FTP(oFTPConnection)

                    If client IsNot Nothing AndAlso Not client.Exists(AzureFileBlobName) Then
                        Dim btfile As Byte() = Robotics.Azure.RoAzureSupport.DownloadFileFromCompanyContainer(AzureFileBlobName, oFTPConnection.storageFolder, roLiveQueueTypes.datalink, True)
                        client.UploadFile(New MemoryStream(btfile), oFTPConnection.ftps.folder & AzureFileBlobName, True)
                        client.Disconnect()
                        client.Dispose()
                        hasExported = True
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationExportManager::ExportFile:: FTP URL is null")
                End If
            Catch ex As WebException
                Dim status As String = (CType(ex.Response, FtpWebResponse)).StatusDescription
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roroStorageFTPSynchronizationExportManager::ExportFile::" & ex.Message)
            End Try

            Return Task.FromResult(hasExported)
        End Function

    End Class

End Namespace