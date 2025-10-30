Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum PunchSummaryType
        <EnumMember> _NOTDEFINDED = 0 ' No definido
        <EnumMember> _IN = 1     ' Entrada
        <EnumMember> _OUT = 2    ' Salida
        <EnumMember> _AUTO = 3   ' Presencia Automatica
        <EnumMember> _TASK = 4   ' Tarea
        <EnumMember> _AV = 5     ' Acceso válido
        <EnumMember> _AI = 6     ' Acceso denegado
        <EnumMember> _L = 7      ' Acceso integrado con presencia
        <EnumMember> _INI = 8    ' Entrada a zona geografica no permitida
        <EnumMember> _OUTI = 9   ' Salida a zona geografica no permitida
        <EnumMember> _DR = 10    ' Comedor
        <EnumMember> _AEV = 11   ' Acceso a evento válido    (Control Acceso a Eventos)
        <EnumMember> _AEI = 12   ' Acceso a evento NO válido (Control Acceso a Eventos)
        <EnumMember> _CENTER = 13   ' Centro de coste
        <EnumMember> _RPTIN = 14 ' Entrada repetida
        <EnumMember> _RPTOUT = 15 ' Salida repetida
    End Enum

    <DataContract>
    Public Enum SummaryType
        <EnumMember> Anual
        <EnumMember> Mensual
        <EnumMember> Daily
        <EnumMember> Semanal
        <EnumMember> Contrato
        <EnumMember> LastYear
        <EnumMember> LastMonth
        <EnumMember> ChoosePeriod
        <EnumMember> ContractAnnualized
        <EnumMember> NextContractAnnualizedPeriod = 10
    End Enum

    <DataContract>
    Public Enum SummaryRequestType
        <EnumMember> All = 0
        <EnumMember> Punch = 1
        <EnumMember> Schedule = 2
        <EnumMember> Accruals = 3
        <EnumMember> Causes = 4
        <EnumMember> Tasks = 5
        <EnumMember> CostCenter = 6
    End Enum

    <DataContract>
    Public Enum EmployeeState
        <EnumMember> ProgrammedAbsence
        <EnumMember> ProgrammedCause
        <EnumMember> Vacation
        <EnumMember> PresenceIn
        <EnumMember> PresenceOut
        <EnumMember> Planned
        <EnumMember> Unplanned
    End Enum

    <DataContract>
    Public Class roPlanificationSummary
        <DataMember>
        Public Property EmployeeState() As EmployeeState

        <DataMember>
        Public Property Params() As List(Of String)

        <DataMember>
        Public Property Shift() As String

        <DataMember>
        Public Property Asiggment() As String
    End Class

    <DataContract>
    Public Class roEmployeeSummary

        Public Sub New()
            employeeLastPunch = New roLastPunchSummary
            employeePlanification = New roPlanificationSummary
            employeeAccruals = {}
            employeeCauses = {}
            employeeTasks = {}
            employeeBussinessCenters = {}
        End Sub

#Region "Propiedades"

        <DataMember>
        Public Property employeeLastPunch As roLastPunchSummary

        <DataMember>
        Public Property employeePlanification As roPlanificationSummary

        <DataMember>
        Public Property employeeAccruals As roAccrualsSummary()

        <DataMember>
        Public Property employeeCauses As roCausesSummary()

        <DataMember>
        Public Property employeeTasks As roTasksSummary()

        <DataMember>
        Public Property employeeBussinessCenters As roBussinessCentersSummary()

