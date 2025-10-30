Imports System.Drawing
Public Class clsGlobal

    Public Shared oLog As New Robotics.Comms.Base.roLogComms("mxDataBase")
    'Public Shared oDebug As New clsDebug
    Public Shared Clossing As Boolean
    Private Shared _UTC As Integer = 0
    Public Shared InitCamera As Boolean
    Public Shared NeedReconnect As Boolean = False
    Private Shared mShowSystemControl As Boolean = False
    'Private Shared mShowDebugControl As Boolean
    Public Shared mLastShowSystemControl As DateTime = Now
    Public Shared HasFingers As Boolean = False


    Public Const NULLDATE = "1900-01-01 00:00:00"
    Public Shared NULLDATETIME As Date = New DateTime(1900, 1, 1)

    'Public Shared ReadOnly Property ShowDebugControl() As Boolean
    '    Get
    '        Return mShowDebugControl
    '    End Get
    'End Property

    Public Shared Property ShowSystemControl() As Boolean
        Get
            Return mShowSystemControl
        End Get
        Set(ByVal value As Boolean)
            mShowSystemControl = value
            'If value Then
            '    If Now.Subtract(mLastShowSystemControl).TotalSeconds < 10 Then
            '        mShowDebugControl = True
            '    End If
            '    mLastShowSystemControl = Now
            'Else
            '    mShowDebugControl = False
            'End If

        End Set
    End Property

    ''' <summary>
    ''' Convierte un color en texto a tipo color
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RGB2Color(ByVal Value As String) As Color
        Try
            'Si ponemos solo un 0 es que no hay color
            If Value = "0" Then Return Color.Empty

            Dim iRed As Byte
            Dim iGreen As Byte
            Dim iBlue As Byte
            If Value.IndexOf(".") >= 0 Then
                iRed = clsGlobal.Any2Long(Value.Split(".")(0))
                iGreen = clsGlobal.Any2Long(Value.Split(".")(1))
                iBlue = clsGlobal.Any2Long(Value.Split(".")(2))
            ElseIf Value.Length = 6 Then
                iRed = Convert.ToInt16(Value.Substring(0, 2), 16)
                iGreen = Convert.ToInt16(Value.Substring(2, 2), 16)
                iBlue = Convert.ToInt16(Value.Substring(4, 2), 16)
            End If
            Return Color.FromArgb(iRed, iGreen, iBlue)
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            Return Color.Black
        End Try
    End Function

    ''' <summary>
    ''' Copia un array dentro de otro
    ''' </summary>
    ''' <param name="Values"></param>
    ''' <param name="Position"></param>
    ''' <param name="Lenght"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ArrayCopy(ByVal Values As Byte(), ByVal Position As Integer, Optional ByVal Lenght As Integer = 0) As Byte()
        Dim i As Integer
        Dim bytes As Byte()
        Try
            If Lenght = 0 Then
                Lenght = Values.Length - Position
            End If

            bytes = Array.CreateInstance(GetType(Byte), Lenght)

            For i = 0 To Lenght - 1
                bytes(i) = Values(Position + i)
            Next
            Return bytes
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::ArrayCopy:" + Bytes2StrByte(Values) + ";" + Position.ToString + ";" + Lenght.ToString, ex)
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try
    End Function

    ''' <summary>
    ''' Compara dos array para saber si son iguales
    ''' </summary>
    ''' <param name="Value1"></param>
    ''' <param name="Value2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ArrayCompare(ByVal Value1 As Byte(), ByVal Value2 As Byte()) As Boolean
        Try

            If Value1.Length = Value2.Length Then
                Dim i As Integer
                While Value1(i) = Value2(i) And i < Value1.Length
                    i += 1
                End While
                Return (i = Value1.Length)
            Else
                Return False
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::ArrayCompare:Error:" + Bytes2StrByte(Value1) + ";" + Bytes2StrByte(Value2), ex)
            Return False
        End Try

    End Function

    ''' <summary>
    ''' Copia un array dentro de otro desde una posición dada
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <param name="Destination"></param>
    ''' <param name="Position"></param>
    ''' <remarks></remarks>
    Public Shared Sub ArrayCopyTo(ByVal Source As Byte(), ByRef Destination As Byte(), ByVal Position As Integer)
        Dim i As Integer
        Try

            For i = 0 To Source.Length - 1
                If (Position + i) >= Destination.Length Then Exit For
                Destination(Position + i) = Source(i)
            Next
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::ArrayCopyTo:" + Bytes2StrByte(Source) + ";" + Bytes2StrByte(Destination) + ";" + Position.ToString, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Transforma un array de bytes a texto (para log)
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <param name="Space"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Bytes2StrByte(ByVal Value As Byte(), Optional ByVal Space As Boolean = True) As String
        Dim tmp As String = ""
        Try
            If Not Value Is Nothing Then
                For Each bin As Byte In Value
                    tmp += System.Convert.ToString(bin, 16).PadLeft(2, "0") + IIf(Space, " ", "")
                Next
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGlobal::Bytes2StrByte::Error: ", ex)
            Return "0"
        End Try
        Return tmp
    End Function

    ''' <summary>
    ''' Pasa un byte a texto
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StrByte2Bytes(ByVal value As String) As Byte()
        Try

            Dim arr As Byte()
            Dim i As Integer = 0
            Dim tmp As String = ""
            tmp = value.Trim.Replace(" ", "")
            ReDim arr(tmp.Length / 2)
            While tmp.Length > 0
                arr(i) = Long.Parse(tmp.Substring(0, 2), Globalization.NumberStyles.HexNumber)
                i += 1
                tmp = tmp.Substring(2)
            End While
            Return arr
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::StrByte2Bytes:", ex)
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try

    End Function

    ''' <summary>
    ''' Pasa un array de byte a numero
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Bytes2Lng(ByVal Value As Byte()) As Int64
        Dim tmp As String = ""
        Dim i As Integer
        Try

            For i = 0 To Value.Length - 1
                tmp += System.Convert.ToString(Long.Parse(Value(i)), 16).PadLeft(2, "0")
            Next
            Return UInt64.Parse(tmp, Globalization.NumberStyles.HexNumber)
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Bytes2Lng:" + Bytes2StrByte(Value), ex)
            Return 0
        End Try
    End Function


    Public Shared Function Any2Bytes(ByVal Value As Object) As Byte()
        Try
            If Value Is Nothing Then
                Return Array.CreateInstance(GetType(Byte), 0)
            End If

            If Value Is System.DBNull.Value Then
                Return Array.CreateInstance(GetType(Byte), 0)
            End If

            Try
                Dim tmp As Byte() = Value
                Return tmp
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                Return Array.CreateInstance(GetType(Byte), 0)
            End Try

            Return Array.CreateInstance(GetType(Byte), 0)


        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2Bytes:", ex)
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try
    End Function

    ''' <summary>
    ''' Trasforma una variable a boleana
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2Boolean(ByVal Value As Object) As Boolean
        Try
            If Value.GetType.Equals(GetType(Integer)) Then
                If CType(Value, Integer) = 1 Then
                    Return True
                Else
                    Return False
                End If
            ElseIf Value.GetType.Equals(GetType(String)) Then
                If Value.ToString.ToUpper = "TRUE" Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return Boolean.Parse(Value.ToString)
            End If


        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2Booleand:", ex)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Transforma una variable a numero
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2Long(ByVal Value As Object) As Long
        Try

            If Value.GetType.Equals(GetType(Integer)) Then
                Return CLng(Value)
            ElseIf Value.GetType.Equals(GetType(String)) Then
                Try
                    Return CLng(Value)
                Catch ex As Exception
                    clsSystemControl.ControlException(ex)
                    Return 0
                End Try
            ElseIf IsNumeric(Value) Then
                Return Long.Parse(Value)
            End If
            Return CLng(Value)
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2Long:", ex)
            Return 0
        End Try

    End Function

    ''' <summary>
    ''' transforma una variable a texto
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2String(ByVal Value As Object) As String
        Try
            If Value Is Nothing Then
                Return ""
            Else
                Return Value.ToString
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2String:", ex)
            Return ""
        End Try

    End Function

    ''' <summary>
    ''' transforma una variable a fecha
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2Date(ByVal Value As Object) As DateTime
        Try
            If Value.GetType.Equals(GetType(String)) Then
                If Value = "0001-01-01 00:00:00" Then
                    Return NULLDATETIME
                Else
                    Try
                        Return DateTime.ParseExact(Value, "yyyy-MM-dd HH:mm:ss", Nothing)
                    Catch ex As Exception
                        Return DateTime.ParseExact(Value, "yyyy-MM-dd HH.mm.ss", Nothing)
                    End Try
                End If
            ElseIf Value.GetType.Equals(GetType(DateTime)) Then
                Return Value
            Else
                Return DateTime.Parse(Value.ToString)
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2Date:", ex)

        End Try
    End Function

    ''' <summary>
    ''' transforma una variable a fecha
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2DateString(ByVal Value As Object) As String
        Try
            Dim dat As DateTime = Any2Date(Value)
            Return dat.ToString("yyyy") + "-" + dat.ToString("MM") + "-" + dat.ToString("dd") + " " + dat.ToString("HH") + ":" + dat.ToString("mm") + ":" + dat.ToString("ss")
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2DateString:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' tranforma una variable a texto para el SQLite
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Any2SQL(ByVal value As Object) As String
        Try
            If value Is Nothing Then Return "''"

            Dim tmp As String
            If value.GetType() Is Type.GetType("System.Boolean") Then
                Return IIf(value, "1", "0")
            End If

            If value.GetType() Is Type.GetType("System.Integer") Or value.GetType() Is Type.GetType("System.Int32") Or value.GetType() Is Type.GetType("System.byte") Then
                Return value.ToString
            End If

            If value.GetType() Is Type.GetType("System.DateTime") Or value.GetType() Is Type.GetType("System.Date") Then
                Dim dat As DateTime = CType(value, DateTime)
                Return "'" + dat.ToString("yyyy") + "-" + dat.ToString("MM") + "-" + dat.ToString("dd") + " " + dat.ToString("HH") + ":" + dat.ToString("mm") + ":" + dat.ToString("ss") + "'"
            End If

            tmp = value.ToString
            If tmp.Trim = "" Then Return "''"

            Return "'" & tmp.Replace("'", "´") & "'"

        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::Any2SQL:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Tranforma una fecha a una fecha valida para el SQLite
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function date2SQLDate(ByVal value As DateTime) As String
        Try

            Return value.ToString("yyyy") + "-" + value.ToString("MM") + "-" + value.ToString("dd") + " " + value.ToString("HH") + ":" + value.ToString("mm") + ":" + value.ToString("ss")

        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::date2SQLDate:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Codifica un texto a base64
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StrEncoding(ByVal value As String) As String
        Try
            Dim byt() As Byte = System.Text.Encoding.UTF8.GetBytes(value)
            Dim str As String = Convert.ToBase64String(byt)
            Return str.Replace("=", "-")
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::StrEncoding:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Codifica un array de byte a base64
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function BytesEncoding(ByVal value() As Byte) As String
        Try
            If Not value Is Nothing Then
                Dim str As String = Convert.ToBase64String(value)
                Return str.Replace("=", "-")
            Else
                Return ""
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::BytesEncoding:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Trandorma un texto en base64 a texto
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StrDecoding(ByVal value As String) As String
        Try
            If value.Length > 0 Then
                Dim byt() As Byte = Convert.FromBase64String(value.Replace("-", "="))
                Return System.Text.Encoding.UTF8.GetString(byt, 0, byt.Length)
            Else
                Return ""
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::StrDecoding:", ex)
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' tranforma un texto en base64 a array de bytes
    ''' </summary>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function BytesDecoding(ByVal value As String) As Byte()
        Try
            If value.Length > 0 Then
                Dim byt() As Byte = Convert.FromBase64String(value.Replace("-", "="))
                Return byt
            Else
                Return Array.CreateInstance(GetType(Byte), 0)
            End If
        Catch ex As Exception
            clsSystemControl.ControlException(ex)
            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsGlobal::BytesDecoding:", ex)
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try
    End Function

End Class
