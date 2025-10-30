Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

' <summary>
' Algoritmos permitidos
' </summary>
Public Enum Algorithm
    MD5
    SHA1
    SHA256
    SHA384
    SHA512
End Enum

''' <summary>
''' This class shall be replaced by the real CryptographyHelper class once I have it.
''' </summary>
Public NotInheritable Class CryptographyHelper
    Public Const DecryptError As String = "DecryptExError"
    Public Const EncryptError As String = "EncryptError"

    ''' <summary>
    ''' Derives a pseudo-random hash key from an user name and a password.
    ''' </summary>
    ''' <param name="identityName">The user name from which to derive the key.</param>
    ''' <param name="password">The password from which to derive the key.</param>
    ''' <returns>A byte array containing the pseudo-random hash key.</returns>
    Public Shared Function GetPasswordHash(ByVal identityName As String, ByVal password As String) As Byte()
        If identityName Is Nothing OrElse identityName.Length = 0 OrElse password Is Nothing OrElse password.Length = 0 Then
            Return New Byte() {}
        End If

        ' Hash identity name bytes instead of using it directly
        ' because the salt must be at least 8 bytes. Even if the identity
        ' is shorter, the hash will have a fixed size.
        Dim Salt As Byte() = Hash(Encoding.UTF8.GetBytes(identityName.ToLower()), 16)
        Dim pdb As New Rfc2898DeriveBytes(password, Salt)
        Return pdb.GetBytes(16)
    End Function

    ''' <summary>
    ''' Returns a hash of the specified length for specified value.
    ''' </summary>
    Public Shared Function Hash(ByVal inData As Byte(), ByVal length As Integer) As Byte()
        ' Hash value.
        Dim Alg As MD5 = MD5.Create()
        Dim Data As Byte() = Alg.ComputeHash(inData)
        Alg.Clear()

        ' Convert hash to specified length.
        Dim Buffer(8) As Byte
        For I As Integer = 0 To inData.Length - 1
            Buffer(I Mod length) = Buffer(I Mod length) Xor inData(I)
        Next

        Return Buffer
    End Function

    Private Const _EncrKey As String = "&%#@?,:*"

    Public Shared Function EncryptEx(ByVal strText As String, ByVal strEncrKey As String) As String
        If String.IsNullOrEmpty(strText) Then
            Return ""
        End If

        Dim aesAlg As New AesManaged()

        Try
            Dim key As Byte() = StringToByteArray(strEncrKey)
            If key.Length = 64 Then
                key = DeriveKey(key, 32)
            End If
            aesAlg.Key = key

            ' Generar un IV aleatorio
            aesAlg.GenerateIV()

            ' Usar el IV generado en el proceso de cifrado
            Dim iv As Byte() = aesAlg.IV

            Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, iv)

            Using msEncrypt As New MemoryStream()
                ' Escribir el IV en la corriente de memoria
                msEncrypt.Write(iv, 0, iv.Length)

                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    Using swEncrypt As New StreamWriter(csEncrypt)
                        swEncrypt.Write(strText)
                    End Using
                End Using

                ' Convertir la corriente de memoria a una cadena Base64
                Return Convert.ToBase64String(msEncrypt.ToArray())
            End Using
        Finally
            aesAlg.Clear()
        End Try
    End Function

    Public Shared Function DecryptEx(ByVal encryptedText As String, ByVal strEncrKey As String) As String
        If String.IsNullOrEmpty(encryptedText) Then
            Return ""
        End If

        Dim aesAlg As New AesManaged()

        Try
            Dim key As Byte() = StringToByteArray(strEncrKey)
            If key.Length = 64 Then
                key = DeriveKey(key, 32)
            End If
            aesAlg.Key = key

            ' Convertir el mensaje cifrado de Base64 a bytes
            Dim cipherText As Byte() = Convert.FromBase64String(encryptedText)

            ' Extraer el IV de las primeras posiciones del mensaje cifrado
            Dim ivLength As Integer = aesAlg.IV.Length
            Dim iv As Byte() = cipherText.Take(ivLength).ToArray()

            ' Resto del mensaje cifrado sin el IV
            Dim cipherTextWithoutIV As Byte() = cipherText.Skip(ivLength).ToArray()

            aesAlg.IV = iv

            Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)

            Using msDecrypt As New MemoryStream(cipherTextWithoutIV)
                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                    Using srDecrypt As New StreamReader(csDecrypt)
                        Return srDecrypt.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return "DecError!"
        Finally
            aesAlg.Clear()
        End Try
    End Function

    Private Shared Function StringToByteArray(ByVal hex As String) As Byte()
        Dim retBytes As Byte() = New Byte() {}
        Try
            Dim numberChars As Integer = hex.Length
            retBytes = New Byte(numberChars / 2 - 1) {}

            For i As Integer = 0 To numberChars - 1 Step 2
                retBytes(i / 2) = Convert.ToByte(hex.Substring(i, 2), 16)
            Next
        Catch ex As Exception
            Return New Byte() {}
        End Try
        Return retBytes
    End Function

    Public Shared Function DeriveKey(ByVal originalKey As Byte(), Optional desiredKeyLength As Integer = 32) As Byte()
        Try
            Using hkdf As New HMACSHA256(originalKey)
                Dim derivedKey As Byte() = hkdf.ComputeHash(originalKey)
                Array.Resize(derivedKey, desiredKeyLength)
                Return derivedKey
            End Using
        Catch ex As Exception
            Return New Byte() {}
        End Try
    End Function

    Public Shared Function Encrypt(ByVal strText As String, Optional ByVal strEncrKey As String = _EncrKey) As String
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Try
            If strText <> "" Then
                Dim bykey() As Byte = System.Text.Encoding.UTF8.GetBytes(Left(strEncrKey, 8))
                Dim InputByteArray() As Byte = System.Text.Encoding.UTF8.GetBytes(strText)
                Dim des As New DESCryptoServiceProvider
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, des.CreateEncryptor(bykey, IV), CryptoStreamMode.Write)
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

    Public Shared Function Decrypt(ByVal strText As String, Optional ByVal sDecrKey As String = _EncrKey) As String
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim inputByteArray(strText.Length) As Byte
        Try
            If strText <> "" Then
                Dim byKey() As Byte = System.Text.Encoding.UTF8.GetBytes(Left(sDecrKey, 8))
                Dim des As New DESCryptoServiceProvider
                inputByteArray = Convert.FromBase64String(strText)
                Dim ms As New MemoryStream
                Dim cs As New CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write)
                cs.Write(inputByteArray, 0, inputByteArray.Length)
                cs.FlushFinalBlock()
                Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8
                Return encoding.GetString(ms.ToArray())
            Else
                Return ""
            End If
        Catch ex As Exception
            Return "DecryptError#725"
        End Try
    End Function

    Public Shared Function DecryptLiveApi(ByVal strText As String, Optional ByVal padding As PaddingMode = PaddingMode.PKCS7, Optional ByVal mode As CipherMode = CipherMode.CBC) As (Boolean, String)
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
                Return (True, encoding.GetString(ms.ToArray()))
            Else
                Return (True, "")
            End If
        Catch ex As Exception
            Return (False, ex.Message)
        End Try
    End Function

    Public Shared Function EncryptWithMD5(ByVal strText As String) As String
        Try
            Dim x As System.Security.Cryptography.MD5CryptoServiceProvider = New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(strText)
            bs = x.ComputeHash(bs)
            Dim s As System.Text.StringBuilder = New System.Text.StringBuilder()
            For Each b As Byte In bs
                s.Append(b.ToString("x2").ToLower())
            Next
            Return s.ToString()
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function EncryptWithSHA256(ByVal strText As String) As String
        Try
            Dim x As System.Security.Cryptography.SHA256CryptoServiceProvider = New System.Security.Cryptography.SHA256CryptoServiceProvider()
            Dim bs As Byte() = System.Text.Encoding.UTF8.GetBytes(strText)
            bs = x.ComputeHash(bs)
            Dim s As System.Text.StringBuilder = New System.Text.StringBuilder()
            For Each b As Byte In bs
                s.Append(b.ToString("x2").ToLower())
            Next
            Return s.ToString()
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Public Shared Function GetRoboticsValidationCode(ByVal sCode As String) As String
        Return GetRoHashCode(EncryptMD5(sCode + (9999999 - Convert.ToInt32(sCode)).ToString + Hex(sCode)))
    End Function

    Private Shared Function GetRoHashCode(ByVal sCodeToHash As String) As String
        Dim sInitial As String
        Dim sPart1_hex, sPart2_hex, sPart3_hex, sPart4_hex, sPart5_hex As String
        Dim iPart1_dec, iPart2_dec, iPart3_dec, iPart4_dec, iPart5_dec, iCRC As Integer

        Try
            sInitial = sCodeToHash.Substring(0, 30)

            'Obtengo 5 grupos de 6 caracteres HEX
            sPart1_hex = sInitial.Substring(0, 6)
            sPart2_hex = sInitial.Substring(6, 6)
            sPart3_hex = sInitial.Substring(12, 6)
            sPart4_hex = sInitial.Substring(18, 6)
            sPart5_hex = sInitial.Substring(24, 6)

            'Obtengo los 5 grupos de decimales
            iPart1_dec = Int32.Parse(sPart1_hex, System.Globalization.NumberStyles.HexNumber)
            iPart2_dec = Int32.Parse(sPart2_hex, System.Globalization.NumberStyles.HexNumber)
            iPart3_dec = Int32.Parse(sPart3_hex, System.Globalization.NumberStyles.HexNumber)
            iPart4_dec = Int32.Parse(sPart4_hex, System.Globalization.NumberStyles.HexNumber)
            iPart5_dec = Int32.Parse(sPart5_hex, System.Globalization.NumberStyles.HexNumber)

            'Cada bloque se reduce a un dígito, sumando los dígitos
            iPart1_dec = DigitSum(iPart1_dec)
            iPart2_dec = DigitSum(iPart2_dec)
            iPart3_dec = DigitSum(iPart3_dec)
            iPart4_dec = DigitSum(iPart4_dec)
            iPart5_dec = DigitSum(iPart5_dec)

            ' Por último obtengo CRC
            iCRC = DigitSum(iPart1_dec * 10000 + iPart2_dec * 1000 + iPart3_dec * 100 + iPart4_dec * 10 + iPart5_dec)

            Return iPart1_dec.ToString + iPart2_dec.ToString + iPart3_dec.ToString + iPart4_dec.ToString + iPart5_dec.ToString + iCRC.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Shared Function DigitSum(ByVal num As Integer) As Integer
        Dim sum As Integer = 0
        While num > 0
            sum += num Mod 10
            num \= 10
        End While
        If sum > 9 Then
            sum = DigitSum(sum)
        End If
        Return sum
    End Function

    ''' <summary>
    ''' Devuelve la una cadena con encriptación MD5
    ''' </summary>
    ''' <param name="strText"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EncryptMD5(ByVal strText As String) As String

        Dim MD5 As New System.Security.Cryptography.MD5CryptoServiceProvider()
        MD5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strText))
        Dim arrResult As Byte() = MD5.Hash()

        Dim strBuilder As New System.Text.StringBuilder()
        For i As Integer = 0 To arrResult.Length - 1
            strBuilder.Append(arrResult(i).ToString("x2"))
        Next i

        Return strBuilder.ToString()

    End Function

