Imports System.ComponentModel
Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Tipos de solicitud
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eRequestType
        ''' <summary>
        ''' Ninguno
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> None = 0
        ''' <summary>
        ''' Cambio campos de la ficha
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> UserFieldsChange = 1
        ''' <summary>
        ''' Marcaje olvidado
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ForbiddenPunch = 2
        ''' <summary>
        ''' Justificación marcaje existente
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> JustifyPunch = 3
        ''' <summary>
        ''' Parte de trabajo externo
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ExternalWorkResumePart = 4
        ''' <summary>
        ''' Cambio de horario
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ChangeShift = 5
        ''' <summary>
        ''' Vacaciones o permisos
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> VacationsOrPermissions = 6
        ''' <summary>
        ''' Ausencias previstas
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> PlannedAbsences = 7
        ''' <summary>
        ''' Intercambio de horarios entre empleados
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ExchangeShiftBetweenEmployees = 8
        ''' <summary>
        ''' Incidencias Previstas
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> PlannedCauses = 9
        ''' <summary>
        ''' Incidencias Previstas
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ForbiddenTaskPunch = 10
        ''' <summary>
        ''' Solicitud cancelación de vacaciones
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> CancelHolidays = 11
        ''' <summary>
        ''' Solicitud fichaje olvidado de centros de coste
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ForgottenCostCenterPunch = 12
        ''' <summary>
        ''' Prevision de vacaciones por horas
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> PlannedHolidays = 13
        ''' <summary>
        ''' Prevision de exceso de horas
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> PlannedOvertimes = 14
        ''' <summary>
        ''' Resumen de trabajo externo semanal
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> ExternalWorkWeekResume = 15
        ''' <summary>
        ''' Resumen de teletrabajo
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Telecommute = 16
        ''' <summary>
        ''' Declaración de Jornada
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> DailyRecord = 17
    End Enum

    ''' <summary>
    ''' Estado de la solicitud
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eRequestStatus
        ''' <summary>
        ''' Solicitud Pendiente
        ''' </summary>
        ''' <remarks></remarks>
        <Description("Request not reviewed by a supervisor")>
        <EnumMember> Pending
        ''' <summary>
        ''' Solicitud en curso
        ''' </summary>
        ''' <remarks></remarks>
        <Description("Request approved by a supervisor with no sufficient approval level")>
        <EnumMember> OnGoing
        ''' <summary>
        ''' Solicitud Aceptada
        ''' </summary>
        ''' <remarks></remarks>
        <Description("Request approved")>
        <EnumMember> Accepted
        ''' <summary>
        ''' Solicitud Denegada
        ''' </summary>
        ''' <remarks></remarks>
        <Description("Request denied")>
        <EnumMember> Denied
        ''' <summary>
        ''' Cancelada por el usuario
        ''' </summary>
        ''' <remarks></remarks>
        <Description("Request canceled by user")>
        <EnumMember> Canceled
    End Enum

    ''' <summary>
    ''' Tipos de reglas de solicitud
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eRequestRuleType
        <EnumMember> None = 0
        <EnumMember> NegativeAccrual = 1
        <EnumMember> MaxNumberDays = 2
        <EnumMember> PeriodEnjoyment = 3
        <EnumMember> AutomaticValidation = 4
        <EnumMember> LimitDateRequested = 5
        <EnumMember> MinimumDaysBeforeRequestedDate = 6
        <EnumMember> MaxNotScheduledDays = 7
        <EnumMember> MinCoverageRequiered = 8
        <EnumMember> AutomaticRejection = 9
        <EnumMember> ScheduleRuleToValidate = 10
        <EnumMember> MinimumConsecutiveDays = 11
    End Enum

    Public Enum eActionType
        <EnumMember> Denied_Action = 0
        <EnumMember> WarnAsk_Value = 1
    End Enum

    Public Enum eTypePlannedDay
        <EnumMember> NaturalDay = 0
        <EnumMember> LaboralDay = 1
    End Enum

    Public Enum eTypeEmployeeCriteria
        <EnumMember> AllEmployees = 0
        <EnumMember> FilteredEmployees = 1
    End Enum

    <DataContract()>
    Public Class roEmployeeRankingInformation

        <DataMember>
        Public Property RankingEnabled As Boolean

        <DataMember>
        Public Property Rankings As roRequestEmployeeRanking

        <DataMember>
        Public Property CoverageEnabled As Boolean

        <DataMember>
        Public Property Coverages As roRequestSummaryCoverageOnEmployeeDate

        <DataMember>
        Public Property ActualPosition As Integer

        <DataMember>
        Public Property Status As Long

        Public Sub New()
            Rankings = Nothing
            Coverages = Nothing
            RankingEnabled = False
            CoverageEnabled = False

            ActualPosition = 0
        End Sub

    End Class

    <DataContract()>
    Public Class roRequestEmployeeRanking

        Protected _lstEmployees As roRequestEmployeeRankingPosition()

        Public Sub New()

            _lstEmployees = Nothing

        End Sub

        <DataMember()>
        Public Property RequestEmployeeRankingPositions As roRequestEmployeeRankingPosition()
            Get
                Return _lstEmployees
            End Get
            Set(value As roRequestEmployeeRankingPosition())
                _lstEmployees = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roRequestEmployeeRankingPosition
        Protected _iIDEmployee As Integer
        Protected _iEmployeeName As String
        Protected _iIDPosition As Integer

        Public Sub New()
            _iIDEmployee = -1
            _iEmployeeName = ""
            _iIDPosition = -1

        End Sub

        <DataMember()>
        Public Property IDEmployee As Integer
            Get
                Return _iIDEmployee
            End Get
            Set(value As Integer)
                _iIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property EmployeeName As String
            Get
                Return _iEmployeeName
            End Get
            Set(value As String)
                _iEmployeeName = value
            End Set
        End Property

        <DataMember()>
        Public Property IDPosition As Integer
            Get
                Return _iIDPosition
            End Get
            Set(value As Integer)
                _iIDPosition = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roRequestSummaryCoverageOnEmployeeDate

        Protected _MinCoverage As Double = 0
        Protected _DifferentialQuantity As Double = 0

        Public Sub New()

            _MinCoverage = 0
            _DifferentialQuantity = 0

        End Sub

        <DataMember()>
        Public Property MinCoverage As Double
            Get
                Return _MinCoverage
            End Get
            Set(value As Double)
                _MinCoverage = value
            End Set
        End Property

        <DataMember()>
        Public Property DifferentialQuantity As Double
            Get
                Return _DifferentialQuantity
            End Get
            Set(value As Double)
                _DifferentialQuantity = value
            End Set
        End Property

    End Class

    <DataContract()>
    Public Class roRequestType

        <DataMember>
        Public Property IDType As eRequestType

        <DataMember>
        Public Property TypeDescription As String

        <DataMember>
        Public Property IDCategory As CategoryType

        <DataMember>
        Public Property CategoryOnCause As Boolean

        Public Sub New()
            IDType = eRequestType.None
            TypeDescription = ""
            IDCategory = CategoryType.AttendanceControl
            CategoryOnCause = False
        End Sub

    End Class

End Namespace