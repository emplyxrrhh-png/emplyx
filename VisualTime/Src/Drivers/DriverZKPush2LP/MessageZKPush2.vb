Imports System.Xml
Imports Robotics.Base
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class MessageZKPush2
        Inherits Robotics.Comms.Base.CMessageBase

        ' Cabecera
        Public Const FIRST_CONNECT_URL As String = "GET /iclock/cdata"
        Public Const REGISTRY_URL As String = "POST /iclock/registry"
        Public Const GET_PUSH_INFO_URL As String = "GET /iclock/push"

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
        Public Const prtTABLE = "TABLE"
        Public Const prtDATA = "DATA"

        Public Enum msgCommand
            init    'Primer mensaje enviado por la centralita
            punch  'Fichaje
            operation  'Estado
            queryresult   'Resultado de una solicitud de datos por parte del servidor
            getrequest   'Solicitud de comandos de configuración
            command
            commandresult   'Resultado de comando
            none  'FaseII: subida de fichero
            reboot  'FaseII: reinicio de terminal
            genericresponse
            info
            biodata  'Información biológica
            punchphoto
            noprocessedtable
        End Enum

        Public Enum msgTables
            USERINFO 'información básica de usuario
            userauthorize 'autorizaciones de usuarios
            holiday 'festivos
            timezone 'periodos de accesos
            transaction 'fichajes
            firstcard 'funcionalidad avanzada FirstCard
            multicard 'funcionalidad avanzada MultiCard
            inoutfun 'Funcionalidad avanzada de Linkage (triggers)
            FINGERTMP 'Biometría v10
            commands 'Comandos a enviar a la centralita
            none
            WORKCODE ' Justificaciones en rx1
            BELLINFO
            AccGroup
            BIODATA
            USERPIC
        End Enum

        Private mID As Integer
        Private mCommand As msgCommand = msgCommand.none
        Private mTable As msgTables = msgTables.none
        Private mData As MsgData
        Private bCorrect As Boolean
        Private bReceived As Boolean
        Private mLastMessageReceived As String
        Private mMessageTimeStamp As DateTime

        Public Property MessageTimeStamp As DateTime
            Get
                Return mMessageTimeStamp
            End Get
            Set(value As DateTime)
                mMessageTimeStamp = value
                Me.mData.MessageDateTime = value
            End Set
        End Property

        Public ReadOnly Property SN As String
            Get
                Return mData.SN
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

        Public Property data_punches() As MsgData_Punches
            Get
                Return CType(mData, MsgData_Punches)
            End Get
            Set(ByVal value As MsgData_Punches)
                mData = value
            End Set
        End Property

        Public Property data_operation() As MsgData_Operation
            Get
                Return CType(mData, MsgData_Operation)
            End Get
            Set(ByVal value As MsgData_Operation)
                mData = value
            End Set
        End Property

        Public Property data_biodata() As MsgData_Biodata
            Get
                Return CType(mData, MsgData_Biodata)
            End Get
            Set(ByVal value As MsgData_Biodata)
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

        Public Property data_punchphoto() As MsgData_PunchPhoto
            Get
                Return CType(mData, MsgData_PunchPhoto)
            End Get
            Set(ByVal value As MsgData_PunchPhoto)
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

        Public Property data_nonprocessedtable() As MsgData_NoProcessedTable
            Get
                Return CType(mData, MsgData_NoProcessedTable)
            End Get
            Set(ByVal value As MsgData_NoProcessedTable)
                mData = value
            End Set
        End Property

        Public Sub New()
            MyBase.New("ZKPush2")
        End Sub

        Public Sub New(ByRef bInput() As Byte)
            MyBase.New("ZKPush2")
            Me.Parse(bInput)
        End Sub

        Public Sub New(ByVal pType As msgCommand, Optional ByVal pTable As msgTables = msgTables.none, Optional ByVal bIsResponse As Boolean = False, Optional sHttpVersion As String = "HTTP/1.1")
            MyBase.New("ZKPush2")
            mCommand = pType
            mTable = pTable
            Select Case mCommand
                Case msgCommand.init
                    If Not bIsResponse Then
                        mData = New MsgData_Init
                    Else
                        mData = New MsgData_InitResponse(sHttpVersion)
                    End If
                Case msgCommand.operation
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.getrequest
                    mData = New MsgData_GenericResponse(sHttpVersion)
                Case msgCommand.punch
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
                If strReceive.Length > 0 Then
#If DEBUG Then
                    roLog.GetInstance.logMessage(VTBase.roLog.EventType.roDebug, "TerminalLogicZKPush2::Received:Message:" + vbCrLf + strReceive)
