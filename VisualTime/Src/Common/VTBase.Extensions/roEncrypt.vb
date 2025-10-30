Imports System.IO
Imports System.Security.Cryptography

Public Class roEncrypt

    Private Shared desCriptoService As TripleDESCryptoServiceProvider = New TripleDESCryptoServiceProvider
    Public Const LICENSE_FILE = "License.vtl"

    Private Shared ReadOnly Property Key As Byte()
        Get
            Dim oServerLic As New Robotics.VTBase.Extensions.roServerLicense()
            Dim ProductID = oServerLic.FeatureData("License", "ProductSerialNum")

            Dim slt(8) As Byte
            Dim keyBytes As New Rfc2898DeriveBytes(ProductID, slt)
            Return keyBytes.GetBytes(24)
        End Get
    End Property

    Private Shared ReadOnly Property EmptyKey As Byte()
        Get
            Dim ProductID = String.Empty
            Dim slt(8) As Byte
            Dim keyBytes As New Rfc2898DeriveBytes(ProductID, slt)
            Return keyBytes.GetBytes(24)
        End Get
    End Property

    Private Shared ReadOnly Property Vector As Byte()
        Get
            Return {0, 0, 0, 0, 0, 0, 0, 0}
        End Get
    End Property

    Public Shared Function Encrypt(toEncrypt As Byte()) As Byte()
        Return Transform(toEncrypt, desCriptoService.CreateEncryptor(Key, Vector))
    End Function

    Public Shared Function Decrypt(toDecrypt As Byte()) As Byte()

        Try
            Return Transform(toDecrypt, desCriptoService.CreateDecryptor(Key, Vector))
        Catch ex As CryptographicException
            Return Transform(toDecrypt, desCriptoService.CreateDecryptor(EmptyKey, Vector))
        End Try
    End Function

    Private Shared Function Transform(file As Byte(), ByVal CryptoTransform As ICryptoTransform) As Byte()
        Dim stream As MemoryStream = New MemoryStream
        Dim cryptoStream As CryptoStream = New CryptoStream(stream, CryptoTransform, CryptoStreamMode.Write)

        Dim Input As Byte() = file

        cryptoStream.Write(Input, 0, Input.Length)
        cryptoStream.FlushFinalBlock()

        Return stream.ToArray()
    End Function

End Class