Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone

Namespace API

    Public NotInheritable Class ZoneServiceMethods

#Region "Zones"

        Public Shared Function GetZones(ByVal oPage As System.Web.UI.Page, Optional idPassport As Integer = 0) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ZoneMethods.GetZonesDataSet(oState, idPassport)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ZoneState.Result = ZoneResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-813")
            End Try

            Return oRet

        End Function

        Public Shared Function GetZoneByID(ByVal oPage As System.Web.UI.Page, ByVal intIDZone As Integer, ByVal bAudit As Boolean) As roZone

            Dim oRet As roZone = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roZone) = VTLiveApi.ZoneMethods.GetZoneByID(intIDZone, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-814")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveZone(ByVal oPage As System.Web.UI.Page, ByRef oZone As roZone, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roZone) = VTLiveApi.ZoneMethods.SaveZone(oZone, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If wsRet.Value IsNot Nothing Then
                    oRet = True
                    oZone = wsRet.Value
                Else
                    oRet = False
                End If

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-815")
            End Try
            Return oRet

        End Function

        Public Shared Function DeleteAccessZone(ByVal oPage As System.Web.UI.Page, ByVal intIDZone As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ZoneMethods.DeleteZoneByID(intIDZone, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-816")
            End Try

            Return oRet

        End Function



        Public Shared Function SetIpRestrictionStatus(ByVal oPage As System.Web.UI.Page, ByVal ipRestrictionStatus As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ZoneMethods.SetIpRestrictionStatus(oState, ipRestrictionStatus)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-816")
            End Try

            Return oRet

        End Function

#End Region

#Region "ZonePlanes"

        Public Shared Function GetZonePlanes(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ZoneMethods.GetZonesPlanesDataSet(oState)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ZoneState.Result = ZoneResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-817")
            End Try
            Return oRet

        End Function

        Public Shared Function GetZonePlaneByID(ByVal oPage As System.Web.UI.Page, ByVal intIDZonePlane As Integer, ByVal bAudit As Boolean) As roZonePlane
            Dim oRet As roZonePlane = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roZonePlane) = VTLiveApi.ZoneMethods.GetZonePlaneByID(intIDZonePlane, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-818")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveZonePlane(ByVal oPage As System.Web.UI.Page, ByRef oZonePlane As roZonePlane, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roZonePlane) = VTLiveApi.ZoneMethods.SaveZonePlane(oZonePlane, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If wsRet.Value IsNot Nothing Then
                    oRet = True
                    oZonePlane = wsRet.Value
                Else
                    oRet = False
                End If

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-819")
            End Try

            Return oRet

        End Function

        Public Shared Function DeleteZonePlaneByID(ByVal oPage As System.Web.UI.Page, ByVal intIDZonePlane As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ZoneMethods.DeleteZonePlaneByID(intIDZonePlane, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ZoneState.Result <> ZoneResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-820")
            End Try

            Return oRet

        End Function

        Public Shared Function GetZonesFromPlane(ByVal oPage As PageBase, ByVal intIDZonePlane As Integer, ByVal bAudit As Boolean) As Generic.List(Of roZone)

            Dim oRet As Generic.List(Of roZone) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roZone)) = VTLiveApi.ZoneMethods.GetZonesFromPlane(intIDZonePlane, oState, bAudit)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ZoneState.Result = ZoneResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-821")
            End Try

            Return oRet

        End Function

#End Region

#Region "ZoneWorkCenters"

        Public Shared Function GetZoneWorkCenters(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ZoneState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ZoneMethods.GetZonesWorkCentersDataSet(oState)

                oSession.States.ZoneState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ZoneState.Result = ZoneResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ZoneState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-817")
            End Try
            Return oRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ZoneState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace