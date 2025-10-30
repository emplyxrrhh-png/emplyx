Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports PgpCore

''' <summary>
''' This class shall be replaced by the real CryptographyHelper class once I have it.
''' </summary>
Public Class PGPCryptographyHelper


    Private privateKeyLoaded As Boolean = False
    Private publicKeyLoaded As Boolean = False

    Public Sub New()
    End Sub

#Region "PGPcore"
    Public Function EncryptPGP(inputBytes As Byte(), publicKeyBytes As Byte(), Optional withIntegrityCheck As Boolean = True) As Byte()
        Dim encryptedData As Byte()


        Using publicKeyStream As New MemoryStream(publicKeyBytes)
            Dim encryptionKeys As EncryptionKeys = New EncryptionKeys(publicKeyStream)

            Using pgp As New PGP(encryptionKeys)
                ' Encriptar el array de bytes
                Using inputStream As New MemoryStream(inputBytes)
                    Using outputStream As New MemoryStream()
                        ' Encriptar el contenido
                        pgp.EncryptStream(inputStream, outputStream, True, True)
                        ' Obtener el array de bytes del resultado
                        encryptedData = outputStream.ToArray()
                    End Using
                End Using
            End Using
        End Using

        Return encryptedData
    End Function

    Public Function DecryptPGP(inputBytes As Byte(), privateKeyBytes As Byte(), passPhrase As String) As Byte()
        Dim decryptedData As Byte()

        ' Usar streams en memoria para desencriptar
        Using inputStream As New MemoryStream(inputBytes)
            Using privateKeyStream As New MemoryStream(privateKeyBytes)
                Using outputStream As New MemoryStream()

                    Dim decriptionKey As EncryptionKeys = New EncryptionKeys(privateKeyStream, passPhrase)

                    ' Crear una instancia de PGP
                    Using pgp As New PGP(decriptionKey)
                        ' Desencriptar usando los streams en memoria
                        pgp.DecryptStream(inputStream, outputStream)

                        ' Obtener el array de bytes del resultado
                        decryptedData = outputStream.ToArray()
                    End Using
                End Using
            End Using
        End Using

        Return decryptedData
    End Function
#End Region

End Class

