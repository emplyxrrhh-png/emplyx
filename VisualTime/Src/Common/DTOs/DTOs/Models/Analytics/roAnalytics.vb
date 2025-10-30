Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum AnalyticsTypeEnum
        <EnumMember> _NOTDEFINDED
        <EnumMember> _SCHEDULER
        <EnumMember> _ACCESS
        <EnumMember> _PRODUCTIV
        <EnumMember> _COSTCENTERS
        <EnumMember> _EQUALITYPLAN
        <EnumMember> _ALL
    End Enum

    <DataContract>
    Public Enum GeniusTypeEnum
        <EnumMember> _NOTDEFINDED
        <EnumMember> _SCHEDULER
        <EnumMember> _ACCRUALS
        <EnumMember> _ACCESS
        <EnumMember> _PRODUCTIV
        <EnumMember> _COSTCENTERS
        <EnumMember> _EQUALITYPLAN
        <EnumMember> _USERS
        <EnumMember> _PUNCHES
        <EnumMember> _REQUESTS
        <EnumMember> _ALL
    End Enum

    <DataContract>
    Public Class roAnalyticView
        ''' <summary>
        ''' Identificador único de horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Id As Integer
        ''' <summary>
        ''' Nombre del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdView As Integer
        ''' <summary>
        ''' Nombre abreviado del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property NameView As String
        ''' <summary>
        ''' Descripción del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Description As String
        ''' <summary>
        ''' Color del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateView As DateTime
        ''' <summary>
        ''' Horas teoricas del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdPassport As Integer
        ''' <summary>
        ''' Limite inicial del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Employees As String
        ''' <summary>
        ''' Limite final del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Concepts As String
        ''' <summary>
        ''' Centro de coste asignado al horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateInf As DateTime
        ''' <summary>
        ''' Aplicar el centro asignado tambien a las ausencias
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateSup As DateTime
        ''' <summary>
        ''' Permitir indicar horas ordinarias y complementarias al planificar el horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CubeLayout As String
        ''' <summary>
        ''' Tipo de Horario: De trabajo, Baja, Vacaciones o Descanso
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property TypeView As String
        ''' <summary>
        ''' Codigo de exportacion
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property FilterData As String
        <DataMember>
        Public Property UserFields As String
        ''' <summary>
        ''' Permite indicar valores al asignar el horario (hora de inicio de cada franja o complementarias )
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property GraphOptions As String
        <DataMember()>
        Public Property BusinessCenters As String
    End Class

End Namespace