Imports System.Runtime.Serialization

Namespace DTOs

    <DataContract()>
    Public Class EmployeeInfo

        Public Sub New()
            BeginDate = Date.Now.Date
        End Sub

        <DataMember()>
        Public EmployeeId As Integer
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public Group As String
        <DataMember()>
        Public Image As String
        <DataMember()>
        Public PresenceStatus As String
        <DataMember()>
        Public TaskTitle As String
        <DataMember()>
        Public InTelecommute As String
        <DataMember()>
        Public CostCenterName As String
        <DataMember()>
        Public BeginDate As Date
        <DataMember()>
        Public LastPunchTime As String
    End Class

    <DataContract()>
    Public Class EmployeeList

        Public Sub New()
            Status = 0
            DefaultImage = String.Empty
            Description = String.Empty
            Employees = {}
        End Sub

        <DataMember()>
        Public DefaultImage As String
        <DataMember()>
        Public Employees As EmployeeInfo()
        <DataMember()>
        Public Status As Long
        <DataMember()>
        Public Description As String
    End Class

    <DataContract()>
    Public Class Days
        <DataMember()>
        Public Employees As EmployeesPlanned()
        <DataMember()>
        Public DatePlanned As Date
    End Class

    <DataContract()>
    Public Class EmployeesPlanned
        <DataMember()>
        Public IdCounter As Integer
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public EmployeeName As String
        <DataMember()>
        Public EmployeeImage As String
        <DataMember()>
        Public IdShift As Integer
        <DataMember()>
        Public ShiftName As String
        <DataMember()>
        Public Pos As Integer

    End Class

    <DataContract()>
    Public Class EmployeePlanning
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public DatePlanned As Date
        <DataMember()>
        Public IdShift As Integer
        <DataMember()>
        Public ShiftName As String
        <DataMember()>
        Public ShiftColor As String
    End Class

    <DataContract()>
    Public Class Overlays
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public EmployeeName As String
        <DataMember()>
        Public EmployeeGroup As String
        <DataMember()>
        Public EmployeeImage As String
        <DataMember()>
        Public IdAbsence As Integer
        <DataMember()>
        Public IdCause As Integer
        <DataMember()>
        Public IsRequest As Boolean
        <DataMember()>
        Public AbsenceType As String
        <DataMember()>
        Public AbsenceDetails As String
        <DataMember()>
        Public AbsenceResume As String
        <DataMember()>
        Public AbsenceBeginDate As Date
        <DataMember()>
        Public AbsenceEndDate As Nullable(Of DateTime)
        <DataMember()>
        Public AbsenceBeginTime As Nullable(Of DateTime)
        <DataMember()>
        Public AbsenceEndTime As Nullable(Of DateTime)
        <DataMember()>
        Public Duration As Double
        <DataMember()>
        Public IdCounter As Integer
    End Class

    <DataContract()>
    Public Class OverlappingEmployees

        Public Sub New()
            Status = 0
            DefaultImage = String.Empty
            Overlays = {}
        End Sub

        <DataMember()>
        Public DefaultImage As String
        <DataMember()>
        Public Overlays As Overlays()
        <DataMember()>
        Public Status As Long
    End Class

    <DataContract()>
    Public Class MyTeamPlanInfo

        Public Sub New()
            Status = 0
            DefaultImage = String.Empty
            Days = {}
            EmployeePlanning = {}
        End Sub

        <DataMember()>
        Public DefaultImage As String
        <DataMember()>
        Public Days As Days()
        <DataMember()>
        Public EmployeePlanning As EmployeePlanning()
        <DataMember()>
        Public Status As Long
    End Class

End Namespace