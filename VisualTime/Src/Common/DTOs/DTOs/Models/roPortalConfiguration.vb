Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roPosition
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Name As String
    End Class

    <DataContract>
    Public Class roGeolocalizationType
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Name As String
    End Class

    <DataContract>
    <Serializable>
    Public Class roPortalConfiguration
        <DataMember>
        Public Property HeaderConfiguration As roPortalHeaderConfiguration
        <DataMember>
        Public Property GeolocalizationConfiguration As roPortalGeolocalizationConfiguration
        <DataMember>
        Public Property PunchOptions As roPortalPunchOptions
        <DataMember>
        Public Property DailyRecordPattern As Boolean
        <DataMember>
        Public Property GeneralConfiguration As roPortalGeneralConfiguration
        <DataMember>
        Public Property DailyRecordMaxDaysOnPast As Integer

        Public Sub New()
            Me.HeaderConfiguration = New roPortalHeaderConfiguration()
            Me.GeolocalizationConfiguration = New roPortalGeolocalizationConfiguration()
            Me.PunchOptions = New roPortalPunchOptions()
            Me.DailyRecordPattern = False
            Me.GeneralConfiguration = New roPortalGeneralConfiguration()
        End Sub

    End Class

    <DataContract>
    <Serializable>
    Public Class roPortalHeaderConfiguration
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

    <DataContract>
    <Serializable>
    Public Class roPortalGeolocalizationConfiguration

        <DataMember>
        Public Property Geolocalization As Integer

    End Class

    <DataContract>
    <Serializable>
    Public Class roPortalPunchOptions

        <DataMember>
        Public Property ZoneRequired As Boolean

    End Class

    <DataContract>
    <Serializable>
    Public Class roPortalConfigurationForPortal
        <DataMember>
        Public Property PortalConfiguration As String
        <DataMember()>
        Public Property Status As Long

    End Class

    <DataContract>
    <Serializable>
    Public Class roPortalGeneralConfiguration

        <DataMember>
        Public Property Impersonate As Boolean

    End Class

End Namespace