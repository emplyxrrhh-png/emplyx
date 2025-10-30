Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum DataLinkGuideResultEnum
        <EnumMember> NoError
        <EnumMember> Exception
        <EnumMember> ConnectionError
        <EnumMember> NoSuchGuide
        <EnumMember> ErrorSavingGuides
        <EnumMember> ExportTemplateRequired
        <EnumMember> ImportTemplateRequired
        <EnumMember> ExportLocationRequired
        <EnumMember> ImportLocationRequired
        <EnumMember> SourceFileRequired
        <EnumMember> ExportFileRequired
        <EnumMember> ScheduleRequired
        <EnumMember> EmployeeFilterRequired
        <EnumMember> DatePeriodRequired
        <EnumMember> NoGuideDataToSave
        <EnumMember> ErrorSettingNextExecutionTime
    End Enum

    <DataContract()>
    Public Enum roDatalinkConcept
        <EnumMember> Employees
        <EnumMember> Accruals
        <EnumMember> Schedule
        <EnumMember> Punches
        <EnumMember> Causes
        <EnumMember> Requests
        <EnumMember> Absences
        <EnumMember> Tasks
        <EnumMember> Costs
        <EnumMember> Dinner
        <EnumMember> CustomArgal
        <EnumMember> ZCustom
        <EnumMember> Iberper
        <EnumMember> Supervisors
        <EnumMember> IvecoMDOMadridD 'MDO Madrid Diaria (ID 10005)
        <EnumMember> IvecoMDOMadridM 'MDO Logistic Madrid Diaria (ID 10014)
        <EnumMember> IvecoMDOLogisticMadridD 'MDO Madrid Mensual (ID 10007)
        <EnumMember> IvecoMDOLogisticMadridM 'MDO Logistic Madrid Mensual (ID 10015)
        <EnumMember> IvecoMDOMadridMWorkanalysis 'MDO Madrid Mensual - Workanalysis (ID 10018).
        <EnumMember> IvecoMDOLogisticMWorkanalysis 'MDO Logistic Mensual - Workanalysis (ID 10019).
        <EnumMember> IvecoMDOValladolidM 'MDO Valladolid Mensual (ID 10006)
        <EnumMember> IvecoMDOValladolidD 'MDO Valladolid- Diaria (ID 10004)
        <EnumMember> IvecoPlusPresenciaMadrid 'Plus presencia- MADRID (ID 10016, 10017)
        <EnumMember> IvecoPlusPresenciaValladolid 'Plus presencia- Valladolid 
        <EnumMember> IvecoWorkanalysisReportM 'WORKANALISIS – REPORT MENSUAL (ID 10022)
        <EnumMember> IvecoWorkanalysisHoursSection 'Workanalysis - Horas trabajadas diarias por sección  (ID 10021)
        <EnumMember> IvecoWorkanalysisHoursEmployee 'Workanalysis - Horas trabajadas diarias por empleado   (ID 10020)
        <EnumMember> IvecoConguallo 'Expotacion Conguallo (ID 10024)
        <EnumMember> IvecoJobHistory 'Exportación Historial de Puestos  (ID 8995)
        <EnumMember> IvecoCategoryChange 'Exportación HISTORIAL CAMBIO DE CATEGORIA (ID 8997)
        <EnumMember> VSLWorkSheets 'Importación de partes de trabajo de VSL
        <EnumMember> LivenDailyCauses ' Exportacion de justificaciones diarias LIVEN (ID 9880)
        <EnumMember> RosRocaDynamics 'Exportación a dynamics (ID 9880)
        <EnumMember> TisvolPrimas 'Exportación Primas (ID 10026)
        <EnumMember> TisvolPrimasSummary 'Exportación Primas Summary (ID 10028)
        <EnumMember> TisvolFichajes 'Exportación Primas Summary (ID 10029)








    End Enum

    <DataContract()>
    Public Enum roDatalinkExecutionMode
        <EnumMember> Undefined
        <EnumMember> Manual
        <EnumMember> Automatic
    End Enum

    <DataContract()>
    Public Enum roDatalinkFormat
        <EnumMember> Excel
        <EnumMember> Text
    End Enum

    <DataContract()>
    Public Class roDatalinkGuideResponse

        Public Sub New()
            Guide = New roDatalinkGuide
            oState = New roWsState
        End Sub

        <DataMember()>
        Public Property Guide As roDatalinkGuide

        <DataMember>
        Public Property oState As roWsState

    End Class

    <DataContract>
    Public Class roDatalinkTemplateBytesResponse

        Public Sub New()
            Content = {}
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Content As Byte()

        <DataMember>
        Public Property oState As roWsState

    End Class

    <DataContract()>
    Public Class roDatalinkGuideListResponse

        Public Sub New()
            Guides = {}
            oState = New roWsState
        End Sub

        <DataMember()>
        Public Property Guides As roDatalinkGuide()

        <DataMember>
        Public Property oState As roWsState

    End Class

    <DataContract()>
    Public Class roDatalinkGuide
        Private _concept As roDatalinkConcept
        Private _name As String
        Private _importDescription As String
        Private _exportDescription As String
        Private _import As roDatalinkImportGuide
        Private _export As roDatalinkExportGuide
        Private _isCustom As Boolean

        Public Sub New()
            _concept = roDatalinkConcept.Employees
            _name = String.Empty
            _importDescription = String.Empty
            _exportDescription = String.Empty
            _import = Nothing
            _export = Nothing
            _isCustom = False
        End Sub

        <DataMember()>
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        <DataMember()>
        Public Property ImportDescription As String
            Get
                Return _importDescription
            End Get
            Set(value As String)
                _importDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportDescription As String
            Get
                Return _exportDescription
            End Get
            Set(value As String)
                _exportDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property Concept As roDatalinkConcept
            Get
                Return _concept
            End Get
            Set(value As roDatalinkConcept)
                _concept = value
            End Set
        End Property

        <DataMember()>
        Public Property Import As roDatalinkImportGuide
            Get
                Return _import
            End Get
            Set(value As roDatalinkImportGuide)
                _import = value
            End Set
        End Property

        <DataMember()>
        Public Property Export As roDatalinkExportGuide
            Get
                Return _export
            End Get
            Set(value As roDatalinkExportGuide)
                _export = value
            End Set
        End Property
        <DataMember()>
        Public Property IsCustom As Boolean
            Get
                Return _isCustom
            End Get
            Set(value As Boolean)
                _isCustom = value
            End Set
        End Property
    End Class

    <DataContract(),
    KnownType(GetType(roDatalinkImportGuide)),
    KnownType(GetType(roDatalinkExportGuide))>
    Public Class roDatalinkGuideBase
        Private _ExecutionMode As roDatalinkExecutionMode
        Private _templates() As roDatalinkGuideTemplateBase
        Private _id As Integer

        Private _IsActive As Boolean
        Private _IsEnabled As Boolean
        Private _Version As Integer
        Private _FeatureAliasID As String
        Private _RequiredFunctionalities As String
        Private _IsCustom As Boolean
        Private _Name As String

        Public Sub New()
            _templates = {}
            _ExecutionMode = roDatalinkExecutionMode.Manual
            _Version = 2
            _IsEnabled = False
            _IsActive = False
            _FeatureAliasID = String.Empty
            _RequiredFunctionalities = String.Empty
            _IsCustom = False
            _Name = String.Empty
        End Sub

        <DataMember()>
        Public Property Id As Integer
            Get
                Return _id
            End Get
            Set(value As Integer)
                _id = value
            End Set
        End Property

        <DataMember()>
        Public Property Templates As roDatalinkGuideTemplateBase()
            Get
                Return _templates
            End Get
            Set(value As roDatalinkGuideTemplateBase())
                _templates = value
            End Set
        End Property

        <DataMember()>
        Public Property ExecutionMode As roDatalinkExecutionMode
            Get
                Return _ExecutionMode
            End Get
            Set(value As roDatalinkExecutionMode)
                _ExecutionMode = value
            End Set
        End Property

        <DataMember()>
        Public Property Version As Integer
            Get
                Return _Version
            End Get
            Set(value As Integer)
                _Version = value
            End Set
        End Property

        <DataMember()>
        Public Property IsActive As Boolean
            Get
                Return _IsActive
            End Get
            Set(value As Boolean)
                _IsActive = value
            End Set
        End Property

        <DataMember()>
        Public Property IsEnabled As Boolean
            Get
                Return _IsEnabled
            End Get
            Set(value As Boolean)
                _IsEnabled = value
            End Set
        End Property

        <DataMember()>
        Public Property FeatureAliasID As String
            Get
                Return _FeatureAliasID
            End Get
            Set(value As String)
                _FeatureAliasID = value
            End Set
        End Property

        <DataMember()>
        Public Property RequiredFunctionalities As String
            Get
                Return _RequiredFunctionalities
            End Get
            Set(value As String)
                _RequiredFunctionalities = value
            End Set
        End Property

        <DataMember()>
        Public Property IsCustom As Boolean
            Get
                Return _IsCustom
            End Get
            Set(value As Boolean)
                _IsCustom = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roDatalinkImportGuide
        Inherits roDatalinkGuideBase

        Private _LastExecutionLog As String
        Private _CopySource As Boolean
        Private _sourceFilePath As String
        Private _formatFilePath As String
        Private _Separator As String
        Private _Concept As String
        Private _FileType As Integer
        Private _location As String
        Private _IdDefaultTemplate As Integer

        Public Sub New()
            MyBase.New
            _CopySource = False
            _LastExecutionLog = String.Empty
            _sourceFilePath = String.Empty
            _formatFilePath = String.Empty
            _location = String.Empty
            _IdDefaultTemplate = 0
        End Sub

        <DataMember()>
        Public Property SourceFilePath As String
            Get
                Return _sourceFilePath
            End Get
            Set(value As String)
                _sourceFilePath = value
            End Set
        End Property

        <DataMember()>
        Public Property FormatFilePath As String
            Get
                Return _formatFilePath
            End Get
            Set(value As String)
                _formatFilePath = value
            End Set
        End Property

        <DataMember()>
        Public Property CopySource As Boolean
            Get
                Return _CopySource
            End Get
            Set(value As Boolean)
                _CopySource = value
            End Set
        End Property

        <DataMember()>
        Public Property Separator As String
            Get
                Return _Separator
            End Get
            Set(value As String)
                _Separator = value
            End Set
        End Property

        <DataMember()>
        Public Property Concept As String
            Get
                Return _Concept
            End Get
            Set(value As String)
                _Concept = value
            End Set
        End Property

        <DataMember()>
        Public Property LastExecutionLog As String
            Get
                Return _LastExecutionLog
            End Get
            Set(value As String)
                _LastExecutionLog = value
            End Set
        End Property

        <DataMember()>
        Public Property FileType As Integer
            Get
                Return _FileType
            End Get
            Set(value As Integer)
                _FileType = value
            End Set
        End Property

        <DataMember()>
        Public Property Location As String
            Get
                Return _location
            End Get
            Set(value As String)
                _location = value
            End Set
        End Property

        <DataMember()>
        Public Property IdDefaultTemplate As Integer
            Get
                Return _IdDefaultTemplate
            End Get
            Set(value As Integer)
                _IdDefaultTemplate = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkExportGuide
        Inherits roDatalinkGuideBase

        Private _ProfileType As Integer
        Private _Concept As String
        Private _DefaultSeparator As String
        Private _Schedules As roDatalinkExportSchedule()


        Public Sub New()
            MyBase.New
            _ProfileType = 0
        End Sub

        <DataMember()>
        Public Property ProfileType As Integer
            Get
                Return _ProfileType
            End Get
            Set(value As Integer)
                _ProfileType = value
            End Set
        End Property

        <DataMember()>
        Public Property Concept As String
            Get
                Return _Concept
            End Get
            Set(value As String)
                _Concept = value
            End Set
        End Property

        <DataMember()>
        Public Property DefaultSeparator As String
            Get
                Return _DefaultSeparator
            End Get
            Set(value As String)
                _DefaultSeparator = value
            End Set
        End Property

        <DataMember()>
        Public Property Schedules As roDatalinkExportSchedule()
            Get
                Return _Schedules
            End Get
            Set(value As roDatalinkExportSchedule())
                _Schedules = value
            End Set
        End Property
    End Class

    <DataContract()>
    Public Class roDatalinkExportSchedule
        Private _id As Integer
        Private _name As String

        Private _exportFileName As String
        Private _scheduler As roReportSchedulerSchedule
        Private _employeeFilter As String
        Private _automaticDatePeriod As String
        Private _nextExecution As Date
        Private _applyLockDate As Boolean
        Private _DataSourceFilename As String
        Private _exportFileType As Integer
        Private _separator As String
        Private _LastExecutionLog As String
        Private _location As String
        Private _idTemplate As Integer
        Private _idGuide As Integer
        Private _WSParameters As String
        Private _ExportFileNameTimeStampFormat As String
        Private _Active As Boolean
        Private _Enabled As Boolean

        Public Sub New()
            _id = -1
            _name = String.Empty
            _exportFileName = String.Empty
            _scheduler = Nothing
            _employeeFilter = String.Empty
            _automaticDatePeriod = String.Empty
            _LastExecutionLog = String.Empty
            _location = String.Empty
            _idTemplate = -1
            _idGuide = -1
            _WSParameters = String.Empty
            _ExportFileNameTimeStampFormat = String.Empty
            _Active = False
            _Enabled = False
        End Sub

        <DataMember()>
        Public Property Id As Integer
            Get
                Return _id
            End Get
            Set(value As Integer)
                _id = value
            End Set
        End Property

        <DataMember()>
        Public Property Active As Boolean
            Get
                Return _Active
            End Get
            Set(value As Boolean)
                _Active = value
            End Set
        End Property

        <DataMember()>
        Public Property Enabled As Boolean
            Get
                Return _Enabled
            End Get
            Set(value As Boolean)
                _Enabled = value
            End Set
        End Property

        <DataMember()>
        Public Property IdGuide As Integer
            Get
                Return _idGuide
            End Get
            Set(value As Integer)
                _idGuide = value
            End Set
        End Property

        <DataMember()>
        Public Property IdTemplate As Integer
            Get
                Return _idTemplate
            End Get
            Set(value As Integer)
                _idTemplate = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _name
            End Get
            Set(value As String)
                _name = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportFileName As String
            Get
                Return _exportFileName
            End Get
            Set(value As String)
                _exportFileName = value
            End Set
        End Property

        <DataMember()>
        Public Property Scheduler As roReportSchedulerSchedule
            Get
                Return _scheduler
            End Get
            Set(value As roReportSchedulerSchedule)
                _scheduler = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeeFilter As String
            Get
                Return _employeeFilter
            End Get
            Set(value As String)
                _employeeFilter = value
            End Set
        End Property

        <DataMember()>
        Public Property AutomaticDatePeriod As String
            Get
                Return _automaticDatePeriod
            End Get
            Set(value As String)
                _automaticDatePeriod = value
            End Set
        End Property

        <DataMember()>
        Public Property NextExecutionDate As Date
            Get
                Return _nextExecution
            End Get
            Set(value As Date)
                _nextExecution = value
            End Set
        End Property

        <DataMember()>
        Public Property ApplyLockDate As Boolean
            Get
                Return _applyLockDate
            End Get
            Set(value As Boolean)
                _applyLockDate = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del fichero en el que se encuentra la select para importaciones a partir de SQL
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DataSourceFileName As String
            Get
                Return _DataSourceFilename
            End Get
            Set(value As String)
                _DataSourceFilename = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportFileType As Integer
            Get
                Return _exportFileType
            End Get
            Set(value As Integer)
                _exportFileType = value
            End Set
        End Property

        <DataMember()>
        Public Property Separator As String
            Get
                Return _separator
            End Get
            Set(value As String)
                _separator = value
            End Set
        End Property

        <DataMember()>
        Public Property LastExecutionLog As String
            Get
                Return _LastExecutionLog
            End Get
            Set(value As String)
                _LastExecutionLog = value
            End Set
        End Property

        <DataMember()>
        Public Property Location As String
            Get
                Return _location
            End Get
            Set(value As String)
                _location = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportFileNameTimeStampFormat As String
            Get
                Return _ExportFileNameTimeStampFormat
            End Get
            Set(value As String)
                _ExportFileNameTimeStampFormat = value
            End Set
        End Property

        <DataMember()>
        Public Property WSParameters As String
            Get
                Return _WSParameters
            End Get
            Set(value As String)
                _WSParameters = value
            End Set
        End Property

    End Class

    <DataContract(),
    KnownType(GetType(roDatalinkImportGuideTemplate)),
    KnownType(GetType(roDatalinkExportGuideTemplate))>
    Public Class roDatalinkGuideTemplateBase
        Private _Id As Integer
        Private _IsDefault As Boolean
        Private _Name As String
        Private _IdName As String
        Private _TemplateFile As String
        Private _IdParentGuide As Integer

        Public Sub New()
            _Id = -1
            _IdParentGuide = -1
            _IsDefault = False
            _Name = String.Empty
            _IdName = String.Empty
            _TemplateFile = String.Empty
        End Sub

        <DataMember()>
        Public Property ID As Integer
            Get
                Return _Id
            End Get
            Set(value As Integer)
                _Id = value
            End Set
        End Property

        <DataMember()>
        Public Property IdParentGuide As Integer
            Get
                Return _IdParentGuide
            End Get
            Set(value As Integer)
                _IdParentGuide = value
            End Set
        End Property

        <DataMember()>
        Public Property IsDefault As Boolean
            Get
                Return _IsDefault
            End Get
            Set(value As Boolean)
                _IsDefault = value
            End Set
        End Property

        <DataMember()>
        Public Property TemplateFile As String
            Get
                Return _TemplateFile
            End Get
            Set(value As String)
                _TemplateFile = value
            End Set
        End Property

        <DataMember()>
        Public Property Name As String
            Get
                Return _Name
            End Get
            Set(value As String)
                _Name = value
            End Set
        End Property

        <DataMember()>
        Public Property IdName As String
            Get
                Return _IdName
            End Get
            Set(value As String)
                _IdName = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkImportGuideTemplate
        Inherits roDatalinkGuideTemplateBase

        Private _PreProcessScript As String

        Public Sub New()
            MyBase.New
            _PreProcessScript = String.Empty
        End Sub

        <DataMember()>
        Public Property PreProcessScript As String
            Get
                Return _PreProcessScript
            End Get
            Set(value As String)
                _PreProcessScript = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkExportGuideTemplate
        Inherits roDatalinkGuideTemplateBase

        Private _PostProcessScript As String

        Public Sub New()
            MyBase.New
        End Sub

        <DataMember()>
        Public Property PostProcessScript As String
            Get
                Return _PostProcessScript
            End Get
            Set(value As String)
                _PostProcessScript = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkExportParameters

        Private _BeginDate As Date
        Private _EndDate As Date
        Private _EmployeesSelected As String
        Private _IdConceptGroup As Integer
        Private _LockDataAfterExport As Boolean
        Private _IDPassport As Integer
        Private _IsProgrammed As Boolean
        Private _Destination As String
        Private _FileName As String
        Private _FileType As String
        Private _IDTemplate As Integer
        Private _Separator As String
        Private _IDSchedule As Integer

        <DataMember()>
        Public Property BeginDate As Date
            Get
                Return _BeginDate
            End Get
            Set(value As Date)
                _BeginDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate As Date
            Get
                Return _EndDate
            End Get
            Set(value As Date)
                _EndDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeesSelected As String
            Get
                Return _EmployeesSelected
            End Get
            Set(value As String)
                _EmployeesSelected = value
            End Set
        End Property

        <DataMember()>
        Public Property IdConceptGroup As Integer
            Get
                Return _IdConceptGroup
            End Get
            Set(value As Integer)
                _IdConceptGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property IdTemplate As Integer
            Get
                Return _IDTemplate
            End Get
            Set(value As Integer)
                _IDTemplate = value
            End Set
        End Property

        <DataMember()>
        Public Property LockDataAfterExport As Boolean
            Get
                Return _LockDataAfterExport
            End Get
            Set(value As Boolean)
                _LockDataAfterExport = value
            End Set
        End Property

        <DataMember()>
        Public Property IdPassport As Integer
            Get
                Return _IDPassport
            End Get
            Set(value As Integer)
                _IDPassport = value
            End Set
        End Property

        <DataMember()>
        Public Property IdSchedule As Integer
            Get
                Return _IDSchedule
            End Get
            Set(value As Integer)
                _IDSchedule = value
            End Set
        End Property

        <DataMember()>
        Public Property IsProgrammed As Boolean
            Get
                Return _IsProgrammed
            End Get
            Set(value As Boolean)
                _IsProgrammed = value
            End Set
        End Property

        <DataMember()>
        Public Property Destination As String
            Get
                Return _Destination
            End Get
            Set(value As String)
                _Destination = value
            End Set
        End Property

        <DataMember()>
        Public Property FileName As String
            Get
                Return _FileName
            End Get
            Set(value As String)
                _FileName = value
            End Set
        End Property

        <DataMember()>
        Public Property FileType As String
            Get
                Return _FileType
            End Get
            Set(value As String)
                _FileType = value
            End Set
        End Property

        <DataMember()>
        Public Property Separator As String
            Get
                Return _Separator
            End Get
            Set(value As String)
                _Separator = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkExportTask

        Private _ExportGuide As roDatalinkExportGuide
        Private _ExportParameters As roDatalinkExportParameters
        Private _ExportResultFileName As String

        <DataMember()>
        Public Property ExportGuide As roDatalinkExportGuide
            Get
                Return _ExportGuide
            End Get
            Set(value As roDatalinkExportGuide)
                _ExportGuide = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportParameters As roDatalinkExportParameters
            Get
                Return _ExportParameters
            End Get
            Set(value As roDatalinkExportParameters)
                _ExportParameters = value
            End Set
        End Property

        <DataMember()>
        Public Property ExportResultFileName As String
            Get
                Return _ExportResultFileName
            End Get
            Set(value As String)
                _ExportResultFileName = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkImportParameters

        Private _BeginDate As Date
        Private _EndDate As Date
        Private _EmployeesSelected As String
        Private _IDPassport As Integer
        Private _OriginalFileName As String
        Private _SchemaFileName As String

        Private _CalendarExcelIsTemplate As Boolean
        Private _CalendarCopyMainShifts As Boolean
        Private _CalendarCopyHolidays As Boolean
        Private _CalendarKeepHolidays As Boolean
        Private _CalendarKeepLockedDays As Boolean
        Private _IsProgrammed As Boolean
        Private _Separator As String
        Private _FileType As Integer
        Private _IDTemplate As Integer

        <DataMember()>
        Public Property BeginDate As Date
            Get
                Return _BeginDate
            End Get
            Set(value As Date)
                _BeginDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate As Date
            Get
                Return _EndDate
            End Get
            Set(value As Date)
                _EndDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeesSelected As String
            Get
                Return _EmployeesSelected
            End Get
            Set(value As String)
                _EmployeesSelected = value
            End Set
        End Property

        <DataMember()>
        Public Property OriginalFileName As String
            Get
                Return _OriginalFileName
            End Get
            Set(value As String)
                _OriginalFileName = value
            End Set
        End Property

        <DataMember()>
        Public Property SchemaFileName As String
            Get
                Return _SchemaFileName
            End Get
            Set(value As String)
                _SchemaFileName = value
            End Set
        End Property

        <DataMember()>
        Public Property IdTemplate As Integer
            Get
                Return _IDTemplate
            End Get
            Set(value As Integer)
                _IDTemplate = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarExcelIsTemplate As Boolean
            Get
                Return _CalendarExcelIsTemplate
            End Get
            Set(value As Boolean)
                _CalendarExcelIsTemplate = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarCopyMainShifts As Boolean
            Get
                Return _CalendarCopyMainShifts
            End Get
            Set(value As Boolean)
                _CalendarCopyMainShifts = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarCopyHolidays As Boolean
            Get
                Return _CalendarCopyHolidays
            End Get
            Set(value As Boolean)
                _CalendarCopyHolidays = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarKeepHolidays As Boolean
            Get
                Return _CalendarKeepHolidays
            End Get
            Set(value As Boolean)
                _CalendarKeepHolidays = value
            End Set
        End Property

        <DataMember()>
        Public Property CalendarKeepLockedDays As Boolean
            Get
                Return _CalendarKeepLockedDays
            End Get
            Set(value As Boolean)
                _CalendarKeepLockedDays = value
            End Set
        End Property

        <DataMember()>
        Public Property IdPassport As Integer
            Get
                Return _IDPassport
            End Get
            Set(value As Integer)
                _IDPassport = value
            End Set
        End Property

        <DataMember()>
        Public Property IsProgrammed As Boolean
            Get
                Return _IsProgrammed
            End Get
            Set(value As Boolean)
                _IsProgrammed = value
            End Set
        End Property

        <DataMember()>
        Public Property Separator As String
            Get
                Return _Separator
            End Get
            Set(value As String)
                _Separator = value
            End Set
        End Property

        <DataMember()>
        Public Property FileType As Integer
            Get
                Return _FileType
            End Get
            Set(value As Integer)
                _FileType = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roDatalinkImportTask

        Private _ImportGuide As roDatalinkImportGuide
        Private _ImportParameters As roDatalinkImportParameters
        Private _ImportResultFileName As String

        <DataMember()>
        Public Property ImportGuide As roDatalinkImportGuide
            Get
                Return _ImportGuide
            End Get
            Set(value As roDatalinkImportGuide)
                _ImportGuide = value
            End Set
        End Property

        <DataMember()>
        Public Property ImportParameters As roDatalinkImportParameters
            Get
                Return _ImportParameters
            End Get
            Set(value As roDatalinkImportParameters)
                _ImportParameters = value
            End Set
        End Property

        <DataMember()>
        Public Property ImportResultFileName As String
            Get
                Return _ImportResultFileName
            End Get
            Set(value As String)
                _ImportResultFileName = value
            End Set
        End Property

    End Class

End Namespace