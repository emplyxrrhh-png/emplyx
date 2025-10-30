Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum TerminalModel
        <EnumMember()> mx9
        <EnumMember()> mxV
    End Enum

    <DataContract()>
    Public Enum TerminalDataType
        <EnumMember()> Config
        <EnumMember()> Employees
        <EnumMember()> Cards
        <EnumMember()> Fingers
        <EnumMember()> EmployeePhotos
        <EnumMember()> Documents
        <EnumMember()> AccessAuthorizations
        <EnumMember()> TimeZones
        <EnumMember()> Causes
        <EnumMember()> Sirens
        <EnumMember()> None
        <EnumMember()> Firmware
        <EnumMember()> Device
    End Enum

    <DataContract()>
    Public Enum TerminalDataAction
        <EnumMember()> Add
        <EnumMember()> Delete
        <EnumMember()> Query
        <EnumMember()> Refresh
    End Enum

    <DataContract()>
    Public Enum TerminalAction
        <EnumMember()> AttendanceIn_
        <EnumMember()> AttendanceOut_
        <EnumMember()> AttendanceAuto_
        <EnumMember()> Access_
        <EnumMember()> Productiv_
        <EnumMember()> AttendanceSmart_
        <EnumMember()> Portal_
        <EnumMember()> Costs_
        <EnumMember()> Dinning_
    End Enum

    <DataContract()>
    Public Enum EmployeeAttStatus
        <EnumMember()> Inside
        <EnumMember()> Outside
        <EnumMember()> Unknown
    End Enum

    <DataContract()>
    Public Enum InteractivePunchCommand
        <EnumMember()> Punch
        <EnumMember()> Display
        <EnumMember()> Idle
    End Enum

    <DataContract()>
    Public Enum InteractivePunchDisplayTimeout
        <EnumMember()> TimeOut_VeryShort = 3
        <EnumMember()> TimeOut_Short = 6
        <EnumMember()> TimeOut_Medium = 10
        <EnumMember()> TimeOut_Long = 15
        <EnumMember()> TimeOut_VeryLong = 20
    End Enum

    <DataContract()>
    Public Enum PunchType
        <EnumMember()> AttIn
        <EnumMember()> AttOut
        <EnumMember()> RepeatIn
        <EnumMember()> RepeatOut
        <EnumMember()> Incomplete
        <EnumMember()> Cancel
    End Enum

    <DataContract()>
    Public Enum DinnerPunchResultType
        <EnumMember()> NoTurn
        <EnumMember()> TurnWithPunch
        <EnumMember()> TurnValid
    End Enum

    <DataContract()>
    Public Enum PunchCommand
        <EnumMember()> Add
        <EnumMember()> Delete
        <EnumMember()> Update
    End Enum

    <DataContract()>
    Public Enum TerminalProductivTaskPunchMethod
        <EnumMember()> List
        <EnumMember()> Tree
        <EnumMember()> Number
    End Enum

