Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum StateReportEnum
        <EnumMember> NeverUsed = 0
        <EnumMember> Launched = 1
        <EnumMember> Executing = 2
        <EnumMember> EndOK = 3
        <EnumMember> EndWithErrors = 4
    End Enum

    <DataContract>
    Public Class roAnalyticSchedule
        ''' <summary>
        ''' ID de la programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ID As Integer

        ''' <summary>
        ''' Nombre descriptivo de la programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Name As String

        ''' <summary>
        ''' Informe a ejecutar
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Report As String

        ''' <summary>
        ''' Perfil del usuario de informes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Profile As Integer

        ''' <summary>
        ''' Formato del informe
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ReportFormat As Integer

        ''' <summary>
        ''' Programación
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Scheduler As roReportSchedulerSchedule

        ''' <summary>
        ''' Última vez que se ejecuto
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property LastDateTimeExecuted As Nullable(Of Date)

        ''' <summary>
        ''' Última vez que se actualizo el informe
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property LastDateTimeUpdated As Nullable(Of Date)

        ''' <summary>
        ''' Siguiente ejecución
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property NextDateTimeExecuted As Nullable(Of Date)

        ''' <summary>
        ''' Se esta ejecutando...
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property ExecuteTask As Nullable(Of Boolean)

        ''' <summary>
        ''' Usuario creador del informe (VisualTime)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDUser As Nullable(Of Integer)

        ''' <summary>
        ''' ID del Passport (VTLive)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDPassport As Nullable(Of Integer)

        ''' <summary>
        ''' Estado del informe: 0=en espera; 1=Lanzado; 2=Preparando Datos; 3=Finalizado OK; 4=Finalizado con Error
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property StateReport() As StateReportEnum

        <DataMember()>
        Public Property LastExecution As Nullable(Of Date)

        <DataMember()>
        Public Property ProfileName As String

        <DataMember()>
        Public Property ReportName As String

        <DataMember()>
        Public Property ProfileParameters As String

        <DataMember()>
        Public Property ReportScheduleType As eReportScheduleType

        <DataMember()>
        Public Property XmlParameters As String
    End Class

    <DataContract()>
    Public Class roEmergencySchedule
        <DataMember()>
        Public Property ID As Integer
        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property ProfileName As String

        <DataMember()>
        Public Property ReportName As String

    End Class

End Namespace