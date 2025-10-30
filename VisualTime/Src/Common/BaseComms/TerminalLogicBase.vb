Public MustInherit Class CTerminalLogicBase

    Public Enum TerminalTaskAction
        Insert
        Update
        Delete
        Syncronisation
        LoadUsers
        GetFinger
        PutFinger
        'mx7
        addemployee
        delemployee
        delallemployees
        addphoto
        delphoto
        delallphotos
        addbio
        delbio
        delallbios
        addcard
        delcard
        delallcards
    End Enum


    Public Enum CommServerEventID
        InitServices = 1
        OpenListener = 2
        OpenConnection = 3
        ConnectionOK = 4
        ConnectionError = 5
        DisconectionError = 6
        Disconection = 7
        UnknownError = 99
    End Enum

    Public Enum mxEventID
        IdentifiedTerminal = 100
        OnlinePunch = 110
        OfflinePunch = 111
        PunchSaved = 112
        AddEmployee = 120
        DelEmployee = 121
        ResetEmployees = 122
        AddCard = 123
        DelCard = 124
        ResetCards = 125
        AddBio = 126
        DelBio = 127
        ResetBio = 128
        AddEmployeePhoto = 129
        DelEmployeePhoto = 130
        ResetEmployeePhoto = 131
        GetBioChanges = 132
        AddDocument = 133
        DelDocument = 134
        ResetDocument = 135
        AddEmployeeGroup = 140
        DelEmployeeGroup = 141
        ResetEmployeeGroup = 142
        AddAccess = 143
        ResetAccess = 144
        AddPeriod = 145
        ResetPeriods = 146
        AddIncidence = 147
        ResetIncidences = 148
        AddSiren = 149
        ResetSirens = 150
        Config = 160
        GetTableVersions = 161
        UnknownError = 199
    End Enum

    Public Enum rxEventID
        ConnectionKO = 298
        ConnectionOK = 200
        ConnectionWithOutSN = 201
        ConnectionInvalidSN = 202
        TerminalNotResponse = 203
        DownloadingPunches = 204
        SavePunch = 205
        DeleteNewPunch = 206
        DeleteAllPunch = 207
        DeleteAllEmployees = 210
        AddEmployee = 211
        AddBio = 212
        DelBio = 213
        DelEmployee = 214
        DelCard = 215
        GetBio = 216
        GetEmployees = 217
        AddGroup = 220
        AddPeriod = 221
        AddSiren = 222
        ConfigTerminal = 223
        ConfigDateTime = 224
        GetRecordInfo = 230
        UnknownError = 299
    End Enum

    Public Enum mxCInbioEventID
        IdentifiedTerminal = 300
        LogicInitialized = 301
        TerminalRegistered = 304
        ProtocolConfigured = 307
        OnlinePunch = 310
        OfflinePunch = 311
        PunchSaved = 312
        AddEmployee = 320
        DelEmployee = 321
        ResetEmployees = 322
        AddCard = 323
        DelCard = 324
        ResetCards = 325
        AddBio = 326
        DelBio = 327
        ResetBio = 328
        AddTimePeriod = 345
        ResetTimePeriods = 346
        DelTimePeriod = 347
        Config = 360
        AddEmployeeAccessLevel = 362
        DelEmployeeAccessLevel = 363
        ResetEmployeeAccessLevel = 364
        TerminalNotRegistered = 370
        UnknownError = 399
        Sirens = 371
    End Enum

