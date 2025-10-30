Imports System.Math

Public Class roConversions

    ' Convierte cualquier variant a un string
    Public Shared Function Any2String(ByVal Value As Object) As String
        Select Case VarType(Value)
            Case vbEmpty, vbNull ' Null
                Any2String = ""
            Case vbBoolean
                If Value Then Any2String = "True" Else Any2String = "False"
            Case Else

                Try
                    Any2String = CStr(Value)
                Catch
                    Any2String = ""
                End Try

        End Select
    End Function

    ' Determina el numero de elementos en una lista separados por comas
    Public Shared Function StringItemsCount(ByVal ItemsString As String, Optional ByVal Separator As String = ",") As Integer
        Dim Count As Integer
        While (InStr(ItemsString, Separator) > 0)
            ItemsString = Mid$(ItemsString, InStr(ItemsString, Separator) + Len(Separator), 9999)
            Count = Count + 1
        End While
        If Trim$(ItemsString) <> "" Then Count = Count + 1 ' Elemento del final
        StringItemsCount = Count
    End Function

    ' Devuelve un string de una lista de elementos separados por el separador indicado.
    Public Shared Function String2Item(ByVal ItemsString As String, ByVal Index As Integer, Optional ByVal Separator As String = ",") As String
        ' Si el indice no es valido, devuelve nada
        If Index < 0 Or Index > StringItemsCount(ItemsString, Separator) - 1 Then
            String2Item = ""
            Exit Function
        End If
        While (Index > 0)
            ItemsString = Mid$(ItemsString, InStr(ItemsString, Separator) + Len(Separator), 9999)
            Index = Index - 1
        End While
        If InStr(ItemsString, Separator) > 0 Then ItemsString = Mid$(ItemsString, 1, InStr(ItemsString, Separator) - 1)
        String2Item = Trim$(ItemsString)
    End Function

    ' Devuelve el resto de items de una lista de elementos separados por el separador indicado.
    Public Shared Function String2RestItems(ByVal ItemsString As String, ByVal Index As Integer, Optional ByVal Separator As String = ",") As String
        ' Si el indice no es valido, devuelve todo
        If Index < 0 Or Index > StringItemsCount(ItemsString, Separator) - 1 Then
            String2RestItems = ItemsString
            Exit Function
        End If
        While (Index >= 0)
            ItemsString = Mid$(ItemsString, InStr(ItemsString, Separator) + Len(Separator), 9999)
            Index = Index - 1
        End While
        String2RestItems = Trim$(ItemsString)
    End Function

    ''' <summary>
    ''' Retorna el parámetro de entrada a un valor de tipo Double
    ''' </summary>
    ''' <param name="Value">Valor a covertir a double</param>
    ''' <remarks>Retorna 0 si se produce una excepción</remarks>
    Public Shared Function Any2Double(ByVal Value As Object) As Double
        Try
            Dim Result As Double = 0

            If Value Is Nothing Or IsDBNull(Value) Then
                Return 0
            ElseIf Double.TryParse(Value, Result) Then
                Return Result
            ElseIf IsDate(Value) Then
                Result = CDbl(CDate(Format$(CDate(Value), "yyyy/MM/dd HH:mm:ss")).ToOADate)
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
    ''' <param name="Value">Valor a covertir a double</param>
    ''' <remarks>Retorna 0 si se produce una excepción</remarks>
    Public Shared Function Any2Double(ByVal Value As Object, ByRef oCultureInfo As Globalization.CultureInfo) As Double
        Try
            Dim Result As Double = 0

            If Value Is Nothing Or IsDBNull(Value) Then
                Return 0
            ElseIf Double.TryParse(Value, System.Globalization.NumberStyles.Number, oCultureInfo.NumberFormat, Result) Then
                Return Result
            ElseIf IsDate(Value) Then
                Result = CDbl(CDate(Format$(CDate(Value), "yyyy/MM/dd HH:mm:ss")).ToOADate)
                If Result < 0 Then Result = Fix(Result) - (1 - (-Result + Fix(Result)))
                Return Result
            Else
                Return 0
            End If
        Catch
            Return 0
        End Try
    End Function

    ' Convierte cualquier dato a fecha/hora
    '   - Si es un integer supone que el numero indicado son minutos absolutos.
    '   - Si es un doble: parte entera=horas, parte decimal=decimas de hora.
    '   - Si es un string: intenta interpretarlo como "hh:mm","h:mm"...(hh puede ser >23).
    Public Shared Function Any2Time(ByVal Value As Object) As roTime
        Dim vResult As New roTime

        Dim Hours As Double
        Dim Minutes As Integer
        Dim Seconds As Integer

        Dim vAux As Object

        ' Procesa valor
        If TypeOf Value Is roTime Then
            ' Ya es un roTime
            Any2Time = Value
            Exit Function
        Else
            Select Case VarType(Value)
                Case vbDate
                    ' Ya es una fecha, devuelve tal cual
                    vResult.Value = Value
                    Any2Time = vResult

                Case vbEmpty, vbNull
                    ' Datos vacios, devuelve tiempo vacio
                    vResult.Value = "00:00"
                    Any2Time = vResult

                Case vbLong, vbSingle, vbDouble, vbDecimal, vbByte, vbInteger
                    ' Pasa a positivo
                    vAux = Math.Abs(Value)

                    ' Obtiene horas, minutos y segundos
                    Hours = Fix(vAux)
                    vAux = (vAux - Hours) * 60
                    Minutes = Fix(vAux)
                    vAux = (vAux - Minutes) * 60
                    Seconds = Fix(vAux)

                    ' Redondea (evita errores debidos a tener solo 6 digitos de precision)
                    If (vAux - Seconds) * 100 > 50 Then Seconds = Seconds + 1

                    ' Crea resultado
                    vResult.Value = DateAdd("h", Math.Sign(Value) * Hours, "00:00")
                    vResult = vResult.Add(Math.Sign(Value) * Minutes, "n")
                    vResult = vResult.Add(Math.Sign(Value) * Seconds, "s")
                    Any2Time = vResult

                Case vbString
                    ' Procesa un string
                    If IsDate(Value) Then
                        ' Si el string ya es una fecha, ya esta hecho
                        vResult.Value = Value
                        Any2Time = vResult

                    ElseIf IsNumeric(Value) Then
                        ' Es un string con un numero, pasa a horas
                        Any2Time = Any2Time(Any2Double(Value))
                    Else
                        ' Si no es una fecha directamente, procesa string
                        If StringItemsCount(Value, ":") > 1 Then
                            ' Hay separadores de horas
                            Any2Time = Any2Time(DateAdd("h", Val(String2Item(Value, 0, ":")), DateAdd("n", Val(String2Item(Value, 1, ":")), "00:00")))
                        Else
                            Any2Time = Nothing
                        End If
                    End If

                Case Else
                    ' Error, tipo no soportado
                    Any2Time = Nothing
                    Err.Raise(3455, "roSupport", "Any2Time: Value can't be converted to roTime.")
            End Select
        End If

    End Function

    Public Shared Function VarTypeVB62VBNET(ByVal intType As Integer) As Integer
        Dim intRet As Integer

        Select Case intType
            Case 0 : intRet = vbEmpty
            Case 1 : intRet = vbNull
            Case 2 : intRet = vbInteger
            Case 3 : intRet = vbLong
            Case 4 : intRet = vbSingle
            Case 5 : intRet = vbDouble
            Case 6 : intRet = vbCurrency
            Case 7 : intRet = vbDate
            Case 8 : intRet = vbString
            Case 12 : intRet = vbVariant
            Case 11 : intRet = vbBoolean
            Case 14 : intRet = vbDecimal
            Case 17 : intRet = vbByte
        End Select

        Return intRet
    End Function

    Public Shared Function VarTypeVBNET2VB6(ByVal intType) As Integer
        Dim intRet As Integer

        Select Case intType
            Case vbEmpty : intRet = 0
            Case vbNull : intRet = 1
            Case vbInteger : intRet = 2
            Case vbLong : intRet = 3
            Case vbSingle : intRet = 4
            Case vbDouble : intRet = 5
            Case vbCurrency : intRet = 6
            Case vbDate : intRet = 7
            Case vbString : intRet = 8
            Case vbVariant : intRet = 12
            Case vbBoolean : intRet = 11
            Case vbDecimal : intRet = 14
            Case vbByte : intRet = 17
        End Select

        Return intRet
    End Function

    Public Shared Function XmlSerialize(ByVal oObject As Object, ByVal oType As System.Type) As String
        Dim oSer As New Xml.Serialization.XmlSerializer(oType)
        Dim strm As New System.IO.MemoryStream
        Dim bMessage() As Byte
        Dim strMessage As String = ""
        oSer.Serialize(strm, oObject)
        If strm.CanRead Then
            ReDim bMessage(strm.Length - 1)
            strm.Position = 0
            strm.Read(bMessage, 0, strm.Length)
            strm.Close()
            strMessage = System.Text.Encoding.UTF8.GetString(bMessage, 0, bMessage.Length)
        End If
        strm.Close()

        Return strMessage
    End Function

    Public Shared Function XmlDeserialize(ByVal strXml As String, ByVal oType As System.Type) As Object
        Dim oRet As Object = Nothing
        Dim strm As System.IO.MemoryStream = Nothing

        Try

            strm = New System.IO.MemoryStream()

            Dim bData() As Byte
            ReDim bData(System.Text.Encoding.UTF8.GetByteCount(strXml) - 1)
            bData = System.Text.Encoding.UTF8.GetBytes(strXml)

            strm.Write(bData, 0, bData.Length)
            strm.Position = 0

            Dim oSer As New Xml.Serialization.XmlSerializer(oType)
            oRet = oSer.Deserialize(strm)
        Catch
        Finally
            If strm IsNot Nothing Then strm.Close()
        End Try

        Return oRet
    End Function

    Public Shared Function ConvertMinutesToTime(ByVal Minutes As Integer) As String
        Try

            Dim strResult As String = "00:00"
            If Minutes = 0 Then Return strResult

            Dim HasSign As Boolean = Minutes < 0
            Minutes = System.Math.Abs(Minutes)

            Dim hour As Double = Math.Floor(Minutes / 60)

            Dim RestMinutes As Double = Minutes Mod 60

            strResult = Format(hour, "00") & ":" & Format(RestMinutes, "00")

            If HasSign Then
                Return "-" & strResult
            Else
                Return strResult
            End If
        Catch
            Return "00:00"
        End Try

    End Function

    Public Shared Function ConvertHoursToTime(ByVal Hours As Double) As String
        Try

            Dim xDate As Date = Now.Date
            Dim strResult As String = "00:00"
            If Hours = 0 Then Return strResult

            Dim Sign As String = ""
            If Hours < 0 Then Sign = "-"

            xDate = Any2Time(System.Math.Abs(Hours)).Value
            Hours = Any2Time(xDate).NumericValue

            strResult = Sign & Format$(Any2Time(Abs(Hours)).Hours, "00") & ":" & Format$((Any2Time(Abs(Hours)).Minutes Mod 60), "00")

            Return strResult
        Catch
            Return "00:00"
        End Try
    End Function

    Public Shared Function ConvertTimeToMinutes(ByVal Time As String) As Integer
        Try

            Dim iResult As Integer = 0
            Dim Sign As String = ""

            If Time.StartsWith("-") Then
                Sign = "-"
                Time = Time.Substring(1)
            End If

            Dim dHour As Integer = CInt(Time.Split(":")(0))
            Dim dMinute As Double = CInt(Time.Split(":")(1))

            iResult = dHour * 60
            iResult += dMinute

            If Sign = "-" Then
                Return iResult * (-1)
            Else
                Return iResult
            End If
        Catch
            Return 0
        End Try
    End Function

    Public Shared Function ConvertTimeToHours(ByVal Time As String) As Double
        Try

            Dim iResult As Double = 0
            Dim Sign As String = ""

            If Time.StartsWith("-") Then
                Sign = "-"
                Time = Time.Substring(1)
            End If

            Dim dHour As Integer = CInt(Time.Split(":")(0))
            Dim dMinute As Double = CInt(Time.Split(":")(1))

            iResult = dHour * 60
            iResult += dMinute

            If Sign = "-" Then
                Return (iResult / 60) * -1
            Else
                Return iResult / 60
            End If
        Catch
            Return 0
        End Try
    End Function

    Public Shared Function GetDecimalDigitFormat() As String
        Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
        Return oInfo.NumberDecimalSeparator
    End Function

    Public Shared Function Max(ByVal ValueA As Object, ByVal ValueB As Object) As Object
        '
        ' Devuelve el mayor de dos valores
        '
        If TypeOf ValueA Is roTime And TypeOf ValueB Is roTime Then
            ' Compara tiempos
            If ValueA.VBNumericValue >= ValueB.VBNumericValue Then Max = ValueA Else Max = ValueB
        ElseIf IsDate(ValueA) And IsDate(ValueB) Then
            Max = DateTimeMax(ValueA, ValueB)
        Else
            ' Compara cualquier cosa o numericos
            If ValueA > ValueB Then Max = ValueA Else Max = ValueB
        End If

    End Function

    Public Shared Function Min(ByVal ValueA As Object, ByVal ValueB As Object) As Object
        '
        ' Devuelve el menor de dos valores
        '
        If TypeOf ValueA Is roTime And TypeOf ValueB Is roTime Then
            ' Compara tiempos
            If ValueA.VBNumericValue < ValueB.VBNumericValue Then Min = ValueA Else Min = ValueB
        ElseIf IsDate(ValueA) And IsDate(ValueB) Then
            Min = DateTimeMin(ValueA, ValueB)
        Else
            ' Compara cualquier otra cosa
            If ValueA < ValueB Then Min = ValueA Else Min = ValueB
        End If

    End Function

    Public Shared Function DateTimeMax(ByVal ValueA As Date, ByVal ValueB As Date) As Date
        '
        ' Devuelve el mayor de dos tiempos
        '
        DateTimeMax = IIf(Any2Double(ValueA) >= Any2Double(ValueB), ValueA, ValueB)

    End Function

    Public Shared Function DateTimeMin(ByVal ValueA As Date, ByVal ValueB As Date) As Date
        '
        ' Devuelve el mayor de dos tiempos
        '
        DateTimeMin = IIf(Any2Double(ValueA) <= Any2Double(ValueB), ValueA, ValueB)

    End Function

End Class