Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Enum EmployeeAIStatus
        <EnumMember()> NotAvailable
        <EnumMember()> AlreadyPlanned
    End Enum

    <DataContract()>
    Public Enum EmployeeAIDayStatus
        <EnumMember()> Present
        <EnumMember()> Absent
        <EnumMember()> Available
    End Enum

    <DataContract>
    Public Class roAIPlannerProblem
        <DataMember()>
        Public ID As String
        <DataMember()>
        Public StartDate As Date
        <DataMember()>
        Public EndDate As Date
        <DataMember()>
        Public Assignments As New List(Of roAIAssignment)
        <DataMember()>
        Public Shifts As New List(Of roAIShift)
        <DataMember()>
        Public Employees As New List(Of roAIEmployee)
        <DataMember()>
        Public CoverRequirements As New List(Of roAIDayCover)
    End Class

    <DataContract>
    Public Class roAIShift
        <DataMember()>
        Public VT_Id As Integer
        <DataMember()>
        Public UID As String
        <DataMember()>
        Public ShiftData As roCalendarRowShiftData

        Public Sub New(iVTID As Integer, sUID As String, oShiftData As roCalendarRowShiftData)
            VT_Id = iVTID
            UID = sUID
            ShiftData = oShiftData
        End Sub

    End Class

    <DataContract>
    Public Class roAIAssignment
        <DataMember()>
        Public Id As Integer
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public AssignmentData As roCalendarAssignmentCellData

        Public Sub New(iID As Integer, sName As String)
            Id = iID
            Name = sName
        End Sub

        Public Sub New(iID As Integer, sName As String, oAssigData As roCalendarAssignmentCellData)
            Id = iID
            Name = sName
            AssignmentData = oAssigData
        End Sub

    End Class

    <DataContract>
    Public Class roAIVirtualAssignment
        <DataMember()>
        Public Assignment As String
        <DataMember()>
        Public IdAssignment As Integer
        <DataMember()>
        Public IDNode As Integer
        <DataMember()>
        Public IDProductiveUnit As Integer
        <DataMember()>
        Public IDDailyBudgetPosition As Integer
        <DataMember()>
        Public ShiftShortName As String

        Public Sub New(sAssignmentName As String, iIdDailyBudgetPosition As Integer, sShortName As String, iIdNode As Integer, iIDProductiveUnit As Integer, iIDAssignment As Integer)
            Assignment = sAssignmentName
            IDNode = iIdNode
            IDDailyBudgetPosition = iIdDailyBudgetPosition
            ShiftShortName = sShortName
            IDProductiveUnit = iIDProductiveUnit
            IdAssignment = iIDAssignment
        End Sub

    End Class

    <DataContract>
    Public Class roAIEmployee
        <DataMember()>
        Public ID As Integer
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public AllowedAssignment As New List(Of String)
        <DataMember()>
        Public AllowedShifts As New List(Of String)
        <DataMember()>
        Public Costs As New List(Of roAIEmployeeCost)
        <DataMember()>
        Public CalendarStartDate As Date
        <DataMember()>
        Public CalendarEndDate As Date

        Public Overrides Function ToString() As String
            Return ID.ToString & "_" & Name.Replace(" ", "_")
        End Function

    End Class

    <DataContract>
    Public Class roAIEmployeeCost
        <DataMember()>
        Public [Date] As Date
        <DataMember()>
        Public Cost As Double

        Public Sub New(dDate As Date, iCost As Double)
            [Date] = dDate
            Cost = iCost
        End Sub

    End Class

    <DataContract>
    Public Class roAIDayCover
        <DataMember()>
        Property Day As Date
        <DataMember()>
        Property Cover As roAICover
    End Class

    <DataContract>
    Public Class roAICover
        <DataMember()>
        Public VirtualAssignment As roAIVirtualAssignment
        <DataMember()>
        Public Quantity As Integer
        <DataMember()>
        Public Pending As Integer

        Public Sub New(oVirtualAssignment As roAIVirtualAssignment, iQuantity As Integer)
            VirtualAssignment = oVirtualAssignment
            Quantity = iQuantity
            Pending = iQuantity
        End Sub

    End Class

    <DataContract>
    Public Class roAIEmployeeDayStatus
        <DataMember()>
        Public Status As EmployeeAIDayStatus
        <DataMember()>
        Public Assignment As String
        <DataMember()>
        Public Shift As String
    End Class

End Namespace