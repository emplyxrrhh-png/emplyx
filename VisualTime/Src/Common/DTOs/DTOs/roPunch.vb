Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ActionTypes
        <EnumMember> aCreate = 0
        <EnumMember> aBegin = 1
        <EnumMember> tChange = 2
        <EnumMember> tComplete = 3
        <EnumMember> tNone = 4
    End Enum

    <DataContract>
    Public Enum ValueTypes
        <EnumMember> aValue = 0
        <EnumMember> aList = 1
    End Enum

    <DataContract>
    Public Enum PunchTypeEnum
        <Description("Not defined")>
        <EnumMember> _NOTDEFINDED = 0 ' No definido
        <Description("In")>
        <EnumMember> _IN = 1     ' Entrada
        <Description("Out")>
        <EnumMember> _OUT = 2    ' Salida
        <Description("Automatic presence")>
        <EnumMember> _AUTO = 3   ' Presencia Automatica
        <Description("Task")>
        <EnumMember> _TASK = 4   ' Tarea
        <Description("Valid access")>
        <EnumMember> _AV = 5     ' Acceso válido
        <Description("Access denied")>
        <EnumMember> _AI = 6     ' Acceso denegado
        <Description("Integrated access with presence")>
        <EnumMember> _L = 7      ' Acceso integrado con presencia
        <Description("Entry to geographical area not allowed")>
        <EnumMember> _INI = 8    ' Entrada a zona geografica no permitida
        <Description("Exit to geographical area not allowed")>
        <EnumMember> _OUTI = 9   ' Salida a zona geografica no permitida
        <Description("Dinning")>
        <EnumMember> _DR = 10    ' Comedor
        <Description("Access to valid event")>
        <EnumMember> _AEV = 11   ' Acceso a evento válido    (Control Acceso a Eventos)
        <Description("Invalid event access")>
        <EnumMember> _AEI = 12   ' Acceso a evento NO válido (Control Acceso a Eventos)
        <Description("Cost Center")>
        <EnumMember> _CENTER = 13   ' Centro de coste
        <Description("Duplicated entry")>
        <EnumMember> _RPTIN = 14 ' Entrada repetida
        <Description("Duplicated out")>
        <EnumMember> _RPTOUT = 15 ' Salida repetida
    End Enum

    <DataContract>
    Public Enum PunchFilterType
        <EnumMember> ByTimeStamp
        <EnumMember> ByDatePeriod
    End Enum

    <DataContract>
    Public Enum PunchStatus
        <EnumMember> Indet_
        <EnumMember> In_
        <EnumMember> Out_
        <EnumMember> Task_
    End Enum

    <DataContract>
    Public Enum InvalidTypeEnum
        <EnumMember> NOHP_ = 0     ' Acceso denegado por documentación incorrecta
        <EnumMember> NTIME_ = 1    ' Acceso denegado por fuera de hora
        <EnumMember> NRDR_ = 2     ' Acceso denegado por lector invàlido
        <EnumMember> NERR_ = 3     ' Error interno de la aplicación
        <EnumMember> NCON_ = 4     ' Acceso denegado: sin contrato
        <EnumMember> NFLD_ = 5     ' Acceso denegado: campos de la ficha del empleado
        <EnumMember> NRPT_ = 6     ' Fichaje de comedor repetido
        <EnumMember> NDEF_ = 7     ' Fichaje offline de comedor valido
        <EnumMember> NEVPGA_ = 8   ' Acceso denegado por PGA (Control Acceso a Eventos)
        <EnumMember> NEVBCL_ = 9   ' Blacklist (Control Acceso a Eventos)
        <EnumMember> NEVAPB_ = 10  ' Antipassback (Control Acceso a Eventos)
        <EnumMember> NEVTGE_ = 11  ' Tiempo maximo de grupo excedido (Control Acceso a Eventos)
        <EnumMember> NEVFOR_ = 12  ' Formato de título de acceso incorrecto (Control Acceso a Eventos)
        <EnumMember> NEVCRC_ = 13  ' CRC de título de acceso incorrecto (Control Acceso a Eventos)
        <EnumMember> NEVFUT_ = 14  ' Título de acceso válido a futuro (Control Acceso a Eventos)
        <EnumMember> NEVEXP_ = 15  ' Título de acceso expirado (Control Acceso a Eventos)
        <EnumMember> NEVTIME_ = 16 ' Título de acceso fuera de horario (Control Acceso a Eventos)
        <EnumMember> NAPB = 17     ' Acceso denegado por Antipassback
        <EnumMember> NSRV = 18     ' Acceso denegado por validación de servidor (pdte especificar motivo concreto
        <EnumMember> NSEC = 19     ' Acceso denegado por incumplimiento de máscara o temperatura
        <EnumMember> NNC = 20     ' Acceso concedido pero no completado (El Pozo)
    End Enum

    <DataContract>
    Public Enum VerificationType
        <EnumMember> UNKNOWN = -1
        <EnumMember> AUTOMATIC = 0
        <EnumMember> FP = 1
        <EnumMember> ID = 2
        <EnumMember> PWD = 3
        <EnumMember> CARD = 4
        <EnumMember> FP_OR_PWD = 5
        <EnumMember> FP_OR_CARD = 6
        <EnumMember> CARD_OR_PWD = 7
        <EnumMember> ID_AND_FP = 8
        <EnumMember> CARD_AND_FP = 10
        <EnumMember> CARD_AND_PWD = 11
        <EnumMember> FP_AND_PWD_AND_CARD = 12
        <EnumMember> ID_AND_FP_AND_PWD = 13
        <EnumMember> ID_AND_FP_OR_CARD_AND_FP = 14
        <EnumMember> FACE = 15
        <EnumMember> FACE_AND_FP = 16
        <EnumMember> FACE_AND_PWD = 17
        <EnumMember> FACE_AND_CARD = 18
        <EnumMember> FACE_AND_FP_AND_CARD = 19
        <EnumMember> FACE_AND_FP_AND_PWD = 20
        <EnumMember> FPVEIN = 21
        <EnumMember> FPVEIN_AND_PWD = 22
        <EnumMember> FPVEIN_AND_CARD = 23
        <EnumMember> FPVEIN_AND_PWD_AND_CARD = 24
        <EnumMember> PALM = 25
        <EnumMember> NFC = 99
    End Enum

    <DataContract>
    Public Enum TypeAccessPunchesEnum
        <EnumMember> AccessPunches
        <EnumMember> InvalidAccessPunches
        <EnumMember> AllAccessPunches
    End Enum

    <DataContract>
    Public Enum TypeReliable
        <EnumMember> Reliable
        <EnumMember> NotReliable
    End Enum

    <DataContract>
    Public Enum PunchSeqStatus
        <EnumMember> NoSequence         ' No hay secuencia fichaje presencia iniciada
        <EnumMember> OK                 ' Secuencia correcta.
        <EnumMember> Max_SeqOK          ' Tiempo máximo entre marcajes superado. Secuencia correcta.
        <EnumMember> Max_SeqERR         ' Tiempo máximo entre marcajes superado. Secuencia incorrecta, último marcaje es una entrada.
        <EnumMember> Repited_IN         ' Dos marcajes seguidos. El último marcaje és una entrada.
        <EnumMember> Repited_OUT        ' Dos marcajes seguidos. El último marcaje és una salida.
        <EnumMember> NFC_TAG_NOT_FOUND  ' Se fichó con un tag NFC que no está configurado
    End Enum

    <DataContract>
    Public Enum MovementStatus
        <EnumMember> Indet_
        <EnumMember> In_
        <EnumMember> Out_
    End Enum

    <DataContract>
    Public Enum MovementSeqStatus
        <EnumMember> NoSequence      ' No hay secuencia movimiento presencia iniciada
        <EnumMember> OK              ' Secuencia correcta.
        <EnumMember> Max_SeqOK       ' Tiempo máximo entre marcajes superado. Secuencia correcta.
        <EnumMember> Max_SeqERR      ' Tiempo máximo entre marcajes superado. Secuencia incorrecta, último marcaje es una entrada.
        <EnumMember> Repited_IN      ' Dos marcajes seguidos. El último marcaje és una entrada.
        <EnumMember> Repited_OUT     ' Dos marcajes seguidos. El último marcaje és una salida.
    End Enum

    <DataContract>
    Public Enum PunchSource
        <EnumMember> Unknown         ' Sin determinar
        <EnumMember> Terminal        ' Terminal físico o Portal web
        <EnumMember> Request         ' Solicitud
        <EnumMember> StandardImport  ' Importación de fichajes desde enlaces Estandar
        <EnumMember> CustomImport    ' Importación de fichajes desde enlaces IDN
        <EnumMember> Collector       ' Behaviors de terminales de terceros
        <EnumMember> Supervisor      ' Edición de fichajes
        <EnumMember> WebService      ' Importación/modificación de fichajes desde servicio web
        <EnumMember> System          ' Introducido por el sistema de manera automática (por ejemplo por definición del horario ...)
        <EnumMember> Suprema         ' Terminal suprema
        <EnumMember> PortalApp       ' Portal App
    End Enum

    <DataContract>
    Public Enum NotReliableCause
        <EnumMember> ForgottenPunch             ' Fichaje olvidado
        <EnumMember> TaskForgottenPunch         ' Fichaje de tarea olvidado
        <EnumMember> CostCenterForgottenPunch   ' Fichaje de centro de coste olvidado
        <EnumMember> DailyRecord                ' Fichaje de declaración de jornada pendiente de aprobar        
        <EnumMember> PunchWithoutLocation       ' Fichaje sin geolocalización
        <EnumMember> ExternalWork               ' Parte de fichaje
    End Enum

    <DataContract>
    Public Enum InvalidEntryReason
        <EnumMember> Unknown         ' Motivo desconocido
        <EnumMember> InvalidFormat   ' Formato incorrecto
        <EnumMember> UnknownEmployee ' Empleado no localizado
        <EnumMember> FuturePunch     ' Fichaje futuro
        <EnumMember> InvalidVirtualTerminal     ' No existe terminal virtual
        <EnumMember> InvalidParser   ' Parser no válido
    End Enum

    <DataContract()>
    Public Class roMovePresenceStatus

        <DataMember>
        Public Property LastMoveType As MovementStatus

        <DataMember>
        Public Property LastMoveDateTime As DateTime

        <DataMember>
        Public Property LastMoveId As Integer

        <DataMember>
        Public Property PresenceMinutes As Integer

        <DataMember>
        Public Property PresenceStatus As PresenceStatus

    End Class

    <DataContract()>
    Public Class roPunchPresenceStatus

        <DataMember>
        Public Property LastMoveType As PunchStatus

        <DataMember>
        Public Property LastMoveDateTime As DateTime

        <DataMember>
        Public Property LastPunchId As Integer

        <DataMember>
        Public Property PresenceMinutes As Integer

        <DataMember>
        Public Property PresenceStatus As PresenceStatus

    End Class

    <DataContract()>
    Public Class roPunchInvalidEntry

        <DataMember>
        Public Property Processed As String

        <DataMember>
        Public Property RawData As String

        <DataMember>
        Public Property Behavior As String

        <DataMember>
        Public Property ID As Integer

        <DataMember>
        Public Property CreatedOn As DateTime

    End Class

    <DataContract()>
    Public Class roLastTaskInfoByEmployee

        <DataMember>
        Public Property LastPunchID As Integer

        <DataMember>
        Public Property LastPunchDateTime As DateTime

        <DataMember>
        Public Property LastPunchType As PunchTypeEnum
    End Class

    ''' <summary>
    ''' Representa un fichaje de VisualTime
    ''' </summary>
    <DataContract>
    Public Class roDailyRecordPunch

        ''' <summary>
        ''' Tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Type As PunchTypeEnum

        ''' <summary>
        ''' Fecha a la que pertenece el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ShiftDate As DateTime

        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Identificador de la zona en que se ha realizado el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDZone As Integer

        ''' <summary>
        ''' Nombre de la zona en que se ha realizado el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ZoneName As String

        ''' <summary>
        ''' Identificador de la justificación indicada en el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDCause As Integer

        ''' <summary>
        ''' Nombre de la justificación indicada en el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CauseName As String

        ''' <summary>
        ''' Indica si el fichaje se hizo en teletrabajo (true) o presencial (false)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property InTelecommute As Boolean

        ''' <summary>
        ''' Identificador del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdEmployee As Integer

        ''' <summary>
        ''' Identificador de la declaración de jornada a la que pertenece el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdDailyRecord As Integer

    End Class

End Namespace