#End If
                    mLastMessageReceived = strReceive
                    ' 0.Primera conexión
                    If strReceive.Substring(0, FIRST_CONNECT_URL.Length) = FIRST_CONNECT_URL Then
                        strSubData = strReceive.Substring(FIRST_CONNECT_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_Init(strSubData)
                        mCommand = msgCommand.init
                        bCorrect = True
                        Return True
                    End If

                    ' 1.Recepción de datos del terminal
                    '          1.table=OPERLOG
                    '          2.table=ATTLOG
                    If strReceive.Substring(0, UPLOAD_URL.Length) = UPLOAD_URL Then
                        strSubData = strReceive.Substring(UPLOAD_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        If strReceive.IndexOf("table=ATTLOG") > -1 Then
                            ' Fichajes ...
                            mData = New MsgData_Punches(strSubData)
                            mCommand = msgCommand.punch
                            bCorrect = True
                            Return True
                        ElseIf strReceive.IndexOf("table=OPERLOG") > -1 Then
                            ' Alta/Modificación de huellas, empleados, tarjetas, ...
                            mData = New MsgData_Operation(strSubData)
                            mCommand = msgCommand.operation
                            bCorrect = True
                            Return True
                        ElseIf strReceive.IndexOf("table=BIODATA") > -1 Then
                            ' Alta/Modificación de datos de biometría unificada (huellas dactilares, facial, patrón de venas de los dedos, patrón de voz, retina, palma de la mano, ... Requiere versión de protocolo PUSH 2.2.14 o superior
                            mData = New MsgData_Biodata(strSubData)
                            mCommand = msgCommand.biodata
                            bCorrect = True
                            Return True
                        ElseIf strReceive.IndexOf("table=options") > -1 Then
                            ' Información del terminal enviada proactivamente por el mismo
                            mData = New MsgData_CommandResult(strSubData)
                            mCommand = msgCommand.commandresult
                            bCorrect = True
                            Return True
                        ElseIf strReceive.IndexOf("table=ATTPHOTO") > -1 Then
                            ' Foto en fichaje
                            mData = New MsgData_PunchPhoto(strSubData, bInput)
                            mCommand = msgCommand.punchphoto
                            bCorrect = True
                            Return True
                        Else
                            ' Otras tablas, que aún no tratamos ...
                            mData = New MsgData_NoProcessedTable(strSubData)
                            mCommand = msgCommand.noprocessedtable
                            bCorrect = True
                            Return True
                        End If
                        bCorrect = True
                        Return True
                    End If

                    ' 2.Recepción de datos solicitados al terminal
                    If strReceive.Substring(0, QUERY_UPLOAD_URL.Length) = QUERY_UPLOAD_URL Then
                        strSubData = strReceive.Substring(QUERY_UPLOAD_URL.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_QueryResult(strSubData)
                        mCommand = msgCommand.queryresult
                        bCorrect = True
                        Return True
                    End If

                    ' 3.Solicitud de comandos por parte del terminal
                    If strReceive.Substring(0, REQUEST_DATA.Length) = REQUEST_DATA Then
                        strSubData = strReceive.Substring(REQUEST_DATA.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_GetRequest(strSubData)
                        mCommand = msgCommand.getrequest
                        bCorrect = True
                        Return True
                    End If

                    ' 4.Respuesta del terminal con resultado de ejectutar comando enviado desde servidor
                    If strReceive.Substring(0, REPONSE_RESULT.Length) = REPONSE_RESULT Then
                        strSubData = strReceive.Substring(REPONSE_RESULT.Length + REQ_LINE_PARAM_SPLIT_STR.Length)
                        mData = New MsgData_CommandResult(strSubData)
                        mCommand = msgCommand.commandresult
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
                    tmp = prtCMD + chrpreValues + getCommandTypeString(mCommand) + chrBlocks + prtSN + chrpreValues + Me.SN + chrBlocks + prtDATA + chrpreValues + mData.toDebugInfo
                    Return tmp
                Else
                    tmp = prtCMD + chrpreValues + getCommandTypeString(mCommand) + chrBlocks + prtDATA + chrpreValues + mData.toDebugInfo
                    Return tmp
                End If
            Catch ex As Exception
                Return "error getting message info for log!"
            End Try
        End Function

        Private Function getCommandType(ByVal value As String) As msgCommand
            Try

                Select Case value.ToLower
                    Case "init"
                        Return msgCommand.init
                    Case "punch"
                        Return msgCommand.punch
                    Case "reboot"
                        Return msgCommand.reboot
                    Case "operation"
                        Return msgCommand.operation
                    Case "getrequest"
                        Return msgCommand.getrequest
                    Case "queryresult"
                        Return msgCommand.queryresult
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
                    Case msgCommand.punch
                        Return "punch"
                    Case msgCommand.commandresult
                        Return "commandresult"
                    Case msgCommand.queryresult
                        Return "queryresult"
                    Case msgCommand.getrequest
                        Return "getrequest"
                    Case msgCommand.operation
                        Return "operation"
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
        Public Const chrOperDelimiter = " "
        Public Const chrPunchesLine = vbLf
        Public Const chrCommandsResultLine = vbLf
        Public Const chrUploadDataParameters = vbTab
        Public Const chrPunchDataParametersSep = vbTab
        Public Const chrUploadDataParametersValueSep = "="
        Public Const chrDataTableRecord As String = vbCrLf + vbCrLf

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
        Public Const HEADER_RESPONSE_CONTENT_TYPE As String = "Content-Type: text/plain"
        Public Const HEADER_RESPONSE_CONTENT_LEN As String = "Content-Length: "
        Public Const HEADER_RESPONSE_CONNECT As String = "Connection: keep-alive"

        Public Const HEADER_REQ_HEADER_COOKIE = "Cookie: "
        Public Const STRING_ENCODING_FMT = "UTF-8"

        Public Const BG_AUTH_SUCCESS As String = "AUTH=SUCCESS"
        Public Const BG_AUTH_FAILED As String = "AUTH=FAILED"
        Public Const BG_AUTH_CTRL_CMD As String = "CONTROL DEVICE 1 1 1 6"
        Public Const PARK_ONLINE_CTRL_CMD As String = "CONTROL DEVICE 01010101\\\nCONTROL DEVICE 02010124E8AFB7E588B7E58DA1E68896E58F96E58DA1\\\nCONTROL DEVICE 02010224E8AFB7E588B7E58DA1E68896E58F96E58DA1"

        ' Tipos de mensajes de operaciones
        Public Const OPERLOG_OPLOG As String = "OPLOG"
        Public Const OPERLOG_NEWFINGER As String = "FP"
        Public Const OPERLOG_USER As String = "USER"

        ' Tipos de mensajes BIODATA
        Public Const BIODATA_FACE As String = "FACE"
        Public Const BIODATA_NEW As String = "BIODATA"

        ' Propiedades
        Protected _ResponseStatus As dataStatus
        Protected _HttpVersion As String
        Protected _Message As String
        Protected _SN As String
        Protected _SessionID As String
        Protected _Token As String
        Protected _PunchStamp As String
        Protected _OperStamp As String
        Protected _MessageDatetime As DateTime

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

        Public Sub New()
            _MessageDatetime = Now
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

        Public Property MessageDateTime As DateTime
            Get
                Return _MessageDatetime
            End Get
            Set(value As DateTime)
                _MessageDatetime = value
            End Set
        End Property

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
            Return Me._httpContent
        End Function

    End Class

    ''' <summary>
    ''' Primera conexión
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MsgData_InitResponse
        Inherits MsgData

        Public Enum dataParameters
            Stamp
            OpStamp
            PhotoStamp
            TransFlag
            ErrorDelay
            Delay
            TimeZone
            TransTimes
            TransInterval
            SyncTime
            Realtime
            ServerVer
            ATTLOGStamp
            OPERLOGStamp
            ATTPHOTOStamp
            Unknown
            Encrypt
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
            tmp = "GET OPTION FROM:" + _SN + chrNewParametersLine
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

    Public Class MsgData_Punches
        Inherits MsgData
        Private _Punches As Generic.List(Of MsgData_Table_punch)

        Public ReadOnly Property PunchStamp() As String
            Get
                Return _PunchStamp
            End Get
        End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property Punches As Generic.List(Of MsgData_Table_punch)
            Get
                Return _Punches
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            _Punches = New Generic.List(Of MsgData_Table_punch)
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
                _PunchStamp = p.Substring(p.IndexOf("Stamp=") + 6, p.IndexOf(" ") - p.IndexOf("Stamp=") - 6)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                If _httpContent.Length > 0 Then
                    For Each sPunchLine As String In _httpContent.Split(chrPunchesLine)
                        If sPunchLine.Trim <> "" Then
                            Dim oPunch As New MsgData_Table_punch
                            Dim iLen As Integer = 0
                            iLen = sPunchLine.Split(chrPunchDataParametersSep).Count - 1
                            If iLen >= 0 Then oPunch.PIN = sPunchLine.Split(chrPunchDataParametersSep)(0).ToString
                            If iLen >= 1 Then oPunch.PunchDateTime = Any2Date(sPunchLine.Split(chrPunchDataParametersSep)(1))
                            If iLen >= 2 Then oPunch.PunchStatus = sPunchLine.Split(chrPunchDataParametersSep)(2)
                            If iLen >= 3 Then oPunch.VerifyMode = sPunchLine.Split(chrPunchDataParametersSep)(3)
                            If iLen >= 4 Then oPunch.WorkCode = sPunchLine.Split(chrPunchDataParametersSep)(4)
                            If iLen >= 5 Then oPunch.Reserved1 = sPunchLine.Split(chrPunchDataParametersSep)(5)
                            If iLen >= 6 Then oPunch.Reserved2 = sPunchLine.Split(chrPunchDataParametersSep)(6)
                            If iLen >= 7 Then oPunch.WearingMask = sPunchLine.Split(chrPunchDataParametersSep)(7)
                            If iLen >= 8 Then oPunch.Temperature = sPunchLine.Split(chrPunchDataParametersSep)(8)
                            oPunch.Reader = 1
                            _Punches.Add(oPunch)
                            oPunch = Nothing
                        End If
                    Next
                End If
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim res As String = ""
            For Each opunch As MsgData_Table_punch In Me._Punches
                res = res & "(" & opunch.PIN & "," & opunch.PunchDateTime.ToString & "," & opunch.WorkCode & "," & opunch.VerifyMode & ")"
            Next
            Return res
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_NoProcessedTable
        Inherits MsgData

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
                _PunchStamp = p.Substring(p.IndexOf("Stamp=") + 6, p.IndexOf(" ") - p.IndexOf("Stamp=") - 6)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData::GetMessageInfo::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Return _httpContent
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_Table_punch
        'Variables de mensaje recibido
        Private _PIN As String = ""
        Private _PunchDateTime As DateTime
        Private _Status As clsPushProtocol.dataPunchStatus
        Private _VerifyMode As DTOs.VerificationType 'clsPushProtocol.dataVerifyType
        Private _WorkCode As String = ""
        Private _Reserved1 As String = ""
        Private _Reserved2 As String = ""
        Private _WearingMask As String = ""
        Private _Temperature As String = ""

        'Variables funcionales
        Private _Reader As Integer = 0
        Private _Action As String = ""
        Private _IsPunch As Boolean = False

        Public Property Reader
            Get
                Return _Reader
            End Get
            Set(value)
                _Reader = value
            End Set
        End Property

        Public Property Action
            Get
                Return _Action
            End Get
            Set(value)
                _Action = value
            End Set
        End Property

        Public Property PunchDateTime() As DateTime
            Get
                Return _PunchDateTime
            End Get
            Set(value As DateTime)
                _PunchDateTime = value
            End Set
        End Property

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(value As String)
                _PIN = value
            End Set
        End Property

        Public Property PunchStatus() As clsPushProtocol.dataPunchStatus
            Get
                Return _Status
            End Get
            Set(value As clsPushProtocol.dataPunchStatus)
                _Status = value
            End Set
        End Property

        Public Property VerifyMode() As DTOs.VerificationType 'clsPushProtocol.dataVerifyType
            Get
                Return _VerifyMode
            End Get
            Set(value As DTOs.VerificationType)
                _VerifyMode = value
            End Set
        End Property

        Public Property WorkCode() As String
            Get
                Return _WorkCode
            End Get
            Set(value As String)
                _WorkCode = value
            End Set
        End Property

        Public Property Reserved1() As String
            Get
                Return _Reserved1
            End Get
            Set(value As String)
                _Reserved1 = value
            End Set
        End Property

        Public Property Reserved2() As String
            Get
                Return _Reserved2
            End Get
            Set(value As String)
                _Reserved2 = value
            End Set
        End Property

        Public Property WearingMask() As String
            Get
                Return _WearingMask
            End Get
            Set(value As String)
                _WearingMask = value
            End Set
        End Property

        Public Property Temperature() As String
            Get
                Return _Temperature
            End Get
            Set(value As String)
                _Temperature = value
            End Set
        End Property

        Public ReadOnly Property IsPunch() As Boolean
            Get
                Return (roTypes.Any2Long(_PIN) > 0)
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(sPin As String, dPunchDateTime As DateTime, sStatus As String, sVerifyMode As String, sWorkCode As String, iReader As Integer, sReserved1 As String, sReserved2 As String)
            _PIN = sPin
            _PunchDateTime = dPunchDateTime
            _Status = sStatus
            _VerifyMode = roTypes.Any2Long(sVerifyMode)
            _WorkCode = sWorkCode
            _Reserved1 = sReserved1
            _Reserved2 = sReserved2
            _Reader = iReader
        End Sub

        Public Function ToLogInfo() As String
            Dim str As String = String.Empty
            str = vbCr
            str = str & "IDEmployee: " & _PIN & vbCr
            str = str & "DateTime: " & _PunchDateTime.ToString & vbCr
            str = str & "Status: " & _Status & vbCr
            str = str & "VerifyMode: " & [Enum].GetName(GetType(DTOs.VerificationType), _VerifyMode)
            str = str & "WorkCode: " & _WorkCode & vbCr
            str = str & "Reader: " & _Reader.ToString & vbCr
            str = str & "WearingMask: " & _WearingMask.ToString & vbCr
            str = str & "Temperature: " & _Temperature.ToString & vbCr
            Return str
        End Function

    End Class

    Public Class MsgData_Operation
        Inherits MsgData
        Private _Employees As Generic.List(Of MsgData_Table_employee)
        Private _Oplog As Generic.List(Of MsgData_Table_operation)
        Private _Fingers As Generic.List(Of MsgData_Table_finger)

        Public ReadOnly Property OpStamp() As String
            Get
                Return _OperStamp
            End Get
        End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property Employees As Generic.List(Of MsgData_Table_employee)
            Get
                Return _Employees
            End Get
        End Property

        Public ReadOnly Property OperationLog As Generic.List(Of MsgData_Table_operation)
            Get
                Return _Oplog
            End Get
        End Property

        Public ReadOnly Property Fingers As Generic.List(Of MsgData_Table_finger)
            Get
                Return _Fingers
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            _Employees = New Generic.List(Of MsgData_Table_employee)
            _Oplog = New Generic.List(Of MsgData_Table_operation)
            _Fingers = New Generic.List(Of MsgData_Table_finger)
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
                _OperStamp = p.Substring(p.IndexOf("Stamp=") + 6, p.IndexOf(" ") - p.IndexOf("Stamp=") - 6)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                If _httpContent.Length > 0 Then
                    For Each sOperLogLine As String In _httpContent.Split(chrPunchesLine)
                        If sOperLogLine.Trim <> "" Then
                            Select Case sOperLogLine.Split(chrOperDelimiter)(0).ToString
                                Case OPERLOG_USER
                                    Dim oEmployee As New MsgData_Table_employee
                                    'TODO proteger para el caso en que pueda llegar algún parámetro menos -> out of index
                                    sOperLogLine = sOperLogLine.Substring(OPERLOG_USER.Length, sOperLogLine.Length - OPERLOG_USER.Length).Trim()
                                    oEmployee.PIN = sOperLogLine.Split(chrPunchDataParametersSep)(0).ToString.Split("=")(1)
                                    oEmployee.Name = sOperLogLine.Split(chrPunchDataParametersSep)(1).ToString.Split("=")(1)
                                    oEmployee.Privilege = sOperLogLine.Split(chrPunchDataParametersSep)(2).ToString.Split("=")(1)
                                    oEmployee.Password = sOperLogLine.Split(chrPunchDataParametersSep)(3).ToString.Split("=")(1)
                                    oEmployee.Card = sOperLogLine.Split(chrPunchDataParametersSep)(4).ToString.Split("=")(1)
                                    oEmployee.Group = sOperLogLine.Split(chrPunchDataParametersSep)(5).ToString.Split("=")(1)
                                    oEmployee.Timezone = sOperLogLine.Split(chrPunchDataParametersSep)(6).ToString.Split("=")(1)
                                    _Employees.Add(oEmployee)
                                    oEmployee = Nothing
                                Case OPERLOG_OPLOG
                                    Dim oOperation As New MsgData_Table_operation
                                    'TODO proteger para el caso en que pueda llegar algún parámetro menos -> out of index
                                    sOperLogLine = sOperLogLine.Substring(OPERLOG_OPLOG.Length, sOperLogLine.Length - OPERLOG_OPLOG.Length).Trim()
                                    oOperation.OperationID = sOperLogLine.Split(chrPunchDataParametersSep)(0).ToString
                                    oOperation.AdminID = sOperLogLine.Split(chrPunchDataParametersSep)(1).ToString
                                    oOperation.OperationTime = Any2Date(sOperLogLine.Split(chrPunchDataParametersSep)(2))
                                    oOperation.OperationParameter1 = sOperLogLine.Split(chrPunchDataParametersSep)(3)
                                    oOperation.OperationParameter2 = sOperLogLine.Split(chrPunchDataParametersSep)(4)
                                    oOperation.OperationParameter3 = sOperLogLine.Split(chrPunchDataParametersSep)(5)
                                    oOperation.OperationParameter4 = sOperLogLine.Split(chrPunchDataParametersSep)(6)
                                    _Oplog.Add(oOperation)
                                    oOperation = Nothing
                                Case OPERLOG_NEWFINGER
                                    Dim oFinger As New MsgData_Table_finger
                                    'TODO proteger para el caso en que pueda llegar algún parámetro menos -> out of index
                                    sOperLogLine = sOperLogLine.Substring(OPERLOG_NEWFINGER.Length, sOperLogLine.Length - OPERLOG_NEWFINGER.Length).Trim()
                                    oFinger.PIN = sOperLogLine.Split(chrPunchDataParametersSep)(0).ToString.Split("=")(1)
                                    oFinger.FID = sOperLogLine.Split(chrPunchDataParametersSep)(1).ToString.Split("=")(1)
                                    oFinger.Size = sOperLogLine.Split(chrPunchDataParametersSep)(2).ToString.Split("=")(1)
                                    oFinger.Valid = sOperLogLine.Split(chrPunchDataParametersSep)(3).ToString.Split("=")(1)
                                    'oFinger.TMP = sOperLogLine.Split(chrPunchDataParametersSep)(4).ToString.Split("TMP=")(1)
                                    'Comoo el separador de parámetros es el "=", que también se usa para completar la cadena Base64 de la huella, ¡¡¡pierdo esos caracteres de relleno!!!. Los recupero ahora
                                    'oFinger.TMP = oFinger.TMP.PadRight(oFinger.Size, "=")
                                    oFinger.TMP = sOperLogLine.Split(chrPunchDataParametersSep)(4).ToString.Split(New String() {"TMP="}, StringSplitOptions.None)(1)
                                    _Fingers.Add(oFinger)
                                    oFinger = Nothing
                            End Select
                        End If
                    Next
                End If
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_Operation::Parse::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim res As String = ""
            For Each oEmployee As MsgData_Table_employee In Me._Employees
                res = res & "(" & oEmployee.PIN & "," & oEmployee.Name & "," & oEmployee.Card & "," & oEmployee.Group & "," & oEmployee.Timezone & ")"
            Next
            For Each oOpLog As MsgData_Table_operation In Me._Oplog
                res = res & "(" & oOpLog.OperationID & "," & oOpLog.AdminID & "," & oOpLog.OperationTime.ToShortTimeString & "," & oOpLog.OperationParameter1 & "," & oOpLog.OperationParameter2 & "," & oOpLog.OperationParameter3 & "," & oOpLog.OperationParameter4 & ")"
            Next
            For Each oFinger As MsgData_Table_finger In Me._Fingers
                res = res & "(" & oFinger.PIN & "," & oFinger.FID & "," & oFinger.Size & "," & oFinger.Valid & "," & oFinger.TMP & ")"
            Next
            Return res
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_Biodata
        Inherits MsgData
        Private _BioElement As Generic.List(Of MsgData_Table_biodata)

        Public ReadOnly Property OpStamp() As String
            Get
                Return _OperStamp
            End Get
        End Property

        Public ReadOnly Property HttpVersion() As String
            Get
                Return _HttpVersion
            End Get
        End Property

        Public ReadOnly Property Bioelements As Generic.List(Of MsgData_Table_biodata)
            Get
                Return _BioElement
            End Get
        End Property

        Public ReadOnly Property HttpContent() As String
            Get
                Return _httpContent
            End Get
        End Property

        Public Sub New(ByVal Input As String)
            MyBase.New()
            _BioElement = New Generic.List(Of MsgData_Table_biodata)
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
                _OperStamp = p.Substring(p.IndexOf("Stamp=") + 6, p.IndexOf(" ") - p.IndexOf("Stamp=") - 6)

                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)

                If _httpContent.Length > 0 Then
                    For Each sBiodataLine As String In _httpContent.Split(chrPunchesLine)
                        If sBiodataLine.Trim <> "" Then
                            Dim oFace As New MsgData_Table_biodata
                            sBiodataLine = sBiodataLine.Substring(BIODATA_NEW.Length, sBiodataLine.Length - BIODATA_NEW.Length).Trim()
                            oFace.PIN = sBiodataLine.Split(chrPunchDataParametersSep)(0).ToString.Split("=")(1)
                            oFace.No = sBiodataLine.Split(chrPunchDataParametersSep)(1).ToString.Split("=")(1)
                            oFace.Index = sBiodataLine.Split(chrPunchDataParametersSep)(2).ToString.Split("=")(1)
                            oFace.Valid = sBiodataLine.Split(chrPunchDataParametersSep)(3).ToString.Split("=")(1)
                            oFace.Duress = sBiodataLine.Split(chrPunchDataParametersSep)(4).ToString.Split("=")(1)
                            oFace.Type = sBiodataLine.Split(chrPunchDataParametersSep)(5).ToString.Split("=")(1)
                            oFace.MajorVer = sBiodataLine.Split(chrPunchDataParametersSep)(6).ToString.Split("=")(1)
                            oFace.MinorVer = sBiodataLine.Split(chrPunchDataParametersSep)(7).ToString.Split("=")(1)
                            oFace.Format = sBiodataLine.Split(chrPunchDataParametersSep)(8).ToString.Split("=")(1)
                            oFace.TMP = sBiodataLine.Substring(sBiodataLine.IndexOf(chrPunchDataParametersSep & "Tmp=") + 5, sBiodataLine.Length - sBiodataLine.IndexOf(chrPunchDataParametersSep & "Tmp=") - 5).Trim()
                            _BioElement.Add(oFace)
                            oFace = Nothing
                        End If
                    Next
                End If
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_Operation::Parse::Error:", ex)
                Return False
            End Try
            Return True
        End Function

        Public Overrides Function toString() As String
            Dim res As String = ""
            For Each oFace As MsgData_Table_biodata In Me._BioElement
                res = res & "(" & oFace.PIN & "," & oFace.No & "," & oFace.Index & "," & oFace.Valid & "," & oFace.TMP & ")"
            Next
            Return res
        End Function

        Public Overrides Function toDebugInfo() As String
            Return Me.toString
        End Function

    End Class

    Public Class MsgData_Table_employee
        'Variables de mensaje recibido
        Private _PIN As String = ""
        Private _Name As String = ""
        Private _Privilege As String = ""
        Private _Password As String = ""
        Private _Card As String = ""
        Private _Group As String = ""
        Private _Timezone As String = ""

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(value As String)
                _PIN = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        Public Property Privilege() As String
            Get
                Return _Privilege
            End Get
            Set(value As String)
                _Privilege = value
            End Set
        End Property

        Public Property Password() As String
            Get
                Return _Password
            End Get
            Set(value As String)
                _Password = value
            End Set
        End Property

        Public Property Card() As String
            Get
                Return _Card
            End Get
            Set(value As String)
                _Card = value
            End Set
        End Property

        Public Property Group() As String
            Get
                Return _Group
            End Get
            Set(value As String)
                _Group = value
            End Set
        End Property

        Public Property Timezone() As String
            Get
                Return _Timezone
            End Get
            Set(value As String)
                _Timezone = value
            End Set
        End Property
    End Class

    Public Class MsgData_Table_operation
        'Variables de mensaje recibido
        Private _OperationID As clsPushProtocol.dataOperations
        Private _AdminID As String = ""
        Private _OperationTime As DateTime
        Private _OperationParameter1 As String = ""
        Private _OperationParameter2 As String = ""
        Private _OperationParameter3 As String = ""
        Private _OperationParameter4 As String = ""

        Public Property OperationID() As clsPushProtocol.dataOperations
            Get
                Return _OperationID
            End Get
            Set(value As clsPushProtocol.dataOperations)
                _OperationID = value
            End Set
        End Property

        Public Property AdminID() As String
            Get
                Return _AdminID
            End Get
            Set(value As String)
                _AdminID = value
            End Set
        End Property

        Public Property OperationTime() As DateTime
            Get
                Return _OperationTime
            End Get
            Set(value As DateTime)
                _OperationTime = value
            End Set
        End Property

        Public Property OperationParameter1() As String
            Get
                Return _OperationParameter1
            End Get
            Set(value As String)
                _OperationParameter1 = value
            End Set
        End Property

        Public Property OperationParameter2() As String
            Get
                Return _OperationParameter2
            End Get
            Set(value As String)
                _OperationParameter2 = value
            End Set
        End Property

        Public Property OperationParameter3() As String
            Get
                Return _OperationParameter3
            End Get
            Set(value As String)
                _OperationParameter3 = value
            End Set
        End Property

        Public Property OperationParameter4() As String
            Get
                Return _OperationParameter4
            End Get
            Set(value As String)
                _OperationParameter4 = value
            End Set
        End Property

        Public Overrides Function Tostring() As String
            Try
                Return "(" & Me.AdminID & "," & [Enum].Parse(GetType(clsPushProtocol.dataOperations), Me.OperationID).ToString & "," & Me.OperationParameter1 & "," & Me.OperationParameter2 & "," & Me.OperationParameter3 & "," & Me.OperationParameter4 & "," & Me.OperationTime & ")"
            Catch ex As Exception
                Return " error retrieving text! "
            End Try
        End Function

    End Class

    Public Class MsgData_Table_finger
        'Variables de mensaje recibido
        Private _PIN As String = ""
        Private _FID As String = ""
        Private _Size As String = ""
        Private _Valid As String = ""
        Private _TMP As String = ""
        Private _TimeStamp As DateTime
        Private _EnrollerID As Integer

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(value As String)
                _PIN = value
            End Set
        End Property

        Public Property FID() As String
            Get
                Return _FID
            End Get
            Set(value As String)
                _FID = value
            End Set
        End Property

        Public Property Size() As String
            Get
                Return _Size
            End Get
            Set(value As String)
                _Size = value
            End Set
        End Property

        Public Property Valid() As String
            Get
                Return _Valid
            End Get
            Set(value As String)
                _Valid = value
            End Set
        End Property

        Public Property TMP() As String
            Get
                Return _TMP
            End Get
            Set(value As String)
                _TMP = value
            End Set
        End Property

        Public Property TimeStamp As DateTime
            Get
                Return _TimeStamp
            End Get
            Set(value As DateTime)
                _TimeStamp = value
            End Set
        End Property

        Public Property EnrollerID As Integer
            Get
                Return _EnrollerID
            End Get
            Set(value As Integer)
                _EnrollerID = value
            End Set
        End Property

    End Class

    Public Class MsgData_Table_biodata
        'Variables de mensaje recibido
        Private _PIN As String = ""
        Private _No As String = ""
        Private _Index As String = ""
        Private _Valid As String = ""
        Private _Duress As String = ""
        Private _Type As String = ""
        Private _MajorVer As String = ""
        Private _MinorVer As String = ""
        Private _Format As String = ""
        Private _TMP As String = ""
        Private _TimeStamp As DateTime

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(value As String)
                _PIN = value
            End Set
        End Property

        Public Property No() As String
            Get
                Return _No
            End Get
            Set(value As String)
                _No = value
            End Set
        End Property

        Public Property Index() As String
            Get
                Return _Index
            End Get
            Set(value As String)
                _Index = value
            End Set
        End Property

        Public Property Valid() As String
            Get
                Return _Valid
            End Get
            Set(value As String)
                _Valid = value
            End Set
        End Property

        Public Property Duress() As String
            Get
                Return _Duress
            End Get
            Set(value As String)
                _Duress = value
            End Set
        End Property

        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(value As String)
                _Type = value
            End Set
        End Property

        Public Property MajorVer() As String
            Get
                Return _MajorVer
            End Get
            Set(value As String)
                _MajorVer = value
            End Set
        End Property

        Public Property MinorVer() As String
            Get
                Return _MinorVer
            End Get
            Set(value As String)
                _MinorVer = value
            End Set
        End Property

        Public Property Format() As String
            Get
                Return _Format
            End Get
            Set(value As String)
                _Format = value
            End Set
        End Property

        Public Property TMP() As String
            Get
                Return _TMP
            End Get
            Set(value As String)
                _TMP = value
            End Set
        End Property

        Public Property TimeStamp As DateTime
            Get
                Return _TimeStamp
            End Get
            Set(value As DateTime)
                _TimeStamp = value
            End Set
        End Property

    End Class

    Public Class MsgData_GetRequest
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

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String)
            MyBase.New()
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
            Catch e As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_QueryResult::Parse::Error:", e)
                Return False
            End Try

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
        Private oDeviceParameters As Dictionary(Of String, String)

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
            oResults = New ArrayList
            oDeviceParameters = New Dictionary(Of String, String)
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
            Dim sLastParameter As String = String.Empty

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
            Catch e As Exception
                Return False
            End Try

            ' Aquí cargo los valores de configuración enviados por el terminal
            Try
                For Each item As String In _httpContent.Split(chrCommandsResultLine)
                    If item.IndexOf("&") > -1 Then
                        oResults.Add(New CommandResult(item.Split("&")(0).Split("=")(1), item.Split("&")(1).Split("=")(1), item.Split("&")(2).Split("=")(1)))
                    Else
                        If item.IndexOf(",") > -1 Then
                            ' Respuesta a GET_OPTIONS, En principio sólo compatible con rx1. Ejemplo de respuesta:  GATEIPAddress = 192.168.16.1,ICLOCKSVRURL=192.168.16.70:8007
                            For Each rx1Item As String In item.Split(",")
                                If rx1Item.Split("=").Count = 2 Then
                                    oDeviceParameters.Add(rx1Item.Split("=")(0), rx1Item.Split("=")(1))
                                End If
                            Next
                        Else
                            If item.IndexOf("=") > -1 Then
                                ' Respuesta a INFO (para cualquier terminal PUSH)
                                If item.Split("=").Count = 2 Then
                                    sLastParameter = item.Split("=")(0)
                                    oDeviceParameters.Add(item.Split("=")(0), item.Split("=")(1))
                                End If
                            ElseIf sLastParameter = "Content" AndAlso oDeviceParameters.ContainsKey("Content") Then
                                oDeviceParameters("Content") = oDeviceParameters("Content") & Environment.NewLine & item
                            End If
                        End If
                    End If
                Next
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Return String.Empty
        End Function

        Public Overrides Function toDebugInfo() As String
            Return mdPublic.AnonimizeLog(Me.HttpContent)
        End Function

    End Class

    Public Class MsgData_PunchPhoto
        Inherits MsgData

        Private _Photo As Byte()
        Private _PIN As Integer
        Private _PunchDateTime As Date
        Private _Command As String
        Private _Filename As String
        Private _Size As Integer

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

        Public ReadOnly Property PIN As Integer
            Get
                Return _PIN
            End Get
        End Property

        Public ReadOnly Property Size As Integer
            Get
                Return _Size
            End Get
        End Property

        Public ReadOnly Property Photo As Byte()
            Get
                Return _Photo
            End Get
        End Property

        Public ReadOnly Property PunchDateTime As Date
            Get
                Return _PunchDateTime
            End Get
        End Property
        Public ReadOnly Property Filename As String
            Get
                Return _Filename
            End Get
        End Property

        Public ReadOnly Property Command As String
            Get
                Return _Command
            End Get
        End Property

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByVal sData As String, ByVal bData As Byte())
            MyBase.New()
            Dim sDataAux As String = String.Empty
            sDataAux = sData.Split(vbNullChar)(0)
            ParseEx(sDataAux, bData)
        End Sub

        Public Overrides Function Parse(sInput As String) As Boolean
            'NOT NEEDED
            Return True
        End Function

        Public Function ParseEx(ByVal sInput As String, ByVal bData As Byte()) As Boolean
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

                ' Versión de http
                endIdx = p.IndexOf(" ")
                _HttpVersion = p.Substring(endIdx + 1)

                ' Cuerpo del mensaje
                startIdx = sInput.IndexOf(HEADER_END_STR) + HEADER_END_STR.Length
                _httpContent = sInput.Substring(startIdx)
            Catch e As Exception
                Return False
            End Try

            ' Aquí cargo los valores de configuración enviados por el terminal
            Try
                Dim sKey As String = String.Empty
                Dim sValue As String = String.Empty
                For Each item As String In _httpContent.Split(chrCommandsResultLine)
                    If item.Split("=").Count > 0 Then
                        sKey = item.Split("=")(0)
                        sValue = item.Split("=")(1)
                    End If
                    Select Case sKey.ToUpper
                        Case "PIN"
                            _Filename = sValue
                        Case "SIZE"
                            _Size = sValue
                        Case "CMD"
                            _Command = sValue
                    End Select
                Next

                If _Filename <> String.Empty Then
                    _PIN = _Filename.Split(".")(0).Split("-")(1)
                    _PunchDateTime = DateTime.ParseExact(_Filename.Split(".")(0).Split("-")(0), "yyyyMMddHHmmss", Nothing)
                End If

                Dim imgrecindex As Integer = 0
                For x As Integer = 0 To bData.Length - 1
                    imgrecindex = x
                    If CInt(bData(x)) = 0 Then Exit For
                Next
                imgrecindex += 1

                Dim imgReceive As Byte() = New Byte(bData.Length - imgrecindex - 1) {}
                Array.Copy(bData, imgrecindex, imgReceive, 0, bData.Length - imgrecindex)
                _Photo = imgReceive
                'System.IO.File.WriteAllBytes("C:\TMP\test.jpeg", imgReceive)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Return String.Empty
        End Function

        Public Overrides Function toDebugInfo() As String
            Return mdPublic.AnonimizeLog(Me.HttpContent)
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

        Private mTable As MessageZKPush2.msgTables
        Private mData() As MsgData_Table_generic
        Private index As Integer = 0
        Private offset As Integer = 0

        Public Sub New(ByVal Table As MessageZKPush2.msgTables, Optional sHttpVersion As String = "")
            MyBase.New()
            mTable = Table
            _HttpVersion = sHttpVersion
            Select Case mTable
                Case MessageZKPush2.msgTables.transaction
                    mData = Array.CreateInstance(GetType(MsgData_Table_Transaction), 20)
                    For i As Byte = 0 To 19
                        mData(i) = New MsgData_Table_Transaction
                    Next
                Case MessageZKPush2.msgTables.USERINFO
                    mData = Array.CreateInstance(GetType(msgdata_table_Employees), 50)
                Case MessageZKPush2.msgTables.FINGERTMP
                    mData = Array.CreateInstance(GetType(msgdata_table_Fingers), 50)
                Case MessageZKPush2.msgTables.timezone
                    mData = Array.CreateInstance(GetType(msgdata_table_TimeZone), 50)
                Case MessageZKPush2.msgTables.firstcard
                Case MessageZKPush2.msgTables.holiday
                Case MessageZKPush2.msgTables.inoutfun
                Case MessageZKPush2.msgTables.multicard
                Case MessageZKPush2.msgTables.userauthorize
                    mData = Array.CreateInstance(GetType(msgdata_table_TimeZone), 50)
                Case MessageZKPush2.msgTables.commands
                    mData = Array.CreateInstance(GetType(msgdata_table_command), 200)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::adddel::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addEmployee::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addFinger::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addTimezone::Error:", ex)
            End Try
        End Sub

        Public Sub addCommand(ByVal IDCommand As Long, ByVal oCommand As msgdata_table_command.dataCommands, ByVal oTable As MessageZKPush2.msgTables, Optional bNewSyncTask As Boolean = False)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addCommand::Error:", ex)
            End Try
        End Sub

        Public Sub addCommandParameters(ByVal Key As msgdata_table_command.dataParameters, ByVal value As String)
            Try
                CType(mData(index - 1), msgdata_table_command).setValue(Key, value)
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addCommandParameters::Error:", ex)
            End Try
        End Sub

        Public Sub addCommandParametersEx(ByVal Key As String, ByVal value As String)
            Try
                CType(mData(index - 1), msgdata_table_command).setValueEx(Key, value)
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::addCommandParameters::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::parse::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table::toString::Error:", ex)
                Return ""
            End Try
        End Function

        Public Overrides Function toDebugInfo() As String
            Return mdPublic.AnonimizeLog(HttpContent)
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
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Employees::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _ID.ToString + chrParamPreValueSep + StrEncoding(_Name) + chrParamPreValueSep + StrEncoding(_PIN) + chrParamPreValueSep + _Language + chrParamPreValueSep + _IsOnline.ToString
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Employees::toString::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Cards::toString::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDEmployee.ToString + chrParamPreValueSep + _Card
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Cards::toString::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Fingers::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDEmployee.ToString + chrParamPreValueSep + _IDFinger.ToString + chrParamPreValueSep + BytesEncoding(_Finger)
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_Fingers::toString::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table_Offline::parse::Error:", ex)
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
                gLog.logMessage(roLog.EventType.roError, "MsgData_Table_Offline::toString::Error:", ex)
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

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Function Parse(ByVal sInput As String) As Boolean
            Try
                _IDGroup = sInput.Split(chrParamPreValueSep)(0)
                _IDReader = sInput.Split(chrParamPreValueSep)(1)
                _WeekDay = sInput.Split(chrParamPreValueSep)(2)
                _BeginTime = sInput.Split(chrParamPreValueSep)(3)
                _EndTime = sInput.Split(chrParamPreValueSep)(4)
                Return True
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_TimeZone::parse::Error:", ex)
                Return False
            End Try
        End Function

        Public Overrides Function toString() As String
            Try
                Return _IDGroup.ToString + chrParamPreValueSep + _IDReader.ToString + chrParamPreValueSep + _WeekDay.ToString + chrParamPreValueSep + _BeginTime.ToString("HH:mm") + chrParamPreValueSep + _EndTime.ToString("HH:mm")
            Catch ex As Exception
                gLog.logMessage(roLog.EventType.roError, "msgdata_table_TimeZone::toString::Error:", ex)
                Return ""
            End Try
        End Function

    End Class

    Public Class msgdata_table_command
        Inherits MsgData_Table_generic

        Private _IDCommand As Long
        Private _CommandWord As String
        Private _Command As dataCommands
        Private _Table As MessageZKPush2.msgTables

        Public Enum dataCommands
            DATA_UPDATE
            DATA_DELETE
            DATA_QUERY
            SET_OPTION
            GET_OPTION
            CONTROL_DEVICE
            CLEAR_DATA
            SetTZInfo
            SetUserTZStr
            DelUserTZStr
            REBOOT
            CHECK
            INFO
            LOG
            SHELL
        End Enum

        Public Enum dataParameters
            Card
            PIN
            Passwd
            Grp
            Name
            StartTime
            EndTime
            All
            Size
            UID
            FID
            Valid
            TMP
            Resverd
            EndTag
            tablename
            TimezoneId
            AuthorizeTimezoneId
            DateTime
            Pri '0=Usuario normal, 2=Enroll, 14=Super Administrator
            TZ
            TZs
            none
            Unknown
            ID
            No
            Index
            Duress
            Type
            MajorVer
            MinorVer
            Format
            Content
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

        Public Property Table() As MessageZKPush2.msgTables
            Get
                Return _Table
            End Get
            Set(ByVal Value As MessageZKPush2.msgTables)
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

            If _Table = MessageZKPush2.msgTables.none Then
                cWord = _Command.ToString.Replace("_", " ") + " "
            Else
                cWord = _Command.ToString.Replace("_", " ") + " " + _Table.ToString + " "
            End If

            tmp += cWord

            For Each item As KeyValuePair(Of String, String) In oParameters
                If tmp1.Length > 0 Then tmp1 += vbTab
                If parseEnum(item.Key) = dataParameters.All Then
                    tmp1 += "*"
                ElseIf parseEnum(item.Key) = dataParameters.none Then
                    tmp1 += item.Value
                Else
                    tmp1 += item.Key + "=" + item.Value
                End If
            Next
            Return tmp + tmp1 + vbLf
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

        Public Sub setValueEx(ByVal Parameter As String, Optional ByVal Value As String = "")
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
            _ID = roTypes.Any2Integer(sID.Trim)
            _ReturnCode = roTypes.Any2Integer(sReturnCode.Trim)
            _CMD = sCMD.Trim
        End Sub

    End Class

End Namespace