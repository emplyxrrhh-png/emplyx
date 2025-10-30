Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Estado de la solicitud
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract>
    Public Enum eDailyRecordDateStatus
        ''' <summary>
        ''' Aún no se ha hecho la declaración de jornada
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Pending = 0
        ''' <summary>
        ''' Declaración realizada, pero aún no aprobada
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> OnGoing = 1
        ''' <summary>
        ''' Declaración hecha y ok
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Done = 3
        ''' <summary>
        ''' No se requiere declaración
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> NotNeeded = 4
        ''' <summary>
        ''' No se puede realizar declaración
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> NotAllowed = 5
    End Enum

    <DataContract>
    Public Class roDailyRecord

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property IdEmployee As Integer

        <DataMember>
        Public Property RecordDate As Date

        <DataMember()>
        Public Property EmployeeName As String

        <DataMember()>
        Public Property EmployeeImage As String

        <DataMember()>
        Public Property EmployeeGroup As String

        <DataMember>
        Public Property Punches As roDailyRecordPunch()

        <DataMember>
        Public Property TimeExpected As Long

        <DataMember>
        Public Property TimeAccrued As Long

        <DataMember>
        Public Property Adjusted As Boolean

        <DataMember>
        Public Property DailyRecordStatus As eDailyRecordDateStatus

        <DataMember>
        Public Property Modified As Boolean

        <DataMember>
        Public Property DailyRecordInfo As String

        Public Sub New()
            Id = -1
            Modified = False
        End Sub

    End Class

    <DataContract>
    Public Class roDailyRecordCalendar
        <DataMember>
        Public Property DayData As roDailyRecordCalendarItem()
    End Class

    <DataContract>
    Public Class roDailyRecordCalendarItem
        <DataMember>
        Public Property IdRecord As Integer

        ''' <summary>
        ''' Deprecated. Form compatibility purpose only. Eliminate ASAP
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdReport As Integer

        <DataMember>
        Public Property IdEmployee As Integer

        <DataMember>
        Public Property [Date] As Date

        <DataMember>
        Public Property CanTelecommute As Boolean

        <DataMember>
        Public Property DateStatus As eDailyRecordDateStatus

        <DataMember>
        Public Property HasPunchesPattern As Boolean

        Public Sub New()
            IdRecord = -1
            IdReport = -1
        End Sub

    End Class

    <DataContract>
    Public Class roDailyRecordPunchesPattern
        <DataMember>
        Public Property Punches As roDailyRecordPunch()
    End Class

End Namespace