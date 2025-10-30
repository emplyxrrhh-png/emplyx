Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs



    <DataContract()>
    Public Class Timegate

        Public Sub New()
            LastConnection = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)
        End Sub

        <DataMember()>
        Public Id As Integer

        <DataMember()>
        Public Name As String

        <DataMember()>
        Public Behaviour As String

        <DataMember()>
        Public Language As String

        <DataMember()>
        Public ScreenTimeout As Integer

        <DataMember()>
        Public SerialNumber As String

        <DataMember()>
        Public Firmware As String

        <DataMember()>
        Public InZone As Integer

        <DataMember()>
        Public OutZone As Integer

        <DataMember()>
        Public IDmode As Integer

        <DataMember()>
        Public LastConnection As DateTime

        <DataMember()>
        Public BackgroundMD5 As String

    End Class


    <DataContract()>
    Public Class TimegateConfiguration

        Public Sub New()

        End Sub

        <DataMember()>
        Public CustomUserFieldEnabled As Boolean

        <DataMember()>
        Public UserFieldId As Integer
        <DataMember()>
        Public BackgroundConfiguration As TimeGateBackgroundConfiguration

    End Class

    <DataContract>
    <Serializable>
    Public Class TimeGateBackgroundConfiguration
        <DataMember>
        Public Property Image As String
        <DataMember>
        Public Property Position As Integer
        <DataMember>
        Public Property Opacity As Integer
        <DataMember>
        Public Property LeftColor As String
        <DataMember>
        Public Property RightColor As String

    End Class

End Namespace