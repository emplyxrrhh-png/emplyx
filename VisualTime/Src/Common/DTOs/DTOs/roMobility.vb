Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roMobilityValidation

        <DataMember>
        Public Property Mobilities As DataSet

        <DataMember>
        Public Property InvalidRow As Integer

        <DataMember>
        Public Property Valid As Boolean

    End Class

End Namespace