Imports System.IO
Imports System.Linq
Imports System.Net
Imports Renci.SshNet
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Namespace VTStorageFTPsynchronization

    Public Class roStorageFTPsynchronizationImportManager
        Inherits roStorageFTPsynchronizationManager

        Public Sub Move2Storage(ByVal oFTPConnection As roFTPConnection)
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile::Start Import " & oFTPConnection.Id)

            Try
                Dim client As SftpClient = connect2FTP(oFTPConnection)

                If client IsNot Nothing Then
                    Dim filesInFolder As List(Of String) = New List(Of String)()
                    filesInFolder = client.ListDirectory(oFTPConnection.ftps.folder).Where(Function(it) it.IsDirectory = False).[Select](Function(it) it.Name).ToList()
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile::" & filesInFolder.Count & "files for import")

                    For Each fileName As String In filesInFolder
                        Try
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile::Importing file " & fileName)
                            Dim hasImported As Boolean = ImportFile(oFTPConnection, fileName, client)

                            If hasImported Then
                                Dim folderExists As Boolean = client.Exists(oFTPConnection.ftps.folder + BAK_FOLDER)
                                If Not folderExists Then client.CreateDirectory(oFTPConnection.ftps.folder + BAK_FOLDER)
                                client.RenameFile(oFTPConnection.ftps.folder & fileName, oFTPConnection.ftps.folder + BAK_FOLDER & fileName)
                            Else
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile:: An error occurred while importing from FTP")
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::Import::Problem importing file " & fileName & " - " & ex.Message)
                        End Try
                    Next

                    client.Disconnect()
                    client.Dispose()
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::Import::" & ex.Message)
            End Try

            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::End Import " & oFTPConnection.Id)
        End Sub

        Private Function ImportFile(ByVal oFTPConnection As roFTPConnection, ByVal FTPFileName As String, ByVal client As SftpClient) As Boolean
            Dim hasImported As Boolean = False

            Try

                If client IsNot Nothing AndAlso RoAzureSupport.GetLastModifiedOnCompanyContainter(FTPFileName, oFTPConnection.storageFolder, roLiveQueueTypes.datalink) = DateTime.MinValue Then
                    Dim btfile As Stream = New MemoryStream()
                    client.DownloadFile(oFTPConnection.ftps.folder & FTPFileName, btfile)
                    btfile.Position = 0
                    hasImported = RoAzureSupport.SaveFileOnCompanyContainer(btfile, FTPFileName, oFTPConnection.storageFolder, roLiveQueueTypes.datalink)
                    btfile.Close()
                    btfile.Dispose()
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile:: File exists")
                End If
            Catch ex As WebException
                Dim status As String = (CType(ex.Response, FtpWebResponse)).StatusDescription
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile::" & ex.Message)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationImportManager::ImportFile::" & ex.Message)
            End Try

            Return hasImported
        End Function

    End Class

End Namespace