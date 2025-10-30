Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class clsTerminalPunch

        Public Enum eCurrentMoveType
            In_
            Out_
            Acc_
            Center_
        End Enum

        Protected _Card As String
        Protected _IDEmployee As Integer
        Protected _Reader As Integer
        Protected _Action As String
        Protected _Incidence As Integer
        Protected _IDIncidence As Integer
        Protected _IncidenceName As String = ""
        Protected _Terminal As TerminalMxS
        Protected _ReaderMode As String
        Protected _SaveOnClose As Boolean = True
        Protected _MoveType As eCurrentMoveType

        'Información del marcaje actual
        Protected _CurrMoveStatus As PunchStatus
        Protected _CurrMoveDateTime As DateTime
        Protected _CurrPunchState As Punch.roPunchState
        Protected _CurrPunch As Punch.roPunch

#Region "Property"

        Public Property DatePunch() As DateTime
            Get
                Return _CurrMoveDateTime
            End Get
            Set(ByVal Value As DateTime)
                _CurrMoveDateTime = Value
            End Set
        End Property

        Public Property Card(_CardType As clsParameters.eCardType) As String
            Get
                Dim sDecCard As String = 0
                Try
                    ' La tarjeta llega del terminal en hexa ... 
                    ' ... pues han cambiado el firmware y ya no !!!!!!!!!!!! 
                    'sDecCard = Convert.ToInt32(_Card, 16)
                    sDecCard = _Card
                    Select Case _CardType
                        Case clsParameters.eCardType.HID
                            ' Tarjeta HID.
                            ' TODO: En rxC, rxCe y rxF, se guarda en BBDD en OCTAL
                            'Return _Card
                            Return mdPublic.AddHIDParityBits(sDecCard)
                        Case clsParameters.eCardType.Mifare
                            ' Tarjeta MiFare. En BBDD es guarda con codificación Robotics
                            Return mdPublic.EncodeRoboticsCard(sDecCard)
                        Case clsParameters.eCardType.Unique
                            ' Tarjeta UNIQUE, con codificación Robotics
                            Return mdPublic.EncodeRoboticsCard(sDecCard)
                        Case clsParameters.eCardType.UniqueNumeric
                            ' Tarjeta UNIQUE, sin codificación Robotics
                            Return sDecCard
                        Case Else
                            Return sDecCard
                    End Select
                Catch ex As Exception
                    Return sDecCard
                End Try
            End Get
            Set(ByVal Value As String)
                _Card = Value
            End Set
        End Property

        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal Value As Integer)
                _IDEmployee = Value
            End Set
        End Property

        Public ReadOnly Property Terminal() As TerminalMxS
            Get
                Return _Terminal
            End Get
        End Property

        Public Property Incidence() As Integer
            Get
                Return _Incidence
            End Get
            Set(ByVal Value As Integer)
                _IDIncidence = Value
                If _IDIncidence >= 0 Then
                    Dim oCauseState As Cause.roCauseState = New Cause.roCauseState
                    Dim oCause As Cause.roCause = New Cause.roCause(_IDIncidence, oCauseState)
                    oCause.Load()
                    _IncidenceName = oCause.Name
                    _Incidence = oCause.ReaderInputcode
                End If

            End Set
        End Property

        Public ReadOnly Property IncidenceName() As String
            Get
                Return _IncidenceName
            End Get
        End Property

        Public ReadOnly Property IDIncidence() As Integer
            Get
                Return _IDIncidence
            End Get
        End Property

        Public Property Reader() As Integer
            Get
                Return _Reader
            End Get
            Set(ByVal Value As Integer)
                _Reader = Value
            End Set
        End Property

        Public Property Action() As String
            Get
                Return _Action
            End Get
            Set(ByVal value As String)
                _Action = value
            End Set
        End Property

        Public Property SaveOnClose() As Boolean
            Get
                Return _SaveOnClose
            End Get
            Set(ByVal value As Boolean)
                _SaveOnClose = value
            End Set
        End Property

