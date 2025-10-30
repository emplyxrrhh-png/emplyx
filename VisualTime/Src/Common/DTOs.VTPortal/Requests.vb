Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Detalle de una solicitud utilizado en los listados
    ''' </summary>
    <DataContract()>
    Public Class EmpRequest
        ''' <summary>
        ''' Nombre del tipo de solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Estado de la solicitud (0 Pendiente,1 Solicitud en curso,2 Solicitud Aceptada,3 Solicitud Denegada)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Status As Integer

        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IcoStatus As String

        ''' <summary>
        ''' Estado de la solicitud localizado en el idioma del solicitante
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NameStatus As String

        ''' <summary>
        ''' Descripción extendida de la información solicitada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String

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
        '''     13 --> Solcitud de Vacaciones por horas
        '''     14 --> Solcitud de Horas de Exceso
        '''     15 --> Solicitud de trabajo externo
        '''     16 --> Solicitud de teletrabajo
        '''     17 --> Solicitud de Declaración de jornada
        '''
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdRequestType As Integer

        ''' <summary>
        ''' Cadena con el tipo de solicitud(obsoleto)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestType As String

        ''' <summary>
        ''' Indica si la solicitud tiene una alerta de no leida
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NotReaded As Boolean

        ''' <summary>
        ''' Tipo de solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Id As Integer

        ''' <summary>
        ''' Id del empleado propietario de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployee As Integer

        ''' <summary>
        ''' Obsoleto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Title As String

        ''' <summary>
        ''' Solicitudes de declaración de jornada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DailyRecord As roDailyRecord
    End Class

    <DataContract()>
    Public Class ExternalWorkResume
        ''' <summary>
        ''' Inofrmación sobre el dia del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Remark As String

        ''' <summary>
        ''' Identificador de la justificación si atañe, sinó -1
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Dirección del fichaje "E:entrada", "S:Salida"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Direction As String

        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Descripción extendida del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String
    End Class

    ''' <summary>
    ''' Listado de solicitudes de un empleado
    ''' </summary>
    <DataContract()>
    Public Class RequestsList
        ''' <summary>
        ''' Estado de la petición de listado de solicitude
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

        <DataMember()>
        Public DefaultImage As String

        ''' <summary>
        ''' Indica si el empleado dispone de permisos para crear solicitudes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCreateRequest As Boolean

        ''' <summary>
        ''' Listado de solicitudes del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Requests As EmpRequest()

    End Class

    ''' <summary>
    ''' Listado de solicitudes con detalle de un empleado
    ''' </summary>
    <DataContract()>
    Public Class UserRequestsList
        ''' <summary>
        ''' Estado de la petición de listado de solicitude
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

        ''' <summary>
        ''' Indica si el empleado dispone de permisos para crear solicitudes
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CanCreateRequest As Boolean

        ''' <summary>
        ''' Listado de solicitudes del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Requests As UserRequest()
    End Class

    ''' <summary>
    ''' Detalle de una solicitud (la información que contiene depende del tipo de solicitud)
    ''' </summary>
    <DataContract()>
    Public Class UserRequest
        ''' <summary>
        ''' Comentarios que se han añadido al crear la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Comments As String

        ''' <summary>
        ''' Tipo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestType As Integer

        ''' <summary>
        ''' Fecha inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Date1 As DateTime

        ''' <summary>
        ''' Fecha final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Date2 As DateTime

        ''' <summary>
        ''' Fecha inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strDate1 As String

        ''' <summary>
        ''' Fecha final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strDate2 As String

        ''' <summary>
        ''' Nombre del campo de la ficha que se ha solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldName As String

        ''' <summary>
        ''' Valor del campo de la ficha que se ha solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldValue As String

        ''' <summary>
        ''' Hora inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strFromTime As String

        ''' <summary>
        ''' Duración en la solicitud en formato(HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strHours As String

        ''' <summary>
        ''' Hora de horario incial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strStartShift As String

        ''' <summary>
        ''' Hora final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strToTime As String

        ''' <summary>
        ''' Identificador de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Id As Integer

        ''' <summary>
        ''' Identificador del empleado que realiza la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployee As Integer

        ''' <summary>
        ''' Identificador del empleado con el que solicita el cambio de turno
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployeeExchange As Integer

        ''' <summary>
        ''' Identificador de la justificación solicitada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Identificador del horario solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdShift As Integer

        ''' <summary>
        ''' Indica si la solicitud tiene alguna alerta pendietne
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NotReaded As Boolean

        ''' <summary>
        ''' Estado de la solicitud (0 Pendiente,1 Solicitud en curso,2 Solicitud Aceptada,3 Solicitud Denegada)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ReqStatus As Integer

        ''' <summary>
        ''' Identificador de la tarea a la que se quiere cambiar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdTask As Integer

        ''' <summary>
        ''' Identificador de la tarea en la que se continua si se indica final de tarea
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdTask2 As Integer

        ''' <summary>
        ''' Indica si se ha solicitado finalizar la tarea a la que se ha cambiado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CompletedTask As Boolean

        ''' <summary>
        ''' Identificador de centro de costo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCenter As Integer

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field1 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field2 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field3 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field4 As Double

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field5 As Double

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field6 As Double

        ''' <summary>
        ''' Array de historico de aprobaciones de una solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestsHistoryEntries As RequestHistoryEntry()

        ''' <summary>
        ''' Estado de la petición de solicitudes
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Integer

        <DataMember()>
        Public Property AbsenceId As Integer

        ''' <summary>
        ''' Array con el listado de días que se han solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestDays As RequestDays()
    End Class

    ''' <summary>
    ''' Registro en el historico de aprobaciones de una solicitud
    ''' </summary>
    <DataContract()>
    Public Class RequestDays

        Public Sub New()
            RequestDate = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            FromTime = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ToTime = New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            AllDay = False
            Duration = 0
        End Sub

        ''' <summary>
        ''' Fecha de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestDate As Date

        ''' <summary>
        ''' Indica si la ausencia dura todo el día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AllDay As Boolean

        ''' <summary>
        ''' Hora inicial de la ausencia del dia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FromTime As DateTime

        ''' <summary>
        ''' Hora final de la ausencia del día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ToTime As DateTime

        ''' <summary>
        ''' Duranción en horas de la auséncia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Duration As Double

        ''' <summary>
        ''' Id de la justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Tipo del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ActualType As Integer

        ''' <summary>
        ''' Comentario del día
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Remark As String
    End Class

    ''' <summary>
    ''' Registro en el historico de aprobaciones de una solicitud
    ''' </summary>
    <DataContract()>
    Public Class RequestHistoryEntry
        ''' <summary>
        ''' Estado de la acción que se ha producido en el historico de la solicitud
        ''' 0 Pendiente,
        ''' 1 Solicitud en curso,
        ''' 2 Solicitud Aceptada,
        ''' 3 Solicitud Denegada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Action As Integer

        ''' <summary>
        ''' Comentario que se ha añadido al aprobar o denegar la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Comment As String

        ''' <summary>
        ''' Fecha en la que se ha llevado a cabo la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ActionDate As Date

        ''' <summary>
        ''' Nombre del usuario que ha aprobado o denegado la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property User As String
    End Class

    <DataContract()>
    Public Class HolidayAccrualsResume
        ''' <summary>
        ''' Comentarios que se han añadido al crear la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Done As Double
        <DataMember()>
        Public Property Available As Double
        <DataMember()>
        Public Property Pending As Double
        <DataMember()>
        Public Property Lasting As Double
        <DataMember()>
        Public Property Expired As Double
        <DataMember()>
        Public Property WithoutEnjoyment As Double
    End Class

    <DataContract()>
    Public Class UserRequestForSupervisor
        ''' <summary>
        ''' Comentarios que se han añadido al crear la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Comments As String
        <DataMember()>
        Public Property RequestInfo As String

        ''' <summary>
        ''' Tipo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestType As Integer

        <DataMember()>
        Public Property EmployeeAccruals As HolidayAccrualsResume

        ''' <summary>
        ''' Fecha inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Date1 As DateTime

        ''' <summary>
        ''' Fecha final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Date2 As DateTime

        ''' <summary>
        ''' Fecha inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strDate1 As String

        ''' <summary>
        ''' Fecha final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strDate2 As String

        ''' <summary>
        ''' Nombre del campo de la ficha que se ha solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldName As String

        ''' <summary>
        ''' Valor del campo de la ficha que se ha solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FieldValue As String

        ''' <summary>
        ''' Hora inicial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strFromTime As String

        ''' <summary>
        ''' Duración en la solicitud en formato(HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strHours As String

        ''' <summary>
        ''' Hora de horario incial solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strStartShift As String

        ''' <summary>
        ''' Hora final solicitada en la solicitud en formato(YYYY-MM-DD HH:mm)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property strToTime As String

        ''' <summary>
        ''' Identificador de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Id As Integer

        ''' <summary>
        ''' Identificador del empleado que realiza la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployee As Integer

        <DataMember()>
        Public Property EmployeeName As String

        <DataMember()>
        Public Property EmployeeImage As String

        <DataMember()>
        Public Property EmployeeGroup As String

        ''' <summary>
        ''' Identificador del empleado con el que solicita el cambio de turno
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployeeExchange As Integer

        ''' <summary>
        ''' Identificador de la justificación solicitada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Identificador del horario solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdShift As Integer

        ''' <summary>
        ''' Indica si la solicitud tiene alguna alerta pendietne
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property NotReaded As Boolean

        ''' <summary>
        ''' Estado de la solicitud (0 Pendiente,1 Solicitud en curso,2 Solicitud Aceptada,3 Solicitud Denegada)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ReqStatus As Integer

        ''' <summary>
        ''' Identificador de la tarea a la que se quiere cambiar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdTask As Integer

        ''' <summary>
        ''' Identificador de la tarea en la que se continua si se indica final de tarea
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdTask2 As Integer

        ''' <summary>
        ''' Indica si se ha solicitado finalizar la tarea a la que se ha cambiado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property CompletedTask As Boolean

        ''' <summary>
        ''' Identificador de centro de costo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCenter As Integer

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field1 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field2 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field3 As String

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field4 As Double

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field5 As Double

        ''' <summary>
        ''' Información adicional de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Field6 As Double

        ''' <summary>
        ''' Array de historico de aprobaciones de una solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestsHistoryEntries As RequestHistoryEntry()

        ''' <summary>
        ''' Estado de la petición de solicitudes
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Integer

        <DataMember()>
        Public Property AbsenceId As Integer

        <DataMember()>
        Public Property OldShiftName As String

        <DataMember()>
        Public Property NewShiftName As String
        <DataMember()>
        Public Property OldShiftColor As String

        <DataMember()>
        Public Property NewShiftColor As String

        ''' <summary>
        ''' Array con el listado de días que se han solicitado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequestDays As RequestDays()
        <DataMember()>
        Public Property IsAnnualWork As Boolean
    End Class

End Namespace