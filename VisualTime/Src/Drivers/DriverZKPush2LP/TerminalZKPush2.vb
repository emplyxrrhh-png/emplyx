Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Comms.Base
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Enum eTerminalModel
        unknown
        rxcp
        rxcep
        rx1
        rxFP
        rxFL
        rxFPTD
        rxFe
        rxTe
    End Enum

    Public Class TerminalZKPush2
        Private _ID As Integer = 0
        Private _SN As String = ""
        Private _FirmVersion As String = ""
        Private _Version As String = ""
        Private _Type As String = ""
        Private _Model As String = ""
        Private _ModelVersion As String = ""
        Private _Port As Integer = 0
        Private _IP As String = ""
        Private _DefInputs As String = ""
        Private _DBTerminal As Terminal.roTerminal
        Private _Language As String = ""
        Private _State As Terminal.roTerminalState
        Private _RDR As Dictionary(Of Byte, Terminal.roTerminal.roTerminalReader) = New Dictionary(Of Byte, Terminal.roTerminal.roTerminalReader)
        Private _RDRConfig As Dictionary(Of Byte, roCollection) = New Dictionary(Of Byte, roCollection)
        Private _RDRWorkingZone As Dictionary(Of Byte, Boolean) = New Dictionary(Of Byte, Boolean)
        Private _Config As roCollection = New roCollection
        Private _Connected As Boolean = False
        Private _PathFiles As String = ""
        Private _AllowCauses As Boolean
        Private _EmployeePermit As List(Of Integer)
        Private _PrinterName As String = ""
        Private _LastStatusUpdate As DateTime = New DateTime(2000, 1, 1)
        Private _TimeZoneName As String
        Private _AutoDayLight As Boolean = True
        Private _CheckAPB As Boolean = False
        Private _APBControlledMinutes As Integer
        Private _APBCourtesySeconds As Integer
        Private _APBControlledZones As String = ""
        Private _Customization As String
        Private _RegistrationCode As String = ""
        Private oLog As roLog
        Private _PunchTimeStamp As String
        Private _OperTimeStamp As String
        Private _IsInDifferentTimeZone As Boolean = False
        Private _IsInDST As Boolean = False
        Private _LastInDSTStatus As Boolean = False
        Private _AdminPWD As String = "9999"
        Private _AdminID As Integer = 9999
        Private _NextRebootTime As DateTime = DateTime.MinValue
        Private _TimeZone As Integer
        Private _StandardTime As Long
        Private _DaylightSavingTime As Long
        Private _DaylightSavingTimeOn As Integer
        Private _RequestDelaySeconds As Integer
        Private _DLSTMode As Integer

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Public Property DBTerminal As Terminal.roTerminal
            Set(value As Terminal.roTerminal)
                _DBTerminal = value
            End Set
            Get
                Return _DBTerminal
            End Get
        End Property

        Public Property SN() As String
            Get
                Return _SN
            End Get
            Set(ByVal value As String)
                _SN = value
            End Set
        End Property

        Public Property RegistrationCode As String
            Get
                Return _RegistrationCode
            End Get
            Set(value As String)
                _RegistrationCode = value
            End Set
        End Property

        Public ReadOnly Property PunchTimeStatmp As String
            Get
                Return _PunchTimeStamp
            End Get
        End Property

        Public Property Other As String
            Get
                Return _DBTerminal.Other
            End Get
            Set(value As String)
                _DBTerminal.Other = value
            End Set
        End Property

        Public Property SecurityOptionsDefinition As String
            Get
                Return _DBTerminal.SecurityOptionsDefinition
            End Get
            Set(value As String)
                _DBTerminal.SecurityOptionsDefinition = value
            End Set
        End Property

        Public ReadOnly Property OperTimeStatmp As String
            Get
                Return _OperTimeStamp
            End Get
        End Property

        Public Property FirmVersion() As String
            Get
                Return _FirmVersion
            End Get
            Set(value As String)
                _FirmVersion = value
            End Set
        End Property

        Public Property IP() As String
            Get
                Return _IP
            End Get
            Set(value As String)
                _IP = value
            End Set
        End Property

        Public Property Port() As Integer
            Get
                Return _Port
            End Get
            Set(value As Integer)
                _Port = value
            End Set
        End Property

        Public ReadOnly Property TerminalType() As String
            Get
                If _Model <> "" Then
                    Return _Model
                Else
                    Return "rxCP"
                End If
            End Get
        End Property

        Public Property Model() As String
            Get
                Return _Model
            End Get
            Set(ByVal value As String)
                _Model = value
            End Set
        End Property

        Public Property ModelVersion() As String
            Get
                Return _ModelVersion
            End Get
            Set(ByVal value As String)
                _ModelVersion = value
            End Set
        End Property

        Public ReadOnly Property Language() As String
            Get
                If _Language.Length > 0 Then
                    Return _Language
                Else
                    Return "ESP"
                End If
            End Get
        End Property

        Public ReadOnly Property PathFiles() As String
            Get
                Return _DBTerminal.Path
            End Get
        End Property

        Public ReadOnly Property RDRCount() As Integer
            Get
                Try
                    Return _DBTerminal.ReadersCount
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRMode(ByVal Index As Byte) As String
            Get
                Try
                    Return _RDR(Index).Mode
                Catch ex As Exception
                    Return ""
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRInteractionAction(ByVal Index As Byte) As Nullable(Of InteractionAction)
            Get
                Try
                    Return _RDR(Index).InteractionAction
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRInteractionMode(ByVal Index As Byte) As Nullable(Of InteractionMode)
            Get
                Try
                    Return _RDR(Index).InteractionMode
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRAllowABK(ByVal Index As Byte) As Boolean
            Get
                Return False '_RDR(Index).UseDispKey
            End Get
        End Property

        Public ReadOnly Property RDRIsOHP(ByVal Index As Byte) As Boolean
            Get
                Return _RDR(Index).OHP
            End Get
        End Property

        Public ReadOnly Property RDRIsAttendance(ByVal index As Byte) As Boolean
            Get
                Return Not (_RDR(index).Mode = "ACC" Or _RDR(index).Mode = "ACCTA")
            End Get
        End Property

        Public ReadOnly Property RDRIsAttendanceEx(ByVal index As Byte) As Boolean
            Get
                Return _RDR(index).Mode.ToUpper.IndexOf("TA") >= 0
            End Get
        End Property

        Public ReadOnly Property RDRAllowMoveChange(ByVal Index As Byte) As Boolean
            Get
                Return Not (_RDR(Index).Mode = "ACC" Or _RDR(Index).Mode = "ACCTA")
            End Get
        End Property

        Public ReadOnly Property RDRAllowJob(ByVal Index As Byte) As Boolean
            Get
                Return _RDR(Index).Mode.ToUpper.IndexOf("JOB") >= 0
            End Get
        End Property

        Public ReadOnly Property RDRAllowProductiv(ByVal Index As Byte) As Boolean
            Get
                Return _RDR(Index).Mode.ToUpper.IndexOf("TSK") >= 0 And clsParameters.AllowProductiv
            End Get
        End Property

        Public ReadOnly Property RDRAllowDiner(ByVal Index As Byte) As Boolean
            Get
                Return _RDR(Index).Mode.ToUpper.IndexOf("DIN") >= 0
            End Get
        End Property

        Public ReadOnly Property RDROpenTime(ByVal Index As Byte) As Integer
            Get
                Try
                    If _RDR(Index).Output <> 0 Then
                        Return _RDR(Index).Duration
                    Else
                        Return 0
                    End If
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRZone(ByVal Index As Byte) As Integer
            Get
                Try
                    Return _RDR(Index).IDZone
                Catch ex As Exception
                    Return 0
                End Try
            End Get
        End Property

        Public ReadOnly Property RDRWorkingZone(ByVal Index As Byte) As Boolean
            Get
                Try
                    Return _RDRWorkingZone(Index)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public ReadOnly Property DefInputs() As String
            Get
                Dim str As String = "KBD,"
                If _RDR(1).Mode <> "NA" And _RDR(1).Mode.Length > 0 Then str += "RDR1,"
                If _RDR(2).Mode <> "NA" And _RDR(2).Mode.Length > 0 Then str += "RDR2"
                Return str
            End Get
        End Property

        Public ReadOnly Property AllowCauses() As Boolean
            'Indica si el terminal permite fichar justificaciones
            Get
                Return _AllowCauses
            End Get
        End Property

        Public ReadOnly Property TimeZoneName() As String
            Get
                Return _TimeZoneName
            End Get
        End Property

        Public ReadOnly Property AutoDayLight() As Boolean
            Get
                Return _AutoDayLight
            End Get
        End Property

        Public ReadOnly Property CheckAPB() As Boolean
            Get
                Return _CheckAPB
            End Get
        End Property

        Public ReadOnly Property APBControlledMinutes() As Integer
            Get
                Return _APBControlledMinutes
            End Get
        End Property

        Public ReadOnly Property APBCourtesySeconds() As Integer
            Get
                Return _APBCourtesySeconds
            End Get
        End Property

        Public ReadOnly Property APBControlledZones() As String
            Get
                Return _APBControlledZones
            End Get
        End Property

        Public Property Customization() As String
            Get
                Return _Customization
            End Get
            Set(ByVal value As String)

            End Set
        End Property

        Public ReadOnly Property LastAction As Date
            Get
                Return _DBTerminal.LastAction
            End Get
        End Property

        Public ReadOnly Property IsInDifferentTimeZone As Boolean
            Get
                Return _IsInDifferentTimeZone '_DBTerminal.IsDifferentZoneTime
            End Get
        End Property

        Public ReadOnly Property IsInDST As Boolean
            Get
                Return _IsInDST '_DBTerminal.IsDifferentZoneTime
            End Get
        End Property

        Public Property LastInDSTStatus As Boolean
            Get
                Return _LastInDSTStatus
            End Get
            Set(value As Boolean)
                _LastInDSTStatus = value
            End Set
        End Property

        Public Property AdminID As Integer
            Get
                Return _AdminID
            End Get
            Set(value As Integer)
                _AdminID = value
            End Set
        End Property

        Public Property AdminPWD As String
            Get
                Return _AdminPWD
            End Get
            Set(value As String)
                _AdminPWD = value
            End Set
        End Property

        Public Property TimeZone As Integer
            Get
                Return _TimeZone
            End Get
            Set(value As Integer)
                _TimeZone = value
            End Set
        End Property

        Public Property StandardTime As Long
            Get
                Return _StandardTime
            End Get
            Set(value As Long)
                _StandardTime = value
            End Set
        End Property

        Public Property DaylightSavingTime As Long
            Get
                Return _DaylightSavingTime
            End Get
            Set(value As Long)
                _DaylightSavingTime = value
            End Set
        End Property

        Public Property DaylightSavingTimeOn As Integer
            Get
                Return _DaylightSavingTimeOn
            End Get
            Set(value As Integer)
                _DaylightSavingTimeOn = value
            End Set
        End Property

        Public Property RequestDelaySeconds As Integer
            Get
                Return _RequestDelaySeconds
            End Get
            Set(value As Integer)
                _RequestDelaySeconds = value
            End Set
        End Property

        Public Property DLSTMode As Integer
            Get
                Return _DLSTMode
            End Get
            Set(value As Integer)
                _DLSTMode = value
            End Set
        End Property

        Public Property NextRebootTime As DateTime
            Get
                Return _NextRebootTime
            End Get
            Set(value As DateTime)
                _NextRebootTime = value
            End Set
        End Property

        Public Function EmployeeHasPermit(ByVal IDEmployee As Integer) As Boolean
            Try
                Return _DBTerminal.ReaderByID(1).EmployeePermit(IDEmployee, True)
            Catch ex As Exception
                Return True
            End Try
        End Function

        Public Function CurrentDatetime() As Date
            Return _DBTerminal.GetCurrentDateTimeEx(_IsInDST)
        End Function


        Public Sub New(ByVal pType As String, ByRef oTerminal As Terminal.roTerminal, ByRef Log As roLog)
            oLog = Log

            _Type = pType
            _ID = 0
            _Port = 0
            _IP = "0.0.0.0"
            _Language = "ESP"
            _DBTerminal = oTerminal
        End Sub

        Public Function ModeDebugInfo() As String
            Return ":Terminal " + _ID.ToString + ":" + _Type.ToString + ":[" + _IP + "@" + _Port.ToString + "]"
        End Function

        Public Sub ParseSocket(ByVal oSck As System.Net.Sockets.Socket)
            Try
                Dim oIPAddress As System.Net.IPAddress = System.Net.IPAddress.Parse(CType(oSck.RemoteEndPoint, System.Net.IPEndPoint).Address.ToString())
                _IP = oIPAddress.ToString
                If _IP.IndexOf(":") >= 0 Then
                    Dim Splits() As String = _IP.Split(":")
                    _IP = Splits(Splits.Length - 1)
                End If
                _Port = CType(oSck.RemoteEndPoint, System.Net.IPEndPoint).Port
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::ParseSocket:" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Function Exist() As Boolean
            Try
                Dim oterminalstate As New Terminal.roTerminalState
                Return Terminal.roTerminalList.GetTerminalsLive("SerialNumber='" + _SN.ToString + "'", oterminalstate).Rows.Count > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::Exist:" + Me.ToString + ":Error:", ex)
                Return True
            End Try
        End Function

        Public Function Load() As Boolean
            Try
                Dim TerminalList As Terminal.roTerminalList = New Terminal.roTerminalList()
                Dim TerminalReader As Terminal.roTerminal.roTerminalReader
                Dim sSQL As String
                Dim sLiveAdvancedParameter As String = ""

                roTrace.GetInstance.InitTraceEvent()

                TerminalList.GetTerminals("SerialNumber='" + _SN.ToString + "'")
                If TerminalList.Terminals.Count > 0 Then
                    _DBTerminal = TerminalList.Terminals(0)
                    _ID = _DBTerminal.ID
                    _Model = _DBTerminal.Type
                    _IsInDifferentTimeZone = _DBTerminal.IsDifferentZoneTime
                    Try
                        GetDLSTConfig(Me.DLSTMode, Me.TimeZone, Me.DaylightSavingTimeOn, Me.DaylightSavingTime, Me.StandardTime)
                        Me.LoadRegistrationCode()
                    Catch ex As Exception
                        'do nothing
                    End Try

                    _FirmVersion = _DBTerminal.FirmVersion
                    If _DBTerminal.Type.ToUpper <> Me.TerminalType.ToUpper Then
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::Load:Terminal " + _DBTerminal.ID.ToString + " is an invalid type")
                        Return False
                    End If

                    'Elimina la tarea si se ha creado anteriormente
                    'mdPublic.DelUserTask(_ID)

                    If _DBTerminal.Location <> _IP Then
                        sSQL = $"@UPDATE# Terminals SET Location='{_IP}' WHERE SerialNumber='{_SN}'"
                        ExecuteSql(sSQL)
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalLogicZKPush2::Load:Update IP to terminal " + _DBTerminal.ID.ToString + "(" + _DBTerminal.Location + "->" + _IP + ")")
                    End If

                    If _DBTerminal.ConfData.Length > 0 Then
                        _Config.LoadXMLString(_DBTerminal.ConfData)
                    End If

                    'Miro si hay justificaciones disponibles para mostrar en el terminal
                    sSQL = "@SELECT# isnull(count(*),0) from Causes where [ID]>0 AND AllowInputFromReader = 1"
                    _AllowCauses = (ExecuteScalar(sSQL) > 0)

                    'PWD de administrador
                    clsParameters.LoadAdvancedParameter("PushTerminalsAdminPWD", sLiveAdvancedParameter)
                    _AdminPWD = roTypes.Any2String(sLiveAdvancedParameter)
                    If Not (_AdminPWD.Trim.Length = 4 AndAlso IsNumeric(_AdminPWD)) Then _AdminPWD = "9999"

                    'Tiempo entre peticiones
                    _RequestDelaySeconds = mdPublic.GetZKPUSHTerminalsRequestDelay

                    'ID de administrador
                    _AdminID = 9999
                    'Si existe el empleado con ID 9999, asignamos el 999999
                    sSQL = "@SELECT# COUNT(*) FROM Employees WHERE [ID] = 9999"
                    If (roTypes.Any2Integer(ExecuteScalar(sSQL)) > 0) Then _AdminID = 99999

                    _RDR.Clear()
                    _RDRWorkingZone.Clear()
                    _RDRConfig.Clear()
                    If _DBTerminal.ReadersCount > 0 Then
                        For i As Byte = 0 To _DBTerminal.ReadersCount - 1
                            TerminalReader = _DBTerminal.LoadReaderByIndex(i)
                            _RDR.Add(TerminalReader.ID, TerminalReader)
                            If TerminalReader.IDZone IsNot Nothing Then
                                Dim ostate As Zone.roZoneState = New Zone.roZoneState
                                Dim ozone As Zone.roZone
                                ozone = New Zone.roZone(TerminalReader.IDZone, ostate)
                                _RDRWorkingZone.Add(TerminalReader.ID, ozone.IsWorkingZone)
                            Else
                                _RDRWorkingZone.Add(TerminalReader.ID, True)
                            End If
                            _RDRConfig.Add(TerminalReader.ID, New roCollection(TerminalReader.InteractiveConfig.GetXml()))
                        Next
                    End If

                    ' Varios parámetros
                    Dim sql As String = String.Empty
                    sql = $"@SELECT# RegistrationCode, PunchStamp, OperStamp, Other FROM Terminals WHERE ID = {_DBTerminal.ID}"
                    Dim table As DataTable = CreateDataTable(sql)
                    If table IsNot Nothing AndAlso table.Rows.Count > 0 Then
                        ' Código de registro
                        Me.RegistrationCode = roTypes.Any2String(table.Rows(0)("RegistrationCode"))
                        'Timestamp de fichajes y operaciones
                        Me._PunchTimeStamp = roTypes.Any2String(table.Rows(0)("PunchStamp"))
                        Me._OperTimeStamp = roTypes.Any2String(table.Rows(0)("OperStamp"))
                        'Estado de DST
                        Try
                            Dim sAux As String = ""
                            sAux = roTypes.Any2String(table.Rows(0)("Other"))
                            _DBTerminal.Other = sAux
                            Me.LastInDSTStatus = sAux.ToUpper.Contains("INDST:TRUE") OrElse sAux.ToUpper.Contains("INDST=TRUE")
                            'Fuerzo actualización de propiedad IsINDST
                            Me.CurrentDatetime()
                        Catch ex As Exception
                            Me.LastInDSTStatus = False
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Load:" + Me.ToString + ":Error recovering DST Status:", ex)
                        End Try
                    End If

                    ' Configuración horaria
                    _TimeZoneName = _DBTerminal.TimeZoneName
                    If _DBTerminal.IsDifferentZoneTime Then
                        _AutoDayLight = _DBTerminal.AutoDaylight
                    Else
                        _AutoDayLight = True
                    End If

                    ' Version del modelo
                    _ModelVersion = _DBTerminal.Model

                    ' Antipassback
                    _APBControlledZones = ""
                    _CheckAPB = False
                    If Me.RDRMode(1).IndexOf("ACC") > -1 Then
                        ' Aquí verifico si la zona que controla tiene activado control de antipassback
                        Try
                            ' El APB se aplica por grupos de zona. Una zona está en un único grupo
                            ' La cadena que contiene las zonas controladas de APB tiene el siguiente formato:
                            '           grupo1@grupo2@grupo3
                            ' y cada grupo tiene el formato id1,id2,id3,...
                            ' Ejemplo: 1,2@3,4@5,6
                            Dim sAPBZoneGroups As String = ""
                            clsParameters.LoadAdvancedParameter("APBControlledZones", sAPBZoneGroups)

                            ' Si hay varios grupos, los recorro en busca de uno que contenga la zona asignada al terminal
                            For i = 1 To sAPBZoneGroups.Split("@").Count
                                If System.Array.IndexOf(sAPBZoneGroups.Split("@")(i - 1).Split(","), roTypes.Any2String(RDRZone(1))) > -1 Then
                                    _APBControlledZones = sAPBZoneGroups.Split("@")(i - 1)
                                    _CheckAPB = True
                                    Exit For
                                End If
                            Next

                            clsParameters.LoadAdvancedParameter("APBControlledMinutes", sLiveAdvancedParameter)
                            _APBControlledMinutes = roTypes.Any2Integer(sLiveAdvancedParameter)
                            If _APBControlledMinutes = 0 Then _APBControlledMinutes = 480
                            clsParameters.LoadAdvancedParameter("APBCourtesyTime", sLiveAdvancedParameter)
                            _APBCourtesySeconds = roTypes.Any2Integer(sLiveAdvancedParameter)
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Load:" + Me.ToString + ":Error checking Antipassback settings. Assuming NO antipassback", ex)
                        End Try
                    End If

                    ' Cargo información sobre personalizaciones
                    Try
                        sLiveAdvancedParameter = ""
                        clsParameters.LoadAdvancedParameter("Customization", sLiveAdvancedParameter)
                        _Customization = sLiveAdvancedParameter
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Load:" + Me.ToString + ":Error checking Customized settings. Assuming no customizations", ex)
                    End Try

                    ' Cargo hora de reinicio diario
                    Try
                        sLiveAdvancedParameter = ""
                        clsParameters.LoadAdvancedParameter("Terminals.rx1.DailyRebootTime", sLiveAdvancedParameter)
                        If IsDate(sLiveAdvancedParameter) Then
                            _NextRebootTime = New Date(Now.Year, Now.Month, Now.Day, roTypes.Any2DateTime(sLiveAdvancedParameter).Hour, roTypes.Any2DateTime(sLiveAdvancedParameter).Minute, 0)
                            If Now.Subtract(_NextRebootTime).TotalSeconds > 5 Then _NextRebootTime = _NextRebootTime.AddDays(1)
                        End If
                    Catch ex As Exception
                        _NextRebootTime = DateTime.MinValue
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Load:" + Me.ToString + ":Error checking rx1 DailyRebootTime settings. Assuming no reboot", ex)
                    End Try

                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::Load:" + Me.ToString + ":Error:", ex)
                Return False
            Finally
                roTrace.GetInstance.AddTraceEvent("TerminalZKPUSH loaded")
            End Try

        End Function

        Public Function CreateNew(Optional iDoorsCount As Integer = 1) As Boolean
            Try
                _DBTerminal = New Terminal.roTerminal()
                _DBTerminal.ID = Terminal.roTerminal.RetrieveTerminalNextID(_DBTerminal.State)
                _DBTerminal.Behavior = ""
                _DBTerminal.Location = _IP
                _DBTerminal.SerialNumber = _SN
                _DBTerminal.SupportedModes = "TA,ACC,ACCTA"
                _DBTerminal.SupportedOutputs = iDoorsCount
                _DBTerminal.SupportedSirens = "0"
                _DBTerminal.Type = Me.TerminalType '"rxCP"
                _DBTerminal.Other = ""
                _DBTerminal.Description = "Terminal" + _DBTerminal.ID.ToString
                _DBTerminal.TimeZoneName = "Romance Standard Time"
                _DBTerminal.IsDifferentZoneTime = False
                _DBTerminal.AutoDaylight = True
                _DBTerminal.ZoneTime = 0
                _DBTerminal.RegistrationCode = _DBTerminal.GetTerminalRegistrationCode(_SN)
                _DBTerminal.Save()

                Me.ID = _DBTerminal.ID

                Dim sSQL As String
                For i As Integer = 1 To iDoorsCount
                    sSQL = "@INSERT# INTO TerminalReaders(IDTerminal,ID,Description,Mode,Type)"
                    'sSQL += " VALUES (" + _DBTerminal.ID.ToString + ", " + i.ToString + ", 'Reader " + i.ToString + "', 'TA', 'RDR')"
                    sSQL += " VALUES (" + _DBTerminal.ID.ToString + ", " + i.ToString + ", 'Reader " + i.ToString + "', '', 'RDR')"
                    ExecuteSql(sSQL)
                Next

                ' Finalmente, y dado que es un terminal nuevo y para evitar que caigan fichajes y logs que pudieran existir (si por ejemplo viene de  un RMA mal gestionado ....)
                Me.UpdateAttlogStamp(Helper.Normal2ZKDatetime(Now))
                Me.UpdateOperStamp(Helper.Normal2ZKDatetime(Now))

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalZKPush2::CreateNew:Terminal created on VisualTime, with ID " & _ID.ToString & " waiting to be registered")

                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::CreateNew::" + Me.ToString + ":Error creating terminal:", ex)
                Return False
            End Try
        End Function

        Public Sub GetDLSTConfig(ByRef iDLSTMode As Integer, ByRef iTimeZone As Integer, ByRef iDaylightSavingTimeOn As Integer, ByRef lDayLightTime As Long, ByRef lStandardTime As Long)
            Dim dStandardTime As Date
            Dim dDayLightTime As Date

            Dim oTerminalTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_DBTerminal.TimeZoneName)
            iDLSTMode = 0 ' Modo fecha/hora

            ' Diferente cálculo según el modelo de terminal (gracias ZK)
            Select Case Me.Model.ToUpper
                Case eTerminalModel.rx1.ToString.ToUpper
                    iTimeZone = oTerminalTimeZone.BaseUtcOffset.TotalMinutes.ToString
                Case eTerminalModel.rxFP.ToString.ToUpper, eTerminalModel.rxFL.ToString.ToUpper, eTerminalModel.rxFPTD.ToString.ToUpper, eTerminalModel.rxFe.ToString.ToUpper, eTerminalModel.rxTe.ToString.ToUpper
                    If oTerminalTimeZone.BaseUtcOffset.TotalMinutes Mod 60 = 0 Then
                        iTimeZone = oTerminalTimeZone.BaseUtcOffset.TotalMinutes / 60
                    Else
                        iTimeZone = oTerminalTimeZone.BaseUtcOffset.TotalMinutes.ToString
                    End If
            End Select

            iDaylightSavingTimeOn = 0

            For Each oAR As TimeZoneInfo.AdjustmentRule In oTerminalTimeZone.GetAdjustmentRules
                If oAR.DateEnd > Now.Date And oAR.DateStart < Now.Date Then
                    dDayLightTime = Helper.GetAdjustmentDate(oAR.DaylightTransitionStart, Now.Year)
                    lDayLightTime = ((dDayLightTime.Month << 24) + (dDayLightTime.Day << 16) + (dDayLightTime.Hour << 8) + dDayLightTime.Minute)
                    dStandardTime = Helper.GetAdjustmentDate(oAR.DaylightTransitionEnd, Now.Year)
                    lStandardTime = ((dStandardTime.Month << 24) + (dStandardTime.Day << 16) + (dStandardTime.Hour << 8) + dStandardTime.Minute)
                    iDaylightSavingTimeOn = 1
                    Exit For
                End If
            Next

        End Sub

        Public Function AddReaders(iCurrentReadersCount As Integer, iDoorsCount As Integer)
            Try

                Dim sSQL As String
                For i As Integer = iCurrentReadersCount + 1 To iDoorsCount
                    sSQL = "@INSERT# INTO TerminalReaders(IDTerminal,ID,Description,Mode,Type)"
                    sSQL += " VALUES (" + _DBTerminal.ID.ToString + ", " + i.ToString + ", 'Reader " + i.ToString + "', '', 'RDR')"
                    ExecuteSql(sSQL)
                Next

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalZKPush2::AddReaders:Readers added to terminal ID " & _ID.ToString)

                CallBroadcaster(_DBTerminal.ID)
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::AddReaders::" + Me.ToString + ":Error creating terminal:", ex)
                Return False
            End Try
        End Function

        Public Function LoadRegistrationCode() As Boolean
            Try
                Dim sSQL As String
                sSQL = "@SELECT# RegistrationCode FROM Terminals WHERE ID = " & _DBTerminal.ID.ToString
                Me.RegistrationCode = roTypes.Any2String(ExecuteScalar(sSQL))
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::LoadRegistrationCode::" + Me.ToString + ":Error getting registration code:", ex)
                Return False
            End Try
        End Function

        Public Sub UpdateStatus(ByVal Connected As Boolean)
            Try
                _DBTerminal.UpdateStatus(Connected, True)
                _LastStatusUpdate = Now
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateStatus::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateFirmVersion(ByVal sVersion As String)
            Try
                Me.FirmVersion = sVersion
                _DBTerminal.UpdateFirmVersion(sVersion)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateFirmVersion::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Function UpdateDSTState(ByVal bInDST As Boolean) As Boolean
            Dim oret As Boolean = False
            Try
                Dim sSQL As String = String.Empty
                Me.Other = Me.Other.Replace("InDST=", "InDST:") 'Compatibilidad versiones antiguas
                If Me.Other.ToUpper.Contains("INDST:") Then
                    If bInDST Then
                        Me.Other = Me.Other.Replace("InDST:False", "InDST:True")
                    Else
                        Me.Other = Me.Other.Replace("InDST:True", "InDST:False")
                    End If
                Else
                    ' Aún no estaba informado
                    If Me.Other.Length > 0 Then
                        Me.Other = "InDST:" & bInDST.ToString & ";" & Me.Other
                    Else
                        Me.Other = "InDST:" & bInDST.ToString
                    End If
                End If
                sSQL = "@UPDATE# Terminals set Other = '" & Me.Other & "' WHERE ID = " + _ID.ToString
                oret = ExecuteSql(sSQL)
                If oret Then Me.LastInDSTStatus = bInDST
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateDSTState::" + Me.ToString + ":Error:", ex)
            End Try
            Return oret
        End Function

        Public Sub UpdateUserCount(ByVal sUserCount As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set UserCount = '" + sUserCount + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateUserCount::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateFaceCount(ByVal sFaceCount As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set FaceCount = '" + sFaceCount + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateFaceCount::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdatePalmCount(ByVal sPalmCount As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set PalmCount = '" + sPalmCount + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdatePalmCount::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateFingerCount(ByVal sFingerCount As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set FingerCOunt = '" + sFingerCount + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateFingerCount::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateWifiOn(ByVal sWifiOn As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set Wifi = '" + sWifiOn + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateWifiOn::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateAttlogStamp(ByVal sStamp As String)

            Try

                Dim sSQL As String = "@UPDATE# Terminals set PunchStamp = '" + sStamp + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _PunchTimeStamp = sStamp
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateAttlogStamp::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateOperStamp(ByVal sStamp As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set OperStamp = '" + sStamp + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _OperTimeStamp = sStamp
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateOther::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateOther(ByVal sOther As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set Other = '" + sOther + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _DBTerminal.Other = sOther
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateOther::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateSecurityOptions(ByVal strSecurityOptionsJSON As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set SecurityOptions = '" + strSecurityOptionsJSON + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _DBTerminal.SecurityOptionsDefinition = strSecurityOptionsJSON
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateSecurityOptions::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub SaveShellCommandResult(ByVal idTerminal As Integer, ByVal sTask As String, ByVal sShellCommand As String, ByVal dTaskSent As Date, ByVal sShellOut As String)
            Try
                Dim sSQL As String = $"@INSERT# INTO [dbo].[TerminalsShellCommandsResult]
                                        ([IDTerminal]
                                        ,[Task]
                                        ,[TaskData]
                                        ,[TaskSent]
                                        ,[ShellOut])
                                    VALUES
                                        ({idTerminal},'{sTask}','{sShellCommand.Replace("'", "''")}', {roTypes.Any2Time(dTaskSent).SQLDateTime},'{sShellOut}')"
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateSecurityOptions::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateLastUpdate(Optional bUpdateLastAction As Boolean = True)
            Try
                _DBTerminal.UpdateLastUpdate(bUpdateLastAction)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::UpdateLastUpdate::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Sincronización de cero para terminales PUSH con persistencia en BBDD
        ''' </summary>
        Public Sub ForceFullDataSync()
            Try
                Dim sSQL As String
                sSQL = "@DELETE# TerminalsSyncBiometricData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncCardsData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncEmployeesData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncConfigData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncSirensData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncCausesData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncPushTimeZonesData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncPushEmployeeTimeZonesData WHERE TerminalID = " + _ID.ToString

                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::ForceFullConfig::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub DeleteEmployeeOnterminal(ByVal idEmployee As Integer)
            Try
                Dim sSQL As String
                If idEmployee = 0 Then Exit Sub
                sSQL = "@DELETE# TerminalsSyncBiometricData WHERE EmployeeID = " + idEmployee.ToString + " AND TerminalID = " + _ID.ToString
                sSQL += ";"
                sSQL += "@DELETE# TerminalsSyncCardsData WHERE EmployeeID = " + idEmployee.ToString + " AND TerminalID = " + _ID.ToString
                sSQL += ";"
                sSQL += "@DELETE# TerminalsSyncEmployeesData WHERE EmployeeID = " + idEmployee.ToString + " AND TerminalID = " + _ID.ToString
                sSQL += ";"
                sSQL += "@DELETE# TerminalsSyncPushEmployeeTimeZonesData WHERE IDEmployee = " + idEmployee.ToString + " AND TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                ' Si alguna tabla quedó vacía, inserto la marca que indica que el terminal está vacío (para que no se fuerce programación de cero). Gracias Nestor por no hacer ni p... caso
                sSQL = "IF NOT EXISTS(@SELECT# * FROM TerminalsSyncBiometricData WHERE TerminalID = " + _ID.ToString + ") @INSERT# INTO TerminalsSyncBiometricData  (TerminalId, TimeStamp) VALUES (" + _ID.ToString + ",getdate())"
                sSQL += ";"
                sSQL += "IF NOT EXISTS(@SELECT# * FROM TerminalsSyncCardsData WHERE TerminalID = " + _ID.ToString + ") @INSERT# INTO TerminalsSyncCardsData  (TerminalId, TimeStamp) VALUES (" + _ID.ToString + ",getdate())"
                sSQL += ";"
                sSQL += "IF NOT EXISTS(@SELECT# * FROM TerminalsSyncEmployeesData WHERE TerminalID = " + _ID.ToString + ") @INSERT# INTO TerminalsSyncEmployeesData  (TerminalId, TimeStamp) VALUES (" + _ID.ToString + ",getdate())"
                sSQL += ";"
                sSQL += "IF NOT EXISTS(@SELECT# * FROM TerminalsSyncPushEmployeeTimeZonesData WHERE TerminalID = " + _ID.ToString + ") @INSERT# INTO TerminalsSyncPushEmployeeTimeZonesData  (TerminalId, TimeStamp) VALUES (" + _ID.ToString + ",getdate())"
                sSQL += ";"
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::DeleteEmployeeOnterminal::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Overrides Function ToString() As String
            If _Port = 0 Then
                Return ":Terminal " + _ID.ToString + ":" + _DBTerminal.Type + ":SN=" + _SN
            Else
                Return ":Terminal " + _ID.ToString + ":" + Me.TerminalType + ":SN=" + _SN + ":[" + _IP + "@" + _Port.ToString + "]:"
            End If
        End Function

        Public Sub DeleteTimeZoneFile()
            Try
                'Funciono por tablas
                Dim sql As String = $"@DELETE# FROM TerminalsSyncPushTimeZonesData WHERE TerminalId = {Me.ID}"
                Try
                    AccessHelper.ExecuteSql(sql)
                Catch Ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalLogicZKPush2::DeleteTimeZoneFile: Unexpected error: ", Ex)
                End Try
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::DeleteTimeZoneFile:Terminal " & Me.ID & ":", ex)
            End Try
        End Sub

        Public Function GetCostCenter() As Integer
            Dim iRet As Integer = 0
            Try
                iRet = roTypes.Any2Double(ExecuteScalar("@SELECT# idCostCenter FROM TerminalReaders WHERE Mode like '%CO%' and ID= " & 1 & " AND IDTerminal = " & Me.ID))
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::GetCostCenter::" + Me.ToString + ":Error:", ex)
            End Try
            Return iRet
        End Function

    End Class

End Namespace