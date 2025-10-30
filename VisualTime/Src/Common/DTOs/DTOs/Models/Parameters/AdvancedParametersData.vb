Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class AdvancedParametersData

        Public Sub New()
            AdvancedParameters = New Hashtable
            ValidUntil = DateTime.Now.AddDays(1)
        End Sub

        <DataMember>
        Public Property AdvancedParameters As Hashtable
        <DataMember>
        Public Property ValidUntil As DateTime

    End Class

End Namespace