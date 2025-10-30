Imports System.Runtime.Serialization

Namespace DTOs

    <DataContract()>
    Public Class Zones

        Public Sub New()
            ListZones = New List(Of ZoneElement)
            Status = 0
        End Sub

        <DataMember()>
        Public Property ListZones As List(Of ZoneElement)

        <DataMember()>
        Public Property Status As Long

    End Class

    Public Class ZoneElement

        Public Sub New()

        End Sub

        <DataMember()>
        Public Property Id As Integer

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property Description As String
        <DataMember()>
        Public Property TelecommutingZoneType As Integer
        <DataMember()>
        Public Property MapInfo As roGoogleMapInfo
        <DataMember()>
        Public Property Area As Double

    End Class

    Public Class roGoogleMapInfo
        Public Property Center As roMapCoordinate
        Public Property Zoom As Integer
        Public Property Shape As String
        Public Property Coordinates As List(Of roMapCoordinate)
    End Class

    Public Class roMapCoordinate
        Public Property Latitud As Single
        Public Property Longitud As Single
    End Class

End Namespace