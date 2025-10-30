Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Objeto representante de la petición de vacaciones de un empleado
    ''' </summary>
    <DataContract()>
    Public Class HolidaysInfo

        Public Sub New()
            HolidaysGroup = {}
            Status = 0
        End Sub

        ''' <summary>
        ''' Array de los distintos grupos donde un empleado puede solicitar vacaciones(Año actual / año anterior / libre disposición, etc...)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property HolidaysGroup As VacationResumeShifts()

        ''' <summary>
        ''' Estado de la petición de vacaciones
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Objecto representante de la información de un grupo de vacaciones
    ''' </summary>
    <DataContract()>
    Public Class VacationResumeShifts
        ''' <summary>
        ''' Nombre del grupo
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property key As String

        ''' <summary>
        ''' Conjunto de información disponible sobre un grupo de vacaciones(disponibles, aprobadas, restantes, ya disfrutadas, aprobadas pendientes de disfrutar)
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property items As HolidayGroupInfo()
    End Class

    ''' <summary>
    ''' Objeto representante de la información sobre un valor concreto de un grupo de vacaciones
    ''' </summary>
    Public Class HolidayGroupInfo
        ''' <summary>
        ''' Nombre del parametro del grupo de vacaciones
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String

        ''' <summary>
        ''' Valor para el parametro
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Value As String
    End Class

    ''' <summary>
    ''' Información sobre un horario
    ''' </summary>
    <DataContract()>
    Public Class ShiftFloatingInfo
        ''' <summary>
        ''' Identificador del horario
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IDShift As Integer

        ''' <summary>
        ''' El horario permite un inicio flotante
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsFloating As Boolean

        ''' <summary>
        ''' Hora del inicio flotante del horario si aplica
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property StartFloating As DateTime

        ''' <summary>
        ''' Estado de la petición de detalle de un horario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long
    End Class

End Namespace