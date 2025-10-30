Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Security.Base
Imports Robotics.VTBase.Extensions

Public Class ZoneMethods

    Public Shared Function GetZones(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZone))

        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roZone))
        oResult.Value = roZone.GetZonesList("", bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonesDataSet(ByVal oState As roWsState, ByVal idPassport As Integer) As roGenericVtResponse(Of DataSet)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roZone.GetZonesDataTable("", bState, idPassport)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZoneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZone)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roZone)
        oResult.Value = New roZone(intID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveZone(ByVal oZone As roZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZone)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roZone)
        oZone.State = bState
        If oZone.Save(bAudit) Then
            oResult.Value = oZone
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZone.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteZone(ByVal oZone As roZone, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oZone.State = bState
        oResult.Value = oZone.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZone.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteZoneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oZone As New roZone(intID, bState, False)
        oZone.State = bState
        oResult.Value = oZone.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZone.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonePlanes(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZonePlane))
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roZonePlane))
        oResult.Value = roZonePlane.GetZonePlanesList("", bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonesPlanesDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roZonePlane.GetZonePlanesDataTable("", bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonePlaneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZonePlane)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roZonePlane)
        oResult.Value = New roZonePlane(intID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveZonePlane(ByVal oZonePlane As roZonePlane, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roZonePlane)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roZonePlane)

        oZonePlane.State = bState
        If oZonePlane.Save(bAudit) Then
            oResult.Value = oZonePlane
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZonePlane.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteZonePlane(ByVal oZonePlane As roZonePlane, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oZonePlane.State = bState
        oResult.Value = oZonePlane.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZonePlane.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteZonePlaneByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oZonePlane As New roZonePlane(intID, bState, False)

        oZonePlane.State = bState
        oResult.Value = oZonePlane.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oZonePlane.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonesFromPlane(ByVal IDZonePlane As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roZone))

        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roZone))
        oResult.Value = roZonePlane.GetZonesFromPlane(IDZonePlane, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetZonesWorkCentersDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roZoneState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roZoneWorkCenter.GetZoneWorkCentersDataTable("", bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function


    Public Shared Function SetIpRestrictionStatus(ByVal oState As roWsState, ByVal ipRestrictionStatus As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAdvancedParameter.SetIpRestrictionStatus(ipRestrictionStatus, bState)

        Dim newGState As New roWsState
        Dim bZoneState As New roZoneState(-1)
        roBusinessState.CopyTo(bState, bZoneState)

        If bState.Result <> AdvancedParameterResultEnum.NoError Then
            bZoneState.Result = ZoneResultEnum.ZoneIpRestrictionChange
        End If

        roWsStateManager.CopyTo(bZoneState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class