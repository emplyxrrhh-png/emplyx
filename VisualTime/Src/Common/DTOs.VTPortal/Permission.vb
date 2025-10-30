Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Indica si el empleado dispone de permiso sobre una solicitud concreta
    ''' </summary>
    <DataContract()>
    Public Class ReqPermission

        ''' <summary>
        ''' Tipo de solicitud:
        '''     1 --> Solicitud de campo de la ficha
        '''     2 --> Solicitud de fichaje no realizado
        '''     3 --> Solicitud de justificación de fichaje
        '''     4 --> Solicitud de trabajo externo
        '''     5 --> Solicitud de cambio de horario
        '''     6 --> Solicitud de vacaciones
        '''     7 --> Solicitud de ausencia por dias
        '''     8 --> ---
        '''     9 --> Solicitud de ausencia por horas
        '''     10 --> solicitud de fichaje de tareas no realizado
        '''     11 --> Solicitud de cancelación de vacaciones
        '''     12 --> Solcitud de fichaje de centro de costo no realizado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestType As Integer

        ''' <summary>
        ''' Si el empleado dispone de permiso para crear dicha solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Permission As Boolean

        ''' <summary>
        ''' Indica el listado de justificaciones para la solicitud que se gestionarán de forma automática
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AutomaticReasons As Integer()

        ''' <summary>
        ''' Indica si la solicitud para el empleado debe mostrar rankings de posiciones
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HasRankings As Boolean
    End Class

    ''' <summary>
    ''' Permisos relacionados con los fichajes del empleado
    ''' </summary>
    <DataContract()>
    Public Class PunchPermissions
        ''' <summary>
        ''' El empleado puede crear un fichaje de presencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SchedulePunch As Boolean

        ''' <summary>
        ''' El empleado puede crear un fichaje de tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiVPunch As Boolean

        ''' <summary>
        ''' El empleado puede crear un fichaje de centro de costo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CostCenterPunch As Boolean

        ''' <summary>
        ''' El empleado puede consultar alguno de sus listado de fichajes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Query As Boolean

        ''' <summary>
        ''' El empleado puede consultar su listado de fichajes de presencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ScheduleQuery As Boolean

        ''' <summary>
        ''' El empleado puede consultar su listado de fichajes de presencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductiVQuery As Boolean
    End Class

    ''' <summary>
    ''' Permisos de un empelado relacionados con gestión horaria
    ''' </summary>
    <DataContract()>
    Public Class SchedulePermissions
        ''' <summary>
        ''' El empleado puede consultar los saldos de tareas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ProductivAccruals As Boolean

        ''' <summary>
        ''' El empleado puede consultar los saldos de presencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ScheduleAccruals As Boolean

        ''' <summary>
        ''' El empleado puede consultar su planificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property QuerySchedule As Boolean
    End Class

    ''' <summary>
    ''' Permisos de un empelado relacionados con declaración de jornada
    ''' </summary>
    <DataContract()>
    Public Class DailyRecordPermissions
        ''' <summary>
        ''' El empleado puede consultar su calendario de declaración de jornada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property QueryDailyRecord As Boolean
    End Class

    ''' <summary>
    ''' Conjunto de permisos de un empleado
    ''' </summary>
    <DataContract()>
    Public Class PermissionList

        Public Sub New()
            Requests = {}
            Punch = New PunchPermissions
            Schedule = New SchedulePermissions
            DailyRecord = New DailyRecordPermissions
            MustUseGPS = False
            MustUsePhoto = False
            CanCreateRequests = False
            Status = 0
        End Sub

        ''' <summary>
        ''' Indica si el empleado debe fichar utilizando posición gps
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MustUseGPS As Boolean

        ''' <summary>
        ''' Indica si el empleado debe seleccionar una zona en caso de no ser geolocalizado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MustSelectZone As Boolean

        ''' <summary>
        ''' Indica si el empleado debe fichar utilizando foto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MustUsePhoto As Boolean

        ''' <summary>
        ''' Indica si el empleado tiene los documentos instalados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DocumentsEnabled As Boolean

        ''' <summary>
        ''' Indica si el empleado tiene los documentos instalados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LeavesEnabled As Boolean

        ''' <summary>
        ''' El empleado dispone de permisos para crear solicitudes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCreateRequests As Boolean

        ''' <summary>
        ''' Array de los permisos para cada uno de los distintos tipos de solicitudes de VisualTime
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Requests As ReqPermission()

        ''' <summary>
        ''' Permisos sobre los fichajes del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Punch As PunchPermissions

        ''' <summary>
        ''' Permisos sobre gestión horaria del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Schedule As SchedulePermissions

        ''' <summary>
        ''' Permisos sobre declaración de jornada del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DailyRecord As DailyRecordPermissions
        ''' <summary>
        ''' Permisos sobre la consulta de la ficha del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property UserFieldQuery As Boolean

        ''' <summary>
        ''' Permisos sobre la cancelación de ausencias previstas
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Forecast As Boolean

        ''' <summary>
        ''' Estado de la petición de permisos del empleado
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

        ''' <summary>
        ''' El empleado dispone de permisos de configuración de terminales
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Terminals As Boolean
    End Class

End Namespace