Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    'TO BE COMPLETED

    <DataContract()>
    Public Class roShiftPunchesPatternItem
        ''' <summary>
        ''' Tipo de fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Type As PunchTypeEnum

        ''' <summary>
        ''' Fecha y hora del fichaje
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property DateTime As DateTime
    End Class

    <DataContract()>
    Public Class roShiftPunchesPattern
        <DataMember>
        Public Property Punches As roShiftPunchesPatternItem()
    End Class

End Namespace