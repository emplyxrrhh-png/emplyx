Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa las alertas que un empleado debe atender
    ''' </summary>
    <DataContract>
    Public Class SupervisorAlertsTypeDetail

        Public Sub New()
            Alerts = {}
            Status = 0
        End Sub

        ''' <summary>
        ''' Indica si la notificación sebe tratarse con prioridad
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Alerts As DesktopAlertDetail()

        ''' <summary>
        ''' Estado de la información de las alertas
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Representa las alertas que un empleado debe atender
    ''' </summary>
    <DataContract>
    Public Class SupervisorAlerts

        Public Sub New()
            Alerts = {}
            DocumentAlerts = New DocumentAlerts
            Status = 0
        End Sub

        ''' <summary>
        ''' Indica si la notificación sebe tratarse con prioridad
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentAlerts As DocumentAlerts

        ''' <summary>
        ''' Indica si la notificación sebe tratarse con prioridad
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Alerts As SupervisorAlert()

        ''' <summary>
        ''' Estado de la información de las alertas
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Status As Long
    End Class

    <DataContract>
    Public Class DesktopAlertDetail

        ''' <summary>
        ''' Identificador del tipo de alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Id As Integer

        ''' <summary>
        ''' Descripción de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Descripción de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String
    End Class

    ''' <summary>
    ''' Representa las alertas que un empleado debe atender
    ''' </summary>
    <DataContract>
    Public Class SupervisorAlert

        ''' <summary>
        ''' Identificador del tipo de alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertType As Integer

        ''' <summary>
        ''' Descripción de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Descripción de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String

        ''' <summary>
        ''' Indica si la notificación ya es antorior a 7 días
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsUrgent As Boolean

        ''' <summary>
        ''' Indica si la notificación ya es antorior a 7 días
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DetailCount As Integer

        ''' <summary>
        ''' Indica si la notificación sebe tratarse con prioridad
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsCritic As Boolean
    End Class

    ''' <summary>
    ''' Representa las alertas que un empleado debe atender
    ''' </summary>
    <DataContract>
    Public Class EmployeeAlerts

        Public Sub New()
            RequestAlerts = {}
            ExpiredDocAlert = Nothing
            IncompletePunches = {}
            TrackingDocuments = {}
            Notifications = {}
            ForecastDocuments = {}
            Status = 0
        End Sub

        ''' <summary>
        ''' Array con las alertas de solicitudes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property RequestAlerts As RequestAlert()

        ''' <summary>
        ''' Alerta de documentación de PRL expirada en Visualtime
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ExpiredDocAlert As ExpiredDoc

        ''' <summary>
        ''' Array con las alertas de fichajes incompletos o no realizados en Visualtime
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IncompletePunches As IncompletePunch()

        ''' <summary>
        ''' Array con las alertas de documentos de absentismo seguimiento no entregados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property TrackingDocuments As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos no entregados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ForecastDocuments As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de notificaciones que el empleado puede atender
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Notifications As EmployeeNotification()

        ''' <summary>
        ''' Array con las alertas de documentos pendientes de firma
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SignDocuments As DocumentAlert()

        ''' <summary>
        ''' Estado de la información de las alertas
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Representa una notificación de empleado
    ''' </summary>
    <DataContract>
    Public Class EmployeeNotification
        ''' <summary>
        ''' Fecha desde la que el documento esta pendiente
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Nombre del documento que no se ha entregado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' Identificador de la notificación para poder marcarla como leida
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdNotification As Integer

        ''' <summary>
        ''' Identificador del tipo de notificación para poder mostrar los iconos correctos
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdNotificationType As Integer

        ''' <summary>
        ''' Texto descriptivo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String
    End Class

    ''' <summary>
    ''' Representa una alerta de seguimiento no entregada
    ''' </summary>
    <DataContract>
    Public Class TrackingDocument
        ''' <summary>
        ''' Fecha desde la que el documento esta pendiente
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Nombre del documento que no se ha entregado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Name As String

        ''' <summary>
        ''' La alerta es anterior a 3 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsUrgent As Boolean

        ''' <summary>
        ''' La alerta es anterior a 7 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsCritic As Boolean
    End Class

    ''' <summary>
    ''' Representa la alerta de fichaje incompleto o no realizado
    ''' </summary>
    <DataContract>
    Public Class IncompletePunch
        ''' <summary>
        ''' Fecha en la que exite algún fichaje incompleto o no realizado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' La alerta es anterior a 3 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsUrgent As Boolean

        ''' <summary>
        ''' La alerta es anterior a 7 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsCritic As Boolean

        ''' <summary>
        ''' Texto de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertSubject As String

        ''' <summary>
        ''' Texto descriptivo de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertDescription As String
    End Class

    ''' <summary>
    ''' Representa la alerta de cambio de estado de una solicitud
    ''' </summary>
    <DataContract>
    Public Class RequestAlert
        ''' <summary>
        ''' Identificador de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdRequest As Integer

        ''' <summary>
        ''' Estado actual de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Integer

        ''' <summary>
        ''' Fecha de creación de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

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
        <DataMember>
        Public Property IdRequestType As Integer

        ''' <summary>
        ''' La alerta es anterior a 3 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsUrgent As Boolean

        ''' <summary>
        ''' La alerta es anterior a 7 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsCritic As Boolean

        ''' <summary>
        ''' Texto de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertSubject As String

        ''' <summary>
        ''' Texto descriptivo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String

    End Class

    ''' <summary>
    ''' Representa la alerta de documento de prl expirado
    ''' </summary>
    <DataContract>
    Public Class ExpiredDoc
        ''' <summary>
        ''' Fecha en la que ha expirado el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' La alerta es anterior a 3 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsUrgent As Boolean

        ''' <summary>
        ''' La alerta es anterior a 7 dias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsCritic As Boolean

        ''' <summary>
        ''' Texto de la notificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertSubject As String

        ''' <summary>
        ''' Texto descriptivo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AlertDescription As String
    End Class

End Namespace