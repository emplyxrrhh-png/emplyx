Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Zone

Public Class ZoneProxy
    Implements IZoneSvc

    Public Function KeepAlive() As Boolean Implements IZoneSvc.KeepAlive
        Return True
    End Function


    Public Function GetZones(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZone)) Implements IZoneSvc.GetZones
        Return ZoneMethods.GetZones(oState, bAudit)
    End Function

    Public Function GetZonesDataSet(ByVal oState As roWsState, ByVal idPassport As Integer) As roGenericVtResponse(Of DataSet) Implements IZoneSvc.GetZonesDataSet
        Return ZoneMethods.GetZonesDataSet(oState, idPassport)
    End Function

    Public Function GetZoneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZone) Implements IZoneSvc.GetZoneByID
        Return ZoneMethods.GetZoneByID(intID, oState, bAudit)
    End Function

    Public Function SaveZone(ByVal oZone As roZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZone) Implements IZoneSvc.SaveZone
        Return ZoneMethods.SaveZone(oZone, oState, bAudit)
    End Function


    Public Function DeleteZone(ByVal oZone As roZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IZoneSvc.DeleteZone
        Return ZoneMethods.DeleteZone(oZone, oState, bAudit)
    End Function

    Public Function DeleteZoneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IZoneSvc.DeleteZoneByID
        Return ZoneMethods.DeleteZoneByID(intID, oState, bAudit)
    End Function

    Public Function GetZonePlanes(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZonePlane)) Implements IZoneSvc.GetZonePlanes
        Return ZoneMethods.GetZonePlanes(oState, bAudit)
    End Function

    Public Function GetZonesPlanesDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IZoneSvc.GetZonesPlanesDataSet
        Return ZoneMethods.GetZonesPlanesDataSet(oState)
    End Function

    Public Function GetZonePlaneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZonePlane) Implements IZoneSvc.GetZonePlaneByID
        Return ZoneMethods.GetZonePlaneByID(intID, oState, bAudit)
    End Function

    Public Function SaveZonePlane(ByVal oZonePlane As roZonePlane, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZonePlane) Implements IZoneSvc.SaveZonePlane
        Return ZoneMethods.SaveZonePlane(oZonePlane, oState, bAudit)
    End Function

    Public Function DeleteZonePlane(ByVal oZonePlane As roZonePlane, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IZoneSvc.DeleteZonePlane
        Return ZoneMethods.DeleteZonePlane(oZonePlane, oState, bAudit)
    End Function


    Public Function DeleteZonePlaneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IZoneSvc.DeleteZonePlaneByID
        Return ZoneMethods.DeleteZonePlaneByID(intID, oState, bAudit)
    End Function

    Public Function GetZonesFromPlane(ByVal IDZonePlane As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZone)) Implements IZoneSvc.GetZonesFromPlane
        Return ZoneMethods.GetZonesFromPlane(IDZonePlane, oState, bAudit)
    End Function


End Class
