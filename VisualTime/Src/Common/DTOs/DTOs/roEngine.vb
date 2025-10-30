Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    ''' <summary>
    ''' Representa una prevision de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roEngineStandarResponse
        <DataMember>
        Public Property Result As Boolean

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa un listado de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roEngineListResponse
        <DataMember>
        Public Property ProcessEngine As roEngine()

        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Representa una prevision de vacaciones por horas
    ''' </summary>
    <DataContract>
    Public Class roEngineResponse
        <DataMember>
        Public Property ProcessEngine As roEngine

        <DataMember>
        Public Property oState As roWsState
    End Class

    <Serializable>
    <DataContract>
    Public Class roEngine

        Public Sub New()
        End Sub

    End Class

End Namespace