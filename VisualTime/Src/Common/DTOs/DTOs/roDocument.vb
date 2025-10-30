Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum DocumentErrorResultDayEnum
        <EnumMember()> NoContract
        <EnumMember()> FreezingDate
        <EnumMember()> PermissionDenied
        <EnumMember()> ShiftWithoutPermission
        <EnumMember()> ShiftNotExist
        <EnumMember()> InvalidStartFloating
        <EnumMember()> InvalidShiftBase
        <EnumMember()> InvalidAreWorkingDay
        <EnumMember()> InvalidComplementaryData
        <EnumMember()> InvalidFloatingData
        <EnumMember()> EmployeeDoesNotExist
        <EnumMember()> DateOutOfScope
        <EnumMember()> GenericInvalidData
        <EnumMember()> UnknownEmployee
        <EnumMember()> UnknownError
        <EnumMember()> GenericError
        <EnumMember()> InvalidShiftDefinition
        <EnumMember()> InvalidAssignmentData
    End Enum

    <DataContract>
    Public Enum DocumentArea
        <EnumMember> Prevention
        <EnumMember> Labor
        <EnumMember> Legal
        <EnumMember> Security
        <EnumMember> Quality
    End Enum

    <DataContract>
    Public Enum ForecastType
        <EnumMember> AbsenceDays
        <EnumMember> AbsenceHours
        <EnumMember> Leave
        <EnumMember> OverWork
        <EnumMember> AnyAbsence
        <EnumMember> Any
        <EnumMember> Request
    End Enum

    <DataContract>
    Public Enum LeaveDocumentType
        <EnumMember> LeaveReport
        <EnumMember> ReturnReport
        <EnumMember> ConfirmationReport
        <EnumMember> NotDefined
        <EnumMember> NotApplicable
    End Enum

    <DataContract>
    Public Enum DocumentLOPDLevel
        <EnumMember> Low = 0
        <EnumMember> Medium = 1
        <EnumMember> High = 2
    End Enum

    ''' <summary>
    ''' Indica a qué aplica el documento
    ''' </summary>
    <DataContract>
    Public Enum DocumentScope
        ''' <summary>
        ''' Documentos relacionados con la ficha del empleado
        ''' </summary>
        <EnumMember>
        EmployeeField = 0
        ''' <summary>
        ''' Documentos relacionados al contrato del empleado
        ''' </summary>
        <EnumMember>
        EmployeeContract = 1
        ''' <summary>
        ''' Documentos de empresa
        ''' </summary>
        <EnumMember>
        Company = 2
        '''' <summary>
        '''' Documentos relativos a una baja o permiso
        '''' </summary>
        <EnumMember>
        LeaveOrPermission = 3
        '''' <summary>
        '''' Documentos relacionados con una justifiación
        '''' </summary>
        <EnumMember>
        CauseNote = 4
        '''' <summary>
        '''' Documentos relacionados con una autorización de acceso de empleado
        '''' </summary>
        <EnumMember>
        EmployeeAccessAuthorization = 5
        '''' <summary>
        '''' Documentos relacionados con una autorización de acceso de empresa
        '''' </summary>
        <EnumMember>
        CompanyAccessAuthorization = 6
        '''' <summary>
        '''' Documentos relativos a comunicados
        '''' </summary>
        <EnumMember>
        Communique = 7
        '''' <summary>
        '''' Documentos relativos a comunicados
        '''' </summary>
        <EnumMember>
        BioCertificate = 8

        '''' <summary>
        '''' Documentos relativos a fichajes
        '''' </summary>
        '<EnumMember>
        'Punch
        '''' <summary>
        '''' Documentos de visitante
        '''' </summary>
        '<EnumMember>
        'Visit
    End Enum

    <DataContract>
    Public Enum DocumentType
        ''' <summary>
        ''' Documentos relativos a empleados
        ''' </summary>
        <EnumMember>
        Employee
        ''' <summary>
        ''' Documentos relativos a empresas
        ''' </summary>
        <EnumMember>
        Company
        ''' <summary>
        ''' Documentos relativos a visitantes
        ''' </summary>
        <EnumMember>
        Visit
    End Enum

    ''' <summary>
    ''' Especifica si se deben tener en cuenta los documentos de este tipo a la hora de validar el acceso de un empleado
    ''' </summary>
    <DataContract>
    Public Enum DocumentAccessValidation
        ''' <summary>
        ''' No se tienen en cuenta
        ''' </summary>
        <EnumMember>
        NonCritical
        ''' <summary>
        ''' Se avisa si hay un documento incorrecto, pero se habilita el acceso igualmente
        ''' </summary>
        <EnumMember>
        Advise
        ''' <summary>
        ''' Si hay documentos incorrectos o no presentados, no se habilita el acceso
        ''' </summary>
        <EnumMember>
        AccessDenied
    End Enum

    ''' <summary>
    ''' Vía por la que se entregó un documento
    ''' </summary>
    <DataContract>
    Public Enum DocumentChannel
        ''' <summary>
        ''' Desktop
        ''' </summary>
        <EnumMember>
        LiveDesktop
        ''' <summary>
        ''' Portal del empleado
        ''' </summary>
        <EnumMember>
        EmployeePortal
        ''' <summary>
        ''' Portal del Supervisor
        ''' </summary>
        <EnumMember>
        SupervisorPortal
        ''' <summary>
        ''' Portal de documentación
        ''' </summary>
        <EnumMember>
        DocumentationPortal
    End Enum

    ''' <summary>
    ''' Estados en los que se puede encontrar un documento en la biblioteca de VisualTime
    ''' </summary>
    <DataContract>
    Public Enum DocumentStatus
        ''' <summary>
        ''' Estado tras ser entregado, si es que requiere validación
        ''' </summary>
        <EnumMember>
        Pending
        ''' <summary>
        ''' Algún supervisor se ha aprobado el documento. Dependiendo del nivel de mando del supervisor, este estad es un estado final
        ''' </summary>
        <EnumMember>
        Validated
        ''' <summary>
        ''' Documento caducado
        ''' </summary>
        <EnumMember>
        Expired
        ''' <summary>
        ''' Documento rechazado
        ''' </summary>
        <EnumMember>
        Rejected
        ''' <summary>
        ''' Documento invalidado. Se llega a este estado tras haber sido validado
        ''' </summary>
        <EnumMember>
        Invalidated
    End Enum

    ''' <summary>
    ''' Indica si un documento que requiere aprobación, y está validado (status=1), es realmente válido en la fecha actual.
    ''' Esto contempla :
    '''   -	Estado validado (status=1)
    '''   -	Si requiere aprobación, está aprobado por supervisor con suficiente nivel de mando
    '''   -	Dentro de periodo de vigencia del documento
    ''' </summary>
    <DataContract>
    Public Enum DocumentValidity
        ''' <summary>
        ''' Pendiente de evaluar
        ''' </summary>
        <EnumMember>
        CheckPending
        ''' <summary>
        ''' Válido en fecha
        ''' </summary>
        <EnumMember>
        CurrentlyValid
        ''' <summary>
        ''' Válido a futuro
        ''' </summary>
        <EnumMember>
        ValidOnFuture
        ''' <summary>
        ''' Documento aprobado y en fecha pero con menor nivel de mando del requerido
        ''' </summary>
        <EnumMember>
        NotEnoughAuthorityLevel
        ''' <summary>
        ''' Documento no válido por cualquier motivo
        ''' </summary>
        <EnumMember>
        Invalid
    End Enum

    ''' <summary>
    ''' Indica el estado de un documento que requiere firma
    ''' </summary>
    <DataContract>
    Public Enum SignStatusEnum
        ''' <summary>
        ''' No aplica
        ''' </summary>
        <EnumMember>
        NA
        ''' <summary>
        ''' Pendiente de firma
        ''' </summary>
        <EnumMember>
        Pending
        ''' <summary>
        ''' En progreso de firma (en ese momento se está realziando el proceso de firma)
        ''' </summary>
        <EnumMember>
        InProgress
        ''' <summary>
        ''' Firmado
        ''' </summary>
        <EnumMember>
        Signed
        ''' <summary>
        ''' Rechazado
        ''' </summary>
        <EnumMember>
        Rejected

    End Enum

    ''' <summary>
    ''' Representa un listado de plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentStandarResponse

        Public Sub New()
            Result = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentTemplateListResponse

        Public Sub New()
            DocumentTemplates = {}
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property DocumentTemplates As roDocumentTemplate()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentTemplateResponse

        Public Sub New()
            DocumentTemplate = New roDocumentTemplate
            oState = New roWsState
        End Sub

        <DataMember>
        Public Property DocumentTemplate As roDocumentTemplate

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentListResponse

        Public Sub New()
            Documents = {}
            oState = New roWsState
        End Sub

        <DataMember>
        Public Property Documents As roDocument()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roEmployeeDocumentsResponse
        <DataMember>
        Public Property Documents As roDocument()

        <DataMember>
        Public Property Status As Integer
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentResponse

        Public Sub New()
            Document = New roDocument
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Document As roDocument

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentPermission

        Public Sub New()
            CanEditDocument = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property CanEditDocument As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roRequestDocument

        Public Sub New()
            DocumentId = 0
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property DocumentId As Integer

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentFile

        Public Sub New()
            DocumentContent = {}
            DocumentName = String.Empty
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property DocumentContent As Byte()

        <DataMember>
        Public Property DocumentName As String

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una plantilla de documento
    ''' </summary>
    <DataContract>
    Public Class roDocumentFileRespone

        Public Sub New()
            DocumentFile = New roDocumentFile
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property DocumentFile As roDocumentFile

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class roDocumentTemplate
        ''' <summary>
        ''' Identificador único de plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Id As Integer
        ''' <summary>
        ''' Nombre de la plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Name As String
        ''' <summary>
        ''' Nombre abreviado de la plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ShortName As String
        ''' <summary>
        ''' Descripción de la plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String
        ''' <summary>
        ''' Tipo de documento: empleado, empresa
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Type As DocumentType
        ''' <summary>
        ''' A quién o qué aplica el documento: ficha de empleado, fichaje, ..., empresa, visita
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Scope As DocumentScope
        ''' <summary>
        ''' Inicio de validez de la plantilla. Antes de esta fecha no puede ser usada en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property BeginValidity As Date
        ''' <summary>
        ''' Fin de vlaidez de la plantilla. Después de esta fecha no puede ser usada en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EndValidity As Date
        ''' <summary>
        ''' Área de aplicación del documento: Prevención, Laboral, Legal, Seguridad, Calidad
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Area As DocumentArea
        ''' <summary>
        ''' Cómo debe afectar una no conformidad de un documento de este tipo: No afecta, Sólo avidar, Denegar acceso
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AccessValidation As DocumentAccessValidation
        ''' <summary>
        ''' Nivel de aprobación mínimo requerido para considerar un documento válido.
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ApprovalLevelRequired As Integer
        ''' <summary>
        ''' Se permite presentar el documento electrónico a los empleados
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EmployeeDeliverAllowed As Boolean
        ''' <summary>
        ''' Se permite presentar el documento electrónico a los supervisores
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property SupervisorDeliverAllowed As Boolean
        ''' <summary>
        ''' Es una plantilla de documento de sistema. No puede ser modificada
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsSystem As Boolean
        ''' <summary>
        ''' Caducidad por defecto de los documentos de este tipo: periodo@número de periodos
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DefaultExpiration As String
        ''' <summary>
        ''' Al aprobar un documento de este tipo, el resto del mismo tipo para el empleado o empresa quedan automáricamente marcados como caducados
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ExpirePrevious As Boolean
        ''' <summary>
        ''' Indica si los documentos de este tipo son de obligada presentación o no
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Compulsory As Boolean
        ''' <summary>
        ''' Nivel de protección de los documentos de este tipo según LOPD: bajo, medio, alto
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property LOPDAccessLevel As Integer
        ''' <summary>
        ''' Días que el documento entregado se mantiene en la biblioteca tras ser introducido en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DaysBeforeDelete As Integer
        ''' <summary>
        ''' Notificaciones necesarias para este tipo de documentos
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Notifications As String
        ''' <summary>
        ''' Para documentos de baja y permiso, tipo de documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property LeaveDocumentType As LeaveDocumentType

        ''' <summary>
        ''' Si el documento requiere firma digital
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property RequiresSigning As Boolean

    End Class

    ''' <summary>
    ''' Representa un documento de la biblioteca (documentos entregados)
    ''' </summary>
    <DataContract>
    Public Class roDocument

        Public Sub New()
            DocumentTemplate = New roDocumentTemplate
        End Sub

        ''' <summary>
        ''' Identificador único de documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Id As Integer
        ''' <summary>
        ''' Título del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Title As String
        ''' <summary>
        ''' Plantilla de documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentTemplate As roDocumentTemplate
        ''' <summary>
        ''' Para documentos relativos a empleados, identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdEmployee As Integer
        ''' <summary>
        ''' Para documentos relativos a empleados, nombre de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EmployeeName As String
        ''' <summary>
        ''' Para documentos relativos a empleados, identificador de contrato del empleado, si aplica.
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdContract As String
        ''' <summary>
        ''' Para documentos relativos a fichajes, identificador del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdPunch As Integer
        ''' <summary>
        ''' Para documentos de ausencia prolongada, identificador de la ausencia
        ''' </summary>
        ''' <returns></returns>'''
        <DataMember>
        Public Property IdDaysAbsence As Integer
        ''' <summary>
        ''' Para documentos de ausencia por horas, identificador de la ausencia
        ''' </summary>
        ''' <returns></returns>'''
        <DataMember>
        Public Property IdHoursAbsence As Integer
        ''' <summary>
        ''' Para documentos de previsiones de horas de exceso, identificador de la previsión
        ''' </summary>
        ''' <returns></returns>'''
        <DataMember>
        Public Property IdOvertimeForecast As Integer
        ''' <summary>
        ''' Para documentos de empresa, identificador de la empresa
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdCompany As Integer
        ''' <summary>
        ''' Documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Document As Byte()
        ''' <summary>
        ''' Tipo de documento (p.ej. xlsx, pdf)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentType As String
        ''' <summary>
        ''' Fecha de registro del documento en la biblioteca de VisualTime
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DeliveredDate As Date
        ''' <summary>
        ''' Canal por el que se recibió el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DeliveryChannel As String
        ''' <summary>
        ''' Nombre de quién registró el documento en la bibioteca
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DeliveredBy As String
        ''' <summary>
        ''' Estado del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As DocumentStatus
        ''' <summary>
        ''' Nivel de mando del último supervisor que cambió el estado del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property StatusLevel As Integer
        ''' <summary>
        ''' Fecha del último cambio en el estado del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property LastStatusChange As Date
        ''' <summary>
        ''' Identificador del supervisor que realizó el último cambio de estado del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdLastStatusSupervisor As Integer
        ''' <summary>
        ''' Identificador de la request (portal)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdRequest As Integer
        ''' <summary>
        ''' Fecha de inicio de validez del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property BeginDate As Date
        ''' <summary>
        ''' Fecha de caducidad del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EndDate As Date
        ''' <summary>
        ''' Observaciones introducidas al guardar el fichero
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Remarks As String
        ''' <summary>
        ''' Observaciones introducidas al denegar o invalidar un fichero
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property SupervisorRemarks As String
        ''' <summary>
        ''' Indica si un documento que requiere aprobación, y está validado (status=1), es realmente válido en la fecha actual
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Validity As DocumentValidity
        ''' <summary>
        ''' Nombre del fichero en el Blob de Azure (si aplica)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property BlobFileName As String
        ''' <summary>
        ''' CRC del fichero (una vez encriptado)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CRC As String

        ''' <summary>
        ''' Identificador de si el empleado ha leído o no el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Received As Integer
        <DataMember>
        Public Property ReceivedDate As Date?

        ''' <summary>
        ''' Para documentos adjuntos a comunicados, identificador del comunicado
        ''' </summary>
        ''' <returns></returns>'''
        <DataMember>
        Public Property IdCommunique As Integer

        ''' <summary>
        ''' Peso del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Weight As Integer

        ''' <summary>
        ''' Estado del proceso de firma
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property SignStatus As SignStatusEnum

        ''' <summary>
        ''' Fecha en que se firmó un documento, si aplica
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property SignDate As Date?

        ''' <summary>
        ''' Informe de evidencias firma digital
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property SignReport As Byte()

        ''' <summary>
        ''' Identificador unico externo de un documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentExternalID As String

    End Class

    ''' <summary>
    ''' Representa las alertas de documentos
    ''' </summary>
    <DataContract>
    Public Class DocumentAlertsRespone

        Public Sub New()
            DocumentAlerts = New DocumentAlerts
            oState = New roWsState
        End Sub

        <DataMember>
        Public Property DocumentAlerts As DocumentAlerts

        <DataMember>
        Public Property oState As roWsState
    End Class

    <DataContract>
    Public Class DocumentAlerts

        Public Sub New()
            DocumentsValidation = {}
            MandatoryDocuments = {}
            AbsenteeismDocuments = {}
            GpaAlerts = {}
            WorkForecastDocuments = {}
            AccessAuthorizationDocuments = {}
        End Sub

        ''' <summary>
        ''' Array con las alertas de documentos pendientes de validar
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentsValidation As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos de bajas no entregados o en estado incorrecto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AbsenteeismDocuments As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos de gpa
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property GpaAlerts As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos de seguimiento no entregados
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property MandatoryDocuments As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos de previsión de trabajo no entregados o en estado incorrecto
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property WorkForecastDocuments As DocumentAlert()

        ''' <summary>
        ''' Array con las alertas de documentos de autorizaciones de acceso
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AccessAuthorizationDocuments As DocumentAlert()
    End Class

    ''' <summary>
    ''' Representa una alerta de un documento con estado pendiente
    ''' </summary>
    <DataContract>
    Public Class DocumentAlert
        ''' <summary>
        ''' Identificador del documento pendiente de validar
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDDocument As Integer
        ''' <summary>
        ''' Identificador de la plantilla de documento pendiente de validar (para cuando sea un documento no entregado)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDDocumentTemplate As Integer
        ''' <summary>
        ''' Fecha desde la que el documento esta pendiente
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
        ''' Identificador del empleado/empresa relacionado con el la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdRelatedObject As Integer
        ''' <summary>
        ''' Identificador del empleado/empresa relacionado con el la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ObjectName As String
        ''' <summary>
        ''' Identificador del ausencia por días al que hace referencia la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdDaysAbsence As Integer
        ''' <summary>
        ''' Identificador del ausencia por días al que hace referencia la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdHoursAbsence As Integer
        ''' <summary>
        ''' Identificador del ausencia por días al que hace referencia la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdOvertimeForecast As Integer
        ''' <summary>
        ''' Identificador de la solicitud a la que hace referencia la alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdRequest As Integer
        ''' <summary>
        ''' Texto descriptivo de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String
        ''' <summary>
        ''' Nombre de la plantilla con alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DocumentTemplateName As String
        ''' <summary>
        ''' Nombre de la plantilla con alerta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Scope del documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Scope As Integer

        ''' <summary>
        ''' Identificador de autorización de acceso
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdAccessAuthorization As Integer

        ''' <summary>
        ''' Cómo afecta a control de accesos esta alerta (0: Entra, 1:Aviso, 2:Prohibe)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AccessValidation As Integer

        Public Sub New()
            DateTime = New DateTime(1970, 1, 1)
        End Sub

    End Class


    Public Enum PayrollRecognitionMechanism
        <EnumMember> ReferenceWord
        <EnumMember> Calibration
    End Enum

    ''' <summary>
    ''' Representa un documento de la biblioteca (documentos entregados)
    ''' </summary>

    <DataContract>
    Public Class PayrollFormTemplate
        ''' <summary>
        ''' Mecanismo usado para localizar el DNI en el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property RecognitionType As PayrollRecognitionMechanism
        ''' <summary>
        ''' Palabra usada como referencia para localizar el DNI en el documento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ReferenceWord As String
        <DataMember>
        Public Property ReferenceOffsetX As Double
        <DataMember>
        Public Property ReferenceOffsetY As Double
        <DataMember>
        Public Property CalibrationX As Double
        <DataMember>
        Public Property CalibrationY As Double
        <DataMember>
        Public Property Width As Double
        <DataMember>
        Public Property Height As Double
    End Class

End Namespace