Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa el estado en que está un empleado
    ''' </summary>
    <DataContract()>
    Public Class UserStatus

        Public Sub New()
            ForbiddenPunchDate = DateTime.Now
            LastPunchDate = DateTime.Now
            ServerDate = DateTime.Now
            LastTaskDate = DateTime.Now
            ScheduleStatus = New EmployeeAlerts
            SupervisorStatus = New SupervisorAlerts
            Communiques = {}
            Surveys = {}
            Zones = New Zones()
            Status = 0
            HasAlertPermission = True
            Telecommuting = False
            ReadMode = False
            AvailableTasks = 0
            Channels = {}
        End Sub

        ''' <summary>
        ''' Nombre del empleado a mostrar por pantalla
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeName As String
        ''' <summary>
        ''' Hora del servidor
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ServerDate As Date
        ''' <summary>
        ''' Fecha y hora del último fichaje de presencia del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchDate As Date
        ''' <summary>
        ''' Estado de presencia del empleado. Los posibles valores son "Inside" o "Outside"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PresenceStatus As String
        ''' <summary>
        ''' Permiso para realizar fichajes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchesEnabled As Boolean
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FirstEntrance As Boolean
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ForbiddenPunch As Boolean
        <DataMember()>
        Public Property ReadMode As Boolean
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ForbiddenPunchDate As Date
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ExistsCause As Integer
        <DataMember()>
        Public Property AvailableTasks As Integer
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchId As Integer
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        'Propiedades de ProductiV

        ''' <summary>
        ''' Fecha y hora del último fichaje de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastTaskDate As Date
        ''' <summary>
        ''' Nombre de la tarea en que se encuentra trabajando el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskTitle As String
        <DataMember()>
        Public Property TaskDescription As String
        ''' <summary>
        ''' identificador de la tarea en que se encuentra trabajando el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskId As Integer
        ''' <summary>
        ''' Permiso del empleado para acceder a funcionalidades del módulo de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiVEnabled As Boolean
        ''' <summary>
        ''' Indica si una Tarea puede ser completada (básicamente indica si hay algún empleado más trabajando en ella)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCompleteTask As Boolean
        ''' <summary>
        ''' Permiso del empleado para dar tareas por completadas (ningún otro empleado las podrá fichar)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasCompletePermission As Boolean

        <DataMember()>
        Public Property HasAlertPermission As Boolean
        'Propiedades de Centro de costes

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

        <DataMember()>
        Public Property WorkCenterName As String

        <DataMember()>
        Public Property Telecommuting As Boolean

        <DataMember()>
        Public Property TelecommutingDays As String

        <DataMember()>
        Public Property StatusInfoMsg As String

        <DataMember()>
        Public Property TelecommutingExpected As Boolean
        ''' <summary>
        ''' Identificador del Centro de Coste al que el empleado está asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterId As Integer

        ''' <summary>
        ''' Representa las alertas que un empleado debe atender
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ScheduleStatus As EmployeeAlerts

        ''' <summary>
        ''' Representa las alertas que un supervisor debe atender
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SupervisorStatus As SupervisorAlerts
        <DataMember()>
        Public Property Communiques As roCommuniqueWithStatistics()
        <DataMember()>
        Public Property Surveys As roSurvey()
        <DataMember()>
        Public Property Zones As Zones
        <DataMember()>
        Public Property Channels As roChannel()
    End Class

    <DataContract()>
    Public Class EmployeeTelecommuting
        Public Sub New()
            Telecommute = New TelecommuteInfo
        End Sub

        <DataMember()>
        Public Property Telecommute As TelecommuteInfo
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    <DataContract()>
    Public Class TelecommuteInfo
        <DataMember()>
        Public Property Telecommuting As Boolean

        <DataMember()>
        Public Property TelecommutingDays As String

        <DataMember()>
        Public Property TelecommutingExpected As Boolean

        <DataMember()>
        Public Property WorkCenterName As String
    End Class


    <DataContract()>
    Public Class EmployeeCommuniques
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        <DataMember()>
        Public Property Communiques As roCommuniqueWithStatistics()
    End Class

    <DataContract()>
    Public Class EmployeeSurveys
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        <DataMember()>
        Public Property Surveys As roSurvey()
    End Class

    <DataContract()>
    Public Class EmployeeStatus

        Public Sub New()
            LastPunchDate = DateTime.Now
            ServerDate = DateTime.Now
            LastTaskDate = DateTime.Now
            Status = 0
            AvailableTasks = 0
            ReadMode = False
            HasAlertPermission = False
        End Sub

        ''' <summary>
        ''' Nombre del empleado a mostrar por pantalla
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeName As String
        ''' <summary>
        ''' Hora del servidor
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ServerDate As Date
        ''' <summary>
        ''' Fecha y hora del último fichaje de presencia del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchDate As Date
        ''' <summary>
        ''' Estado de presencia del empleado. Los posibles valores son "Inside" o "Outside"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PresenceStatus As String
        ''' <summary>
        ''' Permiso para realizar fichajes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property PunchesEnabled As Boolean

        <DataMember()>
        Public Property ReadMode As Boolean

        <DataMember()>
        Public Property AvailableTasks As Integer
        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchId As Integer
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long

        'Propiedades de ProductiV
        ''' <summary>
        ''' Fecha y hora del último fichaje de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastTaskDate As Date
        ''' <summary>
        ''' Nombre de la tarea en que se encuentra trabajando el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskTitle As String
        <DataMember()>
        Public Property TaskDescription As String
        ''' <summary>
        ''' identificador de la tarea en que se encuentra trabajando el empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TaskId As Integer
        ''' <summary>
        ''' Permiso del empleado para acceder a funcionalidades del módulo de Tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiVEnabled As Boolean
        ''' <summary>
        ''' Indica si una Tarea puede ser completada (básicamente indica si hay algún empleado más trabajando en ella)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCompleteTask As Boolean
        ''' <summary>
        ''' Permiso del empleado para dar tareas por completadas (ningún otro empleado las podrá fichar)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasCompletePermission As Boolean

        <DataMember()>
        Public Property HasAlertPermission As Boolean


        'Propiedades de Centro de costes

        ''' <summary>
        ''' Permiso para realizar fichajes de Centros de Coste
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterEnabled As Boolean

        ''' <summary>
        ''' Identificador del Centro de Coste al que el empleado está asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterId As Integer
        ''' <summary>
        ''' Centro de Coste al que el empleado está asignado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterName As String

        <DataMember()>
        Public Property StatusInfoMsg As String
        ''' <summary>
        ''' Fecha y hora del último fichaje de presencia del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LastPunchDirection As String
    End Class


    Public Class EmployeeNotificationStatus

        Public Sub New()
            HasStatusAlerts = False
            HasSupervisorAlerts = False
            HasMandatoryCommuniques = False
        End Sub
        <DataMember()>
        Public Property HasStatusAlerts As Boolean

        <DataMember()>
        Public Property HasSupervisorAlerts As Boolean

        <DataMember()>
        Public Property HasMandatoryCommuniques As Boolean
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    Public Class EmployeeNotifications

        Public Sub New()
            ScheduleStatus = New EmployeeAlerts
            SupervisorStatus = New SupervisorAlerts
        End Sub


        ''' <summary>
        ''' Representa las alertas que un empleado debe atender
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ScheduleStatus As EmployeeAlerts

        ''' <summary>
        ''' Representa las alertas que un supervisor debe atender
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SupervisorStatus As SupervisorAlerts
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Long
    End Class

End Namespace