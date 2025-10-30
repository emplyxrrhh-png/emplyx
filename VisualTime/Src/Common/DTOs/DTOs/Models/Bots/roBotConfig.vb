Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs
    <DataContract>
    <Serializable>
    Public Class roBotConfig

        <DataMember>
        Public Property SourceIds As Integer()
    End Class

End Namespace