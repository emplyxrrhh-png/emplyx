Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract, KnownType(GetType(roWsState)),
        KnownType(GetType(roDocumentTemplate)),
        KnownType(GetType(roDocument))
        >
    Public Class roResponseTypes
        <DataMember>
        Public Property Type As Object
        <DataMember>
        Public Property TypeArray() As Object()
        <DataMember>
        Public Property oState As Object
    End Class

End Namespace