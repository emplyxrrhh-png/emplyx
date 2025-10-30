Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace BusinesProtocol

    Public Class MessageMxS
        Inherits Robotics.Comms.Base.CMessageBase

        ' Cabecera
        Public Const FIRST_CONNECT_URL As String = "GET /iclock/cdata"
        Public Const REGISTRY_URL As String = "POST /iclock/registry"
        Public Const GET_PUSH_INFO_URL As String = "GET /iclock/push"
        Public Const SYNC_TIME As String = "GET /iclock/rtdata"

        ' Handshake
        Public Const REQUEST_DATA As String = "GET /iclock/getrequest"
        Public Const UPLOAD_URL As String = "POST /iclock/cdata"
        Public Const QUERY_UPLOAD_URL As String = "POST /iclock/querydata"

        ' For remote verify
        Public Const AUTH_VERIFY_URL As String = "AuthType="

        ' Cabecera para resultado de comandos
        Public Const REPONSE_RESULT As String = "POST /iclock/devicecmd"

        ' Cabecera para actualización de firmware
        Public Const DOWN_LOAD_FILE_URL As String = "GET /iclock/file"

        ' Caracteres especiales protocolo PUSH
        Public Const REQ_LINE_PARAM_SPLIT_STR As String = "?"
        Public Const HEADER_END_STR As String = vbCrLf + vbCrLf '"\r\n\r\n"
        Public Const HEADER_SPLIT_STR As String = vbCrLf '"\r\n"
        Public Const STRING_ENCODING_FMT As String = "UTF-8"

        ' Info logs
        Public Const chrBlocks = "|"
        Public Const chrpreValues = "="
        Public Const chrEnd = "~"
        Public Const prtCMD = "CMD"
        Public Const prtSN = "SN"
        Public Const prtTOKEN = "TOKEN"
        Public Const prtTABLE = "TABLE"
        Public Const prtDATA = "DATA"

        Public Enum msgCommand
            init    'Primer mensaje enviado por la centralita
            register    'Mensaje de registro
            getpushconfig   'Solicitud de parámetros de configuración
            logevent  'Fichaje
            state  'Estado
            queryresult   'Resultado de una solicitud de datos por parte del servidor
            getrequest   'Solicitud de comandos de configuración
            command
            commandresult   'Resultado de comando
            upfile  'FaseII: subida de fichero
            downfile  'FaseII: descarga de fichero
            clear  'FaseII: borrado
            none  'FaseII: subida de fichero
            reboot  'FaseII: reinicio de terminal
            genericresponse
            synctime
        End Enum

        Public Enum msgTables
            user 'información básica de usuario
            userauthorize 'autorizaciones de usuarios
            holiday 'festivos
            timezone 'periodos de accesos
            transaction 'fichajes
            firstcard 'funcionalidad avanzada FirstCard
            multicard 'funcionalidad avanzada MultiCard
            inoutfun 'Funcionalidad avanzada de Linkage (triggers)
            templatev10 'Biometría v10
            commands 'Comandos a enviar a la centralita
            none
            mulcarduser
            extuser
            DSTSetting
        End Enum

        Private mID As Integer
        Private mCommand As msgCommand = msgCommand.none
        Private mTable As msgTables = msgTables.none
        Private mData As MsgData
        Private bCorrect As Boolean
        Private bReceived As Boolean
        Private mLastMessageReceived As String

        Public ReadOnly Property SN As String
            Get
                Return mData.SN
            End Get
        End Property

        Public ReadOnly Property Token As String
            Get
                Return mData.Token
            End Get
        End Property

        Public Overrides ReadOnly Property IsCorrect() As Boolean
            Get
                Return bCorrect
            End Get
        End Property

        Public ReadOnly Property ID() As Integer
            Get
                Return mID
            End Get
        End Property

        Public ReadOnly Property Command() As msgCommand
            Get
                Return mCommand
            End Get
        End Property

        Public ReadOnly Property Table() As msgTables
            Get
                Return mTable
            End Get
        End Property

        Public Property data_init() As MsgData_Init
            Get
                Return CType(mData, MsgData_Init)
            End Get
            Set(ByVal value As MsgData_Init)
                mData = value
            End Set
        End Property

        Public Property data_initresponse() As MsgData_InitResponse
            Get
                Return CType(mData, MsgData_InitResponse)
            End Get
            Set(ByVal value As MsgData_InitResponse)
                mData = value
            End Set
        End Property

        Public Property data_register() As MsgData_Register
            Get
                Return CType(mData, MsgData_Register)
            End Get
            Set(ByVal value As MsgData_Register)
                mData = value
            End Set
        End Property

        Public Property data_registerresponse() As MsgData_RegisterResponse
            Get
                Return CType(mData, MsgData_RegisterResponse)
            End Get
            Set(ByVal value As MsgData_RegisterResponse)
                mData = value
            End Set
        End Property

        Public Property data_synctime() As MsgData_SyncTime
            Get
                Return CType(mData, MsgData_SyncTime)
            End Get
            Set(ByVal value As MsgData_SyncTime)
                mData = value
            End Set
        End Property

        Public Property data_synctimeresponse() As MsgData_SyncTimeResponse
            Get
                Return CType(mData, MsgData_SyncTimeResponse)
            End Get
            Set(ByVal value As MsgData_SyncTimeResponse)
                mData = value
            End Set
        End Property

        Public Property data_config() As MsgData_Config
            Get
                Return CType(mData, MsgData_Config)
            End Get
            Set(ByVal value As MsgData_Config)
                mData = value
            End Set
        End Property

        Public Property data_configresponse() As MsgData_ConfigResponse
            Get
                Return CType(mData, MsgData_ConfigResponse)
            End Get
            Set(ByVal value As MsgData_ConfigResponse)
                mData = value
            End Set
        End Property

        Public Property data_event() As MsgData_Event
            Get
                Return CType(mData, MsgData_Event)
            End Get
            Set(ByVal value As MsgData_Event)
                mData = value
            End Set
        End Property

        Public Property data_state() As MsgData_RealTimeState
            Get
                Return CType(mData, MsgData_RealTimeState)
            End Get
            Set(ByVal value As MsgData_RealTimeState)
                mData = value
            End Set
        End Property

        Public Property data_getrequest() As MsgData_GetRequest
            Get
                Return CType(mData, MsgData_GetRequest)
            End Get
            Set(ByVal value As MsgData_GetRequest)
                mData = value
            End Set
        End Property

        Public Property data_genericresponse() As MsgData_GenericResponse
            Get
                Return CType(mData, MsgData_GenericResponse)
            End Get
            Set(ByVal value As MsgData_GenericResponse)
                mData = value
            End Set
        End Property

        Public Property data_commandresult() As MsgData_CommandResult
            Get
                Return CType(mData, MsgData_CommandResult)
            End Get
            Set(ByVal value As MsgData_CommandResult)
                mData = value
            End Set
        End Property

        Public Property data_queryresult() As MsgData_QueryResult
            Get
                Return CType(mData, MsgData_QueryResult)
            End Get
            Set(ByVal value As MsgData_QueryResult)
                mData = value
            End Set
        End Property

        Public Property data_table() As MsgData_Table
            Get
                Return CType(mData, MsgData_Table)
            End Get
            Set(ByVal value As MsgData_Table)
                mData = value
            End Set
        End Property

        Public Sub New()
            MyBase.New("mxS")
        End Sub

        Public Sub New(ByRef bInput() As Byte)
            MyBase.New("mxS")
            Me.Parse(bInput)
        End Sub

        Public Sub New(ByVal pType As msgCommand, Optional ByVal pTable As msgTables = msgTables.none, Optional ByVal bIsResponse As Boolean = False, Optional sHttpVersion As String = "HTTP/1.1")
            MyBase.New("mxS")
            mCommand = pType
            mTable = pTable
            Select Case mCommand
                Case msgCommand.init
                    If Not bIsResponse Then
                        mData = New MsgData_Init
                    Else
                        mData = New MsgData_InitResponse(sHttpVersion)
                    End If
                Case msgCommand.register
                    If Not bIsResponse Then
                        mData = New MsgData_Register
                    Else
                        mData = New MsgData_RegisterResponse(sHttpVersion)
                    End If
                Case msgCommand.getpushconfig
                    If Not bIsResponse Then
                        mData = New MsgData_Config
                    Else
                        mData = New MsgData_ConfigResponse(sHttpVersion)
                    End If
                Case msgCommand.state
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.getrequest
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.synctime
                    If Not bIsResponse Then
                        mData = New MsgData_GenericResponse(sHttpVersion)
                    Else
                        mData = New MsgData_SyncTimeResponse(sHttpVersion)
                    End If
                Case msgCommand.logevent
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.genericresponse
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.command
                    mData = New MsgData_Table(pTable, sHttpVersion)
            End Select
        End Sub

        Public Overrides Function Parse(ByRef bInput() As Byte) As Boolean
            Try
                bReceived = True
                Dim strReceive As String = Text.Encoding.ASCII.GetString(bInput)
                Dim strSubData As String = ""
                'TODO: Log para desarrollo
                'VTBase.roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TerminalsPushServer::Message received:" & vbCrLf & strReceive)
                If strReceive.Length > 0 Then
                    mLastMessageReceived = strReceive
                    ' 0.Primera conexión
                    If strReceive.Substring(0, FIRST_CONNECT_URL.Length) = FIRST_CONNECT_URL Then
                        strSubData = strReceive.Substring(FIRST_CONNECT_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_Init(strSubData)
                        mCommand = msgCommand.init
                        bCorrect = True
                        Return True
                    End If

                    ' 1.Registro de terminal [Access control only，pushurl,registrycode]
                    If strReceive.Substring(0, REGISTRY_URL.Length) = REGISTRY_URL Then
                        strSubData = strReceive.Substring(REGISTRY_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_Register(strSubData)
                        mCommand = msgCommand.register
                        bCorrect = True
                        Return True
                    End If

                    ' 2.Terminal solicita información de parametrización del servidor. Se debe responder con el mismo mensaje con el que se responde al de primera conexión
                    If strReceive.Substring(0, GET_PUSH_INFO_URL.Length) = GET_PUSH_INFO_URL Then
                        strSubData = strReceive.Substring(GET_PUSH_INFO_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_Config(strSubData)
                        mCommand = msgCommand.getpushconfig
                        bCorrect = True
                        Return True
                    End If

                    ' 3.Recepción de datos del terminal
                    '          1.table=rtstate
                    '          2.table=rtlog
                    '          3.authtype=CARD
                    '          4.authtype=FP
                    '          5.authtype=device
                    '          6.authtype=online
                    If strReceive.Substring(0, UPLOAD_URL.Length) = UPLOAD_URL Then
                        strSubData = strReceive.Substring(UPLOAD_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        If strReceive.IndexOf("table=rtstate") > -1 Then
                            ' Realtime State
                            mData = New MsgData_RealTimeState(strSubData)
                            mCommand = msgCommand.state
                            bCorrect = True
                            Return True
                        ElseIf strReceive.IndexOf("table=rtlog") > -1 Then
                            mData = New MsgData_Event(strSubData)
                            mCommand = msgCommand.logevent
                            bCorrect = True
                            Return True
                        Else
                            ' Comando desconocido.
                            ' TODO: Debo responder OK para no perder la secuencia ...
                        End If
                        bCorrect = True
                        Return True
                    End If

                    ' 3.Recepción de datos solicitados al terminal
                    '          1.type=data
                    '          2.type=options
                    '          3.type=count
                    If strReceive.Substring(0, QUERY_UPLOAD_URL.Length) = QUERY_UPLOAD_URL Then
                        strSubData = strReceive.Substring(QUERY_UPLOAD_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_QueryResult(strSubData)
                        mCommand = msgCommand.queryresult
                        bCorrect = True
                        Return True
                    End If

                    ' 4.Solicitud de comandos por parte del terminal
                    If strReceive.Substring(0, REQUEST_DATA.Length) = REQUEST_DATA Then
                        strSubData = strReceive.Substring(REQUEST_DATA.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_GetRequest(strSubData)
                        mCommand = msgCommand.getrequest
                        bCorrect = True
                        Return True
                    End If

                    ' 5.Respuesta del terminal con resultado de ejectutar comando enviado desde servidor
                    If strReceive.Substring(0, REPONSE_RESULT.Length) = REPONSE_RESULT Then
                        strSubData = strReceive.Substring(REPONSE_RESULT.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_CommandResult(strSubData)
                        mCommand = msgCommand.commandresult
                        bCorrect = True
                        Return True
                    End If

                    ' 6.Sincronización de hora
                    If strReceive.Substring(0, SYNC_TIME.Length) = SYNC_TIME Then
                        strSubData = strReceive.Substring(SYNC_TIME.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_SyncTime(strSubData)
                        mCommand = msgCommand.synctime
                        bCorrect = True
                        Return True
                    End If

                    ' 10.Actualización de firmware
                    If strReceive.Substring(0, DOWN_LOAD_FILE_URL.Length) = DOWN_LOAD_FILE_URL Then
                        'subData = strReceive.Substring(DOWN_LOAD_FILE_URL.Length + REQ_LINE_PARAM_SPIT_STR.Length)
                        'HandleDownLoadFile(subData, mySocket)
                        bCorrect = True
                        Return True
                    End If

                End If
            Catch ex As Exception
                Return False
            End Try
            Return False
        End Function

        Public Overrides Function ToBytes() As Byte()
            Return System.Text.Encoding.GetEncoding(STRING_ENCODING_FMT).GetBytes(Me.toString)
        End Function

        Public Overrides Function ToDebugInfo() As String
            Try
                Dim tmp As String = ""
                If bReceived Then
                    tmp = prtCMD + chrpreValues + getCommandTypeString(mCommand) + chrBlocks + prtSN + chrpreValues + Me.SN + chrBlocks + prtTOKEN + chrpreValues + Me.Token + chrBlocks + prtDATA + chrpreValues + mData.toDebugInfo
                    Return tmp
                Else
                    tmp = prtCMD + chrpreValues + getCommandTypeString(mCommand) + chrBlocks + prtDATA + chrpreValues + mData.toDebugInfo
                    Return tmp
                End If
            Catch ex As Exception
                Return "error getting info for log!"
            End Try
        End Function

        Private Function getCommandType(ByVal value As String) As msgCommand
            Try

                Select Case value.ToLower
                    Case "init"
                        Return msgCommand.init
                    Case "logevent"
                        Return msgCommand.logevent
                    Case "getpushconfig"
                        Return msgCommand.getpushconfig
                    Case "clear"
                        Return msgCommand.clear
                    Case "download"
                        Return msgCommand.downfile
                    Case "upload"
                        Return msgCommand.upfile
                    Case "reboot"
                        Return msgCommand.reboot
                    Case Else
                        Return msgCommand.none
                End Select
            Catch ex As Exception
                Return msgCommand.none
            End Try
        End Function

        Private Function getCommandTypeString(ByVal value As msgCommand) As String
            Try

                Select Case value
                    Case msgCommand.init
                        Return "init"
                    Case msgCommand.register
                        Return "register"
                    Case msgCommand.getpushconfig
                        Return "getconfig"
                    Case msgCommand.logevent
                        Return "logevent"
                    Case msgCommand.commandresult
                        Return "commandresult"
                    Case msgCommand.queryresult
                        Return "queryresult"
                    Case msgCommand.getrequest
                        Return "getrequest"
                    Case msgCommand.state
                        Return "state"
                    Case msgCommand.clear
                        Return "clear"
                    Case msgCommand.upfile
                        Return "upload"
                    Case msgCommand.downfile
                        Return "download"
                    Case msgCommand.reboot
                        Return "reboot"
                    Case Else
                        Return value.ToString
                End Select
            Catch ex As Exception
                Return msgCommand.none
            End Try
        End Function

        Private Function getTableType(ByVal value As String) As msgTables
            Try
                Return System.Enum.Parse(GetType(msgTables), value, True)
                'Select Case value.ToLower
                '    Case "offline"
                '        Return msgTables.offline
                '    Case "employees"
                '        Return msgTables.employees
                '    Case "employeephoto"
                '        Return msgTables.employeephoto
                '    Case "cards"
                '        Return msgTables.cards
                '    Case "biometrics"
                '        Return msgTables.biometrics

                '    Case "causes"
                '        Return msgTables.causes
                '    Case "groups"
                '        Return msgTables.groups
                '    Case "access"
                '        Return msgTables.access
                '    Case "timezones"
                '        Return msgTables.timezones
                '    Case "sirens"
                '        Return msgTables.sirens
                '    Case "punchephoto"
                '        Return msgTables.photo
                '    Case Else
                '        Return msgTables.none
                'End Select
            Catch ex As Exception
                Return msgTables.none
            End Try
        End Function

        ''' <summary>
        ''' Crea el mensaje de respuesta
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function toString() As String
            Try
                'TODO: Actualizar Sesion en tabla de terminales - Machine.UpdateSessionIDBySN(SessionID,SN);
                Return mData.toString
            Catch ex As Exception
                Return "ERROR"
            End Try
        End Function

    End Class

    <Serializable()>
    Public MustInherit Class MsgData
        ' Constantes
        Public Const chrParameters = ","
        Public Const chrParamValueSep = "="
        Public Const chrNewParametersLine = vbCrLf
        Public Const chrUploadDataParameters = vbTab
        Public Const chrUploadDataParametersValueSep = "="
        Public Const chrDataTableRecord As String = vbCrLf + vbCrLf
        Public Const chrCommandsResultLine = vbLf

        ' Caracteres especiales protocolo PUSH
        Public Const REQ_LINE_PARAM_SPLIT_STR As String = "?"
        Public Const HEADER_END_STR As String = vbCrLf + vbCrLf '"\r\n\r\n"
        Public Const HEADER_SPLIT_STR As String = vbCrLf '"\r\n"

        Public Const STATUS_SUCCESS As String = " 200 OK"
        Public Const SUCCESS_CODE As String = "OK"
        Public Const STATUS_NO_REGISTRY As String = " 405 Device Not Registry"
        Public Const ERR_CODE_NO_REGISTRY As String = "405"
        Public Const STATUS_SESSION_INVALID As String = " 406 Device Session Invalid"
        Public Const ERR_CODE_SESSION_INVALID As String = "406"
        Public Const STATUS_DATA_INVALID As String = " 407 Device Data Invalid"
        Public Const ERR_CODE_DATA_INVALID As String = "407"
        Public Const STATUS_FORBIDDEN As String = " 403 Forbidden"
        Public Const ERR_CODE_FORBIDDEN As String = "403"

        Public Const RETURN_OK_SUCESS As String = "OK"

        Public Const HEADER_RESPONSE_SERVER As String = "Server: C#demo/1.1" '"Server: VisualTime Live"
        Public Const HEADER_RESPONSE_DATE As String = "Date: "
        Public Const HEADER_RESPONSE_CONTENT_TYPE As String = "Content-Type: application/push;charset=UTF-8"
        Public Const HEADER_RESPONSE_CONTENT_LEN As String = "Content-Length: "
        Public Const HEADER_RESPONSE_CONNECT As String = "Connection: keep-alive"

        Public Const HEADER_REQ_HEADER_COOKIE = "Cookie: "
        Public Const STRING_ENCODING_FMT = "UTF-8"

        Public Const BG_AUTH_SUCCESS As String = "AUTH=SUCCESS"
        Public Const BG_AUTH_FAILED As String = "AUTH=FAILED"
        Public Const BG_AUTH_CTRL_CMD As String = "CONTROL DEVICE 1 1 1 6"
        Public Const PARK_ONLINE_CTRL_CMD As String = "CONTROL DEVICE 01010101\\\nCONTROL DEVICE 02010124E8AFB7E588B7E58DA1E68896E58F96E58DA1\\\nCONTROL DEVICE 02010224E8AFB7E588B7E58DA1E68896E58F96E58DA1"

        ' Propiedades
        Protected _ResponseStatus As dataStatus
        Protected _HttpVersion As String
        Protected _Message As String
        Protected _SN As String
        Protected _SessionID As String
        Protected _Token As String
        Protected _Timestamp As String

        'Cabecera y cuerpo del mensaje
        Protected _httpHeader As String
        Protected _httpContent As String

        ' Estado para cabecera
        Public Enum dataStatus
            success
            no_registry
            session_invalid
            data_invalid
            forbidden
        End Enum

        Public ReadOnly Property SN As String
            Get
                Return _SN
            End Get
        End Property

        Public ReadOnly Property Timestamp As String
            Get
                Return _Timestamp
            End Get
        End Property

        Public ReadOnly Property Token As String
            Get
                Return _Token
            End Get
        End Property

        Public Sub New()
        End Sub

        Public MustOverride Function Parse(ByVal sInput As String) As Boolean

        Public MustOverride Overrides Function toString() As String

        Public MustOverride Function toDebugInfo() As String

        Public Sub GetHttpHeader()
            _httpHeader = _HttpVersion.Trim + getHeaderStatusString(_ResponseStatus) + HEADER_SPLIT_STR
            _httpHeader = _httpHeader + HEADER_RESPONSE_SERVER + HEADER_SPLIT_STR
            _httpHeader = _httpHeader + HEADER_RESPONSE_DATE + DateTime.UtcNow.GetDateTimeFormats("r"c)(0).ToString + HEADER_SPLIT_STR
            _httpHeader = _httpHeader + HEADER_RESPONSE_CONTENT_TYPE + HEADER_SPLIT_STR
            _httpHeader = _httpHeader + HEADER_RESPONSE_CONTENT_LEN + _httpContent.Length.ToString + HEADER_SPLIT_STR
            _httpHeader = _httpHeader + HEADER_RESPONSE_CONNECT + HEADER_END_STR
        End Sub

        Protected Function getHeaderStatusString(ByVal value As dataStatus) As String
            Try
                Select Case value
                    Case dataStatus.success
                        Return STATUS_SUCCESS
                    Case dataStatus.no_registry
                        Return STATUS_NO_REGISTRY
                    Case dataStatus.session_invalid
                        Return STATUS_SESSION_INVALID
                    Case dataStatus.data_invalid
                        Return STATUS_DATA_INVALID
                    Case dataStatus.forbidden
                        Return STATUS_FORBIDDEN
                    Case Else
                        Return ""
                End Select
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Protected Function getContentStatusString(ByVal value As dataStatus) As String
            Try
                Select Case value
                    Case dataStatus.success
                        Return RETURN_OK_SUCESS
                    Case dataStatus.no_registry
                        Return ERR_CODE_NO_REGISTRY
                    Case dataStatus.session_invalid
                        Return ERR_CODE_SESSION_INVALID
                    Case dataStatus.data_invalid
                        Return ERR_CODE_DATA_INVALID
                    Case dataStatus.forbidden
                        Return ERR_CODE_FORBIDDEN
                    Case Else
                        Return ""
                End Select
            Catch ex As Exception
                Return ""
            End Try
        End Function

    End Class

    ''' <summary>
    ''' Primera conexión
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_Init
        Inherits MsgData

        Private _Options As String

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public ReadOnly Property Options() As String
            Get
                Return _Options
            End Get
        End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
            Parse(sData)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing
            Dim registrycode As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                'Get options
                startIdx = requestLine.IndexOf("options=") + "options=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _Options = p.Substring(0, endIdx - 0)
                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try

            Return True
        End Function

        Public Overrides Function toString() As String
            Return ""
        End Function

        Public Overrides Function toDebugInfo() As String
            Return ""
        End Function

    End Class

    ''' <summary>
    ''' Primera conexión
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_InitResponse
        Inherits MsgData

        Public Enum dataParameters
            Registry
            RegistryCode
            ServerVersion
            ServerName
            PushVersion
            ErrorDelay
            RequestDelay
            TransTimes
            TransInterval
            TransTables
            Realtime
            SessionID
            Unknown
        End Enum

        Private oParameters As Dictionary(Of String, String)

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Property Status As dataStatus
            Get
                Return _ResponseStatus
            End Get
            Set(value As dataStatus)
                _ResponseStatus = value
            End Set
        End Property

        Public Sub New(sHttpVersion As String)
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
            _HttpVersion = sHttpVersion
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim tmp As String = ""
            For Each item As KeyValuePair(Of String, String) In oParameters
                If tmp.Length > 0 Then tmp += chrNewParametersLine
                tmp += item.Key + chrParamValueSep + item.Value
            Next
            _httpContent = tmp
            GetHttpHeader()
            Return _httpHeader + _httpContent
        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    ''' <summary>
    ''' Registro de terminal
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_Register
        Inherits MsgData

        Public Enum dataParameters
            DeviceType
            _DeviceName '~DeviceName
            FirmVer
            PushVersion
            CommType
            MaxPackageSize
            LockCount
            ReaderCount
            AuxInCount
            AuxOutCount
            MachineType
            _IsOnlyRFMachine '~IsOnlyRFMachine
            _MaxUserCount '~MaxUserCount
            _MaxAttLogCount '~MaxAttLogCount
            _MaxUserFingerCount '~MaxUserFingerCount
            MThreshold
            IPAddress
            NetMask
            GATEIPAddress
            _ZKFPVersion '~ZKFPVersion
            _REXInputFunOn '~REXInputFunOn
            _CardFormatFunOn '~CardFormatFunOn
            _SupAuthrizeFunOn '~SupAuthrizeFunOn
            _ReaderCFGFunOn '~ReaderCFGFunOn
            _ReaderLinkageFunOn '~ReaderLinkageFunOn
            _RelayStateFunOn '~RelayStateFunOn
            _Ext485ReaderFunOn '~Ext485ReaderFunOn
            _TimeAPBFunOn '~TimeAPBFunOn
            _CtlAllRelayFunOn '~CtlAllRelayFunOn
            _LossCardFunOn '~LossCardFunOn
            SimpleEventType
            VerifyStyles
            EventTypes
            DisableUserFunOn
            DeleteAndFunOn
            LogIDFunOn
            DateFmtFunOn
            Unknown
        End Enum

        Private oParameters As Dictionary(Of String, String)
        Private _Response As String

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property HttpConent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Sub New()
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
            Parse(sData)
        End Sub

        Public ReadOnly Property Response() As String
            Get
                Return _Response
            End Get
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            'Dim httpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim registrycode As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)
            Catch e As Exception
                Return False
            End Try

            ' Aquí cargo los valores de configuración enviados por el terminal
            Try
                If _httpContent.IndexOf(chrParameters) > 0 Then
                    Dim ic As Integer
                    For Each item As String In _httpContent.Split(chrParameters)
                        ic = item.IndexOf(chrParamValueSep)
                        If ic > 0 Then
                            ' Parámetro con valor asociado
                            setValue(parseEnum(item.Substring(0, ic).Replace("~", "_")), item.Substring(ic + 1))
                        Else
                            ' Parámetro sin valor asociado
                            setValue(item, "")
                        End If
                    Next
                Else
                    _Response = _httpContent
                End If
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try

                Dim tmp As String = ""
                For Each item As KeyValuePair(Of String, String) In oParameters
                    If tmp.Length > 0 Then tmp += chrParameters
                    tmp += item.Key + chrParamValueSep + item.Value
                Next
                Return tmp
            Catch ex As Exception
                Return ""
            End Try

        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpConent
        End Function

    End Class

    ''' <summary>
    ''' Respuesta a mesaje de registro
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_RegisterResponse
        Inherits MsgData

        Public Enum dataParameters
            registrycode
            Unknown
        End Enum

        Private oParameters As Dictionary(Of String, String)
        Private _Response As String

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public ReadOnly Property HttpContent As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Property Status As dataStatus
            Get
                Return _ResponseStatus
            End Get
            Set(value As dataStatus)
                _ResponseStatus = value
            End Set
        End Property

        Public Sub New(sHttpVersion As String)
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
            _HttpVersion = sHttpVersion
        End Sub

        Public Property Response() As String
            Get
                Return _Response
            End Get
            Set(value As String)
                _Response = value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Try
                Dim tmp As String = ""
                For Each item As KeyValuePair(Of String, String) In oParameters
                    If tmp.Length > 0 Then tmp += chrParameters
                    tmp += item.Key + chrParamValueSep + item.Value
                Next
                _httpContent = tmp
                GetHttpHeader()
                Return _httpHeader + _httpContent
            Catch ex As Exception
                Return ""
            End Try

        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    ''' <summary>
    ''' Solicitud de parámetros de configuración
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_Config
        Inherits MsgData

        Private _Options As String

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public ReadOnly Property Options() As String
            Get
                Return _Options
            End Get
        End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
            Parse(sData)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing
            Dim registrycode As String = Nothing
            Dim cookie As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                'Obtengo Token y Timestamp
                startIdx = sInput.IndexOf("Cookie: ")
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)

                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                endIdx = cookie.IndexOf(","c)
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = _Token.IndexOf("="c) + 1
                _Token = _Token.Substring(startIdx)

                startIdx = cookie.IndexOf(","c) + 1
                _Timestamp = cookie.Substring(startIdx)
                startIdx = _Timestamp.IndexOf("="c) + 1
                _Timestamp = _Timestamp.Substring(startIdx)

                'Get options
                startIdx = requestLine.IndexOf("options=") + "options=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _Options = p.Substring(0, endIdx - 0)
                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
            Catch ex As Exception
                'gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try

            Return True
        End Function

        Public Overrides Function toString() As String
            Return ""
        End Function

        Public Overrides Function toDebugInfo() As String
            Return ""
        End Function

    End Class

    ''' <summary>
    ''' Respuesta a Solicitud de parámetros de configuración
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_ConfigResponse
        Inherits MsgData

        Public Enum dataParameters
            registry
            RegistryCode
            ServerVersion
            ServerName
            PushVersion
            ErrorDelay
            RequestDelay
            TransTimes
            TransInterval
            TransTables
            Realtime
            SessionID
            Unknown
            TimeoutSec
        End Enum

        Private oParameters As Dictionary(Of String, String)

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public ReadOnly Property HttpContent As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Property Status As dataStatus
            Get
                Return _ResponseStatus
            End Get
            Set(value As dataStatus)
                _ResponseStatus = value
            End Set
        End Property

        Public Sub New(sHttpVersion As String)
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
            _HttpVersion = sHttpVersion
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim tmp As String = ""
            For Each item As KeyValuePair(Of String, String) In oParameters
                If tmp.Length > 0 Then tmp += chrNewParametersLine
                tmp += item.Key + chrParamValueSep + item.Value
            Next
            _httpContent = tmp
            GetHttpHeader()
            Return _httpHeader + _httpContent
        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    ''' <summary>
    ''' Respuesta a Solicitud de sincronización de hora y zona horaria del servidor
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_SyncTimeResponse
        Inherits MsgData

        Public Enum dataParameters
            DateTime
            ServerTz
        End Enum

        Private oParameters As Dictionary(Of String, String)

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public ReadOnly Property HttpContent As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Property Status As dataStatus
            Get
                Return _ResponseStatus
            End Get
            Set(value As dataStatus)
                _ResponseStatus = value
            End Set
        End Property

        Public Sub New(sHttpVersion As String)
            MyBase.New()
            oParameters = New Dictionary(Of String, String)
            _HttpVersion = sHttpVersion
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim tmp As String = ""
            For Each item As KeyValuePair(Of String, String) In oParameters
                If tmp.Length > 0 Then tmp += chrParameters
                tmp += item.Key + chrParamValueSep + item.Value
            Next
            _httpContent = tmp
            GetHttpHeader()
            Return _httpHeader + _httpContent
        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    Public Class MsgData_RealTimeState
        Inherits MsgData

        Private _DateTime As DateTime
        Private _Sensor As String = ""
        Private _Relay As String = ""
        Private _Alarm As String = ""

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Property DateTime() As DateTime
            Get
                Return _DateTime
            End Get
            Set(ByVal Value As DateTime)
                _DateTime = Value
            End Set
        End Property

        Public Property Sensor() As String
            Get
                Return _Sensor
            End Get
            Set(ByVal Value As String)
                _Sensor = Value
            End Set
        End Property

        Public Property Relay() As String
            Get
                Return _Relay
            End Get
            Set(ByVal Value As String)
                _Relay = Value
            End Set
        End Property

        Public Property Alarm() As String
            Get
                Return _Alarm
            End Get
            Set(ByVal Value As String)
                _Alarm = Value
            End Set
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            Parse(Input)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim registrycode As String = Nothing
            Dim cookie As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)
                _HttpVersion = p.Substring(endIdx + 1)

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)

                If _httpContent.Length > 0 Then
                    _DateTime = Any2Date(_httpContent.Split(chrUploadDataParameters)(0).Split(chrUploadDataParametersValueSep)(1))
                    _Sensor = _httpContent.Split(chrUploadDataParameters)(1).Split(chrUploadDataParametersValueSep)(1)
                    _Relay = _httpContent.Split(chrUploadDataParameters)(2).Split(chrUploadDataParametersValueSep)(1)
                    _Alarm = _httpContent.Split(chrUploadDataParameters)(3).Split(chrUploadDataParametersValueSep)(1)
                End If
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Return "(" + _DateTime.ToString + ", Door state: " + _Sensor.ToString + ", Relay state: " + _Relay.ToString + ", Alarm state: " + _Alarm.ToString + ")"
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_Event
        Inherits MsgData

        'Variables de mensaje recibido
        Private _PunchDateTime As DateTime
        Private _PIN As String = ""
        Private _Card As String = ""
        Private _EventAddr As String = ""
        Private _Event As String = ""
        Private _InOutStatus As String = ""
        Private _VerifyType As String = ""
        Private _Index As Integer

        'Variables funcionales
        Private _Reader As Integer
        Private _Action As String
        Private _IsPunch As Boolean = False

        Public ReadOnly Property Reader
            Get
                Return _Reader
            End Get
        End Property

        Public ReadOnly Property Action
            Get
                Return _Action
            End Get
        End Property

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public ReadOnly Property PunchDateTime() As DateTime
            Get
                Return _PunchDateTime
            End Get
        End Property

        Public ReadOnly Property PIN() As String
            Get
                Return _PIN
            End Get
        End Property

        Public ReadOnly Property Card() As String
            Get
                Return _Card
            End Get
        End Property

        Public ReadOnly Property VerifyType() As String
            Get
                Return _VerifyType
            End Get
        End Property

        Public ReadOnly Property IsPunch() As Boolean
            Get
                Return _IsPunch
            End Get
        End Property

        Public ReadOnly Property Index() As Integer
            Get
                Return _VerifyType
            End Get
        End Property

        Public ReadOnly Property LogEvent() As String
            Get
                Return _Event
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            Parse(Input)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            Dim ohttpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim registrycode As String = Nothing
            Dim cookie As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)
                _HttpVersion = p.Substring(endIdx + 1)

                If _httpContent.Length > 0 Then
                    Dim iLen As Integer = 0
                    iLen = _httpContent.Split(chrUploadDataParameters).Count - 1
                    If iLen >= 0 Then _PunchDateTime = Any2Date(_httpContent.Split(chrUploadDataParameters)(0).Split(chrUploadDataParametersValueSep)(1))
                    If iLen >= 1 Then _PIN = _httpContent.Split(chrUploadDataParameters)(1).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 2 Then _Card = _httpContent.Split(chrUploadDataParameters)(2).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 3 Then _EventAddr = _httpContent.Split(chrUploadDataParameters)(3).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 4 Then _Event = _httpContent.Split(chrUploadDataParameters)(4).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 5 Then _InOutStatus = _httpContent.Split(chrUploadDataParameters)(5).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 6 Then _VerifyType = _httpContent.Split(chrUploadDataParameters)(6).Split(chrUploadDataParametersValueSep)(1)
                    If iLen >= 7 Then _Index = _httpContent.Split(chrUploadDataParameters)(7).Split(chrUploadDataParametersValueSep)(1)

                    ' Calculo parámetros funcionales
                    _Reader = Any2Integer(_EventAddr)
                    _IsPunch = clsPushProtocol.IsPunch(_Event, _Action)
                End If

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)

                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Return "(" + _PunchDateTime.ToString + "," + _PIN.ToString + "," + _Card.ToString + "," + _EventAddr.ToString + "," + _Event.ToString + "," + _InOutStatus.ToString + "," + _VerifyType.ToString + "," + _Index.ToString + ")"
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_GetRequest
        Inherits MsgData

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            Parse(Input)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            Dim httpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim cookie As String = Nothing
            Dim registrycode As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                httpContent = sInput.Substring(startIdx)
                _HttpVersion = p.Substring(endIdx + 1)
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Return "ok"
        End Function

        Public Overrides Function toDebugInfo() As String
            Return ""
        End Function

    End Class

    Public Class MsgData_SyncTime
        Inherits MsgData

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            Parse(Input)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            Dim httpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim cookie As String = Nothing
            Dim registrycode As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                httpContent = sInput.Substring(startIdx)
                _HttpVersion = p.Substring(endIdx + 1)
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Return "ok"
        End Function

        Public Overrides Function toDebugInfo() As String
            Return ""
        End Function

    End Class

    ''' <summary>
    ''' Resultado de consulta de datos
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_QueryResult
        Inherits MsgData

        Public Enum eType
            tabledata
            options
            count
        End Enum

        Private _type As eType
        Private _cmdid As Integer
        Private _count As Integer
        Private _tablename As String
        Private oDeviceParameters As Dictionary(Of String, String)

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public ReadOnly Property Type As eType
            Get
                Return _type
            End Get
        End Property

        Public ReadOnly Property Count As Integer
            Get
                Return _count
            End Get
        End Property

        Public ReadOnly Property CommandID As Integer
            Get
                Return _cmdid
            End Get
        End Property

        Public ReadOnly Property DeviceParameters As Dictionary(Of String, String)
            Get
                Return oDeviceParameters
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
            oDeviceParameters = New Dictionary(Of String, String)
            Me.Parse(sData)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            Dim httpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim registrycode As String = Nothing
            Dim cookie As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                ' Número de serie
                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If
                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _SN = p.Substring(0, endIdx - 0)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)

                ' Contenido
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                ' Tipo de resultado
                startIdx = requestLine.IndexOf("type=") + "type=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If
                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                Select Case p.Substring(0, endIdx - 0).ToUpper
                    Case "TABLEDATA"
                        _type = eType.tabledata
                    Case "OPTIONS"
                        _type = eType.options
                    Case "COUNT"
                        _type = eType.count
                End Select

                ' Número de registros
                startIdx = requestLine.IndexOf("cmdid=") + "cmdid=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If
                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _cmdid = CInt(p.Substring(0, endIdx - 0))

                ' Id de comando
                startIdx = requestLine.IndexOf("count=") + "count=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If
                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _count = CInt(p.Substring(0, endIdx - 0))

                ' Tabla consultada
                startIdx = requestLine.IndexOf("tablename=") + "tablename=".Length
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If
                If -1 = endIdx Then
                    'Error data
                    Return False
                End If
                _tablename = p.Substring(0, endIdx - 0)

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)

                ' Si es de tipo OPTIONS, recojo los parámetros
                If Type = eType.options Then
                    For Each item As String In _httpContent.Split(chrParameters)
                        If item.IndexOf("=") > -1 Then
                            ' Son parámetros de configuración del terminal (command INFO)
                            oDeviceParameters.Add(item.Split("=")(0), item.Split("=")(1))
                        End If
                    Next
                End If
            Catch e As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_QueryResult::Parse::Error:", e)
                Return False
            End Try

            ' TODO: Aquí cargo los valores de configuración enviados por el terminal
            Try
                ' Decodifica esto: type=tabledata&type=tabledata&cmdid=25570&tablename=user&count=3&packcnt=1&packidx=1
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Return String.Empty
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    ''' <summary>
    ''' Resultado de comandos
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_CommandResult
        Inherits MsgData

        Private oResults As ArrayList

        Public Enum dataParameters
            ID
            ReturnCode
            CMD
            unknown
        End Enum

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public ReadOnly Property Results As ArrayList
            Get
                Return oResults
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
            oResults = New ArrayList
            Parse(sData)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Dim startIdx As Integer = 0
            Dim endIdx As Integer = 0
            Dim requestLine As String = Nothing
            Dim p As String = Nothing

            'Dim httpContent As String = Nothing
            Dim responseHeader As String = Nothing
            Dim responseContent As String = Nothing
            Dim sendStr As [String] = Nothing
            Dim sendBytes As [Byte]() = Nothing
            Dim i As Integer = 0
            Dim idx As Integer = 0
            Dim registrycode As String = Nothing
            Dim cookie As String = Nothing

            Try
                endIdx = sInput.IndexOf(vbCr & vbLf)
                requestLine = sInput.Substring(0, endIdx - startIdx)

                startIdx = requestLine.IndexOf("SN=") + 3
                p = requestLine.Substring(startIdx)
                endIdx = p.IndexOf("&")
                If -1 = endIdx Then
                    endIdx = p.IndexOf(" ")
                End If

                If -1 = endIdx Then
                    'Error data
                    Return False
                End If

                _SN = p.Substring(0, endIdx - 0)

                ' Versión de http
                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)

                ' Cuerpo del mensaje
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                'Obtengo Token
                startIdx = sInput.IndexOf(HEADER_REQ_HEADER_COOKIE)
                cookie = sInput.Substring(startIdx, sInput.Length - startIdx)
                endIdx = cookie.IndexOf(vbCr & vbLf)
                cookie = cookie.Substring(0, endIdx)
                startIdx = HEADER_REQ_HEADER_COOKIE.Length
                _Token = cookie.Substring(startIdx, endIdx - startIdx)
                startIdx = Token.IndexOf("="c) + 1
                _Token = Token.Substring(startIdx)
            Catch e As Exception
                'TODO: Log
                Return False
            End Try

            ' Aquí cargo los valores de configuración enviados por el terminal
            Try
                If _httpContent.IndexOf(chrNewParametersLine) > 0 OrElse _httpContent.IndexOf(vbCr) > 0 Then
                    For Each item As String In _httpContent.Split(chrNewParametersLine)
                        If item.IndexOf("&") > -1 Then oResults.Add(New CommandResult(item.Split("&")(0).Split("=")(1), item.Split("&")(1).Split("=")(1), item.Split("&")(2).Split("=")(1)))
                    Next
                End If
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Return String.Empty
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    ''' <summary>
    ''' Primera conexión
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_GenericResponse
        Inherits MsgData

        Public Enum dataParameters
            Registry
            RegistryCode
            ServerVersion
            ServerName
            PushVersion
            ErrorDelay
            RequestDelay
            TransTimes
            TransInterval
            TransTables
            Realtime
            SessionID
            Unknown
        End Enum

        Private oParameters As Dictionary(Of String, String)

        'Public ReadOnly Property SN() As String
        '    Get
        '        Return _SN
        '    End Get
        'End Property

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public Property Status As dataStatus
            Get
                Return _ResponseStatus
            End Get
            Set(value As dataStatus)
                _ResponseStatus = value
            End Set
        End Property

        Public ReadOnly Property HttpContent As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Sub New(sHttpVersion As String)
            MyBase.New()
            _HttpVersion = sHttpVersion
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Select Case _ResponseStatus
                Case dataStatus.success
                    _httpContent = RETURN_OK_SUCESS
                Case dataStatus.no_registry
                    _httpContent = ERR_CODE_NO_REGISTRY
                Case dataStatus.session_invalid
                    _httpContent = ERR_CODE_SESSION_INVALID
                Case dataStatus.data_invalid
                    _httpContent = ERR_CODE_DATA_INVALID
                Case dataStatus.forbidden
                    _httpContent = ERR_CODE_FORBIDDEN
                Case Else
                    _httpContent = ""
            End Select
            GetHttpHeader()
            Return _httpHeader + _httpContent
        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    Public Class MsgData_Table
        Inherits MsgData

        Private mTable As MessageMxS.msgTables
        Private mData() As MsgData_Table_generic
        Private index As Integer = 0
        Private offset As Integer = 0

        Public Sub New(ByVal Table As MessageMxS.msgTables, Optional sHttpVersion As String = "")
            MyBase.New()
            mTable = Table
            _HttpVersion = sHttpVersion
            Select Case mTable
                Case MessageMxS.msgTables.transaction
                    mData = Array.CreateInstance(GetType(MsgData_Table_Transaction), 20)
                    For i As Byte = 0 To 19
                        mData(i) = New MsgData_Table_Transaction
                    Next
                Case MessageMxS.msgTables.user
                    mData = Array.CreateInstance(GetType(msgdata_table_Employees), 50)
                Case MessageMxS.msgTables.templatev10
                    mData = Array.CreateInstance(GetType(msgdata_table_Fingers), 50)
                Case MessageMxS.msgTables.timezone
                    mData = Array.CreateInstance(GetType(msgdata_table_TimeZone), 50)
                Case MessageMxS.msgTables.firstcard
                Case MessageMxS.msgTables.holiday
                Case MessageMxS.msgTables.inoutfun
                Case MessageMxS.msgTables.multicard
                Case MessageMxS.msgTables.userauthorize
                    mData = Array.CreateInstance(GetType(msgdata_table_TimeZone), 50)
                Case MessageMxS.msgTables.commands
                    mData = Array.CreateInstance(GetType(msgdata_table_command), 100)
            End Select
        End Sub

        Public Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
            Set(value As String)
                _HttpVersion = value
            End Set
        End Property

        Public ReadOnly Property HttpContent As String
            Get
                Return _httpContent
            End Get
        End Property

        Public ReadOnly Property offline() As MsgData_Table_Transaction()
            Get
                Return CType(mData, MsgData_Table_Transaction())
            End Get
        End Property

        Public Sub addDelete(ByVal IDEmployee As Integer, Optional ByVal IDFinger As Byte = 0)
            Try
                mData(index) = New msgdata_table_Delete
                CType(mData(index), msgdata_table_Delete).ID = IDEmployee
                CType(mData(index), msgdata_table_Delete).IDFinger = IDFinger
                index += 1
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::adddel::Error:", ex)
            End Try
        End Sub

        Public Sub addEmployee(ByVal IDEmployee As Integer, ByVal Name As String, Optional ByVal PIN As String = "", Optional ByVal Language As String = "", Optional ByVal IsOnline As Boolean = False)
            Try
                mData(index) = New msgdata_table_Employees
                CType(mData(index), msgdata_table_Employees).ID = IDEmployee
                CType(mData(index), msgdata_table_Employees).Name = Name
                CType(mData(index), msgdata_table_Employees).PIN = PIN
                CType(mData(index), msgdata_table_Employees).Language = Language
                CType(mData(index), msgdata_table_Employees).IsOnline = IsOnline
                index += 1
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::addEmployee::Error:", ex)
            End Try
        End Sub

        Public Sub addBio(ByVal IDEmployee As Integer, ByVal IDFinger As Byte, ByVal Finger As Byte())
            Try
                mData(index) = New msgdata_table_Fingers
                CType(mData(index), msgdata_table_Fingers).IDEmployee = IDEmployee
                CType(mData(index), msgdata_table_Fingers).IDFinger = IDFinger
                CType(mData(index), msgdata_table_Fingers).Finger = Finger
                index += 1
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::addFinger::Error:", ex)
            End Try
        End Sub

        Public Sub addTimezone(ByVal IDGroup As String, ByVal IDReader As Byte, ByVal Weekday As Byte, ByVal BeginTime As DateTime, ByVal EndTime As DateTime)
            Try
                mData(index) = New msgdata_table_TimeZone
                CType(mData(index), msgdata_table_TimeZone).IDGroup = IDGroup
                CType(mData(index), msgdata_table_TimeZone).IDReader = IDReader
                CType(mData(index), msgdata_table_TimeZone).WeekDay = Weekday
                CType(mData(index), msgdata_table_TimeZone).BeginTime = BeginTime
                CType(mData(index), msgdata_table_TimeZone).EndTime = EndTime
                index += 1
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::addTimezone::Error:", ex)
            End Try
        End Sub

        Public Sub addCommand(ByVal IDCommand As Long, ByVal oCommand As msgdata_table_command.dataCommands, ByVal oTable As MessageMxS.msgTables, Optional bNewSyncTask As Boolean = False)
            Try
                offset = index
                If bNewSyncTask Then offset = 0
                mData(index) = New msgdata_table_command
                ' Como identificador de secuencia usamos el ID de tarea en TerminalsSynctaks, con un offset de 10 porque algunas tareas se pueden componer de varias sencuencias
                ' El id de secuencia en la centralita tiene un valor máximo de long (2.147.483.647)
                CType(mData(index), msgdata_table_command).IDCommnad = IDCommand * 10 + offset
                CType(mData(index), msgdata_table_command).Command = oCommand
                CType(mData(index), msgdata_table_command).Table = oTable

                index += 1
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::addCommand::Error:", ex)
            End Try
        End Sub

        Public Sub addCommandParameters(ByVal Key As msgdata_table_command.dataParameters, ByVal value As String)
            Try
                CType(mData(index - 1), msgdata_table_command).setValue(Key, value)
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::addCommandParameters::Error:", ex)
            End Try
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            'TODO
            Try
                index = 0
                For Each Str As String In sInput.Split(chrParameters)
                    If Not mData(index).Parse(Str) Then Return False
                    index += 1
                Next
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Dim i As Integer
                Dim str As String = ""
                For i = 0 To index - 1
                    If str.Length > 0 Then str += vbCr + vbLf + vbCr + vbLf
                    str += mData(i).toString
                Next

                _httpContent = str
                GetHttpHeader()
                Return _httpHeader + _httpContent
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table::toString::Error:", ex)
                Return ""
            End Try
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.HttpContent
        End Function

    End Class

    Public MustInherit Class MsgData_Table_generic
        Public Const chrParamPreValueSep = ","

        Public MustOverride Function Parse(ByVal sInput As String) As Boolean

        Public MustOverride Overrides Function toString() As String

    End Class

    Public Class msgdata_table_Delete
        Inherits MsgData_Table_generic

        Private _ID As Integer
        Private _Finger As Byte

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal value As Integer)
                _ID = value
            End Set
        End Property

        Public Property IDFinger() As Byte
            Get
                Return _Finger
            End Get
            Set(ByVal value As Byte)
                _Finger = value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Return _ID.ToString + chrParamPreValueSep + _Finger.ToString
        End Function

    End Class

    Public Class msgdata_table_Employees
        Inherits MsgData_Table_generic

        Private _ID As Integer
        Private _Name As String
        Private _PIN As String = ""
        Private _Language As String = "es"
        Private _IsOnline As Boolean = False

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal Value As Integer)
                _ID = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(ByVal Value As String)
                _PIN = Value
            End Set
        End Property

        Public Property Language() As String
            Get
                Return _Language
            End Get
            Set(ByVal Value As String)
                _Language = Value
            End Set
        End Property

        Public Property IsOnline() As Boolean
            Get
                Return _IsOnline
            End Get
            Set(ByVal value As Boolean)
                _IsOnline = value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try

                For i As Byte = 0 To sInput.Split(chrParamPreValueSep).Length - 1
                    Select Case i
                        Case 0
                            _ID = sInput.Split(chrParamPreValueSep)(0)
                        Case 1
                            _Name = StrDecoding(sInput.Split(chrParamPreValueSep)(1))
                        Case 2
                            _PIN = StrDecoding(sInput.Split(chrParamPreValueSep)(2))
                        Case 3
                            _Language = StrDecoding(sInput.Split(chrParamPreValueSep)(3))
                        Case 4
                            _IsOnline = StrDecoding(sInput.Split(chrParamPreValueSep)(4))
                    End Select
                Next
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Employees::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _ID.ToString + chrParamPreValueSep + StrEncoding(_Name) + chrParamPreValueSep + StrEncoding(_PIN) + chrParamPreValueSep + _Language + chrParamPreValueSep + _IsOnline.ToString
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Employees::toString::Error:", ex)
                Return ""
            End Try
        End Function

    End Class

    Public Class msgdata_table_Cards
        Inherits MsgData_Table_generic

        Private _IDEmployee As Integer
        Private _Card As String

        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal Value As Integer)
                _IDEmployee = Value
            End Set
        End Property

        Public Property Card() As String
            Get
                Return _Card
            End Get
            Set(ByVal Value As String)
                _Card = Value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try

                _IDEmployee = sInput.Split(chrParamPreValueSep)(0)
                _Card = sInput.Split(chrParamPreValueSep)(1)
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Cards::toString::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDEmployee.ToString + chrParamPreValueSep + _Card
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Cards::toString::Error:", ex)
                Return ""
            End Try
        End Function

    End Class

    Public Class msgdata_table_Fingers
        Inherits MsgData_Table_generic

        Private _IDEmployee As Integer
        Private _IDFinger As Integer
        Private _Finger As Byte()

        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal Value As Integer)
                _IDEmployee = Value
            End Set
        End Property

        Public Property IDFinger() As Integer
            Get
                Return _IDFinger
            End Get
            Set(ByVal Value As Integer)
                _IDFinger = Value
            End Set
        End Property

        Public Property Finger() As Byte()
            Get
                Return _Finger
            End Get
            Set(ByVal value As Byte())
                _Finger = value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try
                _IDEmployee = sInput.Split(chrParamPreValueSep)(0)
                _IDFinger = sInput.Split(chrParamPreValueSep)(1)
                _Finger = StrByte2Bytes(sInput.Split(chrParamPreValueSep)(2))
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Fingers::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDEmployee.ToString + chrParamPreValueSep + _IDFinger.ToString + chrParamPreValueSep + BytesEncoding(_Finger)
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_Fingers::toString::Error:", ex)
                Return ""
            End Try
        End Function

    End Class

    Public Class MsgData_Table_Transaction
        Inherits MsgData_Table_generic

        'time=2015-05-29 13:58:01	pin=0	cardno=0	eventaddr=0	event=206	inoutstatus=2	verifytype=200	index=822

        Private _PunchDateTime As DateTime = NULLDATE
        Private _EmployeeID As Integer = 0
        Private _Card As String = ""
        Private _EventAddr As Integer
        Private _EventType As Integer
        Private _InOutStatus As Integer
        Private _VerifiedMode As Integer
        Private _Index As Integer

        Public Property PunchDateTime() As DateTime
            Get
                Return _PunchDateTime
            End Get
            Set(ByVal Value As DateTime)
                _PunchDateTime = Value
            End Set
        End Property

        Public Property EmployeeID() As Integer
            Get
                Return _EmployeeID
            End Get
            Set(ByVal Value As Integer)
                _EmployeeID = Value
            End Set
        End Property

        Public Property Card() As String
            Get
                Return _Card
            End Get
            Set(ByVal Value As String)
                _Card = Value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try
                If sInput.Length > 0 Then
                    _PunchDateTime = Any2Date(sInput.Split(chrParamPreValueSep)(0))
                    _Card = sInput.Split(chrParamPreValueSep)(1)
                End If
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table_Offline::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Dim str As String = ""
            Try
                If _Card.Length > 0 Then
                    str = _PunchDateTime + chrParamPreValueSep
                    str += _Card + chrParamPreValueSep
                End If
                Return str
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "MsgData_Table_Offline::toString::Error:", ex)
                Return str
            End Try
        End Function

    End Class

    Public Class msgdata_table_TimeZone
        Inherits MsgData_Table_generic

        Private _IDGroup As Integer
        Private _IDReader As Byte
        Private _WeekDay As Integer
        Private _BeginTime As DateTime
        Private _EndTime As DateTime

        Public Property IDGroup() As Integer
            Get
                Return _IDGroup
            End Get
            Set(ByVal Value As Integer)
                _IDGroup = Value
            End Set
        End Property

        Public Property IDReader() As Integer
            Get
                Return _IDReader
            End Get
            Set(ByVal Value As Integer)
                _IDReader = Value
            End Set
        End Property

        Public Property WeekDay() As Integer
            Get
                Return _WeekDay
            End Get
            Set(ByVal Value As Integer)
                If Value > 6 Then
                    _WeekDay = 0
                Else
                    _WeekDay = Value
                End If
            End Set
        End Property

        Public Property BeginTime() As DateTime
            Get
                Return _BeginTime
            End Get
            Set(ByVal Value As DateTime)
                _BeginTime = Value
            End Set
        End Property

        Public Property EndTime() As DateTime
            Get
                Return _EndTime
            End Get
            Set(ByVal Value As DateTime)
                _EndTime = Value
            End Set
        End Property

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try
                _IDGroup = sInput.Split(chrParamPreValueSep)(0)
                _IDReader = sInput.Split(chrParamPreValueSep)(1)
                _WeekDay = sInput.Split(chrParamPreValueSep)(2)
                _BeginTime = sInput.Split(chrParamPreValueSep)(3)
                _EndTime = sInput.Split(chrParamPreValueSep)(4)
                Return True
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_TimeZone::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDGroup.ToString + chrParamPreValueSep + _IDReader.ToString + chrParamPreValueSep + _WeekDay.ToString + chrParamPreValueSep + _BeginTime.ToString("HH:mm") + chrParamPreValueSep + _EndTime.ToString("HH:mm")
            Catch ex As Exception
                gLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "msgdata_table_TimeZone::toString::Error:", ex)
                Return ""
            End Try
        End Function

    End Class

    Public Class msgdata_table_command
        Inherits MsgData_Table_generic

        Private _IDCommand As Long
        Private _CommandWord As String
        Private _Command As dataCommands
        Private _Table As MessageMxS.msgTables

        Public Enum dataCommands
            DATA_UPDATE
            DATA_DELETE
            DATA_QUERY
            SET_OPTIONS
            GET_OPTIONS
            CONTROL_DEVICE
            NONE
        End Enum

        Public Enum dataParameters
            CardNo
            Pin
            Password
            Group
            StartTime
            EndTime
            All
            Size
            UID
            FingerID
            Valid
            Template
            Resverd
            EndTag
            tablename
            TimezoneId
            AuthorizeTimezoneId
            DateTime
            none
            Unknown
            MachineTZ
            LossCardFlag
            CardType
            FunSwitch
            FirstName
            LastName
            AuthorizeDoorId
            Name
            SuperAuthorize
            Disable
            Year
            _Loop
            ServerTZ
        End Enum

        Private oParameters As Dictionary(Of String, String)

        Public Property IDCommnad() As Long
            Get
                Return _IDCommand
            End Get
            Set(ByVal Value As Long)
                _IDCommand = Value
            End Set
        End Property

        Public Property Command() As dataCommands
            Get
                Return _Command
            End Get
            Set(ByVal Value As dataCommands)
                _Command = Value
            End Set
        End Property

        Public Property Table() As MessageMxS.msgTables
            Get
                Return _Table
            End Get
            Set(ByVal Value As MessageMxS.msgTables)
                _Table = Value
            End Set
        End Property

        Public Sub New()
            oParameters = New Dictionary(Of String, String)
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim tmp As String = ""
            Dim tmp1 As String = ""
            tmp = "C:" + _IDCommand.ToString + ":"
            ' Parseo comando
            Dim cWord As String = 0
            If _Table = MessageMxS.msgTables.none Then
                cWord = _Command.ToString.Replace("_", " ") + " "
            Else
                cWord = _Command.ToString.Replace("_", " ") + " " + _Table.ToString + " "
            End If

            tmp += cWord

            If tmp.Contains("NONE") Then
                tmp = ""
            End If

            For Each item As KeyValuePair(Of String, String) In oParameters
                If tmp1.Length > 0 Then tmp1 += vbTab
                If parseEnum(item.Key) = dataParameters.All Then
                    tmp1 += "*"
                ElseIf parseEnum(item.Key) = dataParameters.none Then
                    tmp1 += item.Value
                Else
                    tmp1 += item.Key.Replace("_", "") + "=" + item.Value
                End If
            Next
            Return tmp + tmp1
        End Function

        Public Function getValue(ByVal Parameter As dataParameters) As String
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    Return oParameters(Parameter.ToString)
                Else
                    Return ""
                End If
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub setValue(ByVal Parameter As dataParameters, Optional ByVal Value As String = "")
            Try
                If oParameters.ContainsKey(Parameter.ToString) Then
                    oParameters(Parameter.ToString) = Value
                Else
                    oParameters.Add(Parameter.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Function parseEnum(ByVal Parameter As String) As dataParameters
            Try
                Return System.Enum.Parse(GetType(dataParameters), Parameter, True)
            Catch ex As Exception
                Return dataParameters.Unknown
            End Try

        End Function

    End Class

    Public Class CommandResult
        Private _ID As Integer
        Private _CMD As String
        Private _ReturnCode As Integer = -1
        Public ReadOnly Property ID As String
            Get
                Return _ID
            End Get
        End Property

        Public ReadOnly Property CMD As String
            Get
                Return _CMD
            End Get
        End Property
        Public ReadOnly Property ReturnCode As Integer
            Get
                Return _ReturnCode
            End Get
        End Property

        Public Sub New(sID As String, sReturnCode As String, sCMD As String)
            _ID = Any2Integer(sID.Trim)
            _ReturnCode = Any2Integer(sReturnCode.Trim)
            _CMD = sCMD.Trim
        End Sub

    End Class

End Namespace