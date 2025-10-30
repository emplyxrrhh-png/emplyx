Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class ZonesHelper

        Private Const UNESPECIFIED_ZONE As Integer = 255

        Public Shared Function GetListZones(ByVal oState As roZoneState, ByVal oPassport As roPassportTicket) As Zones

            Dim oResult As New Zones()

            Try
                oResult.Status = ErrorCodes.OK
                Dim listZones As Generic.List(Of roZone) = roZone.GetZonesList("", oState, oPassport.ID)

                Dim bRestrictByIP As Boolean = False
                bRestrictByIP = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName, "VTPortal.ZoneRestrictedByIP"))

                Dim strIP As String = oState.ClientIP

                If listZones IsNot Nothing Then
                    For Each oZone As roZone In listZones
                        If bRestrictByIP Then
                            'Paso todas las zonas que tienen la IP en la lista de restricciones, y la zona mundial
                            If oZone.IsWorkingZone AndAlso oZone.ParentZone IsNot Nothing AndAlso (oZone.IpsRestriction.Contains(strIP) OrElse oZone.ID = UNESPECIFIED_ZONE) Then
                                Dim zone As ZoneElement = New ZoneElement()
                                zone.Id = oZone.ID
                                zone.Name = oZone.Name
                                zone.Description = oZone.Description
                                oResult.ListZones.Add(zone)
                            End If
                        ElseIf oZone.ID <> UNESPECIFIED_ZONE AndAlso oZone.IsWorkingZone AndAlso oZone.ParentZone IsNot Nothing Then
                            Dim zone As ZoneElement = New ZoneElement()
                            zone.Id = oZone.ID
                            zone.Name = oZone.Name
                            zone.Description = oZone.Description
                            oResult.ListZones.Add(zone)
                        End If

                        'Si restrinjo por IP, y oREsult.Listzones tiene más de un elemento, elimino la zona mundial
                        If bRestrictByIP AndAlso oResult.ListZones.Count > 1 Then
                            oResult.ListZones.RemoveAll(Function(x) x.Id = UNESPECIFIED_ZONE)
                        End If
                    Next
                End If
            Catch ex As Exception
                oResult.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ZonesHelper::GetListZones")
            End Try
            Return oResult
        End Function

        Public Shared Function GetListAllZones(ByVal oPassport As Integer) As Zones

            Dim oResult As New Zones
            Dim oEmpState As New roZoneState(oPassport)
            Try
                oResult.Status = ErrorCodes.OK
                Dim listZones As Generic.List(Of roZone) = roZone.GetZonesList("", oEmpState, oPassport)
                If listZones IsNot Nothing Then
                    For Each oZone As roZone In listZones
                        If oZone.ID <> UNESPECIFIED_ZONE Then
                            Dim zone As ZoneElement = New ZoneElement()
                            zone.Id = oZone.ID
                            zone.Name = oZone.Name
                            zone.Description = oZone.Description
                            zone.TelecommutingZoneType = oZone.TelecommutingZoneType
                            If oZone.GoogleMapInfo IsNot Nothing Then
                                zone.MapInfo = oZone.GoogleMapInfo
                                zone.Area = oZone.Area
                            End If
                            oResult.ListZones.Add(zone)
                        End If
                    Next
                End If
            Catch ex As Exception
                oResult.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::ZonesHelper::GetListZones")
            End Try
            Return oResult
        End Function

    End Class

End Namespace