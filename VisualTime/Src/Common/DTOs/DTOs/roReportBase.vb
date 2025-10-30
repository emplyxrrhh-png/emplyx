Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum eRSDestinationType
        ''' <summary>
        ''' Envio a impresora del servidor
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Printer
        ''' <summary>
        ''' Envio a dirección de correo
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Email
        ''' <summary>
        ''' Ubicación fisica en el servidor
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Location
        ''' <summary>
        ''' Supervisores. Excluyente con el resto de tipos de destinos
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Supervisors
        ''' <summary>
        ''' DocumentManager
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> DocumentManager
    End Enum

    <DataContract>
    Public Enum eReportScheduleType
        ''' <summary>
        ''' Tipo supervisor
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> General = 0
        ''' <summary>
        ''' Informe de empleado
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Employee = 1
        ''' <summary>
        ''' Analitica
        ''' </summary>
        ''' <remarks></remarks>
        <EnumMember> Analytics = 2
    End Enum

    <DataContract>
    Public Enum eRSScheduleType
        <EnumMember> Daily = 0
        <EnumMember> Weekly = 1
        <EnumMember> Monthly = 2
        <EnumMember> OneTime = 3
        <EnumMember> Hours = 4
    End Enum

    <DataContract>
    Public Enum eRSWeekDay
        <EnumMember> Monday = 1
        <EnumMember> Tuesday = 2
        <EnumMember> Wednesday = 3
        <EnumMember> Thursday = 4
        <EnumMember> Friday = 5
        <EnumMember> Saturday = 6
        <EnumMember> Sunday = 7
    End Enum

    <DataContract>
    Public Enum eRSMonthlyType
        <EnumMember> DayOfMonth
        <EnumMember> StartAndDayMonth
    End Enum

    <DataContract()>
    Public Class roReportSchedulerSchedule

        Private intMonthlyType As eRSMonthlyType = eRSMonthlyType.DayOfMonth 'Tipo de mes
        Private intScheduleType As eRSScheduleType = eRSScheduleType.Daily  'diario, semanal, mensual, etc.
        Private intDays As Integer = 1 'Dias (cada x dias)
        Private intWeeks As Integer = 1 'Semanas (cada x semanas)
        Private dDateSchedule As Date ' El día XX/XX/XXXX
        Private strHour As String = "00:00" 'Hora (hh:mm)
        Private strWeekDays As String = "0000000" 'Dias de la semana (1=Checked, 0=Unchecked, 1100000=Lunes y martes)
        Private intDay As Integer = 1 'El dia X de cada mes
        Private intStart As Integer = 1 '1er, 2on, etc. (del 1 al 5)
        Private intWeekDay As eRSWeekDay = eRSWeekDay.Monday  'Dia xxx de la semana

        Public Sub New()
        End Sub

#Region "Properties"

        <DataMember()>
        Public Property ScheduleType() As eRSScheduleType
            Get
                Return intScheduleType
            End Get
            Set(ByVal value As eRSScheduleType)
                intScheduleType = value
            End Set
        End Property
        <DataMember()>
        Public Property Days() As Integer
            Get
                Return intDays
            End Get
            Set(ByVal value As Integer)
                intDays = value
            End Set
        End Property
        <DataMember()>
        Public Property Weeks() As Integer
            Get
                Return intWeeks
            End Get
            Set(ByVal value As Integer)
                intWeeks = value
            End Set
        End Property
        <DataMember()>
        Public Property DateSchedule() As Date
            Get
                Return dDateSchedule
            End Get
            Set(ByVal value As Date)
                dDateSchedule = value
            End Set
        End Property
        <DataMember()>
        Public Property Hour() As String
            Get
                Return strHour
            End Get
            Set(ByVal value As String)
                strHour = value
            End Set
        End Property
        <DataMember()>
        Public Property WeekDays() As String
            Get
                Return strWeekDays
            End Get
            Set(ByVal value As String)
                strWeekDays = value
            End Set
        End Property
        <DataMember()>
        Public Property Day() As Integer
            Get
                Return intDay
            End Get
            Set(ByVal value As Integer)
                intDay = value
            End Set
        End Property
        <DataMember()>
        Public Property Start() As Integer
            Get
                Return intStart
            End Get
            Set(ByVal value As Integer)
                intStart = value
            End Set
        End Property
        <DataMember()>
        Public Property WeekDay() As eRSWeekDay
            Get
                Return intWeekDay
            End Get
            Set(ByVal value As eRSWeekDay)
                intWeekDay = value
            End Set
        End Property
        <DataMember()>
        Public Property MonthlyType() As eRSMonthlyType
            Get
                Return intMonthlyType
            End Get
            Set(ByVal value As eRSMonthlyType)
                intMonthlyType = value
            End Set
        End Property

#End Region

    End Class

End Namespace