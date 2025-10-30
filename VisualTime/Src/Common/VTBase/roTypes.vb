Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports Robotics.Base.DTOs

Public Class roTypes

    Public Shared ReadOnly MinDate As Date = New Date(1900, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)

    Public Shared ReadOnly MaxDate As Date = New Date(2079, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)

    Public Shared Function UnspecifiedNow() As DateTime
        Return CreateDateTime(DateTimeKind.Unspecified)
    End Function
    Public Shared Function CreateDateTime(Optional ByVal dateKind As DateTimeKind = DateTimeKind.Unspecified) As DateTime
        Dim xNow As Date = DateAndTime.Now
        Return New DateTime(xNow.Year, xNow.Month, xNow.Day, xNow.Hour, xNow.Minute, xNow.Second, dateKind)
    End Function

    Public Shared Function CreateDateTime(ByVal year As Integer, ByVal month As Integer, ByVal day As Integer,
                                      Optional hour As Integer = 0, Optional minute As Integer = 0, Optional seconds As Integer = 0) As DateTime
        Return CreateDateTime(year, month, day, hour, minute, seconds, DateTimeKind.Unspecified)
    End Function
    Public Shared Function CreateDateTime(ByVal year As Integer, ByVal month As Integer, ByVal day As Integer,
                                      ByVal hour As Integer, ByVal minute As Integer, ByVal seconds As Integer, Optional ByVal dateKind As DateTimeKind = DateTimeKind.Unspecified) As DateTime
        Return New DateTime(year, month, day, hour, minute, seconds, dateKind)
    End Function

    ''' <summary>
    ''' Retorna el parámetro de entrada a un valor de tipo Double
    ''' </summary>
    ''' <param name="inputValue">Valor a covertir a double</param>
    ''' <remarks>Retorna 0 si se produce una excepción</remarks>
    Public Shared Function Any2Double(ByVal inputValue As Object) As Double
        Try
            Dim Result As Double = 0

            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return 0
            ElseIf Double.TryParse(inputValue, Result) Then
                Return Result
            ElseIf IsDate(inputValue) Then
                Result = CDbl(CDate(Format$(CDate(inputValue), "yyyy/MM/dd HH:mm:ss")).ToOADate)
                If Result < 0 Then Result = Fix(Result) - (1 - (-Result + Fix(Result)))
                Return Result
            Else
                Return 0
            End If
        Catch
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Retorna el parámetro de entrada a un valor de tipo Double teniendo en cuenta la cultura recibida
    ''' </summary>
    ''' <param name="inputValue">Valor a covertir a double</param>
    ''' <remarks>Retorna 0 si se produce una excepción</remarks>
    Public Shared Function Any2Double(ByVal inputValue As Object, ByRef oCultureInfo As Globalization.CultureInfo) As Double
        Try
            Dim Result As Double = 0

            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return 0
            ElseIf Double.TryParse(inputValue, System.Globalization.NumberStyles.Number, oCultureInfo.NumberFormat, Result) Then
                Return Result
            ElseIf IsDate(inputValue) Then
                Result = CDbl(CDate(Format$(CDate(inputValue), "yyyy/MM/dd HH:mm:ss")).ToOADate)
                If Result < 0 Then Result = Fix(Result) - (1 - (-Result + Fix(Result)))
                Return Result
            Else
                Return 0
            End If
        Catch
            Return 0
        End Try
    End Function

    Public Shared Function Any2Boolean(ByVal inputValue As Object) As Boolean
        Try
            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then Return False
            If inputValue.GetType() Is GetType(String) Then
                If inputValue.ToString.Trim.ToUpper = "FALSE" OrElse inputValue.ToString.Trim.ToUpper = "FALSO" OrElse inputValue.ToString.Trim.ToUpper = "0" Then Return False
                If inputValue.ToString.Trim.ToUpper = "TRUE" OrElse inputValue.ToString.Trim.ToUpper = "VERDADERO" OrElse inputValue.ToString.Trim.ToUpper = "1" Then Return True
            Else
                If inputValue = 0 Then Return False Else Return True
            End If

            Return False
        Catch
            Return False
        End Try
    End Function

    Public Shared Function Any2String(ByVal inputValue As Object) As String
        Try
            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then Return ""
            Return inputValue.ToString
        Catch
            Return ""
        End Try
    End Function

    Public Shared Function Any2String(input As String, maxSizeInKB As Integer) As String
        If String.IsNullOrEmpty(input) OrElse IsDBNull(input) Then Return String.Empty

        Dim maxSizeInBytes As Integer = maxSizeInKB * 1024

        ' Convert the string to a byte array
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(input)

        ' Check if the byte array is larger than the specified size
        If bytes.Length > maxSizeInBytes Then
            ' Truncate the byte array to the specified size
            Dim truncatedBytes(maxSizeInBytes - 1) As Byte
            Array.Copy(bytes, truncatedBytes, maxSizeInBytes)

            ' Convert the truncated byte array back to a string
            Return $"{Encoding.UTF8.GetString(truncatedBytes)}..."
        Else
            ' If the byte array is already within the specified size, return the original string
            Return input
        End If
    End Function


    Public Shared Function Double2Sql(ByVal inputValue As Double) As String
        Try
            Return Any2String(inputValue).Replace(",", ".")
        Catch
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Retorna el parámetro de entrada a un valor de tipo Long
    ''' </summary>
    ''' <param name="inputValue">Valor a covertir a double</param>
    ''' <remarks>Retorna 0 si se produce una excepción</remarks>
    Public Shared Function Any2Long(ByVal inputValue As Object) As Long
        Try
            Dim Result As Long = 0

            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return 0
            ElseIf Long.TryParse(inputValue, Result) Then
                Return Result
            ElseIf IsDate(inputValue) Then
                Return (Second(inputValue) / 3600) + (Minute(inputValue) / 60) + Hour(inputValue) + DateDiff("d", "00:00", inputValue) * 24
            Else
                Return 0
            End If
        Catch
            Return 0
        End Try
    End Function

    Public Shared Function Any2Integer(ByVal inputValue As Object) As Integer
        Dim result As Integer = 0
        Try
            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return 0
            ElseIf Integer.TryParse(inputValue, result) Then
                Return result
            Else
                Return 0
            End If
        Catch
            Return 0
        End Try
    End Function

    Public Shared Function Any2IntegerArray(ByVal inputValue As Object) As Integer()
        Dim result As Integer() = New Integer() {}
        Try
            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return result
            Else
                Dim stringArray = inputValue.ToString().Split(",")
                Dim intList = stringArray.ToList().ConvertAll(Function(str) Int32.Parse(str))
                result = intList.ToArray()
                Return result
            End If
        Catch
            Return result
        End Try
    End Function

    Public Shared Function IntegerArray2String(ByVal inputValue As Integer()) As String
        Dim result As String = ""
        Try
            If inputValue Is Nothing OrElse IsDBNull(inputValue) Then
                Return result
            Else
                Dim strList = inputValue.ToList().ConvertAll(Function(str) str.ToString())
                result = String.Join(",", strList)
                Return result
            End If
        Catch
            Return result
        End Try
    End Function

    ' Convierte cualquier dato a fecha/hora
    '   - Si es un integer supone que el numero indicado son minutos absolutos.
    '   - Si es un doble: parte entera=horas, parte decimal=decimas de hora.
    '   - Si es un string: intenta interpretarlo como "hh:mm","h:mm"...(hh puede ser >23).
    Public Shared Function Any2Time(ByVal inputValue As Object, Optional ByVal engineData As Boolean = False) As roTime
        Dim vResult As roTime = Nothing

        ' Procesa valor
        If TypeOf inputValue Is roTime Then
            ' Ya es un roTime
            vResult = inputValue
        Else
            Select Case VarType(inputValue)
                Case vbDate
                    ' Ya es una fecha, devuelve tal cual
                    vResult = New roTime
                    vResult.Value = inputValue

                Case vbEmpty, vbNull
                    ' Datos vacios, devuelve tiempo vacio
                    vResult = New roTime
                    If Not engineData Then
                        vResult.Value = "00:00"
                    Else
                        vResult.Value = "1899/12/30"
                    End If

                Case vbLong, vbSingle, vbDouble, vbDecimal, vbByte, vbInteger
                    vResult = ConvertNumberToTime(inputValue, engineData)

                Case vbString
                    vResult = ConvertStringToTime(inputValue, engineData)

                Case Else
                    ' Error, tipo no soportado
                    Err.Raise(3455, "roSupport", "Any2Time: inputValue can't be converted to roTime.")
            End Select
        End If

        Return vResult

    End Function

    Private Shared Function ConvertStringToTime(ByRef inputValue As Object, engineData As Boolean) As roTime
        Dim vResult As New roTime

        ' Procesa un string
        If IsDate(inputValue) Then
            ' Si el string ya es una fecha, ya esta hecho
            If engineData AndAlso CDate(inputValue).Year = 1 Then inputValue = "1899/12/30 " & inputValue

            vResult.Value = inputValue
            Return vResult
        End If

        ' Es un string con un numero, pasa a horas
        If IsNumeric(inputValue) Then Return Any2Time(Any2Double(inputValue))

        Dim tmpDate As DateTime = roTypes.UnspecifiedNow()

        Dim formatos As String() = {"dd-MM-yyyy", "dd/MM/yyyy", "dd-MM-yyyy HH:mm", "dd-MM-yyyy HH:mm:ss", "dd-MM-yyyy HH:mm:ss.fff", "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss.fff",
                                    "yyyy-MM-dd", "yyyy/MM/dd", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss.fff", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff",
                                    "MM/dd/yyyy", "MM/dd/yyyy HH:mm", "MM/dd/yyyy HH:mm:ss", "MM/dd/yyyy HH:mm:ss.fff"} ' Formatos aceptados
        Dim cultura As CultureInfo = CultureInfo.InvariantCulture ' Cultura invariable para evitar problemas regionales

        If Date.TryParseExact(inputValue, formatos, cultura, DateTimeStyles.None, tmpDate) Then
            vResult.Value = tmpDate
            Return vResult
        End If

        ' Si no es una fecha directamente, procesa string
        If StringItemsCount(inputValue, ":") > 1 Then
            If Not engineData Then
                ' Hay separadores de horas
                vResult = Any2Time(DateAdd("h", Val(String2Item(inputValue, 0, ":")), DateAdd("n", Val(String2Item(inputValue, 1, ":")), "00:00")))
            Else
                vResult = Any2Time(DateAdd("h", Val(String2Item(inputValue, 0, ":")), DateAdd("n", Val(String2Item(inputValue, 1, ":")), "1899/12/30")))
            End If
        End If

        Return vResult
    End Function

    Private Shared Function ConvertNumberToTime(ByRef inputValue As Object, engineData As Boolean) As roTime
        Dim vResult As New roTime

        Dim Hours As Double
        Dim Minutes As Integer
        Dim Seconds As Integer

        Dim vAux As Object


        ' Pasa a positivo
        vAux = System.Math.Abs(inputValue)

        ' Obtiene horas, minutos y segundos
        Hours = Fix(vAux)
        vAux = (vAux - Hours) * 60
        Minutes = Fix(vAux)
        vAux = (vAux - Minutes) * 60
        Seconds = Fix(vAux)

        ' Redondea (evita errores debidos a tener solo 6 digitos de precision)
        If (vAux - Seconds) * 100 > 50 Then Seconds = Seconds + 1

        ' Crea resultado
        If Not engineData Then
            vResult.Value = DateAdd("h", System.Math.Sign(inputValue) * Hours, "00:00")
        Else
            vResult.Value = DateAdd("h", System.Math.Sign(inputValue) * Hours, "1899/12/30")
        End If
        vResult = vResult.Add(System.Math.Sign(inputValue) * Minutes, "n")
        vResult = vResult.Add(System.Math.Sign(inputValue) * Seconds, "s")

        Return vResult
    End Function

    ' Devuelve un datetime de cualquier cosa
    Public Shared Function Any2DateTime(ByVal inputValue As Object) As DateTime
        Dim dtime As roTime = Any2Time(inputValue)
        Return dtime.ValueDateTime
    End Function

    ' Devuelve solo la fecha, sin horas minutos ni segundos
    Public Shared Function DateTime2Date(ByVal inputValue As Date) As Date
        Return roTypes.CreateDateTime(inputValue.Year, inputValue.Month, inputValue.Day)
    End Function

    ' Devuelve la hora,minutos y segundos, sin fecha
    Public Shared Function DateTime2Time(ByVal inputValue As Date) As Date
        Return TimeSerial(Hour(inputValue), Minute(inputValue), Second(inputValue))
    End Function

    ' Determina el numero de elementos en una lista separados por comas
    Public Shared Function StringItemsCount(ByVal itemsString As String, Optional ByVal separator As String = ",") As Integer
        Dim Count As Integer

        While (InStr(itemsString, separator) > 0)
            itemsString = Mid$(itemsString, InStr(itemsString, separator) + Len(separator), 9999)
            Count = Count + 1
        End While
        If Trim$(itemsString) <> "" Then Count = Count + 1 ' Elemento del final

        Return Count
    End Function

    ' Devuelve un string de una lista de elementos separados por el separador indicado.
    Public Shared Function String2Item(ByVal itemsString As String, ByVal index As Integer, Optional ByVal separator As String = ",") As String
        ' Si el indice no es valido, devuelve nada
        If index < 0 OrElse index > StringItemsCount(itemsString, separator) - 1 Then
            Return ""
        End If
        While (index > 0)
            itemsString = Mid$(itemsString, InStr(itemsString, separator) + Len(separator), 9999)
            index = index - 1
        End While
        If InStr(itemsString, separator) > 0 Then itemsString = Mid$(itemsString, 1, InStr(itemsString, separator) - 1)
        Return Trim$(itemsString)
    End Function

    Public Function Bytes2StrByte(ByVal inputValue As Byte(), Optional ByVal space As Boolean = True) As String
        Dim tmp As String = ""
        Try
            If inputValue IsNot Nothing Then
                For Each bin As Byte In inputValue
                    tmp &= System.Convert.ToString(roTypes.Any2Long(bin), 16).PadLeft(2, "0") & IIf(space, " ", "")
                Next
            End If
        Catch
            Return "err"
        End Try

        Return tmp
    End Function

    Public Shared Function Image2Bytes(ByVal oImage As System.Drawing.Image) As Byte()
        Dim bRet(-1) As Byte

        If oImage IsNot Nothing Then

            Dim strm As System.IO.MemoryStream = Nothing
            Try
                strm = New System.IO.MemoryStream()
                Dim oBitmap As New System.Drawing.Bitmap(oImage)
                oBitmap.Save(strm, System.Drawing.Imaging.ImageFormat.Jpeg)
                bRet = strm.GetBuffer()
            Catch
                'do nothing
            Finally
                If strm IsNot Nothing Then strm.Close()
            End Try

        End If

        Return bRet
    End Function

    Public Shared Function Bytes2Image(ByVal bBytes As Byte()) As System.Drawing.Image
        Dim oImage As System.Drawing.Image = Nothing

        If bBytes IsNot Nothing AndAlso bBytes.Length > 0 Then
            Dim strm As System.IO.MemoryStream = Nothing
            Try
                strm = New System.IO.MemoryStream(bBytes, 0, bBytes.Length)
                oImage = New System.Drawing.Bitmap(strm)
            Catch ex As Exception
                Return Nothing
            Finally
                If strm IsNot Nothing Then
                    strm.Close()
                    strm.Dispose()
                End If
            End Try
        End If

        Return oImage
    End Function

    ' Suma dos tiempos
    Public Shared Function DateTimeAdd(ByVal sourceValue As Date, ByVal addValue As Date, Optional ByVal precission As String = "n") As Date
        Dim mDateDiff As Long = DateDiff(precission, "00:00", addValue)
        If precission = "n" Then
            If addValue.Date = roTypes.CreateDateTime(1899, 12, 30) Then
                Return sourceValue.AddMinutes(addValue.Minute).AddHours(addValue.Hour)
            End If
            If addValue.Date = roTypes.CreateDateTime(1899, 12, 29) Then
                Return sourceValue.AddDays(-1).AddMinutes(addValue.Minute).AddHours(addValue.Hour)
            End If
            If addValue.Date = roTypes.CreateDateTime(1899, 12, 31) Then
                Return sourceValue.AddDays(1).AddMinutes(addValue.Minute).AddHours(addValue.Hour)
            End If

            If sourceValue.Date = roTypes.CreateDateTime(1899, 12, 30) Then
                Return addValue.AddMinutes(sourceValue.Minute).AddHours(sourceValue.Hour)
            End If

            If sourceValue.Date = roTypes.CreateDateTime(1899, 12, 29) Then
                Return addValue.AddDays(-1).AddMinutes(sourceValue.Minute).AddHours(sourceValue.Hour)
            End If

            If sourceValue.Date = roTypes.CreateDateTime(1899, 12, 31) Then
                Return addValue.AddDays(1).AddMinutes(sourceValue.Minute).AddHours(sourceValue.Hour)
            End If
        End If
        Return DateAdd(precission, mDateDiff, sourceValue)
    End Function

    ' Resta dos tiempos
    Public Shared Function DateTimeSubstract(ByVal subjectValue As Date, ByVal substractValue As Date, Optional ByVal precission As String = "n") As Date
        Return DateAdd(precission, -DateDiff(precission, "00:00", substractValue), subjectValue)
    End Function

    ' Devuelve un string con el DateTime del valor listo para usar en una sentencia SQL.
    Public Shared Function SQLDateTime(ByVal xDate As DateTime) As String
        Dim strRet As String = ""
        strRet = "CONVERT(DATETIME,'" & Format(xDate, "yyyy/MM/dd") & " " & Format$(xDate.Hour, "00") & ":" & Format$(xDate.Minute, "00") & ":" & Format$(xDate.Second, "00") & " ',120)"
        Return strRet
    End Function

    ' Devuelve un string con el DateTime del valor listo para usar en una sentencia SQL.
    Public Shared Function SQLSmallDateTime(ByVal xDate As DateTime) As String
        Dim strRet As String = ""
        strRet = "CONVERT(SMALLDATETIME,'" & Format(xDate, "yyyy/MM/dd") & " " & Format$(xDate.Hour, "00") & ":" & Format$(xDate.Minute, "00") & " ',120)"
        Return strRet
    End Function

    Public Function SQLiteDateTime(ByVal xDate As DateTime) As String
        Dim strRet As String = ""
        strRet = xDate.ToString("yyyy") & "-" & xDate.ToString("MM") & "-" & xDate.ToString("dd")
        strRet &= " " & xDate.ToString("HH") & ":" & xDate.ToString("mm") & ":" & xDate.ToString("ss")
        Return strRet
    End Function

    ' Devuelve un tiempo en horas, centesimas de hora...
    Public Shared Function DateTime2Double(ByVal inputValue As Date, Optional ByVal engineData As Boolean = False) As Double
        Dim mDateDiff As Long = 0
        If engineData Then
            Return Any2Time(inputValue).NumericValue(True)
        Else
            mDateDiff = DateDiff("d", "00:00", inputValue)
        End If
        Return (inputValue.TimeOfDay.TotalSeconds / 3600) + (inputValue.TimeOfDay.TotalMinutes / 60) + inputValue.TimeOfDay.TotalHours + mDateDiff * 24
    End Function


    Public Shared Function ReplaceFirst(ByVal text As String, ByVal search As String, ByVal replace As String) As String
        Dim pos As Integer = text.IndexOf(search)

        If pos < 0 Then
            Return text
        End If

        Return text.Substring(0, pos) & replace & text.Substring(pos + search.Length)
    End Function



#Region "Serialize structure to file"

    Public Shared Sub Any2File(Of T)(ByVal list As IList(Of T), ByVal filename As String)
        Using stream As MemoryStream = New System.IO.MemoryStream()

            Dim bin As BinaryFormatter = New BinaryFormatter()
            For Each item As T In list
                bin.Serialize(stream, item)
            Next

            stream.Seek(0, SeekOrigin.Begin)

            Using tmpFStream As New FileStream(filename, FileMode.Create)
                Using cFile As GZipStream = New GZipStream(tmpFStream, CompressionMode.Compress, True)
                    ' Copy the source file into the compression stream.
                    stream.CopyTo(cFile)
                End Using
                tmpFStream.Seek(0, SeekOrigin.Begin)

                tmpFStream.Close()
            End Using
        End Using
    End Sub

    Public Shared Sub Any2Genius(ByVal list As IEnumerable(Of Analytics_Base), ByVal filename As String, ByVal userFieldNames As String(), ByVal oLng As roLanguage, oSchema As Analytics_Base)
        Dim oTranlateHash As Hashtable = Nothing

        If oLng IsNot Nothing Then
            oTranlateHash = New Hashtable()

            Dim actualType As Type = oSchema.GetType()
            For Each oProperty In actualType.GetProperties
                oTranlateHash.Add(oProperty.Name, oLng.Translate("columnname." & actualType.Name & "." & oProperty.Name & ".rotext", ""))

                If oProperty.Name.StartsWith("UserField") Then
                    Dim index = roTypes.Any2Integer(oProperty.Name.Replace("UserField", ""))

                    If index - 1 < userFieldNames.Length Then
                        oTranlateHash(oProperty.Name) = userFieldNames(index - 1)
                    End If

                End If
            Next
        End If

        Using stream As StreamWriter = New StreamWriter(File.Open(filename, FileMode.OpenOrCreate))
            stream.WriteLine("[")

            stream.WriteLine(roJSONHelper.ToGeniusJSONDefinitionString(oSchema, oTranlateHash))
            If list.Count > 0 Then stream.WriteLine(",")

            For i = 0 To list.Count - 1
                If i < list.Count - 1 Then
                    stream.WriteLine(roJSONHelper.ToGeniusJSONString(list(i), oTranlateHash, userFieldNames) & ",")
                Else
                    stream.WriteLine(roJSONHelper.ToGeniusJSONString(list(i), oTranlateHash, userFieldNames))
                End If
            Next

            stream.WriteLine("]")

            stream.Flush()
            stream.Close()
        End Using
    End Sub


    Public Shared Function File2Any(Of T)(ByVal fileContent As Byte()) As Generic.List(Of T)
        Dim deserializaedList As New Generic.List(Of T)

        Try
            Dim fStream As New MemoryStream(fileContent)
            fStream.Seek(0, SeekOrigin.Begin)
            Using Decompress As GZipStream = New GZipStream(fStream, CompressionMode.Decompress)

                Try
                    Dim fileName = Path.GetTempFileName()
                    If File.Exists(fileName) Then File.Delete(fileName)

                    Dim decompressStream As New FileStream(fileName, FileMode.OpenOrCreate)
                    Decompress.CopyTo(decompressStream)
                    decompressStream.Seek(0, SeekOrigin.Begin)
                    Dim bin As BinaryFormatter = New BinaryFormatter()
                    While decompressStream.Position < decompressStream.Length
                        deserializaedList.Add(CType(bin.Deserialize(decompressStream), T))
                    End While

                    decompressStream.Close()
                    If File.Exists(fileName) Then File.Delete(fileName)
                Catch ex As Exception
                    deserializaedList = Nothing
                End Try

            End Using
        Catch ex As Exception
            deserializaedList = Nothing
        End Try
        Return deserializaedList
    End Function

    Public Shared Function ToBytes(ByVal value As String, ByVal encoding As Encoding) As Byte()
        Using stream = New MemoryStream()

            Using sw = New StreamWriter(stream, encoding)
                sw.Write(value)
                sw.Flush()
                Return stream.ToArray()
            End Using
        End Using
    End Function

#End Region

End Class