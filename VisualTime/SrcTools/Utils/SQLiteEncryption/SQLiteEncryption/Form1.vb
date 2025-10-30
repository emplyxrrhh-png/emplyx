Imports System.IO
Imports System.Security.Cryptography
Public Class Form1
    Private oTrans As SQLite.SQLiteTransaction
    Private oCon As SQLite.SQLiteConnection
    Private databasePath As String = String.Empty
    Private bSerialInformed As Boolean = False

    Private Sub Cargar(sender As Object, e As EventArgs) Handles btnLoad.Click
        OpenFileDialog1.ShowDialog()
        databasePath = OpenFileDialog1.FileName
        If Not DatabaseHelper.IsEncrypted(databasePath) Then
            MsgBox("La base de datos no está encriptada !!!")
        Else
            btnDecrypt.Enabled = True
        End If

    End Sub

    Private Sub Desencriptar(sender As Object, e As EventArgs) Handles btnDecrypt.Click
        If txtSerial.Text.Length > 0 Then
            Try
                Int64.Parse(txtSerial.Text)
                bSerialInformed = True
            Catch ex As Exception
                If txtSerial.Text = "?" Then bSerialInformed = True
            End Try
        End If

        If Not bSerialInformed Then
            MsgBox("Indique un número de serie válido")
            txtSerial.Select()
            Exit Sub
        End If

        oCon = New SQLite.SQLiteConnection("Data Source=" & databasePath & ";")
        ' Probamos el password proporcionado
        oCon.SetPassword(SecurityHelper.Encrypt(txtSerial.Text))

        Try
            oCon.Open()
            oTrans = oCon.BeginTransaction()
        Catch ex As Exception
            If ex.Message.Contains("encry") Then
                MsgBox("Está encriptada para otro terminal ?")
                Exit Sub
            End If
        End Try
        oTrans.Rollback()

        oCon.ChangePassword("")
        oCon.Close()
        oCon = New SQLite.SQLiteConnection("Data Source=" & databasePath & ";")
        Try
            oCon.Open()
            MsgBox("Hecho!")
        Catch ex As Exception
            MsgBox("Ops!")
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        btnDecrypt.Enabled = False
    End Sub
End Class

Public Class DatabaseHelper
    Public Shared Function IsEncrypted(oConn As SQLite.SQLiteConnection)
        Dim oRet As Boolean = False
        Try
            oConn.Open()
            oConn.BeginTransaction()
        Catch ex As Exception
            If ex.Message.Contains("encry") Then
                oRet = True
            End If
        End Try

        Return oret
    End Function
    Public Shared Function IsEncrypted(dbPath As String)
        Dim oRet As Boolean = False
        Dim oConn As SQLite.SQLiteConnection

        Try
            oConn = New SQLite.SQLiteConnection("Data Source=" & dbPath & ";")
            oConn.Open()
            oConn.BeginTransaction()
        Catch ex As Exception
            If ex.Message.Contains("encry") Then
                oRet = True
            End If
        End Try

        Return oRet
    End Function
End Class

Public Class SecurityHelper
    Private Const _EncrKey As String = "@@##!!**"
    Public Shared Function Encrypt(ByVal strText As String, Optional ByVal strEncrKey As String = _EncrKey) As String
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Try
            Dim bykey() As Byte = System.Text.Encoding.UTF8.GetBytes(Left(strEncrKey, 8))
            Dim InputByteArray() As Byte = System.Text.Encoding.UTF8.GetBytes(strText)
            Dim des As New DESCryptoServiceProvider
            Dim ms As New MemoryStream
            Dim cs As New CryptoStream(ms, des.CreateEncryptor(bykey, IV), CryptoStreamMode.Write)
            cs.Write(InputByteArray, 0, InputByteArray.Length)
            cs.FlushFinalBlock()
            Return Convert.ToBase64String(ms.ToArray())
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function Decrypt(ByVal strText As String, Optional ByVal sDecrKey As String = _EncrKey) As String
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim inputByteArray(strText.Length) As Byte
        Try
            Dim byKey() As Byte = System.Text.Encoding.UTF8.GetBytes(Left(sDecrKey, 8))
            Dim des As New DESCryptoServiceProvider
            inputByteArray = Convert.FromBase64String(strText)
            Dim ms As New MemoryStream
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
            Return encoding.GetString(ms.ToArray(), 0, ms.Length)
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
End Class

Public Class SecurityHelerVTBusiness

    Public Shared Function Encrypt(ByVal strText As String, Optional ByVal padding As PaddingMode = PaddingMode.PKCS7, Optional ByVal mode As CipherMode = CipherMode.CBC) As String
        Dim key() As Byte = {&H15, &H2A, &H32, &H43, &HB4, &H15, &H76, &H17, &HC8, &H1F, &H2A, &H6B, &H1C, &H2D, &H3E, &H4F}
        Dim IV() As Byte = {&H10, &H1A, &H12, &H64, &H14, &H15, &H16, &H17, &H13, &H39, &H1A, &H1C, &H1C, &H1D, &H9E, &H1F}
        Try
            If strText <> "" Then
                Dim InputByteArray() As Byte = System.Text.Encoding.UTF8.GetBytes(strText)
                Dim aes As New AesCryptoServiceProvider
                aes.Padding = padding
                aes.Mode = mode
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, aes.CreateEncryptor(key, IV), CryptoStreamMode.Write)
                cs.Write(InputByteArray, 0, InputByteArray.Length)
                cs.FlushFinalBlock()
                Return Convert.ToBase64String(ms.ToArray())
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    ''' <summary>
    ''' Decrypt Text
    ''' </summary>
    ''' <param name="strText">text to decrypt</param>
    ''' <param name="padding">parameter on cipher process</param>
    ''' <param name="mode">cipher mode</param>
    ''' <returns>return text decrypter</returns>
    ''' <changes> rtort -20160511 - Change visibility to public for use in roLocation for SFTP</changes>
    Public Shared Function Decrypt(ByVal strText As String, Optional ByVal padding As PaddingMode = PaddingMode.PKCS7, Optional ByVal mode As CipherMode = CipherMode.CBC) As String
        Dim key() As Byte = {&H15, &H2A, &H32, &H43, &HB4, &H15, &H76, &H17, &HC8, &H1F, &H2A, &H6B, &H1C, &H2D, &H3E, &H4F}
        Dim IV() As Byte = {&H10, &H1A, &H12, &H64, &H14, &H15, &H16, &H17, &H13, &H39, &H1A, &H1C, &H1C, &H1D, &H9E, &H1F}
        Dim inputByteArray(strText.Length) As Byte
        Try
            If strText <> "" Then
                Dim aes As New AesCryptoServiceProvider
                aes.Padding = padding
                aes.Mode = mode
                inputByteArray = Convert.FromBase64String(strText)
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, aes.CreateDecryptor(key, IV), CryptoStreamMode.Write)
                cs.Write(inputByteArray, 0, inputByteArray.Length)
                cs.FlushFinalBlock()
                Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
                Return encoding.GetString(ms.ToArray())
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function
End Class