Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract()>
    Public Class roNotificationTask

        Public Sub New()
            Id = 0

        End Sub

        ''' <summary>
        ''' Resultado de la petición
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Id As Integer

        <DataMember()>
        Public Property IdNotification As Integer

        <DataMember()>
        Public Property NotificationName As String

        <DataMember()>
        Public Property Key1Numeric As Integer

        <DataMember>
        Public Property key2Numeric As Integer

        <DataMember>
        Public Property key3Datetime As Nullable(Of DateTime)

        <DataMember>
        Public Property key4Datetime As Nullable(Of DateTime)

        <DataMember>
        Public Property key5Numeric As Integer

        <DataMember>
        Public Property key6Datetime As Nullable(Of DateTime)

        <DataMember>
        Public Property Parameters As String

        <DataMember>
        Public Property Executed As Boolean

        <DataMember>
        Public Property IsReaded As Boolean

        <DataMember>
        Public Property Repetition As Integer

        <DataMember>
        Public Property NextRepetition As Nullable(Of DateTime)

        <DataMember>
        Public Property GUID As String

        <DataMember>
        Public Property Conditions As Object
        <DataMember>
        Public Property Destination As Object

        <DataMember>
        Public Property AllowMail As Boolean

        <DataMember>
        Public Property RequestType As eRequestType

        <DataMember>
        Public Property IdRequest As Integer

        <DataMember>
        Public Property FiredDate As Nullable(Of DateTime)

        <DataMember>
        Public Property IDDocument As Integer

        <DataMember>
        Public Property DocumentScope As DocumentScope

        <DataMember>
        Public Property CheckNotificationRepeat As Boolean

        <DataMember>
        Public Property IDChannel As Integer

    End Class

    <DataContract()>
    Public Class roNotificationDestinationConfig
        <DataMember>
        Public Property Email As String

        <DataMember>
        Public Property Language As String

        <DataMember>
        Public Property AddEmployeeWarning As Boolean

        <DataMember>
        Public Property IdUser As roNotificationUserConfig
    End Class

    <DataContract()>
    Public Class roNotificationUserConfig
        <DataMember>
        Public Property IdEmployee As Integer

        <DataMember>
        Public Property IdPassport As Integer

        <DataMember>
        Public Property IsRoboticsUser As Boolean
    End Class

End Namespace