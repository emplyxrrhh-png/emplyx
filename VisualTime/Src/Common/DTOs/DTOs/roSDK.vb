Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa es estado de una petición al SDK de fichajes
    ''' </summary>
    <DataContract>
    Public Class roSDKGenericResponse
        ''' <summary>
        ''' Indica si la petición ha tenido éxito o no.
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Result As Boolean

        ''' <summary>
        ''' Estado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de fichajes obtenido mediante el SDK de fichajes
    ''' </summary>
    <DataContract>
    Public Class roSDKPunchList
        ''' <summary>
        ''' Lista de fichajes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Result As roSDKPunch()

        ''' <summary>
        ''' Estado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un fichaje de VisualTime
    ''' </summary>
    <DataContract>
    Public Class roSDKPunch
        ''' <summary>
        ''' Identificador del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdPunch As Integer

        ''' <summary>
        ''' Identificador del empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IdEmployee As Integer

        ''' <summary>
        ''' Tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Type As String

        ''' <summary>
        ''' Fecha a la que pertenece el fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ShiftDate As DateTime

        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime

        ''' <summary>
        ''' Identificador del terminal
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Terminal As Integer

        ''' <summary>
        ''' Nombre corto de la justificación del fichaje(solo si la tiene)
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Cause As String

    End Class

End Namespace