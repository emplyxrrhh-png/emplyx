Imports System.IO
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets
Imports Renci.SshNet
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Namespace VTStorageFTPsynchronization

    Public Class roStorageFTPsynchronizationManager
        Protected Const BAK_FOLDER As String = "BAK/"

        Public Function connect2FTP(ByVal oFTPConnection As roFTPConnection) As SftpClient
            If oFTPConnection.ftps IsNot Nothing Then
                Dim oFTPInfo As FTPInfo = oFTPConnection.ftps
                Dim keyVaultUri As String = Environment.GetEnvironmentVariable("Azure.KeyVaultUrl")
                Dim secretClient = New SecretClient(New Uri(keyVaultUri), New DefaultAzureCredential())
                Dim secretValue = secretClient.GetSecret(oFTPInfo.secretKeyId)
                Dim secretBytes = Convert.FromBase64String(secretValue.Value.Value)
                Dim ms As MemoryStream = New MemoryStream()
                ms.Write(secretBytes, 0, secretBytes.Length)
                ms.Position = 0
                Dim privateKey = New PrivateKeyFile(ms, oFTPInfo.password)
                Dim privateKeyFile As PrivateKeyFile() = {privateKey}
                Dim client = New SftpClient(oFTPInfo.server, oFTPInfo.user, privateKeyFile)
                client.Connect()
                Return client
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTStorageFTPSynchronization::roStorageFTPSynchronizationManager::Connect2FTP:: FTP URL is null")
                Return Nothing
            End If
        End Function

    End Class

End Namespace