#End Region

        Public Sub New(ByVal Terminal As TerminalMxS)
            Try
                'oLog = Log
                _Terminal = Terminal
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::New::Error:", ex)
            End Try
        End Sub

        Public Sub New(ByRef Terminal As TerminalMxS, ByVal PunchDateTime As DateTime, ByVal UserId As String, ByVal Card As String, ByVal Reader As Integer, ByVal Action As String, ByVal Incidence As Integer, ByVal Photo As Byte(), Optional ByVal Other As String = "")
            Try

                '
                ' Aquí bucle de espera para simular lentitud al guardar fichajes (simulando cuaje de BBDD)
                '

                _Terminal = Terminal
                _CurrMoveDateTime = PunchDateTime
                _Reader = Reader
                _Action = Action
                _Card = Card
                _IDEmployee = UserId
                Me.Incidence = Incidence
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::New::Error:", ex)
            End Try

        End Sub

        Public Overridable Sub Load()

            Try
                ' Obtener información del último marcaje de presencia del empleado
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                Dim oPunch As Punch.roPunch = New Punch.roPunch(_IDEmployee, -1, oPunchState)

                If oPunchState.Result <> PunchResultEnum.NoError Then
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::Load::Debug:" & oPunchState.ErrorDetail)
                End If

                VerifyMove()
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::Load::Error:", ex)
            End Try
        End Sub

        Protected Overridable Sub VerifyMove()
            Try
                Select Case _Terminal.RDRMode(_Reader)
                    Case "ACC"
                        _MoveType = eCurrentMoveType.Acc_
                    Case "ACCTA"
                        VerifyMove_ACCTA()
                    Case "CO"
                        _MoveType = eCurrentMoveType.Center_
                End Select
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::VerifyMove::Error:", ex)
            End Try
        End Sub

        Protected Overridable Sub VerifyMove_ACCTA()
            Try
                If _Terminal.RDRWorkingZone(_Reader) Then
                    _MoveType = eCurrentMoveType.In_
                Else
                    _MoveType = eCurrentMoveType.Out_
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::VerifyMove_ACCTA::Error:", ex)
            End Try
        End Sub

        Public Overridable Function SavePunch(_CardType As clsParameters.eCardType) As Boolean
            Try
                'Guardamos el marcaje
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                Dim oPunch As Punch.roPunch = New Punch.roPunch(_IDEmployee, -1, oPunchState)
                Dim oExistingPunch As DataTable
                Dim sModeForLog As String = "acc"

                If _IDEmployee = 0 Then oPunch.IDCredential = Me.Card(_CardType)

                ' Corrección de hora para centralitas en perímetro por si el desfase en segundos con fichajes de presencia provoca un problema en informe de emergencia
                Try
                    If _Terminal.PunchTimeOffset <> 0 AndAlso _Terminal.RDRIsOnlyAccess(_Reader) Then
                        _CurrMoveDateTime = _CurrMoveDateTime.AddSeconds(_Terminal.PunchTimeOffset)
                    End If
                Catch ex As Exception
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::" & _Terminal.ToString & ":Error checking if punch offset should be applied. Datetime kept as it is!", ex)
                End Try

                'No se guarda un fichaje si ya existe otro con la misma hora:min:seg para el mismo empleado desde el mismo terminal y del mismo tipo
                Dim strFilter As String = ""
                strFilter = "IDEmployee = " + Me.IDEmployee.ToString + " AND DateTime = " + roTypes.Any2Time(Me._CurrMoveDateTime).SQLDateTime + " AND IDTerminal = " + Me._Terminal.ID.ToString
                oExistingPunch = Punch.roPunch.GetPunches(strFilter, oPunchState)
                If oExistingPunch.Rows.Count > 0 Then
                    ' No guardo
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::" & _Terminal.ToString & ":Punch duplicated. It will not be saved (" + _Card + ";" + _IDEmployee.ToString + ";" + _Terminal.ID.ToString + ";" + _Reader.ToString + ";" + _CurrMoveDateTime.ToShortDateString + " " + _CurrMoveDateTime.ToLongTimeString + ";" + _IDIncidence.ToString + ";" + oPunch.Type.ToString + ";" + oPunch.InvalidType.ToString + ")")
                    Return True
                End If

                ' Guardo
                oPunch.TypeData = _IDIncidence
                oPunch.IDTerminal = _Terminal.ID
                oPunch.IDReader = _Reader
                oPunch.IDZone = _Terminal.RDRZone(_Reader)
                oPunch.DateTime = _CurrMoveDateTime

                Select Case _Action
                    Case "AIC", "AIR"
                        oPunch.Type = PunchTypeEnum._AI
                        oPunch.InvalidType = InvalidTypeEnum.NRDR_
                        'If _IDEmployee = 0 Then bSave = False
                    Case "AIT"
                        oPunch.Type = PunchTypeEnum._AI
                        oPunch.InvalidType = InvalidTypeEnum.NTIME_
                        'If _IDEmployee = 0 Then bSave = False
                    Case "AID"
                        oPunch.Type = PunchTypeEnum._AI
                        oPunch.InvalidType = InvalidTypeEnum.NOHP_
                    Case "A", "AV"
                        ' Miro si además de accesos tengo que guardar presencia en función del modo configurado para el lector
                        If _Terminal.RDRMode(_Reader).IndexOf("ACCTA") > -1 Then
                            oPunch.Type = PunchTypeEnum._L
                            sModeForLog = "accta"
                        ElseIf _Terminal.RDRMode(_Reader) = "CO" Then
                            oPunch.Type = PunchTypeEnum._CENTER
                            sModeForLog = "co"
                        Else
                            oPunch.Type = PunchTypeEnum._AV
                        End If
                    Case "AIAPB"
                        oPunch.Type = PunchTypeEnum._AI
                        oPunch.InvalidType = InvalidTypeEnum.NAPB
                    Case "E"
                        oPunch.Type = PunchTypeEnum._IN
                    Case "S"
                        oPunch.Type = PunchTypeEnum._OUT
                    Case "L"
                        oPunch.Type = PunchTypeEnum._L

                End Select
                If _MoveType = eCurrentMoveType.Center_ Then
                    oPunch.TypeData = Me.Terminal.GetCostCenter
                End If

                If oPunch.Save() Then
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Punch saved (" + _Card + ";" + _IDEmployee.ToString + ";" + _Terminal.ID.ToString + ";" + _Reader.ToString + ";" + _CurrMoveDateTime.ToShortDateString + " " + _CurrMoveDateTime.ToLongTimeString + ";" + _IDIncidence.ToString + ";" + oPunch.Type.ToString + ";" + oPunch.InvalidType.ToString + ")")
                    Return True
                Else
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::SavePunch::Error:" & oPunchState.ErrorDetail & ". Check entries.vtr file on Readings folder.")
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch:: SavePunch()::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function ToString() As String
            Try
                Return _Terminal.ID.ToString + ";" + _IDEmployee.ToString + ";" + _Card + ";" + _CurrMoveDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + _Action + ";" + _Incidence.ToString + ";" + _Reader.ToString
            Catch ex As Exception
                Return ""
            End Try
        End Function

    End Class

End Namespace