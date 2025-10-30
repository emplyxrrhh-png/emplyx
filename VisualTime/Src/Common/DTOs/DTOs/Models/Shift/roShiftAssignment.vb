Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roShiftEngineAssignment

#Region "Properties"

        <DataMember()>
        Public Property IDShift() As Integer
        <DataMember()>
        Public Property IDAssignment() As Integer
        <DataMember()>
        Public Property Coverage() As Double

#End Region

    End Class

End Namespace