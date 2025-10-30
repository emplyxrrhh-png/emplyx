Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <Serializable>
    <DataContract>
    Public Class SupremaConfigurationParameters
        <DataMember>
        Public Property URL As String = String.Empty
        <DataMember>
        Public Property Username As String = String.Empty
        <DataMember>
        Public Property Password As String = String.Empty
        <DataMember>
        Public Property HasPassword As Boolean
        <DataMember>
        Public Property EmployeeUserfieldId As Integer
        <DataMember>
        Public Property StartDate As String = String.Empty
        <DataMember>
        Public Property CheckPeriod As String = String.Empty
        <DataMember>
        Public Property DailyInitialTime As DateTime
        <DataMember>
        Public Property LastRun As DateTime
        <DataMember>
        Public Property IsActive As Boolean
        <DataMember>
        Public Property Timestamp As DateTime
    End Class


End Namespace