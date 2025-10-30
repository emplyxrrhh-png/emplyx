Imports Robotics.Base
Imports Robotics.Comms
Public Class CardManager
    Public gLog As New Robotics.Comms.Base.roLogComms("CardManager")
    Private _ReaderType As eCardType
    Private _TerminalCardCodification As eCardCode
    Private _DriverCardCodification As eCardCode
    Private _ReaderBytes As Byte
    Private _CommsReceivedCardCode As String
    Private _BBDDCode As String

    Public Enum eCardCode
        Hex = 0
        Numeric = 1
        Robotics = 2
    End Enum

    Public Enum eCardType
        None = -1
        Unique = 1
        Mifare = 2
        HID = 3
    End Enum

    Public Property ReaderType As eCardType
        Get
            Return _ReaderType
        End Get
        Set(value As eCardType)
            _ReaderType = value
        End Set
    End Property

    Public Property TerminalCardCodification As eCardCode
        Get
            Return _TerminalCardCodification
        End Get
        Set(value As eCardCode)
            _TerminalCardCodification = value
        End Set
    End Property

    Public Property DriverCardCodification As eCardCode
        Get
            Return _DriverCardCodification
        End Get
        Set(value As eCardCode)
            _DriverCardCodification = value
        End Set
    End Property

    Public Property ReaderBytes As Byte
        Get
            Return _ReaderBytes
        End Get
        Set(value As Byte)
            _ReaderBytes = value
        End Set
    End Property

    Public Property CommsReceivedCardCode As String
        Get
            Return _CommsReceivedCardCode
        End Get
        Set(value As String)
            _CommsReceivedCardCode = value
            Dim sCardTemp As String = ""
            ' Si el terminal aplicó alguna codificación, lo tengo en cuenta ...
            Select Case TerminalCardCodification
                Case eCardCode.Hex
                    ' TODO: De momento consideramos que los terminales no codifican
                    sCardTemp = _CommsReceivedCardCode
                Case eCardCode.Numeric
                    ' Lo habitual
                    sCardTemp = _CommsReceivedCardCode
                Case eCardCode.Robotics
                    ' TODO: De momento consideramos que los terminales no codifican
                    sCardTemp = _CommsReceivedCardCode
            End Select

            ' Ahora miro si debo codificar antes de guardar
            Select Case DriverCardCodification
                Case eCardCode.Hex
                    'TODO
                    _BBDDCode = "Comming Soon"
                Case eCardCode.Numeric
                    _BBDDCode = CommsReceivedCardCode
                Case eCardCode.Robotics
                    _BBDDCode = EncodeRoboticsCard(sCardTemp)
            End Select
        End Set
    End Property

    Public Property BBDDCode As String
        Get
            Return _BBDDCode
        End Get
        Set(value As String)
            _BBDDCode = value
            Dim sCardTemp As String = ""
            ' Ahora miro si debo codificar antes de guardar
            Select Case DriverCardCodification
                Case eCardCode.Hex
                    'TODO
                    sCardTemp = "Comming Soon"
                Case eCardCode.Numeric
                    sCardTemp = CutCard(_BBDDCode, _ReaderBytes * 2)
                Case eCardCode.Robotics
                    sCardTemp = DecodeRoboticsCard(_BBDDCode, _ReaderBytes * 4)
            End Select

            ' Si el terminal aplicó alguna codificación, lo tengo en cuenta ...
            Select Case TerminalCardCodification
                Case eCardCode.Hex
                    ' TODO: De momento consideramos que los terminales no codifican
                    _CommsReceivedCardCode = sCardTemp
                Case eCardCode.Numeric
                    ' Lo habitual
                    _CommsReceivedCardCode = sCardTemp
                Case eCardCode.Robotics
                    ' TODO: De momento consideramos que los terminales no codifican
                    _CommsReceivedCardCode = sCardTemp
            End Select
        End Set
    End Property

    Public Sub New(readerbytes As Byte, drivercardcodification As eCardCode, readertype As eCardType, Optional terminalcardcodification As eCardCode = eCardCode.Numeric)
        _ReaderBytes = readerbytes
        _TerminalCardCodification = terminalcardcodification
        _DriverCardCodification = drivercardcodification
        _ReaderType = readertype
    End Sub

    ''' <summary>
    ''' Convierte código de tarjeta numérico a codificación Robotics
    ''' </summary>
    ''' <param name="IDCard"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function EncodeRoboticsCard(ByVal IDCard As String) As Long
        Dim sIDCard As String = ""
        Dim tmp As String = ""
        Try

            tmp = Convert.ToString(Long.Parse(Robotics.Base.roConversions.Any2Double(IDCard)), 16)
            While tmp.Length > 0
                sIDCard += IIf(Convert.ToInt16(tmp.Substring(0, 1), 16) > 9, "", "0") + Convert.ToInt16(tmp.Substring(0, 1), 16).ToString
                tmp = tmp.Substring(1)
            End While
            Return Robotics.Base.roTypes.Any2Long(sIDCard)

        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::EncodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
            Return 0
        End Try

    End Function

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
                        gLog.logMessage(roLog.EventType.roWarning, "mdPublic::DecodeRoboticsCard::Warning: IDCard " + IDCard + " is invalid.")
                        Return 0
                    End If
                    sIDCard = Convert.ToString(Integer.Parse(Right(stmp, 2)), 16) + sIDCard
                    stmp = stmp.Substring(0, stmp.Length - 2)
                End While
                If stmp.Length > 0 Then sIDCard = Convert.ToString(Integer.Parse(stmp), 16) + sIDCard
                sIDCard = Convert.ToInt64(sIDCard, 16).ToString
            End If

            Return Robotics.Base.roTypes.Any2Long(sIDCard)
        Catch ex As Exception
            gLog.logMessage(roLog.EventType.roError, "mdPublic::DecodeRoboticsCard::Error:(IDCard:" + IDCard + "):", ex)
            Return 0
        End Try
    End Function

    Public Function CutCard(ByVal Card As String, ByVal ByteLenght As Byte) As Long
        Try
            Dim tmp As String = ""
            'Convertimos a hex
            tmp = Convert.ToString(Long.Parse(Robotics.Base.roConversions.Any2Double(Card)), 16)
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
End Class
