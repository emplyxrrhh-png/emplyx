Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class clsTerminalPunch

        Public Enum eCurrentMoveType
            In_
            Out_
            InOut_
            Acc_
            Center_
            Din_
        End Enum

        Protected _Card As String
        Protected _IDEmployee As Integer
        Protected _Reader As Integer
        Protected _Action As String
        Protected _Incidence As Integer
        Protected _IDIncidence As Integer
        Protected _IncidenceName As String = ""
        Protected _Terminal As TerminalZKPush2
        Protected _ReaderMode As String
        Protected _SaveOnClose As Boolean = True
        Protected _MoveType As eCurrentMoveType
        Protected _VerifyType As DTOs.VerificationType  'clsPushProtocol.dataVerifyType
        Protected _Status As clsPushProtocol.dataPunchStatus
        Protected _WearingMask As String = String.Empty
        Protected _Temperature As String = String.Empty

        'Información del marcaje actual
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

        Public Property Card() As String
            Get
                Try
                    Select Case clsParameters.CardType
                        Case clsParameters.eCardType.HID
                            ' Tarjeta HID.
                            'Return _Card
                            Return mdPublic.AddHIDParityBits(_Card)
                        Case clsParameters.eCardType.Mifare
                            ' Tarjeta MiFare. En BBDD es guarda con codificación Robotics
                            Return mdPublic.EncodeRoboticsCard(_Card)
                        Case clsParameters.eCardType.Unique
                            ' Tarjeta UNIQUE, con codificación Robotics
                            Return mdPublic.EncodeRoboticsCard(_Card)
                        Case clsParameters.eCardType.UniqueNumeric
                            ' Tarjeta UNIQUE, sin codificación Robotics
                            Return _Card
                        Case Else
                            Return _Card
                    End Select
                Catch ex As Exception
                    Return _Card
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

        Public ReadOnly Property Terminal() As TerminalZKPush2
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

        Public Sub New(ByRef Terminal As TerminalZKPush2, ByVal PunchDateTime As DateTime, ByVal EmployeeId As String, ByVal Card As String, ByVal Reader As Integer, ByVal Action As String, ByVal Incidence As Integer, ByVal VerifyType As DTOs.VerificationType, ByVal WearingMask As String, ByVal Temperature As String, ByVal Photo As Byte(), Optional ByVal Other As String = "")
            Try
                _Terminal = Terminal
                _CurrMoveDateTime = PunchDateTime
                _Reader = Reader
                _Action = Action
                _Card = Card
                _IDEmployee = EmployeeId
                _VerifyType = VerifyType
                _WearingMask = WearingMask
                _Temperature = Temperature
                ' Obtengo el id de la justificación correspondiente al código indicado en el terminal
                If Incidence > 0 Then
                    Me.Incidence = GetIDCauseFromReaderInputCode(Incidence)
                Else
                    Me.Incidence = Incidence
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::New::Error:", ex)
            End Try

        End Sub

        Public Overridable Sub Load()
            Try
                ' Obtener información del último marcaje de presencia del empleado
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState

                If oPunchState.Result <> PunchResultEnum.NoError Then
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::Load::Error:" & oPunchState.ErrorDetail)
                End If

                VerifyMove()
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::Load::Error:", ex)
            End Try
        End Sub

        Protected Overridable Sub VerifyMove()
            Try
                Select Case _Terminal.RDRMode(_Reader)
                    Case "TA", "TACO"
                        Select Case _Terminal.RDRInteractionAction(_Reader)
                            Case InteractionAction.E
                                _MoveType = eCurrentMoveType.In_
                            Case InteractionAction.S
                                _MoveType = eCurrentMoveType.Out_
                            Case Else
                                If _Terminal.RDRInteractionMode(1) = InteractionMode.Fast AndAlso (_VerifyType = VerificationType.FACE OrElse _VerifyType = VerificationType.PALM) Then
                                    Select Case _VerifyType
                                        Case VerificationType.FACE
                                            _MoveType = eCurrentMoveType.In_
                                        Case VerificationType.PALM
                                            _MoveType = eCurrentMoveType.Out_
                                    End Select
                                Else
                                    _MoveType = eCurrentMoveType.InOut_
                                End If
                        End Select
                    Case "ACC", "ACCTA"
                        _MoveType = eCurrentMoveType.Acc_
                    Case "CO"
                        _MoveType = eCurrentMoveType.Center_
                    Case "DIN"
                        _MoveType = eCurrentMoveType.Din_
                    Case Else
                        roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::VerifyMove::Unexpected reader mode" & _Terminal.RDRMode(_Reader))
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::VerifyMove::Error:", ex)
            End Try
        End Sub

        Public Overridable Function SavePunch(Optional ByVal sMessagesIds As String = "") As Boolean
            Try
                'Guardamos el marcaje
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState
                Dim oPunch As Punch.roPunch = New Punch.roPunch(_IDEmployee, -1, oPunchState)
                Dim oCostCenterPunch As Punch.roPunch = Nothing
                Dim oExistingPunch As DataTable
                Dim bSave As Boolean = True
                Dim sModeForLog As String = "acc"

                If _IDEmployee = 0 Then oPunch.IDCredential = Me.Card 'roTypes.Any2Long(_Card)

                ' Guardo
                oPunch.TypeData = _IDIncidence
                oPunch.IDTerminal = _Terminal.ID
                oPunch.IDReader = _Reader
                oPunch.IDZone = _Terminal.RDRZone(_Reader)
                oPunch.DateTime = _CurrMoveDateTime

                'Tratamiento BUG rx1: algunos rx1 envían el fichaje con un año futuro (día, mes y hora correctos) o se envian con un dia mas tardío (Offline), se arreglan ambos casos
                If _Terminal.Model.ToUpper.StartsWith("RX1") AndAlso oPunch.DateTime.HasValue AndAlso oPunch.DateTime.Value.Year > Now.Year Then
                    Dim isSameDay As Boolean = (oPunch.DateTime.Value.Month = Now.Month AndAlso oPunch.DateTime.Value.Day = Now.Day)
                    Dim dOriginalDateTime As Date = oPunch.DateTime.Value
                    oPunch.DateTime = New DateTime(Now.Year, oPunch.DateTime.Value.Month, oPunch.DateTime.Value.Day, oPunch.DateTime.Value.Hour, oPunch.DateTime.Value.Minute, oPunch.DateTime.Value.Second)
                    oPunch.SystemDetails = $"Punch moved from {dOriginalDateTime.ToString("yyyy-MM-dd HH:mm:ss")} to {oPunch.DateTime.Value.ToString("yyyy-MM-dd HH:mm:ss")}"

                    If isSameDay Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::LoadCurrentData:" + _Terminal.ToString + ":Punch received with future year. Year fixed:" & oPunch.ToLogInfo)
                    Else
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::LoadCurrentData:" + _Terminal.ToString + ":Punch received with future year and different day (Offline). Year fixed:" & oPunch.ToLogInfo)
                    End If
                End If

                oPunch.VerificationType = _VerifyType

                ' Control de temperatura y máscara. Versión 1, Zk no envía la confiuración de alerta e impedimento de acceder, tampoco la temperatura límite
                ' Sólo en el caso en que los valores sean 255, quiere decir que el control correspondiente no está habilitado. En caso contrario recupero configuración que las comunicaciones han guardado en el terminal
                Dim oSecurityOptions As New clsSecurityOptions
                Dim bDiscardPunchBySecurity As Boolean = False
                Try
                    If _Terminal.Model.ToUpper <> eTerminalModel.rx1.ToString.ToUpper AndAlso _Terminal.Model.ToUpper <> eTerminalModel.rxFe.ToString.ToUpper AndAlso _Terminal.Model.ToUpper <> eTerminalModel.rxTe.ToString.ToUpper Then
                        If (_WearingMask <> String.Empty AndAlso _WearingMask <> "255") OrElse _Temperature <> String.Empty AndAlso _Temperature <> "255" Then
                            ' Ha llegado parámetro de máscara o temperatura. Luego alguno está habilitado ...
                            oSecurityOptions = roJSONHelper.DeserializeNewtonSoft(_Terminal.SecurityOptionsDefinition, GetType(clsSecurityOptions))
                            If _Terminal.SecurityOptionsDefinition = String.Empty OrElse oSecurityOptions Is Nothing OrElse (oSecurityOptions.EnalbeMaskDetection = -1) Then
                                ' No he recuperado la configuración de seguridad del terminal
                                ' Creo una por defecto
                                oSecurityOptions = New clsSecurityOptions
                                oSecurityOptions.EnalbeIRTempDetection = If(_Temperature <> String.Empty AndAlso _Temperature <> "255", 1, 0)
                                oSecurityOptions.IRTempLowThreshold = 0
                                oSecurityOptions.IRTempThreshold = 3730
                                oSecurityOptions.EnableNormalIRTempPass = 0 'Quiere decir que se permite el acceso (al revés del mundo)
                                oSecurityOptions.EnalbeMaskDetection = If(_WearingMask <> String.Empty AndAlso _WearingMask <> "255", 1, 0)
                                oSecurityOptions.EnableWearMaskPass = 0 'Quiere decir que se permite el acceso (al revés del mundo)
                                ' No tengo configuración de seguridad en la tabla de terminales. Asumo una por defecto
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Unable to recover security configuration paramterers to check punch. Assuming default values ")
                            End If

                            If oSecurityOptions.EnalbeMaskDetection = 1 AndAlso _WearingMask <> String.Empty AndAlso _WearingMask <> "255" Then
                                oPunch.MaskAlert = (_WearingMask = 0)
                                If oPunch.MaskAlert AndAlso oSecurityOptions.EnableWearMaskPass = 1 Then ' NOTA: Según protocolo, Enable = 1 es igual a Disable !
                                    bDiscardPunchBySecurity = True
                                End If
                            End If

                            If oSecurityOptions.EnalbeIRTempDetection = 1 AndAlso _Temperature <> String.Empty AndAlso _Temperature <> "255" Then
                                oPunch.TemperatureAlert = (roTypes.Any2Double(_Temperature.Replace(".", ",")) >= (roTypes.Any2Long(oSecurityOptions.IRTempThreshold) / 100))
                                If oPunch.TemperatureAlert AndAlso oSecurityOptions.EnableNormalIRTempPass = 1 Then
                                    bDiscardPunchBySecurity = True
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Unable to recover security configuration paramterers to check punch. Detail: " & oPunch.ToLogInfo, ex)
                End Try

                Select Case _MoveType
                    Case eCurrentMoveType.Acc_
                        If ((Me.Terminal.Model.ToUpper = eTerminalModel.rxcep.ToString.ToUpper OrElse Me.Terminal.Model.ToUpper = eTerminalModel.rxcp.ToString.ToUpper) AndAlso _Action = 7) OrElse (Me.Terminal.Model.ToUpper = eTerminalModel.rx1.ToString.ToUpper AndAlso _Action = 252) Then
                            'Acceso denegado (AttendanceStatus = 7 en terminales rxCP y rxCeP, 252 en terminales rx1
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NTIME_
                        Else
                            If bDiscardPunchBySecurity Then
                                oPunch.Type = PunchTypeEnum._AI
                                oPunch.InvalidType = InvalidTypeEnum.NSEC
                            Else
                                ' Acceso válido. Miro si además de accesos tengo que guardar presencia en función del modo configurado para el lector
                                If _Terminal.RDRMode(_Reader).IndexOf("ACCTA") > -1 Then
                                    oPunch.Type = PunchTypeEnum._L
                                    sModeForLog = "accta"
                                Else
                                    oPunch.Type = PunchTypeEnum._AV
                                End If
                            End If
                        End If
                    Case eCurrentMoveType.In_
                        oPunch.Type = PunchTypeEnum._IN
                    Case eCurrentMoveType.Out_
                        oPunch.Type = PunchTypeEnum._OUT
                    Case eCurrentMoveType.InOut_
                        oPunch.Type = PunchTypeEnum._AUTO
                    Case eCurrentMoveType.Center_
                        oPunch.Type = PunchTypeEnum._CENTER
                    Case eCurrentMoveType.Din_
                        oPunch.Type = PunchTypeEnum._DR
                    Case Else
                        roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Unexpected MoveType:" & _MoveType.ToString & ". Assuming _AUTO")
                        oPunch.Type = PunchTypeEnum._AUTO
                End Select
                If _MoveType = eCurrentMoveType.Center_ Then
                    ' Es sólo centros de coste
                    oPunch.TypeData = Me.Terminal.GetCostCenter
                ElseIf _Terminal.RDRMode(1) = "TACO" Then
                    ' Además de un fichaje de Presencia (en el que estoy), debo guardar otro de centros de coste siempre que el lector tenga un centro de coste asignado
                    Dim iCostCenter As Integer = Me.Terminal.GetCostCenter
                    oCostCenterPunch = New Punch.roPunch(oPunch.IDEmployee, -1, oPunchState)
                    oCostCenterPunch.IDTerminal = oPunch.IDTerminal
                    oCostCenterPunch.IDReader = oPunch.IDReader
                    oCostCenterPunch.IDZone = oPunch.IDZone
                    oCostCenterPunch.DateTime = oPunch.DateTime
                    oCostCenterPunch.Type = PunchTypeEnum._CENTER
                    oCostCenterPunch.ActualType = PunchTypeEnum._CENTER
                    oCostCenterPunch.TypeData = iCostCenter
                    oCostCenterPunch.VerificationType = oPunch.VerificationType
                End If

                'No se guarda un fichaje si ya existe otro con la misma hora:min:seg para el mismo empleado y del mismo tipo
                Dim strFilter As String = ""
                strFilter = "IDEmployee = " + Me.IDEmployee.ToString + " AND DateTime = " + roTypes.Any2Time(Me._CurrMoveDateTime).SQLDateTime + " AND Type = " & oPunch.Type
                oExistingPunch = VTBusiness.Punch.roPunch.GetPunches(strFilter, oPunchState)
                If oExistingPunch.Rows.Count > 0 Then
                    ' No guardo
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Punch duplicated. It will not be saved (" + _Card + ";" + _IDEmployee.ToString + ";" + _Terminal.ID.ToString + ";" + _Reader.ToString + ";" + _CurrMoveDateTime.ToShortDateString + " " + _CurrMoveDateTime.ToLongTimeString + ";" + _IDIncidence.ToString + ";" + oPunch.Type.ToString + ";" + oPunch.InvalidType.ToString + ")")
                    Return True
                End If

                Dim customization As String = Common.roBusinessSupport.GetCustomizationCode().ToUpper()
                If roTypes.Any2String(customization) = "OZOPLE" Then
                    ' Para el pozo, los fichajes de accesos de ciertos terminales no se guardan como concedidos, sino como pendientes de confirmar por la señal de cierre del relé que envía el terminal.
                    Dim sACCTerminalsToCheckRelayClose As String = String.Empty
                    If oPunch.Type = PunchTypeEnum._AV Then
                        sACCTerminalsToCheckRelayClose = roTypes.Any2String(roAdvancedParameter.GetAdvancedParameterValue("ACCTerminalsToCheckRelayClose", Nothing))
                        ' 1.- El terminal es uno de los configurados ...
                        If sACCTerminalsToCheckRelayClose.Trim.Length > 0 AndAlso sACCTerminalsToCheckRelayClose.Split(",").Contains(oPunch.IDTerminal.ToString) Then
                            oPunch.Type = PunchTypeEnum._AI
                            oPunch.InvalidType = InvalidTypeEnum.NNC
                        End If
                    End If
                End If

                If bSave Then
                    If oPunch.Save() Then
                        If oCostCenterPunch IsNot Nothing Then
                            If oCostCenterPunch.Save() Then
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Cost Center punch registered " & oCostCenterPunch.ToLogInfo)
                            Else
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Unable to save Cost Center punch " & oCostCenterPunch.ToLogInfo)
                            End If
                        End If

                        roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "clsTerminalPunch::SavePunch::Punch registered" & oPunch.ToLogInfo)
                        Return True
                    Else
                        roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::SavePunch::Error:" + oPunchState.ErrorDetail)
                        Return False
                    End If
                Else
                    Return True
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalPunch::SavePunch::Error:", ex)
                Return False
            Finally
                roTrace.GetInstance.AddTraceEvent("Punch saved")
            End Try
        End Function

        Protected Function GetIDCauseFromReaderInputCode(iID As Integer) As Integer
            Try
                Dim sSQL As String = ""
                Dim iRet As Integer = 0
                sSQL = "@SELECT# ID from Causes  where ReaderInputCode = " & iID.ToString & " and AllowInputFromReader = 1"
                iRet = roTypes.Any2Integer(ExecuteScalar(sSQL))
                ' Verifico permisos del empleado sobre la justificación fichada
                If iRet > 0 Then
                    Dim oCauseState As New VTBusiness.Cause.roCauseState
                    Dim oData As System.Data.DataTable = Nothing
                    oData = VTBusiness.Cause.roCause.GetCausesByEmployeeInputPermissions(Me.IDEmployee, oCauseState)
                    Dim dv As DataView = oData.DefaultView
                    dv.RowFilter = "ID = " & iRet.ToString
                    If dv.ToTable.Rows.Count = 0 Then iRet = 0
                End If
                Return iRet
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Overrides Function ToString() As String
            Try
                Return _Terminal.ID.ToString + ";" + _IDEmployee.ToString + ";" + _Card + ";" + _CurrMoveDateTime.ToString("yyyy-MM-dd HH:mm:ss") + ";" + _Action + ";" + _Incidence.ToString + ";" + _Reader.ToString
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Shared Function SavePhoto(_Photo As Byte(), _PIN As Integer, _DateTime As Date, oLog As roLog) As Boolean
            Dim bRet As Boolean = False
            Try
                Dim oPunchTable As DataTable
                Dim oPunch As Punch.roPunch
                Dim oPunchState As Punch.roPunchState = New Punch.roPunchState(-1)
                Dim strFilter As String = "IDEmployee = " + _PIN.ToString + " AND DateTime = " + roTypes.Any2Time(_DateTime).SQLDateTime
                oPunchTable = Punch.roPunch.GetPunches(strFilter, oPunchState)
                Dim lID As Long
                If oPunchTable IsNot Nothing AndAlso oPunchTable.Rows.Count > 0 Then
                    lID = roTypes.Any2Long(oPunchTable.Rows(0)("ID"))
                    oPunch = New Punch.roPunch(_PIN, lID, oPunchState)
                    oPunch.SetCaptureBytes(_Photo)
                    bRet = oPunch.Save()
                Else
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SavePhoto:No punch with id " & lID.ToString)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SavePhoto:Error:", ex)
                bRet = False
            End Try
            Return bRet
        End Function

    End Class

End Namespace