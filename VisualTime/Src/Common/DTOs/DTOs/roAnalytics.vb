Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <Serializable>
    <DataContract>
    Public Class Analytics_Base
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_Access
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As Decimal

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property DateTime As Date

        <DataMember> Public Property [Date] As System.Nullable(Of Date)

        <DataMember> Public Property Time As String

        <DataMember> Public Property Day As System.Nullable(Of Integer)

        <DataMember> Public Property Month As System.Nullable(Of Integer)

        <DataMember> Public Property Year As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As String

        <DataMember> Public Property Value As Integer

        <DataMember> Public Property IDZone As System.Nullable(Of Byte)

        <DataMember> Public Property ZoneName As String

        <DataMember> Public Property IDTerminal As System.Nullable(Of Byte)

        <DataMember> Public Property TerminalName As String

        <DataMember> Public Property InvalidType As System.Nullable(Of Byte)
        <DataMember> Public Property InvalidDesc As String

        <DataMember> Public Property IsInvalid As Integer

        <DataMember> Public Property Details As String

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_Authorizations
        Inherits Analytics_Base
        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property GroupPath As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property AuthorizationName As String

        <DataMember> Public Property ZoneName As String

        <DataMember> Public Property IsWorkingZone As Boolean

        <DataMember> Public Property AccessPeriodName As String

        <DataMember> Public Property BelongsToGroup As Integer

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_RequestsCostCenters
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As Integer

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property RequestsStatus As Byte
        <DataMember> Public Property StatusDesc As String

        <DataMember> Public Property ApproveRefusePassport As String

        <DataMember> Public Property PendingPassport As String

        <DataMember> Public Property RequestType As Byte
        <DataMember> Public Property TypeDesc As String

        <DataMember> Public Property PunchDate As String

        <DataMember> Public Property NameCenter As String

        <DataMember> Public Property CommentsRequest As String

        <DataMember> Public Property Time As String

        <DataMember> Public Property Day As System.Nullable(Of Integer)

        <DataMember> Public Property Month As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property Value As Integer

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Partial Public Class Analytics_CostCentersDetail
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As String

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property CauseName As String

        <DataMember> Public Property CostFactor As System.Nullable(Of Decimal)

        <DataMember> Public Property IDCenter As Short

        <DataMember> Public Property CenterName As String

        <DataMember> Public Property Field1 As String

        <DataMember> Public Property Field2 As String

        <DataMember> Public Property Field3 As String

        <DataMember> Public Property Field4 As String

        <DataMember> Public Property Field5 As String

        <DataMember> Public Property Value As Decimal

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property WeekOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DefaultCenter As System.Nullable(Of Boolean)

        <DataMember> Public Property ManualCenter As System.Nullable(Of Boolean)

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property Path As String

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property Cost As String
        <DataMember> Public Property TotalCost As Decimal

        <DataMember> Public Property PVP As String

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_CostCenters
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property CauseName As String

        <DataMember> Public Property CostFactor As System.Nullable(Of Decimal)

        <DataMember> Public Property IDCenter As Short

        <DataMember> Public Property CenterName As String

        <DataMember> Public Property Field1 As String

        <DataMember> Public Property Field2 As String

        <DataMember> Public Property Field3 As String

        <DataMember> Public Property Field4 As String

        <DataMember> Public Property Field5 As String

        <DataMember> Public Property Value As Decimal

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property WeekOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DefaultCenter As System.Nullable(Of Boolean)

        <DataMember> Public Property ManualCenter As System.Nullable(Of Boolean)

        <DataMember> Public Property Cost As String

        <DataMember> Public Property PVP As String
        <DataMember> Public Property TotalCost As Decimal
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_RequestsTasks
        Inherits Analytics_Base

        <DataMember> Public Property KeyView As Integer

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property RequestsStatus As Byte
        <DataMember> Public Property StatusDesc As String

        <DataMember> Public Property ApproveRefusePassport As String

        <DataMember> Public Property PendingPassport As String

        <DataMember> Public Property RequestType As Byte
        <DataMember> Public Property TypeDesc As String

        <DataMember> Public Property DateTask1 As String

        <DataMember> Public Property DateTask2 As String

        <DataMember> Public Property NameTask1 As String

        <DataMember> Public Property NameTask2 As String

        <DataMember> Public Property CompletedTask As Boolean

        <DataMember> Public Property TaskField1 As String

        <DataMember> Public Property TaskField2 As String

        <DataMember> Public Property TaskField3 As String

        <DataMember> Public Property TaskField4 As Decimal

        <DataMember> Public Property TaskField5 As Decimal

        <DataMember> Public Property TaskField6 As Decimal

        <DataMember> Public Property CommentsRequest As String

        <DataMember> Public Property Time As String

        <DataMember> Public Property Day As System.Nullable(Of Integer)

        <DataMember> Public Property Month As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property Value As Integer

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Partial Public Class Analytics_Tasks
        Inherits Analytics_Base
        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property Date_Year As System.Nullable(Of Integer)

        <DataMember> Public Property Date_Month As System.Nullable(Of Integer)

        <DataMember> Public Property Date_Day As System.Nullable(Of Integer)

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property IDCenter As Short

        <DataMember> Public Property CenterName As String

        <DataMember> Public Property IDTask As Integer

        <DataMember> Public Property TaskName As String

        <DataMember> Public Property InitialTime As Decimal

        <DataMember> Public Property Field1_Task As String

        <DataMember> Public Property Field2_Task As String

        <DataMember> Public Property Field3_Task As String

        <DataMember> Public Property Field4_Task As Decimal

        <DataMember> Public Property Field5_Task As Decimal

        <DataMember> Public Property Field6_Task As Decimal

        <DataMember> Public Property Field1_Total As String

        <DataMember> Public Property Field2_Total As String

        <DataMember> Public Property Field3_Total As String

        <DataMember> Public Property Field4_Total As Decimal

        <DataMember> Public Property Field5_Total As Decimal

        <DataMember> Public Property Field6_Total As Decimal

        <DataMember> Public Property Value As Decimal

        <DataMember> Public Property Project As String

        <DataMember> Public Property Tag As String

        <DataMember> Public Property IDPassport As Integer
        <DataMember> Public Property PassportName As String

        <DataMember> Public Property TimeChangedRequirements As Decimal

        <DataMember> Public Property ForecastErrorTime As Decimal

        <DataMember> Public Property NonProductiveTimeIncidence As Decimal

        <DataMember> Public Property EmployeeTime As Decimal

        <DataMember> Public Property TeamTime As Decimal

        <DataMember> Public Property MaterialTime As Decimal

        <DataMember> Public Property OtherTime As Decimal

        <DataMember> Public Property Duration As System.Nullable(Of Decimal)

        <DataMember> Public Property Estado As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Partial Public Class Analytics_Schedule
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property ShiftName As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property Hours As System.Nullable(Of Decimal)

        <DataMember> Public Property HoursAbs As System.Nullable(Of Decimal)

        <DataMember> Public Property Count As System.Nullable(Of Integer)

        <DataMember> Public Property Mes As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property WeekOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property Path As String

        <DataMember> Public Property Quarter As System.Nullable(Of Integer)

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property IDAssignment As System.Nullable(Of Short)

        <DataMember> Public Property Remark As String

        <DataMember> Public Property AssignmentName As String

        <DataMember> Public Property IDProductiveUnit As System.Nullable(Of Integer)

        <DataMember> Public Property ProductiveUnitName As String

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String

        <DataMember> Public Property LockDate As Date

    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_RequestsSchedule
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As Integer

        <DataMember> Public Property NumeroDias As Integer
        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property RequestsStatus As Byte
        <DataMember> Public Property StatusDesc As String

        <DataMember> Public Property ApproveRefusePassport As String

        <DataMember> Public Property PendingPassport As String

        <DataMember> Public Property RequestType As Byte
        <DataMember> Public Property TypeDesc As String

        <DataMember> Public Property BeginDateRequest As String

        <DataMember> Public Property BeginHourRequest As String

        <DataMember> Public Property EndDateRequest As String

        <DataMember> Public Property EndHourRequest As String

        <DataMember> Public Property CauseNameRequest As String

        <DataMember> Public Property ShiftNameRequest As String

        <DataMember> Public Property CommentsRequest As String
        <DataMember> Public Property FieldNameRequest As String
        <DataMember> Public Property FieldValueRequest As String

        <DataMember> Public Property Time As String

        <DataMember> Public Property Day As System.Nullable(Of Integer)

        <DataMember> Public Property Month As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property Value As Integer

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Partial Public Class Analytics_Incidences
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As String

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property IncidenceName As String

        <DataMember> Public Property ZoneTime As String

        <DataMember> Public Property CauseName As String

        <DataMember> Public Property Value As System.Nullable(Of Decimal)

        <DataMember> Public Property Mes As System.Nullable(Of Integer)
        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property WeekOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property ShiftName As String

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property Path As String
        <DataMember> Public Property Quarter As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property IDAssignment As System.Nullable(Of Short)

        <DataMember> Public Property AssignmentName As String

        <DataMember> Public Property IDProductiveUnit As System.Nullable(Of Integer)

        <DataMember> Public Property ProductiveUnitName As String

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String

        <DataMember> Public Property Remark As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_Punches
        Inherits Analytics_Base

        <DataMember> Public Property KeyView As Decimal

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property PunchDirection As System.Nullable(Of Byte)
        <DataMember> Public Property PDDesc As String

        <DataMember> Public Property PunchIDTypeData As System.Nullable(Of Integer)

        <DataMember> Public Property PunchTypeData As String

        <DataMember> Public Property DateTime As Date

        <DataMember> Public Property [Date] As System.Nullable(Of Date)

        <DataMember> Public Property Time As String

        <DataMember> Public Property Day As System.Nullable(Of Integer)

        <DataMember> Public Property Month As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property Value As Integer

        <DataMember> Public Property IDZone As System.Nullable(Of Byte)

        <DataMember> Public Property ZoneName As String

        <DataMember> Public Property IDTerminal As System.Nullable(Of Byte)

        <DataMember> Public Property TerminalName As String

        <DataMember> Public Property InvalidType As System.Nullable(Of Byte)
        <DataMember> Public Property InvalidDesc As String

        <DataMember> Public Property IsInvalid As Integer

        <DataMember> Public Property Details As String

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IsNotReliable As System.Nullable(Of Boolean)

        <DataMember> Public Property PunchLocation As String

        <DataMember> Public Property PunchLocationZone As String

        <DataMember> Public Property FullAddress As String

        <DataMember> Public Property PunchTimeZone As String

        <DataMember> Public Property EditorName As String

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_Concepts
        Inherits Analytics_Base
        <DataMember> Public Property KeyView As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property IDConcept As Short

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property ConceptName As String

        <DataMember> Public Property [Date] As System.Nullable(Of Date)

        <DataMember> Public Property Value As System.Nullable(Of Decimal)

        <DataMember> Public Property Count As System.Nullable(Of Integer)

        <DataMember> Public Property Mes As System.Nullable(Of Integer)

        <DataMember> Public Property Año As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfWeek As System.Nullable(Of Integer)

        <DataMember> Public Property WeekOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property DayOfYear As System.Nullable(Of Integer)

        <DataMember> Public Property Path As String
        <DataMember> Public Property Quarter As System.Nullable(Of Integer)
        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

    <Serializable>
    <DataContract>
    Partial Public Class Analytics_Audit
        Inherits Analytics_Base

        <DataMember> Public Property ID As Decimal

        <DataMember> Public Property PassportName As String

        <DataMember> Public Property [Date] As Date

        <DataMember> Public Property ActionID As Integer

        <DataMember> Public Property ElementID As Integer

        <DataMember> Public Property ElementName As String

        <DataMember> Public Property MessageParameters As String

        <DataMember> Public Property SessionID As Integer

        <DataMember> Public Property ClientLocation As String
    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_CostCenters_ActualStatus
        Inherits Analytics_Base

        <DataMember> Public Property KeyView As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property IDGroup As Integer

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property IDCenter As Short

        <DataMember> Public Property CenterName As String

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property CostDateTime As System.Nullable(Of Date)

        <DataMember> Public Property Path As String

        <DataMember> Public Property CurrentEmployee As System.Nullable(Of Integer)

        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date

        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date

        <DataMember> Public Property BeginPeriod As System.Nullable(Of Date)

        <DataMember> Public Property EndPeriod As System.Nullable(Of Date)

        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String

    End Class

    <Serializable>
    <DataContract>
    Public Class Analytics_Biometric
        Inherits Analytics_Base

        <DataMember> Public Property KeyView As String

        <DataMember> Public Property GroupName As String

        <DataMember> Public Property FullGroupName As String

        <DataMember> Public Property IDEmployee As Integer

        <DataMember> Public Property EmployeeName As String

        <DataMember> Public Property Method As String
        <DataMember> Public Property MethodDesc As String
        <DataMember> Public Property Credential As String

        <DataMember> Public Property Version As String
        <DataMember> Public Property VersionDesc As String

        <DataMember> Public Property BiometricData As String
        <DataMember> Public Property BiometricID As Integer
        <DataMember> Public Property Terminal As String
        <DataMember> Public Property BiometricAlgorithm As String

        <DataMember> Public Property IDContract As String

        <DataMember> Public Property TimeStamp As System.Nullable(Of Date)

        <DataMember> Public Property Enabled As Boolean
        <DataMember> Public Property BeginContract As Date

        <DataMember> Public Property EndContract As Date
        <DataMember> Public Property BeginDate As Date

        <DataMember> Public Property EndDate As Date
        <DataMember> Public Property Nivel1 As String

        <DataMember> Public Property Nivel2 As String

        <DataMember> Public Property Nivel3 As String

        <DataMember> Public Property Nivel4 As String

        <DataMember> Public Property Nivel5 As String

        <DataMember> Public Property Nivel6 As String

        <DataMember> Public Property Nivel7 As String

        <DataMember> Public Property Nivel8 As String

        <DataMember> Public Property Nivel9 As String

        <DataMember> Public Property Nivel10 As String

        <DataMember> Public Property UserField1 As String

        <DataMember> Public Property UserField2 As String

        <DataMember> Public Property UserField3 As String

        <DataMember> Public Property UserField4 As String

        <DataMember> Public Property UserField5 As String

        <DataMember> Public Property UserField6 As String

        <DataMember> Public Property UserField7 As String

        <DataMember> Public Property UserField8 As String

        <DataMember> Public Property UserField9 As String

        <DataMember> Public Property UserField10 As String
    End Class

End Namespace