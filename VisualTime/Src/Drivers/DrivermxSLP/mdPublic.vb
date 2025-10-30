'Imports Robotics.Comms.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Module mdPublic

    Public Const NULLDATE = "1900-01-01 00:00:00"

    Private oBroadcasterThread As Threading.Thread

    Public gLog As New roLog("DriverMxS")

    Public Function Any2Date(ByVal Value As Object) As DateTime
        Try
            If Value.GetType.Equals(GetType(String)) Then
                Return DateTime.ParseExact(Value, "yyyy-MM-dd HH:mm:ss", Nothing)
            ElseIf Value.GetType.Equals(GetType(DateTime)) Then
                Return Value
            Else
                Return DateTime.Parse(Value.ToString)
            End If
        Catch ex As Exception
            Return roTypes.Any2Time(Value).Value
        End Try
    End Function

    Public Function date2TermDate(ByVal value As DateTime) As String
        Try

            Return value.ToString("yyyy") + "-" + value.ToString("MM") + "-" + value.ToString("dd") + " " + value.ToString("HH") + ":" + value.ToString("mm") + ":" + value.ToString("ss")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function Byte2StrByte(ByVal Value As Byte) As String
        Return System.Convert.ToString(roTypes.Any2Long(Value), 16)
    End Function

    Public Function Bytes2StrByte(ByVal Value As Byte(), Optional ByVal Space As Boolean = True) As String
        Dim tmp As String = ""
        Try
            If Value IsNot Nothing Then
                For Each bin As Byte In Value
                    tmp += Byte2StrByte(bin).PadLeft(2, "0") + IIf(Space, " ", "")
                Next
            End If
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::Bytes2StrByte::Error: ", ex)
            Return "0"
        End Try
        Return tmp
    End Function

    Public Function StrByte2Bytes(ByVal value As String) As Byte()
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

    End Function

    Public Function CompareArray(ByVal Array1 As Byte(), ByVal Array2 As Byte()) As Boolean
        Dim i As Integer = 0
        'Si son de diferente tamaño ya son diferentes
        If Array2.Length <> Array1.Length Then Return False
        While Array1(i) = Array2(i)
            i += 1
            If i = Array1.Length Then Exit While
        End While
        If Array2.Length = i Then
            Return True
        Else
            Return False
        End If
    End Function

    Private oProcessIDs() As TasksType

    Public Sub ExecuteEntries()
        Try
            If Not System.Reflection.Assembly.GetExecutingAssembly.GetName.Name.Contains("LP") Then
                ' Sólo si no es una versión Punches, llamo al proceso de Movimientos
                roConnector.InitTask(TasksType.ENTRIES)
                gLog.logMessage(roLog.EventType.roDebug, "mdPublic::ExecuteEntries: Calling entries.")
            End If
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ExecuteEntries:Unexpected error:", ex)
        End Try
    End Sub

    Public Sub ExecuteMoves()
        Try
            If Not System.Reflection.Assembly.GetExecutingAssembly.GetName.Name.Contains("LP") Then
                ' Sólo si no es una versión Punches, llamo al proceso de Movimientos
                roConnector.InitTask(TasksType.MOVES)
                gLog.logMessage(roLog.EventType.roDebug, "mdPublic::ExecuteMoves: Calling moves.")
            End If
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ExecuteMoves:Unexpected error:", ex)
        End Try
    End Sub

    Public Sub CallBroadcaster(Optional ByVal IDTerminal As Integer = 0, Optional ByVal Task As Base.roTerminalsSyncTasks.SyncActions = Base.roTerminalsSyncTasks.SyncActions.none, Optional ByVal IDEmployee As Integer = 0, Optional ByVal OnlyTask As Boolean = True, Optional ByVal IDFinger As Integer = 0)
        Try
            Dim roConnector As New roConnector()
            Dim oCollection As New roCollection
            If IDTerminal > 0 Then
                oCollection.Add("Command", "ON_ADD_TASK")
                oCollection.Add("IDTerminal", IDTerminal.ToString)
                oCollection.Add("TerminalsTask", Task.ToString)
                oCollection.Add("IDEmployees", IDEmployee.ToString)
                oCollection.Add("OnlyTask", OnlyTask.ToString)
                oCollection.Add("IDFinger", IDFinger.ToString)
            End If
            roConnector.InitTask(TasksType.BROADCASTER, oCollection)
            gLog.logMessage(roLog.EventType.roDebug, "mdPublic::CallBroadcaster::Terminal " + IDTerminal.ToString + "::Call broadcaster(" + Task.ToString + "," + IDEmployee.ToString + "," + OnlyTask.ToString + ")")
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::CallBroadcaster::Terminal " + IDTerminal.ToString + "::Error::", ex)
        End Try
    End Sub

    Private Sub CallBroadcasterExDriver()
        ' Espera cinco segundos antes de llamar finalmente al Broadcaser. Si antes de esos 5 segundos llega otra llamada, este thread será abortado, y por tanto no se llamará a Broadcaster
        Try
            Threading.Thread.Sleep(10000)
            CallBroadcaster()
        Catch ex As Exception
            ' gLog.logMessage(roLog.EventType.roError, "mdPublic::CallBroadcasterExDriver::Error::", ex)
        End Try
    End Sub

    Public Sub CallBroadcasterEx()
        Try
            If Not oBroadcasterThread Is Nothing Then
                oBroadcasterThread.Abort()
                gLog.logMessage(roLog.EventType.roDebug, "mdPublic::CallBroadcasterEx::Delaying Broadcaster execution")
            End If
            oBroadcasterThread = New Threading.Thread(AddressOf CallBroadcasterExDriver)
            oBroadcasterThread.Start()
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::CallBroadcasterEx::Error::", ex)
        End Try
    End Sub

    Public Sub ExecuteJobMoves()
        Try
            roConnector.InitTask(TasksType.EMPLOYEEJOBTIME)
            gLog.logMessage(roLog.EventType.roDebug, "mdPublic::ExecuteJobMoves: Calling job moves.")
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ExecuteJobMoves:Unexpected error:", ex)
        End Try
    End Sub

    Public Sub ExecuteJobTeamMoves()
        Try
            roConnector.InitTask(TasksType.TEAMJOBTIME)
            gLog.logMessage(roLog.EventType.roDebug, "mdPublic::ExecuteJobTeamMoves: Calling job teams moves.")
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ExecuteJobTeamMoves:Unexpected error:", ex)
        End Try
    End Sub

    Public Sub ExecuteJobMachineMoves()
        Try
            roConnector.InitTask(TasksType.MACHINEJOBMOVES)
            gLog.logMessage(roLog.EventType.roDebug, "mdPublic::ExecuteJobMachineMoves: Calling job machines moves.")
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ExecuteJobMachineMoves:Unexpected error:", ex)
        End Try
    End Sub

    Public Function StrEncoding(ByVal value As String) As String
        Try
            Dim byt() As Byte = System.Text.Encoding.UTF8.GetBytes(value)
            Dim str As String = Convert.ToBase64String(byt)
            Return str.Replace("=", "-")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function BytesEncoding(ByVal value() As Byte) As String
        Try
            Dim str As String = Convert.ToBase64String(value)
            Return str.Replace("=", "-")
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function StrDecoding(ByVal value As String) As String
        Try
            Dim byt() As Byte = Convert.FromBase64String(value.Replace("-", "="))
            Return System.Text.Encoding.UTF8.GetString(byt, 0, byt.Length)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Function BytesDecoding(ByVal value As String) As Byte()
        Try
            Dim byt() As Byte = Convert.FromBase64String(value.Replace("-", "="))
            Return byt
        Catch ex As Exception
            Return Array.CreateInstance(GetType(Byte), 0)
        End Try
    End Function

    Public Sub CreateUserTask(ByVal ID As Integer)
        Dim oState As New UserTask.roUserTaskState()
        Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\TERMINAL_TYPEMISMATCH_" & ID.ToString, oState)
        If oTaskExist.Message = "" Then
            Dim oTask As New UserTask.roUserTask()
            With oTask
                .ID = "USERTASK:\\TERMINAL_TYPEMISMATCH_" & ID.ToString
                .Message = "Existe un terminal con el ID " + ID.ToString + " que no corresponde a un tipo correcto, contacte con su administrador"
                .DateCreated = Now
                .TaskType = UserTask.TaskType.UserTaskRepair
                .ResolverURL = ""
                .Save()
                gLog.logMessage(roLog.EventType.roDebug, "mdPublic::CreateUserTask:" & ID.ToString & ":User task created.")
            End With
        End If
    End Sub

    Public Sub DelUserTask(ByVal ID As Integer)
        Dim oState As New UserTask.roUserTaskState()
        Dim oTaskExist As New UserTask.roUserTask("USERTASK:\\TERMINAL_TYPEMISMATCH_" & ID.ToString, oState)
        'Si existe la tarea la borramos
        If oTaskExist.Message <> "" Then
            oTaskExist.Delete()
            gLog.logMessage(roLog.EventType.roDebug, "mdPublic::DelUserTask:" & ID.ToString & ":User task deleted because the terminal registered yet.")
        End If
    End Sub

    Public Function DecodeRoboticsCard(ByVal IDCard As String, Optional ByVal MaxLen As Byte = 16) As Long
        Dim sIDCard As String = ""
        Try
            Dim stmp As String

            If IDCard.Trim <> "" Then
                If IDCard.Length > MaxLen Then
                    stmp = Right(IDCard, MaxLen)
                Else
                    stmp = IDCard
                End If
                While stmp.Length >= 2
                    If Convert.ToString(Integer.Parse(Right(stmp, 2)), 16).Length > 1 Then
                        gLog.logMessage(roLog.EventType.roDebug, "mdPublic::DecodeRoboticsCard::Warning: IDCard " + IDCard + " is invalid.")
                        Return 0
                    End If
                    sIDCard = Convert.ToString(Integer.Parse(Right(stmp, 2)), 16) + sIDCard
                    stmp = stmp.Substring(0, stmp.Length - 2)
                End While
                If stmp.Length > 0 Then sIDCard = Convert.ToString(Integer.Parse(stmp), 16) + sIDCard
                sIDCard = Convert.ToInt64(sIDCard, 16).ToString
            End If

            Return Robotics.VTBase.roTypes.Any2Long(sIDCard)
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::DecodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Convierte un código de tarjeta guardado en VT para enviarlo al terminal
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ConvertCardForTerminal(_VTCard As String, _CardType As clsParameters.eCardType, Optional uniqueReadBytes As Integer = 3, Optional mifareReadBytes As Integer = 4) As String
        Dim sResCard As String = String.Empty
        Try
            Select Case _CardType
                Case clsParameters.eCardType.HID
                    ' Tarjeta HID. En BBDD se guarda como un número octal, incluyendo bits de paridad ...
                    sResCard = RemoveHIDParityBits(_VTCard)
                Case clsParameters.eCardType.Mifare
                    ' Tarjeta MiFare. En BBDD se guarda con codificación Robotics, y se envían 4 bytes al terminal
                    ' sResCard = mdPublic.DecodeRoboticsCard(_VTCard, 16)
                    sResCard = mdPublic.DecodeRoboticsCard(_VTCard, mifareReadBytes * 4)
                Case clsParameters.eCardType.Unique
                    ' Tarjeta MiFare. En BBDD se guarda con codificación Robotics, y se envían 3 bytes al terminal
                    sResCard = mdPublic.DecodeRoboticsCard(_VTCard, 12)
                Case clsParameters.eCardType.UniqueNumeric
                    ' Tarjeta UNIQUE, sin codificación Robotics. Simplemente paso a hexa y corto a 3 bytes (6 caracteres HEXA)
                    sResCard = CutCard(_VTCard, 6)
                Case clsParameters.eCardType.MifareNumeric
                    ' Tarjeta Mifare, sin codificación Robotics. Simplemente paso a hexa y corto a 4 bytes (8 caracteres HEXA)
                    ' sResCard = CutCard(_VTCard, 8)
                    sResCard = CutCard(_VTCard, mifareReadBytes * 2)
                Case Else
                    sResCard = CutCard(_VTCard, 6)
            End Select

            ' Hay que enviar en HEXA ... pues con el nuevo firmware no!!
            ' Return roTypes.Any2Integer(sResCard).ToString("x")
            Return sResCard
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ConvertCardForTerminal::Error:(IDCard:" + _VTCard + "):", ex)
            Return _VTCard
        End Try

    End Function

    ''' <summary>
    ''' Convierte código de tarjeta numérico a codificación Robotics
    ''' </summary>
    ''' <param name="IDCard"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function EncodeRoboticsCard(ByVal IDCard As String) As Long
        Dim sIDCard As String = ""
        Dim tmp As String = ""
        Try

            tmp = Convert.ToString(Long.Parse(Robotics.VTBase.roConversions.Any2Double(IDCard)), 16)
            While tmp.Length > 0
                sIDCard += IIf(Convert.ToInt16(tmp.Substring(0, 1), 16) > 9, "", "0") + Convert.ToInt16(tmp.Substring(0, 1), 16).ToString
                tmp = tmp.Substring(1)
            End While
            Return Robotics.VTBase.roTypes.Any2Long(sIDCard)
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::EncodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
            Return 0
        End Try

    End Function

    Public Function CutCard(ByVal Card As String, ByVal ByteLenght As Byte) As Long
        Try
            Dim tmp As String = ""
            'Convertimos a hex
            tmp = Convert.ToString(Long.Parse(Robotics.VTBase.roConversions.Any2Double(Card)), 16)
            If tmp.Length > ByteLenght Then
                'Cortamos los bytes sobrantes
                tmp = tmp.Substring(tmp.Length - ByteLenght)
                'Devolvemos el numero cortado
                Return Convert.ToInt32(tmp, 16)
            Else
                Return Long.Parse(Card)
            End If
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::CutCard::Error:(IDCard:" + Card + "):", ex)
            Return 0
        End Try
    End Function

    Public Function RemoveHIDParityBits(ByVal _VTCard As String) As String
        Try
            ' A partir de un número en octal, devuelve un código de tarjeta válido para programar terminales (número en decimal resultante de eliminar los bits de paridad y el bit 27)

            Dim iDecimalIDCard As Long
            Dim sBinaryIDCard As String
            Dim sBinaryIDCardWithoutParity As String

            ' Verifico si el número que me han pasado es octal. Si no, seguramente se trata de un código de tarjeta sin asignar
            Try
                iDecimalIDCard = Convert.ToInt32(_VTCard, 8)
            Catch ex As Exception
                ' No es octal. No es una tarjeta válida
                gLog.logMessage(roLog.EventType.roError, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. It should be octal")
                Return ""
            End Try

            ' Obtengo el binario
            sBinaryIDCard = Convert.ToString(iDecimalIDCard, 2)

            ' Elimino los bits 1, 26 y 27 de derecha a izquierda (No valido que sean correctos)
            If Len(sBinaryIDCard) <> 27 Then
                gLog.logMessage(roLog.EventType.roError, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. Should be 27 bits long")
                Return ""
            Else
                sBinaryIDCardWithoutParity = Mid$(sBinaryIDCard, 3, 24)
                Return Convert.ToInt32(sBinaryIDCardWithoutParity, 2).ToString
            End If
            Return ""
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::ConvertCardForTerminal::Error:(IDCard:" + _VTCard + "):", ex)
            Return ""
        End Try
    End Function

    Public Function AddHIDParityBits(ByVal _VTCard As String) As String
        Try
            ' A partir de un número entregado por un lector HID (24 bits), genera el número Octal que se va a guardar en BBDD, que es el mismo
            ' que lee un lector HID estandar Wiegand 26 bits (los terminales suelen procesar la lectura y quitan los bits de paridad)

            Dim sBinaryIDCard As String
            Dim sBinaryIDCardWithoutParity As String
            Dim sOddParityBit As String
            Dim sEvenParityBit As String
            Dim iCount As Integer

            iCount = 0

            ' Paso a binario
            sBinaryIDCardWithoutParity = Convert.ToString(Convert.ToInt32(_VTCard), 2)
            ' Completo hasta 24 con 0 por la izquierda
            sBinaryIDCardWithoutParity = sBinaryIDCardWithoutParity.PadLeft(24, "0")

            ' Añado el bit de paridad impar por la derecha
            ' - Cuento el número de veces que aparece el 1 en los doce dígitos de la derecha (menos significativos)
            '   - Si sale un número par, el bit de paridad es 1
            '   - Si sale un número impar, el bit de paridad es 0

            If sBinaryIDCardWithoutParity.Substring(12, 12).Replace("0", "").Length Mod 2 = 0 Then
                sOddParityBit = "1"
            Else
                sOddParityBit = "0"
            End If

            ' Añado el bit de paridad par por la izquierda
            ' - Cuento el número de veces que aparece el 1 en los dígitos a la izquierda de los doce dígitos menos significativos
            '   - Si sale un número par, el bit de paridad es 0
            '   - Si sale un número imapr, el bit de paridad es 1
            If sBinaryIDCardWithoutParity.Substring(0, 12).Replace("0", "").Length Mod 2 = 0 Then
                sEvenParityBit = "0"
            Else
                sEvenParityBit = "1"
            End If

            ' Añado el bit fijo "1" por la izquierda
            sBinaryIDCard = "1" & sEvenParityBit + sBinaryIDCardWithoutParity + sOddParityBit

            ' Valido que el número en binario tiene 27 dígitos
            If sBinaryIDCard.Length <> 27 Then
                'MsgBox("mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. Should be 27 bits long")
                gLog.logMessage(roLog.EventType.roError, "mdPublic::AddHIDParityBits::Error:IDCard:" + _VTCard + " not seems to be a valid car number. Should be 27 bits long")
                Return ""
            End If

            Return Convert.ToString(Convert.ToInt32(sBinaryIDCard, 2), 8)
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::AddHIDParityBits::Error:(IDCard:" + _VTCard + "):" & ex.Message)
            Return ""
        End Try
    End Function

    Private mCurrLang As String = ""


End Module