#Region "Sincronización de información"

    ''' <summary>
    ''' Datos de un cierto tipo para el terminal
    ''' </summary>
    <DataContract>
    Public Class roTerminalSyncData

        <DataMember>
        Public Property Type As TerminalDataType

        <DataMember>
        Public Property Action As TerminalDataAction

        <DataMember>
        Public Property IDTask As Integer

        <DataMember>
        Public Property ConfigParameters As roTerminalSyncParameter()

        <DataMember>
        Public Property Employees As roTerminalEmployeeSyncData()

        <DataMember>
        Public Property Cards As roTerminalCardSyncData()

        <DataMember>
        Public Property Fingers As roTerminalFingerSyncData()

        <DataMember>
        Public Property Causes As roTerminalCauseSyncData()

        <DataMember>
        Public Property Photos As roTerminalEmployeePhotoSyncData()

        <DataMember>
        Public Property Documents As roTerminalDocumentSyncData()

        <DataMember>
        Public Property AccessAuthorizations As roTerminalAccessAuthorizationSyncData()

        <DataMember>
        Public Property Sirens As roTerminalSirensSyncData()

        <DataMember>
        Public Property TimeZones As roTerminalTimezoneSyncData()

        ''' <summary>
        ''' Estado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As roWsTerminalState

    End Class

    <DataContract>
    Public Class roTerminalSyncParameter
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Valor del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Value As String

        Public Sub New(sName As String, sValue As String)
            IDTask = -1
            Name = sName
            Value = sValue
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalEmployeeSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Nombre del empleado a mostrar en el terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Idioma del empleado para sus mensajes en pantalla del terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Language As String

        ''' <summary>
        ''' Identificadores de justificaciones (separados por comas) que puede fichar el empleado (podría ser que no sean todas las disponibles en el terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AllowedCauses As String

        ''' <summary>
        ''' Indica si en modos rápidos, hay que preguntar de todos modos al servidor porque hay mensajes para este empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsOnline As Boolean

        ''' <summary>
        ''' PIN del empleado (para identificación ID+PIN)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PIN As String

        ''' <summary>
        ''' Indica si se debe exigir el consentimiento al empleado cuando se registre su huella en el terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ConsentRequired As Boolean

    End Class

    <DataContract>
    Public Class roTerminalCardSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Código interno de tarjeta
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDCard As String
    End Class

    <DataContract>
    Public Class roTerminalFingerSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Índice de la huella
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDFinger As Integer

        ''' <summary>
        ''' Cadena de la huella en base64
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FingerData As String

        ''' <summary>
        ''' Fecha en que se guardó
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TimeStamp As DateTime

    End Class

    <DataContract>
    Public Class roTerminalCauseSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de la justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDCause As Integer

        ''' <summary>
        ''' Nombre de la justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String
    End Class

    <DataContract>
    Public Class roTerminalEmployeePhotoSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Foto en base64
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PhotoData As String
    End Class

    <DataContract>
    Public Class roTerminalDocumentSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Nombre del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Fecha de inicio de validez del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property BeginDate As Date?

        ''' <summary>
        ''' Fecha de fin de validez del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EndDate As Date?

        ''' <summary>
        ''' Indica si debe denegar el acceso o sólo avisar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DenyAccess As Boolean

        ''' <summary>
        ''' Nombre de la empresa a la que se exige el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Company As String

    End Class

    <DataContract>
    Public Class roTerminalAccessAuthorizationSyncData
        <DataMember()>
        Public Property IDTask As Integer
        <DataMember()>
        Public Property IDEmployee As Integer
        <DataMember()>
        Public Property IDAuthorization As Integer
    End Class

    <DataContract>
    Public Class roTerminalSirensSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Día de la semana
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DayOf As Integer

        ''' <summary>
        ''' Hora de la sirena. La fecha se debe ignoarar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property StartDate As DateTime?

        ''' <summary>
        ''' Relé a accionar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Relay As Integer

        ''' <summary>
        ''' Duración de la sirena en segundos
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Duration As Integer

    End Class

    <DataContract>
    Public Class roTerminalTimezoneSyncData
        ''' <summary>
        ''' Nombre del parámetro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDTask As Integer

        ''' <summary>
        ''' Id de la autorización de acceso a la que se asocia el periodo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDAuthorization As Integer

        ''' <summary>
        ''' Día de la semana
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DayOf As Integer

        ''' <summary>
        ''' Hora de inicio del periodo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property StartTime As DateTime?

        ''' <summary>
        ''' Hora de fin del periodo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EndTime As DateTime?
    End Class

#End Region

#Region "Objetos de terminal"

    <DataContract>
    Public Class roTerminalEmployeeStatus

        ''' <summary>
        ''' Nombre del empleado a mostrar por pantalla del terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeName As String

        ''' <summary>
        ''' Foto del empleado a mostrar por pantalla del terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeePhoto As String

        ''' <summary>
        ''' Estado de control horario del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AttendanceStatus As roAttendanceEmployeeStatus

        ''' <summary>
        ''' Estado de ProductiV del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiveStatus As roProductiveEmployeeStatus

        ''' <summary>
        ''' Estado de Control de Costes del empleados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostsStatus As roCostsEmployeeStatus

        ''' <summary>
        ''' Estado general del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DinnerStatus As roDinnerEmployeeStatus

        ''' <summary>
        ''' Estado general del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PortalStatus As roPortalEmployeeStatus

        <DataMember()>
        Public Property ServerDate As Date

        <DataMember()>
        Public Property EmployeeMessages As roEmployeeMessage()

        <DataMember()>
        Public Property Status As roWsTerminalState

        Public Sub New()
            Me.AttendanceStatus = New roAttendanceEmployeeStatus
            Me.ProductiveStatus = New roProductiveEmployeeStatus
            Me.CostsStatus = New roCostsEmployeeStatus
            Me.PortalStatus = New roPortalEmployeeStatus
            Me.DinnerStatus = New roDinnerEmployeeStatus
            Me.EmployeeMessages = {}
        End Sub

    End Class

    Public Class roEmployeeMessage
        <DataMember()>
        Public Property ID As Integer

        <DataMember()>
        Public Property Message As String
    End Class

    Public Class roAttendanceEmployeeStatus
        ''' <summary>
        ''' Fecha y hora del último fichaje de presencia del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchDateTime As Date?

        <DataMember()>
        Public Property LastPunchAction As String

        <DataMember()>
        Public Property LastPunchCause As Integer

        ''' <summary>
        ''' Estado de presencia del empleado. Los posibles valores son "Inside" o "Outside"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AttendanceStatus As EmployeeAttStatus

        <DataMember>
        Public Property AvailableCauses As roTerminalCause()

        ''' <summary>
        ''' Acción del fichaje de presencia en función del contexto del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchType As PunchType

        ''' <summary>
        ''' Cierto si es el primer fichaje de gestión horaria del día para el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FirstDayPunch As Boolean

        ''' <summary>
        ''' Minutos desde el último fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MinutesFromLastPunch As Integer

        ''' <summary>
        ''' Minutos desde el el primer fichaje del día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MinutesSinceFirstPunch As Integer

        Public Sub New()
            Me.AttendanceStatus = EmployeeAttStatus.Outside
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalCause
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        Public Sub New(id As Integer, sname As String)
            Me.Id = id
            Me.Name = sname
        End Sub

    End Class

    Public Class roProductiveEmployeeStatus

        ''' <summary>
        ''' Fecha y hora del último fichaje de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastTaskDate As Date?
        ''' <summary>
        ''' Nombre de la tarea en que se encuentra trabajando el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskName As String

        ''' <summary>
        ''' Permiso del empleado para dar tareas por completadas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasCompletePermission As Boolean

        ''' <summary>
        ''' Permiso del empleado para acceder a funcionalidades del módulo de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiVEnabled As Boolean

        <DataMember>
        Public Property AvailableTasks As roTerminalProductivTask()

        <DataMember>
        Public Property TaskPunchMethod As TerminalProductivTaskPunchMethod
    End Class

    Public Class roCostsEmployeeStatus
        ''' <summary>
        ''' Permiso para realizar fichajes de Centros de Coste
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterEnabled As Boolean
        ''' <summary>
        ''' Centro de Coste al que el empleado está asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterName As String
        ''' <summary>
        ''' Identificador del Centro de Coste al que el empleado está asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterId As Integer

        <DataMember>
        Public Property AvailableCostCenters As roTerminalCostCenter()
    End Class

    <DataContract>
    Public Class roDinnerEmployeeStatus
        <DataMember>
        Public Property IdTurn As Integer

        <DataMember>
        Public Property TurnName As String

        <DataMember>
        Public Property SaveAttOut As Boolean

        <DataMember>
        Public Property Allowed As Boolean

        <DataMember>
        Public Property AvailableTurns As roTerminalDiningTurn()

        <DataMember>
        Public Property Result As DinnerPunchResultType
    End Class

    Public Class roPortalEmployeeStatus
        'TODO
    End Class

#End Region

#Region "Fichajes"

    <DataContract()>
    Public Class roTerminalPunch

        <DataMember()>
        Public Property GUID As String

        ''' <summary>
        ''' ID del empleado si se identificó
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDEmployee As Integer

        ''' <summary>
        ''' Credencial con la que se identificó el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Credential As String

        ''' <summary>
        ''' Medio de identificación usado por el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Method As Integer

        ''' <summary>
        ''' Acción realizada. Tipo de fichaje, y si procede, motivo de la novalidez
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Action As String

        ''' <summary>
        ''' Sentido del fichaje (Si es entrada, salida, etc)
        ''' </summary>
        ''' <returns></returns>
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ActualType As Nullable(Of PunchTypeEnum)
        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchDateTime As DateTime

        ''' <summary>
        ''' Nombre de la zona horaria en la que se realizó el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TimeZoneName As String

        ''' <summary>
        ''' PIN del empleado para fichaje con ID + PIN
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PIN As String

        ''' <summary>
        ''' Datos adicionales del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchData As roTerminalPunchData

        ''' <summary>
        ''' Foto realizada en el fichaje, en base64
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Photo As String

        ''' <summary>
        ''' Estado del fichaje, para fichajes interactivos gestionados por el servidor
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchState As String

        ''' <summary>
        ''' Informado por el terminal, indica si se debe crear o borrar el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Command As PunchCommand

        Public Sub New()
            Me.PunchDateTime = DateSerial(1970, 1, 1)
            Me.Command = PunchCommand.Add
            Me.TimeZoneName = String.Empty
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalInteractivePunch

        ''' <summary>
        ''' Indica si el fichaje recibido es un fichaje, o datos recogidos por pantalla tras haber fichado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Command As InteractivePunchCommand

        ''' <summary>
        ''' Cuando Command = Punch
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Punch As roTerminalPunch

        ''' <summary>
        ''' Cuando Command = Display
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Display As roTerminalDisplayData

        ''' <summary>
        ''' Información de estado del empleado, para mostrar datos por pantalla del terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EmployeeStatus As roTerminalEmployeeStatus

        <DataMember>
        Public Property Status As roWsTerminalState

        Public Sub New()
            Me.Punch = New roTerminalPunch
            Me.Display = New roTerminalDisplayData
            Me.EmployeeStatus = New roTerminalEmployeeStatus
        End Sub

        Public Sub New(oCommand As InteractivePunchCommand)
            Me.Punch = New roTerminalPunch
            Me.Command = oCommand
            Me.Display = New roTerminalDisplayData
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalDisplayData
        <DataMember>
        Public Property Response As String

        <DataMember>
        Public Property UserInfo As String

        <DataMember>
        Public Property WorkArea As String

        <DataMember>
        Public Property List As roTerminalListItem()

        <DataMember>
        Public Property RightButtons As roTerminalButton()

        <DataMember>
        Public Property ButtonValue As String

        <DataMember>
        Public Property DateTimeValue As Nullable(Of Date)

        <DataMember>
        Public Property StringValue As String

        <DataMember>
        Public Property Timeout As Integer

        <DataMember>
        Public Property ActiveInputs As String

        Public Sub New()
            Me.DateTimeValue = Date.Now
            Me.List = {}
            Me.RightButtons = {}
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalButton
        <DataMember>
        Public Property Icon As String

        <DataMember>
        Public Property Action As String

        <DataMember>
        Public Property Text As String

        Public Sub New(sText As String, sAction As String, sIcon As String)
            Me.Icon = sIcon
            Me.Action = sAction
            Me.Text = sText
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalListItem
        <DataMember>
        Public Property Id As String

        <DataMember>
        Public Property Text As String

        Public Sub New(sId As String, sText As String)
            Me.Id = sId
            Me.Text = sText
        End Sub

    End Class

    <DataContract>
    Public Class roTerminalPunchData
        <DataMember>
        Public Property AttendanceData As roAttendancePunchData

        <DataMember>
        Public Property AccessAndAttendanceData As roAccessAndAttendancePunchData

        <DataMember>
        Public Property ProductivData As roProductiVPunchData

        <DataMember>
        Public Property CostCenterData As roCostCenterPunchData

        <DataMember>
        Public Property DinnerData As roDinnerPunchData

        <DataMember()>
        Public Property ReadMessages As Integer()

    End Class

    <DataContract>
    Public Class roAttendancePunchData
        <DataMember>
        Public Property IdCause As Integer
    End Class

    <DataContract>
    Public Class roAccessAndAttendancePunchData
        <DataMember>
        Public Property IdCause As Integer
    End Class

    <DataContract>
    Public Class roProductiVPunchData
        <DataMember>
        Public Property CurrentTask As roTerminalProductivTask

        <DataMember>
        Public Property NextTask As roTerminalProductivTask

        <DataMember>
        Public Property PunchAction As ActionTypes
    End Class

    <DataContract>
    Public Class roDinnerPunchData
        <DataMember>
        Public Property IdTurn As Integer

        <DataMember>
        Public Property SaveAttOut As Boolean

        <DataMember>
        Public Property Result As DinnerPunchResultType
    End Class

    <DataContract>
    Public Class roTerminalProductivTask
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Project As String

        <DataMember>
        Public Property BarCode As String

        <DataMember>
        Public Property RequiredUserFields As roTerminalProductivTaskUserField()

        ''' <summary>
        ''' Indica si una Tarea puede ser completada (básicamente indica si hay algún empleado más trabajando en ella)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanBeCompleted As Boolean

    End Class

    <DataContract>
    Public Class roTerminalProductivTaskUserField
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        <DataMember()>
        Public Property Value As String

        <DataMember()>
        Public Property ValuesList As String()

        <DataMember()>
        Public Property OnAction As ActionTypes
    End Class

    <DataContract>
    Public Class roCostCenterPunchData
        <DataMember>
        Public Property IdCostCenter As Integer
    End Class

    <DataContract>
    Public Class roTerminalCostCenter
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String
    End Class

    <DataContract>
    Public Class roTerminalDiningTurn
        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String
    End Class

#End Region

    <DataContract()>
    Public Class TerminalStdResponse

        Public Sub New()
            Status = New roWsTerminalState
            TaskResult = {}
        End Sub

        ''' <summary>
        ''' Resultado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Result As Boolean

        <DataMember()>
        Public Property Status As roWsTerminalState

        <DataMember()>
        Public Property TaskResult As roTasksResult()
    End Class

    <DataContract()>
    Public Class roTasksResult

        <DataMember()>
        Public Property IDTask As Integer

        <DataMember()>
        Public Property Code As Integer

        <DataMember()>
        Public Property Result As Boolean
    End Class

    <DataContract()>
    Public Class TerminalConfig
        ''' <summary>
        ''' Token de seguridad para establecimiento de comunicación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SecurityToken As String

        ''' <summary>
        ''' Lista de parámetros de configuración registrados en el terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Config As roTerminalSyncParameter()

        <DataMember()>
        Public Property Status As roWsTerminalState

        Public Sub New()
            SecurityToken = String.Empty
            Config = {}
            Status = New roWsTerminalState
        End Sub

    End Class

End Namespace