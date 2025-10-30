Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Class roShiftEngineTimeZone

#Region "Properties"

        <DataMember()>
        Public Property IDShift() As Integer
        <DataMember()>
        Public Property IDZone() As Integer
        <DataMember()>
        Public Property BeginTime() As DateTime
        <DataMember()>
        Public Property EndTime() As DateTime
        <DataMember()>
        Public Property Name() As String
        <DataMember()>
        Public Property IsBlocked() As Boolean

#End Region

    End Class

End Namespace