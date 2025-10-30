Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class TerminalMxS
        Private _ID As Integer = 0
        Private _SN As String = ""
        Private _FirmVersion As Byte = 0
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
        Private _PrinterName As String = ""
        Private _LastStatusUpdate As DateTime = New DateTime(2000, 1, 1)
        Private _TimeZoneName As String
        Private _AutoDayLight As Boolean = True
        Private _CheckAPB As Boolean = False
        Private _APBControlledMinutes As Integer
        Private _APBCourtesySeconds As Integer
        Private _APBControlledZones As String = ""
        Private _Customization As String
        Private _PunchTimeOffset As Integer
        Private _RegistrationCode As String = ""
        Private _RequestdelaySeconds As Integer
        Private _ZKInbioHeartBeatControl As Boolean = True
        Private _DefaultRequestdelaySeconds As Integer
        Private _RebootTime As Nullable(Of DateTime)
        Private _TimeZone As String = "+0100"
        Private _ServerTimeZone As String = "+0100"
        Private _StandardTime As String
        Private _DaylightSavingTime As String
        Private _DaylightSavingTimeOn As Integer
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

        Public Property TimeZone As String
            Get
                Return _TimeZone
            End Get
            Set(value As String)
                _TimeZone = value
            End Set
        End Property

        Public Property ServerTimeZone As String
            Get
                Return _ServerTimeZone
            End Get
            Set(value As String)
                _ServerTimeZone = value
            End Set
        End Property

        Public Property StandardTime As String
            Get
                Return _StandardTime
            End Get
            Set(value As String)
                _StandardTime = value
            End Set
        End Property

        Public Property DaylightSavingTime As String
            Get
                Return _DaylightSavingTime
            End Get
            Set(value As String)
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

        Public Property DLSTMode As Integer
            Get
                Return _DLSTMode
            End Get
            Set(value As Integer)
                _DLSTMode = value
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

        Public ReadOnly Property FirmVersion() As Byte
            Get
                Return _FirmVersion
            End Get
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
                    Return "mxS"
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

        Public ReadOnly Property RDRIsOnlyAccess(ByVal index As Byte) As Boolean
            Get
                Return _RDR(index).Mode = "ACC"
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

        Public Function GetCostCenter() As Integer
            Dim iRet As Integer = 0
            Try
                iRet = roTypes.Any2Double(ExecuteScalar("@SELECT# idCostCenter FROM TerminalReaders WHERE Mode like '%CO%' and ID= " & 1 & " AND IDTerminal = " & Me.ID))
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalLogicZKPush2::GetCostCenter::" + Me.ToString + ":Error:", ex)
            End Try
            Return iRet
        End Function

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

        Public ReadOnly Property PunchTimeOffset() As Integer
            Get
                Return _PunchTimeOffset
            End Get
        End Property

        Public ReadOnly Property ZKInbioHeartBeatControl As Boolean
            Get
                Return _ZKInbioHeartBeatControl
            End Get
        End Property

        Public Property RequestDelaySeconds As Integer
            Get
                Return _RequestdelaySeconds
            End Get
            Set(value As Integer)
                _RequestdelaySeconds = value
            End Set
        End Property

        Public Property DefaultRequestDelaySeconds As Integer
            Get
                Return _DefaultRequestdelaySeconds
            End Get
            Set(value As Integer)
                _DefaultRequestdelaySeconds = value
            End Set
        End Property

        Public ReadOnly Property RebootTime As Nullable(Of DateTime)
            Get
                Return _RebootTime
            End Get
        End Property

        Public Function CurrentDatetime() As Date
            Return _DBTerminal.GetCurrentDateTime
        End Function

        Public Sub New(ByVal pType As String)
            'oLog = Log
            ' oLog.Detail = _Type

            _Type = pType
            _ID = 0
            _Port = 0
            _IP = "0.0.0.0"
            _Language = "ESP"
            _DBTerminal = New Terminal.roTerminal
        End Sub

        Public Sub New(ByVal pType As String, ByRef oTerminal As Terminal.roTerminal)
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
                    Dim Splits As String() = _IP.Split(":")
                    _IP = Splits(Splits.Length - 1)
                    ' oLog.IP = _IP
                End If
                _Port = CType(oSck.RemoteEndPoint, System.Net.IPEndPoint).Port
                ' oLog.Port = _Port
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::ParseSocket:" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Function Exist() As Boolean
            Try
                Dim oterminalstate As New Terminal.roTerminalState
                Return Terminal.roTerminalList.GetTerminalsLive("SerialNumber='" + _SN.ToString + "'", oterminalstate).Rows.Count > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::Exist:" + Me.ToString + ":Error:", ex)
                Return True
            End Try
        End Function

        Public Function RefreshDLSTConfig() As Boolean
            Try
                Dim terminalTimeZoneName As String = roTypes.Any2String(ExecuteScalar($"@SELECT# TimeZoneName FROM Terminals where SerialNumber='{_SN}'"))
                If terminalTimeZoneName.Trim <> String.Empty Then
                    _DBTerminal.TimeZoneName = terminalTimeZoneName
                    Try
                        GetDLSTConfig(Me.DLSTMode, Me.TimeZone, Me.DaylightSavingTimeOn, Me.DaylightSavingTime, Me.StandardTime, Me.ServerTimeZone)
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::RefreshDLSTConfig:" + Me.ToString + ":Error getting DLSTConfigParams:", ex)
                        Return False
                    End Try
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::RefreshDLSTConfig:" + Me.ToString + ":Error:", ex)
                Return False
            End Try
        End Function

        Public Function Load() As Boolean
            Try
                Dim TerminalList As Terminal.roTerminalList = New Terminal.roTerminalList()
                Dim TerminalReader As Terminal.roTerminal.roTerminalReader
                Dim sSQL As String
                Dim sLiveAdvancedParameter As String = ""

                TerminalList.GetTerminals("SerialNumber='" + _SN.ToString + "'")
                If TerminalList.Terminals.Count > 0 Then
                    _DBTerminal = TerminalList.Terminals(0)
                    _ID = _DBTerminal.ID
                    _Model = _DBTerminal.Type

                    Try
                        GetDLSTConfig(Me.DLSTMode, Me.TimeZone, Me.DaylightSavingTimeOn, Me.DaylightSavingTime, Me.StandardTime, Me.ServerTimeZone)
                        Me.LoadRegistrationCode()
                    Catch ex As Exception
                    End Try

                    If _DBTerminal.Type.ToUpper <> "MXS" Then
                        'mdPublic.CreateUserTask(_ID)
                        'oLog.PrepareEvent(CTerminalLogicBase.mxEventID.IdentifiedTerminal, "Se ha encontrado un terminal del tipo '" + _DBTerminal.Type + "' que no es un modelo valido.")
                        'oLog.EventDone("", EventLogEntryType.Error)
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalMxS::Load:Terminal " + _DBTerminal.ID.ToString + " is a type invalid.")
                        Return False
                    End If

                    'Elimina la tarea si se ha creado anteriormente
                    'mdPublic.DelUserTask(_ID)

                    If _DBTerminal.Location <> _IP Then
                        sSQL = "@UPDATE# Terminals SET Location='" + _IP + "' WHERE SerialNumber='" + _SN.ToString + "'"
                        ExecuteSql(sSQL)
                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalMxS::Load:Update IP to terminal " + _DBTerminal.ID.ToString + "(" + _DBTerminal.Location + "->" + _IP + ")")
                    End If

                    Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                    ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                    If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                        _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                    End If
                    Dim oSettings As New roSettings(_RegistryRoot & "Robotics\VisualTime")
                    _Language = oSettings.GetVTSetting(eKeys.DefaultLanguage)
                    _PathFiles = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" + _ID.ToString + "\"

                    If _DBTerminal.ConfData.Length > 0 Then
                        _Config.LoadXMLString(_DBTerminal.ConfData)
                    End If

                    'Miro si hay justificaciones disponibles para mostrar en el terminal
                    sSQL = "@SELECT# isnull(count(*),0) from Causes where [ID]>0 AND AllowInputFromReader = 1"
                    _AllowCauses = (ExecuteScalar(sSQL) > 0)

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

                    ' Código de registro
                    sSQL = "@SELECT# RegistrationCode FROM Terminals WHERE ID = " & _DBTerminal.ID.ToString
                    Me.RegistrationCode = roTypes.Any2String(ExecuteScalar(sSQL))

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
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error checking Antipassback settings. Assuming NO antipassback", ex)
                        End Try
                    End If

                    ' Cargo información sobre personalizaciones
                    Try
                        sLiveAdvancedParameter = ""
                        clsParameters.LoadAdvancedParameter("Customization", sLiveAdvancedParameter)
                        _Customization = sLiveAdvancedParameter
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error checking Customized settings. Assuming no customizations", ex)
                    End Try

                    ' Carga offset para fichajes
                    Try
                        sLiveAdvancedParameter = ""
                        clsParameters.LoadAdvancedParameter("ZKTerminalsPunchTimeOffset", sLiveAdvancedParameter)
                        _PunchTimeOffset = 0
                        ' Formato: idterm1@offset,idterm2@offset,...,idtermN@offset
                        If sLiveAdvancedParameter.Trim.Length > 0 AndAlso sLiveAdvancedParameter.Contains("@") Then
                            For Each param As String In sLiveAdvancedParameter.Split(",")
                                If param.Split("@").Count = 2 AndAlso param.Split("@")(0) = _ID.ToString Then
                                    _PunchTimeOffset = roTypes.Any2Integer(param.Split("@")(1))
                                    Exit For
                                End If
                            Next
                        End If
                        If _PunchTimeOffset > 60 Then _PunchTimeOffset = 60
                    Catch ex As Exception
                        _PunchTimeOffset = 0
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error checking punch datetime offset settings. Assuming no offset", ex)
                    End Try

                    ' Cargo tiempo entre solicitudes
                    Try
                        sLiveAdvancedParameter = ""
                        _RequestdelaySeconds = 30
                        clsParameters.LoadAdvancedParameter("ZKTerminalsRequestDelay", sLiveAdvancedParameter)
                        If sLiveAdvancedParameter.Trim.Length > 0 AndAlso roTypes.Any2Integer(sLiveAdvancedParameter) >= 5 AndAlso roTypes.Any2Integer(sLiveAdvancedParameter) <= 120 Then
                            _RequestdelaySeconds = roTypes.Any2Integer(sLiveAdvancedParameter)
                        End If
                        _DefaultRequestdelaySeconds = _RequestdelaySeconds
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error checking ZKTerminalsRequestDelay settings. Assuming 10 seconds", ex)
                    End Try

                    'Cargo control de heartbeat
                    Try
                        sLiveAdvancedParameter = ""
                        _ZKInbioHeartBeatControl = True
                        clsParameters.LoadAdvancedParameter("ZKInbioHeartBeatControl", sLiveAdvancedParameter)
                        If sLiveAdvancedParameter.Trim <> String.Empty Then _ZKInbioHeartBeatControl = roTypes.Any2Boolean(sLiveAdvancedParameter)
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error checking ZKInbioHeartBeatControl settings. Assuming true", ex)
                    End Try

                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalMxS::Load:" + Me.ToString + ":Error:", ex)
                Return False
            End Try

        End Function

        Public Function CreateNew(Optional iDoorsCount As Integer = 1)
            Try
                _DBTerminal = New Terminal.roTerminal()
                _DBTerminal.ID = Terminal.roTerminal.RetrieveTerminalNextID(_DBTerminal.State)
                _DBTerminal.Behavior = ""
                _DBTerminal.Location = _IP
                _DBTerminal.SerialNumber = _SN
                _DBTerminal.SupportedModes = "ACC,ACCTA"
                'TODO: El número de relés que tiene la centralita viene como parte del mensaje de Registro
                _DBTerminal.SupportedOutputs = iDoorsCount
                _DBTerminal.SupportedSirens = "0"
                _DBTerminal.Type = "mxS"
                _DBTerminal.Other = ""
                _DBTerminal.Description = "Terminal" + _DBTerminal.ID.ToString
                _DBTerminal.Save()

                Dim sSQL As String
                For i As Integer = 1 To iDoorsCount
                    sSQL = "@INSERT# INTO TerminalReaders(IDTerminal,ID,Description,Mode,Type)"
                    sSQL += " VALUES (" + _DBTerminal.ID.ToString + ", " + i.ToString + ", 'Reader " + i.ToString + "', '', 'RDR')"
                    ExecuteSql(sSQL)
                Next

                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::CreateNew::" + Me.ToString + ":Error creating terminal:", ex)
                Return False
            End Try
        End Function

        Public Sub GetDLSTConfig(ByRef iDLSTMode As Integer, ByRef sTimeZone As String, ByRef iDaylightSavingTimeOn As Integer, ByRef sDayLightTime As String, ByRef sStandardTime As String, ByRef sServerTimeZone As String)
            Dim dStandardTime As Date
            Dim dDayLightTime As Date

            Dim oTerminalTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_DBTerminal.TimeZoneName)
            Dim oServerTimeZone As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time")
            iDLSTMode = 1 ' Modo fecha/hora

            sTimeZone = If(oTerminalTimeZone.BaseUtcOffset.TotalMinutes < 0, "-", "+") & Math.Abs(oTerminalTimeZone.BaseUtcOffset.Hours).ToString.PadLeft(2, "0") & oTerminalTimeZone.BaseUtcOffset.Minutes.ToString.PadLeft(2, "0")
            sServerTimeZone = If(oServerTimeZone.BaseUtcOffset.TotalMinutes < 0, "-", "+") & Math.Abs(oServerTimeZone.BaseUtcOffset.Hours).ToString.PadLeft(2, "0") & oServerTimeZone.BaseUtcOffset.Minutes.ToString.PadLeft(2, "0")

            iDaylightSavingTimeOn = 0

            For Each oAR As TimeZoneInfo.AdjustmentRule In oTerminalTimeZone.GetAdjustmentRules
                If oAR.DateEnd > Now.Date AndAlso oAR.DateStart < Now.Date Then
                    dDayLightTime = GetAdjustmentDate(oAR.DaylightTransitionStart, Now.Year)
                    sDayLightTime = ((dDayLightTime.Month.ToString.PadLeft(2, "0")) + (GetWeekOfMonth(dDayLightTime).ToString.PadLeft(2, "0")) + (CInt(dDayLightTime.DayOfWeek).ToString.PadLeft(2, "0")) + dDayLightTime.Hour.ToString.PadLeft(2, "0"))
                    dStandardTime = GetAdjustmentDate(oAR.DaylightTransitionEnd, Now.Year)
                    sStandardTime = ((dStandardTime.Month.ToString.PadLeft(2, "0")) + (GetWeekOfMonth(dStandardTime).ToString.PadLeft(2, "0")) + (CInt(dStandardTime.DayOfWeek).ToString.PadLeft(2, "0")) + dStandardTime.Hour.ToString.PadLeft(2, "0"))
                    iDaylightSavingTimeOn = 1
                    Exit For
                End If
            Next

        End Sub

        Private Function GetWeekOfMonth(dDate As Date) As Integer
            Dim oRes As Integer
            Try
                Dim dfi As Globalization.DateTimeFormatInfo = Globalization.DateTimeFormatInfo.CurrentInfo
                Dim cal As Globalization.Calendar = dfi.Calendar

                Dim date1 As Date = New Date(dDate.Year, dDate.Month, 1) 'first day of a month
                Dim dateChk As Date = dDate.Date 'day of same month to check

                Dim firstWeek As Integer = cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                                         dfi.FirstDayOfWeek)

                Dim whichWeek As Integer = cal.GetWeekOfYear(dateChk, dfi.CalendarWeekRule,
                                                         dfi.FirstDayOfWeek)

                If firstWeek > whichWeek Then firstWeek = 0

                oRes = whichWeek - firstWeek + 1
            Catch ex As Exception

            End Try

            Return oRes
        End Function

        Public Function GetAdjustmentDate(ByVal transitionTime As TimeZoneInfo.TransitionTime, ByVal year As Integer) As DateTime
            If transitionTime.IsFixedDateRule Then
                Return New DateTime(year, transitionTime.Month, transitionTime.Day)
            Else
                Dim cal As System.Globalization.Calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar
                Dim startOfWeek As Integer = transitionTime.Week * 7 - 6
                Dim firstDayOfWeek As Integer = CInt(cal.GetDayOfWeek(New DateTime(year, transitionTime.Month, 1)))
                Dim transitionDay As Integer
                Dim changeDayOfWeek As Integer = CInt(transitionTime.DayOfWeek)

                If firstDayOfWeek <= changeDayOfWeek Then
                    transitionDay = startOfWeek + (changeDayOfWeek - firstDayOfWeek)
                Else
                    transitionDay = startOfWeek + (7 - firstDayOfWeek + changeDayOfWeek)
                End If

                If transitionDay > cal.GetDaysInMonth(year, transitionTime.Month) Then transitionDay -= 7
                Return New DateTime(year, transitionTime.Month, transitionDay, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second)
            End If
        End Function

        Public Function AddReaders(iCurrentReadersCount As Integer, iDoorsCount As Integer)
            Try

                Dim sSQL As String
                For i As Integer = iCurrentReadersCount + 1 To iDoorsCount
                    sSQL = "@INSERT# INTO TerminalReaders(IDTerminal,ID,Description,Mode,Type)"
                    sSQL += " VALUES (" + _DBTerminal.ID.ToString + ", " + i.ToString + ", 'Reader " + i.ToString + "', '', 'RDR')"
                    ExecuteSql(sSQL)
                Next

                CallBroadcaster(_DBTerminal.ID)
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::AddReaders::" + Me.ToString + ":Error creating terminal:", ex)
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
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::LoadRegistrationCode::" + Me.ToString + ":Error getting registration code:", ex)
                Return False
            End Try
        End Function

        Public Function CheckRegistrationCode() As Boolean
            Try
                If Me.RegistrationCode.Trim <> "" Then ' AndAlso Me.RegistrationCode.Length = 10 Then
                    Return Me._DBTerminal.CheckTerminalSerialNum(Me.RegistrationCode, Me.SN)
                Else
                    Return False
                End If
                Return True
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::CheckRegistrationCode::" + Me.ToString + ":Error getting registration code:", ex)
                Return False
            End Try
        End Function

        Public Function ByPassRegitrationCode() As Boolean
            Dim oRet As Boolean = False
            Try
                Dim strRegistrationCode As String = _DBTerminal.GetTerminalRegistrationCode(Me.SN)
                Dim sSQL As String = "@UPDATE# Terminals set RegistrationCode = '" + strRegistrationCode + "' WHERE ID = " + _ID.ToString
                oRet = ExecuteSql(sSQL)
                If oRet Then Me.RegistrationCode = strRegistrationCode
            Catch ex As Exception
                oRet = False
            End Try
            Return oRet
        End Function

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
                sSQL = "@DELETE# TerminalsSyncEmployeeAccessLevelData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
                sSQL = "@DELETE# TerminalsSyncTimeZonesInbioData WHERE TerminalID = " + _ID.ToString
                ExecuteSql(sSQL)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::ForceFullConfig::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateStatus(ByVal Connected As Boolean)
            Try
                _DBTerminal.UpdateStatus(Connected, True)
                _LastStatusUpdate = Now
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateStatus::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateModel(ByVal sModel As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set Model = '" + sModel + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _DBTerminal.Model = sModel
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateModel::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateOther(ByVal sOther As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set Other = '" + sOther + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _DBTerminal.Other = sOther
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateOther::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateFirmVer(ByVal sFirmVer As String)
            Try
                Dim sSQL As String = "@UPDATE# Terminals set FirmVersion = '" + sFirmVer + "' WHERE ID = " + _ID.ToString
                ExecuteSql(sSQL)
                _DBTerminal.FirmVersion = sFirmVer
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateFirmVer::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

        Public Sub UpdateLastAction()
            Try
                _DBTerminal.UpdateLastAction()
                If Now.Subtract(_LastStatusUpdate).TotalMinutes > 2 Then UpdateStatus(True)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateLastAction::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Sub UpdateLastUpdate(Optional bUpdateLastAction As Boolean = True)
            Try
                _DBTerminal.UpdateLastUpdate(bUpdateLastAction)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::UpdateLastUpdate::" + Me.ToString + ":Error:", ex)
            End Try
        End Sub

        Public Overrides Function ToString() As String
            If _Port = 0 Then
                Return ":Terminal " + _ID.ToString + ":" + _Type + ":SN=" + _SN
            Else
                Return ":Terminal " + _ID.ToString + ":" + _Type + ":[" + _IP + "@" + _Port.ToString + "]:"
            End If
        End Function

        Public Sub RestartTerminal()
            Try
                Dim sSQL As String = "@INSERT# INTO TerminalsSyncTasks (IDterminal,Task) values (" & _ID.ToString & ",'reboot')"
                ExecuteSql(sSQL)
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::RestartTerminal::Terminal should restart:: " + Me.ToString)
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "TerminalMxS::RestartTerminal::" + Me.ToString + ":Error:", ex)
            End Try

        End Sub

    End Class

End Namespace