Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

#Region "roRGeniusScheduler"

    <DataContract()>
    Public Class roGeniusScheduler

#Region "Declarations - Constructor"

        Public Enum TypePeriodEnum
            PeriodOther = 0
            PeriodTomorrow = 1
            PeriodToday = 2
            PeriodYesterday = 3
            PeriodCurrentWeek = 4
            PeriodLastWeek = 5
            PeriodCurrentMonth = 6
            PeriodLastMonth = 7
            PeriodCurrentYear = 8
            PeriodNextWeek = 9
            PeriodNextMonth = 10
        End Enum

        Public Enum StateReportEnum
            NeverUsed = 0
            Launched = 1
            Executing = 2
            EndOK = 3
            EndWithErrors = 4
        End Enum

        'Private oState As roReportsState

        Private intID As Integer
        Private strName As String
        Private oScheduler As roReportSchedulerSchedule
        Private oPeriodType As TypePeriodEnum
        Private dLastDateTimeExecuted As Nullable(Of Date)
        Private dLastDateTimeUpdated As Nullable(Of Date)
        Private dScheduleBeginDate As Nullable(Of Date)
        Private dScheduleEndDate As Nullable(Of Date)
        Private dNextDateTimeExecuted As Nullable(Of Date)
        Private bolExecuteTask As Nullable(Of Boolean)
        Private intIDPassport As Nullable(Of Integer)
        Private intIDGeniusView As Nullable(Of Integer)
        Private intStateReport As StateReportEnum
        Private dLastExecution As Nullable(Of Date)

#End Region

#Region "Properties"

        ''' <summary>
        ''' ID de la programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre descriptivo de la programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        ''' <summary>
        ''' Programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Scheduler() As roReportSchedulerSchedule
            Get
                Return oScheduler
            End Get
            Set(ByVal value As roReportSchedulerSchedule)
                oScheduler = value
            End Set
        End Property

        <DataMember()>
        Public Property SchedulerText As String

        ''' <summary>
        ''' Tipo de periodo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property PeriodType() As TypePeriodEnum
            Get
                Return oPeriodType
            End Get
            Set(ByVal value As TypePeriodEnum)
                oPeriodType = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha de inicio de periodo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ScheduleBeginDate() As Nullable(Of Date)
            Get
                Return dScheduleBeginDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                dScheduleBeginDate = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha de fin de periodo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ScheduleEndDate() As Nullable(Of Date)
            Get
                Return dScheduleEndDate
            End Get
            Set(ByVal value As Nullable(Of Date))
                dScheduleEndDate = value
            End Set
        End Property

        ''' <summary>
        ''' Última vez que se ejecuto
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property LastDateTimeExecuted() As Nullable(Of Date)
            Get
                Return dLastDateTimeExecuted
            End Get
            Set(ByVal value As Nullable(Of Date))
                dLastDateTimeExecuted = value
            End Set
        End Property

        ''' <summary>
        ''' Última vez que se actualizo el informe
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property LastDateTimeUpdated() As Nullable(Of Date)
            Get
                Return dLastDateTimeUpdated
            End Get
            Set(ByVal value As Nullable(Of Date))
                dLastDateTimeUpdated = value
            End Set
        End Property

        ''' <summary>
        ''' Siguiente ejecución
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property NextDateTimeExecuted() As Nullable(Of Date)
            Get
                Return dNextDateTimeExecuted
            End Get
            Set(ByVal value As Nullable(Of Date))
                dNextDateTimeExecuted = value
            End Set
        End Property

        ''' <summary>
        ''' Se esta ejecutando...
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ExecuteTask() As Nullable(Of Boolean)
            Get
                Return bolExecuteTask
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                bolExecuteTask = value
            End Set
        End Property

        ''' <summary>
        ''' ID del Passport (VTLive)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDPassport() As Nullable(Of Integer)
            Get
                Return intIDPassport
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDPassport = value
            End Set
        End Property

        ''' <summary>
        ''' ID Estudio Genius
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDGeniusView() As Nullable(Of Integer)
            Get
                Return intIDGeniusView
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDGeniusView = value
            End Set
        End Property

        ''' <summary>
        ''' Estado del informe: 0=en espera; 1=Lanzado; 2=Preparando Datos; 3=Finalizado OK; 4=Finalizado con Error
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StateReport() As StateReportEnum
            Get
                Return intStateReport
            End Get
            Set(ByVal value As StateReportEnum)
                intStateReport = value
            End Set
        End Property

        ''' <summary>
        ''' Última ejecución (Planificada o Manual)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property LastExecution() As Nullable(Of Date)
            Get
                Return dLastExecution
            End Get
            Set(ByVal value As Nullable(Of Date))
                dLastExecution = value
            End Set
        End Property

#End Region

    End Class

#End Region

End Namespace