#End Region

    End Class

    <DataContract>
    Public Class roTasksSummary

        <DataMember>
        Public Property IdTask As Integer

        <DataMember>
        Public Property TaskName As String

        <DataMember>
        Public Property TaskValue As Double

        <DataMember>
        Public Property TaskValueFormat As String
    End Class

    <DataContract>
    Public Class roLastPunchSummary

        <DataMember>
        Public Property IdPunch As Integer

        <DataMember>
        Public Property PunchType As PunchSummaryType

        <DataMember>
        Public Property PunchDateTime As Date?

        <DataMember>
        Public Property PunchTerminal As String

        <DataMember>
        Public Property PunchZone As String

        <DataMember>
        Public Property PunchLocation As String

        <DataMember>
        Public Property PunchLocationZone As String

        <DataMember>
        Public Property PunchImage As Byte()

        <DataMember>
        Public Property HasPhoto As Boolean

        <DataMember>
        Public Property PhotoOnAzure As Boolean

        <DataMember>
        Public Property TaskName As String

        <DataMember>
        Public Property PunchPresenceType As PunchSummaryType
    End Class

    <DataContract>
    Public Class roCausesSummary
        <DataMember>
        Public Property IDCause As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Total As Double

        <DataMember>
        Public Property WorkingType As String

        <DataMember>
        Public Property TotalFormat As String

        <DataMember>
        Public Property Limit As Double

        <DataMember>
        Public Property Type As SummaryType
    End Class

    <DataContract>
    Public Class roBussinessCentersSummary

        <DataMember>
        Public Property CauseName As String

        <DataMember>
        Public Property CauseType As Boolean

        <DataMember>
        Public Property CauseCostFactor As Double

        <DataMember>
        Public Property IdCenter As Integer

        <DataMember>
        Public Property CenterName As String

        <DataMember>
        Public Property DefaultCenter As Boolean

        <DataMember>
        Public Property Total As Double

        <DataMember>
        Public Property TotalFormat As String

        <DataMember>
        Public Property CostHour As Double

        <DataMember>
        Public Property PriceHour As Double

        <DataMember>
        Public Property EmployeeCost As Double
    End Class

    <DataContract>
    Public Class roEmployeeAccrualsProductivSummary

        Public Sub New()
            DailyAccruals = {}
            WeekAccruals = {}
            MonthAccruals = {}
            YearAccruals = {}
            ContractAccruals = {}
            YearWorkAccruals = {}
        End Sub

        <DataMember>
        Public Property DailyAccruals As roTasksSummary()

        <DataMember>
        Public Property WeekAccruals As roTasksSummary()

        <DataMember>
        Public Property MonthAccruals As roTasksSummary()

        <DataMember>
        Public Property YearAccruals As roTasksSummary()

        <DataMember>
        Public Property ContractAccruals As roTasksSummary()
        <DataMember>
        Public Property YearWorkAccruals As roTasksSummary()

    End Class

    <DataContract>
    Public Class roEmployeeHolidaysSummary

        Public Sub New()
            HolidaysInfo = {}
        End Sub

        <DataMember>
        Public Property HolidaysInfo As roHolidaysSummary()

    End Class

    <DataContract>
    Public Class roHolidaysSummary

        <DataMember>
        Public Property IDShift As Integer

        <DataMember>
        Public Property IDCause As Integer

        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property AccrualDefaultQuery As String

        <DataMember>
        Public Property Done As Double

        <DataMember>
        Public Property Pending As Double

        <DataMember>
        Public Property Lasting As Double

        <DataMember>
        Public Property Available As Double

        <DataMember>
        Public Property Expired As Double

        <DataMember>
        Public Property WithoutEnjoynment As Double

        <DataMember>
        Public Property Prevision As Double

        <DataMember>
        Public Property ValueFormat As String

    End Class


    <DataContract>
    Public Class roHolidaysSummaryByPeriod

        <DataMember>
        Public Property BeginPeriod As Date

        <DataMember>
        Public Property EndPeriod As Date

        <DataMember>
        Public Property IDConcept As Integer

        <DataMember>
        Public Property StartupValue As Double

        <DataMember>
        Public Property ExpiredDate As Date?

        <DataMember>
        Public Property StartEnjoymentDate As Date?

        <DataMember>
        Public Property ActualValue As Double

        <DataMember>
        Public Property ExpiredDays As Double

        <DataMember>
        Public Property DaysWithoutEnjoyment As Double

        <DataMember>
        Public Property ExpectedValue As Double
    End Class

    <DataContract>
    Public Class roHolidaysDay

        <DataMember>
        Public Property HolidayDate As Date

        <DataMember>
        Public Property Factor As Double

        <DataMember>
        Public Property IsExpiredDate As Integer

        <DataMember>
        Public Property BeginPeriod As Date?

        <DataMember>
        Public Property EndPeriod As Date?


    End Class


    <DataContract>
    Public Class roEmployeeAccrualsSummary

        Public Sub New()
            DailyAccruals = {}
            WeekAccruals = {}
            MonthAccruals = {}
            YearAccruals = {}
            ContractAccruals = {}
            YearWorkAccruals = {}
        End Sub

        <DataMember>
        Public Property DailyAccruals As roAccrualsSummary()

        <DataMember>
        Public Property WeekAccruals As roAccrualsSummary()

        <DataMember>
        Public Property MonthAccruals As roAccrualsSummary()

        <DataMember>
        Public Property YearAccruals As roAccrualsSummary()

        <DataMember>
        Public Property ContractAccruals As roAccrualsSummary()
        <DataMember>
        Public Property YearWorkAccruals As roAccrualsSummary()

    End Class

    <DataContract>
    Public Class roHolidayConceptsSummary

        <DataMember>
        Public Property ID As Integer

        <DataMember>
        Public Property TransactionDate As Date

        <DataMember>
        Public Property Detail As String

        <DataMember>
        Public Property Days As Double

        <DataMember>
        Public Property Total As Double

        <DataMember>
        Public Property TransactionDateOrder As Integer

    End Class

    <DataContract>
    Public Class roHolidayConceptsDetail

        <DataMember>
        Public Property DayType As String

        <DataMember>
        Public Property NumberOfDays As Double

    End Class

    <DataContract>
    Public Class roAccrualsSummary

        <DataMember>
        Public Property IDConcept As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Total As Double

        <DataMember>
        Public Property Type As String

        <DataMember>
        Public Property TotalFormat As String

        <DataMember>
        Public Property MaxValue As Double

        <DataMember>
        Public Property YearWorkPeriod As String
    End Class

    <DataContract>
    Public Class roCurrentStatusEmployeesSummary

        Public Sub New()
            EmployeesWithoutAssignment = {}
            EmployeesNoWorkingShift = {}
            EmployeesOnHolidays = {}
            EmployeesOnProgrammedAbsence = {}
        End Sub

        ' Empleados con horario de trabajo pero sin puesto asignado
        <DataMember>
        Public Property EmployeesWithoutAssignment As roCurrentStatusEmployeesSummary_EmployeeDetail()

        ' Empleados con horario de no trabajo (en descanso)
        <DataMember>
        Public Property EmployeesNoWorkingShift As roCurrentStatusEmployeesSummary_EmployeeDetail()

        ' Empleados de vacaciones
        <DataMember>
        Public Property EmployeesOnHolidays As roCurrentStatusEmployeesSummary_EmployeeDetail()

        ' Empleados en ausencia prolongada
        <DataMember>
        Public Property EmployeesOnProgrammedAbsence As roCurrentStatusEmployeesSummary_EmployeeDetail()
    End Class

    <DataContract>
    Public Class roCurrentStatusEmployeesSummary_EmployeeDetail

        <DataMember>
        Public Property IDEmployee As Integer

        <DataMember>
        Public Property EmployeeName As String

        <DataMember>
        Public Property IDGroup As Integer

        <DataMember>
        Public Property GroupName As String

        <DataMember>
        Public Property FullGroupName As String

    End Class

    <DataContract()>
    Public Class UserInfo
        <DataMember()>
        Public UserId As Integer
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public Description As String
        <DataMember()>
        Public ClientLocation As String
        <DataMember()>
        Public ApplicationName As String
    End Class

    <DataContract()>
    Public Class UserList

        Public Sub New()
            Status = New roWsState
            Users = {}
        End Sub

        <DataMember()>
        Public Users As UserInfo()
        <DataMember()>
        Public Status As roWsState
    End Class

    <DataContract()>
    Public Class ConcurrencyInfo
        <DataMember()>
        Public Datetime As Date
        <DataMember()>
        Public Count As Int32
        <DataMember()>
        Public RealCount As Double
        <DataMember()>
        Public VTPortal As Double
        <DataMember()>
        Public VTLive As Double
        <DataMember()>
        Public VTSupervisor As Double
        <DataMember()>
        Public Max As Int32
    End Class

    <DataContract()>
    Public Class ConcurrencyInfoList

        Public Sub New()
            Status = New roWsState
            ConcurrencyInfoValues = {}
        End Sub

        <DataMember()>
        Public ConcurrencyInfoValues As ConcurrencyInfo()
        <DataMember()>
        Public Status As roWsState
    End Class

    <DataContract()>
    Public Class EmployeesDashboardInfo
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public EmployeeName As String
        <DataMember()>
        Public ShiftName As String
        <DataMember()>
        Public Image As String
        <DataMember()>
        Public PresenceStatus As String
        <DataMember()>
        Public PresenceStatusImage As String
        <DataMember()>
        Public LastPunch As String
        <DataMember()>
        Public LastPunchFormattedDateTime As String
        <DataMember()>
        Public Details As String
        <DataMember()>
        Public LastCause As String
        <DataMember()>
        Public TaskName As String
        <DataMember()>
        Public CostCenterName As String
        <DataMember()>
        Public LocationName As String
        <DataMember()>
        Public ZoneName As String
        <DataMember()>
        Public InHolidays As String
        <DataMember()>
        Public InAbsence As String
        <DataMember()>
        Public InAbsenceCause As String
        <DataMember()>
        Public InHoursAbsence As String
        <DataMember()>
        Public InHoursAbsenceCause As String
        <DataMember()>
        Public DaysAbsenceRequested As String
        <DataMember()>
        Public DaysAbsenceRequestedCause As String
        <DataMember()>
        Public HoursAbsenceRequested As String
        <DataMember()>
        Public HoursAbsenceRequestedCause As String
        <DataMember()>
        Public InAbsenceImage As String
        <DataMember()>
        Public InHoursHolidays As String
        <DataMember()>
        Public InTelecommute As Boolean
        <DataMember()>
        Public InRealTimeTC As String
        <DataMember()>
        Public InTelecommuteImage As String
        <DataMember()>
        Public RealLastPunch As DateTime
        <DataMember()>
        Public InAnyHoliday As String
        <DataMember()>
        Public InAnyAbsence As String
        <DataMember()>
        Public GroupPath As String
    End Class

End Namespace