#Region "Declarations"

    Private strDriver As String

    Protected strModeInfo As String

    Protected intTerminalID As Integer
    Protected strTerminalType As String
    Protected strTerminalLocation As String
    Protected strTerminalOther As String
    Protected bKeepLoaded As Boolean = False
    Protected strRemoteEndPoint As String
    Protected strEndPointInProcess As String
    Protected dLastStartProcessing As Date

    Public Sub New(ByVal _Driver As String, ByVal _ModeInfo As String)
        Me.strDriver = _Driver
        Me.strModeInfo = _ModeInfo
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property Driver() As String
        Get
            Return Me.strDriver
        End Get
    End Property

    Public ReadOnly Property ModeInfo() As String
        Get
            Return Me.strModeInfo
        End Get
    End Property

    Public ReadOnly Property TerminalID() As Integer
        Get
            Return Me.intTerminalID
        End Get
    End Property

    Public ReadOnly Property TerminalType() As String
        Get
            Return Me.strTerminalType
        End Get
    End Property

    Public ReadOnly Property TerminalLocation() As String
        Get
            Return Me.strTerminalLocation
        End Get
    End Property

    Public Property TerminalOther() As String
        Get
            Return Me.strTerminalOther
        End Get
        Set(ByVal value As String)
            Me.strTerminalOther = value
        End Set
    End Property

    Public Property KeepLoaded() As Boolean
        Get
            Return Me.bKeepLoaded
        End Get
        Set(ByVal value As Boolean)
            Me.bKeepLoaded = value
        End Set
    End Property

    Public Property RemoteEndPoint As String
        Get
            Return Me.strRemoteEndPoint
        End Get
        Set(value As String)
            Me.strRemoteEndPoint = value
        End Set
    End Property

    Public Property EndPointInProcess As String
        Get
            Return Me.strEndPointInProcess
        End Get
        Set(value As String)
            Me.strEndPointInProcess = value
        End Set
    End Property

    Public Property LastStartProcessing As Date
        Get
            Return Me.dLastStartProcessing
        End Get
        Set(value As Date)
            Me.dLastStartProcessing = value
        End Set
    End Property

#End Region

#Region "Events"

    Public Event OnSendMessage(ByVal oMessage As CMessageBase, ByVal oSource As CTerminalLogicBase, ByVal bolAliveResponse As Boolean)

    Public Event OnSendMessageEx(ByVal oMessage As CMessageBase, ByVal sAdditionalInfo As String, ByVal oSource As CTerminalLogicBase, ByVal bolAliveResponse As Boolean)

    Public Event OnCloseRequired(ByVal oSource As CTerminalLogicBase)

#End Region

#Region "Methods"

    Public MustOverride Function Initialize(strSN As String, strIP As String, strPort As String, strModel As String) As Boolean

    Public MustOverride Function ParseMessage(ByRef bInput() As Byte) As CMessageBase

    Public MustOverride Sub Abort()

    Public MustOverride Sub Close()

    Public MustOverride Function ModeDebugInfo() As String

    ''' <summary>
    ''' Cmd a enviar al Terminal
    ''' </summary>
    ''' <param name="oMessage">Clase Message per enviar</param>
    ''' <param name="oSource">Clase heredada Terminal</param>
    ''' <param name="bolAliveResponse">obsoleto</param>
    ''' <remarks></remarks>
    Protected Sub RaiseOnSendMessage(ByVal oMessage As CMessageBase, ByVal oSource As CTerminalLogicBase, ByVal bolAliveResponse As Boolean)
        RaiseEvent OnSendMessage(oMessage, oSource, bolAliveResponse)
    End Sub

    Protected Sub RaiseOnSendMessageEx(ByVal oMessage As CMessageBase, ByVal sAdditionalInfo As String, ByVal oSource As CTerminalLogicBase, ByVal bolAliveResponse As Boolean)
        RaiseEvent OnSendMessageEx(oMessage, sAdditionalInfo, oSource, bolAliveResponse)
    End Sub

    Protected Sub RaiseOnCloseRequired(ByVal oSource As CTerminalLogicBase)
        RaiseEvent OnCloseRequired(oSource)
    End Sub

    ''' <summary>
    ''' Método a sobreescrivir en cada driver que permite pausar el funcionamiento del comportamiento.
    ''' </summary>
    Public MustOverride Sub Interrupt()

    ''' <summary>
    ''' Método a sobreescrivir en cada driver que permite reanudar el funcionamiento del comportamiento.
    ''' </summary>
    Public MustOverride Sub ResumeStart()

    ''' <summary>
    ''' Método a sobreescrivir en cada driver para insertar las tareas necesarias para actualizar la información del empleado a los terminales
    ''' </summary>
    ''' <param name="_IDEmployee">Código del empleado</param>
    ''' <param name="_Action">Acción que se ha realizado sobre el empleado: Insert, Update o Delete</param>
    ''' <remarks></remarks>
    Public MustOverride Sub TerminalTask(ByVal _IDEmployee As Integer, ByVal _Action As TerminalTaskAction, Optional ByVal _IDTerminal As Integer = -1)

    ''' <summary>
    ''' Método a sobreescrivir para que el driver devuelva el mensaje inicial a enviar al terminal. Si no es necesario enviar ningún mensaje, tiene que devolver Nothing.
    ''' </summary>
    Public MustOverride Function GetInitMessage() As CMessageBase

#End Region

End Class