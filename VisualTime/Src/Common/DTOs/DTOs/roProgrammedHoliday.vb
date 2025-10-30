Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa una prevision de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roProgrammedHolidayStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roProgrammedHolidayListResponse
        <DataMember>
        Public Property ProgrammedHolidays As roProgrammedHoliday()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una prevision de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roProgrammedHolidayResponse
        <DataMember>
        Public Property ProgrammedHoliday As roProgrammedHoliday

        <DataMember>
        Public Property oState As roWsState
    End Class

    <Serializable>
    <DataContract>
    Public Class roProgrammedHoliday

        Public Sub New()
            Me.BeginTime = New DateTime(1899, 12, 30, 0, 0, 0)
            Me.EndTime = New DateTime(1899, 12, 30, 0, 0, 0)
        End Sub

        ''' <summary>
        ''' Identificador único de las vacaciones por horas
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ID As Long
        ''' <summary>
        ''' Nombre de la plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDEmployee As Integer
        ''' <summary>
        ''' Nombre abreviado de la plantilla
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ProgrammedDate As Date
        ''' <summary>
        ''' Descripción de la plantilla
        ''' </summary>
        ''' <returns></returns>

        <DataMember>
        Public Property IDCause As Integer
        ''' <summary>
        ''' Descripción de la plantilla
        ''' </summary>

        <DataMember>
        Public Property Description As String
        ''' <summary>
        ''' Tipo de documento: empleado, empresa
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property AllDay As Boolean
        ''' <summary>
        ''' A quién o qué aplica el documento: ficha de empleado, fichaje, ..., empresa, visita
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property BeginTime As DateTime
        ''' <summary>
        ''' Inicio de validez de la plantilla. Antes de esta fecha no puede ser usada en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property EndTime As DateTime
        ''' <summary>
        ''' Fin de vlaidez de la plantilla. Después de esta fecha no puede ser usada en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Duration As Double
    End Class

End Namespace