Public Module roConstants

    '==================================================================
    ' roCONSTANTS
    '
    ' Definición de las constantes de ambito global de VisualTi
    '
    ' Cualquier programa que utilice librerias o se comunique con
    '  procesos o programas de VisualTime debe utilizar este fichero
    '  de constantes.
    '==================================================================

    ' Nombre del producto
    Public Const PRODUCTNAME = "VisualTime"

    ' Constantes para carpetas de sistema (GetExplorerFolder)
    Public Const roFolderProgramsMenu = "Programs"
    Public Const roFolderStartMenu = "Start Menu"
    Public Const roFolderProgramsStartupMenu = "Startup"
    Public Const roFolderDesktop = "Desktop"

    ' Nombre del servidor
    Public Const VTSERVER_CLASS = "VisualTimeServer.Server"
    Public Const VTSERVER_CAPTION = "VisualTime Server"

    ' Nombre del controlador de terminales
    Public Const VTTERMINALCONTROLLER_CLASS = "TerminalsController.Server"
    Public Const VTTERMINALCONTROLLER_CAPTION = "VisualTime Terminal's Controller"

    ' Fichero de licencia
    Public Const LICENSE_FILE = "License.vtl"
    Public Const LICENSECLIENTCOPY_FILE = "License.ClientCopy.vtl"
    Public Const LICENSE_EXTENSION = "vtl"

    ' Fichero de Actualización de VisualTime
    Public Const VTUPDATE_FILE = "VT Update.exe"

    ' Valores de retorno especificos de RequestClientLicense
    Public Const REQUESTRESPONSE_LICENSE_ERROR = "<RequestLicense:Error>"
    Public Const REQUESTRESPONSE_NOLICENSE = "<RequestLicense:NoLicense>"
    Public Const REQUESTRESPONSE_LICENSE_SAME = "<RequestLicense:CopyIsSame>"

    ' Valores nulos
    Public Const roNoEntry = "<NOENTRY>"
    Public Const roNullEntry = "(null)"
    Public Const roNullDate = "1/1/2079"
    Public Const roMaxTimeNumericValue = 999999999999.0#

    ' Constantes de retorno del registro
    Public Const roInvalidRegistry = "(Registry_Error)"
    Public Const roClassNotFound = "(Class_Not_Found)"
    Public Const roNotFound = "(empty)"

    ' Tipos de objetos registrables
    Public Const roDBParameterObject = "DBPARAM"
    Public Const roSessionObject = "SESSION"
    Public Const roTableObject = "TABLE"
    Public Const roProcObject = "PROC"
    Public Const roTaskObject = "TASK"
    Public Const roUserTaskObject = "USERTASK"
    Public Const roFunctionCallObject = "FN"
    Public Const roHookObject = "HOOK"
    Public Const roDriverObject = "DRIVER"
    Public Const roUnknownObject = "DUMMY"

    ' Constantes del registro
    Public Const HKEY_CLASSES_ROOT = &H80000000
    Public Const HKEY_CURRENT_USER = &H80000001
    Public Const HKEY_LOCAL_MACHINE = &H80000002
    Public Const HKEY_USERS = &H80000003
    Public Const HKEY_CURRENT_CONFIG = &H80000005
    Public Const HKCR = &H80000000
    Public Const HKCU = &H80000001
    Public Const HKLM = &H80000002
    Public Const HKU = &H80000003
    Public Const HKCC = &H80000005

    ' Estado de procesos y clientes
    Public Const roInitializing = "INITIALIZING"
    Public Const roReady = "READY"
    Public Const roBusy = "BUSY"
    Public Const roWaiting = "WAITING"
    Public Const roClosed = "CLOSED"
    Public Const roNotLoaded = "NOT LOADED"

    ' Estado de tareas
    Public Const roTaskPending = "PENDING"
    Public Const roTaskCompleted = "COMPLETED"
    Public Const roTaskCancelled = "CANCELLED"
    Public Const roTaskRunning = "RUNNING"

    ' Nombres de variables especiales de sistema
    Public Const roVarStatus = "Status"
    Public Const roVarProgress = "Progress"
    Public Const roVarProcID = "ProcessID"
    Public Const roVarCommand = "Command"
    Public Const roVarTaskStatus = "TaskStatus"
    Public Const roVarEvent = "Event"
    Public Const roVarConnectionString = "ADOConnectionString"
    Public Const roVarODBCConnectionString = "ODBCConnectionString"
    Public Const roVarUserGroup = "UserGroup"
    Public Const roVarDBServer = "DBServer"
    Public Const roVarDBName = "DBName"
    Public Const roVarDBUserName = "DBUserName"
    Public Const roVarDBUserPassword = "DBUserPassword"

    ' Constantes para las variables del contexto
    Public Const roVarDataOp = "DataOp"
    Public Const roVarCompletedAction = "CompletedAction"

    Public Const roVarID = "ID"
    Public Const roVarIDType = "IDType"

    Public Const roVarUser = "User"
    Public Const roVarDate = "Date"
    Public Const roVarShift = "Shift.ID"
    Public Const roVarCauseID = "Cause.ID"
    Public Const roVarConceptID = "Concept.ID"
    Public Const roVarEmployeeID = "Employee.ID"
    Public Const roVarEmployeeName = "Employee.Name"
    Public Const roVarTeam = "Team"
    Public Const roVarJob = "Job"
    Public Const roVarMachine = "Machine"
    Public Const roVarMachineGroup = "MachineGroup"

    Public Const roVarViewType = "ViewType"
    Public Const roVarFirstDay = "FirstDay"
    Public Const roVarInterval = "Interval"

    Public Const roVarIsRegistered = "IsRegistered"
    Public Const roVarWorkstation = "Workstation"

    Public Const roVarCommsConfig = "CommsConfig"
    Public Const roVarCommsReaderStatus = "LastReaderStatus_"
    Public Const roVarCommsReaderDetails = "LastReaderDetails_"
    Public Const roVarReaderID = "ReaderID"
    Public Const roVarPortID = "Port"
    Public Const roVarBehaviorID = "Behavior"

    'Parámetros
    Public Const roParStopComms = "StopComms"
    Public Const roParMovMaxHours = "MaxMovementHours"
    Public Const roParFirstDate = "FirstDate"
    Public Const roParFuncTerminal = "FunctionsTerminal"
    Public Const roParMonthPeriod = "MonthPeriod"
    Public Const roParYearPeriod = "YearPeriod"
    Public Const roParJobCounter = "JobCounter"
    Public Const roParNumMonthsAccess = "NumMonthsAccess"
    Public Const roParLastDateAccess = "LastDateAccess"

    'Valores por defecto de parámetros
    Public Const roParDefMovMaxHours = "12:00"
    Public Const roParDefFirstDate = "1/1/1900"
    Public Const roParMaxNumMonthAccess = 36

    ' Tipos de comandos a procesos
    Public Const roCommandRunService = "Service"

    ' Tipos de eventos de tareas o procesos
    Public Const roEventNewTask = "NEW_TASK"
    Public Const roEventRunTask = "RUN_TASK"
    Public Const roEventCompleteTask = "COMPLETE_TASK"
    Public Const roEventCancelledTask = "CANCELLED_TASK"
    Public Const roEventLoadProcess = "LOAD_PROCESS"

    ' Tipos de funcionamiento del controlador de terminales
    Public Const roTerCon_OnShares = "NETSHARES"
    Public Const roTerCon_OnDCOM = "NETDCOM"
    Public Const roTerCon_Standalone = "STANDALONE"

    ' Tipos de metodos implementados en los drivers
    Public Const roFeatureDetect = "DETECT"
    Public Const roFeatureSetTime = "SETTIME"
    Public Const roFeatureReprogram = "PROGRAM"
    Public Const roFeatureGetStatus = "GETSTATUS"
    Public Const roFeatureRead_Begin = "READ_BEGIN"
    Public Const roFeatureRead_End = "READ_END"
    Public Const roFeatureRead_Now = "READ_NOW"
    Public Const roFeatureOpenDialUp = "OPEN_DIALUP"
    Public Const roFeatureCloseDialUp = "CLOSE_DIALUP"
    Public Const roFeatureRead_IfNecessary = "READ_IF_NECESSARY"
    Public Const roFeatureUpdateDynamicData = "SETDYNDATA"
    Public Const roFeatureEnterSetup = "ENTERSETUP"
    Public Const roFeatureProcessCommand = "PROCESSCOMMAND"
    Public Const roFeatureTCPIPProcessStack = "TCPIPSTACKPROCESS"

    ' Bases de datos
    Public Const roDefaultDatabase = "(default)"

    ' Detalles del usuario de sistema
    Public Const roLoginSystemUser = "(VisualTime Module)"
    Public Const roLoginSystemPassword = "(98hrjin20fg8)"
    Public Const roSystemUserGroup = "0"

    ' Estados posibles de los terminales
    Public Const roReaderStatusOk = "Ok"
    Public Const roReaderStatusNoBehavior = "NoBehavior"
    Public Const roReaderStatusCrashed = "Crashed"
    Public Const roReaderStatusTimeOut = "Timeout"
    Public Const roReaderStatusBreak = "Break"
    Public Const roReaderStatusFrame = "Frame"
    Public Const roReaderStatusOverrun = "Overrun"
    Public Const roReaderStatusCommandError = "CommandError"
    Public Const roReaderStatusCRCError = "CRCError"
    Public Const roReaderStatusPortError = "PortError"
    Public Const roReaderStatusRemoved = "REMOVED"
    Public Const roReaderStatusNotConnected = "NotConnected"

    ' Archivo de sincronizacion de fichajes/sirenas/accesos para instalaciones multiservidor
    Public Const REMOTE_SERVERS_CONFIG = "RemoteServers.VTRS"
    Public Const REMOTE_SERVER_OUT_FOLDER_PREFIX = "To "

    ' Reglas para justificaciones automáticas
    Public Const roRuleIncidence = "Incidence"
    Public Const roRuleZone = "Zone"
    Public Const roRuleFromTime = "FromTime"
    Public Const roRuleToTime = "ToTime"
    Public Const roRuleCause = "Cause"
    Public Const roRuleMaxTime = "MaxTime"

    ' Tipos de tratamiento de filtros de incidencias
    Public Const roFilterTreatAsWork = "TreatAsWork"
    Public Const roFilterIgnore = "Ignore"
    Public Const roFilterGenerateIncidence = "Incidence"
    Public Const roFilterGenerateOvertime = "Overtime"

    ' Tipos de tratamiento de horas superiores al máximo de las flexibles
    Public Const roOvertimeAsOvertime = "Overtime"
    Public Const roOvertimeAsIncidence = "Incidence"

    ' Tipos de tratamiento de horas inferiores

    ' Tipos de tratamientos de descansos superiores al máximo
    Public Const roBreakCreateIncidence = "CreateIncidence"
    Public Const roBreakSubstractAttendanceTime = "SubstractAttendanceTime"

    ' Variables usadas para triggers de cambio de fecha, hora, etc.
    Public Const roTimerFlag_DayChange = "TimerFlag_DayChange"
    Public Const roTimerFlag_HourChange = "TimerFlag_HourChange"

    ' Estado del servidor de VisualTime
    Public Enum roServerStatus
        roServerStatusIdle = 0
        roServerStatusBusy = 1
        roServerStatusStopping = 9
    End Enum

    'Tipo usado en user fields
    Public Enum roUserFieldType
        roText
        roNumeric
        roDateTime
        roDecimal
    End Enum

    ' Tipo de procesado de capas de horario
    Public Enum roShiftLayersEngineMode
        roEngineAccurate = 1    ' Modo usado por el calculador
        roEngineFastDetect = 2  ' Modo usado por el detector de horarios
    End Enum

    ' IDs de justificaciones predeterminadas
    Public Const CAUSE_INCIDENCE_DEFAULT = 0
    Public Const CAUSE_WORKING_DEFAULT = 1
    Public Const CAUSE_OVERWORKING = 2
    Public Const CAUSE_BREAK = 3

    'Tipos de acumulados de los conceptos
    Public Const CONCEPT_TYPE_TIME = "H"
    Public Const CONCEPT_TYPE_OCCURRENCES = "O"
    Public Const CONCEPT_TYPE_MIXED = "M"

    ' Conceptos especiales prefijados
    Public Const CONCEPT_SHIFTEXPECTEDHOURS = 4

    'Tablas para NotifyTableChange
    Public Const roTableDailyTotals = "DAILY_TOTALS"
    Public Const roTableMoves = "MOVES"

    'Constantes de grupos especiales
    Public Const roRootGroup = 1

    'Constantes de identificadores minimos
    Public Const MIN_CONCEPT_ID = 20
    Public Const MIN_CAUSE_ID = 20
    Public Const MIN_TIMEZONE_ID = 20
    Public Const MIN_INCIDENCE_ID = 20
    Public Const MIN_JOBINCIDENCE_ID = 20
    Public Const MIN_INCIDENCECATEGORY_ID = 20

    ' Comandos para el proceso de comunicaciones
    Public Const CommsCommandSetBehavior = "ON_SET_BEHAVIOR"
    Public Const CommsCommandSetTime = "ON_SET_TIME"
    Public Const CommsCommandDetect = "ON_DETECT"
    Public Const CommsCommandRemoveReaders = "ON_REMOVEREADERS"
    Public Const CommsCommandEnterSetup = "ON_ENTER_SETUP"
    Public Const CommsCommandUpdateDynamicData = "ON_SET_DYNDATA"
    Public Const CommsCommandUpdateAccrual = "ON_CHANGE_ACCRUAL"
    Public Const CommsCommandUpdateDayChange = "ON_DAY_CHANGE"

    ' Comandos para el proceso de comunicaciones TCP
    Public Const TcpConfigRoot = "SOFTWARE\Robotics\VisualTime\TcpConfig"
    Public Const TcpCommandConsoleView = "CONSOLE_VIEW"
    Public Const TcpCommandConsoleHide = "CONSOLE_HIDE"

    ' Tipos de nodos en el control VTSelector
    Public Const roItemType_Employee = "ITEM"
    Public Const roItemType_Employee_WillMove = "ITEM_MOVING"
    Public Const roItemType_Employee_Obsolete = "ITEM_OLD"
    Public Const roItemType_Employee_NewInFuture = "ITEM_NEW"
    Public Const roItemType_JobEmployee = "JOBITEM"
    Public Const roItemType_JobEmployee_WillMove = "JOBITEM_MOVING"
    Public Const roItemType_JobEmployee_Obsolete = "JOBITEM_OLD"
    Public Const roItemType_JobEmployee_NewInFuture = "JOBITEM_NEW"

    'Modo Vista Visualtime
    Public Const roSimulationView = "SIMULATION"
    Public Const roNormalView = "NORMAL"

    'Tipos de Informes
    Public Const REPORT_TYPE_ACCESS = "Access"
    Public Const REPORT_TYPE_ATTENDANCE = "Attendance"
    Public Const REPORT_TYPE_SHOPFLOOR = "ShopFloor"
    Public Const REPORT_TYPE_CR_ATTENDANCE = "CrystalAttendance"

    'Tipos de acciones en auditoria
    Public Const AUDIT_CONNECT = "Connect"
    Public Const AUDIT_DISCONNECT = "Disconnect"
    Public Const AUDIT_QUERY = "Query"
    Public Const AUDIT_UPDATE = "Update"
    Public Const AUDIT_DELETE = "Delete"
    Public Const AUDIT_INSERT = "Insert"

End Module