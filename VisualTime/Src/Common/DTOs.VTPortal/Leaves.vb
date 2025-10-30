Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Detalle de una baja de un empleado
    ''' </summary>
    <DataContract()>
    Public Class Leave
        ''' <summary>
        ''' Fecha inicio de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property BeginDate As DateTime
        ''' <summary>
        ''' Identificador de ausencia
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AbsenceID As Integer
        ''' <summary>
        ''' Fecha fin de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property FinishDate As String

        ''' <summary>
        ''' Nombre completo de la justificación
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Cause As String

        ''' <summary>
        ''' Identificador de la justificación de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdCause As Integer

        ''' <summary>
        ''' Identificador del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IdEmployee As Integer
        ''' <summary>
        ''' Descripción extendida de la información solicitada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DocAlerts As DocumentAlert()
        ''' <summary>
        ''' Lista de alertas de documentación de la baja
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Description As String
    End Class

    ''' <summary>
    ''' Listado de bajas de un empleado
    ''' </summary>
    <DataContract()>
    Public Class LeavesList
        ''' <summary>
        ''' Estado de la petición de listado de solicitude
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember()>
        Public Property Status As Long

        ''' <summary>
        ''' Listado de solicitudes del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Leaves As Leave()
    End Class

End Namespace