End Class

' <summary>
' Permite calcular valores Hash de cadenas y archivos.
' </summary>
Public NotInheritable Class HashCheckSum

    ' <summary>
    ' Calcula el valor hash para la cadena pasada como parámetro.
    ' </summary>
    ' <param name="cadena">Cadena a procesar.</param>
    ' <param name="alg">Algoritmo que vamos a utilizar.</param>
    ' <returns>Cadena representando el valor Hash Obtenido.</returns>
    Public Shared Function CalculateString(ByVal clearstring As String, ByVal alg As Algorithm) As String
        Dim hash As HashAlgorithm
        Dim tempsource As Byte()

        ' del algoritmo seleccionado obtenemos un proveedor de cifrado.
        hash = GetHashProvider(alg)
        ' obtenemos un array de bytes de los caracteres dentro de la cadena.
        tempsource = Encoding.ASCII.GetBytes(clearstring)

        ' el método ComputeHash calcula el valor Hash y devuelve un array de bytes.
        ' convertimos el array a string antes de devolverlo.
        Return ArrayToString(hash.ComputeHash(tempsource))
    End Function

    ' <summary>
    ' Calcula el valor Hash del archivo pasado como parámetro.
    ' </summary>
    ' <param name="filename">Nombre completo del archivo a procesar.</param>
    ' <param name="alg">Algortimo que vamos a utilizar.</param>
    ' <returns>Cadena representando el valor Hash del archivo.</returns>
    Private Shared Function CalculateHash(ByVal arrBytes() As Byte, ByVal alg As Algorithm) As String
        Dim hash As HashAlgorithm
        Dim resul As String

        ' del algoritmo seleccionado obtenemos un proveedor de cifrado.
        hash = GetHashProvider(alg)

        ' el método ComputeHash calcula el valor Hash del flujo
        ' que representa al archivo. convertimos el array a string y lo asignamos
        ' a una variable
        resul = ArrayToString(hash.ComputeHash(arrBytes))
        Return resul ' devolvemos el valor de la variable de cadena.
    End Function

    ' <summary>
    ' Calcula el valor Hash del archivo pasado como parámetro.
    ' </summary>
    ' <param name="filename">Nombre completo del archivo a procesar.</param>
    ' <param name="alg">Algortimo que vamos a utilizar.</param>
    ' <returns>Cadena representando el valor Hash del archivo.</returns>
    Private Shared Function CalculateFileHash(ByVal filename As String, ByVal alg As Algorithm) As String
        Dim hash As HashAlgorithm
        Dim fs As FileStream
        Dim resul As String

        ' del algoritmo seleccionado obtenemos un proveedor de cifrado.
        hash = GetHashProvider(alg)
        ' creamos un objeto stream con el archivo especificado.
        fs = New FileStream(filename, FileMode.Open, FileAccess.Read)

        ' el método ComputeHash calcula el valor Hash del flujo
        ' que representa al archivo. convertimos el array a string y lo asignamos
        ' a una variable
        resul = ArrayToString(hash.ComputeHash(fs))
        fs.Close() ' importante!! cerramos el flujo.

        Return resul ' devolvemos el valor de la variable de cadena.
    End Function

    ' <summary>
    ' Convierte un Array de bytes en una cadena de caracteres.
    ' </summary>
    ' <param name="byteArray">Byte array origen.</param>
    ' <returns>Cadena de caracteres obtenida del array.</returns>
    Private Shared Function ArrayToString(ByVal bytearray As Byte()) As String
        ' usamos un objeto stringbuilder por su mejor rendimiento con operaciones
        ' de cadenas.
        Dim sb As New StringBuilder(bytearray.Length)
        Dim i As Integer

        ' por cada byte en el array
        For i = 0 To bytearray.Length - 1
            ' obtenemos su valor hexadecimal y lo agregamos al objeto
            ' stringbuilder.
            sb.Append(bytearray(i).ToString("X2"))
        Next

        ' devolvemos el objeto stringbuilder, formateado a string
        Return sb.ToString()
    End Function

    ' <summary>
    ' Obtiene un proveedor HashAlgorithm de la enumeración utilizada.
    ' </summary>
    ' <param name="alg">Algoritmo seleccionado.</param>
    ' <returns>Proveedor Hash correpondiente al algoritmo deseado.</returns>
    Private Shared Function GetHashProvider(ByVal alg As Algorithm) As HashAlgorithm
        Select Case alg
            Case Algorithm.MD5
                Return New MD5CryptoServiceProvider()
            Case Algorithm.SHA1
                Return New SHA1CryptoServiceProvider()
            Case Algorithm.SHA256
                Return New SHA256Managed()
            Case Algorithm.SHA384
                Return New SHA384Managed()
            Case Algorithm.SHA512
                Return New SHA512Managed()
            Case Else ' sino se ha encontrado un valor correspondiente en la
                ' enumeración, generamos un excepción para indicarlo.
                Throw New Exception("¡¡Invalid Provider!!")
        End Select
    End Function

End Class